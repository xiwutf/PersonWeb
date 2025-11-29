<template>
  <div class="categories-page">
    <div class="page-header">
      <h1 class="page-title">分类管理</h1>
      <n-button type="primary" @click="openModal()">
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        新建分类
      </n-button>
    </div>

    <!-- 分类列表 -->
    <div class="table-container">
      <div v-if="loading" class="table-loading">
        加载中...
      </div>
      <div v-else-if="categories.length === 0" class="table-empty">
        暂无分类
      </div>
      <table v-else class="data-table">
        <thead class="table-header">
          <tr>
            <th>名称</th>
            <th>别名 (Slug)</th>
            <th>排序</th>
            <th>创建时间</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="category in categories" :key="category.id" class="table-row">
            <td class="table-cell">{{ category.name }}</td>
            <td class="table-cell">{{ category.slug || '-' }}</td>
            <td class="table-cell">{{ category.sort }}</td>
            <td class="table-cell">
              {{ new Date(category.createdAt).toLocaleDateString() }}
            </td>
            <td class="table-cell">
              <div class="action-buttons">
                <button 
                  @click="openModal(category)" 
                  class="btn-link btn-link-blue"
                >
                  编辑
                </button>
                <button 
                  @click="handleDelete(category)" 
                  class="btn-link btn-link-red"
                >
                  删除
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 编辑/新建弹窗 -->
    <n-modal v-model:show="showModal" preset="card" :title="isEdit ? '编辑分类' : '新建分类'" style="width: 600px">
      <n-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-placement="left"
        label-width="120"
      >
        <n-form-item label="名称" path="name">
          <n-input v-model:value="form.name" placeholder="例如：前端开发" />
        </n-form-item>
        <n-form-item label="别名 (Slug)" path="slug">
          <n-input v-model:value="form.slug" placeholder="例如：frontend" />
        </n-form-item>
        <n-form-item label="排序" path="sort">
          <n-input-number v-model:value="form.sort" :min="0" placeholder="越小越靠前" style="width: 100%" />
        </n-form-item>
      </n-form>
      <template #footer>
        <div style="display: flex; justify-content: flex-end; gap: 12px">
          <n-button @click="showModal = false">取消</n-button>
          <n-button type="primary" @click="handleSave">保存</n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { NButton, NModal, NForm, NFormItem, NInput, NInputNumber, type FormInst, type FormRules } from 'naive-ui'
import type { Category } from '~/types/api'
import { useSafeMessage, useSafeDialog } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 Naive UI 组件在服务端渲染时出错
})

const api = useApi()
const { handleError } = useErrorHandler()

// 使用安全的 Naive UI composables，避免 provider 未挂载时的错误
const message = useSafeMessage()
const dialog = useSafeDialog()

const categories = ref<Category[]>([])
const loading = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInst | null>(null)
const form = ref({
  id: 0,
  name: '',
  slug: '',
  sort: 0
})

// 表单验证规则
const rules: FormRules = {
  name: {
    required: true,
    message: '请输入分类名称',
    trigger: 'blur'
  }
}

const fetchCategories = async () => {
  loading.value = true
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
    message.error('加载分类列表失败')
  } finally {
    loading.value = false
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
  if (!formRef.value) return
  
  await formRef.value.validate((errors) => {
    if (!errors) {
      saveCategory()
    }
  })
}

const saveCategory = async () => {
  try {
    if (isEdit.value) {
      await api.put(`/Categories/${form.value.id}`, form.value)
    } else {
      await api.post('/Categories', form.value)
    }
    message.success('保存成功')
    showModal.value = false
    fetchCategories()
  } catch (e: unknown) {
    handleError(e, '保存失败')
  }
}

const handleDelete = async (item: Category) => {
  if (!confirm(`确定要删除分类 "${item.name}" 吗？`)) return
  
  try {
    await api.del(`/Categories/${item.id}`)
    message.success('删除成功')
    fetchCategories()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

onMounted(() => {
  fetchCategories()
})
</script>

<style scoped>
/* 表格容器 */
.table-container {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: 0.5rem;
  overflow: hidden;
  margin-bottom: 1.5rem;
  box-shadow: var(--shadow-sm);
}

.table-loading,
.table-empty {
  padding: 2rem;
  text-align: center;
  color: var(--color-text-muted);
  font-weight: 500;
}

/* 数据表格 */
.data-table {
  width: 100%;
  text-align: left;
  border-collapse: collapse;
}

.table-header {
  background: var(--color-bg-elevated);
  border-bottom: 2px solid var(--color-border-subtle);
}

.table-header th {
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-text-main);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.table-body {
  border-collapse: collapse;
}

.table-row {
  border-bottom: 1px solid var(--color-border-subtle);
  transition: background-color 0.2s ease;
}

.table-row:hover {
  background: var(--color-bg-elevated);
}

.table-row:last-child {
  border-bottom: none;
}

.table-cell {
  padding: 1rem 1.5rem;
  color: var(--color-text-main);
  font-size: 0.875rem;
  font-weight: 500;
}

/* 操作按钮 */
.action-buttons {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  text-decoration: none;
  transition: color 0.2s ease;
  font-size: 0.875rem;
}

.btn-link-blue {
  color: #60a5fa;
}

.btn-link-blue:hover {
  color: #93c5fd;
}

.btn-link-red {
  color: #f87171;
}

.btn-link-red:hover {
  color: #fca5a5;
}
</style>

