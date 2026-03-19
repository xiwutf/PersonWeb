<template>
  <n-card 
    class="person-card" 
    :class="{ 'person-card-selected': props.selected && props.selectable }"
    hoverable
    @click="handleCardClick"
  >
    <!-- 第1行：昵称 + 阶段 Badge + 热度 -->
    <div class="row-1-header">
      <div class="nickname">{{ person.nickname }}</div>
      <div class="header-badges">
        <n-tag :type="stageType" size="small">{{ stageText }}</n-tag>
        <span v-if="person.heatScore > 0" class="heat-badge">
          <i class="fas fa-fire"></i>
          {{ person.heatScore }}
        </span>
      </div>
    </div>

    <!-- 第2行：最后联系时间 + 状态提示语 -->
    <div class="row-2-contact">
      <div class="contact-info">
        <i class="fas fa-clock"></i>
        <span>{{ formatTime(person.lastContactAt) }}</span>
      </div>
      <div class="status-hint">
        {{ getStatusHint() }}
      </div>
    </div>

    <!-- 第3行：Next Action 行动条（高亮） -->
    <div class="row-3-action" :class="{ 'has-action': person.nextAction, 'no-action': !person.nextAction }">
      <div v-if="person.nextAction" class="next-action-bar">
        <div class="action-icon">
          <i class="fas fa-lightbulb"></i>
        </div>
        <div class="action-content">
          <div class="action-text">{{ person.nextAction }}</div>
        </div>
        <div class="action-buttons">
          <n-button
            size="tiny"
            quaternary
            @click.stop="handleEditAction"
            title="编辑"
          >
            <template #icon>
              <i class="fas fa-edit"></i>
            </template>
          </n-button>
          <n-button
            size="tiny"
            quaternary
            type="success"
            @click.stop="handleCompleteAction"
            title="标记完成"
          >
            <template #icon>
              <i class="fas fa-check"></i>
            </template>
          </n-button>
          <n-button
            size="tiny"
            quaternary
            type="info"
            @click.stop="handleAiSuggestion"
            title="AI 生成建议"
          >
            <template #icon>
              <i class="fas fa-magic"></i>
            </template>
          </n-button>
        </div>
      </div>
      <div v-else class="next-action-empty">
        <span class="empty-text">暂无下一步行动</span>
        <n-button
          size="tiny"
          type="primary"
          @click.stop="handleAddAction"
        >
          设置行动
        </n-button>
      </div>
    </div>

    <!-- 选择框（看板视图时显示） -->
    <div v-if="props.selectable" class="card-selection" @click.stop>
      <n-checkbox
        :checked="props.selected"
        @update:checked="handleSelectChange"
      />
    </div>

    <!-- 选择框（看板视图时显示） -->
    <div v-if="props.selectable" class="card-selection" @click.stop>
      <n-checkbox
        :checked="props.selected"
        @update:checked="handleSelectChange"
      />
    </div>

    <!-- 第4行：快捷操作区 -->
    <div class="row-4-actions">
      <n-button
        size="small"
        type="primary"
        @click.stop="$emit('add-interaction', person.id)"
      >
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        记录互动
      </n-button>
      <n-button
        size="small"
        quaternary
        type="info"
        @click.stop="handleAiSuggestion"
        :title="getAiSuggestionTitle()"
      >
        <template #icon>
          <i class="fas fa-robot"></i>
        </template>
        AI 建议
      </n-button>
      <n-dropdown :options="moreOptions" @select="handleMoreAction">
        <n-button size="small" @click.stop>
          更多
          <template #icon>
            <i class="fas fa-chevron-down"></i>
          </template>
        </n-button>
      </n-dropdown>
    </div>

    <!-- AI 建议卡片（轻量级） -->
    <AiSuggestionCard
      v-if="showAiSuggestion"
      :show="showAiSuggestion"
      :loading="aiLoading"
      :suggestion="aiSuggestion"
      :error="aiError"
      compact
      @close="showAiSuggestion = false"
      @apply="handleApplySuggestion"
    />

    <!-- 长时间未联系提示 -->
    <div v-if="shouldShowLongTimeWarning" class="long-time-warning">
      <i class="fas fa-exclamation-circle"></i>
      <span>{{ getLongTimeWarningText() }}</span>
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { computed, h, ref } from 'vue'
import { NCard, NTag, NButton, NDropdown, NCheckbox } from 'naive-ui'
import type { RelationPerson } from '~/composables/useRelationsApi'
import { useSafeMessage } from '~/composables/useNaiveUI'
import AiSuggestionCard from './AiSuggestionCard.vue'

interface Props {
  person: RelationPerson
  selected?: boolean
  selectable?: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  view: [id: string]
  addInteraction: [id: string]
  editAction: [person: RelationPerson]
  completeAction: [id: string]
  aiSuggestion: [id: string]
  setReminder: [id: string]
  generateSuggestion: [id: string]
  delete: [id: string]
  applyAiSuggestion: [id: string, action: any]
  select: [id: string, selected: boolean]
}>()

const message = useSafeMessage()

// AI 建议相关
const showAiSuggestion = ref(false)
const aiLoading = ref(false)
const aiSuggestion = ref<any>(null)
const aiError = ref<string | null>(null)

// 阶段枚举
const stages = [
  '新认识',
  '熟悉中',
  '准备约见',
  '已见面',
  '升温中',
  '观察期',
  '已结束'
]

const stageText = computed(() => {
  return stages[props.person.stage] || '未知'
})

const stageType = computed(() => {
  const types: Record<number, 'default' | 'success' | 'warning' | 'error' | 'info'> = {
    0: 'default',
    1: 'info',
    2: 'warning',
    3: 'success',
    4: 'success',
    5: 'warning',
    6: 'error'
  }
  return types[props.person.stage] || 'default'
})

const formatTime = (timeStr: string | undefined) => {
  if (!timeStr) return '从未联系'
  const time = new Date(timeStr)
  const now = new Date()
  const diff = now.getTime() - time.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) return '今天'
  if (days === 1) return '昨天'
  if (days < 7) return `${days}天前`
  if (days < 30) return `${Math.floor(days / 7)}周前`
  return time.toLocaleDateString('zh-CN')
}

const getStatusHint = () => {
  if (!props.person.lastContactAt) return '未开始接触'
  
  const time = new Date(props.person.lastContactAt)
  const now = new Date()
  const diff = now.getTime() - time.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) return '今天联系过'
  if (days === 1) return '昨天联系'
  if (days < 7) return '本周联系'
  if (days < 30) return '本月联系'
  if (days < 90) return '3个月内联系'
  return '较久未联系'
}

const handleCardClick = (e: MouseEvent) => {
  // 如果点击的是按钮、链接、下拉菜单或输入框，不触发卡片点击
  const target = e.target as HTMLElement
  if (
    target.closest('button') || 
    target.closest('a') || 
    target.closest('.n-dropdown') ||
    target.closest('.action-buttons') ||
    target.closest('.row-4-actions') ||
    target.tagName === 'BUTTON' ||
    target.tagName === 'INPUT' ||
    target.tagName === 'TEXTAREA'
  ) {
    return
  }
  emit('view', props.person.id)
}

const handleEditAction = () => {
  emit('editAction', props.person)
}

const handleCompleteAction = async () => {
  emit('completeAction', props.person.id)
}

const handleAddAction = () => {
  emit('editAction', props.person)
}

const handleAiSuggestion = () => {
  emit('aiSuggestion', props.person.id)
}

const handleSelectChange = (checked: boolean) => {
  emit('select', props.person.id, checked)
}

const moreOptions = [
  {
    label: '设提醒',
    key: 'reminder',
    icon: () => h('i', { class: 'fas fa-bell' })
  },
  {
    label: '生成建议',
    key: 'suggestion',
    icon: () => h('i', { class: 'fas fa-magic' })
  },
  {
    label: '查看详情',
    key: 'view',
    icon: () => h('i', { class: 'fas fa-info-circle' })
  }
]

moreOptions.push({
  label: '删除对象',
  key: 'delete',
  icon: () => h('i', { class: 'fas fa-trash' })
})

const handleMoreAction = (key: string) => {
  if (key === 'reminder') {
    emit('setReminder', props.person.id)
  } else if (key === 'suggestion') {
    emit('generateSuggestion', props.person.id)
  } else if (key === 'view') {
    emit('view', props.person.id)
  } else if (key === 'delete') {
    emit('delete', props.person.id)
  }
}

// 计算是否应该显示长时间未联系警告
const shouldShowLongTimeWarning = computed(() => {
  if (!props.person.lastContactAt) return false
  const days = getDaysSinceLastContact()
  return days >= 7 // 7天未联系显示警告
})

const getDaysSinceLastContact = (): number => {
  if (!props.person.lastContactAt) return Infinity
  const time = new Date(props.person.lastContactAt)
  const now = new Date()
  const diff = now.getTime() - time.getTime()
  return Math.floor(diff / (1000 * 60 * 60 * 24))
}

const getLongTimeWarningText = (): string => {
  const days = getDaysSinceLastContact()
  if (days >= 30) return `已 ${days} 天未联系`
  if (days >= 14) return `已 ${days} 天未联系`
  return `已 ${days} 天未联系`
}

const getAiSuggestionTitle = (): string => {
  if (!props.person.lastContactAt) {
    return '获取 AI 建议（基于对象信息）'
  }
  const days = getDaysSinceLastContact()
  if (days >= 7) {
    return '获取 AI 建议（长时间未联系）'
  }
  return '获取 AI 建议（基于最近互动）'
}

const handleApplySuggestion = (action: any) => {
  emit('applyAiSuggestion', props.person.id, action)
  showAiSuggestion.value = false
}

// 暴露方法供父组件调用
defineExpose({
  showSuggestion: (suggestion: any) => {
    aiSuggestion.value = suggestion
    aiLoading.value = false
    showAiSuggestion.value = true
  },
  setAiLoading: (loading: boolean) => {
    aiLoading.value = loading
  },
  setAiError: (error: string) => {
    aiError.value = error
    aiLoading.value = false
  },
  hideSuggestion: () => {
    showAiSuggestion.value = false
  }
})
</script>

<style scoped>
.person-card {
  margin-bottom: 16px;
  cursor: pointer;
  transition: all 0.2s;
}

.person-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px var(--shadow);
}

/* 第1行：昵称 + 阶段 Badge + 热度 */
.row-1-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.nickname {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-color);
}

.header-badges {
  display: flex;
  align-items: center;
  gap: 8px;
}

.heat-badge {
  display: flex;
  align-items: center;
  gap: 4px;
  color: var(--color-danger, #ff6b6b);
  font-size: 12px;
  font-weight: 500;
}

/* 第2行：最后联系时间 + 状态提示语 */
.row-2-contact {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  font-size: 13px;
  color: var(--text-color-2);
}

.contact-info {
  display: flex;
  align-items: center;
  gap: 6px;
}

.contact-info i {
  color: var(--text-color-3);
  font-size: 12px;
}

.status-hint {
  color: var(--text-color-3);
  font-size: 12px;
}

/* 第3行：Next Action 行动条（高亮） */
.row-3-action {
  margin-bottom: 12px;
  min-height: 48px;
}

.next-action-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px;
  background: linear-gradient(135deg, var(--color-primary, #667eea) 0%, var(--color-primary-hover, #764ba2) 100%);
  border-radius: var(--radius-md, 8px);
  box-shadow: var(--shadow-md, 0 2px 8px rgba(102, 126, 234, 0.3));
}

:global(.dark) .next-action-bar,
:global([data-theme="dark"]) .next-action-bar {
  background: linear-gradient(135deg, var(--color-primary-dark, #4c5fd5) 0%, var(--color-primary-darker, #5a3f8c) 100%);
}

.action-icon {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  color: white;
}

.action-icon i {
  font-size: 14px;
}

.action-content {
  flex: 1;
  min-width: 0;
}

.action-text {
  color: white;
  font-size: 14px;
  font-weight: 500;
  line-height: 1.5;
  word-wrap: break-word;
  overflow-wrap: break-word;
}

.action-buttons {
  display: flex;
  gap: 4px;
  flex-shrink: 0;
}

.action-buttons :deep(.n-button) {
  color: rgba(255, 255, 255, 0.9);
  border-color: rgba(255, 255, 255, 0.3);
}

.action-buttons :deep(.n-button:hover) {
  color: white;
  background: rgba(255, 255, 255, 0.2);
  border-color: rgba(255, 255, 255, 0.5);
}

.next-action-empty {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background: var(--color-info-50);
  border-radius: 8px;
  border: 1px dashed var(--color-info);
}

.empty-text {
  color: var(--text-color-2);
  font-size: 13px;
}

/* 第4行：快捷操作区 */
.row-4-actions {
  display: flex;
  gap: 8px;
  justify-content: flex-end;
}

.row-4-actions :deep(.n-button) {
  margin: 0;
}

.long-time-warning {
  margin-top: 12px;
  padding: 10px 12px;
  background: var(--color-warning-50);
  border-radius: var(--radius-sm, 6px);
  border-left: 3px solid var(--color-warning);
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: var(--color-warning-text);
}

:global(.dark) .long-time-warning,
:global([data-theme="dark"]) .long-time-warning {
  background: var(--color-warning-900, #3f2419);
  border-left-color: var(--color-warning);
  color: var(--color-warning-200);
}

.long-time-warning i {
  color: var(--color-warning);
  font-size: 14px;
}

.person-card-selected {
  border: 2px solid var(--color-primary);
  box-shadow: 0 0 0 2px rgba(18, 150, 219, 0.2);
}

.card-selection {
  position: absolute;
  top: 12px;
  right: 12px;
  z-index: 10;
  background: rgba(255, 255, 255, 0.9);
  border-radius: 4px;
  padding: 4px;
}

[data-theme="dark"] .card-selection {
  background: rgba(31, 41, 55, 0.9);
}
</style>
