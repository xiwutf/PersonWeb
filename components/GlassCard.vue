<template>
  <div 
    :class="[
      'backdrop-blur-md bg-white/10 dark:bg-gray-900/10 border border-white/20 dark:border-gray-700/20 rounded-xl p-6 shadow-lg',
      hoverEffect && 'transition-all duration-300 hover:bg-white/20 dark:hover:bg-gray-900/20 hover:shadow-xl hover:scale-[1.02]',
      className
    ]"
  >
    <slot />
  </div>
</template>

<script setup lang="ts">
defineProps<{
  hoverEffect?: boolean
  className?: string
}>()
</script>

