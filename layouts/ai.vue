<template>
  <!-- 
    AI 实验室布局（ai.vue）
    用途：AI 相关页面的专用布局，包含顶部导航栏
    使用场景：AI 实验室首页 (/ai)、AI 相关详情页 (/ai/[type]/[slug])
    特点：主内容区域使用 pt-24（顶部内边距）
  -->
  <!-- 使用 AppNaiveConfig 统一管理 Naive UI 主题配置 -->
  <!-- mode="theme": 前台轻量模式，仅提供主题配置，不含 Message/Dialog/Notification -->
  <AppNaiveConfig mode="theme">
    <!-- AI 布局：使用主题背景色和文字颜色，替换写死的深色背景 -->
    <div class="min-h-screen flex flex-col bg-bg-body text-text-main">
    <!-- 注意：Header 已移至 app.vue 全局挂载，此处不再需要 -->
    
    <!-- 主要内容区域 -->
    <main class="flex-1 pt-24">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail v-if="showDesktopEnhancements" />
    <!-- AI 智能助手 -->
    <AIAssistant v-if="showPrimaryFloatingAssistant" />
    
    <!-- 访客互动功能 -->
    <VisitorInteractionPanel v-if="showSecondaryFloatingTools" />
    
    <!-- 访客互动式玩法（包含在抽屉中） -->
    <VisitorBehaviorListener v-if="showDesktopEnhancements" />
    <!-- 访客侧边栏抽屉（包含留言、互动等功能） -->
    <VisitorSidebarDrawer v-if="showDesktopEnhancements" />
    
    <!-- 智能客服（前台访客使用） -->
    <SupportChat v-if="showSecondaryFloatingTools" />
    </div>
  </AppNaiveConfig>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, onMounted, onUnmounted, ref } from 'vue'

// 显式导入组件，确保 Nuxt 3 自动导入正常工作
import AppNaiveConfig from '~/components/layout/AppNaiveConfig.vue'
import Footer from '~/components/layout/Footer.vue'

const MouseTrail = defineAsyncComponent(() => import('~/components/effects/MouseTrail.vue'))
const AIAssistant = defineAsyncComponent(() => import('~/components/ai/AIAssistant.vue'))
const VisitorInteractionPanel = defineAsyncComponent(() => import('~/components/VisitorInteractionPanel.vue'))
const VisitorBehaviorListener = defineAsyncComponent(() => import('~/components/VisitorBehaviorListener.vue'))
const VisitorSidebarDrawer = defineAsyncComponent(() => import('~/components/VisitorSidebarDrawer.vue'))
const SupportChat = defineAsyncComponent(() => import('~/components/ai/SupportChat.vue'))

const shouldMountDeferredUi = ref(false)
const isLowPowerMode = ref(false)
const isCompactFloatingMode = ref(false)

const showDeferredWidgets = computed(() => shouldMountDeferredUi.value && !isLowPowerMode.value)
const showDesktopEnhancements = computed(() => showDeferredWidgets.value)
const showPrimaryFloatingAssistant = computed(() => showDeferredWidgets.value)
const showSecondaryFloatingTools = computed(() => showDeferredWidgets.value && !isCompactFloatingMode.value)

let deferredMountTimer: number | null = null

const detectLowPowerMode = () => {
  const coarsePointer = window.matchMedia('(pointer: coarse)').matches
  const narrowScreen = window.innerWidth < 1024
  const compactViewport = window.innerWidth < 1480 || window.innerHeight < 920
  const saveData = 'connection' in navigator && (navigator as Navigator & {
    connection?: { saveData?: boolean }
  }).connection?.saveData === true
  const lowMemoryValue = 'deviceMemory' in navigator
    ? Number((navigator as Navigator & { deviceMemory?: number }).deviceMemory || 0)
    : 0
  const lowMemory = lowMemoryValue > 0 && lowMemoryValue <= 4

  isLowPowerMode.value = coarsePointer || narrowScreen || saveData || lowMemory
  isCompactFloatingMode.value = compactViewport
}

const scheduleDeferredWidgets = () => {
  if (isLowPowerMode.value) {
    shouldMountDeferredUi.value = false
    return
  }

  const mountWidgets = () => {
    shouldMountDeferredUi.value = true
  }

  if ('requestIdleCallback' in window) {
    ;(window as Window & {
      requestIdleCallback: (callback: IdleRequestCallback, options?: IdleRequestOptions) => number
    }).requestIdleCallback(() => mountWidgets(), { timeout: 2000 })
    return
  }

  deferredMountTimer = window.setTimeout(mountWidgets, 1200)
}

onMounted(() => {
  detectLowPowerMode()
  scheduleDeferredWidgets()
})

onUnmounted(() => {
  if (deferredMountTimer) {
    window.clearTimeout(deferredMountTimer)
  }
})
</script>
