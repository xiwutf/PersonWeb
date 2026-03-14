<template>
  <ClientOnly>
    <div class="intelligence-source-page">
      <!-- 页面标题 -->
      <div class="page-header">
        <h1 class="page-title">情报来源管理</h1>
        <n-button type="primary" @click="handleCreate">
          <template #icon>
            <i class="fas fa-plus"></i>
          </template>
          新增来源
        </n-button>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <n-spin size="large" />
      </div>

      <div v-else>
        <!-- 空状态 -->
        <div v-if="sources.length === 0" class="empty-container">
          <i class="fas fa-rss"></i>
          <p>暂无情报来源</p>
          <n-button type="primary" @click="handleCreate">
            新增第一个来源
          </n-button>
        </div>

        <!-- 来源列表 -->
        <div v-else class="sources-list">
          <div
            v-for="source in sources"
            :key="source.id"
            class="source-card"
          >
            <div class="source-header">
              <div class="source-icon">
                <i v-if="source.sourceType === 'RSS'" class="fas fa-rss"></i>
                <i v-else class="fas fa-globe"></i>
              </div>
              <div class="source-info">
                <div class="source-name">{{ source.sourceName }}</div>
                <div class="source-url">{{ source.sourceUrl }}</div>
              </div>
              <div class="source-status">
                <n-switch
                  v-model:value="source.enabled"
                  @update:value="handleToggleEnabled(source)"
                />
              </div>
            </div>

            <div class="source-meta">
              <n-tag size="small" :bordered="false">{{ source.category }}</n-tag>
              <n-tag size="small" :bordered="false" type="info">{{ source.sourceType }}</n-tag>
              <span class="priority-badge">优先级: {{ source.priority }}</span>
              <span class="interval-badge">{{ source.fetchIntervalMinutes }} 分钟</span>
              <span v-if="source.lastFetchTime" class="last-fetch-time">
                上次抓取: {{ formatTime(source.lastFetchTime) }}
              </span>
            </div>

            <div v-if="source.tags && source.tags.length > 0" class="source-tags">
              <n-tag
                v-for="tag in source.tags"
                :key="tag"
                size="small"
                :bordered="false"
                type="info"
              >
                {{ tag }}
              </n-tag>
            </div>

            <div v-if="source.remark" class="source-remark">
              <i class="fas fa-info-circle"></i> {{ source.remark }}
            </div>

            <div class="source-actions">
              <n-button size="small" @click="handleEdit(source)">
                <template #icon>
                  <i class="fas fa-edit"></i>
                </template>
                编辑
              </n-button>
              <n-button size="small" type="error" @click="handleDelete(source)">
                <template #icon>
                  <i class="fas fa-trash"></i>
                </template>
                删除
              </n-button>
            </div>
          </div>
        </div>
      </div>

      <!-- 新增/编辑弹窗 -->
      <n-modal
        v-model:show="showModal"
        :title="isEditing ? '编辑来源' : '新增来源'"
        preset="card"
        class="source-modal"
      >
        <n-form
          ref="formRef"
          :model="formData"
          :rules="formRules"
          label-placement="left"
          label-width="100px"
        >
          <n-form-item label="来源名称" path="sourceName">
            <n-input
              v-model:value="formData.sourceName"
              placeholder="请输入来源名称"
            />
          </n-form-item>

          <n-form-item label="来源类型" path="sourceType">
            <n-select
              v-model:value="formData.sourceType"
              :options="sourceTypeOptions"
            />
          </n-form-item>

          <n-form-item label="来源地址" path="sourceUrl">
            <n-input
              v-model:value="formData.sourceUrl"
              placeholder="请输入 RSS 地址或网页地址"
            />
          </n-form-item>

          <n-form-item label="分类" path="category">
            <n-input
              v-model:value="formData.category"
              placeholder="请输入分类，如：AI技术"
            />
          </n-form-item>

          <n-form-item label="标签" path="tags">
            <n-dynamic-tags v-model:value="formData.tags" />
          </n-form-item>

          <n-form-item label="优先级" path="priority">
            <n-input-number
              v-model:value="formData.priority"
              :min="0"
              :max="100"
              class="full-width-input"
            />
          </n-form-item>

          <n-form-item label="抓取间隔" path="fetchIntervalMinutes">
            <n-input-number
              v-model:value="formData.fetchIntervalMinutes"
              :min="5"
              :step="10"
              class="full-width-input"
            />
            <template #feedback>
              <span class="form-feedback-text">单位：分钟，最小 5 分钟</span>
            </template>
          </n-form-item>

          <n-form-item label="是否启用" path="enabled">
            <n-switch v-model:value="formData.enabled" />
          </n-form-item>

          <n-form-item label="备注" path="remark">
            <n-input
              v-model:value="formData.remark"
              type="textarea"
              placeholder="请输入备注信息（可选）"
              :rows="3"
            />
          </n-form-item>
        </n-form>

        <template #footer>
          <div class="modal-footer">
            <n-button @click="showModal = false">取消</n-button>
            <n-button type="primary" :loading="submitting" @click="handleSubmit">
              {{ isEditing ? '保存' : '创建' }}
            </n-button>
          </div>
        </template>
      </n-modal>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import { useNotification } from '~/composables/useToast'
import type { IntelligenceSource, SourceRequest } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useIntelligenceApi()
const notification = useNotification()

// 状态
const loading = ref(false)
const sources = ref<IntelligenceSource[]>([])
const showModal = ref(false)
const isEditing = ref(false)
const submitting = ref(false)
const editingId = ref<number | null>(null)

// 表单数据
const formData = reactive<SourceRequest>({
  sourceName: '',
  sourceType: 'RSS',
  sourceUrl: '',
  category: '',
  tags: [],
  priority: 0,
  enabled: true,
  fetchIntervalMinutes: 60,
  remark: ''
})

// 表单验证规则
const formRules = {
  sourceName: { required: true, message: '请输入来源名称', trigger: 'blur' },
  sourceType: { required: true, message: '请选择来源类型', trigger: 'change' },
  sourceUrl: { required: true, message: '请输入来源地址', trigger: 'blur' },
  category: { required: true, message: '请输入分类', trigger: 'blur' }
}

// 来源类型选项
const sourceTypeOptions = [
  { label: 'RSS', value: 'RSS' },
  { label: '网页', value: 'WEB' }
]

// 获取来源列表
const fetchSources = async () => {
  loading.value = true
  try {
    sources.value = await api.getSourceList()
  } catch (error) {
    console.error('获取来源列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 新增来源
const handleCreate = () => {
  isEditing.value = false
  editingId.value = null
  resetForm()
  showModal.value = true
}

// 编辑来源
const handleEdit = (source: IntelligenceSource) => {
  isEditing.value = true
  editingId.value = source.id
  Object.assign(formData, {
    sourceName: source.sourceName,
    sourceType: source.sourceType,
    sourceUrl: source.sourceUrl,
    category: source.category,
    tags: [...(source.tags || [])],
    priority: source.priority,
    enabled: source.enabled,
    fetchIntervalMinutes: source.fetchIntervalMinutes,
    remark: source.remark || ''
  })
  showModal.value = true
}

// 删除来源
const handleDelete = async (source: IntelligenceSource) => {
  window.$dialog?.warning({
    title: '确认删除',
    content: `确定要删除来源「${source.sourceName}」吗？`,
    positiveText: '确定',
    negativeText: '取消',
    onPositiveClick: async () => {
      try {
        await api.deleteSource(source.id)
        notification.success('删除成功')
        await fetchSources()
      } catch (error) {
        console.error('删除失败:', error)
        notification.error('删除失败')
      }
    }
  })
}

// 启用/禁用来源
const handleToggleEnabled = async (source: IntelligenceSource) => {
  try {
    await api.toggleSource(source.id, source.enabled)
    notification.success(source.enabled ? '已启用' : '已禁用')
  } catch (error) {
    console.error('操作失败:', error)
    // 恢复原状态
    source.enabled = !source.enabled
    notification.error('操作失败')
  }
}

// 提交表单
const handleSubmit = async () => {
  submitting.value = true
  try {
    if (isEditing.value && editingId.value) {
      await api.updateSource(editingId.value, formData)
      notification.success('更新成功')
    } else {
      await api.createSource(formData)
      notification.success('创建成功')
    }
    showModal.value = false
    await fetchSources()
  } catch (error) {
    console.error('提交失败:', error)
    notification.error('提交失败')
  } finally {
    submitting.value = false
  }
}

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    sourceName: '',
    sourceType: 'RSS',
    sourceUrl: '',
    category: '',
    tags: [],
    priority: 0,
    enabled: true,
    fetchIntervalMinutes: 60,
    remark: ''
  })
}

// 格式化时间
const formatTime = (time?: string) => {
  if (!time) return ''
  const d = new Date(time)
  const now = new Date()
  const diff = now.getTime() - d.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  return `${days}天前`
}

// 初始化
onMounted(() => {
  fetchSources()
})
</script>

<style scoped>
.intelligence-source-page {
  padding: var(--spacing-lg);
  max-width: 1000px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
  color: var(--color-text-main);
}

.loading-container,
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  color: var(--color-text-sub);
}

.empty-container i {
  font-size: 48px;
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.empty-container p {
  margin: 0 0 var(--spacing-md) 0;
}

/* 来源列表 */
.sources-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.source-card {
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  box-shadow: var(--shadow-card);
  transition: all 0.2s;
}

.source-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}

.source-header {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-sm);
}

.source-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-md);
  background: var(--color-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-bg-light);
  font-size: 20px;
  flex-shrink: 0;
}

.source-info {
  flex: 1;
  min-width: 0;
}

.source-name {
  font-weight: 600;
  color: var(--color-text-main);
  font-size: 16px;
  margin-bottom: var(--spacing-sm);
}

.source-url {
  font-size: 13px;
  color: var(--color-text-sec);
  overflow: hidden;
  text-overflow: ellipsis;
  var(--color-bg-light, white)-space: nowrap;
}

.source-status {
  flex-shrink: 0;
}

.source-meta {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
  font-size: 13px;
  color: var(--color-text-sec);
  margin-bottom: var(--spacing-sm);
  flex-wrap: wrap;
}

.priority-badge,
.interval-badge,
.last-fetch-time {
  color: var(--color-text-sub);
}

.source-tags {
  display: flex;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-sm);
  flex-wrap: wrap;
}

.source-remark {
  font-size: 13px;
  color: var(--color-text-sec);
  margin-bottom: var(--spacing-sm);
  padding: var(--spacing-sm) 10px;
  background: var(--color-bg-card);
  border-radius: var(--radius-sm);
}

.source-remark i {
  margin-right: var(--spacing-sm);
}

.source-actions {
  display: flex;
  gap: var(--spacing-sm);
}

.source-modal {
  width: 600px;
}

.full-width-input {
  width: 100%;
}

.form-feedback-text {
  font-size: 12px;
  color: var(--color-text-sub);
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
