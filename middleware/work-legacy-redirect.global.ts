/**
 * 根路径遗留 Work 栏目 → /work/... （301）
 */
import { nestLegacyWorkPath } from '~/utils/work-paths'

export default defineNuxtRouteMiddleware((to) => {
  const nested = nestLegacyWorkPath(to.fullPath)
  if (nested === to.fullPath) return
  return navigateTo(nested, { redirectCode: 301, external: false })
})
