<template>
  <div class="min-h-screen bg-[var(--color-text-main)] text-slate-200 relative overflow-hidden font-['Outfit']">
    <div class="relative z-10 max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 加载状态 -->
      <div v-if="loading" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
        <p class="mt-4 text-slate-400">加载中...</p>
      </div>

      <!-- 工具详情 -->
      <div v-else-if="tool" class="space-y-8">
        <!-- 头部信息 -->
        <div class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl p-8">
          <div class="flex flex-col md:flex-row gap-8">
            <!-- 封面图片 -->
            <div class="md:w-1/3">
              <div v-if="tool.coverImage" class="rounded-2xl overflow-hidden">
                <img :src="tool.coverImage" :alt="tool.name" class="w-full h-full object-cover" />
              </div>
              <div v-else class="w-full h-64 rounded-2xl bg-gradient-to-br from-orange-500/20 to-red-500/20 border border-white/10 flex items-center justify-center text-6xl">
                {{ tool.icon || '🔧' }}
              </div>
            </div>

            <!-- 基本信息 -->
            <div class="md:w-2/3 flex-1">
              <div class="flex items-start justify-between mb-4">
                <div>
                  <h1 class="text-4xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r from-orange-200 to-red-200">
                    {{ tool.name }}
                  </h1>
                  <div class="flex items-center gap-4 text-slate-400 mb-4">
                    <span v-if="tool.category">{{ tool.category.icon }} {{ tool.category.name }}</span>
                    <span>v{{ tool.version }}</span>
                    <span v-if="tool.author">作者：{{ tool.author }}</span>
                  </div>
                </div>
                <div class="text-right">
                  <div v-if="tool.isFree" class="text-3xl font-bold text-emerald-400 mb-2">免费</div>
                  <div v-else>
                    <div class="text-3xl font-bold text-emerald-400 mb-2">¥{{ tool.price }}</div>
                    <div v-if="tool.originalPrice" class="text-lg text-slate-500 line-through">¥{{ tool.originalPrice }}</div>
                  </div>
                </div>
              </div>

              <p class="text-slate-300 text-lg mb-6">{{ tool.description }}</p>

              <!-- 统计信息 -->
              <div class="flex items-center gap-6 mb-6">
                <div class="flex items-center gap-2">
                  <span class="text-2xl">⭐</span>
                  <div>
                    <div class="font-bold">{{ tool.rating.toFixed(1) }}</div>
                    <div class="text-xs text-slate-500">{{ tool.ratingCount }} 评价</div>
                  </div>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-2xl">📦</span>
                  <div>
                    <div class="font-bold">{{ tool.purchaseCount }}</div>
                    <div class="text-xs text-slate-500">购买</div>
                  </div>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-2xl">👆</span>
                  <div>
                    <div class="font-bold">{{ tool.useCount }}</div>
                    <div class="text-xs text-slate-500">使用</div>
                  </div>
                </div>
              </div>

              <!-- 操作按钮 -->
              <div class="flex gap-4">
                <button
                  @click="handlePurchase"
                  :disabled="purchasing || hasPurchased"
                  class="flex-1 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white px-6 py-3 rounded-xl transition-all font-medium disabled:opacity-50"
                >
                  {{ hasPurchased ? '已拥有' : tool.isFree ? '免费获取' : '立即购买' }}
                </button>
                <a
                  v-if="tool.demoUrl"
                  :href="tool.demoUrl"
                  target="_blank"
                  class="px-6 py-3 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 rounded-xl transition-all"
                >
                  在线演示
                </a>
                <button
                  v-if="tool.apiEndpoint"
                  @click="showApiDocs = true"
                  class="px-6 py-3 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 rounded-xl transition-all"
                >
                  API文档
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- 详细描述 -->
        <div v-if="tool.detailedDescription" class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl p-8">
          <h2 class="text-2xl font-bold mb-6">详细介绍</h2>
          <div class="prose prose-invert max-w-none" v-html="markdownToHtml(tool.detailedDescription)"></div>
        </div>

        <!-- 功能特性 -->
        <div v-if="tool.features && tool.features.length > 0" class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl p-8">
          <h2 class="text-2xl font-bold mb-6">功能特性</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div
              v-for="feature in tool.features"
              :key="feature"
              class="flex items-center gap-3 p-4 bg-slate-700/30 rounded-xl"
            >
              <span class="text-orange-400">✓</span>
              <span>{{ feature }}</span>
            </div>
          </div>
        </div>

        <!-- 标签 -->
        <div v-if="tool.tags && tool.tags.length > 0" class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl p-8">
          <h2 class="text-2xl font-bold mb-6">标签</h2>
          <div class="flex flex-wrap gap-2">
            <span
              v-for="tag in tool.tags"
              :key="tag"
              class="px-4 py-2 bg-slate-700/50 text-slate-300 rounded-lg"
            >
              {{ tag }}
            </span>
          </div>
        </div>

        <!-- 使用要求 -->
        <div v-if="tool.requirements" class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl p-8">
          <h2 class="text-2xl font-bold mb-6">使用要求</h2>
          <div class="prose prose-invert max-w-none" v-html="markdownToHtml(tool.requirements)"></div>
        </div>
      </div>

      <!-- API文档模态框 -->
      <div
        v-if="showApiDocs"
        class="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4"
        @click.self="showApiDocs = false"
      >
        <div class="bg-slate-800 rounded-3xl p-8 max-w-4xl w-full max-h-[90vh] overflow-y-auto">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-2xl font-bold">API 文档</h2>
            <button
              @click="showApiDocs = false"
              class="text-slate-400 hover:text-white"
            >
              ✕
            </button>
          </div>
          <div v-if="tool?.apiDocumentation" class="prose prose-invert max-w-none">
            <pre class="bg-slate-900 p-4 rounded-lg overflow-x-auto"><code>{{ JSON.stringify(JSON.parse(tool.apiDocumentation), null, 2) }}</code></pre>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const api = useApi()

interface Tool {
  id: number
  name: string
  slug: string
  icon?: string
  description?: string
  detailedDescription?: string
  coverImage?: string
  demoUrl?: string
  price: number
  originalPrice?: number
  isFree: boolean
  purchaseCount: number
  useCount: number
  rating: number
  ratingCount: number
  apiEndpoint?: string
  apiDocumentation?: string
  requirements?: string
  version: string
  author?: string
  category?: {
    name: string
    slug: string
    icon?: string
  }
  tags: string[]
  features: string[]
}

const tool = ref<Tool | null>(null)
const loading = ref(false)
const purchasing = ref(false)
const hasPurchased = ref(false)
const showApiDocs = ref(false)

// 获取工具详情
const fetchTool = async () => {
  loading.value = true
  try {
    // 先通过slug查找工具ID（这里简化处理，实际应该后端支持slug查询）
    const res = await api.get(`/Toolbox/marketplace?search=${route.params.slug}`)
    if (res && res.tools && res.tools.length > 0) {
      const toolId = res.tools[0].id
      const detailRes = await api.get(`/Toolbox/${toolId}`)
      if (detailRes) {
        tool.value = detailRes as Tool
        checkPurchaseStatus()
      }
    }
  } catch (e) {
    console.error('获取工具详情失败', e)
  } finally {
    loading.value = false
  }
}

// 检查购买状态
const checkPurchaseStatus = async () => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId || !tool.value) return

  try {
    const res = await api.post('/Toolbox/my-tools', { userId: visitorId })
    if (res && Array.isArray(res)) {
      hasPurchased.value = res.some((t: any) => t.tool.id === tool.value!.id)
    }
  } catch (e) {
    console.error('检查购买状态失败', e)
  }
}

// 购买工具
const handlePurchase = async () => {
  if (!tool.value) return

  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    alert('请先登录')
    return
  }

  purchasing.value = true
  try {
    const res = await api.post('/Toolbox/purchase', {
      toolId: tool.value.id,
      userId: visitorId,
      purchaseType: 'one_time'
    })

    if (res) {
      hasPurchased.value = true
      alert(tool.value.isFree ? '工具已添加到我的工具' : '购买成功，请完成支付')
    }
  } catch (e) {
    alert('购买失败，请重试')
    console.error('购买失败', e)
  } finally {
    purchasing.value = false
  }
}

// Markdown转HTML（简化版）
const markdownToHtml = (markdown: string) => {
  // 这里可以使用 markdown-it 或其他库
  return markdown.replace(/\n/g, '<br>')
}

onMounted(() => {
  fetchTool()
})

useHead({
  title: tool.value ? `${tool.value.name} - 工具商城` : '工具详情 - 溪午听风',
  meta: [
    { name: 'description', content: tool.value?.description || '工具详情' }
  ]
})
</script>

<style scoped>
.prose {
  color: rgb(226 232 240);
}
.prose h1, .prose h2, .prose h3 {
  color: rgb(255 255 255);
}
</style>
