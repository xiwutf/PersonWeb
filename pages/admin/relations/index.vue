<template>
  <div class="relations-page">
    <div class="page-header">
      <div>
        <h1 class="page-title">关系跟进</h1>
        <p class="page-desc">管理正在接触的对象与互动进展</p>
      </div>
      <n-button type="primary" @click="showAddModal = true">
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        新增对象
      </n-button>
    </div>

    <!-- 筛选栏 -->
    <n-card class="filter-card">
      <div class="filter-bar">
        <n-input
          v-model:value="filters.q"
          placeholder="搜索昵称、来源..."
          clearable
          style="flex: 1; max-width: 300px;"
          @keyup.enter="loadPersons"
        >
          <template #prefix>
            <i class="fas fa-search"></i>
          </template>
        </n-input>

        <n-select
          v-model:value="filters.stage"
          placeholder="筛选阶段"
          clearable
          style="width: 150px;"
          :options="stageOptions"
        />

        <n-input
          v-model:value="filters.tag"
          placeholder="筛选标签"
          clearable
          style="width: 150px;"
        />

        <n-select
          v-model:value="filters.sort"
          placeholder="排序方式"
          style="width: 150px;"
          :options="sortOptions"
        />

        <n-button type="primary" @click="loadPersons">
          <template #icon>
            <i class="fas fa-filter"></i>
          </template>
          筛选
        </n-button>
      </div>
    </n-card>

    <!-- 列表 -->
    <div v-if="loading" class="loading-container">
      <n-spin size="large" />
    </div>

    <div v-else-if="persons.length === 0" class="empty-container">
      <n-empty description="暂无数据">
        <template #extra>
          <n-button type="primary" @click="showAddModal = true">新增对象</n-button>
        </template>
      </n-empty>
    </div>

    <div v-else class="persons-grid">
      <PersonCard
        v-for="person in persons"
        :key="person.id"
        :person="person"
        @view="handleView"
        @add-interaction="handleAddInteraction"
        @set-reminder="handleSetReminder"
        @generate-suggestion="handleGenerateSuggestion"
      />
    </div>

    <!-- 新增/编辑对象 Modal -->
    <AddPersonModal
      v-model:show="showAddModal"
      @success="handleModalSuccess"
    />

    <!-- 设置提醒 Modal -->
    <n-modal
      v-model:show="showReminderModal"
      preset="card"
      title="设置提醒"
      style="width: 400px"
    >
      <n-form :model="reminderForm" label-placement="left" label-width="80px">
        <n-form-item label="提醒时间">
          <n-date-picker
            v-model:value="reminderForm.remindAt"
            type="datetime"
            format="yyyy-MM-dd HH:mm"
            placeholder="请选择提醒时间"
            style="width: 100%"
          />
        </n-form-item>
      </n-form>
      <template #footer>
        <div style="display: flex; justify-content: flex-end; gap: 8px;">
          <n-button @click="showReminderModal = false">取消</n-button>
          <n-button type="primary" :loading="reminderLoading" @click="handleSaveReminder">
            保存
          </n-button>
        </div>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  NCard,
  NButton,
  NInput,
  NSelect,
  NSpin,
  NEmpty,
  NModal,
  NForm,
  NFormItem,
  NDatePicker,
  useMessage,
  useDialog
} from 'naive-ui'
import PersonCard from '~/components/relations/PersonCard.vue'
import AddPersonModal from '~/components/relations/modals/AddPersonModal.vue'
import { useRelationsApi, type RelationPerson } from '~/composables/useRelationsApi'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const router = useRouter()
const message = useMessage()
const dialog = useDialog()
const relationsApi = useRelationsApi()

const loading = ref(false)
const persons = ref<RelationPerson[]>([])
const showAddModal = ref(false)
const showReminderModal = ref(false)
const reminderLoading = ref(false)
const currentReminderPersonId = ref<string | null>(null)

const filters = reactive({
  q: '',
  stage: null as number | null,
  tag: '',
  sort: 'lastContact'
})

const stageOptions = [
  { label: '新认识', value: 0 },
  { label: '熟悉中', value: 1 },
  { label: '准备约见', value: 2 },
  { label: '已见面', value: 3 },
  { label: '升温中', value: 4 },
  { label: '观察期', value: 5 },
  { label: '已结束', value: 6 }
]

const sortOptions = [
  { label: '最久未联系', value: 'lastContact' },
  { label: '热度最高', value: 'heat' },
  { label: '提醒最近', value: 'recent' }
]

const reminderForm = reactive({
  remindAt: null as number | null
})

const loadPersons = async () => {
  loading.value = true
  try {
    const result = await relationsApi.getPersons({
      q: filters.q || undefined,
      stage: filters.stage !== null ? filters.stage : undefined,
      tag: filters.tag || undefined,
      sort: filters.sort || 'lastContact'
    })
    persons.value = result || []
  } catch (error: any) {
    message.error(error.message || '加载失败')
  } finally {
    loading.value = false
  }
}

const handleView = (id: string) => {
  router.push(`/admin/relations/${id}`)
}

const handleAddInteraction = (id: string) => {
  router.push(`/admin/relations/${id}`)
}

const handleSetReminder = (id: string) => {
  currentReminderPersonId.value = id
  reminderForm.remindAt = null
  showReminderModal.value = true
}

const handleSaveReminder = async () => {
  if (!currentReminderPersonId.value || !reminderForm.remindAt) {
    message.warning('请选择提醒时间')
    return
  }

  reminderLoading.value = true
  try {
    await relationsApi.updatePerson(currentReminderPersonId.value, {
      remindAt: new Date(reminderForm.remindAt).toISOString()
    })
    message.success('提醒设置成功')
    showReminderModal.value = false
    await loadPersons()
  } catch (error: any) {
    message.error(error.message || '设置失败')
  } finally {
    reminderLoading.value = false
  }
}

const handleGenerateSuggestion = async (id: string) => {
  dialog.info({
    title: '生成建议',
    content: '此功能需要查看对象的互动记录，请前往详情页操作',
    positiveText: '前往详情',
    onPositiveClick: () => {
      router.push(`/admin/relations/${id}`)
    }
  })
}

const handleModalSuccess = () => {
  loadPersons()
}

onMounted(() => {
  loadPersons()
})
</script>

<style scoped>
.relations-page {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0 0 8px 0;
}

.page-desc {
  color: var(--text-color-2);
  margin: 0;
}

.filter-card {
  margin-bottom: 24px;
}

.filter-bar {
  display: flex;
  gap: 12px;
  align-items: center;
  flex-wrap: wrap;
}

.loading-container,
.empty-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.persons-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 16px;
}

@media (max-width: 768px) {
  .persons-grid {
    grid-template-columns: 1fr;
  }
}
</style>

