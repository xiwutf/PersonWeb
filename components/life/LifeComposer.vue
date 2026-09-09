<template>
  <form
    v-if="open"
    class="life-composer"
    @submit.prevent="$emit('submit')"
  >
    <p class="life-composer-title">{{ title }}</p>
    <slot />
    <div class="life-composer-actions">
      <button type="button" class="life-admin-add" @click="$emit('cancel')">取消</button>
      <button type="submit" class="life-admin-add life-admin-add--primary">保存</button>
    </div>
  </form>
</template>

<script setup lang="ts">
defineProps<{
  open: boolean
  title: string
}>()

defineEmits<{
  submit: []
  cancel: []
}>()
</script>
