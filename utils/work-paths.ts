/**
 * Work 世界公开 URL。
 * 规范前缀：/work/... ；根路径旧地址由 nestLegacyWorkPath 归一。
 */

export const WORK_LEGACY_ROOT_SEGMENTS = [
  'projects',
  'products',
  'blog',
  'about',
  'contact',
  'tools',
  'skills',
  'knowledge',
  'cognition',
  'module-store',
  'side-projects',
  'game',
  'links',
  'changelog',
  'pricing',
  'download',
  'english',
  'ai',
  'lab',
  'modules',
  'my-licenses',
] as const

export type WorkLegacyRootSegment = (typeof WORK_LEGACY_ROOT_SEGMENTS)[number]

export const WORK_ROUTES = {
  home: '/work',
  projects: '/work/projects',
  products: '/work/products',
  blog: '/work/blog',
  about: '/work/about',
  contact: '/work/contact',
  tools: '/work/tools',
  ai: '/work/ai',
  lab: '/work/lab',
  skills: '/work/skills',
  knowledge: '/work/knowledge',
  cognition: '/work/cognition',
  moduleStore: '/work/module-store',
  sideProjects: '/work/side-projects',
  game: '/work/game',
  links: '/work/links',
  changelog: '/work/changelog',
  pricing: '/work/pricing',
  download: '/work/download',
  english: '/work/english',
  search: '/search',
} as const

const LEGACY_SET = new Set<string>(WORK_LEGACY_ROOT_SEGMENTS)

function splitPathAndTail(input: string): { pathname: string; tail: string } {
  const raw = input || '/'
  const q = raw.search(/[?#]/)
  if (q === -1) return { pathname: raw, tail: '' }
  return { pathname: raw.slice(0, q), tail: raw.slice(q) }
}

function normalizePathname(pathname: string): string {
  let out = pathname.startsWith('/') ? pathname : `/${pathname}`
  out = out.replace(/\/+/g, '/')
  if (out.length > 1 && out.endsWith('/')) out = out.slice(0, -1)
  return out || '/'
}

/**
 * 将根路径遗留 Work URL 归一到 /work/...
 * 已是 /work、Life、Admin、API、搜索等路径则原样返回。
 */
export function nestLegacyWorkPath(fullPath: string): string {
  const { pathname, tail } = splitPathAndTail(fullPath || '/')
  let out = normalizePathname(pathname)

  const projectLegacy = out.match(/^\/projects\/detail-(.+)$/)
  if (projectLegacy) return `${WORK_ROUTES.projects}${tail}`

  const toolLegacy = out.match(/^\/tools\/detail-(.+)$/)
  if (toolLegacy) return `${WORK_ROUTES.tools}/${toolLegacy[1]}${tail}`

  if (out === '/work' || out.startsWith('/work/')) return `${out}${tail}`

  const first = out.split('/').filter(Boolean)[0]
  if (first && LEGACY_SET.has(first)) {
    return `/work${out}${tail}`
  }

  return `${out}${tail}`
}

export function workBlogPath(slug?: string | null): string {
  return slug ? `${WORK_ROUTES.blog}/${slug}` : WORK_ROUTES.blog
}

export function workProjectPath(id?: string | number | null): string {
  return id ? `${WORK_ROUTES.projects}/${id}` : WORK_ROUTES.projects
}

export function workToolPath(slug?: string | null): string {
  return slug ? `${WORK_ROUTES.tools}/${slug}` : WORK_ROUTES.tools
}

export function workCognitionPath(slug?: string | null): string {
  return slug ? `${WORK_ROUTES.cognition}/${slug}` : WORK_ROUTES.cognition
}

export function isWorkLegacyRootSegment(segment: string): boolean {
  return LEGACY_SET.has(segment)
}
