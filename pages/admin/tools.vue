<template>
  <div class="space-y-6">
    <div class="page-header">
      <h1 class="page-title">工具管理</h1>
      <button @click="openModal()" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>新增工具
      </button>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总工具数</div>
        <div class="text-2xl font-bold text-gray-800 dark:text-white">{{ stats.total }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">已发布</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats.published }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">草稿</div>
        <div class="text-2xl font-bold text-yellow-600 dark:text-yellow-400">{{ stats.draft }}</div>
      </div>
      <div class="card p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">已归档</div>
        <div class="text-2xl font-bold text-gray-600 dark:text-gray-400">{{ stats.archived }}</div>
      </div>
    </div>

    <!-- 图表区域 -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
      <!-- 状态分布饼图 -->
      <div class="card p-6">
        <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-4">状态分布</h3>
        <ClientOnly>
          <div v-if="statusChartOption" class="h-64">
            <v-chart :option="statusChartOption" autoresize class="w-full h-full" />
          </div>
          <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
            暂无数据
          </div>
        </ClientOnly>
      </div>

      <!-- 价格分布柱状图 -->
      <div class="card p-6">
        <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-4">价格分布</h3>
        <ClientOnly>
          <div v-if="priceChartOption" class="h-64">
            <v-chart :option="priceChartOption" autoresize class="w-full h-full" />
          </div>
          <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
            暂无数据
          </div>
        </ClientOnly>
      </div>
    </div>

    <!-- 工具列表 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tool in tools" :key="tool.id" class="card p-6 flex flex-col">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-gray-100 dark:bg-gray-700 flex items-center justify-center text-2xl">
              {{ tool.icon || '🛠️' }}
            </div>
            <div>
              <h3 class="font-bold text-gray-800 dark:text-white">{{ tool.name }}</h3>
              <div class="flex items-center gap-2 mt-1">
                <span class="text-xs px-2 py-1 rounded" :class="{
                  'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300': tool.status === 'published',
                  'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-300': tool.status === 'draft',
                  'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300': tool.status === 'archived'
                }">
                  {{ tool.status === 'published' ? '已发布' : tool.status === 'draft' ? '草稿' : '已归档' }}
                </span>
                <span v-if="tool.category" class="text-xs text-gray-500 dark:text-gray-400">
                  {{ tool.category.name }}
                </span>
              </div>
            </div>
          </div>
          <div class="flex gap-2">
            <button @click="openModal(tool)" class="text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
              <span class="sr-only">编辑</span>
              ✏️
            </button>
            <button @click="handleDelete(tool)" class="text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-colors">
              <span class="sr-only">删除</span>
              🗑️
            </button>
          </div>
        </div>
        <p class="text-gray-600 dark:text-gray-400 text-sm mb-4 flex-1">{{ tool.description || '暂无描述' }}</p>
        <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-500">
          <span>价格: {{ tool.isFree ? '免费' : `¥${tool.price}` }}</span>
          <span v-if="tool.demoUrl">
            <a :href="tool.demoUrl" target="_blank" class="btn-link btn-link--blue">演示</a>
          </span>
        </div>
      </div>
      
      <!-- 空状态 -->
      <div v-if="tools.length === 0" class="col-span-full text-center py-12 empty-state card border-dashed">
        暂无工具数据
      </div>
    </div>

    <!-- 编辑/新建弹窗 -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-content max-w-lg max-h-[90vh] overflow-y-auto">
        <div class="modal-header">
          <h3 class="modal-title">
            {{ isEdit ? '编辑工具' : '新增工具' }}
          </h3>
        </div>
        
        <div class="modal-body space-y-4">
          <div class="form-group">
            <label class="form-label">名称 *</label>
            <input v-model="form.name" type="text" class="form-input" placeholder="例如：JSON 格式化工具" required>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="form-label">别名 (Slug) *</label>
              <input v-model="form.slug" type="text" class="form-input" placeholder="json-formatter" required>
            </div>
            <div class="form-group">
              <label class="form-label">图标 (Emoji)</label>
              <input v-model="form.icon" type="text" class="form-input" placeholder="🔧">
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="form.description" class="form-textarea h-24" placeholder="简短描述该工具的作用..."></textarea>
          </div>
          <div class="form-group">
            <label class="form-label">演示地址</label>
            <input v-model="form.demoUrl" type="text" class="form-input" placeholder="https://...">
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="form-label">价格</label>
              <input v-model.number="form.price" type="number" step="0.01" class="form-input" placeholder="0.00">
            </div>
            <div class="form-group">
              <label class="form-label">原价</label>
              <input v-model.number="form.originalPrice" type="number" step="0.01" class="form-input" placeholder="0.00">
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div class="form-group">
              <label class="form-label">状态</label>
              <select v-model="form.status" class="form-input">
                <option value="draft">草稿</option>
                <option value="published">已发布</option>
                <option value="archived">已归档</option>
              </select>
            </div>
            <div class="form-group">
              <label class="flex items-center gap-2">
                <input v-model="form.isFree" type="checkbox" class="form-checkbox">
                <span>免费工具</span>
              </label>
              <label class="flex items-center gap-2 mt-2">
                <input v-model="form.isPremium" type="checkbox" class="form-checkbox">
                <span>高级工具</span>
              </label>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="showModal = false" class="btn-secondary">取消</button>
          <button @click="handleSave" class="btn-primary">保存</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Tool {
  id: number
  name: string
  slug: string
  icon?: string
  description?: string
  coverImage?: string
  demoUrl?: string
  price: number
  originalPrice?: number
  isFree: boolean
  isPremium: boolean
  status: string
  category?: {
    id: number
    name: string
    slug: string
  }
}

import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'
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

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 ECharts 在服务端渲染时出错
})

const api = useApi()
const showModal = ref(false)
const isEdit = ref(false)
const editingToolId = ref<number | null>(null)
const form = ref({
  name: '',
  slug: '',
  icon: '',
  description: '',
  demoUrl: '',
  price: 0,
  originalPrice: null as number | null,
  isFree: false,
  isPremium: false,
  status: 'draft'
})

// 使用 useAsyncData 避免 SSR/客户端重复请求
const { data: toolsData, pending: toolsPending, refresh: refreshTools } = useAsyncData(
  'admin-tools-list',
  async () => {
    try {
      // 使用后端 API 获取数据库中的工具数据（去掉 /api/ 前缀，让 useApi 正确添加后端 baseURL）
      const res = await api.get('/Toolbox/admin/list?pageSize=1000')
      if (res && res.tools) {
        return res.tools as Tool[]
      } else if (res && res.data && res.data.tools) {
        // 兼容 ApiResponse 格式
        return res.data.tools as Tool[]
      } else if (Array.isArray(res)) {
        // 兼容直接返回数组的情况
        return res as Tool[]
      } else {
        return [] as Tool[]
      }
    } catch (e: unknown) {
      if (process.env.NODE_ENV === 'development') {
        console.error('Failed to fetch tools:', e)
      }
      return [] as Tool[]
    }
  },
  {
    server: true,  // 在服务端也执行
    default: () => [] as Tool[]  // 默认值
  }
)

// 将 useAsyncData 返回的数据绑定到响应式变量
const tools = computed(() => toolsData.value || [])
const loading = computed(() => toolsPending.value)

// 统计数据
const stats = computed(() => {
  const total = tools.value.length
  const published = tools.value.filter(t => t.status === 'published').length
  const draft = tools.value.filter(t => t.status === 'draft').length
  const archived = tools.value.filter(t => t.status === 'archived').length
  
  return {
    total,
    published,
    draft,
    archived
  }
})

// 获取 CSS 变量的辅助函数
const getCssVar = (varName: string): string => {
  if (process.client) {
    const root = document.documentElement
    return getComputedStyle(root).getPropertyValue(varName).trim()
  }
  return ''
}

// 状态分布饼图配置
const statusChartOption = computed(() => {
  if (stats.value.total === 0) return null
  
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const bgColor = getCssVar('--color-bg-card') || getCssVar('--n-card-color')
  
  return {
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      textStyle: {
        color: textColor
      }
    },
    series: [
      {
        name: '工具状态',
        type: 'pie',
        radius: ['40%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 8,
          borderColor: bgColor,
          borderWidth: 2
        },
        label: {
          show: true,
          formatter: '{b}\n{d}%',
          color: textColor
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 14,
            fontWeight: 'bold'
          }
        },
        data: [
          { value: stats.value.published, name: '已发布', itemStyle: { color: getCssVar('--color-success') || '#10b981' } },
          { value: stats.value.draft, name: '草稿', itemStyle: { color: getCssVar('--color-warning') || '#f59e0b' } },
          { value: stats.value.archived, name: '已归档', itemStyle: { color: getCssVar('--color-text-muted') || '#6b7280' } }
        ]
      }
    ]
  }
})

// 价格分布柱状图配置
const priceChartOption = computed(() => {
  if (tools.value.length === 0) return null
  
  const textColor = getCssVar('--color-text-main') || getCssVar('--n-text-color')
  const gridColor = getCssVar('--color-border-subtle') || getCssVar('--n-border-color')
  
  // 统计价格区间
  const freeCount = tools.value.filter(t => t.isFree).length
  const paidTools = tools.value.filter(t => !t.isFree && t.price > 0)
  const priceRanges = [
    { name: '免费', count: freeCount },
    { name: '0-50元', count: paidTools.filter(t => t.price <= 50).length },
    { name: '50-100元', count: paidTools.filter(t => t.price > 50 && t.price <= 100).length },
    { name: '100-200元', count: paidTools.filter(t => t.price > 100 && t.price <= 200).length },
    { name: '200元以上', count: paidTools.filter(t => t.price > 200).length }
  ].filter(r => r.count > 0)
  
  return {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
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
      data: priceRanges.map(r => r.name),
      axisLabel: {
        color: textColor
      },
      axisLine: {
        lineStyle: {
          color: gridColor
        }
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: textColor
      },
      axisLine: {
        lineStyle: {
          color: gridColor
        }
      },
      splitLine: {
        lineStyle: {
          color: gridColor
        }
      }
    },
    series: [
      {
        name: '工具数量',
        type: 'bar',
        data: priceRanges.map(r => r.count),
        itemStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              { offset: 0, color: getCssVar('--color-primary') || '#3b82f6' },
              { offset: 1, color: getCssVar('--color-secondary') || getCssVar('--color-primary-hover') || '#8b5cf6' }
            ]
          },
          borderRadius: [4, 4, 0, 0]
        }
      }
    ]
  }
})

// 手动刷新函数（用于保存/删除后刷新）
const fetchTools = async () => {
  await refreshTools()
}

const openModal = (item?: Tool) => {
  if (item) {
    isEdit.value = true
    editingToolId.value = item.id
    form.value = {
      name: item.name,
      slug: item.slug,
      icon: item.icon || '',
      description: item.description || '',
      demoUrl: item.demoUrl || '',
      price: item.price,
      originalPrice: item.originalPrice || null,
      isFree: item.isFree,
      isPremium: item.isPremium,
      status: item.status
    }
  } else {
    isEdit.value = false
    editingToolId.value = null
    form.value = {
      name: '',
      slug: '',
      icon: '',
      description: '',
      demoUrl: '',
      price: 0,
      originalPrice: null,
      isFree: false,
      isPremium: false,
      status: 'draft'
    }
  }
  showModal.value = true
}

const handleSave = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!form.value.name) {
    warning('请输入工具名称')
    return
  }
  
  try {
    // 使用后端 API（去掉 /api/ 前缀，让 useApi 正确添加后端 baseURL）
    if (isEdit.value && editingToolId.value) {
      await api.put(`/Toolbox/${editingToolId.value}`, form.value)
    } else {
      await api.post('/Toolbox', form.value)
    }
    success('保存成功')
    showModal.value = false
    editingToolId.value = null
    fetchTools()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const handleDelete = async (item: Tool) => {
  if (!confirm(`确定要删除工具 "${item.name}" 吗？`)) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // 使用后端 API（去掉 /api/ 前缀，让 useApi 正确添加后端 baseURL）
    await api.del(`/Toolbox/${item.id}`)
    success('删除成功')
    fetchTools()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

// 使用 useAsyncData 后，不需要在 onMounted 中调用，数据会自动加载
</script>
