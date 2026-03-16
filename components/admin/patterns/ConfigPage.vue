<template>
  <div class="admin-config-page" :class="[`preview-${props.previewPosition}`, props.collapsible ? 'collapsible' : '', props.class]" :style="props.style">
    <!-- 页面头部 -->
    <slot name="header">
      <PageHeader
        :title="props.title"
        :description="props.description"
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

    <!-- 主内容区域 -->
    <div class="config-content" :class="{ 'with-preview': props.showPreview }">
      <!-- 配置表单区域 -->
      <div class="config-form-section">
        <n-spin :show="props.loading">
          <!-- 分组配置 -->
          <template v-if="props.groups?.length">
            <div
              v-for="group in props.groups"
              :key="group.key"
              class="config-group"
              :class="{ 'collapsed': group.collapsed ?? groupCollapsedStates[group.key] }"
            >
              <div class="group-header" @click="toggleGroup(group.key, group.collapsible)">
                <h3 class="group-title">
                  <i v-if="group.collapsible" :class="groupCollapsedStates[group.key] ? 'fas fa-chevron-right' : 'fas fa-chevron-down'" class="group-toggle-icon"></i>
                  {{ group.title }}
                </h3>
                <p v-if="group.description" class="group-description">{{ group.description }}</p>
              </div>

              <div v-show="!group.collapsible || !groupCollapsedStates[group.key]" class="group-content">
                <slot :name="`config-group-${group.key}`" :group="group" :value="internalValue">
                  <div class="config-items">
                    <ConfigItemRenderer
                      v-for="item in group.items"
                      :key="item.key"
                      :item="item"
                      :value="internalValue[item.key]"
                      @update:value="handleConfigChange"
                    >
                      <template
                        v-for="slot in Object.keys($slots)"
                        :key="slot"
                        #[slot]="slotProps"
                      >
                        <slot
                          :name="slot"
                          v-bind="slotProps"
                          :item="item.key"
                          :value="internalValue[item.key]"
                        ></slot>
                      </template>
                    </ConfigItemRenderer>
                  </div>
                </slot>
              </div>
            </div>
          </template>

          <!-- 无分组配置 -->
          <template v-else>
            <div class="config-items">
              <slot name="default" :value="internalValue">
                <!-- 由外部通过 slot 传入配置项 -->
              </slot>
            </div>
          </template>

          <!-- 操作按钮 -->
          <div v-if="props.showActions" class="config-actions">
            <slot name="actions-prepend"></slot>

            <n-space :size="12">
              <n-button
                type="primary"
                :loading="props.saving"
                size="large"
                @click="handleSave"
              >
                <template #icon>
                  <i class="fas fa-save"></i>
                </template>
                {{ props.saveText || '保存配置' }}
              </n-button>

              <n-button
                v-if="props.showReset"
                :disabled="props.saving"
                @click="handleReset"
              >
                <template #icon>
                  <i class="fas fa-undo"></i>
                </template>
                {{ props.resetText || '恢复默认' }}
              </n-button>
            </n-space>

            <slot name="actions-append"></slot>
          </div>
        </n-spin>
      </div>

      <!-- 预览区域 -->
      <div v-if="props.showPreview" class="config-preview-section">
        <div class="preview-header">
          <h3 class="preview-title">{{ props.previewTitle || '预览' }}</h3>
        </div>
        <div class="preview-content">
          <slot name="preview" :value="internalValue">
            <div class="preview-placeholder">
              <i class="fas fa-eye"></i>
              <p>配置预览区域</p>
            </div>
          </slot>
        </div>
      </div>
    </div>

    <!-- 页面底部 -->
    <div v-if="$slots.footer" class="page-footer">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { NSpin, NSpace, NButton } from 'naive-ui'
import PageHeader from './PageHeader.vue'
import ConfigItemRenderer from './ConfigItemRenderer.vue'
import type { ConfigGroup } from './types'

interface Props {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 配置分组列表 */
  groups?: ConfigGroup[]
  /** 配置值对象 */
  modelValue: Record<string, any>
  /** 是否加载中 */
  loading?: boolean
  /** 是否显示预览区域 */
  showPreview?: boolean
  /** 预览标题 */
  previewTitle?: string
  /** 预览位置 */
  previewPosition?: 'side' | 'bottom' | 'modal'
  /** 是否显示操作按钮 */
  showActions?: boolean
  /** 保存按钮文本 */
  saveText?: string
  /** 保存按钮加载状态 */
  saving?: boolean
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 重置按钮文本 */
  resetText?: string
  /** 支持实时预览 */
  collapsible?: boolean
  /** 额外类名 */
  class?: string | string[] | Record<string, boolean>
  /** 自定义样式 */
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({}),
  loading: false,
  showPreview: false,
  previewPosition: 'side',
  showActions: true,
  saveText: '保存配置',
  showReset: true,
  resetText: '恢复默认',
  collapsible: false
})

const emit = defineEmits<{
  (event: 'update:modelValue', value: Record<string, any>): void
  (event: 'save', value: Record<string, any>): void
  (event: 'reset'): void
  (event: 'config-change', key: string, value: any): void
}>()

// 内部值
const internalValue = ref<Record<string, any>>({ ...props.modelValue })

// 分组折叠状态
const groupCollapsedStates = ref<Record<string, boolean>>({})

// 初始化分组折叠状态
const initCollapsedStates = () => {
  if (props.groups) {
    props.groups.forEach(group => {
      if (group.collapsed !== undefined) {
        groupCollapsedStates.value[group.key] = group.collapsed
      }
    })
  }
}

// 切换分组折叠状态
const toggleGroup = (groupKey: string, collapsible?: boolean) => {
  if (collible) {
    groupCollapsedStates.value[groupKey] = !groupCollapsedStates.value[groupKey]
  }
}

// 处理配置变化
const handleConfigChange = (key: string, value: any) => {
  internalValue.value[key] = value
  emit('config-change', key, value)
  emit('update:modelValue', { ...internalValue.value })
}

// 处理保存
const handleSave = () => {
  emit('save', { ...internalValue.value })
}

// 处理重置
const handleReset = () => {
  emit('reset')
}

// 监听外部 modelValue 变化
watch(() => props.modelValue, (newValue) => {
  internalValue.value = { ...newValue }
}, { deep: true })

// 初始化
initCollapsedStates()
</script>

<style scoped>
.admin-config-page {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

/* 侧边预览布局 */
.preview-side.config-content {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: var(--spacing-xl);
  align-items: start;
}

/* 底部预览布局 */
.preview-bottom.config-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

/* 模态预览布局 */
.preview-modal .config-preview-section {
  display: none;
}

.config-form-section {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  min-width: 0;
}

.config-group {
  margin-bottom: var(--spacing-2xl);
}

.config-group:last-child {
  margin-bottom: 0;
}

.group-header {
  padding: var(--spacing-md) 0;
  margin-bottom: var(--spacing-lg);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.group-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-xs) 0;
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.group-toggle-icon {
  font-size: var(--text-sm);
  transition: transform 0.2s;
  cursor: pointer;
}

.config-group.collapsed .group-toggle-icon {
  transform: rotate(-90deg);
}

.group-description {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  margin: 0;
  padding-left: calc(var(--text-sm) + var(--spacing-sm));
}

.group-content {
  padding-top: var(--spacing-md);
}

.config-items {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.config-actions {
  margin-top: var(--spacing-2xl);
  padding-top: var(--spacing-xl);
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

/* 预览区域 */
.config-preview-section {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  position: sticky;
  top: var(--spacing-lg);
}

.preview-header {
  margin-bottom: var(--spacing-lg);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.preview-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.preview-content {
  min-height: 200px;
}

.preview-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: var(--color-text-muted);
  gap: var(--spacing-md);
}

.preview-placeholder i {
  font-size: var(--text-3xl);
}

.page-footer {
  padding: var(--spacing-lg) 0;
}

/* 响应式 */
@media (max-width: 1200px) {
  .preview-side.config-content {
    grid-template-columns: 1fr;
  }

  .config-preview-section {
    position: static;
  }
}

@media (max-width: 768px) {
  .config-actions :deep(.n-space) {
    flex-direction: column;
  }

  .config-actions :deep(.n-button) {
    width: 100%;
  }
}
</style>
