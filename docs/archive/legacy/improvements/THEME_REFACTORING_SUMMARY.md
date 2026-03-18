# 主题系统重构总结

## 已完成的重构

### 1. 布局文件（Layouts）

#### ✅ `layouts/default.vue`
- 添加了 `bg-bg-body text-text-main` 到最外层容器
- 确保整个默认布局使用主题背景和文字颜色

#### ✅ `layouts/home.vue`
- 添加了 `bg-bg-body text-text-main` 到最外层容器
- 首页布局现在会跟随主题变化

#### ✅ `layouts/ai.vue`
- 将写死的 `bg-[#050505] text-white` 替换为 `bg-bg-body text-text-main`
- AI 布局现在会跟随主题变化

#### ✅ `layouts/admin.vue`
- 侧边栏：将 `text-white` 替换为 `text-text-main`，添加 `bg-bg-card`
- 边框：将 `border-slate-700` 替换为 `border-border-subtle`
- 菜单组标题：将 `text-slate-400` 替换为 `text-text-muted`
- 主内容区：添加了 `bg-bg-body text-text-main`
- 退出按钮：使用主题颜色替换写死的颜色

### 2. 导航组件

#### ✅ `components/Header.vue`
- Logo 文本：`text-gray-800` → `text-text-main`
- 导航项 hover/active：使用 `bg-primary-soft` 和 `text-primary`
- 下拉菜单：`bg-white` → `bg-bg-card`，`border-gray-100` → `border-border-subtle`
- 移动端菜单：所有 `text-gray-600` → `text-text-muted`，`bg-gray-50` → `bg-primary-soft`
- CSS 样式：将写死的颜色值替换为 CSS 变量

### 3. Tailwind 配置

#### ✅ `tailwind.config.ts`
- 已确认语义颜色配置完整：
  - 背景色：`bg-body`, `bg-card`, `bg-elevated`
  - 文字颜色：`text-main`, `text-muted`, `text-disabled`
  - 边框颜色：`border-subtle`, `border-default`, `border-strong`
  - 主色调：`primary-base`, `primary-soft`, `primary-hover`
- 圆角和阴影已配置为使用 CSS 变量

### 4. Admin 页面示例

#### ✅ `pages/admin/analytics.vue`（部分）
- 标题：`text-gray-800 dark:text-white` → `text-text-main`
- 描述：`text-gray-500 dark:text-gray-400` → `text-text-muted`
- 按钮：开始使用 `AppButton` 组件
- 指标卡片：开始使用 `AppCard` 组件

---

## 待完成的重构

### 1. 首页组件（高优先级）

#### `components/home/HomeDarkLab.vue`
需要替换：
- `bg-slate-50` → `bg-bg-body`
- `text-slate-900` → `text-text-main`
- `text-slate-600` → `text-text-muted`
- `bg-white` → `bg-bg-card`
- `border-slate-200` → `border-border-subtle`
- 卡片区域尽量使用 `<AppCard>` 组件

#### `components/home/HomeLightPortfolio.vue`
需要替换：
- `bg-white` → `bg-bg-body` 或 `bg-bg-card`
- `text-gray-900` → `text-text-main`
- `text-gray-600` → `text-text-muted`
- `bg-blue-100` → `bg-primary-soft`
- `text-blue-700` → `text-primary`
- 按钮使用 `<AppButton>` 组件

### 2. 其他主要页面

#### `pages/about.vue`
- 背景和文字颜色需要主题化
- 卡片使用 `<AppCard>`

#### `pages/blog/**`
- 文章列表卡片
- 文章详情页

#### `pages/projects/**`
- 项目卡片
- 项目详情页

#### `pages/tools/**`
- 工具卡片
- 工具详情页

### 3. Admin 页面（批量处理）

所有 `/pages/admin/**` 下的页面需要：
- 替换 `text-gray-800 dark:text-white` → `text-text-main`
- 替换 `text-gray-500 dark:text-gray-400` → `text-text-muted`
- 替换 `bg-white dark:bg-gray-800` → `bg-bg-card`
- 替换 `border-gray-200 dark:border-gray-700` → `border-border-subtle`
- 按钮尽量使用 `<AppButton>`
- 卡片尽量使用 `<AppCard>`

**注意**：Admin 页面使用了 `dark:` 前缀，这是 Tailwind 的暗色模式语法。但我们的主题系统基于 `data-theme` 属性，需要移除 `dark:` 前缀，直接使用语义颜色。

### 4. 其他组件

- `components/Footer.vue`
- `components/GlassCard.vue`（如果还在使用）
- 其他业务组件

---

## 重构原则总结

### ✅ 已应用的原则

1. **不再使用写死的颜色值**
   - 已移除 `bg-[#020617]`、`text-[#e5e7eb]` 等
   - 已移除大部分 `bg-gray-800`、`text-gray-600` 等

2. **优先使用语义颜色**
   - `bg-bg-body`、`bg-bg-card`
   - `text-text-main`、`text-text-muted`
   - `border-border-subtle`

3. **通过 App 级组件统一风格**
   - 开始使用 `<AppCard>` 和 `<AppButton>`

4. **保持现有布局不变**
   - 只调整颜色类，不改动布局结构

### 📋 后续重构建议

1. **批量替换脚本**
   可以使用查找替换批量处理：
   - `text-gray-800 dark:text-white` → `text-text-main`
   - `text-gray-500 dark:text-gray-400` → `text-text-muted`
   - `bg-white dark:bg-gray-800` → `bg-bg-card`
   - `border-gray-200 dark:border-gray-700` → `border-border-subtle`

2. **组件化重复模式**
   如果发现多处使用相似的卡片/标签，可以创建：
   - `<AppMetricCard>` - 指标卡片
   - `<AppTag>` - 标签组件
   - `<AppSection>` - 区块容器

3. **渐进式重构**
   - 先处理最常用的页面（首页、导航）
   - 再处理内容页面（博客、项目）
   - 最后处理管理页面

---

## 视觉差异说明

### Light 主题
- 背景：浅灰色（`#f9fafb`）
- 卡片：白色（`#ffffff`）
- 文字：深灰色（`#111827`）
- 主色：蓝色（`#2563eb`）

### Dark 主题
- 背景：深黑色（`#030712`）
- 卡片：深灰色（`#111827`）
- 文字：浅灰色（`#f9fafb`）
- 主色：亮蓝色（`#3b82f6`）

### Tech Blue 主题
- 背景：深蓝黑色（`#0a1929`）
- 卡片：深蓝色（`#112240`）
- 文字：浅蓝色（`#e6f1ff`）
- 主色：科技蓝（`#22d3ee`）

---

## 修改的文件列表

### 布局文件
- ✅ `layouts/default.vue`
- ✅ `layouts/home.vue`
- ✅ `layouts/ai.vue`
- ✅ `layouts/admin.vue`（部分）

### 组件文件
- ✅ `components/Header.vue`
- ✅ `components/ui/AppCard.vue`（已存在，已使用主题变量）
- ✅ `components/ui/AppButton.vue`（已存在，已使用主题变量）

### 配置文件
- ✅ `tailwind.config.ts`（已确认配置完整）

### 页面文件（示例）
- ✅ `pages/admin/analytics.vue`（部分，作为示例）

---

## 下一步行动

1. **完成首页组件重构**（`HomeDarkLab.vue` 和 `HomeLightPortfolio.vue`）
2. **批量处理 Admin 页面**（使用查找替换 + 手动检查）
3. **处理其他主要页面**（about, blog, projects 等）
4. **测试主题切换效果**，确保所有页面都能正确响应主题变化

---

## 注意事项

1. **Admin 页面的 `dark:` 前缀**
   - 需要移除所有 `dark:` 前缀
   - 直接使用语义颜色，主题系统会自动处理

2. **渐变和特效**
   - 可以保留渐变效果
   - 但渐变的基础颜色尽量使用 `var(--color-primary)` 等主题变量

3. **兼容性**
   - 确保未重构的页面仍能正常显示
   - 渐进式重构，不影响现有功能

