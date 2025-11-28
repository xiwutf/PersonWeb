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
    <n-card>
      <n-data-table
        :columns="columns"
        :data="capsules"
        :loading="loading"
        :bordered="false"
      />
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { NCard, NDataTable, NTag, NButton, NPopconfirm, NSelect, NGrid, NGi, NStatistic, h } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { TimeCapsule, TimeCapsuleListResponse } from '~/types/api'
import { useMessage, useDialog } from 'naive-ui'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const message = useMessage()
const dialog = useDialog()
const { handleError } = useErrorHandler()

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

// 表格列定义
const columns: DataTableColumns<TimeCapsule> = [
  {
    title: '内容',
    key: 'content',
    ellipsis: {
      tooltip: true
    },
    width: 300
  },
  {
    title: '访客',
    key: 'visitorName',
    width: 120,
    render(row) {
      return row.visitorName || '匿名'
    }
  },
  {
    title: '提交时间',
    key: 'createdAt',
    width: 180,
    render(row) {
      return formatDate(row.createdAt)
    }
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render(row) {
      const statusConfig = {
        0: { type: 'warning' as const, text: '待审核' },
        1: { type: 'success' as const, text: '已展示' },
        2: { type: 'error' as const, text: '已拒绝' }
      }
      const config = statusConfig[row.status as keyof typeof statusConfig] || { type: 'default' as const, text: '未知' }
      return h(NTag, { type: config.type, size: 'small' }, {
        default: () => config.text
      })
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 200,
    render(row) {
      const buttons = []
      
      if (row.status === 0) {
        buttons.push(
          h(NButton, {
            size: 'small',
            type: 'success',
            quaternary: true,
            onClick: () => approveCapsule(row.id)
          }, {
            default: () => '通过'
          }),
          h(NButton, {
            size: 'small',
            type: 'error',
            quaternary: true,
            onClick: () => rejectCapsule(row.id)
          }, {
            default: () => '拒绝'
          })
        )
      }
      
      buttons.push(
        h(NPopconfirm, {
          onPositiveClick: () => deleteCapsule(row.id)
        }, {
          trigger: () => h(NButton, {
            size: 'small',
            type: 'error',
            quaternary: true
          }, {
            default: () => h('i', { class: 'fas fa-trash' })
          }),
          default: () => '确定要删除这个时间胶囊吗？'
        })
      )
      
      return h('div', { class: 'action-buttons' }, buttons)
    }
  }
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
  dialog.warning({
    title: '确认通过',
    content: '确定要通过这个时间胶囊吗？',
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      try {
        await api.post(`/TimeCapsule/${id}/approve`)
        message.success('已通过审核')
        await fetchCapsules() // 刷新列表和统计
      } catch (e: unknown) {
        handleError(e, '操作失败')
      }
    }
  })
}

const rejectCapsule = async (id: number) => {
  dialog.warning({
    title: '确认拒绝',
    content: '确定要拒绝这个时间胶囊吗？',
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      try {
        await api.post(`/TimeCapsule/${id}/reject`)
        message.success('已拒绝')
        await fetchCapsules() // 刷新列表和统计
      } catch (e: unknown) {
        handleError(e, '操作失败')
      }
    }
  })
}

const deleteCapsule = async (id: number) => {
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

/* 操作按钮组 */
.action-buttons {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}
</style>

