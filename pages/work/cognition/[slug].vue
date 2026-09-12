<script setup lang="ts">
const route = useRoute()
const slug = String(route.params.slug || '')
await navigateTo(slug ? `/life/cognition/${slug}` : '/life/cognition', {
  redirectCode: 301,
  external: false,
})
</script>
<template>
  <div />
</template>
