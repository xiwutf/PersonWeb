<template>
  <n-card class="dashboard-card chart-card">
    <div class="chart-card-header">
      <h3 class="chart-card-title">项目类型</h3>
    </div>
    <div v-if="!categoryData || categoryData.length === 0" class="chart-empty">
      暂无数据
    </div>
    <div v-else class="chart-container">
      <v-chart :option="categoryChartOption" autoresize />
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
import type { CategoryDistributionItemDto } from '~/types/api'

use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent
])

interface Props {
  categoryData: CategoryDistributionItemDto[] | null
}

const props = defineProps<Props>()

const { isDark, applyTheme, getCssVar: getChartCssVar } = useEChartsTheme()

// 使用图表颜色变量（从 theme.css 中定义的 --chart-primary 到 --chart-denary）
const colors = computed(() => [
  getChartCssVar('--chart-primary'),
  getChartCssVar('--chart-secondary'),
  getChartCssVar('--chart-tertiary'),
  getChartCssVar('--chart-quaternary'),
  getChartCssVar('--chart-quinary'),
  getChartCssVar('--chart-senary'),
  getChartCssVar('--chart-septenary'),
  getChartCssVar('--chart-octonary'),
  getChartCssVar('--chart-nonary'),
  getChartCssVar('--chart-denary')
])

const categoryChartOption = computed(() => {
  if (!props.categoryData || props.categoryData.length === 0) {
    return {}
  }

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
      type: 'scroll',
      icon: 'circle',
      textStyle: {
        color: getChartCssVar('--color-text-main'),
        fontSize: 12
      },
      itemGap: 15,
      itemWidth: 10,
      itemHeight: 10
    },
    series: [
      {
        name: '项目类型',
        type: 'pie',
        radius: '65%',
        center: ['50%', '42%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 4,
          borderColor: getChartCssVar('--color-bg-card'),
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
            // 阴影颜色：使用主题变量，但 ECharts 需要具体颜色值
            // 通过 getCssVar 获取当前主题的阴影颜色
            shadowColor: getChartCssVar('--color-text-main') 
              ? `${getChartCssVar('--color-text-main')}40` // 添加透明度
              : (isDark.value ? 'rgba(0, 0, 0, 0.5)' : 'rgba(0, 0, 0, 0.2)')
          }
        },
        data: props.categoryData.map((item, index) => ({
          value: item.count,
          name: item.category || '未分类',
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

/* 使用全局 chart-card-header 和 chart-card-title 样式 */
/* 图表容器高度统一为 320px，由全局样式 glassmorphism.css 中的 .chart-container 控制 */
/* 空状态样式由全局样式 glassmorphism.css 中的 .chart-empty 控制 */
</style>

