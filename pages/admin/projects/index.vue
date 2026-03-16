<template>
  <div class="projects-page">
    <!-- 使用 ListPage Pattern 组件 -->
    <ListPage
      title="项目管理"
      description="管理展示项目的信息、状态和访问统计"
      :columns="internalColumns"
      :data="projects"
      :loading="loading"
      :empty-config="{
        icon: 'fas fa-folder-open',
        text: '暂无项目',
        description: '点击「新建项目」开始创建您的第一个展示项目'
      }"
    >
      <!-- 头部操作按钮区域 -->
      <template #header-actions>
        <n-space :size="12">
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
        </n-space>
      </template>
    </ListPage>

    <!-- AI 生成 Dialog -->
    <n-modal
      v-model="showAiDialog"
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
          <n-input v-model="aiForm.name" placeholder="项目名称" />
        </n-form-item>
        <n-form-item label="技术栈">
          <n-input
            v-model="aiForm.techStackText"
            placeholder="用逗号分隔，例如：Vue3, TypeScript, Node.js"
          />
        </n-form-item>
        <n-form-item label="用途">
          <n-input
            v-model="aiForm.usage"
            type="textarea"
            :rows="3"
            placeholder="项目的主要用途和功能"
          />
        </n-form-item>
        <n-form-item label="目标用户">
          <n-input
            v-model="aiForm.targetAudience"
            placeholder="例如：前端开发者、产品经理等"
          />
        </n-form-item>
        <n-form-item label="价格提示">
          <n-input
            v-model="aiForm.priceHint"
            placeholder="例如：免费、付费、定制开发等"
          />
        </n-form-item>
        <n-form-item label="额外说明">
          <n-input
            v-model="aiForm.extraNotes"
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
import { h } from 'vue'
import { NButton, NModal, NForm, NFormItem, NInput, NSpace, NTag, NImage } from 'naive-ui'
import type { Project } from '~/types/api'
import { useSafeMessage } from '~/composables/useNaiveUI'
import { useErrorHandler } from '~/composables/useErrorHandler'
import ListPage from '~/components/admin/patterns/ListPage.vue'
import type { DataTableColumns } from 'naive-ui'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
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

// 表格列配置
const internalColumns: DataTableColumns<Project> = [
  {
    title: '项目名称',
    key: 'title',
    width: 300,
    render(row) {
      return h('div', { class: 'project-info' }, [
        h(NImage, {
          src: row.coverUrl || 'https://placehold.co/100',
          alt: row.title,
          width: 40,
          height: 40,
          objectFit: 'cover',
          style: {
            borderRadius: '50%',
            marginRight: '12px',
            border: '2px solid rgba(255,255,255,0.1)'
          }
        }),
        h('div', { class: 'project-details' }, [
          h('div', { class: 'project-title' }, row.title),
          h('div', { class: 'project-desc' }, (row.description?.substring(0, 30) + '...') || '-')
        ])
      ])
    }
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render(row) {
      return h(NTag, {
        type: row.status === 'Active' ? 'success' : 'default',
        bordered: false
      }, { default: () => row.status })
    }
  },
  {
    title: '访问量',
    key: 'viewCount',
    width: 100,
    render(row) {
      return h('div', { class: 'view-count' }, [
        h('i', { class: 'fas fa-eye', style: { marginRight: '4px', color: 'var(--color-text-muted)' } }),
        h('span', {}, row.viewCount || 0)
      ])
    }
  },
  {
    title: 'GitHub',
    key: 'githubUrl',
    width: 100,
    render(row) {
      if (row.githubUrl) {
        return h('a', {
          href: row.githubUrl,
          target: '_blank',
          class: 'btn-link btn-link-blue'
        }, 'Repo')
      }
      return h('span', { class: 'text-muted' }, '-')
    }
  },
  {
    title: '创建日期',
    key: 'createdAt',
    width: 120,
    render(row) {
      return new Date(row.createdAt).toLocaleDateString()
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    fixed: 'right',
    render(row) {
      return h('div', { class: 'action-buttons' }, [
        h('button', {
          onClick: () => handleAiGenerate(row),
          class: 'btn-link btn-link-purple',
          title: 'AI 生成展示文案'
        }, [h('i', { class: 'fas fa-magic' }), ' AI']),
        h('button', {
          onClick: () => router.push(`/admin/projects/edit/${row.id}`),
          class: 'btn-link btn-link-blue'
        }, '编辑'),
        h('button', {
          onClick: () => handleDelete(row.id),
          class: 'btn-link btn-link-red'
        }, '删除')
      ])
    }
  }
]

const fetchProjects = async () => {
  loading.value = true
  try {
    const res = await api.get<Project[]>('/Projects')
    projects.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch projects:', e)
    }
    projects.value = []
    try {
      message.error('加载项目列表失败')
    } catch (msgError) {
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
      await fetchProjects()
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

/* 项目信息 */
.project-info {
  display: flex;
  align-items: center;
}

.project-details {
  flex: 1;
}

.project-title {
  font-size: var(--text-sm);
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: 4px;
}

.project-desc {
  font-size: var(--text-xs);
  color: var(--color-text-muted);
  font-weight: 500;
}

/* 访问量 */
.view-count {
  display: flex;
  align-items: center;
  gap: 4px;
  color: var(--color-text-main);
  font-weight: 500;
}

.text-muted {
  color: var(--color-text-muted);
  font-weight: 500;
}

/* 操作按钮 */
.action-buttons {
  display: flex;
  gap: 8px;
  align-items: center;
}

.btn-link {
  background: none;
  border: none;
  cursor: pointer;
  text-decoration: none;
  transition: color 0.2s ease;
  font-size: var(--text-sm);
}

.btn-link-blue {
  color: var(--color-primary);
}

.btn-link-blue:hover {
  color: var(--color-primary-hover);
}

.btn-link-red {
  color: var(--color-error);
}

.btn-link-red:hover {
  color: var(--color-error-hover);
}

.btn-link-purple {
  color: var(--color-purple);
}

.btn-link-purple:hover {
  color: var(--color-purple-hover);
}
</style>
