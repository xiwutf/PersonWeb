<template>
  <div class="container mx-auto px-4 py-8">
    <div class="flex justify-between items-center mb-8">
      <h1 class="text-3xl font-bold text-gray-800 dark:text-white">Content Management</h1>
      <NuxtLink to="/admin/edit" class="bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded">
        Create New
      </NuxtLink>
    </div>

    <div class="bg-white dark:bg-gray-800 shadow overflow-hidden sm:rounded-lg">
      <ul class="divide-y divide-gray-200 dark:divide-gray-700">
        <li v-for="file in files" :key="file.path" class="px-6 py-4 hover:bg-gray-50 dark:hover:bg-gray-700">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-blue-600 dark:text-blue-400 truncate">
                {{ file.name }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400">
                {{ file.path }}
              </p>
            </div>
            <div class="flex space-x-4">
              <NuxtLink 
                :to="`/admin/edit?filename=${file.path}`"
                class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300"
              >
                Edit
              </NuxtLink>
              <button 
                @click="deleteFile(file.path)"
                class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
              >
                Delete
              </button>
            </div>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup>
definePageMeta({
  middleware: 'auth'
})

const files = ref([])

const fetchFiles = async () => {
  try {
    files.value = await $fetch('/api/admin/articles')
  } catch (e) {
    console.error('Failed to fetch files', e)
  }
}

const deleteFile = async (filename) => {
  if (!confirm(`Are you sure you want to delete ${filename}?`)) return
  
  try {
    await $fetch(`/api/admin/articles?filename=${filename}`, {
      method: 'DELETE'
    })
    await fetchFiles()
  } catch (e) {
    alert('Failed to delete file')
  }
}

onMounted(fetchFiles)
</script>
