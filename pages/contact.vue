<script setup lang="ts">
// Simple redirect page to avoid missing route warnings
definePageMeta({
  layout: 'home'
})

onMounted(() => {
  navigateTo('/about')
})
</script>

<template>
  <div class="contact-redirect">
    正在跳转...
  </div>
</template>

<style scoped>
.contact-redirect {
  padding: 40px;
  text-align: center;
  color: var(--color-text-main, #0f172a);
}
</style>

