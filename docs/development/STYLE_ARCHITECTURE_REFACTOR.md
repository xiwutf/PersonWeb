# 样式架构重构完成报告

**完成日期**：2024-12-03  
**重构目标**：结合 Naive UI 主题系统，建立统一的样式架构

## ✅ 已完成的工作

### 1. 创建全局样式文件

#### 1.1 `assets/styles/tokens.css` ✅
- **职责**：定义全局 CSS 变量（颜色、圆角、阴影、字号、间距等）
- **特点**：
  - 通过 `:root` 定义默认主题变量
  - 通过 `html[data-theme="xxx"]` 为不同主题设置变量覆盖
  - 支持的主题：`light`、`dark`、`lab`、`tech-blue`、`paper`、`forest`、`hybrid-super`、`hybrid-super-dark`、`hybrid-super-light`
- **使用方式**：在组件中使用 `var(--color-primary)` 等变量

#### 1.2 `assets/styles/base.css` ✅
- **职责**：基础样式和 Reset
- **内容**：
  - `html, body` 的默认字体、背景色、文字颜色
  - 链接、图片、按钮、输入框、列表等基础重置
  - 标题、段落、代码等排版样式
  - 滚动条和选择文本样式
- **特点**：不覆盖 Naive UI 内部样式，只做基础重置

#### 1.3 `assets/styles/ui-patch-naive.css` ✅
- **职责**：Naive UI 全局补丁样式
- **内容**：
  - Card、Dialog、Button、Input、Select、Form、Table、Menu 等组件的统一风格补丁
  - 使用 CSS 变量（如 `var(--radius-xl)`、`var(--shadow-soft)`）确保与主题系统联动
- **特点**：只做视觉风格调整，不覆盖核心功能

### 2. 创建 AppNaiveConfig 组件

#### 2.1 `components/layout/AppNaiveConfig.vue` ✅
- **职责**：统一管理 Naive UI 的主题配置
- **功能**：
  - 使用 `NConfigProvider`、`NDialogProvider`、`NMessageProvider`、`NNotificationProvider` 包裹内容
  - 基于 `useTheme().currentTheme` 计算 Naive 主题（light 用默认，其他用 darkTheme）
  - 在 `themeOverrides` 中使用 CSS 变量（如 `var(--color-primary)`）作为主题色
  - 支持 SSR，使用动态导入避免服务端错误
- **使用方式**：在 `layouts/default.vue` 中使用 `<AppNaiveConfig>` 包裹内容

### 3. 修改 useTheme Composable

#### 3.1 `composables/useTheme.ts` ✅
- **更新内容**：
  - 确保 `applyTheme` 函数设置 `document.documentElement.dataset.theme`
  - 添加注释说明 `data-theme` 是新的样式架构的核心
- **工作原理**：
  - 通过 `data-theme` 属性驱动 `tokens.css` 中的主题变量
  - 同时将 CSS 变量写入 `document.documentElement.style`（兼容旧系统）

#### 3.2 `plugins/init-theme.client.ts` ✅
- **更新内容**：
  - 在从后端获取主题后，立即设置 `document.documentElement.dataset.theme`
  - 确保主题切换时 `data-theme` 属性正确更新

### 4. 修改布局文件

#### 4.1 `layouts/default.vue` ✅
- **更新内容**：
  - 使用 `<AppNaiveConfig>` 包裹所有内容
  - 确保所有 Naive UI 组件自动应用统一的主题配置

### 5. 更新 Nuxt 配置

#### 5.1 `nuxt.config.ts` ✅
- **更新内容**：
  - 在 `css` 数组中添加新的样式文件：
    - `~/assets/styles/tokens.css`（最前面，定义 CSS 变量）
    - `~/assets/styles/base.css`（基础样式）
    - `~/assets/styles/ui-patch-naive.css`（Naive UI 补丁）
  - 保留 `~/assets/styles/theme.css`（兼容性）

### 6. 更新后台管理页面

#### 6.1 `pages/admin/config.vue` ✅
- **更新内容**：
  - 在 `saveTheme` 函数中，保存成功后调用 `setTheme(themeKey)` 立即切换主题
  - 添加 `lab` 和 `hybrid-super` 主题选项
  - 更新主题类型定义，包含所有支持的主题

## 📋 架构说明

### 样式分层结构

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

### 关键设计点

1. **CSS 变量驱动**：
   - 所有颜色、圆角、阴影等设计变量都定义在 `tokens.css` 中
   - 通过 `html[data-theme="xxx"]` 切换主题
   - 自定义组件和 Naive UI 都使用相同的 CSS 变量

2. **Naive UI 主题联动**：
   - `AppNaiveConfig` 组件统一管理 Naive UI 主题
   - `themeOverrides` 中使用 CSS 变量，确保与主题系统联动
   - light 主题使用 Naive 默认主题，其他主题使用 darkTheme

3. **后端配置驱动**：
   - 从 `/api/Config/theme` 获取当前主题
   - 保存主题时调用 `PUT /api/Config/theme`
   - 保存成功后立即调用 `setTheme()` 切换前端主题

## 🎯 使用指南

### 在组件中使用 CSS 变量

```vue
<template>
  <div class="my-component">
    <button class="my-button">按钮</button>
  </div>
</template>

<style scoped>
.my-component {
  background-color: var(--color-bg-card);
  color: var(--color-text-main);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
}

.my-button {
  background-color: var(--color-primary);
  color: var(--color-text-main);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
}

.my-button:hover {
  background-color: var(--color-primary-hover);
  box-shadow: var(--shadow-lg);
}
</style>
```

### 在 Naive UI 组件中使用

所有 Naive UI 组件会自动应用 `AppNaiveConfig` 中的主题配置，无需额外设置。

```vue
<template>
  <n-button type="primary">按钮</n-button>
  <n-card>卡片内容</n-card>
  <n-input v-model:value="inputValue" placeholder="请输入" />
</template>
```

### 切换主题

```typescript
// 在组件中
const { setTheme } = useTheme()
setTheme('dark') // 切换到深色主题
setTheme('lab')  // 切换到实验室主题
```

## 📝 注意事项

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

## 🔗 相关文件

- `assets/styles/tokens.css` - 设计 Token
- `assets/styles/base.css` - 基础样式
- `assets/styles/ui-patch-naive.css` - Naive UI 补丁
- `components/layout/AppNaiveConfig.vue` - Naive UI 配置容器
- `composables/useTheme.ts` - 主题管理
- `layouts/default.vue` - 默认布局
- `pages/admin/config.vue` - 后台主题配置页面

---

**重构完成时间**：2024-12-03  
**下次更新**：根据使用反馈优化

