<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">编辑文章</h1>
      <div class="flex gap-2">
        <NuxtLink :to="`/admin/articles/${route.params.id}/versions`" class="btn-secondary">
          版本历史
        </NuxtLink>
        <NuxtLink to="/admin/articles" class="btn-secondary">
          取消
        </NuxtLink>
      </div>
    </div>

    <div v-if="loading" class="card p-6 text-center">
      <p class="loading">加载中...</p>
    </div>

    <div v-else class="card p-6">
      <form class="space-y-6" @submit.prevent>
        <div class="form-group">
          <label class="form-label">文章标题 <span class="text-red-500">*</span></label>
          <input 
            v-model="form.title" 
            type="text" 
            class="form-input" 
            placeholder="输入标题" 
          />
        </div>

        <div class="form-group">
          <label class="form-label">URL Slug</label>
          <div class="flex gap-2">
            <input 
              v-model="form.slug" 
              type="text" 
              class="form-input flex-1" 
              placeholder="article-slug-url" 
            />
            <button 
              @click="generateSlugFromTitle" 
              type="button" 
              class="btn-secondary var(--color-bg-light, white)space-nowrap"
            >
              自动生成
            </button>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-6">
          <div class="form-group">
            <label class="form-label">分类</label>
            <select v-model="form.categoryId" class="form-select">
              <option :value="0">无分类</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">封面图</label>
            <div class="flex gap-2">
              <input v-model="form.coverUrl" type="text" class="form-input flex-1" placeholder="输入 URL 或上传" />
              <button @click="triggerUpload" type="button" class="btn-secondary">上传</button>
              <input ref="fileInput" type="file" class="hidden" accept="image/*" @change="handleUpload" />
            </div>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">摘要</label>
          <textarea v-model="form.summary" class="form-textarea h-20" placeholder="文章简短描述..."></textarea>
        </div>

        <!-- 正文编辑区：明显区块 + 说明，便于找到可填写位置 -->
        <div class="article-content-section">
          <div class="article-content-section-header">
            <label class="article-content-section-label">内容 (Markdown)</label>
            <span class="article-content-section-hint">左侧输入 Markdown，右侧实时预览</span>
          </div>
          <div class="article-content-section-editor">
            <AdminSimpleMarkdownEditor
              v-model="form.contentMd"
              placeholder="开始编写你的文章内容...支持 Markdown 语法（标题、列表、代码块等）"
              :min-rows="24"
              :max-rows="60"
            />
          </div>
        </div>

        <div class="flex justify-end gap-4 pt-4 border-t border-gray-100 dark:border-gray-700">
          <button @click="handleSave(0)" type="button" class="btn-secondary" :disabled="saving">
            存草稿
          </button>
          <button @click="handleSave(1)" type="button" class="btn-primary" :disabled="saving">
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

<style scoped>
/* 正文编辑区：独立区块，便于一眼找到「写内容」的位置（无边框，避免与编辑器重复嵌套） */
.article-content-section {
  margin-top: 1.5rem;
  padding: 1rem 1.25rem;
  background: var(--color-bg-elevated, var(--color-bg-card));
  border-radius: var(--radius-lg, 0.75rem);
}

.article-content-section-header {
  display: flex;
  flex-wrap: wrap;
  align-items: baseline;
  gap: 0.5rem 1rem;
  margin-bottom: 0.75rem;
}

.article-content-section-label {
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-text-main);
}

.article-content-section-hint {
  font-size: 0.8125rem;
  color: var(--color-text-muted);
}

.article-content-section-editor {
  border-radius: var(--radius-md, 0.5rem);
  overflow: hidden;
}

/* 深色主题下正文区块更明显 */
[data-theme='dark'] .article-content-section,
[data-theme='tech-blue'] .article-content-section,
[data-theme='forest'] .article-content-section,
[data-theme='hybrid-super-dark'] .article-content-section {
  background: rgba(255, 255, 255, 0.06);
}
</style>

