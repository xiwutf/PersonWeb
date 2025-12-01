<template>
  <section class="ai-playground-preview py-24 sm:py-32 relative">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-12">
        <h2 class="text-4xl lg:text-5xl font-bold text-text-main mb-4">AI Playground</h2>
        <p class="text-lg text-text-muted">在首页直接体验 AI 能力</p>
      </div>

      <div
        class="playground-container bg-bg-card backdrop-blur-xl border border-border-subtle rounded-3xl p-8"
        ref="containerRef"
      >
        <!-- 按钮区域 -->
        <div class="flex flex-wrap justify-center gap-4 mb-8">
          <button
            v-for="(prompt, index) in prompts"
            :key="index"
            @click="handlePromptClick(prompt)"
            :disabled="loading"
            class="prompt-button"
            :class="{ 'opacity-50 cursor-not-allowed': loading }"
          >
            <i :class="prompt.icon" class="mr-2"></i>
            {{ prompt.label }}
          </button>
        </div>

        <!-- AI 回复区域 -->
        <div
          v-if="aiResponse"
          class="ai-response-container bg-bg-card rounded-xl p-6 border border-border-subtle"
        >
          <div class="flex items-start gap-3">
            <div class="ai-avatar bg-primary-base rounded-full w-10 h-10 flex items-center justify-center flex-shrink-0">
              <i class="fas fa-robot text-text-main"></i>
            </div>
            <div class="flex-1">
              <div class="text-sm text-text-muted mb-2">AI 回复</div>
              <div class="text-text-main leading-relaxed" v-html="aiResponse"></div>
            </div>
          </div>
        </div>

        <!-- 加载状态 -->
        <div v-if="loading" class="text-center py-8">
          <div class="inline-flex items-center gap-2 text-text-muted">
            <div class="animate-spin rounded-full h-5 w-5 border-b-2 border-primary-base"></div>
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
.ai-playground-preview {
  background-color: var(--color-bg-body);
}

.playground-container {
  box-shadow: var(--shadow-md, 0 4px 12px -2px rgba(0, 0, 0, 0.6));
}

.prompt-button {
  @apply px-6 py-3 bg-bg-elevated hover:bg-bg-card border border-border-default rounded-full text-text-main font-medium transition-all duration-300;
  @apply hover:scale-105 hover:border-border-strong;
}

.prompt-button:active {
  @apply scale-95;
}

.ai-response-container {
  animation: fadeIn 0.5s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>

