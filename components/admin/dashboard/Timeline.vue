<template>
  <div class="timeline-content">
    <div class="timeline-list">
      <div
        v-for="(item, index) in items"
        :key="item.path"
        class="timeline-item"
      >
        <div class="timeline-line" v-if="index < items.length - 1"></div>
        <div class="timeline-dot"></div>
        <div class="timeline-content-inner">
          <div class="timeline-row">
            <div class="timeline-date">
              <div class="timeline-date-text">{{ item.date || '今天' }}</div>
            </div>
            <div class="timeline-main">
              <div class="timeline-header">
                <div class="timeline-title-wrapper">
                  <div class="timeline-icon">{{ item.icon }}</div>
                  <div class="timeline-title">{{ item.title }}</div>
                </div>
                <n-tag v-if="getStatusTag(item)" :type="getStatusTag(item).type" size="small">
                  {{ getStatusTag(item).label }}
                </n-tag>
              </div>
              <div class="timeline-desc">{{ item.desc }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { NTag } from 'naive-ui'

interface TimelineItem {
  path: string
  icon: string
  title: string
  desc: string
  color: 'blue' | 'purple' | 'yellow' | 'teal'
  date?: string
}

interface Props {
  items: TimelineItem[]
}

const props = withDefaults(defineProps<Props>(), {
  items: () => []
})

const getStatusTag = (item: TimelineItem) => {
  if (item.desc.includes('待处理') || item.desc.includes('新')) {
    return { type: 'warning' as const, label: '待处理' }
  }
  return null
}
</script>

<style scoped>
/* 只保留布局相关的样式，不写颜色 */
.timeline-content {
  width: 100%;
}

.timeline-list {
  position: relative;
  padding-left: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.timeline-item {
  position: relative;
}

.timeline-line {
  position: absolute;
  left: 9px;
  top: 20px;
  bottom: -16px;
  width: 2px;
  background-color: var(--color-border-subtle);
}

.timeline-item:last-child .timeline-line {
  display: none;
}

.timeline-dot {
  position: absolute;
  left: 0;
  top: 4px;
  width: 18px;
  height: 18px;
  border-radius: 50%;
  border: 3px solid var(--color-primary);
  background-color: var(--color-primary);
  z-index: 1;
  box-shadow: 
    0 0 0 2px var(--color-bg-card),
    0 0 12px rgba(59, 130, 246, 0.6); /* Vision Pro 风格光晕 */
}

.timeline-content-inner {
  margin-left: 28px;
}

.timeline-row {
  display: flex;
  gap: 16px;
  align-items: flex-start;
}

.timeline-date {
  flex-shrink: 0;
  width: 60px;
  text-align: right;
}

.timeline-date-text {
  font-size: 12px;
  opacity: 0.7;
  font-weight: 500;
}

.timeline-main {
  flex: 1;
  min-width: 0;
}

.timeline-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  margin-bottom: 4px;
}

.timeline-title-wrapper {
  display: flex;
  align-items: center;
  gap: 8px;
  flex: 1;
  min-width: 0;
}

.timeline-icon {
  font-size: 18px;
  flex-shrink: 0;
}

.timeline-title {
  font-size: 14px;
  font-weight: 600;
  flex: 1;
  min-width: 0;
}

.timeline-desc {
  font-size: 13px;
  opacity: 0.7;
  line-height: 1.5;
}
</style>
