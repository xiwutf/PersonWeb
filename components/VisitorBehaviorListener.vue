<template>
  <!-- 这个组件不渲染任何内容，只负责监听行为 -->
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'

const route = useRoute()
const api = useApi()

// 行为记录防抖
const behaviorDebounce = new Map<string, NodeJS.Timeout>()
const lastBehaviorTime = new Map<string, number>()

// 闲置检测
let idleTimer: NodeJS.Timeout | null = null
let lastActivityTime = Date.now()
const IDLE_THRESHOLD = 10000 // 10秒

// 记录行为
const recordBehavior = async (
  behaviorType: string,
  triggeredEvent?: string,
  behaviorData?: Record<string, any>
) => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) return

  // 防抖：相同行为在5秒内只记录一次
  const debounceKey = `${behaviorType}_${triggeredEvent || ''}`
  const now = Date.now()
  const lastTime = lastBehaviorTime.get(debounceKey) || 0

  if (now - lastTime < 5000) {
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

    // 如果升级了，显示提示
    if (response?.leveledUp) {
      showLevelUpNotification(response.newLevel, response.currentExp)
    }

    // 触发前端效果
    if (triggeredEvent) {
      triggerEffect(triggeredEvent)
    }
  } catch (e) {
    console.error('记录行为失败', e)
  }
}

// 触发效果
const triggerEffect = (eventType: string) => {
  if (process.client) {
    window.dispatchEvent(new CustomEvent('visitor-trigger', {
      detail: { type: eventType }
    }))
  }
}

// 显示升级提示
const showLevelUpNotification = (level: number, exp: number) => {
  if (process.client) {
    window.dispatchEvent(new CustomEvent('show-toast', {
      detail: {
        message: `🎉 恭喜升级到 Level ${level}！`,
        type: 'success',
        duration: 5000
      }
    }))
  }
}

// 滚动到底部检测
const handleScroll = () => {
  if (process.client) {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop
    const windowHeight = window.innerHeight
    const documentHeight = document.documentElement.scrollHeight

    // 滚动到底部（允许10px误差）
    if (scrollTop + windowHeight >= documentHeight - 10) {
      recordBehavior('scroll_to_bottom', 'skill_tree_glow', {
        path: route.path,
        scrollTop,
        documentHeight
      })
    }
  }
}

// 头像悬停检测
const handleAvatarHover = () => {
  // 这个会在 Header 组件中调用
  recordBehavior('avatar_hover', 'avatar_blink', {
    path: route.path
  })
}

// 闲置检测
const resetIdleTimer = () => {
  lastActivityTime = Date.now()
  
  if (idleTimer) {
    clearTimeout(idleTimer)
  }

  // 检查用户是否禁用了自动弹出
  if (process.client) {
    const autoOpenDisabled = localStorage.getItem('ai-assistant-auto-open-disabled')
    if (autoOpenDisabled === 'true') {
      return // 用户已禁用自动弹出，不再检测闲置
    }
  }

  idleTimer = setTimeout(() => {
    const idleTime = Date.now() - lastActivityTime
    if (idleTime >= IDLE_THRESHOLD) {
      // 检查今天是否已经触发过（避免频繁弹出）
      const lastTriggerDate = process.client ? localStorage.getItem('ai-assistant-last-trigger-date') : null
      const today = new Date().toDateString()
      
      if (lastTriggerDate !== today) {
        recordBehavior('idle_10s', 'assistant_greet', {
          path: route.path,
          idleTime
        })
        // 记录今天已触发
        if (process.client) {
          localStorage.setItem('ai-assistant-last-trigger-date', today)
        }
      }
    }
  }, IDLE_THRESHOLD)
}

// 活动检测
const handleActivity = () => {
  resetIdleTimer()
}

onMounted(() => {
  if (process.client) {
    // 监听滚动
    window.addEventListener('scroll', handleScroll, { passive: true })

    // 监听用户活动（鼠标移动、键盘输入等）
    window.addEventListener('mousemove', handleActivity, { passive: true })
    window.addEventListener('keydown', handleActivity, { passive: true })
    window.addEventListener('click', handleActivity, { passive: true })

    // 启动闲置检测
    resetIdleTimer()

    // 暴露头像悬停处理函数（供其他组件调用）
    ;(window as any).handleAvatarHover = handleAvatarHover
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('scroll', handleScroll)
    window.removeEventListener('mousemove', handleActivity)
    window.removeEventListener('keydown', handleActivity)
    window.removeEventListener('click', handleActivity)

    if (idleTimer) {
      clearTimeout(idleTimer)
    }

    delete (window as any).handleAvatarHover
  }
})
</script>

<style scoped>
/* 此组件不渲染任何内容 */
</style>

