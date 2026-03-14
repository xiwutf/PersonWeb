<template>
  <div class="container mx-auto px-4 py-8 h-[calc(100vh-64px)] flex flex-col">
    <div class="flex-1 bg-var(--color-bg-light, white) rounded-xl shadow-lg overflow-hidden flex flex-col border border-gray-200">
      <!-- 聊天头部 -->
      <div class="bg-gradient-to-r from-blue-600 to-purple-600 p-4 text-var(--color-bg-light, white) flex justify-between items-center">
        <h1 class="text-lg font-bold flex items-center gap-2">
          <span>🤖</span> AI 助手 (演示版)
        </h1>
        <span class="text-xs bg-var(--color-bg-light, white)/20 px-2 py-1 rounded">在线</span>
      </div>
      
      <!-- 聊天记录区域 -->
      <div class="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-50" ref="chatContainer">
        <div v-for="(msg, index) in messages" :key="index" 
             class="flex" :class="msg.role === 'user' ? 'justify-end' : 'justify-start'">
          <div class="max-w-[80%] rounded-lg p-3 shadow-sm"
               :class="msg.role === 'user' ? 'bg-blue-600 text-var(--color-bg-light, white) rounded-tr-none' : 'bg-var(--color-bg-light, white) text-gray-800 rounded-tl-none border border-gray-200'">
            <p>{{ msg.content }}</p>
          </div>
        </div>
        
        <div v-if="loading" class="flex justify-start">
          <div class="bg-var(--color-bg-light, white) text-gray-500 rounded-lg p-3 rounded-tl-none border border-gray-200 shadow-sm">
            <span class="animate-pulse">正在思考...</span>
          </div>
        </div>
      </div>
      
      <!-- 输入区域 -->
      <div class="p-4 bg-var(--color-bg-light, white) border-t border-gray-200">
        <div class="flex gap-2">
          <input 
            v-model="userInput" 
            @keyup.enter="sendMessage"
            type="text" 
            placeholder="输入您的问题..." 
            class="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            :disabled="loading"
          />
          <button 
            @click="sendMessage"
            class="bg-blue-600 text-var(--color-bg-light, white) px-6 py-2 rounded-lg hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
            :disabled="loading || !userInput.trim()"
          >
            发送
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'

interface Message {
  role: 'user' | 'assistant'
  content: string
}

const messages = ref<Message[]>([
  { role: 'assistant', content: '你好！我是你的 AI 助手。有什么我可以帮你的吗？' }
])
const userInput = ref('')
const loading = ref(false)
const chatContainer = ref<HTMLElement | null>(null)

// 滚动到底部
const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

const sendMessage = async () => {
  if (!userInput.value.trim() || loading.value) return
  
  // 添加用户消息
  messages.value.push({ role: 'user', content: userInput.value })
  const question = userInput.value
  userInput.value = ''
  loading.value = true
  await scrollToBottom()
  
  // 模拟 API 调用延迟
  setTimeout(() => {
    messages.value.push({ 
      role: 'assistant', 
      content: `这是对 "${question}" 的模拟回复。实际项目中，这里会调用后端 LLM 接口。` 
    })
    loading.value = false
    scrollToBottom()
  }, 1000)
  
  // 实际项目中调用 API:
  // const api = useApi()
  // const response = await api.post('/chat', { message: question })
}
</script>

