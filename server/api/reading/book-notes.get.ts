/**
 * 公开读书笔记代理 → .NET GET /api/reading/book-notes
 */
export default defineEventHandler(async (event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=30, s-maxage=30')

  const config = useRuntimeConfig()
  const base = String(config.backendApiBase || 'http://localhost:5234/api').replace(/\/$/, '')

  try {
    const response = await $fetch<{
      code?: number
      data?: unknown
    }>(`${base}/reading/book-notes`, { timeout: 8000 })

    if (response && typeof response === 'object' && response.code !== undefined && response.code !== 0) {
      return { items: [] as unknown[] }
    }

    const raw = response?.data
    return { items: Array.isArray(raw) ? raw : [] }
  }
  catch {
    return { items: [] as unknown[] }
  }
})
