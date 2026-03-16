<template>
  <section class="cta-section" :class="`cta-section--${variant}`">
    <div class="cta-content">
      <h2 v-if="title" class="cta-title">
        {{ title }}
      </h2>
      <p v-if="description" class="cta-description">
        {{ description }}
      </p>
      <div class="cta-actions">
        <button
          v-for="(action, index) in actions"
          :key="index"
          :class="['cta-button', `cta-button-${action.variant || 'primary'}`]"
          @click="handleActionClick(action)"
        >
          <span class="cta-button-text">{{ action.text }}</span>
          <i v-if="action.icon" :class="action.icon" class="cta-button-icon"></i>
        </button>
      </div>
    </div>

    <!-- 背景装饰 -->
    <div class="cta-background"></div>
  </section>
</template>

<script setup lang="ts">
export interface CTAAction {
  text: string
  icon?: string
  variant?: 'primary' | 'secondary' | 'outline'
  to?: string
  onClick?: () => void
}

interface Props {
  title?: string
  description?: string
  actions?: CTAAction[]
  variant?: 'default' | 'gradient' | 'dark' | 'light'
  class?: string | string[] | Record<string, boolean>
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  actions: () => []
})

const emit = defineEmits<{
  (event: 'action-click', action: CTAAction): void
}>()

const handleActionClick = (action: CTAAction) => {
  if (action.to) {
    window.location.href = action.to
  }
  if (action.onClick) {
    action.onClick()
  }
  emit('action-click', action)
}
</script>

<style scoped>
.cta-section {
  position: relative;
  padding: var(--section-padding, var(--spacing-20) 0);
  text-align: center;
}

/* Default Variant */
.cta-section--default {
  background: var(--cta-bg, var(--color-bg-body));
  color: var(--cta-text, var(--color-text-main));
}

.cta-section--default .cta-title {
  color: var(--cta-title-color, var(--color-text-main));
}

/* Gradient Variant */
.cta-section--gradient {
  background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-purple-500) 100%);
  color: var(--color-white);
}

.cta-section--gradient .cta-title {
  color: var(--color-white);
}

.cta-section--gradient .cta-description {
  color: rgba(255, 255, 255, 0.9);
}

/* Dark Variant */
.cta-section--dark {
  background: var(--color-bg-dark);
  color: var(--color-text-main);
}

/* Light Variant */
.cta-section--light {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.cta-content {
  position: relative;
  z-index: 1;
  max-width: 800px;
  margin: 0 auto;
}

.cta-title {
  font-size: var(--cta-title-size, var(--text-3xl));
  font-weight: 700;
  line-height: 1.2;
  margin: 0 0 var(--spacing-md) 0;
}

.cta-description {
  font-size: var(--text-lg);
  color: var(--cta-description-color, var(--color-text-muted));
  line-height: 1.6;
  margin: 0 0 var(--spacing-2xl) 0;
}

.cta-actions {
  display: flex;
  justify-content: center;
  gap: var(--spacing-md);
  flex-wrap: wrap;
}

.cta-button {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md) var(--spacing-xl);
  border-radius: var(--radius-lg);
  font-size: var(--text-base);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  border: 2px solid transparent;
}

.cta-button-text {
  font-size: var(--text-base);
}

.cta-button-icon {
  font-size: var(--text-lg);
}

/* Primary Button */
.cta-button-primary {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
}

.cta-button-primary:hover {
  background: var(--color-primary-hover);
  transform: translateY(-2px);
  box-shadow: var(--shadow-lg);
}

/* Secondary Button */
.cta-button-secondary {
  background: transparent;
  color: var(--color-text-main);
  border-color: var(--color-text-main);
}

.cta-button-secondary:hover {
  background: var(--color-bg-elevated);
}

/* Outline Button */
.cta-button-outline {
  background: transparent;
  color: var(--color-text-main);
  border-color: rgba(255, 255, 255, 0.3);
}

.cta-button-outline:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.5);
}

.cta-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 0;
  opacity: 0.1;
  background-image: url("data:image/svg+xml,%3Csvg width='100' height='100' viewBox='0 0 100 100' xmlns='http://www.w3.org/2000/svg'%3E%3Cdefs%3E%3Cpattern id='grid' width='10' height='10' patternUnits='userSpaceOnUse'%3E%3Cpath d='M 10 0 L 0 0 0 10' fill='none' stroke='currentColor' stroke-width='0.5'/%3E%3C/pattern%3E%3C/defs%3E%3Crect width='100' height='100' fill='url(%23grid)' /%3E%3C/svg%3E");
  background-size: 20px 20px;
  background-position: center;
}

/* Responsive */
@media (max-width: 768px) {
  .cta-section {
    padding: var(--spacing-12) 0;
  }

  .cta-title {
    font-size: var(--text-2xl);
  }

  .cta-description {
    font-size: var(--text-base);
  }

  .cta-actions {
    flex-direction: column;
    width: 100%;
  }

  .cta-button {
    width: 100%;
    justify-content: center;
  }
}
</style>
