<template>
  <!-- AI 助手按钮 -->
  <button
    v-if="!isOpen"
    @click="toggleAssistant"
    class="fixed bottom-8 right-8 w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full shadow-lg hover:shadow-xl transition-all duration-300 hover:scale-110 z-50 flex items-center justify-center group"
    aria-label="打开 AI 助手"
  >
    <svg class="w-8 h-8 text-white group-hover:rotate-12 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
    </svg>
    <span class="absolute -top-1 -right-1 w-4 h-4 bg-green-500 rounded-full border-2 border-white animate-pulse"></span>
  </button>

  <!-- AI 助手对话框 -->
  <Transition name="slide-up">
    <div
      v-if="isOpen"
      class="fixed bottom-8 right-8 w-96 h-[600px] bg-white dark:bg-gray-800 rounded-2xl shadow-2xl border border-gray-200 dark:border-gray-700 z-50 flex flex-col overflow-hidden"
    >
      <!-- 头部 -->
      <div class="bg-gradient-to-r from-blue-500 to-purple-600 p-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 bg-white/20 rounded-full flex items-center justify-center">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
            </svg>
          </div>
          <div>
            <h3 class="text-white font-bold">AI 小智</h3>
            <p class="text-white/80 text-xs">{{ statusText }}</p>
          </div>
        </div>
        <button
          @click="toggleAssistant"
          class="text-white/80 hover:text-white transition-colors"
          aria-label="关闭"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- 消息列表 -->
      <div ref="messagesRef" class="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50 dark:bg-gray-900">
        <!-- 欢迎消息 -->
        <div v-if="messages.length === 0" class="space-y-2">
          <div class="bg-blue-500 text-white rounded-2xl rounded-tl-none p-4 max-w-[80%]">
            <p class="text-sm">👋 你好！我是 AI 小智，有什么可以帮你的吗？</p>
          </div>
        </div>

        <!-- 消息列表 -->
        <div
          v-for="(message, index) in messages"
          :key="index"
          :class="[
            'flex',
            message.role === 'user' ? 'justify-end' : 'justify-start'
          ]"
        >
          <div
            :class="[
              'rounded-2xl p-4 max-w-[80%]',
              message.role === 'user'
                ? 'bg-blue-500 text-white rounded-tr-none'
                : 'bg-white dark:bg-gray-800 text-gray-800 dark:text-gray-200 rounded-tl-none border border-gray-200 dark:border-gray-700'
            ]"
          >
            <p class="text-sm whitespace-pre-wrap">{{ message.content }}</p>
            <p v-if="message.role === 'assistant' && message.loading" class="text-xs text-gray-500 mt-2">正在思考...</p>
          </div>
        </div>

        <!-- 加载中 -->
        <div v-if="isLoading" class="flex justify-start">
          <div class="bg-white dark:bg-gray-800 rounded-2xl rounded-tl-none p-4 border border-gray-200 dark:border-gray-700">
            <div class="flex gap-1">
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0s"></div>
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.4s"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- 输入区域 -->
      <div class="p-4 border-t border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
        <div class="flex gap-2">
          <input
            v-model="inputText"
            @keyup.enter="sendMessage"
            type="text"
            placeholder="输入你的问题..."
            class="flex-1 px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500"
            :disabled="isLoading"
          />
          <button
            @click="sendMessage"
            :disabled="isLoading || !inputText.trim()"
            class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
            </svg>
          </button>
        </div>
        <!-- 快捷操作 -->
        <div class="flex flex-wrap gap-2 mt-2">
          <button
            v-for="quickAction in quickActions"
            :key="quickAction.text"
            @click="sendQuickMessage(quickAction.text)"
            class="px-3 py-1 text-xs bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-full hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
            :disabled="isLoading"
          >
            {{ quickAction.text }}
          </button>
        </div>
      </div>
    </div>
  </Transition>
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
  }
}

// 监听打开助手事件（用于触发问候）
onMounted(() => {
  if (process.client) {
    window.addEventListener('open-ai-assistant', ((e: CustomEvent) => {
      if (!isOpen.value) {
        isOpen.value = true
        nextTick(() => {
          if (e.detail?.message) {
            // 自动发送问候消息
            inputText.value = e.detail.message
            sendMessage()
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
</style>

