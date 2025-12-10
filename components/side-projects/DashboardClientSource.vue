<template>
  <n-card class="dashboard-card">
    <template #header>
      <div class="chart-header">
        <h3 class="chart-title">客户来源分布</h3>
      </div>
    </template>
    <div v-if="!data || data.length === 0" class="chart-empty">
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
import { BarChart, LineChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import { registerTheme } from 'echarts/core'
import type { ClientSourceItemDto } from '~/types/api'

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
  BarChart,
  LineChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

interface Props {
  data: ClientSourceItemDto[] | null
}

const props = defineProps<Props>()

const { isDark } = useEChartsTheme()
const chartTheme = computed(() => (isDark.value ? 'dark-custom' : 'light-custom'))

const chartOption = computed(() => {
  if (!props.data || props.data.length === 0) {
    return {}
  }

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      },
      formatter: (params: any) => {
        let result = `${params[0].name}<br/>`
        params.forEach((param: any) => {
          if (param.seriesName === '项目数') {
            result += `${param.seriesName}: ${param.value}<br/>`
          } else {
            result += `${param.seriesName}: ¥${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}<br/>`
          }
        })
        return result
      }
    },
    legend: {
      data: ['项目数', '收入'],
      textStyle: {
        color: isDark.value ? '#e5e7eb' : '#6b7280'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: props.data.map(item => item.source || '未知'),
      axisLine: {
        lineStyle: {
          color: isDark.value ? 'rgba(255, 255, 255, 0.2)' : 'rgba(0, 0, 0, 0.1)'
        }
      },
      axisLabel: {
        color: isDark.value ? '#e5e7eb' : '#6b7280',
        rotate: props.data.length > 5 ? 45 : 0
      }
    },
    yAxis: [
      {
        type: 'value',
        name: '项目数',
        position: 'left',
        axisLine: {
          lineStyle: {
            color: isDark.value ? 'rgba(255, 255, 255, 0.2)' : 'rgba(0, 0, 0, 0.1)'
          }
        },
        axisLabel: {
          color: isDark.value ? '#e5e7eb' : '#6b7280'
        },
        splitLine: {
          lineStyle: {
            color: isDark.value ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)'
          }
        }
      },
      {
        type: 'value',
        name: '收入',
        position: 'right',
        axisLine: {
          lineStyle: {
            color: isDark.value ? 'rgba(255, 255, 255, 0.2)' : 'rgba(0, 0, 0, 0.1)'
          }
        },
        axisLabel: {
          color: isDark.value ? '#e5e7eb' : '#6b7280',
          formatter: (value: number) => {
            if (value >= 10000) {
              return `¥${(value / 10000).toFixed(1)}万`
            }
            return `¥${value}`
          }
        },
        splitLine: {
          show: false
        }
      }
    ],
    series: [
      {
        name: '项目数',
        type: 'bar',
        data: props.data.map(item => item.count),
        itemStyle: {
          color: '#3b82f6'
        }
      },
      {
        name: '收入',
        type: 'line',
        yAxisIndex: 1,
        data: props.data.map(item => item.income),
        lineStyle: {
          color: '#10b981',
          width: 3
        },
        itemStyle: {
          color: '#10b981'
        }
      }
    ]
  }
})
</script>

<style scoped>
/* dashboard-chart-card 的颜色、边框、阴影已移除，由 themeOverrides.Card 统一控制 */

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.chart-title {
  font-size: var(--font-size-h4);
  font-weight: bold;
  color: var(--color-text-main);
}

.chart-container {
  height: 320px;
  width: 100%;
}

.chart-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 320px;
  color: var(--color-text-muted);
}
</style>

