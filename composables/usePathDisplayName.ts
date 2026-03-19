/**
 * 路径显示名称映射
 *
 * 新增版块时，只需在 ROUTE_LABELS 中添加一行即可，无需修改多处：
 *   '新路由': '中文显示名'
 *
 * 若该版块有子路径（如 /blog/slug），可在 ROUTE_SUB_LABELS 中设置子路径时的简称。
 */
const ROUTE_LABELS: Record<string, string> = {
  '': '首页',
  'blog': '技术博客',
  'tools': '插件工具',
  'projects': '项目展示',
  'about': '关于我',
  'ai': 'AI / 智能体',
  'lab': 'AI 实验室',
  'life': '生活随笔',
  'skills': '技能树',
  'english': '英语学习',
  'dashboard': '仪表盘',
  'cognition': '认知说明书',
  'side-projects': '副业项目',
  'admin': '管理后台',
  'investment': '投资记录',
  'game': '小游戏'
}

/** 子路径显示时的栏目简称（用于 blog/slug、tools/slug 等） */
const ROUTE_SUB_LABELS: Record<string, string> = {
  blog: '博客',
  tools: '工具',
  projects: '项目',
  life: '随笔',
  cognition: '认知'
}

/**
 * 将路径转换为中文显示名称
 * @param path 原始路径，如 /blog、/tools、/blog/my-post
 * @returns 中文显示名称
 */
export const usePathDisplayName = () => {
  const formatPath = (path: string | null | undefined): string => {
    if (!path || path === '') return '首页'
    const normalized = path
      .replace(/^landing:/, '')
      .replace(/^page:/, '')
      .replace(/^\/+/, '')
      .split('?')[0]
      .split('#')[0]
      .trim()
    if (!normalized || normalized === '/') return '首页'

    const parts = normalized.split('/').filter(Boolean)
    if (parts.length === 0) return '首页'

    const first = parts[0].toLowerCase()
    const label = ROUTE_LABELS[first]
    const subLabel = ROUTE_SUB_LABELS[first] ?? label

    if (parts.length === 1) {
      return label ?? first.substring(0, 30)
    }

    const sub = parts.slice(1).join('/')
    // 子路径为技术性 slug（如 doc-xxx、id）时，显示为「栏目 · 文档」而非原始 slug，更易读
    const isTechnicalSlug = /^[a-z0-9-]+$/i.test(sub) && (sub.length > 8 || sub.includes('-'))
    const displaySub = isTechnicalSlug
      ? '文档'
      : (sub.length > 25 ? sub.substring(0, 25) + '...' : sub)
    const sectionName = label ?? subLabel ?? first
    return sub ? `${sectionName} · ${displaySub}` : (label ?? first.substring(0, 30))
  }

  return { formatPath, ROUTE_LABELS }
}
