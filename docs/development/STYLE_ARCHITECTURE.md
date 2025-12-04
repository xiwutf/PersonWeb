# 样式架构规范

**创建日期**：2024-12-03  
**基于**：样式架构重构完成报告

## 📋 概述

本文档详细说明项目的样式架构设计，包括分层结构、CSS 变量系统、Naive UI 主题集成等。

## 🏗️ 架构分层

### 整体架构图

```
┌─────────────────────────────────────────┐
│  后端主题配置 (/api/Config/theme)       │
│  ↓                                      │
│  useTheme().setTheme()                  │
│  ↓                                      │
│  document.documentElement.dataset.theme │
│  ↓                                      │
│  tokens.css (CSS 变量)                  │
│  ↓                                      │
│  ┌─────────────────┬─────────────────┐ │
│  │ 自定义组件样式   │  Naive UI 主题  │ │
│  │ (使用 CSS 变量) │ (themeOverrides)│ │
│  └─────────────────┴─────────────────┘ │
└─────────────────────────────────────────┘
```

### 分层说明

#### 第 1 层：设计 Token（`assets/styles/tokens.css`）

**职责**：定义全局 CSS 变量，统一管理所有设计变量

**文件位置**：`assets/styles/tokens.css`

**变量分类**：

1. **颜色变量**：
   - `--color-bg-*` - 背景色系列
   - `--color-text-*` - 文字颜色系列
   - `--color-primary-*` - 主色调系列
   - `--color-border-*` - 边框颜色系列

2. **圆角变量**：
   - `--radius-sm` - 小圆角（0.375rem）
   - `--radius-md` - 中等圆角（0.5rem）
   - `--radius-lg` - 大圆角（0.75rem）
   - `--radius-xl` - 超大圆角（1rem）

3. **阴影变量**：
   - `--shadow-sm` - 小阴影
   - `--shadow-md` - 中等阴影
   - `--shadow-lg` - 大阴影
   - `--shadow-xl` - 超大阴影
   - `--shadow-soft` - 柔和阴影

4. **字号变量**：
   - `--font-size-h1` - 一级标题（2rem）
   - `--font-size-h2` - 二级标题（1.5rem）
   - `--font-size-h3` - 三级标题（1.25rem）
   - `--font-size-body` - 正文（0.875rem）

5. **间距变量**：
   - `--spacing-xs` - 超小间距（0.25rem）
   - `--spacing-sm` - 小间距（0.5rem）
   - `--spacing-md` - 中等间距（1rem）
   - `--spacing-lg` - 大间距（1.5rem）

**主题切换机制**：

```css
/* 默认主题（:root） */
:root {
  --color-primary: #2563eb;
  --color-bg-body: #f8fafc;
  /* ... */
}

/* 深色主题 */
html[data-theme="dark"] {
  --color-primary: #60a5fa;
  --color-bg-body: #020617;
  /* ... */
}

/* 实验室主题 */
html[data-theme="lab"] {
  --color-primary: #38bdf8;
  --color-bg-body: #020617;
  /* ... */
}
```

**使用方式**：

```vue
<style scoped>
.my-component {
  background-color: var(--color-bg-card);
  color: var(--color-text-main);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  padding: var(--spacing-md);
}
</style>
```

#### 第 2 层：基础样式（`assets/styles/base.css`）

**职责**：基础样式重置和全局排版

**文件位置**：`assets/styles/base.css`

**内容**：
- `html, body` 的默认字体、背景色、文字颜色
- 链接、图片、按钮、输入框、列表等基础重置
- 标题、段落、代码等排版样式
- 滚动条和选择文本样式

**特点**：
- 不覆盖 Naive UI 内部样式
- 使用 CSS 变量确保与主题系统联动

#### 第 3 层：Naive UI 补丁（`assets/styles/ui-patch-naive.css`）

**职责**：Naive UI 组件的统一风格补丁

**文件位置**：`assets/styles/ui-patch-naive.css`

**补丁范围**：
- Card、Dialog、Button、Input、Select、Form、Table、Menu 等组件
- 统一圆角、阴影、间距等视觉风格
- 使用 CSS 变量确保与主题系统联动

**示例**：

```css
.n-card {
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-soft);
  background-color: var(--color-bg-card);
}

.n-button {
  border-radius: var(--radius-lg);
}

.n-input {
  border-radius: var(--radius-md);
}
```

#### 第 4 层：Naive UI 配置（`components/layout/AppNaiveConfig.vue`）

**职责**：统一管理 Naive UI 的主题配置

**文件位置**：`components/layout/AppNaiveConfig.vue`

**功能**：
1. 使用 `NConfigProvider` 包裹所有内容
2. 基于 `useTheme().currentTheme` 计算 Naive 主题
3. 在 `themeOverrides` 中使用 CSS 变量作为主题色

**主题映射规则**：

```typescript
// light/paper/hybrid-super-light → 使用 Naive 默认主题（null）
// 其他主题 → 使用 darkTheme
const naiveTheme = computed(() => {
  if (currentTheme.value === 'light' || 
      currentTheme.value === 'paper' || 
      currentTheme.value === 'hybrid-super-light') {
    return null  // 使用默认主题
  }
  return darkTheme  // 使用深色主题
})
```

**themeOverrides 配置**：

```typescript
const naiveThemeOverrides = computed(() => ({
  common: {
    primaryColor: 'var(--color-primary)',
    primaryColorHover: 'var(--color-primary-hover)',
    textColorBase: 'var(--color-text-main)',
    borderRadius: 'var(--radius-md)',
  },
  Button: {
    borderRadius: 'var(--radius-lg)',
  },
  Card: {
    borderRadius: 'var(--radius-xl)',
    boxShadow: 'var(--shadow-soft)',
  },
  // ... 其他组件配置
}))
```

#### 第 5 层：功能模块统一样式（`assets/css/`）

**职责**：跨组件复用的样式类

**文件位置**：`assets/css/` 目录

**命名规范**：使用功能前缀，如 `visitor-*`、`admin-*`、`home-*` 等

**要求**：
- 使用 CSS 变量，避免硬编码
- 确保与主题系统联动

**示例文件**：
- `assets/css/header.css` - Header 组件统一样式
- `assets/css/footer.css` - Footer 组件统一样式
- `assets/css/home.css` - 首页组件统一样式
- `assets/css/visitor-interaction.css` - 访客互动统一样式

#### 第 6 层：组件级样式（`<style scoped>`）

**职责**：组件特有的样式

**位置**：组件文件内的 `<style scoped>` 块

**要求**：
- 优先使用 CSS 变量
- 避免硬编码颜色、尺寸等值
- 确保样式与主题系统联动

## 🔄 主题切换机制

### 工作流程

1. **后端配置**：
   - 后台管理页面（`pages/admin/config.vue`）选择主题
   - 调用 `PUT /api/Config/theme` 保存主题
   - 保存成功后调用 `setTheme(themeKey)` 立即切换前端主题

2. **前端应用**：
   - `useTheme().setTheme()` 设置 `document.documentElement.dataset.theme`
   - `tokens.css` 中的 `html[data-theme="xxx"]` 选择器生效
   - CSS 变量自动切换，所有使用变量的组件自动更新

3. **Naive UI 联动**：
   - `AppNaiveConfig` 组件监听 `currentTheme` 变化
   - 自动计算 Naive 主题（light 用默认，其他用 darkTheme）
   - `themeOverrides` 中使用 CSS 变量，确保与主题系统联动

### 代码示例

#### 切换主题

```typescript
// 在组件中
const { setTheme } = useTheme()

// 切换到深色主题
setTheme('dark')

// 切换到实验室主题
setTheme('lab')
```

#### 获取当前主题

```typescript
// 在组件中
const { currentTheme } = useTheme()

// currentTheme 是响应式的，会自动更新
console.log(currentTheme.value)  // 'dark' | 'light' | 'lab' | ...
```

## 📝 开发规范

### 1. 使用 CSS 变量

**✅ 正确**：
```vue
<style scoped>
.my-button {
  background-color: var(--color-primary);
  color: var(--color-text-main);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
  box-shadow: var(--shadow-md);
}
</style>
```

**❌ 错误**：
```vue
<style scoped>
.my-button {
  background-color: #3b82f6;  /* ❌ 硬编码颜色 */
  color: #ffffff;            /* ❌ 硬编码颜色 */
  border-radius: 8px;        /* ❌ 硬编码尺寸 */
  padding: 16px;             /* ❌ 硬编码尺寸 */
}
</style>
```

### 2. 组件样式开发

**✅ 正确流程**：

1. 确定样式类型：
   - 设计变量（颜色、圆角、阴影等）→ 使用 CSS 变量
   - 组件特有样式 → 使用 `<style scoped>`
   - 跨组件复用样式 → 提取到 `assets/css/` 统一样式文件

2. 使用 CSS 变量：
   - 优先使用 `tokens.css` 中定义的变量
   - 避免硬编码颜色、尺寸等值
   - 确保样式与主题系统联动

3. Naive UI 组件：
   - 直接使用 Naive UI 组件，无需额外配置
   - 组件会自动应用 `AppNaiveConfig` 中的主题配置
   - 如需自定义样式，使用 CSS 变量覆盖

### 3. 主题切换开发

**✅ 正确**：
```typescript
// 使用 useTheme().setTheme() 切换主题
const { setTheme } = useTheme()
setTheme('dark')
```

**❌ 错误**：
```typescript
// ❌ 禁止直接设置 data-theme
document.documentElement.dataset.theme = 'dark'

// ❌ 禁止直接修改 CSS 变量
document.documentElement.style.setProperty('--color-primary', '#3b82f6')
```

## 🎯 最佳实践

### 1. 颜色使用

- ✅ 使用 `var(--color-primary)` 而非 `#3b82f6`
- ✅ 使用 `var(--color-bg-card)` 而非 `rgba(255, 255, 255, 0.1)`
- ✅ 使用 `var(--color-text-main)` 而非 `#ffffff`

### 2. 圆角使用

- ✅ 使用 `var(--radius-md)` 而非 `8px`
- ✅ 使用 `var(--radius-lg)` 而非 `12px`
- ✅ 使用 `var(--radius-xl)` 而非 `16px`

### 3. 阴影使用

- ✅ 使用 `var(--shadow-md)` 而非 `0 4px 6px rgba(0, 0, 0, 0.1)`
- ✅ 使用 `var(--shadow-soft)` 而非硬编码阴影值

### 4. 间距使用

- ✅ 使用 `var(--spacing-md)` 而非 `16px`
- ✅ 使用 `var(--spacing-lg)` 而非 `24px`

### 5. Naive UI 组件

- ✅ 直接使用，无需额外配置
- ✅ 如需自定义样式，使用 CSS 变量覆盖
- ✅ 避免直接覆盖 Naive UI 内部样式

## ⚠️ 注意事项

1. **CSS 变量优先级**：
   - `tokens.css` 中的变量定义在 `:root` 和 `html[data-theme="xxx"]` 中
   - `useTheme` 会动态设置 `data-theme` 属性，驱动主题切换
   - 旧的 `theme.css` 文件保留用于兼容性

2. **Naive UI 主题**：
   - `AppNaiveConfig` 组件会自动根据当前主题计算 Naive 主题
   - light/paper/hybrid-super-light 使用默认主题
   - 其他主题使用 darkTheme

3. **后端主题配置**：
   - 后台管理页面（`pages/admin/config.vue`）可以切换主题
   - 保存成功后立即切换前端主题，无需刷新页面
   - 访客刷新页面后会看到新的主题

4. **样式文件加载顺序**：
   - `tokens.css` 必须在最前面（定义 CSS 变量）
   - `base.css` 其次（基础样式）
   - `ui-patch-naive.css` 再次（Naive UI 补丁）
   - 其他样式文件最后

## 🔗 相关文件

- `assets/styles/tokens.css` - 设计 Token
- `assets/styles/base.css` - 基础样式
- `assets/styles/ui-patch-naive.css` - Naive UI 补丁
- `components/layout/AppNaiveConfig.vue` - Naive UI 配置容器
- `composables/useTheme.ts` - 主题管理
- `layouts/default.vue` - 默认布局
- `pages/admin/config.vue` - 后台主题配置页面
- `plugins/init-theme.client.ts` - 主题初始化

## 📚 参考文档

- [开发规范文档](./DEVELOPMENT_GUIDELINES.md) - 完整的开发规范
- [样式架构重构报告](../STYLE_ARCHITECTURE_REFACTOR.md) - 重构详细说明
- [样式管理开发提醒](./STYLE_MANAGEMENT_REMINDER.md) - 样式开发流程

---

**创建时间**：2024-12-03  
**下次更新**：根据使用反馈优化

