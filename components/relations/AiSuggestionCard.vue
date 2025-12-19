<template>
  <div v-if="show" class="ai-suggestion-card" :class="{ 'compact': compact }">
    <div class="suggestion-header">
      <div class="ai-icon">
        <i class="fas fa-robot"></i>
      </div>
      <div class="suggestion-title">AI 建议</div>
      <div class="suggestion-actions">
        <n-button
          size="tiny"
          quaternary
          @click="$emit('close')"
        >
          <i class="fas fa-times"></i>
        </n-button>
      </div>
    </div>

    <div v-if="loading" class="suggestion-loading">
      <n-spin size="small" />
      <span>AI 思考中...</span>
    </div>

    <div v-else-if="suggestion" class="suggestion-content">
      <!-- 状态提示语 -->
      <div v-if="suggestion.statusHint" class="status-hint">
        <i class="fas fa-info-circle"></i>
        <span>{{ suggestion.statusHint }}</span>
      </div>

      <!-- Next Action 建议 -->
      <div v-if="suggestion.nextActions && suggestion.nextActions.length > 0" class="next-actions">
        <div class="actions-title">建议的下一步行动（可参考）：</div>
        <div
          v-for="(action, index) in suggestion.nextActions"
          :key="index"
          class="action-item"
        >
          <div class="action-content">
            <div class="action-text">{{ action.title || action.Title }}</div>
            <div v-if="action.when || action.When" class="action-meta">
              <i class="fas fa-clock"></i>
              {{ action.when || action.When }}
            </div>
            <div v-if="action.why || action.Why" class="action-reason">
              {{ action.why || action.Why }}
            </div>
          </div>
          <div class="action-buttons">
            <n-button
              size="tiny"
              type="primary"
              @click="$emit('apply', action)"
            >
              采用
            </n-button>
          </div>
        </div>
      </div>

      <div v-else class="no-suggestion">
        <span>暂无具体建议</span>
      </div>
    </div>

    <div v-else-if="error" class="suggestion-error">
      <i class="fas fa-exclamation-triangle"></i>
      <span>{{ error }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { NCard, NButton, NSpin } from 'naive-ui'

interface Props {
  show: boolean
  loading?: boolean
  suggestion?: {
    statusHint?: string
    nextActions?: Array<{
      title?: string
      Title?: string
      when?: string
      When?: string
      why?: string
      Why?: string
    }>
  }
  error?: string
  compact?: boolean
}

defineProps<Props>()

defineEmits<{
  close: []
  apply: [action: any]
}>()
</script>

<style scoped>
.ai-suggestion-card {
  margin-top: 12px;
  padding: 16px;
  background: linear-gradient(135deg, #f0f4ff 0%, #e8f0fe 100%);
  border-radius: 8px;
  border: 1px solid #d0e7ff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

[data-theme="dark"] .ai-suggestion-card {
  background: linear-gradient(135deg, #1e293b 0%, #1a2332 100%);
  border-color: #334155;
}

.ai-suggestion-card.compact {
  padding: 12px;
}

.suggestion-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.ai-icon {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #3b82f6;
  border-radius: 50%;
  color: white;
  font-size: 14px;
}

.suggestion-title {
  flex: 1;
  font-size: 14px;
  font-weight: 600;
  color: var(--text-color);
}

.suggestion-actions {
  display: flex;
  gap: 4px;
}

.suggestion-loading {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 0;
  color: var(--text-color-2);
  font-size: 13px;
}

.suggestion-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.status-hint {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  padding: 10px 12px;
  background: rgba(59, 130, 246, 0.1);
  border-radius: 6px;
  border-left: 3px solid #3b82f6;
  font-size: 13px;
  color: var(--text-color);
  line-height: 1.6;
}

[data-theme="dark"] .status-hint {
  background: rgba(59, 130, 246, 0.15);
}

.status-hint i {
  color: #3b82f6;
  margin-top: 2px;
  flex-shrink: 0;
}

.next-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.actions-title {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-color);
  margin-bottom: 4px;
}

.action-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 12px;
  padding: 12px;
  background: white;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  transition: all 0.2s;
}

[data-theme="dark"] .action-item {
  background: #1f2937;
  border-color: #374151;
}

.action-item:hover {
  border-color: #3b82f6;
  box-shadow: 0 2px 8px rgba(59, 130, 246, 0.15);
}

.action-content {
  flex: 1;
  min-width: 0;
}

.action-text {
  font-size: 14px;
  font-weight: 500;
  color: var(--text-color);
  margin-bottom: 6px;
  line-height: 1.5;
}

.action-meta {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: var(--text-color-2);
  margin-bottom: 4px;
}

.action-meta i {
  font-size: 11px;
}

.action-reason {
  font-size: 12px;
  color: var(--text-color-3);
  line-height: 1.5;
  margin-top: 4px;
}

.action-buttons {
  flex-shrink: 0;
}

.no-suggestion {
  padding: 12px;
  text-align: center;
  color: var(--text-color-3);
  font-size: 13px;
}

.suggestion-error {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: #fef2f2;
  border-radius: 6px;
  border-left: 3px solid #ef4444;
  color: #dc2626;
  font-size: 13px;
}

[data-theme="dark"] .suggestion-error {
  background: #3f1f1f;
  border-left-color: #ef4444;
  color: #fca5a5;
}
</style>

