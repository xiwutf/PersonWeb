<template>
  <div class="admin-form-page" :class="props.class" :style="props.style">
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

    <!-- 表单区域 -->
    <div class="form-section">
      <n-spin :show="props.loading">
        <n-form
          ref="formRef"
          :model="internalValue"
          :label-placement="props.labelPlacement"
          :label-width="props.labelWidth"
          :require-mark-placement="props.requireMarkPlacement"
          :show-label="!!props.layout || props.labelPlacement !== 'inline'"
        >
          <!-- 分组表单 -->
          <template v-if="props.groups?.length">
            <div
              v-for="group in props.groups"
              :key="group.key"
              class="form-group"
            >
              <div v-if="group.title" class="group-header">
                <h3 class="group-title">{{ group.title }}</h3>
                <p v-if="group.description" class="group-description">{{ group.description }}</p>
              </div>

              <slot name="form" :value="internalValue">
                <div class="group-fields">
                  <template
                    v-for="fieldKey in group.fields"
                    :key="fieldKey"
                  >
                    <slot
                      :name="`field-${fieldKey}`"
                      :field="fieldKey"
                      :value="internalValue[fieldKey]"
                      :item="getFieldConfig(fieldKey)"
                    >
                      <FormItemRenderer
                        :field="getFieldConfig(fieldKey)"
                        :value="internalValue[fieldKey]"
                        @update:value="(val) => handleFieldChange(fieldKey, val)"
                      />
                    </slot>
                  </template>
                </div>
              </slot>
            </div>
          </template>

          <!-- 默认表单（无分组） -->
          <template v-else>
            <slot name="form" :value="internalValue">
              <div class="form-fields">
                <FormItemRenderer
                  v-for="field in props.fields"
                  :key="field.field"
                  :field="field"
                  :value="internalValue[field.field]"
                  @update:value="(val) => handleFieldChange(field.field, val)"
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
                      :value="internalValue[field.field]"
                    ></slot>
                  </template>
                </FormItemRenderer>
              </div>
            </slot>
          </template>
        </n-form>

        <!-- 分隔线 -->
        <div v-if="props.showDivider" class="form-divider"></div>

        <!-- 操作按钮 -->
        <div class="form-footer">
          <slot name="form-footer">
            <n-space :size="12">
              <n-button
                type="primary"
                :loading="props.submitting"
                size="large"
                @click="handleSubmit"
              >
                <template #icon>
                  <i class="fas fa-save"></i>
                </template>
                {{ props.submitText || '保存' }}
              </n-button>

              <n-button
                v-if="props.showReset"
                :disabled="props.submitting"
                @click="handleReset"
              >
                <template #icon>
                  <i class="fas fa-undo"></i>
                </template>
                {{ props.resetText || '重置' }}
              </n-button>
            </n-space>
          </slot>
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
import { ref, computed, watch } from 'vue'
import type { FormInst, FormRules } from 'naive-ui'
import {
  NSpin, NForm, NButton, NSpace
} from 'naive-ui'
import PageHeader from './PageHeader.vue'
import FormItemRenderer from './FormItemRenderer.vue'

export interface FormItemConfig {
  field: string
  label: string
  type:
    | 'input'
    | 'textarea'
    | 'input-number'
    | 'select'
    | 'radio'
    | 'checkbox'
    | 'switch'
    | 'date-picker'
    | 'time-picker'
    | 'date-range-picker'
    | 'upload'
    | 'color-picker'
  placeholder?: string
  rows?: number
  options?: Array<{ label: string; value: any; disabled?: boolean }>
  min?: number
  max?: number
  step?: number
  required?: boolean
  rules?: any[]
  disabled?: boolean
  readonly?: boolean
  hint?: string
  errorTip?: string
  layout?: 'vertical' | 'horizontal'
  labelWidth?: number | string
  class?: string | string[] | Record<string, boolean>
  props?: Record<string, any>
}

export interface FormGroup {
  key: string
  title: string
  description?: string
  collapsible?: boolean
  collapsed?: boolean
  fields: string[]
}

interface Props {
  /** 页面标题 */
  title: string
  /** 页面描述 */
  description?: string
  /** 表单字段配置 */
  fields?: FormItemConfig[]
  /** 表单值对象 */
  modelValue: Record<string, any>
  /** 是否加载中 */
  loading?: boolean
  /** 提交按钮文本 */
  submitText?: string
  /** 提交按钮加载状态 */
  submitting?: boolean
  /** 是否显示重置按钮 */
  showReset?: boolean
  /** 重置按钮文本 */
  resetText?: string
  /** 表单布局方式 */
  layout?: 'vertical' | 'horizontal' | 'inline'
  /** 标签对齐方式 */
  labelPlacement?: 'left' | 'top' | 'right'
  /** 标签宽度 */
  labelWidth?: number | string
  /** 必填标记位置 */
  requireMarkPlacement?: 'left' | 'right' | 'right-hanging'
  /** 是否显示分隔线 */
  showDivider?: boolean
  /** 分组配置（可选） */
  groups?: FormGroup[]
  /** 额外类名 */
  class?: string | string[] | Record<string, boolean>
  /** 自定义样式 */
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  fields: () => [],
  loading: false,
  submitText: '保存',
  showReset: true,
  resetText: '重置',
  layout: 'vertical',
  labelPlacement: 'top',
  requireMarkPlacement: 'right-hanging',
  showDivider: false
})

const emit = defineEmits<{
  (event: 'update:modelValue', value: Record<string, any>): void
  (event: 'submit', value: Record<string, any>): void
  (event: 'reset'): void
  (event: 'field-change', field: string, value: any): void
}>()

const formRef = ref<FormInst | null>(null)

// 初始化表单值
const initializeValue = () => {
  const value: Record<string, any> = { ...props.modelValue }
  if (props.fields?.length) {
    props.fields.forEach(field => {
      if (value[field.field] === undefined && field.type === 'switch') {
        value[field.field] = false
      }
    })
  }
  return value
}

const internalValue = ref<Record<string, any>>(initializeValue())

// 获取字段配置
const getFieldConfig = (fieldKey: string): FormItemConfig | undefined => {
  return props.fields?.find(f => f.field === fieldKey)
}

// 处理字段变化
const handleFieldChange = (field: string, value: any) => {
  internalValue.value[field] = value
  emit('field-change', field, value)
  emit('update:modelValue', { ...internalValue.value })
}

// 处理提交
const handleSubmit = async () => {
  // 触发表单验证
  if (formRef.value) {
    try {
      await formRef.value.validate()
    } catch (errors) {
      return
    }
  }
  emit('submit', { ...internalValue.value })
}

// 处理重置
const handleReset = () => {
  internalValue.value = { ...props.modelValue }
  emit('reset')
}

// 监听外部 modelValue 变化
watch(() => props.modelValue, (newValue) => {
  internalValue.value = { ...newValue }
}, { deep: true })

// 暴露验证方法
defineExpose({
  validate: () => formRef.value?.validate(),
  restoreValidation: () => formRef.value?.restoreValidation()
})
</script>

<style scoped>
.admin-form-page {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.form-section {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
}

.form-group {
  margin-bottom: var(--spacing-2xl);
}

.form-group:last-child {
  margin-bottom: 0;
}

.group-header {
  margin-bottom: var(--spacing-xl);
}

.group-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-xs) 0;
}

.group-description {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  margin: 0;
}

.form-fields,
.group-fields {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.form-divider {
  height: 1px;
  background: rgba(255, 255, 255, 0.08);
  margin: var(--spacing-xl) 0;
}

.form-footer {
  padding-top: var(--spacing-md);
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

.page-footer {
  padding: var(--spacing-lg) 0;
}

/* 响应式 */
@media (max-width: 768px) {
  .form-section {
    padding: var(--spacing-lg);
  }

  .form-footer :deep(.n-space) {
    flex-direction: column;
  }

  .form-footer :deep(.n-button) {
    width: 100%;
  }
}
</style>
