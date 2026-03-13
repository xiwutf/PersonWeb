<template>
  <!-- 粒子流背景 -->
  <div v-if="effect === 'particles'" class="background-particles">
    <canvas ref="particlesCanvas" class="particles-canvas"></canvas>
  </div>

  <!-- 噪声纹理背景 -->
  <div v-else-if="effect === 'noise'" class="background-noise"></div>

  <!-- 波浪动画背景 -->
  <div v-else-if="effect === 'waves'" class="background-waves">
    <svg class="waves-svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320" preserveAspectRatio="none">
      <path
        ref="wavePath"
        fill="rgba(59, 130, 246, 0.1)"
        d="M0,96L48,112C96,128,192,160,288,160C384,160,480,128,576,112C672,96,768,96,864,112C960,128,1056,160,1152,160C1248,160,1344,128,1392,112L1440,96L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"
      ></path>
    </svg>
  </div>

  <!-- 星空背景 -->
  <div v-else-if="effect === 'stars'" class="background-stars">
    <div
      v-for="(star, index) in stars"
      :key="index"
      class="star"
      :style="{
        left: star.x + '%',
        top: star.y + '%',
        animationDelay: star.delay + 's',
        animationDuration: star.duration + 's'
      }"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'

const props = withDefaults(defineProps<{
  effect?: string
  config?: Record<string, any>
}>(), {
  effect: 'particles',
  config: () => ({})
})

const particlesCanvas = ref<HTMLCanvasElement | null>(null)
const wavePath = ref<SVGPathElement | null>(null)
const stars = ref<Array<{ x: number; y: number; delay: number; duration: number }>>([])

let animationFrame: number | null = null
let particles: Array<{
  x: number
  y: number
  vx: number
  vy: number
  radius: number
}> = []
let waveOffset = 0

// 粒子流效果
const initParticles = () => {
  if (!particlesCanvas.value) return

  const canvas = particlesCanvas.value
  const ctx = canvas.getContext('2d')
  if (!ctx) return

  const resize = () => {
    canvas.width = window.innerWidth
    canvas.height = window.innerHeight
  }
  resize()
  window.addEventListener('resize', resize)

  const count = props.config.count || 100
  const speed = props.config.speed || 1
  const color = props.config.color || 'var(--color-primary)'

  particles = []
  for (let i = 0; i < count; i++) {
    particles.push({
      x: Math.random() * canvas.width,
      y: Math.random() * canvas.height,
      vx: (Math.random() - 0.5) * speed,
      vy: (Math.random() - 0.5) * speed,
      radius: Math.random() * 2 + 1
    })
  }

  const animate = () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    
    particles.forEach((particle, i) => {
      particle.x += particle.vx
      particle.y += particle.vy

      if (particle.x < 0 || particle.x > canvas.width) particle.vx *= -1
      if (particle.y < 0 || particle.y > canvas.height) particle.vy *= -1

      ctx.beginPath()
      ctx.arc(particle.x, particle.y, particle.radius, 0, Math.PI * 2)
      ctx.fillStyle = color
      ctx.globalAlpha = 0.6
      ctx.fill()

      // 连线
      particles.slice(i + 1).forEach(other => {
        const dx = particle.x - other.x
        const dy = particle.y - other.y
        const distance = Math.sqrt(dx * dx + dy * dy)

        if (distance < 150) {
          ctx.beginPath()
          ctx.moveTo(particle.x, particle.y)
          ctx.lineTo(other.x, other.y)
          ctx.strokeStyle = color
          ctx.globalAlpha = 0.2 * (1 - distance / 150)
          ctx.lineWidth = 1
          ctx.stroke()
        }
      })
    })

    ctx.globalAlpha = 1
    animationFrame = requestAnimationFrame(animate)
  }

  animate()

  return () => {
    window.removeEventListener('resize', resize)
    if (animationFrame) {
      cancelAnimationFrame(animationFrame)
    }
  }
}

// 噪声纹理效果
const initNoise = () => {
  // 噪声纹理通过 CSS 实现
  return null
}

// 波浪动画效果
const initWaves = () => {
  if (!wavePath.value) return

  const animate = () => {
    if (!wavePath.value) return

    waveOffset += props.config.speed || 0.5
    const path = wavePath.value
    const d = `M0,96L48,${112 + Math.sin(waveOffset) * 10}C96,${128 + Math.sin(waveOffset + 0.5) * 10},192,${160 + Math.sin(waveOffset + 1) * 10},288,${160 + Math.sin(waveOffset + 1.5) * 10}C384,${160 + Math.sin(waveOffset + 2) * 10},480,${128 + Math.sin(waveOffset + 2.5) * 10},576,${112 + Math.sin(waveOffset + 3) * 10}C672,${96 + Math.sin(waveOffset + 3.5) * 10},768,${96 + Math.sin(waveOffset + 4) * 10},864,${112 + Math.sin(waveOffset + 4.5) * 10}C960,${128 + Math.sin(waveOffset + 5) * 10},1056,${160 + Math.sin(waveOffset + 5.5) * 10},1152,${160 + Math.sin(waveOffset + 6) * 10}C1248,${160 + Math.sin(waveOffset + 6.5) * 10},1344,${128 + Math.sin(waveOffset + 7) * 10},1392,${112 + Math.sin(waveOffset + 7.5) * 10}L1440,${96 + Math.sin(waveOffset + 8) * 10}L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z`
    path.setAttribute('d', d)

    animationFrame = requestAnimationFrame(animate)
  }

  animate()

  return () => {
    if (animationFrame) {
      cancelAnimationFrame(animationFrame)
    }
  }
}

// 星空背景效果
const initStars = () => {
  const count = props.config.count || 200
  const twinkleSpeed = props.config.twinkleSpeed || 2

  stars.value = []
  for (let i = 0; i < count; i++) {
    stars.value.push({
      x: Math.random() * 100,
      y: Math.random() * 100,
      delay: Math.random() * 3,
      duration: 1 + Math.random() * twinkleSpeed
    })
  }
}

let cleanup: (() => void) | null = null

watch(() => props.effect, (newEffect) => {
  if (cleanup) {
    cleanup()
    cleanup = null
  }

  if (animationFrame) {
    cancelAnimationFrame(animationFrame)
    animationFrame = null
  }

  if (newEffect === 'particles') {
    cleanup = initParticles()
  } else if (newEffect === 'waves') {
    cleanup = initWaves()
  } else if (newEffect === 'stars') {
    initStars()
  }
}, { immediate: true })

onMounted(() => {
  if (props.effect === 'particles') {
    cleanup = initParticles()
  } else if (props.effect === 'waves') {
    cleanup = initWaves()
  } else if (props.effect === 'stars') {
    initStars()
  }
})

onUnmounted(() => {
  if (cleanup) {
    cleanup()
  }
  if (animationFrame) {
    cancelAnimationFrame(animationFrame)
  }
})
</script>

<style scoped>
.background-particles {
  position: fixed;
  inset: 0;
  z-index: -1;
  pointer-events: none;
}

.particles-canvas {
  width: 100%;
  height: 100%;
}

.background-noise {
  position: fixed;
  inset: 0;
  z-index: -1;
  pointer-events: none;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E");
  opacity: 0.3;
  animation: noise-move 0.1s infinite;
}

@keyframes noise-move {
  0% { transform: translate(0, 0); }
  10% { transform: translate(-5%, -5%); }
  20% { transform: translate(-10%, 5%); }
  30% { transform: translate(5%, -10%); }
  40% { transform: translate(-5%, 15%); }
  50% { transform: translate(-10%, 5%); }
  60% { transform: translate(15%, 0); }
  70% { transform: translate(0, 10%); }
  80% { transform: translate(-15%, 0); }
  90% { transform: translate(10%, 5%); }
  100% { transform: translate(5%, 0); }
}

.background-waves {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: 200px;
  z-index: -1;
  pointer-events: none;
  overflow: hidden;
}

.waves-svg {
  width: 100%;
  height: 100%;
}

.background-stars {
  position: fixed;
  inset: 0;
  z-index: -1;
  pointer-events: none;
  overflow: hidden;
}

.star {
  position: absolute;
  width: 2px;
  height: 2px;
  background: white;
  border-radius: 50%;
  box-shadow: 0 0 4px white;
  animation: twinkle linear infinite;
}

@keyframes twinkle {
  0%, 100% {
    opacity: 0.3;
    transform: scale(1);
  }
  50% {
    opacity: 1;
    transform: scale(1.5);
  }
}
</style>

