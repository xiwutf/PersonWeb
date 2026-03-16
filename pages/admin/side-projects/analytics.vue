<template>
  <div class="analytics-page">
    <!-- 顶部筛选条 -->
    <n-card class="filter-card mb-4">
      <div class="filter-row">
        <n-select
          v-model:value="filters.timeRange"
          :options="timeRangeOptions"
          placeholder="时间范围"
          style="width: 150px"
          @update:value="handleTimeRangeChange"
        />
        <n-date-picker
          v-if="filters.timeRange === 'custom'"
          v-model:value="customDateRange"
          type="daterange"
          clearable
          placeholder="自定义日期范�?
          style="width: 300px"
          @update:value="handleCustomDateChange"
        />
        <n-select
          v-model:value="filters.projectType"
          :options="projectTypeOptions"
          placeholder="项目类型"
          style="width: 150px"
          clearable
          @update:value="handleFilterChange"
        />
        <n-select
          v-model:value="filters.isPublic"
          :options="isPublicOptions"
          placeholder="是否公开"
          style="width: 120px"
          clearable
          @update:value="handleFilterChange"
        />
        <n-button type="primary" @click="handleRefresh">刷新</n-button>
      </div>
    </n-card>

    <!-- KPI 卡片 -->
    <div class="kpi-cards mb-4">
      <n-card class="kpi-card">
        <div class="kpi-content">
          <div class="kpi-label">项目总数</div>
          <div class="kpi-value">{{ summary.kpis?.totals || 0 }}</div>
        </div>
      </n-card>
      <n-card class="kpi-card">
        <div class="kpi-content">
          <div class="kpi-label">进行中项�?/div>
          <div class="kpi-value">{{ summary.kpis?.inProgressCount || 0 }}</div>
        </div>
      </n-card>
      <n-card class="kpi-card">
        <div class="kpi-content">
          <div class="kpi-label">逾期项目</div>
          <div class="kpi-value error">{{ summary.kpis?.overdueCount || 0 }}</div>
        </div>
      </n-card>
      <n-card class="kpi-card">
        <div class="kpi-content">
          <div class="kpi-label">卡住项目</div>
          <div class="kpi-value warning">{{ summary.kpis?.blockedCount || 0 }}</div>
        </div>
      </n-card>
      <n-card class="kpi-card">
        <div class="kpi-content">
          <div class="kpi-label">本期已收金额</div>
          <div class="kpi-value success">¥{{ formatAmount(summary.kpis?.receivedSum || 0) }}</div>
        </div>
      </n-card>
      <n-card class="kpi-card">
        <div class="kpi-content">
          <div class="kpi-label">本期待收金额</div>
          <div class="kpi-value">¥{{ formatAmount(summary.kpis?.receivableSum || 0) }}</div>
        </div>
      </n-card>
    </div>

    <!-- 图表�?-->
    <div v-if="loading" class="loading-container">
      <n-spin size="large" />
    </div>

    <div v-else class="charts-grid">
      <!-- 项目状态分�?-->
      <n-card class="chart-card">
        <template #header>
          <h3>项目状态分�?/h3>
        </template>
        <div v-if="!summary.statusAgg || summary.statusAgg.length === 0" class="chart-empty">
          暂无数据
        </div>
        <div v-else class="chart-container">
          <v-chart :option="statusChartOption" autoresize />
        </div>
      </n-card>

      <!-- 月度收入趋势 -->
      <n-card class="chart-card">
        <template #header>
          <h3>月度收入趋势</h3>
        </template>
        <div v-if="!summary.monthlyRevenue || summary.monthlyRevenue.length === 0" class="chart-empty">
          暂无数据
        </div>
        <div v-else class="chart-container">
          <v-chart :option="revenueChartOption" autoresize />
        </div>
      </n-card>

      <!-- 交付周期统计 -->
      <n-card class="chart-card">
        <template #header>
          <h3>交付周期统计</h3>
        </template>
        <div v-if="!summary.deliveryCycle || summary.deliveryCycle.length === 0" class="chart-empty">
          暂无数据
        </div>
        <div v-else class="chart-container">
          <v-chart :option="deliveryChartOption" autoresize />
        </div>
      </n-card>

      <!-- 客户贡献 Top10 -->
      <n-card class="chart-card">
        <template #header>
          <h3>客户贡献 Top10</h3>
        </template>
        <div v-if="!summary.customerTop || summary.customerTop.length === 0" class="chart-empty">
          暂无数据
        </div>
        <div v-else class="chart-container">
          <v-chart :option="customerChartOption" autoresize />
        </div>
      </n-card>
    </div>

    <!-- 风险列表 -->
    <div class="risk-lists mt-4">
      <!-- 即将到期项目 -->
      <n-card class="risk-card mb-4">
        <template #header>
          <h3>即将到期项目（≤7天）</h3>
        </template>
        <n-data-table
          :columns="dueSoonColumns"
          :data="summary.riskDueSoon || []"
          :loading="loading"
          :pagination="false"
          @row-click="handleRowClick"
        />
      </n-card>

      <!-- 逾期项目 -->
      <n-card class="risk-card">
        <template #header>
          <h3>逾期项目</h3>
        </template>
        <n-data-table
          :columns="overdueColumns"
          :data="summary.riskOverdue || []"
          :loading="loading"
          :pagination="false"
          @row-click="handleRowClick"
        />
      </n-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { NCard, NSelect, NDatePicker, NButton, NDataTable, NSpin } from 'naive-ui'
import { useNotification } from '~/composables/useToast'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { BarChart, LineChart, PieChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import { useApi } from '~/composables/useApi'
import type { SideProjectAnalyticsSummary, ProjectBrief } from '~/types/api'
import type { DataTableColumns } from 'naive-ui'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

// 注册 ECharts 组件
use([
  CanvasRenderer,
  BarChart,
  LineChart,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

const router = useRouter()
const api = useApi()
const notification = useNotification()
const { isDark, getCssVar } = useEChartsTheme()

// 数据
const loading = ref(false)
const summary = ref<SideProjectAnalyticsSummary>({
  kpis: {
    totals: 0,
    inProgressCount: 0,
    overdueCount: 0,
    blockedCount: 0,
    receivedSum: 0,
    receivableSum: 0
  },
  statusAgg: [],
  monthlyRevenue: [],
  deliveryCycle: [],
  customerTop: [],
  riskDueSoon: [],
  riskOverdue: []
})

// 筛选条�?const filters = ref({
  timeRange: '30d',
  projectType: undefined as string | undefined,
  isPublic: undefined as string | undefined // 使用字符串：'true'/'false'/undefined
})

const customDateRange = ref<[number, number] | null>(null)

// 选项
const timeRangeOptions = [
  { label: '�?0�?, value: '30d' },
  { label: '�?0�?, value: '90d' },
  { label: '本年', value: 'year' },
  { label: '自定�?, value: 'custom' }
]

const projectTypeOptions = [
  { label: '全部', value: undefined },
  { label: '软件开�?, value: 'development' },
  { label: '网站', value: '网站' },
  { label: 'AI', value: 'AI' },
  { label: '脚本', value: '脚本' },
  { label: '投资', value: 'investment' }
]

const isPublicOptions = [
  { label: '全部', value: undefined },
  { label: '公开', value: 'true' },
  { label: '不公开', value: 'false' }
]

// 计算日期范围
const getDateRange = (): { start: Date | null; end: Date | null } => {
  if (filters.value.timeRange === 'custom' && customDateRange.value) {
    return {
      start: new Date(customDateRange.value[0]),
      end: new Date(customDateRange.value[1])
    }
  }

  const now = new Date()
  let start: Date | null = null

  switch (filters.value.timeRange) {
    case '30d':
      start = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000)
      break
    case '90d':
      start = new Date(now.getTime() - 90 * 24 * 60 * 60 * 1000)
      break
    case 'year':
      start = new Date(now.getFullYear(), 0, 1)
      break
  }

  return { start, end: now }
}

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { start, end } = getDateRange()
    const params: any = {}
    if (start) params.start = start.toISOString().split('T')[0]
    if (end) params.end = end.toISOString().split('T')[0]
    if (filters.value.projectType) params.type = filters.value.projectType
    if (filters.value.isPublic !== undefined) {
      params.isPublic = filters.value.isPublic === 'true'
    }

    const res = await api.get<SideProjectAnalyticsSummary>('/side-projects/analytics/summary', { params })
    summary.value = res
  } catch (e: any) {
    console.error('获取数据分析失败:', e)
    notification.error('获取数据分析失败: ' + (e.message || '未知错误'))
  } finally {
    loading.value = false
  }
}

// 筛选变化处�?const handleTimeRangeChange = () => {
  if (filters.value.timeRange !== 'custom') {
    customDateRange.value = null
  }
  handleFilterChange()
}

const handleCustomDateChange = () => {
  handleFilterChange()
}

const handleFilterChange = () => {
  fetchData()
}

const handleRefresh = () => {
  fetchData()
}

// 图表配置
const statusChartOption = computed(() => {
  if (!summary.value.statusAgg || summary.value.statusAgg.length === 0) {
    return {}
  }

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)<br/>金额: ¥{@amountSum}'
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      textStyle: { color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)' }
    },
    series: [
      {
        type: 'pie',
        radius: '60%',
        data: summary.value.statusAgg.map(s => ({
          value: s.count,
          name: s.statusName,
          amountSum: s.amountSum
        })),
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'var(--overlay-color, rgba(0, 0, 0, 0.5))'
          }
        }
      }
    ]
  }
})

const revenueChartOption = computed(() => {
  if (!summary.value.monthlyRevenue || summary.value.monthlyRevenue.length === 0) {
    return {}
  }

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>收入: ¥${formatAmount(param.value)}`
      }
    },
    xAxis: {
      type: 'category',
      data: summary.value.monthlyRevenue.map(m => m.month),
      axisLabel: { color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)' }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)',
        formatter: (value: number) => `¥${(value / 1000).toFixed(0)}k`
      }
    },
    series: [
      {
        type: 'line',
        data: summary.value.monthlyRevenue.map(m => m.receivedSum),
        smooth: true,
        areaStyle: {
          opacity: 0.3
        }
      }
    ]
  }
})

const deliveryChartOption = computed(() => {
  if (!summary.value.deliveryCycle || summary.value.deliveryCycle.length === 0) {
    return {}
  }

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>平均天数: ${param.value.toFixed(1)}�?br/>项目�? ${param.data.count}`
      }
    },
    xAxis: {
      type: 'category',
      data: summary.value.deliveryCycle.map(d => d.groupName),
      axisLabel: {
        color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)',
        rotate: 45
      }
    },
    yAxis: {
      type: 'value',
      name: '天数',
      axisLabel: { color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)' }
    },
    series: [
      {
        type: 'bar',
        data: summary.value.deliveryCycle.map(d => ({
          value: d.avgDays,
          count: d.count
        }))
      }
    ]
  }
})

const customerChartOption = computed(() => {
  if (!summary.value.customerTop || summary.value.customerTop.length === 0) {
    return {}
  }

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'axis',
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>金额: ¥${formatAmount(param.value)}<br/>项目�? ${param.data.count}`
      }
    },
    xAxis: {
      type: 'value',
      axisLabel: {
        color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)',
        formatter: (value: number) => `¥${(value / 1000).toFixed(0)}k`
      }
    },
    yAxis: {
      type: 'category',
      data: summary.value.customerTop.map(c => c.customerName),
      axisLabel: { color: isDark.value ? 'var(--color-bg-light, white)' : 'var(--color-gray-700)' }
    },
    series: [
      {
        type: 'bar',
        data: summary.value.customerTop.map(c => ({
          value: c.receivedSum,
          count: c.projectCount
        })),
        label: {
          show: true,
          position: 'right',
          formatter: (params: any) => `¥${formatAmount(params.value)}`
        }
      }
    ]
  }
})

// 表格列定�?const dueSoonColumns: DataTableColumns<ProjectBrief> = [
  { title: '项目名称', key: 'title', width: 200 },
  { title: '客户', key: 'clientName', width: 150 },
  {
    title: '截止日期',
    key: 'deadlineAt',
    width: 120,
    render: (row) => row.deadlineAt ? new Date(row.deadlineAt).toLocaleDateString('zh-CN') : '-'
  },
  {
    title: '剩余天数',
    key: 'daysRemaining',
    width: 100,
    render: (row) => row.daysRemaining !== undefined ? `${row.daysRemaining}天` : '-'
  },
  { title: '下一�?, key: 'nextAction', width: 200 },
  {
    title: '金额',
    key: 'totalAmount',
    width: 120,
    render: (row) => row.totalAmount ? `¥${formatAmount(row.totalAmount)}` : '-'
  }
]

const overdueColumns: DataTableColumns<ProjectBrief> = [
  { title: '项目名称', key: 'title', width: 200 },
  {
    title: '逾期天数',
    key: 'overdueDays',
    width: 100,
    render: (row) => row.overdueDays !== undefined ? `${row.overdueDays}天` : '-'
  },
  { title: '阻塞原因', key: 'blockReason', width: 300 },
  {
    title: '金额',
    key: 'totalAmount',
    width: 120,
    render: (row) => row.totalAmount ? `¥${formatAmount(row.totalAmount)}` : '-'
  }
]

// 行点�?const handleRowClick = (row: ProjectBrief) => {
  router.push(`/admin/side-projects/projects/${row.id}`)
}

// 工具函数
const formatAmount = (amount: number): string => {
  return amount.toFixed(2)
}

// 初始�?onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.analytics-page {
  padding: var(--spacing-lg);
}

.filter-card {
  background: var(--color-bg-card);
}

.filter-row {
  display: flex;
  gap: var(--spacing-md);
  align-items: center;
  flex-wrap: wrap;
}

.kpi-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: var(--spacing-md);
}

.kpi-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
}

.kpi-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.kpi-label {
  font-size: 14px;
  color: var(--color-text-muted);
}

.kpi-value {
  font-size: 24px;
  font-weight: 600;
  color: var(--color-text-main);
}

.kpi-value.success {
  color: var(--color-success);
}

.kpi-value.error {
  color: var(--color-error);
}

.kpi-value.warning {
  color: var(--color-warning);
}

.charts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  gap: var(--spacing-lg);
}

.chart-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
}

.chart-container {
  height: 400px;
  width: 100%;
}

.chart-empty {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 400px;
  color: var(--color-text-muted);
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.risk-lists {
  display: flex;
  flex-direction: column;
}

.risk-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
}

.risk-card :deep(.n-data-table) {
  cursor: pointer;
}

.risk-card :deep(.n-data-table tr:hover) {
  background-color: var(--color-bg-hover);
}
</style>

