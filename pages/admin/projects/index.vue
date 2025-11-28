<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">项目管理</h1>
      <div class="flex gap-2">
        <NuxtLink to="/admin/projects/stats" class="btn-success">
          访问统计
        </NuxtLink>
        <NuxtLink to="/admin/projects/edit" class="btn-primary">
          新建项目
        </NuxtLink>
      </div>
    </div>

    <div class="table-container">
      <table class="table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">项目名称</th>
            <th class="table-header-cell">状态</th>
            <th class="table-header-cell">访问量</th>
            <th class="table-header-cell">GitHub</th>
            <th class="table-header-cell">创建日期</th>
            <th class="table-header-cell text-right">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="item in projects" :key="item.id" class="table-row">
            <td class="table-cell">
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
            <td class="table-cell">
              <span class="badge"
                :class="item.status === 'Active' ? 'badge-green' : 'badge-gray'">
                {{ item.status }}
              </span>
            </td>
            <td class="table-cell">
              <div class="flex items-center gap-1">
                <i class="fas fa-eye text-gray-400"></i>
                <span>{{ item.viewCount || 0 }}</span>
              </div>
            </td>
            <td class="table-cell">
              <a v-if="item.githubUrl" :href="item.githubUrl" target="_blank" class="btn-link btn-link--blue">Repo</a>
              <span v-else>-</span>
            </td>
            <td class="table-cell">
              {{ new Date(item.createdAt).toLocaleDateString() }}
            </td>
            <td class="table-cell text-right">
              <NuxtLink :to="`/admin/projects/edit/${item.id}`" class="btn-link btn-link--blue mr-4">编辑</NuxtLink>
              <button @click="handleDelete(item.id)" class="btn-link btn-link--red">删除</button>
            </td>
          </tr>
          <tr v-if="projects.length === 0">
            <td colspan="6" class="table-cell text-center empty-state">
              暂无项目
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '~/types/api'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const projects = ref<Project[]>([])

const fetchProjects = async () => {
  try {
    // 后端返回格式: { code: 0, data: List<Project> }
    // useApi 已经处理了响应格式，直接返回 data (即 List<Project>)
    const res = await api.get<Project[]>('/Projects')
    projects.value = Array.isArray(res) ? res : []
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch projects:', e)
    }
    projects.value = []
  }
}

const handleDelete = async (id: string) => {
  if (!confirm('确定要删除这个项目吗？')) return
  
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.del(`/Projects/${id}`)
    success('删除成功')
    await fetchProjects()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

onMounted(() => {
  fetchProjects()
})
</script>
