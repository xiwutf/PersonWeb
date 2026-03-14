<template>
  <div class="min-h-screen relative overflow-hidden font-['Outfit']" style="background-color: var(--bg); color: var(--text-main);">
    <!-- 全局背景噪点 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <!-- 动态背景光斑 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 left-1/4 w-[500px] h-[500px] bg-purple-600/20 rounded-full blur-[100px] animate-blob mix-blend-screen"></div>
      <div class="absolute bottom-0 right-1/4 w-[500px] h-[500px] bg-pink-600/20 rounded-full blur-[100px] animate-blob animation-delay-2000 mix-blend-screen"></div>
    </div>

    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 返回按钮 -->
      <div class="mb-8">
        <NuxtLink to="/" class="game-back-button">
          <svg class="game-back-button-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          <span>返回首页</span>
        </NuxtLink>
      </div>

      <!-- 页面头部 -->
      <header class="text-center mb-16 relative">
        <div class="inline-flex items-center justify-center w-20 h-20 rounded-2xl bg-gradient-to-br from-purple-500/20 to-pink-500/20 backdrop-blur-xl border border-var(--color-bg-light, white)/10 mb-6 shadow-lg shadow-purple-500/10 animate-float">
          <span class="text-4xl">🎮</span>
        </div>
        <h1 class="text-4xl md:text-5xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-purple-200 via-var(--color-bg-light, white) to-pink-200 tracking-tight">
          小游戏
        </h1>
        <p class="text-lg max-w-2xl mx-auto leading-relaxed" style="color: var(--text-secondary);">
          放松一下，玩个小游戏，测试你的反应能力和手速
        </p>
      </header>

      <!-- 游戏列表 -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        <TransitionGroup name="list">
          <div
            v-for="(game, index) in games"
            :key="game.id"
            class="game-card"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <div class="game-card-header">
              <div class="game-card-icon">
                {{ game.icon }}
              </div>
              <div class="game-card-info">
                <h3 class="game-card-title">{{ game.name }}</h3>
                <p class="game-card-description">{{ game.description }}</p>
              </div>
            </div>
            <div class="game-card-content">
              <div class="game-card-stats">
                <div class="game-card-stat">
                  <span class="game-card-stat-label">难度</span>
                  <div class="game-card-difficulty">
                    <span
                      v-for="i in 5"
                      :key="i"
                      class="game-card-difficulty-dot"
                      :class="{ 'game-card-difficulty-dot-active': i <= game.difficulty }"
                    ></span>
                  </div>
                </div>
                <div class="game-card-stat">
                  <span class="game-card-stat-label">最高分</span>
                  <span class="game-card-stat-value">{{ game.highScore || 0 }}</span>
                </div>
              </div>
            </div>
            <div class="game-card-footer">
              <button
                @click="startGame(game)"
                class="game-card-play-btn"
              >
                开始游戏
              </button>
            </div>
          </div>
        </TransitionGroup>
      </div>
    </div>

    <!-- 游戏画布（全屏显示） -->
    <div
      v-if="currentGame"
      class="fixed inset-0 z-50 bg-slate-900"
    >
      <component :is="currentGame.component" @close="closeGame" />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Game {
  id: string
  name: string
  description: string
  icon: string
  difficulty: number
  component: any
  highScore?: number
}

const games = ref<Game[]>([
  {
    id: 'dodge',
    name: '躲避方块',
    description: '使用方向键或WASD移动，躲避红色方块，坚持越久分数越高',
    icon: '🎯',
    difficulty: 3,
    component: resolveComponent('DodgeGame'),
    highScore: 0
  }
])

const currentGame = ref<Game | null>(null)

// 加载最高分
onMounted(() => {
  if (process.client) {
    games.value.forEach(game => {
      const saved = localStorage.getItem(`game_${game.id}_high_score`)
      if (saved) {
        game.highScore = parseInt(saved, 10)
      }
    })
  }
})

const startGame = (game: Game) => {
  currentGame.value = game
}

const closeGame = () => {
  currentGame.value = null
  // 重新加载最高分
  if (process.client) {
    games.value.forEach(game => {
      const saved = localStorage.getItem(`game_${game.id}_high_score`)
      if (saved) {
        game.highScore = parseInt(saved, 10)
      }
    })
  }
}

definePageMeta({
  layout: 'default'
})

useHead({
  title: '小游戏 - 溪午听风',
  meta: [
    { name: 'description', content: '放松一下，玩个小游戏，测试你的反应能力' }
  ]
})
</script>

<style scoped>
/* 返回按钮 */
.game-back-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 0.5rem;
  color: var(--text-secondary);
  text-decoration: none;
  transition: all 0.3s ease;
  font-size: 0.875rem;
}

.game-back-button:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  color: var(--text-main);
}

.game-back-button-icon {
  width: 1.25rem;
  height: 1.25rem;
}

/* 游戏卡片 */
.game-card {
  background: var(--bg-card);
  backdrop-filter: blur(12px);
  border: 1px solid var(--border-color);
  border-radius: var(--card-radius, 1.5rem);
  padding: 1.5rem;
  transition: all 0.5s ease;
  display: flex;
  flex-direction: column;
}

.game-card:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  box-shadow: var(--shadow-soft);
  transform: translateY(-4px);
}

.game-card-header {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.game-card-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 1rem;
  background: rgba(147, 51, 234, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  flex-shrink: 0;
}

.game-card-info {
  flex: 1;
  min-width: 0;
}

.game-card-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0 0 0.5rem 0;
}

.game-card-description {
  font-size: 0.875rem;
  color: var(--text-secondary);
  margin: 0;
  line-height: 1.5;
}

.game-card-content {
  flex: 1;
  margin-bottom: 1rem;
}

.game-card-stats {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: rgba(15, 23, 42, 0.5);
  border-radius: 0.75rem;
}

.game-card-stat {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.game-card-stat-label {
  font-size: 0.75rem;
  color: var(--text-secondary);
}

.game-card-stat-value {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
}

.game-card-difficulty {
  display: flex;
  gap: 0.25rem;
}

.game-card-difficulty-dot {
  width: 0.5rem;
  height: 0.5rem;
  border-radius: 9999px;
  background: rgba(148, 163, 184, 0.3);
}

.game-card-difficulty-dot-active {
  background: rgb(147, 51, 234);
}

.game-card-footer {
  margin-top: auto;
}

.game-card-play-btn {
  width: 100%;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(to right, rgb(147, 51, 234), rgb(236, 72, 153));
  color: var(--color-bg-light, white);
  border: none;
  border-radius: 0.75rem;
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.game-card-play-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(147, 51, 234, 0.3);
}

/* 动画 */
.list-enter-active,
.list-leave-active {
  transition: all 0.5s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(30px);
}

.animate-blob {
  animation: blob 7s infinite;
}
.animation-delay-2000 {
  animation-delay: 2s;
}
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -50px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}
</style>

