<template>
  <section class="time-capsule-super py-24 relative">
    <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="capsule-container relative overflow-hidden rounded-3xl" ref="containerRef">
        <!-- 背景渐变 -->
        <div class="absolute inset-0 bg-gradient-to-br from-slate-900 via-purple-900/20 to-slate-900"></div>
        
        <!-- 光效装饰 -->
        <div class="absolute -top-20 -left-10 w-60 h-60 bg-blue-500/20 rounded-full blur-3xl"></div>
        <div class="absolute bottom-0 right-0 w-72 h-72 bg-purple-500/20 rounded-full blur-3xl"></div>
        
        <!-- 内容 -->
        <div class="relative z-10 py-8 px-4 sm:px-8">
          <div class="mb-6 flex items-center justify-between">
            <div>
              <h3 class="text-xl sm:text-2xl font-bold text-text-main flex items-center gap-2 mb-1">
                <span>时间胶囊弹幕墙</span>
                <span class="text-xs px-2 py-0.5 rounded-full bg-primary-soft text-primary-base border border-primary-base">
                  实时滚动
                </span>
              </h3>
              <p class="text-xs sm:text-sm text-text-muted mt-1">
                访客的留言会以弹幕的形式划过这里。
              </p>
            </div>
          </div>

          <div class="relative h-64 overflow-hidden rounded-2xl bg-bg-card backdrop-blur-sm border border-border-subtle">
            <div
              v-for="(item, index) in capsules"
              :key="item.id || index"
              class="barrage-item-super absolute inline-flex items-center px-3 py-1.5 rounded-full bg-bg-elevated backdrop-blur-md border border-border-default text-xs sm:text-sm whitespace-nowrap hover:bg-bg-card transition-colors cursor-pointer"
              :style="getBarrageStyle(index)"
              @mouseenter="pauseAnimation"
              @mouseleave="resumeAnimation"
            >
              <span class="mr-2 text-primary-base font-medium">@{{ generateVisitorName(index, item.visitorName) }}</span>
              <span class="max-w-[220px] sm:max-w-[320px] truncate text-text-main">{{ item.content }}</span>
            </div>
          </div>
          
          <!-- 查看更多按钮 -->
          <div class="text-center mt-8">
            <button
              @click="scrollToWall"
              class="inline-flex items-center px-6 py-3 bg-primary-base text-text-main rounded-full font-medium hover:bg-primary-hover transition-all shadow-lg hover:-translate-y-1"
            >
              <i class="fas fa-clock mr-2"></i>
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
.time-capsule-super {
  background: var(--color-bg-body);
}

.capsule-container {
  opacity: 0;
}

@keyframes barrage-move-super {
  0% {
    right: -100%;
    transform: translateX(0);
  }
  100% {
    right: 100%;
    transform: translateX(0);
  }
}

.barrage-item-super {
  animation-name: barrage-move-super;
  animation-timing-function: linear;
  animation-iteration-count: infinite;
  right: -100%;
  will-change: right;
}
</style>

