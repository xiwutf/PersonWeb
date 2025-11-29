# 🎨 样式管理开发提醒

## ⚠️ 重要提醒

**每次修改页面样式时，请务必遵循样式统一管理规范！**

## 📋 开发前检查清单

在开始修改页面样式之前，请先检查：

- [ ] 是否已查看 `docs/development/DEVELOPMENT_GUIDELINES.md` 中的样式管理规范？
- [ ] 是否已查看 `docs/development/FRONTEND_STYLE_SYSTEM.md` 了解样式系统？
- [ ] 是否已检查 `assets/css/` 目录下是否有可复用的样式类？
- [ ] 是否已检查 `composables/useStyle.ts` 或 `composables/useAdminStyle.ts` 是否有可用的样式管理功能？

## 🚫 禁止事项

### ❌ 禁止在 template 中使用内联样式（`:style` 除外）

```vue
<!-- ❌ 错误：固定样式使用内联 -->
<div style="padding: 1rem; background: #fff; border-radius: 8px;">
  <button style="width: 100px; height: 40px; background: #3b82f6;">
    按钮
  </button>
</div>

<!-- ✅ 正确：使用 CSS 类 -->
<div class="admin-card">
  <button class="admin-button-primary">
    按钮
  </button>
</div>
```

### ❌ 禁止在组件中直接写固定颜色、尺寸等样式

```vue
<!-- ❌ 错误 -->
<div class="bg-blue-600 text-white px-4 py-2 rounded">
  内容
</div>

<!-- ✅ 正确：使用统一样式类 -->
<div class="admin-card-header">
  内容
</div>
```

## ✅ 正确的样式管理方式

### 1. 使用统一样式文件（推荐）

**位置**：`assets/css/` 目录

**命名规范**：
- 管理后台样式：`admin-*.css`
- 访客页面样式：`visitor-*.css`
- 通用样式：`common-*.css` 或 `main.css`

**示例**：
```css
/* assets/css/admin-analytics.css */
.admin-analytics-card {
  @apply bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6;
}

.admin-analytics-title {
  @apply text-lg font-bold text-gray-800 dark:text-white mb-4;
}

.admin-analytics-button {
  @apply px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700;
}
```

**在组件中使用**：
```vue
<template>
  <div class="admin-analytics-card">
    <h2 class="admin-analytics-title">访客分析</h2>
    <button class="admin-analytics-button">刷新数据</button>
  </div>
</template>
```

### 2. 使用组件级样式（`<style scoped>`）

**适用场景**：组件特有的样式，不会在其他地方复用

```vue
<template>
  <div class="analytics-container">
    <!-- 内容 -->
  </div>
</template>

<style scoped>
.analytics-container {
  padding: 1rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
}
</style>
```

### 3. 使用样式管理系统（高级）

**适用场景**：需要动态配置的样式

#### 管理后台样式
```vue
<script setup>
import { useAdminGlobalStyle } from '~/composables/useAdminStyle'

const { cssVariables, inlineStyle } = useAdminGlobalStyle()
</script>

<template>
  <div :style="inlineStyle">
    <!-- 使用 CSS 变量 -->
    <div style="background: var(--admin-card-bg)">
      内容
    </div>
  </div>
</template>
```

#### 通用样式管理
```vue
<script setup>
import { useStyle } from '~/composables/useStyle'

const { getStyleClass } = useStyle()
const buttonClass = getStyleClass('button-primary')
</script>

<template>
  <button :class="buttonClass">
    按钮
  </button>
</template>
```

### 4. 动态样式（`:style` 绑定）

**仅用于必须动态计算的属性**：

```vue
<template>
  <!-- ✅ 正确：位置需要动态计算 -->
  <div 
    class="visitor-footprint"
    :style="{ 
      left: `${xPosition}%`,
      top: `${yPosition}%`
    }"
  >
    足迹
  </div>

  <!-- ✅ 正确：颜色需要动态计算 -->
  <div 
    class="chart-item"
    :style="{ 
      backgroundColor: dynamicColor 
    }"
  >
    图表项
  </div>

  <!-- ❌ 错误：固定值不应使用 :style -->
  <div :style="{ padding: '1rem', background: '#fff' }">
    内容
  </div>
</template>
```

## 📝 开发流程

### 步骤 1：检查现有样式

1. 查看 `assets/css/` 目录，寻找可复用的样式类
2. 查看 `composables/useStyle.ts` 和 `composables/useAdminStyle.ts`
3. 查看其他类似页面的样式实现

### 步骤 2：决定样式位置

- **跨组件复用** → `assets/css/` 统一样式文件
- **组件特有** → `<style scoped>`
- **动态配置** → 样式管理系统
- **动态计算** → `:style` 绑定（最小化）

### 步骤 3：创建/使用样式

#### 如果是新样式类：

1. **确定文件位置**：
   - 管理后台：`assets/css/admin-*.css`
   - 访客页面：`assets/css/visitor-*.css`
   - 通用：`assets/css/common-*.css`

2. **命名规范**：
   ```css
   /* 功能前缀 + 元素类型 + 状态/变体 */
   .admin-analytics-card
   .admin-analytics-card-header
   .admin-analytics-button-primary
   .admin-analytics-button-secondary
   ```

3. **使用 Tailwind 工具类**（推荐）：
   ```css
   .admin-analytics-card {
     @apply bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6;
   }
   ```

4. **在组件中引用**：
   ```vue
   <template>
     <div class="admin-analytics-card">
       <!-- 内容 -->
     </div>
   </template>
   ```

### 步骤 4：验证

- [ ] 样式是否遵循命名规范？
- [ ] 是否避免了内联样式（`:style` 除外）？
- [ ] 是否使用了统一样式文件或组件级样式？
- [ ] 是否考虑了暗色模式（dark mode）？
- [ ] 是否考虑了响应式设计？

## 🔍 常见错误示例

### 错误 1：直接使用内联样式

```vue
<!-- ❌ 错误 -->
<div style="padding: 1rem; background: #fff; border-radius: 8px;">
  内容
</div>

<!-- ✅ 正确 -->
<div class="admin-card">
  内容
</div>
```

### 错误 2：在 template 中直接写 Tailwind 类

```vue
<!-- ❌ 错误：应该提取到统一样式文件 -->
<div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
  内容
</div>

<!-- ✅ 正确 -->
<div class="admin-card">
  内容
</div>
```

### 错误 3：使用 `:style` 绑定固定值

```vue
<!-- ❌ 错误 -->
<div :style="{ padding: '1rem', background: '#fff' }">
  内容
</div>

<!-- ✅ 正确 -->
<div class="admin-card">
  内容
</div>
```

## 📚 相关文档

- [开发规范文档](./DEVELOPMENT_GUIDELINES.md) - 完整的开发规范
- [前端样式系统文档](./FRONTEND_STYLE_SYSTEM.md) - 样式管理系统详细说明
- [样式重构进度文档](./STYLE_REFACTORING_PROGRESS.md) - 样式重构进度跟踪

## 🛠️ 工具和资源

### 样式文件位置

- **管理后台样式**：`assets/css/admin-*.css`
- **访客页面样式**：`assets/css/visitor-*.css`
- **通用样式**：`assets/css/common-*.css` 或 `assets/css/main.css`

### Composables

- `composables/useStyle.ts` - 通用样式管理
- `composables/useAdminStyle.ts` - 管理后台样式管理
- `composables/usePageStyle.ts` - 页面样式配置

### 管理页面

- **样式管理**：`/admin/settings/styles` - 管理系统样式
- **前端样式配置**：`/admin/settings/frontend-styles` - 配置前端页面样式

## 💡 快速参考

### 管理后台常用样式类

```css
/* 卡片 */
.admin-card
.admin-card-header
.admin-card-body

/* 按钮 */
.admin-button-primary
.admin-button-secondary
.admin-button-danger

/* 表格 */
.admin-table
.admin-table-header
.admin-table-row

/* 表单 */
.admin-form-input
.admin-form-label
.admin-form-error
```

### 访客页面常用样式类

```css
/* 卡片 */
.visitor-card
.visitor-card-header

/* 按钮 */
.visitor-button-primary
.visitor-button-secondary

/* 模态框 */
.visitor-modal
.visitor-modal-overlay
.visitor-modal-content
```

## ⚡ 快速检查命令

在提交代码前，请检查：

```bash
# 搜索内联样式（style="）
grep -r 'style="' pages/ components/ --include="*.vue"

# 搜索 :style 绑定（检查是否合理）
grep -r ':style=' pages/ components/ --include="*.vue"

# 检查是否使用了统一样式类
grep -r 'class="admin-' pages/admin/ --include="*.vue"
```

## 🎯 记住这个原则

**所有样式必须统一管理，禁止在 template 中使用内联样式（`:style` 除外，但需最小化使用）**

每次修改页面时，问自己：
1. 这个样式是否可以在其他地方复用？→ 提取到统一样式文件
2. 这个样式是否只在这个组件使用？→ 使用 `<style scoped>`
3. 这个样式是否需要动态计算？→ 使用 `:style` 绑定（最小化）
4. 这个样式是否可以配置？→ 使用样式管理系统

---

**最后更新**：2024年
**维护者**：开发团队

