<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 via-red-50 to-pink-50 py-8">
    <div class="container mx-auto px-4 max-w-4xl">
      <!-- 面包屑导航 -->
      <nav class="mb-8">
        <div class="flex items-center space-x-2 text-sm text-gray-600">
          <NuxtLink to="/" class="hover:text-orange-600 transition-colors">首页</NuxtLink>
          <span>/</span>
          <NuxtLink to="/tools" class="hover:text-orange-600 transition-colors">插件工具</NuxtLink>
          <span>/</span>
          <span class="text-gray-900">{{ tool?.title || '加载中...' }}</span>
        </div>
      </nav>

      <!-- 返回按钮 -->
      <div class="mb-6">
        <NuxtLink
          to="/tools"
          class="inline-flex items-center px-4 py-2 bg-white/70 backdrop-blur-sm rounded-lg shadow-sm hover:shadow-md transition-all duration-200 text-orange-700 hover:text-orange-800 border border-orange-200"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          返回工具列表
        </NuxtLink>
      </div>

      <!-- 工具内容 -->
      <div v-if="tool" class="space-y-8">
        <!-- 工具信息头部 -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
          <div class="flex flex-col lg:flex-row gap-8">
            <div class="lg:w-1/3">
              <div class="h-64 bg-gradient-to-br from-orange-400 to-red-600 rounded-xl flex items-center justify-center">
                <div class="text-center text-white">
                  <span class="text-4xl mb-2 block">🔧</span>
                  <p class="text-lg font-semibold">专业工具</p>
                </div>
              </div>
            </div>
            <div class="lg:w-2/3">
              <div class="flex items-center justify-between mb-4">
                <div class="flex items-center space-x-2">
                  <span class="text-3xl font-bold text-green-600">¥{{ tool.price }}</span>
                  <span class="text-sm text-gray-500 line-through">¥{{ Math.round(tool.price * 1.5) }}</span>
                </div>
                <span class="text-sm text-gray-500">{{ formatDate(tool.date) }}</span>
              </div>
              
              <h1 class="text-3xl font-bold text-gray-900 mb-4">{{ tool.title }}</h1>
              <p class="text-gray-600 mb-6 text-lg leading-relaxed">{{ tool.description }}</p>
              
              <!-- 标签 -->
              <div class="mb-6">
                <h3 class="text-sm font-medium text-gray-700 mb-3">特性标签</h3>
                <div class="flex flex-wrap gap-2">
                  <span
                    v-for="tag in tool.tags"
                    :key="tag"
                    class="px-3 py-1 bg-orange-100 text-orange-700 rounded-full text-sm font-medium"
                  >
                    {{ tag }}
                  </span>
                </div>
              </div>
              
              <!-- 购买按钮 -->
              <div class="flex flex-wrap gap-4">
                <a
                  :href="tool.buy_link"
                  target="_blank"
                  class="bg-gradient-to-r from-orange-500 to-red-500 text-white px-8 py-3 rounded-xl hover:from-orange-600 hover:to-red-600 transition-all font-medium inline-flex items-center gap-2 shadow-lg"
                >
                  <i class="fas fa-shopping-cart"></i>
                  立即购买
                </a>
                <button class="border-2 border-orange-600 text-orange-600 px-8 py-3 rounded-xl hover:bg-orange-50 transition-colors font-medium inline-flex items-center gap-2">
                  <i class="fas fa-envelope"></i>
                  联系客服
                </button>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 详细内容 -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl overflow-hidden">
          <div class="p-8">
            <div class="prose prose-lg max-w-none">
              <ContentDoc :path="tool._path" />
            </div>
          </div>
        </div>
      </div>

      <!-- 无数据状态 -->
      <div v-else class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="bg-yellow-100 border border-yellow-400 text-yellow-700 px-4 py-3 rounded mb-4">
          <strong>警告:</strong> 没有找到工具数据！<br>
          当前 slug: {{ $route.params.slug }}<br>
          查询错误: {{ error }}
        </div>
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded w-2/3"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute()
const slug = route.params.slug

// 获取具体工具数据
const { data: tool, error } = await useAsyncData(`tool-${slug}`, () =>
  queryContent('/tools').where({
    $or: [
      { _path: `/tools/${slug}` },
      { slug: slug }
    ]
  }).findOne()
)

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 设置页面标题和SEO
useHead({
  title: `${tool.value?.title || '工具详情'} - 插件工具 - 溪午听风`,
  meta: [
    { name: 'description', content: tool.value?.description || '工具详情页面' },
    { name: 'keywords', content: tool.value?.tags?.join(', ') || '插件工具' }
  ]
})
</script>

<style scoped>
/* 优化代码块背景 */
:deep(.prose pre) {
  @apply bg-gray-900 text-gray-100;
  border-radius: 12px;
  padding: 1.5rem;
  overflow-x: auto;
}

:deep(.prose code) {
  @apply bg-gray-100 text-gray-800 px-1 py-0.5 rounded text-sm;
}

:deep(.prose pre code) {
  @apply bg-transparent text-gray-100 px-0 py-0;
}

/* 优化标题样式 */
:deep(.prose h1) {
  @apply text-2xl font-bold text-gray-800 mb-4;
}

:deep(.prose h2) {
  @apply text-xl font-semibold text-gray-800 mb-3 mt-6;
}

:deep(.prose h3) {
  @apply text-lg font-medium text-gray-800 mb-2 mt-4;
}

/* 优化列表样式 */
:deep(.prose ul) {
  @apply list-disc pl-6 mb-4;
}

:deep(.prose ol) {
  @apply list-decimal pl-6 mb-4;
}

:deep(.prose li) {
  @apply mb-1;
}

/* 优化表格样式 */
:deep(.prose table) {
  @apply border-collapse border border-gray-200 rounded-lg overflow-hidden;
}

:deep(.prose th) {
  @apply bg-gray-50 border border-gray-200 px-4 py-2;
}

:deep(.prose td) {
  @apply border border-gray-200 px-4 py-2;
}

/* 优化引用样式 */
:deep(.prose blockquote) {
  @apply border-l-4 border-orange-500 bg-orange-50 pl-4 py-2 my-4 rounded-r-lg;
}
</style> 