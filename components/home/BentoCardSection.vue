<template>
  <div class="bento-card-section">
    <div class="bento-card-section-glass">
      <div class="bento-card-section-header">
        <div class="bento-card-section-icon" :class="iconBg">
          <i :class="icon"></i>
        </div>
        <h3 class="bento-card-section-title">{{ title }}</h3>
        <p class="bento-card-section-subtitle">{{ subtitle }}</p>
      </div>

      <div class="bento-card-section-content">
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
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

