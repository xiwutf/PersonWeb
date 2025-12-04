/**
 * 主题初始化插件（升级版）
 * 
 * 职责：
 * - 在应用启动（客户端）时，从后端获取全站主题配置、模块主题配置、主题 tokens
 * - 将后端配置应用到页面，覆盖 localStorage 中的旧主题
 * - 确保后台设置的主题在所有访客刷新页面后自动生效
 * 
 * 加载内容：
 * 1. 全局主题（GET /api/Config/theme）
 * 2. 模块主题配置（GET /api/Config/module-themes）
 * 3. 主题 tokens（可选，GET /api/Config/theme-tokens/{themeKey}）
 * 
 * 优先级规则：
 * - 后端配置 > localStorage 本地记录
 * - 如果后端配置获取成功，使用后端配置并覆盖 localStorage
 * - 如果后端配置获取失败，回退到 useTheme() 的默认逻辑（localStorage 或默认 'light'）
 * 
 * 降级策略：
 * - 如果任意后端请求失败，不阻塞应用启动
 * - 使用 localStorage 中的主题或默认 'light'
 * - 模块主题和 tokens 失败不影响全局主题应用
 */

export default defineNuxtPlugin(async () => {
  // 只在客户端执行
  if (!process.client) return

  const api = useApi()
  let globalTheme: string | null = null
  let moduleThemesData: any = null
  const tokensMap: Record<string, Record<string, string>> = {}

  // ==================== 1. 获取全局主题 ====================
  try {
    const response = await api.get<{ theme: string }>('/Config/theme')
    if (response && response.theme) {
      // 验证主题值是否在支持的列表中
      const validThemes = ['light', 'dark', 'tech-blue', 'paper', 'forest', 'hybrid-super', 'hybrid-super-dark', 'hybrid-super-light'] as const
      const themeFromBackend = response.theme
      if (validThemes.includes(themeFromBackend as any)) {
        globalTheme = themeFromBackend
        // 设置 data-theme 属性，驱动 tokens.css 中的主题变量
        document.documentElement.dataset.theme = themeFromBackend
        if (process.env.NODE_ENV === 'development') {
          console.log('[init-theme] 已从后端加载全局主题:', themeFromBackend)
        }
      }
    }
  } catch (error) {
    if (process.env.NODE_ENV === 'development') {
      console.warn('[init-theme] 获取全局主题失败:', error)
    }
  }

  // ==================== 2. 获取模块主题配置 ====================
  try {
    moduleThemesData = await api.get<{
      globalTheme: string
      modules: Array<{ moduleId: string; theme: string | null }>
      availableThemes: string[]
    }>('/Config/module-themes')
    
    if (process.env.NODE_ENV === 'development') {
      console.log('[init-theme] 已从后端加载模块主题配置:', moduleThemesData)
    }
  } catch (error) {
    if (process.env.NODE_ENV === 'development') {
      console.warn('[init-theme] 获取模块主题配置失败:', error)
    }
  }

  // ==================== 3. 获取主题 tokens（可选，前台暂时先不加载，只在后台编辑页加载）====================
  // 注意：为了性能考虑，前台启动时暂时不加载 tokens
  // tokens 主要用于后台编辑界面，前台可以使用默认 tokens
  // 如果需要前台也支持后端 tokens，可以取消下面的注释
  /*
  try {
    const availableThemes = ['light', 'dark', 'tech-blue']
    for (const themeKey of availableThemes) {
      try {
        const tokensResponse = await api.get<{ themeKey: string; tokens: Record<string, string> }>(
          `/Config/theme-tokens/${themeKey}`
        )
        if (tokensResponse && tokensResponse.tokens) {
          tokensMap[themeKey] = tokensResponse.tokens
        }
      } catch (e) {
        // 单个主题 tokens 获取失败不影响其他主题
        if (process.env.NODE_ENV === 'development') {
          console.warn(`[init-theme] 获取主题 ${themeKey} 的 tokens 失败:`, e)
        }
      }
    }
    if (process.env.NODE_ENV === 'development' && Object.keys(tokensMap).length > 0) {
      console.log('[init-theme] 已从后端加载主题 tokens:', tokensMap)
    }
  } catch (error) {
    if (process.env.NODE_ENV === 'development') {
      console.warn('[init-theme] 获取主题 tokens 失败:', error)
    }
  }
  */

  // ==================== 4. 注入到 useTheme 和 useModuleTheme ====================
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
      // 使用 setModuleThemes 函数（从 useModuleTheme 导出）
      const { setModuleThemes } = await import('~/composables/useModuleTheme')
      setModuleThemes(moduleThemesMap)
    }

    // 应用全局主题（如果获取成功）
    if (globalTheme) {
      // 类型断言：globalTheme 已经在上面验证过，属于有效主题
      setTheme(globalTheme as 'light' | 'dark' | 'tech-blue' | 'paper' | 'forest' | 'hybrid-super' | 'hybrid-super-dark' | 'hybrid-super-light')
    }
  } catch (error) {
    // 注入失败时的降级策略：不阻塞应用启动
    if (process.env.NODE_ENV === 'development') {
      console.warn('[init-theme] 注入主题配置失败，使用默认逻辑:', error)
    }
  }
})

