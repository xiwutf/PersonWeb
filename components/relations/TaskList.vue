<template>
  <div class="task-list">
    <div class="task-header">
      <h3>任务列表</h3>
      <n-button type="primary" size="small" @click="$emit('add')">
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        新增任务
      </n-button>
    </div>

    <n-empty v-if="tasks.length === 0" description="暂无任务" />

    <div v-else class="tasks">
      <div
        v-for="task in sortedTasks"
        :key="task.id"
        class="task-item"
        :class="{ 'task-done': task.status === 1 }"
      >
        <n-checkbox
          :checked="task.status === 1"
          @update:checked="handleToggleTask(task)"
        />

        <div class="task-content">
          <div class="task-title">{{ task.title }}</div>
          <div class="task-meta">
            <n-tag :type="getPriorityTag(task.priority)" size="small">
              {{ getPriorityText(task.priority) }}
            </n-tag>
            <span v-if="task.dueAt" class="due-date">
              <i class="fas fa-calendar"></i>
              {{ formatDate(task.dueAt) }}
            </span>
          </div>
        </div>

        <div class="task-actions">
          <n-button
            text
            size="small"
            @click="$emit('edit', task)"
          >
            编辑
          </n-button>
          <n-popconfirm @positive-click="$emit('delete', task.id)">
            <template #trigger>
              <n-button text size="small" type="error">删除</n-button>
            </template>
            确定删除这个任务吗？
          </n-popconfirm>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { NCheckbox, NTag, NButton, NEmpty, NPopconfirm } from 'naive-ui'
import type { RelationTask } from '~/composables/useRelationsApi'

interface Props {
  tasks: RelationTask[]
}

const props = defineProps<Props>()

const emit = defineEmits<{
  add: []
  edit: [task: RelationTask]
  delete: [id: string]
  toggle: [task: RelationTask]
}>()

const priorityTexts = ['低', '中', '高', '紧急']
const priorityTags = ['default', 'info', 'warning', 'error']

const getPriorityText = (priority: number) => {
  return priorityTexts[priority] || '中'
}

const getPriorityTag = (priority: number): 'default' | 'info' | 'warning' | 'error' => {
  return (priorityTags[priority] || 'info') as any
}

const formatDate = (dateStr: string) => {
  const date = new Date(dateStr)
  return date.toLocaleDateString('zh-CN', {
    month: '2-digit',
    day: '2-digit'
  })
}

const sortedTasks = computed(() => {
  return [...props.tasks].sort((a, b) => {
    // 未完成的在前，已完成的在后
    if (a.status !== b.status) {
      return a.status - b.status
    }
    // 优先级高的在前
    if (a.priority !== b.priority) {
      return b.priority - a.priority
    }
    // 截止时间早的在前
    if (a.dueAt && b.dueAt) {
      return new Date(a.dueAt).getTime() - new Date(b.dueAt).getTime()
    }
    if (a.dueAt) return -1
    if (b.dueAt) return 1
    return 0
  })
})

const handleToggleTask = (task: RelationTask) => {
  emit('toggle', task)
}
</script>

<style scoped>
.task-list {
  width: 100%;
}

.task-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.task-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.tasks {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.task-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 12px;
  background: var(--card-color);
  border-radius: 4px;
  border-left: 3px solid var(--color-primary);
}

.task-item.task-done {
  opacity: 0.6;
  border-left-color: var(--color-success);
}

.task-item.task-done .task-title {
  text-decoration: line-through;
}

.task-content {
  flex: 1;
}

.task-title {
  margin-bottom: 4px;
  font-size: 14px;
  line-height: 1.5;
}

.task-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: var(--text-color-2);
}

.due-date {
  display: flex;
  align-items: center;
  gap: 4px;
}

.task-actions {
  display: flex;
  gap: 4px;
}
</style>

