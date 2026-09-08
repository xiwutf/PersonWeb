<template>
  <div v-if="isAdmin" class="admin-edit-banner" role="status">
    已登录 · 点击文案可编辑
  </div>
</template>

<script setup lang="ts">
const { isAdmin } = useAdminSession()
</script>
