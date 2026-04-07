<template>
  <ClientOnly>
    <button
      v-if="!isOpen"
      :class="['ai-assistant-button', { 'ai-assistant-button-has-prompt': hasUnreadPrompt }]"
      @click="toggleAssistant"
      aria-label="打开 AI 助手"
    >
      <svg class="ai-assistant-button__icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"
        />
      </svg>
      <span class="ai-assistant-status-dot"></span>
    </button>

    <Transition name="slide-up">
      <div v-if="isOpen" class="ai-assistant-dialog">
        <div class="ai-assistant-header">
          <div class="ai-assistant-header-content">
            <div class="ai-assistant-avatar">
              <svg class="ai-assistant-avatar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"
                />
              </svg>
            </div>
            <div>
              <h3 class="ai-assistant-title">AI 小智</h3>
              <p class="ai-assistant-status">{{ statusText }}</p>
            </div>
          </div>
          <button @click="toggleAssistant" class="ai-assistant-close-btn" aria-label="关闭">
            <svg class="ai-assistant-close-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div ref="messagesRef" class="ai-assistant-messages">
          <div v-if="messages.length === 0" class="ai-assistant-welcome">
            <div class="ai-assistant-message ai-assistant-message-assistant">
              <p class="ai-assistant-message-text">你好，我是 AI 小智。有问题可以随时问我。</p>
            </div>
          </div>

          <div
            v-for="(message, index) in messages"
            :key="index"
            :class="[
              'ai-assistant-message-wrapper',
              message.role === 'user' ? 'ai-assistant-message-user-wrapper' : 'ai-assistant-message-assistant-wrapper'
            ]"
          >
            <div
              :class="[
                'ai-assistant-message',
                message.role === 'user' ? 'ai-assistant-message-user' : 'ai-assistant-message-assistant'
              ]"
            >
              <p class="ai-assistant-message-text">{{ message.content }}</p>
              <p v-if="message.role === 'assistant' && message.loading" class="ai-assistant-message-loading">正在思考...</p>
            </div>
          </div>

          <div v-if="isLoading" class="ai-assistant-message-wrapper ai-assistant-message-assistant-wrapper">
            <div class="ai-assistant-message ai-assistant-message-assistant">
              <div class="ai-assistant-loading">
                <div class="ai-assistant-loading-dot" style="animation-delay: 0s"></div>
                <div class="ai-assistant-loading-dot" style="animation-delay: 0.2s"></div>
                <div class="ai-assistant-loading-dot" style="animation-delay: 0.4s"></div>
              </div>
            </div>
          </div>
        </div>

        <div class="ai-assistant-input-area">
          <div v-if="!isCompactMode" class="ai-assistant-quick-actions">
            <button
              v-for="quickAction in quickActions"
              :key="quickAction.text"
              @click="sendQuickMessage(quickAction.text)"
              class="ai-assistant-quick-action-btn"
              :disabled="isLoading"
            >
              {{ quickAction.text }}
            </button>
          </div>

          <div class="ai-assistant-input-wrapper">
            <input
              v-model="inputText"
              @keyup.enter="sendMessage"
              type="text"
              placeholder="输入你的问题..."
              class="ai-assistant-input"
              :disabled="isLoading"
            />
            <button
              @click="sendMessage"
              :disabled="isLoading || !inputText.trim()"
              class="ai-assistant-send-btn"
            >
              <svg class="ai-assistant-send-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </ClientOnly>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'

interface Message {
  role: 'user' | 'assistant'
  content: string
  loading?: boolean
}

const isOpen = ref(false)
const inputText = ref('')
const messages = ref<Message[]>([])
const isLoading = ref(false)
const messagesRef = ref<HTMLDivElement | null>(null)
const statusText = ref('在线')
const hasUserClosed = ref(false)
const hasUnreadPrompt = ref(false)
const pendingPromptMessage = ref('')
const isCompactMode = ref(false)

const quickActions = [
  { text: '推荐文章' },
  { text: '搜索文章' }
]

let openAssistantHandler: EventListener | null = null

const scrollToBottom = () => {
  if (messagesRef.value) {
    messagesRef.value.scrollTop = messagesRef.value.scrollHeight
  }
}

const toggleAssistant = () => {
  isOpen.value = !isOpen.value

  if (isOpen.value) {
    if (pendingPromptMessage.value) {
      messages.value.push({
        role: 'assistant',
        content: pendingPromptMessage.value
      })
      pendingPromptMessage.value = ''
    }

    hasUnreadPrompt.value = false
    hasUserClosed.value = false

    if (process.client) {
      localStorage.removeItem('ai-assistant-auto-open-disabled')
    }

    nextTick(() => {
      scrollToBottom()
    })
    return
  }

  hasUserClosed.value = true
}

const sendQuickMessage = (text: string) => {
  inputText.value = text
  sendMessage()
}

const sendMessage = async () => {
  if (!inputText.value.trim() || isLoading.value) return

  const userMessage = inputText.value.trim()
  inputText.value = ''

  messages.value.push({
    role: 'user',
    content: userMessage
  })

  scrollToBottom()
  isLoading.value = true
  statusText.value = '正在思考...'

  try {
    const api = useApi()
    const response = await api.post<any>('/AI/chat', {
      message: userMessage,
      history: messages.value.slice(-5).map(message => ({
        role: message.role,
        content: message.content
      }))
    })

    messages.value.push({
      role: 'assistant',
      content: response.message || response.content || '抱歉，我暂时无法回答这个问题。'
    })
    statusText.value = '在线'
  } catch (error) {
    console.error('AI Chat error:', error)
    messages.value.push({
      role: 'assistant',
      content: '抱歉，服务暂时不可用，请稍后再试。'
    })
    statusText.value = '离线'
  } finally {
    isLoading.value = false
    nextTick(() => {
      scrollToBottom()
    })
  }
}

onMounted(() => {
  if (!process.client) return

  const coarsePointer = window.matchMedia('(pointer: coarse)').matches
  isCompactMode.value = coarsePointer || window.innerWidth < 768

  if (localStorage.getItem('ai-assistant-auto-open-disabled') === 'true') {
    return
  }

  openAssistantHandler = ((event: Event) => {
    if (hasUserClosed.value) return

    const customEvent = event as CustomEvent<{ message?: string; forceOpen?: boolean }>
    const promptMessage = customEvent.detail?.message

    if (promptMessage) {
      pendingPromptMessage.value = promptMessage
      hasUnreadPrompt.value = true
    }

    if (!isCompactMode.value && customEvent.detail?.forceOpen === true && !isOpen.value) {
      isOpen.value = true
      nextTick(() => {
        scrollToBottom()
      })
    }
  }) as EventListener

  window.addEventListener('open-ai-assistant', openAssistantHandler)
})

onUnmounted(() => {
  if (process.client && openAssistantHandler) {
    window.removeEventListener('open-ai-assistant', openAssistantHandler)
  }
})
</script>

<style scoped>
.ai-assistant-button {
  position: fixed;
  right: var(--floating-dock-right, max(14px, env(safe-area-inset-right)));
  bottom: var(--floating-dock-bottom, max(18px, calc(env(safe-area-inset-bottom) + 14px)));
  z-index: 9990;
  display: flex;
  align-items: center;
  justify-content: center;
  width: var(--floating-dock-button-size, 48px);
  height: var(--floating-dock-button-size, 48px);
  border: 0;
  border-radius: 9999px;
  color: white;
  cursor: pointer;
  background:
    radial-gradient(circle at 30% 30%, rgba(255, 255, 255, 0.28) 0%, rgba(255, 255, 255, 0) 42%),
    linear-gradient(135deg, color-mix(in srgb, var(--color-primary) 84%, white) 0%, color-mix(in srgb, var(--color-purple) 82%, #111827) 100%);
  box-shadow:
    0 16px 32px rgba(37, 99, 235, 0.28),
    0 0 0 1px rgba(255, 255, 255, 0.18) inset;
  transition: transform 0.2s ease, box-shadow 0.2s ease, opacity 0.2s ease;
  backdrop-filter: blur(16px);
}

.ai-assistant-button:hover {
  transform: translateY(-1px);
  box-shadow: var(--shadow-xl, 0 12px 28px rgba(0, 0, 0, 0.28));
}

.ai-assistant-button__icon {
  width: 20px;
  height: 20px;
}

.ai-assistant-button-has-prompt::before {
  content: '';
  position: absolute;
  inset: -5px;
  border-radius: inherit;
  border: 1px solid color-mix(in srgb, var(--color-primary) 58%, white);
  opacity: 0.8;
}

.ai-assistant-status-dot {
  position: absolute;
  top: 2px;
  right: 2px;
  width: 10px;
  height: 10px;
  border-radius: 9999px;
  border: 2px solid white;
  background: var(--color-success);
}

.ai-assistant-dialog {
  position: fixed;
  right: var(--floating-dock-right, max(14px, env(safe-area-inset-right)));
  bottom: calc(var(--floating-dock-bottom, max(18px, calc(env(safe-area-inset-bottom) + 14px))) + 2px);
  z-index: 9990;
  width: min(336px, calc(100vw - 28px));
  height: min(520px, calc(100vh - 110px));
  background: var(--color-bg-card, white);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-xl, 0 20px 25px -5px rgba(0, 0, 0, 0.2));
  border: 1px solid var(--color-border-subtle);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.ai-assistant-header {
  background: linear-gradient(to right, var(--color-primary), var(--color-purple));
  padding: var(--spacing-md);
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.ai-assistant-header-content {
  display: flex;
  align-items: center;
  gap: var(--spacing-3);
}

.ai-assistant-avatar {
  width: 36px;
  height: 36px;
  background: color-mix(in srgb, var(--color-bg-card) 82%, white);
  border-radius: 9999px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ai-assistant-avatar-icon,
.ai-assistant-close-icon {
  width: 18px;
  height: 18px;
}

.ai-assistant-title {
  margin: 0;
  font-size: var(--text-base);
  font-weight: 700;
  color: white;
}

.ai-assistant-status {
  margin: 0;
  font-size: var(--text-xs);
  color: rgba(255, 255, 255, 0.82);
}

.ai-assistant-close-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 30px;
  height: 30px;
  border: 0;
  border-radius: 9999px;
  background: transparent;
  color: rgba(255, 255, 255, 0.8);
  cursor: pointer;
}

.ai-assistant-close-btn:hover {
  background: rgba(255, 255, 255, 0.12);
  color: white;
}

.ai-assistant-messages {
  flex: 1;
  overflow-y: auto;
  padding: var(--spacing-md);
  background: var(--color-bg-body);
}

.ai-assistant-welcome,
.ai-assistant-message-wrapper {
  display: flex;
  margin-bottom: var(--spacing-md);
}

.ai-assistant-message-user-wrapper {
  justify-content: flex-end;
}

.ai-assistant-message-assistant-wrapper {
  justify-content: flex-start;
}

.ai-assistant-message {
  max-width: 82%;
  padding: var(--spacing-md);
  border-radius: var(--radius-xl);
}

.ai-assistant-message-user {
  background: var(--color-primary);
  color: white;
  border-top-right-radius: 6px;
}

.ai-assistant-message-assistant {
  background: var(--color-bg-card, white);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-subtle);
  border-top-left-radius: 6px;
}

.ai-assistant-message-text,
.ai-assistant-message-loading {
  margin: 0;
  white-space: pre-wrap;
}

.ai-assistant-message-text {
  font-size: var(--text-sm);
}

.ai-assistant-message-loading {
  margin-top: var(--spacing-2);
  font-size: var(--text-xs);
  color: var(--color-text-muted);
}

.ai-assistant-loading {
  display: flex;
  gap: var(--spacing-1);
}

.ai-assistant-loading-dot {
  width: 8px;
  height: 8px;
  border-radius: 9999px;
  background: var(--color-text-muted);
  animation: bounce 1.4s infinite;
}

.ai-assistant-input-area {
  padding: var(--spacing-3);
  border-top: 1px solid var(--color-border-subtle);
  background: var(--color-bg-card, white);
}

.ai-assistant-quick-actions {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-2);
  margin-bottom: var(--spacing-3);
}

.ai-assistant-quick-action-btn {
  padding: var(--spacing-1_5) var(--spacing-3);
  border: 0;
  border-radius: 9999px;
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  font-size: var(--text-xs);
  cursor: pointer;
}

.ai-assistant-quick-action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-input-wrapper {
  display: flex;
  gap: var(--spacing-2);
}

.ai-assistant-input {
  flex: 1;
  padding: var(--spacing-2) var(--spacing-md);
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-md);
  background: var(--color-bg-card, white);
  color: var(--color-text-main);
  font-size: var(--text-sm);
}

.ai-assistant-input:focus {
  outline: none;
  border-color: var(--color-primary);
}

.ai-assistant-input:disabled,
.ai-assistant-send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-send-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2) var(--spacing-md);
  border: 0;
  border-radius: var(--radius-md);
  background: var(--color-primary);
  color: white;
  cursor: pointer;
}

.ai-assistant-send-btn:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

.ai-assistant-send-icon {
  width: 18px;
  height: 18px;
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.24s ease;
}

.slide-up-enter-from,
.slide-up-leave-to {
  opacity: 0;
  transform: translateY(var(--spacing-lg)) scale(0.96);
}

@keyframes bounce {
  0%,
  80%,
  100% {
    transform: scale(0);
  }

  40% {
    transform: scale(1);
  }
}

@media (max-width: 768px) {
  .ai-assistant-button {
    right: var(--floating-dock-right, max(12px, env(safe-area-inset-right)));
    bottom: var(--floating-dock-bottom, max(12px, calc(env(safe-area-inset-bottom) + 10px)));
  }

  .ai-assistant-dialog {
    left: 12px;
    right: 12px;
    width: auto;
    bottom: var(--floating-dock-bottom, max(12px, calc(env(safe-area-inset-bottom) + 10px)));
    height: min(66vh, 460px);
    border-radius: var(--radius-lg);
  }

  .ai-assistant-message {
    max-width: 88%;
  }
}
</style>
