<template>
  <header class="life-thoughts-hero">
    <p class="life-thoughts-kicker">THOUGHTS</p>
    <h1>{{ title }}</h1>
    <p class="life-thoughts-lead">
      <template v-for="(line, index) in lines" :key="index">
        {{ line }}<br v-if="index < lines.length - 1">
      </template>
    </p>
  </header>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  title?: string
  description?: string
}>(), {
  title: '生活里的想法',
  description: '把一些反复想起的话，按主题慢慢收起来。',
})

const lines = computed(() =>
  props.description
    .split(/\n/)
    .map(line => line.trim())
    .filter(Boolean),
)
</script>
