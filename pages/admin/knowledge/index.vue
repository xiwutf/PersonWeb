<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">知识库管理</h1>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
        + 新建条目
      </button>
    </div>

    <!-- 列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <table class="w-full text-left">
        <thead class="bg-gray-50 dark:bg-gray-700/50 border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">标题</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">分类</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">标签</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">查看数</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">更新时间</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">操作</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="item in list" :key="item.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/30">
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">{{ item.title }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400">{{ item.category || '-' }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400 text-sm">
              <span v-for="tag in parseTags(item.tags)" :key="tag" class="mr-1 px-2 py-0.5 bg-gray-100 dark:bg-gray-700 rounded text-xs">
                {{ tag }}
              </span>
            </td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400">{{ item.viewCount }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400 text-sm">{{ formatDate(item.updatedAt) }}</td>
            <td class="px-6 py-4">
              <div class="flex gap-2">
                <button @click="editItem(item)" class="text-blue-600 hover:text-blue-800">编辑</button>
                <button @click="deleteItem(item.id)" class="text-red-600 hover:text-red-800">删除</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingItem" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-2xl m-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h2 class="text-xl font-bold mb-4">{{ editingItem ? '编辑' : '新建' }}知识库条目</h2>
          
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium mb-1">标题</label>
              <input v-model="form.title" type="text" class="w-full border rounded px-3 py-2" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">分类</label>
              <select v-model="form.category" class="w-full border rounded px-3 py-2">
                <option value="">选择分类</option>
                <option value="开发笔记">开发笔记</option>
                <option value="踩坑记录">踩坑记录</option>
                <option value="想法灵感">想法灵感</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">标签（逗号分隔）</label>
              <input v-model="form.tags" type="text" class="w-full border rounded px-3 py-2" placeholder="例如: Vue, Nuxt, 前端" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">内容（Markdown）</label>
              <textarea v-model="form.content" rows="10" class="w-full border rounded px-3 py-2"></textarea>
            </div>
          </div>

          <div class="flex gap-2 mt-6">
            <button @click="saveItem" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
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
import type { KnowledgeBase, KnowledgeBaseRequest } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const list = ref<KnowledgeBase[]>([])
const showCreateModal = ref(false)
const editingItem = ref<KnowledgeBase | null>(null)
const form = ref({
  title: '',
  content: '',
  category: '',
  tags: ''
})

const fetchList = async () => {
  try {
    const res = await api.get<{ Total: number; List: KnowledgeBase[] }>('/KnowledgeBase', { params: { page: 1, pageSize: 100 } })
    if (res && res.List) {
      list.value = res.List
    }
  } catch (e) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch knowledge base:', e)
    }
  }
}

const editItem = (item: KnowledgeBase) => {
  editingItem.value = item
  form.value = {
    title: item.title,
    content: item.content || '',
    category: item.category || '',
    tags: Array.isArray(item.tags) ? item.tags.join(',') : (item.tags || '')
  }
}

const saveItem = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    if (!form.value.title || !form.value.title.trim()) {
      warning('请输入标题')
      return
    }

    const tagsArray = form.value.tags ? form.value.tags.split(',').map((t: string) => t.trim()).filter((t: string) => t) : []
    const payload: KnowledgeBaseRequest = {
      title: form.value.title.trim(),
      content: form.value.content || undefined,
      category: form.value.category || undefined,
      tags: tagsArray.length > 0 ? JSON.stringify(tagsArray) : undefined
    }

    if (editingItem.value) {
      await api.put(`/KnowledgeBase/${editingItem.value.id}`, payload)
    } else {
      await api.post('/KnowledgeBase', payload)
    }

    success('保存成功')
    cancelEdit()
    fetchList()
  } catch (e: unknown) {
    handleError(e, '保存失败，请检查后端日志')
  }
}

const deleteItem = async (id: number) => {
  if (!confirm('确定要删除吗？')) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.delete(`/KnowledgeBase/${id}`)
    success('删除成功')
    fetchList()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingItem.value = null
  form.value = { title: '', content: '', category: '', tags: '' }
}

const parseTags = (tags: string) => {
  if (!tags) return []
  try {
    return JSON.parse(tags)
  } catch {
    return tags.split(',').map(t => t.trim()).filter(t => t)
  }
}

const formatDate = (dateStr: string) => {
  return new Date(dateStr).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchList()
})
</script>

