<template>
  <div class="kpi-card relative overflow-hidden rounded-2xl p-5 transition-all duration-300 group hover:translate-y-[-2px]">
    <!-- 背景光效 -->
    <div class="absolute top-0 right-0 w-24 h-24 bg-primary/10 rounded-full blur-2xl -mr-8 -mt-8 transition-opacity group-hover:opacity-100 opacity-60"></div>
    
    <div class="subtitle-text text-sm font-medium mb-3 flex items-center gap-2">
      {{ label }}
      <div v-if="loading" class="animate-pulse w-4 h-4 rounded kpi-loading-dot"></div>
    </div>
    
    <div class="flex items-baseline gap-2 mb-1">
      <div class="text-3xl font-bold tracking-tight text-main relative z-10">
        <template v-if="loading">
          <div class="h-8 w-24 kpi-loading-bar rounded animate-pulse"></div>
        </template>
        <template v-else>{{ formattedValue }}</template>
      </div>
      <div v-if="unit" class="text-sm text-muted">{{ unit }}</div>
    </div>

    <!-- Trend -->
    <div v-if="trend != null && trend !== undefined && !loading" class="flex items-center text-xs font-medium mt-1">
      <span 
        class="flex items-center gap-1 px-1.5 py-0.5 rounded-md"
        :class="(trend ?? 0) >= 0 ? 'text-success bg-success/10' : 'text-error bg-error/10'"
      >
        <span>{{ (trend ?? 0) >= 0 ? '↑' : '↓' }}</span>
        <span>{{ Math.abs(Number(trend) || 0).toFixed(1) }}%</span>
      </span>
      <span class="text-muted ml-2 transform scale-90 origin-left">较昨日</span>
    </div>
    
    <!-- 右上角装饰点 -->
    <div class="absolute top-3 right-3 w-1.5 h-1.5 rounded-full bg-primary shadow-glow-sm opacity-60"></div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  label: string
  value: number | string
  trend?: number | null
  unit?: string
  loading?: boolean
  precision?: number
}>()

const formattedValue = computed(() => {
  if (typeof props.value === 'string') return props.value
  const num = Number(props.value)
  if (Number.isNaN(num)) return '0'
  return num.toLocaleString('zh-CN', {
    minimumFractionDigits: props.precision ?? 0,
    maximumFractionDigits: props.precision ?? 0
  })
})
</script>

<style scoped>
.kpi-card {
  background: var(--bg-card);
  border: 1px solid var(--n-border-color);
  box-shadow: var(--shadow-card);
}

.text-main {
  color: var(--n-text-color);
}

.text-muted {
  color: var(--n-text-color-3);
}

.bg-primary\/10 {
  background-color: var(--n-primary-color-suppl);
}

.shadow-glow-sm {
  box-shadow: 0 0 8px var(--n-primary-color);
}

.text-success { color: var(--n-success-color); }
.bg-success\/10 { background-color: rgba(34, 197, 94, 0.1); }

.text-error { color: var(--n-error-color); }
.bg-error\/10 { background-color: rgba(239, 68, 68, 0.1); }

/* 加载状态样式 - 使用 CSS 变量 */
.kpi-loading-dot {
  background: var(--color-bg-elevated, var(--color-border));
}

.kpi-loading-bar {
  background: var(--color-bg-elevated, var(--color-border));
}

/* 深色主题适配 */
html[data-theme="dark"] .kpi-loading-dot,
html.dark .kpi-loading-dot {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.1));
}

html[data-theme="dark"] .kpi-loading-bar,
html.dark .kpi-loading-bar {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.1));
}
</style>
