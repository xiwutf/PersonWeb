<template>
  <div v-if="isDanmakuEnabled" class="danmaku-container">
    <div
      v-for="(danmaku, index) in activeDanmakus"
      :key="danmaku.id"
      class="danmaku-item"
      :data-danmaku-id="danmaku.id"
      :class="getDanmakuClass(index)"
      :style="getDanmakuDynamicStyle(danmaku)"
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
const isDanmakuEnabled = ref(true)

let spawnTimer: NodeJS.Timeout | null = null
let cleanupTimer: NodeJS.Timeout | null = null
let visibilityHandler: (() => void) | null = null
let newDanmakuHandler: EventListener | null = null

const detectConstrainedDevice = () => {
  const reducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const narrowScreen = window.innerWidth < 1024
  const saveData = navigator.connection?.saveData === true
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  isDanmakuEnabled.value = !(reducedMotion || coarsePointer || narrowScreen || saveData || lowMemory)
}

const fetchDanmakus = async () => {
  if (!isDanmakuEnabled.value) return

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

const stopDanmakuAnimation = () => {
  if (spawnTimer) {
    clearInterval(spawnTimer)
    spawnTimer = null
  }

  if (cleanupTimer) {
    clearInterval(cleanupTimer)
    cleanupTimer = null
  }
}

const startDanmakuAnimation = () => {
  if (!isDanmakuEnabled.value || danmakus.value.length === 0 || spawnTimer || cleanupTimer) return

  spawnTimer = setInterval(() => {
    if (document.hidden) return
    if (activeDanmakus.value.length >= props.maxCount || danmakus.value.length === 0) return

    const randomDanmaku = danmakus.value[Math.floor(Math.random() * danmakus.value.length)]
    if (!randomDanmaku) return

    activeDanmakus.value.push({
      ...randomDanmaku,
      id: Date.now() + Math.random()
    })
  }, 2200)

  cleanupTimer = setInterval(() => {
    if (document.hidden) return

    activeDanmakus.value = activeDanmakus.value.filter(item => {
      const element = document.querySelector(`[data-danmaku-id="${item.id}"]`)
      if (!element) return false
      return element.getBoundingClientRect().right > 0
    })
  }, 1200)
}

const getDanmakuClass = (index: number) => {
  const row = index % 5
  return `danmaku-row-${row}`
}

const getDanmakuDynamicStyle = (danmaku: Danmaku) => {
  const duration = 16 + Math.random() * 8
  const color = danmaku.color || getRandomColor(danmaku.messageType)

  return {
    color,
    animationDuration: `${duration}s`,
    animationDelay: `${Math.random() * 1.5}s`
  }
}

const getRandomColor = (type?: string): string => {
  const colors: Record<string, string[]> = {
    message: ['var(--color-primary-soft)', '#34d399', '#fbbf24', '#a78bfa'],
    mood: ['#f472b6', '#fb923c', '#22d3ee'],
    blessing: ['#fbbf24', '#f59e0b', '#eab308']
  }

  const typeColors = colors[type || 'message'] || colors.message
  return typeColors[Math.floor(Math.random() * typeColors.length)]
}

onMounted(() => {
  detectConstrainedDevice()
  if (!isDanmakuEnabled.value) return

  fetchDanmakus()

  visibilityHandler = () => {
    if (document.hidden) {
      stopDanmakuAnimation()
      return
    }

    startDanmakuAnimation()
  }

  document.addEventListener('visibilitychange', visibilityHandler)

  newDanmakuHandler = ((e: Event) => {
    const customEvent = e as CustomEvent
    if (!customEvent.detail) return

    danmakus.value.unshift(customEvent.detail)
    if (activeDanmakus.value.length < props.maxCount) {
      activeDanmakus.value.push({
        ...customEvent.detail,
        id: Date.now() + Math.random()
      })
    }
  }) as EventListener

  window.addEventListener('new-danmaku', newDanmakuHandler)
})

onUnmounted(() => {
  stopDanmakuAnimation()

  if (visibilityHandler) {
    document.removeEventListener('visibilitychange', visibilityHandler)
  }

  if (newDanmakuHandler) {
    window.removeEventListener('new-danmaku', newDanmakuHandler)
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

.danmaku-row-0 { top: 10%; }
.danmaku-row-1 { top: 25%; }
.danmaku-row-2 { top: 40%; }
.danmaku-row-3 { top: 55%; }
.danmaku-row-4 { top: 70%; }

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
