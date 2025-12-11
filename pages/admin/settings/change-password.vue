<template>
  <div class="change-password-page">
    <div class="page-header">
      <h1 class="page-title">修改密码</h1>
      <p class="text-gray-400 text-sm">修改当前管理员账户的登录密码</p>
    </div>

    <div class="card max-w-2xl">
      <form @submit.prevent="handleChangePassword" class="space-y-6">
        <!-- 当前密码 -->
        <div class="form-group">
          <label class="form-label">
            当前密码 <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <input
              v-model="form.oldPassword"
              :type="showOldPassword ? 'text' : 'password'"
              required
              class="form-input pr-10"
              placeholder="请输入当前密码"
              autocomplete="current-password"
            />
            <button
              type="button"
              @click="showOldPassword = !showOldPassword"
              class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <i :class="showOldPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </button>
          </div>
        </div>

        <!-- 新密码 -->
        <div class="form-group">
          <label class="form-label">
            新密码 <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <input
              v-model="form.newPassword"
              :type="showNewPassword ? 'text' : 'password'"
              required
              class="form-input pr-10"
              placeholder="请输入新密码（至少6位）"
              autocomplete="new-password"
              minlength="6"
            />
            <button
              type="button"
              @click="showNewPassword = !showNewPassword"
              class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <i :class="showNewPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </button>
          </div>
          <p class="form-hint">密码长度至少6位字符</p>
        </div>

        <!-- 确认新密码 -->
        <div class="form-group">
          <label class="form-label">
            确认新密码 <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <input
              v-model="form.confirmPassword"
              :type="showConfirmPassword ? 'text' : 'password'"
              required
              class="form-input pr-10"
              placeholder="请再次输入新密码"
              autocomplete="new-password"
              minlength="6"
            />
            <button
              type="button"
              @click="showConfirmPassword = !showConfirmPassword"
              class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <i :class="showConfirmPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </button>
          </div>
          <p v-if="form.confirmPassword && form.newPassword !== form.confirmPassword" class="form-error">
            两次输入的密码不一致
          </p>
        </div>

        <!-- 错误提示 -->
        <div v-if="error" class="alert alert-error">
          <i class="fas fa-exclamation-circle mr-2"></i>
          {{ error }}
        </div>

        <!-- 成功提示 -->
        <div v-if="success" class="alert alert-success">
          <i class="fas fa-check-circle mr-2"></i>
          密码修改成功！请使用新密码登录。
        </div>

        <!-- 操作按钮 -->
        <div class="flex justify-end gap-3 pt-4">
          <button
            type="button"
            @click="$router.back()"
            class="px-4 py-2 bg-gray-200 dark:bg-gray-700 rounded hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors font-bold border-2 border-gray-400 dark:border-gray-500 shadow-sm"
            class="text-gray-900 dark:text-gray-100"
          >
            <span class="dark:text-gray-100" style="color: inherit;">取消</span>
          </button>
          <button
            type="submit"
            :disabled="loading || form.newPassword !== form.confirmPassword"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors font-medium shadow-md disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ loading ? '修改中...' : '确认修改' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const toast = useNotification()
const router = useRouter()

const form = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const showOldPassword = ref(false)
const showNewPassword = ref(false)
const showConfirmPassword = ref(false)

const loading = ref(false)
const error = ref('')
const success = ref(false)

const handleChangePassword = async () => {
  // 重置状态
  error.value = ''
  success.value = false

  // 验证
  if (!form.value.oldPassword) {
    error.value = '请输入当前密码'
    return
  }

  if (!form.value.newPassword || form.value.newPassword.length < 6) {
    error.value = '新密码长度至少6位字符'
    return
  }

  if (form.value.newPassword !== form.value.confirmPassword) {
    error.value = '两次输入的密码不一致'
    return
  }

  if (form.value.oldPassword === form.value.newPassword) {
    error.value = '新密码不能与当前密码相同'
    return
  }

  loading.value = true

  try {
    await api.put('/Auth/change-password', {
      oldPassword: form.value.oldPassword,
      newPassword: form.value.newPassword
    })

    toast.success('密码修改成功！请使用新密码登录。')
    success.value = true

    // 清空表单
    form.value = {
      oldPassword: '',
      newPassword: '',
      confirmPassword: ''
    }

    // 3秒后返回上一页
    setTimeout(() => {
      router.back()
    }, 3000)
  } catch (e: any) {
    console.error('修改密码失败:', e)
    error.value = e?.response?.data?.message || e?.message || '修改密码失败，请检查当前密码是否正确'
    toast.error(error.value)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.change-password-page {
  width: 100%;
}

.page-header {
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.875rem;
  font-weight: 700;
  color: var(--color-text-main, #ffffff);
  margin-bottom: 0.5rem;
}

.card {
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.05));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle, rgba(255, 255, 255, 0.1));
  border-radius: 0.75rem;
  padding: 2rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-text-main, #ffffff);
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.1));
  border: 1px solid var(--color-border-default, rgba(255, 255, 255, 0.2));
  border-radius: 0.5rem;
  color: var(--color-text-main, #ffffff);
  font-size: 1rem;
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary, #3b82f6);
  background: var(--color-bg-elevated, rgba(255, 255, 255, 0.15));
}

.form-input::placeholder {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.5));
}

.form-hint {
  margin-top: 0.5rem;
  font-size: 0.75rem;
  color: var(--color-text-muted, rgba(255, 255, 255, 0.6));
}

.form-error {
  margin-top: 0.5rem;
  font-size: 0.75rem;
  color: var(--color-error, #ef4444);
}

.alert {
  padding: 1rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  font-size: 0.875rem;
}

.alert-error {
  background: var(--color-error-soft, rgba(239, 68, 68, 0.1));
  border: 1px solid var(--color-error, rgba(239, 68, 68, 0.3));
  color: var(--color-error-hover, #fca5a5);
}

.alert-success {
  background: var(--color-success-soft, rgba(34, 197, 94, 0.1));
  border: 1px solid var(--color-success, rgba(34, 197, 94, 0.3));
  color: var(--color-success-hover, #86efac);
}
</style>

