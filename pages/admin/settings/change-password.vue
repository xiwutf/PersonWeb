<template>
  <div class="change-password-page">
    <div class="page-header">
      <h1 class="page-title">修改密码</h1>
      <p class="change-password-subtitle text-sm">修改当前管理员账户的登录密码</p>
    </div>

    <div class="card max-w-2xl">
      <form @submit.prevent="handleChangePassword" class="space-y-6">
        <!-- 当前密码 -->
        <div class="form-group">
          <label class="form-label">
            当前密码 <span class="change-password-required">*</span>
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
              class="absolute right-3 top-1/2 -translate-y-1/2 change-password-toggle-btn"
            >
              <i :class="showOldPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </button>
          </div>
        </div>

        <!-- 新密码 -->
        <div class="form-group">
          <label class="form-label">
            新密码 <span class="change-password-required">*</span>
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
              class="absolute right-3 top-1/2 -translate-y-1/2 change-password-toggle-btn"
            >
              <i :class="showNewPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
            </button>
          </div>
          <p class="form-hint">密码长度至少6位字符</p>
        </div>

        <!-- 确认新密码 -->
        <div class="form-group">
          <label class="form-label">
            确认新密码 <span class="change-password-required">*</span>
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
              class="absolute right-3 top-1/2 -translate-y-1/2 change-password-toggle-btn"
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
            class="px-4 py-2 change-password-cancel-btn rounded transition-colors font-bold border-2 shadow-sm"
          >
            <span class="dark:text-gray-100" style="color: inherit;">取消</span>
          </button>
          <button
            type="submit"
            :disabled="loading || form.newPassword !== form.confirmPassword"
            class="px-4 py-2 change-password-submit-btn rounded transition-colors font-medium shadow-md disabled:opacity-50 disabled:cursor-not-allowed"
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
  color: var(--color-text-main);
  margin-bottom: 0.5rem;
}

.card {
  background: var(--color-bg-elevated, var(--color-bg-card));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle);
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
  color: var(--color-text-main);
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  background: var(--color-bg-elevated, var(--color-bg-card));
  border: 1px solid var(--color-border-default);
  border-radius: 0.5rem;
  color: var(--color-text-main);
  font-size: 1rem;
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary);
  background: var(--color-bg-elevated);
}

.form-input::placeholder {
  color: var(--color-text-muted);
}

.form-hint {
  margin-top: 0.5rem;
  font-size: 0.75rem;
  color: var(--color-text-muted);
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
  background: var(--color-error-soft);
  border: 1px solid var(--color-error);
  color: var(--color-error-hover, #fca5a5);
}

.alert-success {
  background: var(--color-success-soft);
  border: 1px solid var(--color-success);
  color: var(--color-success-hover, #86efac);
}

/* 修改密码页面样式 - 使用 CSS 变量 */
.change-password-subtitle {
  color: var(--color-text-muted, #9ca3af);
}

.change-password-required {
  color: var(--color-error, #ef4444);
}

.change-password-toggle-btn {
  color: var(--color-text-muted, #9ca3af);
  transition: color 0.2s ease;
}

.change-password-toggle-btn:hover {
  color: var(--color-text-sub, #6b7280);
}

.change-password-cancel-btn {
  background: var(--color-bg-elevated, #e5e7eb);
  border-color: var(--color-border-default, #9ca3af);
  color: var(--color-text-main, #111827);
  transition: all 0.2s ease;
}

.change-password-cancel-btn:hover {
  background: var(--color-bg-elevated, #d1d5db);
  border-color: var(--color-border-default);
}

.change-password-submit-btn {
  background: var(--color-primary);
  color: var(--color-text-main);
  transition: background-color 0.2s ease;
}

.change-password-submit-btn:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

/* 深色主题适配 */
html[data-theme="dark"] .change-password-subtitle,
html.dark .change-password-subtitle {
  color: var(--color-text-muted);
}

html[data-theme="dark"] .change-password-toggle-btn,
html.dark .change-password-toggle-btn {
  color: var(--color-text-muted);
}

html[data-theme="dark"] .change-password-toggle-btn:hover,
html.dark .change-password-toggle-btn:hover {
  color: var(--color-text-sub);
}

html[data-theme="dark"] .change-password-cancel-btn,
html.dark .change-password-cancel-btn {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-default);
  color: var(--color-text-main);
}

html[data-theme="dark"] .change-password-cancel-btn:hover,
html.dark .change-password-cancel-btn:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-default);
}
</style>

