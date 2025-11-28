<template>
  <div ref="trailRef" class="fixed pointer-events-none z-50" />
</template>

<script setup lang="ts">
const trailRef = ref<HTMLDivElement | null>(null)
const trail: Array<{ x: number; y: number; element: HTMLElement }> = []
const maxTrailLength = 20

const createTrailPoint = (x: number, y: number) => {
  if (!trailRef.value) return

  const point = document.createElement('div')
  point.className = 'absolute w-2 h-2 bg-blue-500 rounded-full opacity-60 pointer-events-none transition-all duration-300'
  point.style.left = `${x}px`
  point.style.top = `${y}px`
  point.style.transform = 'translate(-50%, -50%)'
  trailRef.value.appendChild(point)

  trail.push({ x, y, element: point })

  // 限制轨迹长度
  if (trail.length > maxTrailLength) {
    const old = trail.shift()
    if (old) {
      old.element.style.opacity = '0'
      old.element.style.transform = 'translate(-50%, -50%) scale(0)'
      setTimeout(() => {
        if (old.element.parentNode) {
          old.element.parentNode.removeChild(old.element)
        }
      }, 300)
    }
  }

  // 逐渐消失效果
  setTimeout(() => {
    point.style.opacity = '0'
    point.style.transform = 'translate(-50%, -50%) scale(0)'
    setTimeout(() => {
      if (point.parentNode) {
        point.parentNode.removeChild(point)
      }
      const index = trail.findIndex(t => t.element === point)
      if (index > -1) {
        trail.splice(index, 1)
      }
    }, 300)
  }, 500)
}

const handleMouseMove = (e: MouseEvent) => {
  createTrailPoint(e.clientX, e.clientY)
}

onMounted(() => {
  window.addEventListener('mousemove', handleMouseMove)
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)
})
</script>

