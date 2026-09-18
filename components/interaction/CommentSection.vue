<template>
  <section class="ci-comments" aria-labelledby="ci-comments-title">
    <header class="ci-comments-head">
      <h2 id="ci-comments-title">{{ title }}</h2>
      <p v-if="approved.length" class="ci-comments-count">{{ approved.length }} 条回应</p>
    </header>

    <form class="ci-form" @submit.prevent="submit">
      <label class="ci-field">
        <span>昵称</span>
        <input
          v-model="form.nickname"
          type="text"
          maxlength="30"
          autocomplete="nickname"
          required
          placeholder="怎么称呼你"
        >
      </label>

      <label class="ci-field">
        <span>邮箱</span>
        <input
          v-model="form.email"
          type="email"
          maxlength="120"
          autocomplete="email"
          required
          placeholder="name@example.com"
        >
        <em>仅用于审核，不公开</em>
      </label>

      <label class="ci-field">
        <span>回应</span>
        <textarea
          v-model="form.content"
          rows="4"
          maxlength="1000"
          required
          placeholder="说说你的想法……"
        />
      </label>

      <p v-if="formError" class="ci-form-error">{{ formError }}</p>
      <p v-if="formSuccess" class="ci-form-success">{{ formSuccess }}</p>

      <button type="submit" class="ci-submit" :disabled="submitting">
        {{ submitting ? '提交中…' : '提交回应' }}
      </button>
    </form>

    <div v-if="loading" class="ci-empty">加载中…</div>
    <div v-else-if="!approved.length" class="ci-empty">还没有公开回应，来写第一条吧。</div>
    <ul v-else class="ci-list">
      <li v-for="item in approved" :key="item.id" class="ci-item">
        <div class="ci-item-meta">
          <strong>{{ item.nickname }}</strong>
          <time>{{ formatTime(item.createdAt) }}</time>
        </div>
        <p class="ci-item-body">{{ item.content }}</p>
      </li>
    </ul>
  </section>
</template>

<script setup lang="ts">
type PublicComment = {
  id: number
  nickname: string
  content: string
  createdAt: string
}

const props = withDefaults(defineProps<{
  targetType: string
  targetId: string
  title?: string
}>(), {
  title: '回应这篇内容',
})

const api = useApi()
const { getVisitorId } = useVisitorId()

const approved = ref<PublicComment[]>([])
const loading = ref(false)
const submitting = ref(false)
const formError = ref('')
const formSuccess = ref('')

const form = reactive({
  nickname: '',
  email: '',
  content: '',
})

const mapComment = (raw: Record<string, unknown>): PublicComment => ({
  id: Number(raw.id ?? raw.Id ?? 0),
  nickname: String(raw.nickname ?? raw.Nickname ?? ''),
  content: String(raw.content ?? raw.Content ?? ''),
  createdAt: String(raw.createdAt ?? raw.CreatedAt ?? ''),
})

const load = async () => {
  if (!props.targetType || !props.targetId) return
  loading.value = true
  try {
    const data = await api.get<unknown[]>(
      '/comments',
      {
        query: {
          targetType: props.targetType,
          targetId: props.targetId,
        },
        silent: true,
      },
    )
    const list = Array.isArray(data) ? data : []
    approved.value = list.map(item => mapComment(item as Record<string, unknown>))
  } catch {
    approved.value = []
  } finally {
    loading.value = false
  }
}

const validate = (): string | null => {
  const nickname = form.nickname.trim()
  const email = form.email.trim()
  const content = form.content.trim()

  if (nickname.length < 2 || nickname.length > 30) return '昵称长度需为 2～30 字'
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return '请填写有效邮箱'
  if (content.length < 2 || content.length > 1000) return '回应内容长度需为 2～1000 字'
  return null
}

const submit = async () => {
  formError.value = ''
  formSuccess.value = ''
  const err = validate()
  if (err) {
    formError.value = err
    return
  }

  submitting.value = true
  try {
    await api.post('/comments', {
      targetType: props.targetType,
      targetId: props.targetId,
      nickname: form.nickname.trim(),
      email: form.email.trim(),
      content: form.content.trim(),
      visitorId: getVisitorId(),
      // TODO(V1): Cloudflare Turnstile token 预留
      captchaToken: null,
    })
    form.content = ''
    formSuccess.value = '回应已提交，审核通过后会展示。'
  } catch (e: unknown) {
    formError.value = e instanceof Error ? e.message : '提交失败，请稍后再试'
  } finally {
    submitting.value = false
  }
}

const formatTime = (value: string) => {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

watch(
  () => [props.targetType, props.targetId],
  () => { void load() },
  { immediate: true },
)
</script>
