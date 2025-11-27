<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">项目管理</h1>
      <NuxtLink to="/admin/projects/edit" class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition">
        新建项目
      </NuxtLink>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-900">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">项目名称</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">状态</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">GitHub</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">创建日期</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">操作</th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="item in projects" :key="item.id">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="h-10 w-10 flex-shrink-0">
                  <img class="h-10 w-10 rounded object-cover" :src="item.coverUrl || 'https://placehold.co/100'" alt="" />
                </div>
                <div class="ml-4">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ item.title }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ item.description?.substring(0, 20) }}...</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                :class="item.status === 'Active' ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                {{ item.status }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              <a v-if="item.githubUrl" :href="item.githubUrl" target="_blank" class="text-blue-600 hover:underline">Repo</a>
              <span v-else>-</span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ new Date(item.createdAt).toLocaleDateString() }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <NuxtLink :to="`/admin/projects/edit/${item.id}`" class="text-blue-600 hover:text-blue-900 dark:hover:text-blue-400 mr-4">编辑</NuxtLink>
              <button @click="handleDelete(item.id)" class="text-red-600 hover:text-red-900 dark:hover:text-red-400">删除</button>
            </td>
          </tr>
          <tr v-if="projects.length === 0">
            <td colspan="5" class="px-6 py-4 text-center text-gray-500 dark:text-gray-400">
              暂无项目
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const projects = ref<any[]>([])

const fetchProjects = async () => {
  try {
    const res = await api.get<any[]>('/Projects')
    projects.value = res
  } catch (e) {
    console.error('Failed to fetch projects', e)
  }
}

const handleDelete = async (id: string) => {
  if (!confirm('确定要删除这个项目吗？')) return
  
  try {
    await api.del(`/Projects/${id}`)
    await fetchProjects()
  } catch (e: any) {
    alert(e.message || '删除失败')
  }
}

onMounted(() => {
  fetchProjects()
})
</script>
