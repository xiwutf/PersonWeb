import { ref, computed } from 'vue'
import type { AdminGlobalStyle, AdminModuleStyle } from '~/types/api'

// 全局风格状态（使用 reactive 以便在组件间共享）
export const globalStyleState = {
  globalStyle: ref<AdminGlobalStyle | null>(null),
  moduleStyles: ref<Record<string, AdminModuleStyle>>({}),
  loading: ref(false)
}

/**
 * 获取当前全局风格
 */
export const useAdminGlobalStyle = () => {
  const api = useApi()
  const { globalStyle, loading } = globalStyleState

  const fetchGlobalStyle = async () => {
    loading.value = true
    try {
      const res = await api.get<AdminGlobalStyle>('/AdminStyle/global/current')
      if (res) {
        globalStyle.value = res
      }
    } catch (error) {
      console.error('Failed to fetch global style:', error)
      // 使用默认风格
      globalStyle.value = null
    } finally {
      loading.value = false
    }
  }

  // 解析风格配置
  const styleConfig = computed(() => {
    if (!globalStyle.value?.styleConfig) {
      return getDefaultStyleConfig()
    }
    try {
      return JSON.parse(globalStyle.value.styleConfig)
    } catch {
      return getDefaultStyleConfig()
    }
  })

  // 获取 CSS 变量
  const cssVariables = computed(() => {
    const config = styleConfig.value
    const vars: Record<string, string> = {}

    // 背景
    if (config.background) {
      if (config.background.type === 'gradient' && config.background.colors) {
        vars['--admin-bg-gradient'] = `linear-gradient(135deg, ${config.background.colors.join(', ')})`
      } else if (config.background.color) {
        vars['--admin-bg-color'] = config.background.color
      }
      
      if (config.background.grid?.enabled) {
        vars['--admin-grid-opacity'] = String(config.background.grid.opacity || 0.03)
        vars['--admin-grid-size'] = `${config.background.grid.size || 40}px`
      }
    }

    // 侧边栏
    if (config.sidebar) {
      vars['--admin-sidebar-bg'] = config.sidebar.bgColor || '#1e293b'
      vars['--admin-sidebar-text'] = config.sidebar.textColor || '#ffffff'
      vars['--admin-sidebar-active-bg'] = config.sidebar.activeBgColor || '#3b82f6'
      vars['--admin-sidebar-hover-bg'] = config.sidebar.hoverBgColor || '#334155'
    }

    // 头部
    if (config.header) {
      vars['--admin-header-bg'] = config.header.bgColor || '#ffffff'
      vars['--admin-header-text'] = config.header.textColor || '#1e293b'
      vars['--admin-header-border'] = config.header.borderColor || '#e2e8f0'
    }

    // 卡片
    if (config.card) {
      vars['--admin-card-bg'] = config.card.bgColor || '#ffffff'
      vars['--admin-card-border'] = config.card.borderColor || '#e2e8f0'
      vars['--admin-card-shadow'] = config.card.shadow || '0 1px 3px rgba(0,0,0,0.1)'
    }

    // 主题色
    vars['--admin-primary'] = config.primaryColor || '#3b82f6'
    vars['--admin-secondary'] = config.secondaryColor || '#8b5cf6'
    vars['--admin-accent'] = config.accentColor || '#06b6d4'

    return vars
  })

  // 获取内联样式
  const inlineStyle = computed(() => {
    const vars = cssVariables.value
    return Object.entries(vars)
      .map(([key, value]) => `${key}: ${value}`)
      .join('; ')
  })

  return {
    globalStyle,
    styleConfig,
    cssVariables,
    inlineStyle,
    fetchGlobalStyle,
    loading: computed(() => loading.value)
  }
}

/**
 * 获取模块风格
 */
export const useAdminModuleStyle = (moduleKey: string) => {
  const api = useApi()
  const { moduleStyles } = globalStyleState

  const fetchModuleStyle = async () => {
    try {
      const res = await api.get<AdminModuleStyle>(`/AdminStyle/module/${moduleKey}`)
      if (res) {
        moduleStyles.value[moduleKey] = res
      }
    } catch (error) {
      console.error(`Failed to fetch module style for ${moduleKey}:`, error)
    }
  }

  // 解析模块风格配置
  const moduleStyleConfig = computed(() => {
    const style = moduleStyles.value[moduleKey]
    if (!style?.styleConfig) {
      return null
    }
    try {
      return JSON.parse(style.styleConfig)
    } catch {
      return null
    }
  })

  // 获取模块样式类
  const moduleStyleClasses = computed(() => {
    const config = moduleStyleConfig.value
    if (!config) return ''

    const classes: string[] = []
    
    // 背景渐变
    if (config.bgGradient && Array.isArray(config.bgGradient)) {
      classes.push(`bg-gradient-to-br from-[${config.bgGradient[0]}] to-[${config.bgGradient[1]}]`)
    }

    return classes.join(' ')
  })

  // 获取模块样式变量
  const moduleStyleVars = computed(() => {
    const config = moduleStyleConfig.value
    if (!config) return {}

    const vars: Record<string, string> = {}
    
    if (config.iconColor) {
      vars['--module-icon-color'] = config.iconColor
    }
    if (config.accentColor) {
      vars['--module-accent-color'] = config.accentColor
    }

    return vars
  })

  return {
    moduleStyle: computed(() => moduleStyles.value[moduleKey]),
    moduleStyleConfig,
    moduleStyleClasses,
    moduleStyleVars,
    fetchModuleStyle
  }
}

/**
 * 默认风格配置
 */
function getDefaultStyleConfig() {
  return {
    background: {
      type: 'gradient',
      colors: ['#0f172a', '#1e293b', '#334155'],
      grid: {
        enabled: true,
        opacity: 0.03,
        size: 40
      }
    },
    sidebar: {
      bgColor: '#1e293b',
      textColor: '#ffffff',
      activeBgColor: '#3b82f6',
      hoverBgColor: '#334155'
    },
    header: {
      bgColor: '#ffffff',
      textColor: '#1e293b',
      borderColor: '#e2e8f0'
    },
    card: {
      bgColor: '#ffffff',
      borderColor: '#e2e8f0',
      shadow: '0 1px 3px rgba(0,0,0,0.1)'
    },
    primaryColor: '#3b82f6',
    secondaryColor: '#8b5cf6',
    accentColor: '#06b6d4'
  }
}

