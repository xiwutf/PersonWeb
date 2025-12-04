<template>
  <div
    ref="cardRef"
    class="tilt-card relative transition-all duration-200 ease-out transform-style-3d"
    @mousemove="handleMouseMove"
    @mouseleave="handleMouseLeave"
    :style="cardStyle"
  >
    <slot />
    <!-- 光泽效果 -->
    <div 
      class="absolute inset-0 pointer-events-none opacity-0 transition-opacity duration-300 bg-gradient-to-br from-white/40 to-transparent rounded-xl"
      :style="{ opacity: isHovering ? 0.3 : 0 }"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const cardRef = ref(null)
const isHovering = ref(false)
const rotateX = ref(0)
const rotateY = ref(0)

const handleMouseMove = (e) => {
  if (!cardRef.value) return
  
  isHovering.value = true
  const rect = cardRef.value.getBoundingClientRect()
  const x = e.clientX - rect.left
  const y = e.clientY - rect.top
  
  const centerX = rect.width / 2
  const centerY = rect.height / 2
  
  // 计算旋转角度 (最大旋转 10 度)
  rotateX.value = ((y - centerY) / centerY) * -10
  rotateY.value = ((x - centerX) / centerX) * 10
}

const handleMouseLeave = () => {
  isHovering.value = false
  rotateX.value = 0
  rotateY.value = 0
}

const cardStyle = computed(() => {
  return {
    transform: `perspective(1000px) rotateX(${rotateX.value}deg) rotateY(${rotateY.value}deg) scale(${isHovering.value ? 1.02 : 1})`,
  }
})
</script>

<style scoped>
.transform-style-3d {
  transform-style: preserve-3d;
}
</style>

