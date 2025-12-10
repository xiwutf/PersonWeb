/**
 * Naive UI 主题管理 Composable
 * 提供主题切换、主题色配置等功能
 */
import { computed, ref, watch } from 'vue'
import { darkTheme, type GlobalTheme, type GlobalThemeOverrides } from 'naive-ui'

export const useNaiveTheme = () => {
  // 主题模式：'light' | 'dark' | 'auto'
  const themeMode = ref<'light' | 'dark' | 'auto'>('dark')

  // 主题色配置
  const themeOverrides = ref<GlobalThemeOverrides>({
    common: {
      primaryColor: '#3b82f6', // 蓝色
      primaryColorHover: '#2563eb',
      primaryColorPressed: '#1d4ed8',
      primaryColorSuppl: '#60a5fa'
    }
  })

  // 检测系统主题
  const prefersDark = computed(() => {
    if (process.client) {
      return window.matchMedia('(prefers-color-scheme: dark)').matches
    }
    return false
  })

  // 当前主题（根据 themeMode 和系统偏好计算）
  const currentTheme = computed<GlobalTheme | null>(() => {
    if (process.client) {
      let shouldUseDark = false

      if (themeMode.value === 'dark') {
        shouldUseDark = true
      } else if (themeMode.value === 'light') {
        shouldUseDark = false
      } else if (themeMode.value === 'auto') {
        shouldUseDark = prefersDark.value
      }

      // 同步到 document.documentElement
      if (shouldUseDark) {
        document.documentElement.classList.add('dark')
        document.documentElement.dataset.theme = 'dark'
      } else {
        document.documentElement.classList.remove('dark')
        document.documentElement.dataset.theme = 'light'
      }

      return shouldUseDark ? darkTheme : null
    }
    return null
  })

  // 是否深色模式
  const isDark = computed(() => {
    return currentTheme.value === darkTheme
  })

  // 初始化主题（从 localStorage 读取）
  const initTheme = () => {
    if (process.client) {
      const savedMode = localStorage.getItem('naive-theme-mode') as 'light' | 'dark' | 'auto' | null
      if (savedMode) {
        themeMode.value = savedMode
      }

      const savedOverrides = localStorage.getItem('naive-theme-overrides')
      if (savedOverrides) {
        try {
          themeOverrides.value = JSON.parse(savedOverrides)
        } catch (e) {
          console.warn('Failed to parse theme overrides:', e)
        }
      }
    }
  }

  // 切换主题模式
  const setThemeMode = (mode: 'light' | 'dark' | 'auto') => {
    themeMode.value = mode
    if (process.client) {
      localStorage.setItem('naive-theme-mode', mode)
    }
  }

  // 设置主题色
  const setThemeColor = (color: string) => {
    themeOverrides.value = {
      ...themeOverrides.value,
      common: {
        ...themeOverrides.value.common,
        primaryColor: color,
        primaryColorHover: adjustBrightness(color, -0.1),
        primaryColorPressed: adjustBrightness(color, -0.2),
        primaryColorSuppl: adjustBrightness(color, 0.2)
      }
    }

    if (process.client) {
      localStorage.setItem('naive-theme-overrides', JSON.stringify(themeOverrides.value))
    }
  }

  // 调整颜色亮度（改进版）
  const adjustBrightness = (color: string, amount: number): string => {
    if (!color.startsWith('#')) return color

    // 移除 # 并解析 RGB
    const hex = color.slice(1)
    const num = parseInt(hex, 16)
    const r = (num >> 16) & 0xff
    const g = (num >> 8) & 0xff
    const b = num & 0xff

    // 计算新颜色值（amount: -1 到 1，负数变暗，正数变亮）
    const newR = Math.max(0, Math.min(255, Math.round(r + amount * 255)))
    const newG = Math.max(0, Math.min(255, Math.round(g + amount * 255)))
    const newB = Math.max(0, Math.min(255, Math.round(b + amount * 255)))

    return `#${((newR << 16) | (newG << 8) | newB).toString(16).padStart(6, '0')}`
  }

  // 预设主题色
  const presetColors = {
    blue: '#3b82f6',
    green: '#10b981',
    purple: '#8b5cf6',
    orange: '#f59e0b',
    red: '#ef4444',
    cyan: '#06b6d4',
    pink: '#ec4899',
    indigo: '#6366f1'
  }

  // 应用预设主题色
  const applyPresetColor = (colorName: keyof typeof presetColors) => {
    setThemeColor(presetColors[colorName])
  }

  // 监听系统主题变化
  if (process.client) {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
    mediaQuery.addEventListener('change', () => {
      if (themeMode.value === 'auto') {
        // 自动模式下，系统主题变化时更新
        // currentTheme 会自动响应
      }
    })
  }

  // 初始化
  if (process.client) {
    initTheme()
  }

  return {
    themeMode: computed(() => themeMode.value),
    currentTheme,
    isDark,
    themeOverrides: computed(() => themeOverrides.value),
    setThemeMode,
    setThemeColor,
    applyPresetColor,
    presetColors,
    initTheme
  }
}

