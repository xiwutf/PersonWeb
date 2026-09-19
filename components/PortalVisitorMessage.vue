<template>
  <form class="portal-note" @submit.prevent="submit">
    <p class="portal-note-prompt">路过？留一句。</p>
    <div class="portal-note-fields">
      <input
        id="portal-note-name"
        v-model="visitorName"
        type="text"
        class="portal-note-name"
        placeholder="称呼"
        maxlength="20"
        autocomplete="nickname"
        required
        aria-label="称呼"
      />
      <input
        id="portal-note-text"
        v-model="content"
        type="text"
        class="portal-note-text"
        placeholder=""
        maxlength="100"
        required
        aria-label="一句话"
      />
      <button
        type="submit"
        class="portal-note-submit"
        :disabled="!visitorName.trim() || !content.trim() || submitting"
      >
        {{ submitting ? '…' : '留' }}
      </button>
    </div>
    <p v-if="feedback" class="portal-note-feedback" :data-type="feedbackType" role="status">
      {{ feedback }}
    </p>
  </form>
</template>

<script setup lang="ts">
const api = useApi()

const VISITOR_NAME_KEY = 'visitor_display_name'

const visitorName = ref('')
const content = ref('')
const submitting = ref(false)
const feedback = ref('')
const feedbackType = ref<'success' | 'error'>('success')

let feedbackTimer: ReturnType<typeof setTimeout> | null = null

const showFeedback = (message: string, type: 'success' | 'error') => {
  feedback.value = message
  feedbackType.value = type
  if (feedbackTimer) clearTimeout(feedbackTimer)
  feedbackTimer = setTimeout(() => {
    feedback.value = ''
  }, 4200)
}

const submit = async () => {
  const name = visitorName.value.trim()
  const text = content.value.trim()
  if (!name || !text || submitting.value) return

  submitting.value = true
  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'
    await api.post('/VisitorInteraction/message', {
      visitorId,
      visitorName: name,
      messageType: 'message',
      content: text,
      emoji: null,
      location: null,
    })

    localStorage.setItem(VISITOR_NAME_KEY, name)

    content.value = ''
    showFeedback('记下了，审核后会出现。', 'success')
  } catch (error) {
    console.error('Portal visitor message failed', error)
    showFeedback('没发出去，稍后再试。', 'error')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  const saved = localStorage.getItem(VISITOR_NAME_KEY)
  if (saved) visitorName.value = saved
})

onUnmounted(() => {
  if (feedbackTimer) clearTimeout(feedbackTimer)
})
</script>
