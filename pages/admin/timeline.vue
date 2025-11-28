<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">成长轨迹管理</h1>
      <button @click="showCreateModal = true" class="btn-primary">
        + 新建事件
      </button>
    </div>

    <!-- 列表 -->
    <div class="space-y-4">
      <div
        v-for="event in events"
        :key="event.id"
        class="card p-6"
      >
        <div class="flex items-start justify-between">
          <div class="flex items-start gap-4">
            <div
              class="w-12 h-12 rounded-full flex items-center justify-center text-xl"
              :style="{ backgroundColor: event.color || '#3b82f6' }"
            >
              {{ event.icon || '⭐' }}
            </div>
            <div>
              <div class="flex items-center gap-2 mb-1">
                <span class="text-lg font-bold text-gray-800 dark:text-white">{{ event.title }}</span>
                <span class="badge badge-blue">
                  {{ event.year }}
                </span>
              </div>
              <p class="text-gray-600 dark:text-gray-400">{{ event.description }}</p>
            </div>
          </div>
          <div class="flex gap-2">
            <button @click="editEvent(event)" class="btn-link btn-link--blue">编辑</button>
            <button @click="deleteEvent(event.id)" class="btn-link btn-link--red">删除</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingEvent" class="modal-overlay">
      <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title">{{ editingEvent ? '编辑' : '新建' }}时间线事件</h2>
        </div>
        
        <div class="modal-body space-y-4">
          <div class="form-group">
            <label class="form-label">年份</label>
            <input v-model.number="form.year" type="number" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">标题</label>
            <input v-model="form.title" type="text" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="form.description" rows="3" class="form-textarea"></textarea>
          </div>
          <div class="form-group">
            <label class="form-label">图标（emoji）</label>
            <input v-model="form.icon" type="text" class="form-input" placeholder="例如: 🚀" />
          </div>
          <div class="form-group">
            <label class="form-label">颜色</label>
            <input v-model="form.color" type="color" class="w-full h-10 border rounded" />
          </div>
        </div>

        <div class="modal-footer">
          <button @click="saveEvent" class="btn-primary">保存</button>
          <button @click="cancelEdit" class="btn-secondary">取消</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { TimelineEvent, TimelineEventRequest } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const events = ref<TimelineEvent[]>([])
const showCreateModal = ref(false)
const editingEvent = ref<TimelineEvent | null>(null)
const form = ref({
  year: new Date().getFullYear(),
  title: '',
  description: '',
  icon: '⭐',
  color: '#3b82f6'
})

const fetchEvents = async () => {
  try {
    const res = await api.get<TimelineEvent[] | { List: TimelineEvent[] }>('/Timeline')
    if (Array.isArray(res)) {
      events.value = res
    } else if (res && 'List' in res) {
      events.value = res.List
    }
  } catch (e) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch timeline:', e)
    }
  }
}

const editEvent = (event: TimelineEvent) => {
  editingEvent.value = event
  form.value = {
    year: event.year,
    title: event.title,
    description: event.description || '',
    icon: event.icon || '⭐',
    color: event.color || '#3b82f6'
  }
}

const saveEvent = async () => {
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    const payload: TimelineEventRequest = {
      year: form.value.year,
      title: form.value.title,
      description: form.value.description || undefined,
      icon: form.value.icon || undefined,
      color: form.value.color || undefined
    }
    
    if (editingEvent.value) {
      await api.put(`/Timeline/${editingEvent.value.id}`, payload)
    } else {
      await api.post('/Timeline', payload)
    }

    success('保存成功')
    cancelEdit()
    fetchEvents()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const deleteEvent = async (id: number) => {
  if (!confirm('确定要删除吗？')) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.delete(`/Timeline/${id}`)
    success('删除成功')
    fetchEvents()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingEvent.value = null
  form.value = {
    year: new Date().getFullYear(),
    title: '',
    description: '',
    icon: '⭐',
    color: '#3b82f6'
  }
}

onMounted(() => {
  fetchEvents()
})
</script>

