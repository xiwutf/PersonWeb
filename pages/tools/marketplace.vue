<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <!-- 全局背景 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 返回按钮 -->
      <div class="mb-6">
        <NuxtLink
          to="/tools"
          class="inline-flex items-center gap-2 px-4 py-2 bg-slate-800/50 hover:bg-slate-700/50 border border-white/10 rounded-lg transition-all text-slate-300 hover:text-white"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          <span>返回插件工具</span>
        </NuxtLink>
      </div>

      <!-- 页面头部 -->
      <header class="text-center mb-12">
        <h1 class="text-4xl md:text-5xl font-bold mb-4 bg-clip-text text-transparent bg-gradient-to-r from-orange-200 via-white to-red-200">
          工具商城
        </h1>
        <p class="text-lg text-slate-400 max-w-2xl mx-auto">
          发现专业工具，提升工作效率
        </p>
      </header>

      <!-- 筛选栏 -->
      <div class="mb-8 flex flex-wrap gap-4 items-center justify-between">
        <!-- 分类筛选 -->
        <div class="flex flex-wrap gap-2">
          <button
            v-for="cat in categories"
            :key="cat.id"
            @click="selectedCategory = selectedCategory === cat.slug ? null : cat.slug"
            :class="[
              'px-4 py-2 rounded-xl transition-all text-sm font-medium',
              selectedCategory === cat.slug
                ? 'bg-orange-600 text-white'
                : 'bg-slate-800/50 text-slate-300 hover:bg-slate-700/50'
            ]"
          >
            <span class="mr-2">{{ cat.icon }}</span>
            {{ cat.name }}
            <span class="ml-2 text-xs opacity-75">({{ cat.toolCount }})</span>
          </button>
        </div>

        <!-- 搜索和排序 -->
        <div class="flex gap-2 flex-1 max-w-md">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="搜索工具..."
            class="flex-1 px-4 py-2 bg-slate-800/50 border border-white/10 rounded-xl text-slate-200 placeholder-slate-500 focus:outline-none focus:border-orange-500/50"
          />
          <select
            v-model="sortBy"
            class="px-4 py-2 bg-slate-800/50 border border-white/10 rounded-xl text-slate-200 focus:outline-none focus:border-orange-500/50"
          >
            <option value="popular">最受欢迎</option>
            <option value="newest">最新发布</option>
            <option value="rating">最高评分</option>
            <option value="price">价格从低到高</option>
            <option value="price_desc">价格从高到低</option>
          </select>
        </div>
      </div>

      <!-- 工具列表 -->
      <div v-if="loading" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
        <p class="mt-4 text-slate-400">加载中...</p>
      </div>

      <div v-else-if="error" class="text-center py-20">
        <div class="text-6xl mb-4 opacity-50">😵</div>
        <h3 class="text-xl font-semibold text-slate-300 mb-2">加载失败</h3>
        <p class="text-slate-500">{{ error }}</p>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
        <TransitionGroup name="list">
          <div
            v-for="tool in tools"
            :key="tool.id"
            class="group relative bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all duration-500 hover:border-orange-500/30 hover:shadow-[0_0_30px_rgba(249,115,22,0.15)] flex flex-col"
          >
            <!-- 封面图片 -->
            <div v-if="tool.coverImage" class="h-48 overflow-hidden">
              <img :src="tool.coverImage" :alt="tool.name" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
            </div>

            <!-- 卡片内容 -->
            <div class="p-6 flex-1 flex flex-col">
              <div class="flex items-start justify-between mb-4">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-orange-500/20 to-red-500/20 border border-white/10 flex items-center justify-center text-xl">
                  {{ tool.icon || '🔧' }}
                </div>
                <div class="text-right">
                  <div v-if="tool.isFree" class="text-sm font-bold text-emerald-400">免费</div>
                  <div v-else>
                    <div class="text-xl font-bold text-emerald-400">¥{{ tool.price }}</div>
                    <div v-if="tool.originalPrice" class="text-xs text-slate-500 line-through">¥{{ tool.originalPrice }}</div>
                  </div>
                </div>
              </div>

              <h3 class="text-xl font-bold text-slate-100 mb-2 group-hover:text-orange-400 transition-colors">
                {{ tool.name }}
              </h3>
              
              <p class="text-slate-400 text-sm mb-4 flex-1 line-clamp-2">
                {{ tool.description }}
              </p>

              <!-- 标签和分类 -->
              <div class="flex flex-wrap gap-2 mb-4">
                <span
                  v-for="tag in tool.tags"
                  :key="tag"
                  class="px-2 py-1 bg-slate-700/50 text-slate-300 text-xs rounded-md"
                >
                  {{ tag }}
                </span>
              </div>

              <!-- 统计信息 -->
              <div class="flex items-center gap-4 text-xs text-slate-500 mb-4">
                <span>⭐ {{ tool.rating.toFixed(1) }} ({{ tool.ratingCount }})</span>
                <span>📦 {{ tool.purchaseCount }} 购买</span>
                <span>👆 {{ tool.useCount }} 使用</span>
              </div>

              <!-- 底部按钮 -->
              <div class="flex gap-2 mt-auto">
                <NuxtLink
                  :to="`/tools/${tool.slug}`"
                  class="flex-1 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 px-4 py-2 rounded-xl transition-all text-center text-sm"
                >
                  查看详情
                </NuxtLink>
                <button
                  @click="handlePurchase(tool)"
                  :disabled="purchasing"
                  class="flex-1 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white px-4 py-2 rounded-xl transition-all text-sm font-medium disabled:opacity-50"
                >
                  {{ tool.isFree ? '免费获取' : '立即购买' }}
                </button>
              </div>
            </div>
          </div>
        </TransitionGroup>
      </div>

      <!-- 分页 -->
      <div v-if="totalPages > 1" class="mt-12 flex justify-center gap-2">
        <button
          v-for="page in totalPages"
          :key="page"
          @click="currentPage = page"
          :class="[
            'px-4 py-2 rounded-xl transition-all',
            currentPage === page
              ? 'bg-orange-600 text-white'
              : 'bg-slate-800/50 text-slate-300 hover:bg-slate-700/50'
          ]"
        >
          {{ page }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()

interface Tool {
  id: number
  name: string
  slug: string
  icon?: string
  description?: string
  coverImage?: string
  price: number
  originalPrice?: number
  isFree: boolean
  purchaseCount: number
  useCount: number
  rating: number
  ratingCount: number
  tags: string[]
}

interface Category {
  id: number
  name: string
  slug: string
  icon?: string
  toolCount: number
}

const categories = ref<Category[]>([])
const tools = ref<Tool[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const selectedCategory = ref<string | null>(null)
const searchQuery = ref('')
const sortBy = ref('popular')
const currentPage = ref(1)
const pageSize = 20
const totalPages = ref(1)
const purchasing = ref(false)

// 获取分类
const fetchCategories = async () => {
  try {
    const res = await api.get('/Toolbox/categories')
    if (res) {
      categories.value = res as Category[]
    }
  } catch (e) {
    console.error('获取分类失败', e)
  }
}

// 获取工具列表
const fetchTools = async () => {
  loading.value = true
  error.value = null
  try {
    const params = new URLSearchParams({
      page: currentPage.value.toString(),
      pageSize: pageSize.toString(),
      sortBy: sortBy.value
    })
    if (selectedCategory.value) {
      params.append('category', selectedCategory.value)
    }
    if (searchQuery.value) {
      params.append('search', searchQuery.value)
    }

    const res = await api.get(`/Toolbox/marketplace?${params.toString()}`)
    if (res) {
      tools.value = res.tools as Tool[]
      totalPages.value = res.totalPages || 1
    }
  } catch (e) {
    error.value = '加载工具列表失败'
    console.error('获取工具列表失败', e)
  } finally {
    loading.value = false
  }
}

// 购买工具
const handlePurchase = async (tool: Tool) => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    alert('请先登录')
    return
  }

  purchasing.value = true
  try {
    const res = await api.post('/Toolbox/purchase', {
      toolId: tool.id,
      userId: visitorId,
      purchaseType: 'one_time'
    })

    if (res) {
      if (tool.isFree) {
        alert('工具已添加到我的工具')
      } else {
        alert('购买成功，请完成支付')
      }
    }
  } catch (e) {
    alert('购买失败，请重试')
    console.error('购买失败', e)
  } finally {
    purchasing.value = false
  }
}

// 监听筛选条件变化
watch([selectedCategory, searchQuery, sortBy, currentPage], () => {
  currentPage.value = 1
  fetchTools()
})

onMounted(() => {
  fetchCategories()
  fetchTools()
})

useHead({
  title: '工具商城 - 溪午听风',
  meta: [
    { name: 'description', content: '发现专业工具，提升工作效率' }
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

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

