<template>
  <div class="interaction-panel">
    <button
      type="button"
      class="visitor-button-circle visitor-button-circle-blue panel-button"
      title="发送留言"
      aria-label="发送留言"
      @click="showMessageModal = true"
    >
      <i class="fas fa-comment-dots"></i>
    </button>

    <Teleport to="body">
      <div v-if="showMessageModal" class="visitor-modal-overlay" @click="showMessageModal = false">
        <div class="visitor-modal" @click.stop>
          <div class="visitor-modal-header">
            <h3>发送留言</h3>
            <button class="visitor-modal-close" @click="showMessageModal = false">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <div class="visitor-modal-body">
            <div class="visitor-form-group">
              <label class="visitor-form-label">消息类型</label>
              <div class="visitor-type-buttons">
                <button
                  v-for="type in messageTypes"
                  :key="type.value"
                  type="button"
                  :class="['visitor-type-button', { 'visitor-type-button-active': messageType === type.value }]"
                  @click="messageType = type.value"
                >
                  <i :class="type.icon"></i>
                  {{ type.label }}
                </button>
              </div>
            </div>

            <div class="visitor-form-group">
              <label class="visitor-form-label">表情</label>
              <div class="visitor-emoji-picker">
                <button
                  v-for="emoji in quickEmojis"
                  :key="emoji"
                  type="button"
                  :class="['visitor-emoji-button', { 'visitor-emoji-button-selected': selectedEmoji === emoji }]"
                  @click="selectedEmoji = emoji"
                >
                  {{ emoji }}
                </button>
              </div>
            </div>

            <div class="visitor-form-group">
              <label class="visitor-form-label">内容</label>
              <textarea
                v-model="messageContent"
                class="visitor-form-textarea"
                placeholder="输入你想说的话..."
                rows="4"
                maxlength="100"
              ></textarea>
              <div class="visitor-form-char-count">{{ messageContent.length }}/100</div>
            </div>

            <div class="visitor-form-group">
              <label class="visitor-form-label">位置</label>
              <input
                v-model="messageLocation"
                type="text"
                class="visitor-form-input"
                placeholder="例如：杭州"
                maxlength="20"
              />
            </div>

            <button class="visitor-button-primary" :disabled="!messageContent.trim() || submitting" @click="sendMessage">
              {{ submitting ? '发送中...' : '发送留言' }}
            </button>

            <p class="visitor-form-hint">
              <i class="fas fa-info-circle"></i>
              内容提交后会进入审核流程
            </p>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
const api = useApi()

const showMessageModal = ref(false)
const messageType = ref('message')
const selectedEmoji = ref('')
const messageContent = ref('')
const messageLocation = ref('')
const submitting = ref(false)

const messageTypes = [
  { value: 'message', label: '留言', icon: 'fas fa-comment' },
  { value: 'mood', label: '心情', icon: 'fas fa-smile' },
  { value: 'blessing', label: '祝福', icon: 'fas fa-heart' }
]

const quickEmojis = ['😊', '❤️', '👏', '🎉', '✨', '🔥', '👍', '🌟', '💡', '🚀', '✔️', '🫶']

const sendMessage = async () => {
  if (!messageContent.value.trim()) return

  submitting.value = true

  try {
    const visitorId = localStorage.getItem('visitor_id') || 'anonymous'

    await api.post('/VisitorInteraction/message', {
      visitorId,
      messageType: messageType.value,
      content: messageContent.value.trim(),
      emoji: selectedEmoji.value || null,
      location: messageLocation.value.trim() || null
    })

    messageContent.value = ''
    selectedEmoji.value = ''
    messageLocation.value = ''
    messageType.value = 'message'
    showMessageModal.value = false

    if (process.client) {
      window.dispatchEvent(new CustomEvent('show-toast', {
        detail: { message: '留言已提交，等待审核后展示', type: 'success' }
      }))
    }
  } catch (error) {
    console.error('Visitor message send failed', error)

    if (process.client) {
      window.dispatchEvent(new CustomEvent('show-toast', {
        detail: { message: '发送失败，请稍后重试', type: 'error' }
      }))
    }
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.interaction-panel {
  position: fixed;
  bottom: calc(
    var(--floating-dock-bottom, 18px)
    + (var(--floating-dock-button-size, 52px) + var(--floating-dock-gap, 14px)) * 2
  );
  right: var(--floating-dock-right, 2rem);
  z-index: 10000;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  pointer-events: auto;
  isolation: isolate;
  transform: translateZ(0);
}

.panel-button {
  width: var(--floating-dock-button-size, 52px);
  height: var(--floating-dock-button-size, 52px);
  margin: 0;
  position: relative;
  z-index: 10000;
  display: flex;
  background:
    radial-gradient(circle at 30% 30%, rgba(255, 255, 255, 0.2) 0%, rgba(255, 255, 255, 0) 42%),
    linear-gradient(145deg, #5b7cff 0%, #3b82f6 58%, #155eef 100%) !important;
  border: 1px solid rgba(255, 255, 255, 0.22) !important;
  box-shadow:
    0 16px 30px rgba(37, 99, 235, 0.26),
    0 0 0 1px rgba(255, 255, 255, 0.12) inset !important;
  backdrop-filter: blur(16px);
}

@media (max-width: 768px) {
  .interaction-panel {
    bottom: calc(
      var(--floating-dock-bottom, 12px)
      + (var(--floating-dock-button-size, 44px) + var(--floating-dock-gap, 10px)) * 2
    );
    right: var(--floating-dock-right, 12px);
  }
}
</style>
