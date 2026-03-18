# Admin Page Patterns Specification

> Phase 3 - Admin Page Pattern Interface Design
>
> 本文档定义了后台管理页面模式的设计接口规范，包括各 Pattern 的 Props、Emits、Slots、适用场景、不适用场景和扩展边界。

---

## Table of Contents

- [Pattern Overview](#pattern-overview)
- [PageHeader](#pageheader)
- [FilterBar](#filterbar)
- [ListPage](#listpage)
- [FormPage](#formpage)
- [DetailPage](#detailpage)
- [ConfigPage](#configpage)

---

## Pattern Overview

| Pattern | Purpose | Complexity | Priority |
|---------|---------|------------|----------|
| PageHeader | 页面头部区域：标题、描述、操作按钮 | Low | P0 |
| FilterBar | 筛选器区域：搜索框、下拉筛选、日期范围等 | Medium | P0 |
| ListPage | 完整列表页面：页头 + 筛选 + 数据表格 | High | P0 |
| FormPage | 完整表单页面：页头 + 表单区域 | High | P0 |
| DetailPage | 详情页面：页头 + 详情内容区 | Medium | P1 |
| ConfigPage | 配置页面：页头 + 配置表单 | Medium | P1 |

---

## PageHeader

页面头部组件，提供统一的标题、描述和操作按钮布局。

### Props

```typescript
interface PageHeaderProps {
  /** 页面标题 */
  title: string
  /** 页面描述（可选） */
  description?: string
  /** 页面标题样式级别 */
  level?: 'h1' | 'h2'
  /** 是否显示面包屑（预留） */
  showBreadcrumb?: boolean
  /** 面包屑配置（预留） */
  breadcrumbs?: Array<{ label: string; to?: string }>
  /** 是否显示右侧操作栏 */
  showActions?: boolean
  /** 操作按钮配置 */
  actions?: Array<{
    /** 按钮类型 */
    type?: 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
    /** 按钮文本 */
    label: string
    /** 按钮 icon */
    icon?: string
    /** 是否禁用 */
    disabled?: boolean
    /** 加载状态 */
    loading?: boolean
    /** 按钮点击事件 */
    onClick?: () => void
  }>
  /** 额外类名 */
  class?: string
  /** 自定义样式 */
  style?: Record<string, string>
}
```

### Emits

```typescript
interface PageHeaderEmits {
  /** 点击任意操作按钮时触发 */
  (event: 'action-click', action: string): void
}
```

### Slots

| Slot Name | Description | Scope |
|-----------|-------------|-------|
| `default` | 自定义标题区域内容（覆盖 title 和 description） | - |
| `title` | 自定义标题内容 | - |
| `description` | 自定义描述内容 | - |
| `actions` | 自定义操作按钮区域 | - |
| `actions-prepend` | 操作按钮区域前部插入 | - |
| `actions-append` | 操作按钮区域后部插入 | - |

### 适用场景

- 标准后台管理页面头部
- 需要统一视觉规范的页面标题区
- 带有操作按钮的页面头部

### 不适用场景

- 全屏无头部页面
- 完全自定义头部设计的特殊页面
- 仪表板类页面（使用 DashboardHeader 替代）

### 扩展边界

- 支持自定义头部内容插槽
- 支持通过 actions prop 快速配置操作按钮
- 预留面包屑导航支持
- 预留搜索框扩展点

---

## FilterBar

筛选器组件，提供统一的搜索框、下拉筛选、日期范围等筛选控件。

### Props

```typescript
interface FilterItem {
  /** 筛选器类型 */
  type: 'input' | 'select' | 'date-range' | 'date-picker' | 'number-range'
  /** 字段标识 */
  field: string
  /** 标签文本 */
  label?: string
  /** 占位符文本 */
  placeholder?: string
  /** 选择选项（type 为 select 时） */
  options?: Array<{ label: string; value: any }>
  /** 默认值 */
  defaultValue?: any
  /** 是否可清空 */
  clearable?: boolean
  /** 禁用状态 */
  disabled?: boolean
  /** 自定义类名 */
  class?: string
}

interface FilterBarProps {
  /** 筛选器配置项 */
  filters: FilterItem[]
  /** 筛选值对象 */
  modelValue: Record<string, any>
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 重置按钮文本 */
  resetText?: string
  /** 是否显示搜索按钮（type 为 input 时自动搜索时为 false） */
  showSearch?: boolean
  /** 搜索按钮文本 */
  searchText?: string
  /** 搜索按钮加载状态 */
  searching?: boolean
  /** 筛选器布局方式 */
  layout?: 'horizontal' | 'vertical' | 'grid'
  /** 水平布局时每行显示的筛选器数量 */
  perLine?: number
  /** 折叠展开筛选器（当筛选器过多时） */
  collapsible?: boolean
  /** 默认折叠状态 */
  collapsed?: boolean
  /** 额外类名 */
  class?: string
  /** 自定义样式 */
  style?: Record<string, string>
}
```

### Emits

```typescript
interface FilterBarEmits {
  /** 筛选值变化时触发 */
  (event: 'update:modelValue', value: Record<string, any>): void
  /** 点击搜索按钮时触发 */
  (event: 'search', value: Record<string, any>): void
  /** 点击重置按钮时触发 */
  (event: 'reset'): void
  /** 单个筛选器值变化时触发 */
  (event: 'filter-change', field: string, value: any): void
  /** 折叠状态变化时触发 */
  (event: 'toggle-collapse', collapsed: boolean): void
}
```

### Slots

| Slot Name | Description | Scope |
|-----------|-------------|-------|
| `default` | 自定义筛选器内容（覆盖 filters prop） | - |
| `filter-${field}` | 自定义单个筛选器内容 | { field: string; value: any; item: FilterItem } |
| `actions-prepend` | 操作按钮区域前部插入 | - |
| `actions-append` | 操作按钮区域后部插入 | - |

### 适用场景

- 列表页的筛选区域
- 需要多条件组合搜索的页面
- 数据查询页面

### 不适用场景

- 只有单个搜索框的简单场景（直接使用 NInput 即可）
- 完全自定义筛选逻辑的复杂场景

### 扩展边界

- 支持自定义筛选器类型扩展
- 支持通过 slot 自定义单个筛选器
- 支持折叠展开功能
- 支持响应式布局调整

---

## ListPage

完整的列表页面组件，集成 PageHeader、FilterBar 和数据表格。

### Props

```typescript
interface ListPageProps {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 筛选器配置（可选） */
  filters?: FilterItem[]
  /** 筛选值对象 */
  filterValue?: Record<string, any>
  /** 数据表格列配置 */
  columns: DataTableColumns
  /** 表格数据 */
  data: any[]
  /** 是否加载中 */
  loading?: boolean
  /** 分页配置 */
  pagination?: {
    /** 当前页码 */
    page: number
    /** 每页条数 */
    pageSize: number
    /** 总条数 */
    total: number
    /** 显示总数开关 */
    showSizePicker?: boolean
    /** 页面大小选项 */
    pageSizes?: number[]
    /** 是否显示快速跳转 */
    showQuickJumper?: boolean
  }
  /** 是否显示统计卡片 */
  showStats?: boolean
  /** 统计卡片配置 */
  stats?: Array<{
    /** 统计标题 */
    label: string
    /** 统计值 */
    value: string | number
    /** 统计图标 */
    icon?: string
    /** 图标颜色 */
    iconColor?: string
  }>
  /** 表格配置 */
  tableConfig?: {
    /** 行标识字段 */
    rowKey?: string | ((row: any) => string | number)
    /** 是否单行模式 */
    singleLine?: boolean
    /** 最大高度 */
    maxHeight?: string | number
    /** 是否显示边框 */
    bordered?: boolean
    /** 行样式类名 */
    rowClassName?: string | ((row: any, index: number) => string)
  }
  /** 操作按钮配置（自动添加到表格最后列） */
  actions?: Array<{
    /** 按钮标签 */
    label: string
    /** 按钮类型 */
    type?: 'text' | 'link' | 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
    /** 按钮 icon */
    icon?: string
    /** 显示条件函数 */
    show?: (row: any) => boolean
    /** 禁用条件函数 */
    disabled?: (row: any) => boolean
    /** 点击事件 */
    onClick: (row: any) => void | Promise<void>
  }>
  /** 空状态配置 */
  emptyConfig?: {
    /** 空状态图标 */
    icon?: string
    /** 空状态文本 */
    text?: string
    /** 空状态描述 */
    description?: string
    /** 是否显示操作按钮 */
    showAction?: boolean
    /** 操作按钮配置 */
    action?: {
      label: string
      onClick: () => void
    }
  }
  /** 额外类名 */
  class?: string
  /** 自定义样式 */
  style?: Record<string, string>
}
```

### Emits

```typescript
interface ListPageEmits {
  /** 筛选值变化时触发 */
  (event: 'filter-change', value: Record<string, any>): void
  /** 搜索时触发 */
  (event: 'search', value: Record<string, any>): void
  /** 重置筛选时触发 */
  (event: 'reset'): void
  /** 分页变化时触发 */
  (event: 'page-change', page: number, pageSize: number): void
  /** 行点击时触发 */
  (event: 'row-click', row: any, index: number): void
  /** 行复选框变化时触发 */
  (event: 'row-check', rowKeys: (string | number)[]): void
  /** 排序变化时触发 */
  (event: 'sort-change', sorters: Record<string, 'ascend' | 'descend'>): void
}
```

### Slots

| Slot Name | Description | Scope |
|-----------|-------------|-------|
| `header` | 自定义页面头部（覆盖 PageHeader） | - |
| `header-title` | 自定义标题内容 | - |
| `header-description` | 自定义描述内容 | - |
| `header-actions` | 自定义头部操作按钮区域 | - |
| `filter` | 自定义筛选器区域（覆盖 FilterBar） | - |
| `stats` | 自定义统计卡片区域 | - |
| `table-empty` | 自定义表格空状态 | - |
| `table-footer` | 表格底部区域（分页前） | - |
| `footer` | 页面底部区域 | - |
| `actions-prepend` | 操作列按钮前部插入 | { row: any; index: number } |
| `actions-append` | 操作列按钮后部插入 | { row: any; index: number } |

### 适用场景

- 标准后台 CRUD 列表页面
- 需要展示数据集合的管理页面
- 需要筛选、排序、分页的表格页面

### 不适用场景

- 仪表板类页面
- 复杂的多视图页面（如看板视图）
- 完全自定义布局的特殊页面

### 扩展边界

- 支持自定义页面头部、筛选器、统计卡片
- 支持自定义表格空状态
- 支持通过 actions prop 快速配置操作列
- 支持响应式布局

---

## FormPage

完整的表单页面组件，集成 PageHeader 和表单区域。

### Props

```typescript
interface FormItemConfig {
  /** 字段标识 */
  field: string
  /** 字段标签 */
  label: string
  /** 表单控件类型 */
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
  /** 占位符 */
  placeholder?: string
  /** 输入框行数（textarea 类型） */
  rows?: number
  /** 选择选项（select/radio/checkbox 类型） */
  options?: Array<{ label: string; value: any; disabled?: boolean }>
  /** 最小值（input-number 类型） */
  min?: number
  /** 最大值（input-number 类型） */
  max?: number
  /** 步长（input-number 类型） */
  step?: number
  /** 是否必填 */
  required?: boolean
  /** 验证规则 */
  rules?: any[]
  /** 禁用状态 */
  disabled?: boolean
  /** 只读状态 */
  readonly?: boolean
  /** 提示文本 */
  hint?: string
  /** 错误提示文本 */
  errorTip?: string
  /** 布局方式 */
  layout?: 'vertical' | 'horizontal'
  /** 标签宽度（horizontal 布局时） */
  labelWidth?: number | string
  /** 额外类名 */
  class?: string
  /** 自定义属性 */
  props?: Record<string, any>
}

interface FormPageProps {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 表单字段配置 */
  fields: FormItemConfig[]
  /** 表单值对象 */
  modelValue: Record<string, any>
  /** 是否加载中 */
  loading?: boolean
  /** 提交按钮文本 */
  submitText?: string
  /** 提交按钮加载状态 */
  submitting?: boolean
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 重置按钮文本 */
  resetText?: string
  /** 表单布局方式 */
  layout?: 'vertical' | 'horizontal' | 'inline'
  /** 标签对齐方式 */
  labelPlacement?: 'left' | 'top' | 'right'
  /** 标签宽度 */
  labelWidth?: number | string
  /** 必填标记位置 */
  requireMarkPlacement?: 'left' | 'right' | 'right-hanging'
  /** 是否显示分隔线 */
  showDivider?: boolean
  /** 分组配置（可选） */
  groups?: Array<{
    /** 分组标题 */
    title: string
    /** 分组描述 */
    description?: string
    /** 分组包含的字段列表 */
    fields: string[]
  }>
  /** 额外类名 */
  class?: string
  /** 自定义样式 */
  style?: Record<string, string>
}
```

### Emits

```typescript
interface FormPageEmits {
  /** 表单值变化时触发 */
  (event: 'update:modelValue', value: Record<string, any>): void
  /** 点击提交按钮时触发 */
  (event: 'submit', value: Record<string, any>): void
  /** 点击重置按钮时触发 */
  (event: 'reset'): void
  /** 单个字段值变化时触发 */
  (event: 'field-change', field: string, value: any): void
  /** 表单验证完成时触发（验证通过） */
  (event: 'validate-success', value: Record<string, any>): void
  /** 表单验证失败时触发 */
  (event: 'validate-error', errors: Record<string, string[]>): void
}
```

### Slots

| Slot Name | Description | Scope |
|-----------|-------------|-------|
| `header` | 自定义页面头部（覆盖 PageHeader） | - |
| `header-title` | 自定义标题内容 | - |
| `header-description` | 自定义描述内容 | - |
| `header-actions` | 自定义头部操作按钮区域 | - |
| `form` | 自定义表单区域（覆盖 fields prop） | - |
| `field-${field}` | 自定义单个表单字段 | { field: string; value: any; item: FormItemConfig } |
| `field-${field}-prepend` | 字段前部插入 | { field: string; value: any } |
| `field-${field}-append` | 字段后部插入 | { field: string; value: any } |
| `field-${field}-hint` | 自定义提示文本 | { field: string; value: any } |
| `form-footer` | 表单底部操作按钮区域 | - |
| `footer` | 页面底部区域 | - |

### 适用场景

- 标准后台表单页面（新建/编辑）
- 配置页面
- 设置页面

### 不适用场景

- 复杂的多步骤表单（使用 Stepper 组件）
- 动态表单（字段动态变化）
- 完全自定义布局的表单

### 扩展边界

- 支持表单分组
- 支持自定义单个字段渲染
- 支持多种表单控件类型
- 支持响应式布局

---

## DetailPage

详情页面组件，展示单条数据的详细信息。

### Props

```typescript
interface DetailItemConfig {
  /** 字段标识 */
  field: string
  /** 字段标签 */
  label: string
  /** 显示类型 */
  type?:
    | 'text'
    | 'number'
    | 'date'
    | 'datetime'
    | 'time'
    | 'boolean'
    | 'tag'
    | 'image'
    | 'link'
    | 'html'
    | 'json'
  /** 值格式化函数 */
  formatter?: (value: any, row: any) => string | VNode
  /** Tag 标签配置（type 为 tag 时） */
  tagConfig?: {
    /** 标签类型 */
    type?: 'default' | 'primary' | 'info' | 'success' | 'warning' | 'error'
    /** 是否显示边框 */
    bordered?: boolean
  }
  /** 链接配置（type 为 link 时） */
  linkConfig?: {
    /** 链接地址 */
    to: string | ((value: any, row: any) => string)
    /** 是否在新窗口打开 */
    target?: '_blank' | '_self'
  }
  /** 是否显示 */
  show?: (value: any, row: any) => boolean
  /** 自定义类名 */
  class?: string
}

interface DetailPageProps {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 数据详情对象 */
  data: Record<string, any>
  /** 详情字段配置 */
  fields: DetailItemConfig[]
  /** 是否加载中 */
  loading?: boolean
  /** 是否显示操作按钮 */
  showActions?: boolean
  /** 操作按钮配置 */
  actions?: Array<{
    /** 按钮标签 */
    label: string
    /** 按钮类型 */
    type?: 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
    /** 按钮 icon */
    icon?: string
    /** 按钮点击事件 */
    onClick: (data: Record<string, any>) => void | Promise<void>
    /** 禁用条件 */
    disabled?: (data: Record<string, any>) => boolean
  }>
  /** 详情列表布局方式 */
  layout?: 'list' | 'grid' | 'table'
  /** Grid 布局时列数 */
  columns?: number
  /** 是否显示分隔线 */
  showDivider?: boolean
  /** 分组配置（可选） */
  groups?: Array<{
    /** 分组标题 */
    title: string
    /** 分组包含的字段列表 */
    fields: string[]
  }>
  /** 额外类名 */
  class?: string
  /** 自定义样式 */
  style?: Record<string, string>
}
```

### Emits

```typescript
interface DetailPageEmits {
  /** 点击操作按钮时触发 */
  (event: 'action-click', action: string, data: Record<string, any>): void
}
```

### Slots

| Slot Name | Description | Scope |
|-----------|-------------|-------|
| `header` | 自定义页面头部 | - |
| `header-title` | 自定义标题内容 | - |
| `header-description` | 自定义描述内容 | - |
| `header-actions` | 自定义头部操作按钮区域 | - |
| `detail` | 自定义详情区域 | - |
| `field-${field}` | 自定义单个详情字段 | { field: string; value: any; data: Record<string, any> } |
| `footer` | 页面底部区域 | - |

### 适用场景

- 数据详情展示页面
- 订单详情、用户详情等查看页面

### 不适用场景

- 需要编辑的页面（使用 FormPage）
- 复杂的展示型页面

### 扩展边界

- 支持多种显示类型
- 支持自定义字段格式化
- 支持详情分组
- 支持响应式布局

---

## ConfigPage

配置页面组件，专用于系统配置、设置等场景。

### Props

```typescript
interface ConfigGroup {
  /** 分组标识 */
  key: string
  /** 分组标题 */
  title: string
  /** 分组描述 */
  description?: string
  /** 是否可折叠 */
  collapsible?: boolean
  /** 默认折叠状态 */
  collapsed?: boolean
  /** 配置项列表 */
  items: ConfigItem[]
}

interface ConfigItem extends FormItemConfig {
  /** 配置项标识 */
  key: string
  /** 配置项类型 */
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
  /** 是否支持实时预览 */
  livePreview?: boolean
  /** 预览渲染函数 */
  preview?: (value: any) => VNode | string
  /** 配置项提示 */
  hint?: string
  /** 配置项警告 */
  warning?: string
}

interface ConfigPageProps {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 配置分组列表 */
  groups: ConfigGroup[]
  /** 配置值对象 */
  modelValue: Record<string, any>
  /** 是否加载中 */
  loading?: boolean
  /** 是否显示预览区域 */
  showPreview?: boolean
  /** 预览标题 */
  previewTitle?: string
  /** 预览位置 */
  previewPosition?: 'side' | 'bottom' | 'modal'
  /** 是否显示操作按钮 */
  showActions?: boolean
  /** 保存按钮文本 */
  saveText?: string
  /** 保存按钮加载状态 */
  saving?: boolean
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 重置按钮文本 */
  resetText?: string
  /** 额外类名 */
  class?: string
  /** 自定义样式 */
  style?: Record<string, string>
}
```

### Emits

```typescript
interface ConfigPageEmits {
  /** 配置值变化时触发 */
  (event: 'update:modelValue', value: Record<string, any>): void
  /** 点击保存按钮时触发 */
  (event: 'save', value: Record<string, any>): void
  /** 点击重置按钮时触发 */
  (event: 'reset'): void
  /** 单个配置项变化时触发 */
  (event: 'config-change', key: string, value: any): void
}
```

### Slots

| Slot Name | Description | Scope |
|-----------|-------------|-------|
| `header` | 自定义页面头部 | - |
| `header-title` | 自定义标题内容 | - |
| `header-description` | 自定义描述内容 | - |
| `preview` | 自定义预览区域 | { value: Record<string, any> } |
| `config-group-${key}` | 自定义配置分组 | { group: ConfigGroup; value: Record<string, any> } |
| `config-item-${key}` | 自定义单个配置项 | { item: ConfigItem; value: any } |
| `actions-prepend` | 操作按钮区域前部插入 | - |
| `actions-append` | 操作按钮区域后部插入 | - |
| `footer` | 页面底部区域 | - |

### 适用场景

- 系统设置页面
- 配置管理页面
- 主题配置页面

### 不适用场景

- 新建/编辑数据表单（使用 FormPage）
- 复杂的多步骤配置

### 扩展边界

- 支持配置分组
- 支持实时预览
- 支持多种配置类型
- 支持自定义预览区域

---

## Migration Targets

基于 Pattern 设计，选择以下页面进行迁移：

### 典型列表页（已选择）
- `pages/admin/categories.vue` - 标准后台列表页
  - 页头（标题 + 描述）
  - 统计卡片（3 个）
  - 搜索框 + 新建按钮
  - NDataTable 表格
  - 编辑/删除操作

### 典型表单页（已选择）
- `pages/admin/ai/support-config.vue` - 客服智能体配置表单页
  - 页头（标题 + 描述）
  - NForm 表单
  - 系统提示词（textarea）
  - FAQ 列表（动态数组）

### 暂不迁移页面
- `pages/admin/modules/index.vue` - 复杂卡片布局列表
- `pages/admin/settings/fonts.vue` - 复杂配置页
- `pages/admin/investment.vue` - 投资页（复杂图表）
- `pages/admin/analytics.vue` - 仪表板页

---

## Implementation Notes

### 组件目录结构

```
components/
  admin/
    patterns/
      PageHeader.vue
      FilterBar.vue
      ListPage.vue
      FormPage.vue
      DetailPage.vue
      ConfigPage.vue
```

### 设计变量引用

所有 Pattern 组件应遵循以下设计系统：

- 使用 `--color-*` 变量进行颜色
- 使用 `--spacing-*` 变量进行间距
- 使用 `--radius-*` 变量进行圆角
- 使用 `--shadow-*` 变量进行阴影
- 使用 `--text-*` 变量进行字号

### 主题适配

所有 Pattern 组件应自动适配以下主题：

- `light` - 浅色主题
- `dark` - 深色主题
- `hybrid-super-dark` - 混合超深色主题

### 响应式设计

所有 Pattern 组件应支持以下断点：

- `< 768px` - 移动端
- `768px - 1024px` - 平板
- `> 1024px` - 桌面端

---

## Next Steps

1. 实现 PageHeader 组件
2. 实现 FilterBar 组件
3. 实现 ListPage 组件
4. 实现 FormPage 组件
5. 迁移 `categories.vue` 使用 ListPage
6. 迁移 `ai/support-config.vue` 使用 FormPage
7. 输出 Phase 3 完成报告
