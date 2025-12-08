<template>
  <div class="timeline-card">
    <div class="card-header">
      <h3 class="card-title">最近活动</h3>
    </div>
    <div class="timeline-list">
      <div
        v-for="(item, index) in items"
        :key="item.path"
        class="timeline-item"
      >
        <div class="timeline-line" v-if="index < items.length - 1"></div>
        <div class="timeline-dot" :class="`timeline-dot--${item.color}`"></div>
        <div class="timeline-content">
          <div class="timeline-header">
            <div class="timeline-icon">{{ item.icon }}</div>
            <div class="timeline-title">{{ item.title }}</div>
          </div>
          <div class="timeline-desc">{{ item.desc }}</div>
          <NuxtLink :to="item.path" class="timeline-link">查看 →</NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface TimelineItem {
  path: string
  icon: string
  title: string
  desc: string
  color: 'blue' | 'purple' | 'yellow' | 'teal'
}

interface Props {
  items: TimelineItem[]
}

withDefaults(defineProps<Props>(), {
  items: () => []
})
</script>

<style scoped>
.timeline-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.5rem;
}

.card-header {
  margin-bottom: 1.5rem;
}

.card-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #f3f4f6;
}

.timeline-list {
  position: relative;
  padding-left: 1.5rem;
}

.timeline-item {
  position: relative;
  padding-bottom: 1.5rem;
}

.timeline-item:last-child {
  padding-bottom: 0;
}

.timeline-line {
  position: absolute;
  left: 0.5rem;
  top: 1.5rem;
  bottom: -1.5rem;
  width: 2px;
  background: rgba(255, 255, 255, 0.1);
}

.timeline-item:last-child .timeline-line {
  display: none;
}

.timeline-dot {
  position: absolute;
  left: 0;
  top: 0.25rem;
  width: 1rem;
  height: 1rem;
  border-radius: 50%;
  border: 2px solid rgba(255, 255, 255, 0.1);
  background: rgba(255, 255, 255, 0.05);
  z-index: 1;
}

.timeline-dot--blue {
  background: rgba(59, 130, 246, 0.2);
  border-color: rgba(59, 130, 246, 0.4);
}

.timeline-dot--purple {
  background: rgba(168, 85, 247, 0.2);
  border-color: rgba(168, 85, 247, 0.4);
}

.timeline-dot--yellow {
  background: rgba(234, 179, 8, 0.2);
  border-color: rgba(234, 179, 8, 0.4);
}

.timeline-dot--teal {
  background: rgba(20, 184, 166, 0.2);
  border-color: rgba(20, 184, 166, 0.4);
}

.timeline-content {
  margin-left: 2rem;
}

.timeline-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.5rem;
}

.timeline-icon {
  font-size: 1.25rem;
}

.timeline-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #f3f4f6;
}

.timeline-desc {
  font-size: 0.875rem;
  color: #9ca3af;
  margin-bottom: 0.5rem;
}

.timeline-link {
  font-size: 0.875rem;
  color: #60a5fa;
  text-decoration: none;
  transition: color 0.2s;
}

.timeline-link:hover {
  color: #93c5fd;
  text-decoration: underline;
}
</style>

