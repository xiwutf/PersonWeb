<template>
  <div
    :class="[
      'app-card',
      {
        'app-card--hover': hover,
        'app-card--elevated': elevated
      }
    ]"
  >
    <slot />
  </div>
</template>

<script setup lang="ts">
/**
 * AppCard 基础卡片组件
 * 
 * 职责：
 * - 提供统一的卡片样式，使用主题变量控制外观
 * - 支持 hover 和 elevated 两种变体
 * - 所有样式通过 Tailwind 类 + CSS 变量实现，自动适配当前主题
 * 
 * 为什么要统一使用这个组件？
 * - 避免在业务组件中随意拼 Tailwind 类，导致样式不一致
 * - 集中管理卡片样式，后续修改主题时只需改这里
 * - 保证所有卡片在不同主题下都能正确显示
 */

interface Props {
  /** 是否启用 hover 效果 */
  hover?: boolean
  /** 是否使用 elevated 背景（更明显的层次感） */
  elevated?: boolean
}

withDefaults(defineProps<Props>(), {
  hover: false,
  elevated: false
})
</script>

<style scoped>
.app-card {
  /* 使用主题变量控制背景、边框、圆角、阴影 */
  /* Aurora DS: 增加 backdrop-blur 实现毛玻璃效果 */
  @apply bg-bg-card border border-border-subtle rounded-2xl shadow-sm p-6 transition-all duration-500 backdrop-blur-xl;
  /* 确保在深色模式下有细腻的内描边 */
  box-shadow: var(--shadow-sm);
}

.app-card--hover {
  /* hover 时提升阴影和轻微上移，增强交互感 */
  @apply cursor-pointer;
}

.app-card--hover:hover {
  @apply -translate-y-1;
  /* 使用主题变量中的 hover 阴影 (光晕) */
  box-shadow: var(--shadow-lg), var(--shadow-glow);
  border-color: var(--border-focus);
}

.app-card--elevated {
  /* 使用 elevated 背景色，提供更明显的层次感 */
  @apply bg-bg-elevated shadow-md;
}
</style>

