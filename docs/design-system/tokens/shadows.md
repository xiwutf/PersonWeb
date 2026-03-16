# 阴影系统 (Shadows)

Aurora Design System 的阴影规范，用于表达深度和层级关系。

## 阴影层级

| 变量 | 值 | 说明 |
|------|-----|------|
| `--shadow-xs` | 0 1px 2px rgba(0, 0, 0, 0.05) | 极浅阴影 |
| `--shadow-sm` | 0 2px 4px rgba(0, 0, 0, 0.1) | 小阴影 |
| `--shadow-md` | 0 4px 6px rgba(0, 0, 0, 0.1) | 中等阴影 |
| `--shadow-lg` | 0 8px 16px rgba(0, 0, 0, 0.15) | 大阴影 |
| `--shadow-xl` | 0 12px 24px rgba(0, 0, 0, 0.2) | 超大阴影 |
| `--shadow-2xl` | 0 20px 40px rgba(0, 0, 0, 0.25) | 巨大阴影 |

### 深色主题阴影

深色主题使用更浅的阴影，避免过度对比：

```css
[data-theme="dark"] {
  --shadow-xs: 0 1px 2px rgba(0, 0, 0, 0.3);
  --shadow-sm: 0 2px 4px rgba(0, 0, 0, 0.4);
  --shadow-md: 0 4px 6px rgba(0, 0, 0, 0.4);
  --shadow-lg: 0 8px 16px rgba(0, 0, 0, 0.6);
  --shadow-xl: 0 12px 24px rgba(0, 0, 0, 6);
  --shadow-2xl: 0 20px 40px rgba(0, 0, 0, 0.8);
}
```

---

## 组件阴影规范

### 按钮阴影

```css
--shadow-button-sm      var(--shadow-sm)
--shadow-button-md      var(--shadow-md)
--shadow-button-hover    var(--shadow-lg)
```

### 卡片阴影

```css
--shadow-card-sm        var(--shadow-sm)
--shadow-card-md        var(--shadow-md)
--shadow-card-lg        var(--shadow-lg)
--shadow-card-hover      var(--shadow-xl)
```

### 弹层阴影

```css
--shadow-modal          var(--shadow-xl)
--shadow-popover        var(--shadow-lg)
--shadow-tooltip        var(--shadow-md)
```

---

## 阴影工具类

```css
.shadow-none        { box-shadow: none; }
.shadow-xs          { box-shadow: var(--shadow-xs); }
.shadow-sm          { box-shadow: var(--shadow-sm); }
.shadow-md          { box-shadow: var(--shadow-md); }
.shadow-lg          { box-shadow: var(--shadow-lg); }
.shadow-xl          { box-shadow: var(--shadow-xl); }
.shadow-2xl         { box-shadow: var(--shadow-2xl); }
```

---

## 阴影使用示例

### 按钮阴影

```css
.button {
  box-shadow: var(--shadow-button-sm);
  transition: all 0.2s;
}

.button:hover {
  box-shadow: var(--shadow-button-hover);
  transform: translateY(-2px);
}

.button:active {
  box-shadow: var(--shadow-xs);
  transform: translateY(0);
}
```

### 卡片阴影

```css
.card {
  box-shadow: var(--shadow-card-md);
  border-radius: var(--radius-lg);
  transition: all 0.3s;
}

.card:hover {
  box-shadow: var(--shadow-card-hover);
  transform: translateY(-4px);
}
```

### 弹层阴影

```css
.modal-overlay {
  box-shadow: var(--shadow-modal);
}

.popover {
  box-shadow: var(--shadow-popover);
}

.tooltip {
  box-shadow: var(--shadow-tooltip);
}
```

---

## 阴影与深度

### 深度层级

```css
/* 深度 1: 浮动元素 */
.depth-1 {
  box-shadow: var(--shadow-sm);
}

/* 深度 2: 卡片 */
.depth-2 {
  box-shadow: var(--shadow-md);
}

/* 深度 3: 悬浮卡片 */
.depth-3 {
  box-shadow: var(--shadow-lg);
}

/* 深度 4: 弹层 */
.depth-4 {
  box-shadow: var(--shadow-xl);
}

/* 深度 5: 模态框 */
.depth-5 {
  box-shadow: var(--shadow-2xl);
}
```

### Z-index 与阴影配合

```css
.z-10  { z-index: 10; box-shadow: var(--shadow-md); }
.z-20  { z-index: 20; box-shadow: var(--shadow-lg); }
.z-30  { z-index: 30; box-shadow: var(--shadow-xl); }
.z-40  { z-index: 40; box-shadow: var(--shadow-2xl); }
```

---

## 彩色阴影

### 主色阴影

```css
--shadow-primary      0 4px 14px rgba(41, 151, 255, 0.25)
--shadow-success      0 4px 14px rgba(16, 185, 129, 0.25)
--shadow-warning      0 4px 14px rgba(245, 158, 11, 0.25)
--shadow-error        0 4px 14px rgba(239, 68, 68, 0.25)
```

### 使用示例

```css
.alert-success {
  border-left: 4px solid var(--color-success);
  box-shadow: var(--shadow-success);
}

.alert-error {
  border-left: 4px solid var(--color-error);
  box-shadow: var(--shadow-error);
}
```

---

## 内阴影

```css
--shadow-inner        inset 0 2px 4px rgba(0, 0, 0, 0.1)
--shadow-inner-lg     inset 0 4px 8px rgba(0, 0, 0, 0.15)
```

### 使用示例

```css
.input-focus {
  box-shadow: inset 0 0 0 2px var(--color-primary);
}

.button:active {
  box-shadow: var(--shadow-inner);
}
```

---

## 无阴影模式

某些场景需要移除阴影：

```css
.flat {
  box-shadow: none;
}

.no-shadow {
  box-shadow: none !important;
}
```

---

## 响应式阴影

```css
/* 移动端减小阴影 */
@media (max-width: 768px) {
  .card {
    box-shadow: var(--shadow-sm);
  }

  .button:hover {
    box-shadow: var(--shadow-md);
  }
}
```

---

## 最佳实践

1. **使用阴影表达深度**，而不是颜色
2. **保持一致的阴影大小**，相同层级使用相同阴影
3. **深色主题使用较浅阴影**，避免过度对比
4. **避免过度阴影**，小元素使用小阴影
5. **悬停状态增大阴影**，增强交互反馈

---

## 性能考虑

```css
/* 使用 box-shadow 而不是 filter: drop-shadow */
.card {
  box-shadow: var(--shadow-md);  /* 推荐 */
}

/* 避免使用多重阴影 */
.card {
  box-shadow: var(--shadow-md), var(--shadow-sm);  /* 避免过度使用 */
}
```

---

## 相关文档

- [颜色系统](./colors.md)
- [圆角系统](./border-radius.md)
- [动画系统](./animations.md)
