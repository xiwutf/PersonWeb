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
    <ClientOnly>
      <n-card>
        <n-data-table
          :columns="columns"
          :data="articles"
          :loading="loading"
          :pagination="pagination"
          :bordered="false"
          remote
        />
      </n-card>
      <template #fallback>
        <n-card>
          <div style="padding: 20px; text-align: center; color: #9ca3af;">
            加载中...
          </div>
        </n-card>
      </template>
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import { h } from 'vue'
import { NButton, NInput, NCard, NDataTable, NTag, NPopconfirm } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { Article, ArticleListResponse } from '~/types/api'
import { useMessage, useDialog } from 'naive-ui'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const route = useRoute()
const api = useApi()
const { handleError } = useErrorHandler()

// 只在客户端使用 Naive UI 的 composables
const message = process.client ? useMessage() : { success: () => {}, error: () => {}, warning: () => {}, info: () => {}, loading: () => {} }
const dialog = process.client ? useDialog() : { warning: () => {}, error: () => {}, info: () => {}, success: () => {} }

const articles = ref<Article[]>([])
const loading = ref(false)
const keyword = ref('')
const page = ref(1)
const pageSize = ref(10)
const total = ref(0)

// 表格列定义
const columns: DataTableColumns<Article> = [
  {
    title: '标题',
    key: 'title',
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '分类',
    key: 'categoryName',
    width: 120,
    render(row) {
      return h(NTag, { type: 'info', size: 'small' }, {
        default: () => row.categoryName || '未分类'
      })
    }
  },
  {
    title: '发布时间',
    key: 'publishTime',
    width: 150,
    render(row) {
      return formatDate(row.publishTime || row.createdAt)
    }
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render(row) {
      return h(NTag, {
        type: row.status === 1 ? 'success' : 'default',
        size: 'small'
      }, {
        default: () => row.status === 1 ? '已发布' : '草稿'
      })
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    render(row) {
      return h('div', { class: 'action-buttons' }, [
        h(NButton, {
          size: 'small',
          type: 'primary',
          quaternary: true,
          onClick: () => router.push(`/admin/articles/edit/${row.id}`)
        }, {
          default: () => '编辑'
        }),
        h(NPopconfirm, {
          onPositiveClick: () => handleDelete(row.id)
        }, {
          trigger: () => h(NButton, {
            size: 'small',
            type: 'error',
            quaternary: true
          }, {
            default: () => '删除'
          }),
          default: () => '确定要删除这篇文章吗？'
        })
      ])
    }
  }
]

// 分页配置
const pagination = computed(() => ({
  page: page.value,
  pageSize: pageSize.value,
  itemCount: total.value,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  onChange: (p: number) => {
    page.value = p
    fetchArticles()
  },
  onUpdatePageSize: (size: number) => {
    pageSize.value = size
    page.value = 1
    fetchArticles()
  }
}))

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

/* 操作按钮组 */
.action-buttons {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}
</style>

