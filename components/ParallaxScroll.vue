<template>
  <div ref="containerRef" class="parallax-container">
    <slot />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const props = withDefaults(defineProps<{
  speed?: number
  direction?: 'up' | 'down'
}>(), {
  speed: 0.5,
  direction: 'up'
})

const containerRef = ref<HTMLDivElement | null>(null)
let scrollHandler: (() => void) | null = null

const handleScroll = () => {
  if (!containerRef.value) return

  const scrollY = window.scrollY
  const rect = containerRef.value.getBoundingClientRect()
  const elementTop = rect.top + scrollY
  const elementHeight = rect.height
  const windowHeight = window.innerHeight

  // 计算元素在视口中的位置
  const elementCenter = elementTop + elementHeight / 2
  const viewportCenter = scrollY + windowHeight / 2
  const distance = viewportCenter - elementCenter

  // 应用视差效果
  const offset = distance * props.speed * (props.direction === 'up' ? -1 : 1)
  
  containerRef.value.style.transform = `translateY(${offset}px)`
}

onMounted(() => {
  scrollHandler = handleScroll
  window.addEventListener('scroll', handleScroll, { passive: true })
  handleScroll() // 初始调用
})

onUnmounted(() => {
  if (scrollHandler) {
    window.removeEventListener('scroll', scrollHandler)
  }
})
</script>

<style scoped>
.parallax-container {
  will-change: transform;
  transition: transform 0.1s ease-out;
}
</style>

