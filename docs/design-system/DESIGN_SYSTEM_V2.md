# PersonWeb Design System V2

> **核心理念**: Modern Fintech + Light Glassmorphism
> **关键词**: 高级感、高对比度、呼吸感、科技蓝

## 1. 颜色系统 (Color System)

我们移除了所有旧的杂乱主题，只保留 **Light** (默认) 和 **Dark** (科技深空)。

### 1.1 主色调 (Primary)

采用更具金融科技感的"电光蓝"。

| Token | Light Value | Dark Value | 用途 |
|Val|---|---|---|
| `primary` | `#0052FF` | `#3B82F6` | 主要按钮、链接、高亮 |
| `primary-hover` | `#0040D6` | `#60A5FA` | 悬停状态 |
| `primary-soft` | `rgba(0, 82, 255, 0.08)` | `rgba(59, 130, 246, 0.15)` | 次级背景、选区 |

### 1.2 背景色 (Background)

| Token | Light Value | Dark Value | 说明 |
|Val|---|---|---|
| `body` | `#F5F7FA` | `#0B0E14` | 全局背景，Light 偏冷灰，Dark 偏深蓝黑 |
| `card` | `#FFFFFF` | `#151B28` | 卡片背景 |
| `glass` | `rgba(255, 255, 255, 0.7)` | `rgba(21, 27, 40, 0.7)` | 玻璃态背景 (需配合 backdrop-filter) |

### 1.3 辅助色 (Functional)

- **Success**: `#00D18B` (明亮薄荷绿)
- **Warning**: `#FFAA00` (琥珀金)
- **Error**: `#FF3B30` (醒目红)

## 2. 字体排版 (Typography)

统一使用系统字体栈，强调数字的可读性。

- **Font Family**: `Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif`
- **Headings**: 紧凑字间距 (`letter-spacing: -0.02em`)，加粗 (`600/700`)
- **Body**: 清晰易读 (`line-height: 1.6`)

## 3. 组件风格 (Components)

### 3.1 卡片 (Card)
- **Light**: 纯白背景 + 柔和阴影 (`0 4px 20px rgba(0,0,0,0.05)`) + 极细边框 (`1px solid rgba(0,0,0,0.05)`)
- **Dark**: 深蓝黑背景 + 顶部微亮渐变边框 + 辉光阴影

### 3.2 按钮 (Button)
- **Primary**: 这里的按钮不仅是颜色填充，Hover 时会有轻微的上浮和光效增强。
- **Radius**: 小组件（按钮/输入框）统一 `6px` 或 `8px`，大卡片统一 `16px`。

### 3.3 图表 (Charts)
- **Grid**: 极淡或者是虚线。
- **Tooltip**: 玻璃态背景。
- **Color**: 对应 Design System 的主色和辅助色序列。

## 4. 实现规范

- **禁止** 在页面组件中写死颜色值 (如 `#FFF`, `#000`)。
- **必须** 使用 `useTheme` 提供的 Naive UI `themeOverrides` 或 CSS 变量。
- **必须** 遵循 CSS 变量语义，例如使用 `var(--bg-body)` 而不是 `var(--gray-100)`.
