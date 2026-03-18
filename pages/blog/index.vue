<template>
  <div class="blog-page">
    <!-- 全局背景噪点 -->
    <div class="blog-background-noise"></div>

    <!-- 动态背景光斑 -->
    <div class="blog-background-container">
      <div class="blog-background-blob blog-background-blob--blue"></div>
      <div class="blog-background-blob blog-background-blob--emerald"></div>
      <div class="blog-background-blob blog-background-blob--indigo"></div>
    </div>

    <div class="blog-content">
      <!-- 页面头部 -->
      <header class="blog-header">
        <div class="blog-header-icon">
          <span>📝</span>
        </div>
        <h1 class="blog-title">技术博客</h1>
        <p class="blog-subtitle">
          分享技术心得、项目经验与生活感悟，记录成长路上的点点滴滴
        </p>
      </header>

      <!-- 搜索与筛选区域 -->
      <div class="blog-search-section">
        <!-- 搜索框 -->
        <div class="blog-search-container">
          <div class="blog-search-wrapper">
            <div class="blog-search-icon">
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
              </svg>
            </div>
            <input
              v-model="searchKeyword"
              type="text"
              placeholder="搜索文章标题、内容、标签..."
              class="blog-search-input"
            >
            <button
              v-if="searchKeyword"
              @click="clearSearch"
              class="blog-search-clear"
            >
              <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>
        </div>

        <!-- 分类标签 -->
        <div class="blog-categories">
          <button
            v-for="category in categories"
            :key="category"
            @click="selectCategory(category)"
            class="blog-category-button"
            :class="category === selectedCategory ? 'blog-category-button--active' : 'blog-category-button--inactive'"
          >
            {{ category }}
          </button>
        </div>
      </div>

      <!-- 文章统计和分页信息 -->
      <div v-if="filteredPosts.length > 0" class="blog-info-bar">
        <div class="blog-info-text">
          共找到 <span class="blog-info-count">{{ filteredPosts.length }}</span> 篇文章
          <span v-if="currentPage > 1" class="blog-info-page">
            第 {{ currentPage }} / {{ totalPages }} 页
          </span>
        </div>
        <div class="blog-page-size-container">
          <label class="blog-page-size-label">每页显示：</label>
          <select v-model="pageSize" class="blog-page-size-select">
            <option :value="10">10</option>
            <option :value="20">20</option>
            <option :value="30">30</option>
          </select>
        </div>
      </div>

      <!-- 文章列表 -->
      <div class="blog-posts-list">
        <div v-if="filteredPosts.length === 0" class="blog-empty">
          <div class="blog-empty-icon">📭</div>
          <h3 class="blog-empty-title">暂无相关文章</h3>
          <p class="blog-empty-text">换个关键词试试？</p>
        </div>

        <TransitionGroup name="blog-list" tag="div">
          <article
            v-for="(post, index) in paginatedPosts"
            :key="post.id || post._path"
            class="blog-post-card"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <div class="blog-post-card-inner">
              <!-- 分类图标/封面 -->
              <div class="blog-post-icon">
                <NuxtLink :to="getArticlePath(post)" class="blog-post-icon-link">
                  <div class="blog-post-icon-container">
                    <div class="blog-post-icon-overlay"></div>
                    <span class="blog-post-icon-text">{{ getCategoryIcon(post.category) }}</span>
                  </div>
                </NuxtLink>
              </div>

              <!-- 内容区域 -->
              <div class="blog-post-content">
                <div class="blog-post-meta">
                  <NuxtLink 
                    :to="`/blog?category=${encodeURIComponent(post.category)}`" 
                    class="blog-post-category-link"
                    @click.stop
                  >
                    {{ post.category }}
                  </NuxtLink>
                  <span class="blog-post-meta-item">
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                    </svg>
                    {{ formatDate(post.date) }}
                  </span>
                  <span v-if="post.viewCount" class="blog-post-meta-item">
                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                    </svg>
                    {{ post.viewCount }}
                  </span>
                </div>

                <NuxtLink :to="getArticlePath(post)" class="blog-post-title-link">
                  <h2 class="blog-post-title">
                    {{ post.title }}
                  </h2>
                  <p class="blog-post-description">
                    {{ post.description }}
                  </p>
                </NuxtLink>

                <!-- 底部信息 -->
                <div class="blog-post-footer">
                  <div class="blog-post-tags">
                    <NuxtLink
                      v-for="tag in post.tags"
                      :key="tag"
                      :to="`/blog?search=${encodeURIComponent(tag)}`"
                      class="blog-post-tag"
                      @click.stop
                    >
                      #{{ tag }}
                    </NuxtLink>
                  </div>
                  <NuxtLink :to="getArticlePath(post)" class="blog-post-read-more">
                    阅读全文 →
                  </NuxtLink>
                </div>
              </div>
            </div>
          </article>
        </TransitionGroup>
      </div>

      <!-- 分页器 -->
      <div v-if="totalPages > 1" class="blog-pagination">
        <button
          @click="goToPage(currentPage - 1)"
          :disabled="currentPage === 1"
          class="blog-pagination-button"
        >
          <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
          </svg>
        </button>
        
        <div class="blog-pagination-numbers">
          <template v-for="page in visiblePages" :key="page">
            <button
              v-if="page !== -1"
              @click="goToPage(page)"
              class="blog-pagination-button"
              :class="page === currentPage ? 'blog-pagination-button--active' : ''"
            >
              {{ page }}
            </button>
            <span v-else class="blog-pagination-ellipsis">...</span>
          </template>
        </div>
        
        <button
          @click="goToPage(currentPage + 1)"
          :disabled="currentPage === totalPages"
          class="blog-pagination-button"
        >
          <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
          </svg>
        </button>
      </div>

      <!-- 归档区域 -->
      <div class="blog-sidebar">
        <!-- 热门标签 -->
        <div class="blog-sidebar-card">
          <h3 class="blog-sidebar-title">
            <span class="blog-sidebar-icon blog-sidebar-icon--blue">🏷️</span> 热门标签
          </h3>
          <div class="blog-tags">
            <NuxtLink
              v-for="tagInfo in popularTags"
              :key="tagInfo.tag"
              :to="`/blog?search=${encodeURIComponent(tagInfo.tag)}`"
              class="blog-tag-item"
            >
              {{ tagInfo.tag }} <span class="blog-tag-count">{{ tagInfo.count }}</span>
            </NuxtLink>
          </div>
        </div>

        <!-- 时间归档 -->
        <div class="blog-sidebar-card">
          <h3 class="blog-sidebar-title">
            <span class="blog-sidebar-icon blog-sidebar-icon--emerald">📅</span> 时间归档
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
            <span class="blog-sidebar-icon blog-sidebar-icon--purple">📚</span> 分类统计
          </h3>
          <ul class="blog-archive-list">
            <li
              v-for="cat in categories.filter(c => c !== '全部文章').slice(0, 6)"
              :key="cat"
              class="blog-archive-item blog-archive-item--clickable"
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
            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
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
import { ref, computed, watch } from 'vue'

// 确保使用 default 布局（包含 Header）
definePageMeta({
  layout: 'default'
})

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
    '生活随笔': '🌿',
    '运维部署': '⚙️'
  }
  return icons[category] || '📝'
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
/* 页面特有样式已移至 assets/css/blog.css */
/* 这里只保留组件特有的样式（如果有） */

.blog-back-button-container {
  margin-bottom: 2rem;
}

.blog-page-size-container {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.blog-page-size-label {
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.blog-info-page {
  margin-left: 0.5rem;
}

.blog-post-card-inner {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

@media (min-width: 768px) {
  .blog-post-card-inner {
    flex-direction: row;
    gap: 2rem;
  }
}

.blog-pagination-numbers {
  display: flex;
  gap: 0.125rem;
}

.blog-sidebar {
  margin-top: 5rem;
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.5rem;
}

@media (min-width: 768px) {
  .blog-sidebar {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1024px) {
  .blog-sidebar {
    grid-template-columns: repeat(4, 1fr);
  }
}

.blog-sidebar-icon {
  font-size: 1.25rem;
}

.blog-sidebar-icon--blue {
  color: var(--color-primary-soft);
}

.blog-sidebar-icon--emerald {
  color: var(--color-success-400);
}

.blog-sidebar-icon--purple {
  color: var(--color-purple-400);
}

.blog-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.blog-tag-count {
  opacity: 0.5;
  margin-left: 0.25rem;
}

.blog-archive-item--clickable {
  cursor: pointer;
}
</style>
