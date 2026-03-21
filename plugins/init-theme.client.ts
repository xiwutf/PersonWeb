/**
 * 主题初始化插件（升级版）
 * 
 * 职责：
 * - 在应用启动（客户端）时，从后端获取全站主题配置、模块主题配置、主题 tokens
 * - 将后端配置应用到页面，覆盖 localStorage 中的旧主题
 * - 确保后台设置的主题在所有访客刷新页面后自动生效
 * 
 * 重要修复：改为非阻塞模式（不使用 async 插件），避免阻塞 NuxtPage 挂载
 * - async 插件会阻塞整个应用渲染直到 await 完成
 * - 如果某个 API 请求卡住，所有页面都不会渲染
 * - 改为 fire-and-forget 模式：异步加载主题但不阻塞页面显示
 */

import { normalizeTheme } from '~/constants/design/tokens'

export default defineNuxtPlugin(() => {
  // 只在客户端执行
  if (!process.client) return

  // 非阻塞: 用 IIFE 执行异步逻辑，不阻塞 Nuxt 应用挂载
  ;(async () => {
    const api = useApi()
    let globalTheme: string | null = null
    let moduleThemesData: any = null
    const tokensMap: Record<string, Record<string, string>> = {}

    // ==================== 1. 获取全局主题 ====================
    try {
      const response = await api.get<{ theme: string }>('/Config/theme')
      if (response && response.theme) {
        const themeFromBackend = response.theme
        const normalizedTheme = normalizeTheme(themeFromBackend)
        globalTheme = normalizedTheme
        document.documentElement.dataset.theme = normalizedTheme
      }
    } catch (error) {
      // 获取全局主题失败，使用默认逻辑
    }

    // ==================== 2. 获取模块主题配置 ====================
    try {
      moduleThemesData = await api.get<{
        globalTheme: string
        modules: Array<{ moduleId: string; theme: string | null }>
        availableThemes: string[]
      }>('/Config/module-themes')
    } catch (error) {
      // 获取模块主题配置失败，使用默认逻辑
    }

    // ==================== 3. 注入到 useTheme 和 useModuleTheme ====================
    try {
      const { setTheme, setBackendThemeTokens } = useTheme()
      
      // 注入后端 tokens（如果有）
      if (Object.keys(tokensMap).length > 0) {
        setBackendThemeTokens(tokensMap)
      }

      // 注入模块主题配置
      if (moduleThemesData && moduleThemesData.modules) {
        const moduleThemesMap: Record<string, string | null> = {}
        moduleThemesData.modules.forEach((module: { moduleId: string; theme: string | null }) => {
          moduleThemesMap[module.moduleId] = module.theme
        })
        const { setModuleThemes } = await import('~/composables/useModuleTheme')
        setModuleThemes(moduleThemesMap)
      }

      // 应用全局主题（如果获取成功）
      if (globalTheme) {
        setTheme(globalTheme)
      }
    } catch (error) {
      // 注入失败时的降级策略：不阻塞应用启动，使用默认逻辑
    }
  })().catch(() => {
    // 整个初始化失败也不阻塞应用
  })
})
