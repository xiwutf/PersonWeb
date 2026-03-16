<template>
  <ClientOnly>
    <div class="side-projects-admin-page">
    <div class="page-header">
      <h1 class="page-title">副业项目管理</h1>
      <n-button type="primary" @click="handleCreate">
        <template #icon>
          <i class="fas fa-plus"></i>
        </template>
        新建项目
      </n-button>
    </div>

    <!-- 筛选栏 -->
    <div class="filters-bar">
      <n-input
        v-model:value="searchKeyword"
        placeholder="搜索项目名称、客户名�?.."
        clearable
        style="width: 300px;"
        @keyup.enter="handleSearch"
      >
        <template #prefix>
          <i class="fas fa-search"></i>
        </template>
      </n-input>
      <n-select
        v-model:value="filterStatus"
        placeholder="状态筛�?
        clearable
        style="width: 150px;"
        :options="statusOptions"
      />
      <n-select
        v-model:value="filterIncomeType"
        placeholder="收入类型"
        clearable
        style="width: 150px;"
        :options="incomeTypeOptions"
      />
      <n-select
        v-model:value="filterCategory"
        placeholder="项目类型"
        clearable
        style="width: 150px;"
        :options="categoryOptions"
      />
      <n-button type="primary" @click="handleSearch">搜索</n-button>
      <n-button @click="handleReset">重置</n-button>
    </div>

    <!-- 数据表格 -->
    <div class="table-container">
      <div v-if="loading" class="table-loading">
        加载�?..
      </div>
      <div v-else-if="projects.length === 0" class="table-empty">
        暂无项目数据
        <n-button type="primary" @click="handleCreate" style="margin-top: 16px;">新建项目</n-button>
      </div>
      <table v-else class="data-table">
        <thead class="table-header">
          <tr>
            <th>项目名称</th>
            <th>客户名称</th>
            <th>项目类型</th>
            <th>状�?/th>
            <th>最终价�?/th>
            <th>是否公开</th>
            <th>开始时�?/th>
            <th>结束时间</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr 
            v-for="project in projects" 
            :key="project.id" 
            class="table-row"
            @click="handleRowClick(project.id)"
            style="cursor: pointer;"
          >
            <td class="table-cell">
              <div class="project-title-cell">{{ project.title || '-' }}</div>
            </td>
            <td class="table-cell">{{ project.clientName || '-' }}</td>
            <td class="table-cell">
              <div style="display: flex; gap: 4px; align-items: center; flex-wrap: wrap;">
                <span v-if="project.incomeType === 'investment'" class="tag tag-success" style="font-size: 11px;">投资</span>
                <span v-else-if="project.incomeType === 'development'" class="tag tag-info" style="font-size: 11px;">软件开�?/span>
                <span class="tag tag-info">{{ project.category || '未分�? }}</span>
              </div>
            </td>
            <td class="table-cell">
              <span :class="getStatusTagClass(project.status)" class="tag">
                {{ getStatusText(project.status) }}
              </span>
            </td>
            <td class="table-cell">
              {{ project.priceFinal ? `¥${formatNumber(project.priceFinal)}` : '-' }}
            </td>
            <td class="table-cell">
              <span :class="project.isPublic ? 'tag tag-success' : 'tag tag-default'">
                {{ project.isPublic ? '公开' : '不公开' }}
              </span>
            </td>
            <td class="table-cell">{{ formatDate(project.startTime) }}</td>
            <td class="table-cell">{{ formatDate(project.endTime) }}</td>
            <td class="table-cell">
              <div class="action-buttons" @click.stop>
                <button 
                  @click="handleEdit(project)" 
                  class="btn-link btn-link-blue"
                >
                  编辑
                </button>
                <button 
                  @click="handleDelete(project.id)" 
                  class="btn-link btn-link-red"
                >
                  删除
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- 分页 -->
      <div v-if="pagination.itemCount > 0" class="table-pagination">
        <div class="pagination-info">
          �?{{ pagination.itemCount }} 条记�?        </div>
        <div class="pagination-controls">
          <select 
            v-model="pagination.pageSize" 
            @change="handlePageSizeChange(pagination.pageSize)"
            class="pagination-select"
          >
            <option :value="10">10/�?/option>
            <option :value="20">20/�?/option>
            <option :value="50">50/�?/option>
            <option :value="100">100/�?/option>
          </select>
          <div class="pagination-buttons">
            <button 
              @click="pagination.page = 1; fetchProjects()"
              :disabled="pagination.page === 1"
              class="pagination-btn"
            >
              首页
            </button>
            <button 
              @click="pagination.page--; fetchProjects()"
              :disabled="pagination.page === 1"
              class="pagination-btn"
            >
              上一�?            </button>
            <span class="pagination-page">
              {{ pagination.page }} / {{ Math.ceil(pagination.itemCount / pagination.pageSize) }}
            </span>
            <button 
              @click="pagination.page++; fetchProjects()"
              :disabled="pagination.page >= Math.ceil(pagination.itemCount / pagination.pageSize)"
              class="pagination-btn"
            >
              下一�?            </button>
            <button 
              @click="pagination.page = Math.ceil(pagination.itemCount / pagination.pageSize); fetchProjects()"
              :disabled="pagination.page >= Math.ceil(pagination.itemCount / pagination.pageSize)"
              class="pagination-btn"
            >
              末页
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑模态框 -->
    <n-modal v-model:show="showModal" preset="card" :title="editingId ? '编辑项目' : '新建项目'" style="width: 800px;">
      <n-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-placement="left"
        label-width="100"
      >
        <n-form-item label="项目名称" path="title">
          <n-input v-model:value="form.title" placeholder="请输入项目名�? />
        </n-form-item>
        <n-form-item label="项目描述" path="description">
          <n-input
            v-model:value="form.description"
            type="textarea"
            :rows="3"
            placeholder="请输入项目描�?
          />
        </n-form-item>
        <n-grid :cols="2" :x-gap="16">
          <n-grid-item>
            <n-form-item label="客户名称" path="clientName">
              <n-input v-model:value="form.clientName" placeholder="请输入客户名�? />
            </n-form-item>
          </n-grid-item>
          <n-grid-item>
            <n-form-item label="客户联系方式" path="clientContact">
              <n-input v-model:value="form.clientContact" placeholder="请输入客户联系方�? />
            </n-form-item>
          </n-grid-item>
        </n-grid>
        <n-grid :cols="2" :x-gap="16">
          <n-grid-item>
            <n-form-item label="客户来源" path="source">
              <n-input v-model:value="form.source" placeholder="如：朋友介绍、平台接单等" />
            </n-form-item>
          </n-grid-item>
          <n-grid-item>
            <n-form-item label="项目类型" path="category">
              <n-input v-model:value="form.category" placeholder="如：网站、小程序、AI等（软件开发）�?股票、基金等（投资）" />
            </n-form-item>
          </n-grid-item>
        </n-grid>
        <n-grid :cols="2" :x-gap="16">
          <n-grid-item>
            <n-form-item label="收入类型" path="incomeType">
              <n-select
                v-model:value="form.incomeType"
                placeholder="选择收入类型"
                :options="incomeTypeOptions"
              />
            </n-form-item>
          </n-grid-item>
        </n-grid>
        <n-form-item label="技术栈" path="techStack" v-if="form.incomeType !== 'investment'">
          <n-input
            v-model:value="form.techStack"
            placeholder="多个技术栈用逗号分隔，如：Vue3,TypeScript,Node.js"
          />
        </n-form-item>
        <n-grid :cols="3" :x-gap="16">
          <n-grid-item>
            <n-form-item label="预算最�? path="budgetMin">
              <n-input-number
                v-model:value="form.budgetMin"
                :min="0"
                :precision="2"
                placeholder="预算最�?
                style="width: 100%;"
              />
            </n-form-item>
          </n-grid-item>
          <n-grid-item>
            <n-form-item label="预算最�? path="budgetMax">
              <n-input-number
                v-model:value="form.budgetMax"
                :min="0"
                :precision="2"
                placeholder="预算最�?
                style="width: 100%;"
              />
            </n-form-item>
          </n-grid-item>
          <n-grid-item>
            <n-form-item label="最终价�? path="priceFinal">
              <n-input-number
                v-model:value="form.priceFinal"
                :min="0"
                :precision="2"
                placeholder="最终价�?
                style="width: 100%;"
              />
            </n-form-item>
          </n-grid-item>
        </n-grid>
        <n-grid :cols="2" :x-gap="16">
          <n-grid-item>
            <n-form-item label="项目状�? path="status">
              <n-select
                v-model:value="form.status"
                :options="statusOptions"
                placeholder="选择项目状�?
              />
            </n-form-item>
          </n-grid-item>
          <n-grid-item>
            <n-form-item label="是否公开" path="isPublic">
              <n-switch v-model:value="form.isPublic" />
              <span class="form-switch-label">{{ form.isPublic ? '公开' : '不公开' }}</span>
            </n-form-item>
          </n-grid-item>
        </n-grid>
        <n-grid :cols="2" :x-gap="16">
          <n-grid-item>
            <n-form-item label="开始时�? path="startTime">
              <n-date-picker
                v-model:value="form.startTime"
                type="datetime"
                clearable
                placeholder="选择开始时�?
                style="width: 100%;"
              />
            </n-form-item>
          </n-grid-item>
          <n-grid-item>
            <n-form-item label="结束时间" path="endTime">
              <n-date-picker
                v-model:value="form.endTime"
                type="datetime"
                clearable
                placeholder="选择结束时间"
                style="width: 100%;"
              />
            </n-form-item>
          </n-grid-item>
        </n-grid>
      </n-form>
      <template #footer>
        <div style="display: flex; justify-content: flex-end; gap: 12px;">
          <n-button @click="showModal = false">取消</n-button>
          <n-button type="primary" @click="handleSubmit" :loading="submitting">保存</n-button>
        </div>
      </template>
    </n-modal>
    </div>
    <template #fallback>
      <div class="side-projects-admin-page">
        <div style="padding: 2rem; text-align: center; color: var(--color-text-muted);">
          加载�?..
        </div>
      </div>
    </template>
  </ClientOnly>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'
import { useRouter } from 'vue-router'
import {
  NButton,
  NInput,
  NSelect,
  NModal,
  NForm,
  NFormItem,
  NGrid,
  NGridItem,
  NInputNumber,
  NSwitch,
  NDatePicker,
  type FormInst,
  type FormRules
} from 'naive-ui'
import { useApi } from '~/composables/useApi'
import { useNotification } from '~/composables/useToast'
import type { SideProject, CreateSideProjectDto, UpdateSideProjectDto } from '~/types/api'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const router = useRouter()
const api = useApi()
const notification = useNotification()

// 数据状�?const loading = ref(false)
const projects = ref<SideProject[]>([])
const searchKeyword = ref('')
const filterStatus = ref<number | null>(null)
const filterIncomeType = ref<string | null>(null)
const filterCategory = ref<string | null>(null)

// 分页
const pagination = ref({
  page: 1,
  pageSize: 20,
  itemCount: 0,
  showSizePicker: true,
  pageSizes: [10, 20, 50, 100]
})

// 模态框
const showModal = ref(false)
const editingId = ref<number | null>(null)
const submitting = ref(false)
const formRef = ref<FormInst | null>(null)

// 表单数据
const form = ref<CreateSideProjectDto | UpdateSideProjectDto>({
  title: '',
  description: '',
  clientName: '',
  clientContact: '',
  source: '',
  category: '',
  incomeType: 'development', // 默认软件开�?  techStack: '',
  budgetMin: null,
  budgetMax: null,
  priceFinal: null,
  status: 1,
  startTime: null,
  endTime: null,
  isPublic: false
})

// 表单验证规则
const rules: FormRules = {
  title: [{ required: true, message: '请输入项目名�?, trigger: 'blur' }]
}

// 状态选项
const statusOptions = [
  { label: '进行�?, value: 0 },
  { label: '已完�?, value: 1 },
  { label: '待付�?, value: 2 },
  { label: '已取�?, value: 3 }
]

// 收入类型选项
const incomeTypeOptions = [
  { label: '软件开�?, value: 'development' },
  { label: '投资', value: 'investment' }
]

// 类型选项（从数据中提取）
const categoryOptions = ref<Array<{ label: string; value: string }>>([])

// 获取状态文�?const getStatusText = (status: number): string => {
  switch (status) {
    case 0: return '进行�?
    case 1: return '已完�?
    case 2: return '待付�?
    case 3: return '已取�?
    default: return '未知'
  }
}

// 获取状态标签样式类
const getStatusTagClass = (status: number): string => {
  switch (status) {
    case 0: return 'tag-info'
    case 1: return 'tag-success'
    case 2: return 'tag-warning'
    case 3: return 'tag-error'
    default: return 'tag-default'
  }
}

// 格式化数�?const formatNumber = (num: number): string => {
  if (typeof num !== 'number' || isNaN(num)) return '0'
  return num.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 格式化日�?const formatDate = (dateStr: string | null | undefined): string => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })
  } catch {
    return dateStr
  }
}

// 获取项目列表
const fetchProjects = async () => {
  loading.value = true
  try {
    const params: any = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize
    }
    if (searchKeyword.value) {
      // 后端需要支持搜索功�?    }
    if (filterStatus.value !== null) {
      params.status = filterStatus.value
    }
    if (filterIncomeType.value) {
      params.incomeType = filterIncomeType.value
    }
    if (filterCategory.value) {
      params.category = filterCategory.value
    }

    const res = await api.get<any>('/side-projects', { params })
    
    if (res && typeof res === 'object') {
      // 处理不同的响应格�?      let list: any[] = []
      let total = 0
      
      // 检查是否是分页响应格式 { total, list }
      if ('list' in res || 'List' in res) {
        list = res.list ?? res.List ?? []
        total = res.total ?? res.Total ?? 0
      } 
      // 检查是否是数组格式
      else if (Array.isArray(res)) {
        list = res
        total = res.length
      }
      // 检查是否是 ApiResponse 格式 { data: { total, list } }
      else if (res.data && typeof res.data === 'object') {
        list = res.data.list ?? res.data.List ?? []
        total = res.data.total ?? res.data.Total ?? 0
      }
      
      projects.value = Array.isArray(list) ? list : []
      pagination.value.itemCount = total
      
      // 提取类型选项
      const categories = new Set<string>()
      projects.value.forEach(p => {
        if (p && p.category) categories.add(p.category)
      })
      categoryOptions.value = Array.from(categories).map(cat => ({ label: cat, value: cat }))
    } else {
      projects.value = []
      pagination.value.itemCount = 0
    }
  } catch (e: any) {
    notification.error('加载项目列表失败', e.message)
  } finally {
    loading.value = false
  }
}

// 创建项目
const handleCreate = () => {
  editingId.value = null
  form.value = {
    title: '',
    description: '',
    clientName: '',
    clientContact: '',
    source: '',
    category: '',
    incomeType: 'development',
    techStack: '',
    budgetMin: null,
    budgetMax: null,
    priceFinal: null,
    status: 1,
    startTime: null,
    endTime: null,
    isPublic: false
  }
  showModal.value = true
}

// 行点击，跳转到详情页
const handleRowClick = (id: number) => {
  router.push(`/admin/side-projects/projects/${id}?from=list`)
}

// 编辑项目
const handleEdit = (project: SideProject) => {
  editingId.value = project.id
  form.value = {
    title: project.title || '',
    description: project.description || '',
    clientName: project.clientName || '',
    clientContact: project.clientContact || '',
    source: project.source || '',
    category: project.category || '',
    incomeType: project.incomeType || 'development',
    techStack: project.techStack || '',
    budgetMin: project.budgetMin,
    budgetMax: project.budgetMax,
    priceFinal: project.priceFinal,
    status: project.status,
    startTime: project.startTime ? new Date(project.startTime).getTime() : null,
    endTime: project.endTime ? new Date(project.endTime).getTime() : null,
    isPublic: project.isPublic
  }
  showModal.value = true
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (errors) => {
    if (errors) return
    
    submitting.value = true
    try {
      const formData = { ...form.value }
      
      // 转换时间戳为 ISO 字符�?      if (formData.startTime && typeof formData.startTime === 'number') {
        formData.startTime = new Date(formData.startTime).toISOString() as any
      }
      if (formData.endTime && typeof formData.endTime === 'number') {
        formData.endTime = new Date(formData.endTime).toISOString() as any
      }
      
      if (editingId.value) {
        // 更新
        await api.put(`/side-projects/${editingId.value}`, formData)
        notification.success('更新成功')
      } else {
        // 创建
        await api.post('/side-projects', formData)
        notification.success('创建成功')
      }
      
      showModal.value = false
      await fetchProjects()
    } catch (e: any) {
      notification.error(editingId.value ? '更新失败' : '创建失败', e.message)
    } finally {
      submitting.value = false
    }
  })
}

// 删除项目
const handleDelete = async (id: number) => {
  if (!confirm('确定要删除这个项目吗�?)) return
  
  try {
    await api.delete(`/side-projects/${id}`)
    notification.success('删除成功')
    await fetchProjects()
  } catch (e: any) {
    notification.error('删除失败', e.message)
  }
}

// 搜索
const handleSearch = () => {
  pagination.value.page = 1
  fetchProjects()
}

// 重置
const handleReset = () => {
  searchKeyword.value = ''
  filterStatus.value = null
  filterIncomeType.value = null
  filterCategory.value = null
  handleSearch()
}

// 分页变化
const handlePageSizeChange = (pageSize: number) => {
  pagination.value.pageSize = pageSize
  pagination.value.page = 1
  fetchProjects()
}

// 初始�?onMounted(() => {
  fetchProjects()
})
</script>

<style scoped>
.side-projects-admin-page {
  padding: var(--spacing-xl);
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.page-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0;
}

.filters-bar {
  display: flex;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
  flex-wrap: wrap;
}

/* 表格容器 */
.table-container {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  overflow: hidden;
  margin-bottom: var(--spacing-xl);
  box-shadow: var(--shadow-sm);
}

.table-loading,
.table-empty {
  padding: var(--spacing-3xl);
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

.project-title-cell {
  font-weight: 600;
  color: var(--color-text-main);
}

/* 标签样式 */
.tag {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 600;
}

.tag-success {
  background: rgba(34, 197, 94, 0.2);
  border: 1px solid rgba(34, 197, 94, 0.4);
  color: var(--color-success);
}

.tag-info {
  background: rgba(59, 130, 246, 0.2);
  border: 1px solid rgba(59, 130, 246, 0.4);
  color: var(--color-primary);
}

.tag-warning {
  background: rgba(245, 158, 11, 0.2);
  border: 1px solid rgba(245, 158, 11, 0.4);
  color: var(--color-warning);
}

.tag-error {
  background: rgba(239, 68, 68, 0.2);
  border: 1px solid rgba(239, 68, 68, 0.4);
  color: var(--color-danger);
}

.tag-default {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  color: var(--color-text-main);
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
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
}

.btn-link-blue {
  color: var(--color-primary-soft);
}

.btn-link-blue:hover {
  color: var(--color-primary-400);
  background: rgba(96, 165, 250, 0.1);
}

.btn-link-red {
  color: var(--color-danger-400);
}

.btn-link-red:hover {
  color: var(--color-danger-300);
  background: rgba(248, 113, 113, 0.1);
}

/* 分页样式 */
.table-pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-lg) var(--spacing-xl);
  border-top: 1px solid var(--color-border-subtle);
  background: var(--color-bg-elevated);
}

.pagination-info {
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.pagination-select {
  padding: var(--spacing-xs) var(--spacing-sm);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-sm);
  background: var(--color-bg-card);
  color: var(--color-text-main);
  font-size: var(--font-size-sm);
  cursor: pointer;
}

.pagination-buttons {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.pagination-btn {
  padding: var(--spacing-xs) var(--spacing-sm);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-sm);
  background: var(--color-bg-card);
  color: var(--color-text-main);
  font-size: var(--font-size-sm);
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination-btn:hover:not(:disabled) {
  background: var(--color-bg-elevated);
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-page {
  padding: var(--spacing-xs) var(--spacing-sm);
  color: var(--color-text-main);
  font-size: var(--font-size-sm);
  font-weight: 500;
}

.form-switch-label {
  margin-left: var(--spacing-sm);
  color: var(--color-text-main);
  font-size: var(--font-size-sm);
}
</style>

