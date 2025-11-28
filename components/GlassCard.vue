<template>
  <div
    ref="cardRef"
    class="glass-card relative overflow-hidden"
    :class="cardClass"
    @mousemove="handleMouseMove"
    @mouseleave="handleMouseLeave"
    :style="cardStyle"
  >
    <!-- 玻璃背景 -->
    <div class="absolute inset-0 bg-white/10 dark:bg-gray-900/20 backdrop-blur-xl border border-white/20 dark:border-white/10 rounded-2xl"></div>
    
    <!-- 光泽效果 -->
    <div
      class="absolute inset-0 opacity-0 transition-opacity duration-300 pointer-events-none"
      :class="{ 'opacity-100': isHovering }"
      :style="shineStyle"
    ></div>
    
    <!-- 内容 -->
    <div class="relative z-10">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const props = withDefaults(defineProps<{
  tilt?: boolean
  physics?: boolean
}>(), {
  tilt: true,
  physics: false
})

const cardRef = ref<HTMLDivElement | null>(null)
const isHovering = ref(false)
const rotateX = ref(0)
const rotateY = ref(0)
const shineX = ref(0)
const shineY = ref(0)

const handleMouseMove = (e: MouseEvent) => {
  if (!cardRef.value || !props.tilt) return

  isHovering.value = true
  const rect = cardRef.value.getBoundingClientRect()
  const x = e.clientX - rect.left
  const y = e.clientY - rect.top

  const centerX = rect.width / 2
  const centerY = rect.height / 2

  // 计算旋转角度
  rotateX.value = ((y - centerY) / centerY) * -8
  rotateY.value = ((x - centerX) / centerX) * 8

  // 光泽位置
  shineX.value = (x / rect.width) * 100
  shineY.value = (y / rect.height) * 100
}

const handleMouseLeave = () => {
  isHovering.value = false
  rotateX.value = 0
  rotateY.value = 0
}

const cardStyle = computed(() => {
  if (!props.tilt) return {}
  
  return {
    transform: `perspective(1000px) rotateX(${rotateX.value}deg) rotateY(${rotateY.value}deg) scale(${isHovering.value ? 1.02 : 1})`,
    transition: isHovering.value ? 'none' : 'transform 0.3s ease-out'
  }
})

const shineStyle = computed(() => {
  return {
    background: `radial-gradient(circle at ${shineX.value}% ${shineY.value}%, rgba(255, 255, 255, 0.3), transparent 70%)`
  }
})

const cardClass = computed(() => {
  return {
    'hover:shadow-2xl': true,
    'transition-all': true,
    'duration-300': true
  }
})
</script>

<style scoped>
.glass-card {
  transform-style: preserve-3d;
}
</style>
