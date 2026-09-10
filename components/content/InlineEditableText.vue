<template>
  <component
    :is="tag"
    v-if="!isAdmin || !editing"
    :id="id"
    :class="rootClass"
    :role="isAdmin ? 'button' : undefined"
    :tabindex="isAdmin ? 0 : undefined"
    :aria-label="isAdmin ? `编辑：${fieldPath}` : undefined"
    @click="isAdmin ? startEdit() : undefined"
    @keydown.enter.prevent="isAdmin ? startEdit() : undefined"
  >{{ modelValue }}</component>

  <textarea
    v-else-if="multiline"
    ref="inputRef"
    class="inline-edit__textarea"
    :value="draft"
    :disabled="saving"
    rows="3"
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
  displayClass?: string
  id?: string
  save: (path: string, value: string) => Promise<unknown>
}>(), {
  as: 'span',
  multiline: false,
  displayClass: '',
  id: undefined,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  saved: [payload: unknown]
}>()

const { isAdmin } = useAdminSession()

const editing = ref(false)
const saving = ref(false)
const draft = ref(props.modelValue)
const inputRef = ref<HTMLInputElement | HTMLTextAreaElement | null>(null)

const tag = computed(() => props.as)
const isBlock = computed(() =>
  props.multiline || ['p', 'h1', 'h2', 'h3'].includes(props.as),
)

const rootClass = computed(() => {
  const classes = ['inline-edit', props.displayClass]
  if (isBlock.value) classes.push('inline-edit--block')
  if (isAdmin.value) classes.push('inline-edit--editable')
  return classes.filter(Boolean).join(' ')
})

watch(isAdmin, (admin) => {
  if (!admin && editing.value) {
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

function startEdit() {
  if (!isAdmin.value || saving.value) return
  draft.value = props.modelValue
  editing.value = true
  nextTick(() => {
    inputRef.value?.focus()
    inputRef.value?.select()
  })
}

function onInput(event: Event) {
  const target = event.target as HTMLInputElement | HTMLTextAreaElement
  draft.value = target.value
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
  useNotification().error('无法写入内容文件')
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
