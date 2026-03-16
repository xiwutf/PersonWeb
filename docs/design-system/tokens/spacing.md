# 间距系统 (Spacing)

Aurora Design System 的间距规范，基于 4px 基准网格。

## 间距标尺

间距使用 4px 作为基准单位，乘数从 0.5 到 16。

| 变量 | 值 | 说明 |
|------|-----|------|
| `--spacing-0` | 0px | 无间距 |
| `--spacing-0.5` | 2px | 极小 |
| `--spacing-1` | 4px | 超小 |
| `--spacing-2` | 8px | 特小 |
| `--spacing-3` | 12px | 小 |
| `--spacing-4` | 16px | 基础（默认） |
| `--spacing-5` | 20px | 中小 |
| `--spacing-6` | 24px | 中等 |
| `--spacing-8` | 32px | 中大 |
| `--spacing-10` | 40px | 大 |
| `--spacing-12` | 48px | 特大 |
| `--spacing-16` | 64px | 超大 |
| `--spacing-20` | 80px | 巨大 |
| `--spacing-24` | 96px | 极大 |

---

## 语义化间距

### 区块内边距

```css
--spacing-sm          var(--spacing-4)     /* 16px - 小间距 */
--spacing-md          var(--spacing-6)     /* 24px - 中间距 */
--spacing-lg          var(--spacing-8)     /* 32px - 大间距 */
--spacing-xl          var(--spacing-12)    /* 48px - 超大间距 */
--spacing-2xl         var(--spacing-20)    /* 80px - 巨大间距 */
--spacing-3xl         var(--spacing-24)    /* 96px - 极大间距 */
```

### 组件内边距

```css
--padding-sm          var(--spacing-2)     /* 8px - 小组件内边距 */
--padding-md          var(--spacing-3)     /* 12px - 组件内边距 */
--padding-lg          var(--spacing-4)     /* 16px - 大组件内边距 */
--padding-xl          var(--spacing-6)     /* 24px - 超大内边距 */
```

---

## 间距工具类

### Padding 工具类

```css
.p-0         { padding: var(--spacing-0); }
.p-0.5       { padding: var(--spacing-0.5); }
.p-1          { padding: var(--spacing-1); }
.p-2          { padding: var(--spacing-2); }
.p-3          { padding: var(--spacing-3); }
.p-4          { padding: var(--spacing-4); }
.p-6          { padding: var(--spacing-6); }
.p-8          { padding: var(--spacing-8); }
.p-10         { padding: var(--spacing-10); }
.p-12         { padding: var(--spacing-12); }
```

### 方向性 Padding

```css
.px-4         { padding-left: var(--spacing-4); padding-right: var(--spacing-4); }
.py-4         { padding-top: var(--spacing-4); padding-bottom: var(--spacing-4); }
.pt-4         { padding-top: var(--spacing-4); }
.pb-4         { padding-bottom: var(--spacing-4); }
.pl-4         { padding-left: var(--spacing-4); }
.pr-4         { padding-right: var(--spacing-4); }
```

### Margin 工具类

```css
.m-0          { margin: var(--spacing-0); }
.m-1          { margin: var(--spacing-1); }
.m-2          { margin: var(--spacing-2); }
.m-3          { margin: var(--spacing-3); }
.m-4          { margin: var(--spacing-4); }
.m-6          { margin: var(--spacing-6); }
.m-8          { margin: var(--spacing-8); }
.m-10         { margin: var(--spacing-10); }
```

### 方向性 Margin

```css
.mx-4         { margin-left: var(--spacing-4); margin-right: var(--spacing-4); }
.my-4         { margin-top: var(--spacing-4); margin-bottom: var(--spacing-4); }
.mt-4         { margin-top: var(--spacing-4); }
.mb-4         { margin-bottom: var(--spacing-4); }
.ml-4         { margin-left: var(--spacing-4); }
.mr-4         { margin-right: var(--spacing-4); }
.m-auto       { margin: auto; }
```

---

## 组件间距示例

### 按钮间距

```css
.button-sm {
  padding: var(--padding-sm) var(--spacing-4);
}

.button-md {
  padding: var(--padding-md) var(--spacing-6);
}

.button-lg {
  padding: var(--padding-lg) var(--spacing-8);
}
```

### 卡片间距

```css
.card {
  padding: var(--spacing-lg);
  gap: var(--spacing-md);
}

.card-header {
  margin-bottom: var(--spacing-md);
}

.card-footer {
  margin-top: var(--spacing-lg);
}
```

### 列表间距

```css
.list-item {
  padding: var(--spacing-md) var(--spacing-lg);
}

.list-item + .list-item {
  margin-top: var(--spacing-2);
}
```

---

## 响应式间距

```css
/* 移动端减小间距 */
@media (max-width: 768px) {
  .card {
    padding: var(--spacing-md);
  }

  .section {
    padding: var(--spacing-lg) var(--spacing-md);
  }
}
```

---

## 间距模式

### 组件内间距

```css
.component {
  padding: var(--spacing-lg);     /* 内边距 */
  gap: var(--spacing-md);         /* 子元素间距 */
}
```

### 区块间距

```css
.section {
  padding: var(--spacing-3xl) 0;  /* 上下大间距 */
}

.section + .section {
  margin-top: var(--spacing-2xl);    /* 区块间间距 */
}
```

### 表单间距

```css
.form-group {
  margin-bottom: var(--spacing-lg);
}

.form-group:last-child {
  margin-bottom: 0;
}
```

---

## 最佳实践

1. **使用间距标尺**，保持倍数关系
2. **使用语义化变量**，提高可维护性
3. **保持一致性**，相同元素使用相同间距
4. **响应式调整**，移动端适当减小间距
5. **控制间距层次**，通过间距表达关系

---

## 相关文档

- [颜色系统](./colors.md)
- [排版系统](./typography.md)
- [圆角系统](./border-radius.md)
