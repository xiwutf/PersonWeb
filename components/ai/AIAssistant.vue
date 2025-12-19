<template>
  <ClientOnly>
    <!-- AI 助手按钮 -->
    <button
    v-if="!isOpen"
    @click="toggleAssistant"
    class="fixed bottom-2 right-2 w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full shadow-lg hover:shadow-xl transition-all duration-300 hover:scale-110 z-[1001] flex items-center justify-center group ai-assistant-button"
    style="z-index: 10000 !important; position: fixed !important; bottom: 0.5rem !important; right: 0.5rem !important; visibility: visible !important; opacity: 1 !important; display: flex !important; pointer-events: auto !important; isolation: isolate; transform: translateZ(0); width: 3rem !important; height: 3rem !important;"
    aria-label="打开 AI 助手"
  >
    <svg class="w-6 h-6 text-white group-hover:rotate-12 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
    </svg>
    <span class="absolute -top-0.5 -right-0.5 w-3 h-3 ai-assistant-status-dot rounded-full border-2 border-white animate-pulse"></span>
  </button>

  <!-- AI 助手对话框 -->
  <Transition name="slide-up">
    <div
      v-if="isOpen"
      class="ai-assistant-dialog"
    >
      <!-- 头部 -->
      <div class="ai-assistant-header">
        <div class="ai-assistant-header-content">
          <div class="ai-assistant-avatar">
            <svg class="ai-assistant-avatar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
            </svg>
          </div>
          <div>
            <h3 class="ai-assistant-title">AI 小智</h3>
            <p class="ai-assistant-status">{{ statusText }}</p>
          </div>
        </div>
        <button
          @click="toggleAssistant"
          class="ai-assistant-close-btn"
          aria-label="关闭"
        >
          <svg class="ai-assistant-close-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- 消息列表 -->
      <div ref="messagesRef" class="ai-assistant-messages">
        <!-- 欢迎消息 -->
        <div v-if="messages.length === 0" class="ai-assistant-welcome">
          <div class="ai-assistant-message ai-assistant-message-assistant">
            <p class="ai-assistant-message-text">👋 你好！我是 AI 小智，有什么可以帮你的吗？</p>
          </div>
        </div>

        <!-- 消息列表 -->
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

        <!-- 加载中 -->
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

      <!-- 输入区域 -->
      <div class="ai-assistant-input-area">
        <!-- 快捷操作 -->
        <div class="ai-assistant-quick-actions">
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

// 简化快捷操作，只保留最核心的功能
const quickActions = [
  { text: '推荐文章' },
  { text: '搜索文章' }
]

const toggleAssistant = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    // 滚动到底部
    nextTick(() => {
      scrollToBottom()
    })
    // 用户手动打开，清除关闭标记和禁用状态
    hasUserClosed.value = false
    if (process.client) {
      localStorage.removeItem('ai-assistant-auto-open-disabled')
    }
  } else {
    // 用户手动关闭，记录状态
    hasUserClosed.value = true
  }
}

// 记录用户是否手动关闭过助手（避免频繁自动弹出）
const hasUserClosed = ref(false)

// 监听打开助手事件（用于触发问候）
onMounted(() => {
  if (process.client) {
    // 检查用户是否设置了不自动弹出
    const autoOpenDisabled = localStorage.getItem('ai-assistant-auto-open-disabled')
    if (autoOpenDisabled === 'true') {
      return // 用户已禁用自动弹出
    }

    window.addEventListener('open-ai-assistant', ((e: CustomEvent) => {
      // 如果用户手动关闭过，不再自动打开
      if (hasUserClosed.value) {
        return
      }
      
      if (!isOpen.value) {
        isOpen.value = true
        nextTick(() => {
          if (e.detail?.message) {
            // 直接添加为AI的问候消息，而不是用户消息
            messages.value.push({
              role: 'assistant',
              content: e.detail.message
            })
          }
          scrollToBottom()
        })
      }
    }) as EventListener)
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('open-ai-assistant', () => {})
  }
})

const scrollToBottom = () => {
  if (messagesRef.value) {
    messagesRef.value.scrollTop = messagesRef.value.scrollHeight
  }
}

const sendQuickMessage = (text: string) => {
  inputText.value = text
  sendMessage()
}

const sendMessage = async () => {
  if (!inputText.value.trim() || isLoading.value) return

  const userMessage = inputText.value.trim()
  inputText.value = ''

  // 添加用户消息
  messages.value.push({
    role: 'user',
    content: userMessage
  })

  scrollToBottom()
  isLoading.value = true
  statusText.value = '正在思考...'

  try {
    // 调用后端 AI API
    const api = useApi()
    const response = await api.post<any>('/AI/chat', {
      message: userMessage,
      history: messages.value.slice(-5).map(m => ({
        role: m.role,
        content: m.content
      })) // 只发送最近5条消息作为上下文
    })

    // 添加 AI 回复
    const aiResponse = response.message || response.content || '抱歉，我暂时无法回答这个问题。'
    messages.value.push({
      role: 'assistant',
      content: aiResponse
    })

    statusText.value = '在线'
  } catch (error: any) {
    console.error('AI Chat error:', error)
    messages.value.push({
      role: 'assistant',
      content: '抱歉，服务暂时不可用，请稍后再试。'
    })
    statusText.value = '离线'
  } finally {
    isLoading.value = false
    scrollToBottom()
  }
}

// 注意：点击外部关闭功能可以后续添加
</script>

<style scoped>
/* 对话框容器 - 使用 CSS 变量 */
.ai-assistant-button {
  position: fixed !important;
  bottom: 0.5rem !important;
  right: 0.5rem !important;
  z-index: 10000 !important;
  display: flex !important;
  visibility: visible !important;
  opacity: 1 !important;
  pointer-events: auto !important;
  width: 3rem !important;
  height: 3rem !important;
  background: linear-gradient(to bottom right, var(--color-primary, rgb(59, 130, 246)), var(--color-purple, rgb(147, 51, 234))) !important;
  box-shadow: var(--shadow-lg, 0 4px 12px rgba(0, 0, 0, 0.5)) !important;
}

/* 对话框容器 - 使用 CSS 变量 */
.ai-assistant-dialog {
  position: fixed;
  bottom: 0.5rem;
  right: 0.5rem;
  width: 20rem;
  max-width: calc(100vw - 1rem);
  height: 28rem;
  max-height: calc(100vh - 1rem);
  background: var(--color-bg-card, white);
  border-radius: 1rem;
  box-shadow: var(--shadow-xl, 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04));
  border: 1px solid var(--color-border-subtle, rgba(229, 231, 235, 1));
  z-index: 10000 !important; /* 确保对话框也在最上层 */
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 头部 - 使用 CSS 变量 */
.ai-assistant-header {
  background: linear-gradient(to right, var(--color-primary, rgb(59, 130, 246)), var(--color-purple, rgb(147, 51, 234)));
  padding: 1rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.ai-assistant-header-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.ai-assistant-avatar {
  width: 2.5rem;
  height: 2.5rem;
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.2));
  border-radius: 9999px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ai-assistant-avatar-icon {
  width: 1.5rem;
  height: 1.5rem;
  color: var(--color-text-main, white);
}

.ai-assistant-title {
  color: var(--color-text-main, white);
  font-weight: 700;
  font-size: 1rem;
  margin: 0;
}

.ai-assistant-status {
  color: var(--color-text-sub, rgba(255, 255, 255, 0.8));
  font-size: 0.75rem;
  margin: 0;
}

.ai-assistant-close-btn {
  color: var(--color-text-sub, rgba(255, 255, 255, 0.8));
  transition: color 0.2s;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.25rem;
}

.ai-assistant-close-btn:hover {
  color: var(--color-text-main, white);
}

.ai-assistant-close-icon {
  width: 1.5rem;
  height: 1.5rem;
}

/* 消息区域 - 使用 CSS 变量 */
.ai-assistant-messages {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
  background: var(--color-bg-body, rgb(249, 250, 251));
}

.ai-assistant-welcome {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.ai-assistant-message-wrapper {
  display: flex;
  margin-bottom: 1rem;
}

.ai-assistant-message-user-wrapper {
  justify-content: flex-end;
}

.ai-assistant-message-assistant-wrapper {
  justify-content: flex-start;
}

.ai-assistant-message {
  border-radius: 1rem;
  padding: 1rem;
  max-width: 80%;
}

/* 用户消息 - 使用 CSS 变量 */
.ai-assistant-message-user {
  background: var(--color-primary, rgb(59, 130, 246));
  color: var(--color-text-main, white);
  border-top-right-radius: 0;
}

/* AI 消息 - 使用 CSS 变量 */
.ai-assistant-message-assistant {
  background: var(--color-bg-card, white);
  color: var(--color-text-main, rgb(31, 41, 55));
  border-top-left-radius: 0;
  border: 1px solid var(--color-border-subtle, rgba(229, 231, 235, 1));
}

.ai-assistant-message-text {
  font-size: 0.875rem;
  white-space: pre-wrap;
  margin: 0;
}

.ai-assistant-message-loading {
  font-size: 0.75rem;
  color: var(--color-text-muted, rgb(107, 114, 128));
  margin-top: 0.5rem;
  margin-bottom: 0;
}

.ai-assistant-loading {
  display: flex;
  gap: 0.25rem;
}

.ai-assistant-loading-dot {
  width: 0.5rem;
  height: 0.5rem;
  background: var(--color-text-muted, rgb(156, 163, 175));
  border-radius: 9999px;
  animation: bounce 1.4s infinite;
}

@keyframes bounce {
  0%, 80%, 100% {
    transform: scale(0);
  }
  40% {
    transform: scale(1);
  }
}

/* 输入区域 - 使用 CSS 变量 */
.ai-assistant-input-area {
  padding: 0.75rem;
  border-top: 1px solid var(--color-border-subtle, rgba(229, 231, 235, 1));
  background: var(--color-bg-card, white);
}

.ai-assistant-quick-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.ai-assistant-quick-action-btn {
  padding: 0.375rem 0.75rem;
  font-size: 0.75rem;
  background: var(--color-bg-elevated, rgb(243, 244, 246));
  color: var(--color-text-main, rgb(55, 65, 81));
  border-radius: 9999px;
  border: none;
  cursor: pointer;
  transition: background 0.2s;
}

.ai-assistant-quick-action-btn:hover:not(:disabled) {
  background: var(--color-bg-elevated, rgb(229, 231, 235));
}

.ai-assistant-quick-action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-input-wrapper {
  display: flex;
  gap: 0.5rem;
}

.ai-assistant-input {
  flex: 1;
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-border-default, rgba(209, 213, 219, 1));
  border-radius: 0.5rem;
  background: var(--color-bg-card, white);
  color: var(--color-text-main, rgb(31, 41, 55));
  font-size: 0.875rem;
}

.ai-assistant-input:focus {
  outline: none;
  ring: 2px;
  ring-color: var(--color-primary, rgb(59, 130, 246));
}

.ai-assistant-input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-send-btn {
  padding: 0.5rem 1rem;
  background: var(--color-primary, rgb(59, 130, 246));
  color: var(--color-text-main, white);
  border-radius: 0.5rem;
  border: none;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ai-assistant-send-btn:hover:not(:disabled) {
  background: var(--color-primary-hover, rgb(37, 99, 235));
}

.ai-assistant-send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.ai-assistant-send-icon {
  width: 1.25rem;
  height: 1.25rem;
}

/* 动画 */
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(20px) scale(0.9);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.9);
}

/* AI 助手状态点样式 - 使用 CSS 变量 */
.ai-assistant-status-dot {
  background: var(--color-success, #10b981);
}
</style>

