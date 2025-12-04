<template>
  <n-card class="dashboard-table-card">
    <template #header>
      <div class="table-header">
        <h3 class="table-title">最近项目</h3>
      </div>
    </template>
    <n-data-table
      :columns="columns"
      :data="items"
      :loading="loading"
      :pagination="false"
      :bordered="false"
      size="small"
    />
  </n-card>
</template>

<script setup lang="ts">
import { h } from 'vue'
import { NTag } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { SideProject } from '~/types/api'

interface Props {
  items: SideProject[]
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})

const statusMap: Record<number, { label: string; type: 'default' | 'success' | 'warning' | 'error' | 'info' }> = {
  0: { label: '进行中', type: 'info' },
  1: { label: '已完成', type: 'success' },
  2: { label: '待付款', type: 'warning' },
  3: { label: '已取消', type: 'error' }
}

const formatPrice = (price: number | null | undefined) => {
  if (price == null) return '-'
  return `¥${price.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
}

const formatDate = (date: string | null | undefined) => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('zh-CN')
}

const columns: DataTableColumns<SideProject> = [
  {
    title: '项目名称',
    key: 'title',
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '类型',
    key: 'category',
    render: (row) => row.category || '-'
  },
  {
    title: '金额',
    key: 'priceFinal',
    render: (row) => formatPrice(row.priceFinal)
  },
  {
    title: '状态',
    key: 'status',
    render: (row) => {
      const status = statusMap[row.status] || { label: '未知', type: 'default' }
      return h(NTag, { type: status.type, size: 'small' }, { default: () => status.label })
    }
  },
  {
    title: '完成时间',
    key: 'endTime',
    render: (row) => formatDate(row.endTime)
  }
]
</script>

<style scoped>
.dashboard-table-card {
  background-color: var(--color-bg-card);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  border: 1px solid var(--color-border-subtle);
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-title {
  font-size: var(--font-size-h4);
  font-weight: bold;
  color: var(--color-text-main);
}
</style>

