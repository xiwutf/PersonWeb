<template>
  <div ref="containerRef" class="relative w-full h-full">
    <canvas ref="canvasRef" class="w-full h-full" />
    <div
      v-if="showText"
      class="absolute inset-0 flex items-center justify-center pointer-events-none z-10"
    >
      <div class="text-center">
        <h3 class="text-2xl font-bold text-white mb-2">{{ displayText }}</h3>
        <p v-if="subtitle" class="text-sm text-gray-300">{{ subtitle }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const props = withDefaults(defineProps<{
  imageUrl?: string
  text?: string
  subtitle?: string
  particleCount?: number
  interactive?: boolean
}>(), {
  imageUrl: '/images/avatar.jpg',
  text: '欢迎',
  subtitle: '',
  particleCount: 2000,
  interactive: true
})

const containerRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)
const showText = ref(false)
const displayText = ref(props.text)

let ctx: CanvasRenderingContext2D | null = null
let image: HTMLImageElement | null = null
let particles: Array<{
  x: number
  y: number
  originalX: number
  originalY: number
  vx: number
  vy: number
  color: string
  size: number
}> = []
let animationFrame: number | null = null
let mouseX = 0
let mouseY = 0
let isMouseOver = false

const initParticles = () => {
  if (!canvasRef.value || !image || !ctx) return

  const canvas = canvasRef.value
  const width = canvas.width
  const height = canvas.height

  // 创建临时canvas来读取图像数据
  const tempCanvas = document.createElement('canvas')
  tempCanvas.width = width
  tempCanvas.height = height
  const tempCtx = tempCanvas.getContext('2d')
  if (!tempCtx) return

  tempCtx.drawImage(image, 0, 0, width, height)
  const imageData = tempCtx.getImageData(0, 0, width, height)
  const data = imageData.data

  particles = []

  // 采样图像像素创建粒子
  const step = Math.ceil(Math.sqrt((width * height) / props.particleCount))
  
  for (let y = 0; y < height; y += step) {
    for (let x = 0; x < width; x += step) {
      const index = (y * width + x) * 4
      const r = data[index]
      const g = data[index + 1]
      const b = data[index + 2]
      const a = data[index + 3]

      // 只处理不透明的像素
      if (a > 128) {
        particles.push({
          x: x + width / 2,
          y: y + height / 2,
          originalX: x + width / 2,
          originalY: y + height / 2,
          vx: 0,
          vy: 0,
          color: `rgb(${r}, ${g}, ${b})`,
          size: 1 + Math.random() * 1.5
        })
      }
    }
  }
}

const animate = () => {
  if (!ctx || !canvasRef.value) return

  ctx.clearRect(0, 0, canvasRef.value.width, canvasRef.value.height)

  particles.forEach(particle => {
    if (isMouseOver && props.interactive) {
      // 鼠标悬停时粒子爆炸
      const dx = particle.x - mouseX
      const dy = particle.y - mouseY
      const distance = Math.sqrt(dx * dx + dy * dy)
      const force = Math.min(200 / (distance + 1), 5)

      particle.vx += (dx / distance) * force * 0.1
      particle.vy += (dy / distance) * force * 0.1
    } else {
      // 恢复原始位置
      const dx = particle.originalX - particle.x
      const dy = particle.originalY - particle.y
      const distance = Math.sqrt(dx * dx + dy * dy)

      if (distance > 1) {
        particle.vx += dx * 0.05
        particle.vy += dy * 0.05
      } else {
        particle.vx *= 0.9
        particle.vy *= 0.9
      }
    }

    // 应用速度
    particle.x += particle.vx
    particle.y += particle.vy

    // 摩擦力
    particle.vx *= 0.95
    particle.vy *= 0.95

    // 绘制粒子
    ctx.fillStyle = particle.color
    ctx.beginPath()
    ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2)
    ctx.fill()
  })

  animationFrame = requestAnimationFrame(animate)
}

const handleMouseMove = (e: MouseEvent) => {
  if (!containerRef.value) return
  
  const rect = containerRef.value.getBoundingClientRect()
  mouseX = e.clientX - rect.left
  mouseY = e.clientY - rect.top
}

const handleMouseEnter = () => {
  isMouseOver = true
  showText.value = true
  displayText.value = props.text
}

const handleMouseLeave = () => {
  isMouseOver = false
  setTimeout(() => {
    if (!isMouseOver) {
      showText.value = false
    }
  }, 2000)
}

const init = async () => {
  if (!canvasRef.value || !containerRef.value) return

  const canvas = canvasRef.value
  ctx = canvas.getContext('2d', { willReadFrequently: true })
  if (!ctx) {
    console.error('Failed to get 2d context')
    return
  }

  // 设置canvas大小
  const resize = () => {
    if (!containerRef.value || !canvasRef.value) return
    const rect = containerRef.value.getBoundingClientRect()
    canvas.width = rect.width || 400
    canvas.height = rect.height || 400
  }
  resize()
  
  // 延迟一下确保容器已渲染
  await new Promise(resolve => setTimeout(resolve, 100))
  resize()
  
  window.addEventListener('resize', resize)

  // 加载图像
  image = new Image()
  
  // 确保图片路径正确（如果是相对路径，确保以 / 开头）
  const imageUrl = props.imageUrl.startsWith('/') 
    ? props.imageUrl 
    : `/${props.imageUrl}`
  
  image.src = imageUrl

  image.onload = () => {
    console.log('Avatar image loaded successfully:', imageUrl)
    if (ctx && canvasRef.value) {
      initParticles()
      animate()
    }
  }

  image.onerror = (error) => {
    console.error('Failed to load avatar image:', imageUrl, error)
    // 如果图像加载失败，显示文字提示
    showText.value = true
    displayText.value = props.text || '头像加载失败'
    
    // 创建一个简单的占位符粒子效果
    if (canvasRef.value && ctx) {
      const width = canvasRef.value.width
      const height = canvasRef.value.height
      
      // 创建简单的粒子网格作为占位符
      particles = []
      const cols = 20
      const rows = 20
      const cellWidth = width / cols
      const cellHeight = height / rows
      
      for (let i = 0; i < cols; i++) {
        for (let j = 0; j < rows; j++) {
          particles.push({
            x: i * cellWidth + cellWidth / 2,
            y: j * cellHeight + cellHeight / 2,
            originalX: i * cellWidth + cellWidth / 2,
            originalY: j * cellHeight + cellHeight / 2,
            vx: 0,
            vy: 0,
            color: `rgb(${Math.random() * 100 + 100}, ${Math.random() * 100 + 100}, ${Math.random() * 100 + 200})`,
            size: 2 + Math.random() * 2
          })
        }
      }
      
      animate()
    }
  }

  // 鼠标事件
  if (props.interactive) {
    containerRef.value.addEventListener('mousemove', handleMouseMove)
    containerRef.value.addEventListener('mouseenter', handleMouseEnter)
    containerRef.value.addEventListener('mouseleave', handleMouseLeave)
  }
}

onMounted(() => {
  init()
})

onUnmounted(() => {
  if (animationFrame) {
    cancelAnimationFrame(animationFrame)
  }
  if (containerRef.value && props.interactive) {
    containerRef.value.removeEventListener('mousemove', handleMouseMove)
    containerRef.value.removeEventListener('mouseenter', handleMouseEnter)
    containerRef.value.removeEventListener('mouseleave', handleMouseLeave)
  }
})
</script>

<style scoped>
canvas {
  display: block;
  width: 100%;
  height: 100%;
}
</style>

