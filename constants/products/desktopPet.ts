export interface FeatureItem {
  icon: string
  title: string
  description: string
}

export interface ScreenshotItem {
  alt: string
  src: string
}

export interface PricingRow {
  feature: string
  free: string | boolean
  pro: string | boolean
}

export interface ChangelogItem {
  version: string
  date: string
  status: 'stable' | 'beta'
  changes: string[]
}

export interface FaqItem {
  question: string
  answer: string
}

export interface DownloadInfo {
  version: string
  platform: string
  releaseDate: string
  fileSize: string
  downloadUrl: string
}

export interface TargetUser {
  title: string
  desc: string
}

export const DESKTOP_PET = {
  name: 'AI 桌面陪伴助手',
  tagline: '打工人的 AI 桌面陪伴助手',
  description: '陪你工作、提醒专注、轻度成长、支持 AI 快捷工具。让冰冷的电脑桌面，变成有温度的工作搭子。',
  positioningTitle: '不只是桌宠，更是你的桌面效率搭子',
  positioningDesc: '它不是单纯卖萌的桌面宠物。它会在你工作时陪伴你，在专注时提醒你，在疲惫时鼓励你，在需要时提供 AI 快捷工具支持。',
  slogan: '让桌面，不再冷冰冰。',

  features: [
    {
      icon: 'fas fa-heart',
      title: '桌面不再冷冰冰',
      description: '随机互动语录、轻量陪伴反馈、角色切换，让你的桌面多一点情绪价值。'
    },
    {
      icon: 'fas fa-clock',
      title: '帮你进入工作状态',
      description: '番茄钟、喝水提醒、久坐提醒、专注模式，让效率更自然发生。'
    },
    {
      icon: 'fas fa-robot',
      title: '需要时，随手帮你一下',
      description: '日报生成、待办整理、文本总结、工作复盘。不打扰你，只在需要时出现。'
    },
    {
      icon: 'fas fa-star',
      title: '用得越久，越有反馈',
      description: '签到、等级成长、角色解锁、在线记录。你的每一次坚持，都被看见。'
    }
  ] as FeatureItem[],

  screenshots: [
    { alt: '宠物主界面', src: '' },
    { alt: '专注系统', src: '' },
    { alt: 'AI 面板', src: '' },
    { alt: '统计页', src: '' },
    { alt: '角色切换', src: '' }
  ] as ScreenshotItem[],

  pricing: [
    { feature: '基础桌宠陪伴', free: true, pro: true },
    { feature: '成长系统', free: true, pro: true },
    { feature: '基础角色', free: true, pro: true },
    { feature: 'AI 快捷助手', free: '每日 3 次', pro: '更高额度' },
    { feature: '高级专注模式', free: false, pro: true },
    { feature: '高级统计', free: false, pro: true },
    { feature: 'Pro 专属角色', free: false, pro: true }
  ] as PricingRow[],

  changelog: [
    {
      version: 'v0.3.0',
      date: '2026-04-10',
      status: 'beta' as const,
      changes: [
        '新增 AI 快捷助手面板',
        '支持日报生成与待办整理',
        '优化专注模式交互体验'
      ]
    },
    {
      version: 'v0.2.0',
      date: '2026-03-20',
      status: 'beta' as const,
      changes: [
        '新增番茄钟与喝水提醒',
        '成长系统上线，支持签到与等级',
        '修复角色切换闪烁问题'
      ]
    },
    {
      version: 'v0.1.0',
      date: '2026-03-01',
      status: 'stable' as const,
      changes: [
        '首个内测版本发布',
        '基础桌宠陪伴功能',
        '支持 Windows 10/11'
      ]
    }
  ] as ChangelogItem[],

  faq: [
    {
      question: '是否收费？',
      answer: '提供免费版，高级功能将提供 Pro 版本。当前阶段可免费体验所有基础功能。'
    },
    {
      question: '是否需要联网？',
      answer: '基础功能无需联网，部分 AI 功能需要联网使用。'
    },
    {
      question: '是否占用电脑资源？',
      answer: '采用轻量化设计，日常办公可安心使用，不影响正常工作。'
    },
    {
      question: '是否支持 Windows 10/11？',
      answer: '支持 Windows 10 及 Windows 11，建议使用最新系统版本获得最佳体验。'
    },
    {
      question: '会持续更新吗？',
      answer: '会。当前已进入持续迭代阶段，后续将陆续加入更多角色、更强 AI 工具等功能。'
    }
  ] as FaqItem[],

  download: {
    version: 'v0.3.0',
    platform: 'Windows',
    releaseDate: '2026-04-10',
    fileSize: '约 45 MB',
    downloadUrl: '#'
  } as DownloadInfo,

  targetUsers: [
    { title: '打工人', desc: '需要轻提醒、轻陪伴、轻效率工具。' },
    { title: '程序员 / 设计师', desc: '长期坐在电脑前，希望桌面更有生命力。' },
    { title: '自由职业者', desc: '需要节奏感、专注感、自我反馈机制。' },
    { title: '独处办公人群', desc: '希望桌面不是只有窗口和文件夹。' }
  ] as TargetUser[],

  iterationRoadmap: [
    '更多角色',
    '更强 AI 工具',
    '数据同步',
    '更完整专注体系',
    '企业版桌面助手能力'
  ]
}
