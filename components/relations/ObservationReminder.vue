<template>
  <n-card class="observation-reminder-card" :class="cardClass">
    <div class="reminder-header">
      <div class="reminder-icon">
        <i :class="iconClass"></i>
      </div>
      <div class="reminder-content">
        <div class="reminder-title">{{ title }}</div>
        <div class="reminder-subtitle">{{ subtitle }}</div>
        <div v-if="reminder.reason" class="reminder-reason">
          <span class="reason-label">原因：</span>
          <span>{{ reminder.reason }}</span>
        </div>
      </div>
      <n-button
        size="small"
        quaternary
        @click="handleClose"
      >
        <template #icon>
          <i class="fas fa-times"></i>
        </template>
      </n-button>
    </div>
    
    <!-- 决策按钮（仅当需要决策时显示） -->
    <div v-if="reminder.type === 'DecisionRequired'" class="reminder-actions">
      <n-button
        type="primary"
        @click="handleDecision('Continue')"
      >
        继续（退出观察期）
      </n-button>
      <n-button
        @click="handleDecision('Downgrade')"
      >
        降级（延长观察）
      </n-button>
      <n-button
        type="error"
        @click="handleDecision('End')"
      >
        结束（标记已结束）
      </n-button>
    </div>
    
    <!-- 非决策类型的操作 -->
    <div v-else class="reminder-actions">
      <n-button
        size="small"
        quaternary
        @click="handleView"
      >
        查看详情
      </n-button>
      <n-button
        size="small"
        quaternary
        @click="handleClose"
      >
        稍后提醒
      </n-button>
    </div>
  </n-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { NCard, NButton, useDialog } from 'naive-ui'
import { useSafeDialog } from '~/composables/useNaiveUI'
import type { ObservationReminder as ObservationReminderType } from '~/composables/useRelationsApi'
import { useRelationsApi } from '~/composables/useRelationsApi'
import { useRouter } from 'vue-router'

interface Props {
  reminder: ObservationReminderType
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  decision: [decision: 'Continue' | 'Downgrade' | 'End']
}>()

const router = useRouter()
const dialog = useSafeDialog()
const relationsApi = useRelationsApi()

const title = computed(() => {
  switch (props.reminder.type) {
    case 'OnGoing':
      return `观察期进行中：${props.reminder.personNickname}`
    case 'EndingSoon':
      return `观察期即将结束：${props.reminder.personNickname}`
    case 'DecisionRequired':
      return `观察期已结束，需要决策：${props.reminder.personNickname}`
    default:
      return `观察期提醒：${props.reminder.personNickname}`
  }
})

const subtitle = computed(() => {
  switch (props.reminder.type) {
    case 'OnGoing':
      return `已观察 ${props.reminder.daysInObservation} 天，预计还需 ${props.reminder.daysUntilEnd} 天`
    case 'EndingSoon':
      return `已观察 ${props.reminder.daysInObservation} 天，剩余 ${props.reminder.daysUntilEnd} 天`
    case 'DecisionRequired':
      return `观察期已持续 ${props.reminder.daysInObservation} 天，请做出决策`
    default:
      return ''
  }
})

const iconClass = computed(() => {
  switch (props.reminder.type) {
    case 'OnGoing':
      return 'fas fa-eye'
    case 'EndingSoon':
      return 'fas fa-clock'
    case 'DecisionRequired':
      return 'fas fa-exclamation-triangle'
    default:
      return 'fas fa-info-circle'
  }
})

const cardClass = computed(() => {
  return {
    'reminder-ongoing': props.reminder.type === 'OnGoing',
    'reminder-ending': props.reminder.type === 'EndingSoon',
    'reminder-decision': props.reminder.type === 'DecisionRequired'
  }
})

const handleClose = async () => {
  // 标记提醒已查看
  try {
    await relationsApi.markReminderViewed(props.reminder.personId)
  } catch (error) {
    console.error('标记提醒已查看失败:', error)
  }
  emit('close')
}

const handleView = () => {
  router.push(`/admin/relations/${props.reminder.personId}`)
}

const handleDecision = async (decision: 'Continue' | 'Downgrade' | 'End') => {
  let confirmMessage = ''
  switch (decision) {
    case 'Continue':
      confirmMessage = '确定要退出观察期并继续跟进吗？'
      break
    case 'Downgrade':
      confirmMessage = '确定要延长观察期吗？'
      break
    case 'End':
      confirmMessage = '确定要标记为已结束吗？此操作不可撤销。'
      break
  }

  dialog.warning({
    title: '确认操作',
    content: confirmMessage,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      try {
        await relationsApi.handleObservationDecision(props.reminder.personId, {
          decision,
          reason: decision === 'End' ? '观察期结束后决定结束关系' : undefined
        })
        emit('decision', decision)
        emit('close')
      } catch (error: any) {
        console.error('处理决策失败:', error)
        // 错误提示由 useApi 处理
      }
    }
  })
}
</script>

<style scoped>
.observation-reminder-card {
  margin-bottom: 16px;
  border-left: 4px solid;
}

.reminder-ongoing {
  border-left-color: var(--color-primary);
  background: rgba(59, 130, 246, 0.05);
}

[data-theme="dark"] .reminder-ongoing {
  background: rgba(59, 130, 246, 0.1);
}

.reminder-ending {
  border-left-color: var(--color-warning);
  background: var(--color-warning-50);
}

:global(.dark) .reminder-ending,
:global([data-theme="dark"]) .reminder-ending {
  background: var(--color-warning-900, rgba(245, 158, 11, 0.1));
}

.reminder-decision {
  border-left-color: var(--color-danger);
  background: var(--color-danger-50);
}

:global(.dark) .reminder-decision,
:global([data-theme="dark"]) .reminder-decision {
  background: var(--color-danger-900, rgba(239, 68, 68, 0.1));
}

.reminder-header {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  margin-bottom: 12px;
}

.reminder-icon {
  flex-shrink: 0;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  font-size: 18px;
}

.reminder-ongoing .reminder-icon {
  background: rgba(59, 130, 246, 0.2);
  color: var(--color-primary);
}

.reminder-ending .reminder-icon {
  background: var(--color-warning-200, rgba(245, 158, 11, 0.2));
  color: var(--color-warning);
}

.reminder-decision .reminder-icon {
  background: var(--color-danger-200, rgba(239, 68, 68, 0.2));
  color: var(--color-danger);
}

.reminder-content {
  flex: 1;
  min-width: 0;
}

.reminder-title {
  font-size: 15px;
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: 4px;
}

.reminder-subtitle {
  font-size: 13px;
  color: var(--color-text-muted);
  margin-bottom: 8px;
}

.reminder-reason {
  font-size: 12px;
  color: var(--color-text-muted);
  padding: 6px 10px;
  background: var(--color-bg-elevated);
  border-radius: 4px;
  margin-top: 8px;
}

.reason-label {
  font-weight: 500;
  margin-right: 4px;
}

.reminder-actions {
  display: flex;
  gap: 8px;
  justify-content: flex-end;
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid var(--color-border-subtle);
}
</style>

