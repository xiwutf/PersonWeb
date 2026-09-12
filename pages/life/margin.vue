<script setup lang="ts">
definePageMeta({
  layout: 'life',
})

await navigateTo('/life/thoughts', { redirectCode: 301, replace: true })
</script>

<template>
  <div class="life-empty">正在跳转到想法精选…</div>
</template>
