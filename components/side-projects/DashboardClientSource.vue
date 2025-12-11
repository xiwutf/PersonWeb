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
      <v-chart :option="chartOption" autoresize />
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
import type { ClientSourceItemDto } from '~/types/api'

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

const { isDark, echartsTheme, applyTheme, buildNeonBarOptions, buildNeonLineOptions, getCssVar } = useEChartsTheme()

const chartOption = computed(() => {
  if (!props.data || props.data.length === 0) {
    return {}
  }

  const baseOption = {
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
      data: ['项目数', '收入']
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
      data: props.data.map(item => item.source || '未知'),
      axisLabel: {
        rotate: props.data.length > 5 ? 45 : 0
      }
    },
    yAxis: [
      {
        type: 'value',
        name: '项目数',
        position: 'left',
        splitLine: {
          show: true
        }
      },
      {
        type: 'value',
        name: '收入',
        position: 'right',
        axisLabel: {
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
        ...buildNeonBarOptions(
          '--chart-primary',
          '--chart-septenary'
        )
      },
      buildNeonLineOptions('--chart-secondary', {
        name: '收入',
        yAxisIndex: 1,
        data: props.data.map(item => item.income)
      })
    ]
  }

  // 应用主题（自动处理颜色）
  return applyTheme(baseOption)
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

/* 图表容器高度统一为 320px，由全局样式 glassmorphism.css 中的 .chart-container 控制 */

/* 图表容器高度统一为 320px，由全局样式 glassmorphism.css 中的 .chart-container 控制 */
/* 空状态样式由全局样式 glassmorphism.css 中的 .chart-empty 控制 */
</style>

