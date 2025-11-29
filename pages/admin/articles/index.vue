<template>
  <div class="articles-page">
    <div class="page-header">
      <h1 class="page-title">文章管理</h1>
      <n-button type="primary" @click="handleNewArticle">
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        新增文章
      </n-button>
    </div>

    <!-- 搜索栏 -->
    <div class="filter-bar">
      <n-input
        v-model:value="keyword"
        placeholder="搜索文章标题..."
        clearable
        @keyup.enter="fetchArticles"
        style="flex: 1"
      >
        <template #prefix>
          <i class="fas fa-search"></i>
        </template>
      </n-input>
      <n-button type="primary" @click="fetchArticles">搜索</n-button>
    </div>

    <!-- 文章列表表格 -->
    <div class="table-container">
      <div v-if="loading" class="table-loading">
        加载中...
      </div>
      <div v-else-if="articles.length === 0" class="table-empty">
        暂无文章
      </div>
      <table v-else class="data-table">
        <thead class="table-header">
          <tr>
            <th>标题</th>
            <th>分类</th>
            <th>发布时间</th>
            <th>状态</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="article in articles" :key="article.id" class="table-row">
            <td class="table-cell">{{ article.title }}</td>
            <td class="table-cell">
              <span class="tag tag-info">
                {{ article.categoryName || '未分类' }}
              </span>
            </td>
            <td class="table-cell">{{ formatDate(article.publishTime || article.createdAt) }}</td>
            <td class="table-cell">
              <span 
                class="tag"
                :class="article.status === 1 ? 'tag-success' : 'tag-default'"
              >
                {{ article.status === 1 ? '已发布' : '草稿' }}
              </span>
            </td>
            <td class="table-cell">
              <div class="action-buttons">
                <button 
                  @click="router.push(`/admin/articles/edit/${article.id}`)" 
                  class="btn-link btn-link-blue"
                >
                  编辑
                </button>
                <button 
                  @click="handleDelete(article.id)" 
                  class="btn-link btn-link-red"
                >
                  删除
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- 分页 -->
      <div v-if="total > 0" class="table-pagination">
        <div class="pagination-info">
          共 {{ total }} 条记录
        </div>
        <div class="pagination-controls">
          <select 
            v-model="pageSize" 
            @change="page = 1; fetchArticles()"
            class="pagination-select"
          >
            <option :value="10">10/页</option>
            <option :value="20">20/页</option>
            <option :value="50">50/页</option>
          </select>
          <div class="pagination-buttons">
            <button 
              @click="page = 1; fetchArticles()"
              :disabled="page === 1"
              class="pagination-btn"
            >
              首页
            </button>
            <button 
              @click="page--; fetchArticles()"
              :disabled="page === 1"
              class="pagination-btn"
            >
              上一页
            </button>
            <span class="pagination-page">
              {{ page }} / {{ Math.ceil(total / pageSize) }}
            </span>
            <button 
              @click="page++; fetchArticles()"
              :disabled="page >= Math.ceil(total / pageSize)"
              class="pagination-btn"
            >
              下一页
            </button>
            <button 
              @click="page = Math.ceil(total / pageSize); fetchArticles()"
              :disabled="page >= Math.ceil(total / pageSize)"
              class="pagination-btn"
            >
              末页
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { NButton, NInput } from 'naive-ui'
import type { Article, ArticleListResponse } from '~/types/api'
import { useSafeMessage, useSafeDialog } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 Naive UI 组件在服务端渲染时出错
})

const router = useRouter()
const route = useRoute()
const api = useApi()
const { handleError } = useErrorHandler()

// 使用安全的 Naive UI composables，避免 provider 未挂载时的错误
const message = useSafeMessage()
const dialog = useSafeDialog()

const articles = ref<Article[]>([])
const loading = ref(false)
const keyword = ref('')
const page = ref(1)
const pageSize = ref(10)
const total = ref(0)

const fetchArticles = async () => {
  loading.value = true
  try {
    const res = await api.get<ArticleListResponse>('/Articles', {
      params: {
        page: page.value,
        pageSize: pageSize.value,
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
    message.error('加载文章列表失败')
  } finally {
    loading.value = false
  }
}

const handleDelete = async (id: number) => {
  if (!confirm('确定要删除这篇文章吗？')) return
  
  try {
    await api.del(`/Articles/${id}`)
    message.success('删除成功')
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

<style scoped>
/* 页面容器 */
.articles-page {
  width: 100%;
}

/* 搜索栏 */
.filter-bar {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

/* 表格容器 */
.table-container {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: 0.5rem;
  overflow: hidden;
  margin-bottom: 1.5rem;
  box-shadow: var(--shadow-sm);
}

.table-loading,
.table-empty {
  padding: 2rem;
  text-align: center;
  color: var(--color-text-muted);
  font-weight: 500;
}

/* 数据表格 */
.data-table {
  width: 100%;
  text-align: left;
  border-collapse: collapse;
}

.table-header {
  background: var(--color-bg-elevated);
  border-bottom: 2px solid var(--color-border-subtle);
}

.table-header th {
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-text-main);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.table-body {
  border-collapse: collapse;
}

.table-row {
  border-bottom: 1px solid var(--color-border-subtle);
  transition: background-color 0.2s ease;
}

.table-row:hover {
  background: var(--color-bg-elevated);
}

.table-row:last-child {
  border-bottom: none;
}

.table-cell {
  padding: 1rem 1.5rem;
  color: var(--color-text-main);
  font-size: 0.875rem;
  font-weight: 500;
}

/* 标签样式 - 提高文字对比度 */
.tag {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.tag-info {
  background: var(--color-primary-soft);
  border: 1px solid var(--color-primary);
  color: var(--color-primary);
}

.tag-success {
  background: rgba(34, 197, 94, 0.2);
  border: 1px solid rgba(34, 197, 94, 0.4);
  color: #22c55e;
}

.tag-default {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  color: var(--color-text-main);
}

/* 操作按钮 */
.action-buttons {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  text-decoration: none;
  transition: color 0.2s ease;
  font-size: 0.875rem;
}

.btn-link-blue {
  color: #60a5fa;
}

.btn-link-blue:hover {
  color: #93c5fd;
}

.btn-link-red {
  color: #f87171;
}

.btn-link-red:hover {
  color: #fca5a5;
}

/* 分页样式 */
.table-pagination {
  padding: 1rem 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.pagination-info {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.6);
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.pagination-select {
  padding: 0.25rem 0.75rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.25rem;
  background: rgba(255, 255, 255, 0.1);
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.875rem;
  cursor: pointer;
}

.pagination-select:hover {
  background: rgba(255, 255, 255, 0.15);
}

.pagination-buttons {
  display: flex;
  gap: 0.25rem;
  align-items: center;
}

.pagination-btn {
  padding: 0.25rem 0.75rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.25rem;
  background: rgba(255, 255, 255, 0.1);
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.15);
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-page {
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.9);
}
</style>

