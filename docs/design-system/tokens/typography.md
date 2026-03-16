# 排版系统 (Typography)

Aurora Design System 的排版规范，定义字体、字号、行高等排版元素。

## 字体家族

### 主字体

```css
--font-sans    'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif
--font-serif   'Georgia', 'Times New Roman', serif
--font-mono    'SF Mono', 'Monaco', 'Consolas', 'Courier New', monospace
```

### 使用场景

- **Sans-serif**：正文、标题、按钮（默认字体）
- **Serif**：引用、特殊排版
- **Mono**：代码、数据展示

---

## 字号系统

### 标题字号

| 变量 | 值 | 使用 |
|------|-----|------|
| `--text-display` | 64px | 超大标题 |
| `--text-4xl` | 48px | 大标题 |
| `--text-3xl` | 36px | 页面标题 |
| `--text-2xl` | 24px | 区块标题 |
| `--text-xl` | 20px | 卡片标题 |

### 正文字号

| 变量 | 值 | 使用 |
|------|-----|------|
| `--text-lg` | 18px | 重要文字 |
| `--text-base` | 16px | 正文（默认） |
| `--text-md` | 14px | 次要文字 |
| `--text-sm` | 13px | 辅助文字 |
| `--text-xs` | 12px | 微小文字 |

### 使用示例

```css
h1 {
  font-size: var(--text-3xl);
  line-height: 1.2;
}

p {
  font-size: var(--text-base);
  line-height: 1.6;
}

caption {
  font-size: var(--text-sm);
}
```

---

## 行高系统

| 变量 | 值 | 使用 |
|------|-----|------|
| `--leading-tight` | 1.25 | 标题 |
| `--leading-normal` | 1.5 | 正文 |
| `--leading-relaxed` | 1.75 | 引用 |
| `--leading-loose` | 2 | 特殊排版 |

### 使用示例

```css
.title {
  font-size: var(--text-2xl);
  line-height: var(--leading-tight);
}

.body {
  font-size: var(--text-base);
  line-height: var(--leading-normal);
}
```

---

## 字重系统

| 变量 | 值 | 使用 |
|------|-----|------|
| `--font-light` | 300 | 轻字重 |
| `--font-normal` | 400 | 正常（默认） |
| `--font-medium` | 500 | 中等 |
| `--font-semibold` | 600 | 半粗 |
| `--font-bold` | 700 | 粗体 |

### 使用示例

```css
h1, h2, h3 {
  font-weight: var(--font-semibold);
}

.button {
  font-weight: var(--font-medium);
}
```

---

## 字母间距

| 变量 | 值 | 使用 |
|------|-----|------|
| `--tracking-tighter` | -0.05em | 紧凑 |
| `--tracking-tight` | -0.025em | 较紧凑 |
| `--tracking-normal` | 0em | 正常（默认） |
| `--tracking-wide` | 0.025em | 宽松 |
| `--tracking-wider` | 0.05em | 较宽松 |

### 使用场景

```css
.uppercase-text {
  text-transform: uppercase;
  letter-spacing: var(--tracking-wider);
}
```

---

## 文本颜色

### 主文本颜色

```css
--color-text-main        主要文本
--color-text-sec         次要文本
--color-text-muted        淡化文本
--color-text-disabled    禁用文本
```

### 使用示例

```css
h1, h2, h3 {
  color: var(--color-text-main);
}

p {
  color: var(--color-text-sec);
}

small {
  color: var(--color-text-muted);
}

button:disabled {
  color: var(--color-text-disabled);
}
```

---

## 文本工具类

### 标题类

```css
.text-display      { font-size: var(--text-display); }
.text-4xl          { font-size: var(--text-4xl); }
.text-3xl          { font-size: var(--text-3xl); }
.text-2xl          { font-size: var(--text-2xl); }
.text-xl           { font-size: var(--text-xl); }
```

### 正文类

```css
.text-lg           { font-size: var(--text-lg); }
.text-base         { font-size: var(--text-base); }
.text-md           { font-size: var(--text-md); }
.text-sm           { font-size: var(--text-sm); }
.text-xs           { font-size: var(--text-xs); }
```

### 字重类

```css
.font-light        { font-weight: var(--font-light); }
.font-normal       { font-weight: var(--font-normal); }
.font-medium       { font-weight: var(--font-medium); }
.font-semibold     { font-weight: var(--font-semibold); }
.font-bold         { font-weight: var(--font-bold); }
```

---

## 排版模式

### 标题层级

```html
<h1 class="text-3xl font-semibold">Page Title</h1>
<h2 class="text-2xl font-semibold">Section Title</h2>
<h3 class="text-xl font-semibold">Subsection Title</h3>
<h4 class="text-lg font-medium">Component Title</h4>
```

### 文本段落

```html
<p class="text-base leading-normal">
  正文内容，使用基础字号和标准行高。
</p>

<p class="text-sm leading-normal text-muted">
  辅助说明文本，使用较小字号和淡化颜色。
</p>
```

### 代码文本

```html
<code class="text-sm font-mono bg-code text-code">
  const message = "Hello World"
</code>

<pre class="text-sm font-mono bg-code p-4 rounded">
  const message = "Hello World"
  console.log(message)
</pre>
```

---

## 响应式排版

### 移动端字号

```css
@media (max-width: 768px) {
  h1 {
    font-size: var(--text-2xl); /* 降级 */
  }
  h2 {
    font-size: var(--text-xl);
  }
}
```

### 流式排版

```css
.text-fluid {
  font-size: clamp(16px, 2.5vw, 20px);
}
```

---

## 最佳实践

1. **使用 CSS 变量**定义字号和行高
2. **保持对比度**，标题与正文有明显区分
3. **控制行高**，正文 1.5-1.75，标题 1.1-1.3
4. **合理使用字重**，正文 400-500，标题 600-700
5. **移动端适配**，适当减小标题字号
6. **代码使用等宽字体**，确保可读性

---

## 相关文档

- [颜色系统](./colors.md)
- [间距系统](./spacing.md)
- [开发规范](../guides/coding-standards.md)
