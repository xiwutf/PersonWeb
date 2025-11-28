<template>
  <div class="bg-gradient-to-br from-indigo-50 to-purple-50 min-h-screen">
    <!-- 页面头部 -->
    <section class="py-16 bg-gradient-to-r from-indigo-600 to-purple-600 text-white">
      <div class="max-w-6xl mx-auto px-4 text-center">
        <div class="w-20 h-20 bg-white/20 rounded-2xl flex items-center justify-center mx-auto mb-6">
          <span class="text-3xl">🔍</span>
        </div>
        <h1 class="text-4xl lg:text-5xl font-bold mb-4">全站搜索</h1>
        <p class="text-xl text-indigo-100 max-w-3xl mx-auto">
          搜索博客文章、项目作品、知识库等所有内容
        </p>
      </div>
    </section>

    <!-- 搜索内容 -->
    <section class="py-20">
      <div class="max-w-6xl mx-auto px-4">
        
        <!-- 搜索框 -->
        <div class="max-w-3xl mx-auto mb-12">
          <div class="bg-white rounded-xl shadow-lg p-8">
            <div class="flex items-center gap-4 mb-6">
              <div class="flex-1 relative">
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="搜索文章、项目、知识库..."
                  class="w-full px-6 py-4 pl-14 text-lg border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
                  @keyup.enter="performSearch"
                  @input="handleInput"
                >
                <div class="absolute left-5 top-1/2 transform -translate-y-1/2 text-gray-400">
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                  </svg>
                </div>
              </div>
              <button
                @click="performSearch"
                :disabled="loading"
                class="px-8 py-4 bg-indigo-600 text-white rounded-xl hover:bg-indigo-700 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ loading ? '搜索中...' : '搜索' }}
              </button>
            </div>
            
            <!-- 搜索类型选择 -->
            <div class="flex flex-wrap gap-3">
              <button
                v-for="type in searchTypes"
                :key="type.key"
                @click="selectedType = type.key; performSearch()"
                :class="getTypeButtonClass(type.key)"
              >
                <span class="mr-2">{{ type.icon }}</span>
                {{ type.label }}
              </button>
            </div>
          </div>
        </div>

        <!-- 搜索结果统计 -->
        <div v-if="hasSearched && !loading" class="text-center mb-8">
          <p class="text-gray-600">
            <span v-if="searchQuery">
              关键词 "<strong>{{ searchQuery }}</strong>" 的搜索结果：
            </span>
            共找到 <strong>{{ searchResults?.total || 0 }}</strong> 条结果
          </p>
        </div>

        <!-- 加载状态 -->
        <div v-if="loading" class="text-center py-16">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mb-4"></div>
          <p class="text-gray-600">正在搜索...</p>
        </div>

        <!-- 搜索结果 -->
        <div v-else-if="hasSearched && searchResults">
          <!-- 博客文章结果 -->
          <div v-if="searchResults.articles && searchResults.articles.length > 0" class="mb-12">
            <h2 class="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              <span class="text-3xl mr-3">📝</span>
              博客文章 ({{ searchResults.articles.length }})
            </h2>
            <div class="grid gap-6">
              <article
                v-for="article in searchResults.articles"
                :key="article.id"
                class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
              >
                <div class="flex items-start gap-4">
                  <div class="flex-shrink-0 w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                    <span class="text-blue-600 text-lg">📄</span>
                  </div>
                  <div class="flex-1">
                    <div class="flex items-center gap-2 mb-2">
                      <span v-if="article.category" class="text-xs bg-blue-100 text-blue-600 px-2 py-1 rounded">
                        {{ article.category }}
                      </span>
                      <span class="text-gray-500 text-sm">{{ formatDate(article.createdAt) }}</span>
                    </div>
                    <NuxtLink
                      :to="article.url"
                      class="text-xl font-semibold text-gray-800 mb-2 hover:text-blue-600 cursor-pointer block"
                      v-html="highlightText(article.title, searchQuery)"
                    ></NuxtLink>
                    <p 
                      class="text-gray-600 mb-3 line-clamp-2"
                      v-html="highlightText(article.summary || article.content.substring(0, 200), searchQuery)"
                    ></p>
                    <div class="flex items-center justify-between">
                      <NuxtLink
                        :to="article.url"
                        class="text-blue-600 hover:text-blue-800 font-medium"
                      >
                        阅读全文 →
                      </NuxtLink>
                    </div>
                  </div>
                </div>
              </article>
            </div>
          </div>

          <!-- 项目结果 -->
          <div v-if="searchResults.projects && searchResults.projects.length > 0" class="mb-12">
            <h2 class="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              <span class="text-3xl mr-3">🧪</span>
              项目作品 ({{ searchResults.projects.length }})
            </h2>
            <div class="grid md:grid-cols-2 gap-6">
              <article
                v-for="project in searchResults.projects"
                :key="project.id"
                class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow"
              >
                <div class="p-6">
                  <div class="flex items-center gap-2 mb-2">
                    <span class="text-xs bg-green-100 text-green-600 px-2 py-1 rounded">
                      项目
                    </span>
                  </div>
                  <h3 
                    class="text-lg font-semibold text-gray-800 mb-2"
                    v-html="highlightText(project.title, searchQuery)"
                  ></h3>
                  <p 
                    class="text-gray-600 mb-4 line-clamp-3"
                    v-html="highlightText(project.summary || project.content.substring(0, 200), searchQuery)"
                  ></p>
                  <div class="flex items-center justify-between">
                    <NuxtLink
                      :to="project.url"
                      class="text-green-600 hover:text-green-800 font-medium"
                    >
                      查看详情 →
                    </NuxtLink>
                  </div>
                </div>
              </article>
            </div>
          </div>

          <!-- 知识库结果 -->
          <div v-if="searchResults.knowledgeBases && searchResults.knowledgeBases.length > 0" class="mb-12">
            <h2 class="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              <span class="text-3xl mr-3">📚</span>
              知识库 ({{ searchResults.knowledgeBases.length }})
            </h2>
            <div class="grid gap-6">
              <article
                v-for="knowledge in searchResults.knowledgeBases"
                :key="knowledge.id"
                class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
              >
                <div class="flex items-start gap-4">
                  <div class="flex-shrink-0 w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
                    <span class="text-purple-600 text-lg">📖</span>
                  </div>
                  <div class="flex-1">
                    <div class="flex items-center gap-2 mb-2">
                      <span v-if="knowledge.category" class="text-xs bg-purple-100 text-purple-600 px-2 py-1 rounded">
                        {{ knowledge.category }}
                      </span>
                      <span class="text-gray-500 text-sm">{{ formatDate(knowledge.createdAt) }}</span>
                    </div>
                    <NuxtLink
                      :to="knowledge.url"
                      class="text-xl font-semibold text-gray-800 mb-2 hover:text-purple-600 cursor-pointer block"
                      v-html="highlightText(knowledge.title, searchQuery)"
                    ></NuxtLink>
                    <p 
                      class="text-gray-600 mb-3 line-clamp-2"
                      v-html="highlightText(knowledge.content.substring(0, 200), searchQuery)"
                    ></p>
                    <div class="flex items-center justify-between">
                      <NuxtLink
                        :to="knowledge.url"
                        class="text-purple-600 hover:text-purple-800 font-medium"
                      >
                        查看详情 →
                      </NuxtLink>
                    </div>
                  </div>
                </div>
              </article>
            </div>
          </div>

          <!-- 无结果提示 -->
          <div v-if="searchResults.total === 0" class="text-center py-16">
            <div class="text-6xl mb-4">🔍</div>
            <h3 class="text-2xl font-semibold text-gray-700 mb-4">未找到相关内容</h3>
            <p class="text-gray-500 mb-6">试试调整搜索关键词或选择不同的内容类型</p>
            <div class="space-y-2 text-sm text-gray-400">
              <p>搜索建议：</p>
              <p>• 使用更简短的关键词</p>
              <p>• 检查拼写是否正确</p>
              <p>• 尝试使用同义词</p>
            </div>
          </div>
        </div>

        <!-- 初始状态 -->
        <div v-else-if="!hasSearched" class="text-center py-16">
          <div class="text-6xl mb-6">🚀</div>
          <h2 class="text-2xl font-semibold text-gray-700 mb-4">开始搜索吧！</h2>
          <p class="text-gray-500 mb-8">输入关键词，搜索博客、项目、知识库等内容</p>
          
          <!-- 快速搜索标签 -->
          <div class="max-w-2xl mx-auto">
            <p class="text-sm text-gray-400 mb-4">热门搜索：</p>
            <div class="flex flex-wrap justify-center gap-2">
              <button
                v-for="tag in popularSearchTags"
                :key="tag"
                @click="quickSearch(tag)"
                class="px-4 py-2 bg-white text-gray-600 rounded-full text-sm hover:bg-gray-50 hover:text-gray-800 transition-colors border"
              >
                {{ tag }}
              </button>
            </div>
          </div>
        </div>

        <!-- 返回首页 -->
        <div class="text-center mt-12">
          <NuxtLink to="/" class="inline-flex items-center text-indigo-600 hover:text-indigo-800 font-medium">
            ← 返回首页
          </NuxtLink>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { SearchResults, SearchResultItem } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

// 搜索状态
const searchQuery = ref('')
const selectedType = ref('all')
const hasSearched = ref(false)
const loading = ref(false)
const searchResults = ref<SearchResults | null>(null)

// 搜索类型配置
const searchTypes = [
  { key: 'all', label: '全部', icon: '🔍' },
  { key: 'articles', label: '文章', icon: '📝' },
  { key: 'projects', label: '项目', icon: '🧪' },
  { key: 'knowledge', label: '知识库', icon: '📚' }
]

// 热门搜索标签
const popularSearchTags = [
  'Vue', 'Nuxt', 'C#', 'ASP.NET', 'MySQL', '全栈开发', '前端', '后端', '数据库'
]

// API 调用
const api = useApi()
const notification = useNotification()
const errorHandler = useErrorHandler()

// 执行搜索
const performSearch = async () => {
  if (!searchQuery.value.trim()) {
    notification.warning('请输入搜索关键词')
    return
  }

  loading.value = true
  hasSearched.value = true

  try {
    // 映射前端类型到后端类型
    const backendType = selectedType.value === 'all' ? 'all' :
                       selectedType.value === 'articles' ? 'articles' :
                       selectedType.value === 'projects' ? 'projects' :
                       selectedType.value === 'knowledge' ? 'knowledge' : 'all'

    const res = await api.get<SearchResults>('/Search', {
      params: {
        keyword: searchQuery.value.trim(),
        type: backendType,
        page: 1,
        pageSize: 20
      }
    })

    searchResults.value = res
  } catch (error) {
    errorHandler.handleError(error, '搜索失败')
    searchResults.value = {
      keyword: searchQuery.value,
      type: selectedType.value,
      total: 0,
      articles: [],
      projects: [],
      knowledgeBases: []
    }
  } finally {
    loading.value = false
  }
}

// 输入处理（防抖）
let searchTimeout: NodeJS.Timeout | null = null
const handleInput = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  // 可以在这里实现实时搜索（防抖）
  // searchTimeout = setTimeout(() => {
  //   if (searchQuery.value.trim()) {
  //     performSearch()
  //   }
  // }, 500)
}

// 快速搜索
const quickSearch = (tag: string) => {
  searchQuery.value = tag
  performSearch()
}

// 获取类型按钮样式
const getTypeButtonClass = (type: string) => {
  const baseClass = 'px-4 py-2 rounded-full text-sm transition-colors'
  if (type === selectedType.value) {
    return `${baseClass} bg-indigo-600 text-white`
  }
  return `${baseClass} bg-gray-100 text-gray-600 hover:bg-gray-200`
}

// 高亮文本
const highlightText = (text: string, keyword: string): string => {
  if (!text || !keyword) return text
  
  const regex = new RegExp(`(${keyword})`, 'gi')
  return text.replace(regex, '<mark class="bg-yellow-200 px-1 rounded">$1</mark>')
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 设置页面标题
useHead({
  title: '全站搜索 - 溪午听风',
  meta: [
    { name: 'description', content: '搜索博客文章、项目作品、知识库等所有内容' }
  ]
})
</script>

<style scoped>
/* 搜索框动画效果 */
input:focus {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

/* 结果卡片动画 */
article {
  transition: all 0.3s ease;
}

article:hover {
  transform: translateY(-5px);
}

/* 搜索按钮效果 */
button:active {
  transform: scale(0.98);
}

/* 文本截断 */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
