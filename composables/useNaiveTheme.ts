/**
 * Naive UI 主题管理 Composable
 * 提供主题切换功能 (Strictly Light/Dark)
 */
import { computed, ref } from 'vue'
import { darkTheme, type GlobalTheme } from 'naive-ui'

export const useNaiveTheme = () => {
  // 主题模式：'light' | 'dark'
  // 默认为 'dark' 以展示 Fintech 风格
  const themeMode = ref<'light' | 'dark'>('dark')

  // 当前主题（根据 themeMode 计算）
  const currentTheme = computed<GlobalTheme | null>(() => {
    if (process.client) {
      const isDark = themeMode.value === 'dark'

      // 同步到 document.documentElement
      if (isDark) {
        document.documentElement.classList.add('dark')
        document.documentElement.dataset.theme = 'dark'
      } else {
        document.documentElement.classList.remove('dark')
        document.documentElement.dataset.theme = 'light'
      }

      return isDark ? darkTheme : null
    }
    return null
  })

  // 是否深色模式
  const isDark = computed(() => {
    return themeMode.value === 'dark'
  })

  // 初始化主题（从 localStorage 读取）
  const initTheme = () => {
    if (process.client) {
      const savedMode = localStorage.getItem('naive-theme-mode') as 'light' | 'dark' | null
      if (savedMode && (savedMode === 'light' || savedMode === 'dark')) {
        themeMode.value = savedMode
      }
    }
  }

  // 切换主题模式
  const setThemeMode = (mode: 'light' | 'dark') => {
    themeMode.value = mode
    if (process.client) {
      localStorage.setItem('naive-theme-mode', mode)
    }
  }

  // 简单的切换功能
  const toggleTheme = () => {
    setThemeMode(themeMode.value === 'dark' ? 'light' : 'dark')
  }

  // 初始化
  if (process.client) {
    initTheme()
  }

  return {
    themeMode: computed(() => themeMode.value),
    currentTheme,
    isDark,
    setThemeMode,
    toggleTheme,
    initTheme
  }
}

