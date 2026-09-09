import { fetchBackendApi } from '~/composables/useBackendFetch'
import type { HomeOverview, HomeArticleCard } from '~/types/home'
import {
  EMPTY_HOME_OVERVIEW,
  buildHomeOverview,
  extractToolsCount,
} from '~/utils/home-overview'

type ArticlesListPayload = {
  List?: unknown[]
  list?: unknown[]
}

/**
 * Home / Work overview — always from .NET (same path local + production static).
 * Do not call Nitro home overview route (missing on OSS/nginx).
 */
export const useHomeOverview = () => {
  const config = useRuntimeConfig()
  const articlesSot = String(config.public.articlesSot || 'git').toLowerCase()

  const { data, pending, error } = useAsyncData(
    'home-overview',
    async (): Promise<HomeOverview> => {
      const [rawProjects, rawTimeline, rawTools, rawArticles] = await Promise.allSettled([
        fetchBackendApi<unknown[]>('/Projects', { timeout: 8000 }),
        fetchBackendApi<unknown[]>('/Timeline', { timeout: 8000 }),
        fetchBackendApi<unknown>('/Toolbox/marketplace', {
          query: { page: 1, pageSize: 1 },
          timeout: 8000,
        }),
        // Articles：仅 mysql SoT 走 .NET；git SoT 由内容页自行读 Markdown，不阻塞精选项目
        articlesSot === 'mysql'
          ? fetchBackendApi<ArticlesListPayload>('/Articles', {
              query: { status: 1, pageSize: 50, page: 1 },
              timeout: 8000,
            })
          : Promise.resolve(null),
      ])

      const projects = rawProjects.status === 'fulfilled' && Array.isArray(rawProjects.value)
        ? rawProjects.value
        : []

      const timeline = rawTimeline.status === 'fulfilled' && Array.isArray(rawTimeline.value)
        ? rawTimeline.value
        : []

      const toolsCount = rawTools.status === 'fulfilled'
        ? extractToolsCount(rawTools.value)
        : 0

      let articles: HomeArticleCard[] = []
      let featuredArticle: HomeArticleCard | null = null
      let latestArticles: HomeArticleCard[] = []

      if (articlesSot === 'mysql' && rawArticles.status === 'fulfilled' && rawArticles.value) {
        const list = rawArticles.value.List ?? rawArticles.value.list ?? []
        const mapped = list.map((item) => {
          const a = item as Record<string, unknown>
          const id = Number(a.Id ?? a.id ?? 0)
          const slug = (a.Slug ?? a.slug ?? null) as string | null
          return {
            id,
            title: String(a.Title ?? a.title ?? ''),
            slug,
            summary: (a.Summary ?? a.summary ?? null) as string | null,
            coverUrl: (a.CoverUrl ?? a.coverUrl ?? null) as string | null,
            publishTime: (a.PublishTime ?? a.publishTime ?? a.CreatedAt ?? a.createdAt ?? null) as string | null,
            viewCount: Number(a.ViewCount ?? a.viewCount ?? 0),
            categoryName: (a.CategoryName ?? a.categoryName ?? null) as string | null,
            path: `/work/blog/${slug ?? id}`,
          } satisfies HomeArticleCard
        })
        articles = mapped
        featuredArticle = mapped[0] ?? null
        latestArticles = mapped.filter(a => a.id !== featuredArticle?.id).slice(0, 4)
      }

      return buildHomeOverview({
        projects,
        articles,
        featuredArticle,
        latestArticles,
        timeline,
        toolsCount,
      })
    },
  )

  const overview = computed<HomeOverview>(() => data.value ?? EMPTY_HOME_OVERVIEW)
  const loading = computed(() => pending.value)
  const unavailable = computed(() => Boolean(error.value) && !data.value)

  return { overview, loading, error, unavailable }
}
