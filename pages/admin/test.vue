<template>
  <div style="padding: 50px; color: red; background: white; min-height: 100vh;">
    <h1>测试页面 - /admin/test</h1>
    <p>如果你能看到这个页面，说明 Nuxt 路由工作正常</p>
    <p>当前路由: {{ route.path }}</p>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()

console.log('[Test Page] 已加载, 路由:', route.path)
</script>
