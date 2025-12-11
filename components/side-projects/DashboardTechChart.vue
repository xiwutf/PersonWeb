<template>
  <n-card class="dashboard-card">
    <div class="card-header">
      <h3 class="card-title">技术栈</h3>
    </div>
    <div v-if="!techData || techData.length === 0" class="chart-empty">
      暂无数据
    </div>
    <div v-else class="chart-container">
      <v-chart :option="techChartOption" autoresize />
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { PieChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import type { TechStackDistributionItemDto } from '~/types/api'

use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent
])

interface Props {
  techData: TechStackDistributionItemDto[] | null
}

const props = defineProps<Props>()

const { isDark, applyTheme, getCssVar } = useEChartsTheme()

// 使用图表颜色变量（从 theme.css 中定义的 --chart-primary 到 --chart-denary）
const colors = computed(() => [
  getCssVar('--chart-primary'),
  getCssVar('--chart-secondary'),
  getCssVar('--chart-tertiary'),
  getCssVar('--chart-quaternary'),
  getCssVar('--chart-quinary'),
  getCssVar('--chart-senary'),
  getCssVar('--chart-septenary'),
  getCssVar('--chart-octonary'),
  getCssVar('--chart-nonary'),
  getCssVar('--chart-denary')
])

const techChartOption = computed(() => {
  if (!props.techData || props.techData.length === 0) {
    return {}
  }

  // 只显示前10个技术栈
  const topTech = props.techData.slice(0, 10)

  const baseOption = {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      formatter: (params: any) => {
        return `${params.name}<br/>项目数: ${params.value}<br/>收入: ¥${params.data.income.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}<br/>占比: ${params.percent}%`
      }
    },
    legend: {
      orient: 'horizontal',
      bottom: 10,
      left: 'center',
      textStyle: {
        color: getCssVar('--color-text-muted') || getCssVar('--color-text-sub') || (isDark.value ? 'rgba(255, 255, 255, 0.7)' : 'rgba(107, 114, 128, 1)'),
        fontSize: 12
      },
      itemGap: 15,
      itemWidth: 10,
      itemHeight: 10
    },
    series: [
      {
        name: '技术栈',
        type: 'pie',
        radius: '70%',
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 4,
          borderColor: getCssVar('--color-bg-card') || (isDark.value ? 'rgba(15, 23, 42, 1)' : 'rgba(255, 255, 255, 1)'),
          borderWidth: 2
        },
        label: {
          show: false
        },
        labelLine: {
          show: false
        },
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: getCssVar('--color-text-main') 
              ? `${getCssVar('--color-text-main')}40` 
              : 'rgba(0, 0, 0, 0.5)'
          }
        },
        data: topTech.map((item, index) => ({
          value: item.count,
          name: item.tech,
          income: item.income,
          itemStyle: {
            color: colors.value[index % colors.value.length]
          }
        }))
      }
    ]
  }
  
  // 应用主题（弱化网格、强化数据线）
  return applyTheme(baseOption)
})
</script>

<style scoped>
/* dashboard-chart-card 的颜色、边框、阴影已移除，由 themeOverrides.Card 统一控制 */
.dashboard-card {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.card-header {
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border-subtle);
}

.card-title {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.chart-container {
  flex: 1;
  height: 400px;
  width: 100%;
  min-height: 400px;
}

.chart-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 400px;
  color: var(--color-text-muted);
}
</style>

