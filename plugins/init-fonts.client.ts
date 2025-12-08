/**
 * 字体初始化插件
 * 
 * 职责：
 * - 在应用启动（客户端）时，从后端获取字体配置
 * - 将字体设置应用到页面，设置 CSS 变量
 * - 确保后台设置的字体在所有访客刷新页面后自动生效
 */

export default defineNuxtPlugin(async () => {
  // 只在客户端执行
  if (!process.client) return

  const api = useApi()

  // 应用字体设置到页面
  const applyFontSettings = (settings: {
    fontFamily?: string
    baseFontSize?: number
    defaultFontWeight?: string
    defaultLineHeight?: number
    h1FontSize?: number
    h2FontSize?: number
    h3FontSize?: number
    h4FontSize?: number
  }) => {
    const root = document.documentElement
    
    // 应用字体族
    if (settings.fontFamily) {
      root.style.setProperty('--font-family-base', settings.fontFamily)
      document.body.style.fontFamily = settings.fontFamily
    }
    
    // 应用基础字号
    if (settings.baseFontSize) {
      root.style.setProperty('--font-size-base', `${settings.baseFontSize}px`)
      document.body.style.fontSize = `${settings.baseFontSize}px`
    }
    
    // 应用默认字重
    if (settings.defaultFontWeight) {
      root.style.setProperty('--font-weight-base', settings.defaultFontWeight)
      document.body.style.fontWeight = settings.defaultFontWeight
    }
    
    // 应用默认行高
    if (settings.defaultLineHeight) {
      root.style.setProperty('--line-height-base', String(settings.defaultLineHeight))
      document.body.style.lineHeight = String(settings.defaultLineHeight)
    }
    
    // 应用标题字号
    if (settings.h1FontSize) {
      root.style.setProperty('--font-size-h1', `${settings.h1FontSize}px`)
    }
    if (settings.h2FontSize) {
      root.style.setProperty('--font-size-h2', `${settings.h2FontSize}px`)
    }
    if (settings.h3FontSize) {
      root.style.setProperty('--font-size-h3', `${settings.h3FontSize}px`)
    }
    if (settings.h4FontSize) {
      root.style.setProperty('--font-size-h4', `${settings.h4FontSize}px`)
    }
    if (settings.textMainColor) {
      root.style.setProperty('--color-text-main', settings.textMainColor)
      document.body.style.color = settings.textMainColor
    }
    if (settings.textMutedColor) {
      root.style.setProperty('--color-text-muted', settings.textMutedColor)
    }
  }

  // ==================== 从后端获取字体设置 ====================
  try {
    const configs = await api.get<Record<string, string>>('/Config')
    
    if (configs) {
      const fontSettings: {
        fontFamily?: string
        baseFontSize?: number
        defaultFontWeight?: string
        defaultLineHeight?: number
        h1FontSize?: number
        h2FontSize?: number
        h3FontSize?: number
        h4FontSize?: number
        textMainColor?: string
        textMutedColor?: string
      } = {}

      // 解析字体设置
      if (configs.font_family) {
        fontSettings.fontFamily = configs.font_family
      }
      if (configs.base_font_size) {
        fontSettings.baseFontSize = parseInt(configs.base_font_size) || 16
      }
      if (configs.default_font_weight) {
        fontSettings.defaultFontWeight = configs.default_font_weight
      }
      if (configs.default_line_height) {
        fontSettings.defaultLineHeight = parseFloat(configs.default_line_height) || 1.6
      }
      if (configs.h1_font_size) {
        fontSettings.h1FontSize = parseInt(configs.h1_font_size) || 32
      }
      if (configs.h2_font_size) {
        fontSettings.h2FontSize = parseInt(configs.h2_font_size) || 24
      }
      if (configs.h3_font_size) {
        fontSettings.h3FontSize = parseInt(configs.h3_font_size) || 20
      }
      if (configs.h4_font_size) {
        fontSettings.h4FontSize = parseInt(configs.h4_font_size) || 18
      }
      if (configs.text_main_color) {
        fontSettings.textMainColor = configs.text_main_color
      }
      if (configs.text_muted_color) {
        fontSettings.textMutedColor = configs.text_muted_color
      }

      // 如果有任何字体设置，则应用
      if (Object.keys(fontSettings).length > 0) {
        applyFontSettings(fontSettings)
      }
    }
  } catch (error) {
    // 字体设置获取失败不影响应用启动
  }
})

