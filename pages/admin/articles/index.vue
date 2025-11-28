<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">文章管理</h1>
      <button @click="handleNewArticle" class="btn-primary">
        + 新增文章
      </button>
    </div>

    <!-- 搜索栏 -->
    <div class="filter-bar mb-6">
      <input v-model="keyword" @keyup.enter="fetchArticles" type="text" placeholder="搜索文章标题..." class="form-input flex-1" />
      <button @click="fetchArticles" class="btn-secondary">搜索</button>
    </div>

    <!-- 文章列表表格 -->
    <div class="table-container">
      <table class="table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">标题</th>
            <th class="table-header-cell">分类</th>
            <th class="table-header-cell">发布时间</th>
            <th class="table-header-cell">状态</th>
            <th class="table-header-cell">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-if="loading">
            <td colspan="5" class="table-cell text-center loading">加载中...</td>
          </tr>
          <tr v-else-if="articles.length === 0">
            <td colspan="5" class="table-cell text-center empty-state">暂无文章</td>
          </tr>
          <tr v-for="article in articles" :key="article.id" class="table-row">
            <td class="table-cell">{{ article.title }}</td>
            <td class="table-cell">
              <span class="badge badge-blue">{{ article.categoryName || '未分类' }}</span>
            </td>
            <td class="table-cell">{{ formatDate(article.publishTime || article.createdAt) }}</td>
            <td class="table-cell">
              <span :class="article.status === 1 ? 'badge badge-green' : 'badge badge-gray'">
                {{ article.status === 1 ? '已发布' : '草稿' }}
              </span>
            </td>
            <td class="table-cell">
              <div class="flex gap-2">
                <NuxtLink :to="`/admin/articles/edit/${article.id}`" class="btn-link btn-link--blue">编辑</NuxtLink>
                <button @click="handleDelete(article.id)" class="btn-link btn-link--red">删除</button>
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

