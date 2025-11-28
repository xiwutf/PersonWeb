<template>
  <div
    v-for="(bubble, index) in activeBubbles"
    :key="bubble.id"
    class="visitor-bubble"
    :class="`bubble-index-${index}`"
    :style="getBubbleDynamicStyle(bubble, index)"
  >
    <div class="bubble-avatar">
      <img v-if="bubble.avatarUrl" :src="bubble.avatarUrl" :alt="bubble.displayText || '访客'" />
      <div v-else class="bubble-avatar-placeholder">
        <i class="fas fa-user"></i>
      </div>
    </div>
    <div class="bubble-text">
      {{ bubble.displayText || '一位新访客进入了网站' }}
    </div>
  </div>
</template>

<script setup lang="ts">
interface Bubble {
  id: number
  visitorId: string
  avatarUrl?: string
  location?: string
  displayText?: string
  displayedAt: string
}

const bubbles = ref<Bubble[]>([])
const activeBubbles = ref<Bubble[]>([])

// 获取访客气泡
const fetchBubbles = async () => {
  try {
    // 这里可以从API获取，或者通过WebSocket实时接收
    // 暂时使用本地存储的访客信息
    const visitorId = localStorage.getItem('visitor_id')
    if (visitorId) {
      // 模拟新访客气泡
      createBubble(visitorId)
    }
  } catch (e) {
    console.error('获取气泡失败', e)
  }
}

// 创建气泡
const createBubble = (visitorId: string, location?: string) => {
  const bubble: Bubble = {
    id: Date.now() + Math.random(),
    visitorId,
    location,
    displayText: location ? `一位来自${location}的访客进入了网站` : '一位新访客进入了网站',
    displayedAt: new Date().toISOString()
  }

  activeBubbles.value.push(bubble)

  // 10秒后移除
  setTimeout(() => {
    activeBubbles.value = activeBubbles.value.filter(b => b.id !== bubble.id)
  }, 10000)
}

// 获取气泡动态样式（仅保留必须动态计算的属性）
const getBubbleDynamicStyle = (bubble: Bubble, index: number) => {
  const startX = 80 + Math.random() * 15 // 80-95%
  const startY = 10 + Math.random() * 20 // 10-30%
  const endY = startY + 30 + Math.random() * 40 // 向下移动

  return {
    left: `${startX}%`,
    top: `${startY}%`,
    animationDelay: `${index * 0.5}s`,
    '--end-y': endY,
    '--start-y': startY
  }
}

onMounted(() => {
  fetchBubbles()

  // 监听新访客事件
  if (process.client) {
    window.addEventListener('new-visitor', ((e: CustomEvent) => {
      if (e.detail) {
        createBubble(e.detail.visitorId, e.detail.location)
      }
    }) as EventListener)
  }
})
</script>

<style scoped>
.visitor-bubble {
  position: fixed;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 1rem;
  background: rgba(59, 130, 246, 0.9);
  backdrop-filter: blur(10px);
  border-radius: 2rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  animation: bubble-float 10s ease-out forwards;
  z-index: 200;
  pointer-events: none;
}

.bubble-avatar {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  overflow: hidden;
  border: 2px solid rgba(255, 255, 255, 0.5);
  flex-shrink: 0;
}

.bubble-avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.bubble-avatar-placeholder {
  width: 100%;
  height: 100%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.25rem;
}

.bubble-text {
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  white-space: nowrap;
}

@keyframes bubble-float {
  0% {
    opacity: 0;
    transform: translateY(0) scale(0.8);
  }
  10% {
    opacity: 1;
    transform: translateY(-10px) scale(1);
  }
  90% {
    opacity: 1;
    transform: translateY(calc(var(--end-y) - var(--start-y))) scale(1);
  }
  100% {
    opacity: 0;
    transform: translateY(calc(var(--end-y) - var(--start-y))) scale(0.8);
  }
}
</style>

