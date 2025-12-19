<template>
  <n-card class="person-card" hoverable>
    <div class="card-header">
      <div class="nickname">{{ person.nickname }}</div>
      <n-tag :type="stageType" size="small">{{ stageText }}</n-tag>
    </div>

    <div class="card-body">
      <div class="info-row" v-if="person.source">
        <i class="fas fa-map-marker-alt"></i>
        <span>{{ person.source }}</span>
      </div>
      <div class="info-row" v-if="person.city">
        <i class="fas fa-city"></i>
        <span>{{ person.city }}</span>
      </div>
      
      <div class="tags" v-if="person.tags && person.tags.length > 0">
        <n-tag
          v-for="tag in person.tags"
          :key="tag"
          size="small"
          type="info"
          style="margin-right: 4px; margin-bottom: 4px;"
        >
          {{ tag }}
        </n-tag>
      </div>

      <div class="info-row" v-if="person.lastContactAt">
        <i class="fas fa-clock"></i>
        <span>最后联系：{{ formatTime(person.lastContactAt) }}</span>
      </div>

      <div class="heat-score" v-if="person.heatScore > 0">
        <i class="fas fa-fire"></i>
        <span>热度：{{ person.heatScore }}</span>
      </div>

      <div class="next-action" v-if="person.nextAction">
        <i class="fas fa-lightbulb"></i>
        <span class="action-text">{{ person.nextAction }}</span>
      </div>
    </div>

    <template #action>
      <div class="card-actions">
        <n-button size="small" @click="$emit('view', person.id)">查看详情</n-button>
        <n-button size="small" @click="$emit('add-interaction', person.id)">记录互动</n-button>
        <n-dropdown :options="moreOptions" @select="handleMoreAction">
          <n-button size="small">更多</n-button>
        </n-dropdown>
      </div>
    </template>
  </n-card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { NCard, NTag, NButton, NDropdown, useMessage } from 'naive-ui'
import type { RelationPerson } from '~/composables/useRelationsApi'

interface Props {
  person: RelationPerson
}

const props = defineProps<Props>()
const emit = defineEmits<{
  view: [id: string]
  addInteraction: [id: string]
  setReminder: [id: string]
  generateSuggestion: [id: string]
}>()

const message = useMessage()

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

const formatTime = (timeStr: string) => {
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
  }
]

const handleMoreAction = (key: string) => {
  if (key === 'reminder') {
    emit('setReminder', props.person.id)
  } else if (key === 'suggestion') {
    emit('generateSuggestion', props.person.id)
  }
}
</script>

<style scoped>
.person-card {
  margin-bottom: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.nickname {
  font-size: 18px;
  font-weight: 600;
}

.card-body {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.info-row {
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--text-color-2);
  font-size: 14px;
}

.info-row i {
  color: var(--text-color-3);
}

.tags {
  margin: 4px 0;
}

.heat-score {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #ff6b6b;
  font-size: 14px;
}

.next-action {
  margin-top: 8px;
  padding: 8px;
  background: var(--color-info-50);
  border-radius: 4px;
  display: flex;
  align-items: flex-start;
  gap: 8px;
}

.next-action i {
  color: var(--color-info);
  margin-top: 2px;
}

.action-text {
  flex: 1;
  color: var(--color-info);
  font-size: 14px;
  line-height: 1.5;
}

.card-actions {
  display: flex;
  gap: 8px;
  justify-content: flex-end;
}
</style>

