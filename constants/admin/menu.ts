// constants/admin/menu.ts
// 后台菜单：分组顺序、命名、路由；侧栏分组可带图标（layouts/admin.vue 渲染）

export type AdminMenuLeaf = {
  label: string
  path: string
}

export type AdminMenuGroup = {
  /** 侧栏分组标题 */
  label: string
  /** Font Awesome 分组图标，可选 */
  icon?: string
  children: AdminMenuLeaf[]
}

export const adminMenu: AdminMenuGroup[] = [
  {
    label: '总览',
    icon: 'fas fa-tachometer-alt',
    children: [{ label: '仪表盘', path: '/admin' }],
  },
  {
    label: '内容与站点',
    icon: 'fas fa-layer-group',
    children: [
      { label: '内容中枢', path: '/admin/content-hub' },
      { label: '文章管理', path: '/admin/articles' },
      { label: '项目管理', path: '/admin/projects' },
      { label: '工具管理', path: '/admin/tools' },
      { label: '分类管理', path: '/admin/categories' },
      { label: '认知说明书', path: '/admin/cognition' },
      { label: '思维记录', path: '/admin/thoughts' },
    ],
  },
  {
    label: '访问与洞察',
    icon: 'fas fa-chart-pie',
    children: [
      { label: '访客数据', path: '/admin/visitors' },
      { label: '访客留言', path: '/admin/visitor-messages' },
      { label: '数据分析', path: '/admin/analytics' },
    ],
  },
  {
    label: '商业与订单',
    icon: 'fas fa-coins',
    children: [
      { label: '投资管理', path: '/admin/investment' },
      { label: '订单管理', path: '/admin/orders' },
    ],
  },
  {
    label: 'AI 与自动化',
    icon: 'fas fa-magic',
    children: [
      { label: 'AI 管理', path: '/admin/ai' },
      { label: 'AI 内容', path: '/admin/ai/content' },
      { label: 'AI 日志', path: '/admin/ai/logs' },
    ],
  },
  {
    label: '情报中心',
    icon: 'fas fa-broadcast-tower',
    children: [
      { label: '情报首页', path: '/admin/intelligence' },
      { label: '内容列表', path: '/admin/intelligence/content' },
      { label: '每日简报', path: '/admin/intelligence/daily-report' },
      { label: '来源管理', path: '/admin/intelligence/source' },
    ],
  },
  {
    label: '个人工作台',
    icon: 'fas fa-briefcase',
    children: [
      { label: '关系管理', path: '/admin/relations' },
      { label: '副业项目', path: '/admin/side-projects' },
      { label: '技能树', path: '/admin/skill-tree' },
      { label: '工具箱', path: '/admin/toolbox' },
    ],
  },
  {
    label: '系统与安全',
    icon: 'fas fa-shield-alt',
    children: [
      { label: '系统设置', path: '/admin/settings' },
      { label: '主题设置', path: '/admin/settings/themes' },
      { label: '模块管理', path: '/admin/settings/modules' },
      { label: '用户管理', path: '/admin/users' },
      { label: '错误日志', path: '/admin/error-logs' },
    ],
  },
]
