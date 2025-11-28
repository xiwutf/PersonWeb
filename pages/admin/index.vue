<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
    <!-- 背景装饰 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 right-0 w-96 h-96 bg-blue-400/10 rounded-full blur-3xl"></div>
      <div class="absolute bottom-0 left-0 w-96 h-96 bg-purple-400/10 rounded-full blur-3xl"></div>
    </div>

    <div class="relative z-10">
      <!-- 头部 -->
      <header class="mb-8">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-4xl font-bold bg-gradient-to-r from-blue-600 via-purple-600 to-cyan-600 bg-clip-text text-transparent mb-2">
              仪表盘
            </h1>
            <p class="text-gray-600 dark:text-gray-400 flex items-center gap-2">
              <span class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></span>
              欢迎回来，Admin
            </p>
          </div>
          <div class="text-right">
            <div class="text-sm text-gray-500 dark:text-gray-400">当前时间</div>
            <div class="text-lg font-semibold text-gray-800 dark:text-white">{{ currentTime }}</div>
          </div>
        </div>
      </header>

      <!-- 统计卡片 - 美化版 -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <!-- 文章数卡片 -->
        <div class="group relative bg-white dark:bg-gray-800 p-6 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-300 border border-gray-100 dark:border-gray-700 overflow-hidden">
          <div class="absolute top-0 right-0 w-32 h-32 bg-blue-500/10 rounded-full -mr-16 -mt-16 group-hover:scale-150 transition-transform duration-500"></div>
          <div class="relative z-10">
            <div class="flex items-center justify-between mb-4">
              <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-xl flex items-center justify-center">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
              <div class="text-xs text-gray-500 dark:text-gray-400">总文章数</div>
            </div>
            <div class="text-4xl font-bold text-blue-600 dark:text-blue-400 mb-1">{{ stats.articleCount || 0 }}</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">篇已发布</div>
          </div>
        </div>

        <!-- 项目数卡片 -->
        <div class="group relative bg-white dark:bg-gray-800 p-6 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-300 border border-gray-100 dark:border-gray-700 overflow-hidden">
          <div class="absolute top-0 right-0 w-32 h-32 bg-purple-500/10 rounded-full -mr-16 -mt-16 group-hover:scale-150 transition-transform duration-500"></div>
          <div class="relative z-10">
            <div class="flex items-center justify-between mb-4">
              <div class="w-12 h-12 bg-purple-100 dark:bg-purple-900/30 rounded-xl flex items-center justify-center">
                <svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                </svg>
              </div>
              <div class="text-xs text-gray-500 dark:text-gray-400">总项目数</div>
            </div>
            <div class="text-4xl font-bold text-purple-600 dark:text-purple-400 mb-1">{{ stats.toolCount || 0 }}</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">个项目</div>
          </div>
        </div>

        <!-- 访问数卡片 -->
        <div class="group relative bg-white dark:bg-gray-800 p-6 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-300 border border-gray-100 dark:border-gray-700 overflow-hidden">
          <div class="absolute top-0 right-0 w-32 h-32 bg-green-500/10 rounded-full -mr-16 -mt-16 group-hover:scale-150 transition-transform duration-500"></div>
          <div class="relative z-10">
            <div class="flex items-center justify-between mb-4">
              <div class="w-12 h-12 bg-green-100 dark:bg-green-900/30 rounded-xl flex items-center justify-center">
                <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
              </div>
              <div class="text-xs text-gray-500 dark:text-gray-400">今日访问</div>
            </div>
            <div class="text-4xl font-bold text-green-600 dark:text-green-400 mb-1">{{ stats.todayVisits || 0 }}</div>
            <div class="text-sm text-gray-500 dark:text-gray-400">次访问</div>
          </div>
        </div>
      </div>

      <!-- 快捷操作 - 美化版 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 p-6">
          <h2 class="text-xl font-bold mb-6 text-gray-800 dark:text-white flex items-center gap-2">
            <svg class="w-5 h-5 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
            快捷操作
          </h2>
          <div class="grid grid-cols-2 gap-4">
            <NuxtLink to="/admin/articles/edit" class="group relative overflow-hidden bg-gradient-to-br from-blue-500 to-blue-600 text-white p-4 rounded-xl hover:from-blue-600 hover:to-blue-700 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-1">
              <div class="relative z-10">
                <div class="text-2xl mb-2">✍️</div>
                <div class="font-semibold">发布新文章</div>
                <div class="text-xs text-blue-100 mt-1">快速创建</div>
              </div>
              <div class="absolute inset-0 bg-white/20 transform scale-0 group-hover:scale-100 transition-transform duration-300"></div>
            </NuxtLink>
            
            <NuxtLink to="/admin/knowledge" class="group relative overflow-hidden bg-gradient-to-br from-purple-500 to-purple-600 text-white p-4 rounded-xl hover:from-purple-600 hover:to-purple-700 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-1">
              <div class="relative z-10">
                <div class="text-2xl mb-2">📚</div>
                <div class="font-semibold">知识库</div>
                <div class="text-xs text-purple-100 mt-1">管理笔记</div>
              </div>
              <div class="absolute inset-0 bg-white/20 transform scale-0 group-hover:scale-100 transition-transform duration-300"></div>
            </NuxtLink>
            
            <NuxtLink to="/admin/analytics" class="group relative overflow-hidden bg-gradient-to-br from-green-500 to-green-600 text-white p-4 rounded-xl hover:from-green-600 hover:to-green-700 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-1">
              <div class="relative z-10">
                <div class="text-2xl mb-2">📊</div>
                <div class="font-semibold">访客分析</div>
                <div class="text-xs text-green-100 mt-1">查看数据</div>
              </div>
              <div class="absolute inset-0 bg-white/20 transform scale-0 group-hover:scale-100 transition-transform duration-300"></div>
            </NuxtLink>
            
            <NuxtLink to="/admin/investment" class="group relative overflow-hidden bg-gradient-to-br from-orange-500 to-orange-600 text-white p-4 rounded-xl hover:from-orange-600 hover:to-orange-700 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-1">
              <div class="relative z-10">
                <div class="text-2xl mb-2">📈</div>
                <div class="font-semibold">投资仪表盘</div>
                <div class="text-xs text-orange-100 mt-1">查看收益</div>
              </div>
              <div class="absolute inset-0 bg-white/20 transform scale-0 group-hover:scale-100 transition-transform duration-300"></div>
            </NuxtLink>
          </div>
        </div>

        <!-- 最近活动 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 p-6">
          <h2 class="text-xl font-bold mb-6 text-gray-800 dark:text-white flex items-center gap-2">
            <svg class="w-5 h-5 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            最近活动
          </h2>
          <div class="space-y-4">
            <div class="flex items-center gap-4 p-3 bg-gray-50 dark:bg-gray-700/50 rounded-lg">
              <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900/30 rounded-full flex items-center justify-center">
                <span class="text-blue-600 dark:text-blue-400">📝</span>
              </div>
              <div class="flex-1">
                <div class="font-medium text-gray-800 dark:text-white">文章管理</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">管理你的文章内容</div>
              </div>
              <NuxtLink to="/admin/articles" class="text-blue-600 dark:text-blue-400 hover:underline text-sm">查看 →</NuxtLink>
            </div>
            
            <div class="flex items-center gap-4 p-3 bg-gray-50 dark:bg-gray-700/50 rounded-lg">
              <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900/30 rounded-full flex items-center justify-center">
                <span class="text-purple-600 dark:text-purple-400">⏰</span>
              </div>
              <div class="flex-1">
                <div class="font-medium text-gray-800 dark:text-white">时间胶囊</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">审核访客留言</div>
              </div>
              <NuxtLink to="/admin/time-capsules" class="text-purple-600 dark:text-purple-400 hover:underline text-sm">查看 →</NuxtLink>
            </div>
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
const stats = ref({
  articleCount: 0,
  toolCount: 0,
  todayVisits: 0
})

const fetchStats = async () => {
  try {
    // 后端 Stats API 返回格式: { code: 0, data: { TotalVisits, TodayVisits, ArticleCount, ProjectCount, ... } }
    // 注意：Stats API 需要 [Authorize]，确保已登录
    const res = await api.get<any>('/Stats')
    // useApi 已经处理了响应格式，直接返回 data
    if (res) {
      stats.value.todayVisits = res.TodayVisits ?? res.todayVisits ?? 0
      stats.value.articleCount = res.ArticleCount ?? res.articleCount ?? 0
      stats.value.toolCount = res.ProjectCount ?? res.projectCount ?? 0
    } else {
      // 如果返回空，使用默认值
      stats.value.todayVisits = 0
      stats.value.articleCount = 0
      stats.value.toolCount = 0
    }
  } catch (e: any) {
    console.error('Failed to fetch stats:', e)
    // 如果 API 返回错误（如 500），使用默认值，不阻塞页面显示
    stats.value.todayVisits = 0
    stats.value.articleCount = 0
    stats.value.toolCount = 0
  }
}

const currentTime = ref('')

const updateTime = () => {
  const now = new Date()
  currentTime.value = now.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

onMounted(() => {
  fetchStats()
  updateTime()
  setInterval(updateTime, 1000)
})
</script>

