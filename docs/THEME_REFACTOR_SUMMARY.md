# 主题系统重构完成说明

## 重构日期
2024-12-XX

## 重构目标
将当前项目的多主题系统精简为仅两个主题：**light（浅色）** 和 **dark（深色）**。

## 修改的文件列表

### 前端文件
1. **`assets/styles/tokens.css`**
   - 删除了除 light 和 dark 之外的所有主题定义
   - 重新设计了 light 和 dark 两套主题的视觉，确保差异明显
   - 添加了新的 CSS 变量（--bg、--bg-elevated、--text-main、--primary 等）

2. **`constants/design/tokens.ts`**
   - 精简 `ThemeName` 类型为 `'light' | 'dark'`
   - 删除了 `defaultThemeTokens` 中所有旧主题的定义
   - 添加了 `normalizeTheme()` 函数，用于将旧主题映射为 light 或 dark

3. **`composables/useTheme.ts`**
   - 更新了 `applyTheme()`、`loadThemeFromStorage()`、`setTheme()` 等方法，使用 `normalizeTheme()` 兼容旧数据
   - 移除了对旧主题的硬编码验证

4. **`plugins/init-theme.client.ts`**
   - 更新了主题获取逻辑，使用 `normalizeTheme()` 标准化后端返回的主题

5. **`components/layout/AppNaiveConfig.vue`**
   - 精简了 Naive UI 主题映射逻辑，只保留 light 和 dark 两种适配

6. **`pages/admin/config.vue`**
   - 更新了主题选择下拉框，只显示 light 和 dark 两个选项
   - 添加了提示信息："目前仅保留两套稳定主题，后续如果有需要再新增。"

7. **`pages/admin/themes.vue`**
   - 更新了主题选项列表，只保留 light 和 dark
   - 更新了相关类型定义和验证逻辑

### 后端文件
8. **`backend/PersonalSite.Api/Controllers/ConfigController.cs`**
   - 更新了 `GetTheme()` 方法，使用 `NormalizeTheme()` 映射旧主题
   - 更新了 `SetTheme()` 方法，只允许 light 或 dark，其他值返回 400 错误
   - 更新了 `GetModuleThemes()` 和 `SetModuleThemes()` 方法，标准化模块主题
   - 更新了 `GetThemeTokens()` 和 `SetThemeTokens()` 方法，只允许 light 或 dark
   - 添加了 `NormalizeTheme()` 私有方法，用于主题映射

## 两个主题的配色预览

### Light（浅色主题）
- **整体风格**：干净、明亮、轻量
- **背景色**：
  - 页面主背景：`#ffffff`（纯白）
  - 卡片/模块背景：`#f8fafc`（极浅灰）
  - 卡片背景：`#ffffff`（纯白）
- **文字颜色**：
  - 主要文字：`#0f172a`（接近黑色）
  - 次要文字：`#475569`（深灰）
  - 弱化文字：`#64748b`（中灰）
- **主色调**：
  - 主色：`#2563eb`（蓝色）
  - 主色悬停：`#1d4ed8`（深蓝）
  - 主色柔和背景：`#dbeafe`（浅蓝）
- **边框/分割线**：
  - 边框颜色：`#e2e8f0`（浅灰）
  - 分割线颜色：`#cbd5e1`（中灰）
- **阴影**：轻微阴影，营造层次感

### Dark（深色主题）
- **整体风格**：深色实验室/科技感
- **背景色**：
  - 页面主背景：`#0a0e1a`（接近黑色，深蓝灰）
  - 卡片/模块背景：`#131720`（稍亮的深灰）
  - 卡片背景：`rgba(19, 23, 32, 0.8)`（半透明深灰，玻璃态）
- **文字颜色**：
  - 主要文字：`rgba(255, 255, 255, 0.92)`（高亮白色）
  - 次要文字：`rgba(255, 255, 255, 0.75)`（半透明白）
  - 弱化文字：`rgba(255, 255, 255, 0.6)`（更透明的白）
- **主色调**：
  - 主色：`#60a5fa`（亮蓝色，对比强烈）
  - 主色悬停：`#3b82f6`（标准蓝）
  - 主色柔和背景：`rgba(96, 165, 250, 0.15)`（透明蓝）
- **边框/分割线**：
  - 边框颜色：`rgba(255, 255, 255, 0.1)`（极细白色边框）
  - 分割线颜色：`rgba(255, 255, 255, 0.15)`（稍明显的白色）
- **阴影**：深色阴影 + 微光边框，营造科技感

## 主题映射规则

旧主题会自动映射为 light 或 dark：

- **映射为 light**：
  - 包含 "light" 的主题（如 `hybrid-super-light`）
  - 包含 "paper" 的主题（如 `paper`）

- **映射为 dark**：
  - 包含 "dark" 的主题（如 `dark`、`hybrid-super-dark`）
  - 包含 "tech" 的主题（如 `tech-blue`）
  - 包含 "forest" 的主题（如 `forest`）
  - 包含 "hybrid" 的主题（如 `hybrid-super`）
  - 包含 "lab" 的主题（如 `lab`）

- **默认**：其他情况默认映射为 `dark`（更安全，避免浅色主题在深色背景下不可见）

## 以后如果要新增第 3 个主题时，需要改动的入口位置

### 1. 前端类型定义
- **`constants/design/tokens.ts`**
  - 更新 `ThemeName` 类型：`export type ThemeName = 'light' | 'dark' | 'new-theme'`
  - 在 `defaultThemeTokens` 中添加新主题的定义
  - 在 `themeVariables` 中添加新主题的 CSS 变量定义

### 2. CSS 变量定义
- **`assets/styles/tokens.css`**
  - 添加 `html[data-theme="new-theme"] { ... }` 块
  - 定义新主题的所有 CSS 变量

### 3. 后端 API 校验
- **`backend/PersonalSite.Api/Controllers/ConfigController.cs`**
  - 在 `GetTheme()`、`SetTheme()`、`GetModuleThemes()`、`SetModuleThemes()`、`GetThemeTokens()`、`SetThemeTokens()` 方法中
  - 更新 `availableThemes` 列表，添加新主题

### 4. 后台管理界面
- **`pages/admin/config.vue`**
  - 在主题选择下拉框中添加新选项：`<option value="new-theme">新主题名称</option>`
  - 更新 `siteTheme` 的类型定义

- **`pages/admin/themes.vue`**
  - 在 `themeOptions` 数组中添加新选项
  - 更新 `globalTheme`、`selectedThemeForTokens` 等变量的类型定义
  - 更新 `availableThemes` 数组

### 5. Naive UI 主题适配（可选）
- **`components/layout/AppNaiveConfig.vue`**
  - 在 `naiveTheme` 和 `actualTheme` 计算属性中添加新主题的映射逻辑
  - 决定新主题使用 Naive UI 的默认主题（null）还是 darkTheme

### 6. 主题映射函数（可选）
- **`constants/design/tokens.ts`** 和 **`backend/PersonalSite.Api/Controllers/ConfigController.cs`**
  - 如果新主题不需要映射，可以保持 `normalizeTheme()` 函数不变
  - 如果需要映射，在函数中添加相应的映射规则

## 注意事项

1. **向后兼容**：通过 `normalizeTheme()` 函数，旧主题会自动映射为 light 或 dark，不会导致错误。

2. **数据库迁移**：如果数据库中有旧主题配置，建议运行迁移脚本，将旧主题值更新为 light 或 dark。或者依赖 `normalizeTheme()` 函数自动映射。

3. **测试建议**：
   - 测试从旧主题切换到新主题的流程
   - 测试后端返回旧主题时的映射逻辑
   - 测试 localStorage 中保存旧主题时的兼容性

4. **性能优化**：删除旧主题定义后，CSS 文件大小会减小，加载速度会提升。

## 总结

本次重构成功将多主题系统精简为仅两个主题（light 和 dark），同时保持了向后兼容性。所有旧主题都会自动映射为 light 或 dark，不会导致系统错误。两套主题的视觉差异明显，用户可以清楚地看到主题切换的效果。

