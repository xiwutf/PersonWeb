import { listAggregatedArticles } from '../../utils/articles-aggregate'
import { adaptArticlesToHomeCards } from '../../utils/home-articles-adapter'
import {
  buildHomeOverview,
  extractToolsCount,
} from '../../../utils/home-overview'
import type { HomeOverview, HomeArticleCard } from '../../../types/home'

/**
 * Legacy Nitro aggregator — kept for local tooling / tests.
 * Frontend Work pages use useHomeOverview → .NET directly (production static has no Nitro).
 */
export default defineEventHandler(async (event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=300, s-maxage=300')

  const config = useRuntimeConfig()
  const base = (config.backendApiBase as string) || 'http://localhost:5234/api'
  const articlesSot = String(config.public.articlesSot || 'git').toLowerCase()

  const [rawProjects, rawArticlesRes, rawTimeline, rawTools, gitArticles] = await Promise.allSettled([
    $fetch<{ code: number; data: any[] }>(`${base}/Projects`, { timeout: 5000 }),
    articlesSot === 'mysql'
      ? $fetch<{ code: number; data: { List: any[]; Total: number } }>(
          `${base}/Articles`,
          { query: { status: 1, pageSize: 50, page: 1 }, timeout: 5000 },
        )
      : Promise.resolve(null),
    $fetch<{ code: number; data: any[] }>(`${base}/Timeline`, { timeout: 5000 }),
    $fetch<{ code: number; data: any }>(
      `${base}/Toolbox/marketplace`,
      { query: { page: 1, pageSize: 1 }, timeout: 5000 },
    ),
    articlesSot === 'git'
      ? listAggregatedArticles({ publicOnly: true, page: 1, pageSize: 50 })
      : Promise.resolve(null),
  ])

  const projects: any[] = rawProjects.status === 'fulfilled'
    ? (Array.isArray(rawProjects.value?.data) ? rawProjects.value.data : [])
    : []

  const timeline: any[] = rawTimeline.status === 'fulfilled'
    ? (Array.isArray(rawTimeline.value?.data) ? rawTimeline.value.data : [])
    : []

  let toolsCount = 0
  if (rawTools.status === 'fulfilled') {
    toolsCount = extractToolsCount(rawTools.value?.data)
  }

  let articles: HomeArticleCard[] = []
  let featuredArticle: HomeArticleCard | null = null
  let latestArticles: HomeArticleCard[] = []

  if (articlesSot === 'git' && gitArticles.status === 'fulfilled' && gitArticles.value) {
    const adapted = adaptArticlesToHomeCards(gitArticles.value.blogList)
    articles = adapted.allArticles
    featuredArticle = adapted.featuredArticle
    latestArticles = adapted.latestArticles
  } else {
    const mysqlArticles: any[] = rawArticlesRes.status === 'fulfilled'
      ? (rawArticlesRes.value?.data?.List ?? [])
      : []
    articles = mysqlArticles.map((a: any) => {
      const id = Number(a.Id ?? a.id ?? 0)
      const slug = a.Slug ?? a.slug ?? null
      return {
        id,
        title: String(a.Title ?? a.title ?? ''),
        slug,
        summary: a.Summary ?? a.summary ?? null,
        coverUrl: a.CoverUrl ?? a.coverUrl ?? null,
        publishTime: a.PublishTime ?? a.publishTime ?? a.CreatedAt ?? a.createdAt ?? null,
        viewCount: Number(a.ViewCount ?? a.viewCount ?? 0),
        categoryName: a.CategoryName ?? a.categoryName ?? null,
        path: `/work/blog/${slug ?? id}`,
      }
    })
    featuredArticle = articles[0] ?? null
    latestArticles = articles
      .filter(a => a.id !== featuredArticle?.id)
      .slice(0, 4)
  }

  return buildHomeOverview({
    projects,
    articles,
    featuredArticle,
    latestArticles,
    timeline,
    toolsCount,
  }) satisfies HomeOverview
})
