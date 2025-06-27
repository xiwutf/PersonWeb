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
</script> 