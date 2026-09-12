<script setup lang="ts">
definePageMeta({
  layout: 'life',
})

await navigateTo('/life/cognition', { redirectCode: 301, replace: true })
</script>

<template>
  <div class="life-empty">正在跳转到说明书…</div>
</template>
