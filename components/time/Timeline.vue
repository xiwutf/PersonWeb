<template>
  <div class="py-16 timeline-container">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h2 class="text-3xl lg:text-4xl font-bold timeline-title mb-4">成长轨迹</h2>
        <p class="text-lg timeline-subtitle">记录每一个重要的时刻</p>
      </div>

      <div class="relative">
        <!-- 时间线 -->
        <div class="absolute left-8 top-0 bottom-0 w-0.5 timeline-line-gradient"></div>

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
              class="timeline-card rounded-xl shadow-md hover:shadow-xl transition-all duration-300 p-6 border-l-4"
              :style="{ borderLeftColor: event.color || defaultColor }"
            >
              <div class="flex items-start justify-between mb-2">
                <h3 class="text-xl font-bold timeline-card-title">{{ event.title }}</h3>
                <span
                  class="px-3 py-1 rounded-full text-sm font-semibold text-white"
                  :style="{ backgroundColor: event.color || defaultColor }"
                >
                  {{ event.year }}
                </span>
              </div>
              <p v-if="event.description" class="timeline-card-description leading-relaxed">
                {{ event.description }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <div v-if="loading" class="text-center py-8 timeline-loading">加载中...</div>
      <div v-if="!loading && events.length === 0" class="text-center py-8 timeline-empty">
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
    return getComputedStyle(document.documentElement).getPropertyValue('--color-primary').trim() || 'var(--color-primary)'
  }
  return 'var(--color-primary)'
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

/* 时间线容器样式 - 使用 CSS 变量 */
.timeline-container {
  background: linear-gradient(to bottom, var(--color-bg-body, #f8fafc), var(--color-bg-card, var(--color-bg-card)));
}

.timeline-title {
  color: var(--color-text-main, var(--color-text-main));
}

.timeline-subtitle {
  color: var(--color-text-sub, var(--color-text-sec));
}

.timeline-line-gradient {
  background: linear-gradient(to bottom, var(--color-primary, var(--color-primary)), var(--color-purple, #a855f7), var(--color-error, #ec4899));
}

.timeline-card {
  background: var(--color-bg-card, var(--color-bg-card));
}

.timeline-card-title {
  color: var(--color-text-main, var(--color-text-main));
}

.timeline-card-description {
  color: var(--color-text-sub, var(--color-text-sec));
}

.timeline-loading,
.timeline-empty {
  color: var(--color-text-muted, var(--color-text-sec));
}

/* 深色主题适配 */
html[data-theme="dark"] .timeline-container,
html.dark .timeline-container {
  background: linear-gradient(to bottom, var(--color-bg-body, #020617), var(--color-bg-card, rgba(255, 255, 255, 0.05)));
}

html[data-theme="dark"] .timeline-title,
html.dark .timeline-title {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .timeline-subtitle,
html.dark .timeline-subtitle {
  color: var(--color-text-sub, #cbd5e1);
}

html[data-theme="dark"] .timeline-card,
html.dark .timeline-card {
  background: var(--color-bg-card, rgba(255, 255, 255, 0.05));
}

html[data-theme="dark"] .timeline-card-title,
html.dark .timeline-card-title {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .timeline-card-description,
html.dark .timeline-card-description {
  color: var(--color-text-sub, #cbd5e1);
}
</style>

