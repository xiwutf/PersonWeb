<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">友情链接管理</h1>
      <NuxtLink to="/admin/friend-links/edit" class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition">
        新建链接
      </NuxtLink>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-900">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Logo</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">名称</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">链接</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">描述</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">排序</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">状态</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">操作</th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="link in friendLinks" :key="link.id">
            <td class="px-6 py-4 whitespace-nowrap">
              <img 
                v-if="link.logoUrl" 
                :src="link.logoUrl" 
                :alt="link.name" 
                class="h-10 w-10 rounded object-cover"
                @error="handleImageError"
              />
              <div v-else class="h-10 w-10 bg-gray-200 dark:bg-gray-700 rounded flex items-center justify-center">
                <i class="fas fa-link text-gray-400"></i>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900 dark:text-white">{{ link.name }}</div>
            </td>
            <td class="px-6 py-4">
              <a :href="link.url" target="_blank" class="text-sm text-blue-600 hover:underline dark:text-blue-400 max-w-xs truncate block">
                {{ link.url }}
              </a>
            </td>
            <td class="px-6 py-4">
              <div class="text-sm text-gray-500 dark:text-gray-400 max-w-xs truncate">
                {{ link.description || '-' }}
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ link.sortOrder }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span 
                class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                :class="link.status === 1 ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'"
              >
                {{ link.status === 1 ? '启用' : '禁用' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <NuxtLink :to="`/admin/friend-links/edit/${link.id}`" class="text-blue-600 hover:text-blue-900 dark:hover:text-blue-400 mr-4">编辑</NuxtLink>
              <button @click="handleDelete(link.id)" class="text-red-600 hover:text-red-900 dark:hover:text-red-400">删除</button>
            </td>
          </tr>
          <tr v-if="friendLinks.length === 0">
            <td colspan="7" class="px-6 py-4 text-center text-gray-500 dark:text-gray-400">
              暂无友情链接
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { FriendLink } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const friendLinks = ref<FriendLink[]>([])
const notification = useNotification()
const errorHandler = useErrorHandler()

const fetchFriendLinks = async () => {
  try {
    const res = await api.get<{ Total: number; List: FriendLink[] }>('/FriendLinks/all')
    friendLinks.value = res.List || []
  } catch (e: unknown) {
    errorHandler.handleError(e, '加载失败')
    friendLinks.value = []
  }
}

const handleDelete = async (id: number) => {
  if (!confirm('确定要删除这个友情链接吗？')) return
  
  try {
    await api.del(`/FriendLinks/${id}`)
    notification.success('删除成功')
    await fetchFriendLinks()
  } catch (e: unknown) {
    errorHandler.handleError(e, '删除失败')
  }
}

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.style.display = 'none'
}

onMounted(() => {
  fetchFriendLinks()
})
</script>

