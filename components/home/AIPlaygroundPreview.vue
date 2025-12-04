<template>
  <section class="ai-playground-preview">
    <div class="ai-playground-preview-container">
      <div class="ai-playground-preview-header">
        <h2 class="ai-playground-preview-title">AI Playground</h2>
        <p class="ai-playground-preview-subtitle">在首页直接体验 AI 能力</p>
      </div>

      <div
        class="ai-playground-container"
        ref="containerRef"
      >
        <!-- 按钮区域 -->
        <div class="ai-playground-buttons">
          <button
            v-for="(prompt, index) in prompts"
            :key="index"
            @click="handlePromptClick(prompt)"
            :disabled="loading"
            class="ai-playground-prompt-button"
            :class="{ 'ai-playground-prompt-button-disabled': loading }"
          >
            <i :class="prompt.icon" class="ai-playground-prompt-button-icon"></i>
            {{ prompt.label }}
          </button>
        </div>

        <!-- AI 回复区域 -->
        <div
          v-if="aiResponse"
          class="ai-playground-response-container"
        >
          <div class="ai-playground-response-content">
            <div class="ai-playground-avatar">
              <i class="fas fa-robot"></i>
            </div>
            <div class="ai-playground-response-text">
              <div class="ai-playground-response-label">AI 回复</div>
              <div class="ai-playground-response-body" v-html="aiResponse"></div>
            </div>
          </div>
        </div>

        <!-- 加载状态 -->
        <div v-if="loading" class="ai-playground-loading">
          <div class="ai-playground-loading-content">
            <div class="ai-playground-loading-spinner"></div>
            <span>AI 正在思考...</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView } from '@motionone/dom'

const containerRef = ref<HTMLElement | null>(null)
const loading = ref(false)
const aiResponse = ref<string>('')

// Prompt 选项
const prompts = [
  {
    label: '帮我生成一句今天的能量语',
    icon: 'fas fa-bolt',
    type: 'energy'
  },
  {
    label: '给我一句关于代码的有趣比喻',
    icon: 'fas fa-code',
    type: 'code'
  },
  {
    label: '写一条适合发朋友圈的小感悟',
    icon: 'fas fa-heart',
    type: 'life'
  }
]

// Mock AI 回复数据
const mockResponses: Record<string, string[]> = {
  energy: [
    '今天最好的状态，就是现在开始行动。',
    '每一个不曾起舞的日子，都是对生命的辜负。',
    '把今天过好，就是对明天最好的准备。'
  ],
  code: [
    '代码就像诗歌，每一行都应该有意义，每一个函数都应该有故事。',
    '写代码就像搭积木，好的架构让每一块都恰到好处。',
    'Bug 是代码的幽默感，它总在不经意间让你笑出声。'
  ],
  life: [
    '生活不是等待暴风雨过去，而是学会在雨中跳舞。',
    '每一个平凡的日子，都值得被认真对待。',
    '简单的生活，丰富的内心，这就是最好的状态。'
  ]
}

// 处理 Prompt 点击
const handlePromptClick = async (prompt: { type: string }) => {
  loading.value = true
  aiResponse.value = ''

  // 模拟 API 调用延迟
  await new Promise(resolve => setTimeout(resolve, 1000 + Math.random() * 1000))

  // 随机选择一个回复
  const responses = mockResponses[prompt.type] || []
  const randomResponse = responses[Math.floor(Math.random() * responses.length)]

  // 打字效果
  loading.value = false
  typeText(randomResponse)
}

// 打字效果
const typeText = (text: string) => {
  let index = 0
  const timer = setInterval(() => {
    if (index < text.length) {
      aiResponse.value = text.slice(0, index + 1)
      index++
    } else {
      clearInterval(timer)
    }
  }, 50)
}

// 预留的 AI API 调用方法
const callAiApi = async (prompt: string): Promise<string> => {
  // TODO: 接入真实的 AI Service API
  // const api = useApi()
  // const response = await api.post('/ai/chat', { prompt })
  // return response.message
  return ''
}

onMounted(() => {
  if (containerRef.value) {
    inView(containerRef.value, () => {
      animate(
        containerRef.value!,
        { opacity: [0, 1], y: [50, 0] },
        { duration: 0.8, easing: 'ease-out' }
      )
    })
  }
})
</script>

<style scoped>
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

