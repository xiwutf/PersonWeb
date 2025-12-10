<template>
  <n-card class="dashboard-card chart-card">
    <template #header>
      <div class="chart-card-header">
        <h3 class="chart-card-title">收入趋势</h3>
      </div>
    </template>
    <div v-if="loading" class="chart-loading">
      <n-spin size="large" />
    </div>
    <div v-else-if="!data || data.length === 0" class="chart-empty">
      暂无数据
    </div>
    <div v-else class="chart-container">
      <v-chart :option="chartOption" :theme="chartTheme" autoresize />
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { LineChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import { registerTheme } from 'echarts/core'
import type { IncomeTrendPointDto } from '~/types/api'

// 注册自定义主题
registerTheme('dark-custom', {
  backgroundColor: 'transparent',
  textStyle: { color: '#ffffff' },
  title: { textStyle: { color: '#ffffff' } },
  legend: { textStyle: { color: '#e5e7eb' } },
  tooltip: {
    backgroundColor: 'rgba(17, 24, 39, 0.98)',
    borderColor: 'rgba(156, 163, 175, 0.5)',
    textStyle: { color: '#ffffff' }
  }
})

registerTheme('light-custom', {
  backgroundColor: 'transparent',
  textStyle: { color: '#374151' },
  title: { textStyle: { color: '#111827' } },
  legend: { textStyle: { color: '#6b7280' } },
  tooltip: {
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
    borderColor: 'rgba(209, 213, 219, 0.8)',
    textStyle: { color: '#111827' }
  }
})

use([
  CanvasRenderer,
  LineChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

interface Props {
  data: IncomeTrendPointDto[] | null
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})

const { isDark, applyTheme, buildNeonLineOptions } = useEChartsTheme()
const chartTheme = computed(() => (isDark.value ? 'dark-custom' : 'light-custom'))

const chartOption = computed(() => {
  if (!props.data || props.data.length === 0) {
    return {}
  }

  // 使用霓虹主题构建折线图
  const baseOption = {
    backgroundColor: 'rgba(15,23,42,0.35)', // 玻璃背景，让曲线"浮起来"
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      },
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>${param.seriesName}: ¥${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
      }
    },
    grid: {
      left: 40,
      right: 40,
      top: 50,
      bottom: 40,
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: props.data.map(item => item.date)
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        formatter: (value: number) => {
          if (value >= 10000) {
            return `¥${(value / 10000).toFixed(1)}万`
          }
          return `¥${value}`
        }
      }
    },
    series: [
      buildNeonLineOptions('--color-primary', {
        name: '收入',
        data: props.data.map(item => item.income),
        emphasis: {
          focus: 'series'
        }
      })
    ]
  }

  // 应用主题（弱化网格、强化数据线）
  return applyTheme(baseOption)
})
</script>

<style scoped>
/* dashboard-chart-card 的颜色、边框、阴影已移除，由 themeOverrides.Card 统一控制 */

/* 使用全局 chart-card-header 和 chart-card-title 样式 */

.chart-container {
  height: 320px;
  width: 100%;
}

.chart-loading,
.chart-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 320px;
  color: var(--color-text-muted);
}
</style>

