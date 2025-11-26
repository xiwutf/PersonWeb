<template>
  <header class="fixed top-4 left-0 right-0 z-50 px-4 sm:px-6 lg:px-8">
    <div class="glass max-w-6xl mx-auto rounded-2xl transition-all duration-300">
      <div class="flex justify-between items-center h-16 px-4">
        <!-- Logo 区域 -->
        <NuxtLink to="/" class="flex items-center space-x-3 group">
          <div class="w-10 h-10 rounded-lg overflow-hidden shadow-sm group-hover:shadow-md transition-all duration-300 border border-gray-200 group-hover:scale-105">
            <img src="/images/avatar.jpg" alt="溪午听风" class="w-full h-full object-cover" />
          </div>
          <div class="hidden sm:block">
            <span class="text-xl font-bold text-gray-800 font-['Outfit']">溪午听风</span>
          </div>
        </NuxtLink>

        <!-- 桌面端导航菜单 -->
        <nav class="hidden md:flex items-center space-x-2">
          <NuxtLink
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            class="relative px-4 py-2 rounded-xl text-sm font-medium transition-all duration-300 hover:bg-gray-100/50 hover:text-blue-600"
            :class="{ 'bg-blue-50 text-blue-600 shadow-sm': $route.path === item.path, 'text-gray-600': $route.path !== item.path }"
          >
            <span class="flex items-center space-x-1.5">
              <span>{{ item.icon }}</span>
              <span>{{ item.title }}</span>
            </span>
          </NuxtLink>
          
          <!-- 搜索按钮 -->
          <NuxtLink
            to="/search"
            class="flex items-center justify-center w-10 h-10 rounded-xl text-gray-600 hover:text-blue-600 hover:bg-blue-50 transition-all duration-300 ml-2"
            :class="{ 'text-blue-600 bg-blue-50 shadow-sm': $route.path === '/search' }"
            title="搜索"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </NuxtLink>

          <!-- 主题切换 -->
          <ThemeToggle />
        </nav>

        <!-- 移动端菜单按钮 -->
        <button
          @click="toggleMobileMenu"
          class="md:hidden p-2 rounded-xl hover:bg-gray-100 transition-colors"
        >
          <svg class="w-6 h-6 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 移动端菜单 -->
      <div
        v-show="isMobileMenuOpen"
        class="md:hidden border-t border-gray-100"
      >
        <nav class="p-2 space-y-1">
          <NuxtLink
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 rounded-xl transition-all duration-200"
            :class="{ 'bg-blue-50 text-blue-600': $route.path === item.path, 'text-gray-600 hover:bg-gray-50': $route.path !== item.path }"
          >
            <span class="text-lg">{{ item.icon }}</span>
            <span class="font-medium">{{ item.title }}</span>
          </NuxtLink>
          
          <!-- 移动端搜索 -->
          <NuxtLink
            to="/search"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 rounded-xl transition-all duration-200"
            :class="{ 'bg-blue-50 text-blue-600': $route.path === '/search', 'text-gray-600 hover:bg-gray-50': $route.path !== '/search' }"
          >
            <span class="text-lg">🔍</span>
            <span class="font-medium">搜索</span>
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
    title: '生活随笔',
    path: '/life',
    icon: '☕'
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