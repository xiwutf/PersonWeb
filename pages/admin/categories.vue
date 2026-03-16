<template>
  <!-- ?? ListPage Pattern ?? -->
  <ListPage
    title="????"
    description="????????????"
    :show-stats="true"
    :stats="stats"
    :columns="internalColumns"
    :data="filteredCategories"
    :loading="loading"
    :pagination="internalPagination"
    :table-config="{
      rowKey: (row: Category) => row.id,
      singleLine: false,
      rowClassName: () => 'category-row'
    }"
    :empty-config="{
      icon: 'fas fa-inbox',
      text: '??????'
    }"
    @row-click="handleRowClick"
  >
    <!-- ???????????? + ???? -->
    <template #header-actions>
      <n-space :size="12">
        <!-- ????-->
        <n-input
          v-model:value="searchQuery"
          placeholder="????..."
          clearable
          style="width: 240px"
        >
          <template #prefix>
            <i class="fas fa-search text-gray-400"></i>
          </template>
        </n-input>

        <!-- ???? -->
        <n-button type="primary" @click="openModal()">
          <template #icon>
            <i class="fas fa-plus"></i>
          </template>
          ????
        </n-button>
      </n-space>
    </template>

    <!-- ???? -->
    <template #stats>
      <n-grid :x-gap="16" :y-gap="16" :cols="3">
        <n-gi>
          <n-card :bordered="false" class="stat-card">
            <div class="stat-content">
              <div class="stat-icon" style="color: var(--color-blue-500)">
                <i class="fas fa-folder"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">{{ categories.length }}</div>
                <div class="stat-label">????</div>
              </div>
            </div>
          </n-card>
        </n-gi>
        <n-gi>
          <n-card :bordered="false" class="stat-card">
            <div class="stat-content">
              <div class="stat-icon" style="color: var(--color-green-500)">
                <i class="fas fa-clock"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value truncate">{{ lastUpdatedText }}</div>
                <div class="stat-label">????</div>
              </div>
            </div>
          </n-card>
        </n-gi>
        <n-gi>
          <n-card :bordered="false" class="stat-card">
            <div class="stat-content">
              <div class="stat-icon" style="color: var(--color-purple-500)">
                <i class="fas fa-sort-amount-down"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">{{ maxSortValue }}</div>
                <div class="stat-label">?????</div>
              </div>
            </div>
          </n-card>
        </n-gi>
      </n-grid>
    </template>
  </ListPage>

    <!-- ??/???? -->
    <n-modal
      v-model:show="showModal"
      preset="card"
      :title="isEdit ? '????' : '????'"
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
        <n-form-item label="??" path="name">
          <n-input v-model:value="form.name" placeholder="???????????????" maxlength="20" show-count />
        </n-form-item>
        <n-form-item label="?? (Slug)" path="slug">
          <n-input v-model:value="form.slug" placeholder="???frontend" />
          <template #feedback>
            <span class="text-xs text-gray-400">?? URL ???????????</span>
          </template>
        </n-form-item>
        <n-form-item label="??" path="sort">
          <n-input-number v-model:value="form.sort" :min="0" placeholder="0" style="width: 100%" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-space justify="end" :size="16">
          <n-button quaternary @click="showModal = false" size="medium">
            ??
          </n-button>
          <n-button type="primary" @click="handleSave" :loading="saving" size="medium">
            <template #icon>
              <i class="fas fa-save"></i>
            </template>
            ??
          </n-button>
        </n-space>
      </template>
    </n-modal>
</template>

<script setup lang="ts">
import { h, ref, computed, onMounted } from 'vue'
import {
  NButton, NModal, NForm, NFormItem, NInput, NInputNumber,
  NCard, NSpace, NTag, NPopconfirm, NGrid, NGi
} from 'naive-ui'
import type { FormInst, FormRules, DataTableColumns } from 'naive-ui'
import { formatDistanceToNow } from 'date-fns'
import { zhCN } from 'date-fns/locale'
import type { Category } from '~/types/api'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'
import ListPage from '~/components/admin/patterns/ListPage.vue'
import type { StatConfig } from '~/components/admin/patterns/ListPage.vue'

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
const pagination = ref({ pageSize: 10, page: 1, total: 0 })

// ????????????????
const stats = computed<StatConfig[]>(() => [
  { label: '????', value: categories.length, icon: 'fas fa-folder', iconColor: 'var(--color-blue-500)' },
  { label: '????', value: lastUpdatedText.value, icon: 'fas fa-clock', iconColor: 'var(--color-green-500)' },
  { label: '????', value: maxSortValue.value, icon: 'fas fa-sort-amount-down', iconColor: 'var(--color-purple-500)' }
])

// Form
const form = ref({
  id: 0,
  name: '',
  slug: '',
  sort: 0
})

const rules: FormRules = {
  name: { required: true, message: '???????', trigger: 'blur' },
  slug: { required: true, message: '??????', trigger: 'blur' }
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
const internalColumns: DataTableColumns<Category> = [
  {
    title: '??',
    key: 'name',
    width: 200,
    render(row) {
      return h('div', { class: 'font-semibold text-base', style: 'color: var(--color-text-main)' }, row.name)
    }
  },
  {
    title: '?? (Slug)',
    key: 'slug',
    width: 150,
    render(row) {
      return row.slug
        ? h(NTag, { type: 'info', size: 'small', bordered: false, style: 'font-weight: 500' }, { default: () => row.slug })
        : h('span', { class: 'text-gray-300' }, '-')
    }
  },
  {
    title: '??',
    key: 'sort',
    sorter: (a, b) => a.sort - b.sort,
    width: 100,
    render(row) {
      return h('div', { class: 'font-mono text-gray-500' }, row.sort)
    }
  },
  {
    title: '????',
    key: 'createdAt',
    width: 180,
    render(row) {
      return h('div', { class: 'text-gray-500 text-sm' },
        new Date(row.createdAt).toLocaleDateString() + ' ' + new Date(row.createdAt).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
      )
    }
  },
  {
    title: '??',
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
              negativeText: '??',
              positiveText: '????',
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
                h('div', { class: 'font-bold mb-1' }, '????'),
                h('div', { class: 'text-xs text-gray-500' }, `??? "${row.name}" ?? URL ??`)
              ])
            }
          )
        ]
      })
    }
  }
]

// ??????
const internalPagination = computed(() => ({
  page: 1,
  pageSize: 10,
  pageCount: Math.ceil(filteredCategories.value.length / 10),
  showSizePicker: true,
  pageSizes: [10, 20, 50, 100],
  showQuickJumper: true
}))

// Actions
const fetchCategories = async () => {
  loading.value = true
  try {
    const res = await api.get<Category[]>('/Categories')
    categories.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') console.error(e)
    message.error('????????????')
    categories.value = []
  } finally {
    // ???????????????????????    setTimeout(() => { loading.value = false }, 300)
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
        message.success(isEdit.value ? '????' : '????')
        showModal.value = false
        fetchCategories()
      } catch (e: unknown) {
        handleError(e, '????')
      } finally {
        saving.value = false
      }
    }
  })
}

const handleDelete = async (item: Category) => {
  try {
    await api.del(`/Categories/${item.id}`)
    message.success('????')
    fetchCategories()
  } catch (e: unknown) {
    handleError(e, '????')
  }
}

// ???
  const handleRowClick = (row: Category) => {
    // ??????
    openModal(row)
}

onMounted(() => {
  fetchCategories()
})
</script>

<style scoped>
/* ?????? */
:deep(.stat-card) {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}
:deep(.stat-card:hover) {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

:deep(.stat-content) {
  display: flex;
  align-items: center;
  gap: var(--spacing-lg);
}

:deep(.stat-icon) {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.05);
  font-size: var(--text-xl);
}

:deep(.stat-info) {
  flex: 1;
}

:deep(.stat-value) {
  font-size: var(--text-2xl);
  font-weight: 700;
  color: var(--color-text-main);
  line-height: 1.2;
}

:deep(.stat-label) {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  margin-top: var(--spacing-xs);
}

/* ??????*/
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
:deep(.category-row:hover .action-btn) {
  opacity: 1;
}
</style>
