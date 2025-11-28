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

    // 加载 GitHub 统计数据
    await loadGithubStats()
  } catch (e) {
    console.error(e)
  }
})
</script>
