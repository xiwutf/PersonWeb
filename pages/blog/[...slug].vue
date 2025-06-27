<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <!-- 调试信息 -->
      <div class="bg-yellow-100 border border-yellow-400 text-yellow-700 px-4 py-3 rounded mb-4">
        <p><strong>博客详情页调试信息:</strong></p>
        <p>当前路由: {{ $route.path }}</p>
        <p>路由参数: {{ JSON.stringify($route.params) }}</p>
        <p>文章路径: {{ articlePath }}</p>
        <p>文章数据存在: {{ !!post }}</p>
        <p>文章标题: {{ post?.title || '未找到' }}</p>
      </div>
      
      <!-- 返回按钮 -->
      <NuxtLink
        to="/blog"
        class="inline-flex items-center text-blue-600 hover:text-blue-800 mb-6 font-medium"
      >
        ← 返回博客列表
      </NuxtLink>
      
      <!-- 文章头部信息 -->
      <div class="bg-white rounded-lg shadow-lg p-8 mb-8">
        <div class="flex items-center gap-2 mb-4">
          <span class="px-3 py-1 bg-blue-100 text-blue-600 rounded-full text-sm font-medium">
            {{ post.category }}
          </span>
          <span class="text-gray-500">{{ formatDate(post.date) }}</span>
          <span class="text-gray-500">·</span>
          <span class="text-gray-500">{{ post.author }}</span>
        </div>
        
        <h1 class="text-3xl lg:text-4xl font-bold text-gray-800 mb-4">{{ post.title }}</h1>
        <p class="text-xl text-gray-600 mb-6">{{ post.description }}</p>
        
        <!-- 标签 -->
        <div class="flex flex-wrap gap-2">
          <span
            v-for="tag in post.tags"
            :key="tag"
            class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm"
          >
            # {{ tag }}
          </span>
        </div>
      </div>
      
      <!-- 文章内容 -->
      <article class="bg-white rounded-lg shadow-lg overflow-hidden">
        <div class="p-8">
          <ContentDoc class="prose prose-lg max-w-none" />
        </div>
      </article>
      
      <!-- 文章导航 -->
      <div class="flex justify-between items-center mt-8 bg-white rounded-lg shadow-md p-6">
        <div class="flex items-center gap-4">
          <button class="text-blue-600 hover:text-blue-800 font-medium">
            👍 点赞
          </button>
          <button class="text-gray-600 hover:text-gray-800 font-medium">
            💬 评论
          </button>
          <button class="text-gray-600 hover:text-gray-800 font-medium">
            📤 分享
          </button>
        </div>
        <div class="text-sm text-gray-500">
          阅读量：{{ Math.floor(Math.random() * 1000) + 100 }}
        </div>
      </div>
      
      <!-- 相关文章推荐 -->
      <div v-if="relatedPosts?.length" class="mt-12">
        <h2 class="text-2xl font-bold text-gray-800 mb-6">相关文章</h2>
        <div class="grid md:grid-cols-2 gap-6">
          <NuxtLink
            v-for="related in relatedPosts"
            :key="related._path"
            :to="related._path"
            class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow block"
          >
            <div class="flex items-center gap-2 mb-2">
              <span class="text-xs bg-gray-100 text-gray-600 px-2 py-1 rounded-full">
                {{ related.category }}
              </span>
              <span class="text-xs text-gray-500">{{ formatDate(related.date) }}</span>
            </div>
            <h3 class="text-lg font-semibold text-gray-800 mb-2">{{ related.title }}</h3>
            <p class="text-gray-600 text-sm mb-3">{{ related.description }}</p>
            <div class="flex items-center justify-between">
              <div class="flex gap-1">
                <span
                  v-for="tag in related.tags?.slice(0, 2)"
                  :key="tag"
                  class="text-xs bg-gray-50 text-gray-500 px-2 py-1 rounded"
                >
                  {{ tag }}
                </span>
              </div>
              <span class="text-blue-600 text-sm">阅读 →</span>
            </div>
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const route = useRoute()
const slug = route.params.slug

// 调试信息
console.log('博客路由参数:', route.params)
console.log('slug值:', slug)

// 构建文章路径 - 使用完整路径
const articlePath = route.path

// 获取文章数据
const { data: post } = await useAsyncData(`blog-${slug}`, () =>
  queryContent(articlePath).findOne()
)

// 调试项目数据
console.log('博客数据:', post.value)

// 如果找不到内容，返回404
if (!post.value) {
  throw createError({
    statusCode: 404,
    statusMessage: '文章不存在'
  })
}

// 获取相关文章（同分类的其他文章）
const { data: relatedPosts } = await useAsyncData(`related-blog-${slug}`, () =>
  queryContent('/blog')
    .where({ 
      category: post.value.category,
      _path: { $ne: post.value._path }
    })
    .limit(4)
    .find()
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
  title: `${post.value.title} - 技术博客 - 溪午听风`,
  meta: [
    { name: 'description', content: post.value.description },
    { name: 'keywords', content: post.value.tags?.join(', ') },
    { name: 'author', content: post.value.author }
  ]
})
</script>

<style scoped>
.container {
  min-height: 100vh;
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
}

/* 优化文章内容样式 */
:deep(.prose) {
  @apply text-gray-800 leading-relaxed;
}

:deep(.prose h1) {
  @apply text-3xl font-bold text-gray-900 mb-6 mt-8;
}

:deep(.prose h2) {
  @apply text-2xl font-semibold text-gray-900 mb-4 mt-8 border-b border-gray-200 pb-2;
}

:deep(.prose h3) {
  @apply text-xl font-medium text-gray-900 mb-3 mt-6;
}

:deep(.prose h4) {
  @apply text-lg font-medium text-gray-900 mb-2 mt-4;
}

:deep(.prose p) {
  @apply mb-6 text-gray-700;
}

:deep(.prose ul) {
  @apply list-disc pl-6 mb-6;
}

:deep(.prose ol) {
  @apply list-decimal pl-6 mb-6;
}

:deep(.prose li) {
  @apply mb-2;
}

:deep(.prose blockquote) {
  @apply border-l-4 border-blue-500 pl-6 italic text-gray-600 bg-blue-50 py-4 my-6;
}

:deep(.prose img) {
  @apply rounded-lg shadow-lg mx-auto;
}

:deep(.prose code) {
  @apply bg-gray-100 px-2 py-1 rounded text-sm font-mono;
}

:deep(.prose pre) {
  @apply bg-gray-900 text-gray-100 p-6 rounded-lg overflow-x-auto my-6;
}

:deep(.prose pre code) {
  @apply bg-transparent p-0;
}

:deep(.prose table) {
  @apply w-full border-collapse;
}

:deep(.prose th) {
  @apply border border-gray-300 px-4 py-2 bg-gray-50 font-semibold;
}

:deep(.prose td) {
  @apply border border-gray-300 px-4 py-2;
}

:deep(.prose a) {
  @apply text-blue-600 hover:text-blue-800 underline;
}

:deep(.prose strong) {
  @apply font-semibold text-gray-900;
}
</style> 