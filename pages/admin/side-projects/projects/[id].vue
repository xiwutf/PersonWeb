<template>
  <div class="project-detail-page">
    <div v-if="loading" class="loading-container">
      加载中...
    </div>

    <div v-else-if="!project" class="error-container">
      <n-result status="404" title="项目不存在" description="请检查项目ID是否正确">
        <template #footer>
          <n-button @click="router.push('/admin/side-projects')">返回项目列表</n-button>
        </template>
      </n-result>
    </div>

    <div v-else>
      <!-- 页面头部 -->
      <div class="page-header">
        <div class="header-left">
          <n-button quaternary @click="handleBack">
            <template #icon>
              <i class="fas fa-arrow-left"></i>
            </template>
            返回
          </n-button>
          <h1 class="page-title">{{ project.title }}</h1>
          <n-tag v-if="project.stage" :type="getStageTagType(project.stage)" size="medium">
            {{ project.stage }}
          </n-tag>
          <n-tag v-if="project.priority !== undefined" :type="getPriorityTagType(project.priority)" size="medium">
            {{ getPriorityText(project.priority) }}
          </n-tag>
        </div>
        <div class="header-right">
          <n-button type="primary" @click="handleEdit">
            <template #icon>
              <i class="fas fa-edit"></i>
            </template>
            编辑项目
          </n-button>
        </div>
      </div>

      <!-- 项目概览卡片 -->
      <div class="overview-cards">
        <n-card class="overview-card">
          <div class="card-item">
            <span class="label">进度</span>
            <div class="value">
              <n-progress
                :percentage="project.progress || 0"
                :height="24"
                :status="getProgressStatus(project.progress)"
              />
              <span class="progress-text">{{ project.progress || 0 }}%</span>
            </div>
          </div>
          <div class="card-item">
            <span class="label">状态</span>
            <span class="value">{{ getStatusText(project.status) }}</span>
          </div>
          <div class="card-item">
            <span class="label">客户</span>
            <span class="value">{{ project.clientName || '-' }}</span>
          </div>
          <div class="card-item">
            <span class="label">截止时间</span>
            <span class="value">{{ formatDate(project.deadlineAt) }}</span>
          </div>
        </n-card>

        <n-card class="overview-card">
          <div class="card-item">
            <span class="label">下一步行动</span>
            <span class="value">{{ project.nextAction || '暂无' }}</span>
          </div>
          <div class="card-item" v-if="project.blocked">
            <span class="label error">阻塞原因</span>
            <span class="value error">{{ project.blockReason || '未说明' }}</span>
          </div>
          <div class="card-item">
            <span class="label">总金额</span>
            <span class="value">¥{{ formatNumber(project.totalAmount || project.priceFinal || 0) }}</span>
          </div>
          <div class="card-item">
            <span class="label">已收款</span>
            <span class="value">¥{{ formatNumber(project.receivedAmount || 0) }}</span>
          </div>
        </n-card>
      </div>

      <!-- Tabs -->
      <n-tabs v-model:value="activeTab" type="line" animated>
        <!-- 概览 -->
        <n-tab-pane name="overview" tab="概览">
          <n-card>
            <div class="detail-section">
              <h3>项目描述</h3>
              <p>{{ project.description || '暂无描述' }}</p>
            </div>
            <div class="detail-section">
              <h3>基本信息</h3>
              <n-descriptions :column="2" bordered>
                <n-descriptions-item label="项目类型">{{ project.category || '-' }}</n-descriptions-item>
                <n-descriptions-item label="收入类型">
                  {{ project.incomeType === 'development' ? '软件开发' : project.incomeType === 'investment' ? '投资' : '-' }}
                </n-descriptions-item>
                <n-descriptions-item label="技术栈">{{ project.techStack || '-' }}</n-descriptions-item>
                <n-descriptions-item label="客户来源">{{ project.source || '-' }}</n-descriptions-item>
                <n-descriptions-item label="开始时间">{{ formatDate(project.startTime) }}</n-descriptions-item>
                <n-descriptions-item label="结束时间">{{ formatDate(project.endTime) }}</n-descriptions-item>
              </n-descriptions>
            </div>
          </n-card>
        </n-tab-pane>

        <!-- 需求与范围 -->
        <n-tab-pane name="requirement" tab="需求与范围">
          <n-card>
            <div v-if="!requirement" class="empty-state">
              <p>暂无需求信息</p>
              <n-button type="primary" @click="showRequirementModal = true">添加需求</n-button>
            </div>
            <div v-else>
              <div class="detail-section">
                <h3>范围内需求</h3>
                <div class="content-box">{{ requirement.scopeIn || '暂无' }}</div>
              </div>
              <div class="detail-section">
                <h3>范围外需求</h3>
                <div class="content-box">{{ requirement.scopeOut || '暂无' }}</div>
              </div>
              <div class="detail-section">
                <h3>验收标准</h3>
                <div class="content-box">{{ requirement.acceptanceCriteria || '暂无' }}</div>
              </div>
              <div class="detail-section">
                <h3>交付物</h3>
                <div class="content-box">{{ requirement.deliverables || '暂无' }}</div>
              </div>
              <n-button type="primary" @click="showRequirementModal = true">编辑需求</n-button>
            </div>
          </n-card>
        </n-tab-pane>

        <!-- 任务 -->
        <n-tab-pane name="tasks" tab="任务">
          <n-card>
            <div class="section-header">
              <h3>任务列表</h3>
              <n-button type="primary" @click="showTaskModal = true">
                <template #icon>
                  <i class="fas fa-plus"></i>
                </template>
                新建任务
              </n-button>
            </div>
            <div class="task-filters">
              <n-select
                v-model:value="taskFilterStatus"
                placeholder="筛选状态"
                clearable
                style="width: 150px"
                :options="taskStatusOptions"
                @update:value="fetchTasks"
              />
            </div>
            <n-list v-if="tasks.length > 0">
              <n-list-item v-for="task in tasks" :key="task.id">
                <div class="task-item">
                  <n-checkbox
                    :checked="task.status === 2"
                    @update:checked="(val) => handleTaskStatusChange(task.id, val ? 2 : 0)"
                  />
                  <div class="task-content">
                    <div class="task-title">{{ task.title }}</div>
                    <div class="task-meta">
                      <n-tag v-if="task.priority !== undefined" :type="getPriorityTagType(task.priority)" size="small">
                        {{ getPriorityText(task.priority) }}
                      </n-tag>
                      <span v-if="task.dueAt">截止: {{ formatDate(task.dueAt) }}</span>
                      <span v-if="task.estHours">预计: {{ task.estHours }}h</span>
                    </div>
                    <div v-if="task.description" class="task-description">{{ task.description }}</div>
                  </div>
                  <div class="task-actions">
                    <n-button quaternary size="small" @click="handleEditTask(task)">编辑</n-button>
                    <n-button quaternary size="small" type="error" @click="handleDeleteTask(task.id)">删除</n-button>
                  </div>
                </div>
              </n-list-item>
            </n-list>
            <div v-else class="empty-state">
              <p>暂无任务</p>
            </div>
          </n-card>
        </n-tab-pane>

        <!-- 里程碑 -->
        <n-tab-pane name="milestones" tab="里程碑">
          <n-card>
            <div class="section-header">
              <h3>里程碑列表</h3>
              <n-button type="primary" @click="showMilestoneModal = true">
                <template #icon>
                  <i class="fas fa-plus"></i>
                </template>
                新建里程碑
              </n-button>
            </div>
            <n-list v-if="milestones.length > 0">
              <n-list-item v-for="milestone in milestones" :key="milestone.id">
                <div class="milestone-item">
                  <n-checkbox
                    :checked="milestone.status === 1"
                    @update:checked="(val) => handleMilestoneStatusChange(milestone.id, val ? 1 : 0)"
                  />
                  <div class="milestone-content">
                    <div class="milestone-title">{{ milestone.title }}</div>
                    <div class="milestone-meta">
                      <span v-if="milestone.dueAt">截止: {{ formatDate(milestone.dueAt) }}</span>
                      <n-tag :type="milestone.status === 1 ? 'success' : 'default'" size="small">
                        {{ milestone.status === 1 ? '已完成' : '未完成' }}
                      </n-tag>
                    </div>
                    <div v-if="milestone.notes" class="milestone-notes">{{ milestone.notes }}</div>
                  </div>
                  <div class="milestone-actions">
                    <n-button quaternary size="small" @click="handleEditMilestone(milestone)">编辑</n-button>
                    <n-button quaternary size="small" type="error" @click="handleDeleteMilestone(milestone.id)">删除</n-button>
                  </div>
                </div>
              </n-list-item>
            </n-list>
            <div v-else class="empty-state">
              <p>暂无里程碑</p>
            </div>
          </n-card>
        </n-tab-pane>

        <!-- 沟通记录 -->
        <n-tab-pane name="logs" tab="沟通记录">
          <n-card>
            <div class="section-header">
              <h3>沟通记录</h3>
              <n-button type="primary" @click="showLogModal = true">
                <template #icon>
                  <i class="fas fa-plus"></i>
                </template>
                新增记录
              </n-button>
            </div>
            <n-timeline v-if="logs.length > 0">
              <n-timeline-item
                v-for="log in logs"
                :key="log.id"
                :time="formatDateTime(log.createdAt)"
              >
                <div class="log-item">
                  <div class="log-header">
                    <n-tag size="small">{{ log.channel || '其他' }}</n-tag>
                  </div>
                  <div class="log-content">{{ log.content || '无内容' }}</div>
                  <div v-if="log.nextTodo" class="log-todo">
                    <strong>下一步：</strong>{{ log.nextTodo }}
                  </div>
                  <n-button quaternary size="small" type="error" @click="handleDeleteLog(log.id)">删除</n-button>
                </div>
              </n-timeline-item>
            </n-timeline>
            <div v-else class="empty-state">
              <p>暂无沟通记录</p>
            </div>
          </n-card>
        </n-tab-pane>

        <!-- 财务与附件 -->
        <n-tab-pane name="finance" tab="财务与附件">
          <n-card>
            <div class="detail-section">
              <h3>财务信息</h3>
              <n-descriptions :column="2" bordered>
                <n-descriptions-item label="总金额">¥{{ formatNumber(project.totalAmount || project.priceFinal || 0) }}</n-descriptions-item>
                <n-descriptions-item label="已收款">¥{{ formatNumber(project.receivedAmount || 0) }}</n-descriptions-item>
                <n-descriptions-item label="预算下限">¥{{ formatNumber(project.budgetMin || 0) }}</n-descriptions-item>
                <n-descriptions-item label="预算上限">¥{{ formatNumber(project.budgetMax || 0) }}</n-descriptions-item>
              </n-descriptions>
            </div>
            <div class="detail-section">
              <div class="section-header">
                <h3>附件</h3>
                <n-button type="primary" @click="showAttachmentModal = true">
                  <template #icon>
                    <i class="fas fa-plus"></i>
                  </template>
                  上传附件
                </n-button>
              </div>
              <n-list v-if="attachments.length > 0">
                <n-list-item v-for="attachment in attachments" :key="attachment.id">
                  <div class="attachment-item">
                    <i class="fas fa-file"></i>
                    <span>{{ attachment.name }}</span>
                    <n-button quaternary size="small" @click="handleOpenAttachment(attachment.url)">查看</n-button>
                    <n-button quaternary size="small" type="error" @click="handleDeleteAttachment(attachment.id)">删除</n-button>
                  </div>
                </n-list-item>
              </n-list>
              <div v-else class="empty-state">
                <p>暂无附件</p>
              </div>
            </div>
          </n-card>
        </n-tab-pane>
      </n-tabs>
    </div>

    <!-- 需求编辑模态框 -->
    <n-modal v-model:show="showRequirementModal" preset="card" title="编辑需求" style="width: 800px;">
      <n-form :model="requirementForm" label-placement="left" label-width="100">
        <n-form-item label="范围内需求">
          <n-input v-model:value="requirementForm.scopeIn" type="textarea" :rows="4" />
        </n-form-item>
        <n-form-item label="范围外需求">
          <n-input v-model:value="requirementForm.scopeOut" type="textarea" :rows="4" />
        </n-form-item>
        <n-form-item label="验收标准">
          <n-input v-model:value="requirementForm.acceptanceCriteria" type="textarea" :rows="4" />
        </n-form-item>
        <n-form-item label="交付物">
          <n-input v-model:value="requirementForm.deliverables" type="textarea" :rows="4" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-button @click="showRequirementModal = false">取消</n-button>
        <n-button type="primary" @click="handleSaveRequirement" :loading="saving">保存</n-button>
      </template>
    </n-modal>

    <!-- 任务编辑模态框 -->
    <n-modal v-model:show="showTaskModal" preset="card" :title="editingTask ? '编辑任务' : '新建任务'" style="width: 600px;">
      <n-form :model="taskForm" label-placement="left" label-width="100">
        <n-form-item label="任务标题">
          <n-input v-model:value="taskForm.title" />
        </n-form-item>
        <n-form-item label="任务描述">
          <n-input v-model:value="taskForm.description" type="textarea" :rows="3" />
        </n-form-item>
        <n-form-item label="状态">
          <n-select v-model:value="taskForm.status" :options="taskStatusOptions" />
        </n-form-item>
        <n-form-item label="优先级">
          <n-select v-model:value="taskForm.priority" :options="priorityOptions" />
        </n-form-item>
        <n-form-item label="截止时间">
          <n-date-picker v-model:value="taskForm.dueAt" type="datetime" clearable />
        </n-form-item>
        <n-form-item label="预计工时">
          <n-input-number v-model:value="taskForm.estHours" :min="0" :precision="1" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-button @click="showTaskModal = false">取消</n-button>
        <n-button type="primary" @click="handleSaveTask" :loading="saving">保存</n-button>
      </template>
    </n-modal>

    <!-- 里程碑编辑模态框 -->
    <n-modal v-model:show="showMilestoneModal" preset="card" :title="editingMilestone ? '编辑里程碑' : '新建里程碑'" style="width: 600px;">
      <n-form :model="milestoneForm" label-placement="left" label-width="100">
        <n-form-item label="里程碑标题">
          <n-input v-model:value="milestoneForm.title" />
        </n-form-item>
        <n-form-item label="截止时间">
          <n-date-picker v-model:value="milestoneForm.dueAt" type="datetime" clearable />
        </n-form-item>
        <n-form-item label="状态">
          <n-select v-model:value="milestoneForm.status" :options="milestoneStatusOptions" />
        </n-form-item>
        <n-form-item label="备注">
          <n-input v-model:value="milestoneForm.notes" type="textarea" :rows="3" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-button @click="showMilestoneModal = false">取消</n-button>
        <n-button type="primary" @click="handleSaveMilestone" :loading="saving">保存</n-button>
      </template>
    </n-modal>

    <!-- 沟通记录模态框 -->
    <n-modal v-model:show="showLogModal" preset="card" title="新增沟通记录" style="width: 600px;">
      <n-form :model="logForm" label-placement="left" label-width="100">
        <n-form-item label="沟通渠道">
          <n-select v-model:value="logForm.channel" :options="channelOptions" />
        </n-form-item>
        <n-form-item label="沟通内容">
          <n-input v-model:value="logForm.content" type="textarea" :rows="4" />
        </n-form-item>
        <n-form-item label="下一步待办">
          <n-input v-model:value="logForm.nextTodo" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-button @click="showLogModal = false">取消</n-button>
        <n-button type="primary" @click="handleSaveLog" :loading="saving">保存</n-button>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  NButton,
  NTag,
  NProgress,
  NCard,
  NTabs,
  NTabPane,
  NDescriptions,
  NDescriptionsItem,
  NList,
  NListItem,
  NCheckbox,
  NSelect,
  NModal,
  NForm,
  NFormItem,
  NInput,
  NInputNumber,
  NDatePicker,
  NTimeline,
  NTimelineItem,
  NResult
} from 'naive-ui'
import { useApi } from '~/composables/useApi'
import { useNotification } from '~/composables/useToast'
import type {
  SideProject,
  SideProjectDetail,
  SideProjectRequirement,
  SideProjectTask,
  SideProjectMilestone,
  SideProjectLog,
  SideProjectAttachment
} from '~/types/api'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const route = useRoute()
const router = useRouter()
const api = useApi()
const notification = useNotification()

const projectId = computed(() => parseInt(route.params.id as string))
const loading = ref(false)
const saving = ref(false)
const project = ref<SideProject | null>(null)
const requirement = ref<SideProjectRequirement | null>(null)
const tasks = ref<SideProjectTask[]>([])
const milestones = ref<SideProjectMilestone[]>([])
const logs = ref<SideProjectLog[]>([])
const attachments = ref<SideProjectAttachment[]>([])

const activeTab = ref('overview')
const taskFilterStatus = ref<number | null>(null)

// 模态框状态
const showRequirementModal = ref(false)
const showTaskModal = ref(false)
const showMilestoneModal = ref(false)
const showLogModal = ref(false)
const showAttachmentModal = ref(false)

const editingTask = ref<SideProjectTask | null>(null)
const editingMilestone = ref<SideProjectMilestone | null>(null)

// 表单数据
const requirementForm = ref({
  scopeIn: '',
  scopeOut: '',
  acceptanceCriteria: '',
  deliverables: ''
})

const taskForm = ref({
  title: '',
  description: '',
  status: 0,
  priority: 1,
  dueAt: null as number | null,
  estHours: null as number | null
})

const milestoneForm = ref({
  title: '',
  dueAt: null as number | null,
  status: 0,
  notes: ''
})

const logForm = ref({
  channel: '',
  content: '',
  nextTodo: ''
})

// 选项
const taskStatusOptions = [
  { label: '未开始', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 }
]

const milestoneStatusOptions = [
  { label: '未完成', value: 0 },
  { label: '已完成', value: 1 }
]

const priorityOptions = [
  { label: '低', value: 0 },
  { label: '中', value: 1 },
  { label: '高', value: 2 },
  { label: '紧急', value: 3 }
]

const channelOptions = [
  { label: '微信', value: '微信' },
  { label: '邮件', value: '邮件' },
  { label: '电话', value: '电话' },
  { label: '会议', value: '会议' },
  { label: '其他', value: '其他' }
]

// 获取项目详情
const fetchProjectDetail = async () => {
  loading.value = true
  try {
    const res = await api.get<SideProjectDetail>(`/side-projects/${projectId.value}/detail`)
    if (res) {
      project.value = res
      requirement.value = res.requirements?.[0] || null
      tasks.value = res.tasks || []
      milestones.value = res.milestones || []
      logs.value = res.logs || []
      attachments.value = res.attachments || []

      // 填充需求表单
      if (requirement.value) {
        requirementForm.value = {
          scopeIn: requirement.value.scopeIn || '',
          scopeOut: requirement.value.scopeOut || '',
          acceptanceCriteria: requirement.value.acceptanceCriteria || '',
          deliverables: requirement.value.deliverables || ''
        }
      }
    }
  } catch (e: any) {
    console.error('获取项目详情失败:', e)
    notification.error('获取项目详情失败: ' + (e.message || '未知错误'))
  } finally {
    loading.value = false
  }
}

// 获取任务列表
const fetchTasks = async () => {
  try {
    const params: any = {}
    if (taskFilterStatus.value !== null) {
      params.status = taskFilterStatus.value
    }
    const res = await api.get<SideProjectTask[]>(`/side-projects/${projectId.value}/tasks`, { params })
    if (Array.isArray(res)) {
      tasks.value = res
    }
  } catch (e: any) {
    console.error('获取任务列表失败:', e)
    notification.error('获取任务列表失败: ' + (e.message || '未知错误'))
  }
}

// 保存需求
const handleSaveRequirement = async () => {
  saving.value = true
  try {
    await api.put(`/side-projects/${projectId.value}/requirement`, requirementForm.value)
    notification.success('保存成功')
    showRequirementModal.value = false
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('保存失败: ' + (e.message || '未知错误'))
  } finally {
    saving.value = false
  }
}

// 保存任务
const handleSaveTask = async () => {
  saving.value = true
  try {
    const formData = { ...taskForm.value }
    if (formData.dueAt && typeof formData.dueAt === 'number') {
      formData.dueAt = new Date(formData.dueAt).toISOString() as any
    }

    if (editingTask.value) {
      await api.put(`/side-projects/${projectId.value}/tasks/${editingTask.value.id}`, formData)
      notification.success('更新成功')
    } else {
      await api.post(`/side-projects/${projectId.value}/tasks`, formData)
      notification.success('创建成功')
    }
    showTaskModal.value = false
    editingTask.value = null
    resetTaskForm()
    await fetchTasks()
    await fetchProjectDetail() // 刷新进度
  } catch (e: any) {
    notification.error('保存失败: ' + (e.message || '未知错误'))
  } finally {
    saving.value = false
  }
}

// 编辑任务
const handleEditTask = (task: SideProjectTask) => {
  editingTask.value = task
  taskForm.value = {
    title: task.title,
    description: task.description || '',
    status: task.status,
    priority: task.priority || 1,
    dueAt: task.dueAt ? new Date(task.dueAt).getTime() : null,
    estHours: task.estHours || null
  }
  showTaskModal.value = true
}

// 删除任务
const handleDeleteTask = async (taskId: number) => {
  if (!confirm('确定要删除这个任务吗？')) return
  try {
    await api.delete(`/side-projects/${projectId.value}/tasks/${taskId}`)
    notification.success('删除成功')
    await fetchTasks()
    await fetchProjectDetail() // 刷新进度
  } catch (e: any) {
    notification.error('删除失败: ' + (e.message || '未知错误'))
  }
}

// 任务状态变更
const handleTaskStatusChange = async (taskId: number, status: number) => {
  try {
    await api.put(`/side-projects/${projectId.value}/tasks/${taskId}`, { status })
    await fetchTasks()
    await fetchProjectDetail() // 刷新进度
  } catch (e: any) {
    notification.error('更新失败: ' + (e.message || '未知错误'))
  }
}

// 保存里程碑
const handleSaveMilestone = async () => {
  saving.value = true
  try {
    const formData = { ...milestoneForm.value }
    if (formData.dueAt && typeof formData.dueAt === 'number') {
      formData.dueAt = new Date(formData.dueAt).toISOString() as any
    }

    if (editingMilestone.value) {
      await api.put(`/side-projects/${projectId.value}/milestones/${editingMilestone.value.id}`, formData)
      notification.success('更新成功')
    } else {
      await api.post(`/side-projects/${projectId.value}/milestones`, formData)
      notification.success('创建成功')
    }
    showMilestoneModal.value = false
    editingMilestone.value = null
    resetMilestoneForm()
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('保存失败: ' + (e.message || '未知错误'))
  } finally {
    saving.value = false
  }
}

// 编辑里程碑
const handleEditMilestone = (milestone: SideProjectMilestone) => {
  editingMilestone.value = milestone
  milestoneForm.value = {
    title: milestone.title,
    dueAt: milestone.dueAt ? new Date(milestone.dueAt).getTime() : null,
    status: milestone.status,
    notes: milestone.notes || ''
  }
  showMilestoneModal.value = true
}

// 删除里程碑
const handleDeleteMilestone = async (milestoneId: number) => {
  if (!confirm('确定要删除这个里程碑吗？')) return
  try {
    await api.delete(`/side-projects/${projectId.value}/milestones/${milestoneId}`)
    notification.success('删除成功')
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('删除失败: ' + (e.message || '未知错误'))
  }
}

// 里程碑状态变更
const handleMilestoneStatusChange = async (milestoneId: number, status: number) => {
  try {
    await api.put(`/side-projects/${projectId.value}/milestones/${milestoneId}`, { status })
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('更新失败: ' + (e.message || '未知错误'))
  }
}

// 保存沟通记录
const handleSaveLog = async () => {
  saving.value = true
  try {
    await api.post(`/side-projects/${projectId.value}/logs`, logForm.value)
    notification.success('保存成功')
    showLogModal.value = false
    resetLogForm()
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('保存失败: ' + (e.message || '未知错误'))
  } finally {
    saving.value = false
  }
}

// 删除沟通记录
const handleDeleteLog = async (logId: number) => {
  if (!confirm('确定要删除这条沟通记录吗？')) return
  try {
    await api.delete(`/side-projects/${projectId.value}/logs/${logId}`)
    notification.success('删除成功')
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('删除失败: ' + (e.message || '未知错误'))
  }
}

// 打开附件
const handleOpenAttachment = (url: string) => {
  if (typeof window !== 'undefined') {
    window.open(url, '_blank')
  }
}

// 删除附件
const handleDeleteAttachment = async (attachmentId: number) => {
  if (!confirm('确定要删除这个附件吗？')) return
  try {
    await api.delete(`/side-projects/${projectId.value}/attachments/${attachmentId}`)
    notification.success('删除成功')
    await fetchProjectDetail()
  } catch (e: any) {
    notification.error('删除失败: ' + (e.message || '未知错误'))
  }
}

// 返回上一页
const handleBack = () => {
  // 检查路由参数中是否有来源页面
  const from = route.query.from as string
  if (from === 'kanban') {
    router.push('/admin/side-projects/kanban')
  } else if (from === 'list') {
    router.push('/admin/side-projects')
  } else {
    // 如果没有指定来源，默认返回到看板视图（因为通常从看板进入）
    router.push('/admin/side-projects/kanban')
  }
}

// 编辑项目
const handleEdit = () => {
  router.push(`/admin/side-projects?edit=${projectId.value}`)
}

// 重置表单
const resetTaskForm = () => {
  taskForm.value = {
    title: '',
    description: '',
    status: 0,
    priority: 1,
    dueAt: null,
    estHours: null
  }
}

const resetMilestoneForm = () => {
  milestoneForm.value = {
    title: '',
    dueAt: null,
    status: 0,
    notes: ''
  }
}

const resetLogForm = () => {
  logForm.value = {
    channel: '',
    content: '',
    nextTodo: ''
  }
}

// 工具函数
const getStatusText = (status: number): string => {
  switch (status) {
    case 0: return '进行中'
    case 1: return '已完成'
    case 2: return '待付款'
    case 3: return '已取消'
    default: return '未知'
  }
}

const getStageTagType = (stage: string): 'default' | 'success' | 'error' | 'warning' | 'info' | 'primary' => {
  switch (stage) {
    case '已完成': return 'success'
    case '卡住': return 'error'
    case '进行中': return 'info'
    case '待验收': return 'warning'
    default: return 'default'
  }
}

const getPriorityTagType = (priority?: number): 'default' | 'success' | 'error' | 'warning' | 'info' | 'primary' => {
  if (!priority) return 'default'
  switch (priority) {
    case 3: return 'error'
    case 2: return 'warning'
    case 1: return 'info'
    default: return 'default'
  }
}

const getPriorityText = (priority?: number): string => {
  if (!priority) return ''
  switch (priority) {
    case 3: return '紧急'
    case 2: return '高'
    case 1: return '中'
    default: return '低'
  }
}

const getProgressStatus = (progress?: number): 'default' | 'success' | 'error' | 'warning' => {
  if (!progress) return 'default'
  if (progress >= 100) return 'success'
  if (progress >= 50) return 'default'
  return 'warning'
}

const formatDate = (dateStr: string | null | undefined): string => {
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

const formatDateTime = (dateStr: string | null | undefined): string => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return dateStr
  }
}

const formatNumber = (num: number): string => {
  if (typeof num !== 'number' || isNaN(num)) return '0'
  return num.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

onMounted(() => {
  fetchProjectDetail()
})
</script>

<style scoped>
.project-detail-page {
  padding: var(--spacing-xl);
}

.loading-container,
.error-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-xl);
}

.header-left {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.page-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  margin: 0;
}

.overview-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
}

.overview-card {
  min-height: 150px;
}

.card-item {
  display: flex;
  justify-content: space-between;
  padding: var(--spacing-sm) 0;
  border-bottom: 1px solid var(--color-border-subtle);
}

.card-item:last-child {
  border-bottom: none;
}

.card-item .label {
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
}

.card-item .value {
  font-weight: 500;
}

.card-item .value.error {
  color: var(--color-danger);
}

.progress-text {
  margin-left: var(--spacing-sm);
  font-size: var(--font-size-sm);
}

.detail-section {
  margin-bottom: var(--spacing-xl);
}

.detail-section h3 {
  font-size: var(--font-size-lg);
  font-weight: 600;
  margin-bottom: var(--spacing-md);
}

.content-box {
  padding: var(--spacing-md);
  background: var(--color-bg-elevated);
  border-radius: var(--radius-md);
  white-space: pre-wrap;
  min-height: 100px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
}

.task-filters {
  margin-bottom: var(--spacing-md);
}

.task-item,
.milestone-item {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  margin-bottom: var(--spacing-sm);
}

.task-content,
.milestone-content {
  flex: 1;
}

.task-title,
.milestone-title {
  font-weight: 600;
  margin-bottom: var(--spacing-xs);
}

.task-meta,
.milestone-meta {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-xs);
}

.task-description {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  margin-top: var(--spacing-xs);
}

.task-actions,
.milestone-actions {
  display: flex;
  gap: var(--spacing-xs);
}

.log-item {
  padding: var(--spacing-md);
  background: var(--color-bg-elevated);
  border-radius: var(--radius-md);
}

.log-header {
  margin-bottom: var(--spacing-sm);
}

.log-content {
  margin-bottom: var(--spacing-sm);
  white-space: pre-wrap;
}

.log-todo {
  padding: var(--spacing-sm);
  background: rgba(59, 130, 246, 0.1);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-sm);
  margin-top: var(--spacing-sm);
}

.attachment-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-sm);
}

.empty-state {
  text-align: center;
  padding: var(--spacing-3xl);
  color: var(--color-text-muted);
}
</style>

