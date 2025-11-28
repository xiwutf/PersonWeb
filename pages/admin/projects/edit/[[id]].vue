<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">编辑项目</h1>
      <NuxtLink to="/admin/projects" class="text-gray-600 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200">
        取消
      </NuxtLink>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <form class="space-y-6" @submit.prevent>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">项目名称</label>
            <input v-model="form.title" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="Project Name" />
          </div>
          <div>
             <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">状态</label>
             <select v-model="form.status" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
               <option value="Active">进行中 (Active)</option>
               <option value="Completed">已完成 (Completed)</option>
               <option value="Archived">已归档 (Archived)</option>
             </select>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">简短描述</label>
          <textarea v-model="form.description" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 h-20 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="一句话介绍项目..."></textarea>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">GitHub URL</label>
            <input v-model="form.githubUrl" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="https://github.com/username/repo" />
            <p class="text-xs text-gray-500 mt-1">用于获取 Commit 统计数据 (格式: https://github.com/owner/repo)</p>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">演示 URL</label>
            <input v-model="form.demoUrl" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="https://demo.com" />
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">封面图</label>
          <div class="flex gap-2">
            <input v-model="form.coverUrl" type="text" class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="输入 URL 或上传" />
            <button @click="triggerUpload" type="button" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 rounded hover:bg-gray-200 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300">上传</button>
            <input ref="fileInput" type="file" class="hidden" accept="image/*" @change="handleUpload" />
          </div>
        </div>

        <div>
           <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">技术栈 (用逗号分隔)</label>
           <input v-model="techStackInput" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="Vue, Nuxt, TypeScript" />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">详细介绍 (Markdown)</label>
          <textarea v-model="form.contentMd" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 h-64 font-mono bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="# Project Details..."></textarea>
        </div>

        <div class="flex justify-end gap-4">
          <button @click="handleSave" type="button" class="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700" :disabled="saving">
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

const form = ref({
  id: '',
  title: '',
  description: '',
  coverUrl: '',
  githubUrl: '',
  demoUrl: '',
  status: 'Active',
  techStack: [] as string[],
  contentMd: '',
  date: ''
})

const isEdit = computed(() => !!route.params.id)

// 加载详情
const fetchProject = async (id: string) => {
  loading.value = true
  const { error } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    const res = await api.get<Project>(`/Projects/${id}`)
    form.value = {
      ...res,
      contentMd: res.content || '' // Map Content to contentMd
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
  
  if (!form.value.title) {
    warning('请输入项目名称')
    return
  }

  saving.value = true
  try {
    // 处理技术栈
    const techStackArray = techStackInput.value.split(/[,，]/).map(s => s.trim()).filter(s => s)
    const techStackString = techStackArray.join(',')
    
    const payload: ProjectRequest = {
      title: form.value.title,
      description: form.value.description,
      coverUrl: form.value.coverUrl || undefined,
      demoUrl: form.value.demoUrl || undefined,
      githubUrl: form.value.githubUrl || undefined,
      status: form.value.status,
      techStack: techStackString,
      content: form.value.contentMd || undefined
    }
    
    if (isEdit.value) {
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
