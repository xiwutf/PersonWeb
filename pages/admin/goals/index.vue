<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">年度目标管理</h1>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
        + 新建目标
      </button>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">总目标</div>
        <div class="text-2xl font-bold text-gray-800 dark:text-white">{{ stats?.Total || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">进行中</div>
        <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">{{ stats?.Active || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">已完成</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats?.Completed || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">平均进度</div>
        <div class="text-2xl font-bold text-purple-600 dark:text-purple-400">{{ stats?.AverageProgress || 0 }}%</div>
      </div>
    </div>

    <!-- 筛选栏 -->
    <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6 flex gap-4 flex-wrap">
      <select v-model.number="filterYear" @change="fetchGoals" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option :value="null">全部年份</option>
        <option v-for="y in years" :key="y" :value="y">{{ y }}年</option>
      </select>
      <select v-model="filterStatus" @change="fetchGoals" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option value="">全部状态</option>
        <option value="active">进行中</option>
        <option value="completed">已完成</option>
        <option value="archived">已归档</option>
      </select>
      <button @click="fetchGoals" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">刷新</button>
    </div>

    <!-- 目标列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div v-if="loading" class="p-8 text-center text-gray-500 dark:text-gray-400">加载中...</div>
      <div v-else-if="goals.length === 0" class="p-8 text-center text-gray-500 dark:text-gray-400">暂无目标</div>
      <div v-else class="divide-y divide-gray-200 dark:divide-gray-700">
        <div v-for="goal in goals" :key="goal.id" class="p-6 hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h3 class="text-lg font-semibold text-gray-800 dark:text-white">{{ goal.title }}</h3>
                <span class="px-2 py-1 bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300 rounded text-xs font-medium">
                  {{ goal.year }}年
                </span>
                <span :class="getStatusClass(goal.status)" class="px-2 py-1 rounded text-xs font-medium">
                  {{ getStatusText(goal.status) }}
                </span>
                <span v-if="goal.category" class="px-2 py-1 bg-gray-100 dark:bg-gray-700 rounded text-xs">
                  {{ goal.category }}
                </span>
              </div>
              <p v-if="goal.description" class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ goal.description }}</p>
              
              <!-- 目标数值 -->
              <div class="flex items-center gap-4 text-sm text-gray-500 dark:text-gray-400 mb-3">
                <span v-if="goal.targetValue">
                  目标: <span class="font-semibold text-gray-800 dark:text-white">{{ goal.targetValue }}{{ goal.unit || '' }}</span>
                </span>
                <span>
                  当前: <span class="font-semibold text-gray-800 dark:text-white">{{ goal.currentValue }}{{ goal.unit || '' }}</span>
                </span>
                <span>
                  进度: <span class="font-semibold text-gray-800 dark:text-white">{{ goal.progress }}%</span>
                </span>
              </div>

              <!-- 进度条 -->
              <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-3 mb-3">
                <div 
                  :class="getProgressColor(goal.progress)"
                  class="h-3 rounded-full transition-all duration-300"
                  :style="{ width: goal.progress + '%' }"
                ></div>
              </div>

              <!-- 月度 KPI 预览 -->
              <div v-if="goal.monthlyKpis && goal.monthlyKpis.length > 0" class="mt-3">
                <div class="text-xs text-gray-500 dark:text-gray-400 mb-2">月度 KPI ({{ goal.monthlyKpis.length }})</div>
                <div class="grid grid-cols-6 gap-2">
                  <div 
                    v-for="kpi in goal.monthlyKpis.slice(0, 6)" 
                    :key="kpi.id"
                    :class="getKpiStatusClass(kpi.status)"
                    class="px-2 py-1 rounded text-xs text-center"
                    :title="`${kpi.month}月: ${kpi.progress}%`"
                  >
                    {{ kpi.month }}月
                  </div>
                </div>
              </div>
            </div>
            <div class="flex gap-2 ml-4">
              <NuxtLink :to="`/admin/goals/${goal.id}/kpis`" class="px-3 py-1 text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">KPI</NuxtLink>
              <button @click="editGoal(goal)" class="px-3 py-1 text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">编辑</button>
              <button @click="deleteGoal(goal.id)" class="px-3 py-1 text-sm text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300">删除</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑目标模态框 -->
    <div v-if="showCreateModal || editingGoal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="closeModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h2 class="text-xl font-bold text-gray-800 dark:text-white mb-4">
            {{ editingGoal ? '编辑目标' : '新建年度目标' }}
          </h2>
          <form @submit.prevent="saveGoal" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">目标年份 *</label>
                <input v-model.number="goalForm.year" type="number" required min="2020" max="2100" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">分类</label>
                <input v-model="goalForm.category" type="text" placeholder="工作/学习/生活等" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">目标标题 *</label>
              <input v-model="goalForm.title" type="text" required class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">目标描述</label>
              <textarea v-model="goalForm.description" rows="3" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"></textarea>
            </div>
            <div class="grid grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">目标数值</label>
                <input v-model.number="goalForm.targetValue" type="number" step="0.01" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">当前数值</label>
                <input v-model.number="goalForm.currentValue" type="number" step="0.01" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">单位</label>
                <input v-model="goalForm.unit" type="text" placeholder="个/篇/小时等" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">开始日期</label>
                <input v-model="goalForm.startDate" type="date" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">结束日期</label>
                <input v-model="goalForm.endDate" type="date" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">状态</label>
              <select v-model="goalForm.status" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                <option value="active">进行中</option>
                <option value="completed">已完成</option>
                <option value="archived">已归档</option>
              </select>
            </div>
            <div class="flex justify-end gap-3 pt-4">
              <button 
                type="button" 
                @click="closeModal" 
                class="px-4 py-2 bg-gray-200 dark:bg-gray-700 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors font-bold border-2 border-gray-400 dark:border-gray-500 shadow-sm"
                class="text-gray-900 dark:text-gray-100"
              >
                <span class="dark:text-gray-100" style="color: inherit;">取消</span>
              </button>
              <button type="submit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors font-medium shadow-md">保存</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Goal {
  id: number
  year: number
  title: string
  description?: string
  category?: string
  targetValue?: number
  currentValue: number
  unit?: string
  status: string
  progress: number
  startDate?: string
  endDate?: string
  monthlyKpis?: any[]
}

interface GoalStats {
  Total: number
  Active: number
  Completed: number
  Archived: number
  AverageProgress: number
}

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const toast = useNotification()

const goals = ref<Goal[]>([])
const stats = ref<GoalStats | null>(null)
const loading = ref(false)
const filterYear = ref<number | null>(null)
const filterStatus = ref('')
const showCreateModal = ref(false)
const editingGoal = ref<Goal | null>(null)

const years = computed(() => {
  const currentYear = new Date().getFullYear()
  return Array.from({ length: 5 }, (_, i) => currentYear - i)
})

const goalForm = ref({
  year: new Date().getFullYear(),
  title: '',
  description: '',
  category: '',
  targetValue: null as number | null,
  currentValue: 0,
  unit: '',
  status: 'active',
  startDate: '',
  endDate: ''
})

const fetchGoals = async () => {
  loading.value = true
  try {
    const params: any = {}
    if (filterYear.value) params.year = filterYear.value
    if (filterStatus.value) params.status = filterStatus.value

    const res = await api.get('/Goals', { params })
    if (res?.data?.Items) {
      goals.value = res.data.Items
    }
  } catch (error) {
    console.error('获取目标列表失败:', error)
    toast.error('获取目标列表失败')
  } finally {
    loading.value = false
  }
}

const fetchStats = async () => {
  try {
    const params: any = {}
    if (filterYear.value) params.year = filterYear.value
    const res = await api.get('/Goals/stats', { params })
    if (res?.data) {
      stats.value = res.data
    }
  } catch (error) {
    console.error('获取统计失败:', error)
  }
}

const saveGoal = async () => {
  try {
    const data = {
      ...goalForm.value,
      startDate: goalForm.value.startDate || null,
      endDate: goalForm.value.endDate || null
    }

    if (editingGoal.value) {
      await api.put(`/Goals/${editingGoal.value.id}`, data)
      toast.success('目标更新成功')
    } else {
      await api.post('/Goals', data)
      toast.success('目标创建成功')
    }

    closeModal()
    fetchGoals()
    fetchStats()
  } catch (error) {
    console.error('保存目标失败:', error)
    toast.error('保存目标失败')
  }
}

const editGoal = (goal: Goal) => {
  editingGoal.value = goal
  goalForm.value = {
    year: goal.year,
    title: goal.title,
    description: goal.description || '',
    category: goal.category || '',
    targetValue: goal.targetValue || null,
    currentValue: goal.currentValue,
    unit: goal.unit || '',
    status: goal.status,
    startDate: goal.startDate ? goal.startDate.split('T')[0] : '',
    endDate: goal.endDate ? goal.endDate.split('T')[0] : ''
  }
}

const deleteGoal = async (id: number) => {
  if (!confirm('确定要删除这个目标吗？删除后关联的月度 KPI 也会被删除。')) return

  try {
    await api.delete(`/Goals/${id}`)
    toast.success('目标删除成功')
    fetchGoals()
    fetchStats()
  } catch (error) {
    console.error('删除目标失败:', error)
    toast.error('删除目标失败')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingGoal.value = null
  goalForm.value = {
    year: new Date().getFullYear(),
    title: '',
    description: '',
    category: '',
    targetValue: null,
    currentValue: 0,
    unit: '',
    status: 'active',
    startDate: '',
    endDate: ''
  }
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    active: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    completed: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300',
    archived: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300'
  }
  return classes[status] || classes.active
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    active: '进行中',
    completed: '已完成',
    archived: '已归档'
  }
  return texts[status] || status
}

const getProgressColor = (progress: number) => {
  if (progress === 100) return 'bg-green-500'
  if (progress >= 50) return 'bg-blue-500'
  if (progress >= 25) return 'bg-yellow-500'
  return 'bg-gray-400'
}

const getKpiStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400',
    in_progress: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    completed: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300'
  }
  return classes[status] || classes.pending
}

onMounted(() => {
  fetchGoals()
  fetchStats()
})
</script>

