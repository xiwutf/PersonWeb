# 个人平台设计系统 v1

本设计系统用于规范整个平台（前台 + 后台）的视觉与交互，依托 Naive UI + 自定义 themeOverrides 实现。

---

## 1. 主题与模式

### 1.1 主题模式

- **支持模式：**
  - Light（浅色）
  - Dark（深色）

- **模式切换入口：**
  - 前台：顶部导航 / 设置入口
  - 后台：主题设置页 `/admin/theme-settings`

- **技术约定：**
  - 所有主题切换统一通过 `useTheme` composable：
    - `currentTheme` - 当前主题（'light' | 'dark'）
    - `setTheme('light' | 'dark')` - 设置主题
    - `toggleDark()` - 切换深色/浅色模式
  
  - 切换时必须：
    - 更新 `document.documentElement.dataset.theme`
    - 更新 Naive UI 的 theme & themeOverrides

- **实现位置：**
  - `composables/useTheme.ts` - 主题管理逻辑
  - `components/layout/AppNaiveConfig.vue` - 统一的 Naive UI 配置容器

---

## 2. 色彩系统（Color System）

### 2.1 颜色角色（而不是颜色值）

**常用角色：**

- **背景类**
  - `bg-page`：页面主背景（`themeOverrides.Layout.color`）
  - `bg-elevated`：浮层/卡片背景（`themeOverrides.Card.color`）
  - `bg-subtle`：轻度分区背景（`themeOverrides.Layout.hoverColor`）

- **文本类**
  - `text-primary`：主要文字（`themeOverrides.common.textColor1`）
  - `text-secondary`：次级文字（`themeOverrides.common.textColor2`）
  - `text-muted`：弱提示文字（`themeOverrides.common.textColor3`）

- **边框/分割线**
  - `border-strong`：明显分区边界（`themeOverrides.common.borderColor`）
  - `border-subtle`：弱分割线（`themeOverrides.common.borderColor`）

- **状态色**
  - `primary`：主色，用于主要按钮、重点高亮（`themeOverrides.common.primaryColor`）
  - `success`：成功态（Naive UI `success` 类型）
  - `warning`：警告态（Naive UI `warning` 类型）
  - `error`：错误态（Naive UI `error` 类型）
  - `info`：普通提示态（Naive UI `info` 类型）

> **重要：** 所有这些角色最终通过 `AppNaiveConfig.vue` 的 `themeOverrides` 来实现，工程代码不直接写颜色值。

### 2.2 Light / Dark 模式下的表现

#### Light 模式

- **背景色：**
  - 页面主背景：`#ffffff`
  - 卡片背景：`#ffffff`
  - 浮层背景：`#f8fafc`

- **文字颜色：**
  - 主要文字：`#0f172a`
  - 次要文字：`#64748b`
  - 弱提示文字：`#94a3b8`

- **主色调：**
  - Primary：`#2563eb`
  - Primary Hover：`#1d4ed8`
  - Primary Soft：`#dbeafe`

- **边框颜色：**
  - 默认边框：`#e2e8f0`

#### Dark 模式

- **背景色：**
  - 页面主背景：`#0a0e1a`
  - 卡片背景：`rgba(19, 23, 32, 0.8)`
  - 浮层背景：`#131720`

- **文字颜色：**
  - 主要文字：`rgba(255, 255, 255, 0.92)`
  - 次要文字：`rgba(255, 255, 255, 0.6)`
  - 弱提示文字：`rgba(255, 255, 255, 0.4)`

- **主色调：**
  - Primary：`#60a5fa`
  - Primary Hover：`#3b82f6`
  - Primary Soft：`rgba(96, 165, 250, 0.15)`

- **边框颜色：**
  - 默认边框：`rgba(255, 255, 255, 0.1)`

---

## 3. 组件规范（Component Guidelines）

### 3.1 Button 按钮

**组件：** `n-button`  
**来源：** Naive UI + `themeOverrides.Button`

#### 使用规则

- **主操作：** `type="primary"`
  ```vue
  <n-button type="primary">保存</n-button>
  ```

- **正向重要操作（成功/确认）：** `type="success"`
  ```vue
  <n-button type="success">确认</n-button>
  ```

- **危险操作：** `type="error"`
  ```vue
  <n-button type="error">删除</n-button>
  ```

- **次级操作：** `secondary`
  ```vue
  <n-button secondary>取消</n-button>
  ```

- **取消操作：** `quaternary`（统一用它）
  ```vue
  <n-button quaternary @click="handleCancel">取消</n-button>
  ```

#### 禁止事项

- ❌ 不允许直接给按钮写 `background-color`、`color`
- ❌ 不允许用 Tailwind 类改按钮颜色，如 `bg-blue-500 text-white`
- ✅ 如需特例，请通过 `themeOverrides.Button` 或新建一个 Button 封装组件

---

### 3.2 Card 卡片

**组件：** `n-card`  
**来源：** Naive UI + `themeOverrides.Card`

#### 使用规则

**普通信息卡片：**
```vue
<n-card class="mb-4">
  <template #header>标题</template>
  内容
</n-card>
```

**仪表盘强调卡片：**
```vue
<n-card class="dashboard-card">
  内容
</n-card>
```

> `dashboard-card` 仅用于：
> - 布局：宽度、高度、内边距的定制
> - 排版：flex、grid 布局
> - ✅ 禁止在该类中写 `background` / `box-shadow` / `border`

#### 禁止事项

- ❌ `class="bg-white shadow-lg rounded-xl"`
- ❌ 单独的 `.xxx-card { background: ...; box-shadow: ... }`

> **所有 Card 的背景、阴影、圆角、边框，由 `themeOverrides.Card` 统一控制。**

#### Card 视觉配置

所有 Card 的视觉样式由 `themeOverrides.Card` 统一控制：

- `color` - 背景色
- `borderRadius` - 圆角（默认：`16px`）
- `boxShadow` - 阴影
- `borderColor` - 边框颜色
- `paddingMedium` - 内边距（默认：`16px 20px`）

---

### 3.3 Input / Select / Form

一律使用 Naive 组件：`n-input`, `n-select`, `n-input-number`, `n-date-picker` 等。

表单项统一使用 `n-form` + `n-form-item`。

#### 约定

- 表单标签文字：由 `Form` / `FormItem` 的 `themeOverrides` 控制
- placeholder、边框 hover/focus 颜色：统一在 `themeOverrides.Input` / `Select` 中配置
- ❌ 禁止在组件外写 `border-color`、`background-color` 覆盖视觉

#### 使用示例

```vue
<n-form :model="form" :rules="rules">
  <n-form-item label="名称" path="name">
    <n-input v-model:value="form.name" placeholder="请输入名称" />
  </n-form-item>
  <n-form-item label="类型" path="type">
    <n-select v-model:value="form.type" :options="options" />
  </n-form-item>
</n-form>
```

---

### 3.4 DataTable（表格）

统一使用 `n-data-table`

#### 视觉规范

- **表头：** 有轻微底色或边框，与内容区拉开层级
- **行 hover：** 有明显背景色变化（但不影响文字可读性）
- **斑马纹：** 在需要时启用，使用统一颜色

所有这些由 `themeOverrides.DataTable` 管控：

- `thColor` - 表头背景色
- `thBorderColor` - 表头边框颜色
- `tdColor` - 表格单元格背景色
- `tdBorderColor` - 表格单元格边框颜色
- `tdColorHover` - 行 hover 背景色
- `tdColorStriped` - 斑马纹背景色

#### 使用示例

```vue
<n-data-table
  :columns="columns"
  :data="data"
  :pagination="pagination"
/>
```

---

### 3.5 Layout / Menu

后台统一使用：`n-layout` + `n-layout-sider` + `n-layout-header` + `n-layout-content`

菜单统一使用：`n-menu`

#### 视觉规范

Layout 与 Menu 的颜色，一律来自 `themeOverrides`：

- **Layout：**
  - `Layout.color` - 页面背景色
  - `Layout.headerColor` - 头部背景色
  - `Layout.siderColor` - 侧边栏背景色
  - `Layout.siderBorderColor` - 侧边栏边框颜色

- **Menu：**
  - `Menu.itemColor` - 菜单项背景色
  - `Menu.itemColorActive` - 激活状态背景色
  - `Menu.itemColorHover` - hover 状态背景色
  - `Menu.itemTextColor` - 菜单项文字颜色
  - `Menu.itemTextColorActive` - 激活状态文字颜色
  - `Menu.itemIconColor` - 图标颜色

---

### 3.6 图表（ECharts）

使用 `useEChartsTheme.ts` 根据 `currentTheme` 输出深/浅主题配置

图表相关颜色不直接散落在组件中，而是在 ECharts 主题配置中统一定义：

包括：
- `tooltip.backgroundColor`, `borderColor`
- `axisLine.lineStyle.color`
- `axisLabel.color`
- `splitLine.lineStyle.color`
- 系列默认颜色列表

#### 使用示例

```vue
<script setup>
import { useEChartsTheme } from '~/composables/useEChartsTheme'

const { currentTheme, applyTheme } = useEChartsTheme()

const chartOption = computed(() => {
  const baseOption = {
    // ... 图表配置
  }
  return applyTheme(baseOption)
})
</script>
```

---

## 4. Tokens 规范（Design Tokens）

### 4.1 Token 分类

**尺寸类：**
- 间距：`--spacing-xs` / `--spacing-sm` / `--spacing-md` / `--spacing-lg` / `--spacing-xl`
- 圆角：`--radius-sm` / `--radius-md` / `--radius-lg` / `--radius-xl`
- 字号：`--font-size-xs` / `--font-size-sm` / `--font-size-base` / `--font-size-lg` / `--font-size-xl` / ...

**阴影类：**
- `--shadow-sm` - 小阴影
- `--shadow-md` - 中等阴影
- `--shadow-lg` - 大阴影
- `--shadow-soft` - 柔和阴影

**布局类：**
- 例如：侧边栏宽度、导航高度等（视需要增加）

### 4.2 禁止在 Token 中继续维护颜色

所有颜色 Token（`--color-*`, `--bg-*`, `--text-*` 等）逐步删除

颜色统一由：
- `AppNaiveConfig.vue` 中的 `themeOverrides`
- `useEChartsTheme.ts` 中的图表主题

控制

> **注意：** `tokens.css` 中保留的颜色变量仅用于向后兼容，新代码不应使用。

---

## 5. 开发约定（给自己和以后协作者看的）

### 5.1 新增页面 / 组件时优先级

1. **能用 Naive 组件 → 优先用**
2. **能通过 themeOverrides 调整 → 不写散乱 CSS**
3. **必须写自定义样式时 → 禁止写颜色，优先控制布局**

### 5.2 代码审查 Checklist

- [ ] 是否新加了 `#fff`、`#000` 等硬编码颜色？
- [ ] 是否给 `n-button` / `n-card` / `n-input` 写了颜色相关的 class？
- [ ] 是否用 `useTheme` 统一处理主题，而不是自己搞 `isDark`？
- [ ] Card 组件是否只使用了布局类，没有颜色/阴影/边框类？
- [ ] 按钮是否都有明确的 `type` 属性？

### 5.3 特殊视觉效果

玻璃态 / 渐变 / 特殊装饰，可以在专用组件内写，但必须：

- 代码里标注注释：`// 特殊视觉组件，允许自定义颜色`
- 将这类组件数量控制在非常少的几个"重点模块"中

---

## 6. 文件结构

### 6.1 核心文件

- `components/layout/AppNaiveConfig.vue` - 统一的 Naive UI 配置容器
- `composables/useTheme.ts` - 主题管理逻辑
- `composables/useEChartsTheme.ts` - ECharts 主题配置
- `assets/styles/tokens.css` - 结构类 Token（圆角、阴影、间距等）

### 6.2 文档文件

- `docs/DESIGN_SYSTEM_V1.md` - 本设计系统文档
- `docs/CODING_STYLE_UI.md` - UI 编码规范
- `docs/COLOR_STATISTICS.md` - 颜色统计和迁移指南
- `docs/PHASE3_THEME_OPTIMIZATION.md` - 第三阶段优化报告

---

## 7. 迁移状态

### 7.1 已完成

- ✅ 主题入口统一（AppNaiveConfig + useTheme）
- ✅ Naive UI 的主组件都接入 themeOverrides
- ✅ Card 完全收编，成为"系统级组件"
- ✅ Button 使用规范统一
- ✅ Input/Select/Form 深色模式适配
- ✅ DataTable 视觉统一
- ✅ Layout/Menu 视觉体系统一
- ✅ ECharts 深色主题适配

### 7.2 进行中

- 🔄 硬编码颜色迁移（736 个匹配，按优先级逐步迁移）
- 🔄 tokens.css 精简（颜色变量标记为已迁移，保留用于向后兼容）

### 7.3 待完成

- ⏳ 颜色迁移完成后的 tokens.css 最终精简
- ⏳ 新增颜色使用守卫（lint 规则）
- ⏳ 组件封装（AdminPageCard, DashboardSectionCard 等）

---

## 8. 参考资源

- [Naive UI 官方文档](https://www.naiveui.com/)
- [Naive UI Theme Overrides](https://www.naiveui.com/zh-CN/os-theme/docs/theme-overrides)
- [设计系统最佳实践](https://www.designsystems.com/)

---

**版本：** v1-beta  
**最后更新：** 2024-12-XX  
**维护者：** 项目团队

