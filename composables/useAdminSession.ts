/**
 * Client-only admin session probe for public-page inline editing / admin UX.
 * Frontend gating is UX only; API authorization remains the security boundary.
 *
 * `isAdmin` = 已登录且未开访客预览（控制前台编辑 UI）
 * `authenticated` = 真实登录态（预览时仍为 true）
 */
import { usesNitroAdminAuth } from '~/utils/admin-runtime-auth'

export function useAdminSession() {
  const authenticated = useState('admin-session-authenticated', () => false)
  const previewAsGuest = useState('admin-session-preview-guest', () => false)
  const pending = useState('admin-session-pending', () => true)
  const loaded = useState('admin-session-loaded', () => false)

  const isAdmin = computed(() => authenticated.value && !previewAsGuest.value)

  async function refresh() {
    if (!import.meta.client) {
      pending.value = false
      return
    }

    pending.value = true
    try {
      if (usesNitroAdminAuth()) {
        const session = await $fetch<{ authenticated: boolean }>('/api/auth/session', {
          credentials: 'include',
        })
        authenticated.value = Boolean(session?.authenticated)
      }
      else {
        const { getBackendToken, probeDotNetSession } = useBackendAuth()
        if (!getBackendToken()) {
          authenticated.value = false
        }
        else {
          authenticated.value = await probeDotNetSession()
        }
      }
      if (!authenticated.value) {
        previewAsGuest.value = false
      }
    }
    catch {
      authenticated.value = false
      previewAsGuest.value = false
    }
    finally {
      pending.value = false
      loaded.value = true
    }
  }

  function setPreviewAsGuest(value: boolean) {
    if (!authenticated.value) {
      previewAsGuest.value = false
      return
    }
    previewAsGuest.value = value
  }

  function togglePreviewAsGuest() {
    setPreviewAsGuest(!previewAsGuest.value)
  }

  async function logout() {
    if (!import.meta.client) return

    try {
      if (usesNitroAdminAuth()) {
        await $fetch('/api/auth/logout', {
          method: 'POST',
          credentials: 'include',
        })
      }
      else {
        const { clearBackendToken } = useBackendAuth()
        clearBackendToken()
      }
      if (typeof localStorage !== 'undefined') {
        localStorage.removeItem('admin_token')
        localStorage.removeItem('admin_user')
      }
    }
    catch {
      // still clear local UX state
    }
    finally {
      authenticated.value = false
      previewAsGuest.value = false
      loaded.value = true
      pending.value = false
    }
  }

  if (import.meta.client && !loaded.value) {
    // Fire-and-forget; does not block SSR HTML.
    void refresh()
  }

  return {
    isAdmin,
    authenticated,
    previewAsGuest,
    pending,
    loaded,
    refresh,
    setPreviewAsGuest,
    togglePreviewAsGuest,
    logout,
  }
}
