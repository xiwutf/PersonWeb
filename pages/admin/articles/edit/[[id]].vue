<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">编辑文章</h1>
      <NuxtLink to="/admin/articles" class="text-gray-600 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200">
        取消
      </NuxtLink>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <form class="space-y-6" @submit.prevent>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">文章标题</label>
          <input v-model="form.title" type="text" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="输入标题" />
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

        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">内容 (Markdown)</label>
          <textarea v-model="form.contentMd" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 h-96 font-mono bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="# Hello World..."></textarea>
        </div>

        <div class="flex justify-end gap-4">
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
const categories = ref<any[]>([])

const form = ref({
  id: 0,
  title: '',
  slug: '',
  summary: '',
  contentMd: '',
  coverUrl: '',
  categoryId: 0,
  status: 0, // 0: 草稿
  tags: [] as string[]
})

const isEdit = computed(() => !!route.params.id)

const fetchCategories = async () => {
  try {
    const res = await api.get<any[]>('/categories')
    categories.value = res
  } catch (e) {
    console.error('Failed to fetch categories', e)
  }
}

// 加载文章详情
const fetchArticle = async (id: string) => {
  loading.value = true
  try {
    const res = await api.get<any>(`/articles/${id}`)
    form.value = {
      ...res,
      categoryId: res.categoryId || 0,
      tags: [] 
    }
  } catch (e) {
    console.error(e)
    alert('加载文章失败')
    router.push('/admin/articles')
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

  try {
    const res = await api.post<any>('/media/upload', formData)
    form.value.coverUrl = res.url
  } catch (e: any) {
    alert(e.message || '上传失败')
  }
}

// 保存文章
const handleSave = async (status: number) => {
  if (!form.value.title) {
    alert('请输入标题')
    return
  }

  saving.value = true
  try {
    const payload = {
      ...form.value,
      status
    }
    
    await api.post('/articles', payload)
    alert('保存成功')
    router.push('/admin/articles')
  } catch (e: any) {
    alert(e.message || '保存失败')
  } finally {
    saving.value = false
  }
}

onMounted(async () => {
  await fetchCategories()
  if (route.params.id) {
    await fetchArticle(route.params.id as string)
  }
})
</script>

