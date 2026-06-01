/**
 * 设计令牌配置文件
 * 
 * 职责：
 * - 定义基础色板（原始颜色值）
 * - 定义主题类型和主题变量映射
 * - 集中管理所有设计相关的变量，便于后续多项目共享
 * 
 * 重构说明（2024-12-XX）：
 * - 精简为仅支持 light（浅色）和 dark（深色）两个主题
 * - 旧主题（tech-blue、paper、forest、hybrid-super 等）已移除
 * - 使用 normalizeTheme() 函数将旧主题自动映射为 light 或 dark
 */

// ==================== 基础色板 ====================
/**
 * 基础色板定义
 * 这里定义原始颜色值，不包含语义含义
 */
export const palette = {
  // 蓝色系
  blue: {
    50: '#eff6ff',
    100: '#dbeafe',
    200: '#bfdbfe',
    300: '#93c5fd',
    400: '#60a5fa',
    500: '#3b82f6',
    600: '#2563eb',
    700: '#1d4ed8',
    800: '#1e40af',
    900: '#1e3a8a',
    950: '#172554'
  },
  // 灰色系
  gray: {
    50: '#f9fafb',
    100: '#f3f4f6',
    200: '#e5e7eb',
    300: '#d1d5db',
    400: '#9ca3af',
    500: '#6b7280',
    600: '#4b5563',
    700: '#374151',
    800: '#1f2937',
    900: '#111827',
    950: '#030712'
  },
  // 科技蓝（保留用于向后兼容，但不再使用）
  techBlue: {
    50: '#ecfeff',
    100: '#cffafe',
    200: '#a5f3fc',
    300: '#67e8f9',
    400: '#22d3ee',
    500: '#06b6d4',
    600: '#0891b2',
    700: '#0e7490',
    800: '#155e75',
    900: '#164e63',
    950: '#083344'
  }
} as const

// ==================== 主题类型定义 ====================
/**
 * 主题名称类型
 * 
 * 重构说明（2024-12-XX）：
 * - 精简为仅支持 light（浅色）和 dark（深色）两个主题
 * - 旧主题（tech-blue、paper、forest、hybrid-super 等）已移除
 * - 使用 normalizeTheme() 函数将旧主题自动映射为 light 或 dark
 * 
 * 主题说明：
 * - light: 清爽浅色通用主题（默认），适合通用网站、后台管理
 * - dark: 深色极简主题，适合夜间浏览、科技类产品
 */
export type ThemeName = 'light' | 'dark'

/**
 * 主题键类型（与 ThemeName 相同，用于 tokens 系统）
 * 注意：后端 AvailableThemes 列表必须与此类型保持一致
 */
export type ThemeKey = ThemeName

// ==================== 主题变量定义 ====================
/**
 * 主题变量定义（已精简，仅保留 light 和 dark）
 * 
 * 注意：此对象主要用于向后兼容，实际样式定义在 tokens.css 中
 * 这里保留是为了 resolveThemeTokens 函数能正常工作
 */
export const themeVariables: Record<ThemeName, Record<string, string>> = {
  // 浅色主题
  light: {
    '--color-bg-body': '#ffffff',
    '--color-bg-card': '#ffffff',
    '--color-bg-elevated': '#f8fafc',
    '--color-text-main': '#0f172a',
    '--color-text-muted': '#64748b',
    '--color-text-disabled': '#94a3b8',
    '--color-primary': '#2563eb',
    '--color-primary-soft': '#dbeafe',
    '--color-primary-hover': '#1d4ed8',
    '--color-border-subtle': '#e2e8f0',
    '--color-border-default': '#e2e8f0',
    '--color-border-strong': '#cbd5e1',
    '--shadow-sm': '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    '--shadow-md': '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
    '--shadow-lg': '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    '--radius-sm': '0.375rem',
    '--radius-md': '0.5rem',
    '--radius-lg': '0.75rem',
    '--radius-xl': '1rem'
  },
  
  // 深色主题
  dark: {
    '--color-bg-body': '#0a0e1a',
    '--color-bg-card': 'rgba(19, 23, 32, 0.8)',
    '--color-bg-elevated': '#131720',
    '--color-text-main': 'rgba(255, 255, 255, 0.92)',
    '--color-text-muted': 'rgba(255, 255, 255, 0.6)',
    '--color-text-disabled': 'rgba(255, 255, 255, 0.4)',
    '--color-primary': '#60a5fa',
    '--color-primary-soft': 'rgba(96, 165, 250, 0.15)',
    '--color-primary-hover': '#3b82f6',
    '--color-border-subtle': 'rgba(255, 255, 255, 0.1)',
    '--color-border-default': 'rgba(255, 255, 255, 0.1)',
    '--color-border-strong': 'rgba(255, 255, 255, 0.15)',
    '--shadow-sm': '0 1px 2px 0 rgb(0 0 0 / 0.3)',
    '--shadow-md': '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.4)',
    '--shadow-lg': '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)',
    '--radius-sm': '0.375rem',
    '--radius-md': '0.5rem',
    '--radius-lg': '0.75rem',
    '--radius-xl': '1rem'
  }
} as const

// ==================== 默认主题 ====================
/**
 * 默认主题名称
 */
export const DEFAULT_THEME: ThemeName = 'dark'

// ==================== 本地存储键名 ====================
/**
 * 主题在 localStorage 中的键名
 * 统一使用此键名，便于后续多项目共享
 */
export const THEME_STORAGE_KEY = 'site-theme'

// ==================== 默认主题 Tokens（已精简，仅保留 light 和 dark）====================
/**
 * 默认主题 tokens（仅保留 light 和 dark）
 * 
 * 用途：
 * - 作为前端内置的默认样式值
 * - 当后端没有提供 tokens 覆盖时使用
 * - 当后端只覆盖部分字段时，未覆盖的字段使用这里的默认值
 * 
 * 格式说明：
 * - key 使用点号分隔的路径格式，例如 "color.bg.body"
 * - value 为 CSS 值，例如颜色值 "#ffffff" 或 "rgba(0,0,0,0.1)"
 * 
 * 注意：
 * - 这些值会被后端返回的 tokens 覆盖（如果后端有配置）
 * - 后端覆盖的优先级更高
 */
export const defaultThemeTokens: Record<ThemeKey, Record<string, string>> = {
  /**
   * 清爽浅色通用主题（默认）
   * 适合：通用网站、后台管理、企业官网
   * 特点：清爽明亮，对比度高，适合长时间阅读
   */
  light: {
    'color.bg.body': '#ffffff',
    'color.bg.card': '#ffffff',
    'color.bg.elevated': '#f8fafc',
    'color.text.main': '#0f172a',
    'color.text.muted': '#64748b',
    'color.text.disabled': '#94a3b8',
    'color.primary': '#2563eb',
    'color.primary.soft': '#dbeafe',
    'color.primary.hover': '#1d4ed8',
    'color.border.subtle': '#e2e8f0',
    'color.border.default': '#e2e8f0',
    'color.border.strong': '#cbd5e1',
    'shadow.sm': '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    'shadow.md': '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
    'shadow.lg': '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（用于饼状图、折线图等）
    'chart.primary': '#3b82f6',
    'chart.secondary': '#10b981',
    'chart.tertiary': '#f59e0b',
    'chart.quaternary': '#ef4444',
    'chart.quinary': '#8b5cf6',
    'chart.senary': '#ec4899',
    'chart.septenary': '#06b6d4',
    'chart.octonary': '#84cc16',
    'chart.nonary': '#f97316',
    'chart.denary': '#6366f1'
  },
  /**
   * 深色极简主题
   * 适合：夜间浏览、科技类产品、开发者工具
   * 特点：极简深色，护眼，适合长时间使用
   */
  dark: {
    'color.bg.body': '#0a0e1a',
    'color.bg.card': 'rgba(19, 23, 32, 0.8)',
    'color.bg.elevated': '#131720',
    'color.text.main': 'rgba(255, 255, 255, 0.92)',
    'color.text.muted': 'rgba(255, 255, 255, 0.6)',
    'color.text.disabled': 'rgba(255, 255, 255, 0.4)',
    'color.primary': '#60a5fa',
    'color.primary.soft': 'rgba(96, 165, 250, 0.15)',
    'color.primary.hover': '#3b82f6',
    'color.border.subtle': 'rgba(255, 255, 255, 0.1)',
    'color.border.default': 'rgba(255, 255, 255, 0.1)',
    'color.border.strong': 'rgba(255, 255, 255, 0.15)',
    'shadow.sm': '0 1px 2px 0 rgb(0 0 0 / 0.3)',
    'shadow.md': '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.4)',
    'shadow.lg': '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（深色主题，使用更亮的颜色）
    'chart.primary': '#60a5fa',
    'chart.secondary': '#34d399',
    'chart.tertiary': '#fbbf24',
    'chart.quaternary': '#f87171',
    'chart.quinary': '#a78bfa',
    'chart.senary': '#f472b6',
    'chart.septenary': '#22d3ee',
    'chart.octonary': '#a3e635',
    'chart.nonary': '#fb923c',
    'chart.denary': '#818cf8'
  }
} as const

// 注意：以下旧主题定义已删除，使用 normalizeTheme() 函数自动映射
// - tech-blue → dark
// - paper → light
// - forest → dark
// - hybrid-super → dark
// - hybrid-super-dark → dark
// - hybrid-super-light → light

/**
 * 主题映射函数：将旧主题名称映射为 light 或 dark
 * 
 * 映射规则：
 * - 如果主题名包含 "light" 或 "paper" → 映射为 "light"
 * - 如果主题名包含 "dark" 或 "tech" 或 "forest" 或 "hybrid" → 映射为 "dark"
 * - 其他情况 → 默认映射为 "dark"
 * 
 * 用途：
 * - 兼容数据库中的旧主题配置（tech-blue、paper、forest、hybrid-super 等）
 * - 避免因旧数据导致主题切换失败
 * 
 * @param theme 主题名称（可能是旧主题名）
 * @returns 标准化后的主题名称（'light' 或 'dark'）
 */
export function normalizeTheme(theme: string | null | undefined): ThemeName {
  if (!theme) return DEFAULT_THEME
  
  const themeLower = theme.toLowerCase()
  
  // 如果已经是 light 或 dark，直接返回
  if (themeLower === 'light' || themeLower === 'dark') {
    return themeLower as ThemeName
  }
  
  // 根据关键词映射
  if (themeLower.includes('light') || themeLower.includes('paper')) {
    return 'light'
  }
  
  // 深色主题关键词
  if (themeLower.includes('dark') || 
      themeLower.includes('tech') || 
      themeLower.includes('forest') || 
      themeLower.includes('hybrid') ||
      themeLower.includes('lab')) {
    return 'dark'
  }
  
  // 默认返回 dark（更安全，避免浅色主题在深色背景下不可见）
  return 'dark'
}

/**
 * 将 tokens 格式（点号分隔）转换为 CSS 变量格式（双横线 + 横线分隔）
 * 
 * 例如：
 * - "color.bg.body" -> "--color-bg-body"
 * - "color.primary" -> "--color-primary"
 */
function tokenKeyToCssVar(key: string): string {
  return `--${key.replace(/\./g, '-')}`
}

/**
 * 将 CSS 变量格式转换为 tokens 格式
 * 
 * 例如：
 * - "--color-bg-body" -> "color.bg.body"
 * - "--color-primary" -> "color.primary"
 */
function cssVarToTokenKey(cssVar: string): string {
  return cssVar.replace(/^--/, '').replace(/-/g, '.')
}

/**
 * 合并默认 tokens 和后端覆盖的 tokens
 * 
 * 用途：
 * - 前端使用默认 tokens 作为基础
 * - 后端返回的 tokens 会覆盖默认值中对应的字段
 * - 如果后端没有提供某个字段，使用默认值
 * 
 * 优先级：后端覆盖 > 默认值
 * 
 * @param themeKey 主题键，必须是 "light" 或 "dark"（已通过 normalizeTheme 处理）
 * @param overrides 后端返回的 tokens 覆盖（可选）
 * @returns 合并后的 tokens，格式为 CSS 变量键值对（--color-xxx: value）
 */
export function resolveThemeTokens(
  themeKey: ThemeKey,
  overrides?: Record<string, string>
): Record<string, string> {
  // 1. 先标准化主题键（兼容旧数据）
  const normalizedKey = normalizeTheme(themeKey) as ThemeKey
  
  // 2. 获取默认 tokens
  const defaultTokens = defaultThemeTokens[normalizedKey] || defaultThemeTokens.light

  // 3. 将默认 tokens 转换为 CSS 变量格式
  const defaultCssVars: Record<string, string> = {}
  Object.entries(defaultTokens).forEach(([key, value]) => {
    defaultCssVars[tokenKeyToCssVar(key)] = value
  })

  // 4. 如果有后端覆盖，将覆盖值也转换为 CSS 变量格式并合并
  if (overrides && Object.keys(overrides).length > 0) {
    Object.entries(overrides).forEach(([key, value]) => {
      // 后端返回的 key 可能是 "color.bg.body" 格式，需要转换为 "--color-bg-body"
      const cssVarKey = key.startsWith('--') ? key : tokenKeyToCssVar(key)
      defaultCssVars[cssVarKey] = value
    })
  }

  return defaultCssVars
}
