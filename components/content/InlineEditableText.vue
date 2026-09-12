<template>
  <component
    :is="tag"
    v-if="!canEditContent || !editing"
    :id="id"
    :class="rootClass"
    :role="canEditContent ? 'button' : undefined"
    :tabindex="canEditContent ? 0 : undefined"
    :aria-label="canEditContent ? `编辑：${fieldPath}` : undefined"
    @click="canEditContent ? startEdit() : undefined"
    @keydown.enter.prevent="canEditContent ? startEdit() : undefined"
  >{{ modelValue }}</component>

  <textarea
    v-else-if="multiline"
    ref="inputRef"
    class="inline-edit__textarea"
    :class="displayClass"
    :value="draft"
    :disabled="saving"
    :rows="resolvedRows"
    @input="onInput"
    @keydown.esc.prevent="cancel"
    @blur="commit"
  />

  <input
    v-else
    ref="inputRef"
    class="inline-edit__input"
    type="text"
    :value="draft"
    :disabled="saving"
    @input="onInput"
    @keydown.enter.prevent="commit"
    @keydown.esc.prevent="cancel"
    @blur="commit"
  >
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  modelValue: string
  fieldPath: string
  as?: 'p' | 'h1' | 'h2' | 'h3' | 'span' | 'small' | 'strong'
  multiline?: boolean
  rows?: number
  displayClass?: string
  id?: string
  save: (path: string, value: string) => Promise<unknown>
}>(), {
  as: 'span',
  multiline: false,
  rows: undefined,
  displayClass: '',
  id: undefined,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  saved: [payload: unknown]
}>()

const { canEditContent } = useAdminSession()

const editing = ref(false)
const saving = ref(false)
const draft = ref(props.modelValue)
const inputRef = ref<HTMLInputElement | HTMLTextAreaElement | null>(null)

const tag = computed(() => props.as)
const isBlock = computed(() =>
  props.multiline || ['p', 'h1', 'h2', 'h3'].includes(props.as),
)

const resolvedRows = computed(() => {
  if (typeof props.rows === 'number' && props.rows > 0) return props.rows
  if (!props.multiline) return 1
  const lines = String(props.modelValue || '').split('\n').length
  return Math.min(28, Math.max(6, lines + 2))
})

const rootClass = computed(() => {
  const classes = ['inline-edit', props.displayClass]
  if (isBlock.value) classes.push('inline-edit--block')
  if (canEditContent.value) classes.push('inline-edit--editable')
  return classes.filter(Boolean).join(' ')
})

watch(canEditContent, (editable) => {
  if (!editable && editing.value) {
    cancel()
  }
})

onBeforeUnmount(() => {
  if (editing.value) {
    cancel()
  }
})

watch(
  () => props.modelValue,
  (value) => {
    if (!editing.value) {
      draft.value = value
    }
  },
)

function fitTextarea() {
  const el = inputRef.value
  if (!el || !(el instanceof HTMLTextAreaElement)) return
  el.style.height = 'auto'
  el.style.height = `${Math.max(el.scrollHeight, 160)}px`
}

function startEdit() {
  if (!canEditContent.value || saving.value) return
  draft.value = props.modelValue
  editing.value = true
  nextTick(() => {
    inputRef.value?.focus()
    if (props.multiline) {
      fitTextarea()
    }
    else {
      inputRef.value?.select()
    }
  })
}

function onInput(event: Event) {
  const target = event.target as HTMLInputElement | HTMLTextAreaElement
  draft.value = target.value
  if (target instanceof HTMLTextAreaElement) {
    fitTextarea()
  }
}

function cancel() {
  draft.value = props.modelValue
  editing.value = false
}

async function notifySaveError(error?: unknown) {
  const { useNotification } = await import('~/composables/useToast')
  const status = typeof error === 'object' && error !== null
    ? Number((error as { statusCode?: number, status?: number }).statusCode
      ?? (error as { statusCode?: number, status?: number }).status
      ?? 0)
    : 0
  if (status === 401) {
    useNotification().error('登录已失效，请重新双击印章登录')
    return
  }
  if (status === 404) {
    useNotification().error('当前环境无法写入内容文件，请用本地 npm run dev 打开后再改')
    return
  }
  useNotification().error(error instanceof Error ? error.message : '无法写入内容文件')
}

async function commit() {
  if (!editing.value || saving.value) return

  const next = draft.value
  if (next === props.modelValue) {
    editing.value = false
    return
  }

  saving.value = true
  try {
    const payload = await props.save(props.fieldPath, next)
    emit('update:modelValue', next)
    emit('saved', payload)
    editing.value = false
  } catch (error) {
    await notifySaveError(error)
    draft.value = props.modelValue
    editing.value = false
  } finally {
    saving.value = false
  }
}
</script>
