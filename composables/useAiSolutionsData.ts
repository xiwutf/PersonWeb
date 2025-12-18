/**
 * AI 解决方案页面数据配置
 * 将页面数据从组件中分离，便于维护和复用
 */

// 能力展示数据类型
export interface Capability {
  id: string
  title: string
  icon: string
  features: string[]
}

// 项目案例类型
export interface FeaturedProject {
  id: string
  title: string
  icon: string
  description: string
  highlights: string[]
  path: string | null
}

// 技术栈分类类型
export interface TechStackCategory {
  name: string
  icon: string
  items: string[]
}

// 合作流程步骤类型
export interface CooperationStep {
  title: string
  description: string
}

// 页面配置类型
export interface AiSolutionsPageConfig {
  // 页面基础信息
  badge: {
    text: string
  }
  title: string
  subtitle: string
  description: string
  
  // SEO 信息
  seo: {
    title: string
    description: string
  }
  
  // 能力展示
  capabilities: Capability[]
  
  // 项目案例
  featuredProjects: FeaturedProject[]
  
  // 技术栈
  techStackCategories: TechStackCategory[]
  
  // 合作流程
  cooperationSteps: CooperationStep[]
  
  // CTA 区域
  cta: {
    text: string
    primaryButton: {
      text: string
      path: string
      icon: string
    }
    secondaryButton: {
      text: string
      anchor: string
      icon: string
    }
  }
  
  // 区块标题
  sectionTitles: {
    capabilities: string
    capabilitiesIcon: string
    projects: string
    projectsIcon: string
    projectsNote: string
    projectsDescription: string
    techStack: string
    techStackIcon: string
    cooperation: string
    cooperationIcon: string
  }
}

/**
 * 获取 AI 解决方案页面配置数据
 */
export const useAiSolutionsData = (): AiSolutionsPageConfig => {
  return {
    // 页面基础信息
    badge: {
      text: 'AI / 智能体解决方案'
    },
    title: 'AI / 智能体解决方案',
    subtitle: '为个人与中小团队打造可落地的 AI 应用与智能体系统',
    description: '我专注于将 AI 技术落地为真实可用的工具与系统，覆盖智能体构建、知识库问答、流程自动化与 AI 工具开发，适用于个人创作者、小团队与中小企业的实际业务场景。',
    
    // SEO 信息
    seo: {
      title: 'AI / 智能体解决方案 | 溪午听风',
      description: '为个人与中小团队打造可落地的 AI 应用与智能体系统。智能体构建、知识库问答、流程自动化与 AI 工具开发。'
    },
    
    // 能力展示数据
    capabilities: [
      {
        id: 'agent-system',
        title: '智能体系统开发',
        icon: 'fas fa-robot',
        features: [
          '多角色智能体设计',
          '任务拆解与自动执行',
          '工具调用与流程编排',
          '支持长期记忆与上下文管理'
        ]
      },
      {
        id: 'ai-tools',
        title: 'AI 工具 / 助手定制',
        icon: 'fas fa-magic',
        features: [
          '智能取名、内容生成',
          '表单 / 数据处理自动化',
          '规则 + AI 混合逻辑',
          '私有化部署支持'
        ]
      },
      {
        id: 'knowledge-qa',
        title: '知识库智能问答',
        icon: 'fas fa-database',
        features: [
          '文档 / 网站 / 私有数据接入',
          '向量化与语义检索',
          '多轮对话与精准引用',
          '适用于企业内部或个人知识管理'
        ]
      },
      {
        id: 'integration',
        title: 'AI + 网站 / 系统集成',
        icon: 'fas fa-plug',
        features: [
          'AI 客服 / 网站聊天助手',
          '管理后台 AI 辅助',
          '与现有系统无缝集成',
          '前后端完整交付'
        ]
      }
    ],
    
    // 代表性项目案例
    featuredProjects: [
      {
        id: 'name-tool',
        title: '智能取名助手',
        icon: 'fas fa-sparkles',
        description: '基于规则与大模型的智能取名系统，支持行业、风格、禁用词等多维度控制。',
        highlights: [
          'Prompt 模板化管理',
          '多轮生成与去重逻辑',
          '可扩展为品牌命名 / 产品命名工具'
        ],
        path: '/tools/name'
      },
      {
        id: 'website-chat',
        title: '网站 AI 聊天助手',
        icon: 'fas fa-comments',
        description: '为个人网站与产品站点提供定制化 AI 问答能力。',
        highlights: [
          '系统 Prompt 与用户 Prompt 分离',
          '支持上下文与历史消息',
          '可对接知识库与业务接口'
        ],
        path: null
      },
      {
        id: 'finance-assistant',
        title: '个人理财 / 资产分析智能助手（规划中）',
        icon: 'fas fa-chart-line',
        description: '用于个人资产统计、分析与长期理财规划的 AI 辅助系统。',
        highlights: [
          '多资产类型建模',
          '数据分析 + AI 解释',
          '长期可演进为个人理财助手'
        ],
        path: null
      }
    ],
    
    // 技术栈分类
    techStackCategories: [
      {
        name: 'AI / 模型层',
        icon: 'fas fa-brain',
        items: [
          '主流大模型（OpenAI / 国产模型等）',
          'Prompt Engineering',
          '多模型策略与降本方案'
        ]
      },
      {
        name: '后端与服务编排',
        icon: 'fas fa-server',
        items: [
          'Python / .NET WebAPI',
          'LangChain / 向量数据库',
          '工具调用、任务流设计',
          '权限与安全控制'
        ]
      },
      {
        name: '前端与交互',
        icon: 'fas fa-desktop',
        items: [
          'Vue 3 / Nuxt 3',
          '管理后台与用户交互设计',
          'AI 结果可视化'
        ]
      }
    ],
    
    // 合作流程步骤
    cooperationSteps: [
      {
        title: '需求沟通',
        description: '明确你的业务场景与目标'
      },
      {
        title: '方案设计',
        description: 'AI 能力拆解与技术方案制定'
      },
      {
        title: '开发实现',
        description: '敏捷开发，阶段性可验收'
      },
      {
        title: '交付与迭代',
        description: '支持后期优化与功能扩展'
      }
    ],
    
    // CTA 区域配置
    cta: {
      text: '如果你正在寻找一个<br>能真正把 AI 做成"工具"和"系统"的开发者<br>欢迎与我交流你的想法。',
      primaryButton: {
        text: '联系我',
        path: '/about',
        icon: 'fas fa-arrow-right'
      },
      secondaryButton: {
        text: '查看项目',
        anchor: '#projects',
        icon: 'fas fa-arrow-down'
      }
    },
    
    // 区块标题配置
    sectionTitles: {
      capabilities: '我能为你构建哪些 AI 能力',
      capabilitiesIcon: 'fas fa-lightbulb',
      projects: '部分 AI 项目与实践',
      projectsIcon: 'fas fa-project-diagram',
      projectsNote: '（可持续新增，不求多，但求真实）',
      projectsDescription: '部分项目仍在持续迭代中，但核心能力已具备并可复用。',
      techStack: '技术栈与架构能力',
      techStackIcon: 'fas fa-code',
      cooperation: '合作流程',
      cooperationIcon: 'fas fa-handshake'
    }
  }
}

