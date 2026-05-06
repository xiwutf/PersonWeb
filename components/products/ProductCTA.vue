<template>
  <section class="product-cta-section">
    <div class="product-cta-container">
      <div class="product-cta-inner">
        <h2 class="product-cta-title">让你的桌面，多一个懂你的搭子</h2>
        <p class="product-cta-desc">现在开始体验一款真正能每天开着用的桌面陪伴助手。</p>
        <div class="product-cta-actions">
          <NuxtLink to="/download" class="product-cta-btn-primary">
            <i class="fas fa-download"></i>
            立即下载体验
          </NuxtLink>
          <a href="mailto:linxiwanting@gmail.com" class="product-cta-btn-secondary">
            <i class="fas fa-envelope"></i>
            联系作者反馈建议
          </a>
        </div>
        <p class="product-cta-slogan">{{ slogan }}</p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const slogan = DESKTOP_PET.slogan
</script>

<style scoped>
.product-cta-section {
  padding: 5rem 0;
  background: var(--color-bg-elevated);
}

.product-cta-container {
  max-width: 56rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.product-cta-inner {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-2xl, 24px);
  padding: 3rem var(--spacing-xl);
  text-align: center;
  box-shadow: var(--shadow-md);
}

.product-cta-title {
  font-size: var(--font-size-h2);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-lg);
}

.product-cta-desc {
  font-size: 1rem;
  color: var(--color-text-muted);
  line-height: 1.7;
  margin: 0 0 var(--spacing-xl);
}

.product-cta-actions {
  display: flex;
  justify-content: center;
  gap: var(--spacing-md);
  flex-wrap: wrap;
  margin-bottom: var(--spacing-xl);
}

.product-cta-btn-primary,
.product-cta-btn-secondary {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: 12px 28px;
  border-radius: var(--radius-lg);
  font-size: 0.95rem;
  font-weight: 500;
  text-decoration: none;
  transition: opacity 0.2s, transform 0.2s;
}

.product-cta-btn-primary {
  background: var(--color-primary);
  color: #fff;
}

.product-cta-btn-primary:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.product-cta-btn-secondary {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.product-cta-btn-secondary:hover {
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.product-cta-slogan {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  font-style: italic;
  margin: 0;
}
</style>
