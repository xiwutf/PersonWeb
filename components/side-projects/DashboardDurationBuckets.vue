<template>
  <n-card class="dashboard-card">
    <template #header>
      <div class="chart-header">
        <h3 class="chart-title">项目周期分布</h3>
      </div>
    </template>
    <div v-if="!data || data.length === 0" class="chart-empty">
      暂无数据
    </div>
    <div v-else class="chart-container">
      <v-chart :option="chartOption" autoresize />
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import type { DurationBucketItemDto } from '~/types/api'

use([
  CanvasRenderer,
  BarChart,
  TitleComponent,
  TooltipComponent,
  GridComponent
])

interface Props {
  data: DurationBucketItemDto[] | null
}

const props = defineProps<Props>()

const { isDark, applyTheme, getCssVar, buildNeonBarOptions } = useEChartsTheme()

const chartOption = computed(() => {
  if (!props.data || props.data.length === 0) {
    return {}
  }

  // 按周期顺序排序
  const order = ['0-7天', '7-15天', '15-30天', '30-60天', '60-90天', '90天以上']
  const sortedData = [...props.data].sort((a, b) => {
    const indexA = order.indexOf(a.bucketName)
    const indexB = order.indexOf(b.bucketName)
    return indexA - indexB
  })

  const option = {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      },
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>项目数: ${param.value}`
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
      data: sortedData.map(item => item.bucketName),
      axisLine: {
        lineStyle: {
          color: isDark.value ? 'rgba(255, 255, 255, 0.2)' : 'rgba(0, 0, 0, 0.1)'
        }
      },
      axisLabel: {
        color: isDark.value ? '#e5e7eb' : '#6b7280'
      }
    },
    yAxis: {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: getCssVar('--color-border-default') || (isDark.value ? 'rgba(255, 255, 255, 0.2)' : 'rgba(0, 0, 0, 0.1)')
        }
      },
      axisLabel: {
        color: getCssVar('--color-text-muted') || (isDark.value ? '#e5e7eb' : '#6b7280')
      },
      splitLine: {
        lineStyle: {
          color: getCssVar('--chart-grid') || (isDark.value ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)')
        }
      }
    },
    series: [
      {
        name: '项目数',
        type: 'bar',
        data: sortedData.map(item => item.count),
        ...buildNeonBarOptions('--chart-quinary', '--chart-primary', {
          itemStyle: {
            borderRadius: [4, 4, 0, 0]
          },
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: getCssVar('--color-text-main') 
                ? `${getCssVar('--color-text-main')}40` 
                : 'rgba(0, 0, 0, 0.5)'
            }
          }
        })
      }
    ]
  }
  
  // 应用主题
  return applyTheme(option)
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

