<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8 font-['Outfit']">
    <div class="max-w-7xl mx-auto">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-blue-400 to-purple-600 mb-2">
          DIGITAL TWIN DASHBOARD
        </h1>
        <p class="text-gray-500 dark:text-gray-400 tracking-widest uppercase text-sm">Quantified Self & Life Metrics</p>
      </div>

      <!-- 核心指标卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <!-- Life Battery -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-green-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Life Battery</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ latest.energy }}%</span>
            <span class="text-sm text-green-500 mb-1">Energy</span>
          </div>
          <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 mt-4">
            <div class="bg-gradient-to-r from-green-400 to-green-600 h-2 rounded-full transition-all duration-1000" :style="{ width: latest.energy + '%' }"></div>
          </div>
          <p class="text-xs text-gray-400 mt-2">Sleep: {{ latest.sleep }}h</p>
        </div>

        <!-- Activity -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-orange-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Daily Activity</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ latest.steps }}</span>
            <span class="text-sm text-orange-500 mb-1">Steps</span>
          </div>
          <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 mt-4">
            <div class="bg-gradient-to-r from-orange-400 to-red-500 h-2 rounded-full transition-all duration-1000" :style="{ width: Math.min((latest.steps / 10000) * 100, 100) + '%' }"></div>
          </div>
          <p class="text-xs text-gray-400 mt-2">Goal: 10,000</p>
        </div>

        <!-- Physique -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-blue-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Physique</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ latest.weight }}</span>
            <span class="text-sm text-blue-500 mb-1">kg</span>
          </div>
          <div class="mt-4">
             <span class="text-xs px-2 py-1 rounded bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">BMI: {{ (latest.weight / (1.75 * 1.75)).toFixed(1) }}</span>
          </div>
        </div>

        <!-- Net Worth -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-purple-500" fill="currentColor" viewBox="0 0 20 20"><path d="M4 4a2 2 0 00-2 2v1h16V6a2 2 0 00-2-2H4z"></path><path fill-rule="evenodd" d="M18 9H2v5a2 2 0 002 2h12a2 2 0 002-2V9zM4 13a1 1 0 011-1h1a1 1 0 110 2H5a1 1 0 01-1-1zm5-1a1 1 0 100 2h1a1 1 0 100-2H9z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Net Worth</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">¥{{ (latest.netWorth / 10000).toFixed(1) }}</span>
            <span class="text-sm text-purple-500 mb-1">万</span>
          </div>
          <div class="mt-4 flex items-center text-xs text-green-500">
            <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path></svg>
            <span>Growing</span>
          </div>
        </div>
      </div>

      <!-- 图表区域 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- 趋势图 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-6">Health Trends</h3>
          <div class="h-64">
            <Line :data="healthChartData" :options="chartOptions" />
          </div>
        </div>

        <!-- 财富趋势 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-6">Wealth Growth</h3>
          <div class="h-64">
            <Line :data="wealthChartData" :options="chartOptions" />
          </div>
        </div>
      </div>

      <!-- 任务统计 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- 任务统计卡片 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-bold text-gray-800 dark:text-white">任务统计</h3>
            <NuxtLink to="/admin/tasks" class="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">管理任务 →</NuxtLink>
          </div>
          <div v-if="taskStats" class="grid grid-cols-2 gap-4">
            <div class="bg-gradient-to-br from-yellow-50 to-yellow-100 dark:from-yellow-900 dark:to-yellow-800 rounded-xl p-4">
              <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">待处理</div>
              <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ taskStats.Pending || 0 }}</div>
            </div>
            <div class="bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900 dark:to-blue-800 rounded-xl p-4">
              <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">进行中</div>
              <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ taskStats.InProgress || 0 }}</div>
            </div>
            <div class="bg-gradient-to-br from-green-50 to-green-100 dark:from-green-900 dark:to-green-800 rounded-xl p-4">
              <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">已完成</div>
              <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ taskStats.Completed || 0 }}</div>
            </div>
            <div class="bg-gradient-to-br from-red-50 to-red-100 dark:from-red-900 dark:to-red-800 rounded-xl p-4">
              <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">已逾期</div>
              <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ taskStats.Overdue || 0 }}</div>
            </div>
          </div>
          <div v-else class="text-center text-gray-500 py-8">加载中...</div>
        </div>

        <!-- 年度目标卡片 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-bold text-gray-800 dark:text-white">年度目标</h3>
            <NuxtLink to="/admin/goals" class="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">管理目标 →</NuxtLink>
          </div>
          <div v-if="goalsLoading" class="text-center text-gray-500 py-8">加载中...</div>
          <div v-else-if="activeGoals.length === 0" class="text-center text-gray-500 py-8">
            <p class="mb-4">暂无年度目标</p>
            <NuxtLink to="/admin/goals" class="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">创建目标</NuxtLink>
          </div>
          <div v-else class="space-y-4 max-h-64 overflow-y-auto">
            <div v-for="goal in activeGoals" :key="goal.id" class="p-4 bg-gradient-to-br from-purple-50 to-indigo-50 dark:from-purple-900/20 dark:to-indigo-900/20 rounded-xl border border-purple-100 dark:border-purple-800">
              <div class="flex items-start justify-between mb-2">
                <h4 class="font-semibold text-gray-800 dark:text-white text-sm">{{ goal.title }}</h4>
                <span class="text-xs text-gray-500 dark:text-gray-400">{{ goal.year }}年</span>
              </div>
              <div v-if="goal.targetValue" class="text-xs text-gray-600 dark:text-gray-400 mb-2">
                {{ goal.currentValue }}{{ goal.unit || '' }} / {{ goal.targetValue }}{{ goal.unit || '' }}
              </div>
              <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 mb-2">
                <div 
                  :class="getGoalProgressColor(goal.progress)"
                  class="h-2 rounded-full transition-all duration-300"
                  :style="{ width: Math.min(goal.progress, 100) + '%' }"
                ></div>
              </div>
              <div class="text-xs text-gray-500 dark:text-gray-400 text-right">{{ goal.progress }}%</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 今日任务列表 -->
      <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 mb-8">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white">今日任务</h3>
          <button @click="fetchTodayTasks" class="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">刷新</button>
        </div>
          <div v-if="todayTasksLoading" class="text-center text-gray-500 py-8">加载中...</div>
          <div v-else-if="todayTasks.length === 0" class="text-center text-gray-500 py-8">暂无任务</div>
          <div v-else class="space-y-3 max-h-64 overflow-y-auto">
            <div v-for="task in todayTasks" :key="task.id" class="p-3 bg-gray-50 dark:bg-gray-700/50 rounded-lg">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <h4 class="font-medium text-gray-800 dark:text-white">{{ task.title }}</h4>
                    <span :class="getStatusClass(task.status)" class="px-2 py-0.5 rounded text-xs">
                      {{ getStatusText(task.status) }}
                    </span>
                  </div>
                  <div class="flex items-center gap-4 text-xs text-gray-500 dark:text-gray-400">
                    <span>进度: {{ task.progress }}%</span>
                    <span v-if="task.dueDate">截止: {{ formatTaskDate(task.dueDate) }}</span>
                  </div>
                  <div class="mt-2 w-full bg-gray-200 dark:bg-gray-600 rounded-full h-1.5">
                    <div 
                      :class="getProgressColor(task.progress)"
                      class="h-1.5 rounded-full transition-all duration-300"
                      :style="{ width: task.progress + '%' }"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- GitHub 代码统计 -->
      <div v-if="githubStats" class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- 代码语言分布 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-6">Code Languages</h3>
          <div v-if="githubLanguages.length > 0" class="h-64">
            <Doughnut :data="languageChartData" :options="languageChartOptions" />
          </div>
          <div v-else class="h-64 flex items-center justify-center text-gray-500">
            暂无数据
          </div>
        </div>

        <!-- GitHub 贡献统计 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-6">Repository Stats</h3>
          <div v-if="githubStats" class="space-y-4">
            <div class="grid grid-cols-2 gap-4">
              <div class="bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900 dark:to-blue-800 rounded-xl p-4">
                <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">Stars</div>
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ githubStats.stars || 0 }}</div>
              </div>
              <div class="bg-gradient-to-br from-green-50 to-green-100 dark:from-green-900 dark:to-green-800 rounded-xl p-4">
                <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">Forks</div>
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ githubStats.forks || 0 }}</div>
              </div>
              <div class="bg-gradient-to-br from-purple-50 to-purple-100 dark:from-purple-900 dark:to-purple-800 rounded-xl p-4">
                <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">Watchers</div>
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ githubStats.watchers || 0 }}</div>
              </div>
              <div class="bg-gradient-to-br from-orange-50 to-orange-100 dark:from-orange-900 dark:to-orange-800 rounded-xl p-4">
                <div class="text-sm text-gray-600 dark:text-gray-300 mb-1">Size</div>
                <div class="text-2xl font-bold text-gray-900 dark:text-white">{{ formatSize(githubStats.size || 0) }}</div>
              </div>
            </div>
          </div>
          <div v-else class="h-64 flex items-center justify-center text-gray-500">
            暂无数据
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
  Filler
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
  Legend,
  Filler
)

const api = useApi()
const metrics = ref<any[]>([])
const latest = ref({
  steps: 0,
  sleep: 0,
  weight: 0,
  netWorth: 0,
  energy: 0
})
const githubLanguages = ref<any[]>([])
const githubStats = ref<any>(null)
const taskStats = ref<any>(null)
const todayTasks = ref<any[]>([])
const todayTasksLoading = ref(false)
const activeGoals = ref<any[]>([])
const goalsLoading = ref(false)

// GitHub 仓库配置（可以从配置中心获取）
const githubRepo = ref<string | null>(null)

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
      labels: { color: '#9ca3af' }
    }
  },
  scales: {
    y: {
      grid: { color: '#374151' },
      ticks: { color: '#9ca3af' }
    },
    x: {
      grid: { display: false },
      ticks: { color: '#9ca3af' }
    }
  },
  interaction: {
    mode: 'index',
    intersect: false,
  },
}

const healthChartData = computed(() => {
  const labels = metrics.value.map(m => m.date.slice(5)).slice(-30) // Last 30 days
  const steps = metrics.value.map(m => m.steps).slice(-30)
  const weight = metrics.value.map(m => m.weight).slice(-30)

  return {
    labels,
    datasets: [
      {
        label: 'Steps',
        data: steps,
        borderColor: '#f97316',
        backgroundColor: 'rgba(249, 115, 22, 0.1)',
        yAxisID: 'y',
        fill: true,
        tension: 0.4
      },
      {
        label: 'Weight (kg)',
        data: weight,
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        yAxisID: 'y1',
        fill: true,
        tension: 0.4
      }
    ]
  }
})

const wealthChartData = computed(() => {
  const labels = metrics.value.map(m => m.date.slice(5)).slice(-30)
  const netWorth = metrics.value.map(m => m.netWorth).slice(-30)

  return {
    labels,
    datasets: [
      {
        label: 'Net Worth',
        data: netWorth,
        borderColor: '#a855f7',
        backgroundColor: 'rgba(168, 85, 247, 0.1)',
        fill: true,
        tension: 0.4
      }
    ]
  }
})

// 格式化文件大小
const formatSize = (kb: number): string => {
  if (kb < 1024) return `${kb} KB`
  if (kb < 1024 * 1024) return `${(kb / 1024).toFixed(1)} MB`
  return `${(kb / (1024 * 1024)).toFixed(1)} GB`
}

// 语言图表数据
const languageChartData = computed(() => {
  if (githubLanguages.value.length === 0) {
    return {
      labels: [],
      datasets: []
    }
  }

  // 语言颜色映射
  const languageColors: Record<string, string> = {
    'JavaScript': '#f7df1e',
    'TypeScript': '#3178c6',
    'Python': '#3776ab',
    'Java': '#ed8b00',
    'C#': '#239120',
    'C++': '#00599c',
    'Go': '#00add8',
    'Rust': '#000000',
    'PHP': '#777bb4',
    'Ruby': '#cc342d',
    'Swift': '#fa7343',
    'Kotlin': '#7f52ff',
    'HTML': '#e34c26',
    'CSS': '#1572b6',
    'Vue': '#4fc08d',
    'React': '#61dafb',
    'Dart': '#0175c2',
    'Shell': '#89e051',
    'PowerShell': '#012456',
    'SQL': '#336791',
    'Markdown': '#083fa1'
  }

  const colors = githubLanguages.value.map((lang: any) => 
    languageColors[lang.language] || '#6b7280'
  )

  return {
    labels: githubLanguages.value.map((lang: any) => lang.language),
    datasets: [{
      label: '代码量',
      data: githubLanguages.value.map((lang: any) => parseFloat(lang.percentage)),
      backgroundColor: colors,
      borderColor: colors.map(c => c + '80'),
      borderWidth: 2
    }]
  }
})

const languageChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right' as const,
      labels: {
        color: '#9ca3af',
        padding: 15,
        font: { size: 12 }
      }
    },
    tooltip: {
      callbacks: {
        label: (context: any) => {
          const label = context.label || ''
          const value = context.parsed || 0
          const lang = githubLanguages.value[context.dataIndex]
          return `${label}: ${value.toFixed(1)}% (${formatSize(lang.bytes / 1024)})`
        }
      }
    }
  }
}

// 加载任务统计
const loadTaskStats = async () => {
  try {
    const res = await api.get('/Tasks/stats')
    if (res?.data) {
      taskStats.value = res.data
    }
  } catch (error) {
    console.error('加载任务统计失败:', error)
  }
}

// 加载年度目标
const loadGoals = async () => {
  goalsLoading.value = true
  try {
    const currentYear = new Date().getFullYear()
    const res = await api.get('/Goals', {
      params: {
        year: currentYear,
        status: 'active'
      }
    })
    if (res?.data?.Items) {
      activeGoals.value = res.data.Items.slice(0, 5) // 只显示前5个
    }
  } catch (error) {
    console.error('加载年度目标失败:', error)
  } finally {
    goalsLoading.value = false
  }
}

// 加载今日任务
const fetchTodayTasks = async () => {
  todayTasksLoading.value = true
  try {
    const today = new Date()
    const todayStart = new Date(today.getFullYear(), today.getMonth(), today.getDate())
    const todayEnd = new Date(todayStart)
    todayEnd.setDate(todayEnd.getDate() + 1)

    const res = await api.get('/Tasks', {
      params: {
        page: 1,
        pageSize: 10,
        status: 'pending,in_progress'
      }
    })
    
    if (res?.data?.Items) {
      // 筛选出今日任务（截止日期在今天或之前，且未完成）
      todayTasks.value = res.data.Items.filter((task: any) => {
        if (!task.dueDate) return false
        const dueDate = new Date(task.dueDate)
        return dueDate >= todayStart && dueDate < todayEnd && task.status !== 'completed'
      }).slice(0, 5) // 只显示前5个
    }
  } catch (error) {
    console.error('加载今日任务失败:', error)
  } finally {
    todayTasksLoading.value = false
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

const getProgressColor = (progress: number) => {
  if (progress === 100) return 'bg-green-500'
  if (progress >= 50) return 'bg-blue-500'
  if (progress >= 25) return 'bg-yellow-500'
  return 'bg-gray-400'
}

const getGoalProgressColor = (progress: number) => {
  if (progress === 100) return 'bg-gradient-to-r from-green-400 to-green-600'
  if (progress >= 50) return 'bg-gradient-to-r from-blue-400 to-blue-600'
  if (progress >= 25) return 'bg-gradient-to-r from-yellow-400 to-yellow-600'
  return 'bg-gradient-to-r from-gray-400 to-gray-500'
}

const formatTaskDate = (date: string) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getMonth() + 1}/${d.getDate()} ${d.getHours()}:${String(d.getMinutes()).padStart(2, '0')}`
}

// 加载 GitHub 统计数据
const loadGithubStats = async () => {
  // 从配置或环境变量获取 GitHub 仓库
  // 这里先使用一个示例仓库，实际应该从配置中心获取
  const defaultRepo = process.env.GITHUB_REPO || 'your-username/your-repo'
  
  if (!defaultRepo || defaultRepo === 'your-username/your-repo') {
    return // 如果没有配置，跳过
  }

  try {
    // 获取语言分布
    const languages = await api.get<any[]>(`/github/stats?repo=${defaultRepo}&type=languages`)
    if (languages && Array.isArray(languages)) {
      githubLanguages.value = languages
    }

    // 获取仓库统计
    const stats = await api.get<any>(`/github/stats?repo=${defaultRepo}&type=contributions`)
    if (stats) {
      githubStats.value = stats
    }
  } catch (e) {
    console.error('Failed to load GitHub stats:', e)
  }
}

onMounted(async () => {
  try {
    const res = await api.get<any[]>('/Metrics')
    // Sort by date ascending for charts
    metrics.value = res.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    
    // Format dates for display if needed, but charts use slice(5)
    // Ensure date is string like YYYY-MM-DD for slice to work expectedly or handle ISO
    metrics.value = metrics.value.map(m => ({
        ...m,
        date: m.date.split('T')[0]
    }))
    
    if (metrics.value.length > 0) {
      latest.value = metrics.value[metrics.value.length - 1]
    }

    // 加载任务统计和今日任务
    await loadTaskStats()
    await fetchTodayTasks()

    // 加载年度目标
    await loadGoals()

    // 加载 GitHub 统计数据
    await loadGithubStats()
  } catch (e) {
    console.error(e)
  }
})
</script>
