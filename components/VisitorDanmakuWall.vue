<template>
  <div class="danmaku-container">
    <!-- 弹幕项 -->
    <div
      v-for="(danmaku, index) in activeDanmakus"
      :key="danmaku.id"
      class="danmaku-item"
      :style="getDanmakuStyle(danmaku, index)"
    >
      <span v-if="danmaku.emoji" class="danmaku-emoji">{{ danmaku.emoji }}</span>
      <span class="danmaku-content">{{ danmaku.content }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Danmaku {
  id: number
  content: string
  emoji?: string
  color?: string
  messageType?: string
}

const props = withDefaults(defineProps<{
  maxCount?: number
  speed?: number
}>(), {
  maxCount: 10,
  speed: 50
})

const api = useApi()
const danmakus = ref<Danmaku[]>([])
const activeDanmakus = ref<Danmaku[]>([])

// 获取已审核的弹幕
const fetchDanmakus = async () => {
  try {
    const res = await api.get<Danmaku[]>('/VisitorInteraction/messages/approved?limit=100')
    if (res && Array.isArray(res)) {
      danmakus.value = res
      startDanmakuAnimation()
    }
  } catch (e) {
    console.error('获取弹幕失败', e)
  }
}

// 启动弹幕动画
const startDanmakuAnimation = () => {
  if (danmakus.value.length === 0) return

  const interval = setInterval(() => {
    if (activeDanmakus.value.length < props.maxCount && danmakus.value.length > 0) {
      const randomDanmaku = danmakus.value[Math.floor(Math.random() * danmakus.value.length)]
      if (randomDanmaku) {
        activeDanmakus.value.push({
          ...randomDanmaku,
          id: Date.now() + Math.random() // 确保唯一ID
        })
      }
    }
  }, 2000)

  // 清理过期的弹幕
  setInterval(() => {
    activeDanmakus.value = activeDanmakus.value.filter(d => {
      const element = document.querySelector(`[data-danmaku-id="${d.id}"]`)
      if (!element) return false
      const rect = element.getBoundingClientRect()
      return rect.right > 0
    })
  }, 1000)

  onUnmounted(() => {
    clearInterval(interval)
  })
}

// 获取弹幕样式
const getDanmakuStyle = (danmaku: Danmaku, index: number) => {
  const top = 10 + (index % 5) * 15 // 5行弹幕
  const duration = 15 + Math.random() * 10 // 15-25秒
  const color = danmaku.color || getRandomColor(danmaku.messageType)

  return {
    top: `${top}%`,
    color: color,
    animationDuration: `${duration}s`,
    animationDelay: `${Math.random() * 2}s`
  }
}

// 根据消息类型获取颜色
const getRandomColor = (type?: string): string => {
  const colors: Record<string, string[]> = {
    message: ['#60a5fa', '#34d399', '#fbbf24', '#a78bfa'],
    mood: ['#f472b6', '#fb923c', '#22d3ee'],
    blessing: ['#fbbf24', '#f59e0b', '#eab308']
  }

  const typeColors = colors[type || 'message'] || colors.message
  return typeColors[Math.floor(Math.random() * typeColors.length)]
}

onMounted(() => {
  fetchDanmakus()
  
  // 监听新弹幕事件
  if (process.client) {
    window.addEventListener('new-danmaku', ((e: CustomEvent) => {
      if (e.detail) {
        danmakus.value.unshift(e.detail)
        if (activeDanmakus.value.length < props.maxCount) {
          activeDanmakus.value.push({
            ...e.detail,
            id: Date.now() + Math.random()
          })
        }
      }
    }) as EventListener)
  }
})
</script>

<style scoped>
.danmaku-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 100;
  overflow: hidden;
}

.danmaku-item {
  position: absolute;
  left: 100%;
  white-space: nowrap;
  font-size: 0.875rem;
  font-weight: 500;
  padding: 0.25rem 0.75rem;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  border-radius: 1rem;
  animation: danmaku-move linear forwards;
  pointer-events: none;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.danmaku-emoji {
  font-size: 1rem;
}

.danmaku-content {
  line-height: 1.5;
}

@keyframes danmaku-move {
  from {
    transform: translateX(0);
  }
  to {
    transform: translateX(calc(-100vw - 200px));
  }
}
</style>

