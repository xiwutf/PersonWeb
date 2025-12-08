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
      // 重构说明（2024-12-XX）：
      // - 现在只支持 light 和 dark 两个主题
      // - 如果后端返回旧主题（tech-blue、paper、forest 等），会自动映射为 light 或 dark
      const { normalizeTheme } = await import('~/constants/design/tokens')
      const themeFromBackend = response.theme
      const normalizedTheme = normalizeTheme(themeFromBackend)
      globalTheme = normalizedTheme
      // 设置 data-theme 属性，驱动 tokens.css 中的主题变量
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
      }
    }
  } catch (error) {
    // 获取主题 tokens 失败，使用默认逻辑
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
      // 重构说明（2024-12-XX）：
      // - globalTheme 已经通过 normalizeTheme 标准化为 'light' 或 'dark'
      setTheme(globalTheme)
    }
  } catch (error) {
    // 注入失败时的降级策略：不阻塞应用启动，使用默认逻辑
  }
})

