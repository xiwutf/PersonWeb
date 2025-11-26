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

      <!-- 文章列表 -->
      <div class="grid gap-8">
        <div v-if="filteredPosts.length === 0" class="text-center py-20">
          <div class="text-6xl mb-6 opacity-50">🍃</div>
          <h3 class="text-xl font-semibold text-slate-300 mb-2">暂无相关文章</h3>
          <p class="text-slate-500">换个关键词试试？</p>
        </div>

        <TransitionGroup name="list" tag="div" class="space-y-6">
          <article
            v-for="(post, index) in filteredPosts"
            :key="post._path"
            class="group relative bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 sm:p-8 hover:bg-slate-800/50 transition-all duration-500 hover:border-blue-500/30 hover:shadow-[0_0_30px_rgba(59,130,246,0.1)]"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <div class="flex flex-col md:flex-row gap-6 md:gap-8">
              <!-- 分类图标/封面 -->
              <div class="flex-shrink-0">
                <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-slate-700/50 to-slate-800/50 border border-white/10 flex items-center justify-center text-3xl shadow-inner group-hover:scale-110 transition-transform duration-500">
                  {{ getCategoryIcon(post.category) }}
                </div>
              </div>

              <!-- 内容区域 -->
              <div class="flex-1 min-w-0">
                <div class="flex flex-wrap items-center gap-3 text-sm text-slate-500 mb-3">
                  <span class="px-2.5 py-0.5 rounded-md bg-blue-500/10 text-blue-400 border border-blue-500/20 font-medium">
                    {{ post.category }}
                  </span>
                  <span class="flex items-center gap-1">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                    {{ formatDate(post.date) }}
                  </span>
                </div>

                <NuxtLink :to="post._path" class="block group-hover:translate-x-1 transition-transform duration-300">
                  <h2 class="text-2xl font-bold text-slate-100 mb-3 group-hover:text-blue-400 transition-colors line-clamp-1">
                    {{ post.title }}
                  </h2>
                  <p class="text-slate-400 leading-relaxed mb-4 line-clamp-2 group-hover:text-slate-300 transition-colors">
                    {{ post.description }}
                  </p>
                </NuxtLink>

                <!-- 底部信息 -->
                <div class="flex items-center justify-between mt-4 pt-4 border-t border-white/5">
                  <div class="flex flex-wrap gap-2">
                    <span v-for="tag in post.tags" :key="tag" class="text-xs text-slate-500 hover:text-blue-400 transition-colors cursor-pointer">
                      #{{ tag }}
                    </span>
                  </div>
                  <NuxtLink :to="post._path" class="inline-flex items-center gap-1 text-sm font-medium text-blue-400 hover:text-blue-300 transition-colors group/link">
                    阅读全文
                    <svg class="w-4 h-4 group-hover/link:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 8l4 4m0 0l-4 4m4-4H3"></path></svg>
                  </NuxtLink>
                </div>
              </div>
            </div>
          </article>
        </TransitionGroup>
      </div>

      <!-- 归档区域 (简化版) -->
      <div class="mt-20 grid md:grid-cols-3 gap-6">
        <!-- 热门标签 -->
        <div class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6">
          <h3 class="text-lg font-semibold text-slate-200 mb-4 flex items-center gap-2">
            <span class="text-blue-400">🏷️</span> 热门标签
          </h3>
          <div class="flex flex-wrap gap-2">
            <span 
              v-for="tagInfo in popularTags" 
              :key="tagInfo.tag"
              class="px-3 py-1 bg-slate-700/50 hover:bg-blue-500/20 text-slate-300 hover:text-blue-300 border border-white/5 hover:border-blue-500/30 rounded-full text-xs transition-all cursor-pointer"
            >
              {{ tagInfo.tag }} <span class="opacity-50 ml-1">{{ tagInfo.count }}</span>
            </span>
          </div>
        </div>

        <!-- 时间归档 -->
        <div class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6">
          <h3 class="text-lg font-semibold text-slate-200 mb-4 flex items-center gap-2">
            <span class="text-emerald-400">📅</span> 时间归档
          </h3>
          <ul class="space-y-2 text-sm text-slate-400">
            <li v-for="archive in timeArchives.slice(0, 5)" :key="archive.period" class="flex justify-between items-center hover:text-slate-200 transition-colors cursor-pointer">
              <span>{{ archive.period }}</span>
              <span class="bg-slate-700/50 px-2 py-0.5 rounded-full text-xs">{{ archive.count }}</span>
            </li>
          </ul>
        </div>
        
        <!-- 返回首页 -->
        <div class="bg-gradient-to-br from-blue-600/20 to-purple-600/20 backdrop-blur-md border border-white/10 rounded-2xl p-6 flex flex-col items-center justify-center text-center group cursor-pointer hover:border-blue-500/50 transition-all" @click="navigateTo('/')">
          <div class="w-12 h-12 bg-white/10 rounded-full flex items-center justify-center mb-3 group-hover:scale-110 transition-transform">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"></path></svg>
          </div>
          <h3 class="text-lg font-semibold text-white mb-1">返回首页</h3>
          <p class="text-sm text-blue-200/70">Back to Home</p>
        </div>
      </div>
    </div>
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
    '生活随笔': '📝',
    '运维部署': '🔧'
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

// 计算时间归档
const timeArchives = computed(() => {
  if (!allPosts.value) return []
  const archives = {}
  allPosts.value.forEach(post => {
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

useHead({
  title: '技术博客 - 溪午听风',
  meta: [
    { name: 'description', content: '分享技术心得、项目经验与生活感悟' }
  ]
})
</script>

<style scoped>
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
</style>