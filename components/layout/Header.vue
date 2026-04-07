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
            @click="handleNavClick(item.path, $event)"
          >
            <span class="header-nav-link-icon">{{ item.icon }}</span>
            <span class="header-nav-link-label">{{ item.title }}</span>
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

  // 插件工具：始终显示，不依赖模块系统状态
  // 这样可以确保用户始终可以访问工具页面
  items.push({
    title: '插件工具',
    path: '/tools',
    icon: '🔧',
    moduleKey: 'tools'
  })

  // 在 SSR 时，默认显示这些导航项，避免 hydration 不匹配
  // 在客户端，会根据模块系统状态动态调整
  const isSSR = process.server
  const shouldShowProjects = isSSR || isModuleEnabled('projects')
  const shouldShowBlog = isSSR || isModuleEnabled('blog')
  const shouldShowLab3D = isSSR || isModuleEnabled('lab-3d')
  const shouldShowLife = isSSR || isModuleEnabled('life')
  const shouldShowEnglish = isSSR || isModuleEnabled('english')
  const shouldShowSkills = isSSR || isModuleEnabled('skills')
  const shouldShowDashboard = isSSR || isModuleEnabled('dashboard')
  const shouldShowGame = isSSR || isModuleEnabled('game')

  if (shouldShowProjects) {
    items.push({
      title: '项目展示',
      path: '/projects',
      icon: '🧪',
      moduleKey: 'projects'
    })
  }

  if (shouldShowBlog) {
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

  if (shouldShowLab3D) {
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
  
  // 在 SSR 时，默认显示这些导航项，避免 hydration 不匹配
  const isSSR = process.server
  const shouldShowLife = isSSR || isModuleEnabled('life')
  const shouldShowEnglish = isSSR || isModuleEnabled('english')
  const shouldShowSkills = isSSR || isModuleEnabled('skills')
  const shouldShowDashboard = isSSR || isModuleEnabled('dashboard')
  const shouldShowGame = isSSR || isModuleEnabled('game')
  
  const items: Array<{ title: string; path: string; icon: string; moduleKey?: string }> = []

  if (shouldShowLife) {
    items.push({
      title: '生活随笔',
      path: '/life',
      icon: '☕',
      moduleKey: 'life'
    })
  }

  if (shouldShowEnglish) {
    items.push({
      title: '英语学习',
      path: '/english',
      icon: '📚',
      moduleKey: 'english'
    })
  }

  if (shouldShowSkills) {
    items.push({
      title: '技能树',
      path: '/skills',
      icon: '🌳',
      moduleKey: 'skills'
    })
  }

  if (shouldShowDashboard) {
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

  if (shouldShowGame) {
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
  // /tools 路径需要前缀匹配
  if (path === '/tools') {
    return route.path === '/tools' || route.path.startsWith('/tools/')
  }
  // /cognition 路径需要前缀匹配
  if (path === '/cognition') {
    return route.path === '/cognition' || route.path.startsWith('/cognition/')
  }
  // 其他路由精确匹配
  return route.path === path
}

// 处理导航点击（调试用）
const handleNavClick = (path: string, event: MouseEvent) => {
  if (process.dev && path === '/tools') {
    console.log('点击插件工具链接，路径:', path)
    console.log('当前路由:', route.path)
  }
  // 不阻止默认行为，让 NuxtLink 正常处理
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
  top: calc(var(--safe-area-top, 0px) + 1rem) !important;
  left: 50% !important;
  transform: translateX(-50%) !important;
  z-index: 10000 !important; /* 提高 z-index，确保在所有元素之上 */
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 独立悬浮导航栏样式 - 使用 CSS 变量 */
.floating-nav {
  background: var(--color-bg-elevated) !important;
  backdrop-filter: blur(20px) !important;
  -webkit-backdrop-filter: blur(20px) !important;
  border: 1px solid var(--color-border-default) !important;
  border-radius: 1rem !important;
  box-shadow: var(--shadow-md) !important;
  width: calc(100% - 2rem) !important;
  max-width: 1280px !important;
  transition: all 0.3s ease !important;
  color: var(--color-text-main) !important;
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 暗色主题下的导航栏 */
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav,
:global([data-theme="hybrid-super-dark"]) .floating-nav {
  background: var(--color-bg-card) !important;
  backdrop-filter: blur(20px) !important;
  -webkit-backdrop-filter: blur(20px) !important;
  border-color: var(--color-border-default) !important;
  box-shadow: var(--shadow-lg) !important;
  color: var(--color-text-main) !important;
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 确保暗色主题下导航栏内的所有文字清晰可见 */
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav,
:global([data-theme="hybrid-super-dark"]) .floating-nav {
  color: var(--color-text-main) !important;
}

/* 左侧 Logo 文字 */
:global(.dark) .floating-nav .text-text-main,
:global([data-theme="dark"]) .floating-nav .text-text-main,
:global([data-theme="hybrid-super-dark"]) .floating-nav .text-text-main,
:global(.dark) .floating-nav a span.text-text-main,
:global([data-theme="dark"]) .floating-nav a span.text-text-main,
:global([data-theme="hybrid-super-dark"]) .floating-nav a span.text-text-main,
:global(.dark) .floating-nav .flex.items-center span,
:global([data-theme="dark"]) .floating-nav .flex.items-center span,
:global([data-theme="hybrid-super-dark"]) .floating-nav .flex.items-center span {
  color: var(--color-text-main, var(--color-bg-card)) !important;
  font-weight: 800 !important;
  opacity: 1 !important;
}

/* 导航链接文字 - 使用 CSS 变量 */
.floating-nav .text-text-muted {
  color: var(--color-text-muted, var(--color-text-sec)) !important;
}

/* 导航链接悬停状态 */
.floating-nav a:hover {
  color: var(--color-text-main, var(--color-text-main)) !important;
}

/* 搜索图标和主题切换图标 */
.floating-nav svg {
  color: var(--color-text-main, var(--color-text-main)) !important;
}

/* 更多菜单按钮文字 */
.floating-nav button {
  color: var(--color-text-main, var(--color-text-main)) !important;
}

/* 移动端菜单按钮图标 */
.floating-nav .md\:hidden svg {
  color: var(--color-text-main) !important;
}

/* 非激活状态的导航链接 - 使用 CSS 变量 */
.floating-nav nav a:not(.header-nav-link-active) {
  background-color: var(--color-bg-elevated) !important;
  color: var(--color-text-muted) !important;
}

/* 激活状态的导航链接 - 使用主色调 */
.floating-nav nav a.header-nav-link-active {
  background-color: var(--color-primary-hover) !important;
  color: var(--color-primary) !important;
}

/* 强制所有 span 文字使用 CSS 变量 */
.floating-nav span {
  color: var(--color-text-main) !important;
  opacity: 1 !important;
}

/* Logo 文字 - 使用 CSS 变量 */
.floating-nav .logo-text {
  color: var(--color-text-main) !important;
  font-weight: 700 !important;
  opacity: 1 !important;
  text-shadow: none !important;
  -webkit-font-smoothing: antialiased !important;
  -moz-osx-font-smoothing: grayscale !important;
}

/* 确保图标 emoji 也清晰可见 */
.floating-nav .mr-1\.5 {
  color: var(--color-text-main) !important;
}

/* 响应式：小屏幕时全宽 */
@media (max-width: 640px) {
  .floating-nav {
    width: calc(100% - 1rem);
    top: calc(var(--safe-area-top, 0px) + 0.5rem);
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

.header-content-wrapper {
  position: relative;
}

.floating-nav {
  background:
    linear-gradient(135deg, rgba(10, 15, 29, 0.84), rgba(19, 28, 48, 0.72)) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
  box-shadow:
    0 28px 60px rgba(7, 12, 24, 0.24),
    inset 0 1px 0 rgba(255, 255, 255, 0.08) !important;
}

.floating-nav::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: inherit;
  background:
    radial-gradient(circle at left top, rgba(120, 119, 198, 0.18), transparent 34%),
    radial-gradient(circle at right top, rgba(56, 189, 248, 0.12), transparent 30%);
  pointer-events: none;
}

.floating-nav::after {
  content: '';
  position: absolute;
  inset: 1px;
  border-radius: calc(1rem - 1px);
  border: 1px solid rgba(255, 255, 255, 0.04);
  pointer-events: none;
}

.header-main {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
  min-height: 4.75rem;
  padding: 0.75rem 1.1rem;
}

.header-logo-link {
  display: inline-flex;
  align-items: center;
  gap: 0.9rem;
  min-width: 0;
}

.header-logo-avatar {
  position: relative;
  overflow: hidden;
  width: 2.9rem;
  height: 2.9rem;
  border-radius: 1rem;
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow:
    0 18px 32px rgba(15, 23, 42, 0.24),
    inset 0 1px 0 rgba(255, 255, 255, 0.12);
}

.header-logo-avatar::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.14), transparent 58%);
  pointer-events: none;
}

.header-logo-text {
  position: relative;
  display: inline-flex;
  flex-direction: column;
  gap: 0.2rem;
  font-size: 1.02rem;
  letter-spacing: 0.08em;
}

.header-logo-text::after {
  content: 'SYSTEMS · AI · TOOLS';
  font-size: 0.56rem;
  letter-spacing: 0.28em;
  color: rgba(226, 232, 240, 0.62);
  font-weight: 600;
}

.header-nav-desktop {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.65rem;
  padding: 0.45rem;
  border-radius: 999px;
  background: rgba(9, 14, 26, 0.38);
  border: 1px solid rgba(255, 255, 255, 0.06);
  box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.header-nav-link,
.header-more-menu-button,
.header-search-button {
  position: relative;
  border-radius: 999px;
  border: 1px solid transparent;
  transition:
    transform 0.25s ease,
    border-color 0.25s ease,
    background-color 0.25s ease,
    box-shadow 0.25s ease,
    color 0.25s ease;
}

.header-nav-link {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  min-height: 3rem;
  padding: 0.72rem 0.9rem;
}

.header-nav-link-inactive,
.header-more-menu-button-inactive,
.header-search-button {
  background: rgba(255, 255, 255, 0.02);
}

.header-nav-link-inactive:hover,
.header-more-menu-button-inactive:hover,
.header-search-button:hover {
  transform: translateY(-1px);
  border-color: rgba(148, 163, 184, 0.2);
  background: rgba(255, 255, 255, 0.06);
  box-shadow: 0 14px 24px rgba(8, 15, 29, 0.18);
}

.header-nav-link-active,
.header-more-menu-button-active,
.header-search-button-active {
  background: linear-gradient(135deg, rgba(56, 189, 248, 0.18), rgba(129, 140, 248, 0.2));
  border-color: rgba(125, 211, 252, 0.3);
  box-shadow:
    0 16px 28px rgba(8, 15, 29, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.12);
}

.header-nav-link-icon {
  flex: 0 0 auto;
  opacity: 0.84;
}

.header-nav-link-label,
.header-more-menu-button > span {
  display: inline-block;
  white-space: nowrap;
  word-break: keep-all;
  line-height: 1;
  font-size: 0.92rem;
  letter-spacing: 0.01em;
}

.header-search-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: auto;
  min-width: 2.9rem;
  padding-inline: 0.85rem;
}

.header-search-button::after {
  content: '/';
  font-size: 0.72rem;
  font-weight: 700;
  line-height: 1;
  color: rgba(226, 232, 240, 0.58);
}

.header-search-button:hover::after,
.header-search-button-active::after {
  color: rgba(255, 255, 255, 0.9);
}

.header-theme-toggle-container {
  margin-left: 0.15rem;
  padding-left: 0.3rem;
  border-left: 1px solid rgba(255, 255, 255, 0.08);
}

.header-more-menu-button {
  display: inline-flex;
  align-items: center;
  gap: 0.42rem;
  min-height: 3rem;
  padding: 0.72rem 0.95rem;
  white-space: nowrap;
}

.header-dropdown-menu {
  padding-top: 0.8rem;
}

.header-dropdown-menu-content,
.header-mobile-menu {
  background: rgba(10, 15, 29, 0.92);
  backdrop-filter: blur(24px);
  -webkit-backdrop-filter: blur(24px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  box-shadow: 0 24px 48px rgba(7, 12, 24, 0.28);
}

.header-mobile-menu {
  position: relative;
  z-index: 1;
  margin: 0 1rem 0.5rem;
  border-radius: 1.1rem;
  overflow: hidden;
}

.header-mobile-menu-content {
  padding: 0.75rem;
}

.header-mobile-menu-item {
  border-radius: 0.9rem;
}

.header-logo-text::after {
  content: 'SYSTEMS / AI / TOOLS';
}

@media (max-width: 960px) {
  .header-main {
    min-height: 4.25rem;
    padding: 0.65rem 0.85rem;
  }

  .header-logo-text::after {
    display: none;
  }
}

@media (min-width: 961px) and (max-width: 1360px) {
  .header-main {
    gap: 1rem;
  }

  .header-logo-link {
    gap: 0.75rem;
  }

  .header-logo-text {
    font-size: 0.95rem;
  }

  .header-logo-text::after {
    font-size: 0.5rem;
    letter-spacing: 0.22em;
  }

  .header-nav-desktop {
    gap: 0.45rem;
    padding: 0.35rem;
  }

  .header-nav-link,
  .header-more-menu-button {
    min-height: 2.85rem;
    padding: 0.68rem 0.78rem;
  }

  .header-nav-link-label,
  .header-more-menu-button > span {
    font-size: 0.86rem;
  }

  .header-search-button {
    min-width: 2.7rem;
    padding-inline: 0.72rem;
  }
}
</style>
