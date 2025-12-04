<template>
  <section class="roadmap-section">
    <div class="roadmap-container">
      <div class="roadmap-header" ref="sectionTitleRef">
        <h2 class="roadmap-title">Roadmap & What's New</h2>
        <p class="roadmap-subtitle">这是一个在迭代的产品</p>
      </div>

      <div class="roadmap-content">
        <!-- 最近完成的里程碑 -->
        <div class="milestone-group">
          <h3 class="milestone-group-title">
            <i class="fas fa-check-circle milestone-group-title-icon" style="color: rgb(74, 222, 128);"></i>
            最近完成
          </h3>
          <div class="milestone-list">
            <div
              v-for="(item, index) in completedItems"
              :key="index"
              class="milestone-item completed"
              :ref="el => { if (el) itemRefs.push(el as HTMLElement) }"
            >
              <div class="milestone-item-icon">✅</div>
              <div class="milestone-item-content">
                <div class="milestone-item-title">{{ item.title }}</div>
                <div class="milestone-item-date">{{ item.date }}</div>
              </div>
              <div class="milestone-item-tag milestone-item-tag-done">Done</div>
            </div>
          </div>
        </div>

        <!-- 正在进行 / 计划中的事项 -->
        <div class="milestone-group">
          <h3 class="milestone-group-title">
            <i class="fas fa-clock milestone-group-title-icon" style="color: rgb(250, 204, 21);"></i>
            进行中 / 计划中
          </h3>
          <div class="milestone-list">
            <div
              v-for="(item, index) in inProgressItems"
              :key="index"
              class="milestone-item"
              :class="item.status === 'in-progress' ? 'in-progress' : 'planned'"
              :ref="el => { if (el) itemRefs.push(el as HTMLElement) }"
            >
              <div class="milestone-item-icon">{{ item.icon }}</div>
              <div class="milestone-item-content">
                <div class="milestone-item-title">{{ item.title }}</div>
                <div class="milestone-item-date">{{ item.date }}</div>
              </div>
              <div class="milestone-item-tag" :class="item.status === 'in-progress' ? 'milestone-item-tag-progress' : 'milestone-item-tag-planned'">
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
/* 样式已移至 assets/css/hero.css */
/* 保留组件特有的样式（如果有） */
</style>

