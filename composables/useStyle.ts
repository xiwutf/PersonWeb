/**
 * 样式管理 Composable
 * 用于动态加载样式并记录使用统计
 */

interface StyleDefinition {
  id: number
  code: string
  cssClass: string
  backgroundColor?: string
  borderColor?: string
  textColor?: string
  fontSize?: string
  fontWeight?: string
  padding?: string
  borderRadius?: string
  borderWidth?: string
  customCss?: string
}

const styleCache = new Map<string, StyleDefinition>()
const styleCacheTime = new Map<string, number>()
const CACHE_DURATION = 5 * 60 * 1000 // 5分钟缓存

/**
 * 获取样式定义
 */
export const useStyle = () => {
  const api = useApi()

  /**
   * 根据代码获取样式
   */
  const getStyle = async (code: string): Promise<StyleDefinition | null> => {
    // 检查缓存
    const cached = styleCache.get(code)
    const cacheTime = styleCacheTime.get(code)
    if (cached && cacheTime && Date.now() - cacheTime < CACHE_DURATION) {
      return cached
    }

    try {
      const style = await api.get<StyleDefinition>(`/StyleManagement/code/${code}`)
      if (style) {
        styleCache.set(code, style)
        styleCacheTime.set(code, Date.now())
        return style
      }
    } catch (e) {
      console.warn(`样式 ${code} 不存在或加载失败`, e)
    }

    return null
  }

  /**
   * 获取样式的内联样式对象
   */
  const getStyleObject = async (code: string): Promise<Record<string, string>> => {
    const style = await getStyle(code)
    if (!style) return {}

    const styleObj: Record<string, string> = {}

    if (style.backgroundColor) {
      styleObj.backgroundColor = style.backgroundColor
    }
    if (style.borderColor) {
      styleObj.borderColor = style.borderColor
    }
    if (style.textColor) {
      styleObj.color = style.textColor
    }
    if (style.fontSize) {
      styleObj.fontSize = style.fontSize
    }
    if (style.fontWeight) {
      styleObj.fontWeight = style.fontWeight
    }
    if (style.padding) {
      styleObj.padding = style.padding
    }
    if (style.borderRadius) {
      styleObj.borderRadius = style.borderRadius
    }
    if (style.borderWidth) {
      styleObj.borderWidth = style.borderWidth
    }

    // 解析自定义CSS
    if (style.customCss) {
      try {
        const custom = JSON.parse(style.customCss)
        Object.assign(styleObj, custom)
      } catch (e) {
        console.warn(`样式 ${code} 的自定义CSS解析失败`, e)
      }
    }

    return styleObj
  }

  /**
   * 记录样式使用
   */
  const recordUsage = async (code: string, componentName?: string) => {
    if (process.server) return // 服务端不记录

    try {
      const style = await getStyle(code)
      if (!style) return

      const route = useRoute()
      const pagePath = route.path

      // 异步记录，不阻塞页面
      api.post('/StyleManagement/usage', {
        styleId: style.id,
        pagePath,
        componentName
      }).catch(err => {
        console.warn('记录样式使用失败', err)
      })
    } catch (e) {
      console.warn('记录样式使用失败', e)
    }
  }

  /**
   * 获取样式类名（同步方法，使用缓存）
   */
  const getStyleClass = (code: string): string => {
    const cached = styleCache.get(code)
    return cached?.cssClass || code
  }

  /**
   * 清除样式缓存
   */
  const clearStyleCache = () => {
    styleCache.clear()
    styleCacheTime.clear()
  }

  return {
    getStyle,
    getStyleObject,
    getStyleClass,
    recordUsage,
    clearStyleCache
  }
}

/**
 * 样式使用组件辅助函数
 * 在组件中使用样式时自动记录使用统计
 */
export const useStyleWithTracking = (code: string, componentName?: string) => {
  const { getStyleClass, recordUsage } = useStyle()

  // 在客户端记录使用
  if (process.client) {
    recordUsage(code, componentName)
  }

  return {
    styleClass: getStyleClass(code),
    code
  }
}

