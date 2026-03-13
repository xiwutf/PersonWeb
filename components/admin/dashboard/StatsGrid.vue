<template>
  <div class="stats-grid">
    <div
      v-for="(stat, index) in stats"
      :key="stat.key"
      class="stat-card"
      :style="{ animationDelay: `${index * 100}ms` }"
    >
      <div class="stat-card-icon-wrapper">
        <div class="stat-card-icon" :class="`stat-card-icon--${stat.color}`">
          <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              :d="stat.iconPath"
            />
          </svg>
        </div>
      </div>
      <div class="stat-card-content">
        <div class="stat-card-label">{{ stat.label }}</div>
        <div class="stat-card-value">{{ formatValue(stat.value) }}</div>
        <div class="stat-card-desc">{{ stat.desc }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface StatItem {
  key: string
  label: string
  value: number
  desc: string
  iconPath: string
  color: 'blue' | 'purple' | 'green' | 'cyan' | 'yellow' | 'teal'
}

interface Props {
  stats: StatItem[]
}

const props = withDefaults(defineProps<Props>(), {
  stats: () => []
})

const formatValue = (value: number) => {
  if (value >= 10000) {
    return (value / 10000).toFixed(1) + 'w'
  }
  return value.toLocaleString('zh-CN')
}
</script>

<style scoped>
.stats-grid {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 1rem;
  margin-bottom: 2rem;
}

@media (min-width: 640px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1.25rem;
  }
}

@media (min-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
  }
}

.stat-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.25rem;
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  transition: all 0.3s ease;
  opacity: 0;
  transform: translateY(20px);
  animation: fadeInUp 0.6s ease forwards;
}

@keyframes fadeInUp {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.2);
  background: var(--color-border);
  border-color: rgba(255, 255, 255, 0.15);
}

.stat-card-icon-wrapper {
  flex-shrink: 0;
}

.stat-card-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-card-icon svg {
  width: 1.25rem;
  height: 1.25rem;
}

.stat-card-icon--blue {
  background-color: rgba(59, 130, 246, 0.15);
  color: var(--color-primary-soft);
}

.stat-card-icon--purple {
  background-color: rgba(168, 85, 247, 0.15);
  color: #a78bfa;
}

.stat-card-icon--green {
  background-color: rgba(34, 197, 94, 0.15);
  color: #86efac;
}

.stat-card-icon--cyan {
  background-color: rgba(6, 182, 212, 0.15);
  color: #22d3ee;
}

.stat-card-icon--yellow {
  background-color: rgba(234, 179, 8, 0.15);
  color: #fbbf24;
}

.stat-card-icon--teal {
  background-color: rgba(20, 184, 166, 0.15);
  color: #2dd4bf;
}

.stat-card-content {
  flex: 1;
  min-width: 0;
}

.stat-card-label {
  font-size: 0.75rem;
  color: #9ca3af;
  margin-bottom: 0.5rem;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stat-card-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #f3f4f6;
  line-height: 1.2;
  margin-bottom: 0.25rem;
}

.stat-card-desc {
  font-size: 0.75rem;
  color: var(--color-text-sec);
  margin-top: 0.25rem;
}
</style>

