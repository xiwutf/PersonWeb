<template>
  <div class="min-h-screen bg-slate-50 py-12">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- 头部 -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-slate-900 mb-2">个人知识库</h1>
        <p class="text-slate-600">记录学习、思考与成长</p>
      </div>

      <!-- 筛选栏 -->
      <div class="bg-white rounded-lg shadow-sm p-4 mb-6 flex flex-wrap gap-4">
        <select v-model="categoryFilter" @change="fetchList" class="border border-gray-300 rounded px-3 py-2">
          <option value="">全部分类</option>
          <option value="开发笔记">开发笔记</option>
          <option value="踩坑记录">踩坑记录</option>
          <option value="想法灵感">想法灵感</option>
        </select>

        <input
          v-model="searchKeyword"
          type="text"
          placeholder="搜索关键词..."
          class="flex-1 min-w-[200px] border border-gray-300 rounded px-3 py-2"
          @keyup.enter="fetchList"
        />

        <button
          @click="fetchList"
          class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          搜索
        </button>
      </div>

      <!-- 知识库列表 -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="item in knowledgeList"
          :key="item.id"
          class="bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow p-6 cursor-pointer"
          @click="viewDetail(item.id)"
        >
          <div class="flex items-start justify-between mb-3">
            <span
              class="px-2 py-1 text-xs rounded"
              :class="getCategoryClass(item.category)"
            >
              {{ item.category || '未分类' }}
            </span>
            <span class="text-xs text-gray-500">{{ formatDate(item.updatedAt) }}</span>
          </div>
          <h3 class="text-lg font-semibold text-slate-900 mb-2 line-clamp-2">
            {{ item.title }}
          </h3>
          <div class="flex items-center justify-between text-sm text-gray-500">
            <span>👁 {{ item.viewCount }}</span>
            <div v-if="item.tags" class="flex gap-1">
              <span
                v-for="tag in parseTags(item.tags)"
                :key="tag"
                class="px-2 py-0.5 bg-gray-100 rounded text-xs"
              >
                {{ tag }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div v-if="loading" class="text-center py-8 text-gray-500">加载中...</div>
      <div v-if="!loading && knowledgeList.length === 0" class="text-center py-8 text-gray-500">
        暂无知识库内容
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const router = useRouter()

const knowledgeList = ref<any[]>([])
const loading = ref(false)
const categoryFilter = ref('')
const searchKeyword = ref('')

const fetchList = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/KnowledgeBase', {
      params: {
        category: categoryFilter.value || undefined,
        keyword: searchKeyword.value || undefined,
        page: 1,
        pageSize: 20
      }
    })

    if (res && res.List) {
      knowledgeList.value = res.List
    } else if (Array.isArray(res)) {
      knowledgeList.value = res
    } else {
      knowledgeList.value = []
    }
  } catch (e) {
    console.error('Failed to fetch knowledge base:', e)
    knowledgeList.value = []
  } finally {
    loading.value = false
  }
}

const viewDetail = (id: number) => {
  router.push(`/knowledge/${id}`)
}

const getCategoryClass = (category: string) => {
  const classes: Record<string, string> = {
    '开发笔记': 'bg-blue-100 text-blue-700',
    '踩坑记录': 'bg-orange-100 text-orange-700',
    '想法灵感': 'bg-purple-100 text-purple-700'
  }
  return classes[category || ''] || 'bg-gray-100 text-gray-700'
}

const parseTags = (tags: string) => {
  if (!tags) return []
  try {
    const parsed = JSON.parse(tags)
    return Array.isArray(parsed) ? parsed : []
  } catch {
    return tags.split(',').map(t => t.trim()).filter(t => t)
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

onMounted(() => {
  fetchList()
})
</script>

