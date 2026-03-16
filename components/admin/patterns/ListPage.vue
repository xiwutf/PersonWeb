<template>
  <div class="admin-list-page" :class="props.class" :style="props.style">
    <!-- 页面头部 -->
    <slot name="header">
      <PageHeader
        :title="props.title"
        :description="props.description"
        :show-actions="props.showActions"
        :actions="headerActions"
        @action-click="handleActionClick"
      >
        <template #title>
          <slot name="header-title">{{ props.title }}</slot>
        </template>
        <template #description>
          <slot name="header-description">{{ props.description }}</slot>
        </template>
        <template #actions>
          <slot name="header-actions"></slot>
        </template>
      </PageHeader>
    </slot>

    <!-- 统计卡片 -->
    <div v-if="props.showStats && (props.stats?.length || $slots.stats)" class="stats-section">
      <slot name="stats">
        <n-grid :x-gap="16" :y-gap="16" :cols="statsCols">
          <n-gi v-for="stat in props.stats" :key="stat.label">
            <n-card :bordered="false" class="stat-card">
              <div class="stat-content">
                <div v-if="stat.icon" class="stat-icon" :style="{ color: stat.iconColor }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-info">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
              </div>
            </n-card>
          </n-gi>
        </n-grid>
      </slot>
    </div>

    <!-- 筛选器 -->
    <div v-if="props.filters?.length || $slots.filter" class="filter-section">
      <slot name="filter" :filter-value="filterValue">
        <FilterBar
          :filters="props.filters"
          :model-value="filterValue"
          :show-reset="showReset"
          :show-search="showSearch"
          :searching="props.loading"
          :layout="filterLayout"
          :per-line="filterPerLine"
          @update:model-value="handleFilterChange"
          @search="handleSearch"
          @reset="handleReset"
        >
          <template
            v-for="filter in props.filters"
            :key="`filter-slot-${filter.field}`"
            #[`filter-${filter.field}`]="slotProps"
          >
            <slot :name="`filter-${filter.field}`" v-bind="slotProps"></slot>
          </template>
        </FilterBar>
      </slot>
    </div>

    <!-- 数据表格 -->
    <div class="table-section">
      <n-spin :show="props.loading">
        <n-data-table
          :key="tableKey"
          :columns="internalColumns"
          :data="props.data"
          :row-key="props.tableConfig?.rowKey"
          :single-line="props.tableConfig?.singleLine"
          :max-height="props.tableConfig?.maxHeight"
          :bordered="props.tableConfig?.bordered"
          :row-class-name="props.tableConfig?.rowClassName"
          :row-props="rowProps"
          :pagination="internalPagination"
          :remote="true"
          @update:page="handlePageChange"
          @update:page-size="handlePageSizeChange"
          @update:checked-row-keys="handleRowCheck"
          @sort="handleSortChange"
        >
          <template #empty>
            <slot name="table-empty">
              <n-empty
                :icon="props.emptyConfig?.icon"
                :description="props.emptyConfig?.text || '暂无数据'"
              >
                <template #extra v-if="props.emptyConfig?.description">
                  <p class="empty-description">{{ props.emptyConfig.description }}</p>
                </template>
                <template #action v-if="props.emptyConfig?.showAction && props.emptyConfig?.action">
                  <n-button type="primary" @click="props.emptyConfig.action.onClick">
                    {{ props.emptyConfig.action.label }}
                  </n-button>
                </template>
              </n-empty>
            </slot>
          </template>
        </n-data-table>

        <!-- 表格底部 -->
        <div v-if="$slots['table-footer']" class="table-footer">
          <slot name="table-footer"></slot>
        </div>
      </n-spin>
    </div>

    <!-- 页面底部 -->
    <div v-if="$slots.footer" class="page-footer">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, h, type VNodeChild } from 'vue'
import { NGrid, NGi, NCard, NSpin, NDataTable, NEmpty, NButton, type DataTableColumns, type DataTableRowKey } from 'naive-ui'
import PageHeader from './PageHeader.vue'
import FilterBar, { type FilterItem } from './FilterBar.vue'

export interface ActionConfig {
  label: string
  type?: 'text' | 'link' | 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
  icon?: string
  show?: (row: any) => boolean
  disabled?: (row: any) => boolean
  onClick: (row: any) => void | Promise<void>
}

export interface StatConfig {
  label: string
  value: string | number
  icon?: string
  iconColor?: string
}

interface Props {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 筛选器配置（可选） */
  filters?: FilterItem[]
  /** 筛选值对象 */
  filterValue?: Record<string, any>
  /** 数据表格列配置 */
  columns: DataTableColumns
  /** 表格数据 */
  data: any[]
  /** 是否加载中 */
  loading?: boolean
  /** 分页配置 */
  pagination?: {
    page: number
    pageSize: number
    total: number
    showSizePicker?: boolean
    pageSizes?: number[]
    showQuickJumper?: boolean
  }
  /** 是否显示统计卡片 */
  showStats?: boolean
  /** 统计卡片配置 */
  stats?: StatConfig[]
  /** 表格配置 */
  tableConfig?: {
    rowKey?: string | ((row: any) => string | number)
    singleLine?: boolean
    maxHeight?: string | number
    bordered?: boolean
    rowClassName?: string | ((row: any, index: number) => string)
  }
  /** 操作按钮配置（自动添加到表格最后列） */
  actions?: ActionConfig[]
  /** 空状态配置 */
  emptyConfig?: {
    icon?: string
    text?: string
    description?: string
    showAction?: boolean
    action?: { label: string; onClick: () => void }
  }
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 是否显示搜索按钮 */
  showSearch?: boolean
  /** 筛选器布局方式 */
  filterLayout?: 'horizontal' | 'vertical' | 'grid'
  /** 筛选器每行显示数量 */
  filterPerLine?: number
  /** 统计卡片列数 */
  statsCols?: number | 'responsive'
  /** 额外类名 */
  class?: string | string[] | Record<string, boolean>
  /** 自定义样式 */
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  filterValue: () => ({}),
  loading: false,
  showStats: false,
  showReset: true,
  showSearch: false,
  filterLayout: 'horizontal',
  filterPerLine: 3,
  statsCols: 3
})

const emit = defineEmits<{
  (event: 'filter-change', value: Record<string, any>): void
  (event: 'search', value: Record<string, any>): void
  (event: 'reset'): void
  (event: 'page-change', page: number, pageSize: number): void
  (event: 'row-click', row: any, index: number): void
  (event: 'row-check', rowKeys: DataTableRowKey[]): void
  (event: 'sort-change', sorters: Record<string, 'ascend' | 'descend'>): void
  (event: 'action-click', action: string, row: any): void
}>()

// 表格 key 用于强制刷新
const tableKey = ref(0)

// 内部筛选值
const internalFilterValue = ref<Record<string, any>>({ ...props.filterValue })

// 内部操作按钮（添加到表头的）
const headerActions = computed(() => {
  // 如果有 filters，可以在这里添加搜索按钮
  // 目前暂不添加，由 FilterBar 组件内部处理
  return []
})

// 构建操作列
const actionsColumn = computed(() => {
  if (!props.actions?.length) return null

  return {
    title: '操作',
    key: 'actions',
    fixed: 'right' as const,
    width: props.actions.length * 80,
    render: (row: any, index: number) => {
      return h('div', { class: 'table-actions' }, [
        h('div', { class: 'actions-prepend' }),
        ...props.actions!.map((action) => {
          // 检查是否显示
          if (action.show && !action.show(row)) return null

          const isDisabled = action.disabled ? action.disabled(row) : false

          return h(
            'n-button',
            {
              type: action.type || 'text',
              size: 'small',
              disabled: isDisabled,
              style: { marginRight: '8px' },
              onClick: async (e: Event) => {
                e.stopPropagation()
                await action.onClick(row)
                emit('action-click', action.label, row)
              }
            },
            { default: () => action.label }
          )
        }).filter(Boolean),
        h('div', { class: 'actions-append' })
      ])
    }
  }
})

// 内部列配置（包含操作列）
const internalColumns = computed(() => {
  if (actionsColumn.value) {
    return [...props.columns, actionsColumn.value]
  }
  return props.columns
})

// 内部分页配置
const internalPagination = computed(() => {
  if (!props.pagination) return false
  return {
    page: props.pagination.page,
    pageSize: props.pagination.pageSize,
    pageCount: Math.ceil(props.pagination.total / props.pagination.pageSize),
    showSizePicker: props.pagination.showSizePicker ?? true,
    pageSizes: props.pagination.pageSizes ?? [10, 20, 50, 100],
    showQuickJumper: props.pagination.showQuickJumper ?? true
  }
})

// 行配置
const rowProps = (row: any) => {
  return {
    style: 'cursor: pointer;',
    onClick: () => {
      const index = props.data.indexOf(row)
      emit('row-click', row, index)
    }
  }
}

// 处理筛选变化
const handleFilterChange = (value: Record<string, any>) => {
  internalFilterValue.value = { ...value }
  emit('filter-change', value)
}

// 处理搜索
const handleSearch = (value: Record<string, any>) => {
  emit('search', value)
}

// 处理重置
const handleReset = () => {
  internalFilterValue.value = {}
  emit('reset')
}

// 处理分页变化
const handlePageChange = (page: number) => {
  if (props.pagination) {
    emit('page-change', page, props.pagination.pageSize)
  }
}

// 处理每页条数变化
const handlePageSizeChange = (pageSize: number) => {
  if (props.pagination) {
    emit('page-change', 1, pageSize) // 切换页大小时回到第一页
  }
}

// 处理行选中
const handleRowCheck = (rowKeys: DataTableRowKey[]) => {
  emit('row-check', rowKeys)
}

// 处理排序变化
const handleSortChange = (sorters: Record<string, 'ascend' | 'descend'>) => {
  emit('sort-change', sorters)
}

// 处理头部操作点击
const handleActionClick = (action: any) => {
  // 目前仅作为占位，头部操作在 actions 插槽中处理
}
</script>

<style scoped>
.admin-list-page {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

/* 统计卡片 */
.stats-section {
  margin: var(--spacing-md) 0;
}

.stat-card {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
}

.stat-content {
  display: flex;
  align-items: center;
  gap: var(--spacing-lg);
}

.stat-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.05);
  font-size: var(--text-xl);
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: var(--text-2xl);
  font-weight: 700;
  color: var(--color-text-main);
  line-height: 1.2;
}

.stat-label {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  margin-top: var(--spacing-xs);
}

/* 筛选器 */
.filter-section {
  margin: var(--spacing-md) 0;
}

/* 表格 */
.table-section {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.table-footer {
  padding: var(--spacing-lg);
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

.empty-description {
  color: var(--color-text-muted);
  margin-top: var(--spacing-sm);
}

.table-actions {
  display: flex;
  align-items: center;
}

/* 页面底部 */
.page-footer {
  padding: var(--spacing-lg) 0;
}

/* 响应式 */
@media (max-width: 1024px) {
  .stats-section :deep(.n-grid) {
    grid-template-columns: repeat(2, 1fr) !important;
  }
}

@media (max-width: 640px) {
  .stats-section :deep(.n-grid) {
    grid-template-columns: 1fr !important;
  }

  .stat-content {
    gap: var(--spacing-md);
  }

  .stat-icon {
    width: 40px;
    height: 40px;
    font-size: var(--text-lg);
  }

  .stat-value {
    font-size: var(--text-xl);
  }
}
</style>
