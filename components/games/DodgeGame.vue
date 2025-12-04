<template>
  <div class="relative w-full h-full bg-gradient-to-b from-slate-900 to-slate-800 overflow-hidden">
    <!-- 关闭按钮 -->
    <button
      @click="$emit('close')"
      class="absolute top-4 right-4 z-20 w-10 h-10 bg-black/50 text-white rounded-lg backdrop-blur-md hover:bg-black/70 transition-colors flex items-center justify-center"
      aria-label="关闭游戏"
    >
      <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>

    <canvas ref="canvasRef" class="w-full h-full" />
    
    <!-- 游戏UI -->
    <div class="absolute top-4 left-4 right-4 flex justify-between items-start z-10">
      <div class="bg-black/50 text-white px-4 py-2 rounded-lg backdrop-blur-md">
        <div class="text-sm">分数: <span class="font-bold text-xl">{{ score }}</span></div>
        <div class="text-xs mt-1">最高分: {{ highScore }}</div>
      </div>
      
      <div v-if="!gameStarted" class="bg-black/50 text-white px-6 py-4 rounded-lg backdrop-blur-md text-center">
        <h3 class="text-xl font-bold mb-2">躲避方块</h3>
        <p class="text-sm mb-4">使用方向键或 WASD 移动</p>
        <button
          @click="startGame"
          class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition font-bold"
        >
          开始游戏
        </button>
      </div>
      
      <div v-if="gameOver" class="bg-black/50 text-white px-6 py-4 rounded-lg backdrop-blur-md text-center">
        <h3 class="text-xl font-bold mb-2 text-red-400">游戏结束</h3>
        <p class="text-sm mb-4">最终分数: {{ score }}</p>
        <button
          @click="restartGame"
          class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition font-bold"
        >
          重新开始
        </button>
      </div>
    </div>
    
    <!-- 游戏说明 -->
    <div v-if="gameStarted && !gameOver" class="absolute bottom-4 left-1/2 transform -translate-x-1/2 bg-black/50 text-white px-4 py-2 rounded-lg backdrop-blur-md text-sm z-10">
      ← → ↑ ↓ 或 WASD 移动 | 躲避红色方块
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

// 定义事件
defineEmits<{
  close: []
}>()

const canvasRef = ref<HTMLCanvasElement | null>(null)
const score = ref(0)
const highScore = ref(0)
const gameStarted = ref(false)
const gameOver = ref(false)

// 游戏对象
let player: { x: number; y: number; size: number; speed: number } | null = null
let enemies: Array<{ x: number; y: number; size: number; speed: number }> = []
let keys: { [key: string]: boolean } = {}
let animationId: number | null = null

const initGame = () => {
  if (!canvasRef.value) return
  
  const canvas = canvasRef.value
  canvas.width = canvas.offsetWidth
  canvas.height = canvas.offsetHeight
  
  // 初始化玩家
  player = {
    x: canvas.width / 2,
    y: canvas.height / 2,
    size: 20,
    speed: 5
  }
  
  // 初始化敌人
  enemies = []
  
  // 加载最高分
  if (process.client) {
    const saved = localStorage.getItem('game_dodge_high_score')
    if (saved) {
      highScore.value = parseInt(saved, 10)
    }
  }
}

const startGame = () => {
  gameStarted.value = true
  gameOver.value = false
  score.value = 0
  initGame()
  gameLoop()
  spawnEnemies()
}

const restartGame = () => {
  startGame()
}

const spawnEnemies = () => {
  if (!canvasRef.value || !gameStarted.value || gameOver.value) return
  
  const canvas = canvasRef.value
  const spawnInterval = setInterval(() => {
    if (!gameStarted.value || gameOver.value) {
      clearInterval(spawnInterval)
      return
    }
    
    // 随机生成敌人
    const side = Math.floor(Math.random() * 4)
    let x = 0
    let y = 0
    
    switch (side) {
      case 0: // 上
        x = Math.random() * canvas.width
        y = -20
        break
      case 1: // 右
        x = canvas.width + 20
        y = Math.random() * canvas.height
        break
      case 2: // 下
        x = Math.random() * canvas.width
        y = canvas.height + 20
        break
      case 3: // 左
        x = -20
        y = Math.random() * canvas.height
        break
    }
    
    enemies.push({
      x,
      y,
      size: 15 + Math.random() * 15,
      speed: 2 + Math.random() * 3 + score.value * 0.01
    })
  }, 1000 - Math.min(900, score.value * 10))
}

const updatePlayer = () => {
  if (!player || !canvasRef.value) return
  
  const canvas = canvasRef.value
  
  if (keys['ArrowLeft'] || keys['a'] || keys['A']) {
    player.x = Math.max(player.size, player.x - player.speed)
  }
  if (keys['ArrowRight'] || keys['d'] || keys['D']) {
    player.x = Math.min(canvas.width - player.size, player.x + player.speed)
  }
  if (keys['ArrowUp'] || keys['w'] || keys['W']) {
    player.y = Math.max(player.size, player.y - player.speed)
  }
  if (keys['ArrowDown'] || keys['s'] || keys['S']) {
    player.y = Math.min(canvas.height - player.size, player.y + player.speed)
  }
}

const updateEnemies = () => {
  if (!player || !canvasRef.value) return
  
  const canvas = canvasRef.value
  
  enemies.forEach((enemy, index) => {
    // 计算方向（朝向玩家）
    const dx = player.x - enemy.x
    const dy = player.y - enemy.y
    const distance = Math.sqrt(dx * dx + dy * dy)
    
    if (distance > 0) {
      enemy.x += (dx / distance) * enemy.speed
      enemy.y += (dy / distance) * enemy.speed
    }
    
    // 移除屏幕外的敌人
    if (
      enemy.x < -50 || enemy.x > canvas.width + 50 ||
      enemy.y < -50 || enemy.y > canvas.height + 50
    ) {
      enemies.splice(index, 1)
      score.value++
    }
    
    // 碰撞检测
    const dist = Math.sqrt(
      Math.pow(player.x - enemy.x, 2) + Math.pow(player.y - enemy.y, 2)
    )
    
    if (dist < player.size + enemy.size) {
      endGame()
    }
  })
}

const endGame = () => {
  gameOver.value = true
  gameStarted.value = false
  
  if (score.value > highScore.value) {
    highScore.value = score.value
    if (process.client) {
      localStorage.setItem('game_dodge_high_score', score.value.toString())
    }
  }
}

const draw = () => {
  if (!canvasRef.value || !player) return
  
  const canvas = canvasRef.value
  const ctx = canvas.getContext('2d')
  if (!ctx) return
  
  // 清空画布
  ctx.fillStyle = '#0f172a'
  ctx.fillRect(0, 0, canvas.width, canvas.height)
  
  // 绘制网格背景
  ctx.strokeStyle = '#1e293b'
  ctx.lineWidth = 1
  const gridSize = 50
  for (let x = 0; x < canvas.width; x += gridSize) {
    ctx.beginPath()
    ctx.moveTo(x, 0)
    ctx.lineTo(x, canvas.height)
    ctx.stroke()
  }
  for (let y = 0; y < canvas.height; y += gridSize) {
    ctx.beginPath()
    ctx.moveTo(0, y)
    ctx.lineTo(canvas.width, y)
    ctx.stroke()
  }
  
  // 绘制玩家
  ctx.fillStyle = '#3b82f6'
  ctx.beginPath()
  ctx.arc(player.x, player.y, player.size, 0, Math.PI * 2)
  ctx.fill()
  
  // 绘制玩家边框
  ctx.strokeStyle = '#60a5fa'
  ctx.lineWidth = 3
  ctx.stroke()
  
  // 绘制敌人
  enemies.forEach((enemy) => {
    ctx.fillStyle = '#ef4444'
    ctx.beginPath()
    ctx.arc(enemy.x, enemy.y, enemy.size, 0, Math.PI * 2)
    ctx.fill()
    
    ctx.strokeStyle = '#dc2626'
    ctx.lineWidth = 2
    ctx.stroke()
  })
}

const gameLoop = () => {
  if (!gameStarted.value || gameOver.value) return
  
  updatePlayer()
  updateEnemies()
  draw()
  
  animationId = requestAnimationFrame(gameLoop)
}

const handleKeyDown = (e: KeyboardEvent) => {
  keys[e.key] = true
}

const handleKeyUp = (e: KeyboardEvent) => {
  keys[e.key] = false
}

const handleResize = () => {
  if (canvasRef.value) {
    canvasRef.value.width = canvasRef.value.offsetWidth
    canvasRef.value.height = canvasRef.value.offsetHeight
  }
}

onMounted(() => {
  initGame()
  window.addEventListener('keydown', handleKeyDown)
  window.addEventListener('keyup', handleKeyUp)
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  window.removeEventListener('keydown', handleKeyDown)
  window.removeEventListener('keyup', handleKeyUp)
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
canvas {
  display: block;
}
</style>

