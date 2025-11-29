<template>
  <div class="admin-page">
    <div class="page-header">
      <h1 class="page-title">API Key 管理</h1>
      <button class="btn-secondary" @click="loadApiUsers">
        <i class="fas fa-sync-alt mr-2"></i>
        刷新
      </button>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-label">总用户数</div>
        <div class="stat-value">{{ totalUsers }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">活跃 API Keys</div>
        <div class="stat-value">{{ totalApiKeys }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">总调用次数</div>
        <div class="stat-value">{{ totalCalls }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">总成本</div>
        <div class="stat-value">¥{{ totalCost.toFixed(2) }}</div>
      </div>
    </div>

    <!-- 筛选栏 -->
    <div class="filter-bar">
      <input
        v-model="filters.search"
        type="text"
        placeholder="搜索用户邮箱或名称..."
        class="filter-input"
        @input="loadApiUsers"
      />
      <button class="btn-secondary" @click="loadApiUsers">
        <i class="fas fa-search mr-2"></i>
        搜索
      </button>
      <button class="btn-secondary" @click="loadStats">
        <i class="fas fa-chart-bar mr-2"></i>
        查看统计
      </button>
    </div>

    <!-- API用户列表 -->
    <div class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>邮箱</th>
            <th>名称</th>
            <th>API Key 数量</th>
            <th>免费调用</th>
            <th>付费调用</th>
            <th>最后调用时间</th>
            <th>注册时间</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in apiUsers" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.name || '-' }}</td>
            <td>
              <span class="badge">{{ user.apiKeyCount }}</span>
            </td>
            <td>{{ user.freeCallsUsed }}</td>
            <td>{{ user.paidCalls }}</td>
            <td>{{ formatDate(user.lastCallAt) }}</td>
            <td>{{ formatDate(user.createdAt) }}</td>
            <td>
              <button class="btn-sm btn-primary" @click="viewUserKeys(user)">
                查看 Keys
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
        @click="page--; loadApiUsers()"
      >
        上一页
      </button>
      <span class="page-info">第 {{ page }} 页，共 {{ totalPages }} 页</span>
      <button
        class="btn-secondary"
        :disabled="page >= totalPages"
        @click="page++; loadApiUsers()"
      >
        下一页
      </button>
    </div>

    <!-- 用户 API Keys 详情模态框 -->
    <div v-if="showKeysModal" class="modal-overlay" @click.self="showKeysModal = false">
      <div class="modal-content">
        <div class="modal-header">
          <h2>API Keys - {{ selectedUser?.email }}</h2>
          <button class="modal-close" @click="showKeysModal = false">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <div v-if="userKeys.length === 0" class="empty-state">
            <p>该用户还没有 API Keys</p>
          </div>
          <table v-else class="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>名称</th>
                <th>API Key</th>
                <th>速率限制</th>
                <th>每日限制</th>
                <th>过期时间</th>
                <th>最后使用</th>
                <th>创建时间</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="key in userKeys" :key="key.id">
                <td>{{ key.id }}</td>
                <td>{{ key.name || '-' }}</td>
                <td class="api-key-cell">
                  <code>{{ key.apiKey }}</code>
                </td>
                <td>{{ key.rateLimit }}/分钟</td>
                <td>{{ key.dailyLimit }}/天</td>
                <td>{{ formatDate(key.expiresAt) }}</td>
                <td>{{ formatDate(key.lastUsedAt) }}</td>
                <td>{{ formatDate(key.createdAt) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- 统计图表模态框 -->
    <div v-if="showStatsModal" class="modal-overlay" @click.self="showStatsModal = false">
      <div class="modal-content modal-large">
        <div class="modal-header">
          <h2>API 调用统计</h2>
          <button class="modal-close" @click="showStatsModal = false">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <div class="stats-summary">
            <div class="stat-item">
              <span class="stat-label">总调用次数：</span>
              <span class="stat-value">{{ statsTotal.totalCalls }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">总成本：</span>
              <span class="stat-value">¥{{ statsTotal.totalCost?.toFixed(2) || '0.00' }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">平均响应时间：</span>
              <span class="stat-value">{{ statsTotal.avgResponseTime?.toFixed(2) || '0' }}ms</span>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
                <th>日期</th>
                <th>调用次数</th>
                <th>总成本</th>
                <th>平均响应时间</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="stat in statsDaily" :key="stat.date">
                <td>{{ formatDate(stat.date) }}</td>
                <td>{{ stat.count }}</td>
                <td>¥{{ stat.totalCost?.toFixed(2) || '0.00' }}</td>
                <td>{{ stat.avgResponseTime?.toFixed(2) || '0' }}ms</td>
              </tr>
              <tr v-if="statsDaily.length === 0">
                <td colspan="4" class="empty-state">暂无统计数据</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

// 数据状态
const apiUsers = ref<any[]>([])
const userKeys = ref<any[]>([])
const statsDaily = ref<any[]>([])
const statsTotal = ref<any>({
  totalCalls: 0,
  totalCost: 0,
  avgResponseTime: 0
})

// 分页
const page = ref(1)
const pageSize = ref(20)
const totalPages = ref(1)
const totalUsers = ref(0)
const totalApiKeys = ref(0)
const totalCalls = ref(0)
const totalCost = ref(0)

// 筛选
const filters = ref({
  search: ''
})

// 模态框
const showKeysModal = ref(false)
const showStatsModal = ref(false)
const selectedUser = ref<any>(null)

// 加载API用户列表
const loadApiUsers = async () => {
  try {
    const params = new URLSearchParams({
      page: page.value.toString(),
      pageSize: pageSize.value.toString()
    })

    const res = await api.get(`/admin/commercial/api-users?${params}`)
    if (res && res.data) {
      apiUsers.value = res.data.users || []
      totalUsers.value = res.data.total || 0
      totalPages.value = Math.ceil(totalUsers.value / pageSize.value)
      
      // 计算总API Key数量
      totalApiKeys.value = apiUsers.value.reduce((sum, user) => sum + (user.apiKeyCount || 0), 0)
    }
  } catch (error) {
    console.error('加载API用户列表失败:', error)
  }
}

// 查看用户的API Keys
const viewUserKeys = async (user: any) => {
  selectedUser.value = user
  showKeysModal.value = true
  userKeys.value = []
  
  try {
    const res = await api.post('/apikey/list', {
      userId: user.userId || user.id
    })
    if (res && res.data) {
      userKeys.value = Array.isArray(res.data) ? res.data : []
    }
  } catch (error) {
    console.error('加载用户API Keys失败:', error)
    userKeys.value = []
  }
}

// 加载统计信息
const loadStats = async () => {
  showStatsModal.value = true
  statsDaily.value = []
  statsTotal.value = { totalCalls: 0, totalCost: 0, avgResponseTime: 0 }
  
  try {
    const res = await api.get('/admin/commercial/api-calls/stats')
    if (res && res.data) {
      statsDaily.value = res.data.daily || []
      statsTotal.value = res.data.total || { totalCalls: 0, totalCost: 0, avgResponseTime: 0 }
      totalCalls.value = statsTotal.value.totalCalls || 0
      totalCost.value = statsTotal.value.totalCost || 0
    }
  } catch (error) {
    console.error('加载统计信息失败:', error)
  }
}

// 格式化日期
const formatDate = (date: string | Date | null | undefined) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-CN')
}

onMounted(() => {
  loadApiUsers()
  loadStats()
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

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.stat-label {
  font-size: 0.875rem;
  color: #666;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: bold;
  color: #333;
}

.filter-bar {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.filter-input {
  flex: 1;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.table-container {
  overflow-x: auto;
  margin-bottom: 1.5rem;
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

.badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  background: #3b82f6;
  color: white;
  border-radius: 4px;
  font-size: 0.875rem;
  font-weight: 500;
}

.api-key-cell code {
  font-family: 'Courier New', monospace;
  background: #f5f5f5;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.875rem;
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

/* 模态框样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 800px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.modal-large {
  max-width: 1200px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #eee;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.25rem;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
  padding: 0;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close:hover {
  color: #333;
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
  flex: 1;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.stats-summary {
  display: flex;
  gap: 2rem;
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: #f5f5f5;
  border-radius: 4px;
}

.stat-item {
  display: flex;
  flex-direction: column;
}

.stat-item .stat-label {
  font-size: 0.875rem;
  color: #666;
  margin-bottom: 0.25rem;
}

.stat-item .stat-value {
  font-size: 1.25rem;
  font-weight: bold;
  color: #333;
}

/* 按钮样式 */
.btn-primary,
.btn-secondary,
.btn-sm {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.875rem;
  transition: all 0.2s;
  display: inline-flex;
  align-items: center;
}

.btn-primary {
  background: #3b82f6;
  color: white;
}

.btn-primary:hover {
  background: #2563eb;
}

.btn-secondary {
  background: #e5e7eb;
  color: #374151;
}

.btn-secondary:hover {
  background: #d1d5db;
}

.btn-secondary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-sm {
  padding: 0.25rem 0.75rem;
  font-size: 0.75rem;
}

.btn-danger {
  background: #ef4444;
  color: white;
}

.btn-danger:hover {
  background: #dc2626;
}
</style>

