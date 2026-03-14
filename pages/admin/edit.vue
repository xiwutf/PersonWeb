<template>
  <div class="container mx-auto px-4 py-8 h-screen flex flex-col">
    <div class="flex justify-between items-center mb-4">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white)">
        {{ isEditing ? 'Edit Article' : 'New Article' }}
      </h1>
      <div class="space-x-4">
        <NuxtLink to="/admin" class="text-gray-600 hover:text-gray-900 dark:text-gray-400 dark:hover:text-var(--color-bg-light, white)">
          Cancel
        </NuxtLink>
        <button 
          @click="save" 
          class="bg-blue-500 hover:bg-blue-600 text-var(--color-bg-light, white) font-bold py-2 px-4 rounded"
          :disabled="saving"
        >
          {{ saving ? 'Saving...' : 'Save' }}
        </button>
      </div>
    </div>

    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Filename (e.g. blog/my-post.md)</label>
      <input 
        v-model="filename" 
        type="text" 
        class="w-full p-2 border rounded dark:bg-gray-700 dark:border-gray-600 dark:text-var(--color-bg-light, white)"
        :disabled="isEditing"
        placeholder="folder/filename.md"
      >
    </div>

    <div class="flex-1">
      <textarea 
        v-model="content" 
        class="w-full h-full p-4 border rounded font-mono text-sm dark:bg-gray-800 dark:border-gray-700 dark:text-gray-300 resize-none focus:outline-none focus:ring-2 focus:ring-blue-500"
        placeholder="# Title..."
      ></textarea>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  middleware: 'admin-auth'
})

const route = useRoute()
const router = useRouter()

const filename = ref('')
const content = ref('')
const saving = ref(false)
const isEditing = computed(() => !!route.query.filename)

onMounted(async () => {
  if (isEditing.value) {
    filename.value = route.query.filename as string
    const { handleError } = useErrorHandler()
    
    try {
      const res = await $fetch<{ content: string }>(`/api/admin/articles?action=read&filename=${filename.value}`)
      content.value = res.content
    } catch (e: unknown) {
      handleError(e, '加载文件失败')
      router.push('/admin')
    }
  }
})

const save = async () => {
  const { warning, success } = useNotification()
  const { handleError } = useErrorHandler()
  
  if (!filename.value) {
    warning('文件名不能为空')
    return
  }
  
  saving.value = true
  try {
    await $fetch('/api/admin/articles', {
      method: isEditing.value ? 'PUT' : 'POST',
      body: {
        filename: filename.value,
        content: content.value
      }
    })
    success('保存成功')
    router.push('/admin')
  } catch (e: unknown) {
    handleError(e, '保存失败')
  } finally {
    saving.value = false
  }
}
</script>

