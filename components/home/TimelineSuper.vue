<template>
  <section class="timeline-super">
    <div class="timeline-super-container">
      <div class="timeline-super-header" ref="titleRef">
        <h2 class="timeline-super-title">平台发展轨迹</h2>
        <p class="timeline-super-subtitle">这个平台 / 这个创作空间是如何一步步搭出来的</p>
      </div>

      <div class="timeline-wrapper" ref="timelineRef">
        <!-- 时间线 -->
        <div class="timeline-line"></div>

        <div class="timeline-items">
          <div
            v-for="(event, index) in events"
            :key="event.id"
            class="timeline-item"
            :ref="el => { if (el) timelineItemsRef[index] = el as HTMLElement }"
          >
            <!-- 时间点 -->
            <div
              class="timeline-dot"
              :style="{
                backgroundColor: event.color || 'var(--primary)',
                boxShadow: `0 0 20px ${event.color || 'var(--primary)'}40`
              }"
            >
              {{ event.icon || '⭐' }}
            </div>

            <!-- 内容卡片 -->
            <div
              class="timeline-card"
              :style="{ borderLeftColor: event.color || 'var(--primary)', borderLeftWidth: '4px' }"
            >
              <div class="timeline-card-header">
                <h3 class="timeline-card-title">{{ event.title }}</h3>
                <span
                  class="timeline-card-year"
                  :style="{ backgroundColor: event.color || 'var(--color-primary)' }"
                >
                  {{ event.year }}
                </span>
              </div>
              <p v-if="event.description" class="timeline-card-description">
                {{ event.description }}
              </p>
            </div>
          </div>
        </div>

        <div v-if="loading" class="timeline-loading">加载中...</div>
        <div v-if="!loading && events.length === 0" class="timeline-empty">
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
/* 样式已移至 assets/css/home.css */
/* 保留组件特有的样式（如果有） */
</style>

