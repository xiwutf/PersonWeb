<template>
  <div
    v-if="authenticated"
    class="admin-edit-banner"
    :class="{ 'is-preview': previewAsGuest }"
    role="status"
  >
    <span class="admin-edit-banner__text">
      <template v-if="previewAsGuest">访客预览中 · 编辑已隐藏</template>
      <template v-else>已登录 · 点击文案可编辑 · 列表可新增 / 删除</template>
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
  togglePreviewAsGuest,
  logout,
} = useAdminSession()

async function onLogout() {
  await logout()
}
</script>
