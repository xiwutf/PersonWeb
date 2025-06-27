<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <!-- 返回按钮 -->
      <NuxtLink
        to="/tools"
        class="inline-flex items-center text-blue-600 hover:text-blue-800 mb-6 font-medium"
      >
        ← 返回工具列表
      </NuxtLink>
      
      <!-- 工具信息头部 -->
      <div class="bg-white rounded-lg shadow-md p-8 mb-8">
        <div class="flex flex-col lg:flex-row gap-8">
          <div class="lg:w-1/3">
            <div class="h-64 bg-gradient-to-br from-blue-100 to-purple-100 rounded-lg flex items-center justify-center">
              <span class="text-6xl">🔧</span>
            </div>
          </div>
          <div class="lg:w-2/3">
            <h1 class="text-3xl font-bold text-gray-800 mb-4">{{ tool.title }}</h1>
            <p class="text-gray-600 mb-4 text-lg">{{ tool.description }}</p>
            
            <!-- 标签 -->
            <div class="flex flex-wrap gap-2 mb-4">
              <span
                v-for="tag in tool.tags"
                :key="tag"
                class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm"
              >
                {{ tag }}
              </span>
            </div>
            
            <!-- 价格和购买 -->
            <div class="flex items-center gap-6 mb-6">
              <span class="text-3xl font-bold text-green-600">{{ tool.price }}</span>
              <div class="text-sm text-gray-500">
                发布时间：{{ formatDate(tool.date) }}
              </div>
            </div>
            
            <div class="flex gap-4">
              <a
                :href="tool.buy_link"
                target="_blank"
                class="bg-blue-600 text-white px-8 py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium"
              >
                立即购买
              </a>
              <button class="border border-gray-300 text-gray-700 px-8 py-3 rounded-lg hover:bg-gray-50 transition-colors">
                联系客服
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <!-- 详细内容 -->
      <div class="bg-white rounded-lg shadow-md overflow-hidden">
        <div class="p-8">
          <ContentDoc :path="`/tools/${tool.slug}`" class="prose prose-lg max-w-none" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute()
const slug = route.params.slug

// 获取具体工具数据
const { data: tool } = await useAsyncData(`tool-${slug}`, () =>
  queryContent('/tools').where({ slug }).findOne()
)

// 如果找不到内容，返回404
if (!tool.value) {
  throw createError({
    statusCode: 404,
    statusMessage: '工具不存在'
  })
}

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
  title: `${tool.value.title} - 插件工具 - 溪午听风`,
  meta: [
    { name: 'description', content: tool.value.description },
    { name: 'keywords', content: tool.value.tags?.join(', ') }
  ]
})
</script>

<style scoped>
.container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

/* 优化内容样式 */
:deep(.prose) {
  @apply text-gray-700;
}

:deep(.prose h1) {
  @apply text-2xl font-bold text-gray-800 mb-4;
}

:deep(.prose h2) {
  @apply text-xl font-semibold text-gray-800 mb-3 mt-6;
}

:deep(.prose h3) {
  @apply text-lg font-medium text-gray-800 mb-2 mt-4;
}

:deep(.prose p) {
  @apply mb-4;
}

:deep(.prose ul) {
  @apply list-disc pl-6 mb-4;
}

:deep(.prose ol) {
  @apply list-decimal pl-6 mb-4;
}

:deep(.prose li) {
  @apply mb-1;
}

:deep(.prose code) {
  @apply bg-gray-100 px-2 py-1 rounded text-sm;
}

:deep(.prose pre) {
  @apply bg-gray-900 text-white p-4 rounded-lg overflow-x-auto;
}
</style> 