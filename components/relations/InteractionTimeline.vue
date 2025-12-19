<template>
  <div class="interaction-timeline">
    <div class="timeline-header">
      <h3>互动记录</h3>
      <n-button type="primary" size="small" @click="$emit('add')">
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        新增互动
      </n-button>
    </div>

    <n-empty v-if="interactions.length === 0" description="暂无互动记录" />

    <n-timeline v-else>
      <n-timeline-item
        v-for="interaction in interactions"
        :key="interaction.id"
        :time="formatTime(interaction.occurredAt)"
        type="default"
      >
        <div class="interaction-item">
          <div class="interaction-header">
            <n-tag :type="getTypeTag(interaction.type)" size="small">
              {{ getTypeText(interaction.type) }}
            </n-tag>
            <div class="actions">
              <n-button
                text
                size="small"
                @click="$emit('edit', interaction)"
              >
                编辑
              </n-button>
              <n-popconfirm @positive-click="$emit('delete', interaction.id)">
                <template #trigger>
                  <n-button text size="small" type="error">删除</n-button>
                </template>
                确定删除这条互动记录吗？
              </n-popconfirm>
            </div>
          </div>

          <div class="summary">{{ interaction.summary }}</div>

          <div v-if="interaction.keyPoints" class="key-points">
            <div v-for="(value, key) in interaction.keyPoints" :key="key" class="key-point">
              <strong>{{ key }}：</strong>{{ formatValue(value) }}
            </div>
          </div>

          <div v-if="interaction.myFeeling !== undefined" class="feeling">
            <i :class="getFeelingIcon(interaction.myFeeling)"></i>
            <span>{{ getFeelingText(interaction.myFeeling) }}</span>
          </div>

          <div v-if="interaction.aiSuggestion" class="ai-suggestion">
            <i class="fas fa-robot"></i>
            <div class="suggestion-text">{{ interaction.aiSuggestion }}</div>
          </div>
        </div>
      </n-timeline-item>
    </n-timeline>
  </div>
</template>

<script setup lang="ts">
import { NTimeline, NTimelineItem, NTag, NButton, NEmpty, NPopconfirm } from 'naive-ui'
import type { RelationInteraction } from '~/composables/useRelationsApi'

interface Props {
  interactions: RelationInteraction[]
}

defineProps<Props>()

defineEmits<{
  add: []
  edit: [interaction: RelationInteraction]
  delete: [id: string]
}>()

const typeTexts = ['文字', '语音', '通话', '见面', '其他']
const typeTags = ['default', 'info', 'warning', 'success', 'default']

const getTypeText = (type: number) => {
  return typeTexts[type] || '未知'
}

const getTypeTag = (type: number): 'default' | 'info' | 'warning' | 'success' | 'error' => {
  return (typeTags[type] || 'default') as any
}

const getFeelingIcon = (feeling: number) => {
  const icons = ['fas fa-smile', 'fas fa-meh', 'fas fa-frown']
  return icons[feeling] || 'fas fa-meh'
}

const getFeelingText = (feeling: number) => {
  const texts = ['正面', '中性', '负面']
  return texts[feeling] || '未知'
}

const formatTime = (timeStr: string) => {
  const time = new Date(timeStr)
  return time.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatValue = (value: any) => {
  if (Array.isArray(value)) {
    return value.join(', ')
  }
  return String(value)
}
</script>

<style scoped>
.interaction-timeline {
  width: 100%;
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.timeline-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.interaction-item {
  background: var(--card-color);
  padding: 12px;
  border-radius: 4px;
  margin-bottom: 8px;
}

.interaction-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.actions {
  display: flex;
  gap: 8px;
}

.summary {
  margin: 8px 0;
  line-height: 1.6;
  color: var(--text-color);
}

.key-points {
  margin: 8px 0;
  padding: 8px;
  background: var(--color-info-50);
  border-radius: 4px;
}

.key-point {
  margin: 4px 0;
  font-size: 14px;
}

.feeling {
  display: flex;
  align-items: center;
  gap: 4px;
  margin: 8px 0;
  color: var(--text-color-2);
}

.ai-suggestion {
  margin-top: 8px;
  padding: 8px;
  background: var(--color-success-50);
  border-radius: 4px;
  display: flex;
  gap: 8px;
}

.ai-suggestion i {
  color: var(--color-success);
  margin-top: 2px;
}

.suggestion-text {
  flex: 1;
  color: var(--color-success);
  font-size: 14px;
  line-height: 1.5;
}
</style>

