/**
 * 字体样式管理 Composable
 * 用于动态加载和应用字体样式
 */

import { useStyle } from './useStyle'

interface FontStyle {
  fontFamily?: string
  fontSize?: string
  fontWeight?: string
  lineHeight?: string
  letterSpacing?: string
  color?: string
}

const fontStyleCache = new Map<string, FontStyle>()

/**
 * 获取字体样式
 */
export const useFontStyle = () => {
  const { getStyle } = useStyle()

  /**
   * 根据代码获取字体样式对象
   */
  const getFontStyle = async (code: string): Promise<FontStyle> => {
    // 检查缓存
    const cached = fontStyleCache.get(code)
    if (cached) {
      return cached
    }

    try {
      const style = await getStyle(code)
      if (!style) {
        return {}
      }

      const fontStyle: FontStyle = {}

      if (style.textColor) {
        fontStyle.color = style.textColor
      }
      if (style.fontSize) {
        fontStyle.fontSize = style.fontSize
      }
      if (style.fontWeight) {
        fontStyle.fontWeight = style.fontWeight
      }

      // 解析自定义CSS
      if (style.customCss) {
        try {
          const custom = JSON.parse(style.customCss)
          if (custom['font-family']) {
            fontStyle.fontFamily = custom['font-family']
          }
          if (custom['line-height']) {
            fontStyle.lineHeight = custom['line-height']
          }
          if (custom['letter-spacing']) {
            fontStyle.letterSpacing = custom['letter-spacing']
          }
          // 合并其他自定义属性
          Object.assign(fontStyle, custom)
        } catch (e) {
          console.warn(`字体样式 ${code} 的自定义CSS解析失败`, e)
        }
      }

      // 缓存样式
      fontStyleCache.set(code, fontStyle)
      return fontStyle
    } catch (e) {
      console.warn(`字体样式 ${code} 加载失败`, e)
      return {}
    }
  }

  /**
   * 获取字体样式的内联样式对象（用于 :style 绑定）
   */
  const getFontStyleObject = async (code: string): Promise<Record<string, string>> => {
    const fontStyle = await getFontStyle(code)
    const styleObj: Record<string, string> = {}

    if (fontStyle.fontFamily) {
      styleObj.fontFamily = fontStyle.fontFamily
    }
    if (fontStyle.fontSize) {
      styleObj.fontSize = fontStyle.fontSize
    }
    if (fontStyle.fontWeight) {
      styleObj.fontWeight = fontStyle.fontWeight
    }
    if (fontStyle.lineHeight) {
      styleObj.lineHeight = fontStyle.lineHeight
    }
    if (fontStyle.letterSpacing) {
      styleObj.letterSpacing = fontStyle.letterSpacing
    }
    if (fontStyle.color) {
      styleObj.color = fontStyle.color
    }

    return styleObj
  }

  /**
   * 获取字体样式的 CSS 变量格式（用于 CSS 变量）
   */
  const getFontStyleVariables = async (code: string, prefix: string = ''): Promise<Record<string, string>> => {
    const fontStyle = await getFontStyle(code)
    const vars: Record<string, string> = {}

    const varPrefix = prefix ? `--${prefix}-` : '--font-'

    if (fontStyle.fontFamily) {
      vars[`${varPrefix}family`] = fontStyle.fontFamily
    }
    if (fontStyle.fontSize) {
      vars[`${varPrefix}size`] = fontStyle.fontSize
    }
    if (fontStyle.fontWeight) {
      vars[`${varPrefix}weight`] = fontStyle.fontWeight
    }
    if (fontStyle.lineHeight) {
      vars[`${varPrefix}line-height`] = fontStyle.lineHeight
    }
    if (fontStyle.letterSpacing) {
      vars[`${varPrefix}letter-spacing`] = fontStyle.letterSpacing
    }
    if (fontStyle.color) {
      vars[`${varPrefix}color`] = fontStyle.color
    }

    return vars
  }

  /**
   * 清除字体样式缓存
   */
  const clearFontStyleCache = () => {
    fontStyleCache.clear()
  }

  return {
    getFontStyle,
    getFontStyleObject,
    getFontStyleVariables,
    clearFontStyleCache
  }
}

