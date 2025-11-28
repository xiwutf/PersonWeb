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
    <n-card>
      <n-data-table
        :columns="columns"
        :data="categories"
        :loading="loading"
        :bordered="false"
      />
    </n-card>

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
import { NButton, NCard, NDataTable, NModal, NForm, NFormItem, NInput, NInputNumber, NPopconfirm, h, type FormInst, type FormRules } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { Category } from '~/types/api'
import { useMessage, useDialog } from 'naive-ui'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const message = useMessage()
const dialog = useDialog()
const { handleError } = useErrorHandler()

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

// 表格列定义
const columns: DataTableColumns<Category> = [
  {
    title: '名称',
    key: 'name',
    ellipsis: {
      tooltip: true
    }
  },
  {
    title: '别名 (Slug)',
    key: 'slug',
    render(row) {
      return row.slug || '-'
    }
  },
  {
    title: '排序',
    key: 'sort',
    width: 100
  },
  {
    title: '创建时间',
    key: 'createdAt',
    width: 180,
    render(row) {
      return new Date(row.createdAt).toLocaleDateString()
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    render(row) {
      return h('div', { class: 'action-buttons' }, [
        h(NButton, {
          size: 'small',
          type: 'primary',
          quaternary: true,
          onClick: () => openModal(row)
        }, {
          default: () => '编辑'
        }),
        h(NPopconfirm, {
          onPositiveClick: () => handleDelete(row)
        }, {
          trigger: () => h(NButton, {
            size: 'small',
            type: 'error',
            quaternary: true
          }, {
            default: () => '删除'
          }),
          default: () => `确定要删除分类 "${row.name}" 吗？`
        })
      ])
    }
  }
]

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

