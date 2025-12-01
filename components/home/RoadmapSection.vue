<template>
  <section class="roadmap-section py-24 sm:py-32 relative">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="text-center mb-16" ref="sectionTitleRef">
        <h2 class="text-4xl lg:text-5xl font-bold text-text-main mb-4">Roadmap & What's New</h2>
        <p class="text-lg text-text-muted">这是一个在迭代的产品</p>
      </div>

      <div class="space-y-6">
        <!-- 最近完成的里程碑 -->
        <div class="milestone-group">
          <h3 class="group-title text-xl font-bold text-text-main mb-4 flex items-center">
            <i class="fas fa-check-circle text-green-400 mr-2"></i>
            最近完成
          </h3>
          <div class="milestone-list space-y-3">
            <div
              v-for="(item, index) in completedItems"
              :key="index"
              class="milestone-item completed"
              :ref="el => { if (el) itemRefs.push(el as HTMLElement) }"
            >
              <div class="item-icon">✅</div>
              <div class="item-content">
                <div class="item-title">{{ item.title }}</div>
                <div class="item-date">{{ item.date }}</div>
              </div>
              <div class="item-tag tag-done">Done</div>
            </div>
          </div>
        </div>

        <!-- 正在进行 / 计划中的事项 -->
        <div class="milestone-group">
          <h3 class="group-title text-xl font-bold text-text-main mb-4 flex items-center">
            <i class="fas fa-clock text-yellow-400 mr-2"></i>
            进行中 / 计划中
          </h3>
          <div class="milestone-list space-y-3">
            <div
              v-for="(item, index) in inProgressItems"
              :key="index"
              class="milestone-item"
              :class="item.status === 'in-progress' ? 'in-progress' : 'planned'"
              :ref="el => { if (el) itemRefs.push(el as HTMLElement) }"
            >
              <div class="item-icon">{{ item.icon }}</div>
              <div class="item-content">
                <div class="item-title">{{ item.title }}</div>
                <div class="item-date">{{ item.date }}</div>
              </div>
              <div class="item-tag" :class="item.status === 'in-progress' ? 'tag-progress' : 'tag-planned'">
                {{ item.status === 'in-progress' ? 'In Progress' : 'Planned' }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, nextTick } from 'vue'
import { animate, inView, stagger } from '@motionone/dom'

const api = useApi()
const sectionTitleRef = ref<HTMLElement | null>(null)
const itemRefs = ref<HTMLElement[]>([])

// 从后端获取数据
const changelogs = ref<Array<{ id: number; version?: string; date: string; title: string; items: string[] }>>([])
const roadmaps = ref<Array<{ id: number; title: string; description?: string; items: string[]; timeline: string; status: string; progress: number }>>([])
const loading = ref(false)

// 获取更新日志（已完成的项目）
const completedItems = computed(() => {
  // 从更新日志中筛选最近完成的
  return changelogs.value
    .slice(0, 5)
    .map(c => ({
      title: c.title,
      date: c.date.substring(0, 7) // 只显示年月
    }))
})

// 获取未来规划（进行中和计划中的项目）
const inProgressItems = computed(() => {
  return roadmaps.value
    .filter(r => r.status === 'in_progress' || r.status === 'planned')
    .map(r => ({
      title: r.title,
      date: getTimelineLabel(r.timeline),
      status: r.status === 'in_progress' ? 'in-progress' : 'planned',
      icon: getTimelineIcon(r.timeline)
    }))
})

// 获取时间线标签
const getTimelineLabel = (timeline: string): string => {
  const labels: Record<string, string> = {
    short: '1 个月',
    medium: '3-6 个月',
    long: '1 年'
  }
  return labels[timeline] || timeline
}

// 获取时间线图标
const getTimelineIcon = (timeline: string): string => {
  const icons: Record<string, string> = {
    short: '🔧',
    medium: '🧪',
    long: '🧭'
  }
  return icons[timeline] || '📋'
}

// 获取更新日志
const fetchChangelogs = async () => {
  try {
    const res = await api.get<Array<{ id: number; version?: string; date: string; title: string; items: string[] }>>('/Changelog')
    if (Array.isArray(res)) {
      changelogs.value = res
    }
  } catch (e) {
    console.error('获取更新日志失败:', e)
    // 使用默认数据
    changelogs.value = [
      { id: 1, version: '2025.11.29', date: '2025-11-29', title: '访客分析与主题系统优化', items: [] },
      { id: 2, version: '2025.11.28', date: '2025-11-28', title: 'AI Service 架构初始化', items: [] }
    ]
  }
}

// 获取未来规划
const fetchRoadmaps = async () => {
  try {
    const res = await api.get<Array<{ id: number; title: string; description?: string; items: string[]; timeline: string; status: string; progress: number }>>('/Roadmap')
    if (Array.isArray(res)) {
      roadmaps.value = res
    }
  } catch (e) {
    console.error('获取未来规划失败:', e)
    // 使用默认数据
    roadmaps.value = [
      { id: 1, title: '开发 AI Service 后端', timeline: 'short', status: 'in_progress', progress: 30, items: [] },
      { id: 2, title: '研发 AI Agent 实验功能', timeline: 'short', status: 'in_progress', progress: 20, items: [] },
      { id: 3, title: '规划：其他人也能注册使用的平台能力', timeline: 'long', status: 'planned', progress: 0, items: [] }
    ]
  }
}

onMounted(async () => {
  loading.value = true
  await Promise.all([fetchChangelogs(), fetchRoadmaps()])
  loading.value = false
  
  // 动画初始化
  if (sectionTitleRef.value) {
    inView(sectionTitleRef.value, () => {
      animate(
        sectionTitleRef.value!,
        { opacity: [0, 1], y: [30, 0] },
        { duration: 0.6, easing: 'ease-out' }
      )
    })
  }

  // 等待数据加载后再初始化动画
  await nextTick()
  if (itemRefs.value.length > 0) {
    inView(itemRefs.value[0], () => {
      animate(
        itemRefs.value,
        { opacity: [0, 1], x: [-20, 0] },
        { duration: 0.5, delay: stagger(0.1), easing: 'ease-out' }
      )
    })
  }
})

onMounted(() => {
  if (sectionTitleRef.value) {
    inView(sectionTitleRef.value, () => {
      animate(
        sectionTitleRef.value!,
        { opacity: [0, 1], y: [30, 0] },
        { duration: 0.6, easing: 'ease-out' }
      )
    })
  }

  if (itemRefs.value.length > 0) {
    inView(itemRefs.value[0], () => {
      animate(
        itemRefs.value,
        { opacity: [0, 1], x: [-20, 0] },
        { duration: 0.5, delay: stagger(0.1), easing: 'ease-out' }
      )
    })
  }
})
</script>

<style scoped>
.roadmap-section {
  background-color: var(--color-bg-body);
}

.milestone-group {
  @apply mb-8;
}

.group-title {
  @apply mb-4;
}

.milestone-item {
  @apply flex items-center gap-4 bg-bg-card backdrop-blur-xl border border-border-subtle rounded-xl p-6 transition-all duration-300;
  box-shadow: var(--shadow-md);
  opacity: 0;
}

.milestone-item:hover {
  @apply scale-[1.02] border-border-default;
  box-shadow: var(--shadow-lg);
}

.item-icon {
  @apply text-2xl flex-shrink-0;
}

.item-content {
  @apply flex-1;
}

.item-title {
  @apply text-text-main font-medium mb-1;
}

.item-date {
  @apply text-sm text-text-muted;
}

.item-tag {
  @apply px-3 py-1 rounded-full text-xs font-semibold;
}

.tag-done {
  @apply bg-green-500/20 text-green-400 border border-green-400/40;
}

.tag-progress {
  @apply bg-yellow-500/20 text-yellow-400 border border-yellow-400/40;
}

.tag-planned {
  @apply bg-blue-500/20 text-blue-400 border border-blue-400/40;
}
</style>

