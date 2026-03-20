<template>
  <!-- 
    默认布局（default.vue）
    用途：所有普通页面的默认布局，包含完整的顶部导航栏和页脚
    使用场景：博客、工具、项目展示、关于等所有前台页面
    注意：如果页面没有使用 definePageMeta 指定布局，Nuxt 3 会自动使用此布局
  -->
  <!-- 使用 AppNaiveConfig 统一管理 Naive UI 主题配置 -->
  <AppNaiveConfig>
    <!-- 最外层容器：使用主题背景色和文字颜色 -->
    <div class="min-h-screen flex flex-col relative bg-bg-body text-text-main">
    <!-- 动态背景效果（根据主题切换） -->
    <!-- 🔴 已禁用：可能导致滚动条闪烁（requestAnimationFrame 持续动画） -->
    <!-- <BackgroundEffects :effect="currentBackground" :config="backgroundConfig" /> -->
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail v-if="showDesktopEnhancements" />
    
    <!-- 风格切换面板 -->
    <ThemeSwitcher v-if="showUtilityWidgets" />
    
    <!-- 注意：Header 已移至 app.vue 全局挂载，此处不再需要 -->
    
    <!-- 主要内容区域 -->
    <main class="flex-1 pt-20 relative z-10">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
    <!-- AI 智能助手（独立，不放入抽屉） -->
    <AIAssistant v-if="showDeferredWidgets" />
    
    <!-- 智能客服（前台访客使用） -->
    <SupportChat v-if="showDeferredWidgets" />
    
    <!-- 访客互动功能 -->
    <!-- 🔴 已禁用：z-index 过高，可能遮挡导航栏 -->
    <!-- VisitorDanmakuWall: z-index: 100 (和导航栏同级) -->
    <!-- <VisitorDanmakuWall /> -->
    <!-- VisitorBubble: z-index: 200 (比导航栏高) -->
    <!-- <VisitorBubble /> -->
    <!-- VisitorInteractionPanel: 留言、互动功能（右下角） -->
    <VisitorInteractionPanel v-if="showDeferredWidgets" />
    
    <!-- 访客互动式玩法（包含在抽屉中） -->
    <VisitorBehaviorListener v-if="showDesktopEnhancements" />
    <!-- 访客侧边栏抽屉（包含留言、互动等功能） -->
    <VisitorSidebarDrawer v-if="showDesktopEnhancements" />
    <!-- 🔴 已禁用：可能导致滚动条闪烁（fixed 定位 + 动画） -->
    <!-- <VisitorTriggerEffects /> -->
    <!-- 🔴 已禁用：可能导致滚动条闪烁（requestAnimationFrame 持续动画） -->
    <!-- <FireworksEffect /> -->
    
    <!-- 隐秘的后台入口 -->
    <SecretAdminAccess />
    </div>
  </AppNaiveConfig>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, onMounted, onUnmounted, ref } from 'vue'

// 显式导入组件，确保 Nuxt 3 自动导入正常工作
import AppNaiveConfig from '~/components/layout/AppNaiveConfig.vue'
import Footer from '~/components/layout/Footer.vue'
import SecretAdminAccess from '~/components/admin/SecretAdminAccess.vue'

const MouseTrail = defineAsyncComponent(() => import('~/components/effects/MouseTrail.vue'))
const ThemeSwitcher = defineAsyncComponent(() => import('~/components/layout/ThemeSwitcher.vue'))
const AIAssistant = defineAsyncComponent(() => import('~/components/ai/AIAssistant.vue'))
const SupportChat = defineAsyncComponent(() => import('~/components/ai/SupportChat.vue'))
const VisitorInteractionPanel = defineAsyncComponent(() => import('~/components/VisitorInteractionPanel.vue'))
const VisitorBehaviorListener = defineAsyncComponent(() => import('~/components/VisitorBehaviorListener.vue'))
const VisitorSidebarDrawer = defineAsyncComponent(() => import('~/components/VisitorSidebarDrawer.vue'))

const shouldMountDeferredUi = ref(false)
const shouldMountUtilityUi = ref(false)
const isLowPowerMode = ref(false)

const showDeferredWidgets = computed(() => shouldMountDeferredUi.value && !isLowPowerMode.value)
const showDesktopEnhancements = computed(() => showDeferredWidgets.value)
const showUtilityWidgets = computed(() => shouldMountUtilityUi.value)

let deferredMountTimer: number | null = null
let utilityMountTimer: number | null = null

const detectLowPowerMode = () => {
  const coarsePointer = window.matchMedia('(pointer: coarse)').matches
  const narrowScreen = window.innerWidth < 1024
  const saveData = 'connection' in navigator && (navigator as Navigator & {
    connection?: { saveData?: boolean }
  }).connection?.saveData === true
  const lowMemoryValue = 'deviceMemory' in navigator
    ? Number((navigator as Navigator & { deviceMemory?: number }).deviceMemory || 0)
    : 0
  const lowMemory = lowMemoryValue > 0 && lowMemoryValue <= 4

  isLowPowerMode.value = coarsePointer || narrowScreen || saveData || lowMemory
}

const scheduleDeferredWidgets = () => {
  const mountUtilityWidgets = () => {
    shouldMountUtilityUi.value = true
  }

  if ('requestIdleCallback' in window) {
    ;(window as Window & {
      requestIdleCallback: (callback: IdleRequestCallback, options?: IdleRequestOptions) => number
    }).requestIdleCallback(() => mountUtilityWidgets(), { timeout: 1200 })
  } else {
    utilityMountTimer = window.setTimeout(mountUtilityWidgets, 500)
  }

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
  if (!process.client) {
    return
  }

  let visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    visitorId = `visitor_${Date.now()}_${Math.random().toString(36).slice(2, 11)}`
    localStorage.setItem('visitor_id', visitorId)
  }

  detectLowPowerMode()
  scheduleDeferredWidgets()
})

onUnmounted(() => {
  if (deferredMountTimer) {
    window.clearTimeout(deferredMountTimer)
  }
  if (utilityMountTimer) {
    window.clearTimeout(utilityMountTimer)
  }
})
</script>
