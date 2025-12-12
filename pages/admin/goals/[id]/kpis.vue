<template>
  <div>
    <div class="flex items-center gap-4 mb-6">
      <NuxtLink to="/admin/goals" class="kpi-page-link">
        ← 返回目标列表
      </NuxtLink>
      <h1 class="text-2xl font-bold kpi-page-title">月度 KPI 管理</h1>
    </div>

    <!-- 目标信息 -->
    <div v-if="goal" class="kpi-goal-card rounded-lg shadow-sm p-6 mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h2 class="text-xl font-bold kpi-goal-title mb-2">{{ goal.title }}</h2>
          <div class="flex items-center gap-4 text-sm kpi-goal-meta">
            <span>{{ goal.year }}年</span>
            <span v-if="goal.targetValue">
              目标: {{ goal.targetValue }}{{ goal.unit || '' }}
            </span>
            <span>
              当前: {{ goal.currentValue }}{{ goal.unit || '' }}
            </span>
            <span>
              进度: <span class="font-semibold kpi-progress-text">{{ goal.progress }}%</span>
            </span>
          </div>
        </div>
        <n-button 
          v-if="!kpis || kpis.length === 0"
          type="success"
          @click="showBatchCreateModal = true"
        >
          批量创建月度 KPI
        </n-button>
      </div>
    </div>

    <!-- KPI 列表 -->
    <div class="kpi-list-container rounded-lg shadow-sm overflow-hidden">
      <div v-if="loading" class="p-8 text-center kpi-list-loading">加载中...</div>
      <div v-else-if="!kpis || kpis.length === 0" class="p-8 text-center kpi-list-empty">
        <p class="mb-4">暂无月度 KPI</p>
        <n-button 
          type="primary"
          @click="showBatchCreateModal = true"
        >
          批量创建月度 KPI
        </n-button>
      </div>
      <div v-else>
        <div v-for="kpi in kpis" :key="kpi.id" class="kpi-list-item p-4">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h3 class="text-lg font-semibold kpi-item-title">{{ kpi.title }}</h3>
                <span class="px-2 py-1 kpi-month-badge rounded text-xs font-medium">
                  {{ kpi.year }}年{{ kpi.month }}月
                </span>
                <span :class="getStatusClass(kpi.status)">
                  {{ getStatusText(kpi.status) }}
                </span>
              </div>
              
              <!-- KPI 数值 -->
              <div class="flex items-center gap-4 text-sm kpi-item-meta mb-2">
                <span v-if="kpi.targetValue">
                  目标: <span class="font-semibold kpi-item-value">{{ kpi.targetValue }}{{ kpi.unit || '' }}</span>
                </span>
                <span>
                  当前: <span class="font-semibold kpi-item-value">{{ kpi.currentValue }}{{ kpi.unit || '' }}</span>
                </span>
                <span>
                  进度: <span class="font-semibold kpi-item-value">{{ kpi.progress }}%</span>
                </span>
              </div>

              <!-- 进度条 -->
              <div class="w-full kpi-progress-bg rounded-full h-2">
                <div 
                  :class="getProgressColor(kpi.progress)"
                  :style="{ width: kpi.progress + '%' }"
                ></div>
              </div>

              <p v-if="kpi.notes" class="text-xs kpi-item-meta mt-2">{{ kpi.notes }}</p>
            </div>
            <div class="flex gap-2 ml-4">
              <button @click="editKpi(kpi)" class="kpi-action-btn kpi-action-btn-primary">编辑</button>
              <button @click="deleteKpi(kpi.id)" class="kpi-action-btn kpi-action-btn-error">删除</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑 KPI 模态框 -->
    <div v-if="showCreateModal || editingKpi" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="closeModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h2 class="text-xl font-bold text-gray-800 dark:text-white mb-4">
            {{ editingKpi ? '编辑 KPI' : '新建月度 KPI' }}
          </h2>
          <form @submit.prevent="saveKpi" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">年份 *</label>
                <input v-model.number="kpiForm.year" type="number" required min="2020" max="2100" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">月份 *</label>
                <select v-model.number="kpiForm.month" required class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                  <option v-for="m in 12" :key="m" :value="m">{{ m }}月</option>
                </select>
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">KPI 标题 *</label>
              <input v-model="kpiForm.title" type="text" required class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div class="grid grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">目标数值</label>
                <input v-model.number="kpiForm.targetValue" type="number" step="0.01" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">当前数值</label>
                <input v-model.number="kpiForm.currentValue" type="number" step="0.01" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">单位</label>
                <input v-model="kpiForm.unit" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">状态</label>
              <select v-model="kpiForm.status" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                <option value="pending">待开始</option>
                <option value="in_progress">进行中</option>
                <option value="completed">已完成</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">备注</label>
              <textarea v-model="kpiForm.notes" rows="3" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"></textarea>
            </div>
            <div class="flex justify-end gap-3 pt-4">
              <button 
                type="button" 
                @click="closeModal" 
                class="px-4 py-2 bg-gray-200 dark:bg-gray-700 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors font-bold border-2 border-gray-400 dark:border-gray-500 shadow-sm text-gray-900 dark:text-gray-100"
              >
                <span class="dark:text-gray-100" style="color: inherit;">取消</span>
              </button>
              <button type="submit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors font-medium shadow-md">保存</button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- 批量创建 KPI 模态框 -->
    <div v-if="showBatchCreateModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="showBatchCreateModal = false">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h2 class="text-xl font-bold text-gray-800 dark:text-white mb-4">批量创建月度 KPI</h2>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
            将为 {{ goal?.year }}年 创建 12 个月的 KPI，目标值将自动平均分配到每个月。
          </p>
          <div class="flex justify-end gap-3">
            <button 
              @click="showBatchCreateModal = false" 
              class="px-4 py-2 bg-gray-200 dark:bg-gray-700 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors font-bold border-2 border-gray-400 dark:border-gray-500 shadow-sm text-gray-900 dark:text-gray-100"
            >
              <span class="dark:text-gray-100" style="color: inherit;">取消</span>
            </button>
            <button @click="batchCreateKpis" class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition-colors">确认创建</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { NButton } from 'naive-ui'

interface Goal {
  id: number
  year: number
  title: string
  targetValue?: number
  currentValue: number
  unit?: string
  progress: number
}

interface MonthlyKpi {
  id: number
  goalId: number
  year: number
  month: number
  title: string
  targetValue?: number
  currentValue: number
  unit?: string
  status: string
  progress: number
  notes?: string
}

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()
const api = useApi()
const toast = useNotification()

const goalId = computed(() => parseInt(route.params.id as string))
const goal = ref<Goal | null>(null)
const kpis = ref<MonthlyKpi[]>([])
const loading = ref(false)
const showCreateModal = ref(false)
const showBatchCreateModal = ref(false)
const editingKpi = ref<MonthlyKpi | null>(null)

const kpiForm = ref({
  goalId: 0,
  year: new Date().getFullYear(),
  month: new Date().getMonth() + 1,
  title: '',
  targetValue: null as number | null,
  currentValue: 0,
  unit: '',
  status: 'pending',
  notes: ''
})

const fetchGoal = async () => {
  try {
    const res = await api.get(`/Goals/${goalId.value}`)
    if (res?.data) {
      goal.value = res.data
      kpiForm.value.goalId = goal.value.id
      kpiForm.value.year = goal.value.year
    }
  } catch (error) {
    console.error('获取目标失败:', error)
    toast.error('获取目标失败')
  }
}

const fetchKpis = async () => {
  loading.value = true
  try {
    const res = await api.get('/MonthlyKpis', {
      params: {
        goalId: goalId.value
      }
    })
    if (res?.data?.Items) {
      kpis.value = res.data.Items.sort((a: MonthlyKpi, b: MonthlyKpi) => a.month - b.month)
    }
  } catch (error) {
    console.error('获取 KPI 列表失败:', error)
    toast.error('获取 KPI 列表失败')
  } finally {
    loading.value = false
  }
}

const batchCreateKpis = async () => {
  try {
    const res = await api.post('/MonthlyKpis/batch', {
      goalId: goalId.value,
      year: goal.value?.year || new Date().getFullYear()
    })
    toast.success('批量创建 KPI 成功')
    showBatchCreateModal.value = false
    fetchKpis()
    fetchGoal() // 刷新目标信息
  } catch (error) {
    console.error('批量创建 KPI 失败:', error)
    toast.error('批量创建 KPI 失败')
  }
}

const saveKpi = async () => {
  try {
    const data = {
      ...kpiForm.value,
      goalId: goalId.value
    }

    if (editingKpi.value) {
      await api.put(`/MonthlyKpis/${editingKpi.value.id}`, data)
      toast.success('KPI 更新成功')
    } else {
      await api.post('/MonthlyKpis', data)
      toast.success('KPI 创建成功')
    }

    closeModal()
    fetchKpis()
    fetchGoal() // 刷新目标信息（因为会自动更新目标值）
  } catch (error) {
    console.error('保存 KPI 失败:', error)
    toast.error('保存 KPI 失败')
  }
}

const editKpi = (kpi: MonthlyKpi) => {
  editingKpi.value = kpi
  kpiForm.value = {
    goalId: kpi.goalId,
    year: kpi.year,
    month: kpi.month,
    title: kpi.title,
    targetValue: kpi.targetValue || null,
    currentValue: kpi.currentValue,
    unit: kpi.unit || '',
    status: kpi.status,
    notes: kpi.notes || ''
  }
}

const deleteKpi = async (id: number) => {
  if (!confirm('确定要删除这个 KPI 吗？')) return

  try {
    await api.delete(`/MonthlyKpis/${id}`)
    toast.success('KPI 删除成功')
    fetchKpis()
    fetchGoal() // 刷新目标信息
  } catch (error) {
    console.error('删除 KPI 失败:', error)
    toast.error('删除 KPI 失败')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingKpi.value = null
  kpiForm.value = {
    goalId: goalId.value,
    year: goal.value?.year || new Date().getFullYear(),
    month: new Date().getMonth() + 1,
    title: '',
    targetValue: null,
    currentValue: 0,
    unit: goal.value?.unit || '',
    status: 'pending',
    notes: ''
  }
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'kpi-status-badge kpi-status-pending',
    in_progress: 'kpi-status-badge kpi-status-in-progress',
    completed: 'kpi-status-badge kpi-status-completed'
  }
  return classes[status] || classes.pending
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    pending: '待开始',
    in_progress: '进行中',
    completed: '已完成'
  }
  return texts[status] || status
}

const getProgressColor = (progress: number) => {
  if (progress === 100) return 'kpi-progress-bar kpi-progress-complete'
  if (progress >= 50) return 'kpi-progress-bar kpi-progress-good'
  if (progress >= 25) return 'kpi-progress-bar kpi-progress-fair'
  return 'kpi-progress-bar kpi-progress-poor'
}

onMounted(() => {
  fetchGoal()
  fetchKpis()
})
</script>

<style scoped>
/* KPI 页面样式 - 使用 CSS 变量 */
.kpi-page-link {
  color: var(--color-text-sub, #4b5563);
  transition: color 0.2s ease;
}

.kpi-page-link:hover {
  color: var(--color-text-main, #111827);
}

.kpi-page-title {
  color: var(--color-text-main);
}

.kpi-goal-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  box-shadow: var(--shadow-sm);
}

.kpi-goal-title {
  color: var(--color-text-main);
}

.kpi-goal-meta {
  color: var(--color-text-sub);
}

.kpi-progress-text {
  color: var(--color-primary);
}

.kpi-list-container {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  box-shadow: var(--shadow-sm);
}

.kpi-list-loading,
.kpi-list-empty {
  color: var(--color-text-muted);
}

.kpi-list-item {
  transition: background-color 0.2s ease;
}

.kpi-list-item:hover {
  background-color: var(--color-bg-elevated);
}

.kpi-item-title {
  color: var(--color-text-main);
}

.kpi-month-badge {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

.kpi-item-meta {
  color: var(--color-text-muted);
}

.kpi-item-value {
  color: var(--color-text-main);
}

.kpi-progress-bg {
  background: var(--color-bg-elevated);
}

.kpi-progress-bar {
  height: 0.5rem;
  border-radius: 9999px;
  transition: all 0.3s ease;
}

.kpi-progress-complete {
  background: var(--color-success);
}

.kpi-progress-good {
  background: var(--color-primary);
}

.kpi-progress-fair {
  background: var(--color-warning);
}

.kpi-progress-poor {
  background: var(--color-text-muted);
}

.kpi-status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.kpi-status-pending {
  background: var(--color-warning-soft);
  color: var(--color-warning);
}

.kpi-status-in-progress {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

.kpi-status-completed {
  background: var(--color-success-soft);
  color: var(--color-success);
}

.kpi-action-btn {
  background: none;
  border: none;
  cursor: pointer;
  transition: color 0.2s ease;
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
}

.kpi-action-btn-primary {
  color: var(--color-primary, #3b82f6);
}

.kpi-action-btn-primary:hover {
  color: var(--color-primary-hover, #2563eb);
}

.kpi-action-btn-error {
  color: var(--color-error, #ef4444);
}

.kpi-action-btn-error:hover {
  color: var(--color-error-hover, #dc2626);
}

.kpi-btn-primary {
  background: var(--color-primary, #3b82f6);
  color: var(--color-text-main, #ffffff);
  transition: background-color 0.2s ease;
}

.kpi-btn-primary:hover {
  background: var(--color-primary-hover, #2563eb);
}

.kpi-btn-success {
  background: var(--color-success, #22c55e);
  color: var(--color-text-main, #ffffff);
  transition: background-color 0.2s ease;
}

.kpi-btn-success:hover {
  background: var(--color-success-hover, #16a34a);
}

/* 深色主题适配 */
html[data-theme="dark"] .kpi-page-link,
html.dark .kpi-page-link {
  color: var(--color-text-muted, #9ca3af);
}

html[data-theme="dark"] .kpi-page-link:hover,
html.dark .kpi-page-link:hover {
  color: var(--color-text-main, #f9fafb);
}

html[data-theme="dark"] .kpi-goal-card,
html.dark .kpi-goal-card {
  background: var(--color-bg-card);
  border-color: var(--color-border-subtle);
}

html[data-theme="dark"] .kpi-list-container,
html.dark .kpi-list-container {
  background: var(--color-bg-card);
  border-color: var(--color-border-subtle);
}

html[data-theme="dark"] .kpi-list-item:hover,
html.dark .kpi-list-item:hover {
  background-color: var(--color-bg-elevated);
}

html[data-theme="dark"] .kpi-month-badge,
html.dark .kpi-month-badge {
  background: var(--color-primary-soft);
  color: var(--color-primary);
}

html[data-theme="dark"] .kpi-progress-bg,
html.dark .kpi-progress-bg {
  background: var(--color-bg-elevated);
}
</style>

