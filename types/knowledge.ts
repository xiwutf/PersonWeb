export interface KnowledgeItem {
  id: number
  title: string
  category: string | null
  tags: string | null
  viewCount: number
  createdAt: string | null
  updatedAt: string | null
  content?: string | null
}

export interface KnowledgeListResponse {
  total: number
  list: KnowledgeItem[]
}
