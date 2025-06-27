<template>
  <header class="bg-white/80 backdrop-blur-md border-b border-gray-200 sticky top-0 z-50">
    <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <!-- Logo 区域 -->
        <NuxtLink to="/" class="flex items-center space-x-3 group">
          <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center shadow-sm group-hover:shadow-md transition-shadow">
            <span class="text-white text-lg font-bold">XF</span>
          </div>
          <div class="hidden sm:block">
            <span class="text-xl font-bold text-gray-800">溪午听风</span>
            <p class="text-xs text-gray-500 -mt-1">开发让生活更高效</p>
          </div>
        </NuxtLink>

        <!-- 桌面端导航菜单 -->
        <nav class="hidden md:flex items-center space-x-8">
          <NuxtLink
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            class="relative text-gray-600 hover:text-gray-900 font-medium transition-colors duration-200 py-2 px-1"
            :class="{ 'text-blue-600': $route.path === item.path }"
          >
            <span class="flex items-center space-x-1">
              <span class="text-sm">{{ item.icon }}</span>
              <span>{{ item.title }}</span>
            </span>
            <!-- 活跃状态下划线 -->
            <div
              v-if="$route.path === item.path"
              class="absolute bottom-0 left-0 right-0 h-0.5 bg-blue-600 rounded-full"
            ></div>
          </NuxtLink>
        </nav>

        <!-- 移动端菜单按钮 -->
        <button
          @click="toggleMobileMenu"
          class="md:hidden p-2 rounded-lg hover:bg-gray-100 transition-colors"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 移动端菜单 -->
      <div
        v-if="isMobileMenuOpen"
        class="md:hidden py-4 border-t border-gray-200 bg-white"
      >
        <nav class="space-y-2">
          <NuxtLink
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 text-gray-600 hover:text-gray-900 hover:bg-gray-50 rounded-lg transition-colors"
            :class="{ 'text-blue-600 bg-blue-50': $route.path === item.path }"
          >
            <span class="text-lg">{{ item.icon }}</span>
            <span class="font-medium">{{ item.title }}</span>
          </NuxtLink>
        </nav>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref } from 'vue'

// 移动端菜单状态
const isMobileMenuOpen = ref(false)

// 导航项配置
const navigationItems = [
  {
    title: '首页',
    path: '/',
    icon: '🏠'
  },
  {
    title: '插件工具',
    path: '/tools',
    icon: '🔧'
  },
  {
    title: '项目展示',
    path: '/projects',
    icon: '🧪'
  },
  {
    title: '技术博客',
    path: '/blog',
    icon: '📝'
  },
  {
    title: '关于我',
    path: '/about',
    icon: '👤'
  }
]

// 切换移动端菜单
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

// 关闭移动端菜单
const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}
</script> 