<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">错误日志</h1>
      <button @click="fetchErrorLogs" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition">
        刷新
      </button>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总错误数</div>
        <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.Total || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">未处理</div>
        <div class="text-2xl font-bold text-red-600 dark:text-red-400">{{ stats.Unhandled || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">已处理</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats.Handled || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">已忽略</div>
        <div class="text-2xl font-bold text-gray-600 dark:text-gray-400">{{ stats.Ignored || 0 }}</div>
      </div>
    </div>

    <!-- 统计图表 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- 错误类型分布 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">错误类型分布</h2>
        <div v-if="stats.ByType && stats.ByType.length > 0" class="h-64">
          <Doughnut :data="typeChartData" :options="typeChartOptions" />
        </div>
        <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
          暂无数据
        </div>
      </div>

      <!-- 最近7天错误趋势 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">最近7天错误趋势</h2>
        <div v-if="stats.RecentErrors && stats.RecentErrors.length > 0" class="h-64">
          <Line :data="trendChartData" :options="trendChartOptions" />
        </div>
        <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
          暂无数据
        </div>
      </div>
    </div>

    <!-- 筛选器和批量操作 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 mb-6">
      <div class="flex flex-col md:flex-row gap-4 items-start md:items-center justify-between">
        <div class="flex gap-4 flex-1">
          <select v-model="filters.errorType" @change="fetchErrorLogs" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
            <option value="">全部类型</option>
            <option value="JavaScript">JavaScript</option>
            <option value="Promise">Promise</option>
            <option value="Vue">Vue</option>
            <option value="API">API</option>
            <option value="Server">Server</option>
          </select>
          <select v-model="filters.status" @change="fetchErrorLogs" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
            <option :value="null">全部状态</option>
            <option :value="0">未处理</option>
            <option :value="1">已处理</option>
            <option :value="2">已忽略</option>
          </select>
        </div>
        <!-- 批量操作按钮 -->
        <div v-if="selectedIds.length > 0" class="flex gap-2 items-center">
          <span class="text-sm text-gray-600 dark:text-gray-400">已选择 {{ selectedIds.length }} 项</span>
          <button
            @click="batchUpdateStatus(1)"
            class="px-4 py-2 bg-green-600 text-white rounded text-sm hover:bg-green-700 transition"
          >
            批量标记已处理
          </button>
          <button
            @click="batchUpdateStatus(2)"
            class="px-4 py-2 bg-gray-600 text-white rounded text-sm hover:bg-gray-700 transition"
          >
            批量标记已忽略
          </button>
          <button
            @click="batchDelete"
            class="px-4 py-2 bg-red-600 text-white rounded text-sm hover:bg-red-700 transition"
          >
            批量删除
          </button>
          <button
            @click="clearSelection"
            class="px-4 py-2 bg-gray-300 dark:bg-gray-600 text-gray-700 dark:text-gray-200 rounded text-sm hover:bg-gray-400 dark:hover:bg-gray-500 transition"
          >
            取消选择
          </button>
        </div>
      </div>
    </div>

    <!-- 错误日志列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div v-if="loading" class="text-center py-12">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
      </div>
      <div v-else-if="errorLogs.length === 0" class="text-center py-12 text-gray-500 dark:text-gray-400">
        暂无错误日志
      </div>
      <div v-else class="divide-y divide-gray-200 dark:divide-gray-700">
        <!-- 全选 -->
        <div class="p-4 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
          <label class="flex items-center cursor-pointer">
            <input
              type="checkbox"
              :checked="isAllSelected"
              @change="toggleSelectAll"
              class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700"
            />
            <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">全选</span>
          </label>
        </div>
        <div
          v-for="log in errorLogs"
          :key="log.id"
          class="p-6 hover:bg-gray-50 dark:hover:bg-gray-900 transition-colors"
          :class="{ 'bg-blue-50 dark:bg-blue-900/20': selectedIds.includes(log.id) }"
        >
          <div class="flex items-start justify-between mb-3">
            <div class="flex items-start gap-3 flex-1">
              <!-- 复选框 -->
              <input
                type="checkbox"
                :checked="selectedIds.includes(log.id)"
                @change="toggleSelect(log.id)"
                class="mt-1 w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700"
              />
              <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <span
                  class="px-2 py-1 rounded text-xs font-semibold"
                  :class="{
                    'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': log.status === 0,
                    'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': log.status === 1,
                    'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300': log.status === 2
                  }"
                >
                  {{ log.status === 0 ? '未处理' : log.status === 1 ? '已处理' : '已忽略' }}
                </span>
                <span class="px-2 py-1 bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 rounded text-xs">
                  {{ log.errorType }}
                </span>
              </div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-1">{{ log.errorMessage }}</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ log.errorUrl }}
              </p>
              <p class="text-xs text-gray-400 dark:text-gray-500 mt-2">
                {{ formatDate(log.createdAt) }} | IP: {{ log.userIp || '未知' }}
              </p>
              </div>
            </div>
            <div class="flex gap-2">
              <button
                @click="viewErrorLog(log.id)"
                class="px-3 py-1 bg-blue-600 text-white rounded text-sm hover:bg-blue-700 transition"
              >
                查看详情
              </button>
              <button
                v-if="log.status === 0"
                @click="updateStatus(log.id, 1)"
                class="px-3 py-1 bg-green-600 text-white rounded text-sm hover:bg-green-700 transition"
              >
                标记已处理
              </button>
              <button
                v-if="log.status === 0"
                @click="updateStatus(log.id, 2)"
                class="px-3 py-1 bg-gray-600 text-white rounded text-sm hover:bg-gray-700 transition"
              >
                忽略
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 分页 -->
    <div v-if="total > pageSize" class="mt-6 flex justify-center">
      <div class="flex gap-2">
        <button
          @click="page = Math.max(1, page - 1); fetchErrorLogs()"
          :disabled="page === 1"
          class="px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50"
        >
          上一页
        </button>
        <span class="px-4 py-2 text-gray-700 dark:text-gray-300">
          第 {{ page }} 页，共 {{ Math.ceil(total / pageSize) }} 页
        </span>
        <button
          @click="page = Math.min(Math.ceil(total / pageSize), page + 1); fetchErrorLogs()"
          :disabled="page >= Math.ceil(total / pageSize)"
          class="px-4 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50"
        >
          下一页
        </button>
      </div>
    </div>

    <!-- 错误详情对话框 -->
    <div
      v-if="showDetail"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showDetail = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        <div class="flex justify-between items-center p-6 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-xl font-bold text-gray-900 dark:text-white">错误详情</h2>
          <button
            @click="showDetail = false"
            class="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <i class="fas fa-times text-2xl"></i>
          </button>
        </div>
        <div class="flex-1 overflow-auto p-6">
          <div v-if="detailLoading" class="text-center py-12">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
          <div v-else-if="errorDetail" class="space-y-4">
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">错误类型</h3>
              <p class="text-gray-900 dark:text-white">{{ errorDetail.errorType }}</p>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">错误消息</h3>
              <p class="text-gray-900 dark:text-white">{{ errorDetail.errorMessage }}</p>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">错误堆栈</h3>
              <pre class="bg-gray-100 dark:bg-gray-900 p-4 rounded text-sm overflow-x-auto">{{ errorDetail.errorStack || '无' }}</pre>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">错误URL</h3>
              <p class="text-gray-900 dark:text-white break-all">{{ errorDetail.errorUrl }}</p>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">用户IP</h3>
                <p class="text-gray-900 dark:text-white">{{ errorDetail.userIp || '未知' }}</p>
              </div>
              <div>
                <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">访客ID</h3>
                <p class="text-gray-900 dark:text-white">{{ errorDetail.visitorId || '未知' }}</p>
              </div>
            </div>
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">用户代理</h3>
              <p class="text-gray-900 dark:text-white text-sm break-all">{{ errorDetail.userAgent || '未知' }}</p>
            </div>
            <div v-if="errorDetail.metadata">
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">额外信息</h3>
              <pre class="bg-gray-100 dark:bg-gray-900 p-4 rounded text-sm overflow-x-auto">{{ errorDetail.metadata }}</pre>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'
import { Line, Doughnut } from 'vue-chartjs'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
)

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { success } = useNotification()
const { handleError } = useErrorHandler()

const errorLogs = ref<any[]>([])
const loading = ref(true)
const stats = ref<any>({})
const page = ref(1)
const pageSize = ref(20)
const total = ref(0)
const showDetail = ref(false)
const detailLoading = ref(false)
const errorDetail = ref<any>(null)
const selectedIds = ref<number[]>([])

const filters = ref({
  errorType: '',
  status: null as number | null
})

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 错误类型分布图表数据
const typeChartData = computed(() => {
  if (!stats.value.ByType || stats.value.ByType.length === 0) {
    return {
      labels: [],
      datasets: []
    }
  }

  const colors = [
    'rgba(239, 68, 68, 0.8)',   // red
    'rgba(59, 130, 246, 0.8)',  // blue
    'rgba(16, 185, 129, 0.8)',  // green
    'rgba(245, 158, 11, 0.8)',  // yellow
    'rgba(139, 92, 246, 0.8)',  // purple
    'rgba(236, 72, 153, 0.8)',  // pink
  ]

  return {
    labels: stats.value.ByType.map((item: any) => item.Type || item.type),
    datasets: [{
      label: '错误数量',
      data: stats.value.ByType.map((item: any) => item.Count || item.count),
      backgroundColor: colors.slice(0, stats.value.ByType.length),
      borderColor: colors.slice(0, stats.value.ByType.length).map(c => c.replace('0.8', '1')),
      borderWidth: 2
    }]
  }
})

const typeChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom' as const
    },
    tooltip: {
      callbacks: {
        label: (context: any) => {
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0)
          const value = context.parsed
          const percentage = ((value / total) * 100).toFixed(1)
          return `${context.label}: ${value} (${percentage}%)`
        }
      }
    }
  }
}

// 错误趋势图表数据
const trendChartData = computed(() => {
  if (!stats.value.RecentErrors || stats.value.RecentErrors.length === 0) {
    return {
      labels: [],
      datasets: []
    }
  }

  return {
    labels: stats.value.RecentErrors.map((item: any) => {
      const date = new Date(item.Date || item.date)
      return date.toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
    }),
    datasets: [{
      label: '错误数量',
      data: stats.value.RecentErrors.map((item: any) => item.Count || item.count),
      borderColor: 'rgb(239, 68, 68)',
      backgroundColor: 'rgba(239, 68, 68, 0.1)',
      tension: 0.4,
      fill: true
    }]
  }
})

const trendChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: true,
      position: 'top' as const
    },
    tooltip: {
      mode: 'index' as const,
      intersect: false
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        stepSize: 1
      }
    }
  }
}

const fetchErrorLogs = async () => {
  loading.value = true
  try {
    const params: any = { page: page.value, pageSize: pageSize.value }
    if (filters.value.errorType) params.errorType = filters.value.errorType
    if (filters.value.status !== null) params.status = filters.value.status

    const res = await api.get<any>('/ErrorLog', { params })
    if (res) {
      errorLogs.value = res.list || res.List || []
      total.value = res.total || res.Total || 0
    }
  } catch (e: unknown) {
    handleError(e, '获取错误日志失败')
  } finally {
    loading.value = false
  }
}

const fetchStats = async () => {
  try {
    const res = await api.get<any>('/ErrorLog/stats')
    if (res) {
      // 确保正确提取统计数据
      stats.value = {
        Total: res.Total || res.total || 0,
        Unhandled: res.Unhandled || res.unhandled || 0,
        Handled: res.Handled || res.handled || 0,
        Ignored: res.Ignored || res.ignored || 0,
        ByType: res.ByType || res.byType || [],
        RecentErrors: res.RecentErrors || res.recentErrors || []
      }
    }
  } catch (e: unknown) {
    handleError(e, '获取统计失败')
  }
}

const viewErrorLog = async (id: number) => {
  detailLoading.value = true
  showDetail.value = true
  try {
    const res = await api.get<any>(`/ErrorLog/${id}`)
    errorDetail.value = res
  } catch (e: unknown) {
    handleError(e, '获取错误详情失败')
    showDetail.value = false
  } finally {
    detailLoading.value = false
  }
}

const updateStatus = async (id: number, status: number) => {
  try {
    await api.put(`/ErrorLog/${id}/status`, { status })
    success('状态已更新')
    await fetchErrorLogs()
    await fetchStats()
    // 如果更新的项被选中，从选中列表中移除
    const index = selectedIds.value.indexOf(id)
    if (index > -1) {
      selectedIds.value.splice(index, 1)
    }
  } catch (e: unknown) {
    handleError(e, '更新状态失败')
  }
}

// 批量处理相关函数
const toggleSelect = (id: number) => {
  const index = selectedIds.value.indexOf(id)
  if (index > -1) {
    selectedIds.value.splice(index, 1)
  } else {
    selectedIds.value.push(id)
  }
}

const toggleSelectAll = () => {
  if (isAllSelected.value) {
    selectedIds.value = []
  } else {
    selectedIds.value = errorLogs.value.map(log => log.id)
  }
}

const isAllSelected = computed(() => {
  return errorLogs.value.length > 0 && selectedIds.value.length === errorLogs.value.length
})

const clearSelection = () => {
  selectedIds.value = []
}

const batchUpdateStatus = async (status: number) => {
  if (selectedIds.value.length === 0) {
    return
  }

  try {
    await api.put('/ErrorLog/batch/status', {
      ids: selectedIds.value,
      status
    })
    const statusText = status === 1 ? '已处理' : status === 2 ? '已忽略' : '未处理'
    success(`已批量标记 ${selectedIds.value.length} 条错误日志为${statusText}`)
    selectedIds.value = []
    await fetchErrorLogs()
    await fetchStats()
  } catch (e: unknown) {
    handleError(e, '批量更新状态失败')
  }
}

const batchDelete = async () => {
  if (selectedIds.value.length === 0) {
    return
  }

  if (!confirm(`确定要删除选中的 ${selectedIds.value.length} 条错误日志吗？此操作不可恢复。`)) {
    return
  }

  try {
    await api.delete('/ErrorLog/batch', {
      body: {
        ids: selectedIds.value
      }
    })
    success(`已删除 ${selectedIds.value.length} 条错误日志`)
    selectedIds.value = []
    await fetchErrorLogs()
    await fetchStats()
  } catch (e: unknown) {
    handleError(e, '批量删除失败')
  }
}

onMounted(() => {
  fetchErrorLogs()
  fetchStats()
})
</script>

