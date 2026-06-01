export type ProjectShowcaseTone = 'blue' | 'cyan' | 'green' | 'purple' | 'orange' | 'pink'

export interface ProjectShowcaseStat {
  icon: string
  value: string
  label: string
  tone?: ProjectShowcaseTone
}

export interface ProjectShowcaseFeature {
  icon: string
  title: string
  description: string
}

export interface ProjectShowcaseScreenshot {
  url: string
  caption: string
}

export interface ProjectShowcaseMilestone {
  date: string
  title: string
}

export interface ProjectShowcaseChallenge {
  title: string
  challenge: string
  solution: string
}

export interface ProjectShowcaseRoadmapItem {
  version: string
  status: 'completed' | 'active' | 'planned'
  statusLabel: string
  items: string[]
}

export interface ProjectShowcaseArchitectureLayer {
  label: string
  icon: string
}

export interface ProjectShowcaseBackground {
  title: string
  paragraphs: string[]
  imageUrl?: string
  highlights?: string[]
}

export interface ProjectShowcaseCta {
  title: string
  subtitle: string
}

export interface ProjectShowcaseHeroFloat {
  label: string
  value: string
  tone?: ProjectShowcaseTone
}

export interface ProjectShowcasePitch {
  /** 这个项目是做什么的 */
  what: string
  /** 解决什么问题 */
  problems: string[]
  /** 达到什么效果（用数据/结果说话） */
  outcomes: ProjectShowcaseStat[]
}

/** 项目详情页模块化展示数据（可由 content JSON 或预设填充） */
export interface ProjectShowcaseData {
  heroEyebrow?: string
  heroFloats?: ProjectShowcaseHeroFloat[]
  pitch: ProjectShowcasePitch
  overview: ProjectShowcaseStat[]
  background: ProjectShowcaseBackground
  features: ProjectShowcaseFeature[]
  screenshots: ProjectShowcaseScreenshot[]
  architecture: ProjectShowcaseArchitectureLayer[]
  timeline: ProjectShowcaseMilestone[]
  challenges: ProjectShowcaseChallenge[]
  achievements: ProjectShowcaseStat[]
  roadmap: ProjectShowcaseRoadmapItem[]
  cta: ProjectShowcaseCta
}

export type ProjectShowcasePartial = Partial<ProjectShowcaseData>
