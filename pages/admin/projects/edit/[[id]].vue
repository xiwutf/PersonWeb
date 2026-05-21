<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">
        {{ isEdit ? '编辑项目' : '新建项目' }}
      </h1>
      <NuxtLink to="/admin/projects" class="btn-secondary">
        取消
      </NuxtLink>
    </div>

    <div class="card p-6">
      <form class="space-y-6" @submit.prevent>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="form-group">
            <label class="form-label">项目名称</label>
            <input v-model="form.title" type="text" class="form-input" placeholder="Project Name" />
          </div>
          <div class="form-group">
             <label class="form-label">状态</label>
             <select v-model="form.status" class="form-select">
               <option value="Active">进行中 (Active)</option>
               <option value="Completed">已完成 (Completed)</option>
               <option value="Archived">已归档 (Archived)</option>
             </select>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">简短描述</label>
          <textarea v-model="form.description" class="form-textarea h-20" placeholder="一句话介绍项目..."></textarea>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="form-group">
            <label class="form-label">GitHub URL</label>
            <input v-model="form.githubUrl" type="text" class="form-input" placeholder="https://github.com/username/repo" />
            <p class="text-xs text-gray-500 mt-1">用于获取 Commit 统计数据 (格式: https://github.com/owner/repo)</p>
          </div>
          <div class="form-group">
            <label class="form-label">演示 URL</label>
            <input v-model="form.demoUrl" type="text" class="form-input" placeholder="https://demo.com" />
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">封面图</label>
          <div class="flex gap-2">
            <input v-model="form.coverUrl" type="text" class="form-input flex-1" placeholder="输入 URL 或上传" />
            <button @click="triggerUpload" type="button" class="btn-secondary">上传</button>
            <input ref="fileInput" type="file" class="hidden" accept="image/*" @change="handleUpload" />
          </div>
        </div>

        <div class="form-group">
           <label class="form-label">技术栈 (用逗号分隔)</label>
           <input v-model="techStackInput" type="text" class="form-input" placeholder="Vue, Nuxt, TypeScript" />
        </div>

        <div class="form-group">
          <label class="form-label">详细介绍 (HTML)</label>
          <p class="text-xs text-gray-500 mb-2">
            由 AI 生成后直接粘贴即可。请使用语义标签（h2、h3、p、ul、blockquote 等），不要写内联 style。
          </p>
          <div class="flex gap-2 mb-2">
            <button
              type="button"
              class="btn-secondary text-sm"
              :class="{ 'opacity-60': contentEditorTab !== 'edit' }"
              @click="contentEditorTab = 'edit'"
            >
              编辑
            </button>
            <button
              type="button"
              class="btn-secondary text-sm"
              :class="{ 'opacity-60': contentEditorTab !== 'preview' }"
              @click="contentEditorTab = 'preview'"
            >
              预览
            </button>
          </div>
          <textarea
            v-if="contentEditorTab === 'edit'"
            v-model="form.contentHtml"
            class="form-textarea h-64 font-mono"
            placeholder="<h2>项目背景</h2><p>...</p>"
          ></textarea>
          <div
            v-else
            class="form-textarea h-64 overflow-y-auto prose max-w-none admin-project-html-preview"
            v-html="contentPreviewHtml"
          ></div>
        </div>

        <div class="flex justify-end gap-4">
          <button @click="handleSave" type="button" class="btn-primary" :disabled="saving">
            {{ saving ? '保存中...' : '保存项目' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project, ProjectRequest } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'
import { resolveProjectBodyHtml } from '~/composables/useProjectContent'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const route = useRoute()
const api = useApi()

const loading = ref(false)
const saving = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const techStackInput = ref('')
const contentEditorTab = ref<'edit' | 'preview'>('edit')

const form = ref({
  id: '',
  title: '',
  description: '',
  coverUrl: '',
  githubUrl: '',
  demoUrl: '',
  status: 'Active',
  techStack: [] as string[],
  contentHtml: '',
  date: ''
})

const isEdit = computed(() => !!route.params.id)

const contentPreviewHtml = computed(() =>
  resolveProjectBodyHtml({
    contentHtml: form.value.contentHtml,
    content: ''
  })
)

// 加载详情
const fetchProject = async (id: string) => {
  loading.value = true
  const { error } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    const res = await api.get<Project>(`/Projects/${id}`)
    form.value = {
      ...res,
      contentHtml: res.contentHtml || res.content || ''
    }
    // Handle TechStack string from API
    techStackInput.value = typeof res.techStack === 'string' ? res.techStack : (Array.isArray(res.techStack) ? res.techStack.join(',') : '')
  } catch (e: unknown) {
    handleError(e, '加载失败')
    router.push('/admin/projects')
  } finally {
    loading.value = false
  }
}

// 上传图片
const triggerUpload = () => {
  fileInput.value?.click()
}

const handleUpload = async (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file) return

  const formData = new FormData()
  formData.append('file', file)

  const { success } = useNotification()
  const { handleError } = useErrorHandler()

  try {
    const res = await api.post<{ url: string }>('/media/upload', formData)
    form.value.coverUrl = res.url
    success('上传成功')
  } catch (e: unknown) {
    handleError(e, '上传失败')
  }
}

// 保存
const handleSave = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!form.value.title || !form.value.title.trim()) {
    warning('请输入项目名称')
    return
  }

  if (!form.value.description || !form.value.description.trim()) {
    warning('请输入项目描述')
    return
  }

  saving.value = true
  try {
    // 处理技术栈
    const techStackArray = techStackInput.value.split(/[,，]/).map(s => s.trim()).filter(s => s)
    const techStackString = techStackArray.length > 0 ? techStackArray.join(',') : undefined
    
    const payload: ProjectRequest = {
      title: form.value.title.trim(),
      description: form.value.description.trim(),
      coverUrl: form.value.coverUrl?.trim() || undefined,
      demoUrl: form.value.demoUrl?.trim() || undefined,
      githubUrl: form.value.githubUrl?.trim() || undefined,
      status: form.value.status,
      techStack: techStackString,
      contentHtml: form.value.contentHtml?.trim() || undefined,
      content: undefined
    }
    
    if (isEdit.value && form.value.id) {
      await api.put(`/Projects/${form.value.id}`, payload)
    } else {
      await api.post('/Projects', payload)
    }
    success('保存成功')
    router.push('/admin/projects')
  } catch (e: unknown) {
    handleError(e, '保存失败')
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  if (route.params.id) {
    await fetchProject(route.params.id as string)
  }
})
</script>
