<template>
  <!-- 该组件只负责记录访客行为，不渲染可见内容 -->
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'

const route = useRoute()
const api = useApi()

const lastBehaviorTime = new Map<string, number>()

let idleTimer: ReturnType<typeof window.setTimeout> | null = null
let lastActivityTime = Date.now()
let lastScrollSampleAt = 0
let hasRecordedScrollBottom = false

const IDLE_THRESHOLD = 10000
const BEHAVIOR_DEBOUNCE_MS = 5000
const SCROLL_THROTTLE_MS = 1200

const isPerformanceConstrained = () => {
  if (!process.client) return false

  const prefersReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches
  const saveData = navigator.connection?.saveData === true
  const coarsePointer = window.matchMedia?.('(pointer: coarse)').matches
  const lowMemory = typeof navigator.deviceMemory === 'number' && navigator.deviceMemory <= 4

  return Boolean(prefersReducedMotion || saveData || coarsePointer || lowMemory)
}

const triggerEffect = (eventType: string) => {
  if (!process.client) return

  window.dispatchEvent(new CustomEvent('visitor-trigger', {
    detail: { type: eventType }
  }))
}

const showLevelUpNotification = (level: number, exp: number) => {
  if (!process.client) return

  window.dispatchEvent(new CustomEvent('show-toast', {
    detail: {
      message: `🎉 恭喜升级到 Level ${level}`,
      type: 'success',
      duration: 5000,
      exp
    }
  }))
}

const recordBehavior = async (
  behaviorType: string,
  triggeredEvent?: string,
  behaviorData?: Record<string, any>
) => {
  if (!process.client) return

  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) return

  const debounceKey = `${behaviorType}_${triggeredEvent || ''}`
  const now = Date.now()
  const lastTime = lastBehaviorTime.get(debounceKey) || 0

  if (now - lastTime < BEHAVIOR_DEBOUNCE_MS) {
    return
  }

  lastBehaviorTime.set(debounceKey, now)

  try {
    const response = await api.post('/VisitorGamification/behavior', {
      visitorId,
      behaviorType,
      behaviorData: behaviorData || {},
      triggeredEvent,
      path: route.path
    })

    if (response?.leveledUp) {
      showLevelUpNotification(response.newLevel, response.currentExp)
    }

    if (triggeredEvent) {
      triggerEffect(triggeredEvent)
    }
  } catch (error) {
    if (process.dev) {
      console.error('记录访客行为失败', error)
    }
  }
}

const handleScroll = () => {
  if (!process.client) return

  const now = Date.now()
  if (now - lastScrollSampleAt < SCROLL_THROTTLE_MS) {
    return
  }

  lastScrollSampleAt = now

  const scrollTop = window.pageYOffset || document.documentElement.scrollTop
  const windowHeight = window.innerHeight
  const documentHeight = document.documentElement.scrollHeight
  const nearBottom = scrollTop + windowHeight >= documentHeight - 24

  if (nearBottom && !hasRecordedScrollBottom) {
    hasRecordedScrollBottom = true
    recordBehavior('scroll_to_bottom', 'skill_tree_glow', {
      path: route.path,
      scrollTop,
      documentHeight
    })
  } else if (!nearBottom) {
    hasRecordedScrollBottom = false
  }
}

const handleAvatarHover = () => {
  recordBehavior('avatar_hover', 'avatar_blink', {
    path: route.path
  })
}

const resetIdleTimer = () => {
  if (!process.client) return

  lastActivityTime = Date.now()

  if (idleTimer) {
    clearTimeout(idleTimer)
  }

  if (localStorage.getItem('ai-assistant-auto-open-disabled') === 'true') {
    return
  }

  idleTimer = window.setTimeout(() => {
    const idleTime = Date.now() - lastActivityTime
    if (idleTime < IDLE_THRESHOLD) {
      return
    }

    const today = new Date().toDateString()
    const lastTriggerDate = localStorage.getItem('ai-assistant-last-trigger-date')

    if (lastTriggerDate === today) {
      return
    }

    recordBehavior('idle_10s', 'assistant_greet', {
      path: route.path,
      idleTime
    })
    localStorage.setItem('ai-assistant-last-trigger-date', today)
  }, IDLE_THRESHOLD)
}

const handleActivity = () => {
  resetIdleTimer()
}

onMounted(() => {
  if (!process.client) return

  const constrained = isPerformanceConstrained()

  if (!constrained) {
    window.addEventListener('scroll', handleScroll, { passive: true })
    window.addEventListener('mousemove', handleActivity, { passive: true })
  }

  window.addEventListener('keydown', handleActivity, { passive: true })
  window.addEventListener('click', handleActivity, { passive: true })

  resetIdleTimer()
  ;(window as any).handleAvatarHover = handleAvatarHover
})

onUnmounted(() => {
  if (!process.client) return

  window.removeEventListener('scroll', handleScroll)
  window.removeEventListener('mousemove', handleActivity)
  window.removeEventListener('keydown', handleActivity)
  window.removeEventListener('click', handleActivity)

  if (idleTimer) {
    clearTimeout(idleTimer)
  }

  delete (window as any).handleAvatarHover
})
</script>

<style scoped>
/* 该组件不渲染任何内容 */
</style>
