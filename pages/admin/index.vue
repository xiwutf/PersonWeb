<template>
  <div>
    <header class="flex justify-between items-center mb-8">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">仪表盘</h1>
      <div class="text-gray-600 dark:text-gray-400">欢迎回来，Admin</div>
    </header>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-gray-500 dark:text-gray-400 text-sm mb-2">总文章数</div>
        <div class="text-3xl font-bold text-blue-600 dark:text-blue-400">{{ stats.articleCount || '-' }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-gray-500 dark:text-gray-400 text-sm mb-2">总工具数</div>
        <div class="text-3xl font-bold text-purple-600 dark:text-purple-400">{{ stats.toolCount || '-' }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="text-gray-500 dark:text-gray-400 text-sm mb-2">今日访问</div>
        <div class="text-3xl font-bold text-green-600 dark:text-green-400">{{ stats.todayVisits || '-' }}</div>
      </div>
    </div>

    <!-- 快捷操作 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h2 class="text-lg font-bold mb-4 text-gray-800 dark:text-white">快捷操作</h2>
      <div class="flex gap-4">
        <NuxtLink to="/admin/articles/edit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
          + 发布新文章
        </NuxtLink>
        <button class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">
          刷新缓存
        </button>
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
const stats = ref({
  articleCount: 0,
  toolCount: 0,
  todayVisits: 0
})

const fetchStats = async () => {
  try {
    // 这里暂时复用 /api/stats 接口，或者后续创建专门的 dashboard 接口
    // 目前 /api/stats 返回的是访问统计，文章数可能需要另外获取
    // 为了演示，先只对接访问统计
    const res = await api.get<any>('/admin/stats')
    stats.value.todayVisits = res.todayVisits
    stats.value.articleCount = res.articleCount
    stats.value.toolCount = res.toolCount
  } catch (e) {
    console.error('Failed to fetch stats', e)
  }
}

onMounted(() => {
  fetchStats()
})
</script>

