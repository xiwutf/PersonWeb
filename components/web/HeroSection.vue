<template>
  <section class="hero-section" :class="[`variant-${variant}`, props.class]" :style="props.style">
    <div class="hero-container">
      <div class="hero-content">
        <!-- 徽章 -->
        <div v-if="badge" class="hero-badge">
          {{ badge }}
        </div>

        <!-- 标题 -->
        <component
          :is="headingTag"
          class="hero-title"
        >
          {{ title }}
        </component>

        <!-- 副标题 -->
        <p v-if="subtitle" class="hero-subtitle">
          {{ subtitle }}
        </p>

        <!-- 描述 -->
        <p v-if="description" class="hero-description">
          {{ description }}
        </p>

        <!-- 操作按钮 -->
        <div v-if="actions && actions.length" class="hero-actions">
          <button
            v-for="(action, index) in actions"
            :key="index"
            :class="['hero-action', `hero-action-${action.variant || 'primary'}`]"
            @click="handleActionClick(action)"
          >
            <i v-if="action.icon" :class="action.icon" class="action-icon"></i>
            <span class="action-text">{{ action.text }}</span>
          </button>
        </div>
      </div>

      <!-- 视觉元素 -->
      <div v-if="visual" class="hero-visual">
        <img
          :src="visual"
          :alt="title"
          class="hero-image"
        />
      </div>
    </div>

    <!-- 背景装饰 -->
    <div v-if="showBackground" class="hero-background"></div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface HeroAction {
  text: string
  icon?: string
  variant?: 'primary' | 'secondary' | 'outline'
  to?: string
  onClick?: () => void
}

interface Props {
  title: string
  subtitle?: string
  description?: string
  badge?: string
  level?: 'h1' | 'h2' | 'h3'
  variant?: 'default' | 'gradient' | 'dark'
  actions?: HeroAction[]
  visual?: string
  showBackground?: boolean
  class?: string | string[] | Record<string, boolean>
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  level: 'h1',
  variant: 'default',
  showBackground: true
})

const emit = defineEmits<{
  (event: 'action-click', action: HeroAction): void
}>()

const headingTag = computed(() => props.level)

const handleActionClick = (action: HeroAction) => {
  if (action.onClick) {
    action.onClick()
  }
  emit('action-click', action)
}
</script>

<style scoped>
.hero-section {
  position: relative;
  padding: var(--hero-padding, var(--spacing-20) 0);
  min-height: 500px;
  display: flex;
  align-items: center;
}

/* Default Variant */
.variant-default {
  background: var(--hero-bg, var(--color-bg-body));
  color: var(--hero-text, var(--color-text-main));
}

.variant-default .hero-title {
  color: var(--hero-title-color, var(--color-text-main));
}

/* Gradient Variant */
.variant-gradient {
  background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-purple-500) 100%);
  color: var(--color-white);
}

.variant-gradient .hero-title {
  color: var(--color-white);
}

.variant-gradient .hero-subtitle,
.variant-gradient .hero-description {
  color: rgba(255, 255, 255, 0.9);
}

/* Dark Variant */
.variant-dark {
  background: var(--color-bg-dark);
  color: var(--color-text-main);
}

.hero-container {
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 var(--spacing-4);
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-8);
  align-items: center;
}

.hero-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  padding: var(--spacing-xs) var(--spacing-sm);
  background: var(--color-primary);
  color: var(--color-white);
  border-radius: var(--radius-full);
  font-size: var(--text-xs);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  align-self: flex-start;
}

.hero-title {
  font-size: var(--hero-title-size, var(--text-4xl));
  font-weight: 700;
  line-height: 1.2;
  margin: 0;
}

.hero-subtitle {
  font-size: var(--text-xl);
  font-weight: 500;
  color: var(--hero-subtitle-color, var(--color-text-sec));
  margin: 0;
}

.hero-description {
  font-size: var(--text-base);
  color: var(--hero-description-color, var(--color-text-muted));
  line-height: 1.6;
  margin: 0;
  max-width: 600px;
}

.hero-actions {
  display: flex;
  gap: var(--spacing-md);
  margin-top: var(--spacing-lg);
  flex-wrap: wrap;
}

.hero-action {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-md) var(--spacing-xl);
  border-radius: var(--radius-lg);
  font-size: var(--text-base);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.action-icon {
  font-size: var(--text-lg);
}

.action-text {
  font-size: var(--text-base);
}

/* Primary Button */
.hero-action-primary {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
}

.hero-action-primary:hover {
  background: var(--color-primary-hover);
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

/* Secondary Button */
.hero-action-secondary {
  background: var(--color-bg-card);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.hero-action-secondary:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-strong);
}

/* Outline Button */
.hero-action-outline {
  background: transparent;
  color: var(--color-text-main);
  border: 2px solid var(--color-border-default);
}

.hero-action-outline:hover {
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.hero-visual {
  display: flex;
  justify-content: center;
  align-items: center;
}

.hero-image {
  max-width: 100%;
  height: auto;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-xl);
}

.hero-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: -1;
  opacity: 0.1;
  background-image: url("data:image/svg+xml,%3Csvg width='100' height='100' viewBox='0 0 100 100' xmlns='http://www.w3.org/2000/svg'%3E%3Cdefs%3E%3Cpattern id='grid' width='10' height='10' patternUnits='userSpaceOnUse'%3E%3Cpath d='M 10 0 L 0 0 0 10' fill='none' stroke='currentColor' stroke-width='0.5'/%3E%3C/pattern%3E%3C/defs%3E%3Crect width='100' height='100' fill='url(%23grid)' /%3E%3C/svg%3E");
  background-size: 10px 10px;
  background-position: center;
}

/* Responsive */
@media (max-width: 1024px) {
  .hero-container {
    grid-template-columns: 1fr;
    text-align: center;
  }

  .hero-content {
    align-items: center;
  }

  .hero-badge {
    align-self: center;
  }

  .hero-actions {
    justify-content: center;
  }

  .hero-visual {
    order: -1;
  }
}

@media (max-width: 640px) {
  .hero-section {
    padding: var(--spacing-12) 0;
    min-height: 400px;
  }

  .hero-container {
    padding: 0 var(--spacing-3);
  }

  .hero-title {
    font-size: var(--text-3xl);
  }

  .hero-subtitle {
    font-size: var(--text-lg);
  }

  .hero-actions {
    flex-direction: column;
    width: 100%;
  }

  .hero-action {
    width: 100%;
    justify-content: center;
  }
}
</style>
