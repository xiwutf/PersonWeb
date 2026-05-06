<template>
  <div class="changelog-page">
    <div class="changelog-page-container">

      <div class="changelog-page-header">
        <p class="changelog-page-kicker">Changelog</p>
        <h1 class="changelog-page-title">更新日志</h1>
        <p class="changelog-page-subtitle">产品持续迭代，记录每一次成长。</p>
      </div>

      <div class="changelog-page-list">
        <div
          v-for="(item, index) in changelog"
          :key="index"
          class="changelog-page-item"
        >
          <div class="changelog-page-item-head">
            <div class="changelog-page-item-left">
              <span class="changelog-page-version">{{ item.version }}</span>
              <span
                class="changelog-page-status"
                :class="`changelog-page-status--${item.status}`"
              >
                {{ item.status === 'stable' ? '稳定版' : '内测版' }}
              </span>
            </div>
            <span class="changelog-page-date">{{ item.date }}</span>
          </div>

          <ul class="changelog-page-changes">
            <li
              v-for="(change, ci) in item.changes"
              :key="ci"
              class="changelog-page-change"
            >
              <i class="fas fa-circle-check changelog-page-check-icon"></i>
              {{ change }}
            </li>
          </ul>
        </div>
      </div>

      <div class="changelog-page-back">
        <NuxtLink to="/products/desktop-pet" class="changelog-page-back-link">
          <i class="fas fa-arrow-left"></i>
          返回产品介绍
        </NuxtLink>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const changelog = DESKTOP_PET.changelog

useHead({
  title: 'AI 桌面陪伴助手 - 更新日志',
  meta: [
    {
      name: 'description',
      content: 'AI 桌面陪伴助手版本更新历史记录，持续迭代中。'
    }
  ]
})
</script>

<style scoped>
.changelog-page {
  padding: 4rem 0 6rem;
}

.changelog-page-container {
  max-width: 56rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.changelog-page-header {
  text-align: center;
  margin-bottom: 3rem;
}

.changelog-page-kicker {
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-primary);
  margin: 0 0 var(--spacing-sm);
}

.changelog-page-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md);
}

.changelog-page-subtitle {
  font-size: 1rem;
  color: var(--color-text-muted);
  margin: 0;
}

.changelog-page-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  margin-bottom: 3rem;
}

.changelog-page-item {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  padding: var(--spacing-xl);
}

.changelog-page-item-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--spacing-lg);
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.changelog-page-item-left {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.changelog-page-version {
  font-size: 1.1rem;
  font-weight: 700;
  color: var(--color-text-main);
  font-family: monospace;
}

.changelog-page-status {
  font-size: 0.75rem;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: var(--radius-full);
}

.changelog-page-status--stable {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

.changelog-page-status--beta {
  background: rgba(234, 179, 8, 0.12);
  color: var(--color-yellow-600, #ca8a04);
}

.changelog-page-date {
  font-size: 0.85rem;
  color: var(--color-text-muted);
}

.changelog-page-changes {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.changelog-page-change {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
  font-size: 0.95rem;
  color: var(--color-text-muted);
  line-height: 1.5;
}

.changelog-page-check-icon {
  color: var(--color-primary);
  font-size: 0.85rem;
  margin-top: 3px;
  flex-shrink: 0;
}

.changelog-page-back {
  text-align: center;
}

.changelog-page-back-link {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: 0.9rem;
  color: var(--color-text-muted);
  text-decoration: none;
  transition: color 0.2s;
}

.changelog-page-back-link:hover {
  color: var(--color-primary);
}
</style>
