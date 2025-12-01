<template>
  <header class="fixed top-4 left-1/2 -translate-x-1/2 z-[1000] floating-nav" style="z-index: 1000 !important;">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-14">
        <!-- Logo 区域 -->
        <NuxtLink to="/" class="flex items-center space-x-3 group shrink-0 cursor-pointer">
          <div 
            @click.stop="handleLogoClick"
            @mouseenter="handleAvatarHover"
            class="w-10 h-10 rounded-lg overflow-hidden shadow-sm group-hover:shadow-md transition-all duration-300 border border-border-subtle group-hover:scale-105 cursor-pointer"
            title="溪午听风"
          >
            <img src="/images/avatar.jpg" alt="溪午听风" class="w-full h-full object-cover" />
          </div>
          <span class="text-xl font-bold text-text-main hidden sm:block">溪午听风</span>
        </NuxtLink>

        <!-- 桌面端导航菜单 -->
        <nav class="hidden md:flex items-center space-x-1">
          <NuxtLink
            v-for="item in mainNavigationItems"
            :key="item.path"
            :to="item.path"
            class="px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200 cursor-pointer"
            :class="route.path === item.path 
              ? 'bg-primary-soft text-primary shadow-sm' 
              : 'text-text-muted hover:bg-bg-elevated hover:text-text-main'"
          >
            <span class="mr-1.5">{{ item.icon }}</span>
            <span>{{ item.title }}</span>
          </NuxtLink>
          
          <!-- 更多菜单 -->
          <div class="relative">
            <button
              @click="toggleMoreMenu"
              class="px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200 cursor-pointer flex items-center space-x-1.5"
              :class="isMoreMenuOpen || isMoreMenuActive
                ? 'bg-primary-soft text-primary shadow-sm'
                : 'text-text-muted hover:bg-bg-elevated hover:text-text-main'"
            >
              <span>更多</span>
              <svg class="w-4 h-4 transition-transform" :class="{ 'rotate-180': isMoreMenuOpen }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </button>
            
            <Transition name="dropdown">
              <div v-if="isMoreMenuOpen" class="absolute right-0 top-full mt-2 w-48 bg-bg-card rounded-xl shadow-lg border border-border-subtle z-50 overflow-hidden">
                <div class="py-1">
                  <NuxtLink
                    v-for="item in moreNavigationItems"
                    :key="item.path"
                    :to="item.path"
                    @click="closeMoreMenu"
                    class="flex items-center space-x-3 px-4 py-2.5 text-sm transition-colors cursor-pointer"
                    :class="route.path === item.path
                      ? 'bg-primary-soft text-primary'
                      : 'text-text-muted hover:bg-bg-elevated hover:text-text-main'"
                  >
                    <span class="text-base">{{ item.icon }}</span>
                    <span>{{ item.title }}</span>
                  </NuxtLink>
                </div>
              </div>
            </Transition>
          </div>
          
          <!-- 搜索按钮 -->
          <NuxtLink
            to="/search"
            class="ml-2 p-2 rounded-lg text-text-muted hover:text-primary hover:bg-primary-soft transition-all duration-200 cursor-pointer"
            :class="{ 'text-primary bg-primary-soft': route.path === '/search' }"
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
          class="md:hidden p-2 rounded-lg hover:bg-bg-elevated transition-colors cursor-pointer"
          aria-label="打开菜单"
        >
          <svg class="w-6 h-6 text-text-muted" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 移动端菜单 -->
      <Transition name="slide-down">
        <div v-if="isMobileMenuOpen" class="md:hidden border-t border-border-subtle/30 py-2 mt-2">
          <nav class="space-y-1">
            <NuxtLink
              v-for="item in navigationItems"
              :key="item.path"
              :to="item.path"
              @click="closeMobileMenu"
              class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-all duration-200 cursor-pointer"
              :class="route.path === item.path
                ? 'bg-primary-soft text-primary'
                : 'text-text-muted hover:bg-bg-elevated hover:text-text-main'"
            >
              <span class="text-lg">{{ item.icon }}</span>
              <span class="font-medium">{{ item.title }}</span>
            </NuxtLink>
            
            <NuxtLink
              to="/search"
              @click="closeMobileMenu"
              class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-all duration-200 cursor-pointer text-text-muted hover:bg-bg-elevated hover:text-text-main"
            >
              <span class="text-lg">🔍</span>
              <span class="font-medium">搜索</span>
            </NuxtLink>
          </nav>
        </div>
      </Transition>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

// Nuxt 3 自动导入
// @ts-ignore - Nuxt 3 auto-imports
const router = useRouter()
// @ts-ignore - Nuxt 3 auto-imports
const route = useRoute()

// 移动端菜单状态
const isMobileMenuOpen = ref(false)

// 更多菜单状态
const isMoreMenuOpen = ref(false)

// 隐秘后台入口 - Logo 点击计数
let logoClickCount = 0
let logoClickTimer: NodeJS.Timeout | null = null
const SECRET_CLICKS = 5
const SECRET_TIMEOUT = 3000

const handleAvatarHover = () => {
  if (process.client && (window as any).handleAvatarHover) {
    (window as any).handleAvatarHover()
  }
}

const handleLogoClick = (e: MouseEvent) => {
  e.preventDefault()
  e.stopPropagation()
  
  logoClickCount++
  
  if (logoClickTimer) {
    clearTimeout(logoClickTimer)
  }
  
  if (logoClickCount >= SECRET_CLICKS) {
    router.push('/admin/login')
    logoClickCount = 0
    return
  }
  
  logoClickTimer = setTimeout(() => {
    logoClickCount = 0
  }, SECRET_TIMEOUT)
}

// 键盘快捷键
onMounted(() => {
  const handleKeyPress = (e: KeyboardEvent) => {
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

// 主要导航项
const mainNavigationItems = computed(() => {
  // @ts-ignore - Nuxt 3 auto-imports
  const { isModuleEnabled } = useModuleSystem()
  
  const items = [
    {
      title: '首页',
      path: '/',
      icon: '🏠',
      moduleKey: 'core'
    }
  ]

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

// 更多菜单项
const moreNavigationItems = computed(() => {
  // @ts-ignore - Nuxt 3 auto-imports
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

// 切换更多菜单
const toggleMoreMenu = () => {
  isMoreMenuOpen.value = !isMoreMenuOpen.value
}

// 关闭更多菜单
const closeMoreMenu = () => {
  isMoreMenuOpen.value = false
}

// 点击外部关闭更多菜单
onMounted(() => {
  const handleClickOutside = (e: MouseEvent) => {
    const target = e.target as HTMLElement
    if (!target.closest('.relative')) {
      isMoreMenuOpen.value = false
    }
  }
  if (process.client) {
    document.addEventListener('click', handleClickOutside)
    onUnmounted(() => {
      document.removeEventListener('click', handleClickOutside)
    })
  }
})
</script>

<style scoped>
/* 独立悬浮导航栏样式 */
.floating-nav {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 1rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05);
  width: calc(100% - 2rem);
  max-width: 1280px;
  transition: all 0.3s ease;
}

/* 暗色主题下的导航栏 */
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav {
  background: rgba(30, 41, 59, 0.9);
  border-color: rgba(255, 255, 255, 0.1);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3), 0 2px 8px rgba(0, 0, 0, 0.2);
}

/* 响应式：小屏幕时全宽 */
@media (max-width: 640px) {
  .floating-nav {
    width: calc(100% - 1rem);
    top: 0.5rem;
    border-radius: 0.75rem;
  }
}

/* 下拉菜单动画 */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* 移动端菜单动画 */
.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.3s ease;
}

.slide-down-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
