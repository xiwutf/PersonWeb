<template>
  <div class="admin-page">
    <div class="page-header">
      <h1 class="page-title">主题商店管理</h1>
      <button class="btn-primary" @click="showAddModal = true">
        <i class="fas fa-plus mr-2"></i>
        添加主题
      </button>
    </div>

    <!-- 筛选栏 -->
    <div class="filter-bar">
      <select v-model="filters.status" class="filter-input">
        <option value="">全部状态</option>
        <option value="draft">草稿</option>
        <option value="published">已发布</option>
        <option value="archived">已归档</option>
      </select>
      <input
        v-model="filters.search"
        type="text"
        placeholder="搜索主题名称..."
        class="filter-input"
        @input="loadThemes"
      />
      <button class="btn-secondary" @click="loadThemes">
        <i class="fas fa-search mr-2"></i>
        搜索
      </button>
    </div>

    <!-- 主题列表 -->
    <div class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>主题名称</th>
            <th>价格</th>
            <th>下载数</th>
            <th>购买数</th>
            <th>评分</th>
            <th>状态</th>
            <th>创建时间</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="theme in themes" :key="theme.id">
            <td>{{ theme.id }}</td>
            <td>{{ theme.name }}</td>
            <td>{{ theme.isFree ? '免费' : `¥${theme.price}` }}</td>
            <td>{{ theme.downloadCount }}</td>
            <td>{{ theme.purchaseCount }}</td>
            <td>{{ theme.rating }} ({{ theme.ratingCount }})</td>
            <td>
              <span :class="getStatusClass(theme.status)">
                {{ getStatusText(theme.status) }}
              </span>
            </td>
            <td>{{ formatDate(theme.createdAt) }}</td>
            <td>
              <button class="btn-sm btn-primary" @click="editTheme(theme)">
                编辑
              </button>
              <button class="btn-sm btn-danger" @click="deleteTheme(theme.id)">
                删除
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 分页 -->
    <div class="pagination">
      <button
        class="btn-secondary"
        :disabled="page === 1"
        @click="page--; loadThemes()"
      >
        上一页
      </button>
      <span class="page-info">第 {{ page }} 页，共 {{ totalPages }} 页</span>
      <button
        class="btn-secondary"
        :disabled="page >= totalPages"
        @click="page++; loadThemes()"
      >
        下一页
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const themes = ref<any[]>([])
const page = ref(1)
const pageSize = ref(20)
const totalPages = ref(1)
const filters = ref({
  status: '',
  search: ''
})

const loadThemes = async () => {
  try {
    const params = new URLSearchParams({
      page: page.value.toString(),
      pageSize: pageSize.value.toString()
    })
    if (filters.value.status) {
      params.append('status', filters.value.status)
    }

    const res = await api.get(`/admin/commercial/themes?${params}`)
    if (res && res.data) {
      themes.value = res.data.themes || []
      totalPages.value = res.data.totalPages || 1
    }
  } catch (error) {
    console.error('加载主题列表失败:', error)
  }
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    draft: 'status-badge status-draft',
    published: 'status-badge status-published',
    archived: 'status-badge status-archived'
  }
  return classes[status] || 'status-badge'
}

const getStatusText = (status: string) => {
  const texts: Record<string, string> = {
    draft: '草稿',
    published: '已发布',
    archived: '已归档'
  }
  return texts[status] || status
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleString('zh-CN')
}

const editTheme = (theme: any) => {
  // TODO: 实现编辑功能
  console.log('编辑主题:', theme)
}

const deleteTheme = async (id: number) => {
  if (!confirm('确定要删除这个主题吗？')) return
  // TODO: 实现删除功能
  console.log('删除主题:', id)
}

onMounted(() => {
  loadThemes()
})
</script>

<style scoped>
.admin-page {
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.5rem;
  font-weight: bold;
}

.filter-bar {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.filter-input {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.table-container {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 8px;
  overflow: hidden;
}

.data-table th,
.data-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.data-table th {
  background: #f5f5f5;
  font-weight: 600;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.875rem;
}

.status-draft {
  background: #fef3c7;
  color: #92400e;
}

.status-published {
  background: #d1fae5;
  color: #065f46;
}

.status-archived {
  background: #e5e7eb;
  color: #374151;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.page-info {
  padding: 0 1rem;
}
</style>

