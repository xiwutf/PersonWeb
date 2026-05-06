<template>
  <section class="pricing-compare-section">
    <div class="pricing-compare-container">
      <div class="pricing-compare-header">
        <p class="pricing-compare-kicker">Pricing</p>
        <h2 class="pricing-compare-title">免费使用，也可升级更强版本</h2>
        <p class="pricing-compare-subtitle">当前阶段暂未正式开放支付，可先联系体验内测。</p>
      </div>

      <div class="pricing-compare-table-wrap">
        <table class="pricing-compare-table">
          <thead>
            <tr>
              <th class="pricing-compare-th pricing-compare-th-feature">功能</th>
              <th class="pricing-compare-th pricing-compare-th-plan">
                <span class="pricing-compare-plan-name">Free</span>
                <span class="pricing-compare-plan-price">免费</span>
              </th>
              <th class="pricing-compare-th pricing-compare-th-plan pricing-compare-th-pro">
                <span class="pricing-compare-plan-badge">推荐</span>
                <span class="pricing-compare-plan-name">Pro</span>
                <span class="pricing-compare-plan-price">内测中</span>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(row, index) in pricing"
              :key="index"
              class="pricing-compare-row"
            >
              <td class="pricing-compare-td pricing-compare-td-feature">{{ row.feature }}</td>
              <td class="pricing-compare-td pricing-compare-td-value">
                <span v-if="row.free === true" class="pricing-compare-check">
                  <i class="fas fa-check"></i>
                </span>
                <span v-else-if="row.free === false" class="pricing-compare-cross">
                  <i class="fas fa-times"></i>
                </span>
                <span v-else class="pricing-compare-text">{{ row.free }}</span>
              </td>
              <td class="pricing-compare-td pricing-compare-td-value pricing-compare-td-pro">
                <span v-if="row.pro === true" class="pricing-compare-check">
                  <i class="fas fa-check"></i>
                </span>
                <span v-else-if="row.pro === false" class="pricing-compare-cross">
                  <i class="fas fa-times"></i>
                </span>
                <span v-else class="pricing-compare-text">{{ row.pro }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="pricing-compare-actions">
        <NuxtLink to="/download" class="pricing-compare-btn-free">
          <i class="fas fa-download"></i>
          免费开始使用
        </NuxtLink>
        <a href="mailto:linxiwanting@gmail.com" class="pricing-compare-btn-pro">
          申请 Pro 内测
        </a>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const pricing = DESKTOP_PET.pricing
</script>

<style scoped>
.pricing-compare-section {
  padding: 5rem 0;
  background: var(--color-bg-elevated);
}

.pricing-compare-container {
  max-width: 56rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.pricing-compare-header {
  text-align: center;
  margin-bottom: 3rem;
}

.pricing-compare-kicker {
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-primary);
  margin: 0 0 var(--spacing-sm);
}

.pricing-compare-title {
  font-size: var(--font-size-h2);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md);
}

.pricing-compare-subtitle {
  font-size: 1rem;
  color: var(--color-text-muted);
  margin: 0;
}

.pricing-compare-table-wrap {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  overflow: hidden;
  margin-bottom: var(--spacing-xl);
}

.pricing-compare-table {
  width: 100%;
  border-collapse: collapse;
}

.pricing-compare-th {
  padding: var(--spacing-lg);
  text-align: left;
  border-bottom: 1px solid var(--color-border-subtle);
  background: var(--color-bg-elevated);
}

.pricing-compare-th-plan {
  text-align: center;
  min-width: 120px;
  position: relative;
}

.pricing-compare-th-pro {
  background: var(--color-primary-soft);
  border-left: 2px solid var(--color-primary-20);
}

.pricing-compare-plan-badge {
  display: block;
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--color-primary);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 2px;
}

.pricing-compare-plan-name {
  display: block;
  font-size: 1rem;
  font-weight: 700;
  color: var(--color-text-main);
}

.pricing-compare-plan-price {
  display: block;
  font-size: 0.8rem;
  color: var(--color-text-muted);
  margin-top: 2px;
}

.pricing-compare-row {
  border-bottom: 1px solid var(--color-border-subtle);
}

.pricing-compare-row:last-child {
  border-bottom: none;
}

.pricing-compare-td {
  padding: var(--spacing-md) var(--spacing-lg);
  color: var(--color-text-main);
  font-size: 0.9rem;
}

.pricing-compare-td-value {
  text-align: center;
}

.pricing-compare-td-pro {
  border-left: 2px solid var(--color-primary-20);
  background: var(--color-primary-10);
}

.pricing-compare-check {
  color: var(--color-primary);
  font-size: 1rem;
}

.pricing-compare-cross {
  color: var(--color-border-default);
  font-size: 0.9rem;
}

.pricing-compare-text {
  font-size: 0.85rem;
  color: var(--color-text-muted);
}

.pricing-compare-actions {
  display: flex;
  justify-content: center;
  gap: var(--spacing-md);
  flex-wrap: wrap;
}

.pricing-compare-btn-free,
.pricing-compare-btn-pro {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: 10px 28px;
  border-radius: var(--radius-lg);
  font-size: 0.95rem;
  font-weight: 500;
  text-decoration: none;
  transition: opacity 0.2s, transform 0.2s;
}

.pricing-compare-btn-free {
  background: var(--color-primary);
  color: #fff;
}

.pricing-compare-btn-free:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.pricing-compare-btn-pro {
  background: var(--color-bg-card);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
}

.pricing-compare-btn-pro:hover {
  border-color: var(--color-primary);
  color: var(--color-primary);
}

@media (max-width: 480px) {
  .pricing-compare-td,
  .pricing-compare-th {
    padding: var(--spacing-md);
  }
}
</style>
