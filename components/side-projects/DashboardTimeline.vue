<template>
  <n-card class="dashboard-timeline-card">
    <template #header>
      <div class="timeline-header">
        <h3 class="timeline-title">项目时间轴</h3>
      </div>
    </template>
    <div v-if="loading" class="timeline-loading">
      <n-spin size="large" />
    </div>
    <div v-else-if="!items || items.length === 0" class="timeline-empty">
      暂无数据
    </div>
    <n-timeline v-else>
      <n-timeline-item
        v-for="item in items"
        :key="item.id"
        :time="formatDate(item.endTime)"
        :title="item.title"
        :content="getContent(item)"
        :type="getTimelineType(item.status)"
      />
    </n-timeline>
  </n-card>
</template>

<script setup lang="ts">
import type { SideProject } from '~/types/api'

interface Props {
  items: SideProject[]
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})

const formatDate = (date: string | null | undefined) => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const formatPrice = (price: number | null | undefined) => {
  if (price == null) return ''
  return `¥${price.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
}

const getContent = (item: SideProject) => {
  const parts: string[] = []
  if (item.category) {
    parts.push(`类型: ${item.category}`)
  }
  if (item.priceFinal) {
    parts.push(`金额: ${formatPrice(item.priceFinal)}`)
  }
  return parts.join(' | ') || '-'
}

const getTimelineType = (status: number): 'default' | 'success' | 'warning' | 'error' => {
  switch (status) {
    case 1:
      return 'success'
    case 2:
      return 'warning'
    case 3:
      return 'error'
    default:
      return 'default'
  }
}
</script>

<style scoped>
.dashboard-timeline-card {
  background-color: var(--color-bg-card);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  border: 1px solid var(--color-border-subtle);
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.timeline-title {
  font-size: var(--font-size-h4);
  font-weight: bold;
  color: var(--color-text-main);
}

.timeline-loading,
.timeline-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-2xl);
  color: var(--color-text-muted);
}
</style>

