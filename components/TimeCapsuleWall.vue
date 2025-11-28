<template>
  <div class="fixed bottom-24 right-8 z-40">
    <!-- 打开按钮 -->
    <button
      v-if="!isOpen"
      @click="toggleWall"
      class="w-14 h-14 bg-gradient-to-br from-purple-500 to-pink-600 rounded-full shadow-lg hover:shadow-xl transition-all duration-300 hover:scale-110 flex items-center justify-center group"
      aria-label="打开时间胶囊墙"
    >
      <svg class="w-7 h-7 text-white group-hover:rotate-12 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>
    </button>

    <!-- 时间胶囊墙 -->
    <Transition name="slide-up">
      <div
        v-if="isOpen"
        class="w-80 h-[500px] bg-white dark:bg-gray-800 rounded-2xl shadow-2xl border border-gray-200 dark:border-gray-700 flex flex-col overflow-hidden"
      >
        <!-- 头部 -->
        <div class="bg-gradient-to-r from-purple-500 to-pink-600 p-4 flex items-center justify-between">
          <div>
            <h3 class="text-white font-bold text-lg">时间胶囊墙</h3>
            <p class="text-white/80 text-xs">留下你的足迹</p>
          </div>
          <button
            @click="toggleWall"
            class="text-white/80 hover:text-white transition-colors"
            aria-label="关闭"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- 胶囊列表 -->
        <div ref="capsulesRef" class="flex-1 overflow-y-auto p-4 space-y-3 bg-gray-50 dark:bg-gray-900">
          <div v-if="loading" class="text-center py-8 text-gray-500">加载中...</div>
          <div v-else-if="capsules.length === 0" class="text-center py-8 text-gray-500">
            <p class="text-sm">还没有时间胶囊</p>
            <p class="text-xs mt-2">成为第一个留言的人吧！</p>
          </div>
          <div
            v-for="capsule in capsules"
            :key="capsule.id"
            class="bg-white dark:bg-gray-800 rounded-lg p-4 border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-md transition-shadow"
          >
            <p class="text-sm text-gray-800 dark:text-gray-200 mb-2">{{ capsule.content }}</p>
            <div class="flex items-center justify-between text-xs text-gray-500">
              <span>{{ capsule.visitorName || '匿名访客' }}</span>
              <span>{{ formatDate(capsule.createdAt) }}</span>
            </div>
          </div>
        </div>

        <!-- 提交区域 -->
        <div class="p-4 border-t border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800">
          <div class="space-y-2">
            <input
              v-model="visitorName"
              type="text"
              placeholder="你的名字（可选）"
              class="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:outline-none focus:ring-2 focus:ring-purple-500"
            />
            <textarea
              v-model="capsuleContent"
              rows="3"
              placeholder="写下你想说的话..."
              class="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:outline-none focus:ring-2 focus:ring-purple-500 resize-none"
              maxlength="200"
            ></textarea>
            <div class="flex items-center justify-between">
              <span class="text-xs text-gray-500">{{ capsuleContent.length }}/200</span>
              <button
                @click="submitCapsule"
                :disabled="!capsuleContent.trim() || submitting"
                class="px-4 py-2 bg-gradient-to-r from-purple-500 to-pink-600 text-white text-sm rounded-lg hover:from-purple-600 hover:to-pink-700 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ submitting ? '提交中...' : '投递胶囊' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
const isOpen = ref(false)
const capsules = ref<any[]>([])
const loading = ref(false)
const submitting = ref(false)
const capsuleContent = ref('')
const visitorName = ref('')
const capsulesRef = ref<HTMLDivElement | null>(null)

const api = useApi()

const toggleWall = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    fetchCapsules()
  }
}

const fetchCapsules = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/TimeCapsule', {
      params: {
        page: 1,
        pageSize: 20
      }
    })
    
    if (res && res.List) {
      capsules.value = res.List
    } else if (Array.isArray(res)) {
      capsules.value = res
    } else {
      capsules.value = []
    }
  } catch (e) {
    console.error('Failed to fetch capsules:', e)
    capsules.value = []
  } finally {
    loading.value = false
  }
}

const submitCapsule = async () => {
  if (!capsuleContent.value.trim() || submitting.value) return

  submitting.value = true
  try {
    const visitorId = localStorage.getItem('visitor_id') || undefined
    
    await api.post('/TimeCapsule', {
      content: capsuleContent.value.trim(),
      visitorId: visitorId,
      visitorName: visitorName.value.trim() || undefined
    })

    alert('时间胶囊已投递！等待审核后会在墙上展示。')
    capsuleContent.value = ''
    visitorName.value = ''
    
    // 刷新列表
    await fetchCapsules()
  } catch (e: any) {
    console.error('Failed to submit capsule:', e)
    alert(e.message || '提交失败，请稍后重试')
  } finally {
    submitting.value = false
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
  return `${Math.floor(days / 365)}年前`
}
</script>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(20px) scale(0.9);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.9);
}
</style>

