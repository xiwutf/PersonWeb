/**
 * Client-only admin session probe for public-page inline editing / admin UX.
 * Frontend gating is UX only; API authorization remains the security boundary.
 */
import { usesNitroAdminAuth } from '~/utils/admin-runtime-auth'

export function useAdminSession() {
  const isAdmin = useState('admin-session-is-admin', () => false)
  const pending = useState('admin-session-pending', () => true)
  const loaded = useState('admin-session-loaded', () => false)

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
        isAdmin.value = Boolean(session?.authenticated)
      }
      else {
        const { getBackendToken, probeDotNetSession } = useBackendAuth()
        if (!getBackendToken()) {
          isAdmin.value = false
        }
        else {
          isAdmin.value = await probeDotNetSession()
        }
      }
    }
    catch {
      isAdmin.value = false
    }
    finally {
      pending.value = false
      loaded.value = true
    }
  }

  if (import.meta.client && !loaded.value) {
    // Fire-and-forget; does not block SSR HTML.
    void refresh()
  }

  return {
    isAdmin,
    pending,
    loaded,
    refresh,
  }
}
