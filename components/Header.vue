<template>
  <header class="fixed top-4 left-0 right-0 z-50 px-4 sm:px-6 lg:px-8">
    <div class="glass max-w-7xl mx-auto rounded-2xl transition-all duration-300">
      <div class="flex justify-between items-center h-16 px-4">
        <!-- Logo 区域 - 添加隐秘后台入口 -->
        <div class="flex items-center space-x-3 group shrink-0">
          <div 
            @click="handleLogoClick"
            class="w-10 h-10 rounded-lg overflow-hidden shadow-sm group-hover:shadow-md transition-all duration-300 border border-gray-200 group-hover:scale-105 cursor-pointer"
            title=""
          >
            <img src="/images/avatar.jpg" alt="溪午听风" class="w-full h-full object-cover" />
          </div>
          <NuxtLink to="/" class="hidden lg:block">
            <span class="text-xl font-bold text-gray-800">溪午听风</span>
          </NuxtLink>
        </div>

        <!-- 桌面端导航菜单 -->
        <nav class="hidden md:flex items-center space-x-1">
          <!-- 主要导航项 -->
          <NuxtLink
            v-for="item in mainNavigationItems"
            :key="item.path"
            :to="item.path"
            class="relative px-3 py-2 rounded-xl text-sm font-medium transition-all duration-300 hover:bg-gray-100/50 hover:text-primary-600 whitespace-nowrap"
            :class="{ 'bg-primary-50 text-primary-600 shadow-sm': route.path === item.path, 'text-gray-600': route.path !== item.path }"
          >
            <span class="flex items-center space-x-1.5">
              <span>{{ item.icon }}</span>
              <span>{{ item.title }}</span>
            </span>
          </NuxtLink>
          
          <!-- 更多菜单下拉 -->
          <div class="relative group">
            <button
              class="relative px-3 py-2 rounded-xl text-sm font-medium transition-all duration-300 hover:bg-gray-100/50 hover:text-primary-600 whitespace-nowrap text-gray-600"
              :class="{ 'bg-primary-50 text-primary-600 shadow-sm': isMoreMenuActive }"
            >
              <span class="flex items-center space-x-1.5">
                <span>⋯</span>
                <span>更多</span>
                <svg class="w-4 h-4 transition-transform group-hover:rotate-180" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
                </svg>
              </span>
            </button>
            
            <!-- 下拉菜单 -->
            <div class="absolute right-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50">
              <div class="py-2">
                <NuxtLink
                  v-for="item in moreNavigationItems"
                  :key="item.path"
                  :to="item.path"
                  class="flex items-center space-x-3 px-4 py-2.5 text-sm text-gray-600 hover:bg-gray-50 hover:text-primary-600 transition-colors"
                  :class="{ 'bg-primary-50 text-primary-600': route.path === item.path }"
                >
                  <span class="text-base">{{ item.icon }}</span>
                  <span>{{ item.title }}</span>
                </NuxtLink>
              </div>
            </div>
          </div>
          
          <!-- 搜索按钮 -->
          <NuxtLink
            to="/search"
            class="flex items-center justify-center w-9 h-9 rounded-xl text-gray-600 hover:text-primary-600 hover:bg-primary-50 transition-all duration-300 ml-1"
            :class="{ 'text-primary-600 bg-primary-50 shadow-sm': route.path === '/search' }"
            title="搜索"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </NuxtLink>

          <!-- 主题切换 -->
          <div class="ml-1">
            <ThemeToggle />
          </div>
        </nav>

        <!-- 移动端菜单按钮 -->
        <button
          @click="toggleMobileMenu"
          class="md:hidden p-2 rounded-xl hover:bg-gray-100 active:bg-gray-200 transition-colors touch-target"
          aria-label="打开菜单"
        >
          <svg class="w-6 h-6 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 移动端菜单 -->
      <div
        v-show="isMobileMenuOpen"
        class="md:hidden border-t border-gray-100 max-h-[calc(100vh-4rem)] overflow-y-auto smooth-scroll"
      >
        <nav class="p-2 space-y-1">
          <NuxtLink
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 rounded-xl transition-all duration-200 touch-target active:bg-gray-100"
            :class="{ 'bg-primary-50 text-primary-600': route.path === item.path, 'text-gray-600 hover:bg-gray-50': route.path !== item.path }"
          >
            <span class="text-lg">{{ item.icon }}</span>
            <span class="font-medium">{{ item.title }}</span>
          </NuxtLink>
          
          <!-- 移动端搜索 -->
          <NuxtLink
            to="/search"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 rounded-xl transition-all duration-200"
            :class="{ 'bg-primary-50 text-primary-600': route.path === '/search', 'text-gray-600 hover:bg-gray-50': route.path !== '/search' }"
          >
            <span class="text-lg">🔍</span>
            <span class="font-medium">搜索</span>
          </NuxtLink>
        </nav>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const router = useRouter()
const route = useRoute()

// 移动端菜单状态
const isMobileMenuOpen = ref(false)

// 隐秘后台入口 - Logo 点击计数
let logoClickCount = 0
let logoClickTimer: NodeJS.Timeout | null = null
const SECRET_CLICKS = 5 // 需要点击5次
const SECRET_TIMEOUT = 3000 // 3秒内完成

const handleLogoClick = (e: MouseEvent) => {
  // 如果点击的是链接部分，不处理
  if ((e.target as HTMLElement).closest('a')) {
    return
  }
  
  logoClickCount++
  
  // 清除之前的定时器
  if (logoClickTimer) {
    clearTimeout(logoClickTimer)
  }
  
  // 如果达到指定次数，跳转到后台
  if (logoClickCount >= SECRET_CLICKS) {
    router.push('/admin/login')
    logoClickCount = 0
    return
  }
  
  // 设置定时器，超时重置计数
  logoClickTimer = setTimeout(() => {
    logoClickCount = 0
  }, SECRET_TIMEOUT)
}

// 键盘快捷键：Ctrl+Shift+A 或 Ctrl+K
onMounted(() => {
  const handleKeyPress = (e: KeyboardEvent) => {
    // Ctrl+Shift+A 或 Ctrl+K
    if ((e.ctrlKey && e.shiftKey && e.key === 'A') || (e.ctrlKey && e.key === 'k')) {
      e.preventDefault()
      router.push('/admin/login')
    }
  }
  
  window.addEventListener('keydown', handleKeyPress)
  
  onUnmounted(() => {
    window.removeEventListener('keydown', handleKeyPress)
    if (logoClickTimer) {
      clearTimeout(logoClickTimer)
    }
  })
})

// 主要导航项（显示在顶部）
const mainNavigationItems = [
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
    title: 'AI 实验室',
    path: '/ai',
    icon: '🔮'
  }
]

// 更多菜单项（合并到下拉菜单）
const moreNavigationItems = [
  {
    title: '生活随笔',
    path: '/life',
    icon: '☕'
  },
  {
    title: '英语学习',
    path: '/english',
    icon: '📚'
  },
  {
    title: '技能树',
    path: '/skills',
    icon: '🌳'
  },
  {
    title: '仪表盘',
    path: '/dashboard',
    icon: '⚡'
  },
  {
    title: '小游戏',
    path: '/game',
    icon: '🎮'
  },
  {
    title: '关于我',
    path: '/about',
    icon: '👤'
  }
]

// 所有导航项（用于移动端菜单）
const navigationItems = [...mainNavigationItems, ...moreNavigationItems]

// 检查"更多"菜单是否包含当前路由
const isMoreMenuActive = computed(() => {
  return moreNavigationItems.some(item => route.path === item.path)
})

// 关闭移动端菜单
const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

// 切换移动端菜单
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}
</script>
