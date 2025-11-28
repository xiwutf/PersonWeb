<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">成长轨迹管理</h1>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
        + 新建事件
      </button>
    </div>

    <!-- 列表 -->
    <div class="space-y-4">
      <div
        v-for="event in events"
        :key="event.id"
        class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6"
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
                <span class="px-2 py-1 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-300 rounded text-sm">
                  {{ event.year }}
                </span>
              </div>
              <p class="text-gray-600 dark:text-gray-400">{{ event.description }}</p>
            </div>
          </div>
          <div class="flex gap-2">
            <button @click="editEvent(event)" class="text-blue-600 hover:text-blue-800">编辑</button>
            <button @click="deleteEvent(event.id)" class="text-red-600 hover:text-red-800">删除</button>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingEvent" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-md m-4">
        <div class="p-6">
          <h2 class="text-xl font-bold mb-4">{{ editingEvent ? '编辑' : '新建' }}时间线事件</h2>
          
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium mb-1">年份</label>
              <input v-model.number="form.year" type="number" class="w-full border rounded px-3 py-2" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">标题</label>
              <input v-model="form.title" type="text" class="w-full border rounded px-3 py-2" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">描述</label>
              <textarea v-model="form.description" rows="3" class="w-full border rounded px-3 py-2"></textarea>
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">图标（emoji）</label>
              <input v-model="form.icon" type="text" class="w-full border rounded px-3 py-2" placeholder="例如: 🚀" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">颜色</label>
              <input v-model="form.color" type="color" class="w-full h-10 border rounded" />
            </div>
          </div>

          <div class="flex gap-2 mt-6">
            <button @click="saveEvent" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
              保存
            </button>
            <button @click="cancelEdit" class="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300">
              取消
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const events = ref<any[]>([])
const showCreateModal = ref(false)
const editingEvent = ref<any>(null)
const form = ref({
  year: new Date().getFullYear(),
  title: '',
  description: '',
  icon: '⭐',
  color: '#3b82f6'
})

const fetchEvents = async () => {
  try {
    const res = await api.get<any>('/Timeline')
    if (Array.isArray(res)) {
      events.value = res
    } else if (res && res.List) {
      events.value = res.List
    }
  } catch (e) {
    console.error('Failed to fetch timeline:', e)
  }
}

const editEvent = (event: any) => {
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
  try {
    if (editingEvent.value) {
      await api.put(`/Timeline/${editingEvent.value.id}`, form.value)
    } else {
      await api.post('/Timeline', form.value)
    }

    alert('保存成功')
    cancelEdit()
    fetchEvents()
  } catch (e: any) {
    alert(e.message || '保存失败')
  }
}

const deleteEvent = async (id: number) => {
  if (!confirm('确定要删除吗？')) return
  try {
    await api.delete(`/Timeline/${id}`)
    alert('删除成功')
    fetchEvents()
  } catch (e: any) {
    alert(e.message || '删除失败')
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

