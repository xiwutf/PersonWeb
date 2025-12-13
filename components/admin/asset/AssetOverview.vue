<template>
  <div class="asset-overview">
    <!-- 总资产统计卡片 -->
    <div class="asset-management-stats-grid">
      <div class="asset-management-stat-card asset-management-stat-card-large">
        <div class="asset-management-stat-label">总资产净值</div>
        <div class="asset-management-stat-value-primary">¥{{ formatMoney(overviewData?.TotalNetWorth || 0) }}</div>
        <div class="asset-management-stat-breakdown">
          <span>资产：¥{{ formatMoney(overviewData?.TotalAssets || 0) }}</span>
          <span>投资：¥{{ formatMoney(overviewData?.TotalInvestments || 0) }}</span>
        </div>
      </div>
      <div class="asset-management-stat-card">
        <div class="asset-management-stat-label">资产总额</div>
        <div class="asset-management-stat-value">¥{{ formatMoney(overviewData?.TotalAssets || 0) }}</div>
      </div>
      <div class="asset-management-stat-card">
        <div class="asset-management-stat-label">投资总额</div>
        <div class="asset-management-stat-value-blue">¥{{ formatMoney(overviewData?.TotalInvestments || 0) }}</div>
      </div>
      <div class="asset-management-stat-card">
        <div class="asset-management-stat-label">投资盈亏</div>
        <div :class="(overviewData?.InvestmentStats?.TotalProfitLoss || 0) >= 0 ? 'asset-management-stat-value-positive' : 'asset-management-stat-value-negative'">
          ¥{{ formatMoney(overviewData?.InvestmentStats?.TotalProfitLoss || 0) }}
        </div>
      </div>
    </div>

    <!-- 资产分布图表 -->
    <div class="asset-management-charts-grid">
      <div class="asset-management-card">
        <h3 class="asset-management-card-title">资产类型分布</h3>
        <div v-if="overviewData?.AssetsByType && overviewData.AssetsByType.length > 0" class="asset-management-chart-wrapper">
          <v-chart :option="assetsTypeChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="asset-management-chart-empty">暂无数据</div>
      </div>

      <div class="asset-management-card">
        <h3 class="asset-management-card-title">资产与投资占比</h3>
        <div v-if="overviewData?.AssetDistribution" class="asset-management-chart-wrapper">
          <v-chart :option="assetDistributionChartOption" :theme="isDark ? 'dark-custom' : 'light-custom'" autoresize />
        </div>
        <div v-else class="asset-management-chart-empty">暂无数据</div>
      </div>
    </div>

    <!-- 资产列表 -->
    <div class="asset-management-card">
      <div class="asset-management-card-header">
        <h3 class="asset-management-card-title">资产列表</h3>
        <button @click="handleAddAsset" class="asset-management-btn-primary">+ 新增资产</button>
      </div>
      <div v-if="loading" class="asset-management-loading">加载中...</div>
      <div v-else-if="!assets || assets.length === 0" class="asset-management-empty-state">
        <p>暂无资产记录</p>
        <button @click="handleAddAsset" class="asset-management-btn-primary">创建第一个资产</button>
      </div>
      <div v-else class="asset-management-assets-grid">
        <div v-for="asset in assets" :key="asset.id" class="asset-management-asset-card">
          <div class="asset-management-asset-header">
            <div class="asset-management-asset-title">
              <h4>{{ asset.name }}</h4>
              <span class="asset-management-asset-type">{{ getAssetTypeName(asset.assetType) }}</span>
            </div>
            <div class="asset-management-asset-amount">¥{{ formatMoney(asset.amount) }}</div>
          </div>
          <div class="asset-management-asset-details">
            <div v-if="asset.institution" class="asset-management-detail-item">
              <span class="asset-management-detail-label">机构：</span>
              <span class="asset-management-detail-value">{{ asset.institution }}</span>
            </div>
            <div v-if="asset.interestRate > 0" class="asset-management-detail-item">
              <span class="asset-management-detail-label">利率：</span>
              <span class="asset-management-detail-value">{{ asset.interestRate }}%</span>
            </div>
            <div v-if="asset.maturityDate" class="asset-management-detail-item">
              <span class="asset-management-detail-label">到期日：</span>
              <span class="asset-management-detail-value">{{ formatDate(asset.maturityDate) }}</span>
            </div>
          </div>
          <div class="asset-management-asset-actions">
            <button @click="handleEditAsset(asset)" class="asset-management-btn-secondary">编辑</button>
            <button @click="handleDeleteAsset(asset.id)" class="asset-management-btn-danger">删除</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 新增/编辑资产模态框 -->
    <div v-if="showModal" class="asset-management-modal-overlay" @click="closeModal">
      <div class="asset-management-modal-content" @click.stop>
        <div class="asset-management-modal-header">
          <h2>{{ editingAsset ? '编辑资产' : '新增资产' }}</h2>
          <button @click="closeModal" class="asset-management-modal-close">×</button>
        </div>
        <div class="asset-management-modal-body">
          <form @submit.prevent="handleSubmit">
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">资产名称 <span class="required">*</span></label>
              <input
                v-model="formData.name"
                type="text"
                class="asset-management-form-input"
                placeholder="例如：招商银行储蓄卡"
                required
                maxlength="50"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">资产类型 <span class="required">*</span></label>
              <select v-model="formData.assetType" class="asset-management-form-select" required>
                <option value="bank_card">银行卡</option>
                <option value="deposit">存单</option>
                <option value="cash">现金</option>
                <option value="other">其他</option>
              </select>
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">机构名称</label>
              <input
                v-model="formData.institution"
                type="text"
                class="asset-management-form-input"
                placeholder="例如：招商银行"
                maxlength="100"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">资产金额 <span class="required">*</span></label>
              <input
                v-model.number="formData.amount"
                type="number"
                class="asset-management-form-input"
                step="0.01"
                min="0"
                placeholder="请输入资产金额"
                required
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">货币类型</label>
              <select v-model="formData.currency" class="asset-management-form-select">
                <option value="CNY">人民币 (CNY)</option>
                <option value="USD">美元 (USD)</option>
                <option value="EUR">欧元 (EUR)</option>
                <option value="HKD">港币 (HKD)</option>
              </select>
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">利率（年化%）</label>
              <input
                v-model.number="formData.interestRate"
                type="number"
                class="asset-management-form-input"
                step="0.01"
                min="0"
                max="100"
                placeholder="例如：3.5"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">到期日期</label>
              <input
                v-model="formData.maturityDate"
                type="date"
                class="asset-management-form-input"
              />
            </div>
            <div class="asset-management-form-group">
              <label class="asset-management-form-label">备注</label>
              <textarea
                v-model="formData.notes"
                class="asset-management-form-textarea"
                rows="3"
                placeholder="可选备注信息"
              />
            </div>
            <div class="asset-management-form-actions">
              <button type="button" @click="closeModal" class="asset-management-btn-secondary">取消</button>
              <button type="submit" class="asset-management-btn-primary" :disabled="submitting">
                {{ submitting ? '保存中...' : '保存' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useApi } from '~/composables/useApi'
import { useNotification } from '~/composables/useToast'
import { useEChartsTheme } from '~/composables/useEChartsTheme'
import { use } from 'echarts/core'
import { CanvasRenderer } from 'echarts/renderers'
import { PieChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent
} from 'echarts/components'
import VChart from 'vue-echarts'

// 注册 ECharts 组件
use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent
])

const { isDark, darkTheme, lightTheme } = useEChartsTheme()

interface Asset {
  id: number
  name: string
  assetType: string
  institution: string | null
  amount: number
  currency: string
  interestRate: number
  maturityDate: string | null
  notes: string | null
  isActive: boolean
  createdAt: string
  updatedAt: string
}

const props = defineProps<{
  overviewData: any
  loading: boolean
}>()

const emit = defineEmits<{
  refresh: []
}>()

const api = useApi()
const { success, error, warning } = useNotification()

const loading = ref(false)
const assets = ref<Asset[]>([])
const showModal = ref(false)
const editingAsset = ref<Asset | null>(null)
const submitting = ref(false)

const formData = ref({
  name: '',
  assetType: 'bank_card',
  institution: '',
  amount: 0,
  currency: 'CNY',
  interestRate: 0,
  maturityDate: '',
  notes: ''
})

// 资产类型分布图表
const assetsTypeChartOption = computed(() => {
  if (!props.overviewData?.AssetsByType || props.overviewData.AssetsByType.length === 0) {
    return {}
  }

  const data = props.overviewData.AssetsByType.map((item: any) => ({
    name: item.TypeName,
    value: item.TotalAmount
  }))

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      formatter: (params: any) => {
        return `${params.name}<br/>¥${formatMoney(params.value)} (${params.percent}%)`
      }
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      textStyle: {
        color: 'var(--color-text-sec)'
      }
    },
    series: [{
      type: 'pie',
      radius: ['40%', '70%'],
      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: 'var(--color-bg-card)',
        borderWidth: 2
      },
      label: {
        show: true,
        formatter: (params: any) => `${params.name}\n${params.percent}%`
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.2)'
        }
      },
      data
    }]
  }
})

// 资产与投资占比图表
const assetDistributionChartOption = computed(() => {
  if (!props.overviewData?.AssetDistribution) {
    return {}
  }

  const dist = props.overviewData.AssetDistribution
  const data = [
    { name: '资产', value: dist.Assets },
    { name: '投资', value: dist.Investments }
  ]

  return {
    backgroundColor: 'transparent',
    tooltip: {
      trigger: 'item',
      formatter: (params: any) => {
        return `${params.name}<br/>¥${formatMoney(params.value)} (${params.percent}%)`
      }
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      textStyle: {
        color: 'var(--color-text-sec)'
      }
    },
    series: [{
      type: 'pie',
      radius: ['40%', '70%'],
      avoidLabelOverlap: true,
      itemStyle: {
        borderRadius: 8,
        borderColor: 'var(--color-bg-card)',
        borderWidth: 2
      },
      label: {
        show: true,
        formatter: (params: any) => `${params.name}\n${params.percent}%`
      },
      emphasis: {
        itemStyle: {
          shadowBlur: 15,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.2)'
        }
      },
      data
    }]
  }
})

// 加载资产列表
const loadAssets = async () => {
  try {
    loading.value = true
    const data = await api.get<Asset[]>('/Asset')
    assets.value = Array.isArray(data) ? data : []
  } catch (err: any) {
    console.error('加载资产列表失败:', err)
    error(err.message || '加载失败')
  } finally {
    loading.value = false
  }
}

// 格式化金额
const formatMoney = (value: number) => {
  return value.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// 格式化日期
const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('zh-CN')
}

// 获取资产类型名称
const getAssetTypeName = (type: string) => {
  const map: Record<string, string> = {
    bank_card: '银行卡',
    deposit: '存单',
    cash: '现金',
    other: '其他'
  }
  return map[type] || type
}

// 处理新增资产
const handleAddAsset = () => {
  editingAsset.value = null
  formData.value = {
    name: '',
    assetType: 'bank_card',
    institution: '',
    amount: 0,
    currency: 'CNY',
    interestRate: 0,
    maturityDate: '',
    notes: ''
  }
  showModal.value = true
}

// 处理编辑资产
const handleEditAsset = (asset: Asset) => {
  editingAsset.value = asset
  formData.value = {
    name: asset.name,
    assetType: asset.assetType,
    institution: asset.institution || '',
    amount: asset.amount,
    currency: asset.currency,
    interestRate: asset.interestRate,
    maturityDate: asset.maturityDate ? asset.maturityDate.split('T')[0] : '',
    notes: asset.notes || ''
  }
  showModal.value = true
}

// 处理删除资产
const handleDeleteAsset = async (id: number) => {
  if (!confirm('确定要删除这个资产记录吗？')) {
    return
  }

  try {
    await api.delete(`/Asset/${id}`)
    success('删除成功')
    loadAssets()
    emit('refresh')
  } catch (err: any) {
    console.error('删除失败:', err)
    error(err.message || '删除失败')
  }
}

// 提交表单
const handleSubmit = async () => {
  try {
    submitting.value = true

    const payload = {
      name: formData.value.name,
      assetType: formData.value.assetType,
      institution: formData.value.institution || undefined,
      amount: formData.value.amount,
      currency: formData.value.currency,
      interestRate: formData.value.interestRate || undefined,
      maturityDate: formData.value.maturityDate || undefined,
      notes: formData.value.notes || undefined
    }

    if (editingAsset.value) {
      await api.put(`/Asset/${editingAsset.value.id}`, payload)
      success('更新成功')
    } else {
      await api.post('/Asset', payload)
      success('创建成功')
    }

    closeModal()
    loadAssets()
    emit('refresh')
  } catch (err: any) {
    console.error('保存失败:', err)
    error(err.message || '保存失败')
  } finally {
    submitting.value = false
  }
}

// 关闭模态框
const closeModal = () => {
  showModal.value = false
  editingAsset.value = null
}

// 监听 overviewData 变化，自动刷新资产列表
watch(() => props.overviewData, () => {
  if (props.overviewData) {
    loadAssets()
  }
}, { immediate: true })

onMounted(() => {
  loadAssets()
})
</script>

<style scoped>
/* 使用统一样式类，样式定义在 assets/css/admin-asset-management.css */
/* 组件特有样式使用 CSS 变量 */

.asset-overview {
  padding: 0;
}

/* 资产卡片特有样式 */
.asset-management-asset-card {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--spacing-md);
  background: var(--color-bg-body);
}

.asset-management-asset-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-md);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
}

.asset-management-asset-title h4 {
  margin: 0 0 var(--spacing-xs) 0;
  font-size: 16px;
  font-weight: 600;
  color: var(--color-text-main);
}

.asset-management-asset-type {
  font-size: 12px;
  color: var(--color-text-sec);
  background: var(--color-bg-body);
  padding: 2px var(--spacing-sm);
  border-radius: var(--radius-sm);
}

.asset-management-asset-amount {
  font-size: 20px;
  font-weight: 600;
  color: var(--color-primary);
}

.asset-management-asset-details {
  margin-bottom: var(--spacing-md);
}

.asset-management-detail-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 6px;
  font-size: 13px;
}

.asset-management-detail-label {
  color: var(--color-text-sec);
}

.asset-management-detail-value {
  font-weight: 500;
  color: var(--color-text-main);
}

.asset-management-asset-actions {
  display: flex;
  gap: var(--spacing-sm);
}

/* 图表容器 */
.asset-management-chart-wrapper {
  height: 300px;
}

.asset-management-chart-empty {
  height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-text-sec);
}
</style>
