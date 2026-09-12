/**
 * Admin route guard.
 * - Local Nitro: cookie session via /api/auth/session
 * - Production static: .NET JWT in sessionStorage (backend_auth_token)
 * Vue middleware is UX only; API authorization remains the security boundary.
 */

import { usesNitroAdminAuth } from '~/utils/admin-runtime-auth'

export default defineNuxtRouteMiddleware(async (to) => {
  if (to.path === '/admin/login') {
    return
  }

  if (!import.meta.client) {
    return
  }

  try {
    if (usesNitroAdminAuth()) {
      const session = await $fetch<{ authenticated: boolean }>('/api/auth/session', {
        credentials: 'include',
      })
      if (!session?.authenticated) {
        return navigateTo({
          path: '/admin/login',
          query: { redirect: to.fullPath },
        })
      }
      return
    }

    const { getBackendToken, ensureBackendToken } = useBackendAuth()
    const token = getBackendToken() || await ensureBackendToken()
    if (!token) {
      return navigateTo({
        path: '/admin/login',
        query: { redirect: to.fullPath },
      })
    }
  }
  catch {
    return navigateTo({
      path: '/admin/login',
      query: { redirect: to.fullPath },
    })
  }
})
