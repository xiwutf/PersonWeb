<template>
  <div>
    <div class="investment-page-header">
      <h1 class="investment-page-title">投资仪表盘</h1>
      <div class="investment-action-buttons">
        <button @click="refreshPrices" class="btn-refresh">刷新价格</button>
        <button @click="showCreateModal = true" class="btn-add">+ 新增投资</button>
      </div>
    </div>

    <!-- 数据说明提示 -->
    <div class="info-box">
      <div class="info-box-content">
        <div class="info-box-icon">
          <svg viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
          </svg>
        </div>
        <div class="info-box-body">
          <h3 class="info-box-title">数据说明</h3>
          <div class="info-box-text">
            <p class="mb-2"><strong>数据来源：</strong></p>
            <ul class="info-box-list mb-3">
              <li><strong>投资记录</strong>：由您手动添加，包括代码、名称、成本价、持仓数量等（真实数据）</li>
              <li><strong>当前价格</strong>：系统尝试从东方财富 API 获取实时价格，如果获取失败则显示为 ¥0.00</li>
              <li><strong>市值和盈亏</strong>：根据当前价格自动计算（当前价格为 0 时，市值和盈亏会显示为负值）</li>
            </ul>
            <p class="mb-2"><strong>价格获取：</strong></p>
            <ul class="info-box-list">
              <li>点击"刷新价格"按钮会尝试从东方财富 API 获取最新价格</li>
              <li>如果 API 调用失败或未配置，当前价格会显示为 ¥0.00</li>
              <li>您也可以手动编辑投资记录来更新当前价格</li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-label">总成本</div>
        <div class="stat-value">¥{{ formatMoney(stats.TotalCost || 0) }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">总市值</div>
        <div class="stat-value-blue">¥{{ formatMoney(stats.TotalMarketValue || 0) }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">总盈亏</div>
        <div :class="(stats.TotalProfitLoss || 0) >= 0 ? 'stat-value-positive' : 'stat-value-negative'">
          ¥{{ formatMoney(stats.TotalProfitLoss || 0) }}
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-label">收益率</div>
        <div :class="(stats.TotalProfitRate || 0) >= 0 ? 'stat-value-positive' : 'stat-value-negative'">
          {{ formatPercent(stats.TotalProfitRate || 0) }}%
        </div>
      </div>
    </div>

    <!-- 图表分析区域 -->
    <div class="charts-grid">
      <!-- 资产类型分布饼状图 -->
      <div class="chart-container">
        <h2 class="chart-title">资产类型分布</h2>
        <div v-if="stats.ByType && stats.ByType.length > 0" class="chart-wrapper">
          <v-chart :option="typeChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>

      <!-- 盈亏状态分布饼状图 -->
      <div class="chart-container">
        <h2 class="chart-title">盈亏状态分布</h2>
        <div v-if="stats.ByProfitStatus && stats.ByProfitStatus.length > 0" class="chart-wrapper">
          <v-chart :option="profitStatusChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>

      <!-- 资产分布（按代码）饼状图 -->
      <div class="chart-container">
        <h2 class="chart-title">资产分布（Top 10）</h2>
        <div v-if="assetDistributionChartOption.series && assetDistributionChartOption.series[0].data.length > 0" class="chart-wrapper">
          <v-chart :option="assetDistributionChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>

      <!-- 收益排行柱状图 -->
      <div class="chart-container">
        <h2 class="chart-title">收益排行（Top 5）</h2>
        <div v-if="stats.TopByProfit && stats.TopByProfit.length > 0" class="chart-wrapper">
          <v-chart :option="profitRankChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="chart-empty">暂无数据</div>
      </div>
    </div>

    <!-- 统计表格 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- 按类型统计表格 -->
      <div class="stats-table-container">
        <h2 class="chart-title">按类型统计</h2>
        <div class="overflow-x-auto">
          <table class="stats-table">
            <thead>
              <tr>
                <th>类型</th>
                <th>数量</th>
                <th>总成本</th>
                <th>总市值</th>
                <th>盈亏</th>
                <th>收益率</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in stats.ByType" :key="index">
                <td>{{ item.TypeName || item.Type }}</td>
                <td>{{ item.Count }}</td>
                <td>¥{{ formatMoney(item.TotalCost) }}</td>
                <td>¥{{ formatMoney(item.TotalMarketValue) }}</td>
                <td :class="item.ProfitLoss >= 0 ? 'profit-positive' : 'profit-negative'">
                  ¥{{ formatMoney(item.ProfitLoss) }}
                </td>
                <td :class="item.ProfitRate >= 0 ? 'profit-positive' : 'profit-negative'">
                  {{ formatPercent(item.ProfitRate) }}%
                </td>
              </tr>
              <tr v-if="!stats.ByType || stats.ByType.length === 0">
                <td colspan="6" class="stats-table-empty">暂无数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Top 5 持仓 -->
      <div class="top-holdings-container">
        <h2 class="top-holdings-title">Top 5 持仓（按市值）</h2>
        <div class="top-holdings-list">
          <div
            v-for="(item, index) in stats.TopByMarketValue"
            :key="index"
            class="holding-item"
          >
            <div class="holding-info">
              <div
                class="holding-rank"
                :class="{
                  'holding-rank-1': index === 0,
                  'holding-rank-2': index === 1,
                  'holding-rank-3': index === 2,
                  'holding-rank-other': index > 2
                }"
              >
                {{ index + 1 }}
              </div>
              <div class="holding-details">
                <div class="holding-name">
                  {{ item.Name }} ({{ item.Code }})
                </div>
                <div class="holding-type">
                  {{ item.Type === 'stock' ? '股票' : '基金' }}
                </div>
              </div>
            </div>
            <div class="holding-value">
              <div class="holding-value-amount">
                ¥{{ formatMoney(item.MarketValue) }}
              </div>
              <div 
                class="holding-value-rate"
                :class="item.ProfitLoss >= 0 ? 'profit-positive' : 'profit-negative'"
              >
                {{ item.ProfitLoss >= 0 ? '+' : '' }}{{ formatPercent(item.ProfitRate) }}%
              </div>
            </div>
          </div>
          <div v-if="!stats.TopByMarketValue || stats.TopByMarketValue.length === 0" class="empty-state">
            暂无数据
          </div>
        </div>
      </div>
    </div>

    <!-- 投资列表 -->
    <div class="table-container">
      <table class="investment-table">
        <thead>
          <tr>
            <th>代码</th>
            <th>名称</th>
            <th>类型</th>
            <th>持仓</th>
            <th>成本价</th>
            <th>当前价</th>
            <th>市值</th>
            <th>盈亏</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in investments" :key="item.id">
            <td class="font-mono">{{ item.code }}</td>
            <td>{{ item.name }}</td>
            <td>
              <span :class="item.type === 'stock' ? 'badge-stock' : 'badge-fund'">
                {{ item.type === 'stock' ? '股票' : '基金' }}
              </span>
            </td>
            <td>{{ item.quantity }}</td>
            <td>¥{{ formatMoney(item.costPrice) }}</td>
            <td>¥{{ formatMoney(item.currentPrice) }}</td>
            <td>¥{{ formatMoney(item.marketValue) }}</td>
            <td>
              <div :class="item.profitLoss >= 0 ? 'profit-positive' : 'profit-negative'">
                ¥{{ formatMoney(item.profitLoss) }}
              </div>
              <div :class="item.profitRate >= 0 ? 'profit-rate-positive' : 'profit-rate-negative'">
                {{ formatPercent(item.profitRate) }}%
              </div>
            </td>
            <td>
              <div class="action-buttons">
                <button @click="editItem(item)" class="btn-edit">编辑</button>
                <button @click="addTransaction(item)" class="btn-transaction">交易</button>
                <button @click="deleteItem(item.id)" class="btn-delete">删除</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingItem" class="modal-overlay">
      <div class="modal-content">
        <div class="modal-body">
          <h2 class="modal-title">{{ editingItem ? '编辑' : '新增' }}投资</h2>
          
          <div class="modal-form">
            <div class="form-group">
              <label class="form-label">代码</label>
              <input v-model="form.code" type="text" class="form-input" placeholder="例如: 000001" />
            </div>
            <div class="form-group">
              <label class="form-label">名称</label>
              <input v-model="form.name" type="text" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">类型</label>
              <select v-model="form.type" class="form-select">
                <option value="stock">股票</option>
                <option value="fund">基金</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">持仓数量</label>
              <input v-model.number="form.quantity" type="number" step="0.01" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">成本价</label>
              <input v-model.number="form.costPrice" type="number" step="0.01" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">备注</label>
              <textarea v-model="form.notes" rows="3" class="form-textarea"></textarea>
            </div>
          </div>

          <div class="modal-actions">
            <button @click="saveItem" class="btn-save">保存</button>
            <button @click="cancelEdit" class="btn-cancel">取消</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { PieChart, BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import VChart from 'vue-echarts'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import { registerTheme } from 'echarts/core'

// 注册 ECharts 组件
use([
  CanvasRenderer,
  PieChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
])

// 使用 ECharts 主题配置
const { isDark, darkTheme, lightTheme } = useEChartsTheme()

// 注册自定义深色主题
registerTheme('dark-custom', {
  backgroundColor: 'transparent',
  textStyle: {
    color: '#ffffff'
  },
  title: {
    textStyle: {
      color: '#ffffff'
    }
  },
  legend: {
    textStyle: {
      color: '#e5e7eb'
    }
  },
  tooltip: {
    backgroundColor: 'rgba(17, 24, 39, 0.98)',
    borderColor: 'rgba(156, 163, 175, 0.5)',
    textStyle: {
      color: '#ffffff'
    }
  }
})

// 注册自定义浅色主题
registerTheme('light-custom', {
  backgroundColor: 'transparent',
  textStyle: {
    color: '#374151'
  },
  title: {
    textStyle: {
      color: '#111827'
    }
  },
  legend: {
    textStyle: {
      color: '#6b7280'
    }
  },
  tooltip: {
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
    borderColor: 'rgba(209, 213, 219, 0.8)',
    textStyle: {
      color: '#111827'
    }
  }
})

import type { Investment, InvestmentRequest } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const investments = ref<Investment[]>([])
const stats = ref<{
  TotalCost?: number
  TotalMarketValue?: number
  TotalProfitLoss?: number
  TotalProfitRate?: number
  TotalCount?: number
  ByType?: Array<{
    Type: string
    TypeName?: string
    Count: number
    TotalCost: number
    TotalMarketValue: number
    ProfitLoss: number
    ProfitRate?: number
  }>
  ByProfitStatus?: Array<{
    Status: string
    StatusName?: string
    Count: number
    TotalCost: number
    TotalMarketValue: number
    ProfitLoss: number
  }>
  TopByMarketValue?: Array<{
    Code: string
    Name: string
    Type: string
    MarketValue: number
    ProfitLoss: number
    ProfitRate: number
  }>
  TopByProfit?: Array<{
    Code: string
    Name: string
    Type: string
    ProfitLoss: number
    ProfitRate: number
  }>
  TopByLoss?: Array<{
    Code: string
    Name: string
    Type: string
    ProfitLoss: number
    ProfitRate: number
  }>
  AssetDistribution?: Array<{
    Code: string
    Name: string
    MarketValue: number
    Percentage: number
  }>
}>({})
const showCreateModal = ref(false)
const editingItem = ref<Investment | null>(null)
const form = ref({
  code: '',
  name: '',
  type: 'stock',
  quantity: 0,
  costPrice: 0,
  notes: ''
})

const fetchList = async () => {
  try {
    const res = await api.get<Investment[] | { List: Investment[] }>('/Investment')
    if (Array.isArray(res)) {
      investments.value = res
    } else if (res && 'List' in res) {
      investments.value = res.List
    }

    // 获取统计数据
    const statsRes = await api.get<any>('/Investment/stats')
    if (statsRes) {
      stats.value = statsRes
      console.log('投资统计数据:', statsRes)
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch investments:', e)
    }
  }
}

const refreshPrices = async () => {
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.post('/Investment/refresh-prices')
    success('价格刷新成功')
    fetchList()
  } catch (e: unknown) {
    handleError(e, '刷新失败')
  }
}

const editItem = (item: Investment) => {
  editingItem.value = item
  form.value = {
    code: item.code,
    name: item.name,
    type: item.type,
    quantity: item.quantity,
    costPrice: item.costPrice,
    notes: item.notes || ''
  }
}

const addTransaction = (item: Investment) => {
  // TODO: 实现交易记录功能
  const { info } = useNotification()
  info('交易功能开发中...')
}

const saveItem = async () => {
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    const payload: InvestmentRequest = {
      code: form.value.code,
      name: form.value.name,
      type: form.value.type,
      quantity: form.value.quantity,
      costPrice: form.value.costPrice,
      notes: form.value.notes || undefined
    }
    
    if (editingItem.value) {
      await api.put(`/Investment/${editingItem.value.id}`, payload)
    } else {
      await api.post('/Investment', payload)
    }

    success('保存成功')
    cancelEdit()
    fetchList()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const deleteItem = async (id: number) => {
  if (!confirm('确定要删除吗？')) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.delete(`/Investment/${id}`)
    success('删除成功')
    fetchList()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingItem.value = null
  form.value = { code: '', name: '', type: 'stock', quantity: 0, costPrice: 0, notes: '' }
}

const formatMoney = (value: number) => {
  return value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const formatPercent = (value: number) => {
  return value.toFixed(2)
}

// 生成图表颜色
const generateColors = (count: number) => {
  const colors = [
    '#3B82F6', // blue
    '#10B981', // green
    '#F59E0B', // amber
    '#EF4444', // red
    '#8B5CF6', // purple
    '#EC4899', // pink
    '#06B6D4', // cyan
    '#84CC16', // lime
    '#F97316', // orange
    '#6366F1'  // indigo
  ]
  return colors.slice(0, count)
}

// ECharts 通用配置
const getCommonPieOption = () => {
  const theme = isDark.value ? darkTheme : lightTheme
  return {
    backgroundColor: theme.backgroundColor,
    textStyle: theme.textStyle,
    tooltip: {
      trigger: 'item',
      backgroundColor: theme.tooltip.backgroundColor,
      borderColor: theme.tooltip.borderColor,
      textStyle: theme.tooltip.textStyle,
      formatter: (params: any) => {
        const { name, value, percent } = params
        return `${name}<br/>¥${value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} (${percent}%)`
      }
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      bottom: 'bottom',
      textStyle: {
        color: theme.legend.textStyle.color,
        fontSize: 12,
        fontWeight: 'normal'
      }
    }
  }
}

// 资产类型分布图表配置
const typeChartOption = computed(() => {
  if (!stats.value.ByType || stats.value.ByType.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme : lightTheme
  const data = stats.value.ByType.map((t: any) => ({
    name: t.TypeName || (t.Type === 'stock' ? '股票' : '基金'),
    value: t.TotalMarketValue || 0
  }))
  const colors = generateColors(data.length)
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color // 确保文字颜色应用
    },
    color: colors,
    series: [{
      type: 'pie',
      radius: ['40%', '70%'], // 环形图，更美观
      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: isDark.value ? '#1f2937' : '#ffffff',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color, // 使用主题文字颜色
        fontSize: 13,
        fontWeight: 'normal',
        formatter: (params: any) => {
          return `${params.name}\n${params.percent}%`
        }
      },
      labelLine: {
        show: true,
        lineStyle: {
          color: theme.textStyle.color
        }
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: isDark.value ? 'rgba(255, 255, 255, 0.5)' : 'rgba(0, 0, 0, 0.5)'
        },
        label: {
          color: theme.textStyle.color,
          fontSize: 15,
          fontWeight: 'bold'
        }
      },
      data
    }]
  }
})

// 盈亏状态分布图表配置
const profitStatusChartOption = computed(() => {
  if (!stats.value.ByProfitStatus || stats.value.ByProfitStatus.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme : lightTheme
  const data = stats.value.ByProfitStatus.map((s: any) => ({
    name: s.StatusName || (s.Status === 'profit' ? '盈利' : '亏损'),
    value: s.Count || 0
  }))
  const colors = stats.value.ByProfitStatus.map((s: any) => s.Status === 'profit' ? '#10B981' : '#EF4444')
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color
    },
    color: colors,
    series: [{
      type: 'pie',
      radius: ['40%', '70%'], // 环形图
      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: isDark.value ? '#1f2937' : '#ffffff',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color,
        fontSize: 13,
        fontWeight: 'normal',
        formatter: (params: any) => {
          return `${params.name}\n${params.value}个`
        }
      },
      labelLine: {
        show: true,
        lineStyle: {
          color: theme.textStyle.color
        }
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: isDark.value ? 'rgba(255, 255, 255, 0.5)' : 'rgba(0, 0, 0, 0.5)'
        },
        label: {
          color: theme.textStyle.color,
          fontSize: 15,
          fontWeight: 'bold'
        }
      },
      data
    }]
  }
})

// 资产分布图表配置（Top 10）
const assetDistributionChartOption = computed(() => {
  if (!stats.value.AssetDistribution || stats.value.AssetDistribution.length === 0) {
    return { series: [{ data: [] }] }
  }
  const theme = isDark.value ? darkTheme : lightTheme
  const top10 = stats.value.AssetDistribution.slice(0, 10)
  const data = top10.map((a: any) => ({
    name: `${a.Name} (${a.Code})`,
    value: a.MarketValue || 0
  }))
  const colors = generateColors(data.length)
  
  return {
    ...getCommonPieOption(),
    backgroundColor: theme.backgroundColor,
    textStyle: {
      ...theme.textStyle,
      color: theme.textStyle.color
    },
    color: colors,
    legend: {
      ...getCommonPieOption().legend,
      orient: 'vertical',
      left: 'left',
      top: 'middle',
      textStyle: {
        color: theme.legend.textStyle.color,
        fontSize: 12
      }
    },
    series: [{
      type: 'pie',
      radius: '55%',
      center: ['60%', '50%'],
      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 6,
        borderColor: isDark.value ? '#1f2937' : '#ffffff',
        borderWidth: 2
      },
      label: {
        show: true,
        color: theme.textStyle.color,
        fontSize: 11,
        formatter: (params: any) => {
          return `${params.name}\n¥${params.value.toLocaleString('zh-CN')}`
        }
      },
      labelLine: {
        show: true,
        lineStyle: {
          color: theme.textStyle.color
        }
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: isDark.value ? 'rgba(255, 255, 255, 0.5)' : 'rgba(0, 0, 0, 0.5)'
        },
        label: {
          color: theme.textStyle.color,
          fontSize: 13,
          fontWeight: 'bold'
        }
      },
      data
    }]
  }
})

// 收益排行图表配置
const profitRankChartOption = computed(() => {
  if (!stats.value.TopByProfit || stats.value.TopByProfit.length === 0) {
    return {}
  }
  const theme = isDark.value ? darkTheme : lightTheme
  const labels = stats.value.TopByProfit.map((p: any) => `${p.Name} (${p.Code})`)
  const data = stats.value.TopByProfit.map((p: any) => p.ProfitLoss || 0)
  const colors = data.map((d: number) => d >= 0 ? '#10B981' : '#EF4444')
  
  return {
    backgroundColor: theme.backgroundColor,
    textStyle: theme.textStyle,
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      },
      backgroundColor: theme.tooltip.backgroundColor,
      borderColor: theme.tooltip.borderColor,
      textStyle: theme.tooltip.textStyle,
      formatter: (params: any) => {
        const param = params[0]
        return `${param.name}<br/>盈亏: ¥${param.value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true,
      borderColor: theme.grid.borderColor
    },
    xAxis: {
      type: 'category',
      data: labels,
      axisLabel: {
        rotate: 45,
        interval: 0,
        color: theme.textStyle.color,
        fontSize: 11
      },
      axisLine: {
        lineStyle: {
          color: theme.grid.borderColor
        }
      },
      splitLine: {
        show: false
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: theme.textStyle.color,
        formatter: (value: number) => '¥' + value.toLocaleString('zh-CN')
      },
      axisLine: {
        lineStyle: {
          color: theme.grid.borderColor
        }
      },
      splitLine: {
        lineStyle: {
          color: theme.grid.borderColor,
          type: 'dashed'
        }
      }
    },
    series: [{
      type: 'bar',
      data: data.map((d: number, index: number) => ({
        value: d,
        itemStyle: {
          color: colors[index]
        }
      })),
      label: {
        show: true,
        position: 'top',
        color: theme.textStyle.color,
        fontSize: 11,
        formatter: (params: any) => {
          const value = params.value
          return value >= 0 ? '+' : '' + '¥' + value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
        }
      }
    }]
  }
})

onMounted(() => {
  fetchList()
})
</script>

