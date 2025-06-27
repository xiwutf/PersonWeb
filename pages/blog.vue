<template>
  <div class="bg-gradient-to-br from-green-50 via-blue-50 to-teal-50">
    <!-- 页面头部 -->
    <section class="py-16 bg-gradient-to-r from-green-600 to-blue-600 text-white">
      <div class="max-w-6xl mx-auto px-4 text-center">
        <div class="w-20 h-20 bg-white/20 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <span class="text-3xl">📝</span>
        </div>
        <h1 class="text-4xl lg:text-5xl font-bold mb-4">技术博客</h1>
        <p class="text-xl text-green-100 max-w-3xl mx-auto">
          分享技术心得、项目经验与生活感悟，记录成长路上的点点滴滴
        </p>
      </div>
    </section>

    <!-- 博客内容 -->
    <section class="py-20">
      <div class="max-w-6xl mx-auto px-4">

      <!-- 分类导航 -->
      <div class="flex flex-wrap justify-center gap-4 mb-12">
        <button class="px-6 py-2 bg-blue-500 text-white rounded-full hover:bg-blue-600 transition-colors">
          全部文章
        </button>
        <button class="px-6 py-2 bg-gray-200 text-gray-700 rounded-full hover:bg-gray-300 transition-colors">
          技术文章
        </button>
        <button class="px-6 py-2 bg-gray-200 text-gray-700 rounded-full hover:bg-gray-300 transition-colors">
          项目总结
        </button>
        <button class="px-6 py-2 bg-gray-200 text-gray-700 rounded-full hover:bg-gray-300 transition-colors">
          理财笔记
        </button>
        <button class="px-6 py-2 bg-gray-200 text-gray-700 rounded-full hover:bg-gray-300 transition-colors">
          生活随笔
        </button>
      </div>

      <!-- 最新文章 -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-800 mb-6 border-b-2 border-blue-500 pb-2">
          最新文章
        </h2>
        <div class="space-y-6">
          <article
            v-for="post in posts"
            :key="post._path"
            class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
          >
            <div class="flex items-start gap-4">
              <div class="flex-shrink-0 w-16 h-16 bg-blue-100 rounded-lg flex items-center justify-center">
                <span class="text-blue-600 text-xl">{{ getCategoryIcon(post.category) }}</span>
              </div>
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-2">
                  <span class="text-xs bg-blue-100 text-blue-600 px-2 py-1 rounded">
                    {{ post.category }}
                  </span>
                  <span class="text-gray-500 text-sm">{{ formatDate(post.date) }}</span>
                </div>
                <NuxtLink
                  :to="post._path"
                  class="text-xl font-semibold text-gray-800 mb-2 hover:text-blue-600 cursor-pointer block"
                >
                  {{ post.title }}
                </NuxtLink>
                <p class="text-gray-600 mb-3">{{ post.description }}</p>
                
                <!-- 标签 -->
                <div class="flex flex-wrap gap-1 mb-3">
                  <span
                    v-for="tag in post.tags"
                    :key="tag"
                    class="px-2 py-1 bg-gray-100 text-gray-600 rounded-full text-xs"
                  >
                    {{ tag }}
                  </span>
                </div>
                
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-4 text-sm text-gray-500">
                    <span>📅 {{ formatDate(post.date) }}</span>
                    <span>✍️ {{ post.author }}</span>
                  </div>
                  <NuxtLink
                    :to="post._path"
                    class="text-blue-600 hover:text-blue-800 font-medium"
                  >
                    阅读全文 →
                  </NuxtLink>
                </div>
              </div>
            </div>
          </article>
        </div>
      </section>

      <!-- 文章归档 -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-800 mb-6 border-b-2 border-gray-300 pb-2">
          文章归档
        </h2>
        <div class="grid md:grid-cols-2 gap-6">
          <div class="bg-white rounded-lg shadow-md p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">按时间归档</h3>
            <ul class="space-y-2">
              <li><a href="#" class="text-blue-600 hover:text-blue-800">2024年1月 (5篇)</a></li>
              <li><a href="#" class="text-blue-600 hover:text-blue-800">2023年12月 (8篇)</a></li>
              <li><a href="#" class="text-blue-600 hover:text-blue-800">2023年11月 (6篇)</a></li>
              <li><a href="#" class="text-blue-600 hover:text-blue-800">2023年10月 (12篇)</a></li>
            </ul>
          </div>
          
          <div class="bg-white rounded-lg shadow-md p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">热门标签</h3>
            <div class="flex flex-wrap gap-2">
              <span class="px-3 py-1 bg-blue-100 text-blue-600 rounded-full text-sm cursor-pointer hover:bg-blue-200">Vue.js</span>
              <span class="px-3 py-1 bg-green-100 text-green-600 rounded-full text-sm cursor-pointer hover:bg-green-200">Nuxt</span>
              <span class="px-3 py-1 bg-purple-100 text-purple-600 rounded-full text-sm cursor-pointer hover:bg-purple-200">JavaScript</span>
              <span class="px-3 py-1 bg-red-100 text-red-600 rounded-full text-sm cursor-pointer hover:bg-red-200">Revit</span>
              <span class="px-3 py-1 bg-yellow-100 text-yellow-600 rounded-full text-sm cursor-pointer hover:bg-yellow-200">理财</span>
              <span class="px-3 py-1 bg-indigo-100 text-indigo-600 rounded-full text-sm cursor-pointer hover:bg-indigo-200">项目管理</span>
            </div>
          </div>
        </div>
      </section>

      <!-- 返回首页链接 -->
      <div class="text-center">
        <NuxtLink to="/" class="inline-flex items-center text-blue-600 hover:text-blue-800 font-medium">
          ← 返回首页
        </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
// 使用 @nuxt/content 查询博客数据
const { data: posts } = await useAsyncData('blog-posts', () =>
  queryContent('/blog').sort({ date: -1 }).find()
)

// 格式化日期
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 获取分类图标
const getCategoryIcon = (category) => {
  const icons = {
    '技术文章': '💻',
    '理财笔记': '💰',
    '项目总结': '🚀',
    '生活随笔': '📝'
  }
  return icons[category] || '📄'
}

// 设置页面标题
useHead({
  title: '技术博客 - 溪午听风',
  meta: [
    { name: 'description', content: '分享技术心得、项目经验与生活感悟' }
  ]
})
</script>

<style scoped>
/* 页面淡入动画 */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

section {
  animation: fadeInUp 0.8s ease-out;
}

section:nth-child(2) {
  animation-delay: 0.2s;
}

/* 文章卡片悬停效果 */
article {
  transition: all 0.3s ease;
}

article:hover {
  transform: translateY(-5px);
}
</style> 