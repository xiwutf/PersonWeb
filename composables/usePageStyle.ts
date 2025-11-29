/**
 * 前端页面样式配置 Composable
 * 用于动态加载和管理前端页面的样式配置
 */

export interface PageStyleConfig {
  primaryColor: string
  secondaryColor: string
  backgroundColor: string
  textColor: string
  cardBackground: string
  borderColor: string
  borderRadius: string
  fontFamily: string
  [key: string]: any
}

export interface StyleVariable {
  id: number
  pageKey: string
  variableKey: string
  variableValue: string
  variableType: string
  description?: string
}

export interface StyleRule {
  id: number
  pageKey: string
  selector: string
  cssProperties: Record<string, string>
  priority: number
  enabled: boolean
  description?: string
}

/**
 * 使用页面样式配置
 */
export const usePageStyle = (pageKey: string) => {
  const api = useApi()
  const styleConfig = ref<PageStyleConfig | null>(null)
  const styleVariables = ref<StyleVariable[]>([])
  const styleRules = ref<StyleRule[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  /**
   * 获取页面样式配置
   */
  const fetchStyleConfig = async () => {
    try {
      loading.value = true
      error.value = null

      // 获取样式配置
      const configRes = await api.get<{
        pageKey: string
        pageName: string
        styleConfig: PageStyleConfig
        enabled: boolean
        version: number
      }>(`/frontend-styles/page/${pageKey}`)

      if (configRes && configRes.styleConfig) {
        styleConfig.value = configRes.styleConfig
      }

      // 获取样式变量
      const variablesRes = await api.get<StyleVariable[]>(
        `/frontend-styles/variables/${pageKey}`
      )
      if (variablesRes && Array.isArray(variablesRes)) {
        styleVariables.value = variablesRes
      }

      // 获取样式规则
      const rulesRes = await api.get<StyleRule[]>(
        `/frontend-styles/rules/${pageKey}`
      )
      if (rulesRes && Array.isArray(rulesRes)) {
        styleRules.value = rulesRes.filter(r => r.enabled)
      }

      // 应用样式
      applyStyles()
    } catch (e: any) {
      console.error(`Failed to fetch style config for ${pageKey}:`, e)
      error.value = e.message || '加载样式配置失败'
      // 使用默认样式
      loadDefaultStyles()
    } finally {
      loading.value = false
    }
  }

  /**
   * 应用样式到页面
   */
  const applyStyles = () => {
    if (typeof window === 'undefined') return

    // 创建或获取样式元素
    let styleElement = document.getElementById(`page-style-${pageKey}`)
    if (!styleElement) {
      styleElement = document.createElement('style')
      styleElement.id = `page-style-${pageKey}`
      document.head.appendChild(styleElement)
    }

    let css = ''

    // 1. 应用 CSS 变量
    if (styleConfig.value) {
      css += `:root {\n`
      Object.entries(styleConfig.value).forEach(([key, value]) => {
        const cssVar = `--${pageKey}-${key.replace(/([A-Z])/g, '-$1').toLowerCase()}`
        css += `  ${cssVar}: ${value};\n`
      })
      css += `}\n\n`
    }

    // 2. 应用样式变量
    styleVariables.value.forEach(variable => {
      const cssVar = `--${pageKey}-${variable.variableKey}`
      css += `:root {\n  ${cssVar}: ${variable.variableValue};\n}\n\n`
    })

    // 3. 应用样式规则
    styleRules.value
      .sort((a, b) => b.priority - a.priority)
      .forEach(rule => {
        css += `${rule.selector} {\n`
        Object.entries(rule.cssProperties).forEach(([prop, value]) => {
          css += `  ${prop}: ${value};\n`
        })
        css += `}\n\n`
      })

    styleElement.textContent = css
  }

  /**
   * 加载默认样式
   */
  const loadDefaultStyles = () => {
    // 默认样式配置
    const defaultConfigs: Record<string, PageStyleConfig> = {
      tools: {
        primaryColor: '#f97316',
        secondaryColor: '#dc2626',
        backgroundColor: '#0f172a',
        textColor: '#e2e8f0',
        cardBackground: 'rgba(30, 41, 59, 0.3)',
        borderColor: 'rgba(255, 255, 255, 0.05)',
        borderRadius: '1.5rem',
        fontFamily: '"Outfit", sans-serif'
      },
      blog: {
        primaryColor: '#3b82f6',
        secondaryColor: '#10b981',
        backgroundColor: '#0f172a',
        textColor: '#e2e8f0',
        cardBackground: 'rgba(30, 41, 59, 0.3)',
        borderColor: 'rgba(255, 255, 255, 0.05)',
        borderRadius: '1rem',
        fontFamily: '"Outfit", sans-serif'
      },
      life: {
        primaryColor: '#f59e0b',
        secondaryColor: '#f97316',
        backgroundColor: '#0f172a',
        textColor: '#e2e8f0',
        cardBackground: 'rgba(30, 41, 59, 0.4)',
        borderColor: 'rgba(255, 255, 255, 0.05)',
        borderRadius: '1rem',
        fontFamily: '"Outfit", sans-serif'
      },
      projects: {
        primaryColor: '#3b82f6',
        secondaryColor: '#10b981',
        backgroundColor: '#ffffff',
        textColor: '#111827',
        cardBackground: '#ffffff',
        borderColor: '#e5e7eb',
        borderRadius: '0.5rem',
        fontFamily: 'system-ui, sans-serif'
      },
      about: {
        primaryColor: '#8b5cf6',
        secondaryColor: '#ec4899',
        backgroundColor: '#0f172a',
        textColor: '#e2e8f0',
        cardBackground: 'rgba(30, 41, 59, 0.3)',
        borderColor: 'rgba(255, 255, 255, 0.05)',
        borderRadius: '1.5rem',
        fontFamily: '"Outfit", sans-serif'
      }
    }

    styleConfig.value = defaultConfigs[pageKey] || defaultConfigs.tools
    applyStyles()
  }

  /**
   * 获取 CSS 变量值
   */
  const getCssVar = (key: string): string => {
    if (typeof window === 'undefined') return ''
    const cssVar = `--${pageKey}-${key.replace(/([A-Z])/g, '-$1').toLowerCase()}`
    return getComputedStyle(document.documentElement).getPropertyValue(cssVar).trim()
  }

  /**
   * 更新样式配置
   */
  const updateStyleConfig = async (config: Partial<PageStyleConfig>) => {
    try {
      await api.put(`/frontend-styles/page/${pageKey}`, {
        styleConfig: { ...styleConfig.value, ...config }
      })
      await fetchStyleConfig()
    } catch (e: any) {
      console.error('Failed to update style config:', e)
      throw e
    }
  }

  // 初始化时加载样式
  onMounted(() => {
    fetchStyleConfig()
  })

  return {
    styleConfig: readonly(styleConfig),
    styleVariables: readonly(styleVariables),
    styleRules: readonly(styleRules),
    loading: readonly(loading),
    error: readonly(error),
    fetchStyleConfig,
    updateStyleConfig,
    getCssVar,
    applyStyles
  }
}

