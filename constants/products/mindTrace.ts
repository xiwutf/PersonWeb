export interface MindTraceFeature {
  icon: string
  title: string
  description: string
}

export interface MindTraceStep {
  title: string
  desc: string
}

export const MIND_TRACE = {
  name: 'MindTrace',
  slug: 'mindtrace',
  tagline: '记录你的思考轨迹',
  description:
    '浏览网页时快速记录灵感，并自动保留思考上下文。MindTrace 是一款帮助你沉淀「思考轨迹」的 Chrome 浏览器扩展。',
  chromeWebStoreUrl:
    'https://chromewebstore.google.com/detail/MindTrace/doeegakabaiompgehlfoeadgnlgendoo?hl=zh-CN',
  extensionId: 'doeegakabaiompgehlfoeadgnlgendoo',
  badge: 'Chrome 扩展 · 已上架',
  slogan: '让每一次灵感，都不止于当下。',

  features: [
    {
      icon: 'fas fa-lightbulb',
      title: '随时捕捉灵感',
      description: '在浏览网页时一键记录想法，不打断当前阅读与思考节奏。'
    },
    {
      icon: 'fas fa-link',
      title: '自动保留上下文',
      description: '关联当前页面与场景信息，日后回看时仍能还原当时的思考背景。'
    },
    {
      icon: 'fas fa-route',
      title: '串联思考轨迹',
      description: '把零散记录连成可追溯的思考路径，方便复盘与二次创作。'
    },
    {
      icon: 'fas fa-shield-halved',
      title: '轻量本地优先',
      description: '以浏览器侧能力为主，专注记录体验，不额外打断你的工作流。'
    }
  ] as MindTraceFeature[],

  installSteps: [
    { title: '打开 Chrome 网上应用店', desc: '点击下方按钮进入 MindTrace 扩展详情页。' },
    { title: '添加到 Chrome', desc: '在商店页点击「添加至 Chrome」，按提示完成安装。' },
    { title: '固定扩展图标', desc: '安装后在工具栏固定 MindTrace，浏览时即可随时记录。' }
  ] as MindTraceStep[]
}
