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

        <form class="admin-login-form" @submit.prevent="submitToAdmin">
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
            {{ loading && pendingTarget === '/admin' ? '登录中…' : '进入后台' }}
          </button>

          <button
            v-if="returnPath"
            type="button"
            class="admin-login-secondary"
            :disabled="loading"
            @click="submitToReturn"
          >
            {{ loading && pendingTarget === returnPath ? '登录中…' : returnLabel }}
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
  htmlAttrs: {
    'data-theme': 'light',
    style: 'color-scheme: light; background: #f7f9fc;',
  },
  bodyAttrs: {
    class: 'admin-login-body',
    style: 'margin:0;background:#f7f9fc;color:#142033;',
  },
  meta: [
    { key: 'robots', name: 'robots', content: 'noindex,nofollow' },
    { key: 'color-scheme', name: 'color-scheme', content: 'light' },
  ],
  style: [
    {
      key: 'admin-login-critical',
      children: `
        html,body{margin:0;min-height:100%;background:#f7f9fc;color:#142033;color-scheme:light}
        .admin-login{min-height:100svh;display:grid;grid-template-columns:minmax(0,1.05fr) minmax(20rem,.95fr);font-family:"PingFang SC","Microsoft YaHei",sans-serif}
        .admin-login-brand{display:grid;align-content:end;min-height:100svh;padding:3.5rem 3.2rem 3.2rem;color:#edf3fa;background:linear-gradient(210deg,#0b1c31 0%,#163556 48%,#1d4a73 100%)}
        .admin-login-brand-kicker{margin:0;opacity:.62;font-size:.78rem;letter-spacing:.18em;text-transform:uppercase}
        .admin-login-brand-title{margin:0;font-size:clamp(2.4rem,4.5vw,3.4rem);font-weight:650;letter-spacing:.08em}
        .admin-login-brand-sub{margin:.35rem 0 0;opacity:.72;letter-spacing:.12em}
        .admin-login-main{display:grid;place-items:center;padding:2.5rem 2rem}
        @media (max-width:860px){.admin-login{grid-template-columns:1fr}.admin-login-brand{min-height:0;padding:1.6rem 1.4rem}}
      `,
    },
  ],
})

const username = ref('admin')
const password = ref('')
const error = ref('')
const loading = ref(false)
const pendingTarget = ref('')
const route = useRoute()
const router = useRouter()
const config = useRuntimeConfig()
const { setBackendToken, clearBackendToken } = useBackendAuth()
const { refresh } = useAdminSession()

const nitroMode = computed(() => usesNitroAdminAuth())

/** Life 等页带来的回跳；主按钮「进入后台」始终进 /admin，不跟这个走。 */
const returnPath = computed(() => {
  const raw = route.query.redirect
  const value = Array.isArray(raw) ? raw[0] : raw
  if (typeof value !== 'string') return ''
  if (!value.startsWith('/') || value.startsWith('//')) return ''
  if (value === '/admin' || value.startsWith('/admin/login')) return ''
  return value
})

const returnLabel = computed(() => {
  if (returnPath.value.startsWith('/life')) return '登录并返回 Life'
  if (returnPath.value.startsWith('/work')) return '登录并返回 Work'
  return '登录并返回原页'
})

async function submitToAdmin() {
  await handleLogin('/admin')
}

async function submitToReturn() {
  if (!returnPath.value) return
  await handleLogin(returnPath.value)
}

async function handleLogin(target: string) {
  if (!username.value || !password.value) {
    error.value = '请输入用户名和密码'
    return
  }

  loading.value = true
  pendingTarget.value = target
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
    await router.push(target)
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
    pendingTarget.value = ''
  }
}
</script>
