<template>
  <ClientOnly>
    <div class="intelligence-content-page">
      <!-- 页面标题 -->
      <div class="page-header">
        <h1 class="page-title">情报内容</h1>
        <n-button type="primary" @click="navigateTo('/admin/intelligence')">
          <template #icon>
            <i class="fas fa-arrow-left"></i>
          </template>
          返回首页
        </n-button>
      </div>

      <!-- 筛选栏 -->
      <div class="filters-bar">
        <n-input
          v-model:value="filters.keyword"
          placeholder="搜索标题、内容..."
          clearable
          @keyup.enter="handleSearch"
        >
          <template #prefix>
            <i class="fas fa-search"></i>
          </template>
        </n-input>
        <n-select
          v-model:value="filters.category"
          placeholder="分类筛选"
          clearable
          :options="categoryOptions"
        />
        <n-select
          v-model:value="filters.sourceId"
          placeholder="来源筛选"
          clearable
          :options="sourceOptions"
          label-field="label"
          value-field="value"
        />
        <n-date-picker
          v-model:value="dateRange"
          type="daterange"
          placeholder="日期范围"
          clearable
          class="date-range-picker"
        />
        <n-checkbox v-model:checked="filters.highValueOnly">
          仅看高价内容
        </n-checkbox>
        <n-button type="primary" @click="handleSearch">搜索</n-button>
        <n-button @click="handleReset">重置</n-button>
      </div>

      <!-- 数据表格 -->
      <div class="table-container">
        <div v-if="loading" class="loading-container">
          <n-spin size="large" />
        </div>
        <div v-else-if="contents.length === 0" class="empty-container">
          <i class="fas fa-inbox"></i>
          <p>暂无内容数据</p>
          <n-button type="primary" @click="navigateTo('/admin/intelligence/source')">
            配置采集来源
          </n-button>
        </div>
        <div v-else>
          <!-- 分类统计 -->
          <div class="category-stats-bar">
            <span v-for="stat in categoryStats" :key="stat.category" class="stat-badge">
              {{ stat.category }}: <strong>{{ stat.count }}</strong>
            </span>
          </div>

          <!-- 内容列表 -->
          <div class="content-list">
            <div
              v-for="content in contents"
              :key="content.id"
              class="content-card"
              :class="{ 'high-value': content.relevanceScore >= 70 }"
            >
              <div class="content-header">
                <div class="content-title">{{ content.title }}</div>
                <div class="content-badges">
                  <n-tag
                    v-if="content.relevanceScore >= 70"
                    type="error"
                    size="small"
                    :bordered="false"
                  >
                    高价内容                  </n-tag>
                  <n-tag size="small" :bordered="false">
                    {{ content.category }}
                  </n-tag>
                </div>
              </div>
              <div class="content-summary">{{ content.summary }}</div>
              <div class="content-footer">
                <div class="content-source">{{ content.sourceName }}</div>
                <div class="content-meta">
                  <span>{{ formatTime(content.createdAt) }}</span>
                  <n-tag size="tiny" :bordered="false">
                    相关性: {{ content.relevanceScore }}
                  </n-tag>
                  <n-tag size="tiny" :bordered="false">
                    学习价值: {{ content.learningValue }}
                  </n-tag>
                </div>
                <div class="content-actions">
                  <n-button text size="small" @click="openOriginalUrl(content.originalUrl)">
                    <template #icon>
                      <i class="fas fa-external-link-alt"></i>
                    </template>
                    原文
                  </n-button>
                </div>
              </div>
              <!-- 标签 -->
              <div v-if="content.tags && content.tags.length > 0" class="content-tags">
                <n-tag
                  v-for="tag in content.tags"
                  :key="tag"
                  size="small"
                  :bordered="false"
                  type="info"
                >
                  {{ tag }}
                </n-tag>
              </div>
            </div>
          </div>

          <!-- 分页 -->
          <div class="pagination-container">
            <n-pagination
              v-model:page="pagination.page"
              v-model:page-size="pagination.pageSize"
              :item-count="pagination.total"
              :page-sizes="[10, 20, 50, 100]"
              show-size-picker
              @update:page="handlePageChange"
              @update:page-size="handlePageSizeChange"
            />
          </div>
        </div>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import type { ContentItemDto, CategoryStatDto } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()
const api = useIntelligenceApi()

// 加载状态
const loading = ref(false)
const contents = ref<ContentItemDto[]>([])
const categoryStats = ref<CategoryStatDto[]>([])
const sources = ref<any[]>([])
const dateRange = ref<[number, number] | null>(null)

// 筛选条件
const filters = reactive({
  keyword: '',
  category: '',
  sourceId: null as number | null,
  startDate: '',
  endDate: '',
  highValueOnly: false
})

// 分页
const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

// 计算属性
const categoryOptions = computed(() => {
  return categoryStats.value.map(s => ({
    label: s.category,
    value: s.category
  }))
})

const sourceOptions = computed(() => {
  return sources.value.map(s => ({
    label: s.sourceName,
    value: s.id
  }))
})

// 获取来源列表
const fetchSources = async () => {
  try {
    sources.value = await api.getSourceList()
  } catch (error) {
    console.error('获取来源列表失败:', error)
  }
}

// 获取分类统计
const fetchCategoryStats = async () => {
  try {
    categoryStats.value = await api.getCategoryStats()
  } catch (error) {
    console.error('获取分类统计失败:', error)
  }
}

// 获取内容列表
const fetchContents = async () => {
  loading.value = true
  try {
    // 处理日期范围
    if (dateRange.value) {
      filters.startDate = new Date(dateRange.value[0]).toISOString().split('T')[0]
      filters.endDate = new Date(dateRange.value[1]).toISOString().split('T')[0]
    } else {
      filters.startDate = ''
      filters.endDate = ''
    }

    const params = {
      keyword: filters.keyword || undefined,
      category: filters.category || undefined,
      sourceId: filters.sourceId || undefined,
      startDate: filters.startDate || undefined,
      endDate: filters.endDate || undefined,
      highValueOnly: filters.highValueOnly || undefined,
      pageIndex: pagination.page,
      pageSize: pagination.pageSize
    }

    const result = await api.getContentList(params)
    contents.value = result.list
    pagination.total = result.total
  } catch (error) {
    console.error('获取内容列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.page = 1
  fetchContents()
}

// 重置
const handleReset = () => {
  filters.keyword = ''
  filters.category = ''
  filters.sourceId = null
  filters.highValueOnly = false
  dateRange.value = null
  pagination.page = 1
  fetchContents()
}

// 翻页
const handlePageChange = (page: number) => {
  pagination.page = page
  fetchContents()
}

// 改变每页数量
const handlePageSizeChange = (pageSize: number) => {
  pagination.pageSize = pageSize
  pagination.page = 1
  fetchContents()
}

// 打开原文链接
const openOriginalUrl = (url: string) => {
  window.open(url, '_blank')
}

// 格式化时间
const formatTime = (time: string) => {
  const d = new Date(time)
  const now = new Date()
  const diff = now.getTime() - d.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  if (days < 7) return `${days}天前`
  return d.toLocaleDateString('zh-CN')
}

// 初始化筛选条件
onMounted(async () => {
  if (route.query.category) {
    filters.category = route.query.category as string
  }
  if (route.query.sourceId) {
    filters.sourceId = Number(route.query.sourceId)
  }

  await fetchSources()
  await fetchCategoryStats()
  await fetchContents()
})
</script>

<style scoped>
.intelligence-content-page {
  padding: var(--spacing-lg);
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.page-title {
  font-size: var(--text-2xl);
  font-weight: 600;
  margin: 0;
  color: var(--color-text-main);
}

/* 筛选栏 */
.filters-bar {
  display: flex;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
  flex-wrap: wrap;
  align-items: center;
}

/* 表格容器 */
.table-container {
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  box-shadow: var(--shadow-card);
}

.loading-container,
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: var(--spacing-3xl);
  color: var(--color-text-sub);
}

.empty-container i {
  font-size: var(--text-5xl);
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.empty-container p {
  margin: 0 0 var(--spacing-md) 0;
}

/* 分类统计�?*/
.category-stats-bar {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-md);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid var(--color-border);
}

.stat-badge {
  font-size: var(--text-sm);
  color: var(--color-text-sec);
  padding: var(--spacing-sm) 10px;
  background: var(--color-border);
  border-radius: var(--radius-sm);
}

/* 内容列表 */
.content-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.content-card {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
  transition: all 0.2s;
  cursor: pointer;
}

.content-card:hover {
  border-color: var(--color-primary);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.12);
}

.content-card.high-value {
  border-left: 4px solid var(--color-error);
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--spacing-sm);
  gap: var(--spacing-md);
}

.content-title {
  flex: 1;
  font-weight: 500;
  color: var(--color-text-main);
  font-size: var(--text-base);
  line-height: 1.4;
}

.content-badges {
  display: flex;
  gap: var(--spacing-sm);
  flex-shrink: 0;
}

.content-summary {
  color: var(--color-text-sec);
  font-size: var(--text-base);
  line-height: 1.6;
  margin-bottom: var(--spacing-md);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.content-footer {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: var(--text-sm);
  color: var(--color-text-sec);
  flex-wrap: wrap;
}

.content-source {
  font-weight: 500;
  color: var(--color-text-main);
}

.content-meta {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
  flex: 1;
}

.content-actions {
  flex-shrink: 0;
}

.content-tags {
  display: flex;
  gap: var(--spacing-sm);
  margin-top: var(--spacing-sm);
  flex-wrap: wrap;
}

/* 分页 */
.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: var(--spacing-lg);
  padding-top: var(--spacing-lg);
  border-top: 1px solid var(--color-border);
}

.ml-1 {
  margin-left: var(--spacing-sm);
}

.date-range-picker {
  width: 280px;
}
</style>
