// Pattern 组件类型定义

/**
 * 筛选器配置项
 */
export interface FilterItem {
  type: 'input' | 'input-number' | 'select' | 'date-range' | 'date-picker' | 'number-range'
  field: string
  label?: string
  placeholder?: string | string[]
  options?: Array<{ label: string; value: any; disabled?: boolean }>
  defaultValue?: any
  clearable?: boolean
  disabled?: boolean
  class?: string | string[] | Record<string, boolean>
}

/**
 * 操作按钮配置
 */
export interface ActionConfig {
  type?: 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
  label: string
  icon?: string
  disabled?: boolean
  loading?: boolean
  onClick?: () => void
}

/**
 * 统计卡片配置
 */
export interface StatConfig {
  label: string
  value: string | number
  icon?: string
  iconColor?: string
}

/**
 * 分页配置
 */
export interface PaginationConfig {
  page: number
  pageSize: number
  total: number
  showSizePicker?: boolean
  pageSizes?: number[]
  showQuickJumper?: boolean
}

/**
 * 表单字段配置
 */
export interface FormItemConfig {
  field: string
  label: string
  type:
    | 'input'
    | 'textarea'
    | 'input-number'
    | 'select'
    | 'radio'
    | 'checkbox'
    | 'switch'
    | 'date-picker'
    | 'time-picker'
    | 'date-range-picker'
    | 'upload'
    | 'color-picker'
  placeholder?: string
  rows?: number
  options?: Array<{ label: string; value: any; disabled?: boolean }>
  min?: number
  max?: number
  step?: number
  required?: boolean
  rules?: any[]
  disabled?: boolean
  readonly?: boolean
  hint?: string
  errorTip?: string
  layout?: 'vertical' | 'horizontal'
  labelWidth?: number | string
  class?: string | string[] | Record<string, boolean>
  props?: Record<string, any>
}

/**
 * 表单分组
 */
export interface FormGroup {
  key: string
  title: string
  description?: string
  collapsible?: boolean
  collapsed?: boolean
  fields: string[]
}

/**
 * 详情字段配置
 */
export interface DetailItemConfig {
  field: string
  label: string
  type?: 'text' | 'number' | 'date' | 'datetime' | 'time' | 'boolean' | 'tag' | 'image' | 'link' | 'html' | 'json'
  formatter?: (value: any, data: Record<string, any>) => string | any
  tagConfig?: {
    type?: 'default' | 'primary' | 'info' | 'success' | 'warning' | 'error'
    bordered?: boolean
  }
  linkConfig?: {
    to: string | ((value: any, data: Record<string, any>) => string)
    target?: '_blank' | '_self'
  }
  show?: (value: any, data: Record<string, any>) => boolean
  class?: string | string[] | Record<string, boolean>
}

/**
 * 详情操作按钮
 */
export interface DetailAction {
  label: string
  type?: 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
  icon?: string
  onClick: (data: Record<string, any>) => void | Promise<void>
  disabled?: (data: Record<string, any>) => boolean
}

/**
 * 详情分组
 */
export interface DetailGroup {
  key: string
  title: string
  fields: string[]
}

/**
 * 配置项
 */
export interface ConfigItem {
  key: string
  label: string
  type:
    | 'input'
    | 'textarea'
    | 'input-number'
    | 'select'
    | 'switch'
    | 'color-picker'
    | 'font-family'
    | 'font-size'
    | 'radio-group'
    | 'checkbox-group'
  placeholder?: string
  rows?: number
  options?: Array<{ label: string; value: any; disabled?: boolean }>
  min?: number
  max?: number
  step?: number
  hint?: string
  warning?: string
  livePreview?: boolean
  preview?: (value: any) => string
  class?: string | string[] | Record<string, boolean>
  props?: Record<string, any>
}

/**
 * 配置分组
 */
export interface ConfigGroup {
  key: string
  title: string
  description?: string
  collapsible?: boolean
  collapsed?: boolean
  items: ConfigItem[]
}

/**
 * 空状态配置
 */
export interface EmptyConfig {
  icon?: string
  text?: string
  description?: string
  showAction?: boolean
  action?: { label: string; onClick: () => void }
}
