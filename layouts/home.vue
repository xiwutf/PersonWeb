<template>
  <!-- 
    首页布局（home.vue）
    用途：首页专用布局，包含顶部导航栏，但主内容区域无顶部内边距（用于沉浸式 Hero 效果）
    使用场景：首页 (/)
    特点：主内容区域使用 pt-0（无顶部内边距），允许 Hero 区域延伸到顶部，包含粒子背景特效
  -->
  <!-- 首页布局：使用主题背景色和文字颜色 -->
  <div class="min-h-screen flex flex-col relative bg-bg-body text-text-main">
    <!-- 动态粒子背景 -->
    <ParticleBackground />
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail />
    
    <!-- 注意：Header 已移至 app.vue 全局挂载，此处不再需要 -->
    
    <!-- 主要内容区域 (无顶部内边距，用于沉浸式 Hero) -->
    <main class="flex-1 pt-0">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
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
    
    <!-- 隐秘的后台入口 -->
    <SecretAdminAccess />
  </div>
</template>

<script setup lang="ts">
// 显式导入组件（确保 Nuxt 3 自动导入正常工作）
// 注意：Header 已移至 app.vue 全局挂载，此处不再需要导入
import ParticleBackground from '~/components/effects/ParticleBackground.vue'
import MouseTrail from '~/components/effects/MouseTrail.vue'
import Footer from '~/components/layout/Footer.vue'
import ThemeSwitcher from '~/components/layout/ThemeSwitcher.vue'
import AIAssistant from '~/components/ai/AIAssistant.vue'
import VisitorInteractionPanel from '~/components/VisitorInteractionPanel.vue'
import VisitorBehaviorListener from '~/components/VisitorBehaviorListener.vue'
import VisitorSidebarDrawer from '~/components/VisitorSidebarDrawer.vue'
import SecretAdminAccess from '~/components/admin/SecretAdminAccess.vue'

const route = useRoute()
const api = useApi()

// 注意：访问追踪由 plugins/analytics.client.ts 统一处理
// 该插件会调用 /api/Analytics/track 接口，同时写入 VisitLogs 和 VisitorAnalytics 表
// 这里不再需要单独调用统计 API
</script>

