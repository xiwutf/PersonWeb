<template>
  <header class="header-container">
    <div class="header-nav-pill">
      <!-- Logo -->
      <NuxtLink to="/" class="header-logo-link">
        <div
          @click.stop="handleLogoClick"
          @mouseenter="handleAvatarHover"
          class="header-logo-avatar"
          title="点击头像可进入管理后台"
        >
          <img src="/images/avatar.jpg" alt="Ming Studio" />
        </div>
        <span class="header-logo-text">Ming Studio</span>
      </NuxtLink>

      <!-- Desktop Nav -->
      <nav class="header-nav-desktop">
        <NuxtLink
          v-for="item in navigationItems"
          :key="item.path"
          :to="item.path"
          class="header-nav-link"
          :class="isActiveRoute(item.path) ? 'header-nav-link-active' : 'header-nav-link-inactive'"
        >
          {{ item.title }}
        </NuxtLink>
        <NavMoreMenu />
      </nav>

      <!-- Right Actions -->
      <div class="header-actions">
        <!-- Search -->
        <NuxtLink to="/search" class="header-action-btn" title="搜索">
          <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
          </svg>
        </NuxtLink>

        <!-- Theme Toggle (placeholder, future multi-theme) -->
        <div class="header-action-btn">
          <ThemeToggle />
        </div>

        <NuxtLink to="/contact" class="header-contact-outline-btn">
          联系
        </NuxtLink>
        <NuxtLink to="/ai" class="header-platform-btn">
          进入平台
        </NuxtLink>

        <!-- Mobile menu -->
        <button
          @click="toggleMobileMenu"
          class="header-mobile-menu-button"
          aria-label="菜单"
        >
          <svg class="header-mobile-menu-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M4 6h16M4 12h16M4 18h16"/>
          </svg>
        </button>
      </div>

      <!-- Mobile dropdown -->
      <Transition name="slide-down">
        <div v-if="isMobileMenuOpen" class="header-mobile-menu">
          <nav class="header-mobile-menu-content">
            <NuxtLink
              v-for="item in navigationItems"
              :key="item.path"
              :to="item.path"
              @click="closeMobileMenu"
              class="header-mobile-menu-item"
              :class="isActiveRoute(item.path)
                ? 'header-mobile-menu-item-active'
                : 'header-mobile-menu-item-inactive'"
            >
              {{ item.title }}
            </NuxtLink>
            <div class="header-mobile-menu-label">更多</div>
            <NuxtLink
              v-for="item in moreNavItems"
              :key="item.path"
              :to="item.path"
              @click="closeMobileMenu"
              class="header-mobile-menu-item"
              :class="isActiveRoute(item.path)
                ? 'header-mobile-menu-item-active'
                : 'header-mobile-menu-item-inactive'"
            >
              {{ item.title }}
            </NuxtLink>
            <NuxtLink
              to="/ai"
              @click="closeMobileMenu"
              class="header-mobile-platform-btn"
            >
              进入平台
            </NuxtLink>
          </nav>
        </div>
      </Transition>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import ThemeToggle from '~/components/layout/ThemeToggle.vue'
import NavMoreMenu from '~/components/layout/NavMoreMenu.vue'
import { moreNavItems, moreNavPaths } from '~/constants/site-more-nav'

// @ts-ignore - Nuxt 3 auto-imports
const router = useRouter()
// @ts-ignore - Nuxt 3 auto-imports
const route = useRoute()

const isMobileMenuOpen = ref(false)

// Secret admin access: 5 clicks within 3s on the logo avatar
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

// Keyboard shortcut: Ctrl+Shift+A or Ctrl+K → admin login
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

const navigationItems: Array<{ title: string; path: string }> = [
  { title: '首页',    path: '/' },
  { title: '产品',    path: '/products' },
  { title: '案例',    path: '/projects' },
  { title: 'AI实验室', path: '/lab' },
  { title: '文章',    path: '/blog' },
  { title: '关于',    path: '/about' },
]

const isActiveRoute = (path: string) => {
  if (!route || !route.path) return false

  if (path === '/') {
    return route.path === '/' || route.path === ''
  }

  const prefixMatchPaths = [
    '/products',
    '/projects',
    '/blog',
    '/lab',
    '/about',
    '/contact',
    ...moreNavPaths,
  ]
  if (prefixMatchPaths.includes(path)) {
    return route.path === path || route.path.startsWith(`${path}/`)
  }

  return route.path === path
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}
</script>
