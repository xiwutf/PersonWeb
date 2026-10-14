<template>
  <LibraryReader :key="String(route.params.id)" :book-id="String(route.params.id)" />
</template>
<script setup lang="ts">
definePageMeta({ layout: 'library' })
useHead({ title: '阅读', meta: [{ name: 'robots', content: 'noindex, nofollow' }] })
const route = useRoute()
</script>
