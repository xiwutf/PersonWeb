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
 * 目前支持三种主题：light（浅色）、dark（深色）、techBlue（科技蓝）
 */
export type ThemeName = 'light' | 'dark' | 'tech-blue'

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

