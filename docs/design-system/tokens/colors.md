# 颜色系统 (Colors)

Aurora Design System 的色彩规范，支持三种主题模式。

## 主题模式

| 主题 | 说明 | 适用场景 |
|------|------|----------|
| `light` | 浅色主题 | 默认主题，适合日间使用 |
| `dark` | 深色主题 | 适合夜间使用 |
| `hybrid-super-dark` | 超级深色主题 | 深度沉浸体验 |

---

## 主色板 (Primary Colors)

### Primary 品牌色

```
--color-primary           #2997FF
--color-primary-hover    #60A5FA
--color-primary-pressed  #1E40AF
--color-primary-suppl    rgba(41, 151, 255, 0.15)
```

### 使用场景

- 主要按钮
- 链接
- 高亮状态
- 激活状态

### 使用示例

```css
.button-primary {
  background-color: var(--color-primary);
  color: var(--color-text-on-primary);
}

.button-primary:hover {
  background-color: var(--color-primary-hover);
}
```

---

## 语义颜色 (Semantic Colors)

### 成功色

```
--color-success         #10B981
--color-success-soft    rgba(16, 185, 129, 0.1)
--color-success-600     #059669
--color-success-700     #047857
```

### 警告色

```
--color-warning         #F59E0B
--color-warning-soft    rgba(245, 158, 11, 0.1)
--color-warning-600     #D97706
--color-warning-700     #B45309
```

### 错误色

```
--color-error           #EF4444
--color-error-soft      rgba(239, 68, 68, 0.1)
--color-error-600       #DC2626
--color-error-700       #B91C1C
```

### 信息色

```
--color-info            #3B82F6
--color-info-soft       rgba(59, 130, 246, 0.1)
--color-info-600        #2563EB
--color-info-700        #1D4ED8
```

---

## 中性色 (Neutral Colors)

### 背景色系

```
--color-bg-body         #020617 (dark) / #FFFFFF (light)
--color-bg-card        rgba(20, 25, 40, 0.6) (dark) / #FFFFFF (light)
--color-bg-elevated    rgba(40, 50, 70, 0.6) (dark) / #F3F4F6 (light)
--color-bg-dark        #0A0A1A (dark) / #1F2937 (light)
```

### 文本色系

```
--color-text-main       #FFFFFF (dark) / #111827 (light)
--color-text-sec       rgba(255, 255, 255, 0.7) (dark) / #6B7280 (light)
--color-text-muted     rgba(255, 255, 255, 0.5) (dark) / #9CA3AF (light)
--color-text-disabled   rgba(255, 255, 255, 0.3) (dark) / #D1D5DB (light)
```

### 边框色系

```
--color-border-default  rgba(255, 255, 255, 0.1) (dark) / rgba(0, 0, 0, 0.1) (light)
--color-border-subtle   rgba(255, 255, 255, 0.05) (dark) / rgba(0, 0, 0, 0.05) (light)
--color-border-hover    rgba(41, 151, 255, 0.5) (dark) / rgba(0, 0, 0, 0.2) (light)
```

---

## 功能色 (Functional Colors)

| 变量 | 值 | 用途 |
|------|-----|------|
| `--color-link` | `var(--color-primary)` | 链接颜色 |
| `--color-link-hover` | `var(--color-primary-hover)` | 链接悬停 |
| `--color-focus-ring` | `var(--color-primary-suppl)` | 焦点环 |
| `--color-overlay` | `rgba(0, 0, 0, 0.5)` | 遮罩层 |
| `--color-backdrop` | `rgba(0, 0, 0, 0.3)` | 背景模糊 |

---

## 渐变色 (Gradient Colors)

### 主要渐变

```css
--gradient-primary   linear-gradient(135deg, var(--color-primary) 0%, #8B5CF6 100%)
--gradient-dark     linear-gradient(135deg, #1E3A5F 0%, #0A0A1A 100%)
--gradient-glass   linear-gradient(135deg, rgba(255, 255, 255, 0.1) 0%, rgba(255, 255, 255, 0.05) 100%)
```

### 使用示例

```css
.hero-section {
  background: var(--gradient-primary);
}

.glass-card {
  background: var(--gradient-glass);
  backdrop-filter: blur(10px);
}
```

---

## 主题切换

### CSS 变量方法

```css
[data-theme="light"] {
  --color-bg-body: #FFFFFF;
  --color-text-main: #111827;
}

[data-theme="dark"] {
  --color-bg-body: #020617;
  --color-text-main: #FFFFFF;
}
```

### JavaScript 方法

```javascript
// 切换到浅色主题
document.documentElement.setAttribute('data-theme', 'light')

// 切换到深色主题
document.documentElement.setAttribute('data-theme', 'dark')

// 切换到超级深色主题
document.documentElement.setAttribute('data-theme', 'hybrid-super-dark')
```

### Vue Composable

```javascript
import { useTheme } from '~/composables/useTheme'

const { currentTheme, setTheme, toggleTheme } = useTheme()

// 切换主题
setTheme('dark')

// 切换主题（自动）
toggleTheme()
```

---

## 颜色工具类

### 文字颜色

```css
.text-primary      { color: var(--color-primary); }
.text-success      { color: var(--color-success); }
.text-warning      { color: var(--color-warning); }
.text-error        { color: var(--color-error); }
.text-muted        { color: var(--color-text-muted); }
```

### 背景颜色

```css
.bg-primary       { background-color: var(--color-primary); }
.bg-success       { background-color: var(--color-success); }
.bg-warning       { background-color: var(--color-warning); }
.bg-error         { background-color: var(--color-error); }
.bg-card          { background-color: var(--color-bg-card); }
```

---

## 无障碍性 (Accessibility)

### 对比度要求

- 正文文本：至少 4.5:1
- 大号文本（18pt+）：至少 3:1
- UI 组件：至少 3:1

### 测试工具

- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)
- [Colour Contrast Analyser](https://www.tpgi.com/color-contrast-checker/)

---

## 最佳实践

1. **始终使用 CSS 变量**，不要硬编码颜色值
2. **使用语义颜色**来表达状态（成功、警告、错误）
3. **确保对比度符合** WCAG 标准
4. **避免使用纯黑/纯白**，使用深灰代替
5. **保持一致性**，相同元素使用相同颜色

---

## 相关文档

- [排版系统](./typography.md)
- [间距系统](./spacing.md)
- [主题系统](../guides/theme-system.md)
