<template>
  <div class="pricing-page">
    <div class="pricing-page-container">

      <div class="pricing-page-header">
        <p class="pricing-page-kicker">Pricing</p>
        <h1 class="pricing-page-title">版本说明</h1>
        <p class="pricing-page-subtitle">当前阶段暂未正式开放支付，可先联系体验 Pro 内测。</p>
      </div>

      <!-- Plans overview -->
      <div class="pricing-plans">
        <div class="pricing-plan-card">
          <div class="pricing-plan-name">Free</div>
          <div class="pricing-plan-price">免费</div>
          <p class="pricing-plan-desc">永久免费使用基础功能，适合想先体验的用户。</p>
          <ul class="pricing-plan-list">
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              基础桌宠陪伴
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              成长系统
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              基础角色
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              AI 快捷助手（每日 3 次）
            </li>
          </ul>
          <NuxtLink to="/download" class="pricing-plan-btn pricing-plan-btn-free">
            <i class="fas fa-download"></i>
            免费下载
          </NuxtLink>
        </div>

        <div class="pricing-plan-card pricing-plan-card--pro">
          <div class="pricing-plan-badge">内测中</div>
          <div class="pricing-plan-name">Pro</div>
          <div class="pricing-plan-price">即将公布</div>
          <p class="pricing-plan-desc">解锁全部高级功能，适合重度效率用户。</p>
          <ul class="pricing-plan-list">
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              Free 版所有功能
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              AI 快捷助手（更高额度）
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              高级专注模式
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              高级统计
            </li>
            <li class="pricing-plan-item">
              <i class="fas fa-check pricing-plan-check"></i>
              Pro 专属角色
            </li>
          </ul>
          <a href="mailto:linxiwanting@gmail.com" class="pricing-plan-btn pricing-plan-btn-pro">
            申请 Pro 内测
          </a>
        </div>
      </div>

      <!-- Target users -->
      <section class="pricing-users-section">
        <h2 class="pricing-section-title">谁适合使用它？</h2>
        <div class="pricing-users-grid">
          <div
            v-for="(user, index) in targetUsers"
            :key="index"
            class="pricing-user-card"
          >
            <h3 class="pricing-user-title">{{ user.title }}</h3>
            <p class="pricing-user-desc">{{ user.desc }}</p>
          </div>
        </div>
      </section>

      <!-- Current stage notice -->
      <div class="pricing-notice">
        <i class="fas fa-circle-info pricing-notice-icon"></i>
        <div>
          <h3 class="pricing-notice-title">当前阶段说明</h3>
          <p class="pricing-notice-desc">
            产品目前处于内测阶段，支付功能暂未正式开放。Free 版可直接下载使用，Pro 版可通过邮件联系作者申请内测体验。
          </p>
        </div>
      </div>

      <div class="pricing-page-back">
        <NuxtLink to="/products/desktop-pet" class="pricing-page-back-link">
          <i class="fas fa-arrow-left"></i>
          返回产品介绍
        </NuxtLink>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const targetUsers = DESKTOP_PET.targetUsers

useHead({
  title: 'AI 桌面陪伴助手 - 版本说明',
  meta: [
    {
      name: 'description',
      content: 'AI 桌面陪伴助手 Free 和 Pro 版本功能对比说明，了解适合你的版本。'
    }
  ]
})
</script>

<style scoped>
.pricing-page {
  padding: 4rem 0 6rem;
}

.pricing-page-container {
  max-width: 64rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.pricing-page-header {
  text-align: center;
  margin-bottom: 3rem;
}

.pricing-page-kicker {
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-primary);
  margin: 0 0 var(--spacing-sm);
}

.pricing-page-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md);
}

.pricing-page-subtitle {
  font-size: 1rem;
  color: var(--color-text-muted);
  margin: 0;
}

.pricing-plans {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-xl);
  margin-bottom: 4rem;
}

.pricing-plan-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  padding: var(--spacing-xl);
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  position: relative;
}

.pricing-plan-card--pro {
  border-color: var(--color-primary-20);
  background: var(--color-primary-10);
}

.pricing-plan-badge {
  position: absolute;
  top: var(--spacing-lg);
  right: var(--spacing-lg);
  font-size: 0.7rem;
  font-weight: 700;
  padding: 2px 10px;
  background: var(--color-primary);
  color: #fff;
  border-radius: var(--radius-full);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.pricing-plan-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text-main);
}

.pricing-plan-price {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--color-primary);
}

.pricing-plan-desc {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
}

.pricing-plan-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  flex: 1;
}

.pricing-plan-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  font-size: 0.9rem;
  color: var(--color-text-muted);
}

.pricing-plan-check {
  color: var(--color-primary);
  font-size: 0.85rem;
  flex-shrink: 0;
}

.pricing-plan-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-sm);
  padding: 10px 24px;
  border-radius: var(--radius-lg);
  font-size: 0.95rem;
  font-weight: 500;
  text-decoration: none;
  transition: opacity 0.2s, transform 0.2s;
  text-align: center;
}

.pricing-plan-btn-free {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.pricing-plan-btn-free:hover {
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.pricing-plan-btn-pro {
  background: var(--color-primary);
  color: #fff;
}

.pricing-plan-btn-pro:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.pricing-section-title {
  font-size: var(--font-size-h3);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-xl);
}

.pricing-users-section {
  margin-bottom: 3rem;
}

.pricing-users-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--spacing-lg);
}

.pricing-user-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg) var(--spacing-xl);
}

.pricing-user-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.pricing-user-desc {
  font-size: 0.85rem;
  color: var(--color-text-muted);
  margin: 0;
  line-height: 1.6;
}

.pricing-notice {
  background: var(--color-primary-soft);
  border: 1px solid var(--color-primary-20);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg) var(--spacing-xl);
  display: flex;
  gap: var(--spacing-lg);
  align-items: flex-start;
  margin-bottom: 3rem;
}

.pricing-notice-icon {
  color: var(--color-primary);
  font-size: 1.2rem;
  flex-shrink: 0;
  margin-top: 2px;
}

.pricing-notice-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.pricing-notice-desc {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
}

.pricing-page-back {
  text-align: center;
}

.pricing-page-back-link {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: 0.9rem;
  color: var(--color-text-muted);
  text-decoration: none;
  transition: color 0.2s;
}

.pricing-page-back-link:hover {
  color: var(--color-primary);
}

@media (max-width: 640px) {
  .pricing-plans {
    grid-template-columns: 1fr;
  }

  .pricing-users-grid {
    grid-template-columns: 1fr;
  }
}
</style>
