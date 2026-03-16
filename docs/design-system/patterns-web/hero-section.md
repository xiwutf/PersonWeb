# HeroSection - 英雄区块组件

灵活的英雄区块组件，支持多种变体和自定义内容。

## 基础用法

```vue
<template>
  <HeroSection
    title="欢迎来到 Aurora"
    description="探索无限可能，创造非凡体验"
    :actions="heroActions"
    variant="gradient"
  />
</template>

<script setup lang="ts">
import HeroSection from '~/components/web/HeroSection.vue'

const heroActions = [
  {
    text: '开始探索',
    icon: 'fa-rocket',
    variant: 'primary',
    to: '/explore'
  },
  {
    text: '了解更多',
    icon: 'fa-info-circle',
    variant: 'secondary',
    to: '/about'
  }
]
</script>
```

## Props

| 属性 | 类型 | 默认值 | 说明 |
|------|------|----------|------|
| `badge` | `string` | - | 徽章文字 |
| `badge-variant` | `'info' \| 'success' \| 'warning' \| 'error'` | `'info'` | 徽章变体 |
| `title` | `string` | - | 标题文字 |
| `description` | `string` | - | 描述文字 |
| `actions` | `CTAAction[]` | `[]` | 操作按钮列表 |
| `variant` | `'default' \| 'gradient' \| 'dark' \| 'light'` | `'default'` | 组件变体 |
| `class` | `string` | - | 自定义样式类 |

## Slots

| 插槽名 | 说明 |
|---------|------|
| `badge` | 自定义徽章内容 |
| `visual` | 视觉元素区域 |
| `title` | 自定义标题内容 |
| `description` | 自定义描述内容 |
| `actions` | 自定义操作按钮区域 |

## CTAAction 类型

```typescript
interface CTAAction {
  text: string                                    // 按钮文字
  icon?: string                                  // 图标类名
  variant?: 'primary' | 'secondary' | 'outline'  // 按钮变体
  to?: string                                   // 链接地址
  onClick?: () => void                           // 点击回调
}
```

## 交互式示例

### 默认变体

```vue
<HeroSection
  title="欢迎来到我的网站"
  description="分享技术与生活，探索无限可能"
/>
```

### 渐变变体

```vue
<HeroSection
  title="构建未来"
  description="AI 驱动的智能应用开发"
  :actions="actions"
  variant="gradient"
/>
```

### 深色变体

```vue
<HeroSection
  title="黑夜模式"
  description="沉浸式深色主题体验"
  :actions="actions"
  variant="dark"
/>
```

### 带徽章

```vue
<HeroSection
  badge="NEW"
  badge-variant="success"
  title="全新功能"
  description="体验最新推出的功能特性"
  :actions="actions"
/>
```

### 自定义内容

```vue
<HeroSection>
  <template #visual>
    <div class="custom-visual">
      <img src="/hero-image.png" alt="Hero" />
    </div>
  </template>

  <template #title>
    <h1>自定义<span class="text-primary">标题</span></h1>
  </template>

  <template #actions>
    <button class="btn-primary">主要操作</button>
    <button class="btn-secondary">次要操作</button>
  </template>
</HeroSection>
```

## 变体对比

| 变体 | 背景 | 文字颜色 |
|------|------|-----------|
| `default` | `var(--color-bg-body)` | `var(--color-text-main)` |
| `gradient` | 线性渐变 | 白色 |
| `dark` | `var(--color-bg-dark)` | `var(--color-text-main)` |
| `light` | `var(--color-bg-elevated)` | `var(--color-text-main)` |

## 按钮变体

| 变体 | 样式 |
|------|------|
| `primary` | 主色调背景，白色文字 |
| `secondary` | 透明背景，主题色边框 |
| `outline` | 透明背景，白色半透明边框 |

## 样式定制

```vue
<HeroSection
  title="自定义英雄区块"
  class="custom-hero"
/>

<style scoped>
.custom-hero {
  /* 自定义英雄区块样式 */
}
</style>
```

## 使用场景

1. **首页顶部** - 网站首页的主要宣传区域
2. **产品页顶部** - 产品介绍和引导
3. **活动页顶部** - 活动宣传和入口

## 最佳实践

1. 标题简洁有力，不超过 20 个字
2. 描述清晰表达价值主张
3. 主要操作按钮放在最前面
4. 移动端适配布局，确保可读性

## 相关组件

- [FeatureGrid](./feature-grid.md)
- [CTASection](./cta-section.md)

## 源码位置

`components/web/HeroSection.vue`
