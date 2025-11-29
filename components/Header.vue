<template>
  <header class="fixed top-4 left-0 right-0 z-50 px-4 sm:px-6 lg:px-8 pointer-events-none">
    <div class="glass max-w-7xl mx-auto rounded-2xl transition-all duration-300 pointer-events-auto">
      <div class="flex justify-between items-center h-16 px-4">
        <!-- Logo 区域 - 添加隐秘后台入口 -->
        <div class="flex items-center space-x-3 group shrink-0">
          <div 
            @click="handleLogoClick"
            @mouseenter="handleAvatarHover"
            class="w-10 h-10 rounded-lg overflow-hidden shadow-sm group-hover:shadow-md transition-all duration-300 border border-gray-200 group-hover:scale-105 cursor-pointer avatar-trigger"
            title=""
          >
            <img src="/images/avatar.jpg" alt="溪午听风" class="w-full h-full object-cover" />
          </div>
          <NuxtLink to="/" class="hidden lg:block">
            <!-- Logo 文本：使用主题文字颜色，替换写死的 gray-800 -->
            <span class="text-xl font-bold text-text-main">溪午听风</span>
          </NuxtLink>
        </div>

        <!-- 桌面端导航菜单 -->
        <nav class="hidden md:flex items-center space-x-1">
          <!-- 主要导航项 -->
          <NuxtLink
            v-for="item in mainNavigationItems"
            :key="item.path"
            :to="item.path"
            class="header-nav-item"
            :class="{ 'header-nav-item-active': route.path === item.path, 'header-nav-item-inactive': route.path !== item.path }"
          >
            <span class="header-nav-item-content">
              <span class="header-nav-item-icon">{{ item.icon }}</span>
              <span class="header-nav-item-title">{{ item.title }}</span>
            </span>
          </NuxtLink>
          
          <!-- 更多菜单下拉 -->
          <div class="relative group">
            <!-- 更多菜单按钮：使用主题颜色，替换写死的 gray 颜色 -->
            <button
              class="relative px-3 py-2 rounded-xl text-sm font-medium transition-all duration-300 hover:bg-primary-soft hover:text-primary whitespace-nowrap text-text-muted"
              :class="{ 'bg-primary-soft text-primary shadow-sm': isMoreMenuActive }"
            >
              <span class="flex items-center space-x-1.5">
                <span>⋯</span>
                <span>更多</span>
                <svg class="w-4 h-4 transition-transform group-hover:rotate-180" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
                </svg>
              </span>
            </button>
            
            <!-- 下拉菜单：使用主题背景和文字颜色，替换写死的 white 和 gray -->
            <div class="absolute right-0 top-full mt-2 w-48 bg-bg-card rounded-xl shadow-lg border border-border-subtle opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50">
              <div class="py-2">
                <NuxtLink
                  v-for="item in moreNavigationItems"
                  :key="item.path"
                  :to="item.path"
                  class="flex items-center space-x-3 px-4 py-2.5 text-sm text-text-muted hover:bg-primary-soft hover:text-primary transition-colors"
                  :class="{ 'bg-primary-soft text-primary': route.path === item.path }"
                >
                  <span class="text-base">{{ item.icon }}</span>
                  <span>{{ item.title }}</span>
                </NuxtLink>
              </div>
            </div>
          </div>
          
          <!-- 搜索按钮：使用主题颜色，替换写死的 gray -->
          <NuxtLink
            to="/search"
            class="flex items-center justify-center w-9 h-9 rounded-xl text-text-muted hover:text-primary hover:bg-primary-soft transition-all duration-300 ml-1"
            :class="{ 'text-primary bg-primary-soft shadow-sm': route.path === '/search' }"
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

        <!-- 移动端菜单按钮：使用主题颜色，替换写死的 gray -->
        <button
          @click="toggleMobileMenu"
          class="md:hidden p-2 rounded-xl hover:bg-primary-soft active:bg-primary-soft transition-colors touch-target"
          aria-label="打开菜单"
        >
          <svg class="w-6 h-6 text-text-muted" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 移动端菜单：使用主题颜色，替换写死的 gray -->
      <div
        v-show="isMobileMenuOpen"
        class="md:hidden border-t border-border-subtle max-h-[calc(100vh-4rem)] overflow-y-auto smooth-scroll"
      >
        <nav class="p-2 space-y-1">
          <NuxtLink
            v-for="item in navigationItems"
            :key="item.path"
            :to="item.path"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 rounded-xl transition-all duration-200 touch-target active:bg-primary-soft"
            :class="{ 'bg-primary-soft text-primary': route.path === item.path, 'text-text-muted hover:bg-primary-soft': route.path !== item.path }"
          >
            <span class="text-lg">{{ item.icon }}</span>
            <span class="font-medium">{{ item.title }}</span>
          </NuxtLink>
          
          <!-- 移动端搜索：使用主题颜色，替换写死的 gray -->
          <NuxtLink
            to="/search"
            @click="closeMobileMenu"
            class="flex items-center space-x-3 px-4 py-3 rounded-xl transition-all duration-200"
            :class="{ 'bg-primary-soft text-primary': route.path === '/search', 'text-text-muted hover:bg-primary-soft': route.path !== '/search' }"
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

const handleAvatarHover = () => {
  // 触发头像悬停行为记录
  if (process.client && (window as any).handleAvatarHover) {
    (window as any).handleAvatarHover()
  }
}

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

// 主要导航项（根据启用的模块动态生成）
const mainNavigationItems = computed(() => {
  const { isModuleEnabled } = useModuleSystem()
  
  const items = [
    {
      title: '首页',
      path: '/',
      icon: '🏠',
      moduleKey: 'core'
    }
  ]

  // 根据模块启用状态添加导航项
  if (isModuleEnabled('tools')) {
    items.push({
      title: '插件工具',
      path: '/tools',
      icon: '🔧',
      moduleKey: 'tools'
    })
  }

  if (isModuleEnabled('projects')) {
    items.push({
      title: '项目展示',
      path: '/projects',
      icon: '🧪',
      moduleKey: 'projects'
    })
  }

  if (isModuleEnabled('blog')) {
    items.push({
      title: '技术博客',
      path: '/blog',
      icon: '📝',
      moduleKey: 'blog'
    })
  }

  if (isModuleEnabled('lab-3d')) {
    items.push({
      title: 'AI 实验室',
      path: '/lab',
      icon: '🔮',
      moduleKey: 'lab-3d'
    })
  }

  return items
})

// 更多菜单项（根据启用的模块动态生成）
const moreNavigationItems = computed(() => {
  const { isModuleEnabled } = useModuleSystem()
  
  const items: Array<{ title: string; path: string; icon: string; moduleKey?: string }> = []

  if (isModuleEnabled('life')) {
    items.push({
      title: '生活随笔',
      path: '/life',
      icon: '☕',
      moduleKey: 'life'
    })
  }

  if (isModuleEnabled('english')) {
    items.push({
      title: '英语学习',
      path: '/english',
      icon: '📚',
      moduleKey: 'english'
    })
  }

  if (isModuleEnabled('skills')) {
    items.push({
      title: '技能树',
      path: '/skills',
      icon: '🌳',
      moduleKey: 'skills'
    })
  }

  if (isModuleEnabled('dashboard')) {
    items.push({
      title: '仪表盘',
      path: '/dashboard',
      icon: '⚡',
      moduleKey: 'dashboard'
    })
  }

  if (isModuleEnabled('game')) {
    items.push({
      title: '小游戏',
      path: '/game',
      icon: '🎮',
      moduleKey: 'game'
    })
  }

  // 关于我始终显示（核心功能）
  items.push({
    title: '关于我',
    path: '/about',
    icon: '👤',
    moduleKey: 'core'
  })

  return items
})

// 所有导航项（用于移动端菜单）
const navigationItems = computed(() => {
  return [...mainNavigationItems.value, ...moreNavigationItems.value]
})

// 检查"更多"菜单是否包含当前路由
const isMoreMenuActive = computed(() => {
  return moreNavigationItems.value.some(item => route.path === item.path)
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

<style scoped>
/* 导航项样式 */
.header-nav-item {
  position: relative;
  padding: 0.5rem 0.75rem;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.3s ease;
  white-space: nowrap;
  display: inline-block;
  text-decoration: none;
  min-width: fit-content;
}

/* 导航项样式：使用主题颜色，替换写死的颜色值 */
.header-nav-item:hover {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

.header-nav-item-active {
  background: var(--color-primary-soft);
  color: var(--color-primary);
  box-shadow: var(--shadow-sm);
}

.header-nav-item-inactive {
  color: var(--color-text-muted);
}

.header-nav-item-content {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  flex-direction: row;
  writing-mode: horizontal-tb;
  text-orientation: mixed;
}

.header-nav-item-icon {
  display: inline-block;
  line-height: 1;
}

.header-nav-item-title {
  display: inline-block;
  line-height: 1.5;
  writing-mode: horizontal-tb;
  text-orientation: mixed;
}
</style>
