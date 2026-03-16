<template>
  <div class="detail-item" :class="props.field.class">
    <div class="detail-label">{{ props.field.label }}</div>
    <div class="detail-value">
      <!-- 自定义插槽 -->
      <slot></slot>

      <!-- 文本类型 -->
      <template v-if="!$slots.default && (props.field.type === 'text' || !props.field.type)">
        <span v-if="props.field.formatter">{{ formattedValue }}</span>
        <span v-else>{{ value ?? '-' }}</span>
      </template>

      <!-- 数字类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'number'">
        <span class="number-value">{{ props.field.formatter ? formattedValue : (value ?? '-') }}</span>
      </template>

      <!-- 日期类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'date'">
        <span v-if="value">{{ formatDate(value) }}</span>
        <span v-else>-</span>
      </template>

      <!-- 日期时间类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'datetime'">
        <span v-if="value">{{ formatDateTime(value) }}</span>
        <span v-else>-</span>
      </template>

      <!-- 时间类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'time'">
        <n-time v-if="value" :time="new Date(value)" />
        <span v-else>-</span>
      </template>

      <!-- 布尔类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'boolean'">
        <n-tag :type="value ? 'success' : 'default'" :bordered="false">
          {{ value ? '是' : '否' }}
        </n-tag>
      </template>

      <!-- 标签类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'tag'">
        <n-tag
          v-if="value"
          :type="props.field.tagConfig?.type || 'info'"
          :bordered="props.field.tagConfig?.bordered ?? false"
        >
          {{ props.field.formatter ? formattedValue : value }}
        </n-tag>
        <span v-else>-</span>
      </template>

      <!-- 图片类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'image'">
        <n-image
          v-if="value"
          :src="value"
          :width="120"
          :height="120"
          object-fit="cover"
          style="border-radius: var(--radius-md)"
        />
        <span v-else>-</span>
      </template>

      <!-- 链接类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'link'">
        <n-button
          v-if="value"
          text
          :type="primary"
          tag="a"
          :href="linkHref"
          :target="props.field.linkConfig?.target || '_blank'"
        >
          {{ props.field.formatter ? formattedValue : value }}
        </n-button>
        <span v-else>-</span>
      </template>

      <!-- HTML 类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'html'">
        <div v-if="value" class="html-content" v-html="value"></div>
        <span v-else>-</span>
      </template>

      <!-- JSON 类型 -->
      <template v-else-if="!$slots.default && props.field.type === 'json'">
        <code v-if="value" class="json-content">{{ JSON.stringify(value, null, 2) }}</code>
        <span v-else>-</span>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { NTag, NImage, NButton, NTime } from 'naive-ui'
import type { DetailItemConfig } from './DetailPage.vue'

interface Props {
  field: DetailItemConfig
  value: any
  data: Record<string, any>
}

const props = defineProps<Props>()

// 格式化后的值
const formattedValue = computed(() => {
  if (props.field.formatter) {
    return props.field.formatter(props.value, props.data)
  }
  return props.value
})

// 链接地址
const linkHref = computed(() => {
  if (props.field.linkConfig) {
    if (typeof props.field.linkConfig.to === 'function') {
      return props.field.linkConfig.to(props.value, props.data)
    }
    return props.field.linkConfig.to
  }
  return '#'
})

// 格式化日期
const formatDate = (date: string | Date) => {
  const d = new Date(date)
  return d.toLocaleDateString()
}

// 格式化日期时间
const formatDateTime = (date: string | Date) => {
  const d = new Date(date)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
.detail-item {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.detail-label {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  font-weight: 500;
}

.detail-value {
  font-size: var(--text-base);
  color: var(--color-text-main);
  word-break: break-word;
}

.number-value {
  font-family: 'SF Mono', Monaco, Consolas, 'Courier New', monospace;
  font-variant-numeric: tabular-nums;
}

.html-content {
  line-height: 1.6;
}

.html-content :deep(img) {
  max-width: 100%;
  height: auto;
}

.json-content {
  display: block;
  padding: var(--spacing-md);
  background: rgba(0, 0, 0, 0.3);
  border-radius: var(--radius-md);
  font-family: 'SF Mono', Monaco, Consolas, 'Courier New', monospace;
  font-size: var(--text-sm);
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 300px;
  overflow-y: auto;
}
</style>
