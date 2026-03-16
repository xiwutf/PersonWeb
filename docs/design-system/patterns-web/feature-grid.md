# FeatureGrid - 功能网格组件

功能网格组件，以卡片形式展示功能特性。

## 基础用法

```vue
<template>
  <FeatureGrid
    :features="features"
    title="核心功能"
    description="强大的功能支持，满足各种需求"
    :columns="3"
  />
</template>

<script setup lang="ts">
import FeatureGrid from '~/components/web/FeatureGrid.vue'

const features = [
  {
    id: '1',
    title: '智能搜索',
    description: '基于 AI 的全文搜索，快速定位内容',
    icon: 'fa-search',
    iconColor: 'var(--color-primary)'
  },
  {
    id: '2',
    title: '实时同步',
    description: '多端数据实时同步，随时随地访问',
    icon: 'fa-sync',
    iconColor: 'var(--color-success)'
  },
  {
    id: '3',
    title: '安全可靠',
    description: '企业级安全保障，保护数据安全',
    icon: 'fa-shield-alt',
    iconColor: 'var(--color-warning)'
  }
]
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `features` | `Feature[]` | - | 功能列表 |
| `title` | `string` | - | 区域标题 |
| `subtitle` | `string` | - | 区域副标题 |
| `columns` | `2 \| 3 \| 4` | `3` | 列数 |
| `class` | `string` | - | 自定义样式类 |

## Feature 类型

```typescript
interface Feature {
  id: string              // 功能 ID
  title?: string          // 功能标题
  description?: string    // 功能描述
  icon?: string          // 图标类名
  iconColor?: string     // 图标颜色
  tags?: string[]        // 标签列表
  to?: string           // 链接地址
}
```

## 交互式示例

### 基础网格

```vue
<FeatureGrid
  :features="features"
  :columns="3"
/>
```

### 带标题和描述

```vue
<FeatureGrid
  title="产品特性"
  subtitle="为什么选择我们的产品"
  :features="features"
  :columns="3"
/>
```

### 两列布局

```vue
<FeatureGrid
  :features="features"
  :columns="2"
/>
```

### 四列布局

```vue
<FeatureGrid
  :features="features"
  :columns="4"
/>
```

### 带标签的功能

```vue
<FeatureGrid
  :features="[
    {
      id: '1',
      title: '智能分析',
      description: 'AI 驱动的数据分析',
      icon: 'fa-chart-line',
      tags: ['AI', '分析']
    },
    {
      id: '2',
      title: '可视化',
      description: '丰富的图表展示',
      icon: 'fa-chart-pie',
      tags: ['图表', '可视化']
    }
  ]"
  :columns="2"
/>
```

### 可点击功能

```vue
<FeatureGrid
  :features="[
    {
      id: '1',
      title: '功能详情',
      description: '点击查看详细信息',
      icon: 'fa-info-circle',
      to: '/features/detail'
    }
  ]"
  :columns="3"
/>
```

## 响应式布局

| 断点 | 列数 |
|--------|------|
| 移动端 | 1 列 |
| 平板端 | 2 列 |
| 桌面端 | 指定列数 |

## 样式定制

```vue
<FeatureGrid
  :features="features"
  class="custom-feature-grid"
/>

<style scoped>
.custom-feature-grid {
  /* 自定义功能网格样式 */
}
</style>
```

## 使用场景

1. **功能介绍** - 产品功能特性展示
2. **优势展示** - 产品优势和亮点
3. **特性列表** - 服务或平台特性

## 最佳实践

1. 功能描述简洁明了，不超过 50 个字
2. 使用语义化图标，提高可识别性
3. 卡片数量适中，建议 3-6 个
4. 移动端自动适配为单列

## 相关组件

- [HeroSection](./hero-section.md)
- [ProjectShowcase](./project-showcase.md)

## 源码位置

`components/web/FeatureGrid.vue`
