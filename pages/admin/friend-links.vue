<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">友情链接管理</h1>
      <NuxtLink to="/admin/friend-links/edit" class="btn-primary">
        新建链接
      </NuxtLink>
    </div>

    <div class="table-container">
      <table class="table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">Logo</th>
            <th class="table-header-cell">名称</th>
            <th class="table-header-cell">链接</th>
            <th class="table-header-cell">描述</th>
            <th class="table-header-cell">排序</th>
            <th class="table-header-cell">状�?/th>
            <th class="table-header-cell text-right">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="link in friendLinks" :key="link.id" class="table-row">
            <td class="table-cell">
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
            <td class="table-cell">
              <div class="text-sm font-medium text-gray-900 dark:text-var(--color-bg-light, white)">{{ link.name }}</div>
            </td>
            <td class="table-cell">
              <a :href="link.url" target="_blank" class="btn-link btn-link--blue max-w-xs truncate block">
                {{ link.url }}
              </a>
            </td>
            <td class="table-cell">
              <div class="text-sm text-gray-500 dark:text-gray-400 max-w-xs truncate">
                {{ link.description || '-' }}
              </div>
            </td>
            <td class="table-cell">
              {{ link.sortOrder }}
            </td>
            <td class="table-cell">
              <span class="badge"
                :class="link.status === 1 ? 'badge-green' : 'badge-gray'">
                {{ link.status === 1 ? '启用' : '禁用' }}
              </span>
            </td>
            <td class="table-cell text-right">
              <NuxtLink :to="`/admin/friend-links/edit/${link.id}`" class="btn-link btn-link--blue mr-4">编辑</NuxtLink>
              <button @click="handleDelete(link.id)" class="btn-link btn-link--red">删除</button>
            </td>
          </tr>
          <tr v-if="friendLinks.length === 0">
            <td colspan="7" class="table-cell text-center empty-state">
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
  if (!confirm('确定要删除这个友情链接吗�?)) return
  
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

