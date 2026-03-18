<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 via-red-50 to-pink-50 py-8">
    <div class="container mx-auto px-4 max-w-4xl">
      <!-- 面包屑导航 -->
      <nav class="mb-8">
        <div class="flex items-center space-x-2 text-sm text-gray-600">
          <NuxtLink to="/" class="hover:text-orange-600 transition-colors">首页</NuxtLink>
          <span>/</span>
          <NuxtLink to="/tools" class="hover:text-orange-600 transition-colors">插件工具</NuxtLink>
          <span>/</span>
          <span class="text-gray-900">{{ tool?.title || '加载中...' }}</span>
        </div>
      </nav>

      <!-- 返回按钮 -->
      <div class="mb-6">
        <NuxtLink
          to="/tools"
          class="inline-flex items-center px-4 py-2 bg-white/70 backdrop-blur-sm rounded-lg shadow-sm hover:shadow-md transition-all duration-200 text-orange-700 hover:text-orange-800 border border-orange-200"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          返回工具列表
        </NuxtLink>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded w-2/3 mx-auto"></div>
        </div>
      </div>

      <!-- 工具内容 -->
      <div v-else-if="tool" class="space-y-8">
        <!-- 工具信息头部 -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
          <div class="flex flex-col lg:flex-row gap-8">
            <div class="lg:w-1/3">
              <div class="h-64 bg-gradient-to-br from-orange-400 to-red-600 rounded-xl flex items-center justify-center">
                <div class="text-center text-var(--color-bg-light, white)">
                  <span class="text-4xl mb-2 block">🔧</span>
                  <p class="text-lg font-semibold">专业工具</p>
                </div>
              </div>
            </div>
            <div class="lg:w-2/3">
              <div class="flex items-center justify-between mb-4">
                <div class="flex items-center space-x-2">
                  <span class="text-3xl font-bold text-green-600">¥{{ tool.price }}</span>
                  <span class="text-sm text-gray-500 line-through">¥{{ Math.round(tool.price * 1.5) }}</span>
                </div>
                <span class="text-sm text-gray-500">{{ formatDate(tool.date) }}</span>
              </div>
              
              <h1 class="text-3xl font-bold text-gray-900 mb-4">{{ tool.title }}</h1>
              <p class="text-gray-600 mb-6 text-lg leading-relaxed">{{ tool.description }}</p>
              
              <!-- 标签 -->
              <div class="mb-6">
                <h3 class="text-sm font-medium text-gray-700 mb-3">特性标签</h3>
                <div class="flex flex-wrap gap-2">
                  <span
                    v-for="tag in tool.tags"
                    :key="tag"
                    class="px-3 py-1 bg-orange-100 text-orange-700 rounded-full text-sm font-medium"
                  >
                    {{ tag }}
                  </span>
                </div>
              </div>

              <!-- 适合人群 / 不适合人群 -->
              <div v-if="tool.fitFor || tool.notFitFor" class="mb-6 grid grid-cols-1 md:grid-cols-2 gap-4">
                <!-- 适合人群 -->
                <div v-if="tool.fitFor" class="bg-green-50 border border-green-200 rounded-lg p-4">
                  <h4 class="text-sm font-semibold text-green-800 mb-2 flex items-center">
                    <i class="fas fa-check-circle mr-2"></i>
                    适合这些人
                  </h4>
                  <div class="text-sm text-green-700 whitespace-pre-line">{{ tool.fitFor }}</div>
                </div>

                <!-- 不适合情况 -->
                <div v-if="tool.notFitFor" class="bg-red-50 border border-red-200 rounded-lg p-4">
                  <h4 class="text-sm font-semibold text-red-800 mb-2 flex items-center">
                    <i class="fas fa-times-circle mr-2"></i>
                    不适合这些情况
                  </h4>
                  <div class="text-sm text-red-700 whitespace-pre-line">{{ tool.notFitFor }}</div>
                </div>
              </div>
              
              <!-- 操作按钮 -->
              <div class="flex flex-wrap gap-4">
                <!-- 咨询按钮（主按钮） -->
                <button
                  @click="showConsultationDialog = true"
                  class="bg-gradient-to-r from-orange-500 to-red-500 text-var(--color-bg-light, white) px-8 py-3 rounded-xl hover:from-orange-600 hover:to-red-600 transition-all font-medium inline-flex items-center gap-2 shadow-lg"
                >
                  <i class="fas fa-comments"></i>
                  咨询
                </button>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 详细内容 -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl overflow-hidden">
          <div class="p-8">
            <div class="prose prose-lg max-w-none">
              <div v-if="tool.content" v-html="renderMarkdown(tool.content)"></div>
              <div v-else class="text-gray-500 italic">暂无详细描述</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 无数据状态 -->
      <div v-else class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="bg-yellow-100 border border-yellow-400 text-yellow-700 px-4 py-3 rounded mb-4">
          <strong>警告:</strong> 没有找到工具数据！<br>
          当前 slug: {{ $route.params.slug }}<br>
          查询错误: {{ error }}
        </div>
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
          <div class="h-4 bg-gray-200 rounded w-2/3"></div>
        </div>
      </div>
    </div>

    <!-- 咨询弹窗 -->
    <ConsultationDialog
      v-model:visible="showConsultationDialog"
      :product-id="tool?.id || 0"
      :product-name="tool?.title || ''"
      @success="handleConsultationSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import MarkdownIt from 'markdown-it'

const route = useRoute()
const router = useRouter()
const api = useApi()
const message = useSafeMessage()
const slug = route.params.slug as string

const tool = ref<any>(null)
const toolId = ref<number>(0)
const loading = ref(true)
const error = ref<string | null>(null)
const showConsultationDialog = ref(false)

// 初始化 markdown 渲染器
const md = new MarkdownIt({
  html: true,
  linkify: true,
  typographer: true
})

// 获取具体工具数据
const fetchTool = async () => {
  loading.value = true
  error.value = null
  try {
    // 先通过 slug 搜索工具
    const searchRes = await api.get<any>('/Toolbox/marketplace', {
      params: { search: slug }
    })
    
    if (searchRes && searchRes.tools && searchRes.tools.length > 0) {
      // 找到匹配的工具（优先精确匹配 slug）
      const matchedTool = searchRes.tools.find((t: any) => t.slug === slug) || searchRes.tools[0]
      const currentToolId = matchedTool.id
      
      // 获取工具详情
      const detailRes = await api.get<any>(`/Toolbox/${currentToolId}`)
      if (detailRes) {
        toolId.value = currentToolId
        // 转换为页面需要的格式
        tool.value = {
          id: detailRes.id,
          title: detailRes.name,
          description: detailRes.description || '',
          price: detailRes.price || 0,
          date: detailRes.createdAt || new Date().toISOString(),
          tags: detailRes.tags || [],
          buy_link: detailRes.isFree ? '#' : '#',
          _path: `/tools/${detailRes.slug}`,
          content: detailRes.detailedDescription || detailRes.description || '',
          fitFor: detailRes.fitFor || null,
          notFitFor: detailRes.notFitFor || null,
          enableOnlineOrder: detailRes.enableOnlineOrder === true || detailRes.enableOnlineOrder === 1
        }
        
        console.log('工具详情加载完成:', {
          id: tool.value.id,
          enableOnlineOrder: tool.value.enableOnlineOrder,
          detailResEnableOnlineOrder: detailRes.enableOnlineOrder
        })
      }
    } else {
      error.value = '未找到工具数据'
    }
  } catch (e: any) {
    console.error('获取工具详情失败:', e)
    error.value = e.message || '获取工具数据失败'
  } finally {
    loading.value = false
  }
}

// 格式化日期
const formatDate = (dateString: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// 渲染 Markdown
const renderMarkdown = (markdown: string) => {
  if (!markdown) return ''
  return md.render(markdown)
}

// 处理直接下单
const handleDirectOrder = () => {
  console.log('点击直接下单，工具信息:', {
    id: tool.value?.id,
    enableOnlineOrder: tool.value?.enableOnlineOrder
  })
  
  if (tool.value?.id) {
    // 使用 router.push 确保在当前窗口跳转，而不是新开窗口
    const targetUrl = `/order/create?productId=${tool.value.id}`
    console.log('跳转到:', targetUrl)
    router.push(targetUrl)
  } else {
    console.error('工具 ID 不存在，无法跳转到下单页面')
    message.error('工具信息不完整，请刷新页面重试')
  }
}

// 处理咨询成功
const handleConsultationSuccess = () => {
  // 咨询提交成功后的处理（可以显示提示等）
  console.log('咨询提交成功')
}

onMounted(() => {
  fetchTool()
})

// 设置页面标题和SEO
useHead({
  title: computed(() => `${tool.value?.title || '工具详情'} - 插件工具 - 溪午听风`),
  meta: [
    { name: 'description', content: computed(() => tool.value?.description || '工具详情页面') },
    { name: 'keywords', content: computed(() => tool.value?.tags?.join(', ') || '插件工具') }
  ]
})
</script>

<style scoped>
/* 优化代码块背景 */
:deep(.prose pre) {
  @apply bg-gray-900 text-gray-100;
  border-radius: 12px;
  padding: 1.5rem;
  overflow-x: auto;
}

:deep(.prose code) {
  @apply bg-gray-100 text-gray-800 px-1 py-0.5 rounded text-sm;
}

:deep(.prose pre code) {
  @apply bg-transparent text-gray-100 px-0 py-0;
}

/* 优化标题样式 */
:deep(.prose h1) {
  @apply text-2xl font-bold text-gray-800 mb-4;
}

:deep(.prose h2) {
  @apply text-xl font-semibold text-gray-800 mb-3 mt-6;
}

:deep(.prose h3) {
  @apply text-lg font-medium text-gray-800 mb-2 mt-4;
}

/* 优化列表样式 */
:deep(.prose ul) {
  @apply list-disc pl-6 mb-4;
}

:deep(.prose ol) {
  @apply list-decimal pl-6 mb-4;
}

:deep(.prose li) {
  @apply mb-1;
}

/* 优化表格样式 */
:deep(.prose table) {
  @apply border-collapse border border-gray-200 rounded-lg overflow-hidden;
}

:deep(.prose th) {
  @apply bg-gray-50 border border-gray-200 px-4 py-2;
}

:deep(.prose td) {
  @apply border border-gray-200 px-4 py-2;
}

/* 优化引用样式 */
:deep(.prose blockquote) {
  @apply border-l-4 border-orange-500 bg-orange-50 pl-4 py-2 my-4 rounded-r-lg;
}
</style> 
