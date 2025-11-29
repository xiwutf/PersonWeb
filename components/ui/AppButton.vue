<template>
  <button
    :type="type"
    :disabled="disabled"
    :class="[
      'app-button',
      `app-button--${variant}`,
      `app-button--${size}`,
      {
        'app-button--disabled': disabled,
        'app-button--full-width': fullWidth
      }
    ]"
    @click="$emit('click', $event)"
  >
    <slot />
  </button>
</template>

<script setup lang="ts">
/**
 * AppButton 基础按钮组件
 * 
 * 职责：
 * - 提供统一的按钮样式，使用主题变量控制外观
 * - 支持多种变体（primary、secondary）和尺寸（sm、md、lg）
 * - 所有样式通过 Tailwind 类 + CSS 变量实现，自动适配当前主题
 * 
 * 为什么要统一使用这个组件？
 * - 避免在业务组件中随意拼 Tailwind 类，导致按钮样式不一致
 * - 集中管理按钮样式，后续修改主题时只需改这里
 * - 保证所有按钮在不同主题下都能正确显示，包括 hover、active、disabled 状态
 */

interface Props {
  /** 按钮变体 */
  variant?: 'primary' | 'secondary' | 'ghost'
  /** 按钮尺寸 */
  size?: 'sm' | 'md' | 'lg'
  /** 是否禁用 */
  disabled?: boolean
  /** 是否全宽 */
  fullWidth?: boolean
  /** 按钮类型 */
  type?: 'button' | 'submit' | 'reset'
}

withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  disabled: false,
  fullWidth: false,
  type: 'button'
})

defineEmits<{
  click: [event: MouseEvent]
}>()
</script>

<style scoped>
/* ==================== 基础样式 ==================== */
.app-button {
  @apply font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2;
  /* 使用主题变量控制圆角 */
  border-radius: var(--radius-md);
}

/* ==================== 尺寸变体 ==================== */
.app-button--sm {
  @apply px-3 py-1.5 text-sm;
}

.app-button--md {
  @apply px-4 py-2 text-base;
}

.app-button--lg {
  @apply px-6 py-3 text-lg;
}

/* ==================== 主要按钮（Primary） ==================== */
.app-button--primary {
  @apply text-white;
  /* 使用主题变量控制背景色和 hover 背景色 */
  background-color: var(--color-primary);
  border: 1px solid var(--color-primary);
}

.app-button--primary:hover:not(.app-button--disabled) {
  background-color: var(--color-primary-hover);
  border-color: var(--color-primary-hover);
  /* 使用主题变量控制阴影 */
  box-shadow: var(--shadow-md);
  @apply -translate-y-0.5;
}

.app-button--primary:active:not(.app-button--disabled) {
  @apply translate-y-0;
  box-shadow: var(--shadow-sm);
}

/* ==================== 次要按钮（Secondary） ==================== */
.app-button--secondary {
  /* 使用主题变量控制文字、背景、边框 */
  color: var(--color-text-main);
  background-color: var(--color-bg-card);
  border: 1px solid var(--color-border-default);
}

.app-button--secondary:hover:not(.app-button--disabled) {
  background-color: var(--color-bg-elevated);
  border-color: var(--color-border-strong);
  box-shadow: var(--shadow-sm);
  @apply -translate-y-0.5;
}

.app-button--secondary:active:not(.app-button--disabled) {
  @apply translate-y-0;
}

/* ==================== 幽灵按钮（Ghost） ==================== */
.app-button--ghost {
  /* 透明背景，只有文字和边框 */
  color: var(--color-text-main);
  background-color: transparent;
  border: 1px solid transparent;
}

.app-button--ghost:hover:not(.app-button--disabled) {
  background-color: var(--color-primary-soft);
  color: var(--color-primary);
}

.app-button--ghost:active:not(.app-button--disabled) {
  background-color: var(--color-primary-soft);
}

/* ==================== 禁用状态 ==================== */
.app-button--disabled {
  @apply cursor-not-allowed opacity-50;
  color: var(--color-text-disabled);
  background-color: var(--color-bg-elevated);
  border-color: var(--color-border-subtle);
}

/* ==================== 全宽按钮 ==================== */
.app-button--full-width {
  @apply w-full;
}

/* ==================== 焦点样式 ==================== */
.app-button:focus {
  /* 使用主题变量控制焦点环颜色 */
  --tw-ring-color: var(--color-primary);
}
</style>

