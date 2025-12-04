<template>
  <!-- 
    Nuxt 3 应用根组件
    顶部导航栏在此全局挂载，确保所有页面（无论使用什么 layout）都显示导航栏
    注意：只有明确设置 layout: false 的页面（如登录页）才会不显示导航栏
  -->
  <div class="min-h-screen">
    <!-- 顶部导航栏，全站统一（排除后台登录页和特殊全屏页面） -->
    <Header v-if="shouldShowHeader" />
    
    <!-- 页面主体区域 -->
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import Header from '~/components/layout/Header.vue'

const route = useRoute()

// 判断是否应该显示顶部导航栏
// 不显示导航栏的页面：登录页、后台访客统计页等特殊页面
const shouldShowHeader = computed(() => {
  const path = route.path
  // 登录页不显示导航栏
  if (path === '/admin/login') {
    return false
  }
  // 后台访客统计页（全屏页面）不显示导航栏
  if (path === '/admin/visitors') {
    return false
  }
  // 其他所有页面都显示导航栏
  return true
})
</script>

