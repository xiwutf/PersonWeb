<template>
  <!-- AI 布局：使用主题背景色和文字颜色，替换写死的深色背景 -->
  <div class="min-h-screen flex flex-col bg-bg-body text-text-main">
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

<script setup lang="ts">
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

