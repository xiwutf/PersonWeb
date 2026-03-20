<template>
  <div class="min-h-screen">
    <component
      :is="AICharacter"
      v-if="showAiCharacter && AICharacter"
      :introduction-text="introText"
      :enable-chat="true"
    />
    <div
      v-else
      class="min-h-screen flex items-center justify-center px-6 text-center bg-gradient-to-b from-blue-50 to-purple-50 dark:from-gray-900 dark:to-gray-800"
    >
      <div class="max-w-xl space-y-4">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">AI 自我介绍</h1>
        <p class="text-base leading-7 text-gray-600 dark:text-gray-300">
          当前设备会优先使用更轻的展示模式，完整动画会在性能更好的环境下再加载。
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, onMounted, ref } from 'vue'

definePageMeta({
  layout: 'default'
})

const AICharacter = defineAsyncComponent(() => import('~/components/ai/AICharacter.vue'))
const shouldMountAiCharacter = ref(false)
const isConstrainedDevice = ref(false)
const showAiCharacter = computed(() => shouldMountAiCharacter.value && !isConstrainedDevice.value)

const introText = [
  '你好！我是溪午听风',
  '我是一名全栈开发者',
  '专注于 Vue.js、Nuxt.js 和 Node.js',
  '也喜欢探索 AI 和机器学习',
  '欢迎来到我的个人网站！',
  '这里展示我的项目、文章和技术分享',
  '希望你能在这里找到有趣的内容！'
]

const detectConstrainedDevice = () => {
  const reducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const narrowScreen = window.innerWidth < 1024
  const saveData = navigator.connection?.saveData === true
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  isConstrainedDevice.value = Boolean(reducedMotion || coarsePointer || narrowScreen || saveData || lowMemory)
}

onMounted(() => {
  detectConstrainedDevice()
  if (isConstrainedDevice.value) return

  const mountCharacter = () => {
    shouldMountAiCharacter.value = true
  }

  if ('requestIdleCallback' in window) {
    ;(window as Window & {
      requestIdleCallback: (callback: IdleRequestCallback, options?: IdleRequestOptions) => number
    }).requestIdleCallback(() => mountCharacter(), { timeout: 1500 })
    return
  }

  window.setTimeout(mountCharacter, 600)
})

useHead({
  title: 'AI 自我介绍 - 溪午听风',
  meta: [
    { name: 'description', content: 'AI 驱动的自我介绍动画，了解我的技术背景和兴趣' }
  ]
})
</script>
