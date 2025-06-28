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

      <!-- 搜索功能 -->
      <div class="max-w-2xl mx-auto mb-12">
        <div class="bg-white rounded-lg shadow-md p-6">
          <div class="flex items-center gap-4">
            <div class="flex-1 relative">
              <input
                v-model="searchKeyword"
                type="text"
                placeholder="搜索文章标题、内容、标签..."
                class="w-full px-4 py-3 pl-12 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
              >
              <div class="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                </svg>
              </div>
            </div>
            <button
              v-if="searchKeyword"
              @click="clearSearch"
              class="px-4 py-3 text-gray-500 hover:text-gray-700 transition-colors"
              title="清空搜索"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>
          
          <!-- 搜索提示信息 -->
          <div v-if="searchKeyword" class="mt-3 flex items-center justify-between text-sm">
            <span class="text-gray-600">
              🔍 搜索 "<strong>{{ searchKeyword }}</strong>" 找到 {{ filteredPosts.length }} 篇文章
            </span>
            <button
              @click="clearAllFilters"
              class="text-blue-600 hover:text-blue-800 transition-colors flex items-center gap-1"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
              清空筛选
            </button>
          </div>
        </div>
      </div>

      <!-- 分类导航 -->
      <div class="flex flex-wrap justify-center gap-4 mb-12">
        <button
          v-for="category in categories"
          :key="category"
          @click="selectCategory(category)"
          :class="getCategoryButtonClass(category)"
        >
          {{ category }}
        </button>
      </div>

      <!-- 分类统计 -->
      <div class="text-center mb-8">
        <p class="text-gray-600">
          <span v-if="searchKeyword && selectedCategory === '全部文章'">
            搜索结果：{{ filteredPosts.length }} 篇文章
          </span>
          <span v-else-if="searchKeyword">
            在 {{ selectedCategory }} 中搜索到 {{ filteredPosts.length }} 篇文章
          </span>
          <span v-else-if="selectedCategory === '全部文章'">
            共 {{ filteredPosts.length }} 篇文章
          </span>
          <span v-else>
            {{ selectedCategory }} 分类下共 {{ filteredPosts.length }} 篇文章
          </span>
        </p>
      </div>

      <!-- 文章列表 -->
      <section class="mb-12">
        <h2 class="text-2xl font-semibold text-gray-800 mb-6 border-b-2 border-blue-500 pb-2">
          <span v-if="selectedCategory === '全部文章'">最新文章</span>
          <span v-else>{{ selectedCategory }}</span>
        </h2>
        
        <!-- 没有文章时的提示 -->
        <div v-if="filteredPosts.length === 0" class="text-center py-12">
          <div class="text-6xl mb-4">📝</div>
          <h3 class="text-xl font-semibold text-gray-700 mb-2">暂无文章</h3>
          <p class="text-gray-500">该分类下还没有发布文章，敬请期待...</p>
        </div>
        
        <!-- 文章列表 -->
        <div v-else class="space-y-6">
          <article
            v-for="post in filteredPosts"
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
        <div class="grid md:grid-cols-3 gap-6">
          <!-- 分类统计 -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">📂 按分类</h3>
            <ul class="space-y-2">
              <li v-for="(count, category) in categoryStats" :key="category">
                <button 
                  @click="selectCategory(category)"
                  class="text-left w-full text-blue-600 hover:text-blue-800 transition-colors"
                  :class="{ 'font-semibold': selectedCategory === category }"
                >
                  {{ category }} ({{ count }}篇)
                </button>
              </li>
            </ul>
          </div>
          
          <!-- 时间归档 -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">📅 按时间</h3>
            <ul class="space-y-2">
              <li v-for="archive in timeArchives" :key="archive.period">
                <span class="text-gray-700">{{ archive.period }} ({{ archive.count }}篇)</span>
              </li>
              <li v-if="timeArchives.length === 0" class="text-gray-500 text-sm">
                暂无文章
              </li>
            </ul>
          </div>
          
          <!-- 热门标签 -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">🏷️ 热门标签</h3>
            <div class="flex flex-wrap gap-2">
              <span 
                v-for="tagInfo in popularTags" 
                :key="tagInfo.tag"
                class="px-3 py-1 bg-blue-100 text-blue-600 rounded-full text-sm cursor-pointer hover:bg-blue-200 transition-colors"
                :title="`${tagInfo.count} 篇文章`"
              >
                {{ tagInfo.tag }}
              </span>
              <span v-if="popularTags.length === 0" class="text-gray-500 text-sm">
                暂无标签
              </span>
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
import { ref, computed } from 'vue'

// 使用 @nuxt/content 查询博客数据
const { data: allPosts } = await useAsyncData('blog-posts', () =>
  queryContent('/blog').sort({ date: -1 }).find()
)

// 当前选中的分类
const selectedCategory = ref('全部文章')

// 搜索关键词
const searchKeyword = ref('')

// 获取所有可用的分类
const categories = computed(() => {
  if (!allPosts.value) return ['全部文章']
  
  const categorySet = new Set(allPosts.value.map(post => post.category))
  return ['全部文章', ...Array.from(categorySet)]
})

// 根据选中分类和搜索关键词过滤文章
const filteredPosts = computed(() => {
  if (!allPosts.value) return []
  
  let posts = allPosts.value
  
  // 分类过滤
  if (selectedCategory.value !== '全部文章') {
    posts = posts.filter(post => post.category === selectedCategory.value)
  }
  
  // 搜索过滤
  if (searchKeyword.value.trim()) {
    const keyword = searchKeyword.value.toLowerCase()
    posts = posts.filter(post => {
      const searchFields = [
        post.title,
        post.description,
        post.category,
        post.author,
        ...(post.tags || [])
      ].join(' ').toLowerCase()
      
      return searchFields.includes(keyword)
    })
  }
  
  return posts
})

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

// 切换分类
const selectCategory = (category) => {
  selectedCategory.value = category
}

// 清空搜索
const clearSearch = () => {
  searchKeyword.value = ''
}

// 清空所有过滤条件
const clearAllFilters = () => {
  selectedCategory.value = '全部文章'
  searchKeyword.value = ''
}

// 获取分类按钮样式
const getCategoryButtonClass = (category) => {
  const baseClass = 'px-6 py-2 rounded-full transition-colors'
  if (category === selectedCategory.value) {
    return `${baseClass} bg-blue-500 text-white`
  }
  return `${baseClass} bg-gray-200 text-gray-700 hover:bg-gray-300`
}

// 计算分类统计
const categoryStats = computed(() => {
  if (!allPosts.value) return {}
  
  const stats = {}
  allPosts.value.forEach(post => {
    const category = post.category
    if (!stats[category]) {
      stats[category] = 0
    }
    stats[category]++
  })
  return stats
})

// 计算时间归档
const timeArchives = computed(() => {
  if (!allPosts.value) return []
  
  const archives = {}
  allPosts.value.forEach(post => {
    const date = new Date(post.date)
    const yearMonth = `${date.getFullYear()}年${date.getMonth() + 1}月`
    if (!archives[yearMonth]) {
      archives[yearMonth] = 0
    }
    archives[yearMonth]++
  })
  
  return Object.entries(archives)
    .map(([period, count]) => ({ period, count }))
    .sort((a, b) => b.period.localeCompare(a.period))
})

// 计算热门标签
const popularTags = computed(() => {
  if (!allPosts.value) return []
  
  const tagCounts = {}
  allPosts.value.forEach(post => {
    if (post.tags) {
      post.tags.forEach(tag => {
        tagCounts[tag] = (tagCounts[tag] || 0) + 1
      })
    }
  })
  
  return Object.entries(tagCounts)
    .map(([tag, count]) => ({ tag, count }))
    .sort((a, b) => b.count - a.count)
    .slice(0, 10)
})

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

/* 分类按钮动画效果 */
button {
  position: relative;
  overflow: hidden;
}

button:active {
  transform: scale(0.98);
}

/* 选中状态的分类按钮增强效果 */
.bg-blue-500 {
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
}

.bg-blue-500:hover {
  background-color: rgb(37, 99, 235);
  box-shadow: 0 6px 16px rgba(59, 130, 246, 0.4);
}
</style> 