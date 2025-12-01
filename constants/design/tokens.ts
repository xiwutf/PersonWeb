/**
 * 设计令牌配置文件
 * 
 * 职责：
 * - 定义基础色板（原始颜色值）
 * - 定义主题类型和主题变量映射
 * - 集中管理所有设计相关的变量，便于后续多项目共享
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
  // 科技蓝（用于 tech-blue 主题）
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
 * 支持5套成型主题方案：
 * - light: 清爽浅色通用主题（默认），适合通用网站、后台管理
 * - dark: 深色极简主题，适合夜间浏览、科技类产品
 * - tech-blue: 科技蓝霓虹主题，适合 AI 实验室、技术展示模块
 * - paper: 纸张阅读风格，适合博客、文章阅读场景
 * - forest: 自然墨绿主题，适合自然、环保类内容模块
 */
export type ThemeName = 'light' | 'dark' | 'tech-blue' | 'paper' | 'forest' | 'hybrid-super' | 'hybrid-super-dark' | 'hybrid-super-light'

/**
 * 主题键类型（与 ThemeName 相同，用于 tokens 系统）
 * 注意：后端 AvailableThemes 列表必须与此类型保持一致
 */
export type ThemeKey = ThemeName

// ==================== 主题变量定义 ====================
/**
 * 每个主题对应的 CSS 变量键值对
 * 
 * 变量命名规范：
 * - 使用语义化命名，不绑定具体业务
 * - 格式：--color-{语义}-{修饰}
 * - 例如：--color-bg-body（背景-主体）、--color-text-main（文字-主要）
 */
export const themeVariables: Record<ThemeName, Record<string, string>> = {
  // 浅色主题
  light: {
    // 背景色
    '--color-bg-body': palette.gray[50],
    '--color-bg-card': '#ffffff',
    '--color-bg-elevated': '#ffffff',
    
    // 文字颜色
    '--color-text-main': palette.gray[900],
    '--color-text-muted': palette.gray[600],
    '--color-text-disabled': palette.gray[400],
    
    // 主色调
    '--color-primary': palette.blue[600],
    '--color-primary-soft': palette.blue[100],
    '--color-primary-hover': palette.blue[700],
    
    // 边框颜色
    '--color-border-subtle': palette.gray[200],
    '--color-border-default': palette.gray[300],
    '--color-border-strong': palette.gray[400],
    
    // 阴影
    '--shadow-sm': '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    '--shadow-md': '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
    '--shadow-lg': '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    
    // 圆角
    '--radius-sm': '0.375rem',
    '--radius-md': '0.5rem',
    '--radius-lg': '0.75rem',
    '--radius-xl': '1rem'
  },
  
  // 深色主题
  dark: {
    // 背景色
    '--color-bg-body': palette.gray[950],
    '--color-bg-card': palette.gray[900],
    '--color-bg-elevated': palette.gray[800],
    
    // 文字颜色
    '--color-text-main': palette.gray[50],
    '--color-text-muted': palette.gray[400],
    '--color-text-disabled': palette.gray[600],
    
    // 主色调
    '--color-primary': palette.blue[500],
    '--color-primary-soft': palette.blue[900],
    '--color-primary-hover': palette.blue[400],
    
    // 边框颜色
    '--color-border-subtle': palette.gray[800],
    '--color-border-default': palette.gray[700],
    '--color-border-strong': palette.gray[600],
    
    // 阴影
    '--shadow-sm': '0 1px 2px 0 rgb(0 0 0 / 0.3)',
    '--shadow-md': '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.4)',
    '--shadow-lg': '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)',
    
    // 圆角
    '--radius-sm': '0.375rem',
    '--radius-md': '0.5rem',
    '--radius-lg': '0.75rem',
    '--radius-xl': '1rem'
  },
  
  // 科技蓝主题
  'tech-blue': {
    // 背景色
    '--color-bg-body': '#0a1929',
    '--color-bg-card': '#112240',
    '--color-bg-elevated': '#1a2f4f',
    
    // 文字颜色
    '--color-text-main': '#e6f1ff',
    '--color-text-muted': '#8892b0',
    '--color-text-disabled': '#495670',
    
    // 主色调（使用科技蓝）
    '--color-primary': palette.techBlue[400],
    '--color-primary-soft': palette.techBlue[900],
    '--color-primary-hover': palette.techBlue[300],
    
    // 边框颜色
    '--color-border-subtle': 'rgba(34, 211, 238, 0.1)',
    '--color-border-default': 'rgba(34, 211, 238, 0.2)',
    '--color-border-strong': 'rgba(34, 211, 238, 0.4)',
    
    // 阴影（带科技蓝光晕效果）
    '--shadow-sm': '0 1px 2px 0 rgba(34, 211, 238, 0.1)',
    '--shadow-md': '0 4px 6px -1px rgba(34, 211, 238, 0.2), 0 2px 4px -2px rgba(34, 211, 238, 0.2)',
    '--shadow-lg': '0 10px 15px -3px rgba(34, 211, 238, 0.3), 0 4px 6px -4px rgba(34, 211, 238, 0.3)',
    
    // 圆角
    '--radius-sm': '0.375rem',
    '--radius-md': '0.5rem',
    '--radius-lg': '0.75rem',
    '--radius-xl': '1rem'
  },
  
  // 混合超级风格主题（深色）
  'hybrid-super-dark': {
    // 背景色
    '--color-bg-body': '#000000',
    '--color-bg-card': 'rgba(20, 20, 30, 0.6)',
    '--color-bg-elevated': 'rgba(30, 30, 40, 0.8)',
    
    // 文字颜色
    '--color-text-main': '#ffffff',
    '--color-text-muted': 'rgba(255, 255, 255, 0.7)',
    '--color-text-disabled': 'rgba(255, 255, 255, 0.4)',
    
    // 主色调
    '--color-primary': '#00d9ff',
    '--color-primary-soft': 'rgba(0, 217, 255, 0.15)',
    '--color-primary-hover': '#00b8d4',
    
    // 边框颜色
    '--color-border-subtle': 'rgba(255, 255, 255, 0.1)',
    '--color-border-default': 'rgba(255, 255, 255, 0.2)',
    '--color-border-strong': 'rgba(255, 255, 255, 0.4)',
    
    // 阴影
    '--shadow-sm': '0 2px 8px rgba(0, 217, 255, 0.1)',
    '--shadow-md': '0 8px 24px rgba(0, 217, 255, 0.15), 0 4px 12px rgba(0, 0, 0, 0.5)',
    '--shadow-lg': '0 16px 48px rgba(0, 217, 255, 0.2), 0 8px 24px rgba(0, 0, 0, 0.6)',
    
    // 圆角
    '--radius-sm': '0.5rem',
    '--radius-md': '0.75rem',
    '--radius-lg': '1rem',
    '--radius-xl': '1.5rem'
  },
  
  // 混合超级风格主题（浅色）
  'hybrid-super-light': {
    // 背景色
    '--color-bg-body': '#ffffff',
    '--color-bg-card': 'rgba(255, 255, 255, 0.8)',
    '--color-bg-elevated': 'rgba(250, 250, 255, 0.9)',
    
    // 文字颜色
    '--color-text-main': '#0f172a',
    '--color-text-muted': 'rgba(15, 23, 42, 0.7)',
    '--color-text-disabled': 'rgba(15, 23, 42, 0.4)',
    
    // 主色调
    '--color-primary': '#0066cc',
    '--color-primary-soft': 'rgba(0, 102, 204, 0.1)',
    '--color-primary-hover': '#0052a3',
    
    // 边框颜色
    '--color-border-subtle': 'rgba(15, 23, 42, 0.1)',
    '--color-border-default': 'rgba(15, 23, 42, 0.2)',
    '--color-border-strong': 'rgba(15, 23, 42, 0.4)',
    
    // 阴影
    '--shadow-sm': '0 2px 8px rgba(0, 102, 204, 0.1)',
    '--shadow-md': '0 8px 24px rgba(0, 102, 204, 0.15), 0 4px 12px rgba(0, 0, 0, 0.1)',
    '--shadow-lg': '0 16px 48px rgba(0, 102, 204, 0.2), 0 8px 24px rgba(0, 0, 0, 0.15)',
    
    // 圆角
    '--radius-sm': '0.5rem',
    '--radius-md': '0.75rem',
    '--radius-lg': '1rem',
    '--radius-xl': '1.5rem'
  },
  
  // 混合超级风格主题（保留旧版本兼容性）
  'hybrid-super': {
    // 背景色
    '--color-bg-body': '#000000',
    '--color-bg-card': 'rgba(20, 20, 30, 0.6)',
    '--color-bg-elevated': 'rgba(30, 30, 40, 0.8)',
    
    // 文字颜色
    '--color-text-main': '#ffffff',
    '--color-text-muted': 'rgba(255, 255, 255, 0.6)',
    '--color-text-disabled': 'rgba(255, 255, 255, 0.3)',
    
    // 主色调
    '--color-primary': '#00d9ff',
    '--color-primary-soft': 'rgba(0, 217, 255, 0.15)',
    '--color-primary-hover': '#00b8d4',
    
    // 边框颜色
    '--color-border-subtle': 'rgba(255, 255, 255, 0.1)',
    '--color-border-default': 'rgba(255, 255, 255, 0.2)',
    '--color-border-strong': 'rgba(255, 255, 255, 0.4)',
    
    // 阴影
    '--shadow-sm': '0 2px 8px rgba(0, 217, 255, 0.1)',
    '--shadow-md': '0 8px 24px rgba(0, 217, 255, 0.15), 0 4px 12px rgba(0, 0, 0, 0.5)',
    '--shadow-lg': '0 16px 48px rgba(0, 217, 255, 0.2), 0 8px 24px rgba(0, 0, 0, 0.6)',
    
    // 圆角
    '--radius-sm': '0.5rem',
    '--radius-md': '0.75rem',
    '--radius-lg': '1rem',
    '--radius-xl': '1.5rem'
  }
} as const

// ==================== 默认主题 ====================
/**
 * 默认主题名称
 */
export const DEFAULT_THEME: ThemeName = 'light'

// ==================== 本地存储键名 ====================
/**
 * 主题在 localStorage 中的键名
 * 统一使用此键名，便于后续多项目共享
 */
export const THEME_STORAGE_KEY = 'site-theme'

// ==================== 默认主题 Tokens ====================
/**
 * 默认主题 tokens（不包含后端覆盖）
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
    'color.bg.body': '#f8fafc',        // 页面背景：很浅的蓝灰
    'color.bg.card': '#ffffff',        // 卡片背景：白
    'color.bg.elevated': '#ffffff',    // 凸起背景：白
    'color.text.main': '#0f172a',      // 主文字：接近黑
    'color.text.muted': '#64748b',     // 次要文字：灰蓝
    'color.text.disabled': '#94a3b8',  // 禁用文字：浅灰
    'color.primary': '#2563eb',        // 主色：偏正的蓝
    'color.primary.soft': '#dbeafe',   // 主色柔和背景
    'color.primary.hover': '#1d4ed8',  // 主色悬停：深蓝
    'color.border.subtle': '#e2e8f0',  // 卡片边框、分割线
    'color.border.default': '#cbd5e1', // 默认边框
    'color.border.strong': '#94a3b8',  // 强调边框
    'shadow.sm': '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    'shadow.md': '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
    'shadow.lg': '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（用于饼状图、折线图等）
    'chart.primary': '#3b82f6',      // 蓝色
    'chart.secondary': '#10b981',    // 绿色
    'chart.tertiary': '#f59e0b',    // 琥珀色
    'chart.quaternary': '#ef4444',   // 红色
    'chart.quinary': '#8b5cf6',      // 紫色
    'chart.senary': '#ec4899',       // 粉色
    'chart.septenary': '#06b6d4',   // 青色
    'chart.octonary': '#84cc16',     // 黄绿色
    'chart.nonary': '#f97316',       // 橙色
    'chart.denary': '#6366f1'        // 靛蓝色
  },
  /**
   * 深色极简主题
   * 适合：夜间浏览、科技类产品、开发者工具
   * 特点：极简深色，护眼，适合长时间使用
   */
  dark: {
    'color.bg.body': '#020617',        // 整体背景：接近黑的深蓝
    'color.bg.card': '#020617',        // 卡片背景：与 body 一致，靠边框/阴影区分
    'color.bg.elevated': '#0f172a',    // 凸起背景：稍亮的深蓝
    'color.text.main': '#e2e8f0',      // 主文字：浅灰
    'color.text.muted': '#94a3b8',     // 次要文字：更浅的灰
    'color.text.disabled': '#64748b',  // 禁用文字：深灰
    'color.primary': '#60a5fa',        // 主色：亮一点的蓝
    'color.primary.soft': '#0b1120',   // 主色柔和背景：接近黑的蓝
    'color.primary.hover': '#3b82f6',  // 主色悬停：标准蓝
    'color.border.subtle': '#1e293b',  // 细边框：深蓝灰
    'color.border.default': '#334155',  // 默认边框：稍亮的深蓝灰
    'color.border.strong': '#475569',   // 强调边框：更亮的深蓝灰
    'shadow.sm': '0 1px 2px 0 rgb(0 0 0 / 0.3)',
    'shadow.md': '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.4)',
    'shadow.lg': '0 10px 15px -3px rgb(0 0 0 / 0.5), 0 4px 6px -4px rgb(0 0 0 / 0.5)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（深色主题，使用更亮的颜色）
    'chart.primary': '#60a5fa',      // 亮蓝色
    'chart.secondary': '#34d399',   // 亮绿色
    'chart.tertiary': '#fbbf24',    // 亮琥珀色
    'chart.quaternary': '#f87171',   // 亮红色
    'chart.quinary': '#a78bfa',      // 亮紫色
    'chart.senary': '#f472b6',       // 亮粉色
    'chart.septenary': '#22d3ee',    // 亮青色
    'chart.octonary': '#a3e635',     // 亮黄绿色
    'chart.nonary': '#fb923c',       // 亮橙色
    'chart.denary': '#818cf8'        // 亮靛蓝色
  },
  /**
   * 科技蓝霓虹主题
   * 适合：AI 实验室、技术展示模块、创新产品
   * 特点：科技感强，蓝色高亮，适合展示技术内容
   */
  'tech-blue': {
    'color.bg.body': '#020617',                    // 深色背景
    'color.bg.card': 'rgba(15,23,42,0.92)',        // 卡片：略带透明、深蓝
    'color.bg.elevated': 'rgba(30,41,59,0.8)',     // 凸起背景：半透明深蓝
    'color.text.main': '#e5f0ff',                  // 主文字：偏蓝的浅色
    'color.text.muted': '#9ca3af',                 // 次要文字：灰
    'color.text.disabled': '#64748b',              // 禁用文字：深灰
    'color.primary': '#38bdf8',                    // 主色：青蓝（科技感）
    'color.primary.soft': 'rgba(56,189,248,0.16)', // 主色柔和背景：透明青蓝
    'color.primary.hover': '#0ea5e9',              // 主色悬停：深青蓝
    'color.border.subtle': 'rgba(148,163,184,0.6)',// 半透明边线
    'color.border.default': 'rgba(148,163,184,0.8)', // 默认边框：更不透明
    'color.border.strong': 'rgba(148,163,184,1)',  // 强调边框：完全不透明
    'shadow.sm': '0 1px 2px 0 rgba(56,189,248,0.1)',
    'shadow.md': '0 4px 6px -1px rgba(56,189,248,0.2), 0 2px 4px -2px rgba(56,189,248,0.2)',
    'shadow.lg': '0 10px 15px -3px rgba(56,189,248,0.3), 0 4px 6px -4px rgba(56,189,248,0.3)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（科技蓝主题，使用青蓝色系）
    'chart.primary': '#38bdf8',      // 青蓝色
    'chart.secondary': '#22d3ee',   // 青色
    'chart.tertiary': '#a78bfa',    // 紫色
    'chart.quaternary': '#f472b6',  // 粉色
    'chart.quinary': '#34d399',      // 绿色
    'chart.senary': '#fbbf24',       // 琥珀色
    'chart.septenary': '#f87171',    // 红色
    'chart.octonary': '#60a5fa',     // 蓝色
    'chart.nonary': '#fb923c',       // 橙色
    'chart.denary': '#818cf8'        // 靛蓝色
  },
  /**
   * 纸张阅读风格
   * 适合：博客、文章阅读场景、内容展示
   * 特点：柔和温暖，类似纸张质感，适合长时间阅读
   */
  paper: {
    'color.bg.body': '#fdfaf3',        // 背景：偏米白，类似纸张
    'color.bg.card': '#ffffff',        // 卡片：纯白
    'color.bg.elevated': '#ffffff',    // 凸起背景：纯白
    'color.text.main': '#1f2933',      // 主文字：略带棕的深色
    'color.text.muted': '#7b8794',     // 次要文字：灰
    'color.text.disabled': '#9ca3af',  // 禁用文字：浅灰
    'color.primary': '#f97316',        // 主色：橙色（温暖）
    'color.primary.soft': '#fff3e0',   // 主色柔和背景：浅橙
    'color.primary.hover': '#ea580c',  // 主色悬停：深橙
    'color.border.subtle': '#e3e2dc',  // 边线：偏暖的灰
    'color.border.default': '#d1d5db', // 默认边框：标准灰
    'color.border.strong': '#9ca3af',  // 强调边框：深灰
    'shadow.sm': '0 1px 2px 0 rgb(0 0 0 / 0.05)',
    'shadow.md': '0 4px 6px -1px rgb(0 0 0 / 0.08), 0 2px 4px -2px rgb(0 0 0 / 0.08)',
    'shadow.lg': '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（纸张主题，使用温暖色调）
    'chart.primary': '#f97316',      // 橙色
    'chart.secondary': '#eab308',   // 黄色
    'chart.tertiary': '#84cc16',    // 黄绿色
    'chart.quaternary': '#22c55e',  // 绿色
    'chart.quinary': '#3b82f6',      // 蓝色
    'chart.senary': '#8b5cf6',       // 紫色
    'chart.septenary': '#ec4899',    // 粉色
    'chart.octonary': '#f59e0b',      // 琥珀色
    'chart.nonary': '#ef4444',        // 红色
    'chart.denary': '#06b6d4'        // 青色
  },
  /**
   * 自然墨绿主题
   * 适合：自然、环保类内容模块、户外相关
   * 特点：墨绿色调，自然清新，适合特定场景
   */
  forest: {
    'color.bg.body': '#020712',         // 背景：接近黑略带绿
    'color.bg.card': '#071018',         // 卡片：深墨绿
    'color.bg.elevated': '#0f1a1f',     // 凸起背景：稍亮的墨绿
    'color.text.main': '#e4f4ec',       // 主文字：偏冷的浅绿色白
    'color.text.muted': '#9fb3c8',      // 次要文字：带蓝灰
    'color.text.disabled': '#64748b',   // 禁用文字：深灰
    'color.primary': '#22c55e',          // 主色：亮一点的绿
    'color.primary.soft': 'rgba(34,197,94,0.18)', // 柔和背景：透明绿
    'color.primary.hover': '#16a34a',   // 主色悬停：深绿
    'color.border.subtle': 'rgba(148,163,184,0.5)', // 边线：灰
    'color.border.default': 'rgba(148,163,184,0.7)', // 默认边框：更不透明
    'color.border.strong': 'rgba(148,163,184,0.9)',  // 强调边框：几乎不透明
    'shadow.sm': '0 1px 2px 0 rgba(34,197,94,0.1)',
    'shadow.md': '0 4px 6px -1px rgba(34,197,94,0.2), 0 2px 4px -2px rgba(34,197,94,0.2)',
    'shadow.lg': '0 10px 15px -3px rgba(34,197,94,0.3), 0 4px 6px -4px rgba(34,197,94,0.3)',
    'radius.sm': '0.375rem',
    'radius.md': '0.5rem',
    'radius.lg': '0.75rem',
    'radius.xl': '1rem',
    // 图表色板（森林主题，使用绿色系）
    'chart.primary': '#22c55e',      // 绿色
    'chart.secondary': '#84cc16',   // 黄绿色
    'chart.tertiary': '#10b981',    // 翠绿色
    'chart.quaternary': '#06b6d4',  // 青色
    'chart.quinary': '#3b82f6',      // 蓝色
    'chart.senary': '#8b5cf6',       // 紫色
    'chart.septenary': '#f59e0b',    // 琥珀色
    'chart.octonary': '#f97316',      // 橙色
    'chart.nonary': '#ec4899',        // 粉色
    'chart.denary': '#ef4444'        // 红色
  },
  /**
   * 混合超级风格主题（Hybrid Super Style - Dark）
   * 融合：深色实验室 × Vision Pro × 创作者极简风
   * 适合：高端个人网站、数字花园、平台化首页
   * 特点：深色背景、玻璃态材质、精致光效、极简布局
   */
  'hybrid-super-dark': {
    'color.bg.body': '#000000',                    // 纯黑背景（Vision Pro 风格）
    'color.bg.card': 'rgba(20, 20, 30, 0.6)',      // 半透明深色卡片（玻璃态）
    'color.bg.elevated': 'rgba(30, 30, 40, 0.8)',  // 凸起背景（更不透明）
    'color.text.main': '#ffffff',                   // 纯白主文字
    'color.text.muted': 'rgba(255, 255, 255, 0.7)', // 半透明白色次要文字（提高对比度）
    'color.text.disabled': 'rgba(255, 255, 255, 0.4)', // 更透明的禁用文字
    'color.primary': '#00d9ff',                     // 青色主色（科技感）
    'color.primary.soft': 'rgba(0, 217, 255, 0.15)', // 主色柔和背景
    'color.primary.hover': '#00b8d4',               // 主色悬停（深青色）
    'color.border.subtle': 'rgba(255, 255, 255, 0.1)', // 极细边框
    'color.border.default': 'rgba(255, 255, 255, 0.2)', // 默认边框
    'color.border.strong': 'rgba(255, 255, 255, 0.4)', // 强调边框
    'shadow.sm': '0 2px 8px rgba(0, 217, 255, 0.1)',
    'shadow.md': '0 8px 24px rgba(0, 217, 255, 0.15), 0 4px 12px rgba(0, 0, 0, 0.5)',
    'shadow.lg': '0 16px 48px rgba(0, 217, 255, 0.2), 0 8px 24px rgba(0, 0, 0, 0.6)',
    'radius.sm': '0.5rem',
    'radius.md': '0.75rem',
    'radius.lg': '1rem',
    'radius.xl': '1.5rem',
    // 图表色板（混合超级风格，使用高饱和度的科技色）
    'chart.primary': '#00d9ff',      // 青色
    'chart.secondary': '#7c3aed',    // 紫色
    'chart.tertiary': '#f59e0b',    // 琥珀色
    'chart.quaternary': '#ef4444',   // 红色
    'chart.quinary': '#10b981',      // 绿色
    'chart.senary': '#ec4899',       // 粉色
    'chart.septenary': '#06b6d4',   // 青色
    'chart.octonary': '#8b5cf6',     // 紫色
    'chart.nonary': '#f97316',       // 橙色
    'chart.denary': '#6366f1'        // 靛蓝色
  },
  /**
   * 混合超级风格主题（Hybrid Super Style - Light）
   * 融合：浅色实验室 × Vision Pro × 创作者极简风
   * 适合：高端个人网站、数字花园、平台化首页（浅色版本）
   * 特点：浅色背景、玻璃态材质、精致光效、极简布局
   */
  'hybrid-super-light': {
    'color.bg.body': '#ffffff',                    // 纯白背景
    'color.bg.card': 'rgba(255, 255, 255, 0.8)',      // 半透明白色卡片（玻璃态）
    'color.bg.elevated': 'rgba(250, 250, 255, 0.9)',  // 凸起背景（更不透明）
    'color.text.main': '#0f172a',                   // 深色主文字
    'color.text.muted': 'rgba(15, 23, 42, 0.7)', // 半透明深色次要文字
    'color.text.disabled': 'rgba(15, 23, 42, 0.4)', // 更透明的禁用文字
    'color.primary': '#0066cc',                     // 蓝色主色（科技感）
    'color.primary.soft': 'rgba(0, 102, 204, 0.1)', // 主色柔和背景
    'color.primary.hover': '#0052a3',               // 主色悬停（深蓝色）
    'color.border.subtle': 'rgba(15, 23, 42, 0.1)', // 极细边框
    'color.border.default': 'rgba(15, 23, 42, 0.2)', // 默认边框
    'color.border.strong': 'rgba(15, 23, 42, 0.4)', // 强调边框
    'shadow.sm': '0 2px 8px rgba(0, 102, 204, 0.1)',
    'shadow.md': '0 8px 24px rgba(0, 102, 204, 0.15), 0 4px 12px rgba(0, 0, 0, 0.1)',
    'shadow.lg': '0 16px 48px rgba(0, 102, 204, 0.2), 0 8px 24px rgba(0, 0, 0, 0.15)',
    'radius.sm': '0.5rem',
    'radius.md': '0.75rem',
    'radius.lg': '1rem',
    'radius.xl': '1.5rem',
    // 图表色板（混合超级风格浅色版，使用高饱和度的科技色）
    'chart.primary': '#0066cc',      // 蓝色
    'chart.secondary': '#7c3aed',    // 紫色
    'chart.tertiary': '#f59e0b',    // 琥珀色
    'chart.quaternary': '#ef4444',   // 红色
    'chart.quinary': '#10b981',      // 绿色
    'chart.senary': '#ec4899',       // 粉色
    'chart.septenary': '#06b6d4',   // 青色
    'chart.octonary': '#8b5cf6',     // 紫色
    'chart.nonary': '#f97316',       // 橙色
    'chart.denary': '#6366f1'        // 靛蓝色
  },
  /**
   * 混合超级风格主题（Hybrid Super Style - 保留旧版本兼容性）
   * 融合：深色实验室 × Vision Pro × 创作者极简风
   * 适合：高端个人网站、数字花园、平台化首页
   * 特点：深色背景、玻璃态材质、精致光效、极简布局
   */
  'hybrid-super': {
    'color.bg.body': '#000000',                    // 纯黑背景（Vision Pro 风格）
    'color.bg.card': 'rgba(20, 20, 30, 0.6)',      // 半透明深色卡片（玻璃态）
    'color.bg.elevated': 'rgba(30, 30, 40, 0.8)',  // 凸起背景（更不透明）
    'color.text.main': '#ffffff',                   // 纯白主文字
    'color.text.muted': 'rgba(255, 255, 255, 0.6)', // 半透明白色次要文字
    'color.text.disabled': 'rgba(255, 255, 255, 0.3)', // 更透明的禁用文字
    'color.primary': '#00d9ff',                     // 青色主色（科技感）
    'color.primary.soft': 'rgba(0, 217, 255, 0.15)', // 主色柔和背景
    'color.primary.hover': '#00b8d4',               // 主色悬停（深青色）
    'color.border.subtle': 'rgba(255, 255, 255, 0.1)', // 极细边框
    'color.border.default': 'rgba(255, 255, 255, 0.2)', // 默认边框
    'color.border.strong': 'rgba(255, 255, 255, 0.4)', // 强调边框
    'shadow.sm': '0 2px 8px rgba(0, 217, 255, 0.1)',
    'shadow.md': '0 8px 24px rgba(0, 217, 255, 0.15), 0 4px 12px rgba(0, 0, 0, 0.5)',
    'shadow.lg': '0 16px 48px rgba(0, 217, 255, 0.2), 0 8px 24px rgba(0, 0, 0, 0.6)',
    'radius.sm': '0.5rem',
    'radius.md': '0.75rem',
    'radius.lg': '1rem',
    'radius.xl': '1.5rem',
    // 图表色板（混合超级风格，使用高饱和度的科技色）
    'chart.primary': '#00d9ff',      // 青色
    'chart.secondary': '#7c3aed',    // 紫色
    'chart.tertiary': '#f59e0b',    // 琥珀色
    'chart.quaternary': '#ef4444',   // 红色
    'chart.quinary': '#10b981',      // 绿色
    'chart.senary': '#ec4899',       // 粉色
    'chart.septenary': '#06b6d4',   // 青色
    'chart.octonary': '#8b5cf6',     // 紫色
    'chart.nonary': '#f97316',       // 橙色
    'chart.denary': '#6366f1'        // 靛蓝色
  }
} as const

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
 * @param themeKey 主题键，例如 "light", "dark", "tech-blue"
 * @param overrides 后端返回的 tokens 覆盖（可选）
 * @returns 合并后的 tokens，格式为 CSS 变量键值对（--color-xxx: value）
 */
export function resolveThemeTokens(
  themeKey: ThemeKey,
  overrides?: Record<string, string>
): Record<string, string> {
  // 1. 获取默认 tokens
  const defaultTokens = defaultThemeTokens[themeKey] || defaultThemeTokens.light

  // 2. 将默认 tokens 转换为 CSS 变量格式
  const defaultCssVars: Record<string, string> = {}
  Object.entries(defaultTokens).forEach(([key, value]) => {
    defaultCssVars[tokenKeyToCssVar(key)] = value
  })

  // 3. 如果有后端覆盖，将覆盖值也转换为 CSS 变量格式并合并
  if (overrides && Object.keys(overrides).length > 0) {
    Object.entries(overrides).forEach(([key, value]) => {
      // 后端返回的 key 可能是 "color.bg.body" 格式，需要转换为 "--color-bg-body"
      const cssVarKey = key.startsWith('--') ? key : tokenKeyToCssVar(key)
      defaultCssVars[cssVarKey] = value
    })
  }

  return defaultCssVars
}

