<template>
  <div class="min-h-screen bg-gradient-to-br from-emerald-50 via-teal-50 to-cyan-50 py-8">
    <div class="container mx-auto px-4 max-w-4xl">
      <!-- 面包屑导航 -->
      <nav class="mb-8">
        <div class="flex items-center space-x-2 text-sm text-gray-600">
          <NuxtLink to="/" class="hover:text-emerald-600 transition-colors">首页</NuxtLink>
          <span>/</span>
          <NuxtLink to="/blog" class="hover:text-emerald-600 transition-colors">技术博客</NuxtLink>
          <span>/</span>
          <span class="text-gray-900">{{ post?.title || '加载中...' }}</span>
        </div>
      </nav>

      <!-- 返回按钮 -->
      <div class="mb-6">
        <NuxtLink 
          to="/blog" 
          class="inline-flex items-center px-4 py-2 bg-white/70 backdrop-blur-sm rounded-lg shadow-sm hover:shadow-md transition-all duration-200 text-emerald-700 hover:text-emerald-800 border border-emerald-200"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          返回博客列表
        </NuxtLink>
      </div>

      <!-- 文章内容 -->
      <div v-if="post" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 mb-8">
        <!-- 文章头部 -->
        <div class="mb-8 pb-6 border-b border-gray-200">
          <div class="flex items-center gap-2 text-sm text-emerald-600 mb-3">
            <span class="px-2 py-1 bg-emerald-100 rounded-full">{{ post.category }}</span>
            <span>{{ formatDate(post.date) }}</span>
            <span>{{ post.author }}</span>
            <span v-if="views > 0" class="flex items-center ml-2 text-gray-500">
              <i class="fas fa-eye mr-1"></i>
              {{ views }} 次阅读
            </span>
          </div>
          <h1 class="text-3xl font-bold text-gray-900 mb-4">{{ post.title }}</h1>
          <p class="text-lg text-gray-600 leading-relaxed">{{ post.description }}</p>
          
          <!-- 标签 -->
          <div class="flex flex-wrap gap-2 mt-4">
            <span 
              v-for="tag in post.tags" 
              :key="tag"
              class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm hover:bg-emerald-100 hover:text-emerald-700 transition-colors"
            >
              # {{ tag }}
            </span>
          </div>
        </div>

        <!-- 文章正文 -->
        <div class="prose prose-lg max-w-none">
          <ContentDoc :path="post._path" />
        </div>

        <!-- 评论区 -->
        <div class="mt-12 pt-8 border-t border-gray-200">
          <GiscusComments :identifier="post._path" :title="post.title" />
        </div>
      </div>

      <!-- 加载状态 -->
      <div v-else class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded w-2/3"></div>
        </div>
      </div>

      <!-- 相关文章推荐 -->
      <div v-if="relatedPosts.length > 0" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <h3 class="text-2xl font-bold text-gray-900 mb-6">相关文章推荐</h3>
        <div class="grid gap-6 md:grid-cols-2">
          <NuxtLink 
            v-for="related in relatedPosts" 
            :key="related._path"
            :to="related._path"
            class="group p-6 border border-gray-200 rounded-xl hover:border-emerald-300 hover:shadow-lg transition-all duration-200"
          >
            <h4 class="font-semibold text-gray-900 group-hover:text-emerald-700 mb-2">{{ related.title }}</h4>
            <p class="text-gray-600 text-sm mb-3">{{ related.description }}</p>
            <div class="flex items-center justify-between text-xs text-gray-500">
              <span>{{ formatDate(related.date) }}</span>
              <span class="text-emerald-600 group-hover:text-emerald-700">阅读更多 →</span>
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

// 构建文章路径 - 处理catch-all路由
const slugString = Array.isArray(slug) ? slug[0] : slug
const articlePath = `/blog/${slugString}`

// 获取文章数据
const { data: post } = await useAsyncData(`blog-${slugString}`, () =>
  queryContent(articlePath).findOne()
)

// 调试项目数据
console.log('文章路径:', articlePath)
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