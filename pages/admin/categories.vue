<template>
  <div class="space-y-6">
    <div class="page-header">
      <h1 class="page-title">分类管理</h1>
      <button @click="openModal()" class="btn-primary">
        <i class="fas fa-plus mr-2"></i>新建分类
      </button>
    </div>

    <!-- 分类列表 -->
    <div class="table-container">
      <table class="table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">名称</th>
            <th class="table-header-cell">别名 (Slug)</th>
            <th class="table-header-cell">排序</th>
            <th class="table-header-cell">创建时间</th>
            <th class="table-header-cell text-right">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="item in categories" :key="item.id" class="table-row">
            <td class="table-cell font-medium">{{ item.name }}</td>
            <td class="table-cell">{{ item.slug || '-' }}</td>
            <td class="table-cell">{{ item.sort }}</td>
            <td class="table-cell text-sm">
              {{ new Date(item.createdAt).toLocaleDateString() }}
            </td>
            <td class="table-cell text-right space-x-2">
              <button @click="openModal(item)" class="btn-link btn-link--blue">
                编辑
              </button>
              <button @click="handleDelete(item)" class="btn-link btn-link--red">
                删除
              </button>
            </td>
          </tr>
          <tr v-if="categories.length === 0">
            <td colspan="5" class="table-cell text-center empty-state">
              暂无分类数据
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 编辑/新建弹窗 -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal-content">
        <div class="modal-header">
          <h3 class="modal-title">
            {{ isEdit ? '编辑分类' : '新建分类' }}
          </h3>
        </div>
        
        <div class="modal-body space-y-4">
          <div class="form-group">
            <label class="form-label">名称</label>
            <input v-model="form.name" type="text" class="form-input" placeholder="例如：前端开发">
          </div>
          <div class="form-group">
            <label class="form-label">别名 (Slug)</label>
            <input v-model="form.slug" type="text" class="form-input" placeholder="例如：frontend">
          </div>
          <div class="form-group">
            <label class="form-label">排序 (越小越靠前)</label>
            <input v-model.number="form.sort" type="number" class="form-input">
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

