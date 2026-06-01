import { normalizeKnowledgeItem, unwrapBackendList } from '../../utils/knowledge'

export default defineEventHandler(async (event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=120, s-maxage=120')

  const query = getQuery(event)
  const config = useRuntimeConfig()
  const base = (config.backendApiBase as string) || 'http://localhost:5234/api'

  try {
    const response = await $fetch<{ code?: number; data?: unknown }>(`${base}/KnowledgeBase`, {
      query: {
        category: query.category || undefined,
        keyword: query.keyword || undefined,
        tag: query.tag || undefined,
        page: query.page || 1,
        pageSize: query.pageSize || 20,
      },
      timeout: 5000,
    })

    const { total, list } = unwrapBackendList(response)

    return {
      total,
      list: list.map(item => normalizeKnowledgeItem(item)),
    }
  } catch {
    return {
      total: 0,
      list: [],
    }
  }
})
