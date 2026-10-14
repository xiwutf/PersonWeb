import { isLocalDevHostname, resolveDotNetApiBase } from '~/utils/admin-runtime-auth'

type ReadingSession = { authenticated: boolean; expiresAt: string; csrfToken: string }

export function useLibraryAccess() {
  const config = useRuntimeConfig()
  const authenticated = useState('library-authorized', () => false)
  const csrfToken = useState('library-csrf', () => '')
  const expiresAt = useState('library-session-expiry', () => '')
  const loading = useState('library-access-loading', () => true)
  const error = useState('library-access-error', () => '')

  function apiBase() {
    const hostname = window.location.hostname
    let base = resolveDotNetApiBase(hostname, String(config.public.apiBase || ''))
    // A phone must address the computer hosting the API, not its own localhost.
    if (isLocalDevHostname(hostname)) {
      const url = new URL(base)
      url.hostname = hostname
      base = url.toString().replace(/\/$/, '')
    }
    return `${base}/library`
  }
  function clear() {
    authenticated.value = false
    csrfToken.value = ''
    expiresAt.value = ''
  }
  function accept(session: ReadingSession) {
    authenticated.value = session.authenticated
    csrfToken.value = session.csrfToken
    expiresAt.value = session.expiresAt
  }
  function statusOf(cause: unknown) {
    return (cause as { response?: { status?: number }; statusCode?: number })?.response?.status
      ?? (cause as { statusCode?: number })?.statusCode
  }
  function errorCode(cause: unknown) {
    const data = (cause as { data?: { error?: string }; response?: { _data?: { error?: string } } })?.data
      ?? (cause as { response?: { _data?: { error?: string } } })?.response?._data
    return typeof data?.error === 'string' ? data.error : ''
  }
  function message(cause: unknown) {
    const status = statusOf(cause)
    const code = errorCode(cause)
    if (status === 401) return '阅读密码不正确或会话已过期。'
    if (status === 429) return '尝试次数过多，请一分钟后再试。'
    if (status === 403) {
      return code === 'origin_rejected'
        ? '当前站点不在 AllowedOrigins 中。请把线上前端完整 Origin（如 https://xifg.com.cn）写入 API 配置后重启。'
        : '请求未通过安全校验，请刷新后重试。'
    }
    if (code === 'reading_not_configured') {
      return '阅读中心未配置完成：请设置 OwnerKey、AccessPasswordHash，以及线上 HTTPS AllowedOrigins，然后重启 API。'
    }
    if (status === 503 || code === 'reading_unavailable') {
      return '阅读服务暂不可用。请确认已执行 reading 表迁移、API 已发布，且数据库可写 reading_sessions。'
    }
    if (!status) {
      return '无法连接阅读 API。请确认前端走 https://api.xifg.com.cn，CORS AllowedOrigins 含当前站点，且全程 HTTPS。'
    }
    return '阅读服务暂不可用，请检查服务配置、数据库迁移和 HTTPS 连接。'
  }
  async function request<T>(path: string, method: 'GET' | 'POST' | 'PUT' = 'GET', body?: unknown,
    options: { responseType?: 'json' | 'blob' | 'arrayBuffer'; signal?: AbortSignal; keepalive?: boolean; timeout?: number } = {}) {
    try {
      const result = await $fetch(`${apiBase()}${path}`, {
        method, body: body as Record<string, unknown> | undefined,
        credentials: 'include', cache: 'no-store', timeout: 30000,
        ...options,
        retry: 0,
        headers: method !== 'GET' && csrfToken.value ? { 'X-Reading-CSRF': csrfToken.value } : {},
      })
      return result as T
    } catch (cause) {
      if (statusOf(cause) === 401) clear()
      throw cause
    }
  }
  async function check() {
    loading.value = true
    error.value = ''
    try {
      accept(await request<ReadingSession>('/auth/session'))
      if (Date.parse(expiresAt.value) - Date.now() < 7 * 86400000)
        accept(await request<ReadingSession>('/auth/renew', 'POST'))
    } catch (cause) {
      clear()
      if (statusOf(cause) !== 401) error.value = message(cause)
    } finally { loading.value = false }
  }
  async function login(password: string) {
    loading.value = true
    error.value = ''
    let passwordAccepted = false
    try {
      accept(await request<ReadingSession>('/auth/login', 'POST', { password }))
      passwordAccepted = true
      // Confirm the browser accepted the Secure cookie before showing private content.
      accept(await request<ReadingSession>('/auth/session'))
    } catch (cause) {
      clear()
      error.value = passwordAccepted && statusOf(cause) === 401
        ? '密码已验证，但浏览器未能使用安全会话。请使用与 API 同站点的 HTTPS 入口；xlfg 别名请改用 xifg 主站。'
        : message(cause)
    }
    finally { loading.value = false }
  }
  async function logout() {
    loading.value = true
    error.value = ''
    try { await request('/auth/logout', 'POST'); clear() }
    catch (cause) { error.value = message(cause) }
    finally { loading.value = false }
  }
  return { authenticated, loading, error, check, login, logout, request }
}
