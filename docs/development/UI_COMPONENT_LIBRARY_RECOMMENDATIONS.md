# UI 组件库和主题色推荐

## 📚 当前使用的组件库

### Naive UI
- **状态**: ✅ 已安装并使用
- **版本**: `^2.43.2`
- **特点**:
  - 完整的 Vue 3 支持
  - TypeScript 友好
  - 内置深色模式支持
  - 丰富的组件库（表格、表单、数据展示等）
  - 可自定义主题色
- **文档**: https://www.naiveui.com/

## 🎨 推荐的主题色组件库

### 1. **Naive UI**（当前使用）⭐ 推荐
```bash
npm install naive-ui
```

**优势**:
- ✅ 已集成，无需额外安装
- 完整的 TypeScript 支持
- 内置深色模式
- 主题色可自定义
- 组件丰富，文档完善

**主题色配置示例**:
```typescript
import { create, NConfigProvider } from 'naive-ui'

const naive = create({
  components: [/* ... */]
})

// 自定义主题色
const themeOverrides = {
  common: {
    primaryColor: '#3b82f6', // 主色
    primaryColorHover: '#2563eb',
    primaryColorPressed: '#1d4ed8'
  }
}
```

### 2. **Element Plus**
```bash
npm install element-plus
```

**优势**:
- 企业级组件库
- 主题色系统完善
- 中文文档齐全
- 社区活跃

**主题色配置**:
```typescript
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import 'element-plus/theme-chalk/dark/css-vars.css' // 深色模式
```

### 3. **Ant Design Vue**
```bash
npm install ant-design-vue
```

**优势**:
- 设计规范完善
- 主题定制能力强
- 企业级应用广泛使用

**主题色配置**:
```typescript
import { ConfigProvider } from 'ant-design-vue'

// 自定义主题
const theme = {
  token: {
    colorPrimary: '#3b82f6',
  },
}
```

### 4. **Arco Design Vue**
```bash
npm install @arco-design/web-vue
```

**优势**:
- 字节跳动开源
- 设计精美
- 主题系统灵活

## 🎯 ECharts 主题配置

### 已实现的深色/浅色主题切换

项目已创建 `composables/useEChartsTheme.ts`，自动根据当前主题切换 ECharts 图表样式。

**使用方式**:
```typescript
import { useEChartsTheme } from '~/composables/useEChartsTheme'

const { isDark, currentTheme, applyTheme } = useEChartsTheme()

// 在图表配置中使用
const chartOption = computed(() => {
  const baseOption = {
    // ... 图表配置
  }
  return applyTheme(baseOption)
})
```

**主题特点**:
- ✅ 自动检测深色/浅色模式
- ✅ 文字颜色对比度优化
- ✅ 工具提示背景适配
- ✅ 坐标轴颜色适配
- ✅ 网格线颜色适配

## 🎨 推荐的主题色方案

### 1. 蓝色系（当前使用）
```css
--primary: #3b82f6;  /* 蓝色 */
--primary-hover: #2563eb;
--primary-pressed: #1d4ed8;
```
**适用场景**: 科技感、专业、可信赖

### 2. 绿色系
```css
--primary: #10b981;  /* 绿色 */
--primary-hover: #059669;
--primary-pressed: #047857;
```
**适用场景**: 环保、健康、成长

### 3. 紫色系
```css
--primary: #8b5cf6;  /* 紫色 */
--primary-hover: #7c3aed;
--primary-pressed: #6d28d9;
```
**适用场景**: 创意、艺术、高端

### 4. 橙色系
```css
--primary: #f59e0b;  /* 橙色 */
--primary-hover: #d97706;
--primary-pressed: #b45309;
```
**适用场景**: 活力、温暖、友好

### 5. 红色系
```css
--primary: #ef4444;  /* 红色 */
--primary-hover: #dc2626;
--primary-pressed: #b91c1c;
```
**适用场景**: 紧急、重要、警示

## 📝 使用建议

### 1. 保持一致性
- 在整个应用中使用统一的主题色
- 使用 CSS 变量管理颜色
- 确保深色/浅色模式都有良好的对比度

### 2. 可访问性
- 文字与背景对比度至少 4.5:1（WCAG AA 标准）
- 重要信息使用更高对比度（7:1，WCAG AAA 标准）
- 避免仅使用颜色传达信息

### 3. 组件库选择
- **当前项目**: 继续使用 Naive UI（已集成，功能完善）
- **如需扩展**: 可以考虑 Element Plus 或 Ant Design Vue
- **图表**: 使用 ECharts（已配置主题切换）

## 🔧 快速开始

### 使用 Naive UI 主题色
```vue
<template>
  <n-config-provider :theme="theme">
    <!-- 你的组件 -->
  </n-config-provider>
</template>

<script setup>
import { computed } from 'vue'
import { darkTheme } from 'naive-ui'

const isDark = computed(() => {
  return document.documentElement.classList.contains('dark')
})

const theme = computed(() => {
  return isDark.value ? darkTheme : null
})
</script>
```

### 使用 ECharts 主题
```vue
<template>
  <v-chart :option="chartOption" autoresize />
</template>

<script setup>
import { useEChartsTheme } from '~/composables/useEChartsTheme'

const { applyTheme } = useEChartsTheme()

const chartOption = computed(() => {
  return applyTheme({
    // 你的图表配置
  })
})
</script>
```

## 📚 相关资源

- [Naive UI 文档](https://www.naiveui.com/)
- [Element Plus 文档](https://element-plus.org/)
- [Ant Design Vue 文档](https://antdv.com/)
- [ECharts 主题编辑器](https://echarts.apache.org/theme-builder.html)
- [WCAG 对比度检查器](https://webaim.org/resources/contrastchecker/)

---

**最后更新**: 2024年
**维护者**: 开发团队

