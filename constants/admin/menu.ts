// constants/admin/menu.ts
// 后台定位：数据统计 / 汇总 / 分析 + 互动线索
// 正文内容改前台页面或 Cursor；工具/友链后台页保留直链，不进侧栏

export type AdminMenuLeaf = {
  label: string
  path: string
}

export type AdminMenuGroup = {
  label: string
  icon?: string
  children: AdminMenuLeaf[]
}

export const adminMenu: AdminMenuGroup[] = [
  {
    label: '数据',
    icon: 'fas fa-chart-pie',
    children: [
      { label: '网站概览', path: '/admin' },
      { label: '数据分析', path: '/admin/analytics' },
      { label: 'AI 中心', path: '/admin/ai' },
    ],
  },
  {
    label: '互动',
    icon: 'fas fa-comments',
    children: [
      { label: '访客互动', path: '/admin/visitor-messages' },
      { label: '咨询管理', path: '/admin/consultations' },
    ],
  },
]

/** 菜单中所有可达路径（守卫测试用） */
export const adminMenuPaths = adminMenu.flatMap(group =>
  group.children.map(item => item.path),
)

/**
 * 已从侧栏拿掉、但仍保留直链的运营页。
 */
export const adminHiddenDirectPaths = [
  '/admin/visitors',
  '/admin/projects/stats',
  '/admin/ai/logs',
  '/admin/ai/support-config',
  '/admin/cognition',
  '/admin/orders',
  '/admin/thoughts',
] as const
