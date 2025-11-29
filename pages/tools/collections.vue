<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
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
          工具合集
        </h1>
        <p class="text-lg text-slate-400 max-w-2xl mx-auto">
          精选工具打包，一次购买，全部拥有
        </p>
      </header>

      <!-- 推荐合集 -->
      <div v-if="featuredCollections.length > 0" class="mb-12">
        <h2 class="text-2xl font-bold mb-6">推荐合集</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div
            v-for="collection in featuredCollections"
            :key="collection.id"
            class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all hover:border-orange-500/30"
          >
            <div v-if="collection.coverImage" class="h-48 overflow-hidden">
              <img :src="collection.coverImage" :alt="collection.name" class="w-full h-full object-cover" />
            </div>
            <div class="p-6">
              <div class="flex items-start justify-between mb-4">
                <h3 class="text-2xl font-bold">{{ collection.name }}</h3>
                <div class="text-right">
                  <div class="text-2xl font-bold text-emerald-400">¥{{ collection.price }}</div>
                  <div v-if="collection.originalPrice" class="text-sm text-slate-500 line-through">¥{{ collection.originalPrice }}</div>
                </div>
              </div>
              <p class="text-slate-400 mb-4">{{ collection.description }}</p>
              <div class="flex items-center justify-between">
                <div class="text-sm text-slate-500">
                  包含 {{ collection.toolCount }} 个工具
                </div>
                <button
                  @click="handlePurchaseCollection(collection)"
                  class="px-6 py-2 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white rounded-xl transition-all"
                >
                  立即购买
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 所有合集 -->
      <div>
        <h2 class="text-2xl font-bold mb-6">所有合集</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="collection in collections"
            :key="collection.id"
            class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all"
          >
            <div class="p-6">
              <h3 class="text-xl font-bold mb-2">{{ collection.name }}</h3>
              <p class="text-slate-400 text-sm mb-4 line-clamp-2">{{ collection.description }}</p>
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-lg font-bold text-emerald-400">¥{{ collection.price }}</div>
                  <div class="text-xs text-slate-500">{{ collection.toolCount }} 个工具</div>
                </div>
                <button
                  @click="handlePurchaseCollection(collection)"
                  class="px-4 py-2 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white rounded-xl transition-all text-sm"
                >
                  购买
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()

interface Collection {
  id: number
  name: string
  slug: string
  description?: string
  coverImage?: string
  price: number
  originalPrice?: number
  toolCount: number
  purchaseCount: number
  isFeatured: boolean
}

const collections = ref<Collection[]>([])
const featuredCollections = computed(() => collections.value.filter(c => c.isFeatured))
const loading = ref(false)

// 获取合集列表
const fetchCollections = async () => {
  loading.value = true
  try {
    // TODO: 实现获取合集列表的API
    // const res = await api.get('/Toolbox/collections')
    // if (res) {
    //   collections.value = res as Collection[]
    // }
  } catch (e) {
    console.error('获取合集列表失败', e)
  } finally {
    loading.value = false
  }
}

// 购买合集
const handlePurchaseCollection = async (collection: Collection) => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    alert('请先登录')
    return
  }

  // TODO: 实现购买合集的API
  alert('购买合集功能开发中')
}

onMounted(() => {
  fetchCollections()
})

useHead({
  title: '工具合集 - 溪午听风',
  meta: [
    { name: 'description', content: '精选工具打包，一次购买，全部拥有' }
  ]
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

