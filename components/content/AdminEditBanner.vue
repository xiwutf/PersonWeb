<template>
  <div
    v-if="authenticated"
    class="admin-edit-banner"
    :class="{ 'is-preview': previewAsGuest }"
    role="status"
  >
    <span class="admin-edit-banner__text">
      <template v-if="previewAsGuest">访客预览中 · 编辑已隐藏</template>
      <template v-else-if="!canEditContent">已登录 · 线上不能改站点文案，请用本地 npm run dev</template>
      <template v-else>已登录 · 说明书点「后台编辑」</template>
    </span>
    <span class="admin-edit-banner__actions">
      <button
        type="button"
        class="admin-edit-banner__btn"
        @mousedown.prevent
        @click="togglePreviewAsGuest"
      >
        {{ previewAsGuest ? '返回编辑' : '访客预览' }}
      </button>
      <button
        type="button"
        class="admin-edit-banner__btn admin-edit-banner__btn--quiet"
        @mousedown.prevent
        @click="onLogout"
      >
        退出登录
      </button>
    </span>
  </div>
</template>

<script setup lang="ts">
const {
  authenticated,
  previewAsGuest,
  canEditContent,
  togglePreviewAsGuest,
  logout,
} = useAdminSession()

async function onLogout() {
  await logout()
}
</script>
