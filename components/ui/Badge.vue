<template>
  <span
    class="badge"
    :class="[
      `badge--${variant}`,
      `badge--${size}`,
      { 'badge--dot': dot },
      { 'badge--rounded': rounded }
    ]"
  >
    <slot>
      <span v-if="!$slots.default" class="badge-content">{{ text }}</span>
    </slot>
  </span>
</template>

<script setup lang="ts">
export interface BadgeProps {
  text?: string
  variant?: 'default' | 'primary' | 'success' | 'warning' | 'error' | 'info'
  size?: 'small' | 'medium' | 'large'
  dot?: boolean
  rounded?: boolean
  max?: number
}

const props = withDefaults(defineProps<BadgeProps>(), {
  text: '',
  variant: 'default',
  size: 'medium',
  dot: false,
  rounded: false,
  max: 999
})
</script>

<style scoped>
.badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-md);
  font-weight: 500;
  white-space: nowrap;
}

/* 变体 */
.badge--default {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.badge--primary {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
}

.badge--success {
  background: var(--color-success);
  color: white;
}

.badge--warning {
  background: var(--color-warning);
  color: white;
}

.badge--error {
  background: var(--color-error);
  color: white;
}

.badge--info {
  background: var(--color-info);
  color: white;
}

/* 尺寸 */
.badge--small {
  font-size: var(--text-xs);
  padding: var(--spacing-xs) var(--spacing-sm);
  height: 20px;
}

.badge--medium {
  font-size: var(--text-sm);
  padding: var(--spacing-xs) var(--spacing-md);
  height: 24px;
}

.badge--large {
  font-size: var(--text-base);
  padding: var(--spacing-sm) var(--spacing-lg);
  height: 28px;
}

/* 点状 Badge */
.badge--dot {
  width: 10px;
  height: 10px;
  padding: 0;
  border-radius: var(--radius-full);
  min-width: 10px;
}

.badge--dot.badge--small {
  width: 8px;
  height: 8px;
}

.badge--dot.badge--large {
  width: 12px;
  height: 12px;
}

/* 圆角 Badge */
.badge--rounded {
  border-radius: var(--radius-full);
}

.badge-content {
  line-height: 1;
}
</style>
