<template>
  <div class="admin-filter-bar" :class="[`layout-${props.layout}`, props.collapsed ? 'collapsed' : '', props.class]" :style="props.style">
    <slot :filter-value="internalValue">
      <div class="filter-form" :class="{ 'grid-layout': props.layout === 'grid' }">
        <div
          v-for="filter in visibleFilters"
          :key="filter.field"
          class="filter-item"
          :class="filter.class"
        >
          <label v-if="filter.label" class="filter-label">
            {{ filter.label }}
          </label>

          <!-- 自定义插槽 -->
          <slot
            :name="`filter-${filter.field}`"
            :field="filter.field"
            :value="internalValue[filter.field]"
            :item="filter"
          >
            <!-- 输入框 -->
            <n-input
              v-if="filter.type === 'input'"
              v-model:value="internalValue[filter.field]"
              :placeholder="filter.placeholder || '请输入...'"
              :clearable="filter.clearable ?? true"
              :disabled="filter.disabled"
              @update:value="handleFilterChange(filter.field, $event)"
            />

            <!-- 数字输入框 -->
            <n-input-number
              v-else-if="filter.type === 'input-number'"
              v-model:value="internalValue[filter.field]"
              :placeholder="filter.placeholder || '请输入数字'"
              :disabled="filter.disabled"
              @update:value="handleFilterChange(filter.field, $event)"
            />

            <!-- 下拉选择 -->
            <n-select
              v-else-if="filter.type === 'select'"
              v-model:value="internalValue[filter.field]"
              :placeholder="filter.placeholder || '请选择'"
              :options="filter.options"
              :clearable="filter.clearable ?? true"
              :disabled="filter.disabled"
              @update:value="handleFilterChange(filter.field, $event)"
            />

            <!-- 日期范围选择 -->
            <n-date-picker
              v-else-if="filter.type === 'date-range'"
              v-model:value="internalValue[filter.field]"
              type="daterange"
              :placeholder="filter.placeholder || ['开始日期', '结束日期']"
              :clearable="filter.clearable ?? true"
              :disabled="filter.disabled"
              @update:value="handleFilterChange(filter.field, $event)"
            />

            <!-- 日期选择 -->
            <n-date-picker
              v-else-if="filter.type === 'date-picker'"
              v-model:value="internalValue[filter.field]"
              type="date"
              :placeholder="filter.placeholder || '请选择日期'"
              :clearable="filter.clearable ?? true"
              :disabled="filter.disabled"
              @update:value="handleFilterChange(filter.field, $event)"
            />

            <!-- 数字范围选择 -->
            <n-input-group v-else-if="filter.type === 'number-range'" class="number-range-group">
              <n-input-number
                v-model:value="internalValue[`${filter.field}_min`]"
                :placeholder="filter.placeholder?.[0] || '最小值'"
                :disabled="filter.disabled"
                @update:value="handleFilterChange(`${filter.field}_min`, $event)"
              />
              <span class="range-separator">-</span>
              <n-input-number
                v-model:value="internalValue[`${filter.field}_max`]"
                :placeholder="filter.placeholder?.[1] || '最大值'"
                :disabled="filter.disabled"
                @update:value="handleFilterChange(`${filter.field}_max`, $event)"
              />
            </n-input-group>
          </slot>
        </div>
      </div>
    </slot>

    <!-- 操作按钮 -->
    <div class="filter-actions">
      <slot name="actions-prepend"></slot>

      <n-space :size="12">
        <n-button
          v-if="props.showSearch"
          type="primary"
          :loading="props.searching"
          @click="handleSearch"
        >
          <template #icon>
            <i class="fas fa-search"></i>
          </template>
          {{ props.searchText || '搜索' }}
        </n-button>

        <n-button
          v-if="props.showReset"
          @click="handleReset"
        >
          <template #icon>
            <i class="fas fa-redo"></i>
          </template>
          {{ props.resetText || '重置' }}
        </n-button>
      </n-space>

      <slot name="actions-append"></slot>

      <!-- 折叠按钮 -->
      <n-button
        v-if="props.collapsible && filters.length > maxVisible"
        text
        @click="toggleCollapse"
      >
        <template #icon>
          <i :class="props.collapsed ? 'fas fa-chevron-down' : 'fas fa-chevron-up'"></i>
        </template>
        {{ props.collapsed ? '展开' : '收起' }}
      </n-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { NInput, NInputNumber, NSelect, NDatePicker, NInputGroup, NButton, NSpace } from 'naive-ui'

export interface FilterItem {
  type: 'input' | 'input-number' | 'select' | 'date-range' | 'date-picker' | 'number-range'
  field: string
  label?: string
  placeholder?: string | string[]
  options?: Array<{ label: string; value: any; disabled?: boolean }>
  defaultValue?: any
  clearable?: boolean
  disabled?: boolean
  class?: string | string[] | Record<string, boolean>
}

interface Props {
  /** 筛选器配置项 */
  filters: FilterItem[]
  /** 筛选值对象 */
  modelValue?: Record<string, any>
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 重置按钮文本 */
  resetText?: string
  /** 是否显示搜索按钮 */
  showSearch?: boolean
  /** 搜索按钮文本 */
  searchText?: string
  /** 搜索按钮加载状态 */
  searching?: boolean
  /** 筛选器布局方式 */
  layout?: 'horizontal' | 'vertical' | 'grid'
  /** 水平布局时每行显示的筛选器数量 */
  perLine?: number
  /** 折叠展开筛选器 */
  collapsible?: boolean
  /** 默认折叠状态 */
  collapsed?: boolean
  /** 额外类名 */
  class?: string | string[] | Record<string, boolean>
  /** 自定义样式 */
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({}),
  showReset: true,
  showSearch: false,
  resetText: '重置',
  searchText: '搜索',
  layout: 'horizontal',
  perLine: 3,
  collapsible: false,
  collapsed: false
})

const emit = defineEmits<{
  (event: 'update:modelValue', value: Record<string, any>): void
  (event: 'search', value: Record<string, any>): void
  (event: 'reset'): void
  (event: 'filter-change', field: string, value: any): void
  (event: 'toggle-collapse', collapsed: boolean): void
}>()

// 内部状态
const isCollapsed = ref(props.collapsed)
const maxVisible = props.perLine || 3

// 初始化筛选值
const initializeValue = () => {
  const value: Record<string, any> = { ...props.modelValue }
  props.filters.forEach(filter => {
    if (value[filter.field] === undefined && filter.defaultValue !== undefined) {
      value[filter.field] = filter.defaultValue
    }
    if (filter.type === 'number-range') {
      if (value[`${filter.field}_min`] === undefined) {
        value[`${filter.field}_min`] = filter.defaultValue?.min
      }
      if (value[`${filter.field}_max`] === undefined) {
        value[`${filter.field}_max`] = filter.defaultValue?.max
      }
    }
  })
  return value
}

const internalValue = ref<Record<string, any>>(initializeValue())

// 计算可见的筛选器
const visibleFilters = computed(() => {
  if (!props.collapsible) return props.filters
  return props.collapsed ? props.filters.slice(0, maxVisible) : props.filters
})

// 处理筛选器变化
const handleFilterChange = (field: string, value: any) => {
  internalValue.value[field] = value
  emit('filter-change', field, value)
  emit('update:modelValue', { ...internalValue.value })
}

// 处理搜索
const handleSearch = () => {
  emit('search', { ...internalValue.value })
}

// 处理重置
const handleReset = () => {
  const resetValue: Record<string, any> = {}
  props.filters.forEach(filter => {
    if (filter.defaultValue !== undefined) {
      resetValue[filter.field] = filter.defaultValue
    }
    if (filter.type === 'number-range') {
      if (filter.defaultValue?.min !== undefined) {
        resetValue[`${filter.field}_min`] = filter.defaultValue.min
      }
      if (filter.defaultValue?.max !== undefined) {
        resetValue[`${filter.field}_max`] = filter.defaultValue.max
      }
    }
  })
  internalValue.value = resetValue
  emit('reset')
  emit('update:modelValue', { ...resetValue })
}

// 切换折叠状态
const toggleCollapse = () => {
  isCollapsed.value = !isCollapsed.value
  emit('toggle-collapse', isCollapsed.value)
}

// 监听外部 modelValue 变化
watch(() => props.modelValue, (newValue) => {
  internalValue.value = { ...newValue }
}, { deep: true })

// 监听折叠 prop 变化
watch(() => props.collapsed, (newValue) => {
  isCollapsed.value = newValue
})
</script>

<style scoped>
.admin-filter-bar {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
}

/* 水平布局 */
.layout-horizontal .filter-form {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-lg);
  align-items: flex-end;
}

.layout-horizontal .filter-item {
  min-width: 200px;
  flex: 0 0 auto;
}

.layout-horizontal.layout-grid .filter-form {
  display: grid;
  grid-template-columns: repeat(var(--per-line, 3), 1fr);
  gap: var(--spacing-lg);
}

/* 垂直布局 */
.layout-vertical .filter-form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.layout-vertical .filter-item {
  width: 100%;
}

/* Grid 布局 */
.layout-grid .filter-form {
  display: grid;
  grid-template-columns: repeat(var(--per-line, 3), 1fr);
  gap: var(--spacing-lg);
}

.filter-label {
  display: block;
  font-size: var(--text-sm);
  color: var(--color-text-sec);
  margin-bottom: var(--spacing-sm);
  font-weight: 500;
}

.filter-item :deep(.n-input),
.filter-item :deep(.n-input-number),
.filter-item :deep(.n-select),
.filter-item :deep(.n-date-picker) {
  width: 100%;
}

.number-range-group {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.range-separator {
  color: var(--color-text-muted);
  font-size: var(--text-sm);
}

.filter-actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: var(--spacing-md);
  padding-top: var(--spacing-md);
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

/* 响应式 */
@media (max-width: 1024px) {
  .layout-horizontal .filter-item,
  .layout-grid .filter-item {
    min-width: calc(50% - var(--spacing-md));
  }

  .layout-horizontal.layout-grid .filter-form,
  .layout-grid .filter-form {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .layout-horizontal .filter-item,
  .layout-grid .filter-item {
    min-width: 100%;
  }

  .layout-horizontal.layout-grid .filter-form,
  .layout-grid .filter-form {
    grid-template-columns: 1fr;
  }

  .filter-actions {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-actions :deep(.n-space) {
    width: 100%;
  }

  .filter-actions :deep(.n-button) {
    width: 100%;
  }
}
</style>
