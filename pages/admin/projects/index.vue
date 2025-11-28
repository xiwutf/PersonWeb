<template>
  <div class="projects-page">
    <div class="page-header">
      <h1 class="page-title">项目管理</h1>
      <div class="header-actions">
        <NuxtLink to="/admin/projects/stats">
          <n-button type="success" secondary>
            <template #icon>
              <i class="fas fa-chart-bar"></i>
            </template>
            访问统计
          </n-button>
        </NuxtLink>
        <NuxtLink to="/admin/projects/edit">
          <n-button type="primary">
            <template #icon>
              <i class="fas fa-plus"></i>
            </template>
            新建项目
          </n-button>
        </NuxtLink>
      </div>
    </div>

    <ClientOnly>
      <n-card>
        <n-data-table
          :columns="columns"
          :data="projects"
          :loading="loading"
          :bordered="false"
        />
      </n-card>
      <template #fallback>
        <n-card>
          <div style="padding: 20px; text-align: center; color: #9ca3af;">
            加载中...
          </div>
        </n-card>
      </template>
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import { h } from 'vue'
import { NButton, NCard, NDataTable, NTag, NPopconfirm, NAvatar } from 'naive-ui'
import type { DataTableColumns } from 'naive-ui'
import type { Project } from '~/types/api'
import { useMessage } from 'naive-ui'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { handleError } = useErrorHandler()

// 只在客户端使用 Naive UI 的 composables
const message = process.client ? useMessage() : { success: () => {}, error: () => {}, warning: () => {}, info: () => {}, loading: () => {} }

const projects = ref<Project[]>([])
const loading = ref(false)

// 表格列定义
const columns: DataTableColumns<Project> = [
  {
    title: '项目名称',
    key: 'title',
    width: 300,
    render(row) {
      return h('div', { class: 'project-info' }, [
        h(NAvatar, {
          src: row.coverUrl || 'https://placehold.co/100',
          size: 40,
          round: true,
          style: { marginRight: '12px' }
        }),
        h('div', { class: 'project-details' }, [
          h('div', { class: 'project-title' }, row.title),
          h('div', { class: 'project-desc' }, row.description?.substring(0, 30) + '...' || '-')
        ])
      ])
    }
  },
  {
    title: '状态',
    key: 'status',
    width: 100,
    render(row) {
      return h(NTag, {
        type: row.status === 'Active' ? 'success' : 'default',
        size: 'small'
      }, {
        default: () => row.status
      })
    }
  },
  {
    title: '访问量',
    key: 'viewCount',
    width: 120,
    render(row) {
      return h('div', { class: 'view-count' }, [
        h('i', { class: 'fas fa-eye', style: { marginRight: '4px', color: '#9ca3af' } }),
        h('span', row.viewCount || 0)
      ])
    }
  },
  {
    title: 'GitHub',
    key: 'githubUrl',
    width: 100,
    render(row) {
      if (row.githubUrl) {
        return h('a', {
          href: row.githubUrl,
          target: '_blank',
          style: { color: '#60a5fa', textDecoration: 'none' }
        }, 'Repo')
      }
      return '-'
    }
  },
  {
    title: '创建日期',
    key: 'createdAt',
    width: 150,
    render(row) {
      return new Date(row.createdAt).toLocaleDateString()
    }
  },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    render(row) {
      return h('div', { class: 'action-buttons' }, [
        h(NuxtLink, {
          to: `/admin/projects/edit/${row.id}`,
          style: { marginRight: '8px' }
        }, {
          default: () => h(NButton, {
            size: 'small',
            type: 'primary',
            quaternary: true
          }, {
            default: () => '编辑'
          })
        }),
        h(NPopconfirm, {
          onPositiveClick: () => handleDelete(row.id)
        }, {
          trigger: () => h(NButton, {
            size: 'small',
            type: 'error',
            quaternary: true
          }, {
            default: () => '删除'
          }),
          default: () => '确定要删除这个项目吗？'
        })
      ])
    }
  }
]

const fetchProjects = async () => {
  loading.value = true
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
    message.error('加载项目列表失败')
  } finally {
    loading.value = false
  }
}

const handleDelete = async (id: string) => {
  try {
    await api.del(`/Projects/${id}`)
    message.success('删除成功')
    await fetchProjects()
  } catch (e: unknown) {
    handleError(e, '删除失败')
  }
}

onMounted(() => {
  fetchProjects()
})
</script>

<style scoped>
.projects-page {
  width: 100%;
}

.header-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.project-info {
  display: flex;
  align-items: center;
}

.project-details {
  flex: 1;
}

.project-title {
  font-size: 0.875rem;
  font-weight: 500;
  color: #e5e7eb;
  margin-bottom: 0.25rem;
}

.project-desc {
  font-size: 0.75rem;
  color: #9ca3af;
}

.view-count {
  display: flex;
  align-items: center;
  color: #9ca3af;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}
</style>
