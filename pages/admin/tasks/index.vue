<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">任务管理</h1>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
        + 新建任务
      </button>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">总任务</div>
        <div class="text-2xl font-bold text-gray-800 dark:text-white">{{ stats?.Total || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">待处理</div>
        <div class="text-2xl font-bold text-yellow-600 dark:text-yellow-400">{{ stats?.Pending || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">进行中</div>
        <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">{{ stats?.InProgress || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">已完成</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats?.Completed || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-500 dark:text-gray-400">已逾期</div>
        <div class="text-2xl font-bold text-red-600 dark:text-red-400">{{ stats?.Overdue || 0 }}</div>
      </div>
    </div>

    <!-- 筛选栏 -->
    <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6 flex gap-4 flex-wrap">
      <input v-model="keyword" @keyup.enter="fetchTasks" type="text" placeholder="搜索任务..." class="flex-1 min-w-[200px] border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
      <select v-model="filterStatus" @change="fetchTasks" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option value="">全部状态</option>
        <option value="pending">待处理</option>
        <option value="in_progress">进行中</option>
        <option value="completed">已完成</option>
        <option value="cancelled">已取消</option>
      </select>
      <select v-model="filterPriority" @change="fetchTasks" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option value="">全部优先级</option>
        <option value="0">低</option>
        <option value="1">中</option>
        <option value="2">高</option>
        <option value="3">紧急</option>
      </select>
      <button @click="fetchTasks" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">搜索</button>
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
              <button type="button" @click="closeModal" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">取消</button>
              <button type="submit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">保存</button>
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
const toast = useToast()

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
    }
  } catch (error) {
    console.error('获取任务列表失败:', error)
    toast.error('获取任务列表失败')
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
  } catch (error) {
    console.error('获取统计失败:', error)
  }
}

const saveTask = async () => {
  try {
    const data = {
      ...taskForm.value,
      dueDate: taskForm.value.dueDate ? new Date(taskForm.value.dueDate).toISOString() : null
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
  } catch (error) {
    console.error('保存任务失败:', error)
    toast.error('保存任务失败')
  }
}

const editTask = (task: Task) => {
  editingTask.value = task
  taskForm.value = {
    title: task.title,
    description: task.description || '',
    status: task.status,
    priority: task.priority,
    category: task.category || '',
    tags: task.tags || '',
    dueDate: task.dueDate ? new Date(task.dueDate).toISOString().slice(0, 16) : '',
    progress: task.progress
  }
}

const deleteTask = async (id: number) => {
  if (!confirm('确定要删除这个任务吗？')) return

  try {
    await api.delete(`/Tasks/${id}`)
    toast.success('任务删除成功')
    fetchTasks()
    fetchStats()
  } catch (error) {
    console.error('删除任务失败:', error)
    toast.error('删除任务失败')
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
    pending: 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-300',
    in_progress: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    completed: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300',
    cancelled: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300'
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
    0: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300',
    1: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    2: 'bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-300',
    3: 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300'
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
  if (progress === 100) return 'bg-green-500'
  if (progress >= 50) return 'bg-blue-500'
  if (progress >= 25) return 'bg-yellow-500'
  return 'bg-gray-400'
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

