<template>
  <div class="max-w-6xl mx-auto">
    <div v-if="loading" class="text-center py-12 text-slate-500">
      <i class="fas fa-spinner fa-spin text-2xl mb-4"></i>
      <p>加载中...</p>
    </div>
    
    <div v-else-if="capsules.length === 0" class="text-center py-12 text-slate-500">
      <div class="w-20 h-20 mx-auto mb-4 bg-slate-100 rounded-full flex items-center justify-center">
        <i class="fas fa-clock text-3xl text-slate-400"></i>
      </div>
      <p class="text-lg font-medium">还没有时间胶囊</p>
      <p class="text-sm mt-2">成为第一个留言的人吧！</p>
    </div>
    
    <!-- 弹幕墙 -->
    <div v-else class="relative overflow-hidden rounded-3xl bg-slate-900 text-white py-8 px-4 sm:px-8">
      <div class="absolute inset-0 pointer-events-none">
        <div class="absolute -top-20 -left-10 w-60 h-60 bg-blue-500/20 rounded-full blur-3xl"></div>
        <div class="absolute bottom-0 right-0 w-72 h-72 bg-purple-500/20 rounded-full blur-3xl"></div>
      </div>

      <div class="relative z-10 mb-6 flex items-center justify-between">
        <div>
          <h3 class="text-xl sm:text-2xl font-bold flex items-center gap-2">
            <span>时间胶囊弹幕墙</span>
            <span class="text-xs px-2 py-0.5 rounded-full bg-emerald-500/20 text-emerald-300 border border-emerald-400/40">
              实时滚动
            </span>
          </h3>
          <p class="text-xs sm:text-sm text-slate-300 mt-1">
            访客的留言会以弹幕的形式划过这里。
          </p>
        </div>
      </div>

      <div class="relative h-64 overflow-hidden">
        <div
          v-for="(item, index) in capsules"
          :key="item.id || index"
          class="barrage-item absolute inline-flex items-center px-3 py-1.5 rounded-full bg-white/10 border border-white/20 text-xs sm:text-sm whitespace-nowrap hover:bg-white/20 transition-colors cursor-pointer"
          :style="getBarrageStyle(index)"
          @mouseenter="pauseAnimation"
          @mouseleave="resumeAnimation"
        >
          <span class="mr-2 text-sky-300 font-medium">@{{ item.visitorName || '匿名' }}</span>
          <span class="max-w-[220px] sm:max-w-[320px] truncate">{{ item.content }}</span>
        </div>
      </div>
      
      <!-- 查看更多按钮 -->
      <div class="text-center mt-8">
        <button
          @click="scrollToWall"
          class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-600 text-white rounded-full font-medium hover:from-purple-600 hover:to-pink-700 transition-all shadow-lg shadow-purple-500/30 hover:shadow-purple-500/50 hover:-translate-y-1"
        >
          <i class="fas fa-clock mr-2"></i>
          查看完整时间胶囊墙
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'

const api = useApi()
const capsules = ref<TimeCapsule[]>([])
const loading = ref(true)

// 弹幕轨道数量
const TRACK_COUNT = 6

const fetchCapsules = async () => {
  loading.value = true
  try {
    const res = await api.get<TimeCapsuleListResponse>('/TimeCapsule', {
      params: {
        page: 1,
        pageSize: 12 // 弹幕形式可以显示更多
      }
    })
    
    // 兼容大小写
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
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch capsules:', e)
    }
    capsules.value = []
  } finally {
    loading.value = false
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) return '今天'
  if (days === 1) return '昨天'
  if (days < 7) return `${days}天前`
  if (days < 30) return `${Math.floor(days / 7)}周前`
  if (days < 365) return `${Math.floor(days / 30)}个月前`
  return date.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long', day: 'numeric' })
}

// 获取弹幕样式
const getBarrageStyle = (index: number) => {
  // 随机分配轨道（0-5）
  const track = index % TRACK_COUNT
  const topPercent = (track * (100 / TRACK_COUNT)) + (Math.random() * (100 / TRACK_COUNT) * 0.5)
  
  // 随机动画时长（20s-40s）
  const duration = 20 + Math.random() * 20
  
  // 随机延迟（负数延迟让弹幕一开始就在路上）
  const delay = -(Math.random() * duration)
  
  // 随机透明度（0.7-1.0）
  const opacity = 0.7 + Math.random() * 0.3
  
  return {
    top: `${topPercent}%`,
    animationDuration: `${duration}s`,
    animationDelay: `${delay}s`,
    opacity: opacity.toString(),
    zIndex: Math.floor(Math.random() * 10)
  }
}

// 暂停动画
const pauseAnimation = (event: MouseEvent) => {
  const target = event.currentTarget as HTMLElement
  if (target) {
    target.style.animationPlayState = 'paused'
  }
}

// 恢复动画
const resumeAnimation = (event: MouseEvent) => {
  const target = event.currentTarget as HTMLElement
  if (target) {
    target.style.animationPlayState = 'running'
  }
}

const scrollToWall = () => {
  // 触发时间胶囊墙组件打开
  const event = new CustomEvent('openTimeCapsuleWall')
  window.dispatchEvent(event)
  
  // 如果时间胶囊墙组件没有响应，则滚动到页面底部
  setTimeout(() => {
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' })
  }, 100)
}

onMounted(() => {
  fetchCapsules()
})
</script>

<style scoped>
@keyframes barrage-move {
  0% {
    transform: translateX(100%);
  }
  100% {
    transform: translateX(-120%);
  }
}

.barrage-item {
  animation-name: barrage-move;
  animation-timing-function: linear;
  animation-iteration-count: infinite;
}
</style>

