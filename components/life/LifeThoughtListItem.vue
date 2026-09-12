<template>
  <article class="life-thought-list-item">
    <p class="life-thought-list-text">{{ item.text }}</p>
    <p v-if="item.note" class="life-thought-list-note">{{ item.note }}</p>
    <p v-if="item.source" class="life-thought-list-source">{{ item.source }}</p>
    <p v-if="item.createdAt" class="life-thought-list-date">{{ item.createdAt }}</p>
  </article>
</template>

<script setup lang="ts">
defineProps<{
  item: {
    id: string
    text: string
    note?: string
    source?: string
    createdAt?: string
    featured?: boolean
  }
}>()
</script>
