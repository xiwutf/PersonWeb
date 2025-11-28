<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">用户管理</h1>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
        + 新建用户
      </button>
    </div>

    <!-- 用户列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <div v-if="loading" class="p-8 text-center text-gray-500 dark:text-gray-400">加载中...</div>
      <div v-else-if="users.length === 0" class="p-8 text-center text-gray-500 dark:text-gray-400">暂无用户</div>
      <table v-else class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-900">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">ID</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">用户名</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">邮箱</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">角色</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">状态</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">最后登录</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">操作</th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ user.id }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">{{ user.username }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{{ user.email || '-' }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              <span :class="getRoleClass(user.role)" class="px-2 py-1 rounded text-xs font-medium">
                {{ user.role }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm">
              <span :class="user.status === 1 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'" class="px-2 py-1 rounded text-xs font-medium">
                {{ user.status === 1 ? '启用' : '禁用' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ user.lastLoginTime ? formatDate(user.lastLoginTime) : '从未登录' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button @click="editUser(user)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300 mr-4">编辑</button>
              <button @click="deleteUser(user.id)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">删除</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑用户模态框 -->
    <div v-if="showCreateModal || editingUser" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="closeModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h2 class="text-xl font-bold text-gray-800 dark:text-white mb-4">
            {{ editingUser ? '编辑用户' : '新建用户' }}
          </h2>
          <form @submit.prevent="saveUser" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">用户名 *</label>
              <input v-model="userForm.username" type="text" required class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div v-if="!editingUser">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">密码 *</label>
              <input v-model="userForm.password" type="password" :required="!editingUser" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div v-else>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">新密码（留空不修改）</label>
              <input v-model="userForm.password" type="password" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">邮箱</label>
              <input v-model="userForm.email" type="email" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">角色</label>
              <select v-model="userForm.role" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                <option value="admin">管理员</option>
                <option value="editor">编辑</option>
                <option value="viewer">查看者</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">状态</label>
              <select v-model.number="userForm.status" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
                <option :value="1">启用</option>
                <option :value="0">禁用</option>
              </select>
            </div>
            <div class="flex justify-end gap-3 pt-4">
              <button type="button" @click="closeModal" class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">取消</button>
              <button type="submit" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">保存</button>
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
const toast = useToast()

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

