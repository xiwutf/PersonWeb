<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold tasks-page-title">任务管理</h1>
      <n-button type="primary" @click="showCreateModal = true">
        + 新建任务
      </n-button>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
      <div class="task-stat-card">
        <div class="task-stat-label">总任务</div>
        <div class="task-stat-value">{{ stats?.Total || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">待处理</div>
        <div class="task-stat-value task-stat-value-warning">{{ stats?.Pending || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">进行中</div>
        <div class="task-stat-value task-stat-value-primary">{{ stats?.InProgress || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">已完成</div>
        <div class="task-stat-value task-stat-value-success">{{ stats?.Completed || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">已逾期</div>
        <div class="task-stat-value task-stat-value-error">{{ stats?.Overdue || 0 }}</div>
      </div>
    </div>

    <!-- 筛选栏 -->
    <div class="task-filter-bar">
      <n-input v-model:value="keyword" @keyup.enter="fetchTasks" placeholder="搜索任务..." class="flex-1 min-w-[200px]" />
      <n-select v-model:value="filterStatus" @update:value="fetchTasks" placeholder="全部状态" class="min-w-[120px]">
        <n-option value="" label="全部状态" />
        <n-option value="pending" label="待处理" />
        <n-option value="in_progress" label="进行中" />
        <n-option value="completed" label="已完成" />
        <n-option value="cancelled" label="已取消" />
      </n-select>
      <n-select v-model:value="filterPriority" @update:value="fetchTasks" placeholder="全部优先级" class="min-w-[120px]">
        <n-option value="" label="全部优先级" />
        <n-option value="0" label="低" />
        <n-option value="1" label="中" />
        <n-option value="2" label="高" />
        <n-option value="3" label="紧急" />
      </n-select>
      <n-button @click="fetchTasks">搜索</n-button>
    </div>

    <!-- 任务列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div v-if="loading" class="p-8 text-center text-gray-500 dark:text-gray-400">加载中...</div>
      <div v-else-if="tasks.length === 0" class="p-8 text-center text-gray-500 dark:text-gray-400">暂无任务</div>
      <div v-else class="divide-y divide-gray-200 dark:divide-gray-700">
        <div v-for="task in tasks" :key="task.id" class="p-4 hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h3 class="text-lg font-semibold text-gray-800 dark:text-white">{{ task.title }}</h3>
                <span :class="getPriorityClass(task.priority)" class="px-2 py-1 rounded text-xs font-medium">
                  {{ getPriorityText(task.priority) }}
                </span>
                <span :class="getStatusClass(task.status)" class="px-2 py-1 rounded text-xs font-medium">
                  {{ getStatusText(task.status) }}
                </span>
              </div>
              <p v-if="task.description" class="text-sm text-gray-600 dark:text-gray-400 mb-2">{{ task.description }}</p>
              <div class="flex items-center gap-4 text-sm text-gray-500 dark:text-gray-400">
                <span v-if="task.category" class="px-2 py-1 bg-gray-100 dark:bg-gray-700 rounded">{{ task.category }}</span>
                <span v-if="task.dueDate">截止: {{ formatDate(task.dueDate) }}</span>
                <span>进度: {{ task.progress }}%</span>
              </div>
              <!-- 进度条 -->
              <div class="mt-2 w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                <div 
                  :class="getProgressColor(task.progress)"
                  class="h-2 rounded-full transition-all duration-300"
                  :style="{ width: task.progress + '%' }"
                ></div>
              </div>
            </div>
            <div class="flex gap-2 ml-4">
              <button @click="editTask(task)" class="px-3 py-1 text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">编辑</button>
              <button @click="deleteTask(task.id)" class="px-3 py-1 text-sm text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300">删除</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑任务模态框 -->
    <div v-if="showCreateModal || editingTask" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="closeModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h2 class="text-xl font-bold text-gray-800 dark:text-white mb-4">
            {{ editingTask ? '编辑任务' : '新建任务' }}
          </h2>
          <form @submit.prevent="saveTask" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">任务标题 *</label>
              <input v-model="taskForm.title" type="text" required class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">任务描述</label>
              <textarea v-model="taskForm.description" rows="3" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">状态</label>
                <select v-model="taskForm.status" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                  <option value="pending">待处理</option>
                  <option value="in_progress">进行中</option>
                  <option value="completed">已完成</option>
                  <option value="cancelled">已取消</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">优先级</label>
                <select v-model="taskForm.priority" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                  <option :value="0">低</option>
                  <option :value="1">中</option>
                  <option :value="2">高</option>
                  <option :value="3">紧急</option>
                </select>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">分类</label>
                <input v-model="taskForm.category" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">截止日期</label>
                <input v-model="taskForm.dueDate" type="datetime-local" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">进度 (%)</label>
              <input v-model.number="taskForm.progress" type="number" min="0" max="100" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
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
interface Task {
  id: number
  title: string
  description?: string
  status: string
  priority: number
  category?: string
  tags?: string
  dueDate?: string
  progress: number
  createdAt: string
  updatedAt: string
}

interface TaskStats {
  Total: number
  Pending: number
  InProgress: number
  Completed: number
  Overdue: number
  AverageProgress: number
}

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const toast = useNotification()

const tasks = ref<Task[]>([])
const stats = ref<TaskStats | null>(null)
const loading = ref(false)
const keyword = ref('')
const filterStatus = ref('')
const filterPriority = ref('')
const showCreateModal = ref(false)
const editingTask = ref<Task | null>(null)

const taskForm = ref({
  title: '',
  description: '',
  status: 'pending',
  priority: 0,
  category: '',
  tags: '',
  dueDate: '',
  progress: 0
})

const fetchTasks = async () => {
  loading.value = true
  try {
    const params: any = {
      page: 1,
      pageSize: 50
    }
    if (filterStatus.value) params.status = filterStatus.value
    if (filterPriority.value) params.priority = parseInt(filterPriority.value)
    if (keyword.value) params.keyword = keyword.value

    const res = await api.get('/Tasks', { params })
    if (res?.data?.Items) {
      tasks.value = res.data.Items
    } else if (Array.isArray(res?.data)) {
      // 兼容直接返回数组的情况
      tasks.value = res.data
    }
  } catch (error: any) {
    console.error('获取任务列表失败:', error)
    const errorMsg = error?.response?.data?.message || error?.data?.message || error?.message || '获取任务列表失败'
    toast.error(errorMsg)
  } finally {
    loading.value = false
  }
}

const fetchStats = async () => {
  try {
    const res = await api.get('/Tasks/stats')
    if (res?.data) {
      stats.value = res.data
    }
  } catch (error: any) {
    console.error('获取统计失败:', error)
    // 统计失败不影响主功能，只记录错误
  }
}

const saveTask = async () => {
  try {
    // 处理日期：datetime-local 输入格式为 YYYY-MM-DDTHH:mm，需要转换为 ISO 字符串
    let dueDateISO: string | null = null
    if (taskForm.value.dueDate) {
      try {
        // datetime-local 格式：YYYY-MM-DDTHH:mm
        // 转换为 Date 对象，然后转换为 ISO 字符串
        const date = new Date(taskForm.value.dueDate)
        if (!isNaN(date.getTime())) {
          dueDateISO = date.toISOString()
        }
      } catch (e) {
        console.error('日期转换失败:', e)
        dueDateISO = null
      }
    }

    const data: any = {
      title: taskForm.value.title,
      description: taskForm.value.description || null,
      status: taskForm.value.status,
      priority: taskForm.value.priority,
      category: taskForm.value.category || null,
      tags: taskForm.value.tags || null,
      dueDate: dueDateISO,
      progress: taskForm.value.progress || 0
    }

    if (editingTask.value) {
      await api.put(`/Tasks/${editingTask.value.id}`, data)
      toast.success('任务更新成功')
    } else {
      await api.post('/Tasks', data)
      toast.success('任务创建成功')
    }

    closeModal()
    fetchTasks()
    fetchStats()
  } catch (error: any) {
    console.error('保存任务失败:', error)
    const errorMsg = error?.response?.data?.message || error?.data?.message || error?.message || '保存任务失败'
    toast.error(errorMsg)
  }
}

const editTask = (task: Task) => {
  editingTask.value = task
  // 处理日期格式：datetime-local 需要 YYYY-MM-DDTHH:mm 格式
  let dueDateFormatted = ''
  if (task.dueDate) {
    try {
      const date = new Date(task.dueDate)
      // 转换为本地时间，格式化为 YYYY-MM-DDTHH:mm
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      const hours = String(date.getHours()).padStart(2, '0')
      const minutes = String(date.getMinutes()).padStart(2, '0')
      dueDateFormatted = `${year}-${month}-${day}T${hours}:${minutes}`
    } catch (e) {
      console.error('日期格式化失败:', e)
      dueDateFormatted = ''
    }
  }
  
  taskForm.value = {
    title: task.title,
    description: task.description || '',
    status: task.status,
    priority: task.priority,
    category: task.category || '',
    tags: task.tags || '',
    dueDate: dueDateFormatted,
    progress: task.progress
  }
}

const deleteTask = async (id: number) => {
  if (!confirm('确定要删除这个任务吗？')) return

  try {
    await api.del(`/Tasks/${id}`)
    toast.success('任务删除成功')
    fetchTasks()
    fetchStats()
  } catch (error: any) {
    console.error('删除任务失败:', error)
    const errorMsg = error?.response?.data?.message || error?.data?.message || error?.message || '删除任务失败'
    toast.error(errorMsg)
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingTask.value = null
  taskForm.value = {
    title: '',
    description: '',
    status: 'pending',
    priority: 0,
    category: '',
    tags: '',
    dueDate: '',
    progress: 0
  }
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    pending: 'task-status-pending',
    in_progress: 'task-status-in-progress',
    completed: 'task-status-completed',
    cancelled: 'task-status-cancelled'
  }
  return classes[status] || classes.pending
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    pending: '待处理',
    in_progress: '进行中',
    completed: '已完成',
    cancelled: '已取消'
  }
  return texts[status] || status
}

const getPriorityClass = (priority: number) => {
  const classes: Record<number, string> = {
    0: 'task-priority-low',
    1: 'task-priority-medium',
    2: 'task-priority-high',
    3: 'task-priority-urgent'
  }
  return classes[priority] || classes[0]
}

const getPriorityText = (priority: number) => {
  const texts: Record<number, string> = {
    0: '低',
    1: '中',
    2: '高',
    3: '紧急'
  }
  return texts[priority] || '低'
}

const getProgressColor = (progress: number) => {
  if (progress === 100) return 'task-progress-complete'
  if (progress >= 50) return 'task-progress-good'
  if (progress >= 25) return 'task-progress-fair'
  return 'task-progress-poor'
}

const formatDate = (date: string) => {
  if (!date) return ''
  return new Date(date).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchTasks()
  fetchStats()
})
</script>

<style scoped>
/* 页面标题 - 使用 CSS 变量 */
.tasks-page-title {
  color: var(--color-text-main, #0f172a);
}

/* 统计卡片样式 - 使用 CSS 变量 */
.task-stat-card {
  background: var(--color-bg-card, #ffffff);
  border: 1px solid var(--color-border-subtle, #e5e7eb);
  border-radius: 0.5rem;
  padding: 1rem;
  box-shadow: var(--shadow-sm, 0 1px 2px 0 rgba(0, 0, 0, 0.05));
}

.task-stat-label {
  font-size: 0.875rem;
  color: var(--color-text-muted, #6b7280);
}

.task-stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text-main, #0f172a);
}

.task-stat-value-warning {
  color: var(--color-warning, #f59e0b);
}

.task-stat-value-primary {
  color: var(--color-primary, #3b82f6);
}

.task-stat-value-success {
  color: var(--color-success, #10b981);
}

.task-stat-value-error {
  color: var(--color-error, #ef4444);
}

/* 筛选栏样式 - 使用 CSS 变量 */
.task-filter-bar {
  background: var(--color-bg-card, #ffffff);
  border: 1px solid var(--color-border-subtle, #e5e7eb);
  border-radius: 0.5rem;
  padding: 1rem;
  margin-bottom: 1.5rem;
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  box-shadow: var(--shadow-sm, 0 1px 2px 0 rgba(0, 0, 0, 0.05));
}

/* 任务状态样式 - 使用 CSS 变量 */
.task-status-pending {
  background: var(--color-warning-soft, rgba(251, 191, 36, 0.1));
  color: var(--color-warning, #f59e0b);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-status-in-progress {
  background: var(--color-primary-soft, rgba(59, 130, 246, 0.1));
  color: var(--color-primary, #3b82f6);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-status-completed {
  background: var(--color-success-soft, rgba(16, 185, 129, 0.1));
  color: var(--color-success, #10b981);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-status-cancelled {
  background: var(--color-bg-elevated, #f3f4f6);
  color: var(--color-text-muted, #6b7280);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

/* 任务优先级样式 - 使用 CSS 变量 */
.task-priority-low {
  background: var(--color-bg-elevated, #f3f4f6);
  color: var(--color-text-muted, #6b7280);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-priority-medium {
  background: var(--color-primary-soft, rgba(59, 130, 246, 0.1));
  color: var(--color-primary, #3b82f6);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-priority-high {
  background: var(--color-warning-soft, rgba(249, 115, 22, 0.1));
  color: var(--color-warning, #f97316);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-priority-urgent {
  background: var(--color-error-soft, rgba(239, 68, 68, 0.1));
  color: var(--color-error, #ef4444);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

/* 任务进度条颜色样式 - 使用 CSS 变量 */
.task-progress-complete {
  background: var(--color-success, #10b981);
}

.task-progress-good {
  background: var(--color-primary, #3b82f6);
}

.task-progress-fair {
  background: var(--color-warning, #f59e0b);
}

.task-progress-poor {
  background: var(--color-text-muted, #9ca3af);
}

/* 深色主题适配 */
html[data-theme="dark"] .tasks-page-title,
html.dark .tasks-page-title {
  color: var(--color-text-main, #ffffff);
}

html[data-theme="dark"] .task-stat-card,
html.dark .task-stat-card {
  background: var(--color-bg-card, rgba(255, 255, 255, 0.05));
  border-color: var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}

html[data-theme="dark"] .task-stat-label,
html.dark .task-stat-label {
  color: var(--color-text-muted, #9ca3af);
}

html[data-theme="dark"] .task-stat-value,
html.dark .task-stat-value {
  color: var(--color-text-main, #ffffff);
}

html[data-theme="dark"] .task-filter-bar,
html.dark .task-filter-bar {
  background: var(--color-bg-card, rgba(255, 255, 255, 0.05));
  border-color: var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}
</style>

