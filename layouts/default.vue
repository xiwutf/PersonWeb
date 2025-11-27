<template>
  <div class="min-h-screen flex flex-col">
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
// 导入组件（Nuxt 3 会自动导入 components 目录下的组件）

// 使用主题组合式函数
const { updateTheme } = useTheme()

<template>
  <div class="min-h-screen flex flex-col">
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
// 导入组件（Nuxt 3 会自动导入 components 目录下的组件）

// 使用主题组合式函数
const { updateTheme } = useTheme()

// 记录访问量
onMounted(async () => {
  try {
    // 初始化主题
    updateTheme()

    // 调用统计 API (由服务器处理去重和记录逻辑)
    await $fetch('/api/stats', { method: 'POST' })
  } catch (e) {
    console.error('Failed to update stats', e)
  }
})
</script>