<template>
  <canvas
    v-if="show"
    ref="canvasRef"
    class="fireworks-canvas"
  ></canvas>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'

const canvasRef = ref<HTMLCanvasElement | null>(null)
const show = ref(false)

let ctx: CanvasRenderingContext2D | null = null
let animationFrame: number | null = null
const particles: Array<{
  x: number
  y: number
  vx: number
  vy: number
  color: string
  life: number
  maxLife: number
}> = []

const initCanvas = () => {
  if (!canvasRef.value) return

  canvasRef.value.width = window.innerWidth
  canvasRef.value.height = window.innerHeight
  ctx = canvasRef.value.getContext('2d')
}

const createFirework = (x: number, y: number) => {
  const colors = ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff', '#00ffff']
  
  for (let i = 0; i < 50; i++) {
    const angle = (Math.PI * 2 * i) / 50
    const speed = 2 + Math.random() * 4
    const color = colors[Math.floor(Math.random() * colors.length)]
    
    particles.push({
      x,
      y,
      vx: Math.cos(angle) * speed,
      vy: Math.sin(angle) * speed,
      color,
      life: 0,
      maxLife: 60 + Math.random() * 40
    })
  }
}

const animate = () => {
  if (!ctx || !canvasRef.value) return

  ctx.fillStyle = 'var(--shadow)'
  ctx.fillRect(0, 0, canvasRef.value.width, canvasRef.value.height)

  for (let i = particles.length - 1; i >= 0; i--) {
    const p = particles[i]
    
    p.x += p.vx
    p.y += p.vy
    p.vy += 0.1 // 重力
    p.life++

    const alpha = 1 - (p.life / p.maxLife)
    ctx.fillStyle = p.color.replace('rgb', 'rgba').replace(')', `, ${alpha})`)
    ctx.beginPath()
    ctx.arc(p.x, p.y, 3, 0, Math.PI * 2)
    ctx.fill()

    if (p.life >= p.maxLife) {
      particles.splice(i, 1)
    }
  }

  if (particles.length > 0) {
    animationFrame = requestAnimationFrame(animate)
  } else {
    show.value = false
  }
}

const startFireworks = () => {
  if (!canvasRef.value) return

  show.value = true
  particles.length = 0
  initCanvas()

  // 创建多个烟花
  const centerX = window.innerWidth / 2
  const centerY = window.innerHeight / 2

  for (let i = 0; i < 5; i++) {
    setTimeout(() => {
      createFirework(
        centerX + (Math.random() - 0.5) * 200,
        centerY + (Math.random() - 0.5) * 200
      )
    }, i * 300)
  }

  animate()
}

watch(show, (newVal) => {
  if (!newVal && animationFrame) {
    cancelAnimationFrame(animationFrame)
    animationFrame = null
  }
})

onMounted(() => {
  if (process.client) {
    window.addEventListener('trigger-fireworks', startFireworks)
    window.addEventListener('resize', initCanvas)
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('trigger-fireworks', startFireworks)
    window.removeEventListener('resize', initCanvas)
    
    if (animationFrame) {
      cancelAnimationFrame(animationFrame)
    }
  }
})
</script>

<style scoped>
.fireworks-canvas {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 10000;
}
</style>

