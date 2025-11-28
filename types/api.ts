/**
 * API 接口类型定义
 */

// ==================== 文章相关 ====================

export interface Article {
  id: number
  title: string
  slug?: string
  summary?: string
  contentMd?: string
  contentHtml?: string
  coverUrl?: string
  categoryId?: number
  status: number
  tags?: string[]
  createdAt: string
  updatedAt: string
  publishTime?: string
  viewCount: number
  categoryName?: string
  authorId?: number
}

export interface ArticleRequest {
  id?: number
  title: string
  slug?: string
  summary?: string
  contentMd?: string
  contentHtml?: string
  coverUrl?: string
  categoryId?: number
  status: number
  tags?: string[]
}

export interface ArticleListResponse {
  Total: number
  List: Article[]
}

// ==================== 分类相关 ====================

export interface Category {
  id: number
  name: string
  slug?: string
  description?: string
  createdAt?: string
  updatedAt?: string
}

// ==================== 项目相关 ====================

export interface Project {
  id: string
  title: string
  description: string
  coverUrl?: string
  demoUrl?: string
  githubUrl?: string
  status: string
  techStack?: string | string[]
  content?: string
  viewCount?: number
  createdAt: string
  updatedAt: string
}

export interface ProjectRequest {
  title: string
  description: string
  coverUrl?: string
  demoUrl?: string
  githubUrl?: string
  status: string
  techStack?: string | string[]
  content?: string
}

// ==================== 时间胶囊相关 ====================

export interface TimeCapsule {
  id: number
  content: string
  visitorId?: string
  visitorName?: string
  status: number
  createdAt: string
  updatedAt?: string
}

export interface TimeCapsuleRequest {
  content: string
  visitorId?: string
  visitorName?: string
}

export interface TimeCapsuleListResponse {
  Total?: number
  total?: number
  List?: TimeCapsule[]
  list?: TimeCapsule[]
}

// ==================== 知识库相关 ====================

export interface KnowledgeBase {
  id: number
  title: string
  content?: string
  category?: string
  tags?: string
  version: number
  parentId?: number
  status: number
  viewCount: number
  createdAt: string
  updatedAt: string
  authorId?: number
}

export interface KnowledgeBaseRequest {
  title: string
  content?: string
  category?: string
  tags?: string
}

// ==================== 时间线相关 ====================

export interface TimelineEvent {
  id: number
  year: number
  title: string
  description?: string
  icon?: string
  color?: string
  sortOrder: number
  status: number
  createdAt: string
  updatedAt: string
}

export interface TimelineEventRequest {
  year: number
  title: string
  description?: string
  icon?: string
  color?: string
}

// ==================== 统计数据相关 ====================

export interface Stats {
  articleCount: number
  projectCount: number
  todayVisits: number
  totalVisitors: number
}

// ==================== 投资相关 ====================

export interface Investment {
  id: number
  code: string
  name: string
  type: string
  quantity: number
  costPrice: number
  currentPrice: number
  totalCost: number
  marketValue: number
  profitLoss: number
  profitRate: number
  notes?: string
  userId?: number
}

export interface InvestmentRequest {
  code: string
  name: string
  type: string
  quantity: number
  costPrice: number
  currentPrice?: number
  notes?: string
}

// ==================== 媒体上传相关 ====================

export interface MediaUploadResponse {
  url: string
  filename?: string
  size?: number
}

// ==================== 搜索相关 ====================

export interface SearchResultItem {
  id: string
  title: string
  summary: string
  content: string
  type: 'article' | 'project' | 'knowledge'
  url: string
  createdAt: string
  category?: string
}

export interface SearchResults {
  keyword: string
  type: string
  total: number
  articles: SearchResultItem[]
  projects: SearchResultItem[]
  knowledgeBases: SearchResultItem[]
}

// ==================== API 响应格式 ====================

export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
}

