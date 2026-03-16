# 动画系统 (Animations)

Aurora Design System 的动画规范，定义过渡、缓动和关键帧动画。

## 缓动函数 (Easing)

| 变量 | 值 | 说明 |
|------|-----|------|
| `--ease-linear` | linear | 线性（无加速） |
| `--ease-in` | cubic-bezier(0.4, 0, 1, 1) | 加速 |
| `--ease-out` | cubic-bezier(0, 0, 0.2, 1) | 减速 |
| `--ease-in-out` | cubic-bezier(0.4, 0, 0.2, 1) | 加速后减速（推荐） |
| `--ease-bounce` | cubic-bezier(0.68, -0.55, 0.265, 1.55) | 弹跳 |
| `--ease-elastic` | cubic-bezier(0.68, -0.6, 0.32, 1.6) | 弹性 |

---

## 过渡时间 (Duration)

| 变量 | 值 | 说明 |
|------|-----|------|
| `--duration-fast` | 150ms | 快速（悬停、焦点） |
| `--duration-normal` | 250ms | 正常（默认） |
| `--duration-slow` | 350ms | 慢速（复杂动画） |
| `--duration-slower` | 500ms | 更慢（页面切换） |

---

## 过渡预设

```css
--transition-fast       var(--duration-fast) var(--ease-in-out)
--transition-normal     var(--duration-normal) var(--ease-in-out)
--transition-slow      var(--duration-slow) var(--ease-in-out)
```

---

## 组件过渡

### 按钮过渡

```css
.button {
  transition: all var(--transition-fast);
}

.button:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-lg);
}
```

### 卡片过渡

```css
.card {
  transition: transform var(--transition-normal),
              box-shadow var(--transition-normal);
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-xl);
}
```

### 输入框过渡

```css
.input {
  transition: border-color var(--transition-fast),
              box-shadow var(--transition-fast);
}

.input:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px var(--color-primary-suppl);
}
```

---

## 过渡工具类

```css
.transition-none        { transition: none; }
.transition-all         { transition: all var(--transition-normal); }
.transition-fast        { transition: all var(--transition-fast); }
.transition-slow       { transition: all var(--transition-slow); }

/* 属性过渡 */
.transition-colors      { transition: color var(--transition-fast), background-color var(--transition-fast); }
.transition-opacity     { transition: opacity var(--transition-fast); }
.transition-transform   { transition: transform var(--transition-normal); }
.transition-shadow     { transition: box-shadow var(--transition-normal); }
```

---

## 关键帧动画

### 淡入淡出

```css
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes fadeOut {
  from { opacity: 1; }
  to { opacity: 0; }
}

.animate-fadeIn {
  animation: fadeIn var(--duration-normal) var(--ease-in-out);
}

.animate-fadeOut {
  animation: fadeOut var(--duration-normal) var(--ease-in-out);
}
```

### 滑入滑出

```css
@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes slideDown {
  from {
    opacity: 1;
    transform: translateY(0);
  }
  to {
    opacity: 0;
    transform: translateY(-20px);
  }
}

.animate-slideUp {
  animation: slideUp var(--duration-normal) var(--ease-out);
}

.animate-slideDown {
  animation: slideDown var(--duration-normal) var(--ease-in);
}
```

### 缩放动画

```css
@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes scaleOut {
  from {
    opacity: 1;
    transform: scale(1);
  }
  to {
    opacity: 0;
    transform: scale(0.9);
  }
}

.animate-scaleIn {
  animation: scaleIn var(--duration-normal) var(--ease-out);
}

.animate-scaleOut {
  animation: scaleOut var(--duration-normal) var(--ease-in);
}
```

### 旋转动画

```css
@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

.animate-pulse {
  animation: pulse 2s ease-in-out infinite;
}
```

### 弹跳动画

```css
@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
    animation-timing-function: var(--ease-in-out);
  }
  50% {
    transform: translateY(-10px);
    animation-timing-function: var(--ease-bounce);
  }
}

.animate-bounce {
  animation: bounce 1s infinite;
}
```

---

## 常用动画模式

### 列表项进入

```css
.list-item {
  opacity: 0;
  transform: translateY(10px);
  animation: slideUp 0.3s var(--ease-out) forwards;
}

.list-item:nth-child(1) { animation-delay: 0ms; }
.list-item:nth-child(2) { animation-delay: 50ms; }
.list-item:nth-child(3) { animation-delay: 100ms; }
.list-item:nth-child(4) { animation-delay: 150ms; }
```

### 悬浮效果

```css
.hover-lift {
  transition: transform var(--transition-normal),
              box-shadow var(--transition-normal);
}

.hover-lift:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-xl);
}
```

### 聚焦环

```css
.focus-ring {
  transition: box-shadow var(--transition-fast);
}

.focus-ring:focus {
  outline: none;
  box-shadow: 0 0 0 3px var(--color-primary-suppl);
}
```

---

## 动画工具类

### 入场动画

```css
.animate-fade-in       { animation: fadeIn var(--duration-normal) var(--ease-in-out); }
.animate-slide-up      { animation: slideUp var(--duration-normal) var(--ease-out); }
.animate-scale-in      { animation: scaleIn var(--duration-normal) var(--ease-out); }
```

### 出场动画

```css
.animate-fade-out      { animation: fadeOut var(--duration-normal) var(--ease-in-out); }
.animate-slide-down    { animation: slideDown var(--duration-normal) var(--ease-in); }
.animate-scale-out     { animation: scaleOut var(--duration-normal) var(--ease-in); }
```

### 持续动画

```css
.animate-spin         { animation: spin 1s linear infinite; }
.animate-pulse        { animation: pulse 2s ease-in-out infinite; }
.animate-bounce       { animation: bounce 1s infinite; }
```

---

## 性能优化

### 使用 transform 和 opacity

```css
/* 推荐：使用 transform 和 opacity */
.card {
  transform: translateY(0);
  opacity: 1;
  transition: transform var(--transition-normal),
              opacity var(--transition-fast);
}

/* 避免：直接动画化布局属性 */
.card {
  /* 避免 */
  left: 0;
  margin: 0;
  width: 100%;
  transition: left var(--transition-normal),
              margin var(--transition-normal),
              width var(--transition-normal);
}
```

### 使用 will-change

```css
.button {
  will-change: transform;
}

.modal {
  will-change: opacity, transform;
}
```

---

## 减少动画

```css
/* 尊重用户偏好：减少动画 */
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
  }
}
```

---

## 最佳实践

1. **使用 CSS 变量**定义动画参数
2. **使用 transform 和 opacity**实现动画，性能更好
3. **避免动画化布局属性**（width、height、margin 等）
4. **尊重用户偏好**，支持减少动画
5. **保持过渡时间一致**，相同元素使用相同过渡
6. **使用适当的缓动函数**，ease-in-out 最常用

---

## 相关文档

- [颜色系统](./colors.md)
- [阴影系统](./shadows.md)
- [开发规范](../guides/coding-standards.md)
