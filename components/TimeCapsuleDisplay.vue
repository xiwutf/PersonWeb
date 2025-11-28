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
    
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="capsule in capsules"
        :key="capsule.id"
        class="bg-white rounded-2xl p-6 shadow-lg border border-slate-100 hover:shadow-xl hover:border-purple-200 transition-all duration-300 group"
        data-aos="fade-up"
      >
        <!-- 内容 -->
        <div class="mb-4">
          <p class="text-slate-800 leading-relaxed line-clamp-3 group-hover:text-purple-700 transition-colors">
            {{ capsule.content }}
          </p>
        </div>
        
        <!-- 底部信息 -->
        <div class="flex items-center justify-between pt-4 border-t border-slate-100">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 bg-gradient-to-br from-purple-400 to-pink-400 rounded-full flex items-center justify-center text-white text-xs font-bold">
              {{ (capsule.visitorName || '匿名').charAt(0) }}
            </div>
            <span class="text-sm text-slate-600">{{ capsule.visitorName || '匿名访客' }}</span>
          </div>
          <span class="text-xs text-slate-400">{{ formatDate(capsule.createdAt) }}</span>
        </div>
      </div>
    </div>
    
    <!-- 查看更多按钮 -->
    <div v-if="capsules.length > 0" class="text-center mt-12">
      <button
        @click="scrollToWall"
        class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-600 text-white rounded-full font-medium hover:from-purple-600 hover:to-pink-700 transition-all shadow-lg shadow-purple-500/30 hover:shadow-purple-500/50 hover:-translate-y-1"
      >
        <i class="fas fa-clock mr-2"></i>
        查看完整时间胶囊墙
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'

const api = useApi()
const capsules = ref<TimeCapsule[]>([])
const loading = ref(true)

const fetchCapsules = async () => {
  loading.value = true
  try {
    const res = await api.get<TimeCapsuleListResponse>('/TimeCapsule', {
      params: {
        page: 1,
        pageSize: 6 // 首页只显示6个
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
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

