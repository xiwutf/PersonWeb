<template>
  <div class="min-h-screen bg-gradient-to-br from-purple-50 via-pink-50 to-indigo-50 py-8">
    <div class="container mx-auto px-4 max-w-4xl">
      <!-- 面包屑导航 -->
      <nav class="mb-8">
        <div class="flex items-center space-x-2 text-sm text-gray-600">
          <NuxtLink to="/" class="hover:text-purple-600 transition-colors">首页</NuxtLink>
          <span>/</span>
          <NuxtLink to="/projects" class="hover:text-purple-600 transition-colors">项目展示</NuxtLink>
          <span>/</span>
          <span class="text-gray-900">{{ project?.title || '加载中...' }}</span>
        </div>
      </nav>

      <!-- 返回按钮 -->
      <div class="mb-6">
        <NuxtLink
          to="/projects"
          class="inline-flex items-center px-4 py-2 bg-white/70 backdrop-blur-sm rounded-lg shadow-sm hover:shadow-md transition-all duration-200 text-purple-700 hover:text-purple-800 border border-purple-200"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          返回项目列表
        </NuxtLink>
      </div>

      <!-- 项目内容 -->
      <div v-if="project" class="space-y-8">
        <!-- 项目信息头部 -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
          <div class="flex flex-col lg:flex-row gap-8">
            <div class="lg:w-1/3">
              <div class="h-64 bg-gradient-to-br from-purple-400 to-pink-600 rounded-xl flex items-center justify-center">
                <div class="text-center text-white">
                  <span class="text-4xl mb-2 block">🚀</span>
                  <p class="text-lg font-semibold">{{ project.category }}</p>
                </div>
              </div>
            </div>
            <div class="lg:w-2/3">
              <div class="flex items-center gap-3 mb-4">
                <span
                  :class="[
                    'px-3 py-1 rounded-full text-sm font-medium',
                    project.status === '已上线' 
                      ? 'bg-green-100 text-green-700' 
                      : project.status === '开发中'
                      ? 'bg-yellow-100 text-yellow-700'
                      : project.status === '已完成'
                      ? 'bg-blue-100 text-blue-700'
                      : 'bg-gray-100 text-gray-700'
                  ]"
                >
                  {{ project.status }}
                </span>
                <span class="text-gray-500">{{ formatDate(project.date) }}</span>
              </div>
              
              <h1 class="text-3xl font-bold text-gray-900 mb-4">{{ project.title }}</h1>
              <p class="text-gray-600 mb-6 text-lg leading-relaxed">{{ project.description }}</p>
              
              <!-- 技术栈 -->
              <div class="mb-6">
                <h3 class="text-sm font-medium text-gray-700 mb-3">技术栈</h3>
                <div class="flex flex-wrap gap-2">
                  <span
                    v-for="tech in project.tech"
                    :key="tech"
                    class="px-3 py-1 bg-purple-100 text-purple-700 rounded-full text-sm font-medium"
                  >
                    {{ tech }}
                  </span>
                </div>
              </div>
              
              <!-- 链接按钮 -->
              <div class="flex flex-wrap gap-4">
                <a
                  v-if="project.demo_link"
                  :href="project.demo_link"
                  target="_blank"
                  class="bg-purple-600 text-white px-6 py-3 rounded-xl hover:bg-purple-700 transition-colors font-medium inline-flex items-center gap-2"
                >
                  <i class="fas fa-external-link-alt"></i>
                  在线体验
                </a>
                <a
                  v-if="project.source_link"
                  :href="project.source_link"
                  target="_blank"
                  class="border-2 border-purple-600 text-purple-600 px-6 py-3 rounded-xl hover:bg-purple-50 transition-colors font-medium inline-flex items-center gap-2"
                >
                  <i class="fab fa-github"></i>
                  源码查看
                </a>
                <button class="border-2 border-gray-300 text-gray-700 px-6 py-3 rounded-xl hover:bg-gray-50 transition-colors font-medium inline-flex items-center gap-2">
                  <i class="fas fa-envelope"></i>
                  联系作者
                </button>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 详细内容 -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl overflow-hidden">
          <div class="p-8">
            <div class="prose prose-lg max-w-none">
              <ContentDoc :path="project._path" />
            </div>
          </div>
        </div>
        
        <!-- 相关项目推荐 -->
        <div v-if="relatedProjects?.length" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
          <h2 class="text-2xl font-bold text-gray-900 mb-6">相关项目推荐</h2>
          <div class="grid gap-6 md:grid-cols-2">
            <NuxtLink
              v-for="related in relatedProjects"
              :key="related._path"
              :to="related._path.replace('/projects/', '/projects/')"
              class="group p-6 border border-gray-200 rounded-xl hover:border-purple-300 hover:shadow-lg transition-all duration-200"
            >
              <h3 class="font-semibold text-gray-900 group-hover:text-purple-700 mb-2">{{ related.title }}</h3>
              <p class="text-gray-600 text-sm mb-3">{{ related.description }}</p>
              <div class="flex items-center justify-between text-xs text-gray-500">
                <span class="px-2 py-1 bg-gray-100 rounded-full">{{ related.category }}</span>
                <span class="text-purple-600 group-hover:text-purple-700">查看详情 →</span>
              </div>
            </NuxtLink>
          </div>
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
    </div>
  </div>
</template>

<script setup>
const route = useRoute()
const slug = route.params.slug

// 获取具体项目数据 - 使用文件名匹配
const { data: project } = await useAsyncData(`project-${slug}`, () =>
  queryContent('/projects').where({ _path: `/projects/${slug}` }).findOne()
)

// 如果找不到内容，返回404
if (!project.value) {
  throw createError({
    statusCode: 404,
    statusMessage: '项目不存在'
  })
}

// 获取相关项目（同类别的其他项目）
const { data: relatedProjects } = await useAsyncData(`related-projects-${slug}`, () =>
  queryContent('/projects')
    .where({ category: project.value.category, _path: { $ne: project.value._path } })
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
  title: `${project.value.title} - 项目展示 - 溪午听风`,
  meta: [
    { name: 'description', content: project.value.description },
    { name: 'keywords', content: project.value.tech?.join(', ') }
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
  @apply border-l-4 border-purple-500 bg-purple-50 pl-4 py-2 my-4 rounded-r-lg;
}
</style> 