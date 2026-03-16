<template>
  <n-form-item
    :label="props.field.label"
    :path="props.field.field"
    :rule="props.field.required ? { required: true, message: `${props.field.label}为必填项`, trigger: 'blur' } : undefined"
    :label-placement="props.field.layout || 'top'"
    :label-width="props.field.labelWidth"
  >
    <!-- 输入框 -->
    <n-input
      v-if="props.field.type === 'input'"
      v-model:value="localValue"
      :placeholder="props.field.placeholder || `请输入${props.field.label}`"
      :clearable="true"
      :disabled="props.field.disabled"
      :readonly="props.field.readonly"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 文本域 -->
    <n-input
      v-else-if="props.field.type === 'textarea'"
      v-model:value="localValue"
      type="textarea"
      :placeholder="props.field.placeholder || `请输入${props.field.label}`"
      :rows="props.field.rows || 4"
      :clearable="true"
      :disabled="props.field.disabled"
      :readonly="props.field.readonly"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 数字输入框 -->
    <n-input-number
      v-else-if="props.field.type === 'input-number'"
      v-model:value="localValue"
      :placeholder="props.field.placeholder || `请输入${props.field.label}`"
      :min="props.field.min"
      :max="props.field.max"
      :step="props.field.step"
      :disabled="props.field.disabled"
      :readonly="props.field.readonly"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 下拉选择 -->
    <n-select
      v-else-if="props.field.type === 'select'"
      v-model:value="localValue"
      :placeholder="props.field.placeholder || `请选择${props.field.label}`"
      :options="props.field.options"
      :clearable="true"
      :disabled="props.field.disabled"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 开关 -->
    <n-switch
      v-else-if="props.field.type === 'switch'"
      v-model:value="localValue"
      :disabled="props.field.disabled"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 日期选择器 -->
    <n-date-picker
      v-else-if="props.field.type === 'date-picker'"
      v-model:value="localValue"
      type="date"
      :placeholder="props.field.placeholder || `请选择${props.field.label}`"
      :clearable="true"
      :disabled="props.field.disabled"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 时间选择器 -->
    <n-time-picker
      v-else-if="props.field.type === 'time-picker'"
      v-model:value="localValue"
      :placeholder="props.field.placeholder || `请选择${props.field.label}`"
      :clearable="true"
      :disabled="props.field.disabled"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 日期范围选择器 -->
    <n-date-picker
      v-else-if="props.field.type === 'date-range-picker'"
      v-model:value="localValue"
      type="daterange"
      :placeholder="props.field.placeholder as any || ['开始日期', '结束日期']"
      :clearable="true"
      :disabled="props.field.disabled"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 颜色选择器 -->
    <n-color-picker
      v-else-if="props.field.type === 'color-picker'"
      v-model:value="localValue"
      :disabled="props.field.disabled"
      v-bind="props.field.props"
      @update:value="handleUpdate"
    />

    <!-- 提示文本 -->
    <template v-if="props.field.hint" #feedback>
      <span class="form-hint">{{ props.field.hint }}</span>
    </template>

    <!-- 错误提示 -->
    <template v-if="props.field.errorTip" #feedback>
      <span class="form-error">{{ props.field.errorTip }}</span>
    </template>

    <!-- 自定义插槽 -->
    <slot></slot>
  </n-form-item>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { NFormItem, NInput, NInputNumber, NSelect, NSwitch, NDatePicker, NTimePicker, NColorPicker } from 'naive-ui'
import type { FormItemConfig } from './FormPage.vue'

interface Props {
  field: FormItemConfig
  value: any
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (event: 'update:value', value: any): void
}>()

const localValue = ref(props.value)

watch(() => props.value, (newValue) => {
  localValue.value = newValue
}, { deep: true })

const handleUpdate = (value: any) => {
  localValue.value = value
  emit('update:value', value)
}
</script>

<style scoped>
.form-hint {
  font-size: var(--text-xs);
  color: var(--color-text-muted);
}

.form-error {
  font-size: var(--text-xs);
  color: var(--color-error);
}
</style>
