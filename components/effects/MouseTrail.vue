<template>
  <div ref="trailRef" class="mouse-trail-layer" aria-hidden="true">
    <div
      v-for="dot in trailDots"
      :key="dot.id"
      class="mouse-trail-dot"
      :style="{
        left: `${dot.x}px`,
        top: `${dot.y}px`,
        opacity: dot.opacity,
        transform: `translate(-50%, -50%) scale(${dot.scale})`
      }"
    />

    <div
      v-for="ripple in ripples"
      :key="ripple.id"
      class="mouse-trail-ripple"
      :style="{
        left: `${ripple.x}px`,
        top: `${ripple.y}px`
      }"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'

interface TrailDot {
  id: number
  x: number
  y: number
  scale: number
  opacity: number
}

interface Ripple {
  id: number
  x: number
  y: number
}

const trailRef = ref<HTMLDivElement | null>(null)
const trailDots = ref<TrailDot[]>([])
const ripples = ref<Ripple[]>([])

let moveThrottleTimer: number | null = null
let fadeTimer: number | null = null
let nextRippleId = 0
let isConstrainedDevice = false
let visibilityHandler: (() => void) | null = null

const DOT_COUNT = 6

const buildInitialDots = () => {
  trailDots.value = Array.from({ length: DOT_COUNT }, (_, index) => ({
    id: index,
    x: -100,
    y: -100,
    scale: 1 - index * 0.1,
    opacity: 0
  }))
}

const detectConstrainedDevice = () => {
  const reducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const saveData = navigator.connection?.saveData === true
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  isConstrainedDevice = Boolean(reducedMotion || coarsePointer || saveData || lowMemory)
}

const fadeTrail = () => {
  trailDots.value = trailDots.value.map((dot, index) => ({
    ...dot,
    opacity: Math.max(0, 0.28 - index * 0.04)
  }))
}

const updateTrail = (x: number, y: number) => {
  const previous = trailDots.value.map(dot => ({ ...dot }))

  trailDots.value = trailDots.value.map((dot, index) => {
    if (index === 0) {
      return {
        ...dot,
        x,
        y,
        scale: 1,
        opacity: 0.6
      }
    }

    const leader = previous[index - 1]
    return {
      ...dot,
      x: leader.x,
      y: leader.y,
      scale: 1 - index * 0.1,
      opacity: Math.max(0.14, 0.45 - index * 0.06)
    }
  })
}

const handleMouseMove = (event: MouseEvent) => {
  if (isConstrainedDevice || document.hidden) return
  if (moveThrottleTimer) return

  moveThrottleTimer = window.setTimeout(() => {
    moveThrottleTimer = null
    updateTrail(event.clientX, event.clientY)

    if (fadeTimer) {
      window.clearTimeout(fadeTimer)
    }

    fadeTimer = window.setTimeout(() => {
      fadeTrail()
    }, 120)
  }, 24)
}

const handleClick = (event: MouseEvent) => {
  if (isConstrainedDevice || document.hidden) return

  const ripple: Ripple = {
    id: nextRippleId++,
    x: event.clientX,
    y: event.clientY
  }

  ripples.value = [...ripples.value, ripple]

  window.setTimeout(() => {
    ripples.value = ripples.value.filter(item => item.id !== ripple.id)
  }, 520)
}

onMounted(() => {
  if (!process.client) return

  detectConstrainedDevice()
  if (isConstrainedDevice) return

  buildInitialDots()

  visibilityHandler = () => {
    if (document.hidden) {
      trailDots.value = trailDots.value.map(dot => ({ ...dot, opacity: 0 }))
    }
  }

  document.addEventListener('visibilitychange', visibilityHandler)
  window.addEventListener('mousemove', handleMouseMove, { passive: true })
  window.addEventListener('click', handleClick, { passive: true })
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)
  window.removeEventListener('click', handleClick)

  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }

  if (moveThrottleTimer) {
    window.clearTimeout(moveThrottleTimer)
  }

  if (fadeTimer) {
    window.clearTimeout(fadeTimer)
  }
})
</script>

<style scoped>
.mouse-trail-layer {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 40;
}

.mouse-trail-dot {
  position: fixed;
  width: 10px;
  height: 10px;
  border-radius: 999px;
  background: radial-gradient(circle, rgba(96, 165, 250, 0.9), rgba(59, 130, 246, 0.1));
  box-shadow: 0 0 16px rgba(96, 165, 250, 0.35);
  transition:
    left 0.12s linear,
    top 0.12s linear,
    opacity 0.22s ease,
    transform 0.22s ease;
  will-change: transform, opacity;
}

.mouse-trail-ripple {
  position: fixed;
  width: 18px;
  height: 18px;
  border-radius: 999px;
  border: 1px solid rgba(96, 165, 250, 0.45);
  transform: translate(-50%, -50%);
  animation: mouse-trail-ripple 0.52s ease-out forwards;
}

@keyframes mouse-trail-ripple {
  0% {
    width: 18px;
    height: 18px;
    opacity: 0.45;
  }

  100% {
    width: 68px;
    height: 68px;
    opacity: 0;
  }
}
</style>
