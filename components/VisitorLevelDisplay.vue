<template>
  <div v-if="levelInfo" class="visitor-level-display">
    <div class="level-badge" :class="`level-${levelInfo.level}`">
      <span class="badge-icon">{{ levelInfo.badge }}</span>
      <span class="badge-level">Lv.{{ levelInfo.level }}</span>
    </div>
    <div class="level-info">
      <div class="level-title">{{ levelInfo.title }}</div>
      <div class="level-progress">
        <div class="progress-bar">
          <div 
            class="progress-fill" 
            :style="{ width: `${levelInfo.progress}%` }"
          ></div>
        </div>
        <div class="progress-text">
          {{ levelInfo.experience }} / {{ levelInfo.nextLevelExp || 'MAX' }} EXP
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()

interface LevelInfo {
  level: number
  experience: number
  totalExperience: number
  title: string
  badge: string
  nextLevelExp: number
  progress: number
}

const levelInfo = ref<LevelInfo | null>(null)
const loading = ref(false)

const fetchLevel = async () => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) return

  loading.value = true
  try {
    const res = await api.post('/VisitorGamification/level', { visitorId })
    if (res) {
      levelInfo.value = res as LevelInfo
    }
  } catch (e) {
    console.error('获取等级失败', e)
  } finally {
    loading.value = false
  }
}

// 监听升级事件
onMounted(() => {
  fetchLevel()

  if (process.client) {
    // 监听行为记录后的升级事件
    window.addEventListener('visitor-level-updated', () => {
      fetchLevel()
    })
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('visitor-level-updated', () => {})
  }
})
</script>

<style scoped>
.visitor-level-display {
  position: relative; /* 改为相对定位，由父容器控制位置 */
  background: var(--color-bg-card, rgba(30, 41, 59, 0.95));
  backdrop-filter: blur(20px);
  border: 1px solid var(--color-border-default, rgba(255, 255, 255, 0.2));
  border-radius: var(--radius-md, 0.75rem);
  padding: 0.75rem;
  width: 100%;
  box-shadow: var(--shadow-md, 0 4px 12px rgba(0, 0, 0, 0.3));
}

.level-badge {
  display: flex;
  align-items: center;
  gap: 0.375rem; /* 减小间距 */
  margin-bottom: 0.5rem; /* 减小下边距 */
  padding: 0.375rem; /* 减小内边距 */
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.1));
  border-radius: var(--radius-sm, 0.375rem);
}

.badge-icon {
  font-size: 1.25rem; /* 减小图标大小 */
}

.badge-level {
  font-size: 0.75rem; /* 减小字体大小 */
  font-weight: 600;
  color: white;
}

.level-info {
  color: white;
}

.level-title {
  font-size: 0.75rem; /* 减小字体大小 */
  font-weight: 500;
  margin-bottom: 0.375rem; /* 减小下边距 */
  color: rgba(255, 255, 255, 0.9);
}

.level-progress {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.progress-bar {
  width: 100%;
  height: 6px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 3px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-purple-500, #8b5cf6));
  border-radius: var(--radius-sm, 3px);
  transition: width 0.3s ease;
}

.progress-text {
  font-size: 0.625rem; /* 减小字体大小 */
  color: rgba(255, 255, 255, 0.7);
  text-align: right;
}

/* 不同等级的样式 */
.level-1 .badge-icon { color: var(--color-gray-400, #9ca3af); }
.level-2 .badge-icon { color: var(--color-primary-soft); }
.level-3 .badge-icon { color: var(--color-success, #34d399); }
.level-4 .badge-icon { color: var(--color-warning, #fbbf24); }
.level-5 .badge-icon { color: var(--color-purple-500, #a78bfa); }
.level-10 .badge-icon { color: var(--color-warning, #fbbf24); animation: glow 2s ease-in-out infinite; }

@keyframes glow {
  0%, 100% { filter: drop-shadow(0 0 5px currentColor); }
  50% { filter: drop-shadow(0 0 15px currentColor); }
}
</style>

