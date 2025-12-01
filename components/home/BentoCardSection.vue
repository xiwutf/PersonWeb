<template>
  <div class="bento-card-section">
    <div class="card-glass h-full">
      <div class="card-header">
        <div class="card-icon" :class="iconBg">
          <i :class="icon"></i>
        </div>
        <h3 class="card-title">{{ title }}</h3>
        <p class="card-subtitle">{{ subtitle }}</p>
      </div>

      <div class="card-content">
        <BentoCardItem
          v-for="item in items"
          :key="item.id"
          :item="item"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import BentoCardItem from './BentoCardItem.vue'

defineProps<{
  title: string
  subtitle: string
  icon: string
  iconBg: string
  items: Array<{
    id: number
    title: string
    type: string
    link: string
    tags: string[]
  }>
}>()
</script>

<style scoped>
.bento-card-section {
  @apply relative;
}

.card-glass {
  @apply bg-bg-card backdrop-blur-xl border border-border-subtle rounded-3xl p-6 transition-all duration-300;
  box-shadow: var(--shadow-md);
}

.bento-card-section:hover .card-glass {
  @apply scale-[1.02] border-border-default;
  box-shadow: var(--shadow-lg);
  transform: translateY(-4px) rotateX(2deg);
}

.card-header {
  @apply mb-6;
}

.card-icon {
  @apply w-12 h-12 rounded-xl flex items-center justify-center text-2xl mb-4;
}

.card-title {
  @apply text-xl font-bold text-text-main mb-1;
}

.card-subtitle {
  @apply text-sm text-text-muted;
}

.card-content {
  @apply space-y-3;
}
</style>

