<template>
  <section class="feature-section">
    <!-- 区域头部 -->
    <div v-if="title || subtitle" class="section-header">
      <h2 v-if="title" class="section-title">
        {{ title }}
      </h2>
      <p v-if="subtitle" class="section-subtitle">
        {{ subtitle }}
      </p>
    </div>

    <!-- 功能网格 -->
    <div class="feature-grid" :class="`feature-grid--${props.columns}`">
      <div
        v-for="feature in features"
        :key="feature.id"
        class="feature-card"
      >
        <!-- 图标 -->
        <div v-if="feature.icon" class="feature-icon" :style="{ color: feature.iconColor }">
          <i :class="feature.icon"></i>
        </div>

        <!-- 内容 -->
        <div class="feature-content">
          <h3 v-if="feature.title" class="feature-title">
            {{ feature.title }}
          </h3>
          <p v-if="feature.description" class="feature-description">
            {{ feature.description }}
          </p>
          <div v-if="feature.tags && feature.tags.length" class="feature-tags">
            <span
              v-for="tag in feature.tags"
              :key="tag"
              class="feature-tag"
            >
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
export interface Feature {
  id: string
  title?: string
  description?: string
  icon?: string
  iconColor?: string
  tags?: string[]
  to?: string
}

interface Props {
  features: Feature[]
  title?: string
  subtitle?: string
  columns?: 2 | 3 | 4
  class?: string | string[] | Record<string, boolean>
}

const props = withDefaults(defineProps<Props>(), {
  columns: 3
})
</script>

<style scoped>
.feature-section {
  padding: var(--section-padding, var(--spacing-20) 0);
}

.section-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.section-title {
  font-size: var(--text-3xl);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.section-subtitle {
  font-size: var(--text-lg);
  color: var(--color-text-muted);
  margin: 0;
}

.feature-grid {
  display: grid;
  gap: var(--spacing-lg);
}

.feature-grid--2 {
  grid-template-columns: repeat(2, 1fr);
}

.feature-grid--3 {
  grid-template-columns: repeat(3, 1fr);
}

.feature-grid--4 {
  grid-template-columns: repeat(4, 1fr);
}

.feature-card {
  background: var(--feature-card-bg, var(--color-bg-card));
  border: 1px solid var(--feature-card-border, var(--color-border-subtle));
  border-radius: var(--radius-xl);
  padding: var(--spacing-2xl);
  transition: all 0.3s;
  display: flex;
  gap: var(--spacing-lg);
}

.feature-card:hover {
  background: var(--feature-card-hover, var(--color-bg-elevated));
  border-color: var(--color-primary);
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.feature-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 64px;
  height: 64px;
  border-radius: var(--radius-xl);
  background: var(--feature-icon-bg, rgba(255, 255, 255, 0.1));
  font-size: var(--text-3xl);
  flex-shrink: 0;
}

.feature-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.feature-title {
  font-size: var(--text-xl);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.feature-description {
  font-size: var(--text-base);
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
}

.feature-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-xs);
  margin-top: var(--spacing-md);
}

.feature-tag {
  display: inline-flex;
  align-items: center;
  padding: var(--spacing-xs) var(--spacing-sm);
  background: var(--feature-tag-bg, var(--color-bg-elevated));
  color: var(--feature-tag-color, var(--color-text-sec));
  border-radius: var(--radius-full);
  font-size: var(--text-sm);
  font-weight: 500;
}

/* Responsive */
@media (max-width: 1024px) {
  .feature-grid--3,
  .feature-grid--4 {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .feature-grid--2,
  .feature-grid--3,
  .feature-grid--4 {
    grid-template-columns: 1fr;
  }

  .feature-card {
    flex-direction: column;
    text-align: center;
  }

  .feature-tags {
    justify-content: center;
  }
}
</style>
