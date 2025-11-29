/**
 * 主题切换组合式函数
 * 
 * 职责：
 * - 管理用户选择的主题（light/dark/tech-blue）
 * - 将主题写入 document.documentElement.dataset.theme
 * - 将主题变量写入 CSS 变量
 * - 持久化主题到 localStorage
 * - 提供主题切换和切换方法
 */

import type { ThemeName } from '~/constants/design/tokens'
import { themeVariables, DEFAULT_THEME, THEME_STORAGE_KEY } from '~/constants/design/tokens'

export const useTheme = () => {
  // 当前主题的响应式引用
  // 初始值从 localStorage 获取，如果没有则使用默认主题
  const currentTheme = ref<ThemeName>(DEFAULT_THEME)

  /**
   * 应用主题到 DOM
   * 这一步会将主题名设置到 data-theme 属性，并将所有 CSS 变量写入 document.documentElement
   */
  const applyTheme = (theme: ThemeName) => {
    if (!process.client) return

    // 1. 设置 data-theme 属性，这样 CSS 选择器 :root[data-theme='xxx'] 会生效
    document.documentElement.dataset.theme = theme

    // 2. 获取当前主题对应的所有 CSS 变量
    const variables = themeVariables[theme]

    // 3. 将每个变量写入 document.documentElement.style
    // 这样即使 CSS 文件还没加载，也能通过内联样式生效
    Object.entries(variables).forEach(([key, value]) => {
      document.documentElement.style.setProperty(key, value)
    })
  }

  /**
   * 从 localStorage 读取保存的主题
   * 如果读取失败或主题不存在，返回默认主题
   */
  const loadThemeFromStorage = (): ThemeName => {
    if (!process.client) return DEFAULT_THEME

    try {
      const saved = localStorage.getItem(THEME_STORAGE_KEY)
      if (saved && (saved === 'light' || saved === 'dark' || saved === 'tech-blue')) {
        return saved as ThemeName
      }
    } catch (error) {
      console.warn('读取主题失败:', error)
    }

    return DEFAULT_THEME
  }

  /**
   * 保存主题到 localStorage
   */
  const saveThemeToStorage = (theme: ThemeName) => {
    if (!process.client) return

    try {
      localStorage.setItem(THEME_STORAGE_KEY, theme)
    } catch (error) {
      console.warn('保存主题失败:', error)
    }
  }

  /**
   * 设置主题
   * 这是对外暴露的主要方法，用于切换主题
   */
  const setTheme = (theme: ThemeName) => {
    currentTheme.value = theme
    applyTheme(theme)
    saveThemeToStorage(theme)
  }

  /**
   * 在 light 和 dark 之间切换
   * 这是一个便捷方法，常用于暗色模式切换按钮
   */
  const toggleDark = () => {
    const nextTheme: ThemeName = currentTheme.value === 'dark' ? 'light' : 'dark'
    setTheme(nextTheme)
  }

  // 初始化：在客户端挂载时，从 localStorage 读取主题并应用
  if (process.client) {
    // 立即读取并应用保存的主题
    const savedTheme = loadThemeFromStorage()
    currentTheme.value = savedTheme
    applyTheme(savedTheme)

    // 监听 currentTheme 的变化，自动应用新主题
    // 这样当通过 setTheme 或 toggleDark 改变主题时，会自动更新 DOM
    watchEffect(() => {
      applyTheme(currentTheme.value)
      saveThemeToStorage(currentTheme.value)
    })
  }

  return {
    // 当前主题（响应式引用）
    currentTheme: readonly(currentTheme),
    // 设置主题
    setTheme,
    // 切换暗色模式
    toggleDark
  }
}
