<template>
  <div class="interaction-panel">
    <!-- 发送弹幕按钮 -->
    <button
      @click="showMessageModal = true"
      class="panel-button panel-button-message"
      title="发送弹幕"
    >
      <i class="fas fa-comment-dots"></i>
    </button>

    <!-- 发送弹幕弹窗 -->
    <div v-if="showMessageModal" class="modal-overlay" @click="showMessageModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>发送弹幕</h3>
          <button @click="showMessageModal = false" class="modal-close">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="modal-body">
          <div class="form-group">
            <label>消息类型</label>
            <div class="type-buttons">
              <button
                v-for="type in messageTypes"
                :key="type.value"
                @click="messageType = type.value"
                :class="['type-button', { 'type-button-active': messageType === type.value }]"
              >
                <i :class="type.icon"></i>
                {{ type.label }}
              </button>
            </div>
          </div>

          <div class="form-group">
            <label>表情（可选）</label>
            <div class="emoji-picker">
              <button
                v-for="emoji in quickEmojis"
                :key="emoji"
                @click="selectedEmoji = emoji"
                :class="['emoji-button', { 'emoji-button-selected': selectedEmoji === emoji }]"
              >
                {{ emoji }}
              </button>
            </div>
          </div>

          <div class="form-group">
            <label>内容 *</label>
            <textarea
              v-model="messageContent"
              class="form-textarea"
              placeholder="输入你想说的话..."
              rows="4"
              maxlength="100"
            ></textarea>
            <div class="char-count">{{ messageContent.length }}/100</div>
          </div>

          <div class="form-group">
            <label>位置（可选）</label>
            <input
              v-model="messageLocation"
              type="text"
              class="form-input"
              placeholder="如：杭州"
              maxlength="20"
            />
          </div>

          <button @click="sendMessage" class="submit-button" :disabled="!messageContent.trim() || submitting">
            {{ submitting ? '发送中...' : '发送弹幕' }}
          </button>

          <p class="form-hint">
            <i class="fas fa-info-circle"></i>
            弹幕需要审核后才能显示
          </p>
        </div>
      </div>
    </div>
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
  position: fixed;
  bottom: 8rem;
  right: 2rem;
  z-index: 150;
  pointer-events: none;
}

.panel-button {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 50%;
  backdrop-filter: blur(10px);
  border: 2px solid rgba(255, 255, 255, 0.3);
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  pointer-events: auto;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.75rem;
}

.panel-button-message {
  background: rgba(59, 130, 246, 0.9);
}

.panel-button:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.4);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  pointer-events: auto;
}

.modal-content {
  background: rgba(30, 41, 59, 0.95);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  pointer-events: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.modal-header h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: white;
}

.modal-close {
  width: 2rem;
  height: 2rem;
  border-radius: 0.25rem;
  background: rgba(255, 255, 255, 0.1);
  border: none;
  color: rgba(255, 255, 255, 0.7);
  font-size: 1.25rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close:hover {
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.modal-body {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 0.75rem;
  font-weight: 500;
}

.type-buttons {
  display: flex;
  gap: 0.5rem;
}

.type-button {
  flex: 1;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.type-button:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.3);
}

.type-button-active {
  background: rgba(59, 130, 246, 0.3);
  border-color: rgba(59, 130, 246, 0.6);
  color: white;
}

.emoji-picker {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.emoji-button {
  width: 2.5rem;
  height: 2.5rem;
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  background: rgba(255, 255, 255, 0.05);
  font-size: 1.25rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.emoji-button:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.3);
  transform: scale(1.1);
}

.emoji-button-selected {
  background: rgba(59, 130, 246, 0.3);
  border-color: rgba(59, 130, 246, 0.6);
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.2);
}

.form-textarea {
  width: 100%;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: white;
  font-size: 0.875rem;
  font-family: inherit;
  resize: vertical;
  transition: all 0.2s ease;
}

.form-textarea:focus {
  outline: none;
  border-color: rgba(59, 130, 246, 0.5);
  background: rgba(255, 255, 255, 0.08);
}

.char-count {
  text-align: right;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  margin-top: 0.25rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: white;
  font-size: 0.875rem;
  transition: all 0.2s ease;
}

.form-input:focus {
  outline: none;
  border-color: rgba(59, 130, 246, 0.5);
  background: rgba(255, 255, 255, 0.08);
}

.submit-button {
  width: 100%;
  padding: 0.75rem;
  background: rgba(59, 130, 246, 0.9);
  border: none;
  border-radius: 0.5rem;
  color: white;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.submit-button:hover:not(:disabled) {
  background: rgba(59, 130, 246, 1);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
}

.submit-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.form-hint {
  margin-top: 1rem;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}
</style>

