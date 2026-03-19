<template>
  <div class="intelligence-tasks-page">
    <PageHeader
      title="任务日志"
      description="查看采集、分析与日报生成任务的最近执行情况。"
    >
      <template #actions>
        <n-space :size="12" wrap>
          <n-button secondary @click="navigateTo('/admin/intelligence')">
            返回首页
          </n-button>
          <n-button type="primary" @click="fetchTaskLogs">
            刷新日志
          </n-button>
        </n-space>
      </template>
    </PageHeader>

    <n-card :bordered="false" class="tasks-panel">
      <n-spin :show="loading">
        <n-data-table
          remote
          :columns="columns"
          :data="taskLogs"
          :pagination="false"
          :bordered="false"
        />

        <n-empty v-if="!loading && !taskLogs.length" description="暂无任务日志" class="tasks-empty" />
      </n-spin>

      <div class="tasks-pagination">
        <n-pagination
          v-model:page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :item-count="pagination.total"
          :page-sizes="[10, 20, 50]"
          show-size-picker
          @update:page="handlePageChange"
          @update:page-size="handlePageSizeChange"
        />
      </div>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { h, onMounted, reactive, ref } from 'vue'
import { NTag } from 'naive-ui'
import PageHeader from '~/components/admin/patterns/PageHeader.vue'
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import type { TaskLogDto } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useIntelligenceApi()
const loading = ref(false)
const taskLogs = ref<TaskLogDto[]>([])

const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

const getStatusType = (status: string) => {
  switch (status) {
    case 'running':
      return 'info'
    case 'success':
      return 'success'
    case 'failed':
      return 'error'
    default:
      return 'default'
  }
}

const getStatusLabel = (status: string) => {
  switch (status) {
    case 'running':
      return '运行中'
    case 'success':
      return '成功'
    case 'failed':
      return '失败'
    default:
      return status || '未知'
  }
}

const formatTime = (time?: string) => {
  if (!time) return '-'

  const date = new Date(time)
  if (Number.isNaN(date.getTime())) return '-'
  return date.toLocaleString('zh-CN', { hour12: false })
}

const columns = [
  {
    title: '任务名称',
    key: 'taskName'
  },
  {
    title: '任务类型',
    key: 'taskType',
    render: (row: TaskLogDto) => row.taskType || '-'
  },
  {
    title: '状态',
    key: 'status',
    render: (row: TaskLogDto) =>
      h(
        NTag,
        {
          type: getStatusType(row.status),
          bordered: false,
          round: true
        },
        { default: () => getStatusLabel(row.status) }
      )
  },
  {
    title: '执行信息',
    key: 'message',
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '开始时间',
    key: 'startTime',
    render: (row: TaskLogDto) => formatTime(row.startTime)
  },
  {
    title: '结束时间',
    key: 'endTime',
    render: (row: TaskLogDto) => formatTime(row.endTime)
  }
]

const fetchTaskLogs = async () => {
  loading.value = true
  try {
    const result = await api.getTaskLogs({
      pageIndex: pagination.page,
      pageSize: pagination.pageSize
    })
    taskLogs.value = result.list || []
    pagination.total = result.total || 0
  } catch (error) {
    console.error('获取任务日志失败:', error)
    taskLogs.value = []
    pagination.total = 0
  } finally {
    loading.value = false
  }
}

const handlePageChange = (page: number) => {
  pagination.page = page
  fetchTaskLogs()
}

const handlePageSizeChange = (pageSize: number) => {
  pagination.pageSize = pageSize
  pagination.page = 1
  fetchTaskLogs()
}

onMounted(() => {
  fetchTaskLogs()
})
</script>

<style scoped>
.intelligence-tasks-page {
  padding: var(--spacing-lg);
  max-width: 1480px;
  margin: 0 auto;
}

.tasks-panel {
  background:
    linear-gradient(180deg, rgba(18, 24, 39, 0.98), rgba(15, 23, 42, 0.92)),
    var(--color-bg-card);
  border: 1px solid var(--color-border-subtle, rgba(148, 163, 184, 0.18));
}

.tasks-empty {
  padding: 40px 0;
}

.tasks-pagination {
  display: flex;
  justify-content: flex-end;
  margin-top: var(--spacing-lg);
}

@media (max-width: 768px) {
  .intelligence-tasks-page {
    padding: var(--spacing-md);
  }

  .tasks-pagination {
    justify-content: center;
  }
}
</style>
