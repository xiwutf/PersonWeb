<template>
  <div class="admin-comments-page">
    <div class="page-header">
      <h1 class="page-title">内容回应</h1>
      <p class="page-desc">审核游客对文章等内容的回应；通过后才会在前台展示</p>
    </div>

    <div class="stats-row">
      <button
        type="button"
        class="stats-card"
        :class="{ 'is-active': statusFilter === 'pending' }"
        @click="statusFilter = 'pending'"
      >
        <span class="stats-label">待审核</span>
        <strong class="stats-value stats-pending">{{ stats.pending }}</strong>
      </button>
      <button
        type="button"
        class="stats-card"
        :class="{ 'is-active': statusFilter === 'approved' }"
        @click="statusFilter = 'approved'"
      >
        <span class="stats-label">已通过</span>
        <strong class="stats-value stats-approved">{{ stats.approved }}</strong>
      </button>
      <button
        type="button"
        class="stats-card"
        :class="{ 'is-active': statusFilter === 'rejected' }"
        @click="statusFilter = 'rejected'"
      >
        <span class="stats-label">已拒绝</span>
        <strong class="stats-value stats-rejected">{{ stats.rejected }}</strong>
      </button>
    </div>

    <div class="toolbar">
      <n-select
        v-model:value="targetTypeFilter"
        placeholder="全部类型"
        clearable
        class="toolbar-select"
        :options="targetTypeOptions"
      />
      <n-button quaternary @click="fetchComments">
        <template #icon><i class="fas fa-sync-alt"></i></template>
        刷新
      </n-button>
    </div>

    <n-data-table
      class="interaction-table"
      :columns="columns"
      :data="rows"
      :loading="loading"
      :bordered="false"
      size="small"
      :scroll-x="980"
      :row-key="(row: AdminCommentRow) => row.id"
      :pagination="pagination"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, h, onMounted, ref, watch } from 'vue'
import { NButton, NDataTable, NSelect, NSpace, NTag } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'
import '~/assets/css/visitor-interaction.css'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
})

type AdminCommentRow = {
  id: number
  targetType: string
  targetId: string
  nickname: string
  email: string
  content: string
  status: string
  createdAt: string
}

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()

const rows = ref<AdminCommentRow[]>([])
const loading = ref(false)
const statusFilter = ref<string>('pending')
const targetTypeFilter = ref<string | null>(null)
const page = ref(1)
const pageSize = ref(20)
const total = ref(0)
const stats = ref({ pending: 0, approved: 0, rejected: 0 })

const targetTypeOptions = [
  { label: '文章', value: 'article' },
  { label: '阅读', value: 'reading' },
  { label: '摘句', value: 'quote' },
  { label: 'Moment', value: 'moment' },
]

const statusTagType = (status: string) => {
  if (status === 'approved') return 'success'
  if (status === 'rejected') return 'error'
  return 'warning'
}

const statusLabel = (status: string) => {
  if (status === 'approved') return '已通过'
  if (status === 'rejected') return '已拒绝'
  return '待审核'
}

const targetLabel = (type: string) => {
  const hit = targetTypeOptions.find(item => item.value === type)
  return hit?.label || type
}

const sourcePath = (row: AdminCommentRow) => {
  if (row.targetType === 'article') return `/work/blog/${encodeURIComponent(row.targetId)}`
  return null
}

const mapRow = (raw: Record<string, unknown>): AdminCommentRow => ({
  id: Number(raw.id ?? raw.Id ?? 0),
  targetType: String(raw.targetType ?? raw.TargetType ?? ''),
  targetId: String(raw.targetId ?? raw.TargetId ?? ''),
  nickname: String(raw.nickname ?? raw.Nickname ?? ''),
  email: String(raw.email ?? raw.Email ?? ''),
  content: String(raw.content ?? raw.Content ?? ''),
  status: String(raw.status ?? raw.Status ?? 'pending'),
  createdAt: String(raw.createdAt ?? raw.CreatedAt ?? ''),
})

const refreshStats = async () => {
  try {
    const [pending, approved, rejected] = await Promise.all([
      api.get<{ total?: number, Total?: number }>('/admin/comments', { query: { status: 'pending', page: 1, pageSize: 1 }, silent: true }),
      api.get<{ total?: number, Total?: number }>('/admin/comments', { query: { status: 'approved', page: 1, pageSize: 1 }, silent: true }),
      api.get<{ total?: number, Total?: number }>('/admin/comments', { query: { status: 'rejected', page: 1, pageSize: 1 }, silent: true }),
    ])
    stats.value = {
      pending: Number(pending?.total ?? pending?.Total ?? 0),
      approved: Number(approved?.total ?? approved?.Total ?? 0),
      rejected: Number(rejected?.total ?? rejected?.Total ?? 0),
    }
  } catch {
    // 统计失败不阻断列表
  }
}

const fetchComments = async () => {
  loading.value = true
  try {
    const data = await api.get<{
      total?: number
      Total?: number
      list?: Record<string, unknown>[]
      List?: Record<string, unknown>[]
    }>('/admin/comments', {
      query: {
        status: statusFilter.value,
        targetType: targetTypeFilter.value || undefined,
        page: page.value,
        pageSize: pageSize.value,
      },
    })
    const list = data?.list ?? data?.List ?? []
    rows.value = list.map(mapRow)
    total.value = Number(data?.total ?? data?.Total ?? rows.value.length)
    await refreshStats()
  } catch (error) {
    handleError(error)
  } finally {
    loading.value = false
  }
}

const approve = async (id: number) => {
  try {
    await api.patch(`/admin/comments/${id}/approve`, {})
    message.success('已通过')
    await fetchComments()
  } catch (error) {
    handleError(error)
  }
}

const reject = async (id: number) => {
  try {
    await api.patch(`/admin/comments/${id}/reject`, {})
    message.success('已拒绝')
    await fetchComments()
  } catch (error) {
    handleError(error)
  }
}

const remove = async (id: number) => {
  try {
    await api.del(`/admin/comments/${id}`)
    message.success('已删除')
    await fetchComments()
  } catch (error) {
    handleError(error)
  }
}

const columns = computed<DataTableColumns<AdminCommentRow>>(() => [
  {
    title: '昵称',
    key: 'nickname',
    width: 110,
    ellipsis: { tooltip: true },
  },
  {
    title: '回应',
    key: 'content',
    minWidth: 220,
    ellipsis: { tooltip: true },
  },
  {
    title: '来源',
    key: 'target',
    width: 180,
    render: (row) => h('div', { class: 'visitor-path-cell' }, [
      h('div', { class: 'visitor-path-title' }, targetLabel(row.targetType)),
      h('div', { class: 'visitor-path-subtitle' }, row.targetId),
    ]),
  },
  {
    title: '提交时间',
    key: 'createdAt',
    width: 160,
    render: (row) => {
      const d = new Date(row.createdAt)
      return Number.isNaN(d.getTime()) ? row.createdAt : d.toLocaleString('zh-CN')
    },
  },
  {
    title: '状态',
    key: 'status',
    width: 90,
    render: (row) => h(NTag, { type: statusTagType(row.status), size: 'small', bordered: false }, {
      default: () => statusLabel(row.status),
    }),
  },
  {
    title: '操作',
    key: 'actions',
    width: 240,
    fixed: 'right',
    render: (row) => {
      const path = sourcePath(row)
      return h(NSpace, { size: 6 }, {
        default: () => [
          path
            ? h(NButton, {
                size: 'tiny',
                quaternary: true,
                tag: 'a',
                href: path,
                target: '_blank',
              }, { default: () => '查看来源' })
            : null,
          row.status !== 'approved'
            ? h(NButton, { size: 'tiny', type: 'success', secondary: true, onClick: () => approve(row.id) }, { default: () => '通过' })
            : null,
          row.status !== 'rejected'
            ? h(NButton, { size: 'tiny', type: 'warning', secondary: true, onClick: () => reject(row.id) }, { default: () => '拒绝' })
            : null,
          h(NButton, { size: 'tiny', type: 'error', quaternary: true, onClick: () => remove(row.id) }, { default: () => '删除' }),
        ].filter(Boolean),
      })
    },
  },
])

const pagination = computed(() => ({
  page: page.value,
  pageSize: pageSize.value,
  itemCount: total.value,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  onChange: (p: number) => {
    page.value = p
    void fetchComments()
  },
  onUpdatePageSize: (size: number) => {
    pageSize.value = size
    page.value = 1
    void fetchComments()
  },
  prefix: ({ itemCount }: { itemCount: number }) => `共 ${itemCount} 条`,
}))

watch([statusFilter, targetTypeFilter], () => {
  page.value = 1
  void fetchComments()
})

onMounted(() => {
  void fetchComments()
})
</script>

<style scoped>
.stats-card {
  text-align: left;
  cursor: pointer;
  border: 1px solid transparent;
}

.stats-card.is-active {
  border-color: var(--color-primary);
}
</style>
