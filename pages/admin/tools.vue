<template>
  <div class="space-y-6">
    <div class="page-header">
      <h1 class="page-title">工具管理</h1>
      <button @click="openModal()" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>新增工具
      </button>
    </div>

    <!-- 工具列表 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tool in tools" :key="tool.path" class="card p-6 flex flex-col">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-gray-100 dark:bg-gray-700 flex items-center justify-center text-2xl">
              {{ tool.icon || '🛠️' }}
            </div>
            <div>
              <h3 class="font-bold text-gray-800 dark:text-white">{{ tool.title }}</h3>
              <a :href="tool.url" target="_blank" class="btn-link btn-link--blue text-xs">{{ tool.url }}</a>
            </div>
          </div>
          <div class="flex gap-2">
            <button @click="openModal(tool)" class="text-gray-400 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
              <span class="sr-only">编辑</span>
              ✏️
            </button>
            <button @click="handleDelete(tool)" class="text-gray-400 hover:text-red-600 dark:hover:text-red-400 transition-colors">
              <span class="sr-only">删除</span>
              🗑️
            </button>
          </div>
        </div>
        <p class="text-gray-600 dark:text-gray-400 text-sm mb-4 flex-1">{{ tool.description }}</p>
        <div class="text-xs text-gray-500 dark:text-gray-500">
          文件: {{ tool.path }}
        </div>
      </div>
      
      <!-- 空状态 -->
      <div v-if="tools.length === 0" class="col-span-full text-center py-12 empty-state card border-dashed">
        暂无工具数据
      </div>
    </div>

    <!-- 编辑/新建弹窗 -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-content max-w-lg max-h-[90vh] overflow-y-auto">
        <div class="modal-header">
          <h3 class="modal-title">
            {{ isEdit ? '编辑工具' : '新增工具' }}
          </h3>
        </div>
        
        <div class="modal-body space-y-4">
          <div class="form-group">
            <label class="form-label">名称</label>
            <input v-model="form.title" type="text" class="form-input" placeholder="例如：JSON 格式化">
          </div>
          <div class="grid grid-cols-2 gap-4">
             <div class="form-group">
              <label class="form-label">图标 (Emoji)</label>
              <input v-model="form.icon" type="text" class="form-input" placeholder="🔧">
            </div>
            <div class="form-group">
              <label class="form-label">别名 (Slug)</label>
              <input v-model="form.slug" type="text" class="form-input" placeholder="json-fmt">
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">链接 URL</label>
            <input v-model="form.url" type="text" class="form-input" placeholder="https://...">
          </div>
          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="form.description" class="form-textarea h-24" placeholder="简短描述该工具的作用..."></textarea>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="showModal = false" class="btn-secondary">取消</button>
          <button @click="handleSave" class="btn-primary">保存</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Tool {
  path: string
  title: string
  slug?: string
  icon?: string
  url: string
  description: string
}

import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const tools = ref<Tool[]>([])
const showModal = ref(false)
const isEdit = ref(false)
const form = ref({
  path: '',
  title: '',
  slug: '',
  icon: '',
  url: '',
  description: ''
})

const fetchTools = async () => {
  try {
    // Nuxt server API，使用 /api/ 前缀
    // 注意：在生产环境静态生成后，Nuxt Server API 可能不可用
    // 如果失败，可能需要使用后端 API 或确保 Nuxt Server API 在生产环境中可用
    const res = await api.get<Tool[]>('/api/admin/tools')
    tools.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch tools:', e)
    }
    tools.value = []
  }
}

const openModal = (item?: Tool) => {
  if (item) {
    isEdit.value = true
    form.value = { ...item }
  } else {
    isEdit.value = false
    form.value = { path: '', title: '', slug: '', icon: '', url: '', description: '' }
  }
  showModal.value = true
}

const handleSave = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!form.value.title) {
    warning('请输入工具名称')
    return
  }
  
  try {
    // Nuxt server API，使用 /api/ 前缀
    if (isEdit.value) {
      await api.put('/api/admin/tools', form.value)
    } else {
      await api.post('/api/admin/tools', form.value)
    }
    success('保存成功')
    showModal.value = false
    fetchTools()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const handleDelete = async (item: Tool) => {
  if (!confirm(`确定要删除工具 "${item.title}" 吗？`)) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    // Nuxt server API，使用 /api/ 前缀
    await api.del(`/api/admin/tools?filename=${item.path}`)
    success('删除成功')
    fetchTools()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

onMounted(() => {
  fetchTools()
})
</script>
