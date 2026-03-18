<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white)">éĄšçŽčŽżéŽçťčŽĄ</h1>
      <button @click="fetchStats" class="px-4 py-2 bg-blue-600 text-var(--color-bg-light, white) rounded hover:bg-blue-700 transition">
        ĺˇć°ć°ćŽ
      </button>
    </div>

    <!-- ćťä˝çťčŽĄ -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">ćťéĄšçŽć°</div>
        <div class="text-3xl font-bold text-blue-600 dark:text-blue-400">{{ stats.TotalProjects || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">ćťčŽżéŽé</div>
        <div class="text-3xl font-bold text-green-600 dark:text-green-400">{{ stats.TotalViews || 0 }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">ĺšłĺčŽżéŽé</div>
        <div class="text-3xl font-bold text-purple-600 dark:text-purple-400">
          {{ stats.TotalProjects > 0 ? Math.round((stats.TotalViews || 0) / stats.TotalProjects) : 0 }}
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- ç­é¨éĄšçŽćčĄ -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-4">ç­é¨éĄšçŽ Top 10</h2>
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
              <div class="font-semibold text-gray-900 dark:text-var(--color-bg-light, white)">{{ project.viewCount || 0 }}</div>
              <div class="text-xs text-gray-500 dark:text-gray-400">ćŹĄčŽżé</div>
            </div>
          </div>
          <div v-if="(!stats.TopProjects || stats.TopProjects.length === 0) && (!stats.topProjects || stats.topProjects.length === 0)" class="text-center text-gray-500 py-4">
            ćć ć°ćŽ
          </div>
        </div>
      </div>

      <!-- éĄšçŽčŽżéŽčśĺżĺžčĄ¨ -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-lg font-bold text-gray-800 dark:text-var(--color-bg-light, white)">čŽżéŽčśĺż</h2>
          <select v-model="selectedProjectId" @change="fetchTrends" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-1 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 text-sm">
            <option value="">éćŠéĄšçŽ</option>
            <option v-for="project in (stats.TopProjects || stats.topProjects || [])" :key="project.id" :value="project.id">
              {{ project.title }}
            </option>
          </select>
        </div>
        <template v-if="selectedProjectId && trends">
          <ClientOnly>
            <div class="h-64">
              <Line :data="chartData" :options="chartOptions" />
            </div>
            <template #fallback>
              <div class="h-64" />
            </template>
          </ClientOnly>
        </template>
        <div v-else class="h-64 flex items-center justify-center text-gray-500 dark:text-gray-400">
          请选择一个项目查看趋势
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineAsyncComponent } from 'vue'

// Chart.js 仅在客户端加载，避免 SSR 预渲染时的 CommonJS 兼容性问题
const Line = defineAsyncComponent(() =>
  import('vue-chartjs').then(async () => {
    await import('chart.js/auto')
    return (await import('vue-chartjs')).Line
  })
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
  if (!trends.value) {
    return {
      labels: [],
      datasets: []
    }
  }

  // ĺçŤŻčżĺçć°ćŽçťćďź{ Trends: [...] } ć?{ trends: [...] }
  const trendsData = trends.value.Trends || trends.value.trends || []
  
  if (trendsData.length === 0) {
    return {
      labels: [],
      datasets: []
    }
  }

  return {
    labels: trendsData.map((t: any) => {
      const date = t.Date || t.date
      // 如果是日期字符串，格式化为更友好的格式
      if (typeof date === 'string') {
        const d = new Date(date)
        return d.toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
      }
      return date
    }),
    datasets: [
      {
        label: '浏览量',
        data: trendsData.map((t: any) => t.Views || t.views || 0),
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
      // 默认选中第一个项目
      if (res.topProjects && res.topProjects.length > 0) {
        selectedProjectId.value = res.topProjects[0].id
        await fetchTrends()
      }
    }
  } catch (e: unknown) {
    handleError(e, 'čˇĺçťčŽĄĺ¤ąč´Ľ')
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
    handleError(e, 'čˇĺčśĺżć°ćŽĺ¤ąč´Ľ')
  }
}

onMounted(() => {
  fetchStats()
})
</script>

