<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <!-- 调试信息 -->
      <div class="bg-yellow-100 border border-yellow-400 text-yellow-700 px-4 py-3 rounded mb-4">
        <p><strong>调试信息:</strong></p>
        <p>当前路由: {{ $route.path }}</p>
        <p>路由参数: {{ JSON.stringify($route.params) }}</p>
        <p>当前slug: {{ slug }}</p>
        <p>项目数据存在: {{ !!project }}</p>
        <p>项目标题: {{ project?.title || '未找到' }}</p>
        <p><strong>这是项目详情页面！</strong></p>
      </div>
      
      <!-- 面包屑导航 -->
      <nav class="flex items-center space-x-2 text-sm text-gray-600 mb-6">
        <NuxtLink to="/" class="hover:text-blue-600">首页</NuxtLink>
        <span>/</span>
        <NuxtLink to="/projects" class="hover:text-blue-600">项目展示</NuxtLink>
        <span>/</span>
        <span class="text-gray-800">{{ project?.title || '项目详情' }}</span>
      </nav>
      
      <!-- 返回按钮 -->
      <NuxtLink
        to="/projects"
        class="inline-flex items-center text-blue-600 hover:text-blue-800 mb-6 font-medium bg-blue-50 px-4 py-2 rounded-lg transition-colors"
      >
        ← 返回项目列表
      </NuxtLink>
      
            <!-- 项目内容 -->
      <div v-if="project">
        <!-- 项目信息头部 -->
      <div class="bg-white rounded-lg shadow-lg p-8 mb-8">
        <div class="flex flex-col lg:flex-row gap-8">
          <div class="lg:w-1/3">
            <div class="h-64 bg-gradient-to-br from-blue-400 to-purple-600 rounded-lg flex items-center justify-center">
              <div class="text-center text-white">
                <span class="text-4xl mb-2 block">🚀</span>
                <p class="text-lg">{{ project.category }}</p>
              </div>
            </div>
          </div>
          <div class="lg:w-2/3">
            <div class="flex items-center gap-3 mb-3">
              <span
                :class="[
                  'px-3 py-1 rounded-full text-sm font-medium',
                  project.status === '已上线' 
                    ? 'bg-green-100 text-green-600' 
                    : project.status === '开发中'
                    ? 'bg-yellow-100 text-yellow-600'
                    : project.status === '已完成'
                    ? 'bg-blue-100 text-blue-600'
                    : 'bg-gray-100 text-gray-600'
                ]"
              >
                {{ project.status }}
              </span>
              <span class="text-gray-500">{{ formatDate(project.date) }}</span>
            </div>
            
            <h1 class="text-3xl font-bold text-gray-800 mb-4">{{ project.title }}</h1>
            <p class="text-gray-600 mb-6 text-lg">{{ project.description }}</p>
            
            <!-- 技术栈 -->
            <div class="mb-6">
              <h3 class="text-sm font-medium text-gray-700 mb-2">技术栈</h3>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="tech in project.tech"
                  :key="tech"
                  class="px-3 py-1 bg-blue-100 text-blue-600 rounded-full text-sm"
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
                class="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium inline-flex items-center gap-2"
              >
                <span>🌐</span> 在线体验
              </a>
              <a
                v-if="project.source_link"
                :href="project.source_link"
                target="_blank"
                class="border border-gray-300 text-gray-700 px-6 py-3 rounded-lg hover:bg-gray-50 transition-colors font-medium inline-flex items-center gap-2"
              >
                <span>📁</span> 源码查看
              </a>
              <button class="border border-blue-600 text-blue-600 px-6 py-3 rounded-lg hover:bg-blue-50 transition-colors font-medium">
                联系作者
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <!-- 详细内容 -->
      <div class="bg-white rounded-lg shadow-lg overflow-hidden">
        <div class="p-8">
          <ContentDoc :path="`/projects/${slug}`" class="prose prose-lg max-w-none" />
        </div>
      </div>
      
      <!-- 相关项目推荐 -->
      <div v-if="relatedProjects?.length" class="mt-12">
        <h2 class="text-2xl font-bold text-gray-800 mb-6">相关项目</h2>
        <div class="grid md:grid-cols-2 gap-6">
          <NuxtLink
            v-for="related in relatedProjects"
            :key="related._path"
            :to="`/projects/${related.slug}`"
            class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow block"
          >
            <h3 class="text-lg font-semibold text-gray-800 mb-2">{{ related.title }}</h3>
            <p class="text-gray-600 text-sm mb-3">{{ related.description }}</p>
            <div class="flex items-center justify-between">
              <span class="text-xs bg-gray-100 text-gray-600 px-2 py-1 rounded-full">
                {{ related.category }}
              </span>
              <span class="text-blue-600 text-sm">查看详情 →</span>
            </div>
          </NuxtLink>
        </div>
      </div>
      
      </div> <!-- 关闭项目内容条件渲染 -->
    </div>
  </div>
</template>

<script setup>
const route = useRoute()
const slug = route.params.slug

// 调试信息
console.log('当前路由参数:', route.params)
console.log('slug值:', slug)

// 获取具体项目数据
const { data: project } = await useAsyncData(`project-${slug}`, () =>
  queryContent('/projects').where({ slug }).findOne()
)

// 调试项目数据
console.log('项目数据:', project.value)

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
    .where({ category: project.value.category, slug: { $ne: slug } })
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
.container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  background-attachment: fixed;
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

:deep(.prose blockquote) {
  @apply border-l-4 border-blue-500 pl-4 italic text-gray-600;
}

:deep(.prose img) {
  @apply rounded-lg shadow-md;
}

:deep(.prose code) {
  @apply bg-gray-100 px-2 py-1 rounded text-sm;
}

:deep(.prose pre) {
  @apply bg-gray-900 text-white p-4 rounded-lg overflow-x-auto;
}
</style> 