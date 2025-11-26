<template>
  <div class="min-h-screen flex flex-col">
    <!-- 头部导航 -->
    <Header />
    
    <!-- 主要内容区域 (无顶部内边距，用于沉浸式 Hero) -->
    <main class="flex-1">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
  </div>
</template>

<script setup>
// 记录访问量
onMounted(async () => {
  try {
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
