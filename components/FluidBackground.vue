<template>
  <canvas ref="canvasRef" class="fixed inset-0 w-full h-full pointer-events-none -z-10" />
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

const props = withDefaults(defineProps<{
  intensity?: number
  color?: string
  interactive?: boolean
}>(), {
  intensity: 0.5,
  color: '#3b82f6',
  interactive: true
})

const canvasRef = ref<HTMLCanvasElement | null>(null)
let animationId: number | null = null
let mouseX = 0
let mouseY = 0

// 流体模拟参数
const gridSize = 32
const cellSize = 20
let grid: number[][] = []
let nextGrid: number[][] = []
let velocityX: number[][] = []
let velocityY: number[][] = []
let nextVelocityX: number[][] = []
let nextVelocityY: number[][] = []

const initGrid = () => {
  const cols = Math.ceil(window.innerWidth / cellSize) + 2
  const rows = Math.ceil(window.innerHeight / cellSize) + 2
  
  grid = Array(rows).fill(null).map(() => Array(cols).fill(0))
  nextGrid = Array(rows).fill(null).map(() => Array(cols).fill(0))
  velocityX = Array(rows).fill(null).map(() => Array(cols).fill(0))
  velocityY = Array(rows).fill(null).map(() => Array(cols).fill(0))
  nextVelocityX = Array(rows).fill(null).map(() => Array(cols).fill(0))
  nextVelocityY = Array(rows).fill(null).map(() => Array(cols).fill(0))
}

const updateFluid = () => {
  if (!canvasRef.value) return
  
  const cols = grid[0].length
  const rows = grid.length
  
  // 扩散
  const diff = 0.1
  const visc = 0.01
  
  for (let i = 1; i < rows - 1; i++) {
    for (let j = 1; j < cols - 1; j++) {
      // 密度扩散
      nextGrid[i][j] = grid[i][j] + diff * (
        grid[i + 1][j] + grid[i - 1][j] +
        grid[i][j + 1] + grid[i][j - 1] -
        4 * grid[i][j]
      )
      
      // 速度扩散
      nextVelocityX[i][j] = velocityX[i][j] + visc * (
        velocityX[i + 1][j] + velocityX[i - 1][j] +
        velocityX[i][j + 1] + velocityX[i][j - 1] -
        4 * velocityX[i][j]
      )
      
      nextVelocityY[i][j] = velocityY[i][j] + visc * (
        velocityY[i + 1][j] + velocityY[i - 1][j] +
        velocityY[i][j + 1] + velocityY[i][j - 1] -
        4 * velocityY[i][j]
      )
    }
  }
  
  // 对流
  for (let i = 1; i < rows - 1; i++) {
    for (let j = 1; j < cols - 1; j++) {
      const u = velocityX[i][j]
      const v = velocityY[i][j]
      
      const i0 = Math.floor(i - v)
      const j0 = Math.floor(j - u)
      const i1 = i0 + 1
      const j1 = j0 + 1
      
      if (i0 >= 0 && i1 < rows && j0 >= 0 && j1 < cols) {
        const s1 = i - v - i0
        const t1 = j - u - j0
        const s0 = 1 - s1
        const t0 = 1 - t1
        
        nextGrid[i][j] = s0 * (t0 * grid[i0][j0] + t1 * grid[i0][j1]) +
                         s1 * (t0 * grid[i1][j0] + t1 * grid[i1][j1])
      }
    }
  }
  
  // 边界条件
  for (let i = 0; i < rows; i++) {
    nextGrid[i][0] = 0
    nextGrid[i][cols - 1] = 0
    nextVelocityX[i][0] = 0
    nextVelocityX[i][cols - 1] = 0
    nextVelocityY[i][0] = 0
    nextVelocityY[i][cols - 1] = 0
  }
  
  for (let j = 0; j < cols; j++) {
    nextGrid[0][j] = 0
    nextGrid[rows - 1][j] = 0
    nextVelocityX[0][j] = 0
    nextVelocityX[rows - 1][j] = 0
    nextVelocityY[0][j] = 0
    nextVelocityY[rows - 1][j] = 0
  }
  
  // 衰减
  for (let i = 0; i < rows; i++) {
    for (let j = 0; j < cols; j++) {
      nextGrid[i][j] *= 0.995
      nextVelocityX[i][j] *= 0.99
      nextVelocityY[i][j] *= 0.99
    }
  }
  
  // 交换
  const temp = grid
  grid = nextGrid
  nextGrid = temp
  
  const tempVX = velocityX
  velocityX = nextVelocityX
  nextVelocityX = tempVX
  
  const tempVY = velocityY
  velocityY = nextVelocityY
  nextVelocityY = tempVY
}

const render = () => {
  if (!canvasRef.value) return
  
  const canvas = canvasRef.value
  const ctx = canvas.getContext('2d')
  if (!ctx) return
  
  canvas.width = window.innerWidth
  canvas.height = window.innerHeight
  
  ctx.clearRect(0, 0, canvas.width, canvas.height)
  
  const cols = grid[0].length
  const rows = grid.length
  
  // 解析颜色
  const color = hexToRgb(props.color)
  if (!color) return
  
  // 绘制流体
  for (let i = 1; i < rows - 1; i++) {
    for (let j = 1; j < cols - 1; j++) {
      const density = Math.min(1, Math.max(0, grid[i][j] * props.intensity))
      
      if (density > 0.01) {
        const alpha = density * 0.3
        const x = (j - 1) * cellSize
        const y = (i - 1) * cellSize
        
        ctx.fillStyle = `rgba(${color.r}, ${color.g}, ${color.b}, ${alpha})`
        ctx.fillRect(x, y, cellSize, cellSize)
      }
    }
  }
}

const hexToRgb = (hex: string) => {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
  return result ? {
    r: parseInt(result[1], 16),
    g: parseInt(result[2], 16),
    b: parseInt(result[3], 16)
  } : null
}

const handleMouseMove = (e: MouseEvent) => {
  if (!props.interactive) return
  
  mouseX = e.clientX
  mouseY = e.clientY
  
  const cols = grid[0].length
  const rows = grid.length
  
  const col = Math.floor(mouseX / cellSize) + 1
  const row = Math.floor(mouseY / cellSize) + 1
  
  if (row > 0 && row < rows - 1 && col > 0 && col < cols - 1) {
    // 添加密度
    grid[row][col] += 0.5
    grid[row + 1][col] += 0.3
    grid[row - 1][col] += 0.3
    grid[row][col + 1] += 0.3
    grid[row][col - 1] += 0.3
    
    // 添加速度
    const dx = (e.movementX || 0) * 0.1
    const dy = (e.movementY || 0) * 0.1
    
    velocityX[row][col] += dx
    velocityY[row][col] += dy
  }
}

const animate = () => {
  updateFluid()
  render()
  animationId = requestAnimationFrame(animate)
}

const handleResize = () => {
  initGrid()
}

onMounted(() => {
  if (!canvasRef.value) return
  
  initGrid()
  animate()
  
  if (props.interactive) {
    window.addEventListener('mousemove', handleMouseMove)
  }
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  window.removeEventListener('mousemove', handleMouseMove)
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
canvas {
  background: transparent;
}
</style>

