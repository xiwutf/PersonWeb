/**
 * Admin 侧栏菜单 active 判断。
 *
 * 规则：
 * - `/admin`（网站概览）必须精确匹配，禁止 startsWith('/admin')
 * - 其他栏目允许子路径保持父级高亮，但多个命中时取最长路径（如 /admin/ai/logs 只亮「AI 日志」）
 */

function normalizePath(path: string): string {
  if (!path) return ''
  const trimmed = path.split('?')[0].split('#')[0]
  if (trimmed.length > 1 && trimmed.endsWith('/')) {
    return trimmed.slice(0, -1)
  }
  return trimmed || '/'
}

function pathMatches(current: string, target: string): boolean {
  if (!current || !target) return false
  if (target === '/admin') {
    return current === '/admin'
  }
  return current === target || current.startsWith(`${target}/`)
}

/** 给定当前路由，返回菜单中唯一应高亮的 path */
export function resolveActiveAdminNavPath(
  currentPath: string,
  menuPaths: ReadonlyArray<string>,
): string | null {
  const current = normalizePath(currentPath)
  const matches = menuPaths
    .map(normalizePath)
    .filter((path) => pathMatches(current, path))

  if (matches.length === 0) return null
  matches.sort((a, b) => b.length - a.length)
  return matches[0] ?? null
}

export function isAdminNavActive(
  currentPath: string,
  itemPath: string,
  menuPaths: ReadonlyArray<string>,
): boolean {
  const active = resolveActiveAdminNavPath(currentPath, menuPaths)
  return active !== null && active === normalizePath(itemPath)
}
