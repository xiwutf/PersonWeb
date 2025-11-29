<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">访客分析</h1>
      <button @click="refreshStats" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
        刷新数据
      </button>
    </div>

    <!-- 核心指标 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">今日 PV</div>
        <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">{{ stats.Today?.Pv || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">
          昨日: {{ stats.Yesterday?.Pv || 0 }}
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">今日 UV</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats.Today?.Uv || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">
          昨日: {{ stats.Yesterday?.Uv || 0 }}
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">在线人数</div>
        <div class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ stats.OnlineCount || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">最近5分钟活跃</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">热门文章数</div>
        <div class="text-2xl font-bold text-purple-600 dark:text-purple-400">{{ stats.TopArticles?.length || 0 }}</div>
        <div class="text-xs text-gray-500 mt-1">Top 10</div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- 热门文章 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">热门文章</h2>
        <div class="space-y-3">
          <div
            v-for="(article, index) in stats.TopArticles"
            :key="index"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
          >
            <div class="flex-1">
              <div class="font-medium text-gray-800 dark:text-gray-200 text-sm truncate">
                {{ article.Path }}
              </div>
              <div class="text-xs text-gray-500 mt-1">{{ article.Views }} 次浏览</div>
            </div>
            <div class="w-8 h-8 rounded-full bg-blue-100 dark:bg-blue-900 flex items-center justify-center text-blue-600 dark:text-blue-400 font-bold text-sm">
              {{ index + 1 }}
            </div>
          </div>
          <div v-if="!stats.TopArticles || stats.TopArticles.length === 0" class="text-center text-gray-500 py-4">
            暂无数据
          </div>
        </div>
      </div>

      <!-- 访问区域 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">访问区域</h2>
        <div class="space-y-3">
          <div
            v-for="(region, index) in stats.RegionStats"
            :key="index"
            class="flex items-center justify-between"
          >
            <div class="flex-1">
              <div class="font-medium text-gray-800 dark:text-gray-200">
                {{ region.Country || '未知' }}
              </div>
            </div>
            <div class="flex items-center gap-2">
              <div class="w-32 bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                <div
                  class="bg-blue-600 h-2 rounded-full"
                  :style="{ width: `${(region.Count / maxRegionCount) * 100}%` }"
                ></div>
              </div>
              <span class="text-sm font-semibold text-gray-600 dark:text-gray-400 w-12 text-right">
                {{ region.Count }}
              </span>
            </div>
          </div>
          <div v-if="!stats.RegionStats || stats.RegionStats.length === 0" class="text-center text-gray-500 py-4">
            暂无数据
          </div>
        </div>
      </div>

      <!-- 搜索来源 -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white mb-4">搜索来源</h2>
        <div class="space-y-2">
          <div
            v-for="(source, index) in stats.SearchSources"
            :key="index"
            class="flex items-center justify-between p-2 bg-gray-50 dark:bg-gray-700 rounded"
          >
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ source.Keyword }}</span>
            <span class="text-xs text-gray-500">{{ source.Count }} 次</span>
          </div>
          <div v-if="!stats.SearchSources || stats.SearchSources.length === 0" class="text-center text-gray-500 py-4">
            暂无搜索数据
          </div>
        </div>
      </div>
    </div>

    <!-- 访客列表 -->
    <div class="mt-6 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-bold text-gray-800 dark:text-white">访客列表</h2>
        <div class="flex items-center gap-4">
          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
            <input
              type="checkbox"
              v-model="onlineOnly"
              @change="fetchVisitors"
              class="rounded"
            />
            仅显示在线访客
          </label>
          <button
            @click="fetchVisitors"
            class="px-3 py-1 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600"
          >
            刷新列表
          </button>
        </div>
      </div>

      <div v-if="visitorsLoading" class="text-center py-8 text-gray-500">
        加载中...
      </div>
      <div v-else-if="visitors.length === 0" class="text-center py-8 text-gray-500">
        暂无访客数据
      </div>
      <div v-else class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">访客ID</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">IP地址</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">地理位置</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">设备信息</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">当前页面</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">浏览量</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">最后活跃</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">状态</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr
              v-for="visitor in visitors"
              :key="visitor.Id"
              class="hover:bg-gray-50 dark:hover:bg-gray-700"
            >
              <td class="px-4 py-3 text-gray-900 dark:text-gray-100 font-mono text-xs">
                {{ visitor.VisitorId?.substring(0, 8) }}...
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300 font-mono text-xs">
                {{ visitor.Ip || '-' }}
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                <div class="text-xs">
                  <div v-if="visitor.Country">{{ visitor.Country }}</div>
                  <div v-if="visitor.Region" class="text-gray-500">{{ visitor.Region }}</div>
                  <div v-if="visitor.City" class="text-gray-500">{{ visitor.City }}</div>
                  <div v-if="!visitor.Country && !visitor.Region && !visitor.City" class="text-gray-400">未知</div>
                </div>
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                <div class="text-xs">
                  <div>{{ visitor.DeviceType || '-' }}</div>
                  <div class="text-gray-500">{{ visitor.Browser || '-' }} / {{ visitor.Os || '-' }}</div>
                </div>
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300">
                <div class="text-xs max-w-xs truncate" :title="visitor.Path">
                  {{ visitor.Path || '-' }}
                </div>
                <div v-if="visitor.SearchKeyword" class="text-xs text-blue-600 dark:text-blue-400 mt-1">
                  搜索: {{ visitor.SearchKeyword }}
                </div>
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300 text-center">
                {{ visitor.PageViews || 0 }}
              </td>
              <td class="px-4 py-3 text-gray-700 dark:text-gray-300 text-xs">
                {{ formatTime(visitor.UpdatedAt) }}
              </td>
              <td class="px-4 py-3">
                <span
                  v-if="visitor.IsOnline"
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200"
                >
                  <span class="w-1.5 h-1.5 bg-green-500 rounded-full mr-1"></span>
                  在线
                </span>
                <span
                  v-else
                  class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300"
                >
                  离线
                </span>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- 分页 -->
        <div v-if="visitorsTotal > pageSize" class="mt-4 flex items-center justify-between">
          <div class="text-sm text-gray-500">
            共 {{ visitorsTotal }} 条记录
          </div>
          <div class="flex gap-2">
            <button
              @click="changePage(visitorsPage - 1)"
              :disabled="visitorsPage <= 1"
              class="px-3 py-1 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一页
            </button>
            <span class="px-3 py-1 text-sm text-gray-700 dark:text-gray-300">
              第 {{ visitorsPage }} / {{ Math.ceil(visitorsTotal / pageSize) }} 页
            </span>
            <button
              @click="changePage(visitorsPage + 1)"
              :disabled="visitorsPage >= Math.ceil(visitorsTotal / pageSize)"
              class="px-3 py-1 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              下一页
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const stats = ref<any>({
  Today: { Pv: 0, Uv: 0 },
  Yesterday: { Pv: 0, Uv: 0 },
  OnlineCount: 0,
  TopArticles: [],
  RegionStats: [],
  SearchSources: []
})

const visitors = ref<any[]>([])
const visitorsLoading = ref(false)
const visitorsPage = ref(1)
const visitorsTotal = ref(0)
const pageSize = ref(20)
const onlineOnly = ref(false)

const maxRegionCount = computed(() => {
  if (!stats.value.RegionStats || stats.value.RegionStats.length === 0) return 1
  return Math.max(...stats.value.RegionStats.map((r: any) => r.Count))
})

const fetchStats = async () => {
  try {
    const res = await api.get<any>('/Analytics/stats')
    if (res) {
      stats.value = res
    }
  } catch (e) {
    console.error('Failed to fetch analytics:', e)
  }
}

const fetchVisitors = async () => {
  try {
    visitorsLoading.value = true
    const res = await api.get<any>('/Analytics/visitors', {
      params: {
        page: visitorsPage.value,
        pageSize: pageSize.value,
        onlineOnly: onlineOnly.value
      }
    })
    if (res) {
      visitors.value = res.Visitors || []
      visitorsTotal.value = res.Total || 0
    }
  } catch (e) {
    console.error('Failed to fetch visitors:', e)
  } finally {
    visitorsLoading.value = false
  }
}

const changePage = (page: number) => {
  visitorsPage.value = page
  fetchVisitors()
}

const formatTime = (timeStr: string) => {
  if (!timeStr) return '-'
  const date = new Date(timeStr)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  
  if (minutes < 1) return '刚刚'
  if (minutes < 60) return `${minutes}分钟前`
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `${hours}小时前`
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days}天前`
  
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const refreshStats = () => {
  fetchStats()
  fetchVisitors()
}

onMounted(() => {
  fetchStats()
  fetchVisitors()
  // 每30秒自动刷新
  setInterval(() => {
    fetchStats()
    if (onlineOnly.value) {
      fetchVisitors()
    }
  }, 30000)
})
</script>

