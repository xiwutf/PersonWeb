export interface HomeStats {
  projects: number
  articles: number
  tools: number
}

export interface HomeProjectCard {
  id: string
  title: string
  description: string
  techStack: string[]
  coverUrl: string | null
  demoUrl: string | null
  githubUrl: string | null
  status: string        // 'Active' | 'Completed' | 'Archived'
  viewCount: number
}

export interface HomeArticleCard {
  id: number
  title: string
  slug: string | null
  summary: string | null
  coverUrl: string | null
  publishTime: string | null
  viewCount: number
  categoryName: string | null
  path: string          // /blog/${slug ?? id}
}

export interface HomeJourneyItem {
  id: number
  year: number
  title: string
  description: string | null
  icon: string | null
  color: string | null
  isNow: boolean        // year === new Date().getFullYear()
}

export interface HomeNowBuildingItem {
  id: string
  title: string
  description: string
  techStack: string[]
  status: string
  progress: number      // 来自 constants/nowBuilding.ts
}

export interface HomeOverview {
  stats: HomeStats
  featuredProjects: HomeProjectCard[]     // ViewCount DESC，最多 5 条，排除 Archived
  featuredArticle: HomeArticleCard | null // CreatedAt DESC 第 1 篇（已发布）
  latestArticles: HomeArticleCard[]       // CreatedAt DESC，最多 4 篇，按 id 排除 featuredArticle
  nowBuilding: HomeNowBuildingItem[]      // status=Active，UpdatedAt DESC，最多 4 条
  journey: HomeJourneyItem[]              // timeline_event，year ASC
}
