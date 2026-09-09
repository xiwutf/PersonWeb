<template>
  <button
    v-if="isAdmin"
    type="button"
    class="life-admin-add"
    @click="$emit('click')"
  >
    {{ label }}
  </button>
</template>

<script setup lang="ts">
withDefaults(defineProps<{
  label?: string
}>(), {
  label: '新增',
})

defineEmits<{
  click: []
}>()

const { isAdmin } = useAdminSession()
</script>
