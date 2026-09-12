/**
 * Work 世界信息架构 — 导航与路由治理的单一配置源。
 * 详见 docs/INFORMATION_ARCHITECTURE.md
 */

export type WorkNavItem = {
  title: string
  path: string
  key?: string
}

/** Desktop / Mobile 一级导航（目标 4–6 项，不含 CTA / More） */
export const WORK_PRIMARY_NAV: WorkNavItem[] = [
  { title: '首页', path: '/work', key: 'home' },
  { title: '案例', path: '/work/projects', key: 'projects' },
  { title: '产品', path: '/work/products', key: 'products' },
  { title: '文章', path: '/work/blog', key: 'blog' },
  { title: '关于', path: '/work/about', key: 'about' },
]

/**
 * More：低频但有价值的入口。
 * 禁止重复 Primary / CTA；禁止塞 Dashboard 等私人页。
 */
export const WORK_MORE_NAV: WorkNavItem[] = [
  { title: '工具', path: '/work/tools' },
  { title: 'AI 方案', path: '/work/ai' },
  { title: '实验室', path: '/work/lab' },
  { title: '技能', path: '/work/skills' },
  { title: '知识笔记', path: '/work/knowledge' },
]

/** 顶栏唯一主 CTA（联系）；AI 放在 More，避免双 CTA 抢注意力 */
export const WORK_HEADER_CTA: WorkNavItem = {
  title: '联系合作',
  path: '/work/contact',
}

export const WORK_WORLD_LINKS = {
  portal: { title: 'Portal', path: '/' },
  work: { title: 'Work', path: '/work' },
  life: { title: 'Life', path: '/life' },
} as const

export type FooterSection = {
  title: string
  items: WorkNavItem[]
}

export const WORK_FOOTER_SECTIONS: FooterSection[] = [
  {
    title: '探索',
    items: [
      { title: 'Work 首页', path: '/work' },
      { title: '案例', path: '/work/projects' },
      { title: '产品', path: '/work/products' },
      { title: '文章', path: '/work/blog' },
    ],
  },
  {
    title: '更多',
    items: [
      { title: '工具', path: '/work/tools' },
      { title: 'AI 方案', path: '/work/ai' },
      { title: '实验室', path: '/work/lab' },
      { title: '模块商店', path: '/work/module-store' },
    ],
  },
  {
    title: '世界',
    items: [
      { title: 'Portal', path: '/' },
      { title: 'Life', path: '/life' },
      { title: '关于', path: '/work/about' },
      { title: '联系', path: '/work/contact' },
    ],
  },
]

/** 实体多视图映射（允许一物多页，但职责不同） */
export const WORK_ENTITY_CROSS_VIEWS = [
  {
    id: 'mindtrace',
    name: 'MindTrace',
    productPath: '/work/products/mindtrace',
    /** 尚无独立 Case Study 详情 ID；案例墙卡片桥接到产品页 */
    projectPath: null as string | null,
    projectListBridge: true,
    productAnswers: '它是什么、有什么用、怎么安装使用',
    projectAnswers: '能力证明入口暂由案例墙卡片承接（链到产品页）',
  },
] as const

export type RouteGovernanceAction =
  | 'KEEP'
  | 'REDIRECT'
  | 'NOINDEX'
  | 'HIDE_NAV'
  | 'LEGACY'
  | 'PRIVATE'

export type RouteGovernanceRule = {
  path: string
  action: RouteGovernanceAction
  to?: string
  note?: string
}

export const WORK_ROUTE_GOVERNANCE: RouteGovernanceRule[] = [
  { path: '/work', action: 'KEEP', note: 'PRIMARY Work hub' },
  { path: '/work/projects', action: 'KEEP', note: 'PRIMARY cases' },
  { path: '/work/products', action: 'KEEP', note: 'PRIMARY products' },
  { path: '/work/blog', action: 'KEEP', note: 'PRIMARY writing' },
  { path: '/work/about', action: 'KEEP', note: 'PRIMARY identity' },
  { path: '/work/contact', action: 'KEEP', note: 'PRIMARY CTA' },
  { path: '/work/tools', action: 'HIDE_NAV', note: 'SECONDARY — More only' },
  { path: '/work/ai', action: 'HIDE_NAV', note: 'SECONDARY commercial AI — More only' },
  { path: '/work/lab', action: 'HIDE_NAV', note: 'EXPERIMENT — More only' },
  { path: '/ai', action: 'REDIRECT', to: '/work/ai', note: 'LEGACY root AI' },
  { path: '/lab', action: 'REDIRECT', to: '/work/lab', note: 'LEGACY root lab' },
  { path: '/projects', action: 'REDIRECT', to: '/work/projects', note: 'LEGACY root cases' },
  { path: '/products', action: 'REDIRECT', to: '/work/products', note: 'LEGACY root products' },
  { path: '/blog', action: 'REDIRECT', to: '/work/blog', note: 'LEGACY root writing' },
  { path: '/about', action: 'REDIRECT', to: '/work/about', note: 'LEGACY root identity' },
  { path: '/contact', action: 'REDIRECT', to: '/work/contact', note: 'LEGACY root CTA' },
  { path: '/tools', action: 'REDIRECT', to: '/work/tools', note: 'LEGACY root tools' },
  { path: '/knowledge', action: 'REDIRECT', to: '/work/knowledge', note: 'LEGACY root notes' },
  { path: '/skills', action: 'REDIRECT', to: '/work/skills', note: 'LEGACY root skills' },
  { path: '/cognition', action: 'REDIRECT', to: '/life/cognition', note: 'LEGACY root methodology → Life' },
  { path: '/work/cognition', action: 'REDIRECT', to: '/life/cognition', note: 'methodology 归入 Life' },
  { path: '/module-store', action: 'REDIRECT', to: '/work/module-store', note: 'LEGACY root modules' },
  { path: '/side-projects', action: 'REDIRECT', to: '/work/projects', note: 'LEGACY side-projects 已下线 → 案例' },
  { path: '/work/side-projects', action: 'REDIRECT', to: '/work/projects', note: '副业展示页已下线 → 案例' },
  { path: '/game', action: 'REDIRECT', to: '/work/game', note: 'LEGACY root experiment' },
  { path: '/english', action: 'REDIRECT', to: '/work/english', note: 'LEGACY root english' },
  { path: '/links', action: 'REDIRECT', to: '/work/links', note: 'LEGACY root links' },
  { path: '/changelog', action: 'REDIRECT', to: '/work/changelog', note: 'LEGACY root changelog' },
  { path: '/pricing', action: 'REDIRECT', to: '/work/pricing', note: 'LEGACY root pricing' },
  { path: '/download', action: 'REDIRECT', to: '/work/download', note: 'LEGACY root download' },
  { path: '/work/knowledge', action: 'HIDE_NAV', note: 'SECONDARY notes — More only' },
  { path: '/work/skills', action: 'HIDE_NAV', note: 'SECONDARY — More only' },
  { path: '/work/module-store', action: 'HIDE_NAV', note: 'Developer modules — Footer 更多' },
  { path: '/work/game', action: 'HIDE_NAV', note: 'EXPERIMENT' },
  { path: '/work/english', action: 'HIDE_NAV', note: 'EXPERIMENT / 偏 Life' },
  { path: '/dashboard', action: 'PRIVATE', note: '数字分身 → 未来 Life；noindex + 退出导航' },
  { path: '/showcase', action: 'REDIRECT', to: '/work', note: 'LEGACY demo wall' },
  { path: '/ai-intro', action: 'REDIRECT', to: '/work/ai', note: 'LEGACY AI character intro' },
  { path: '/search', action: 'KEEP', note: 'UTILITY' },
  { path: '/work/links', action: 'HIDE_NAV', note: 'UTILITY' },
  { path: '/work/changelog', action: 'HIDE_NAV', note: '桌宠附属 UTILITY' },
  { path: '/work/pricing', action: 'HIDE_NAV', note: '桌宠附属' },
  { path: '/work/download', action: 'HIDE_NAV', note: '桌宠附属' },
]

export const WORK_PRIMARY_NAV_MAX = 6

export function collectNavPaths(items: WorkNavItem[]): string[] {
  return items.map((item) => item.path)
}

export function findDuplicatePaths(groups: WorkNavItem[][]): string[] {
  const seen = new Set<string>()
  const dupes = new Set<string>()
  for (const group of groups) {
    for (const item of group) {
      if (seen.has(item.path)) dupes.add(item.path)
      seen.add(item.path)
    }
  }
  return [...dupes]
}

/** Primary 与 More / CTA 不得路径重复 */
export function assertWorkNavIntegrity(): {
  primaryCount: number
  duplicatesPrimaryMoreCta: string[]
  moreContainsPrivate: boolean
} {
  const primaryPaths = collectNavPaths(WORK_PRIMARY_NAV)
  const morePaths = collectNavPaths(WORK_MORE_NAV)
  const ctaPath = WORK_HEADER_CTA.path

  const duplicatesPrimaryMoreCta = findDuplicatePaths([
    WORK_PRIMARY_NAV,
    WORK_MORE_NAV,
    [WORK_HEADER_CTA],
  ])

  return {
    primaryCount: primaryPaths.length,
    duplicatesPrimaryMoreCta,
    moreContainsPrivate: morePaths.includes('/dashboard') || morePaths.includes('/admin'),
  }
}
