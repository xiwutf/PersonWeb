<template>
  <div class="dashboard-page min-h-screen py-12 px-4 sm:px-6 lg:px-8 font-['Outfit']" style="background-color: var(--color-bg-body); color: var(--color-text-main);">
    <div class="max-w-7xl mx-auto">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold mb-2" style="color: var(--color-primary);">
          数字孪生仪表盘
        </h1>
        <p class="text-sm" style="color: var(--color-text-muted);">量化自我与生活指标 · 记录和追踪个人数据，帮助了解自己的生活状态和成长轨迹</p>
      </div>

      <!-- 核心指标卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <!-- Life Battery -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 dashboard-card__icon--green" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="dashboard-card__label text-sm mb-2">生活电量</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold dashboard-card__value">{{ latest.energy }}%</span>
            <span class="text-sm mb-1 dashboard-card__accent--green">精力值</span>
          </div>
          <div class="dashboard-card__track w-full rounded-full h-2 mt-4">
            <div class="dashboard-card__fill--green h-2 rounded-full transition-all duration-1000" :style="{ width: latest.energy + '%' }"></div>
          </div>
          <p class="dashboard-card__hint text-xs mt-2">睡眠: {{ latest.sleep }}小时</p>
        </div>

        <!-- Activity -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 dashboard-card__icon--orange" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="dashboard-card__label text-sm mb-2">日常活动</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold dashboard-card__value">{{ latest.steps }}</span>
            <span class="text-sm mb-1 dashboard-card__accent--orange">步数</span>
          </div>
          <div class="dashboard-card__track w-full rounded-full h-2 mt-4">
            <div class="dashboard-card__fill--orange h-2 rounded-full transition-all duration-1000" :style="{ width: Math.min((latest.steps / 10000) * 100, 100) + '%' }"></div>
          </div>
          <p class="dashboard-card__hint text-xs mt-2">目标: 10,000 步</p>
        </div>

        <!-- Physique -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 dashboard-card__icon--blue" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="dashboard-card__label text-sm mb-2">身体状况</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold dashboard-card__value">{{ latest.weight }}</span>
            <span class="text-sm mb-1 dashboard-card__accent--blue">公斤</span>
          </div>
          <div class="mt-4">
             <span class="dashboard-card__badge--blue text-xs px-2 py-1 rounded">BMI: {{ (latest.weight / (1.75 * 1.75)).toFixed(1) }}</span>
          </div>
        </div>

        <!-- Net Worth -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 dashboard-card__icon--purple" fill="currentColor" viewBox="0 0 20 20"><path d="M4 4a2 2 0 00-2 2v1h16V6a2 2 0 00-2-2H4z"></path><path fill-rule="evenodd" d="M18 9H2v5a2 2 0 002 2h12a2 2 0 002-2V9zM4 13a1 1 0 011-1h1a1 1 0 110 2H5a1 1 0 01-1-1zm5-1a1 1 0 100 2h1a1 1 0 100-2H9z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="dashboard-card__label text-sm mb-2">净资产</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold dashboard-card__value">¥{{ (latest.netWorth / 10000).toFixed(1) }}</span>
            <span class="text-sm mb-1 dashboard-card__accent--purple">万元</span>
          </div>
          <div class="mt-4 flex items-center text-xs dashboard-card__accent--green">
            <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path></svg>
            <span>增长中</span>
          </div>
        </div>
      </div>

      <!-- 图表区域 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- 趋势图 -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg">
          <h3 class="dashboard-section-title text-lg font-bold mb-6">健康趋势</h3>
          <div class="h-64">
            <Line :data="healthChartData" :options="chartOptions" />
          </div>
        </div>

        <!-- 财富趋势 -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg">
          <h3 class="dashboard-section-title text-lg font-bold mb-6">财富增长</h3>
          <div class="h-64">
            <Line :data="wealthChartData" :options="chartOptions" />
          </div>
        </div>
      </div>

      <!-- 任务统计 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- 任务统计卡片 -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg">
          <div class="flex items-center justify-between mb-6">
            <h3 class="dashboard-section-title text-lg font-bold">任务统计</h3>
            <a 
              v-if="isAuthenticated"
              href="/admin/tasks" 
              class="dashboard-link text-sm"
              @click.prevent="router.push('/admin/tasks')"
            >
              管理任务 →
            </a>
            <a 
              v-else
              href="#" 
              class="dashboard-link--muted text-sm cursor-not-allowed"
              @click.prevent="handleUnauthorizedClick"
              title="需要登录才能访问"
            >
              管理任务 →
            </a>
          </div>
          <div v-if="taskStats" class="grid grid-cols-2 gap-4">
            <div class="dashboard-stat-card dashboard-stat-card--yellow rounded-xl p-4">
              <div class="dashboard-stat-label text-sm mb-1">待处理</div>
              <div class="dashboard-stat-value text-2xl font-bold">{{ taskStats.Pending || 0 }}</div>
            </div>
            <div class="dashboard-stat-card dashboard-stat-card--blue rounded-xl p-4">
              <div class="dashboard-stat-label text-sm mb-1">进行中</div>
              <div class="dashboard-stat-value text-2xl font-bold">{{ taskStats.InProgress || 0 }}</div>
            </div>
            <div class="dashboard-stat-card dashboard-stat-card--green rounded-xl p-4">
              <div class="dashboard-stat-label text-sm mb-1">已完成</div>
              <div class="dashboard-stat-value text-2xl font-bold">{{ taskStats.Completed || 0 }}</div>
            </div>
            <div class="dashboard-stat-card dashboard-stat-card--red rounded-xl p-4">
              <div class="dashboard-stat-label text-sm mb-1">已逾期</div>
              <div class="dashboard-stat-value text-2xl font-bold">{{ taskStats.Overdue || 0 }}</div>
            </div>
          </div>
          <div v-else class="text-center py-8 dashboard-hint">加载中...</div>
        </div>

        <!-- 年度目标卡片 -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg">
          <div class="flex items-center justify-between mb-6">
            <h3 class="dashboard-section-title text-lg font-bold">年度目标</h3>
            <a 
              v-if="isAuthenticated"
              href="/admin/goals" 
              class="dashboard-link text-sm"
              @click.prevent="router.push('/admin/goals')"
            >
              管理目标 →
            </a>
            <a 
              v-else
              href="#" 
              class="dashboard-link--muted text-sm cursor-not-allowed"
              @click.prevent="handleUnauthorizedClick"
              title="需要登录才能访问"
            >
              管理目标 →
            </a>
          </div>
          <div v-if="goalsLoading" class="text-center py-8 dashboard-hint">加载中...</div>
          <div v-else-if="activeGoals.length === 0" class="text-center py-8 dashboard-hint">
            <p class="mb-4">暂无年度目标</p>
            <a 
              v-if="isAuthenticated"
              href="/admin/goals" 
              class="dashboard-link text-sm"
              @click.prevent="router.push('/admin/goals')"
            >
              创建目标
            </a>
            <a 
              v-else
              href="#" 
              class="dashboard-link--muted text-sm cursor-not-allowed"
              @click.prevent="handleUnauthorizedClick"
              title="需要登录才能访问"
            >
              创建目标
            </a>
          </div>
          <div v-else class="space-y-4 max-h-64 overflow-y-auto">
            <div v-for="goal in activeGoals" :key="goal.id" class="dashboard-goal-item p-4 rounded-xl">
              <div class="flex items-start justify-between mb-2">
                <h4 class="font-semibold text-sm dashboard-card__value">{{ goal.title }}</h4>
                <span class="text-xs dashboard-card__hint">{{ goal.year }}年</span>
              </div>
              <div v-if="goal.targetValue" class="text-xs dashboard-card__hint mb-2">
                {{ goal.currentValue }}{{ goal.unit || '' }} / {{ goal.targetValue }}{{ goal.unit || '' }}
              </div>
              <div class="dashboard-card__track w-full rounded-full h-2 mb-2">
                <div 
                  :class="getGoalProgressColor(goal.progress)"
                  class="h-2 rounded-full transition-all duration-300"
                  :style="{ width: Math.min(goal.progress, 100) + '%' }"
                ></div>
              </div>
              <div class="text-xs dashboard-card__hint text-right">{{ goal.progress }}%</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 今日任务列表 -->
      <div class="dashboard-card rounded-2xl p-6 shadow-lg mb-8">
        <div class="flex items-center justify-between mb-6">
          <h3 class="dashboard-section-title text-lg font-bold">今日任务</h3>
          <button type="button" @click="fetchTodayTasks" class="dashboard-link text-sm">刷新</button>
        </div>
        <div v-if="todayTasksLoading" class="text-center py-8 dashboard-hint">加载中...</div>
        <div v-else-if="todayTasks.length === 0" class="text-center py-8 dashboard-hint">暂无任务</div>
        <div v-else class="space-y-3 max-h-64 overflow-y-auto">
          <div v-for="task in todayTasks" :key="task.id" class="dashboard-task-item p-3 rounded-lg">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <h4 class="font-medium dashboard-card__value">{{ task.title }}</h4>
                  <span :class="getStatusClass(task.status)" class="px-2 py-0.5 rounded text-xs">
                    {{ getStatusText(task.status) }}
                  </span>
                </div>
                <div class="flex items-center gap-4 text-xs dashboard-card__hint">
                  <span>进度: {{ task.progress }}%</span>
                  <span v-if="task.dueDate">截止: {{ formatTaskDate(task.dueDate) }}</span>
                </div>
                <div class="dashboard-card__track mt-2 w-full rounded-full h-1.5">
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

      <!-- GitHub 代码统计 -->
      <div v-if="githubStats" class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- 代码语言分布 -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg">
          <h3 class="dashboard-section-title text-lg font-bold mb-6">代码语言分布</h3>
          <div v-if="githubLanguages.length > 0" class="h-64">
            <Doughnut :data="languageChartData" :options="languageChartOptions" />
          </div>
          <div v-else class="h-64 flex items-center justify-center dashboard-hint">
            暂无数据
          </div>
        </div>

        <!-- GitHub 贡献统计 -->
        <div class="dashboard-card rounded-2xl p-6 shadow-lg">
          <h3 class="text-lg font-bold mb-6 dashboard-section-title">仓库统计</h3>
          <div v-if="githubStats" class="space-y-4">
            <div class="stat-grid">
              <div class="stat-card stat-card--blue">
                <div class="stat-label">星标数</div>
                <div class="stat-value">{{ githubStats.stars || 0 }}</div>
              </div>
              <div class="stat-card stat-card--green">
                <div class="stat-label">Fork数</div>
                <div class="stat-value">{{ githubStats.forks || 0 }}</div>
              </div>
              <div class="stat-card stat-card--purple">
                <div class="stat-label">关注者</div>
                <div class="stat-value">{{ githubStats.watchers || 0 }}</div>
              </div>
              <div class="stat-card stat-card--orange">
                <div class="stat-label">代码量</div>
                <div class="stat-value">{{ formatSize(githubStats.size || 0) }}</div>
              </div>
            </div>
          </div>
          <div v-else class="h-64 flex items-center justify-center dashboard-hint">
            暂无数据
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* 使用主题 CSS 变量，避免硬编码颜色 */
.dashboard-card {
  background-color: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
}
.dashboard-card__label,
.dashboard-card__hint {
  color: var(--color-text-muted);
}
.dashboard-card__value {
  color: var(--color-text-main);
}
.dashboard-card__track {
  background-color: var(--color-border-subtle);
}
.dashboard-card__icon--green,
.dashboard-card__accent--green,
.dashboard-card__fill--green { color: var(--chart-secondary, var(--color-success)); }
.dashboard-card__fill--green { background: linear-gradient(to right, var(--chart-secondary, var(--color-success)), var(--chart-octonary, var(--color-lime-500))); }
.dashboard-card__icon--orange,
.dashboard-card__accent--orange,
.dashboard-card__fill--orange { color: var(--chart-nonary, var(--color-orange-500)); }
.dashboard-card__fill--orange { background: linear-gradient(to right, var(--chart-nonary, var(--color-orange-500)), var(--chart-quaternary, var(--color-danger))); }
.dashboard-card__icon--blue,
.dashboard-card__accent--blue { color: var(--chart-primary, var(--color-primary)); }
.dashboard-card__badge--blue {
  background-color: var(--color-primary-soft, var(--color-blue-100));
  color: var(--color-primary, var(--color-primary-hover));
}
.dashboard-card__icon--purple,
.dashboard-card__accent--purple { color: var(--chart-quinary, var(--color-purple-500)); }
.dashboard-section-title { color: var(--color-text-main); }
.dashboard-link { color: var(--color-primary); }
.dashboard-link:hover { color: var(--color-primary-hover); }
.dashboard-link--muted { color: var(--color-text-disabled); }
.dashboard-hint { color: var(--color-text-muted); }
.dashboard-stat-label { color: var(--color-text-muted); }
.dashboard-stat-value { color: var(--color-text-main); }
.dashboard-stat-card--yellow { background-color: var(--color-bg-elevated); border-left: 4px solid var(--chart-tertiary); }
.dashboard-stat-card--blue { background-color: var(--color-bg-elevated); border-left: 4px solid var(--chart-primary); }
.dashboard-stat-card--green { background-color: var(--color-bg-elevated); border-left: 4px solid var(--chart-secondary); }
.dashboard-stat-card--red { background-color: var(--color-bg-elevated); border-left: 4px solid var(--chart-quaternary); }
.dashboard-goal-item {
  background-color: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
}
.dashboard-task-item { background-color: var(--color-bg-elevated); }
</style>

<script setup lang="ts">
// 使用默认布局（包含顶部导航栏）
definePageMeta({
  layout: 'default'
})

import { ref, computed, onMounted, watch } from 'vue'
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
const router = useRouter()

// 检查用户是否已登录
const isAuthenticated = computed(() => {
  if (process.client) {
    const token = localStorage.getItem('admin_token')
    const user = localStorage.getItem('admin_user')
    return !!(token && user)
  }
  return false
})

// 处理未授权点击
const handleUnauthorizedClick = () => {
  if (process.client) {
    if (confirm('需要登录才能访问后台管理功能，是否前往登录页面？')) {
      router.push('/admin/login')
    }
  }
}

const metrics = ref<any[]>([])
// 保留模拟数据作为注释，方便参考
// const mockMetrics = [
//   { date: '2024-01-01', steps: 8500, sleep: 7.5, weight: 70, netWorth: 50000, energy: 75 },
//   { date: '2024-01-02', steps: 9200, sleep: 8.0, weight: 70, netWorth: 50200, energy: 80 },
//   { date: '2024-01-03', steps: 7800, sleep: 7.0, weight: 70.2, netWorth: 50500, energy: 70 },
//   { date: '2024-01-04', steps: 10500, sleep: 8.5, weight: 70.1, netWorth: 50800, energy: 85 },
//   { date: '2024-01-05', steps: 9800, sleep: 7.5, weight: 70, netWorth: 51000, energy: 78 },
//   { date: '2024-01-06', steps: 12000, sleep: 8.0, weight: 69.8, netWorth: 51200, energy: 90 },
//   { date: '2024-01-07', steps: 11000, sleep: 8.5, weight: 69.9, netWorth: 51500, energy: 88 }
// ]

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

// 从主题 CSS 变量读取图表颜色，随主题切换生效
const { currentTheme } = useTheme()
const chartTheme = ref({
  text: 'var(--color-text-sec)',
  grid: 'var(--color-text-sub)',
  chartPrimary: 'var(--color-primary)',
  chartSecondary: 'var(--color-success)',
  chartNonary: 'var(--color-orange-500)',
  chartQuinary: 'var(--color-purple-500)'
})

function getCssVar(name: string): string {
  if (typeof document === 'undefined') return ''
  return getComputedStyle(document.documentElement).getPropertyValue(name).trim()
}

function hexToRgba(hex: string, alpha: number): string {
  if (!hex || hex.startsWith('rgba')) return hex
  const h = hex.replace('#', '')
  if (h.length !== 6) return hex
  const r = parseInt(h.slice(0, 2), 16)
  const g = parseInt(h.slice(2, 4), 16)
  const b = parseInt(h.slice(4, 6), 16)
  return `rgba(${r},${g},${b},${alpha})`
}

function updateChartTheme() {
  if (typeof document === 'undefined') return
  chartTheme.value = {
    text: getCssVar('--color-text-muted') || 'var(--color-text-sec)',
    grid: getCssVar('--color-border-default') || getCssVar('--color-border-subtle') || 'var(--color-text-sub)',
    chartPrimary: getCssVar('--chart-primary') || 'var(--color-primary)',
    chartSecondary: getCssVar('--chart-secondary') || 'var(--color-success)',
    chartNonary: getCssVar('--chart-nonary') || 'var(--color-orange-500)',
    chartQuinary: getCssVar('--chart-quinary') || 'var(--color-purple-500)'
  }
}

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
      labels: { color: chartTheme.value.text }
    }
  },
  scales: {
    y: {
      grid: { color: chartTheme.value.grid },
      ticks: { color: chartTheme.value.text }
    },
    x: {
      grid: { display: false },
      ticks: { color: chartTheme.value.text }
    }
  },
  interaction: {
    mode: 'index',
    intersect: false,
  },
}))

const healthChartData = computed(() => {
  const labels = metrics.value.map(m => m.date.slice(5)).slice(-30) // Last 30 days
  const steps = metrics.value.map(m => m.steps).slice(-30)
  const weight = metrics.value.map(m => m.weight).slice(-30)
  const t = chartTheme.value

  return {
    labels,
    datasets: [
      {
        label: '步数',
        data: steps,
        borderColor: t.chartNonary,
        backgroundColor: hexToRgba(t.chartNonary, 0.1),
        yAxisID: 'y',
        fill: true,
        tension: 0.4
      },
      {
        label: '体重 (公斤)',
        data: weight,
        borderColor: t.chartPrimary,
        backgroundColor: hexToRgba(t.chartPrimary, 0.1),
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
  const t = chartTheme.value

  return {
    labels,
    datasets: [
      {
        label: '净资产',
        data: netWorth,
        borderColor: t.chartQuinary,
        backgroundColor: hexToRgba(t.chartQuinary, 0.1),
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
    'Rust': 'var(--color-bg-dark, black)',
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
    languageColors[lang.language] || 'var(--color-text-sec)'
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

const languageChartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right' as const,
      labels: {
        color: chartTheme.value.text,
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
}))

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

watch(currentTheme, () => {
  updateChartTheme()
}, { flush: 'post' })

onMounted(async () => {
  updateChartTheme()
  try {
    // 优先从新的MockData API获取数据
    const mockRes = await api.get<any[]>('/MockData/dashboard-metrics?days=30')
    if (mockRes && Array.isArray(mockRes) && mockRes.length > 0) {
      metrics.value = mockRes.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
      if (metrics.value.length > 0) {
        latest.value = metrics.value[metrics.value.length - 1]
      }
    } else {
      // 如果新API没有数据，尝试从旧API获取
      const res = await api.get<any[]>('/Metrics')
      if (res && Array.isArray(res) && res.length > 0) {
        metrics.value = res.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        metrics.value = metrics.value.map(m => ({
          ...m,
          date: m.date.split('T')[0]
        }))
        if (metrics.value.length > 0) {
          latest.value = metrics.value[metrics.value.length - 1]
        }
      }
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
