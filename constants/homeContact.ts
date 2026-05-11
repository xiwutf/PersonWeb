export const CONTACT_METHODS = [
  {
    title: '邮箱',
    value: 'linxiwanting@gmail.com',
    note: '工作日 24h 内回复',
    action: '发送邮件',
    href: 'mailto:linxiwanting@gmail.com',
    icon: 'icon-mail'
  },
  {
    title: '微信',
    value: '扫码添加',
    note: '备注「合作」优先通过',
    action: '查看二维码',
    href: '/contact',
    icon: 'icon-wechat'
  },
  {
    title: '预约会议',
    value: '适合项目咨询与深度沟通',
    note: '',
    action: '预约时间',
    href: '/contact',
    icon: 'icon-calendar'
  }
] as const

export const COLLAB_DIRECTIONS = [
  { title: 'AI 应用开发', description: 'AI 工具、智能体、自动化系统开发', icon: 'collab-ai', href: '/lab' },
  { title: '产品合作', description: '产品共创、联合开发、资源互补', icon: 'collab-product', href: '/products' },
  { title: '内容与知识', description: '技术内容共创、知识付费、课程合作', icon: 'collab-knowledge', href: '/blog' },
  { title: '咨询与顾问', description: 'AI 落地咨询、产品咨询、技术顾问', icon: 'collab-consult', href: '/contact' }
] as const
