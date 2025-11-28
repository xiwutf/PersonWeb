<template>
  <div class="time-capsules-page">
    <div class="page-header">
      <h1 class="page-title">时间胶囊管理</h1>
    </div>

    <!-- 统计信息 -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-label">待审核</div>
        <div class="stat-value stat-value-orange">{{ stats.pending }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">已展示</div>
        <div class="stat-value stat-value-green">{{ stats.approved }}</div>
      </div>
      <div class="stat-card">
        <div class="stat-label">已拒绝</div>
        <div class="stat-value stat-value-red">{{ stats.rejected }}</div>
      </div>
    </div>

    <!-- 筛选 -->
    <div class="filter-bar">
      <select v-model="statusFilter" @change="fetchCapsules" class="filter-select">
        <option value="">全部状态</option>
        <option value="0">待审核</option>
        <option value="1">已展示</option>
        <option value="2">已拒绝</option>
      </select>
    </div>

    <!-- 胶囊列表 -->
    <div class="table-container">
      <table class="data-table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">内容</th>
            <th class="table-header-cell">访客</th>
            <th class="table-header-cell">提交时间</th>
            <th class="table-header-cell">状态</th>
            <th class="table-header-cell">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-if="loading">
            <td colspan="5" class="table-cell table-cell-center">加载中...</td>
          </tr>
          <tr v-else-if="capsules.length === 0">
            <td colspan="5" class="table-cell table-cell-center">暂无时间胶囊</td>
          </tr>
          <tr v-for="capsule in capsules" :key="capsule.id" class="table-row">
            <td class="table-cell table-cell-content">
              <p class="content-text">{{ capsule.content }}</p>
            </td>
            <td class="table-cell table-cell-secondary">
              {{ capsule.visitorName || '匿名' }}
            </td>
            <td class="table-cell table-cell-secondary">
              {{ formatDate(capsule.createdAt) }}
            </td>
            <td class="table-cell">
              <span :class="getStatusClass(capsule.status)" class="status-badge">
                {{ getStatusText(capsule.status) }}
              </span>
            </td>
            <td class="table-cell">
              <div class="action-buttons">
                <button
                  v-if="capsule.status === 0"
                  @click="approveCapsule(capsule.id)"
                  class="action-button action-button-approve"
                >
                  通过
                </button>
                <button
                  v-if="capsule.status === 0"
                  @click="rejectCapsule(capsule.id)"
                  class="action-button action-button-reject"
                >
                  拒绝
                </button>
                <button
                  @click="deleteCapsule(capsule.id)"
                  class="action-button action-button-delete"
                  title="删除"
                >
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const capsules = ref<TimeCapsule[]>([])
const loading = ref(false)
const statusFilter = ref('')
const stats = ref({
  pending: 0,
  approved: 0,
  rejected: 0
})

const fetchCapsules = async () => {
  loading.value = true
  try {
    // 根据筛选条件选择不同的 API
    let endpoint = '/TimeCapsule/all' // 默认获取全部
    let params: Record<string, unknown> = {
      page: 1,
      pageSize: 100
    }
    
    if (statusFilter.value === '1') {
      // 已展示的 - 使用公开 API
      endpoint = '/TimeCapsule'
    } else if (statusFilter.value === '2') {
      // 已拒绝的 - 使用全部列表 API，筛选状态为 2
      endpoint = '/TimeCapsule/all'
      params.status = 2
    } else if (statusFilter.value === '0') {
      // 待审核的
      endpoint = '/TimeCapsule/pending'
    } else if (statusFilter.value === '') {
      // 全部状态 - 使用全部列表 API，不传 status 参数
      endpoint = '/TimeCapsule/all'
    }
    
    const res = await api.get<TimeCapsuleListResponse>(endpoint, { params })

    // 兼容大小写：后端返回的是 list（小写），但类型定义可能是 List（大写）
    if (process.env.NODE_ENV === 'development') {
      console.log('[TimeCapsules] API Response:', res)
      console.log('[TimeCapsules] Has List?', !!(res as any)?.List)
      console.log('[TimeCapsules] Has list?', !!(res as any)?.list)
      console.log('[TimeCapsules] Is Array?', Array.isArray(res))
    }

    if (res) {
      if (res.List) {
        capsules.value = res.List
      } else if ((res as any).list) {
        capsules.value = (res as any).list
      } else if (Array.isArray(res)) {
        capsules.value = res
      } else {
        capsules.value = []
      }
    } else {
      capsules.value = []
    }

    if (process.env.NODE_ENV === 'development') {
      console.log('[TimeCapsules] Final capsules count:', capsules.value.length)
    }

    // 更新统计（从服务器获取准确的统计数据）
    await fetchStats()
  } catch (e) {
    // 静默失败
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch capsules:', e)
    }
    capsules.value = []
  } finally {
    loading.value = false
  }
}

const fetchStats = async () => {
  try {
    const res = await api.get<{
      Pending: number
      Approved: number
      Rejected: number
      Total: number
    }>('/TimeCapsule/stats')
    
    if (res) {
      stats.value = {
        pending: res.Pending,
        approved: res.Approved,
        rejected: res.Rejected
      }
    }
  } catch (e) {
    // 如果统计接口失败，使用本地计算（降级方案）
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch stats:', e)
    }
    updateStats()
  }
}

const updateStats = () => {
  // 降级方案：基于当前加载的数据计算统计
  stats.value = {
    pending: capsules.value.filter(c => c.status === 0).length,
    approved: capsules.value.filter(c => c.status === 1).length,
    rejected: capsules.value.filter(c => c.status === 2).length
  }
}

const approveCapsule = async (id: number) => {
  if (!confirm('确定要通过这个时间胶囊吗？')) return

  try {
    await api.post(`/TimeCapsule/${id}/approve`)
    const { success } = useNotification()
    success('已通过审核')
    await fetchCapsules() // 刷新列表和统计
  } catch (e: unknown) {
    const { handleError } = useErrorHandler()
    handleError(e, '操作失败')
  }
}

const rejectCapsule = async (id: number) => {
  if (!confirm('确定要拒绝这个时间胶囊吗？')) return

  try {
    await api.post(`/TimeCapsule/${id}/reject`)
    const { success } = useNotification()
    success('已拒绝')
    await fetchCapsules() // 刷新列表和统计
  } catch (e: unknown) {
    const { handleError } = useErrorHandler()
    handleError(e, '操作失败')
  }
}

const deleteCapsule = async (id: number) => {
  if (!confirm('确定要删除这个时间胶囊吗？删除后无法恢复。')) return

  const { success } = useNotification()
  const { handleError } = useErrorHandler()

  try {
    // 确保使用正确的路径格式
    const response = await api.del(`/TimeCapsule/${id}`)
    
    if (process.env.NODE_ENV === 'development') {
      console.log('[Delete] Response:', response)
    }
    
    success('删除成功')
    await fetchCapsules() // 刷新列表和统计
  } catch (e: any) {
    if (process.env.NODE_ENV === 'development') {
      console.error('[Delete] Error:', e)
      console.error('[Delete] Error URL:', e?.url || e?.request?.url)
      console.error('[Delete] Error Status:', e?.status || e?.response?.status)
    }
    handleError(e, '删除失败')
  }
}

const getStatusText = (status: number) => {
  const statusMap: Record<number, string> = {
    0: '待审核',
    1: '已展示',
    2: '已拒绝'
  }
  return statusMap[status] || '未知'
}

const getStatusClass = (status: number) => {
  const classMap: Record<number, string> = {
    0: 'bg-orange-500/20 text-orange-300 border border-orange-500/30',
    1: 'bg-green-500/20 text-green-300 border border-green-500/30',
    2: 'bg-red-500/20 text-red-300 border border-red-500/30'
  }
  return classMap[status] || 'bg-gray-500/20 text-gray-300 border border-gray-500/30'
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchCapsules()
})
</script>

<style scoped>
/* 页面容器 */
.time-capsules-page {
  width: 100%;
}

/* 页面头部 */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.page-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #e5e7eb;
}

/* 统计网格 */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

/* 统计卡片 */
.stat-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(4px);
  border-radius: 0.5rem;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  padding: 1rem;
}

.stat-label {
  font-size: 0.875rem;
  color: #9ca3af;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
}

.stat-value-orange {
  color: #fb923c;
}

.stat-value-green {
  color: #86efac;
}

.stat-value-red {
  color: #fca5a5;
}

/* 筛选栏 */
.filter-bar {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(4px);
  padding: 1rem;
  border-radius: 0.5rem;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  margin-bottom: 1.5rem;
  display: flex;
  gap: 1rem;
}

.filter-select {
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 0.25rem;
  padding: 0.5rem 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  color: #e5e7eb;
  outline: none;
}

.filter-select:focus {
  border-color: rgba(255, 255, 255, 0.3);
}

/* 表格容器 */
.table-container {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(4px);
  border-radius: 0.5rem;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  overflow: hidden;
}

.data-table {
  width: 100%;
  text-align: left;
}

.table-header {
  background: rgba(255, 255, 255, 0.05);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.table-header-cell {
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #9ca3af;
}

.table-body {
  border-color: rgba(255, 255, 255, 0.1);
}

.table-row {
  transition: background-color 0.2s;
}

.table-row:hover {
  background: rgba(255, 255, 255, 0.05);
}

.table-cell {
  padding: 1rem 1.5rem;
  border-color: rgba(255, 255, 255, 0.1);
}

.table-cell-center {
  text-align: center;
  color: #9ca3af;
}

.table-cell-content {
  color: #e5e7eb;
  max-width: 28rem;
}

.content-text {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.table-cell-secondary {
  color: #9ca3af;
  font-size: 0.875rem;
}

/* 状态徽章 */
.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  border: 1px solid;
}

/* 操作按钮 */
.action-buttons {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.action-button {
  font-size: 0.875rem;
  font-weight: 500;
  transition: color 0.2s;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
}

.action-button-approve {
  color: #86efac;
}

.action-button-approve:hover {
  color: #bbf7d0;
}

.action-button-reject {
  color: #fca5a5;
}

.action-button-reject:hover {
  color: #fecaca;
}

.action-button-delete {
  color: #fca5a5;
  margin-left: 0.5rem;
}

.action-button-delete:hover {
  color: #fecaca;
}
</style>

