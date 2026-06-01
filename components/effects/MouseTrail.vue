<template>
  <div class="mouse-trail-layer" aria-hidden="true">
    <span
      v-for="particle in particles"
      :key="particle.id"
      class="mouse-trail-particle"
      :style="{
        left: `${particle.x}px`,
        top: `${particle.y}px`,
        width: `${particle.size}px`,
        height: `${particle.size}px`,
        background: particle.color,
        boxShadow: `0 0 ${particle.size * 3}px ${particle.color}`,
        transform: `translate(-50%, -50%) scale(${particle.scale})`,
        opacity: particle.opacity,
      }"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'

interface StardustParticle {
  id: number
  x: number
  y: number
  size: number
  color: string
  opacity: number
  scale: number
}

/** 星尘拖尾 / Cursor Stardust — 蓝紫青三色轻量粒子 */
const STARDUST_COLORS = ['#60a5fa', '#8b5cf6', '#22d3ee'] as const
const MAX_PARTICLES = 80
const SPAWN_MIN = 3
const SPAWN_MAX = 6
const MOVE_THROTTLE_MS = 20

const particles = ref<StardustParticle[]>([])

let nextParticleId = 0
let moveThrottleTimer: number | null = null
let animationFrameId: number | null = null
let isConstrainedDevice = false
let visibilityHandler: (() => void) | null = null

const detectConstrainedDevice = () => {
  const reducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const saveData = navigator.connection?.saveData === true
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  isConstrainedDevice = Boolean(reducedMotion || coarsePointer || saveData || lowMemory)
}

const spawnStardust = (clientX: number, clientY: number) => {
  const count = Math.floor(Math.random() * (SPAWN_MAX - SPAWN_MIN + 1)) + SPAWN_MIN
  const batch: StardustParticle[] = []

  for (let index = 0; index < count; index += 1) {
    batch.push({
      id: nextParticleId++,
      x: clientX + (Math.random() - 0.5) * 12,
      y: clientY + (Math.random() - 0.5) * 12,
      size: Math.random() * 4 + 2,
      color: STARDUST_COLORS[Math.floor(Math.random() * STARDUST_COLORS.length)],
      opacity: Math.random() * 0.15 + 0.7,
      scale: 1,
    })
  }

  particles.value = [...particles.value, ...batch]

  if (particles.value.length > MAX_PARTICLES) {
    particles.value = particles.value.slice(-MAX_PARTICLES)
  }
}

const animateStardust = () => {
  if (particles.value.length > 0) {
    particles.value = particles.value
      .map(particle => ({
        ...particle,
        opacity: particle.opacity - 0.025,
        scale: particle.scale * 0.96,
        y: particle.y - 0.2,
      }))
      .filter(particle => particle.opacity > 0)
  }

  animationFrameId = window.requestAnimationFrame(animateStardust)
}

const handleMouseMove = (event: MouseEvent) => {
  if (isConstrainedDevice || document.hidden) return
  if (moveThrottleTimer) return

  moveThrottleTimer = window.setTimeout(() => {
    moveThrottleTimer = null
    spawnStardust(event.clientX, event.clientY)
  }, MOVE_THROTTLE_MS)
}

onMounted(() => {
  if (!process.client) return

  detectConstrainedDevice()
  if (isConstrainedDevice) return

  visibilityHandler = () => {
    if (document.hidden) {
      particles.value = []
    }
  }

  document.addEventListener('visibilitychange', visibilityHandler)
  window.addEventListener('mousemove', handleMouseMove, { passive: true })
  animationFrameId = window.requestAnimationFrame(animateStardust)
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)

  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }

  if (moveThrottleTimer) {
    window.clearTimeout(moveThrottleTimer)
  }

  if (animationFrameId) {
    window.cancelAnimationFrame(animationFrameId)
  }
})
</script>

<style scoped>
.mouse-trail-layer {
  position: fixed;
  inset: 0;
  overflow: hidden;
  pointer-events: none;
  z-index: 40;
}

.mouse-trail-particle {
  position: absolute;
  border-radius: 999px;
  filter: blur(1px);
  will-change: transform, opacity;
}
</style>
