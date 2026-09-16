/**
 * 公开阅读摘录代理 → .NET GET /api/reading（published + public）
 */
export default defineEventHandler(async (event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=60, s-maxage=60')

  const config = useRuntimeConfig()
  const base = String(config.backendApiBase || 'http://localhost:5234/api').replace(/\/$/, '')

  try {
    const response = await $fetch<{
      code?: number
      data?: { items?: unknown[], Items?: unknown[] }
      Items?: unknown[]
    }>(`${base}/reading`, { timeout: 8000 })

    if (response && typeof response === 'object' && response.code !== undefined && response.code !== 0) {
      return { items: [] as PublicReadingItem[] }
    }

    const data = response?.data ?? response
    const raw = (data as { items?: unknown[], Items?: unknown[] })?.items
      ?? (data as { items?: unknown[], Items?: unknown[] })?.Items
      ?? []

    const items = (Array.isArray(raw) ? raw : [])
      .map(normalizePublicReadingItem)
      .filter((item): item is PublicReadingItem => Boolean(item))

    return { items }
  }
  catch {
    return { items: [] as PublicReadingItem[] }
  }
})

type PublicReadingItem = {
  id: string
  text: string
  note?: string
  source?: string
  sourceUrl?: string
  createdAt?: string
}

function normalizePublicReadingItem(raw: unknown): PublicReadingItem | null {
  if (!raw || typeof raw !== 'object') {
    return null
  }
  const row = raw as Record<string, unknown>
  const id = String(row.id ?? row.Id ?? '')
  if (!id) {
    return null
  }

  const quote = String(row.quote ?? row.Quote ?? '').trim()
  const note = String(row.note ?? row.Note ?? '').trim()
  const text = quote || note
  if (!text) {
    return null
  }

  const sourceTitle = String(row.sourceTitle ?? row.SourceTitle ?? '').trim()
  const sourceUrl = String(row.sourceUrl ?? row.SourceUrl ?? '').trim()
  const importedAt = row.importedAt ?? row.ImportedAt ?? row.updatedAt ?? row.UpdatedAt

  let createdAt: string | undefined
  if (typeof importedAt === 'string' && importedAt) {
    createdAt = importedAt.slice(0, 10)
  }
  else if (importedAt instanceof Date) {
    createdAt = importedAt.toISOString().slice(0, 10)
  }

  return {
    id,
    text,
    note: quote && note && note !== quote ? note : undefined,
    source: sourceTitle || undefined,
    sourceUrl: sourceUrl || undefined,
    createdAt,
  }
}
