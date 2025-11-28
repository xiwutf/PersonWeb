<template>
  <div class="time-capsules-page">
    <div class="page-header">
      <h1 class="page-title">时间胶囊管理</h1>
    </div>

    <!-- 统计信息 -->
    <n-grid :cols="3" :x-gap="16" class="stats-grid">
      <n-gi>
        <n-statistic label="待审核" :value="stats.pending">
          <template #prefix>
            <i class="fas fa-clock" style="color: #fb923c;"></i>
          </template>
        </n-statistic>
      </n-gi>
      <n-gi>
        <n-statistic label="已展示" :value="stats.approved">
          <template #prefix>
            <i class="fas fa-check-circle" style="color: #86efac;"></i>
          </template>
        </n-statistic>
      </n-gi>
      <n-gi>
        <n-statistic label="已拒绝" :value="stats.rejected">
          <template #prefix>
            <i class="fas fa-times-circle" style="color: #fca5a5;"></i>
          </template>
        </n-statistic>
      </n-gi>
    </n-grid>

    <!-- 筛选 -->
    <div class="filter-bar">
      <n-select
        v-model:value="statusFilter"
        placeholder="全部状态"
        :options="statusOptions"
        clearable
        @update:value="fetchCapsules"
        style="width: 200px"
      />
    </div>

    <!-- 胶囊列表 -->
    <div class="table-container">
      <div v-if="loading" class="table-loading">
        加载中...
      </div>
      <div v-else-if="capsules.length === 0" class="table-empty">
        暂无时间胶囊
      </div>
      <table v-else class="data-table">
        <thead class="table-header">
          <tr>
            <th>内容</th>
            <th>访客</th>
            <th>提交时间</th>
            <th>状态</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="capsule in capsules" :key="capsule.id" class="table-row">
            <td class="table-cell table-cell-content">
              <div class="content-text" :title="capsule.content">{{ capsule.content }}</div>
            </td>
            <td class="table-cell">{{ capsule.visitorName || '匿名' }}</td>
            <td class="table-cell">{{ formatDate(capsule.createdAt) }}</td>
            <td class="table-cell">
              <span 
                class="tag"
                :class="{
                  'tag-warning': capsule.status === 0,
                  'tag-success': capsule.status === 1,
                  'tag-error': capsule.status === 2
                }"
              >
                {{ getStatusText(capsule.status) }}
              </span>
            </td>
            <td class="table-cell">
              <div class="action-buttons">
                <template v-if="capsule.status === 0">
                  <button 
                    @click="approveCapsule(capsule.id)" 
                    class="btn-link btn-link-green"
                  >
                    通过
                  </button>
                  <button 
                    @click="rejectCapsule(capsule.id)" 
                    class="btn-link btn-link-red"
                  >
                    拒绝
                  </button>
                </template>
                <button 
                  @click="deleteCapsule(capsule.id)" 
                  class="btn-link btn-link-red"
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
import { NSelect, NGrid, NGi, NStatistic } from 'naive-ui'
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { handleError } = useErrorHandler()

// 使用安全的 Naive UI composables，避免 provider 未挂载时的错误
const message = useSafeMessage()

const capsules = ref<TimeCapsule[]>([])
const loading = ref(false)
const statusFilter = ref<string | null>(null)
const stats = ref({
  pending: 0,
  approved: 0,
  rejected: 0
})

// 状态选项
const statusOptions = [
  { label: '全部状态', value: null },
  { label: '待审核', value: '0' },
  { label: '已展示', value: '1' },
  { label: '已拒绝', value: '2' }
]

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
    } else if (statusFilter.value === null || statusFilter.value === '') {
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
    message.error('加载时间胶囊列表失败')
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
  // 使用浏览器原生 confirm 作为降级方案
  if (!confirm('确定要通过这个时间胶囊吗？')) return
  
  try {
    await api.post(`/TimeCapsule/${id}/approve`)
    message.success('已通过审核')
    await fetchCapsules() // 刷新列表和统计
  } catch (e: unknown) {
    handleError(e, '操作失败')
  }
}

const rejectCapsule = async (id: number) => {
  // 使用浏览器原生 confirm 作为降级方案
  if (!confirm('确定要拒绝这个时间胶囊吗？')) return
  
  try {
    await api.post(`/TimeCapsule/${id}/reject`)
    message.success('已拒绝')
    await fetchCapsules() // 刷新列表和统计
  } catch (e: unknown) {
    handleError(e, '操作失败')
  }
}

const deleteCapsule = async (id: number) => {
  if (!confirm('确定要删除这个时间胶囊吗？')) return
  
  try {
    await api.del(`/TimeCapsule/${id}`)
    message.success('删除成功')
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

/* 统计网格 */
.stats-grid {
  margin-bottom: 1.5rem;
}

/* 筛选栏 */
.filter-bar {
  margin-bottom: 1.5rem;
}

/* 表格容器 */
.table-container {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  overflow: hidden;
  margin-bottom: 1.5rem;
}

.table-loading,
.table-empty {
  padding: 2rem;
  text-align: center;
  color: rgba(255, 255, 255, 0.6);
}

/* 数据表格 */
.data-table {
  width: 100%;
  text-align: left;
  border-collapse: collapse;
}

.table-header {
  background: rgba(255, 255, 255, 0.05);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.table-header th {
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.6);
}

.table-body {
  border-collapse: collapse;
}

.table-row {
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  transition: background-color 0.2s ease;
}

.table-row:hover {
  background: rgba(255, 255, 255, 0.05);
}

.table-row:last-child {
  border-bottom: none;
}

.table-cell {
  padding: 1rem 1.5rem;
  color: rgba(255, 255, 255, 0.9);
}

.table-cell-content {
  max-width: 24rem;
}

.content-text {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* 标签样式 - 提高文字对比度 */
.tag {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.tag-warning {
  background: rgba(251, 191, 36, 0.3);
  border: 1px solid rgba(251, 191, 36, 0.6);
  color: #fde68a;
}

.tag-success {
  background: rgba(34, 197, 94, 0.3);
  border: 1px solid rgba(34, 197, 94, 0.6);
  color: #a7f3d0;
}

.tag-error {
  background: rgba(239, 68, 68, 0.3);
  border: 1px solid rgba(239, 68, 68, 0.6);
  color: #fecaca;
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

.btn-link-green {
  color: #34d399;
}

.btn-link-green:hover {
  color: #6ee7b7;
}

.btn-link-red {
  color: #f87171;
}

.btn-link-red:hover {
  color: #fca5a5;
}
</style>

