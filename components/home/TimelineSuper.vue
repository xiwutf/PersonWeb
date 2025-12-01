<template>
  <section class="timeline-super py-24 relative">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12" ref="titleRef">
        <h2 class="text-3xl lg:text-4xl font-bold text-text-main mb-4">平台发展轨迹</h2>
        <p class="text-lg text-text-muted">这个平台 / 这个创作空间是如何一步步搭出来的</p>
      </div>

      <div class="relative" ref="timelineRef">
        <!-- 时间线 -->
        <div class="absolute left-8 top-0 bottom-0 w-0.5 bg-gradient-to-b from-cyan-500 via-blue-500 to-purple-500"></div>

        <div class="space-y-8">
          <div
            v-for="(event, index) in events"
            :key="event.id"
            class="timeline-item relative pl-20"
            :ref="el => { if (el) timelineItemsRef[index] = el as HTMLElement }"
          >
            <!-- 时间点 -->
            <div
              class="absolute left-0 top-2 w-16 h-16 rounded-full flex items-center justify-center text-2xl shadow-lg transform transition-all duration-300 hover:scale-110 border-4 border-black"
              :style="{
                backgroundColor: event.color || '#00d9ff',
                boxShadow: `0 0 20px ${event.color || '#00d9ff'}40`
              }"
            >
              {{ event.icon || '⭐' }}
            </div>

            <!-- 内容卡片 -->
            <div
              class="timeline-card bg-bg-card backdrop-blur-xl rounded-xl border border-border-subtle hover:border-border-default transition-all duration-300 p-6"
              :style="{ borderLeftColor: event.color || '#00d9ff', borderLeftWidth: '4px' }"
            >
              <div class="flex items-start justify-between mb-2">
                <h3 class="text-xl font-bold text-text-main">{{ event.title }}</h3>
                <span
                  class="px-3 py-1 rounded-full text-sm font-semibold text-text-main"
                  :style="{ backgroundColor: event.color || 'var(--color-primary)' }"
                >
                  {{ event.year }}
                </span>
              </div>
              <p v-if="event.description" class="text-text-muted leading-relaxed">
                {{ event.description }}
              </p>
            </div>
          </div>
        </div>

        <div v-if="loading" class="text-center py-8 text-text-muted">加载中...</div>
        <div v-if="!loading && events.length === 0" class="text-center py-8 text-text-muted">
          暂无成长记录
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { animate, inView, stagger } from '@motionone/dom'

const api = useApi()
const events = ref<any[]>([])
const loading = ref(false)
const titleRef = ref<HTMLElement | null>(null)
const timelineRef = ref<HTMLElement | null>(null)
const timelineItemsRef = ref<(HTMLElement | null)[]>([])

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
  
  if (titleRef.value) {
    inView(titleRef.value, () => {
      animate(
        titleRef.value!,
        { opacity: [0, 1], y: [30, 0] },
        { duration: 0.6, easing: 'ease-out' }
      )
    })
  }
  
  if (timelineRef.value) {
    inView(timelineRef.value, () => {
      const items = timelineItemsRef.value.filter(Boolean) as HTMLElement[]
      if (items.length > 0) {
        animate(
          items,
          { opacity: [0, 1], x: [-30, 0] },
          { duration: 0.6, delay: stagger(0.1), easing: 'ease-out' }
        )
      }
    })
  }
})
</script>

<style scoped>
.timeline-super {
  background: var(--color-bg-body);
}

.timeline-item {
  opacity: 0;
}

.timeline-card {
  transition: all 0.3s ease;
}

.timeline-card:hover {
  transform: translateX(8px);
  box-shadow: 0 8px 24px rgba(0, 217, 255, 0.15);
}
</style>

