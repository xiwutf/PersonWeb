<template>
  <div class="kpi-row">
    <div
      v-for="(kpi, index) in kpis"
      :key="kpi.key"
      class="kpi-card"
      :style="{ animationDelay: `${index * 100}ms` }"
    >
      <div class="kpi-card-icon-wrapper">
        <div class="kpi-card-icon" :class="`kpi-card-icon--${kpi.color}`">
          <span class="kpi-icon-emoji">{{ kpi.icon }}</span>
        </div>
      </div>
      <div class="kpi-card-content">
        <div class="kpi-card-label">{{ kpi.label }}</div>
        <div class="kpi-card-value">{{ formatValue(kpi.value) }}</div>
        <div class="kpi-card-trend" v-if="kpi.trend !== null">
          <span :class="kpi.trend > 0 ? 'trend-up' : 'trend-down'">
            {{ kpi.trend > 0 ? '↑' : '↓' }}
            {{ Math.abs(kpi.trend).toFixed(1) }}%
          </span>
        </div>
        <div class="kpi-card-trend" v-else>--</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface KpiItem {
  key: string
  label: string
  value: number
  icon: string
  color: 'blue' | 'green' | 'yellow' | 'orange'
  trend: number | null // 百分比变化，null 表示无数据
}

interface Props {
  kpis: KpiItem[]
}

const props = withDefaults(defineProps<Props>(), {
  kpis: () => []
})

const formatValue = (value: number) => {
  if (value >= 10000) {
    return (value / 10000).toFixed(1) + 'w'
  }
  return value.toLocaleString('zh-CN')
}
</script>

<style scoped>
.kpi-row {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 1rem;
  margin-bottom: 2rem;
}

@media (min-width: 640px) {
  .kpi-row {
    grid-template-columns: repeat(2, 1fr);
    gap: 1.25rem;
  }
}

@media (min-width: 1024px) {
  .kpi-row {
    grid-template-columns: repeat(4, 1fr);
    gap: 1.5rem;
  }
}

.kpi-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.5rem;
  display: flex;
  align-items: center;
  gap: 1.25rem;
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

.kpi-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.2);
  background: var(--color-border);
  border-color: rgba(255, 255, 255, 0.15);
}

.kpi-card-icon-wrapper {
  flex-shrink: 0;
}

.kpi-card-icon {
  width: 4rem;
  height: 4rem;
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
}

.kpi-card-icon::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 1rem;
  opacity: 0.2;
  background: linear-gradient(135deg, var(--icon-color-start), var(--icon-color-end));
}

.kpi-card-icon--blue {
  --icon-color-start: var(--color-primary);
  --icon-color-end: var(--color-primary-hover);
}

.kpi-card-icon--green {
  --icon-color-start: var(--color-success);
  --icon-color-end: var(--color-success, #16a34a);
}

.kpi-card-icon--yellow {
  --icon-color-start: var(--color-warning, #eab308);
  --icon-color-end: var(--color-warning-dark, #ca8a04);
}

.kpi-card-icon--orange {
  --icon-color-start: var(--color-orange-500, #f97316);
  --icon-color-end: var(--color-orange-600, #ea580c);
}

.kpi-icon-emoji {
  font-size: 1.75rem;
  position: relative;
  z-index: 1;
}

.kpi-card-content {
  flex: 1;
  min-width: 0;
}

.kpi-card-label {
  font-size: 0.875rem;
  color: var(--color-text-muted, #9ca3af);
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.kpi-card-value {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-text-main, #f3f4f6);
  line-height: 1.2;
  margin-bottom: 0.25rem;
}

@media (min-width: 768px) {
  .kpi-card-value {
    font-size: 2.25rem;
  }
}

.kpi-card-trend {
  font-size: 0.75rem;
  font-weight: 500;
  margin-top: 0.25rem;
}

.trend-up {
  color: var(--color-success);
}

.trend-down {
  color: var(--color-danger, #ef4444);
}
</style>

