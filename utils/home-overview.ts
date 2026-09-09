import { PROJECT_PROGRESS, DEFAULT_PROGRESS } from '~/constants/nowBuilding'
import type {
  HomeOverview,
  HomeProjectCard,
  HomeArticleCard,
  HomeJourneyItem,
  HomeNowBuildingItem,
  HomeStats,
} from '~/types/home'
import { parseTechStack } from '~/utils/parseTechStack'

/** Empty overview when API unavailable — never invent stats. */
export const EMPTY_HOME_OVERVIEW: HomeOverview = {
  stats: { projects: 0, articles: 0, tools: 0 },
  featuredProjects: [],
  featuredArticle: null,
  latestArticles: [],
  nowBuilding: [],
  journey: [],
}

export function mapRawProjectsToCards(projects: unknown[]): HomeProjectCard[] {
  return projects.map((raw) => {
    const p = raw as Record<string, unknown>
    return {
      id: String(p.Id ?? p.id ?? ''),
      title: String(p.Title ?? p.title ?? ''),
      description: String(p.Description ?? p.description ?? ''),
      techStack: parseTechStack(p.TechStack ?? p.techStack),
      coverUrl: (p.CoverUrl ?? p.coverUrl ?? null) as string | null,
      demoUrl: (p.DemoUrl ?? p.demoUrl ?? null) as string | null,
      githubUrl: (p.GithubUrl ?? p.githubUrl ?? null) as string | null,
      status: String(p.Status ?? p.status ?? 'Active'),
      viewCount: Number(p.ViewCount ?? p.viewCount ?? 0),
    }
  })
}

export function buildFeaturedProjects(allProjects: HomeProjectCard[]): HomeProjectCard[] {
  return [...allProjects]
    .filter(p => p.status !== 'Archived')
    .sort((a, b) => b.viewCount - a.viewCount)
    .slice(0, 5)
}

export function buildNowBuilding(projects: unknown[]): HomeNowBuildingItem[] {
  return projects
    .filter((raw) => {
      const p = raw as Record<string, unknown>
      return String(p.Status ?? p.status) === 'Active'
    })
    .sort((a, b) => {
      const aRaw = a as Record<string, unknown>
      const bRaw = b as Record<string, unknown>
      const aT = new Date(String(aRaw.UpdatedAt ?? aRaw.updatedAt ?? 0)).getTime()
      const bT = new Date(String(bRaw.UpdatedAt ?? bRaw.updatedAt ?? 0)).getTime()
      return bT - aT
    })
    .slice(0, 4)
    .map((raw) => {
      const p = raw as Record<string, unknown>
      const title = String(p.Title ?? p.title ?? '')
      return {
        id: String(p.Id ?? p.id ?? ''),
        title,
        description: String(p.Description ?? p.description ?? ''),
        techStack: parseTechStack(p.TechStack ?? p.techStack),
        status: String(p.Status ?? p.status ?? 'Active'),
        progress: PROJECT_PROGRESS[title] ?? DEFAULT_PROGRESS,
      }
    })
}

export function buildJourney(timeline: unknown[]): HomeJourneyItem[] {
  const currentYear = new Date().getFullYear()
  return timeline.map((raw) => {
    const t = raw as Record<string, unknown>
    return {
      id: Number(t.Id ?? t.id ?? 0),
      year: Number(t.Year ?? t.year ?? 0),
      title: String(t.Title ?? t.title ?? ''),
      description: (t.Description ?? t.description ?? null) as string | null,
      icon: (t.Icon ?? t.icon ?? null) as string | null,
      color: (t.Color ?? t.color ?? null) as string | null,
      isNow: Number(t.Year ?? t.year) === currentYear,
    }
  })
}

export function extractToolsCount(toolsData: unknown): number {
  if (!toolsData || typeof toolsData !== 'object') return 0
  const data = toolsData as Record<string, unknown>
  return Number(
    data.total
    ?? data.Total
    ?? (Array.isArray(data.tools) ? data.tools.length : 0)
    ?? (Array.isArray(data.Tools) ? data.Tools.length : 0)
    ?? 0,
  )
}

export function buildHomeOverview(input: {
  projects?: unknown[]
  articles?: HomeArticleCard[]
  featuredArticle?: HomeArticleCard | null
  latestArticles?: HomeArticleCard[]
  timeline?: unknown[]
  toolsCount?: number
}): HomeOverview {
  const rawProjects = Array.isArray(input.projects) ? input.projects : []
  const allProjects = mapRawProjectsToCards(rawProjects)
  const articles = input.articles ?? []

  const stats: HomeStats = {
    projects: allProjects.filter(p => p.status !== 'Archived').length,
    articles: articles.length,
    tools: Number(input.toolsCount ?? 0),
  }

  return {
    stats,
    featuredProjects: buildFeaturedProjects(allProjects),
    featuredArticle: input.featuredArticle ?? articles[0] ?? null,
    latestArticles: input.latestArticles ?? articles.slice(1, 5),
    nowBuilding: buildNowBuilding(rawProjects),
    journey: buildJourney(Array.isArray(input.timeline) ? input.timeline : []),
  }
}
