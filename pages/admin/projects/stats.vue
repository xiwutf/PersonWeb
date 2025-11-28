<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">项目访问统计</h1>
      <button @click="fetchStats" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition">
        刷新数据
      </button>
    </div>

    <!-- 总体统计 -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总项目数</div>
        <div class="text-3xl font-bold text-blue-600 dark:text-blue-400">{{ stats.TotalProjects || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总访问量</div>
        <div class="text-3xl font-bold text-green-600 dark:text-green-400">{{ stats.TotalViews || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">平均访问量</div>
        <div class="text-3xl font-bold text-purple-600 dark:text-purple-400">
          {{ stats.TotalProjects > 0 ? Math.round((stats.TotalViews || 0) / stats.TotalProjects) : 0 }}
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- 热门项目排行 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">热门项目 Top 10</h2>
        <div class="space-y-3">
          <div
            v-for="(project, index) in (stats.TopProjects || stats.topProjects || [])"
            :key="project.id"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
          >
            <div class="flex items-center gap-3 flex-1">
              <div
                class="w-8 h-8 rounded-full flex items-center justify-center font-bold text-sm"
                :class="{
                  'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200': index === 0,
                  'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200': index === 1,
                  'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200': index === 2,
                  'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200': index > 2
                }"
              >
                {{ index + 1 }}
              </div>
              <div class="flex-1">
                <div class="font-medium text-gray-800 dark:text-gray-200">{{ project.title }}</div>
                <div class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                  {{ project.status }}
                </div>
              </div>
            </div>
            <div class="text-right">
              <div class="font-semibold text-gray-900 dark:text-white">{{ project.viewCount || 0 }}</div>
              <div class="text-xs text-gray-500 dark:text-gray-400">次访问</div>
            </div>
          </div>
          <div v-if="(!stats.TopProjects || stats.TopProjects.length === 0) && (!stats.topProjects || stats.topProjects.length === 0)" class="text-center text-gray-500 py-4">
            暂无数据
          </div>
        </div>
      </div>

      <!-- 项目访问趋势图表 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-lg font-bold text-gray-800 dark:text-white">访问趋势</h2>
          <select v-model="selectedProjectId" @change="fetchTrends" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-1 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 text-sm">
            <option value="">选择项目</option>
            <option v-for="project in (stats.TopProjects || stats.topProjects || [])" :key="project.id" :value="project.id">
              {{ project.title }}
            </option>
          </select>
        </div>
        <div v-if="selectedProjectId && trends" class="h-64">
          <Line :data="chartData" :options="chartOptions" />
        </div>
        <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
          请选择一个项目查看访问趋势
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
  Title,
  Tooltip,
  Legend
} from 'chart.js'
import { Line } from 'vue-chartjs'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
)

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { handleError } = useErrorHandler()

const stats = ref<any>({
  TotalProjects: 0,
  TotalViews: 0,
  TopProjects: []
})

const selectedProjectId = ref('')
const trends = ref<any>(null)
const loading = ref(true)

const chartData = computed(() => {
  if (!trends.value || !trends.value.trends) {
    return {
      labels: [],
      datasets: []
    }
  }

  return {
    labels: trends.value.trends.map((t: any) => t.Date || t.date),
    datasets: [
      {
        label: '访问量',
        data: trends.value.trends.map((t: any) => t.Views || t.views),
        borderColor: 'rgb(59, 130, 246)',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        tension: 0.4,
        fill: true
      }
    ]
  }
})

const chartOptions = {
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

const fetchStats = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/Projects/stats')
    if (res) {
      stats.value = res
      // 默认选择第一个项目
      if (res.topProjects && res.topProjects.length > 0) {
        selectedProjectId.value = res.topProjects[0].id
        await fetchTrends()
      }
    }
  } catch (e: unknown) {
    handleError(e, '获取统计失败')
  } finally {
    loading.value = false
  }
}

const fetchTrends = async () => {
  if (!selectedProjectId.value) return

  try {
    const res = await api.get<any>(`/Projects/${selectedProjectId.value}/trends`, {
      params: { days: 30 }
    })
    if (res) {
      trends.value = res
    }
  } catch (e: unknown) {
    handleError(e, '获取趋势数据失败')
  }
}

onMounted(() => {
  fetchStats()
})
</script>

