/**
 * Tool URL canonical helpers (Work marketplace PRIMARY = Toolbox DB).
 */

export function canonicalizeToolPath(pathOrSlug: string): string {
  const raw = String(pathOrSlug || '').trim()
  if (!raw) return '/work/tools'

  if (raw.startsWith('/tools/detail-')) {
    const slug = raw.slice('/tools/detail-'.length)
    return slug ? `/work/tools/${slug}` : '/work/tools'
  }

  if (raw.startsWith('/work/tools/detail-')) {
    const slug = raw.slice('/work/tools/detail-'.length)
    return slug ? `/work/tools/${slug}` : '/work/tools'
  }

  const detailMatch = raw.match(/^detail-(.+)$/i)
  if (detailMatch?.[1]) {
    return `/work/tools/${detailMatch[1]}`
  }

  if (raw.startsWith('/work/tools/')) {
    return raw.replace(/\/+$/, '') || '/work/tools'
  }

  if (raw === '/tools') return '/work/tools'
  if (raw.startsWith('/tools/')) return canonicalizeToolPath(`/work${raw}`)

  if (raw.startsWith('/')) return raw
  return `/work/tools/${raw}`
}

export function extractToolSlug(pathOrSlug: string): string | null {
  const canonical = canonicalizeToolPath(pathOrSlug)
  if (canonical === '/work/tools') return null
  const match = canonical.match(/^\/work\/tools\/([^/]+)$/)
  return match?.[1] || null
}
