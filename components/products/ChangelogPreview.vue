<template>
  <section class="changelog-preview-section">
    <div class="changelog-preview-container">
      <div class="changelog-preview-header">
        <div class="changelog-preview-header-left">
          <p class="changelog-preview-kicker">Changelog</p>
          <h2 class="changelog-preview-title">持续迭代中的真实产品</h2>
          <p class="changelog-preview-subtitle">这不是一次性 demo，产品正在持续开发与更新中。</p>
        </div>
        <NuxtLink to="/changelog" class="changelog-preview-more-link">
          查看完整日志
          <i class="fas fa-arrow-right"></i>
        </NuxtLink>
      </div>

      <div class="changelog-preview-list">
        <div
          v-for="(item, index) in recentChangelog"
          :key="index"
          class="changelog-preview-item"
        >
          <div class="changelog-preview-item-head">
            <span class="changelog-preview-version">{{ item.version }}</span>
            <span
              class="changelog-preview-status"
              :class="`changelog-preview-status--${item.status}`"
            >
              {{ item.status === 'stable' ? '稳定版' : '内测版' }}
            </span>
            <span class="changelog-preview-date">{{ item.date }}</span>
          </div>
          <ul class="changelog-preview-changes">
            <li
              v-for="(change, ci) in item.changes"
              :key="ci"
              class="changelog-preview-change"
            >
              <i class="fas fa-circle-dot changelog-preview-bullet"></i>
              {{ change }}
            </li>
          </ul>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const recentChangelog = DESKTOP_PET.changelog.slice(0, 3)
</script>

<style scoped>
.changelog-preview-section {
  padding: 5rem 0;
}

.changelog-preview-container {
  max-width: 72rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.changelog-preview-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: var(--spacing-lg);
  margin-bottom: 2.5rem;
  flex-wrap: wrap;
}

.changelog-preview-kicker {
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-primary);
  margin: 0 0 var(--spacing-sm);
}

.changelog-preview-title {
  font-size: var(--font-size-h2);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.changelog-preview-subtitle {
  font-size: 0.95rem;
  color: var(--color-text-muted);
  margin: 0;
}

.changelog-preview-more-link {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: 0.9rem;
  color: var(--color-primary);
  text-decoration: none;
  white-space: nowrap;
  padding-bottom: 4px;
}

.changelog-preview-more-link:hover {
  opacity: 0.8;
}

.changelog-preview-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.changelog-preview-item {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg) var(--spacing-xl);
  transition: box-shadow 0.2s;
}

.changelog-preview-item:hover {
  box-shadow: var(--shadow-md);
}

.changelog-preview-item-head {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-md);
  flex-wrap: wrap;
}

.changelog-preview-version {
  font-size: 1rem;
  font-weight: 700;
  color: var(--color-text-main);
  font-family: monospace;
}

.changelog-preview-status {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: var(--radius-full);
}

.changelog-preview-status--stable {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

.changelog-preview-status--beta {
  background: rgba(234, 179, 8, 0.12);
  color: var(--color-yellow-600, #ca8a04);
}

.changelog-preview-date {
  font-size: 0.85rem;
  color: var(--color-text-muted);
  margin-left: auto;
}

.changelog-preview-changes {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.changelog-preview-change {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-sm);
  font-size: 0.9rem;
  color: var(--color-text-muted);
}

.changelog-preview-bullet {
  color: var(--color-primary);
  font-size: 0.5rem;
  margin-top: 6px;
  flex-shrink: 0;
}
</style>
