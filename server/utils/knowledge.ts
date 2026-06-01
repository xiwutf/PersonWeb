import type { KnowledgeItem } from '../../types/knowledge'

export type { KnowledgeItem, KnowledgeListResponse } from '../../types/knowledge'

export function normalizeKnowledgeItem(raw: Record<string, unknown>): KnowledgeItem {
  return {
    id: Number(raw.Id ?? raw.id ?? 0),
    title: String(raw.Title ?? raw.title ?? ''),
    category: (raw.Category ?? raw.category ?? null) as string | null,
    tags: (raw.Tags ?? raw.tags ?? null) as string | null,
    viewCount: Number(raw.ViewCount ?? raw.viewCount ?? 0),
    createdAt: (raw.CreatedAt ?? raw.createdAt ?? null) as string | null,
    updatedAt: (raw.UpdatedAt ?? raw.updatedAt ?? null) as string | null,
    content: (raw.Content ?? raw.content ?? null) as string | null,
  }
}

export function unwrapBackendList(payload: unknown): { total: number; list: Record<string, unknown>[] } {
  const data = (payload as { code?: number; data?: unknown })?.code === 0
    ? (payload as { data: unknown }).data
    : payload

  const record = (data ?? {}) as Record<string, unknown>
  const list = (record.List ?? record.list ?? []) as Record<string, unknown>[]
  const total = Number(record.Total ?? record.total ?? list.length)

  return { total, list: Array.isArray(list) ? list : [] }
}
