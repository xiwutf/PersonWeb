# CTASection - 行动号召区块组件

行动号召区块组件，用于引导用户进行特定操作。

## 基础用法

```vue
<template>
  <CTASection
    title="准备好开始了吗？"
    description="立即注册，开启您的创作之旅"
    :actions="ctaActions"
    variant="gradient"
  />
</template>

<script setup lang="ts">
import CTASection from '~/components/web/CTASection.vue'

const ctaActions = [
  {
    text: '立即注册',
    icon: 'fa-user-plus',
    variant: 'primary',
    to: '/register'
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
| `title` | `string` | - | 标题文字 |
| `description` | `string` | - | 描述文字 |
| `actions` | `CTAAction[]` | `[]` | 操作按钮列表 |
| `variant` | `'default' \| 'gradient' \| 'dark' \| 'light'` | `'default'` | 组件变体 |
| `class` | `string` | - | 自定义样式类 |

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
<CTASection
  title="开始使用"
  :actions="[
    { text: '免费试用', variant: 'primary', to: '/trial' },
    { text: '查看定价', variant: 'secondary', to: '/pricing' }
  ]"
/>
```

### 渐变变体

```vue
<CTASection
  title="准备好开始了吗？"
  description="立即加入，体验强大功能"
  :actions="actions"
  variant="gradient"
/>
```

### 深色变体

```vue
<CTASection
  title="订阅我们的新闻"
  :actions="[
    { text: '立即订阅', variant: 'primary' }
  ]"
  variant="dark"
/>
```

### 浅色变体

```vue
<CTASection
  title="联系我们要"
  description="有任何问题？欢迎随时联系我们"
  :actions="[
    { text: '发送消息', variant: 'primary' }
  ]"
  variant="light"
/>
```

### 轮廓按钮

```vue
<CTASection
  title="了解更多"
  :actions="[
    { text: '查看文档', variant: 'outline', to: '/docs' },
    { text: '联系我们', variant: 'outline', to: '/contact' }
  ]"
/>
```

### 多个操作

```vue
<CTASection
  title="准备好开始了吗？"
  :actions="[
    { text: '立即注册', icon: 'fa-user-plus', variant: 'primary', to: '/register' },
    { text: '免费试用', icon: 'fa-play', variant: 'secondary', to: '/trial' },
    { text: '联系销售', icon: 'fa-envelope', variant: 'outline', to: '/contact' }
  ]"
  variant="gradient"
/>
```

## 变体对比

| 变体 | 背景 | 文字颜色 |
|------|------|-----------|
| `default` | `var(--color-bg-body)` | `var(--color-text-main)` |
| `gradient` | 线性渐变（primary → purple） | 白色 |
| `dark` | `var(--color-bg-dark)` | `var(--color-text-main)` |
| `light` | `var(--color-bg-elevated)` | `var(--color-text-main)` |

## 按钮变体

| 变体 | 背景色 | 文字色 | 边框 |
|------|---------|---------|------|
| `primary` | `var(--color-primary)` | 白色 | 无 |
| `secondary` | 透明 | `var(--color-text-main)` | `var(--color-text-main)` |
| `outline` | 透明 | 白色 | 白色半透明 |

## 样式定制

```vue
<CTASection
  title="自定义 CTA"
  :actions="actions"
  class="custom-cta"
/>

<style scoped>
.custom-cta {
  /* 自定义 CTA 区块样式 */
}
</style>
```

## 使用场景

1. **页面底部** - 引导用户进行下一步操作
2. **功能介绍后** - 展示功能后引导试用
3. **注册转化** - 引导用户注册订阅

## 最佳实践

1. 标题简洁有力，不超过 15 个字
2. 描述清晰表达价值主张
3. 按钮不超过 3 个，主操作放在前面
4. 使用恰当的变体，与页面风格协调

## 相关组件

- [HeroSection](./hero-section.md)
- [FeatureGrid](./feature-grid.md)

## 源码位置

`components/web/CTASection.vue`
