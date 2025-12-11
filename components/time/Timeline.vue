<template>
  <div class="py-16 bg-gradient-to-b from-slate-50 to-white">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h2 class="text-3xl lg:text-4xl font-bold text-slate-900 mb-4">成长轨迹</h2>
        <p class="text-lg text-slate-600">记录每一个重要的时刻</p>
      </div>

      <div class="relative">
        <!-- 时间线 -->
        <div class="absolute left-8 top-0 bottom-0 w-0.5 bg-gradient-to-b from-blue-500 via-purple-500 to-pink-500"></div>

        <div class="space-y-8">
          <div
            v-for="(event, index) in events"
            :key="event.id"
            class="relative pl-20"
            :class="{ 'animate-fade-in': true }"
            :style="{ animationDelay: `${index * 0.1}s` }"
          >
            <!-- 时间点 -->
            <div
              class="absolute left-0 top-2 w-16 h-16 rounded-full flex items-center justify-center text-2xl shadow-lg transform transition-all duration-300 hover:scale-110"
              :style="{
                backgroundColor: event.color || defaultColor,
                border: `4px solid var(--color-bg-card, white)`
              }"
            >
              {{ event.icon || '⭐' }}
            </div>

            <!-- 内容卡片 -->
            <div
              class="bg-white rounded-xl shadow-md hover:shadow-xl transition-all duration-300 p-6 border-l-4"
              :style="{ borderLeftColor: event.color || defaultColor }"
            >
              <div class="flex items-start justify-between mb-2">
                <h3 class="text-xl font-bold text-slate-900">{{ event.title }}</h3>
                <span
                  class="px-3 py-1 rounded-full text-sm font-semibold text-white"
                  :style="{ backgroundColor: event.color || defaultColor }"
                >
                  {{ event.year }}
                </span>
              </div>
              <p v-if="event.description" class="text-slate-600 leading-relaxed">
                {{ event.description }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <div v-if="loading" class="text-center py-8 text-slate-500">加载中...</div>
      <div v-if="!loading && events.length === 0" class="text-center py-8 text-slate-500">
        暂无成长记录
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const events = ref<any[]>([])
const loading = ref(false)

const api = useApi()

// 获取默认颜色（从 CSS 变量）
const getDefaultColor = () => {
  if (process.client) {
    return getComputedStyle(document.documentElement).getPropertyValue('--color-primary').trim() || '#3b82f6'
  }
  return '#3b82f6'
}

const defaultColor = getDefaultColor()

const fetchEvents = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/Timeline')
    if (res && Array.isArray(res)) {
      events.value = res
    } else if (res && res.List) {
      events.value = res.List
    } else {
      events.value = []
    }
  } catch (e) {
    console.error('Failed to fetch timeline events:', e)
    events.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchEvents()
})
</script>

<style scoped>
@keyframes fade-in {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fade-in 0.6s ease-out forwards;
  opacity: 0;
}
</style>

