<template>
  <section class="product-hero">
    <div class="product-hero-container">
      <div class="product-hero-content">
        <div class="product-hero-badge">
          <span class="product-hero-badge-dot"></span>
          <span>桌面效率工具 · 内测中</span>
        </div>

        <h1 class="product-hero-title">{{ data.tagline }}</h1>
        <p class="product-hero-desc">{{ data.description }}</p>

        <div class="product-hero-actions">
          <NuxtLink to="/download" class="product-hero-btn-primary">
            <i class="fas fa-download"></i>
            立即下载体验
          </NuxtLink>
          <NuxtLink to="#features" class="product-hero-btn-secondary">
            查看功能介绍
          </NuxtLink>
          <NuxtLink to="/pricing" class="product-hero-btn-ghost">
            Pro 内测申请
          </NuxtLink>
        </div>
      </div>

      <div class="product-hero-visual">
        <div class="product-hero-mockup">
          <div class="product-hero-mockup-screen">
            <div class="product-hero-mockup-bar">
              <span class="product-hero-mockup-dot"></span>
              <span class="product-hero-mockup-dot"></span>
              <span class="product-hero-mockup-dot"></span>
            </div>
            <div class="product-hero-mockup-body">
              <div class="product-hero-placeholder-icon">
                <i class="fas fa-cat"></i>
              </div>
              <p class="product-hero-placeholder-label">桌面陪伴助手</p>
              <p class="product-hero-placeholder-sub">截图即将更新</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const data = DESKTOP_PET
</script>

<style scoped>
.product-hero {
  padding: var(--spacing-xl) 0;
  padding-top: 4rem;
}

.product-hero-container {
  max-width: 72rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-xl);
  align-items: center;
}

.product-hero-badge {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: 6px var(--spacing-md);
  background: var(--color-primary-soft);
  border: 1px solid var(--color-primary-20);
  border-radius: var(--radius-full);
  font-size: 0.8rem;
  color: var(--color-primary);
  margin-bottom: var(--spacing-lg);
}

.product-hero-badge-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--color-primary);
  animation: pulse-dot 2s infinite;
}

@keyframes pulse-dot {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}

.product-hero-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  line-height: 1.2;
  margin: 0 0 var(--spacing-lg);
}

.product-hero-desc {
  font-size: 1.1rem;
  color: var(--color-text-muted);
  line-height: 1.7;
  margin: 0 0 var(--spacing-xl);
}

.product-hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-md);
  align-items: center;
}

.product-hero-btn-primary,
.product-hero-btn-secondary,
.product-hero-btn-ghost {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: 10px 24px;
  border-radius: var(--radius-lg);
  font-size: 0.95rem;
  font-weight: 500;
  text-decoration: none;
  transition: opacity 0.2s, transform 0.2s;
  cursor: pointer;
}

.product-hero-btn-primary {
  background: var(--color-primary);
  color: #fff;
}

.product-hero-btn-primary:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.product-hero-btn-secondary {
  background: var(--color-bg-card);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.product-hero-btn-secondary:hover {
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.product-hero-btn-ghost {
  background: transparent;
  color: var(--color-text-muted);
  border: 1px solid var(--color-border-subtle);
}

.product-hero-btn-ghost:hover {
  color: var(--color-primary);
  border-color: var(--color-primary-20);
}

.product-hero-mockup {
  display: flex;
  justify-content: center;
}

.product-hero-mockup-screen {
  width: 100%;
  max-width: 380px;
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-xl);
  overflow: hidden;
}

.product-hero-mockup-bar {
  display: flex;
  gap: 6px;
  padding: 12px 16px;
  background: var(--color-bg-elevated);
  border-bottom: 1px solid var(--color-border-subtle);
}

.product-hero-mockup-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: var(--color-border-default);
}

.product-hero-mockup-body {
  padding: 3rem var(--spacing-xl);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
  min-height: 220px;
  justify-content: center;
}

.product-hero-placeholder-icon {
  width: 64px;
  height: 64px;
  border-radius: var(--radius-xl);
  background: var(--color-primary-soft);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.8rem;
  color: var(--color-primary);
}

.product-hero-placeholder-label {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.product-hero-placeholder-sub {
  font-size: 0.8rem;
  color: var(--color-text-muted);
  margin: 0;
}

@media (max-width: 768px) {
  .product-hero-container {
    grid-template-columns: 1fr;
  }

  .product-hero-visual {
    order: -1;
  }

  .product-hero-title {
    font-size: var(--font-size-h2);
  }
}
</style>
