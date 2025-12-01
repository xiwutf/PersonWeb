<template>
  <div ref="trailRef" class="fixed inset-0 pointer-events-none z-50" style="z-index: 50 !important;" />
  <canvas ref="canvasRef" class="fixed inset-0 pointer-events-none z-40" style="z-index: 40 !important;" />
</template>

<script setup lang="ts">
const trailRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)
const trail: Array<{ x: number; y: number; element: HTMLElement; time: number }> = []
const maxTrailLength = 15

// Canvas 粒子系统
let ctx: CanvasRenderingContext2D | null = null
let particles: Array<{
  x: number
  y: number
  vx: number
  vy: number
  life: number
  maxLife: number
  size: number
  color: string
}> = []
let animationFrame: number | null = null
let lastMouseX = 0
let lastMouseY = 0

const initCanvas = () => {
  if (!canvasRef.value) return
  
  const canvas = canvasRef.value
  ctx = canvas.getContext('2d')
  if (!ctx) return

  const resize = () => {
    const dpr = window.devicePixelRatio || 1
    canvas.width = window.innerWidth * dpr
    canvas.height = window.innerHeight * dpr
    if (ctx) {
      ctx.setTransform(1, 0, 0, 1, 0, 0) // 重置变换
      ctx.scale(dpr, dpr) // 缩放以匹配设备像素比
    }
  }
  resize()
  
  // 防抖处理 resize 事件
  let resizeTimer: NodeJS.Timeout | null = null
  const handleResize = () => {
    if (resizeTimer) clearTimeout(resizeTimer)
    resizeTimer = setTimeout(() => {
      resize()
    }, 100)
  }
  window.addEventListener('resize', handleResize)

  const animate = () => {
    if (!ctx || !canvasRef.value) {
      animationFrame = requestAnimationFrame(animate)
      return
    }
    
    ctx.clearRect(0, 0, canvasRef.value.width, canvasRef.value.height)
    
    // 更新和绘制粒子
    particles = particles.filter(p => {
      p.x += p.vx
      p.y += p.vy
      p.vx *= 0.98 // 摩擦力
      p.vy *= 0.98
      p.life--
      
      const alpha = Math.max(0, Math.min(1, p.life / p.maxLife)) // 确保 alpha 在 0-1 范围内
      const radius = p.size * alpha
      
      // 只在半径大于0时才绘制，避免负数半径错误
      if (radius > 0 && alpha > 0) {
        ctx.globalAlpha = alpha
        ctx.fillStyle = p.color
        ctx.beginPath()
        ctx.arc(p.x, p.y, radius, 0, Math.PI * 2)
        ctx.fill()
      }
      
      return p.life > 0
    })
    
    animationFrame = requestAnimationFrame(animate)
  }
  
  animate()
  
  // 返回清理函数
  return () => {
    window.removeEventListener('resize', handleResize)
    if (resizeTimer) clearTimeout(resizeTimer)
  }
}

// 创建爆裂粒子
const createBurst = (x: number, y: number, color: string = '#3b82f6') => {
  const count = 8
  for (let i = 0; i < count; i++) {
    const angle = (Math.PI * 2 * i) / count
    const speed = 2 + Math.random() * 3
    particles.push({
      x,
      y,
      vx: Math.cos(angle) * speed,
      vy: Math.sin(angle) * speed,
      life: 30 + Math.random() * 20,
      maxLife: 50,
      size: 2 + Math.random() * 3,
      color
    })
  }
}

// 创建能量波纹
const createRipple = (x: number, y: number) => {
  if (!trailRef.value) return
  
  // 确保坐标在有效范围内
  const clampedX = Math.max(0, Math.min(x, window.innerWidth))
  const clampedY = Math.max(0, Math.min(y, window.innerHeight))
  
  const ripple = document.createElement('div')
  ripple.className = 'absolute rounded-full border-2 border-blue-400 pointer-events-none'
  ripple.style.position = 'fixed' // 确保使用 fixed 定位
  ripple.style.left = `${clampedX}px`
  ripple.style.top = `${clampedY}px`
  ripple.style.transform = 'translate(-50%, -50%)'
  ripple.style.width = '0px'
  ripple.style.height = '0px'
  ripple.style.opacity = '0.8'
  ripple.style.zIndex = '9999' // 确保在最上层
  trailRef.value.appendChild(ripple)

  // 动画
  let size = 0
  const animate = () => {
    size += 15
    const opacity = 0.8 - (size / 200)
    
    if (opacity > 0) {
      ripple.style.width = `${size}px`
      ripple.style.height = `${size}px`
      ripple.style.opacity = opacity.toString()
      requestAnimationFrame(animate)
    } else {
      if (ripple.parentNode) {
        ripple.parentNode.removeChild(ripple)
      }
    }
  }
  animate()
}

const createTrailPoint = (x: number, y: number) => {
  if (!trailRef.value) return

  // 确保坐标在有效范围内
  const clampedX = Math.max(0, Math.min(x, window.innerWidth))
  const clampedY = Math.max(0, Math.min(y, window.innerHeight))

  // 计算速度
  const dx = clampedX - lastMouseX
  const dy = clampedY - lastMouseY
  const speed = Math.sqrt(dx * dx + dy * dy)
  
  // 根据速度调整粒子数量
  if (speed > 5) {
    createBurst(clampedX, clampedY, `hsl(${(Date.now() / 10) % 360}, 70%, 60%)`)
  }

  const point = document.createElement('div')
  const hue = (Date.now() / 10) % 360
  point.className = 'absolute rounded-full pointer-events-none transition-all duration-500'
  point.style.position = 'fixed' // 确保使用 fixed 定位
  point.style.left = `${clampedX}px`
  point.style.top = `${clampedY}px`
  point.style.width = '6px'
  point.style.height = '6px'
  point.style.background = `radial-gradient(circle, hsl(${hue}, 100%, 70%), hsl(${hue}, 80%, 50%))`
  point.style.transform = 'translate(-50%, -50%)'
  point.style.boxShadow = `0 0 10px hsl(${hue}, 100%, 60%), 0 0 20px hsl(${hue}, 100%, 50%)`
  point.style.opacity = '0.9'
  point.style.zIndex = '9999' // 确保在最上层
  trailRef.value.appendChild(point)

  trail.push({ x: clampedX, y: clampedY, element: point, time: Date.now() })

  // 限制轨迹长度
  if (trail.length > maxTrailLength) {
    const old = trail.shift()
    if (old && old.element.parentNode) {
      old.element.style.opacity = '0'
      old.element.style.transform = 'translate(-50%, -50%) scale(0)'
      setTimeout(() => {
        if (old.element.parentNode) {
          old.element.parentNode.removeChild(old.element)
        }
      }, 500)
    }
  }

  // 逐渐消失效果
  setTimeout(() => {
    if (point.parentNode) {
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
      }, 500)
    }
  }, 800)

  lastMouseX = clampedX
  lastMouseY = clampedY
}

const handleMouseMove = (e: MouseEvent) => {
  // 使用 clientX/clientY（相对于视口），因为 canvas 是 fixed 定位
  // 确保坐标在视口范围内
  const x = Math.max(0, Math.min(e.clientX, window.innerWidth))
  const y = Math.max(0, Math.min(e.clientY, window.innerHeight))
  createTrailPoint(x, y)
}

const handleClick = (e: MouseEvent) => {
  // 使用 clientX/clientY（相对于视口），因为 canvas 是 fixed 定位
  const x = Math.max(0, Math.min(e.clientX, window.innerWidth))
  const y = Math.max(0, Math.min(e.clientY, window.innerHeight))
  createRipple(x, y)
  createBurst(x, y, '#60a5fa')
}

let resizeCleanup: (() => void) | null = null

onMounted(() => {
  resizeCleanup = initCanvas() || null
  window.addEventListener('mousemove', handleMouseMove)
  window.addEventListener('click', handleClick)
})

onUnmounted(() => {
  window.removeEventListener('mousemove', handleMouseMove)
  window.removeEventListener('click', handleClick)
  if (animationFrame) {
    cancelAnimationFrame(animationFrame)
  }
  if (resizeCleanup) {
    resizeCleanup()
  }
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

