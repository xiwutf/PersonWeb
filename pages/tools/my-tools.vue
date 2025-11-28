<template>
  <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- 页面头部 -->
      <header class="text-center mb-12">
        <h1 class="text-4xl md:text-5xl font-bold mb-4 bg-clip-text text-transparent bg-gradient-to-r from-orange-200 via-white to-red-200">
          我的工具
        </h1>
        <p class="text-lg text-slate-400">管理您已购买的工具</p>
      </header>

      <!-- 工具列表 -->
      <div v-if="loading" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
        <p class="mt-4 text-slate-400">加载中...</p>
      </div>

      <div v-else-if="tools.length === 0" class="text-center py-20">
        <div class="text-6xl mb-4 opacity-50">📦</div>
        <h3 class="text-xl font-semibold text-slate-300 mb-2">还没有工具</h3>
        <p class="text-slate-500 mb-6">去工具商城发现好工具吧</p>
        <NuxtLink
          to="/tools/marketplace"
          class="inline-block px-6 py-3 bg-gradient-to-r from-orange-600 to-red-600 text-white rounded-xl hover:from-orange-500 hover:to-red-500 transition-all"
        >
          浏览工具商城
        </NuxtLink>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="purchase in tools"
          :key="purchase.id"
          class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-3xl p-6 hover:bg-slate-800/50 transition-all"
        >
          <div class="flex items-start justify-between mb-4">
            <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-orange-500/20 to-red-500/20 border border-white/10 flex items-center justify-center text-xl">
              {{ purchase.tool.icon || '🔧' }}
            </div>
            <div class="text-right text-xs text-slate-500">
              <div>{{ formatDate(purchase.purchasedAt) }}</div>
              <div v-if="purchase.expiresAt" class="mt-1">
                到期：{{ formatDate(purchase.expiresAt) }}
              </div>
            </div>
          </div>

          <h3 class="text-xl font-bold mb-2">{{ purchase.tool.name }}</h3>
          <p class="text-slate-400 text-sm mb-4 line-clamp-2">{{ purchase.tool.description }}</p>

          <div class="flex gap-2">
            <NuxtLink
              :to="`/tools/${purchase.tool.slug}`"
              class="flex-1 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 px-4 py-2 rounded-xl transition-all text-center text-sm"
            >
              使用工具
            </NuxtLink>
            <button
              v-if="purchase.tool.apiEndpoint"
              @click="showApiKeyModal(purchase.tool.id)"
              class="px-4 py-2 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 rounded-xl transition-all text-sm"
            >
              API密钥
            </button>
          </div>
        </div>
      </div>

      <!-- API密钥模态框 -->
      <div
        v-if="showApiKeyModal"
        class="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4"
        @click.self="showApiKeyModal = false"
      >
        <div class="bg-slate-800 rounded-3xl p-8 max-w-2xl w-full">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-2xl font-bold">生成 API 密钥</h2>
            <button
              @click="showApiKeyModal = false"
              class="text-slate-400 hover:text-white"
            >
              ✕
            </button>
          </div>

          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium mb-2">密钥名称</label>
              <input
                v-model="apiKeyName"
                type="text"
                placeholder="例如：生产环境密钥"
                class="w-full px-4 py-2 bg-slate-700/50 border border-white/10 rounded-xl text-slate-200 focus:outline-none focus:border-orange-500/50"
              />
            </div>

            <div>
              <label class="block text-sm font-medium mb-2">速率限制（每小时）</label>
              <input
                v-model.number="rateLimit"
                type="number"
                min="1"
                max="10000"
                class="w-full px-4 py-2 bg-slate-700/50 border border-white/10 rounded-xl text-slate-200 focus:outline-none focus:border-orange-500/50"
              />
            </div>

            <div v-if="generatedApiKey" class="p-4 bg-slate-900 rounded-xl">
              <div class="text-sm text-slate-400 mb-2">您的 API 密钥（请妥善保管，只显示一次）：</div>
              <div class="font-mono text-sm bg-slate-800 p-3 rounded break-all">{{ generatedApiKey }}</div>
              <button
                @click="copyApiKey"
                class="mt-2 px-4 py-2 bg-orange-600 hover:bg-orange-500 text-white rounded-lg text-sm"
              >
                复制密钥
              </button>
            </div>

            <div class="flex gap-2">
              <button
                @click="generateApiKey"
                :disabled="generating"
                class="flex-1 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-white px-6 py-3 rounded-xl transition-all font-medium disabled:opacity-50"
              >
                {{ generating ? '生成中...' : '生成密钥' }}
              </button>
              <button
                @click="showApiKeyModal = false"
                class="px-6 py-3 bg-slate-700/50 hover:bg-slate-600/50 text-slate-200 rounded-xl transition-all"
              >
                关闭
              </button>
            </div>
          </div>
        </div>
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
  apiEndpoint?: string
  category?: {
    name: string
    slug: string
  }
}

interface Purchase {
  id: number
  tool: Tool
  purchaseType: string
  expiresAt?: string
  purchasedAt: string
}

const tools = ref<Purchase[]>([])
const loading = ref(false)
const showApiKeyModal = ref(false)
const currentToolId = ref<number | null>(null)
const apiKeyName = ref('')
const rateLimit = ref(1000)
const generatedApiKey = ref('')
const generating = ref(false)

// 获取我的工具
const fetchMyTools = async () => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    return
  }

  loading.value = true
  try {
    const res = await api.post('/Toolbox/my-tools', { userId: visitorId })
    if (res && Array.isArray(res)) {
      tools.value = res as Purchase[]
    }
  } catch (e) {
    console.error('获取我的工具失败', e)
  } finally {
    loading.value = false
  }
}

// 显示API密钥模态框
const showApiKeyModalHandler = (toolId: number) => {
  currentToolId.value = toolId
  showApiKeyModal.value = true
  generatedApiKey.value = ''
  apiKeyName.value = ''
  rateLimit.value = 1000
}

// 生成API密钥
const generateApiKey = async () => {
  if (!currentToolId.value) return

  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    alert('请先登录')
    return
  }

  generating.value = true
  try {
    const res = await api.post('/Toolbox/api-key', {
      toolId: currentToolId.value,
      userId: visitorId,
      keyName: apiKeyName.value || undefined,
      rateLimit: rateLimit.value
    })

    if (res && res.apiKey) {
      generatedApiKey.value = res.apiKey
    }
  } catch (e) {
    alert('生成API密钥失败')
    console.error('生成API密钥失败', e)
  } finally {
    generating.value = false
  }
}

// 复制API密钥
const copyApiKey = () => {
  if (generatedApiKey.value) {
    navigator.clipboard.writeText(generatedApiKey.value)
    alert('已复制到剪贴板')
  }
}

// 格式化日期
const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', { year: 'numeric', month: 'short', day: 'numeric' })
}

onMounted(() => {
  fetchMyTools()
})

useHead({
  title: '我的工具 - 溪午听风',
  meta: [
    { name: 'description', content: '管理您已购买的工具' }
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

