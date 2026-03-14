<template>
  <div class="projects-page">
    <div class="page-header">
      <h1 class="page-title">项目管理</h1>
      <div class="header-actions">
        <n-button type="success" secondary @click="() => router.push('/admin/projects/stats')">
          <template #icon>
            <i class="fas fa-chart-bar"></i>
          </template>
          访问统计
        </n-button>
        <n-button type="primary" @click="() => router.push('/admin/projects/edit')">
          <template #icon>
            <i class="fas fa-plus"></i>
          </template>
          新建项目
        </n-button>
      </div>
    </div>

    <div class="table-container">
      <div v-if="loading" class="table-loading">
        加载中...
      </div>
      <div v-else-if="projects.length === 0" class="table-empty">
        暂无项目
      </div>
      <table v-else class="data-table">
        <thead class="table-header">
          <tr>
            <th>项目名称</th>
            <th>状态</th>
            <th>访问量</th>
            <th>GitHub</th>
            <th>创建日期</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="project in projects" :key="project.id" class="table-row">
            <td class="table-cell">
              <div class="project-info">
                <img 
                  :src="project.coverUrl || 'https://placehold.co/100'" 
                  :alt="project.title"
                  class="project-avatar"
                />
                <div class="project-details">
                  <div class="project-title">{{ project.title }}</div>
                  <div class="project-desc">{{ project.description?.substring(0, 30) + '...' || '-' }}</div>
                </div>
              </div>
            </td>
            <td class="table-cell">
              <span 
                class="tag"
                :class="project.status === 'Active' ? 'tag-success' : 'tag-default'"
              >
                {{ project.status }}
              </span>
            </td>
            <td class="table-cell">
              <div class="view-count">
                <i class="fas fa-eye"></i>
                <span>{{ project.viewCount || 0 }}</span>
              </div>
            </td>
            <td class="table-cell">
              <a 
                v-if="project.githubUrl" 
                :href="project.githubUrl" 
                target="_blank"
                class="btn-link btn-link-blue"
              >
                Repo
              </a>
              <span v-else class="text-muted">-</span>
            </td>
            <td class="table-cell">
              {{ new Date(project.createdAt).toLocaleDateString() }}
            </td>
            <td class="table-cell">
              <div class="action-buttons">
                <button 
                  @click="handleAiGenerate(project)" 
                  class="btn-link btn-link-purple"
                  title="AI 生成展示文案"
                >
                  <i class="fas fa-magic"></i> AI
                </button>
                <button 
                  @click="router.push(`/admin/projects/edit/${project.id}`)" 
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
    </div>

    <!-- AI 生成 Dialog -->
    <n-modal
      v-model:show="showAiDialog"
      preset="dialog"
      title="AI 生成展示文案"
      positive-text="生成"
      negative-text="取消"
      @positive-click="handleGenerateConfirm"
      :loading="generating"
    >
      <n-form
        ref="aiFormRef"
        :model="aiForm"
        label-placement="left"
        label-width="100"
        style="margin-top: 20px;"
      >
        <n-form-item label="项目名称">
          <n-input v-model:value="aiForm.name" placeholder="项目名称" />
        </n-form-item>
        <n-form-item label="技术栈">
          <n-input
            v-model:value="aiForm.techStackText"
            placeholder="用逗号分隔，例如：Vue3, TypeScript, Node.js"
          />
        </n-form-item>
        <n-form-item label="用途">
          <n-input
            v-model:value="aiForm.usage"
            type="textarea"
            :rows="3"
            placeholder="项目的主要用途和功能"
          />
        </n-form-item>
        <n-form-item label="目标用户">
          <n-input
            v-model:value="aiForm.targetAudience"
            placeholder="例如：前端开发者、产品经理等"
          />
        </n-form-item>
        <n-form-item label="价格提示">
          <n-input
            v-model:value="aiForm.priceHint"
            placeholder="例如：免费、付费、定制开发等"
          />
        </n-form-item>
        <n-form-item label="额外说明">
          <n-input
            v-model:value="aiForm.extraNotes"
            type="textarea"
            :rows="2"
            placeholder="补充说明，帮助 AI 更好地理解项目"
          />
        </n-form-item>
      </n-form>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { NButton, NModal, NForm, NFormItem, NInput } from 'naive-ui'
import type { Project } from '~/types/api'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，避免 Naive UI 组件在服务端渲染时出错
})

const router = useRouter()
const api = useApi()
const { handleError } = useErrorHandler()

// 使用安全的 Naive UI composables，避免 provider 未挂载时的错误
const message = useSafeMessage()

const projects = ref<Project[]>([])
const loading = ref(false)

// AI 生成相关
const showAiDialog = ref(false)
const generating = ref(false)
const aiFormRef = ref<any>(null)
const currentProject = ref<Project | null>(null)
const aiForm = ref({
  name: '',
  techStackText: '',
  usage: '',
  targetAudience: '',
  priceHint: '',
  extraNotes: ''
})

const fetchProjects = async () => {
  loading.value = true
  try {
    // 后端返回格式: { code: 0, data: List<Project> }
    // useApi 已经处理了响应格式，直接返回 data (即 List<Project>)
    const res = await api.get<Project[]>('/Projects')
    projects.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch projects:', e)
    }
    projects.value = []
    // 使用安全的 message 调用，避免阻塞
    try {
      message.error('加载项目列表失败')
    } catch (msgError) {
      // 如果 message 调用失败，不影响其他功能
      console.warn('Message error:', msgError)
    }
  } finally {
    loading.value = false
  }
}

const handleDelete = async (id: string) => {
  if (!confirm('确定要删除这个项目吗？')) return
  
  try {
    await api.del(`/Projects/${id}`)
    // 使用安全的 message 调用
    try {
      message.success('删除成功')
    } catch (msgError) {
      console.warn('Message error:', msgError)
    }
    await fetchProjects()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

const handleAiGenerate = (project: Project) => {
  currentProject.value = project
  aiForm.value = {
    name: project.title || '',
    techStackText: project.techStack ? (typeof project.techStack === 'string' ? project.techStack : JSON.stringify(project.techStack)) : '',
    usage: project.description || '',
    targetAudience: '',
    priceHint: '',
    extraNotes: ''
  }
  showAiDialog.value = true
}

const handleGenerateConfirm = async () => {
  if (!currentProject.value) return false

  generating.value = true
  try {
    const techStack = aiForm.value.techStackText
      ? aiForm.value.techStackText.split(',').map(s => s.trim()).filter(s => s)
      : null

    const res = await api.post('/ai/demo/describe', {
      projectId: currentProject.value.id,
      name: aiForm.value.name || undefined,
      techStack: techStack || undefined,
      usage: aiForm.value.usage || undefined,
      targetAudience: aiForm.value.targetAudience || undefined,
      priceHint: aiForm.value.priceHint || undefined,
      extraNotes: aiForm.value.extraNotes || undefined
    })

    if (res && res.success) {
      message.success('展示文案生成成功！已自动保存到项目')
      showAiDialog.value = false
      await fetchProjects() // 刷新列表
      return true
    } else {
      message.error(res?.errorMessage || '生成失败')
      return false
    }
  } catch (e: any) {
    console.error('生成展示文案失败:', e)
    message.error(e.response?.data?.message || e.message || '生成失败')
    return false
  } finally {
    generating.value = false
  }
}

onMounted(() => {
  fetchProjects()
})
</script>

<style scoped>
.projects-page {
  width: 100%;
}

.header-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

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

/* 项目信息 */
.project-info {
  display: flex;
  align-items: center;
}

.project-avatar {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  margin-right: 0.75rem;
  object-fit: cover;
  border: 2px solid var(--color-border-subtle, rgba(255, 255, 255, 0.1));
}

.project-details {
  flex: 1;
}

.project-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: 0.25rem;
}

.project-desc {
  font-size: 0.75rem;
  color: var(--color-text-muted);
  font-weight: 500;
}

/* 标签样式 - 提高文字对比度 */
.tag {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 600;
}

.tag-success {
  background: var(--color-success-soft, rgba(34, 197, 94, 0.2));
  border: 1px solid var(--color-success, rgba(34, 197, 94, 0.4));
  color: var(--color-success, var(--color-success));
}

.tag-default {
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  color: var(--color-text-main);
}

/* 访问量 */
.view-count {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-text-main);
  font-weight: 500;
}

.view-count i {
  color: var(--color-text-muted);
}

.text-muted {
  color: var(--color-text-muted);
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
  color: var(--color-primary, var(--color-primary-soft));
}

.btn-link-blue:hover {
  color: var(--color-primary-hover, var(--color-blue-400));
}

.btn-link-red {
  color: var(--color-error, var(--color-red-400));
}

.btn-link-red:hover {
  color: var(--color-error-hover, var(--color-red-300));
}

.btn-link-purple {
  color: var(--color-purple, var(--color-purple-400));
}

.btn-link-purple:hover {
  color: var(--color-purple-hover, var(--color-purple-300));
}
</style>
