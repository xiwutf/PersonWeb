<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">编辑文章</h1>
      <NuxtLink to="/admin/articles" class="px-4 py-2 text-gray-600 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200">
        取消
      </NuxtLink>
    </div>

    <div v-if="loading" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 text-center">
      <p class="text-gray-500 dark:text-gray-400">加载中...</p>
    </div>

    <div v-else class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <form class="space-y-6" @submit.prevent>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">文章标题 <span class="text-red-500">*</span></label>
          <input 
            v-model="form.title" 
            type="text" 
            class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" 
            placeholder="输入标题" 
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">URL Slug</label>
          <div class="flex gap-2">
            <input 
              v-model="form.slug" 
              type="text" 
              class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" 
              placeholder="article-slug-url" 
            />
            <button 
              @click="generateSlugFromTitle" 
              type="button" 
              class="px-4 py-2 bg-gray-100 dark:bg-gray-700 rounded hover:bg-gray-200 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 text-sm whitespace-nowrap"
            >
              自动生成
            </button>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">分类</label>
            <select v-model="form.categoryId" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
              <option :value="0">无分类</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">封面图</label>
            <div class="flex gap-2">
              <input v-model="form.coverUrl" type="text" class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="输入 URL 或上传" />
              <button @click="triggerUpload" type="button" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 rounded hover:bg-gray-200 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300">上传</button>
              <input ref="fileInput" type="file" class="hidden" accept="image/*" @change="handleUpload" />
            </div>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">摘要</label>
          <textarea v-model="form.summary" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 h-20 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="文章简短描述..."></textarea>
        </div>

        <!-- 编辑器区域 -->
        <div class="flex flex-col">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">内容 (Markdown)</label>
          <MarkdownEditor 
            v-model="form.contentMd" 
            placeholder="开始编写你的文章内容...支持 Markdown 语法，可直接粘贴图片自动上传"
            height="600px"
          />
        </div>

        <div class="flex justify-end gap-4 pt-4 border-t border-gray-100 dark:border-gray-700">
          <button @click="handleSave(0)" type="button" class="px-6 py-2 border border-gray-300 dark:border-gray-600 rounded text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700" :disabled="saving">
            存草稿
          </button>
          <button @click="handleSave(1)" type="button" class="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700" :disabled="saving">
            {{ saving ? '保存中...' : '发布文章' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ArticleRequest, Category, Article } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const route = useRoute()
const api = useApi()

const loading = ref(true)
const saving = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const categories = ref<Category[]>([])

const form = ref({
  id: 0,
  title: '',
  slug: '',
  summary: '',
  contentMd: '',
  coverUrl: '',
  categoryId: 0,
  status: 0,
  tags: [] as string[]
})

const fetchCategories = async () => {
  try {
    const res = await api.get<Category[]>('/Categories')
    categories.value = res
  } catch (e) {
    // 静默失败，不影响页面显示
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch categories', e)
    }
  }
}

// 加载文章详情
const fetchArticle = async (id: string) => {
  loading.value = true
  try {
    const res = await api.get<Article>(`/Articles/${id}`)
    if (res) {
      form.value = {
        id: res.id || 0,
        title: res.title || '',
        slug: res.slug || '',
        summary: res.summary || '',
        contentMd: res.contentMd || '',
        coverUrl: res.coverUrl || '',
        categoryId: res.categoryId || 0,
        status: res.status || 0,
        tags: res.tags || []
      }
    } else {
      throw new Error('文章不存在')
    }
  } catch (e: unknown) {
    const { handleError } = useErrorHandler()
    handleError(e, '加载文章失败')
    router.push('/admin/articles')
  } finally {
    loading.value = false
  }
}

// 自动生成 Slug（从标题）
const generateSlugFromTitle = () => {
  const { warning } = useNotification()
  
  if (!form.value.title) {
    warning('请先输入标题')
    return
  }
  
  let slug = form.value.title
    .toLowerCase()
    .trim()
    .replace(/[\s\u4e00-\u9fa5\u3000-\u303f\uff00-\uffef]+/g, '-')
    .replace(/[^a-z0-9-]/g, '-')
    .replace(/-+/g, '-')
    .replace(/^-+|-+$/g, '')
  
  if (!slug || slug === '-') {
    slug = `article-${Date.now()}`
  }
  
  form.value.slug = slug
}

// 上传图片
const triggerUpload = () => {
  fileInput.value?.click()
}

const handleUpload = async (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file) return

  const { warning, success, error } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!file.type.startsWith('image/')) {
    warning('请选择图片文件')
    return
  }

  if (file.size > 5 * 1024 * 1024) {
    warning('图片大小不能超过 5MB')
    return
  }

  const formData = new FormData()
  formData.append('file', file)

  try {
    const res = await api.post<{ url: string }>('/Media/upload', formData)
    if (res && res.url) {
      form.value.coverUrl = res.url
      success('上传成功')
    } else {
      error('上传失败：未返回有效 URL')
    }
  } catch (e: unknown) {
    handleError(e, '上传失败，请检查网络连接或稍后重试')
  }
}

// 保存文章
const handleSave = async (status: number) => {
  const { warning } = useNotification()
  
  if (!form.value.title || !form.value.title.trim()) {
    warning('请输入文章标题')
    return
  }

  if (!form.value.slug || !form.value.slug.trim()) {
    generateSlugFromTitle()
  }

  const slugPattern = /^[a-z0-9]+(?:-[a-z0-9]+)*$/
  if (form.value.slug && !slugPattern.test(form.value.slug)) {
    warning('URL Slug 格式不正确：只能包含小写字母、数字和连字符，且不能以连字符开头或结尾')
    return
  }

  saving.value = true
  try {
    const payload = {
      id: form.value.id,
      title: form.value.title.trim(),
      slug: form.value.slug.trim() || null,
      summary: form.value.summary?.trim() || null,
      contentMd: form.value.contentMd?.trim() || null,
      contentHtml: null,
      coverUrl: form.value.coverUrl?.trim() || null,
      categoryId: form.value.categoryId || null,
      status: status,
      tags: form.value.tags || []
    }
    
    const res = await api.post('/Articles', payload)
    
    const { success } = useNotification()
    success(status === 1 ? '文章发布成功' : '草稿保存成功')
    router.push('/admin/articles')
  } catch (e: unknown) {
    const { handleError } = useErrorHandler()
    handleError(e, '保存失败，请稍后重试')
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  await fetchCategories()
  const articleId = route.params.id as string
  if (articleId) {
    await fetchArticle(articleId)
  } else {
    const { error } = useNotification()
    error('文章ID不存在')
    router.push('/admin/articles')
  }
})
</script>

