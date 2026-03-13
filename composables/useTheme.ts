/**
 * 主题切换组合式函数
 * 
 * 职责：
 * - 管理用户选择的主题（light/dark/tech-blue/paper/forest）
 * - 将主题写入 document.documentElement.dataset.theme
 * - 将主题变量写入 CSS 变量
 * - 持久化主题到 localStorage
 * - 提供主题切换和切换方法
 */

import type { ThemeName, ThemeKey } from '~/constants/design/tokens'
import { themeVariables, DEFAULT_THEME, THEME_STORAGE_KEY, resolveThemeTokens, normalizeTheme } from '~/constants/design/tokens'

// 全局主题状态（单例模式，确保所有组件共享同一个主题状态）
const globalThemeState = {
  currentTheme: ref<ThemeName>(DEFAULT_THEME),
  initialized: false,
  // 后端传来的 theme tokens 覆盖
  // 来源：plugins/init-theme.client.ts 从后端 /api/Config/theme-tokens/{themeKey} 获取
  // 用途：覆盖默认 tokens，实现后台细粒度管理样式
  backendTokens: ref<Partial<Record<ThemeKey, Record<string, string>>>>({})
}

export const useTheme = () => {
  // 使用全局主题状态，确保所有组件共享同一个主题
  const currentTheme = globalThemeState.currentTheme

  /**
   * 应用主题到 DOM
   * 这一步会将主题名设置到 data-theme 属性，并将所有 CSS 变量写入 document.documentElement
   * 
   * 更新：现在支持后端 tokens 覆盖
   * - 先合并默认 tokens 和后端覆盖的 tokens
   * - 再写入 CSS 变量
   * 
   * 重要：设置 data-theme 属性，这样 CSS 选择器 html[data-theme='xxx'] 会生效
   * 这是新的样式架构的核心：通过 data-theme 驱动 tokens.css 中的主题变量
   * 
   * 重构说明（2024-12-XX）：
   * - 现在只支持 light 和 dark 两个主题
   * - 如果传入旧主题（tech-blue、paper、forest 等），会自动映射为 light 或 dark
   */
  const applyTheme = (theme: ThemeName | string) => {
    if (!process.client) return

    // 1. 标准化主题（兼容旧数据）
    const normalizedTheme = normalizeTheme(theme) as ThemeName

    // 2. 设置 data-theme 属性，这样 CSS 选择器 html[data-theme='xxx'] 会生效
    // 这是新的样式架构的核心：通过 data-theme 驱动 tokens.css 中的主题变量
    document.documentElement.dataset.theme = normalizedTheme
    // [New] 必须同步 toggle 'dark' class，以确保 Tailwind 的 darkMode: 'class' 生效
    document.documentElement.classList.toggle('dark', normalizedTheme === 'dark')

    // 2. 获取后端覆盖的 tokens（如果有）
    const backendOverrides = globalThemeState.backendTokens.value[theme]

    // 3. 合并默认 tokens 和后端覆盖，得到最终 tokens
    // resolveThemeTokens 会：
    // - 从 defaultThemeTokens 获取默认值
    // - 用 backendOverrides 覆盖默认值
    // - 返回 CSS 变量格式的键值对（--color-xxx: value）
    const finalTokens = resolveThemeTokens(normalizedTheme, backendOverrides)

    // 4. 将每个变量写入 document.documentElement.style
    // 这样即使 CSS 文件还没加载，也能通过内联样式生效
    Object.entries(finalTokens).forEach(([key, value]) => {
      document.documentElement.style.setProperty(key, value)
    })
  }

  /**
   * 从 localStorage 读取保存的主题
   * 如果读取失败或主题不存在，返回默认主题
   * 
   * 重构说明（2024-12-XX）：
   * - 现在只支持 light 和 dark 两个主题
   * - 如果读取到旧主题（tech-blue、paper、forest 等），会自动映射为 light 或 dark
   */
  const loadThemeFromStorage = (): ThemeName => {
    if (!process.client) return DEFAULT_THEME

    try {
      const saved = localStorage.getItem(THEME_STORAGE_KEY)
      if (saved) {
        // 使用 normalizeTheme 自动映射旧主题
        return normalizeTheme(saved)
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
   * 
   * 重构说明（2024-12-XX）：
   * - 现在只支持 light 和 dark 两个主题
   * - 如果传入旧主题（tech-blue、paper、forest 等），会自动映射为 light 或 dark
   */
  const setTheme = (theme: ThemeName | string) => {
    // 标准化主题（兼容旧数据）
    const normalizedTheme = normalizeTheme(theme) as ThemeName
    currentTheme.value = normalizedTheme
    applyTheme(normalizedTheme)
    saveThemeToStorage(normalizedTheme)
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
  // 注意：init-theme 插件会在应用启动时从后端获取主题并调用 setTheme()
  // 如果插件执行成功，setTheme() 会设置 currentTheme 和 localStorage
  // 如果插件执行失败，这里会回退到使用 localStorage 中的旧值或默认值
  if (process.client && !globalThemeState.initialized) {
    // 标记已初始化，避免重复初始化
    globalThemeState.initialized = true

    // 延迟初始化，确保插件有机会先执行
    // 使用 nextTick 让插件先执行，然后再读取 localStorage
    nextTick(() => {
      // 检查 DOM 是否已经有主题被设置（插件可能已经设置了）
      const domTheme = document.documentElement.dataset.theme

      if (domTheme) {
        // 如果 DOM 已经有主题（说明插件已经执行），使用 DOM 中的主题（自动映射）
        const normalizedTheme = normalizeTheme(domTheme) as ThemeName
        currentTheme.value = normalizedTheme
      } else {
        // 如果 DOM 没有主题，从 localStorage 读取（自动映射）
        const savedTheme = loadThemeFromStorage()
        currentTheme.value = savedTheme
        applyTheme(savedTheme)
      }
    })

    // 监听 currentTheme 的变化，自动应用新主题
    // 这样当通过 setTheme 或 toggleDark 改变主题时，会自动更新 DOM
    watchEffect(() => {
      applyTheme(currentTheme.value)
      saveThemeToStorage(currentTheme.value)
    })
  }

  /**
   * 设置后端 theme tokens 覆盖
   * 
   * 用途：
   * - 由 plugins/init-theme.client.ts 调用，注入从后端获取的 tokens
   * - 后端 tokens 会覆盖默认 tokens 中对应的字段
   * - 如果后端没有提供某个主题的 tokens，使用默认值
   * 
   * @param tokensFromBackend 后端返回的 tokens，格式为 { themeKey: { "color.bg.body": "var(--color-bg-card)", ... } }
   */
  const setBackendThemeTokens = (
    tokensFromBackend: Partial<Record<ThemeKey, Record<string, string>>>
  ) => {
    globalThemeState.backendTokens.value = tokensFromBackend

    // 如果当前主题有 tokens 覆盖，重新应用主题以生效
    if (process.client && currentTheme.value) {
      applyTheme(currentTheme.value)
    }
  }

  return {
    // 当前主题（响应式引用）
    currentTheme: readonly(currentTheme),
    // 设置主题
    setTheme,
    // 切换暗色模式
    toggleDark,
    // 设置后端 theme tokens（供插件调用）
    setBackendThemeTokens
  }
}
