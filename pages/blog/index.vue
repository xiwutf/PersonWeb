<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <!-- 全局背景噪点 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <!-- 动态背景光斑 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 left-1/4 w-[500px] h-[500px] bg-blue-500/20 rounded-full blur-[100px] animate-blob mix-blend-screen"></div>
      <div class="absolute bottom-0 right-1/4 w-[500px] h-[500px] bg-emerald-500/20 rounded-full blur-[100px] animate-blob animation-delay-2000 mix-blend-screen"></div>
      <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] bg-indigo-500/10 rounded-full blur-[120px] animate-pulse mix-blend-screen"></div>
    </div>

    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 返回按钮 -->
      <div class="mb-8">
        <NuxtLink
          to="/lab"
          class="blog-back-button"
        >
          <svg class="blog-back-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          <span>返回AI实验室</span>
        </NuxtLink>
      </div>

      <!-- 页面头部 -->
      <header class="text-center mb-16 relative">
        <div class="inline-flex items-center justify-center w-20 h-20 rounded-2xl bg-gradient-to-br from-blue-500/20 to-emerald-500/20 backdrop-blur-xl border border-white/10 mb-6 shadow-lg shadow-blue-500/10 animate-float">
          <span class="text-4xl">📝</span>
        </div>
        <h1 class="text-4xl md:text-5xl font-bold mb-6 bg-clip-text text-transparent bg-gradient-to-r from-blue-200 via-white to-emerald-200 tracking-tight">
          技术博客
        </h1>
        <p class="text-lg text-slate-400 max-w-2xl mx-auto leading-relaxed">
          分享技术心得、项目经验与生活感悟，记录成长路上的点点滴滴
        </p>
      </header>

      <!-- 搜索与筛选区域 -->
      <div class="mb-16 space-y-8">
        <!-- 搜索框 -->
        <div class="max-w-2xl mx-auto relative group">
          <div class="absolute -inset-1 bg-gradient-to-r from-blue-500 to-emerald-500 rounded-xl opacity-20 group-hover:opacity-40 transition duration-500 blur"></div>
          <div class="relative bg-slate-900/80 backdrop-blur-xl rounded-xl border border-white/10 flex items-center p-2 transition-all duration-300 focus-within:border-blue-500/50 focus-within:bg-slate-900/90">
            <div class="pl-4 text-slate-500">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
            </div>
            <input
              v-model="searchKeyword"
              type="text"
              placeholder="搜索文章标题、内容、标签..."
              class="w-full bg-transparent border-none text-slate-200 placeholder-slate-500 focus:ring-0 px-4 py-2"
            >
            <button
              v-if="searchKeyword"
              @click="clearSearch"
              class="p-2 text-slate-500 hover:text-slate-300 transition-colors"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
            </button>
          </div>
        </div>

        <!-- 分类标签 -->
        <div class="flex flex-wrap justify-center gap-3">
          <button
            v-for="category in categories"
            :key="category"
            @click="selectCategory(category)"
            class="px-5 py-2 rounded-full text-sm font-medium transition-all duration-300 border backdrop-blur-md"
            :class="category === selectedCategory 
              ? 'bg-blue-500/20 border-blue-500/50 text-blue-200 shadow-[0_0_15px_rgba(59,130,246,0.3)]' 
              : 'bg-slate-800/40 border-white/5 text-slate-400 hover:bg-slate-700/50 hover:text-slate-200 hover:border-white/10'"
          >
            {{ category }}
          </button>
        </div>
      </div>

      <!-- 文章统计和分页信息 -->
      <div v-if="filteredPosts.length > 0" class="mb-8 flex flex-col sm:flex-row justify-between items-center gap-4">
        <div class="text-slate-400 text-sm">
          共找到 <span class="text-blue-400 font-semibold">{{ filteredPosts.length }}</span> 篇文章
          <span v-if="currentPage > 1" class="ml-2">
            第 {{ currentPage }} / {{ totalPages }} 页
          </span>
        </div>
        <div class="flex items-center gap-2">
          <label class="text-sm text-slate-400">每页显示：</label>
          <select v-model="pageSize" class="blog-page-size-select">
            <option :value="10">10</option>
            <option :value="20">20</option>
            <option :value="30">30</option>
          </select>
        </div>
      </div>

      <!-- 文章列表 -->
      <div class="grid gap-8">
        <div v-if="filteredPosts.length === 0" class="text-center py-20">
          <div class="text-6xl mb-6 opacity-50">🍃</div>
          <h3 class="text-xl font-semibold text-slate-300 mb-2">暂无相关文章</h3>
          <p class="text-slate-500">换个关键词试试？</p>
        </div>

        <TransitionGroup name="list" tag="div" class="space-y-6">
          <article
            v-for="(post, index) in paginatedPosts"
            :key="post.id || post._path"
            class="blog-post-card"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <div class="flex flex-col md:flex-row gap-6 md:gap-8">
              <!-- 分类图标/封面 -->
              <div class="flex-shrink-0">
                <NuxtLink 
                  :to="getArticlePath(post)" 
                  class="block"
                >
                  <div class="w-24 h-24 md:w-32 md:h-32 rounded-2xl bg-gradient-to-br from-slate-700/50 to-slate-800/50 border border-white/10 flex items-center justify-center text-4xl md:text-5xl shadow-inner group-hover:scale-110 group-hover:border-blue-500/30 transition-all duration-500 relative overflow-hidden">
                    <div class="absolute inset-0 bg-gradient-to-br from-blue-500/0 to-emerald-500/0 group-hover:from-blue-500/20 group-hover:to-emerald-500/20 transition-all duration-500"></div>
                    <span class="relative z-10">{{ getCategoryIcon(post.category) }}</span>
                  </div>
                </NuxtLink>
              </div>

              <!-- 内容区域 -->
              <div class="flex-1 min-w-0">
                <div class="flex flex-wrap items-center gap-3 text-sm text-slate-500 mb-3">
                  <NuxtLink 
                    :to="`/blog?category=${encodeURIComponent(post.category)}`" 
                    class="px-2.5 py-0.5 rounded-md bg-blue-500/10 text-blue-400 border border-blue-500/20 font-medium hover:bg-blue-500/20 transition-colors"
                    @click.stop
                  >
                    {{ post.category }}
                  </NuxtLink>
                  <span class="flex items-center gap-1">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                    {{ formatDate(post.date) }}
                  </span>
                  <span v-if="post.viewCount" class="flex items-center gap-1">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path></svg>
                    {{ post.viewCount }}
                  </span>
                </div>

                <NuxtLink 
                  :to="getArticlePath(post)" 
                  class="block group-hover:translate-x-1 transition-transform duration-300"
                >
                  <h2 class="text-2xl md:text-3xl font-bold text-slate-100 mb-3 group-hover:text-blue-400 transition-colors line-clamp-2">
                    {{ post.title }}
                  </h2>
                  <p class="text-slate-400 leading-relaxed mb-4 line-clamp-3 group-hover:text-slate-300 transition-colors">
                    {{ post.description }}
                  </p>
                </NuxtLink>

                <!-- 底部信息 -->
                <div class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
                  <div class="flex flex-wrap gap-2">
                    <NuxtLink
                      v-for="tag in post.tags"
                      :key="tag"
                      :to="`/blog?search=${encodeURIComponent(tag)}`"
                      class="text-xs px-2 py-1 rounded-md bg-slate-700/30 text-slate-400 hover:text-blue-400 hover:bg-blue-500/10 transition-all cursor-pointer"
                      @click.stop
                    >
                      #{{ tag }}
                    </NuxtLink>
                  </div>
                  <NuxtLink 
                    :to="getArticlePath(post)" 
                    class="inline-flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium text-blue-400 hover:text-blue-300 hover:bg-blue-500/10 transition-all group/link"
                  >
                    阅读全文
                    <svg class="w-4 h-4 group-hover/link:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4-4H3"></path></svg>
                  </NuxtLink>
                </div>
              </div>
            </div>
          </article>
        </TransitionGroup>
      </div>

      <!-- 分页器 -->
      <div v-if="totalPages > 1" class="mt-12 flex justify-center items-center gap-2">
        <button
          @click="goToPage(currentPage - 1)"
          :disabled="currentPage === 1"
          class="blog-pagination-btn blog-pagination-btn-prev"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
          </svg>
        </button>
        
        <div class="flex gap-1">
          <template v-for="page in visiblePages" :key="page">
            <button
              v-if="page !== -1"
              @click="goToPage(page)"
              :class="[
                'blog-pagination-btn',
                'blog-pagination-btn-number',
                page === currentPage ? 'blog-pagination-btn-active' : ''
              ]"
            >
              {{ page }}
            </button>
            <span v-else class="blog-pagination-ellipsis">...</span>
          </template>
        </div>
        
        <button
          @click="goToPage(currentPage + 1)"
          :disabled="currentPage === totalPages"
          class="blog-pagination-btn blog-pagination-btn-next"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
          </svg>
        </button>
      </div>

      <!-- 归档区域 -->
      <div class="mt-20 grid md:grid-cols-2 lg:grid-cols-4 gap-6">
        <!-- 热门标签 -->
        <div class="blog-sidebar-card">
          <h3 class="blog-sidebar-title">
            <span class="text-blue-400">🏷️</span> 热门标签
          </h3>
          <div class="flex flex-wrap gap-2">
            <NuxtLink
              v-for="tagInfo in popularTags"
              :key="tagInfo.tag"
              :to="`/blog?search=${encodeURIComponent(tagInfo.tag)}`"
              class="blog-tag-item"
            >
              {{ tagInfo.tag }} <span class="opacity-50 ml-1">{{ tagInfo.count }}</span>
            </NuxtLink>
          </div>
        </div>

        <!-- 时间归档 -->
        <div class="blog-sidebar-card">
          <h3 class="blog-sidebar-title">
            <span class="text-emerald-400">📅</span> 时间归档
          </h3>
          <ul class="blog-archive-list">
            <li v-for="archive in timeArchives.slice(0, 6)" :key="archive.period" class="blog-archive-item">
              <span>{{ archive.period }}</span>
              <span class="blog-archive-count">{{ archive.count }}</span>
            </li>
          </ul>
        </div>

        <!-- 分类统计 -->
        <div class="blog-sidebar-card">
          <h3 class="blog-sidebar-title">
            <span class="text-purple-400">📂</span> 分类统计
          </h3>
          <ul class="blog-archive-list">
            <li
              v-for="cat in categories.filter(c => c !== '全部文章').slice(0, 6)"
              :key="cat"
              class="blog-archive-item cursor-pointer"
              @click="selectCategory(cat)"
            >
              <span>{{ cat }}</span>
              <span class="blog-archive-count">{{ getCategoryCount(cat) }}</span>
            </li>
          </ul>
        </div>
        
        <!-- 返回AI实验室 -->
        <div class="blog-back-card" @click="navigateTo('/lab')">
          <div class="blog-back-card-icon">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path>
            </svg>
          </div>
          <h3 class="blog-back-card-title">返回AI实验室</h3>
          <p class="blog-back-card-subtitle">Back to AI Lab</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const api = useApi()

// 使用 .NET API 查询博客数据
const { data: allPosts } = await useAsyncData('blog-posts', async () => {
  const res = await api.get<any>('/Articles', {
    params: {
      page: 1,
      pageSize: 100 // Get all for now to support client-side filtering
    }
  })
  // Map .NET response to frontend format
  // 注意：res 已经是 useApi 处理后的 data，格式为 { Total: int, List: [] }
  const articles = res.List ?? res.list ?? []
  return articles.map((article: any) => {
    // 确保有有效的 ID 或 slug
    const articleId = article.id || article.Id
    const articleSlug = article.slug || article.Slug
    
    // 优先使用 slug，如果没有则使用 id
    const pathSegment = articleSlug || articleId
    
    if (!pathSegment) {
      console.warn('Article missing both id and slug:', article)
    }
    
    return {
      ...article,
      id: articleId,
      slug: articleSlug,
      _path: pathSegment ? `/blog/${pathSegment}` : '#', // 确保路径有效
      date: article.publishTime || article.createdAt,
      category: article.categoryName || article.category?.name || '未分类',
      tags: article.tags ? (Array.isArray(article.tags) ? article.tags : article.tags.split(',').filter((t: string) => t.trim())) : []
    }
  })
})

// 当前选中的分类
const selectedCategory = ref('全部文章')

// 搜索关键词
const searchKeyword = ref('')

// 分页相关
const currentPage = ref(1)
const pageSize = ref(10)

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
  
  if (selectedCategory.value !== '全部文章') {
    posts = posts.filter(post => post.category === selectedCategory.value)
  }
  
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

// 分页后的文章列表
const paginatedPosts = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredPosts.value.slice(start, end)
})

// 总页数
const totalPages = computed(() => {
  return Math.ceil(filteredPosts.value.length / pageSize.value)
})

// 可见的页码（最多显示7个）
const visiblePages = computed(() => {
  const pages: number[] = []
  const total = totalPages.value
  const current = currentPage.value
  
  if (total <= 7) {
    // 如果总页数少于等于7，显示所有页码
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    // 否则显示当前页附近的页码
    if (current <= 4) {
      // 前几页
      for (let i = 1; i <= 5; i++) {
        pages.push(i)
      }
      pages.push(-1) // 省略号
      pages.push(total)
    } else if (current >= total - 3) {
      // 后几页
      pages.push(1)
      pages.push(-1) // 省略号
      for (let i = total - 4; i <= total; i++) {
        pages.push(i)
      }
    } else {
      // 中间页
      pages.push(1)
      pages.push(-1) // 省略号
      for (let i = current - 1; i <= current + 1; i++) {
        pages.push(i)
      }
      pages.push(-1) // 省略号
      pages.push(total)
    }
  }
  
  return pages
})

// 跳转到指定页
const goToPage = (page: number) => {
  if (page < 1 || page > totalPages.value || page === currentPage.value) return
  currentPage.value = page
  // 滚动到顶部
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// 监听筛选条件变化，重置到第一页
watch([selectedCategory, searchKeyword], () => {
  currentPage.value = 1
})

// 监听每页数量变化，重置到第一页
watch(pageSize, () => {
  currentPage.value = 1
})

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return ''
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
    '生活随笔': '📝',
    '运维部署': '🔧'
  }
  return icons[category] || '📄'
}

// 切换分类
const selectCategory = (category) => {
  selectedCategory.value = category
  currentPage.value = 1 // 重置到第一页
}

// 清空搜索
const clearSearch = () => {
  searchKeyword.value = ''
}

// 计算时间归档
const timeArchives = computed(() => {
  if (!allPosts.value) return []
  const archives = {}
  allPosts.value.forEach(post => {
    if (!post.date) return
    const date = new Date(post.date)
    const yearMonth = `${date.getFullYear()}年${date.getMonth() + 1}月`
    archives[yearMonth] = (archives[yearMonth] || 0) + 1
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

// 获取分类文章数量
const getCategoryCount = (category: string) => {
  if (!allPosts.value) return 0
  return allPosts.value.filter(post => post.category === category).length
}

// 获取文章路径
const getArticlePath = (post: any) => {
  // 优先使用 slug，如果没有则使用 id
  const pathSegment = post.slug || post.id
  if (!pathSegment) {
    console.warn('Article missing both id and slug:', post)
    return '/blog'
  }
  return `/blog/${pathSegment}`
}


useHead({
  title: '技术博客 - 溪午听风',
  meta: [
    { name: 'description', content: '分享技术心得、项目经验与生活感悟' }
  ]
})
</script>

<style scoped>
/* 文章卡片 */
.blog-post-card {
  position: relative;
  background: rgba(30, 41, 59, 0.3);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 1rem;
  padding: 1.5rem 2rem;
  transition: all 0.5s ease;
}

.blog-post-card:hover {
  background: rgba(30, 41, 59, 0.5);
  border-color: rgba(59, 130, 246, 0.3);
  box-shadow: 0 0 30px rgba(59, 130, 246, 0.1);
}

/* 分页器样式 */
.blog-page-size-select {
  padding: 0.375rem 0.75rem;
  background: rgba(30, 41, 59, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: rgb(203, 213, 225);
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.blog-page-size-select:hover {
  background: rgba(30, 41, 59, 0.7);
  border-color: rgba(59, 130, 246, 0.3);
}

.blog-page-size-select:focus {
  outline: none;
  border-color: rgba(59, 130, 246, 0.5);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.blog-pagination-btn {
  min-width: 2.5rem;
  height: 2.5rem;
  padding: 0.5rem;
  background: rgba(30, 41, 59, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: rgb(203, 213, 225);
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.blog-pagination-btn:hover:not(:disabled) {
  background: rgba(30, 41, 59, 0.7);
  border-color: rgba(59, 130, 246, 0.3);
  color: rgb(148, 163, 184);
}

.blog-pagination-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.blog-pagination-btn-number {
  min-width: 2.5rem;
}

.blog-pagination-btn-active {
  background: rgba(59, 130, 246, 0.2);
  border-color: rgba(59, 130, 246, 0.5);
  color: rgb(96, 165, 250);
  box-shadow: 0 0 15px rgba(59, 130, 246, 0.3);
}

.blog-pagination-btn-active:hover {
  background: rgba(59, 130, 246, 0.3);
  color: rgb(147, 197, 253);
}

.blog-pagination-ellipsis {
  min-width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgb(148, 163, 184);
  font-size: 0.875rem;
}

/* 列表动画 */
.list-enter-active,
.list-leave-active {
  transition: all 0.5s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(30px);
}

.animate-blob {
  animation: blob 7s infinite;
}
.animation-delay-2000 {
  animation-delay: 2s;
}
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -50px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

/* 返回按钮样式 */
.blog-back-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: rgba(139, 92, 246, 0.2);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(139, 92, 246, 0.3);
  border-radius: 0.75rem;
  color: rgb(196, 181, 253);
  text-decoration: none;
  font-weight: 500;
  font-size: 0.875rem;
  transition: all 0.3s ease;
}

.blog-back-button:hover {
  background: rgba(139, 92, 246, 0.3);
  border-color: rgba(139, 92, 246, 0.5);
  color: white;
  transform: translateX(-4px);
}

.blog-back-icon {
  width: 1.25rem;
  height: 1.25rem;
  flex-shrink: 0;
}

/* 侧边栏卡片样式 */
.blog-sidebar-card {
  background: rgba(30, 41, 59, 0.3);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 1rem;
  padding: 1.5rem;
  transition: all 0.3s ease;
}

.blog-sidebar-card:hover {
  background: rgba(30, 41, 59, 0.5);
  border-color: rgba(255, 255, 255, 0.1);
  transform: translateY(-2px);
}

.blog-sidebar-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: rgb(226, 232, 240);
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.blog-tag-item {
  padding: 0.375rem 0.75rem;
  background: rgba(51, 65, 85, 0.5);
  color: rgb(203, 213, 225);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 9999px;
  font-size: 0.75rem;
  transition: all 0.2s ease;
  text-decoration: none;
  display: inline-block;
}

.blog-tag-item:hover {
  background: rgba(59, 130, 246, 0.2);
  color: rgb(147, 197, 253);
  border-color: rgba(59, 130, 246, 0.3);
  transform: scale(1.05);
}

.blog-archive-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.blog-archive-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  color: rgb(148, 163, 184);
  font-size: 0.875rem;
  transition: all 0.2s ease;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.blog-archive-item:hover {
  color: rgb(226, 232, 240);
  padding-left: 0.5rem;
}

.blog-archive-item:last-child {
  border-bottom: none;
}

.blog-archive-count {
  background: rgba(51, 65, 85, 0.5);
  padding: 0.125rem 0.5rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
}

/* 返回AI实验室卡片 */
.blog-back-card {
  background: linear-gradient(135deg, rgba(139, 92, 246, 0.2), rgba(236, 72, 153, 0.2));
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.blog-back-card:hover {
  border-color: rgba(139, 92, 246, 0.5);
  transform: translateY(-4px);
  box-shadow: 0 10px 30px rgba(139, 92, 246, 0.3);
}

.blog-back-card-icon {
  width: 3rem;
  height: 3rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.75rem;
  transition: transform 0.3s ease;
}

.blog-back-card:hover .blog-back-card-icon {
  transform: scale(1.1) rotate(-10deg);
}

.blog-back-card-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: white;
  margin-bottom: 0.25rem;
}

.blog-back-card-subtitle {
  font-size: 0.875rem;
  color: rgba(196, 181, 253, 0.7);
}
</style>
