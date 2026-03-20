<template>
  <canvas ref="canvasRef" class="fixed inset-0 pointer-events-none z-0" />
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const canvasRef = ref<HTMLCanvasElement | null>(null)
let animationId: number | null = null
let resizeHandler: (() => void) | null = null
let visibilityHandler: (() => void) | null = null
let isPaused = false
let lastFrameTime = 0

interface Particle {
  x: number
  y: number
  vx: number
  vy: number
  radius: number
  color: string
}

const particles: Particle[] = []

const isConstrainedDevice = () => {
  if (!process.client) return false

  const reducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const saveData = navigator.connection?.saveData === true
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  return Boolean(reducedMotion || coarsePointer || saveData || lowMemory)
}

const getParticleCount = () => {
  const base = Math.min(Math.max(Math.round((window.innerWidth * window.innerHeight) / 70000), 12), 42)
  return isConstrainedDevice() ? Math.max(10, Math.round(base * 0.45)) : base
}

const initCanvas = () => {
  if (!canvasRef.value) return

  const canvas = canvasRef.value
  const ctx = canvas.getContext('2d')
  if (!ctx) return

  const resizeCanvas = () => {
    canvas.width = window.innerWidth
    canvas.height = window.innerHeight
  }

  resizeCanvas()
  resizeHandler = resizeCanvas
  window.addEventListener('resize', resizeCanvas, { passive: true })

  particles.length = 0
  for (let i = 0; i < getParticleCount(); i++) {
    particles.push({
      x: Math.random() * canvas.width,
      y: Math.random() * canvas.height,
      vx: (Math.random() - 0.5) * 0.5,
      vy: (Math.random() - 0.5) * 0.5,
      radius: Math.random() * 2 + 1,
      color: 'rgba(59, 130, 246, 0.65)'
    })
  }

  const animate = (timestamp = 0) => {
    if (isPaused) {
      animationId = requestAnimationFrame(animate)
      return
    }

    if (timestamp - lastFrameTime < 33) {
      animationId = requestAnimationFrame(animate)
      return
    }

    lastFrameTime = timestamp
    ctx.clearRect(0, 0, canvas.width, canvas.height)

    particles.forEach((particle, index) => {
      particle.x += particle.vx
      particle.y += particle.vy

      if (particle.x < 0 || particle.x > canvas.width) particle.vx *= -1
      if (particle.y < 0 || particle.y > canvas.height) particle.vy *= -1

      ctx.beginPath()
      ctx.arc(particle.x, particle.y, particle.radius, 0, Math.PI * 2)
      ctx.fillStyle = particle.color
      ctx.fill()

      for (let nextIndex = index + 1; nextIndex < particles.length; nextIndex++) {
        const other = particles[nextIndex]
        const dx = particle.x - other.x
        const dy = particle.y - other.y
        const distance = Math.sqrt(dx * dx + dy * dy)

        if (distance < 140) {
          ctx.beginPath()
          ctx.moveTo(particle.x, particle.y)
          ctx.lineTo(other.x, other.y)
          ctx.strokeStyle = `rgba(59, 130, 246, ${0.18 * (1 - distance / 140)})`
          ctx.lineWidth = 0.5
          ctx.stroke()
        }
      }
    })

    animationId = requestAnimationFrame(animate)
  }

  visibilityHandler = () => {
    isPaused = document.hidden
  }
  document.addEventListener('visibilitychange', visibilityHandler)

  animate()
}

onMounted(() => {
  if (!process.client || isConstrainedDevice()) return
  initCanvas()
})

onUnmounted(() => {
  if (animationId) {
    cancelAnimationFrame(animationId)
  }

  if (resizeHandler) {
    window.removeEventListener('resize', resizeHandler)
  }

  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }

  particles.length = 0
})
</script>

<style scoped>
canvas {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}
</style>
