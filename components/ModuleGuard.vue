<template>
  <div v-if="isEnabled">
    <slot />
  </div>
  <div v-else-if="showFallback" class="module-disabled">
    <slot name="fallback">
      <div class="text-center py-12">
        <i class="fas fa-ban text-4xl text-gray-400 mb-4"></i>
        <p class="text-gray-500">此功能模块未启用</p>
      </div>
    </slot>
  </div>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  moduleKey: string
  showFallback?: boolean
}>(), {
  showFallback: true
})

const { isModuleEnabled } = useModuleSystem()
const isEnabled = computed(() => isModuleEnabled(props.moduleKey))
</script>

<style scoped>
.module-disabled {
  min-height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>

