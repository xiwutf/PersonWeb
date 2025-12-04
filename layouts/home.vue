<template>
  <!-- 首页布局：使用主题背景色和文字颜色 -->
  <div class="min-h-screen flex flex-col relative bg-bg-body text-text-main">
    <!-- 动态粒子背景 -->
    <ParticleBackground />
    
    <!-- 鼠标轨迹特效 -->
    <MouseTrail />
    
    <!-- 头部导航 -->
    <Header />
    
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
import ParticleBackground from '~/components/effects/ParticleBackground.vue'
import MouseTrail from '~/components/effects/MouseTrail.vue'
import Header from '~/components/layout/Header.vue'
import Footer from '~/components/layout/Footer.vue'
import ThemeSwitcher from '~/components/layout/ThemeSwitcher.vue'
import SecretAdminAccess from '~/components/admin/SecretAdminAccess.vue'

const route = useRoute()
const api = useApi()

// 注意：访问追踪由 plugins/analytics.client.ts 统一处理
// 该插件会调用 /api/Analytics/track 接口，同时写入 VisitLogs 和 VisitorAnalytics 表
// 这里不再需要单独调用统计 API
</script>

