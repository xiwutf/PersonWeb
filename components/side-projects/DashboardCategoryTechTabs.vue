<template>
  <n-card class="dashboard-card">
    <n-tabs v-model:value="activeTab" type="line" animated>
      <n-tab-pane name="category" tab="项目类型">
        <div v-if="!categoryData || categoryData.length === 0" class="chart-empty">
          暂无数据
        </div>
        <div v-else class="chart-container">
          <v-chart :option="categoryChartOption" :theme="chartTheme" autoresize />
        </div>
      </n-tab-pane>
      <n-tab-pane name="tech" tab="技术栈">
        <div v-if="!techData || techData.length === 0" class="chart-empty">
          暂无数据
        </div>
        <div v-else class="chart-container">
          <v-chart :option="techChartOption" :theme="chartTheme" autoresize />
        </div>
      </n-tab-pane>
    </n-tabs>
  </n-card>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
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
import { registerTheme } from 'echarts/core'
import type { CategoryDistributionItemDto, TechStackDistributionItemDto } from '~/types/api'

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
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent
])

interface Props {
  categoryData: CategoryDistributionItemDto[] | null
  techData: TechStackDistributionItemDto[] | null
}

const props = defineProps<Props>()

const activeTab = ref('category')
const { isDark, applyTheme } = useEChartsTheme()
const chartTheme = computed(() => (isDark.value ? 'dark-custom' : 'light-custom'))

const colors = [
  '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
  '#ec4899', '#06b6d4', '#84cc16', '#f97316', '#6366f1'
]

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
      textStyle: {
        color: isDark.value ? 'rgba(255, 255, 255, 0.7)' : '#6b7280',
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
        radius: '70%',
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 4,
          borderColor: isDark.value ? 'rgba(15, 23, 42, 1)' : '#fff',
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
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        },
        data: props.categoryData.map((item, index) => ({
          value: item.count,
          name: item.category || '未分类',
          income: item.income,
          itemStyle: {
            color: colors[index % colors.length]
          }
        }))
      }
    ]
  }
  
  // 应用主题（弱化网格、强化数据线）
  return applyTheme(baseOption)
})

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
        color: isDark.value ? 'rgba(255, 255, 255, 0.7)' : '#6b7280',
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
          borderColor: isDark.value ? 'rgba(15, 23, 42, 1)' : '#fff',
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
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        },
        data: topTech.map((item, index) => ({
          value: item.count,
          name: item.tech,
          income: item.income,
          itemStyle: {
            color: colors[index % colors.length]
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

