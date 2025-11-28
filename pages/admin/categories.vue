<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">分类管理</h1>
      <button @click="openModal()" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
        <i class="fas fa-plus mr-2"></i>新建分类
      </button>
    </div>

    <!-- 分类列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm overflow-hidden">
      <table class="w-full text-left">
        <thead class="bg-gray-50 dark:bg-gray-700/50 text-gray-500 dark:text-gray-400">
          <tr>
            <th class="px-6 py-4 font-medium">名称</th>
            <th class="px-6 py-4 font-medium">别名 (Slug)</th>
            <th class="px-6 py-4 font-medium">排序</th>
            <th class="px-6 py-4 font-medium">创建时间</th>
            <th class="px-6 py-4 font-medium text-right">操作</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr v-for="item in categories" :key="item.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200 font-medium">{{ item.name }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400">{{ item.slug || '-' }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400">{{ item.sort }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400 text-sm">
              {{ new Date(item.createdAt).toLocaleDateString() }}
            </td>
            <td class="px-6 py-4 text-right space-x-2">
              <button @click="openModal(item)" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
                编辑
              </button>
              <button @click="handleDelete(item)" class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300">
                删除
              </button>
            </td>
          </tr>
          <tr v-if="categories.length === 0">
            <td colspan="5" class="px-6 py-12 text-center text-gray-400">
              暂无分类数据
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 编辑/新建弹窗 -->
    <div v-if="showModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl w-full max-w-md p-6 space-y-4">
        <h3 class="text-xl font-bold text-gray-800 dark:text-white">
          {{ isEdit ? '编辑分类' : '新建分类' }}
        </h3>
        
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">名称</label>
            <input v-model="form.name" type="text" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-blue-500 outline-none transition-all" placeholder="例如：前端开发">
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">别名 (Slug)</label>
            <input v-model="form.slug" type="text" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-blue-500 outline-none transition-all" placeholder="例如：frontend">
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">排序 (越小越靠前)</label>
            <input v-model.number="form.sort" type="number" class="w-full px-4 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200 focus:ring-2 focus:ring-blue-500 outline-none transition-all">
          </div>
        </div>

        <div class="flex justify-end space-x-3 pt-4">
          <button @click="showModal = false" class="px-4 py-2 text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors">取消</button>
          <button @click="handleSave" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">保存</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Category } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const categories = ref<Category[]>([])
const showModal = ref(false)
const isEdit = ref(false)
const form = ref({
  id: 0,
  name: '',
  slug: '',
  sort: 0
})

const fetchCategories = async () => {
  try {
    // 后端返回格式: { code: 0, data: List<Category> }
    // useApi 已经处理了响应格式，直接返回 data (即 List<Category>)
    const res = await api.get<Category[]>('/Categories')
    categories.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch categories:', e)
    }
    categories.value = []
  }
}

const openModal = (item?: Category) => {
  if (item) {
    isEdit.value = true
    form.value = { ...item }
  } else {
    isEdit.value = false
    form.value = { id: 0, name: '', slug: '', sort: 0 }
  }
  showModal.value = true
}

const handleSave = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!form.value.name) {
    warning('请输入分类名称')
    return
  }
  
  try {
    if (isEdit.value) {
      await api.put(`/Categories/${form.value.id}`, form.value)
    } else {
      await api.post('/Categories', form.value)
    }
    success('保存成功')
    showModal.value = false
    fetchCategories()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const handleDelete = async (item: Category) => {
  if (!confirm(`确定要删除分类 "${item.name}" 吗？`)) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.del(`/Categories/${item.id}`)
    success('删除成功')
    fetchCategories()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

onMounted(() => {
  fetchCategories()
})
</script>

