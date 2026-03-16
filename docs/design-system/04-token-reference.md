# PersonWeb Design System - Token 参考文档

**版本**: v1.0
**创建日期**: 2026-03-14
**Token 文件位置**: `assets/styles/theme-variables.css`

---

## 目录

1. [Token 分层架构](#token-分层架构)
2. [Layer 1: Foundation - 基础层](#layer-1-foundation---基础层)
3. [Layer 2: Semantic - 语义层](#layer-2-semantic---语义层)
4. [Layer 3: Component - 组件层](#layer-3-component---组件层)
5. [RGBA 颜色变量](#rgba-颜色变量)
6. [尺寸系统](#尺寸系统)
7. [动画关键帧](#动画关键帧)
8. [通用辅助类](#通用辅助类)
9. [Token 使用规范](#token-使用规范)
10. [兼容性变量](#兼容性变量)

---

## Token 分层架构

PersonWeb Design System 采用三层 Token 架构：

```
┌─────────────────────────────────────────────────────────┐
│              Layer 3: Component (组件层)              │
│  --btn-bg-primary, --card-bg, --input-border-focus   │
└─────────────────────────────────────────────────────────┘
                           │ 映射自
┌─────────────────────────────────────────────────────────┐
│               Layer 2: Semantic (语义层)               │
│  --color-text-main, --color-bg-card, --color-border  │
└─────────────────────────────────────────────────────────┘
                           │ 映射自
┌─────────────────────────────────────────────────────────┐
│              Layer 1: Foundation (基础层)              │
│  --color-blue-500, --color-gray-900, --spacing-4    │
└─────────────────────────────────────────────────────────┘
```

| 层级 | 用途 | 命名模式 | 示例 |
|-----|------|---------|------|
| **Foundation** | 原始颜色和尺寸 | `--color-{hue}-{level}`, `--spacing-{n}` | `--color-blue-500`, `--spacing-4` |
| **Semantic** | 语义化用途 | `--color-{purpose}-{type}` | `--color-text-main`, `--color-bg-card` |
| **Component** | 组件特定样式 | `--{component}-{state}-{prop}` | `--btn-bg-primary`, `--input-border-focus` |

---

## Layer 1: Foundation - 基础层

### 1.1 颜色阶梯 (Color Palette)

完整的颜色阶梯系统，包含 50-950 共 10 个级别。

#### 红色系 (Red)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-red-50` | #fef2f2 | 极浅红 |
| `--color-red-100` | #fee2e2 | 浅红 |
| `--color-red-200` | #fecaca | |
| `--color-red-300` | #fca5a5 | |
| `--color-red-400` | #f87171 | |
| `--color-red-500` | #ef4444 | 标准红 |
| `--color-red-600` | #dc2626 | |
| `--color-red-700` | #b91c1c | 深红 |
| `--color-red-800` | #991b1b | |
| `--color-red-900` | #7f1d1d | 极深红 |
| `--color-red-950` | #450a0a | |

#### 橙色系 (Orange)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-orange-50` | #fff7ed | |
| `--color-orange-100` | #ffedd5 | |
| `--color-orange-200` | #fed7aa | |
| `--color-orange-300` | #fdba74 | |
| `--color-orange-400` | #fb923c | |
| `--color-orange-500` | #f97316 | 标准橙 |
| `--color-orange-600` | #ea580c | |
| `--color-orange-700` | #c2410c | |
| `--color-orange-800` | #9a3412 | |
| `--color-orange-900` | #7c2d12 | |
| `--color-orange-950` | #431407 | |

#### 黄色系 (Yellow)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-yellow-50` | #fefce8 | |
| `--color-yellow-100` | #fef9c3 | |
| `--color-yellow-200` | #fef08a | |
| `--color-yellow-300` | #fde047 | |
| `--color-yellow-400` | #facc15 | |
| `--color-yellow-500` | #eab308 | 标准黄 |
| `--color-yellow-600` | #ca8a04 | |
| `--color-yellow-700` | #a16207 | |
| `--color-yellow-800` | #854d0e | |
| `--color-yellow-900` | #713f12 | |
| `--color-yellow-950` | #422006 | |

#### 绿色系 (Green)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-green-50` | #f0fdf4 | |
| `--color-green-100` | #dcfce7 | |
| `--color-green-200` | #bbf7d0 | |
| `--color-green-300` | #86efac | |
| `--color-green-400` | #4ade80 | |
| `--color-green-500` | #22c55e | 标准绿 |
| `--color-green-600` | #16a34a | |
| `--color-green-700` | #15803d | |
| `--color-green-800` | #166534 | |
| `--color-green-900` | #14532d | |
| `--color-green-950` | #052e16 | |

#### 青色系 (Cyan)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-cyan-50` | #ecfeff | |
| `--color-cyan-100` | #cffafe | |
| `--color-cyan-200` | #a5f3fc | |
| `--color-cyan-300` | #67e8f9 | |
| `--color-cyan-400` | #22d3ee | |
| `--color-cyan-500` | #06b6d4 | 标准青 |
| `--color-cyan-600` | #0891b2 | |
| `--color-cyan-700` | #0e7490 | |
| `--color-cyan-800` | #155e75 | |
| `--color-cyan-900` | #164e63 | |
| `--color-cyan-950` | #083344 | |

#### 蓝色系 (Blue)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-blue-50` | #eff6ff | |
| `--color-blue-100` | #dbeafe | |
| `--color-blue-200` | #bfdbfe | |
| `--color-blue-300` | #93c5fd | |
| `--color-blue-400` | #60a5fa | |
| `--color-blue-500` | #3b82f6 | 标准蓝 |
| `--color-blue-600` | #2563eb | 主色使用 |
| `--color-blue-700` | #1d4ed8 | |
| `--color-blue-800` | #1e40af | |
| `--color-blue-900` | #1e3a8a | |
| `--color-blue-950` | #172554 | |

#### 紫色系 (Purple)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-purple-50` | #faf5ff | |
| `--color-purple-100` | #f3e8ff | |
| `--color-purple-200` | #e9d5ff | |
| `--color-purple-300` | #d8b4fe | |
| `--color-purple-400` | #c084fc | |
| `--color-purple-500` | #a855f7 | 标准紫 |
| `--color-purple-600` | #9333ea | |
| `--color-purple-700` | #7c3aed | |
| `--color-purple-800` | #6d28d9 | |
| `--color-purple-900` | #5b21b6 | |
| `--color-purple-950` | #4c1d95 | |

#### 粉色系 (Pink)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-pink-50` | #fdf2f8 | |
| `--color-pink-100` | #fce7f3 | |
| `--color-pink-200` | #fbcfe8 | |
| `--color-pink-300` | #f9a8d4 | |
| `--color-pink-400` | #f472b6 | |
| `--color-pink-500` | #ec4899 | 标准粉 |
| `--color-pink-600` | #db2777 | |
| `--color-pink-700` | #be185d | |
| `--color-pink-800` | #9d174d | |
| `--color-pink-900` | #831843 | |
| `--color-pink-950` | #500724 | |

#### 灰色系 (Gray) - 中性色

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-gray-50` | #f9fafb | 浅灰背景 |
| `--color-gray-100` | #f3f4f6 | 提升层背景 |
| `--color-gray-200` | #e5e7eb | 默认边框 |
| `--color-gray-300` | #d1d5db | |
| `--color-gray-400` | #9ca3af | 四级文字 |
| `--color-gray-500` | #6b7280 | 三级文字 |
| `--color-gray-600` | #4b5563 | 辅助文字 |
| `--color-gray-700` | #374151 | 次要文字 |
| `--color-gray-800` | #1f2937 | |
| `--color-gray-900` | #111827 | 主要文字 |
| `--color-gray-950` | #030712 | |

**💡 提示**: 灰色系通常不用于主题切换，而是作为中性色使用。

---

## Layer 2: Semantic - 语义层

### 2.1 文字颜色 (Text Colors)

| 变量 | 值 | 用途 | 优先级 |
|------|-----|------|-------|
| `--color-text-main` | `--color-gray-900` | 主要文字（标题、正文） | ⭐ 最高 |
| `--color-text-primary` | `--color-text-main` | 主色文字（别名） | |
| `--color-text-secondary` | `--color-gray-700` | 次要文字 | ⭐ 高 |
| `--color-text-tertiary` | `--color-gray-500` | 三级文字 | ⭐ 中 |
| `--color-text-quaternary` | `--color-gray-400` | 四级文字（占位符） | |
| `--color-text-muted` | `--color-gray-600` | 辅助文字 | |
| `--color-text-sub` | `--color-gray-700` | 次级文字 | |
| `--color-text-disabled` | `--color-gray-400` | 禁用状态文字 | |
| `--color-text-on-primary` | `--color-white` | 主色上的文字 | |

#### 状态文字颜色

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-text-success` | `--color-green-600` | 成功状态文字 |
| `--color-text-warning` | `--color-orange-500` | 警告状态文字 |
| `--color-text-error` | `--color-red-600` | 错误状态文字 |
| `--color-text-info` | `--color-blue-500` | 信息状态文字 |

#### 链接颜色

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-link` | `--color-blue-600` | 链接颜色 |
| `--color-link-hover` | `--color-blue-700` | 链接悬停颜色 |

### 2.2 背景颜色 (Background Colors)

| 变量 | 值 | 用途 | 优先级 |
|------|-----|------|-------|
| `--color-bg-body` | `--color-gray-50` | 页面背景色 | ⭐ 最高 |
| `--color-bg-page` | `--color-bg-body` | 页面背景（别名） | |
| `--color-bg-page-alt` | `--color-white` | 交替页面背景 | |
| `--color-bg-light` | `--color-gray-50` | 浅色背景 | |
| `--color-bg-dark` | `--color-gray-900` | 深色背景 | |
| `--color-bg-elevated` | `--color-gray-100` | 提升层背景 | ⭐ 高 |
| `--color-bg-overlay` | rgba(0, 0, 0, 0.5) | 遮罩层背景 | |
| `--color-bg-card` | `--color-white` | 卡片背景 | ⭐ 高 |
| `--color-bg-card-elevated` | `--color-gray-100` | 提升卡片背景 | |
| `--color-bg-card-muted` | `--color-gray-50` | 静音卡片背景 | |
| `--color-bg-surface` | `--color-white` | 表面背景 | |
| `--color-bg-surface-elevated` | `--color-gray-100` | 提升表面背景 | |
| `--color-bg-surface-muted` | `--color-gray-50` | 静音表面背景 | |
| `--color-bg-input` | `--color-white` | 输入框背景 | |
| `--color-bg-input-disabled` | `--color-gray-100` | 禁用输入框背景 | |
| `--color-bg-input-hover` | `--color-gray-50` | 输入框悬停背景 | |
| `--color-bg-disabled` | `--color-gray-200` | 禁用背景 | |

### 2.3 边框颜色 (Border Colors)

| 变量 | 值 | 用途 | 优先级 |
|------|-----|------|-------|
| `--color-border-subtle` | `--color-gray-200` | 细微边框 | |
| `--color-border-default` | `--color-gray-300` | 默认边框 | ⭐ 高 |
| `--color-border-strong` | `--color-gray-400` | 强调边框 | |
| `--color-border-focus` | `--color-blue-500` | 聚焦边框 | ⭐ 高 |
| `--color-border-muted` | `--color-gray-200` | 静音边框 | |
| `--color-border-hover` | `--color-gray-300` | 悬停边框 | |
| `--color-border-active` | `--color-blue-500` | 激活边框 | |

### 2.4 状态颜色 (Status Colors)

#### 成功状态 (Success)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-success-bg` | `--color-green-50` | 成功背景 |
| `--color-success-border` | `--color-green-200` | 成功边框 |
| `--color-success-text` | `--color-green-700` | 成功文字 |
| `--color-success-icon` | `--color-green-500` | 成功图标 |
| `--color-success-soft` | rgba(16, 185, 129, 0.15) | 成功半透明背景 |
| `--color-success-10` | rgba(16, 185, 129, 0.1) | 成功 10% 透明度 |
| `--color-success-20` | rgba(16, 185, 129, 0.2) | 成功 20% 透明度 |
| `--color-success-30` | rgba(16, 185, 129, 0.3) | 成功 30% 透明度 |

#### 警告状态 (Warning)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-warning-bg` | `--color-orange-50` | 警告背景 |
| `--color-warning-border` | `--color-orange-200` | 警告边框 |
| `--color-warning-text` | `--color-orange-700` | 警告文字 |
| `--color-warning-icon` | `--color-orange-500` | 警告图标 |
| `--color-warning-soft` | rgba(249, 115, 22, 0.15) | 警告半透明背景 |
| `--color-warning-10` | rgba(249, 115, 22, 0.1) | 警告 10% 透明度 |
| `--color-warning-20` | rgba(249, 115, 22, 0.2) | 警告 20% 透明度 |
| `--color-warning-30` | rgba(249, 115, 22, 0.3) | 警告 30% 透明度 |

#### 错误状态 (Error)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-error-bg` | `--color-red-50` | 错误背景 |
| `--color-error-border` | `--color-red-200` | 错误边框 |
| `--color-error-text` | `--color-red-700` | 错误文字 |
| `--color-error-icon` | `--color-red-500` | 错误图标 |
| `--color-error-soft` | rgba(236, 72, 153, 0.15) | 错误半透明背景 |
| `--color-error-10` | rgba(236, 72, 153, 0.1) | 错误 10% 透明度 |
| `--color-error-20` | rgba(236, 72, 153, 0.2) | 错误 20% 透明度 |
| `--color-error-30` | rgba(236, 72, 153, 0.3) | 错误 30% 透明度 |

#### 信息状态 (Info)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-info-bg` | `--color-blue-50` | 信息背景 |
| `--color-info-border` | `--color-blue-200` | 信息边框 |
| `--color-info-text` | `--color-blue-700` | 信息文字 |
| `--color-info-icon` | `--color-blue-500` | 信息图标 |
| `--color-info-soft` | rgba(6, 182, 212, 0.15) | 信息半透明背景 |
| `--color-info-10` | rgba(6, 182, 212, 0.1) | 信息 10% 透明度 |
| `--color-info-20` | rgba(6, 182, 212, 0.2) | 信息 20% 透明度 |
| `--color-info-30` | rgba(6, 182, 212, 0.3) | 信息 30% 透明度 |

#### 紫色状态 (Purple)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-purple-soft` | rgba(168, 85, 247, 0.15) | 紫色半透明背景 |
| `--color-purple-10` | rgba(168, 85, 247, 0.1) | 紫色 10% 透明度 |
| `--color-purple-20` | rgba(168, 85, 247, 0.2) | 紫色 20% 透明度 |
| `--color-purple-30` | rgba(168, 85, 247, 0.3) | 紫色 30% 透明度 |

### 2.5 投资专用语义颜色 (Investment)

#### 盈利状态 (Profit)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-profit` | `--color-emerald-500` | 盈利颜色 |
| `--color-profit-hover` | `--color-emerald-600` | 盈利悬停 |
| `--color-profit-bg` | `--color-emerald-50` | 盈利背景 |
| `--color-profit-text` | `--color-emerald-700` | 盈利文字 |
| `--color-profit-soft` | rgba(16, 185, 129, 0.15) | 盈利半透明 |

#### 亏损状态 (Loss)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-loss` | `--color-red-500` | 亏损颜色 |
| `--color-loss-hover` | `--color-red-600` | 亏损悬停 |
| `--color-loss-bg` | `--color-red-50` | 亏损背景 |
| `--color-loss-text` | `--color-red-700` | 亏损文字 |
| `--color-loss-soft` | rgba(236, 72, 153, 0.15) | 亏损半透明 |

### 2.6 主色调 (Primary Color)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-primary` | `--color-blue-600` | 主色 |
| `--color-primary-base` | `--color-primary` | 主色基础（别名） |
| `--color-primary-hover` | `--color-blue-700` | 主色悬停 |
| `--color-primary-active` | `--color-blue-800` | 主色激活 |
| `--color-primary-light` | `--color-blue-100` | 主色浅 |
| `--color-primary-lighter` | `--color-blue-50` | 主色更浅 |
| `--color-primary-dark` | `--color-blue-500` | 主色深 |
| `--color-primary-soft` | rgba(59, 130, 246, 0.12) | 主色半透明 |
| `--color-primary-10` | rgba(59, 130, 246, 0.1) | 主色 10% 透明度 |
| `--color-primary-20` | rgba(59, 130, 246, 0.2) | 主色 20% 透明度 |
| `--color-primary-text` | `--color-white` | 主色文字 |
| `--color-primary-text-inverse` | `--color-blue-600` | 反向主色文字 |

#### 其他品牌色 (兼容旧变量)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-info` | `--color-cyan-500` | 信息色 |
| `--color-info-hover` | `--color-cyan-400` | 信息悬停 |
| `--color-error` | `--color-pink-500` | 错误色（粉色） |
| `--color-error-hover` | `--color-pink-400` | 错误悬停 |
| `--color-purple` | `--color-purple-500` | 紫色 |

---

## Layer 3: Component - 组件层

### 3.1 按钮 (Button)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--btn-bg-primary` | `--color-primary` | 主按钮背景 |
| `--btn-bg-primary-hover` | `--color-primary-hover` | 主按钮悬停 |
| `--btn-bg-primary-active` | `--color-primary-active` | 主按钮激活 |
| `--btn-text-primary` | `--color-primary-text` | 主按钮文字 |
| `--btn-border-primary` | transparent | 主按钮边框 |
| `--btn-bg-secondary` | transparent | 次要按钮背景 |
| `--btn-bg-secondary-hover` | `--color-gray-100` | 次要按钮悬停 |
| `--btn-bg-secondary-active` | `--color-gray-200` | 次要按钮激活 |
| `--btn-text-secondary` | `--color-text-primary` | 次要按钮文字 |
| `--btn-border-secondary` | `--color-border-default` | 次要按钮边框 |
| `--btn-bg-ghost` | transparent | 虚线按钮背景 |
| `--btn-text-ghost` | `--color-text-secondary` | 虚线按钮文字 |
| `--btn-border-ghost` | `--color-border-subtle` | 虚线按钮边框 |
| `--btn-bg-text` | transparent | 文字按钮背景 |
| `--btn-text-text` | `--color-primary` | 文字按钮文字 |
| `--btn-border-text` | transparent | 文字按钮边框 |

#### 按钮尺寸

| 变量 | 值 | 用途 |
|------|-----|------|
| `--btn-height-sm` | 32px | 小按钮高度 |
| `--btn-height-md` | 40px | 中按钮高度 |
| `--btn-height-lg` | 48px | 大按钮高度 |
| `--btn-padding-x-sm` | 12px | 小按钮水平内边距 |
| `--btn-padding-x-md` | 16px | 中按钮水平内边距 |
| `--btn-padding-x-lg` | 24px | 大按钮水平内边距 |
| `--btn-radius` | `--radius-md` | 按钮圆角 |
| `--btn-font-weight` | 500 | 按钮字重 |
| `--btn-text-shadow` | none | 按钮文字阴影 |

### 3.2 输入框 (Input)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--input-bg` | `--color-bg-input` | 输入框背景 |
| `--input-border` | `--color-border-default` | 输入框边框 |
| `--input-border-focus` | `--color-border-focus` | 输入框聚焦边框 |
| `--input-text` | `--color-text-primary` | 输入框文字 |
| `--input-placeholder` | `--color-text-quaternary` | 输入框占位符 |

#### 输入框尺寸

| 变量 | 值 | 用途 |
|------|-----|------|
| `--input-height-sm` | 32px | 小输入框高度 |
| `--input-height-md` | 40px | 中输入框高度 |
| `--input-height-lg` | 48px | 大输入框高度 |
| `--input-padding-x` | 12px | 输入框水平内边距 |
| `--input-radius` | `--radius-md` | 输入框圆角 |

#### 输入框状态

| 变量 | 值 | 用途 |
|------|-----|------|
| `--input-bg-disabled` | `--color-gray-100` | 禁用输入框背景 |
| `--input-bg-hover` | `--color-gray-50` | 输入框悬停背景 |
| `--input-bg-error` | `--color-error-bg` | 错误输入框背景 |
| `--input-border-error` | `--color-error-border` | 错误输入框边框 |

### 3.3 卡片 (Card)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--card-bg` | `--color-bg-card` | 卡片背景 |
| `--card-border` | `--color-border-default` | 卡片边框 |
| `--card-radius` | 16px | 卡片圆角 |
| `--card-shadow-sm` | `--shadow-sm` | 卡片小阴影 |
| `--card-shadow-md` | `--shadow-md` | 卡片中阴影 |
| `--card-shadow-lg` | `--shadow-lg` | 卡片大阴影 |

#### 卡片内边距

| 变量 | 值 | 用途 |
|------|-----|------|
| `--card-padding-sm` | 16px | 小卡片内边距 |
| `--card-padding-md` | 24px | 中卡片内边距 |
| `--card-padding-lg` | 32px | 大卡片内边距 |

### 3.4 标签 (Tag)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--tag-bg-default` | `--color-gray-100` | 默认标签背景 |
| `--tag-text-default` | `--color-text-secondary` | 默认标签文字 |
| `--tag-border-default` | `--color-border-subtle` | 默认标签边框 |

#### 标签尺寸

| 变量 | 值 | 用途 |
|------|-----|------|
| `--tag-height-sm` | 24px | 小标签高度 |
| `--tag-height-md` | 28px | 中标签高度 |
| `--tag-height-lg` | 32px | 大标签高度 |
| `--tag-padding-x-sm` | 8px | 小标签水平内边距 |
| `--tag-padding-x-md` | 12px | 中标签水平内边距 |
| `--tag-padding-x-lg` | 16px | 大标签水平内边距 |
| `--tag-radius` | `--radius-sm` | 标签圆角 |
| `--tag-font-size` | 0.875rem | 标签字号 |
| `--tag-font-weight` | 500 | 标签字重 |

### 3.5 表格 (Table)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--table-bg-header` | `--color-gray-50` | 表格头部背景 |
| `--table-text-header` | `--color-text-secondary` | 表格头部文字 |
| `--table-border-header` | `--color-border-subtle` | 表格头部边框 |
| `--table-bg-row` | transparent | 表格行背景 |
| `--table-bg-row-hover` | `--color-gray-50` | 表格行悬停背景 |
| `--table-text-row` | `--color-text-primary` | 表格行文字 |
| `--table-border-row` | `--color-border-subtle` | 表格行边框 |
| `--table-bg-stripe` | `--color-gray-50` | 表格斑马纹背景 |
| `--table-bg-footer` | `--color-gray-50` | 表格底部背景 |
| `--table-text-footer` | `--color-text-secondary` | 表格底部文字 |
| `--table-border-footer` | `--color-border-subtle` | 表格底部边框 |

### 3.6 弹窗 (Modal)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--modal-bg` | `--color-bg-card` | 弹窗背景 |
| `--modal-border` | `--color-border-default` | 弹窗边框 |
| `--modal-radius` | `--radius-xl` | 弹窗圆角 |
| `--modal-shadow` | `--shadow-lg` | 弹窗阴影 |

#### 弹窗头部

| 变量 | 值 | 用途 |
|------|-----|------|
| `--modal-header-bg` | `--color-bg-card` | 弹窗头部背景 |
| `--modal-header-border` | `--color-border-default` | 弹窗头部边框 |
| `--modal-header-text` | `--color-text-primary` | 弹窗头部文字 |

#### 弹窗内容

| 变量 | 值 | 用途 |
|------|-----|------|
| `--modal-content-bg` | transparent | 弹窗内容背景 |
| `--modal-content-text` | `--color-text-primary` | 弹窗内容文字 |

#### 弹窗底部

| 变量 | 值 | 用途 |
|------|-----|------|
| `--modal-footer-bg` | `--color-bg-card` | 弹窗底部背景 |
| `--modal-footer-border` | `--color-border-default` | 弹窗底部边框 |
| `--modal-footer-text` | `--color-text-primary` | 弹窗底部文字 |

### 3.7 页面布局 (Page Layout)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--container-sm` | 640px | 小容器宽度 |
| `--container-md` | 768px | 中容器宽度 |
| `--container-lg` | 1024px | 大容器宽度 |
| `--container-xl` | 1280px | 超大容器宽度 |
| `--container-2xl` | 1536px | 双倍超大容器宽度 |
| `--page-padding-x` | `--spacing-md` | 页面水平内边距 |
| `--page-padding-y` | `--spacing-lg` | 页面垂直内边距 |
| `--container-max-width` | `--container-xl` | 容器最大宽度 |

---

## RGBA 颜色变量

用于半透明效果的颜色变量。

| 变量 | 值 | 用途 |
|------|-----|------|
| `--rgba-slate-900-95` | 30, 41, 59, 0.95 | 深色玻璃效果 95% |
| `--rgba-slate-900-98` | 30, 41, 59, 0.98 | 深色玻璃效果 98% |
| `--rgba-primary-90` | 59, 130, 246, 0.9 | 主色玻璃效果 90% |
| `--rgba-white-25` | 255, 255, 255, 0.25 | 白色玻璃 25% |
| `--rgba-white-35` | 255, 255, 255, 0.35 | 白色玻璃 35% |
| `--rgba-white-55` | 255, 255, 255, 0.55 | 白色玻璃 55% |
| `--rgba-white-7` | 255, 255, 255, 0.07 | 白色玻璃 7% |
| `--rgba-white-1` | 255, 255, 255, 0.1 | 白色玻璃 10% |
| `--rgba-white-2` | 255, 255, 255, 0.2 | 白色玻璃 20% |
| `--rgba-white-14` | 255, 255, 255, 0.14 | 白色玻璃 14% |
| `--rgba-black-3` | 0, 0, 0, 0.3 | 黑色玻璃 30% |
| `--rgba-black-95` | 0, 0, 0, 0.95 | 黑色玻璃 95% |
| `--rgba-shadow-primary` | 59, 130, 246, 0.4 | 主色阴影 |
| `--rgba-shadow-secondary` | 0, 0, 0, 0.3 | 次要阴影 |
| `--rgba-shadow-tertiary` | 0, 0, 0, 0.2 | 三级阴影 |

---

## 尺寸系统

### 4.1 间距 (Spacing)

基于 4px 基础单位的间距系统。

| 变量 | 值 | Tailwind 对应 |
|------|-----|------------|
| `--spacing-0` | 0px | gap-0, p-0, m-0 |
| `--spacing-px` | 1px | gap-px |
| `--spacing-0_5` | 2px | gap-0.5, p-0.5, m-0.5 |
| `--spacing-1` | 4px | gap-1, p-1, m-1 |
| `--spacing-1_5` | 6px | - |
| `--spacing-2` | 8px | gap-2, p-2, m-2 |
| `--spacing-2_5` | 10px | - |
| `--spacing-3` | 12px | gap-3, p-3, m-3 |
| `--spacing-3_5` | 14px | - |
| `--spacing-4` | 16px | gap-4, p-4, m-4 |
| `--spacing-5` | 20px | gap-5, p-5, m-5 |
| `--spacing-6` | 24px | gap-6, p-6, m-6 |
| `--spacing-7` | 28px | - |
| `--spacing-8` | 32px | gap-8, p-8, m-8 |
| `--spacing-9` | 36px | - |
| `--spacing-10` | 40px | gap-10, p-10, m-10 |
| `--spacing-11` | 44px | - |
| `--spacing-12` | 48px | gap-12, p-12, m-12 |
| `--spacing-14` | 56px | - |
| `--spacing-16` | 64px | gap-16, p-16, m-16 |
| `--spacing-20` | 80px | - |
| `--spacing-24` | 96px | - |
| `--spacing-28` | 112px | - |
| `--spacing-32` | 128px | - |
| `--spacing-36` | 144px | - |
| `--spacing-40` | 160px | - |
| `--spacing-44` | 176px | - |
| `--spacing-48` | 192px | - |
| `--spacing-56` | 224px | - |
| `--spacing-64` | 256px | - |

### 4.2 圆角 (Radius)

| 变量 | 值 | Tailwind 对应 |
|------|-----|------------|
| `--radius-none` | 0px | rounded-none |
| `--radius-sm` | 4px | rounded-sm |
| `--radius-base` | 6px | rounded (base) |
| `--radius-md` | 8px | rounded-md |
| `--radius-lg` | 12px | rounded-lg |
| `--radius-xl` | 16px | rounded-xl |
| `--radius-2xl` | 24px | rounded-2xl |
| `--radius-3xl` | 32px | rounded-3xl |
| `--radius-full` | 9999px | rounded-full |

### 4.3 阴影 (Shadows)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--shadow` | 0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1) | 基础阴影 |
| `--shadow-none` | 0 0 #0000 | 无阴影 |
| `--shadow-sm` | 0 1px 2px 0 rgb(0 0 0 / 0.05) | 小阴影 |
| `--shadow-base` | `--shadow` | 基础阴影（别名） |
| `--shadow-md` | 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1) | 中阴影 |
| `--shadow-lg` | 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1) | 大阴影 |
| `--shadow-xl` | 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1) | 超大阴影 |
| `--shadow-2xl` | 0 25px 50px -12px rgb(0 0 0 / 0.25) | 双倍大阴影 |
| `--shadow-inner` | inset 0 2px 4px 0 rgb(0 0 0 / 0.05) | 内阴影 |
| `--shadow-primary` | 0 10px 15px -3px rgba(59, 130, 246, 0.2), 0 4px 6px -2px rgba(59, 130, 246, 0.1) | 主色阴影 |
| `--shadow-text` | 0 2px 4px rgba(0, 0, 0, 0.2) | 文字阴影 |

### 4.4 字体系统 (Typography)

#### 字族 (Font Family)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--font-family-base` | -apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", "PingFang SC", "Hiragino Sans GB", "Helvetica Neue", Arial, sans-serif | 基础字族 |
| `--font-family-sans` | 'Inter', `--font-family-base` | 无衬线字族 |
| `--font-family-mono` | 'SF Mono', Monaco, 'Cascadia Code', 'Roboto Mono', Consolas, 'Courier New', monospace | 等宽字族 |

#### 字号 (Font Size)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--font-size-base` | 1rem (16px) | 基础字号 |
| `--font-size-h1` | 2.25rem (36px) | H1 标题 |
| `--font-size-h2` | 1.875rem (30px) | H2 标题 |
| `--font-size-h3` | 1.5rem (24px) | H3 标题 |
| `--font-size-h4` | 1.25rem (20px) | H4 标题 |
| `--text-xs` | 0.75rem (12px) | 极小文字 |
| `--text-sm` | 0.875rem (14px) | 小文字 |
| `--text-base` | `--font-size-base` | 基础文字 |
| `--text-lg` | 1.125rem (18px) | 大文字 |
| `--text-xl` | 1.25rem (20px) | 超大文字 |
| `--text-2xl` | 1.5rem (24px) | 2倍大文字 |
| `--text-3xl` | 1.875rem (30px) | 3倍大文字 |
| `--text-4xl` | 2.25rem (36px) | 4倍大文字 |
| `--text-5xl` | 3rem (48px) | 5倍大文字 |
| `--text-6xl` | 3.75rem (60px) | 6倍大文字 |
| `--text-7xl` | 4.5rem (72px) | 7倍大文字 |
| `--text-8xl` | 6rem (96px) | 8倍大文字 |
| `--text-9xl` | 8rem (128px) | 9倍大文字 |

#### 字重 (Font Weight)

| 变量 | 值 | Tailwind 对应 |
|------|-----|------------|
| `--font-weight-base` | 400 | font-normal |
| `--font-thin` | 100 | font-thin |
| `--font-light` | 300 | font-light |
| `--font-normal` | `--font-weight-base` | font-normal |
| `--font-medium` | 500 | font-medium |
| `--font-semibold` | 600 | font-semibold |
| `--font-bold` | 700 | font-bold |
| `--font-extrabold` | 800 | font-extrabold |
| `--font-black` | 900 | font-black |

#### 行高 (Line Height)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--line-height-base` | 1.6 | 基础行高 |
| `--line-height-none` | 1 | 无行高 |
| `--line-height-tight` | 1.25 | 紧凑行高 |
| `--line-height-snug` | 1.375 | 适中行高 |
| `--line-height-normal` | `--line-height-base` | 正常行高 |
| `--line-height-relaxed` | 1.625 | 宽松行高 |
| `--line-height-loose` | 2 | 极宽行高 |

#### 字母间距 (Letter Spacing)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--tracking-tighter` | -0.025em | 更紧 |
| `--tracking-tight` | -0.0125em | 紧凑 |
| `--tracking-normal` | 0 | 正常 |
| `--tracking-wide` | 0.0125em | 宽松 |
| `--tracking-wider` | 0.025em | 更宽 |
| `--tracking-widest` | 0.05em | 极宽 |

### 4.5 Z-index 层级 (Z-Index)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--z-dropdown` | 1000 | 下拉菜单 |
| `--z-sticky` | 1020 | 粘性元素 |
| `--z-fixed` | 1030 | 固定元素 |
| `--z-modal-backdrop` | 1040 | 弹窗遮罩 |
| `--z-modal` | 1050 | 弹窗 |
| `--z-popover` | 1060 | 气泡 |
| `--z-tooltip` | 1070 | 工具提示 |
| `--z-toast` | 1080 | 提示消息 |

### 4.6 过渡动画 (Transition)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--transition-none` | none | 无过渡 |
| `--transition-all` | all | 全属性过渡 |
| `--transition` | all 0.2s ease-in-out | 默认过渡 |
| `--transition-faster` | all 0.15s ease-in-out | 快速过渡 |
| `--transition-slower` | all 0.3s ease-in-out | 慢速过渡 |

#### 缓动函数 (Easing Functions)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--ease-linear` | linear | 线性 |
| `--ease-in` | ease-in | 渐入 |
| `--ease-out` | ease-out | 渐出 |
| `--ease-in-out` | ease-in-out | 渐入渐出 |
| `--ease-cubic-bezier` | 0.4, 0, 0.2, 1 | 自定义曲线 |

---

## 动画关键帧

### 5.1 可用动画

| 动画名称 | 描述 | 使用类 |
|---------|------|-------|
| `fadeIn` | 淡入 | `.animate-fade-in` |
| `fadeInUp` | 向上淡入 | `.animate-fade-in-up` |
| `fadeInDown` | 向下淡入 | `.animate-fade-in-down` |
| `slideInLeft` | 从左侧滑入 | `.animate-slide-in-left` |
| `slideInRight` | 从右侧滑入 | `.animate-slide-in-right` |
| `pulse` | 脉冲 | `.animate-pulse` |
| `spin` | 旋转 | `.animate-spin` |
| `bounce` | 弹跳 | `.animate-bounce` |

---

## 通用辅助类

### 6.1 文本颜色类

| 类名 | 对应 Token | 用途 |
|------|-----------|------|
| `.text-primary` | `--color-text-primary` | 主要文字 |
| `.text-secondary` | `--color-text-secondary` | 次要文字 |
| `.text-tertiary` | `--color-text-tertiary` | 三级文字 |
| `.text-quaternary` | `--color-text-quaternary` | 四级文字 |
| `.text-success` | `--color-text-success` | 成功文字 |
| `.text-warning` | `--color-text-warning` | 警告文字 |
| `.text-error` | `--color-text-error` | 错误文字 |
| `.text-info` | `--color-text-info` | 信息文字 |

### 6.2 背景颜色类

| 类名 | 对应 Token | 用途 |
|------|-----------|------|
| `.bg-primary` | `--color-primary` | 主色背景 |
| `.bg-primary-hover` | `--color-primary-hover` | 主色悬停背景 |
| `.bg-success` | `--color-success-bg` | 成功背景 |
| `.bg-warning` | `--color-warning-bg` | 警告背景 |
| `.bg-error` | `--color-error-bg` | 错误背景 |
| `.bg-info` | `--color-info-bg` | 信息背景 |

### 6.3 边框颜色类

| 类名 | 对应 Token | 用途 |
|------|-----------|------|
| `.border-primary` | `--color-primary` | 主色边框 |
| `.border-success` | `--color-success-border` | 成功边框 |
| `.border-warning` | `--color-warning-border` | 警告边框 |
| `.border-error` | `--color-error-border` | 错误边框 |
| `.border-info` | `--color-info-border` | 信息边框 |

### 6.4 阴影类

| 类名 | 对应 Token | 用途 |
|------|-----------|------|
| `.shadow-sm` | `--shadow-sm` | 小阴影 |
| `.shadow` | `--shadow-base` | 基础阴影 |
| `.shadow-md` | `--shadow-md` | 中阴影 |
| `.shadow-lg` | `--shadow-lg` | 大阴影 |
| `.shadow-xl` | `--shadow-xl` | 超大阴影 |
| `.shadow-2xl` | `--shadow-2xl` | 双倍大阴影 |

### 6.5 圆角类

| 类名 | 对应 Token | 用途 |
|------|-----------|------|
| `.rounded-sm` | `--radius-sm` | 小圆角 |
| `.rounded` | `--radius-base` | 基础圆角 |
| `.rounded-md` | `--radius-md` | 中圆角 |
| `.rounded-lg` | `--radius-lg` | 大圆角 |
| `.rounded-xl` | `--radius-xl` | 超大圆角 |
| `.rounded-2xl` | `--radius-2xl` | 双倍大圆角 |
| `.rounded-full` | `--radius-full` | 完整圆角 |

### 6.6 过渡类

| 类名 | 对应 Token | 用途 |
|------|-----------|------|
| `.transition` | `--transition` | 默认过渡 |
| `.transition-faster` | `--transition-faster` | 快速过渡 |
| `.transition-slower` | `--transition-slower` | 慢速过渡 |
| `.transition-all` | `--transition-all` | 全属性过渡 |

### 6.7 动画类

| 类名 | 对应动画 | 用途 |
|------|-----------|------|
| `.animate-fade-in` | fadeIn | 淡入动画 |
| `.animate-fade-in-up` | fadeInUp | 向上淡入动画 |
| `.animate-fade-in-down` | fadeInDown | 向下淡入动画 |
| `.animate-slide-in-left` | slideInLeft | 左侧滑入动画 |
| `.animate-slide-in-right` | slideInRight | 右侧滑入动画 |
| `.animate-pulse` | pulse | 脉冲动画 |
| `.animate-spin` | spin | 旋转动画 |
| `.animate-bounce` | bounce | 弹跳动画 |

---

## Token 使用规范

### 7.1 使用优先级

**优先使用顺序**：
1. ✅ Semantic 变量（优先级最高）: `--color-text-main`, `--color-bg-card`
2. ✅ Component 变量: `--btn-bg-primary`, `--input-border-focus`
3. ⚠️ Foundation 变量（仅用于装饰）: `--color-blue-500`, `--color-purple-200`

### 7.2 禁止的使用方式

| 禁止项 | 原因 | 正确做法 |
|--------|------|---------|
| ❌ 直接写十六进制颜色 | 无法主题切换 | 使用 `--color-text-*` / `--color-bg-*` |
| ❌ 使用 Tailwind 颜色类 | 不适配主题 | 使用语义化类或自定义 CSS |
| ❌ 随意发明新 Token | 导致 Token 膨胀 | 优先使用现有 Token |
| ❌ 在组件内定义局部 Token | 破坏统一性 | 全局 Token 统一管理 |

### 7.3 允许的特殊情况

| 情况 | 说明 |
|------|------|
| **装饰性颜色** | 纯装饰性的渐变或特殊效果可以使用基础色板变量 |
| **品牌色** | 品牌官方颜色（如编程语言 Logo 色）可以保留 |
| **图表色** | 图表需要特定配色方案时可以使用硬编码 |
| **后备值** | CSS 变量可以提供后备值，如 `var(--color-primary, #3b82f6)` |

### 7.4 新增 Token 流程

1. **检查现有 Token** - 确认没有类似变量
2. **选择正确层级** - Foundation 层还是 Semantic 层
3. **更新主题变量文件** - 添加到 `assets/styles/theme-variables.css`
4. **更新此文档** - 同步更新 Token 参考文档
5. **Code Review** - 新增 Token 需要技术负责人 Review

---

## 兼容性变量

### 8.1 旧变量别名

为保持向后兼容，以下变量保留作为别名：

| 旧变量 | 新变量 | 状态 |
|--------|--------|------|
| `--primary-soft-bg` | `--color-primary-lighter` | 兼容 |
| `--color-secondary` | `--color-text-secondary` | 兼容 |
| `--color-background` | `--color-bg-body` | 兼容 |
| `--color-dark` | `--color-gray-900` | 兼容 |

### 8.2 管理布局专用变量

用于后台管理系统的特定变量：

| 变量 | 值 | 用途 |
|------|-----|------|
| `--admin-sidebar-text` | #ffffff | 管理侧边栏文字 |
| `--admin-grid-size` | 40px | 管理网格大小 |
| `--color-secondary` | #6b7280 | 次要颜色（兼容） |
| `--color-background` | #f3f4f6 | 背景颜色（兼容） |
| `--color-dark` | #1f2937 | 深色（兼容） |
| `--color-dark-dark` | #111827 | 更深色（兼容） |
| `--color-primary-dark` | #2563eb | 主色深（兼容） |

---

## 快速参考

### 最常用 Token（Top 20）

| 排名 | 变量 | 用途 |
|------|------|------|
| 1 | `--color-text-main` | 主要文字 |
| 2 | `--color-bg-card` | 卡片背景 |
| 3 | `--color-primary` | 主色 |
| 4 | `--color-border-default` | 默认边框 |
| 5 | `--color-bg-elevated` | 提升层背景 |
| 6 | `--spacing-4` | 标准间距 (16px) |
| 7 | `--spacing-6` | 较大间距 (24px) |
| 8 | `--radius-md` | 标准圆角 (8px) |
| 9 | `--shadow-md` | 标准阴影 |
| 10 | `--btn-bg-primary` | 主按钮背景 |
| 11 | `--color-text-secondary` | 次要文字 |
| 12 | `--color-success-bg` | 成功背景 |
| 13 | `--color-error-bg` | 错误背景 |
| 14 | `--input-bg` | 输入框背景 |
| 15 | `--card-bg` | 卡片背景 |
| 16 | `--color-bg-body` | 页面背景 |
| 17 | `--transition` | 默认过渡 |
| 18 | `--spacing-2` | 小间距 (8px) |
| 19 | `--radius-lg` | 大圆角 (12px) |
| 20 | `--color-border-focus` | 聚焦边框 |

---

## 版本历史

- **v1.0** (2026-03-14): 初始版本，完整的 Token 参考文档
