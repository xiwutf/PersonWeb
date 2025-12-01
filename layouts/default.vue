<template>
  <!-- 最外层容器：使用主题背景色和文字颜色 -->
  <div class="min-h-screen flex flex-col relative bg-bg-body text-text-main">
    <!-- 动态背景效果（根据主题切换） -->
    <!-- 🔴 已禁用：可能导致滚动条闪烁（requestAnimationFrame 持续动画） -->
    <!-- <BackgroundEffects :effect="currentBackground" :config="backgroundConfig" /> -->
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail />
    
    <!-- 风格切换面板 -->
    <ThemeSwitcher />
    
    <!-- 头部导航 -->
    <Header />
    
    <!-- 主要内容区域 -->
    <main class="flex-1 pt-24 relative z-10">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
    <!-- AI 智能助手（独立，不放入抽屉） -->
    <AIAssistant />
    
    <!-- 访客互动功能 -->
    <!-- 🔴 已禁用：z-index 过高，可能遮挡导航栏 -->
    <!-- VisitorDanmakuWall: z-index: 100 (和导航栏同级) -->
    <!-- <VisitorDanmakuWall /> -->
    <!-- VisitorBubble: z-index: 200 (比导航栏高) -->
    <!-- <VisitorBubble /> -->
    <!-- VisitorInteractionPanel: 留言、互动功能（右下角） -->
    <VisitorInteractionPanel />
    
    <!-- 访客互动式玩法（包含在抽屉中） -->
    <VisitorBehaviorListener />
    <!-- 访客侧边栏抽屉（包含留言、互动等功能） -->
    <VisitorSidebarDrawer />
    <!-- 🔴 已禁用：可能导致滚动条闪烁（fixed 定位 + 动画） -->
    <!-- <VisitorTriggerEffects /> -->
    <!-- 🔴 已禁用：可能导致滚动条闪烁（requestAnimationFrame 持续动画） -->
    <!-- <FireworksEffect /> -->
    
    <!-- 隐秘的后台入口 -->
    <SecretAdminAccess />
  </div>
</template>

<script setup lang="ts">
// 导入组件（Nuxt 3 会自动导入 components 目录下的组件）

// 使用主题组合式函数
const { currentTheme } = useTheme()
const route = useRoute()
const api = useApi()

// 当前背景效果
const currentBackground = ref<string>('particles')
const backgroundConfig = ref<Record<string, any>>({})

// 监听主题和背景切换事件
onMounted(async () => {
  try {
    // 主题初始化由 useTheme composable 自动处理，无需手动调用

    // 获取或生成 Visitor ID
    // 注意：访问追踪由 plugins/analytics.client.ts 统一处理，这里不再重复调用
    // 该插件会调用 /api/Analytics/track 接口，同时写入 VisitLogs 和 VisitorAnalytics 表
    let visitorId = localStorage.getItem('visitor_id')
    if (!visitorId) {
      visitorId = `visitor_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
      localStorage.setItem('visitor_id', visitorId)
      
      // 触发新访客事件（用于显示气泡）
      if (process.client) {
        window.dispatchEvent(new CustomEvent('new-visitor', {
          detail: { visitorId, location: null }
        }))
      }
    }

    // 加载用户主题偏好
    try {
      const preference = await api.post('/Theme/preference', { visitorId: visitorId || 'anonymous' })
      if (preference) {
        currentBackground.value = preference.backgroundEffectCode || 'particles'
      }
    } catch (e) {
      console.warn('加载主题偏好失败', e)
    }
  } catch (e) {
    console.error('Failed to update stats', e)
  }

  // 监听背景切换事件
  window.addEventListener('background-changed', ((e: CustomEvent) => {
    currentBackground.value = e.detail.background
  }) as EventListener)
})
</script>
