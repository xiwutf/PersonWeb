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
        <div class="task-stat-label">总任�?/div>
        <div class="task-stat-value">{{ stats?.Total || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">待处�?/div>
        <div class="task-stat-value task-stat-value-warning">{{ stats?.Pending || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">进行�?/div>
        <div class="task-stat-value task-stat-value-primary">{{ stats?.InProgress || 0 }}</div>
      </div>
      <div class="task-stat-card">
        <div class="task-stat-label">已完�?/div>
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
      <n-select v-model:value="filterStatus" @update:value="fetchTasks" placeholder="全部状�? class="min-w-[120px]">
        <n-option value="" label="全部状�? />
        <n-option value="pending" label="待处�? />
        <n-option value="in_progress" label="进行�? />
        <n-option value="completed" label="已完�? />
        <n-option value="cancelled" label="已取�? />
      </n-select>
      <n-select v-model:value="filterPriority" @update:value="fetchTasks" placeholder="全部优先�? class="min-w-[120px]">
        <n-option value="" label="全部优先�? />
        <n-option value="0" label="�? />
        <n-option value="1" label="�? />
        <n-option value="2" label="�? />
        <n-option value="3" label="紧�? />
      </n-select>
      <n-button @click="fetchTasks">搜索</n-button>
    </div>

    <!-- 任务列表 -->
    <div class="task-list-container">
      <div v-if="loading" class="p-8 text-center task-list-loading">加载�?..</div>
      <div v-else-if="tasks.length === 0" class="p-8 text-center task-list-empty">暂无任务</div>
      <div v-else class="task-list-divider">
        <div v-for="task in tasks" :key="task.id" class="task-list-item">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h3 class="text-lg font-semibold task-list-item-title">{{ task.title }}</h3>
                <span :class="getPriorityClass(task.priority)" class="px-2 py-1 rounded text-xs font-medium">
                  {{ getPriorityText(task.priority) }}
                </span>
                <span :class="getStatusClass(task.status)" class="px-2 py-1 rounded text-xs font-medium">
                  {{ getStatusText(task.status) }}
                </span>
              </div>
              <p v-if="task.description" class="text-sm task-list-item-description mb-2">{{ task.description }}</p>
              <div class="flex items-center gap-4 text-sm task-list-item-meta">
                <span v-if="task.category" class="px-2 py-1 task-list-item-category rounded">{{ task.category }}</span>
                <span v-if="task.dueDate">截止: {{ formatDate(task.dueDate) }}</span>
                <span>进度: {{ task.progress }}%</span>
              </div>
              <!-- 进度�?-->
              <div class="mt-2 w-full task-list-progress-bg rounded-full h-2">
                <div 
                  :class="getProgressColor(task.progress)"
                  class="h-2 rounded-full transition-all duration-300"
                  :style="{ width: task.progress + '%' }"
                ></div>
              </div>
            </div>
            <div class="flex gap-2 ml-4">
              <button @click="editTask(task)" class="px-3 py-1 text-sm task-list-action-btn task-list-action-btn-primary">编辑</button>
              <button @click="deleteTask(task.id)" class="px-3 py-1 text-sm task-list-action-btn task-list-action-btn-error">删除</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑任务模态框 -->
    <n-modal v-model:show="showModal" preset="card" :title="editingTask ? '编辑任务' : '新建任务'" style="width: 800px; max-height: 90vh;">
      <n-form :model="taskForm" @submit.prevent="saveTask">
        <n-form-item label="任务标题" path="title">
          <n-input v-model:value="taskForm.title" placeholder="请输入任务标�? />
        </n-form-item>
        <n-form-item label="任务描述" path="description">
          <n-input v-model:value="taskForm.description" type="textarea" :rows="3" placeholder="请输入任务描�? />
        </n-form-item>
        <n-grid :cols="2" :x-gap="16">
          <n-form-item-gi label="状�? path="status">
            <n-select v-model:value="taskForm.status" :options="[
              { label: '待处�?, value: 'pending' },
              { label: '进行�?, value: 'in_progress' },
              { label: '已完�?, value: 'completed' },
              { label: '已取�?, value: 'cancelled' }
            ]" />
          </n-form-item-gi>
          <n-form-item-gi label="优先�? path="priority">
            <n-select v-model:value="taskForm.priority" :options="[
              { label: '�?, value: 0 },
              { label: '�?, value: 1 },
              { label: '�?, value: 2 },
              { label: '紧�?, value: 3 }
            ]" />
          </n-form-item-gi>
        </n-grid>
        <n-grid :cols="2" :x-gap="16">
          <n-form-item-gi label="分类" path="category">
            <n-input v-model:value="taskForm.category" placeholder="请输入分�? />
          </n-form-item-gi>
          <n-form-item-gi label="截止日期" path="dueDate">
            <n-date-picker v-model:value="taskForm.dueDate" type="datetime" clearable />
          </n-form-item-gi>
        </n-grid>
        <n-form-item label="进度 (%)" path="progress">
          <n-input-number v-model:value="taskForm.progress" :min="0" :max="100" />
        </n-form-item>
        <n-form-item>
          <n-space justify="end" style="width: 100%">
            <n-button @click="closeModal">取消</n-button>
            <n-button type="primary" attr-type="submit">保存</n-button>
          </n-space>
        </n-form-item>
      </n-form>
    </n-modal>
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

// 计算属性：模态框显示状�?const showModal = computed({
  get: () => showCreateModal.value || !!editingTask.value,
  set: (value: boolean) => {
    if (!value) {
      showCreateModal.value = false
      editingTask.value = null
    }
  }
})

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
      // 兼容直接返回数组的情�?      tasks.value = res.data
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
    // 处理日期：datetime-local 输入格式�?YYYY-MM-DDTHH:mm，需要转换为 ISO 字符�?    let dueDateISO: string | null = null
    if (taskForm.value.dueDate) {
      try {
        // datetime-local 格式：YYYY-MM-DDTHH:mm
        // 转换�?Date 对象，然后转换为 ISO 字符�?        const date = new Date(taskForm.value.dueDate)
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
  // 处理日期格式：datetime-local 需�?YYYY-MM-DDTHH:mm 格式
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
      console.error('日期格式化失�?', e)
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
  if (!confirm('确定要删除这个任务吗�?)) return

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
    pending: '待处�?,
    in_progress: '进行�?,
    completed: '已完�?,
    cancelled: '已取�?
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
    0: '�?,
    1: '�?,
    2: '�?,
    3: '紧�?
  }
  return texts[priority] || '�?
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
  color: var(--color-text-main, var(--color-text-main));
}

/* 统计卡片样式 - 使用 CSS 变量 */
.task-stat-card {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-subtle, var(--color-border));
  border-radius: 0.5rem;
  padding: 1rem;
  box-shadow: var(--shadow-sm, 0 1px 2px 0 var(--color-border));
}

.task-stat-label {
  font-size: 0.875rem;
  color: var(--color-text-muted, var(--color-text-sec));
}

.task-stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text-main, var(--color-text-main));
}

.task-stat-value-warning {
  color: var(--color-warning, var(--color-warning));
}

.task-stat-value-primary {
  color: var(--color-primary, var(--color-primary));
}

.task-stat-value-success {
  color: var(--color-success, var(--color-success));
}

.task-stat-value-error {
  color: var(--color-error, var(--color-danger));
}

/* 筛选栏样式 - 使用 CSS 变量 */
.task-filter-bar {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-subtle, var(--color-border));
  border-radius: 0.5rem;
  padding: 1rem;
  margin-bottom: 1.5rem;
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  box-shadow: var(--shadow-sm, 0 1px 2px 0 var(--color-border));
}

/* 任务状态样�?- 使用 CSS 变量 */
.task-status-pending {
  background: var(--color-warning-soft, rgba(251, 191, 36, 0.1));
  color: var(--color-warning, var(--color-warning));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-status-in-progress {
  background: var(--color-primary-soft, rgba(59, 130, 246, 0.1));
  color: var(--color-primary, var(--color-primary));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-status-completed {
  background: var(--color-success-soft, rgba(16, 185, 129, 0.1));
  color: var(--color-success, var(--color-success));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-status-cancelled {
  background: var(--color-bg-elevated, var(--color-gray-100));
  color: var(--color-text-muted, var(--color-text-sec));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

/* 任务优先级样�?- 使用 CSS 变量 */
.task-priority-low {
  background: var(--color-bg-elevated, var(--color-gray-100));
  color: var(--color-text-muted, var(--color-text-sec));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-priority-medium {
  background: var(--color-primary-soft, rgba(59, 130, 246, 0.1));
  color: var(--color-primary, var(--color-primary));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-priority-high {
  background: var(--color-warning-soft, rgba(249, 115, 22, 0.1));
  color: var(--color-warning, var(--color-orange-500));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.task-priority-urgent {
  background: var(--color-error-soft, rgba(239, 68, 68, 0.1));
  color: var(--color-error, var(--color-danger));
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

/* 任务进度条颜色样�?- 使用 CSS 变量 */
.task-progress-complete {
  background: var(--color-success, var(--color-success));
}

.task-progress-good {
  background: var(--color-primary, var(--color-primary));
}

.task-progress-fair {
  background: var(--color-warning, var(--color-warning));
}

.task-progress-poor {
  background: var(--color-text-muted, var(--color-gray-400));
}

/* 深色主题适配 */
html[data-theme="dark"] .tasks-page-title,
html.dark .tasks-page-title {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .task-stat-card,
html.dark .task-stat-card {
  background: var(--color-bg-card);
  border-color: var(--color-border-subtle);
}

html[data-theme="dark"] .task-stat-label,
html.dark .task-stat-label {
  color: var(--color-text-muted);
}

html[data-theme="dark"] .task-stat-value,
html.dark .task-stat-value {
  color: var(--color-text-main);
}

html[data-theme="dark"] .task-filter-bar,
html.dark .task-filter-bar {
  background: var(--color-bg-card);
  border-color: var(--color-border-subtle);
}

/* 任务列表样式 - 使用 CSS 变量 */
.task-list-container {
  background: var(--color-bg-card, var(--color-bg-card));
  border: 1px solid var(--color-border-subtle, var(--color-border));
  border-radius: 0.5rem;
  box-shadow: var(--shadow-sm, 0 1px 2px 0 var(--color-border));
  overflow: hidden;
}

.task-list-loading,
.task-list-empty {
  color: var(--color-text-muted, var(--color-text-sec));
}

.task-list-divider > * + * {
  border-top: 1px solid var(--color-border-subtle, var(--color-border));
}

.task-list-item {
  padding: 1rem;
  transition: background-color 0.2s ease;
}

.task-list-item:hover {
  background-color: var(--color-bg-elevated, var(--color-gray-50));
}

.task-list-item-title {
  color: var(--color-text-main, var(--color-text-main));
}

.task-list-item-description {
  color: var(--color-text-sub, var(--color-gray-600));
}

.task-list-item-meta {
  color: var(--color-text-muted, var(--color-text-sec));
}

.task-list-item-category {
  background: var(--color-bg-elevated, var(--color-gray-100));
  color: var(--color-text-muted, var(--color-text-sec));
}

.task-list-progress-bg {
  background: var(--color-bg-elevated, var(--color-border));
}

.task-list-action-btn {
  background: none;
  border: none;
  cursor: pointer;
  transition: color 0.2s ease;
}

.task-list-action-btn-primary {
  color: var(--color-primary, var(--color-primary));
}

.task-list-action-btn-primary:hover {
  color: var(--color-primary-hover, var(--color-primary-hover));
}

.task-list-action-btn-error {
  color: var(--color-error, var(--color-danger));
}

.task-list-action-btn-error:hover {
  color: var(--color-error-hover, var(--color-danger-600));
}

/* 深色主题适配 */
html[data-theme="dark"] .task-list-container,
html.dark .task-list-container {
  background: var(--color-bg-card, rgba(255, 255, 255, 0.05));
  border-color: var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}

html[data-theme="dark"] .task-list-item:hover,
html.dark .task-list-item:hover {
  background-color: var(--color-bg-elevated, rgba(255, 255, 255, 0.05));
}

html[data-theme="dark"] .task-list-item-title,
html.dark .task-list-item-title {
  color: var(--color-text-main, var(--color-bg-card));
}

html[data-theme="dark"] .task-list-item-description,
html.dark .task-list-item-description {
  color: var(--color-text-sub, var(--color-gray-400));
}

html[data-theme="dark"] .task-list-item-category,
html.dark .task-list-item-category {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.1));
  color: var(--color-text-muted, var(--color-gray-400));
}

html[data-theme="dark"] .task-list-progress-bg,
html.dark .task-list-progress-bg {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.1));
}
</style>

