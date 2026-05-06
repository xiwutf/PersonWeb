<template>
  <section id="features" class="feature-grid-section">
    <div class="feature-grid-container">
      <div class="feature-grid-header">
        <p class="feature-grid-kicker">Core Features</p>
        <h2 class="feature-grid-title">真正能每天开着用的桌面工具</h2>
        <p class="feature-grid-subtitle">陪伴感 + 专注感 + AI 工具感，集中在一个轻量桌面助手里。</p>
      </div>

      <div class="feature-grid-items">
        <div
          v-for="(item, index) in features"
          :key="index"
          class="feature-grid-card"
        >
          <div class="feature-grid-icon">
            <i :class="item.icon"></i>
          </div>
          <h3 class="feature-grid-card-title">{{ item.title }}</h3>
          <p class="feature-grid-card-desc">{{ item.description }}</p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const features = DESKTOP_PET.features
</script>

<style scoped>
.feature-grid-section {
  padding: 5rem 0;
  background: var(--color-bg-elevated);
}

.feature-grid-container {
  max-width: 72rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.feature-grid-header {
  text-align: center;
  margin-bottom: 3rem;
}

.feature-grid-kicker {
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-primary);
  margin: 0 0 var(--spacing-sm);
}

.feature-grid-title {
  font-size: var(--font-size-h2);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md);
}

.feature-grid-subtitle {
  font-size: 1rem;
  color: var(--color-text-muted);
  margin: 0;
}

.feature-grid-items {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--spacing-lg);
}

.feature-grid-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  padding: var(--spacing-xl);
  transition: box-shadow 0.2s, border-color 0.2s;
}

.feature-grid-card:hover {
  box-shadow: var(--shadow-md);
  border-color: var(--color-border-default);
}

.feature-grid-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-lg);
  background: var(--color-primary-soft);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  color: var(--color-primary);
  margin-bottom: var(--spacing-lg);
}

.feature-grid-card-title {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.feature-grid-card-desc {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
}

@media (max-width: 640px) {
  .feature-grid-items {
    grid-template-columns: 1fr;
  }
}
</style>
