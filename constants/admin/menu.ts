// constants/admin/menu.ts
// 后台菜单配置：只负责"分组/顺序/名字/路由"，不掺业务逻辑

export type AdminMenuItem = {
  label: string
  path?: string
  children?: AdminMenuItem[]
}

export const adminMenu: AdminMenuItem[] = [
  {
    label: '仪表盘',
    children: [{ label: '后台首页', path: '/admin' }],
  },
  {
    label: '内容中心',
    children: [
      { label: '文章管理', path: '/admin/articles' },
      { label: '项目管理', path: '/admin/projects' },
      { label: '工具管理', path: '/admin/tools' },
      { label: '分类管理', path: '/admin/categories' },
      { label: '认知说明书', path: '/admin/cognition' },
    ],
  },
  {
    label: '数据中心',
    children: [
      { label: '访客数据', path: '/admin/visitors' },
      { label: '访客留言', path: '/admin/visitor-messages' },
      { label: '数据分析', path: '/admin/analytics' },
      { label: '投资管理', path: '/admin/investment' },
      { label: '订单管理', path: '/admin/orders' },
    ],
  },
  {
    label: 'AI & 自动化',
    children: [
      { label: 'AI 管理', path: '/admin/ai' },
      { label: 'AI 内容', path: '/admin/ai/content' },
      // 如果 /admin/ai/logs 真实存在，再打开
      // { label: 'AI 日志', path: '/admin/ai/logs' },
    ],
  },
  {
    label: '个人系统',
    children: [
      { label: '关系管理', path: '/admin/relations' },
      { label: '副业项目', path: '/admin/side-projects' },
      { label: '技能树', path: '/admin/skill-tree' },
      { label: '工具箱', path: '/admin/toolbox' },
    ],
  },
  {
    label: '系统设置',
    children: [
      { label: '系统设置', path: '/admin/settings' },
      { label: '主题设置', path: '/admin/settings/themes' },
      { label: '模块管理', path: '/admin/settings/modules' },
      { label: '用户管理', path: '/admin/users' },
      { label: '错误日志', path: '/admin/error-logs' },
    ],
  },
]
