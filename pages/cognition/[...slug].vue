<script setup lang="ts">
const route = useRoute()
const slugParam = route.params.slug
const slug = Array.isArray(slugParam) ? slugParam.join('/') : String(slugParam || '')
await navigateTo(slug ? `/life/cognition/${slug}` : '/life/cognition', {
  redirectCode: 301,
  external: false,
})
</script>
<template>
  <div />
</template>
