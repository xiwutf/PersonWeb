import {
  resolveDotNetApiBase,
  usesNitroAdminAuth,
} from '~/utils/admin-runtime-auth'

const BACKEND_AUTH_KEY = 'backend_auth_token'

export function useBackendAuth() {
  function getBackendToken(): string | null {
    if (!import.meta.client) {
      return null
    }
    return sessionStorage.getItem(BACKEND_AUTH_KEY)
  }

  function setBackendToken(token: string) {
    if (!import.meta.client) {
      return
    }
    sessionStorage.setItem(BACKEND_AUTH_KEY, token)
  }

  function clearBackendToken() {
    if (!import.meta.client) {
      return
    }
    sessionStorage.removeItem(BACKEND_AUTH_KEY)
  }

  async function ensureBackendToken(): Promise<string | null> {
    const existing = getBackendToken()
    if (existing) {
      return existing
    }

    if (!usesNitroAdminAuth()) {
      // Production static site: no Nitro session/cookie bridge.
      return null
    }

    try {
      const session = await $fetch<{ authenticated: boolean }>('/api/auth/session', {
        credentials: 'include',
      })
      if (!session?.authenticated) {
        return null
      }

      const result = await $fetch<{ backendToken: string }>('/api/auth/backend-token', {
        credentials: 'include',
      })
      if (result?.backendToken) {
        setBackendToken(result.backendToken)
        return result.backendToken
      }
    }
    catch {
      return null
    }

    return null
  }

  /** Optional probe: confirm JWT still accepted by .NET. */
  async function probeDotNetSession(): Promise<boolean> {
    const token = getBackendToken()
    if (!token) {
      return false
    }

    try {
      const config = useRuntimeConfig()
      const apiBase = resolveDotNetApiBase(
        undefined,
        typeof config.public.apiBase === 'string' ? config.public.apiBase : undefined,
      )
      await $fetch(`${apiBase}/Auth/profile`, {
        headers: { Authorization: `Bearer ${token}` },
      })
      return true
    }
    catch {
      clearBackendToken()
      return false
    }
  }

  return {
    getBackendToken,
    setBackendToken,
    clearBackendToken,
    ensureBackendToken,
    probeDotNetSession,
  }
}
