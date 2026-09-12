/**
 * Admin auth runtime mode.
 * - Local Nuxt/Nitro: cookie session via /api/auth/*
 * - Production static OSS: no Nitro; use .NET JWT via /Auth/login + sessionStorage
 */

export function isLocalDevHostname(hostname: string): boolean {
  if (hostname === 'localhost' || hostname === '127.0.0.1' || hostname === '0.0.0.0') {
    return true
  }
  // npm run dev:mobile 用局域网 IP 打开时，仍应走 Nitro cookie 鉴权与 /api/content 写入
  if (/^192\.168\.\d{1,3}\.\d{1,3}$/.test(hostname)) return true
  if (/^10\.\d{1,3}\.\d{1,3}\.\d{1,3}$/.test(hostname)) return true
  if (/^172\.(1[6-9]|2\d|3[0-1])\.\d{1,3}\.\d{1,3}$/.test(hostname)) return true
  return false
}

/** True when Nitro auth routes (/api/auth/session 等) are expected to exist. */
export function usesNitroAdminAuth(hostname?: string): boolean {
  if (import.meta.server) {
    return true
  }
  if (typeof window === 'undefined') {
    return true
  }
  return isLocalDevHostname(hostname ?? window.location.hostname)
}

/**
 * Resolve .NET API base for browser calls (no trailing slash issues handled by callers).
 * Mirrors composables/useApi.ts hostname rules.
 */
export function resolveDotNetApiBase(
  hostname?: string,
  configuredBase?: string,
): string {
  const host = hostname ?? (typeof window !== 'undefined' ? window.location.hostname : '')

  if (isLocalDevHostname(host)) {
    return 'http://localhost:5234/api'
  }

  // xifg 主站；xlfg 为同站别名（历史拼写），共用同一套 API
  if (host.includes('xifg.com.cn') || host.includes('xlfg.com.cn')) {
    return 'https://api.xifg.com.cn/api'
  }

  if (host.includes('xing.com.cn')) {
    return 'https://api.xing.com.cn/api'
  }

  if (configuredBase && !configuredBase.includes('localhost:5234')) {
    return configuredBase.replace(/\/$/, '')
  }

  if (typeof window !== 'undefined') {
    return `${window.location.origin}/api`
  }

  return (configuredBase || '/api').replace(/\/$/, '')
}

export type DotNetLoginResult = {
  token: string
  username: string
  role: string
}

type DotNetLoginResponse = {
  code?: number
  message?: string
  data?: {
    token?: string
    Token?: string
    username?: string
    Username?: string
    role?: string
    Role?: string
  }
}

/** POST .NET /Auth/login and normalize token fields. */
export async function loginAgainstDotNet(
  apiBase: string,
  username: string,
  password: string,
): Promise<DotNetLoginResult> {
  const response = await $fetch<DotNetLoginResponse>(`${apiBase.replace(/\/$/, '')}/Auth/login`, {
    method: 'POST',
    body: { username, password },
  })

  const token = response?.data?.token || response?.data?.Token
  if (response?.code !== 0 || !token) {
    const message = response?.message || '登录失败'
    const error = new Error(message) as Error & { statusCode?: number }
    error.statusCode = response?.code === 401 ? 401 : 400
    throw error
  }

  return {
    token,
    username: response.data?.username || response.data?.Username || username,
    role: response.data?.role || response.data?.Role || 'admin',
  }
}
