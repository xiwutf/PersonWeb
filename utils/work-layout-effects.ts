/**
 * Work default layout: which ambient / deferred UI mounts for a path.
 * Kept pure for unit tests (no Vue).
 */

export function isWorkContentFocusRoute(path: string): boolean {
  const p = path || '/'
  if (p === '/search' || p.startsWith('/search/')) return true
  if (p.startsWith('/work/blog/') || p.startsWith('/blog/')) return true
  if ((p.startsWith('/work/cognition/') && p !== '/work/cognition') || (p.startsWith('/cognition/') && p !== '/cognition')) return true
  if (p.startsWith('/work/knowledge/') || p.startsWith('/knowledge/')) return true
  if (/^\/(?:work\/)?projects\/[^/]+$/.test(p)) return true
  if (/^\/(?:work\/)?tools\/[^/]+$/.test(p) && !p.includes('/tools/detail-')) return true
  return false
}

export function isWorkAmbientRoute(path: string): boolean {
  const p = path || '/'
  return p === '/work'
    || p === '/work/lab' || p === '/lab'
    || p === '/work/products' || p.startsWith('/work/products/')
    || p === '/products' || p.startsWith('/products/')
    || p === '/work/ai' || p.startsWith('/work/ai/')
    || p === '/ai' || p.startsWith('/ai/')
}

export function shouldShowWorkParticleLayer(
  path: string,
  options: { deferred: boolean; lowPower: boolean },
): boolean {
  if (!options.deferred || options.lowPower) return false
  if (isWorkContentFocusRoute(path)) return false
  return isWorkAmbientRoute(path)
}

export function shouldShowWorkDeferredChrome(
  path: string,
  options: { deferred: boolean; lowPower: boolean },
): boolean {
  if (!options.deferred || options.lowPower) return false
  return !isWorkContentFocusRoute(path)
}

/**
 * Work layout 不再挂全屏弹幕；入口页 `/` 自行挂载局部弹幕。
 * 保留函数供守卫测试与历史调用，恒为 false。
 */
export function shouldShowVisitorDanmaku(
  _path: string,
  _options: { deferred: boolean; lowPower: boolean },
): boolean {
  return false
}
