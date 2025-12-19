<template>
  <!-- 
    AI 实验室布局（ai.vue）
    用途：AI 相关页面的专用布局，包含顶部导航栏
    使用场景：AI 实验室首页 (/ai)、AI 相关详情页 (/ai/[type]/[slug])
    特点：主内容区域使用 pt-24（顶部内边距）
  -->
  <!-- 使用 AppNaiveConfig 统一管理 Naive UI 主题配置 -->
  <AppNaiveConfig>
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
    <MouseTrail />
    
    <!-- 风格切换面板 -->
    <ThemeSwitcher />
    
    <!-- AI 智能助手 -->
    <AIAssistant />
    
    <!-- 访客互动功能 -->
    <VisitorInteractionPanel />
    
    <!-- 访客互动式玩法（包含在抽屉中） -->
    <VisitorBehaviorListener />
    <!-- 访客侧边栏抽屉（包含留言、互动等功能） -->
    <VisitorSidebarDrawer />
    
    <!-- 智能客服（前台访客使用） -->
    <SupportChat />
    </div>
  </AppNaiveConfig>
</template>

<script setup lang="ts">
// 显式导入组件，确保 Nuxt 3 自动导入正常工作
import AppNaiveConfig from '~/components/layout/AppNaiveConfig.vue'
import MouseTrail from '~/components/effects/MouseTrail.vue'
import ThemeSwitcher from '~/components/layout/ThemeSwitcher.vue'
import Footer from '~/components/layout/Footer.vue'
import AIAssistant from '~/components/ai/AIAssistant.vue'
import VisitorInteractionPanel from '~/components/VisitorInteractionPanel.vue'
import VisitorBehaviorListener from '~/components/VisitorBehaviorListener.vue'
import VisitorSidebarDrawer from '~/components/VisitorSidebarDrawer.vue'
import SupportChat from '~/components/ai/SupportChat.vue'

const route = useRoute()
const api = useApi()

// 记录访问量
onMounted(async () => {
  try {
    // 获取或生成 Visitor ID
    let visitorId = localStorage.getItem('visitor_id')
    
    // 调用统计 API
    const res = await api.post('/tracking/visit', { 
      visitorId,
      path: route.path 
    })

    // 保存 Visitor ID (如果是新生成的)
    if (res && res.visitorId && res.visitorId !== visitorId) {
      localStorage.setItem('visitor_id', res.visitorId)
    }
  } catch (e) {
    console.error('Failed to update stats', e)
  }
})
</script>

