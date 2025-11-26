<template>
  <div class="min-h-screen flex flex-col">
    <!-- 头部导航 -->
    <Header />
    
    <!-- 主要内容区域 -->
    <main class="flex-1">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
  </div>
</template>

<script setup>
// 导入组件（Nuxt 3 会自动导入 components 目录下的组件）

// 记录访问量
onMounted(async () => {
  try {
    // 使用 localStorage 防止重复计数（简单的防刷机制）
    const lastVisit = localStorage.getItem('lastVisit')
    const today = new Date().toISOString().split('T')[0]
    
    if (lastVisit !== today) {
      await $fetch('/api/stats', { method: 'POST' })
      localStorage.setItem('lastVisit', today)
    }
  } catch (e) {
    console.error('Failed to update stats', e)
  }
})
</script>