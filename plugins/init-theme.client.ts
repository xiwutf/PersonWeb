/**
 * 主题初始化插件
 * 
 * 职责：
 * - 在应用启动（客户端）时，从后端获取全站主题配置
 * - 将后端配置的主题应用到页面，覆盖 localStorage 中的旧主题
 * - 确保后台设置的主题在所有访客刷新页面后自动生效
 * 
 * 优先级规则：
 * - 后端配置 > localStorage 本地记录
 * - 如果后端配置获取成功，使用后端配置并覆盖 localStorage
 * - 如果后端配置获取失败，回退到 useTheme() 的默认逻辑（localStorage 或默认 'light'）
 * 
 * 为什么需要这个插件：
 * - 实现"后台切换主题，前台自动应用"的核心逻辑
 * - 在应用启动的最早阶段（插件执行）就应用后端主题，确保用户看到的是后台设置的主题
 * - 避免用户本地 localStorage 中的主题覆盖后台全局设置
 */

export default defineNuxtPlugin(async () => {
  // 只在客户端执行
  if (!process.client) return

  try {
    // 调用后端接口：GET /api/Config/theme
    const api = useApi()
    const response = await api.get<{ theme: string }>('/Config/theme')

    // 获取到 theme 值（"light" | "dark" | "tech-blue"）
    if (response && response.theme) {
      const themeFromBackend = response.theme as 'light' | 'dark' | 'tech-blue'
      
      // 验证主题值是否合法
      if (themeFromBackend === 'light' || themeFromBackend === 'dark' || themeFromBackend === 'tech-blue') {
        // 引入并使用现有的 useTheme()
        const { setTheme } = useTheme()
        
        // 调用 setTheme(themeFromBackend)，让后台配置成为当前主题
        // 这会自动：
        // 1. 设置 document.documentElement.dataset.theme
        // 2. 写入 CSS 变量
        // 3. 写入 localStorage('site-theme')
        // 从而覆盖用户本地可能存在的旧主题设置
        setTheme(themeFromBackend)
        
        if (process.env.NODE_ENV === 'development') {
          console.log('[init-theme] 已从后端加载主题:', themeFromBackend)
        }
      } else {
        // 如果后端返回的主题值不合法，回退到默认逻辑
        console.warn('[init-theme] 后端返回的主题值不合法:', themeFromBackend)
        // 不调用 setTheme，让 useTheme() 使用默认逻辑
      }
    }
  } catch (error) {
    // 错误处理：如果请求失败（后端暂不可用等），不要中断应用启动
    // 这是后端主题初始化失败时的降级策略
    // 回退到 useTheme() 内本地逻辑（即 localStorage 里的值），或默认使用 'light'
    if (process.env.NODE_ENV === 'development') {
      console.warn('[init-theme] 从后端获取主题失败，回退到本地主题:', error)
    }
    // 不抛出错误，让 useTheme() 的默认初始化逻辑继续执行
  }
})

