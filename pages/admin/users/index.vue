<template>
  <div>
    <div class="page-header">
      <h1 class="page-title">用户管理</h1>
      <button @click="showCreateModal = true" class="btn-primary">
        + 新建用户
      </button>
    </div>

    <!-- 用户列表 -->
    <div class="card overflow-hidden">
      <div v-if="loading" class="loading">加载�?..</div>
      <div v-else-if="users.length === 0" class="empty-state">暂无用户</div>
      <table v-else class="table">
        <thead class="table-header">
          <tr>
            <th class="table-header-cell">ID</th>
            <th class="table-header-cell">用户</th>
            <th class="table-header-cell">邮箱</th>
            <th class="table-header-cell">角色</th>
            <th class="table-header-cell">状态</th>
            <th class="table-header-cell">最后登录</th>
            <th class="table-header-cell text-right">操作</th>
          </tr>
        </thead>
        <tbody class="table-body">
          <tr v-for="user in users" :key="user.id" class="table-row">
            <td class="table-cell">{{ user.id }}</td>
            <td class="table-cell font-medium">{{ user.username }}</td>
            <td class="table-cell text-gray-500 dark:text-gray-400">{{ user.email || '-' }}</td>
            <td class="table-cell">
              <span :class="getRoleClass(user.role)" class="badge">
                {{ user.role }}
              </span>
            </td>
            <td class="table-cell">
              <span :class="user.status === 1 ? 'badge-green' : 'badge-red'" class="badge">
                {{ user.status === 1 ? '启用' : '禁用' }}
              </span>
            </td>
            <td class="table-cell text-gray-500 dark:text-gray-400">
              {{ user.lastLoginTime ? formatDate(user.lastLoginTime) : '从未登录' }}
            </td>
            <td class="table-cell text-right">
              <button @click="editUser(user)" class="btn-link btn-link--blue">编辑</button>
              <button @click="deleteUser(user.id)" class="btn-link btn-link--red">删除</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑用户模态框 -->
    <div v-if="showCreateModal || editingUser" class="modal-overlay" @click.self="closeModal">
      <div class="modal-content max-w-md">
        <div class="modal-body">
          <h2 class="modal-title mb-4">
            {{ editingUser ? '编辑用户' : '新建用户' }}
          </h2>
          <form @submit.prevent="saveUser" class="space-y-4">
            <div class="form-group">
              <label class="form-label">用户名 *</label>
              <input v-model="userForm.username" type="text" required class="form-input" />
            </div>
            <div v-if="!editingUser" class="form-group">
              <label class="form-label">密码 *</label>
              <input v-model="userForm.password" type="password" required class="form-input" />
            </div>
            <div v-else class="form-group">
              <label class="form-label">新密码（留空不修改）</label>
              <input v-model="userForm.password" type="password" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">邮箱</label>
              <input v-model="userForm.email" type="email" class="form-input" />
            </div>
            <div class="form-group">
              <label class="form-label">角色</label>
              <select v-model="userForm.role" class="form-select">
                <option value="admin">管理员</option>
                <option value="editor">编辑</option>
                <option value="viewer">查看</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">状态</label>
              <select v-model.number="userForm.status" class="form-select">
                <option :value="1">启用</option>
                <option :value="0">禁用</option>
              </select>
            </div>
            <div class="modal-footer">
              <button type="button" @click="closeModal" class="btn-secondary">取消</button>
              <button type="submit" class="btn-primary">保存</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface User {
  id: number
  username: string
  email?: string
  role: string
  status: number
  lastLoginTime?: string
}

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const toast = useNotification()

const users = ref<User[]>([])
const loading = ref(false)
const showCreateModal = ref(false)
const editingUser = ref<User | null>(null)

const userForm = ref({
  username: '',
  password: '',
  email: '',
  role: 'admin',
  status: 1
})

const fetchUsers = async () => {
  loading.value = true
  try {
    const res = await api.get('/Users')
    if (res?.data) {
      users.value = Array.isArray(res.data) ? res.data : res.data.Items || []
    }
  } catch (error) {
    console.error('获取用户列表失败:', error)
    toast.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

const saveUser = async () => {
  try {
    const data: any = {
      username: userForm.value.username,
      email: userForm.value.email || null,
      role: userForm.value.role,
      status: userForm.value.status
    }

    if (userForm.value.password) {
      data.password = userForm.value.password
    }

    if (editingUser.value) {
      await api.put(`/Users/${editingUser.value.id}`, data)
      toast.success('用户更新成功')
    } else {
      if (!userForm.value.password) {
        toast.error('新建用户必须设置密码')
        return
      }
      await api.post('/Users', data)
      toast.success('用户创建成功')
    }

    closeModal()
    fetchUsers()
  } catch (error: any) {
    console.error('保存用户失败:', error)
    toast.error(error?.response?.data?.message || '保存用户失败')
  }
}

const editUser = (user: User) => {
  editingUser.value = user
  userForm.value = {
    username: user.username,
    password: '',
    email: user.email || '',
    role: user.role,
    status: user.status
  }
}

const deleteUser = async (id: number) => {
  if (!confirm('确定要删除这个用户吗？')) return

  try {
    await api.delete(`/Users/${id}`)
    toast.success('用户删除成功')
    fetchUsers()
  } catch (error: any) {
    console.error('删除用户失败:', error)
    toast.error(error?.response?.data?.message || '删除用户失败')
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingUser.value = null
  userForm.value = {
    username: '',
    password: '',
    email: '',
    role: 'admin',
    status: 1
  }
}

const getRoleClass = (role: string) => {
  const classes: Record<string, string> = {
    admin: 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300',
    editor: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-300',
    viewer: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300'
  }
  return classes[role] || classes.viewer
}

const formatDate = (date: string) => {
  if (!date) return '-'
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
}

onMounted(() => {
  fetchUsers()
})
</script>

