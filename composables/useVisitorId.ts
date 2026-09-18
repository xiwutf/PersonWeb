const VISITOR_KEY = 'aven_visitor_id'
const LEGACY_VISITOR_KEY = 'visitor_id'

/**
 * 游客标识：优先 aven_visitor_id，兼容旧 analytics 的 visitor_id。
 */
export function useVisitorId() {
  const getVisitorId = (): string => {
    if (!import.meta.client) return ''

    const existing = localStorage.getItem(VISITOR_KEY)
      || localStorage.getItem(LEGACY_VISITOR_KEY)

    if (existing && existing.trim()) {
      if (!localStorage.getItem(VISITOR_KEY)) {
        localStorage.setItem(VISITOR_KEY, existing)
      }
      return existing
    }

    const next = crypto.randomUUID()
    localStorage.setItem(VISITOR_KEY, next)
    return next
  }

  return { getVisitorId }
}
