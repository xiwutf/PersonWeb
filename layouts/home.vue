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
    <main class="flex-1">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
    
    <!-- AI 智能助手 -->
    <AIAssistant />
    
    <!-- 时间胶囊墙 -->
    <TimeCapsuleWall />
    
    <!-- 隐秘的后台入口 -->
    <SecretAdminAccess />
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const api = useApi()

// 注意：访问追踪由 plugins/analytics.client.ts 统一处理
// 该插件会调用 /api/Analytics/track 接口，同时写入 VisitLogs 和 VisitorAnalytics 表
// 这里不再需要单独调用统计 API
</script>

