<template>
  <NuxtLink :to="item.link" class="bento-card-item">
    <div class="bento-card-item-inner">
      <div class="bento-card-item-content">
        <h4 class="bento-card-item-title">{{ item.title }}</h4>
        <div class="bento-card-item-tags">
          <span
            v-for="tag in item.tags"
            :key="tag"
            class="bento-card-item-tag"
          >
            {{ tag }}
          </span>
        </div>
      </div>
      <i class="fas fa-arrow-right bento-card-item-arrow"></i>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
defineProps<{
  item: {
    id: number
    title: string
    type: string
    link: string
    tags: string[]
  }
}>()
</script>

<style scoped>
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

