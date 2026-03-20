<template>
  <!-- 
    首页布局（home.vue）
    用途：首页专用布局，包含顶部导航栏，但主内容区域无顶部内边距（用于沉浸式 Hero 效果）
    使用场景：首页 (/)
    特点：主内容区域使用 pt-0（无顶部内边距），允许 Hero 区域延伸到顶部，包含粒子背景特效
  -->
  <!-- 首页布局：使用主题背景色和文字颜色 -->
  <div class="min-h-screen flex flex-col relative" style="background-color: var(--color-bg-body); color: var(--color-text-main);">
    <!-- 动态粒子背景 -->
    <ParticleBackground v-if="showDesktopEnhancements" />
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail v-if="showDesktopEnhancements" />
    
    <!-- 注意：Header 已移至 app.vue 全局挂载，此处不再需要 -->
    
    <!-- 主要内容区域 (无顶部内边距，用于沉浸式 Hero) -->
    <main class="flex-1 pt-0">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
    <!-- 风格切换面板 -->
    <ThemeSwitcher v-if="showUtilityWidgets" />
    
    <!-- AI 智能助手 -->
    <AIAssistant v-if="showDeferredWidgets" />
    
    <!-- 访客互动功能 -->
    <VisitorInteractionPanel v-if="showDeferredWidgets" />
    
    <!-- 访客互动式玩法（包含在抽屉中） -->
    <VisitorBehaviorListener v-if="showDesktopEnhancements" />
    <!-- 访客侧边栏抽屉（包含留言、互动等功能） -->
    <VisitorSidebarDrawer v-if="showDesktopEnhancements" />
    
    <!-- 隐秘的后台入口 -->
    <SecretAdminAccess />
  </div>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent, onMounted, onUnmounted, ref } from 'vue'

// 显式导入组件（确保 Nuxt 3 自动导入正常工作）
// 注意：Header 已移至 app.vue 全局挂载，此处不再需要导入
import Footer from '~/components/layout/Footer.vue'
import SecretAdminAccess from '~/components/admin/SecretAdminAccess.vue'

const ParticleBackground = defineAsyncComponent(() => import('~/components/effects/ParticleBackground.vue'))
const MouseTrail = defineAsyncComponent(() => import('~/components/effects/MouseTrail.vue'))
const ThemeSwitcher = defineAsyncComponent(() => import('~/components/layout/ThemeSwitcher.vue'))
const AIAssistant = defineAsyncComponent(() => import('~/components/ai/AIAssistant.vue'))
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

