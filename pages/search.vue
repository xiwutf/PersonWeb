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
          搜索博客文章、项目作品、插件工具等所有内容
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
                  placeholder="搜索文章、项目、工具、标签..."
                  class="w-full px-6 py-4 pl-14 text-lg border border-gray-300 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
                  @keyup.enter="performSearch"
                >
                <div class="absolute left-5 top-1/2 transform -translate-y-1/2 text-gray-400">
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                  </svg>
                </div>
              </div>
              <button
                @click="performSearch"
                class="px-8 py-4 bg-indigo-600 text-white rounded-xl hover:bg-indigo-700 transition-colors font-medium"
              >
                搜索
              </button>
            </div>
            
            <!-- 搜索类型选择 -->
            <div class="flex flex-wrap gap-3">
              <button
                v-for="type in searchTypes"
                :key="type.key"
                @click="selectedType = type.key"
                :class="getTypeButtonClass(type.key)"
              >
                <span class="mr-2">{{ type.icon }}</span>
                {{ type.label }}
              </button>
            </div>
          </div>
        </div>

        <!-- 搜索结果统计 -->
        <div v-if="hasSearched" class="text-center mb-8">
          <p class="text-gray-600">
            <span v-if="searchQuery">
              关键词 "<strong>{{ searchQuery }}</strong>" 的搜索结果：
            </span>
            共找到 <strong>{{ totalResults }}</strong> 条结果
          </p>
        </div>

        <!-- 搜索结果 -->
        <div v-if="hasSearched">
          <!-- 博客文章结果 -->
          <div v-if="filteredBlogResults.length > 0" class="mb-12">
            <h2 class="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              <span class="text-3xl mr-3">📝</span>
              博客文章 ({{ filteredBlogResults.length }})
            </h2>
            <div class="grid gap-6">
              <article
                v-for="post in filteredBlogResults"
                :key="post._path"
                class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
              >
                <div class="flex items-start gap-4">
                  <div class="flex-shrink-0 w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                    <span class="text-blue-600 text-lg">📄</span>
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
                    <div class="flex items-center justify-between">
                      <div class="flex flex-wrap gap-1">
                        <span
                          v-for="tag in (post.tags || [])"
                          :key="tag"
                          class="px-2 py-1 bg-gray-100 text-gray-600 rounded-full text-xs"
                        >
                          {{ tag }}
                        </span>
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
          </div>

          <!-- 项目结果 -->
          <div v-if="filteredProjectResults.length > 0" class="mb-12">
            <h2 class="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              <span class="text-3xl mr-3">🧪</span>
              项目作品 ({{ filteredProjectResults.length }})
            </h2>
            <div class="grid md:grid-cols-2 gap-6">
              <article
                v-for="project in filteredProjectResults"
                :key="project._path"
                class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow"
              >
                <div class="p-6">
                  <div class="flex items-center gap-2 mb-2">
                    <span class="text-xs bg-green-100 text-green-600 px-2 py-1 rounded">
                      {{ project.category || '项目' }}
                    </span>
                    <span class="text-xs bg-gray-100 text-gray-600 px-2 py-1 rounded">
                      {{ project.status }}
                    </span>
                  </div>
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">{{ project.title }}</h3>
                  <p class="text-gray-600 mb-4">{{ project.description }}</p>
                  <div class="flex items-center justify-between">
                    <div class="flex flex-wrap gap-1">
                      <span
                        v-for="tech in (project.tech || [])"
                        :key="tech"
                        class="px-2 py-1 bg-purple-100 text-purple-600 rounded-full text-xs"
                      >
                        {{ tech }}
                      </span>
                    </div>
                    <NuxtLink
                      :to="`/projects/detail-${project.slug}`"
                      class="text-green-600 hover:text-green-800 font-medium"
                    >
                      查看详情 →
                    </NuxtLink>
                  </div>
                </div>
              </article>
            </div>
          </div>

          <!-- 工具插件结果 -->
          <div v-if="filteredToolResults.length > 0" class="mb-12">
            <h2 class="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              <span class="text-3xl mr-3">🔧</span>
              插件工具 ({{ filteredToolResults.length }})
            </h2>
            <div class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
              <article
                v-for="tool in filteredToolResults"
                :key="tool._path"
                class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow"
              >
                <div class="p-6">
                  <div class="flex items-center gap-2 mb-2">
                    <span class="text-xs bg-orange-100 text-orange-600 px-2 py-1 rounded">
                      插件工具
                    </span>
                    <span class="text-lg font-bold text-red-600">{{ tool.price || '￥0' }}</span>
                  </div>
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">{{ tool.title }}</h3>
                  <p class="text-gray-600 mb-4">{{ tool.description }}</p>
                  <div class="flex items-center justify-between">
                    <div class="flex flex-wrap gap-1">
                      <span
                        v-for="tag in (tool.tags || [])"
                        :key="tag"
                        class="px-2 py-1 bg-gray-100 text-gray-600 rounded-full text-xs"
                      >
                        {{ tag }}
                      </span>
                    </div>
                    <NuxtLink
                      :to="`/tools/detail-${tool.slug}`"
                      class="text-orange-600 hover:text-orange-800 font-medium"
                    >
                      了解详情 →
                    </NuxtLink>
                  </div>
                </div>
              </article>
            </div>
          </div>

          <!-- 无结果提示 -->
          <div v-if="totalResults === 0" class="text-center py-16">
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
        <div v-else class="text-center py-16">
          <div class="text-6xl mb-6">🚀</div>
          <h2 class="text-2xl font-semibold text-gray-700 mb-4">开始搜索吧！</h2>
          <p class="text-gray-500 mb-8">输入关键词，搜索博客、项目、工具等内容</p>
          
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

// 搜索状态
const searchQuery = ref('')
const selectedType = ref('all')
const hasSearched = ref(false)

// 搜索类型配置
const searchTypes = [
  { key: 'all', label: '全部', icon: '🔍' },
  { key: 'blog', label: '博客', icon: '📝' },
  { key: 'projects', label: '项目', icon: '🧪' },
  { key: 'tools', label: '工具', icon: '🔧' }
]

// 热门搜索标签
const popularSearchTags = [
  '助手', 'Revit', '插件', '标注', '理财', 'Vue.js', 'Nuxt', '小程序', '全栈开发', '批量加载'
]

// 使用 useAsyncData 加载所有数据
const { data: blogPosts } = await useAsyncData('search-blog', () =>
  queryContent('/blog').sort({ date: -1 }).find()
)

const { data: projects } = await useAsyncData('search-projects', () =>
  queryContent('/projects').sort({ date: -1 }).find()
)

const { data: tools } = await useAsyncData('search-tools', () =>
  queryContent('/tools').sort({ date: -1 }).find()
)

// 搜索结果计算
const searchResults = computed(() => {
  if (!searchQuery.value.trim()) return { blog: [], projects: [], tools: [] }
  
  const keyword = searchQuery.value.toLowerCase()
  
  const searchInFields = (item, fields) => {
    return fields.some(field => {
      const value = item[field]
      if (typeof value === 'string') {
        return value.toLowerCase().includes(keyword)
      }
      if (Array.isArray(value)) {
        return value.some(v => v.toLowerCase().includes(keyword))
      }
      return false
    })
  }
  
  return {
    blog: (blogPosts.value || []).filter(post => 
      searchInFields(post, ['title', 'description', 'category', 'author', 'tags'])
    ),
    projects: (projects.value || []).filter(project => 
      searchInFields(project, ['title', 'description', 'tech', 'category', 'status'])
    ),
    tools: (tools.value || []).filter(tool => 
      searchInFields(tool, ['title', 'description', 'tags', 'category'])
    )
  }
})

// 根据选中类型过滤结果
const filteredBlogResults = computed(() => 
  selectedType.value === 'all' || selectedType.value === 'blog' ? searchResults.value.blog : []
)
const filteredProjectResults = computed(() => 
  selectedType.value === 'all' || selectedType.value === 'projects' ? searchResults.value.projects : []
)
const filteredToolResults = computed(() => 
  selectedType.value === 'all' || selectedType.value === 'tools' ? searchResults.value.tools : []
)

// 总结果数量
const totalResults = computed(() => 
  filteredBlogResults.value.length + filteredProjectResults.value.length + filteredToolResults.value.length
)

// 执行搜索
const performSearch = () => {
  if (searchQuery.value.trim()) {
    hasSearched.value = true
  }
}

// 快速搜索
const quickSearch = (tag) => {
  searchQuery.value = tag
  performSearch()
}

// 获取类型按钮样式
const getTypeButtonClass = (type) => {
  const baseClass = 'px-4 py-2 rounded-full text-sm transition-colors'
  if (type === selectedType.value) {
    return `${baseClass} bg-indigo-600 text-white`
  }
  return `${baseClass} bg-gray-100 text-gray-600 hover:bg-gray-200`
}

// 格式化日期
const formatDate = (dateString) => {
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
    { name: 'description', content: '搜索博客文章、项目作品、插件工具等所有内容' }
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
</style> 
