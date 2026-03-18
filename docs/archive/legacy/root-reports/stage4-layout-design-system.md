# 页面布局规范与通用视觉骨架

## 概述

本文档定义了 PersonWeb 项目的页面布局规范和通用视觉骨架系统，确保整个应用的布局一致性和可维护性。

## 执行时间

- 开始时间：2026-03-14
- 执行阶段：阶段4 - 建立页面布局规范与通用视觉骨架

## 设计原则

### 1. 一致性原则

- 统一的间距系统：使用语义化间距变量
- 统一的圆角系统：使用语义化圆角变量
- 统一的阴影系统：使用语义化阴影变量
- 统一的容器规范：使用标准化的容器类

### 2. 响应式设计原则

- 移动优先：先考虑移动端布局，再适配桌面端
- 断点规范：使用统一的断点系统
- 流式布局：优先使用 Flexbox 和 Grid
- 弹性容器：使用可伸缩的容器大小

### 3. 视觉层次原则

- 清晰的信息层级：通过间距、大小、颜色区分
- 重要的内容突出：使用阴影、边框、背景色
- 一致的分组：相关内容使用相同的容器样式

## 间距系统

### 间距变量定义

基于 `assets/styles/theme-variables.css` 中的间距变量：

| 变量名 | 值 | 推荐用途 |
|---------|---|---------|
| `--spacing-0` | 0px | 重置间距 |
| `--spacing-1` | 4px | 极小间距（图标与文字） |
| `--spacing-2` | 8px | 小间距（卡片内边距） |
| `--spacing-3` | 12px | 中间距（标准间距） |
| `--spacing-4` | 16px | 大间距（区域分隔） |
| `--spacing-5` | 20px | 超大间距（区块间距） |
| `--spacing-6` | 24px | 巨大间距（段落间距） |
| `--spacing-8` | 32px | 特大间距（区块内部） |
| `--spacing-10` | 40px | 容器内边距 |
| `--spacing-12` | 48px | 卡片组间距 |
| `--spacing-16` | 64px | 区块间距 |
| `--spacing-20` | 80px | 超大容器间距 |

### 间距使用规范

#### 元素间距

```css
/* ✅ 推荐 */
.button {
  padding: var(--spacing-3) var(--spacing-4);
}

.card {
  padding: var(--spacing-6);
}

.card-header {
  padding: var(--spacing-4);
}

.card-body {
  padding: var(--spacing-6);
}

.card-footer {
  padding: var(--spacing-4);
}

/* ❌ 避免 */
.button {
  padding: 12px 24px;
}
```

#### 容器间距

```css
/* ✅ 推荐 */
.container {
  padding: var(--spacing-6);
  padding-top: var(--spacing-8);
  padding-bottom: var(--spacing-8);
}

.section {
  padding: var(--spacing-8) 0;
}

/* ❌ 避免 */
.container {
  padding: 32px 40px;
}
```

#### 列表项间距

```css
/* ✅ 推荐 */
.list-item {
  padding: var(--spacing-3) var(--spacing-4);
}

.list-item-condensed {
  padding: var(--spacing-2) var(--spacing-3);
}

/* ❌ 避免 */
.list-item {
  padding: 12px 16px;
}
```

## 圆角系统

### 圆角变量定义

| 变量名 | 值 | 推荐用途 |
|---------|---|---------|
| `--radius-none` | 0px | 无圆角（方角） |
| `--radius-sm` | 4px | 极小圆角（标签、徽章） |
| `--radius-base` | 6px | 小圆角（输入框、小按钮） |
| `--radius-md` | 8px | 中圆角（卡片、按钮） |
| `--radius-lg` | 12px | 大圆角（大卡片） |
| `--radius-xl` | 16px | 超大圆角（弹窗） |
| `--radius-2xl` | 24px | 特大圆角 |
| `--radius-3xl` | 32px | 特大圆角 |
| `--radius-full` | 9999px | 完全圆角（头像、圆形按钮） |

### 圆角使用规范

#### 按钮圆角

```css
/* ✅ 推荐 */
.button-primary {
  border-radius: var(--radius-md);
}

.button-sm {
  border-radius: var(--radius-sm);
}

.button-lg {
  border-radius: var(--radius-lg);
}

/* ❌ 避免 */
.button {
  border-radius: 8px;
}
```

#### 卡片圆角

```css
/* ✅ 推荐 */
.card {
  border-radius: var(--radius-lg);
}

.card-sm {
  border-radius: var(--radius-md);
}

.modal {
  border-radius: var(--radius-xl);
}

/* ❌ 避免 */
.card {
  border-radius: 16px;
}
```

#### 输入框圆角

```css
/* ✅ 推荐 */
.input {
  border-radius: var(--radius-base);
}

.textarea {
  border-radius: var(--radius-md);
}

/* ❌ 避免 */
.input {
  border-radius: 4px;
}
```

## 容器系统

### 容器分类

#### 1. 页面容器（Page Container）

```css
.page-container {
  width: 100%;
  max-width: var(--container-xl);  /* 1280px */
  margin: 0 auto;
  padding: 0 var(--spacing-6);
}

.page-container-sm {
  max-width: var(--container-lg);   /* 1024px */
}

.page-container-lg {
  max-width: var(--container-2xl);  /* 1536px */
}
```

#### 2. 区域容器（Section Container）

```css
.section {
  padding: var(--spacing-8) 0;
  margin-bottom: var(--spacing-8);
}

.section-sm {
  padding: var(--spacing-6) 0;
  margin-bottom: var(--spacing-6);
}

.section-lg {
  padding: var(--spacing-12) 0;
  margin-bottom: var(--spacing-12);
}
```

#### 3. 卡片容器（Card Container）

```css
.card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-6);
}

.card-sm {
  padding: var(--spacing-4);
}

.card-lg {
  padding: var(--spacing-8);
}

.card-elevated {
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-md);
}
```

#### 4. 网格容器（Grid Container）

```css
.grid-2 {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--spacing-4);
}

.grid-3 {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--spacing-4);
}

.grid-4 {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: var(--spacing-4);
}

.grid-6 {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-2);
}
```

## 响应式断点系统

### 断点定义

| 断点名 | 值 | 用途 |
|--------|---|------|
| `--breakpoint-sm` | 640px | 小型手机 |
| `--breakpoint-md` | 768px | 中型手机 |
| `--breakpoint-lg` | 1024px | 平板 |
| `--breakpoint-xl` | 1280px | 桌面 |
| `--breakpoint-2xl` | 1536px | 大屏 |

### 断点使用示例

```css
/* ✅ 推荐 */
@media (max-width: 640px) {
  .container {
    padding: var(--spacing-4);
  }
}

@media (max-width: 768px) {
  .sidebar {
    position: fixed;
  }
}

@media (min-width: 1024px) {
  .grid-4 {
    grid-template-columns: repeat(4, 1fr);
  }
}

/* ❌ 避免 */
@media (max-width: 768px) {
  .sidebar {
    display: none;
  }
}
```

## 布局模式

### 1. 标准页面布局（Standard Page Layout）

```
┌─────────────────────────────────────────┐
│  Header（固定高度）                        │
├─────────────────────────────────────────┤
│  Main Content（自适应高度）               │
│                                         │
│  - Page Header                           │
│  - Section 1                            │
│  - Section 2                            │
│  - Section 3                            │
│                                         │
└─────────────────────────────────────────┘
```

#### HTML 结构示例

```html
<div class="page-layout">
  <header class="page-header">
    <!-- 页面头部内容 -->
  </header>

  <main class="page-main">
    <section class="page-section">
      <h1 class="page-title">页面标题</h1>
      <!-- 区域内容 -->
    </section>
  </main>

  <footer class="page-footer">
    <!-- 页面底部内容 -->
  </footer>
</div>
```

#### CSS 样式示例

```css
.page-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.page-header {
  position: sticky;
  top: 0;
  z-index: var(--z-sticky);
  height: 64px;
  border-bottom: 1px solid var(--color-border-subtle);
  background: var(--color-bg-card);
}

.page-main {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.page-section {
  padding: var(--spacing-8) 0;
  margin-bottom: var(--spacing-8);
}

.page-title {
  font-size: var(--font-size-h1);
  font-weight: var(--font-bold);
  color: var(--color-text-main);
  margin-bottom: var(--spacing-4);
}
```

### 2. 侧边栏布局（Sidebar Layout）

```
┌──────────────┬──────────────────┬──────┐
│              │  Header         │      │
├──────────────┼──────────────────┼──────┤
│              │                │      │
│              │                │      │
│   Sidebar     │  Main Content   │      │
│   (260px)     │  (自适应)      │      │
│              │                │      │
│              │                │      │
│              │  Footer         │      │
│              │                │      │
└──────────────┴──────────────────┴──────┘
```

#### HTML 结构示例

```html
<div class="sidebar-layout">
  <aside class="sidebar">
    <!-- 侧边栏内容 -->
  </aside>

  <main class="sidebar-main">
    <header class="sidebar-header">
      <!-- 内容头部 -->
    </header>
    <div class="sidebar-content">
      <!-- 主要内容 -->
    </div>
  </main>
</div>
```

#### CSS 样式示例

```css
.sidebar-layout {
  display: flex;
  min-height: 100vh;
}

.sidebar {
  width: 260px;
  flex-shrink: 0;
  background: var(--color-bg-card);
  border-right: 1px solid var(--color-border-subtle);
}

.sidebar-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.sidebar-content {
  padding: var(--spacing-6);
}

@media (max-width: 768px) {
  .sidebar {
    position: fixed;
    left: -260px;
    z-index: var(--z-modal);
  }

  .sidebar-main {
    margin-left: 0;
  }
}
```

### 3. 仪表盘布局（Dashboard Layout）

```
┌─────────────────────────────────────────┐
│  Header（统计概览）                │
├─────────────────────────────────────────┤
│  ┌────┬──────┬──────┬──────┐  │
│  │ 卡1  │ 卡2  │ 卡3  │ 卡4  │
│  ├────┼──────┼──────┼──────┤  │
│  │     │     │     │     │
│  │     │     │     │     │
│  └────┴──────┴──────┴──────┘  │
│                                         │
├─────────────────────────────────────────┤
│  ┌──────────────────┐            │
│  │  内容区域          │            │
│  └──────────────────┘            │
└─────────────────────────────────────────┘
```

#### HTML 结构示例

```html
<div class="dashboard-layout">
  <header class="dashboard-header">
    <div class="dashboard-stats">
      <div class="stat-card">统计1</div>
      <div class="stat-card">统计2</div>
      <div class="stat-card">统计3</div>
      <div class="stat-card">统计4</div>
    </div>
  </header>

  <main class="dashboard-main">
    <section class="dashboard-section">
      <h2 class="section-title">数据表格</h2>
      <!-- 表格内容 -->
    </section>
  </main>
</div>
```

#### CSS 样式示例

```css
.dashboard-layout {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-6);
}

.dashboard-header {
  width: 100%;
}

.dashboard-stats {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--spacing-4);
}

.stat-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-4);
  text-align: center;
}

.dashboard-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-6);
}

@media (max-width: 768px) {
  .dashboard-stats {
    grid-template-columns: repeat(2, 1fr);
  }
}
```

### 4. 表单布局（Form Layout）

```
┌─────────────────────────────────────────┐
│         表单标题                       │
├─────────────────────────────────────────┤
│         ┌──────────────────────┐      │
│         │  表单字段组         │      │
│         ├────────┬─────────────┤      │
│         │ 标签  │ 输入框        │      │
│         ├────────┼─────────────┤      │
│         │ 标签  │ 输入框        │      │
│         ├────────┼─────────────┤      │
│         │ 标签  │ 输入框        │      │
│         └────────┴──────────────┘      │
│                                         │
├─────────────────────────────────────────┤
│         表单操作按钮                   │
└─────────────────────────────────────────┘
```

#### HTML 结构示例

```html
<div class="form-layout">
  <h1 class="form-title">表单标题</h1>

  <form class="form">
    <div class="form-group">
      <label class="form-label">字段名称</label>
      <input type="text" class="form-input" />
    </div>

    <div class="form-group">
      <label class="form-label">字段名称</label>
      <input type="text" class="form-input" />
    </div>

    <div class="form-actions">
      <button type="submit" class="btn btn-primary">保存</button>
      <button type="button" class="btn btn-secondary">取消</button>
    </div>
  </form>
</div>
```

#### CSS 样式示例

```css
.form-layout {
  max-width: 600px;
  margin: 0 auto;
  padding: var(--spacing-8);
}

.form-title {
  font-size: var(--font-size-h2);
  font-weight: var(--font-bold);
  color: var(--color-text-main);
  margin-bottom: var(--spacing-6);
}

.form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-4);
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-2);
}

.form-label {
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  color: var(--color-text-main);
  margin-bottom: var(--spacing-1);
}

.form-input {
  padding: var(--spacing-3) var(--spacing-2);
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-base);
  font-size: var(--font-size-base);
  background: var(--color-bg-input);
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px var(--color-primary-soft);
}

.form-actions {
  display: flex;
  gap: var(--spacing-3);
  justify-content: flex-end;
  margin-top: var(--spacing-4);
}

.btn {
  padding: var(--spacing-3) var(--spacing-4);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  font-weight: var(--font-medium);
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-primary {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
  border: none;
}

.btn-primary:hover {
  background: var(--color-primary-hover);
}

.btn-secondary {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.btn-secondary:hover {
  background: var(--color-bg-card);
  border-color: var(--color-border-strong);
}
```

## 视觉骨架（Skeleton）

### 加载状态骨架

```html
<div class="skeleton">
  <div class="skeleton-header">
    <div class="skeleton-title"></div>
    <div class="skeleton-subtitle"></div>
  </div>
  <div class="skeleton-body">
    <div class="skeleton-line"></div>
    <div class="skeleton-line"></div>
    <div class="skeleton-line short"></div>
  </div>
</div>
```

```css
.skeleton {
  padding: var(--spacing-6);
}

.skeleton-header {
  display: flex;
  gap: var(--spacing-4);
  margin-bottom: var(--spacing-6);
}

.skeleton-title,
.skeleton-subtitle {
  height: 24px;
  border-radius: var(--radius-base);
  background: var(--color-bg-elevated);
}

.skeleton-subtitle {
  width: 60%;
}

.skeleton-body {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-4);
}

.skeleton-line {
  height: 16px;
  border-radius: var(--radius-base);
  background: var(--color-bg-elevated);
}

.skeleton-line.short {
  width: 60%;
}

/* 加载动画 */
@keyframes skeleton-loading {
  0% {
    background-position: -200% 0;
  }
  100% {
    background-position: 200% 0;
  }
}

.skeleton-title,
.skeleton-subtitle,
.skeleton-line {
  background: linear-gradient(
    90deg,
    var(--color-bg-elevated) 25%,
    var(--color-bg-elevated) 50%,
    var(--color-bg-elevated) 75%
  );
  background-size: 200% 100%;
  animation: skeleton-loading 1.5s infinite;
}
```

### 卡片骨架

```html
<div class="card-skeleton">
  <div class="card-skeleton-image"></div>
  <div class="card-skeleton-content">
    <div class="card-skeleton-title"></div>
    <div class="card-skeleton-line"></div>
    <div class="card-skeleton-line"></div>
  <div class="card-skeleton-line short"></div>
  </div>
</div>
```

```css
.card-skeleton {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-6);
  display: flex;
  gap: var(--spacing-4);
}

.card-skeleton-image {
  width: 100%;
  height: 180px;
  border-radius: var(--radius-base);
  background: var(--color-bg-elevated);
}

.card-skeleton-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-4);
  flex: 1;
}

.card-skeleton-title {
  height: 24px;
  width: 60%;
  border-radius: var(--radius-base);
  background: var(--color-bg-elevated);
}

.card-skeleton-line {
  height: 14px;
  border-radius: var(--radius-base);
  background: var(--color-bg-elevated);
}

.card-skeleton-line.short {
  width: 80%;
}
```

## 最佳实践

### 1. 使用语义变量

```css
/* ✅ 推荐 */
.card {
  padding: var(--spacing-6);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
}

/* ❌ 避免 */
.card {
  padding: 24px;
  border-radius: 16px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}
```

### 2. 响应式设计

```css
/* ✅ 推荐 - 使用 Grid 和断点 */
.grid-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--spacing-4);
}

@media (max-width: 768px) {
  .grid-container {
    grid-template-columns: 1fr;
  }
}

/* ❌ 避免 - 使用固定宽度 */
.grid-container {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--spacing-4);
}
```

### 3. 语义化 HTML 结构

```html
<!-- ✅ 推荐 -->
<article class="card">
  <header class="card-header">
    <h2 class="card-title">标题</h2>
  </header>
  <div class="card-body">
    <p>内容</p>
  </div>
</article>

<!-- ❌ 避免 -->
<div class="card">
  <div class="card-header">
    <span class="title">标题</span>
  </div>
  <div class="card-body">
    <p>内容</p>
  </div>
</div>
```

### 4. 状态管理

```vue
<!-- ✅ 推荐 -->
<template>
  <div v-if="loading" class="skeleton">
    <!-- 骨架内容 -->
  </div>
  <div v-else class="content">
    <!-- 实际内容 -->
  </div>
</template>

<!-- ❌ 避免 -->
<template>
  <div v-if="loading">
    <span>加载中...</span>
  </div>
  <div v-else>
    <!-- 实际内容 -->
  </div>
</template>
```

## 组件设计规范

### 按钮组件

#### 尺寸规范

```css
.btn-sm {
  padding: var(--spacing-2) var(--spacing-3);
  font-size: var(--text-sm);
  border-radius: var(--radius-sm);
}

.btn-md {
  padding: var(--spacing-3) var(--spacing-4);
  font-size: var(--font-size-base);
  border-radius: var(--radius-md);
}

.btn-lg {
  padding: var(--spacing-4) var(--spacing-5);
  font-size: var(--text-lg);
  border-radius: var(--radius-lg);
}
```

#### 变体规范

```css
.btn-primary {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
  border: none;
}

.btn-primary:hover {
  background: var(--color-primary-hover);
  transform: translateY(-2px);
  box-shadow: var(--shadow-primary);
}

.btn-secondary {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.btn-secondary:hover {
  background: var(--color-bg-card);
  border-color: var(--color-border-strong);
}

.btn-danger {
  background: var(--color-error);
  color: var(--color-text-on-primary);
}

.btn-danger:hover {
  background: var(--color-error-hover);
}

.btn-success {
  background: var(--color-success);
  color: var(--color-text-on-primary);
}
```

### 输入框组件

```css
.input {
  width: 100%;
  padding: var(--spacing-3) var(--spacing-2);
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-base);
  font-size: var(--font-size-base);
  background: var(--color-bg-input);
  transition: all 0.2s ease;
}

.input:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px var(--color-primary-soft);
}

.input-error {
  border-color: var(--color-error);
}

.input-error:focus {
  border-color: var(--color-error);
  box-shadow: 0 0 0 2px var(--color-error-soft);
}
```

### 卡片组件

```css
.card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-6);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
}

.card-header {
  padding-bottom: var(--spacing-4);
  border-bottom: 1px solid var(--color-border-subtle);
  margin-bottom: var(--spacing-4);
}

.card-title {
  font-size: var(--font-size-h3);
  font-weight: var(--font-bold);
  color: var(--color-text-main);
  margin-bottom: var(--spacing-2);
}

.card-body {
  padding-bottom: var(--spacing-4);
}

.card-footer {
  padding-top: var(--spacing-4);
  border-top: 1px solid var(--color-border-subtle);
  margin-top: var(--spacing-4);
}
```

## 工具类组件

### 徽章和标签

```css
.tag {
  display: inline-flex;
  align-items: center;
  padding: var(--spacing-1) var(--spacing-2);
  border-radius: var(--radius-full);
  font-size: var(--text-xs);
  font-weight: var(--font-medium);
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-subtle);
}

.tag-primary {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
  border-color: var(--color-primary);
}

.tag-success {
  background: var(--color-success);
  color: var(--color-text-on-primary);
}

.tag-warning {
  background: var(--color-warning);
  color: var(--color-text-on-primary);
}

.tag-error {
  background: var(--color-error);
  color: var(--color-text-on-primary);
}

.badge {
  width: 20px;
  height: 20px;
  border-radius: var(--radius-full);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: var(--text-xs);
  font-weight: var(--font-bold);
  background: var(--color-primary);
  color: var(--color-text-on-primary);
}

.badge-success {
  background: var(--color-success);
}

.badge-warning {
  background: var(--color-warning);
}

.badge-error {
  background: var(--color-error);
}
```

### 面包屑导航

```css
.breadcrumb {
  display: flex;
  align-items: center;
  gap: var(--spacing-1);
  padding: var(--spacing-2) 0;
  font-size: var(--text-sm);
  color: var(--color-text-muted);
}

.breadcrumb-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-1);
}

.breadcrumb-link {
  color: var(--color-text-main);
  transition: color 0.2s ease;
}

.breadcrumb-link:hover {
  color: var(--color-primary);
}

.breadcrumb-separator {
  color: var(--color-border-default);
}

.breadcrumb-current {
  color: var(--color-text-main);
  font-weight: var(--font-medium);
}
```

## 实施建议

### 1. 优先迁移高使用值

以下高频使用的硬编码值应优先迁移到语义变量：

1. **间距**：`8px`, `12px`, `16px`, `20px`, `24px`, `32px`
2. **圆角**：`4px`, `8px`, `12px`, `16px`
3. **字体**：`12px`, `14px`, `16px`, `20px`

### 2. 创建布局组件库

建议创建以下可复用的布局组件：

1. **页面容器**：`PageContainer` - 标准页面容器
2. **内容容器**：`ContentContainer` - 内容区容器
3. **卡片容器**：`CardContainer` - 卡片容器
4. **表单容器**：`FormContainer` - 表单容器
5. **网格容器**：`GridContainer` - 网格容器
6. **骨架屏**：`SkeletonLoader` - 骨架加载

### 3. 更新现有布局文件

优先更新以下布局文件：

1. `pages/ai/index.vue` - AI 首页布局
2. `pages/admin/document-agent.vue` - 文档管理页面
3. `pages/admin/modules/index.vue` - 模块管理页面
4. `components/ai/AiCapabilitySection.vue` - AI 功能区域
5. `pages/modules/[moduleKey]/index.vue` - 模块详情页

### 4. 建立设计系统审查流程

建议建立以下流程：

1. **新布局审查**：所有新增页面必须符合本规范
2. **代码审查**：PR 中检查布局规范符合性
3. **自动化检查**：使用 Stylelint 规则自动检测
4. **定期审计**：每季度审计一次代码规范

## 迁移检查清单

在实施布局规范迁移时，检查以下项目：

- [ ] 是否使用了语义间距变量？
- [ ] 是否使用了语义圆角变量？
- [ ] 是否使用了语义阴影变量？
- [ ] 是否遵循了响应式断点规范？
- [ ] 是否使用了语义化容器类？
- [ ] 是否有适当的视觉层次？
- [ ] 是否使用了骨架屏组件？
- [ ] 是否遵循了组件设计规范？

## 相关文档

- `assets/styles/theme-variables.css` - 主题变量系统
- `docs/semantic-variable-mapping.md` - 语义变量映射表
- `docs/semantic-variable-migration-report.md` - 语义变量迁移报告
- `docs/stage3-non-color-hardcoding-report.md` - 非颜色硬编码治理报告

## 验证命令

### 检查布局规范使用

```bash
# 检查语义间距变量使用
grep -r "var(--spacing-\d)" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/

# 检查语义圆角变量使用
grep -r "var(--radius-\w)" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/

# 检查语义阴影变量使用
grep -r "var(--shadow-\w)" \
  --include="*.vue" \
  --include="*.css" \
  pages/ assets/ components/
```

### 统计硬编码减少

```bash
# 统计间距硬编码减少
echo "迁移前：$(grep -r "padding:\s*\d+px" pages/ assets/ components/ | wc -l)"
echo "迁移后：$(grep -r "var(--spacing-\d)" pages/ assets/ components/ | wc -l)"
```

---

**文档生成时间**: 2026-03-14
**报告版本**: 1.0
**生成工具**: Claude Code
