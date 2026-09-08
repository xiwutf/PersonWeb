/**
 * Client-only admin session probe for public-page inline editing.
 * Frontend gating is UX only; Nitro checkAuth remains the security boundary.
 */
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
      const session = await $fetch<{ authenticated: boolean }>('/api/auth/session', {
        credentials: 'include',
      })
      isAdmin.value = Boolean(session?.authenticated)
    } catch {
      isAdmin.value = false
    } finally {
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
