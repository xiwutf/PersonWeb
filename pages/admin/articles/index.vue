<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">文章管理</h1>
      <button @click="handleNewArticle" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
        + 新增文章
      </button>
    </div>

    <!-- 搜索栏 -->
    <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6 flex gap-4">
      <input v-model="keyword" @keyup.enter="fetchArticles" type="text" placeholder="搜索文章标题..." class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
      <button @click="fetchArticles" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">搜索</button>
    </div>

    <!-- 文章列表表格 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <table class="w-full text-left">
        <thead class="bg-gray-50 dark:bg-gray-700/50 border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">标题</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">分类</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">发布时间</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">状态</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">操作</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-if="loading">
            <td colspan="5" class="px-6 py-4 text-center text-gray-500 dark:text-gray-400">加载中...</td>
          </tr>
          <tr v-else-if="articles.length === 0">
            <td colspan="5" class="px-6 py-4 text-center text-gray-500 dark:text-gray-400">暂无文章</td>
          </tr>
          <tr v-for="article in articles" :key="article.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">{{ article.title }}</td>
            <td class="px-6 py-4">
              <span class="px-2 py-1 bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300 rounded text-xs">{{ article.categoryName || '未分类' }}</span>
            </td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400 text-sm">{{ formatDate(article.publishTime || article.createdAt) }}</td>
            <td class="px-6 py-4">
              <span :class="article.status === 1 ? 'text-green-600 dark:text-green-400' : 'text-gray-500 dark:text-gray-400'" class="text-sm">
                {{ article.status === 1 ? '已发布' : '草稿' }}
              </span>
            </td>
            <td class="px-6 py-4">
              <div class="flex gap-2">
                <NuxtLink :to="`/admin/articles/edit/${article.id}`" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">编辑</NuxtLink>
                <button @click="handleDelete(article.id)" class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300">删除</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Article, ArticleListResponse } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const route = useRoute()
const api = useApi()

const articles = ref<Article[]>([])
const loading = ref(false)
const keyword = ref('')
const page = ref(1)
const total = ref(0)

const fetchArticles = async () => {
  loading.value = true
  try {
    const res = await api.get<ArticleListResponse>('/Articles', {
      params: {
        page: page.value,
        pageSize: 10,
        keyword: keyword.value
      }
    })
    
    // .NET API returns { Total: int, List: [] }
    if (res) {
      if (Array.isArray(res)) {
        articles.value = res
        total.value = res.length
      } else if (res.List !== undefined || res.list !== undefined) {
        articles.value = res.List ?? res.list ?? []
        total.value = res.Total ?? res.total ?? 0
      } else {
        articles.value = []
        total.value = 0
      }
    } else {
      articles.value = []
      total.value = 0
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('[Articles] Failed to fetch articles:', e)
    }
    articles.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

const handleDelete = async (id: number) => {
  if (!confirm('确定要删除这篇文章吗？')) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.del(`/Articles/${id}`)
    success('删除成功')
    fetchArticles() // 刷新列表
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleDateString()
}

// 新增文章
const handleNewArticle = () => {
  // 直接使用 window.location 进行硬跳转，确保页面刷新
  window.location.href = '/admin/articles/edit'
}

// 初始加载
onMounted(() => {
  fetchArticles()
})
</script>

