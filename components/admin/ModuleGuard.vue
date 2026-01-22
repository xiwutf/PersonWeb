<template>
  <ClientOnly>
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
    <template #fallback>
      <!-- SSR 时的占位内容，避免 hydration mismatch -->
      <div>
        <slot />
      </div>
    </template>
  </ClientOnly>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  moduleKey: string
  showFallback?: boolean
}>(), {
  showFallback: true
})

// 使用 ClientOnly 确保只在客户端检查模块状态，避免 SSR 不匹配
const { isModuleEnabled } = useModuleSystem()
const isEnabled = computed(() => {
  // 在服务端渲染时，默认返回 true，避免 hydration mismatch
  if (!process.client) {
    return true
  }
  return isModuleEnabled(props.moduleKey)
})
</script>

<style scoped>
.module-disabled {
  min-height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>

