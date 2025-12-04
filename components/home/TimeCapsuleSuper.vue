<template>
  <section class="time-capsule-super">
    <div class="time-capsule-super-container">
      <div class="time-capsule-container" ref="containerRef">
        <!-- 背景渐变 -->
        <div class="time-capsule-bg-gradient"></div>
        
        <!-- 光效装饰 -->
        <div class="time-capsule-glow-blue"></div>
        <div class="time-capsule-glow-purple"></div>
        
        <!-- 内容 -->
        <div class="time-capsule-content">
          <div class="time-capsule-header">
            <div>
              <div class="time-capsule-title-group">
                <h3 class="time-capsule-title">
                  <span>时间胶囊弹幕墙</span>
                </h3>
                <span class="time-capsule-badge">
                  实时滚动
                </span>
              </div>
              <p class="time-capsule-description">
                访客的留言会以弹幕的形式划过这里。
              </p>
            </div>
          </div>

          <div class="time-capsule-wall">
            <div
              v-for="(item, index) in capsules"
              :key="item.id || index"
              class="time-capsule-barrage-item"
              :style="getBarrageStyle(index)"
              @mouseenter="pauseAnimation"
              @mouseleave="resumeAnimation"
            >
              <span class="time-capsule-barrage-visitor">@{{ generateVisitorName(index, item.visitorName) }}</span>
              <span class="time-capsule-barrage-content">{{ item.content }}</span>
            </div>
          </div>
          
          <!-- 查看更多按钮 -->
          <div class="time-capsule-button-wrapper">
            <button
              @click="scrollToWall"
              class="time-capsule-button"
            >
              <i class="fas fa-clock time-capsule-button-icon"></i>
              查看完整时间胶囊墙
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView } from '@motionone/dom'
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'

const api = useApi()
const capsules = ref<TimeCapsule[]>([])
const containerRef = ref<HTMLElement | null>(null)

const TRACK_COUNT = 6

const fetchCapsules = async () => {
  try {
    const res = await api.get<TimeCapsuleListResponse>('/TimeCapsule', {
      params: {
        page: 1,
        pageSize: 12
      }
    })
    
    if (res) {
      if (res.List) {
        capsules.value = res.List
      } else if ((res as any).list) {
        capsules.value = (res as any).list
      } else if (Array.isArray(res)) {
        capsules.value = res
      } else {
        capsules.value = []
      }
    } else {
      capsules.value = []
    }
  } catch (e) {
    if (import.meta.env.DEV) {
      console.error('Failed to fetch capsules:', e)
    }
    capsules.value = []
  }
}

const generateVisitorName = (index: number, visitorName?: string): string => {
  if (visitorName && visitorName.trim()) {
    return visitorName.trim()
  }
  const names = ['访客A', '访客B', '访客C', '访客D', '访客E', '访客F', '访客G', '访客H', '访客I', '访客J']
  return names[index % names.length]
}

const getBarrageStyle = (index: number) => {
  const track = index % TRACK_COUNT
  const topPercent = (track * (100 / TRACK_COUNT)) + (Math.random() * (100 / TRACK_COUNT) * 0.5)
  const duration = 20 + Math.random() * 20
  const delay = Math.random() * 5
  
  return {
    top: `${topPercent}%`,
    animationDuration: `${duration}s`,
    animationDelay: `${delay}s`,
    zIndex: Math.floor(Math.random() * 10)
  }
}

const pauseAnimation = (event: MouseEvent) => {
  const target = event.currentTarget as HTMLElement
  if (target) {
    target.style.animationPlayState = 'paused'
  }
}

const resumeAnimation = (event: MouseEvent) => {
  const target = event.currentTarget as HTMLElement
  if (target) {
    target.style.animationPlayState = 'running'
  }
}

const scrollToWall = () => {
  const event = new CustomEvent('openTimeCapsuleWall')
  window.dispatchEvent(event)
  setTimeout(() => {
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' })
  }, 100)
}

onMounted(() => {
  fetchCapsules()
  
  if (containerRef.value) {
    inView(containerRef.value, () => {
      animate(
        containerRef.value!,
        { opacity: [0, 1], y: [30, 0] },
        { duration: 0.6, easing: 'ease-out' }
      )
    })
  }
})
</script>

<style scoped>
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

