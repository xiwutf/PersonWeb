<template>
  <div class="categories-page space-y-6">
    <!-- 页头区域：包含标题和数据概览 -->
    <div class="page-header flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold tracking-tight" style="color: var(--color-text-main)">
          分类管理
        </h1>
        <p class="text-sm mt-1" style="color: var(--color-text-muted)">
          管理全站文章分类及其排序
        </p>
      </div>
      <div class="flex gap-3">
        <!-- 搜索框 -->
        <n-input
          v-model:value="searchQuery"
          placeholder="搜索分类..."
          clearable
          class="w-64"
        >
          <template #prefix>
            <i class="fas fa-search text-gray-400"></i>
          </template>
        </n-input>
        
        <n-button type="primary" @click="openModal()">
          <template #icon>
            <i class="fas fa-plus"></i>
          </template>
          新建分类
        </n-button>
      </div>
    </div>

    <!-- 数据概览卡片行 -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <n-card :bordered="false" size="small" class="stats-card">
        <div class="flex items-center gap-4">
          <div class="p-3 rounded-lg bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400">
            <i class="fas fa-folder text-xl"></i>
          </div>
          <div>
            <div class="text-sm text-gray-500 dark:text-gray-400">总分类数</div>
            <div class="text-2xl font-bold" style="color: var(--color-text-main)">
              {{ categories.length }}
            </div>
          </div>
        </div>
      </n-card>
      
      <n-card :bordered="false" size="small" class="stats-card">
        <div class="flex items-center gap-4">
          <div class="p-3 rounded-lg bg-green-50 dark:bg-green-900/20 text-green-600 dark:text-green-400">
            <i class="fas fa-clock text-xl"></i>
          </div>
          <div>
            <div class="text-sm text-gray-500 dark:text-gray-400">最近更新</div>
            <div class="text-xl font-bold truncate" style="color: var(--color-text-main)">
              {{ lastUpdatedText }}
            </div>
          </div>
        </div>
      </n-card>
      
      <n-card :bordered="false" size="small" class="stats-card">
        <div class="flex items-center gap-4">
          <div class="p-3 rounded-lg bg-purple-50 dark:bg-purple-900/20 text-purple-600 dark:text-purple-400">
            <i class="fas fa-sort-amount-down text-xl"></i>
          </div>
          <div>
            <div class="text-sm text-gray-500 dark:text-gray-400">最大排序值</div>
            <div class="text-2xl font-bold" style="color: var(--color-text-main)">
              {{ maxSortValue }}
            </div>
          </div>
        </div>
      </n-card>
    </div>

    <!-- 分类列表 -->
    <n-card :bordered="false" content-style="padding: 0;" class="shadow-sm rounded-xl overflow-hidden main-table-card">
      <n-data-table
        :columns="columns"
        :data="filteredCategories"
        :loading="loading"
        :pagination="pagination"
        :row-key="(row) => row.id"
        :single-line="false"
        :row-class-name="() => 'category-row'"
      >
        <template #loading>
          <div class="p-8">
            <n-skeleton text :repeat="3" />
            <n-skeleton text style="width: 60%" class="mt-2" />
          </div>
        </template>
        <template #empty>
          <div class="py-12 text-center text-gray-400">
            <i class="fas fa-inbox text-4xl mb-4"></i>
            <p>暂无分类数据</p>
          </div>
        </template>
      </n-data-table>
    </n-card>

    <!-- 编辑/新建弹窗 -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEdit ? '编辑分类' : '新建分类'"
      style="width: 550px"
      :bordered="false"
      size="huge"
      class="modal-premium"
      :mask-closable="false"
    >
      <n-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-placement="left"
        label-width="100"
        require-mark-placement="right-hanging"
        class="py-4"
      >
        <n-form-item label="名称" path="name">
          <n-input v-model:value="form.name" placeholder="请输入分类名称（如：前端开发）" maxlength="20" show-count />
        </n-form-item>
        <n-form-item label="别名 (Slug)" path="slug">
          <n-input v-model:value="form.slug" placeholder="请输入英文别名（如：frontend）" />
          <template #feedback>
            <span class="text-xs text-gray-400">用于 URL 路径，建议使用小写英文</span>
          </template>
        </n-form-item>
        <n-form-item label="排序" path="sort">
          <n-input-number v-model:value="form.sort" :min="0" placeholder="数字越小越靠前" style="width: 100%" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-space justify="end" :size="16">
          <n-button quaternary @click="showModal = false" size="medium">
            取消
          </n-button>
          <n-button type="primary" @click="handleSave" :loading="saving" size="medium">
            <template #icon>
              <i class="fas fa-save"></i>
            </template>
            保存
          </n-button>
        </n-space>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { h, ref, computed, onMounted } from 'vue'
import { 
  NButton, NModal, NForm, NFormItem, NInput, NInputNumber, 
  NDataTable, NCard, NSpace, NTag, NPopconfirm, NSkeleton 
} from 'naive-ui'
import type { FormInst, FormRules, DataTableColumns } from 'naive-ui'
import { formatDistanceToNow } from 'date-fns'
import { zhCN } from 'date-fns/locale'
import type { Category } from '~/types/api'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const api = useApi()
const { handleError } = useErrorHandler()
const message = useSafeMessage()

// State
const categories = ref<Category[]>([])
const loading = ref(false)
const saving = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInst | null>(null)
const searchQuery = ref('')
const pagination = ref({ pageSize: 10 })

// Form
const form = ref({
  id: 0,
  name: '',
  slug: '',
  sort: 0
})

const rules: FormRules = {
  name: { required: true, message: '请输入分类名称', trigger: 'blur' },
  slug: { required: true, message: '请输入分类别名', trigger: 'blur' }
}

// Computed for Stats
const filteredCategories = computed(() => {
  if (!searchQuery.value) return categories.value
  const q = searchQuery.value.toLowerCase()
  return categories.value.filter(c => 
    c.name.toLowerCase().includes(q) || 
    (c.slug && c.slug.toLowerCase().includes(q))
  )
})

const maxSortValue = computed(() => {
  if (categories.value.length === 0) return 0
  return Math.max(...categories.value.map(c => c.sort))
})

const lastUpdatedText = computed(() => {
  if (categories.value.length === 0) return '-'
  // Find the latest createdAt (assuming no updatedAt field for now, or fallback)
  // Ideally backend should provide updatedAt. For now using createdAt of newest item.
  const latestDate = new Date(Math.max(...categories.value.map(c => new Date(c.createdAt).getTime())))
  return formatDistanceToNow(latestDate, { addSuffix: true, locale: zhCN })
})

// Table Columns
const columns: DataTableColumns<Category> = [
  {
    title: '名称',
    key: 'name',
    width: 200,
    render(row) {
      return h('div', { class: 'font-semibold text-base', style: 'color: var(--color-text-main)' }, row.name)
    }
  },
  {
    title: '别名 (Slug)',
    key: 'slug',
    width: 150,
    render(row) {
      return row.slug 
        ? h(NTag, { type: 'info', size: 'small', bordered: false, style: 'font-weight: 500' }, { default: () => row.slug })
        : h('span', { class: 'text-gray-300' }, '-')
    }
  },
  {
    title: '排序',
    key: 'sort',
    sorter: (a, b) => a.sort - b.sort,
    width: 100,
    render(row) {
      return h('div', { class: 'font-mono text-gray-500' }, row.sort)
    }
  },
  {
    title: '创建时间',
    key: 'createdAt',
    width: 180,
    render(row) {
      return h('div', { class: 'text-gray-500 text-sm' }, 
        new Date(row.createdAt).toLocaleDateString() + ' ' + new Date(row.createdAt).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
      )
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    fixed: 'right',
    render(row) {
      return h(NSpace, null, {
        default: () => [
          h(
            NButton,
            {
              size: 'small',
              quaternary: true,
              type: 'primary',
              class: 'action-btn',
              onClick: () => openModal(row)
            },
            { icon: () => h('i', { class: 'fas fa-edit' }) }
          ),
          h(
            NPopconfirm,
            {
              onPositiveClick: () => handleDelete(row),
              negativeText: '取消',
              positiveText: '确认删除',
            },
            {
              trigger: () => h(
                NButton,
                {
                  size: 'small',
                  quaternary: true,
                  type: 'error',
                  class: 'action-btn'
                },
                { icon: () => h('i', { class: 'fas fa-trash' }) }
              ),
              default: () => h('div', [
                h('div', { class: 'font-bold mb-1' }, '确认删除？'),
                h('div', { class: 'text-xs text-gray-500' }, `分类 "${row.name}" 删除后不可恢复。`)
              ])
            }
          )
        ]
      })
    }
  }
]

// Actions
const fetchCategories = async () => {
  loading.value = true
  try {
    const res = await api.get<Category[]>('/Categories')
    categories.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') console.error(e)
    message.error('加载列表失败，请稍后重试')
    categories.value = []
  } finally {
    // 模拟最小加载时间，防止闪烁，让骨架屏展示一下
    setTimeout(() => { loading.value = false }, 300)
  }
}

const openModal = (item?: Category) => {
  if (item) {
    isEdit.value = true
    form.value = { ...item }
  } else {
    isEdit.value = false
    form.value = { id: 0, name: '', slug: '', sort: (maxSortValue.value || 0) + 1 } // Auto increment sort
  }
  showModal.value = true
}

const handleSave = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (errors) => {
    if (!errors) {
      saving.value = true
      try {
        if (isEdit.value) {
          await api.put(`/Categories/${form.value.id}`, form.value)
        } else {
          await api.post('/Categories', form.value)
        }
        message.success(isEdit.value ? '分类已更新' : '新分类创建成功')
        showModal.value = false
        fetchCategories()
      } catch (e: unknown) {
        handleError(e, '保存失败')
      } finally {
        saving.value = false
      }
    }
  })
}

const handleDelete = async (item: Category) => {
  try {
    await api.del(`/Categories/${item.id}`)
    message.success('分类已删除')
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
/* 深度样式调整 */
.stats-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}
.stats-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

:deep(.category-row td) {
  transition: background-color 0.2s ease;
}
:deep(.category-row:hover td) {
  background-color: var(--color-bg-elevated) !important;
}

.action-btn {
  transition: opacity 0.2s;
  opacity: 0.8;
}
.category-row:hover .action-btn {
  opacity: 1;
}

.main-table-card {
  box-shadow: var(--shadow-soft);
}
</style>
