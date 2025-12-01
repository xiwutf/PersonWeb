<template>
  <NuxtLink :to="item.link" class="bento-item group">
    <div class="item-inner">
      <div class="item-content">
        <h4 class="item-title">{{ item.title }}</h4>
        <div class="item-tags">
          <span
            v-for="tag in item.tags"
            :key="tag"
            class="item-tag"
          >
            {{ tag }}
          </span>
        </div>
      </div>
      <i class="fas fa-arrow-right item-arrow"></i>
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
.bento-item {
  @apply block;
}

.item-inner {
  @apply flex items-center justify-between p-4 bg-bg-elevated rounded-xl border border-border-subtle hover:bg-bg-card hover:border-border-default transition-all duration-300;
}

.bento-item:hover .item-inner {
  @apply transform translate-x-1;
}

.item-content {
  @apply flex-1;
}

.item-title {
  @apply text-text-main font-medium text-sm mb-2;
}

.item-tags {
  @apply flex flex-wrap gap-1;
}

.item-tag {
  @apply px-2 py-0.5 rounded text-xs bg-bg-card text-text-muted border border-border-subtle;
}

.item-arrow {
  @apply text-text-muted group-hover:text-text-main transition-all duration-300;
}

.bento-item:hover .item-arrow {
  @apply translate-x-1;
}
</style>

