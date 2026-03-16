<template>
  <div class="admin-detail-page" :class="props.class" :style="props.style">
    <!-- 页面头部 -->
    <slot name="header">
      <PageHeader
        :title="props.title"
        :description="props.description"
        :show-actions="props.showActions"
        :actions="props.actions"
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

    <!-- 详情区域 -->
    <div class="detail-section">
      <n-spin :show="props.loading">
        <!-- 分组详情 -->
        <template v-if="props.groups?.length">
          <div
            v-for="group in props.groups"
            :key="group.key"
            class="detail-group"
          >
            <div v-if="group.title" class="group-header">
              <h3 class="group-title">{{ group.title }}</h3>
            </div>

            <div class="detail-list" :class="`layout-${props.layout}`">
              <template
                v-for="fieldKey in group.fields"
                :key="fieldKey"
              >
                <slot
                  :name="`field-${fieldKey}`"
                  :field="fieldKey"
                  :value="props.data[fieldKey]"
                  :data="props.data"
                >
                  <DetailItemRenderer
                    :field="getFieldConfig(fieldKey)"
                    :value="props.data[fieldKey]"
                    :data="props.data"
                  />
                </slot>
              </template>
            </div>
          </div>
        </template>

        <!-- 默认详情（无分组） -->
        <template v-else>
          <div class="detail-list" :class="`layout-${props.layout}`">
            <DetailItemRenderer
              v-for="field in visibleFields"
              :key="field.field"
              :field="field"
              :value="props.data[field.field]"
              :data="props.data"
            >
              <template
                v-for="slot in Object.keys($slots)"
                :key="slot"
                #[slot]="slotProps"
              >
                <slot
                  :name="slot"
                  v-bind="slotProps"
                  :field="field.field"
                  :value="props.data[field.field]"
                  :data="props.data"
                ></slot>
              </template>
            </DetailItemRenderer>
          </div>
        </template>
      </n-spin>
    </div>

    <!-- 分隔线 -->
    <div v-if="props.showDivider" class="detail-divider"></div>

    <!-- 底部操作按钮 -->
    <div v-if="props.showActions && props.actions" class="detail-actions">
      <n-space :size="12">
        <n-button
          v-for="action in props.actions"
          :key="action.label"
          :type="action.type"
          :disabled="action.disabled && action.disabled(props.data)"
          @click="handleAction(action)"
        >
          <template v-if="action.icon" #icon>
            <i :class="action.icon"></i>
          </template>
          {{ action.label }}
        </n-button>
      </n-space>
    </div>

    <!-- 页面底部 -->
    <div v-if="$slots.footer" class="page-footer">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, h } from 'vue'
import { NSpin, NSpace, NButton, NTag, NImage, NTime } from 'naive-ui'
import PageHeader from './PageHeader.vue'
import DetailItemRenderer from './DetailItemRenderer.vue'
import type { formatTime } from 'naive-ui'

export interface DetailItemConfig {
  field: string
  label: string
  type?: 'text' | 'number' | 'date' | 'datetime' | 'time' | 'boolean' | 'tag' | 'image' | 'link' | 'html' | 'json'
  formatter?: (value: any, data: Record<string, any>) => string | any
  tagConfig?: {
    type?: 'default' | 'primary' | 'info' | 'success' | 'warning' | 'error'
    bordered?: boolean
  }
  linkConfig?: {
    to: string | ((value: any, data: Record<string, any>) => string)
    target?: '_blank' | '_self'
  }
  show?: (value: any, data: Record<string, any>) => boolean
  class?: string | string[] | Record<string, boolean>
}

export interface DetailAction {
  label: string
  type?: 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
  icon?: string
  onClick: (data: Record<string, any>) => void | Promise<void>
  disabled?: (data: Record<string, any>) => boolean
}

export interface DetailGroup {
  key: string
  title: string
  fields: string[]
}

interface Props {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 数据详情对象 */
  data: Record<string, any>
  /** 详情字段配置 */
  fields?: DetailItemConfig[]
  /** 是否加载中 */
  loading?: boolean
  /** 是否显示操作按钮 */
  showActions?: boolean
  /** 操作按钮配置 */
  actions?: DetailAction[]
  /** 详情列表布局方式 */
  layout?: 'list' | 'grid' | 'table'
  /** Grid 布局时列数 */
  columns?: number
  /** 是否显示分隔线 */
  showDivider?: boolean
  /** 分组配置（可选） */
  groups?: DetailGroup[]
  /** 额外类名 */
  class?: string | string[] | Record<string, boolean>
  /** 自定义样式 */
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  fields: () => [],
  loading: false,
  showActions: false,
  actions: () => [],
  layout: 'list',
  columns: 2,
  showDivider: false
})

const emit = defineEmits<{
  (event: 'action-click', action: string, data: Record<string, any>): void
}>()

// 计算可见字段
const visibleFields = computed(() => {
  return props.fields.filter(field => {
    const value = props.data[field.field]
    return field.show ? field.show(value, props.data) : true
  })
})

// 获取字段配置
const getFieldConfig = (fieldKey: string): DetailItemConfig | undefined => {
  return props.fields.find(f => f.field === fieldKey)
}

// 处理操作按钮点击（头部）
const handleActionClick = (action: DetailAction) => {
  action.onClick(props.data)
  emit('action-click', action.label, props.data)
}

// 处理底部操作按钮
const handleAction = (action: DetailAction) => {
  action.onClick(props.data)
  emit('action-click', action.label, props.data)
}
</script>

<style scoped>
.admin-detail-page {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.detail-section {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
}

.detail-group {
  margin-bottom: var(--spacing-2xl);
}

.detail-group:last-child {
  margin-bottom: 0;
}

.group-header {
  margin-bottom: var(--spacing-lg);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.group-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

/* 列表布局 */
.layout-list .detail-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

/* Grid 布局 */
.layout-grid .detail-list {
  display: grid;
  grid-template-columns: repeat(var(--columns, 2), 1fr);
  gap: var(--spacing-lg);
}

/* 表格布局 */
.layout-table .detail-list {
  display: table;
  width: 100%;
}

.layout-table :deep(.detail-item) {
  display: table-row;
}

.layout-table :deep(.detail-label) {
  display: table-cell;
  padding: var(--spacing-md);
  font-weight: 500;
  color: var(--color-text-sec);
  width: 200px;
}

.layout-table :deep(.detail-value) {
  display: table-cell;
  padding: var(--spacing-md);
  color: var(--color-text-main);
}

.detail-divider {
  height: 1px;
  background: rgba(255, 255, 255, 0.08);
  margin: var(--spacing-xl) 0;
}

.detail-actions {
  padding-top: var(--spacing-md);
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

.page-footer {
  padding: var(--spacing-lg) 0;
}

/* 响应式 */
@media (max-width: 1024px) {
  .layout-grid .detail-list {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .layout-grid .detail-list {
    grid-template-columns: 1fr;
  }

  .layout-table :deep(.detail-label),
  .layout-table :deep(.detail-value) {
    display: block;
    padding: var(--spacing-sm) 0;
  }

  .layout-table :deep(.detail-label) {
    width: 100%;
  }

  .detail-actions :deep(.n-space) {
    flex-direction: column;
  }

  .detail-actions :deep(.n-button) {
    width: 100%;
  }
}
</style>
