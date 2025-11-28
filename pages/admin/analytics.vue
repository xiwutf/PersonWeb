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

const refreshStats = () => {
  fetchStats()
}

onMounted(() => {
  fetchStats()
  // 每30秒自动刷新
  setInterval(fetchStats, 30000)
})
</script>

