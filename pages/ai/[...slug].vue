<script setup lang="ts">
/**
 * LEGACY /ai/* → /work/ai/*
 */
const route = useRoute()
const slug = Array.isArray(route.params.slug)
  ? route.params.slug.join('/')
  : String(route.params.slug || '')
const target = slug ? `/work/ai/${slug}` : '/work/ai'
await navigateTo(target, { redirectCode: 301, external: false })
</script>

<template>
  <div />
</template>
