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
const { isDark } = useEChartsTheme()
const chartTheme = computed(() => (isDark.value ? 'dark-custom' : 'light-custom'))

const colors = [
  '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
  '#ec4899', '#06b6d4', '#84cc16', '#f97316', '#6366f1'
]

const categoryChartOption = computed(() => {
  if (!props.categoryData || props.categoryData.length === 0) {
    return {}
  }

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      formatter: (params: any) => {
        return `${params.name}<br/>项目数: ${params.value}<br/>收入: ¥${params.data.income.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}<br/>占比: ${params.percent}%`
      }
    },
    series: [
      {
        name: '项目类型',
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '50%'],
        avoidLabelOverlap: true,
        itemStyle: {
          borderRadius: 8,
          borderColor: isDark.value ? '#1f2937' : '#fff',
          borderWidth: 2
        },
        label: {
          show: true,
          position: 'outside',
          formatter: '{b}\n{c}个',
          fontSize: 12,
          fontWeight: 500,
          color: isDark.value ? '#e5e7eb' : '#374151',
          backgroundColor: isDark.value ? 'rgba(17, 24, 39, 0.9)' : 'rgba(255, 255, 255, 0.95)',
          borderColor: isDark.value ? 'rgba(156, 163, 175, 0.3)' : 'rgba(209, 213, 219, 0.8)',
          borderWidth: 1,
          borderRadius: 4,
          padding: [4, 8]
        },
        labelLine: {
          show: true,
          length: 15,
          length2: 10,
          lineStyle: {
            color: isDark.value ? 'rgba(229, 231, 235, 0.5)' : 'rgba(107, 114, 128, 0.5)',
            width: 1
          }
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 14,
            fontWeight: 'bold'
          },
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
})

const techChartOption = computed(() => {
  if (!props.techData || props.techData.length === 0) {
    return {}
  }

  // 只显示前10个技术栈
  const topTech = props.techData.slice(0, 10)

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      formatter: (params: any) => {
        return `${params.name}<br/>项目数: ${params.value}<br/>收入: ¥${params.data.income.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}<br/>占比: ${params.percent}%`
      }
    },
    series: [
      {
        name: '技术栈',
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '50%'],
        avoidLabelOverlap: true,
        itemStyle: {
          borderRadius: 8,
          borderColor: isDark.value ? '#1f2937' : '#fff',
          borderWidth: 2
        },
        label: {
          show: true,
          position: 'outside',
          formatter: '{b}\n{c}个',
          fontSize: 12,
          fontWeight: 500,
          color: isDark.value ? '#e5e7eb' : '#374151',
          backgroundColor: isDark.value ? 'rgba(17, 24, 39, 0.9)' : 'rgba(255, 255, 255, 0.95)',
          borderColor: isDark.value ? 'rgba(156, 163, 175, 0.3)' : 'rgba(209, 213, 219, 0.8)',
          borderWidth: 1,
          borderRadius: 4,
          padding: [4, 8]
        },
        labelLine: {
          show: true,
          length: 15,
          length2: 10,
          lineStyle: {
            color: isDark.value ? 'rgba(229, 231, 235, 0.5)' : 'rgba(107, 114, 128, 0.5)',
            width: 1
          }
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 14,
            fontWeight: 'bold'
          },
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

