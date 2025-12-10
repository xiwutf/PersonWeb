<template>
  <n-grid :cols="4" :x-gap="16" :y-gap="16">
    <n-grid-item v-for="(card, index) in cards" :key="index">
      <n-card class="dashboard-card" hoverable>
        <div class="kpi-card-content">
          <div class="kpi-card-header">
            <span class="kpi-card-label">{{ card.label }}</span>
            <n-icon :component="card.icon" :size="20" class="kpi-card-icon" />
          </div>
          <div class="kpi-card-value">{{ card.value }}</div>
          <div class="kpi-card-footer">
            <div class="kpi-card-progress">
              <div 
                class="kpi-card-progress-bar" 
                :style="{ width: `${card.progress}%` }"
              ></div>
            </div>
          </div>
        </div>
      </n-card>
    </n-grid-item>
  </n-grid>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { CashOutline, FolderOutline, TrendingUpOutline, TimeOutline } from '@vicons/ionicons5'
import type { Component } from 'vue'
import type { ProjectDashboardSummaryDto } from '~/types/api'

interface Props {
  summary: ProjectDashboardSummaryDto | null
}

const props = defineProps<Props>()

const cards = computed(() => {
  const summary = props.summary
  if (!summary) {
    return [
      { label: '总收入', value: '¥0', icon: CashOutline as Component, progress: 0 },
      { label: '项目总数', value: '0', icon: FolderOutline as Component, progress: 0 },
      { label: '平均单价', value: '¥0', icon: TrendingUpOutline as Component, progress: 0 },
      { label: '平均周期', value: '0天', icon: TimeOutline as Component, progress: 0 }
    ]
  }

  // 计算进度条（相对于最大值的百分比，这里简化处理）
  const maxIncome = 100000 // 假设最大收入为10万
  const incomeProgress = Math.min((summary.totalIncome / maxIncome) * 100, 100)

  return [
    {
      label: '总收入',
      value: `¥${summary.totalIncome.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`,
      icon: CashOutline as Component,
      progress: incomeProgress
    },
    {
      label: '项目总数',
      value: `${summary.totalProjects}`,
      icon: FolderOutline as Component,
      progress: Math.min((summary.totalProjects / 50) * 100, 100) // 假设最大50个项目
    },
    {
      label: '平均单价',
      value: `¥${summary.avgProjectPrice.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`,
      icon: TrendingUpOutline as Component,
      progress: Math.min((summary.avgProjectPrice / 50000) * 100, 100) // 假设最大5万
    },
    {
      label: '平均周期',
      value: `${Math.round(summary.avgDurationDays)}天`,
      icon: TimeOutline as Component,
      progress: Math.min((summary.avgDurationDays / 90) * 100, 100) // 假设最大90天
    }
  ]
})
</script>

<style scoped>
/* kpi-card 的颜色、边框、阴影已移除，由 themeOverrides.Card 统一控制 */
/* 只保留布局和动画相关的样式 */
.dashboard-card {
  transition: transform 0.3s ease;
  overflow: hidden;
}

.dashboard-card:hover {
  transform: translateY(-4px);
}

.kpi-card-content {
  padding: var(--spacing-sm);
}

.kpi-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
}

.kpi-card-label {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

.kpi-card-icon {
  color: var(--color-primary);
  opacity: 0.7;
}

.kpi-card-value {
  font-size: var(--font-size-h3);
  font-weight: bold;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-md);
}

.kpi-card-footer {
  margin-top: var(--spacing-sm);
}

.kpi-card-progress {
  height: 4px;
  background-color: var(--color-bg-elevated);
  border-radius: var(--radius-sm);
  overflow: hidden;
}

.kpi-card-progress-bar {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-primary-soft));
  border-radius: var(--radius-sm);
  transition: width 0.5s ease;
}
</style>

