# 语义变量映射表

本文档提供了基础色板变量到语义变量的映射关系，用于指导代码迁移工作。

## 目录

1. [状态颜色映射](#状态颜色映射)
2. [文字颜色映射](#文字颜色映射)
3. [背景颜色映射](#背景颜色映射)
4. [边框颜色映射](#边框颜色映射)
5. [投资专用语义变量](#投资专用语义变量)
6. [迁移规则](#迁移规则)
7. [迁移示例](#迁移示例)

---

## 状态颜色映射

### 错误状态（Error）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-error-bg` | `--color-red-50` | 错误提示背景色 |
| `--color-error-border` | `--color-red-200` | 错误提示边框色 |
| `--color-error-text` | `--color-red-700` | 错误提示文字色 |
| `--color-error-icon` | `--color-red-500` | 错误图标颜色 |
| `--color-error-soft` | `rgba(236, 72, 153, 0.15)` | 错误状态半透明背景 |
| `--color-error-10` | `rgba(236, 72, 153, 0.1)` | 错误状态 10% 透明度 |
| `--color-error-20` | `rgba(236, 72, 153, 0.2)` | 错误状态 20% 透明度 |
| `--color-error-30` | `rgba(236, 72, 153, 0.3)` | 错误状态 30% 透明度 |

### 成功状态（Success）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-success-bg` | `--color-green-50` | 成功提示背景色 |
| `--color-success-border` | `--color-green-200` | 成功提示边框色 |
| `--color-success-text` | `--color-green-700` | 成功提示文字色 |
| `--color-success-icon` | `--color-green-500` | 成功图标颜色 |
| `--color-success-soft` | `rgba(16, 185, 129, 0.15)` | 成功状态半透明背景 |
| `--color-success-10` | `rgba(16, 185, 129, 0.1)` | 成功状态 10% 透明度 |
| `--color-success-20` | `rgba(16, 185, 129, 0.2)` | 成功状态 20% 透明度 |
| `--color-success-30` | `rgba(16, 185, 129, 0.3)` | 成功状态 30% 透明度 |

### 警告状态（Warning）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-warning-bg` | `--color-orange-50` | 警告提示背景色 |
| `--color-warning-border` | `--color-orange-200` | 警告提示边框色 |
| `--color-warning-text` | `--color-orange-700` | 警告提示文字色 |
| `--color-warning-icon` | `--color-orange-500` | 警告图标颜色 |
| `--color-warning-soft` | `rgba(249, 115, 22, 0.15)` | 警告状态半透明背景 |
| `--color-warning-10` | `rgba(249, 115, 22, 0.1)` | 警告状态 10% 透明度 |
| `--color-warning-20` | `rgba(249, 115, 22, 0.2)` | 警告状态 20% 透明度 |
| `--color-warning-30` | `rgba(249, 115, 22, 0.3)` | 警告状态 30% 透明度 |

### 信息状态（Info）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-info-bg` | `--color-blue-50` | 信息提示背景色 |
| `--color-info-border` | `--color-blue-200` | 信息提示边框色 |
| `--color-info-text` | `--color-blue-700` | 信息提示文字色 |
| `--color-info-icon` | `--color-blue-500` | 信息图标颜色 |
| `--color-info-soft` | `rgba(6, 182, 212, 0.15)` | 信息状态半透明背景 |
| `--color-info-10` | `rgba(6, 182, 212, 0.1)` | 信息状态 10% 透明度 |
| `--color-info-20` | `rgba(6, 182, 212, 0.2)` | 信息状态 20% 透明度 |
| `--color-info-30` | `rgba(6, 182, 212, 0.3)` | 信息状态 30% 透明度 |

### 紫色状态（Purple）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-purple-soft` | `rgba(168, 85, 247, 0.15)` | 紫色状态半透明背景 |
| `--color-purple-10` | `rgba(168, 85, 247, 0.1)` | 紫色状态 10% 透明度 |
| `--color-purple-20` | `rgba(168, 85, 247, 0.2)` | 紫色状态 20% 透明度 |
| `--color-purple-30` | `rgba(168, 85, 247, 0.3)` | 紫色状态 30% 透明度 |

---

## 文字颜色映射

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-text-main` | `--color-gray-900` | 主要文字 |
| `--color-text-primary` | `--color-gray-900` | 主色文字（别名） |
| `--color-text-secondary` | `--color-gray-700` | 次要文字 |
| `--color-text-tertiary` | `--color-gray-500` | 三级文字 |
| `--color-text-quaternary` | `--color-gray-400` | 四级文字 |
| `--color-text-muted` | `--color-gray-600` | 辅助文字 |
| `--color-text-sub` | `--color-gray-700` | 次级文字 |
| `--color-text-disabled` | `--color-gray-400` | 禁用状态文字 |
| `--color-text-success` | `--color-green-600` | 成功状态文字 |
| `--color-text-warning` | `--color-orange-500` | 警告状态文字 |
| `--color-text-error` | `--color-red-600` | 错误状态文字 |
| `--color-text-info` | `--color-blue-500` | 信息状态文字 |
| `--color-link` | `--color-blue-600` | 链接颜色 |
| `--color-link-hover` | `--color-blue-700` | 链接悬停颜色 |

---

## 背景颜色映射

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-bg-body` | `--color-gray-50` | 页面背景色 |
| `--color-bg-page` | `--color-gray-50` | 页面背景色（别名） |
| `--color-bg-page-alt` | `--color-white` | 交替页面背景色 |
| `--color-bg-light` | `--color-gray-50` | 浅色背景 |
| `--color-bg-dark` | `--color-gray-900` | 深色背景 |
| `--color-bg-elevated` | `--color-gray-100` | 提升层背景色 |
| `--color-bg-overlay` | `rgba(0, 0, 0, 0.5)` | 遮罩层背景色 |
| `--color-bg-card` | `--color-white` | 卡片背景色 |
| `--color-bg-card-elevated` | `--color-gray-100` | 提升卡片背景色 |
| `--color-bg-card-muted` | `--color-gray-50` | 静音卡片背景色 |
| `--color-bg-surface` | `--color-white` | 表面背景色 |
| `--color-bg-surface-elevated` | `--color-gray-100` | 提升表面背景色 |
| `--color-bg-surface-muted` | `--color-gray-50` | 静音表面背景色 |
| `--color-bg-input` | `--color-white` | 输入框背景色 |
| `--color-bg-input-disabled` | `--color-gray-100` | 禁用输入框背景色 |
| `--color-bg-input-hover` | `--color-gray-50` | 输入框悬停背景色 |
| `--color-bg-disabled` | `--color-gray-200` | 禁用背景色 |

---

## 边框颜色映射

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-border-subtle` | `--color-gray-200` | 细微边框 |
| `--color-border-default` | `--color-gray-300` | 默认边框 |
| `--color-border-strong` | `--color-gray-400` | 强调边框 |
| `--color-border-focus` | `--color-blue-500` | 聚焦边框 |
| `--color-border-muted` | `--color-gray-200` | 静音边框 |
| `--color-border-hover` | `--color-gray-300` | 悬停边框 |
| `--color-border-active` | `--color-blue-500` | 激活边框 |

---

## 投资专用语义变量

### 盈利状态（Profit）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-profit` | `--color-emerald-500` | 盈利颜色 |
| `--color-profit-hover` | `--color-emerald-600` | 盈利悬停颜色 |
| `--color-profit-bg` | `--color-emerald-50` | 盈利背景色 |
| `--color-profit-text` | `--color-emerald-700` | 盈利文字色 |
| `--color-profit-soft` | `rgba(16, 185, 129, 0.15)` | 盈利半透明背景 |

### 亏损状态（Loss）

| 语义变量 | 基础色板变量 | 用途 |
|---------|-------------|------|
| `--color-loss` | `--color-red-500` | 亏损颜色 |
| `--color-loss-hover` | `--color-red-600` | 亏损悬停颜色 |
| `--color-loss-bg` | `--color-red-50` | 亏损背景色 |
| `--color-loss-text` | `--color-red-700` | 亏损文字色 |
| `--color-loss-soft` | `rgba(236, 72, 153, 0.15)` | 亏损半透明背景 |

---

## 迁移规则

### 规则 1：错误状态

将以下基础色板变量迁移到对应的语义变量：

```css
/* 迁移前 */
background-color: var(--color-red-50);
border: 1px solid var(--color-red-200);
color: var(--color-red-800);

/* 迁移后 */
background-color: var(--color-error-bg);
border: 1px solid var(--color-error-border);
color: var(--color-error-text);
```

### 规则 2：成功状态

将 emerald 颜色用于投资收益时迁移到 profit 语义变量：

```css
/* 迁移前 */
color: var(--color-emerald-500);
background: var(--color-emerald-500);

/* 迁移后 */
color: var(--color-profit);
background: var(--color-profit);
```

### 规则 3：亏损状态

将 red 颜色用于投资亏损时迁移到 loss 语义变量：

```css
/* 迁移前 */
color: var(--color-red-500);
background: var(--color-red-500);

/* 迁移后 */
color: var(--color-loss);
background: var(--color-loss);
```

### 规则 4：深色模式适配

对于需要适配深色模式的颜色使用，迁移时保持语义变量不变，只需在深色模式主题文件中重新定义语义变量值：

```css
/* 浅色模式（默认） */
.projects-error {
  background-color: var(--color-error-bg);
  border-color: var(--color-error-border);
  color: var(--color-error-text);
}

/* 深色模式 */
@media (prefers-color-scheme: dark) {
  :root {
    --color-error-bg: var(--color-red-900);
    --color-error-border: var(--color-red-800);
    --color-error-text: var(--color-red-300);
  }
}
```

### 规则 5：装饰性颜色

对于纯装饰性的渐变或特殊效果（如彩虹渐变、光晕效果），可以继续使用基础色板变量，因为这类使用不涉及状态或语义含义：

```css
/* 这种情况可以继续使用基础色板 */
background: linear-gradient(135deg, var(--color-orange-600), var(--color-red-600));
```

---

## 迁移示例

### 示例 1：错误提示框迁移

```css
/* 迁移前 */
.projects-error {
  background-color: var(--color-red-100);
  border: 1px solid var(--color-red-200);
  border-radius: 0.5rem;
  padding: 1rem;
  margin-bottom: 2rem;
  color: var(--color-red-800);
}

@media (prefers-color-scheme: dark) {
  .projects-error {
    background-color: var(--color-red-900);
    border-color: var(--color-red-800);
    color: var(--color-red-300);
  }
}

/* 迁移后 */
.projects-error {
  background-color: var(--color-error-bg);
  border: 1px solid var(--color-error-border);
  border-radius: 0.5rem;
  padding: 1rem;
  margin-bottom: 2rem;
  color: var(--color-error-text);
}

@media (prefers-color-scheme: dark) {
  :root {
    --color-error-bg: var(--color-red-900);
    --color-error-border: var(--color-red-800);
    --color-error-text: var(--color-red-300);
  }
}
```

### 示例 2：投资收益显示迁移

```css
/* 迁移前 */
.btn-transaction {
  color: var(--color-emerald-500);
  background: none;
  border: none;
  cursor: pointer;
  transition: color 0.2s;
}

.btn-transaction:hover {
  color: var(--color-emerald-600);
}

/* 迁移后 */
.btn-transaction {
  color: var(--color-profit);
  background: none;
  border: none;
  cursor: pointer;
  transition: color 0.2s;
}

.btn-transaction:hover {
  color: var(--color-profit-hover);
}
```

### 示例 3：删除按钮迁移

```css
/* 迁移前 */
.btn-delete {
  color: var(--color-red-500);
  background: none;
  border: none;
  cursor: pointer;
  transition: color 0.2s;
}

.btn-delete:hover {
  color: var(--color-red-600);
}

/* 迁移后 */
.btn-delete {
  color: var(--color-loss);
  background: none;
  border: none;
  cursor: pointer;
  transition: color 0.2s;
}

.btn-delete:hover {
  color: var(--color-loss-hover);
}
```

---

## 注意事项

1. **语义优先**：优先使用语义变量而不是基础色板变量
2. **主题切换**：使用语义变量后，只需在主题文件中重新定义语义变量值即可实现主题切换
3. **向后兼容**：旧的语义变量（如 `--color-info`）会保留以保持向后兼容
4. **装饰性颜色**：纯装饰性的渐变或特殊效果可以继续使用基础色板变量
5. **一致性**：相同用途的颜色必须使用相同的语义变量

---

## 版本历史

- **v1.0** (2026-03-14): 初始版本，建立语义变量映射表
