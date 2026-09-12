<template>
  <button
    v-if="canEditContent"
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

const { canEditContent } = useAdminSession()
</script>
