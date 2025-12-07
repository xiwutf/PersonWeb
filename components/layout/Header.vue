<template>
  <header class="header-container floating-nav">
    <div class="header-content-wrapper">
      <div class="header-main">
        <!-- Logo 区域 -->
        <NuxtLink to="/" class="header-logo-link">
          <div 
            @click.stop="handleLogoClick"
            @mouseenter="handleAvatarHover"
            class="header-logo-avatar"
            title="溪午听风"
          >
            <img src="/images/avatar.jpg" alt="溪午听风" />
          </div>
          <span 
            class="header-logo-text logo-text"
          >溪午听风</span>
        </NuxtLink>

        <!-- 桌面端导航菜单 -->
        <nav class="header-nav-desktop">
          <NuxtLink
            v-for="item in mainNavigationItems"
            :key="item.path"
            :to="item.path"
            class="header-nav-link"
            :class="isActiveRoute(item.path)
              ? 'header-nav-link-active' 
              : 'header-nav-link-inactive'"
          >
            <span class="header-nav-link-icon">{{ item.icon }}</span>
            <span>{{ item.title }}</span>
          </NuxtLink>
          
          <!-- 更多菜单 -->
          <div class="relative">
            <button
              @click="toggleMoreMenu"
              class="header-more-menu-button"
              :class="isMoreMenuOpen || isMoreMenuActive
                ? 'header-more-menu-button-active'
                : 'header-more-menu-button-inactive'"
            >
              <span>更多</span>
              <svg class="header-more-menu-icon" :class="{ 'header-more-menu-icon-rotated': isMoreMenuOpen }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </button>
            
            <Transition name="dropdown">
              <div v-if="isMoreMenuOpen" class="header-dropdown-menu">
                <div class="header-dropdown-menu-content">
                  <NuxtLink
                    v-for="item in moreNavigationItems"
                    :key="item.path"
                    :to="item.path"
                    @click="closeMoreMenu"
                    class="header-dropdown-menu-item"
                    :class="route.path === item.path
                      ? 'header-dropdown-menu-item-active'
                      : 'header-dropdown-menu-item-inactive'"
                  >
                    <span class="header-dropdown-menu-item-icon">{{ item.icon }}</span>
                    <span>{{ item.title }}</span>
                  </NuxtLink>
                </div>
              </div>
            </Transition>
          </div>
          
          <!-- 搜索按钮 -->
          <NuxtLink
            to="/search"
            class="header-search-button"
            :class="{ 'header-search-button-active': route.path === '/search' }"
            title="搜索"
          >
            <svg class="header-search-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </NuxtLink>

          <!-- 主题切换 -->
          <div class="header-theme-toggle-container">
            <ThemeToggle />
          </div>
        </nav>

        <!-- 移动端菜单按钮 -->
        <button
          @click="toggleMobileMenu"
          class="header-mobile-menu-button"
          aria-label="打开菜单"
        >
          <svg class="header-mobile-menu-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 移动端菜单 -->
      <Transition name="slide-down">
        <div v-if="isMobileMenuOpen" class="header-mobile-menu">
          <nav class="header-mobile-menu-content">
            <NuxtLink
              v-for="item in navigationItems"
              :key="item.path"
              :to="item.path"
              @click="closeMobileMenu"
              class="header-mobile-menu-item"
              :class="route.path === item.path
                ? 'header-mobile-menu-item-active'
                : 'header-mobile-menu-item-inactive'"
            >
              <span class="header-dropdown-menu-item-icon">{{ item.icon }}</span>
              <span>{{ item.title }}</span>
            </NuxtLink>
            
            <NuxtLink
              to="/search"
              @click="closeMobileMenu"
              class="header-mobile-menu-item header-mobile-menu-item-inactive"
            >
              <span class="header-dropdown-menu-item-icon">🔍</span>
              <span>搜索</span>
            </NuxtLink>
          </nav>
        </div>
      </Transition>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
// 显式导入组件，确保 Nuxt 3 自动导入正常工作
import ThemeToggle from '~/components/layout/ThemeToggle.vue'

// Nuxt 3 自动导入
// @ts-ignore - Nuxt 3 auto-imports
const router = useRouter()
// @ts-ignore - Nuxt 3 auto-imports
const route = useRoute()

// 注意：Header 组件已在 app.vue 中全局挂载，确保所有页面都显示导航栏

// 检测是否为暗色主题
const isDark = computed(() => {
  if (process.client) {
    const html = document.documentElement
    return html.classList.contains('dark') || 
           html.dataset.theme === 'dark' || 
           html.dataset.theme === 'hybrid-super-dark'
  }
  return false
})

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

  // AI 实验室 / AI 服务入口（优先显示）
  items.push({
    title: 'AI / 智能体',
    path: '/ai',
    icon: '🤖',
    moduleKey: 'ai'
  })

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

  // 副业项目展示
  items.push({
    title: '副业项目',
    path: '/side-projects',
    icon: '💼',
    moduleKey: 'core'
  })

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

// 检查路由是否激活（精确匹配，首页特殊处理）
const isActiveRoute = (path: string) => {
  if (!route || !route.path) return false
  // 首页需要精确匹配
  if (path === '/') {
    return route.path === '/' || route.path === ''
  }
  // 其他路由精确匹配
  return route.path === path
}

// 检查"更多"菜单是否包含当前路由
const isMoreMenuActive = computed(() => {
  return moreNavigationItems.value.some(item => isActiveRoute(item.path))
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
    // 检查点击是否在下拉菜单或其父容器之外
    const dropdownMenu = document.querySelector('.header-dropdown-menu')
    const moreButton = document.querySelector('.header-more-menu-button')
    if (dropdownMenu && moreButton) {
      if (!dropdownMenu.contains(target) && !moreButton.contains(target)) {
        isMoreMenuOpen.value = false
      }
    } else if (!target.closest('.relative')) {
      isMoreMenuOpen.value = false
    }
  }
  if (process.client) {
    // 延迟添加事件监听器，避免立即触发
    setTimeout(() => {
      document.addEventListener('click', handleClickOutside)
    }, 100)
    onUnmounted(() => {
      document.removeEventListener('click', handleClickOutside)
    })
  }
})
</script>

<style scoped>
/* Header 容器样式 */
.header-container {
  position: fixed !important;
  top: 1rem !important;
  left: 50% !important;
  transform: translateX(-50%) !important;
  z-index: 10000 !important; /* 提高 z-index，确保在所有元素之上 */
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 独立悬浮导航栏样式 - 参考截图效果：浅灰色背景 */
.floating-nav {
  background: #e2e8f0 !important; /* 浅灰色背景（参考截图） */
  backdrop-filter: blur(20px) !important;
  -webkit-backdrop-filter: blur(20px) !important;
  border: 1px solid rgba(148, 163, 184, 0.3) !important;
  border-radius: 1rem !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05) !important;
  width: calc(100% - 2rem) !important;
  max-width: 1280px !important;
  transition: all 0.3s ease !important;
  color: #0f172a !important; /* 浅色主题下使用深色文字 */
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 暗色主题下的导航栏 - 深灰色背景 */
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav,
:global([data-theme="hybrid-super-dark"]) .floating-nav {
  background: #1e293b !important; /* 深灰色背景 */
  backdrop-filter: blur(20px) !important;
  -webkit-backdrop-filter: blur(20px) !important;
  border-color: rgba(148, 163, 184, 0.3) !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.6), 0 2px 8px rgba(0, 0, 0, 0.5) !important;
  color: #ffffff !important; /* 暗色主题下使用白色文字 */
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 确保暗色主题下导航栏内的所有文字清晰可见 - 使用纯白色 */
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav,
:global([data-theme="hybrid-super-dark"]) .floating-nav {
  color: #ffffff !important;
}

/* 左侧 Logo 文字 - 纯白色，无阴影，确保最高优先级 */
:global(.dark) .floating-nav .text-text-main,
:global([data-theme="dark"]) .floating-nav .text-text-main,
:global([data-theme="hybrid-super-dark"]) .floating-nav .text-text-main,
:global(.dark) .floating-nav a span.text-text-main,
:global([data-theme="dark"]) .floating-nav a span.text-text-main,
:global([data-theme="hybrid-super-dark"]) .floating-nav a span.text-text-main,
:global(.dark) .floating-nav .flex.items-center span,
:global([data-theme="dark"]) .floating-nav .flex.items-center span,
:global([data-theme="hybrid-super-dark"]) .floating-nav .flex.items-center span {
  color: #ffffff !important;
  font-weight: 800 !important;
  opacity: 1 !important;
}

/* 导航链接文字 - 使用清晰可见的颜色 */
/* 浅色主题下使用深灰色 */
.floating-nav .text-text-muted {
  color: #64748b !important;
}

/* 暗色主题下使用浅灰色 */
:global(.dark) .floating-nav .text-text-muted,
:global([data-theme="dark"]) .floating-nav .text-text-muted,
:global([data-theme="hybrid-super-dark"]) .floating-nav .text-text-muted {
  color: rgba(255, 255, 255, 0.7) !important;
}

/* 导航链接悬停状态 */
/* 浅色主题下悬停使用深色 */
.floating-nav a:hover {
  color: #0f172a !important;
}

/* 暗色主题下悬停使用白色 */
:global(.dark) .floating-nav a:hover,
:global([data-theme="dark"]) .floating-nav a:hover,
:global([data-theme="hybrid-super-dark"]) .floating-nav a:hover {
  color: #ffffff !important;
}

/* 搜索图标和主题切换图标 - 使用清晰可见的颜色 */
/* 浅色主题下使用深色 */
.floating-nav svg {
  color: #0f172a !important;
}

/* 暗色主题下使用白色 */
:global(.dark) .floating-nav svg,
:global([data-theme="dark"]) .floating-nav svg,
:global([data-theme="hybrid-super-dark"]) .floating-nav svg {
  color: #ffffff !important;
}

/* 更多菜单按钮文字 - 使用清晰可见的颜色 */
/* 浅色主题下使用深色 */
.floating-nav button {
  color: #0f172a !important;
}

/* 暗色主题下使用白色 */
:global(.dark) .floating-nav button,
:global([data-theme="dark"]) .floating-nav button,
:global([data-theme="hybrid-super-dark"]) .floating-nav button {
  color: #ffffff !important;
}

/* 移动端菜单按钮图标 - 使用清晰可见的颜色 */
/* 浅色主题下使用深色 */
.floating-nav .md\:hidden svg {
  color: #0f172a !important;
}

/* 暗色主题下使用白色 */
:global(.dark) .floating-nav .md\:hidden svg,
:global([data-theme="dark"]) .floating-nav .md\:hidden svg,
:global([data-theme="hybrid-super-dark"]) .floating-nav .md\:hidden svg {
  color: #ffffff !important;
}

/* 非激活状态的导航链接：浅灰色背景，浅灰色文字（参考截图效果） */
.floating-nav nav a:not(.header-nav-link-active) {
  background-color: rgba(148, 163, 184, 0.2) !important; /* 浅灰色背景 */
  color: #94a3b8 !important; /* 浅灰色文字 */
}

/* 暗色主题下非激活状态 */
:global(.dark) .floating-nav nav a:not(.header-nav-link-active),
:global([data-theme="dark"]) .floating-nav nav a:not(.header-nav-link-active),
:global([data-theme="hybrid-super-dark"]) .floating-nav nav a:not(.header-nav-link-active) {
  background-color: rgba(148, 163, 184, 0.15) !important; /* 浅灰色背景 */
  color: rgba(255, 255, 255, 0.6) !important; /* 浅灰色文字 */
}

/* 激活状态的导航链接：深蓝色背景，蓝色文字（参考截图效果） */
.floating-nav nav a.header-nav-link-active {
  background-color: #1e40af !important; /* 深蓝色背景 */
  color: #3b82f6 !important; /* 蓝色文字 */
}

/* 暗色主题下激活状态 */
:global(.dark) .floating-nav nav a.header-nav-link-active,
:global([data-theme="dark"]) .floating-nav nav a.header-nav-link-active,
:global([data-theme="hybrid-super-dark"]) .floating-nav nav a.header-nav-link-active {
  background-color: #1e40af !important; /* 深蓝色背景 */
  color: #60a5fa !important; /* 浅蓝色文字 */
}

/* 强制所有 span 文字使用清晰可见的颜色 */
/* 浅色主题下使用深色 */
.floating-nav span {
  color: #0f172a !important;
  opacity: 1 !important;
}

/* 暗色主题下使用白色 */
:global(.dark) .floating-nav span,
:global([data-theme="dark"]) .floating-nav span,
:global([data-theme="hybrid-super-dark"]) .floating-nav span {
  color: #ffffff !important;
  opacity: 1 !important;
}

/* 特别确保 Logo 区域的 span 文字清晰可见 */
:global(.dark) .floating-nav a.flex.items-center span,
:global([data-theme="dark"]) .floating-nav a.flex.items-center span,
:global([data-theme="hybrid-super-dark"]) .floating-nav a.flex.items-center span {
  color: var(--color-text-main) !important;
  font-weight: 700 !important;
  opacity: 1 !important;
  text-shadow: none !important;
}

/* 直接针对 Logo 文字 - 参考截图效果：白色文字 */
.floating-nav .logo-text {
  color: #ffffff !important; /* 白色文字（参考截图） */
  font-weight: 700 !important;
  opacity: 1 !important;
  text-shadow: none !important;
  -webkit-font-smoothing: antialiased !important;
  -moz-osx-font-smoothing: grayscale !important;
}

/* 暗色主题下也使用白色 */
:global(.dark) .floating-nav .logo-text,
:global([data-theme="dark"]) .floating-nav .logo-text,
:global([data-theme="hybrid-super-dark"]) .floating-nav .logo-text {
  color: #ffffff !important;
  font-weight: 700 !important;
  opacity: 1 !important;
  text-shadow: none !important;
  -webkit-font-smoothing: antialiased !important;
  -moz-osx-font-smoothing: grayscale !important;
}

/* 确保在暗色主题下 Logo 文字始终可见 */
:global(.dark) .logo-text,
:global([data-theme="dark"]) .logo-text,
:global([data-theme="hybrid-super-dark"]) .logo-text {
  color: #ffffff !important;
  font-weight: 700 !important;
  opacity: 1 !important;
}

/* 确保图标 emoji 也清晰可见（通过父元素文字颜色） */
/* 浅色主题下使用深色 */
.floating-nav .mr-1\.5 {
  color: #0f172a !important;
}

/* 暗色主题下使用白色 */
:global(.dark) .floating-nav .mr-1\.5,
:global([data-theme="dark"]) .floating-nav .mr-1\.5,
:global([data-theme="hybrid-super-dark"]) .floating-nav .mr-1\.5 {
  color: #ffffff !important;
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
  opacity: 0 !important;
  transform: translateY(-10px);
}

.dropdown-enter-to {
  opacity: 1 !important;
  transform: translateY(0);
}

.dropdown-leave-from {
  opacity: 1 !important;
  transform: translateY(0);
}

.dropdown-leave-to {
  opacity: 0 !important;
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
