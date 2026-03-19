<template>
  <div class="relative h-full w-full overflow-hidden bg-gradient-to-b from-slate-900 to-slate-800">
    <button
      @click="$emit('close')"
      class="absolute right-4 top-4 z-20 flex h-10 w-10 items-center justify-center rounded-lg bg-black/50 text-white backdrop-blur-md transition-colors hover:bg-black/70"
      aria-label="关闭游戏"
    >
      <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18 18 6M6 6l12 12" />
      </svg>
    </button>

    <canvas ref="canvasRef" class="h-full w-full" />

    <div class="absolute left-4 right-4 top-4 z-10 flex items-start justify-between gap-4">
      <div class="rounded-lg bg-black/50 px-4 py-2 text-white backdrop-blur-md">
        <div class="text-sm">
          分数: <span class="text-xl font-bold">{{ score }}</span>
        </div>
        <div class="mt-1 text-xs">最高分: {{ highScore }}</div>
      </div>

      <div v-if="!gameStarted" class="rounded-lg bg-black/50 px-6 py-4 text-center text-white backdrop-blur-md">
        <h3 class="mb-2 text-xl font-bold">躲避方块</h3>
        <p class="mb-4 text-sm">使用方向键或 WASD 移动</p>
        <button
          @click="startGame"
          class="rounded-lg bg-blue-600 px-6 py-2 font-bold text-white transition hover:bg-blue-700"
        >
          开始游戏
        </button>
      </div>

      <div v-if="gameOver" class="rounded-lg bg-black/50 px-6 py-4 text-center text-white backdrop-blur-md">
        <h3 class="mb-2 text-xl font-bold text-red-400">游戏结束</h3>
        <p class="mb-4 text-sm">最终分数: {{ score }}</p>
        <button
          @click="restartGame"
          class="rounded-lg bg-green-600 px-6 py-2 font-bold text-white transition hover:bg-green-700"
        >
          重新开始
        </button>
      </div>
    </div>

    <div
      v-if="gameStarted && !gameOver"
      class="absolute bottom-4 left-1/2 z-10 -translate-x-1/2 rounded-lg bg-black/50 px-4 py-2 text-sm text-white backdrop-blur-md"
    >
      ← → ↑ ↓ 或 WASD 移动 | 躲避红色方块
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'

defineEmits<{
  close: []
}>()

const canvasRef = ref<HTMLCanvasElement | null>(null)
const score = ref(0)
const highScore = ref(0)
const gameStarted = ref(false)
const gameOver = ref(false)

let player: { x: number; y: number; size: number; speed: number } | null = null
let enemies: Array<{ x: number; y: number; size: number; speed: number }> = []
let keys: Record<string, boolean> = {}
let animationId: number | null = null
let spawnTimer: number | null = null

const initGame = () => {
  if (!canvasRef.value) return

  const canvas = canvasRef.value
  canvas.width = canvas.offsetWidth
  canvas.height = canvas.offsetHeight

  player = {
    x: canvas.width / 2,
    y: canvas.height / 2,
    size: 20,
    speed: 5
  }

  enemies = []

  if (process.client) {
    const saved = localStorage.getItem('game_dodge_high_score')
    if (saved) {
      highScore.value = parseInt(saved, 10)
    }
  }
}

const clearSpawnTimer = () => {
  if (spawnTimer) {
    window.clearInterval(spawnTimer)
    spawnTimer = null
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
  clearSpawnTimer()
  startGame()
}

const spawnEnemies = () => {
  if (!canvasRef.value || !gameStarted.value || gameOver.value) return

  const canvas = canvasRef.value
  clearSpawnTimer()

  spawnTimer = window.setInterval(() => {
    if (!gameStarted.value || gameOver.value) {
      clearSpawnTimer()
      return
    }

    const side = Math.floor(Math.random() * 4)
    let x = 0
    let y = 0

    switch (side) {
      case 0:
        x = Math.random() * canvas.width
        y = -20
        break
      case 1:
        x = canvas.width + 20
        y = Math.random() * canvas.height
        break
      case 2:
        x = Math.random() * canvas.width
        y = canvas.height + 20
        break
      default:
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

  if (keys.ArrowLeft || keys.a || keys.A) {
    player.x = Math.max(player.size, player.x - player.speed)
  }
  if (keys.ArrowRight || keys.d || keys.D) {
    player.x = Math.min(canvas.width - player.size, player.x + player.speed)
  }
  if (keys.ArrowUp || keys.w || keys.W) {
    player.y = Math.max(player.size, player.y - player.speed)
  }
  if (keys.ArrowDown || keys.s || keys.S) {
    player.y = Math.min(canvas.height - player.size, player.y + player.speed)
  }
}

const endGame = () => {
  gameOver.value = true
  gameStarted.value = false
  clearSpawnTimer()

  if (score.value > highScore.value) {
    highScore.value = score.value
    if (process.client) {
      localStorage.setItem('game_dodge_high_score', score.value.toString())
    }
  }
}

const updateEnemies = () => {
  if (!player || !canvasRef.value) return

  const canvas = canvasRef.value

  enemies = enemies.filter((enemy) => {
    const dx = player!.x - enemy.x
    const dy = player!.y - enemy.y
    const distance = Math.sqrt(dx * dx + dy * dy) || 1

    enemy.x += (dx / distance) * enemy.speed
    enemy.y += (dy / distance) * enemy.speed

    const dist = Math.sqrt((player!.x - enemy.x) ** 2 + (player!.y - enemy.y) ** 2)
    if (dist < player!.size + enemy.size) {
      endGame()
      return true
    }

    const outOfBounds =
      enemy.x < -50 ||
      enemy.x > canvas.width + 50 ||
      enemy.y < -50 ||
      enemy.y > canvas.height + 50

    if (outOfBounds) {
      score.value += 1
      return false
    }

    return true
  })
}

const draw = () => {
  if (!canvasRef.value || !player) return

  const canvas = canvasRef.value
  const ctx = canvas.getContext('2d')
  if (!ctx) return

  ctx.fillStyle = '#0f172a'
  ctx.fillRect(0, 0, canvas.width, canvas.height)

  ctx.strokeStyle = 'rgba(148, 163, 184, 0.12)'
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

  ctx.fillStyle = '#60a5fa'
  ctx.beginPath()
  ctx.arc(player.x, player.y, player.size, 0, Math.PI * 2)
  ctx.fill()

  ctx.strokeStyle = '#bfdbfe'
  ctx.lineWidth = 3
  ctx.stroke()

  enemies.forEach((enemy) => {
    ctx.fillStyle = '#ef4444'
    ctx.beginPath()
    ctx.arc(enemy.x, enemy.y, enemy.size, 0, Math.PI * 2)
    ctx.fill()

    ctx.strokeStyle = '#fca5a5'
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

const handleKeyDown = (event: KeyboardEvent) => {
  keys[event.key] = true
}

const handleKeyUp = (event: KeyboardEvent) => {
  keys[event.key] = false
}

const handleResize = () => {
  initGame()
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
  clearSpawnTimer()
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
