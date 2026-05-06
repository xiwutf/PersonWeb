<template>
  <NuxtLink :to="to" class="product-card">
    <div class="product-card-badge">{{ badge }}</div>
    <div class="product-card-icon">
      <i :class="icon"></i>
    </div>
    <h3 class="product-card-title">{{ title }}</h3>
    <p class="product-card-desc">{{ description }}</p>
    <div class="product-card-footer">
      <span class="product-card-link">{{ cta }}</span>
      <i class="fas fa-arrow-right product-card-arrow"></i>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  to: string
  badge: string
  icon: string
  title: string
  description: string
  cta?: string
}>(), {
  cta: '查看详情'
})
</script>

<style scoped>
.product-card {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  padding: var(--spacing-xl);
  text-decoration: none;
  transition: box-shadow 0.2s, border-color 0.2s, transform 0.2s;
}

.product-card:hover {
  box-shadow: var(--shadow-lg);
  border-color: var(--color-primary-20);
  transform: translateY(-2px);
}

.product-card-badge {
  display: inline-block;
  font-size: 0.75rem;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: var(--radius-full);
  background: var(--color-primary-soft);
  color: var(--color-primary);
  align-self: flex-start;
}

.product-card-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-lg);
  background: var(--color-primary-soft);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.4rem;
  color: var(--color-primary);
}

.product-card-title {
  font-size: var(--font-size-h3);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0;
}

.product-card-desc {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
  flex: 1;
}

.product-card-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: auto;
}

.product-card-link {
  font-size: 0.9rem;
  color: var(--color-primary);
  font-weight: 500;
}

.product-card-arrow {
  font-size: 0.85rem;
  color: var(--color-primary);
  transition: transform 0.2s;
}

.product-card:hover .product-card-arrow {
  transform: translateX(4px);
}
</style>
