<template>
  <div class="config-item" :class="props.item.class">
    <div class="item-header">
      <label class="item-label">
        {{ props.item.label }}
      </label>
      <button
        v-if="props.item.livePreview && showPreview"
        class="preview-toggle"
        @click="togglePreview"
      >
        <i :class="showPreviewState ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
      </button>
    </div>

    <p v-if="props.item.hint" class="item-hint">
      {{ props.item.hint }}
    </p>

    <p v-if="props.item.warning" class="item-warning">
      <i class="fas fa-exclamation-triangle"></i>
      {{ props.item.warning }}
    </p>

    <!-- 输入框 -->
    <n-input
      v-if="props.item.type === 'input'"
      v-model:value="localValue"
      :placeholder="props.item.placeholder || `请输入${props.item.label}`"
      :clearable="true"
      @update:value="handleUpdate"
    />

    <!-- 文本域 -->
    <n-input
      v-else-if="props.item.type === 'textarea'"
      v-model:value="localValue"
      type="textarea"
      :placeholder="props.item.placeholder || `请输入${props.item.label}`"
      :rows="props.item.rows || 4"
      :clearable="true"
      @update:value="handleUpdate"
    />

    <!-- 数字输入框 -->
    <n-input-number
      v-else-if="props.item.type === 'input-number'"
      v-model:value="localValue"
      :placeholder="props.item.placeholder || `请输入${props.item.label}`"
      :min="props.item.min"
      :max="props.item.max"
      :step="props.item.step"
      @update:value="handleUpdate"
    />

    <!-- 下拉选择 -->
    <n-select
      v-else-if="props.item.type === 'select'"
      v-model:value="localValue"
      :placeholder="props.item.placeholder || `请选择${props.item.label}`"
      :options="props.item.options"
      :clearable="true"
      @update:value="handleUpdate"
    />

    <!-- 开关 -->
    <n-switch
      v-else-if="props.item.type === 'switch'"
      v-model:value="localValue"
      @update:value="handleUpdate"
    />

    <!-- 字体族选择 -->
    <n-select
      v-else-if="props.item.type === 'font-family'"
      v-model:value="localValue"
      :placeholder="props.item.placeholder || '请选择字体'"
      :options="fontFamilyOptions"
      @update:value="handleUpdate"
    >
      <template #label="{ option }">
        <span :style="{ fontFamily: option.value }">{{ option.label }}</span>
      </template>
    </n-select>

    <!-- 字号选择 -->
    <n-select
      v-else-if="props.item.type === 'font-size'"
      v-model:value="localValue"
      :placeholder="props.item.placeholder || '请选择字号'"
      :options="fontSizeOptions"
      @update:value="handleUpdate"
    />

    <!-- 单选组 -->
    <n-radio-group
      v-else-if="props.item.type === 'radio-group'"
      v-model:value="localValue"
      @update:value="handleUpdate"
    >
      <n-space>
        <n-radio
          v-for="option in props.item.options"
          :key="option.value"
          :value="option.value"
          :label="option.label"
        />
      </n-space>
    </n-radio-group>

    <!-- 复选框组 -->
    <n-checkbox-group
      v-else-if="props.item.type === 'checkbox-group'"
      v-model:value="localValue"
      @update:value="handleUpdate"
    >
      <n-space vertical>
        <n-checkbox
          v-for="option in props.item.options"
          :key="option.value"
          :value="option.value"
          :label="option.label"
        />
      </n-space>
    </n-checkbox-group>

    <!-- 预览区域 -->
    <div v-if="showPreviewState && props.item.preview" class="item-preview">
      <slot name="preview" :value="localValue">
        <div v-html="props.item.preview(localValue)"></div>
      </slot>
    </div>

    <!-- 自定义插槽 -->
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import {
  NInput, NInputNumber, NSelect, NSwitch,
  NRadioGroup, NRadio, NCheckboxGroup, NCheckbox, NSpace
} from 'naive-ui'
import type { ConfigItem } from './types'

interface Props {
  item: ConfigItem
  value: any
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (event: 'update:value', value: any): void
}>()

const localValue = ref(props.value)
const showPreviewState = ref(false)
const showPreview = props.item.livePreview !== false

// 字体族选项
const fontFamilyOptions = [
  { label: '系统默认', value: '-apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", sans-serif' },
  { label: '微软雅黑', value: '"Microsoft YaHei", "PingFang SC", sans-serif' },
  { label: '思源黑体', value: '"Source Han Sans CN", "Noto Sans CJK SC", sans-serif' },
  { label: '苹方', value: '"PingFang SC", "Hiragino Sans GB", sans-serif' },
  { label: '宋体', value: '"SimSun", "STSong", serif' },
  { label: '黑体', value: '"SimHei", "STHeiti", sans-serif' },
]

// 字号选项
const fontSizeOptions = [
  { label: '12px', value: 12 },
  { label: '14px', value: 14 },
  { label: '16px', value: 16 },
  { label: '18px', value: 18 },
  { label: '20px', value: 20 },
  { label: '24px', value: 24 },
  { label: '28px', value: 28 },
  { label: '32px', value: 32 },
]

watch(() => props.value, (newValue) => {
  localValue.value = newValue
}, { deep: true })

const handleUpdate = (value: any) => {
  localValue.value = value
  emit('update:value', value)
}

const togglePreview = () => {
  showPreviewState.value = !showPreviewState.value
}
</script>

<style scoped>
.config-item {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.item-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-sm);
}

.item-label {
  font-size: var(--text-sm);
  font-weight: 500;
  color: var(--color-text-main);
}

.preview-toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(255, 255, 255, 0.05);
  border-radius: var(--radius-md);
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all 0.2s;
}

.preview-toggle:hover {
  background: rgba(255, 255, 255, 0.1);
  color: var(--color-text-main);
}

.item-hint {
  font-size: var(--text-xs);
  color: var(--color-text-muted);
  margin: 0;
}

.item-warning {
  font-size: var(--text-xs);
  color: var(--color-warning);
  margin: 0;
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.item-preview {
  margin-top: var(--spacing-md);
  padding: var(--spacing-lg);
  background: rgba(0, 0, 0, 0.2);
  border-radius: var(--radius-md);
  border: 1px solid rgba(255, 255, 255, 0.08);
}

:deep(.n-input),
:deep(.n-input-number),
:deep(.n-select) {
  width: 100%;
}
</style>
