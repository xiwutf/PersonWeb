<template>
  <div class="container mx-auto px-4 py-8 h-screen flex flex-col">
    <div class="flex justify-between items-center mb-4">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">
        {{ isEditing ? 'Edit Article' : 'New Article' }}
      </h1>
      <div class="space-x-4">
        <NuxtLink to="/admin" class="text-gray-600 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white">
          Cancel
        </NuxtLink>
        <button 
          @click="save" 
          class="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded"
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
        class="w-full p-2 border rounded dark:bg-gray-700 dark:border-gray-600 dark:text-white"
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

<script setup>
definePageMeta({
  middleware: 'auth'
})

const route = useRoute()
const router = useRouter()

const filename = ref('')
const content = ref('')
const saving = ref(false)
const isEditing = computed(() => !!route.query.filename)

onMounted(async () => {
  if (isEditing.value) {
    filename.value = route.query.filename
    try {
      const res = await $fetch(`/api/admin/articles?action=read&filename=${filename.value}`)
      content.value = res.content
    } catch (e) {
      alert('Failed to load file')
      router.push('/admin')
    }
  }
})

const save = async () => {
  if (!filename.value) return alert('Filename is required')
  
  saving.value = true
  try {
    await $fetch('/api/admin/articles', {
      method: isEditing.value ? 'PUT' : 'POST',
      body: {
        filename: filename.value,
        content: content.value
      }
    })
    router.push('/admin')
  } catch (e) {
    alert(e.data?.statusMessage || 'Failed to save')
  } finally {
    saving.value = false
  }
}
</script>
