# 圆角系统 (Border Radius)

Aurora Design System 的圆角规范，定义不同组件的圆角样式。

## 圆角标尺

| 变量 | 值 | 说明 |
|------|-----|------|
| `--radius-none` | 0px | 无圆角 |
| `--radius-xs` | 2px | 极小圆角 |
| `--radius-sm` | 4px | 小圆角 |
| `--radius-md` | 8px | 中等圆角 |
| `--radius-lg` | 12px | 大圆角 |
| `--radius-xl` | 16px | 超大圆角 |
| `--radius-2xl` | 24px | 巨大圆角 |
| `--radius-full` | 9999px | 完全圆角 |

---

## 组件圆角规范

### 按钮圆角

```css
--radius-button-sm      var(--radius-sm)    /* 小按钮 */
--radius-button-md      var(--radius-md)    /* 中等按钮 */
--radius-button-lg      var(--radius-lg)    /* 大按钮 */
```

### 卡片圆角

```css
--radius-card-sm        var(--radius-md)    /* 小卡片 */
--radius-card-md        var(--radius-lg)    /* 中等卡片 */
--radius-card-lg        var(--radius-xl)    /* 大卡片 */
```

### 输入框圆角

```css
--radius-input          var(--radius-md)    /* 输入框 */
--radius-select          var(--radius-md)    /* 选择器 */
--radius-textarea        var(--radius-lg)    /* 文本域 */
```

### 标签圆角

```css
--radius-tag            var(--radius-full)  /* 完全圆角 */
--radius-chip           var(--radius-full)  /* 芯片完全圆角 */
```

---

## 圆角工具类

```css
.rounded-none       { border-radius: var(--radius-none); }
.rounded-xs         { border-radius: var(--radius-xs); }
.rounded-sm         { border-radius: var(--radius-sm); }
.rounded-md         { border-radius: var(--radius-md); }
.rounded-lg         { border-radius: var(--radius-lg); }
.rounded-xl         { border-radius: var(--radius-xl); }
.rounded-2xl        { border-radius: var(--radius-2xl); }
.rounded-full       { border-radius: var(--radius-full); }
```

### 方向性圆角

```css
.rounded-t          { border-top-left-radius: var(--radius-md); border-top-right-radius: var(--radius-md); }
.rounded-b          { border-bottom-left-radius: var(--radius-md); border-bottom-right-radius: var(--radius-md); }
.rounded-l          { border-top-left-radius: var(--radius-md); border-bottom-left-radius: var(--radius-md); }
.rounded-r          { border-top-right-radius: var(--radius-md); border-bottom-right-radius: var(--radius-md); }
.rounded-tl         { border-top-left-radius: var(--radius-md); }
.rounded-tr         { border-top-right-radius: var(--radius-md); }
.rounded-bl         { border-bottom-left-radius: var(--radius-md); }
.rounded-br         { border-bottom-right-radius: var(--radius-md); }
```

---

## 圆角使用示例

### 按钮圆角

```css
.button {
  border-radius: var(--radius-button-md);
}

.button-sm {
  border-radius: var(--radius-button-sm);
}

.button-lg {
  border-radius: var(--radius-button-lg);
}
```

### 卡片圆角

```css
.card {
  border-radius: var(--radius-card-lg);
}

.card-sm {
  border-radius: var(--radius-card-sm);
}
```

### 头像圆角

```css
.avatar {
  border-radius: var(--radius-full);
  width: 40px;
  height: 40px;
}

.avatar-lg {
  width: 64px;
  height: 64px;
}
```

### 标签圆角

```css
.tag {
  border-radius: var(--radius-full);
  padding: var(--spacing-1) var(--spacing-3);
}
```

---

## 混合圆角

### 顶部圆角

```css
.modal {
  border-radius: var(--radius-xl);
}

.modal-header {
  border-top-left-radius: var(--radius-xl);
  border-top-right-radius: var(--radius-xl);
  border-bottom-left-radius: 0;
  border-bottom-right-radius: 0;
}
```

### 底部圆角

```css
.modal-footer {
  border-bottom-left-radius: var(--radius-xl);
  border-bottom-right-radius: var(--radius-xl);
}
```

---

## 圆角与边框

### 内边框圆角

```css
.box {
  border: 2px solid var(--color-border-default);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
}
```

### 外边框圆角

```css
.box-wrapper {
  border-radius: var(--radius-xl);
  overflow: hidden;
}

.box-content {
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-lg);
}
```

---

## 圆角最佳实践

1. **使用一致的圆角**，保持视觉统一
2. **小元素用小圆角**，大元素用大圆角
3. **卡片使用较大圆角**（12px+），按钮使用中等圆角（8px）
4. **头像/徽章使用完全圆角**
5. **输入框使用中等圆角**（8px）

---

## 响应式圆角

```css
/* 移动端减小圆角 */
@media (max-width: 768px) {
  .card {
    border-radius: var(--radius-md);
  }

  .button {
    border-radius: var(--radius-sm);
  }
}
```

---

## 相关文档

- [颜色系统](./colors.md)
- [间距系统](./spacing.md)
- [阴影系统](./shadows.md)
