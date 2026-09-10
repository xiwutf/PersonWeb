<template>
  <div class="admin-login">
    <aside class="admin-login-brand" aria-hidden="true">
      <p class="admin-login-brand-kicker">PersonWeb</p>
      <p class="admin-login-brand-title">溪午听风</p>
      <p class="admin-login-brand-sub">管理入口</p>
    </aside>

    <main class="admin-login-main">
      <div class="admin-login-panel">
        <header class="admin-login-head">
          <h1>登录</h1>
        </header>

        <form class="admin-login-form" @submit.prevent="handleLogin">
          <div class="admin-login-field">
            <label for="admin-login-username">用户名</label>
            <input
              id="admin-login-username"
              v-model="username"
              type="text"
              autocomplete="username"
              placeholder="admin"
            >
          </div>

          <div class="admin-login-field">
            <label for="admin-login-password">密码</label>
            <input
              id="admin-login-password"
              v-model="password"
              type="password"
              autocomplete="current-password"
              placeholder="输入密码"
            >
          </div>

          <p v-if="error" class="admin-login-error" role="alert">{{ error }}</p>

          <button
            type="submit"
            class="admin-login-submit"
            :disabled="loading"
          >
            {{ loading ? '登录中…' : '进入后台' }}
          </button>
        </form>

        <nav class="admin-login-links" aria-label="返回站点">
          <NuxtLink to="/life">Life</NuxtLink>
          <NuxtLink to="/work">Work</NuxtLink>
          <NuxtLink to="/">入口</NuxtLink>
        </nav>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import {
  loginAgainstDotNet,
  resolveDotNetApiBase,
  usesNitroAdminAuth,
} from '~/utils/admin-runtime-auth'
import '~/assets/css/admin-login.css'

definePageMeta({
  layout: false,
})

useHead({
  meta: [
    { key: 'robots', name: 'robots', content: 'noindex,nofollow' },
  ],
})

const username = ref('admin')
const password = ref('')
const error = ref('')
const loading = ref(false)
const route = useRoute()
const router = useRouter()
const config = useRuntimeConfig()
const { setBackendToken, clearBackendToken } = useBackendAuth()
const { refresh } = useAdminSession()

const nitroMode = computed(() => usesNitroAdminAuth())

function resolveRedirectTarget() {
  const raw = route.query.redirect
  const value = Array.isArray(raw) ? raw[0] : raw
  if (typeof value !== 'string') return '/admin'
  if (!value.startsWith('/') || value.startsWith('//')) return '/admin'
  if (value.startsWith('/admin/login')) return '/admin'
  return value
}

const handleLogin = async () => {
  if (!username.value || !password.value) {
    error.value = '请输入用户名和密码'
    return
  }

  loading.value = true
  error.value = ''

  const trimmedUsername = username.value.trim() || 'admin'
  const trimmedPassword = password.value.trim()

  try {
    if (nitroMode.value) {
      const result = await $fetch<{ backendToken?: string }>('/api/auth/login', {
        method: 'POST',
        credentials: 'include',
        body: {
          username: trimmedUsername,
          password: trimmedPassword,
        },
      })

      if (import.meta.client) {
        localStorage.removeItem('admin_token')
        localStorage.removeItem('admin_user')
        clearBackendToken()
        if (result?.backendToken) {
          setBackendToken(result.backendToken)
        }
      }
    }
    else {
      const apiBase = resolveDotNetApiBase(
        undefined,
        typeof config.public.apiBase === 'string' ? config.public.apiBase : undefined,
      )
      const result = await loginAgainstDotNet(apiBase, trimmedUsername, trimmedPassword)

      if (import.meta.client) {
        localStorage.removeItem('admin_token')
        localStorage.removeItem('admin_user')
        setBackendToken(result.token)
      }
    }

    await refresh()
    await router.push(resolveRedirectTarget())
  }
  catch (e: any) {
    const status = e?.statusCode ?? e?.response?.status ?? e?.data?.statusCode
    if (status === 503) {
      error.value = '后台认证未配置：请在项目根目录 .env 设置 ADMIN_PASSWORD 后重启 npm run dev'
    }
    else if (status === 401) {
      error.value = '用户名或密码错误，请重试'
    }
    else if (status === 404) {
      error.value = '登录接口不可用：生产环境请确认 api 已部署且 Nginx 已转发 /api'
    }
    else {
      error.value = e?.statusMessage || e?.data?.message || e?.message || '登录失败，请检查用户名或密码'
    }
  }
  finally {
    loading.value = false
  }
}
</script>
