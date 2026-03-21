<template>
  <div>
    <div class="mb-6 flex items-center justify-between">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white)">项目统计</h1>
      <button @click="fetchStats" class="rounded bg-blue-600 px-4 py-2 text-var(--color-bg-light, white) transition hover:bg-blue-700">
        刷新数据
      </button>
    </div>

    <div class="mb-6 grid grid-cols-1 gap-6 md:grid-cols-3">
      <div class="rounded-lg border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
        <div class="mb-1 text-sm text-gray-500 dark:text-gray-400">项目总数</div>
        <div class="text-3xl font-bold text-blue-600 dark:text-blue-400">{{ stats.TotalProjects || 0 }}</div>
      </div>
      <div class="rounded-lg border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
        <div class="mb-1 text-sm text-gray-500 dark:text-gray-400">总浏览量</div>
        <div class="text-3xl font-bold text-green-600 dark:text-green-400">{{ stats.TotalViews || 0 }}</div>
      </div>
      <div class="rounded-lg border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
        <div class="mb-1 text-sm text-gray-500 dark:text-gray-400">平均浏览量</div>
        <div class="text-3xl font-bold text-purple-600 dark:text-purple-400">
          {{ stats.TotalProjects > 0 ? Math.round((stats.TotalViews || 0) / stats.TotalProjects) : 0 }}
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <div class="rounded-lg border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
        <h2 class="mb-4 text-lg font-bold text-gray-800 dark:text-var(--color-bg-light, white)">热门项目 Top 10</h2>
        <div class="space-y-3">
          <div
            v-for="(project, index) in topProjects"
            :key="project.id"
            class="flex items-center justify-between rounded-lg bg-gray-50 p-3 transition-colors hover:bg-gray-100 dark:bg-gray-700 dark:hover:bg-gray-600"
          >
            <div class="flex flex-1 items-center gap-3">
              <div
                class="flex h-8 w-8 items-center justify-center rounded-full text-sm font-bold"
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
                <div class="mt-1 text-xs text-gray-500 dark:text-gray-400">
                  {{ project.status }}
                </div>
              </div>
            </div>
            <div class="text-right">
              <div class="font-semibold text-gray-900 dark:text-var(--color-bg-light, white)">{{ project.viewCount || 0 }}</div>
              <div class="text-xs text-gray-500 dark:text-gray-400">浏览量</div>
            </div>
          </div>
          <div v-if="topProjects.length === 0" class="py-4 text-center text-gray-500">
            暂无统计数据
          </div>
        </div>
      </div>

      <div class="rounded-lg border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="text-lg font-bold text-gray-800 dark:text-var(--color-bg-light, white)">浏览趋势</h2>
          <select v-model="selectedProjectId" @change="fetchTrends" class="rounded border border-gray-300 bg-white px-3 py-1 text-sm text-gray-800 dark:border-gray-600 dark:bg-gray-900 dark:text-gray-200">
            <option value="">选择项目</option>
            <option v-for="project in topProjects" :key="project.id" :value="project.id">
              {{ project.title }}
            </option>
          </select>
        </div>
        <template v-if="selectedProjectId && trendPoints.length > 0">
          <ClientOnly>
            <div class="h-64">
              <ChartsAppEChart :option="chartOption" loading-text="图表加载中..." />
            </div>
            <template #fallback>
              <div class="h-64" />
            </template>
          </ClientOnly>
        </template>
        <div v-else class="flex h-64 items-center justify-center text-gray-500 dark:text-gray-400">
          请选择一个项目查看趋势
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

type ProjectSummary = {
  id: string | number
  title: string
  status?: string
  viewCount?: number
}

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

const topProjects = computed<ProjectSummary[]>(() => stats.value.TopProjects || stats.value.topProjects || [])

const trendPoints = computed(() => trends.value?.Trends || trends.value?.trends || [])

const chartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  grid: { left: 12, right: 12, top: 24, bottom: 16, containLabel: true },
  xAxis: {
    type: 'category',
    boundaryGap: false,
    data: trendPoints.value.map((item: any) => {
      const rawDate = item.Date || item.date
      if (typeof rawDate !== 'string') return rawDate
      return new Date(rawDate).toLocaleDateString('zh-CN', { month: 'short', day: 'numeric' })
    }),
    axisLine: { show: false },
    axisTick: { show: false }
  },
  yAxis: {
    type: 'value',
    minInterval: 1
  },
  series: [
    {
      name: '浏览量',
      type: 'line',
      smooth: true,
      symbol: 'none',
      lineStyle: { color: 'rgb(59, 130, 246)', width: 3 },
      areaStyle: { color: 'rgba(59, 130, 246, 0.1)' },
      data: trendPoints.value.map((item: any) => item.Views || item.views || 0)
    }
  ]
}))

const fetchStats = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/Projects/stats')
    if (res) {
      stats.value = res
      if (!selectedProjectId.value && topProjects.value.length > 0) {
        selectedProjectId.value = String(topProjects.value[0].id)
      }

      if (selectedProjectId.value) {
        await fetchTrends()
      }
    }
  } catch (e: unknown) {
    handleError(e, '获取项目统计失败')
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
    handleError(e, '获取浏览趋势失败')
  }
}

onMounted(() => {
  fetchStats()
})
</script>
