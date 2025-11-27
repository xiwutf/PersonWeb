<template>
  <div class="min-h-screen bg-[#fdfbf7] py-8">
    <div class="container mx-auto px-4 max-w-3xl">
      <!-- 顶部导航 -->
      <nav class="flex items-center justify-between mb-12 text-sm text-gray-500">
        <NuxtLink to="/life" class="hover:text-orange-600 transition-colors flex items-center">
          <i class="fas fa-arrow-left mr-2"></i>
          返回随笔列表
        </NuxtLink>
        <NuxtLink to="/" class="hover:text-orange-600 transition-colors">首页</NuxtLink>
      </nav>

      <!-- 文章内容 -->
      <article v-if="post" class="bg-white shadow-sm border border-gray-100 rounded-none p-8 md:p-12 mb-12">
        <!-- 文章头部 -->
        <header class="text-center mb-12">
          <div class="flex items-center justify-center gap-3 text-sm text-gray-400 mb-4 font-serif">
            <span>{{ formatDate(post.date) }}</span>
            <span>•</span>
            <span>{{ post.category || '随笔' }}</span>
          </div>
          <h1 class="text-3xl md:text-4xl font-bold text-gray-800 mb-6 font-serif tracking-wide">{{ post.title }}</h1>
          <div v-if="post.cover" class="w-full h-64 md:h-80 overflow-hidden rounded-lg mb-8">
            <img :src="post.cover" :alt="post.title" class="w-full h-full object-cover" />
          </div>
        </header>

        <!-- 文章正文 -->
        <div class="prose prose-lg prose-stone mx-auto font-serif">
          <ContentDoc :path="post._path" />
        </div>
        
        <!-- 底部标签 -->
        <div class="mt-12 pt-8 border-t border-gray-100 flex justify-center">
          <div class="flex gap-2">
            <span 
              v-for="tag in post.tags" 
              :key="tag"
              class="text-sm text-gray-400 italic"
            >
              #{{ tag }}
            </span>
          </div>
        </div>
        <!-- 评论区 -->
        <GiscusComments :identifier="post._path" :title="post.title" />
      </article>

      <!-- 加载状态 -->
      <div v-else class="text-center py-20">
        <div class="animate-pulse text-gray-400">加载中...</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const slug = route.params.slug

// 处理slug
const slugString = Array.isArray(slug) ? slug[0] : slug
const articlePath = `/life/${slugString}`

// 获取文章数据
const { data: post } = await useAsyncData(`life-${slugString}`, () =>
  queryContent(articlePath).findOne()
)

// 404处理
if (!post.value) {
  throw createError({ statusCode: 404, statusMessage: '文章不存在' })
}

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// SEO
useHead({
  title: `${post.value.title} - 生活随笔`,
  meta: [
    { name: 'description', content: post.value.description }
  ]
})
</script>

<style scoped>
/* 衬线字体优化阅读体验 */
.font-serif {
  font-family: "Merriweather", "Georgia", serif;
}
</style>

