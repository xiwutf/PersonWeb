<template>
  <div class="interaction-panel" style="position: fixed !important; bottom: 12rem !important; right: 2rem !important; z-index: 10000 !important; pointer-events: auto !important; display: flex !important; visibility: visible !important; opacity: 1 !important; isolation: isolate; transform: translateZ(0); overflow: visible !important;">
    <!-- 发送弹幕按钮 -->
    <button
      @click="showMessageModal = true"
      class="visitor-button-circle visitor-button-circle-blue panel-button"
      title="发送弹幕"
      style="z-index: 10000 !important; position: relative !important; display: flex !important; visibility: visible !important; opacity: 1 !important; pointer-events: auto !important;"
    >
      <i class="fas fa-comment-dots"></i>
    </button>

    <!-- 发送弹幕弹窗 -->
    <Teleport to="body">
      <div v-if="showMessageModal" class="visitor-modal-overlay" @click="showMessageModal = false">
        <div class="visitor-modal" @click.stop>
        <div class="visitor-modal-header">
          <h3>发送弹幕</h3>
          <button @click="showMessageModal = false" class="visitor-modal-close">
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
                @click="messageType = type.value"
                :class="['visitor-type-button', { 'visitor-type-button-active': messageType === type.value }]"
              >
                <i :class="type.icon"></i>
                {{ type.label }}
              </button>
            </div>
          </div>

          <div class="visitor-form-group">
            <label class="visitor-form-label">表情（可选）</label>
            <div class="visitor-emoji-picker">
              <button
                v-for="emoji in quickEmojis"
                :key="emoji"
                @click="selectedEmoji = emoji"
                :class="['visitor-emoji-button', { 'visitor-emoji-button-selected': selectedEmoji === emoji }]"
              >
                {{ emoji }}
              </button>
            </div>
          </div>

          <div class="visitor-form-group">
            <label class="visitor-form-label">内容 *</label>
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
            <label class="visitor-form-label">位置（可选）</label>
            <input
              v-model="messageLocation"
              type="text"
              class="visitor-form-input"
              placeholder="如：杭州"
              maxlength="20"
            />
          </div>

          <button @click="sendMessage" class="visitor-button-primary" :disabled="!messageContent.trim() || submitting">
            {{ submitting ? '发送中...' : '发送弹幕' }}
          </button>

          <p class="visitor-form-hint">
            <i class="fas fa-info-circle"></i>
            弹幕需要审核后才能显示
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

const quickEmojis = ['😊', '❤️', '👍', '🎉', '⭐', '💫', '🌟', '🔥', '💯', '🎊', '✨', '🌈']

// 发送弹幕
const sendMessage = async () => {
  if (!messageContent.value.trim()) {
    return
  }

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

    // 重置表单
    messageContent.value = ''
    selectedEmoji.value = ''
    messageLocation.value = ''
    messageType.value = 'message'
    showMessageModal.value = false

    // 触发成功提示
    if (process.client) {
      window.dispatchEvent(new CustomEvent('show-toast', {
        detail: { message: '弹幕已发送，等待审核', type: 'success' }
      }))
    }
  } catch (e) {
    console.error('发送弹幕失败', e)
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
  position: fixed !important;
  bottom: 12rem !important;
  right: 2rem !important;
  z-index: 10000 !important; /* 确保始终在最上层 */
  pointer-events: auto !important; /* 确保可以点击 */
  display: flex !important;
  flex-direction: column;
  align-items: flex-end;
  visibility: visible !important;
  opacity: 1 !important;
  /* 确保按钮始终可见，不被其他元素遮挡 */
  isolation: isolate;
  transform: translateZ(0); /* 启用硬件加速 */
}

.panel-button {
  margin-bottom: 0.75rem;
  pointer-events: auto !important; /* 确保按钮可以点击 */
  position: relative !important;
  z-index: 10000 !important;
  display: flex !important;
  opacity: 1 !important;
  visibility: visible !important;
  /* 确保按钮有足够的对比度 */
  background: rgba(59, 130, 246, 0.95) !important;
  border: 2px solid rgba(255, 255, 255, 0.6) !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5) !important;
}
</style>

