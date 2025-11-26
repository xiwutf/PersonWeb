<template>
  <div class="min-h-screen flex flex-col bg-[#050505] text-white">
    <!-- 头部导航 -->
    <Header />
    
    <!-- 主要内容区域 -->
    <main class="flex-1 pt-24">
      <slot />
    </main>
    
    <!-- 页脚 -->
    <Footer />
  </div>
</template>

<script setup>
// 记录访问量逻辑 (与 default 布局保持一致)
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
