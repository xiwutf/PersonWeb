<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">工具管理</h1>
      <button @click="openModal()" class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors">
        <i class="fas fa-plus mr-2"></i>新增工具
      </button>
    </div>

    <!-- 工具列表 -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tool in tools" :key="tool.path" class="bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 p-6 flex flex-col">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-lg bg-gray-100 dark:bg-gray-700 flex items-center justify-center text-2xl">
              {{ tool.icon || '🛠️' }}
            </div>
            <div>
              <h3 class="font-bold text-gray-800 dark:text-white">{{ tool.title }}</h3>
              <a :href="tool.url" target="_blank" class="text-xs text-blue-600 dark:text-blue-400 hover:underline">{{ tool.url }}</a>
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
      <div v-if="tools.length === 0" class="col-span-full text-center py-12 text-gray-400 bg-white dark:bg-gray-800 rounded-xl border border-dashed border-gray-300 dark:border-gray-700">
        暂无工具数据
      </div>
    </div>

    <!-- 编辑/新建弹窗 -->
    <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl w-full max-w-lg p-6 space-y-4 max-h-[90vh] overflow-y-auto">
        <h3 class="text-xl font-bold text-gray-800 dark:text-white">
          {{ isEdit ? '编辑工具' : '新增工具' }}
        </h3>
        
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">名称</label>
            <input v-model="form.title" type="text" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-purple-500 outline-none transition-all" placeholder="例如：JSON 格式化">
          </div>
          <div class="grid grid-cols-2 gap-4">
             <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">图标 (Emoji)</label>
              <input v-model="form.icon" type="text" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-purple-500 outline-none transition-all" placeholder="🔧">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">别名 (Slug)</label>
              <input v-model="form.slug" type="text" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-purple-500 outline-none transition-all" placeholder="json-fmt">
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">链接 URL</label>
            <input v-model="form.url" type="text" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-purple-500 outline-none transition-all" placeholder="https://...">
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">描述</label>
            <textarea v-model="form.description" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-purple-500 outline-none transition-all h-24" placeholder="简短描述该工具的作用..."></textarea>
          </div>
        </div>

        <div class="flex justify-end space-x-3 pt-4">
          <button @click="showModal = false" class="px-4 py-2 text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors">取消</button>
          <button @click="handleSave" class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors">保存</button>
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
