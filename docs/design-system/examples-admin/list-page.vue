<!-- ListPage 交互式示例 -->
<template>
  <div class="example-container">
    <div class="example-header">
      <h2>ListPage 示例</h2>
      <p>完整的列表页面组件演示</p>
    </div>

    <ListPage
      title="用户管理"
      description="管理系统用户账户、权限和角色"
      :columns="tableColumns"
      :data="filteredUsers"
      :loading="loading"
      :pagination="pagination"
      :show-filter="true"
      :filter-fields="filterFields"
      @page-change="handlePageChange"
      @filter-change="handleFilter"
      @row-click="handleRowClick"
    >
      <template #header-actions>
        <n-space>
          <n-button type="primary" @click="handleAdd">
            <template #icon>
              <n-icon :component="AddIcon" />
            </template>
            新增用户
          </n-button>
          <n-button @click="handleRefresh">
            <template #icon>
              <n-icon :component="RefreshIcon" />
            </template>
            刷新
          </n-button>
        </n-space>
      </template>

      <template #stats>
        <n-space :size="16">
          <n-statistic label="总用户数" :value="totalUsers" />
          <n-statistic label="活跃用户" :value="activeUsers" />
          <n-statistic label="本月新增" :value="newUsers" />
        </n-space>
      </template>
    </ListPage>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Add as AddIcon, Refresh as RefreshIcon } from '@vicons/ionicons5'
import ListPage from '~/components/admin/patterns/ListPage.vue'

// 模拟数据
const allUsers = ref([
  { id: 1, name: '张三', email: 'zhang@example.com', role: '管理员', status: 'active', createdAt: '2024-01-15' },
  { id: 2, name: '李四', email: 'li@example.com', role: '编辑', status: 'active', createdAt: '2024-01-20' },
  { id: 3, name: '王五', email: 'wang@example.com', role: '用户', status: 'disabled', createdAt: '2024-02-01' },
  { id: 4, name: '赵六', email: 'zhao@example.com', role: '用户', status: 'active', createdAt: '2024-02-10' },
  { id: 5, name: '钱七', email: 'qian@example.com', role: '编辑', status: 'active', createdAt: '2024-02-15' },
])

// 表格列配置
const tableColumns = [
  {
    title: '用户名',
    key: 'name',
    sorter: true
  },
  {
    title: '邮箱',
    key: 'email'
  },
  {
    title: '角色',
    key: 'role',
    render: (row: any) => {
      const roleMap: Record<string, any> = {
        '管理员': { type: 'error', text: '管理员' },
        '编辑': { type: 'warning', text: '编辑' },
        '用户': { type: 'info', text: '用户' }
      }
      const role = roleMap[row.role] || { type: 'default', text: row.role }
      return h('n-tag', { type: role.type, size: 'small' }, () => role.text)
    }
  },
  {
    title: '状态',
    key: 'status',
    render: (row: any) => {
      const statusConfig: Record<string, any> = {
        'active': { type: 'success', text: '正常' },
        'disabled': { type: 'default', text: '禁用' }
      }
      const config = statusConfig[row.status] || { type: 'default', text: row.status }
      return h('n-tag', { type: config.type, size: 'small' }, () => config.text)
    }
  },
  {
    title: '创建时间',
    key: 'createdAt'
  },
  {
    title: '操作',
    key: 'actions',
    render: (row: any) => h('n-space', () => [
      h('n-button', { size: 'small', onClick: () => handleEdit(row) }, () => '编辑'),
      h('n-button', { size: 'small', type: 'error', onClick: () => handleDelete(row) }, () => '删除')
    ])
  }
]

// 筛选字段配置
const filterFields = [
  {
    key: 'keyword',
    label: '关键词',
    type: 'input',
    placeholder: '搜索用户名或邮箱...'
  },
  {
    key: 'role',
    label: '角色',
    type: 'select',
    placeholder: '选择角色',
    options: [
      { label: '全部', value: '' },
      { label: '管理员', value: '管理员' },
      { label: '编辑', value: '编辑' },
      { label: '用户', value: '用户' }
    ]
  },
  {
    key: 'status',
    label: '状态',
    type: 'select',
    placeholder: '选择状态',
    options: [
      { label: '全部', value: '' },
      { label: '正常', value: 'active' },
      { label: '禁用', value: 'disabled' }
    ]
  }
]

// 状态
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(10)
const filters = ref({})

// 计算属性
const filteredUsers = computed(() => {
  let users = [...allUsers.value]

  if (filters.value.keyword) {
    const keyword = filters.value.keyword.toLowerCase()
    users = users.filter(u =>
      u.name.toLowerCase().includes(keyword) ||
      u.email.toLowerCase().includes(keyword)
    )
  }

  if (filters.value.role) {
    users = users.filter(u => u.role === filters.value.role)
  }

  if (filters.value.status) {
    users = users.filter(u => u.status === filters.value.status)
  }

  return users
})

const totalUsers = computed(() => allUsers.value.length)
const activeUsers = computed(() => allUsers.value.filter(u => u.status === 'active').length)
const newUsers = computed(() => allUsers.value.filter(u => {
  const date = new Date(u.createdAt)
  const now = new Date()
  return date.getMonth() === now.getMonth() && date.getFullYear() === now.getFullYear()
}).length)

const pagination = computed(() => ({
  page: currentPage.value,
  pageSize: pageSize.value,
  total: filteredUsers.value.length,
  showSizePicker: true,
  pageSizes: [10, 20, 50]
}))

// 方法
const handlePageChange = (page: number) => {
  currentPage.value = page
}

const handleFilter = (values: Record<string, any>) => {
  filters.value = values
  currentPage.value = 1
}

const handleRefresh = () => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
    window.$message?.success('刷新成功')
  }, 1000)
}

const handleAdd = () => {
  window.$message?.info('点击新增用户')
}

const handleEdit = (row: any) => {
  window.$message?.info(`编辑用户：${row.name}`)
}

const handleDelete = (row: any) => {
  window.$message?.warning(`删除用户：${row.name}`)
}

const handleRowClick = (row: any) => {
  window.$message?.info(`查看用户详情：${row.name}`)
}
</script>

<style scoped>
.example-container {
  min-height: 100vh;
  background: var(--color-bg-body);
}

.example-header {
  padding: var(--spacing-lg);
  text-align: center;
}

.example-header h2 {
  font-size: var(--text-2xl);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.example-header p {
  font-size: var(--text-base);
  color: var(--color-text-sec);
  margin: 0;
}
</style>
