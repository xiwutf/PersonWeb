/**
 * 情报中心类型定义
 */

// ==================== 情报来源 ====================

export interface IntelligenceSource {
  id: number
  sourceName: string
  sourceType: 'RSS' | 'WEB'
  sourceUrl: string
  category: string
  tags: string[]
  priority: number
  enabled: boolean
  fetchIntervalMinutes: number
  remark?: string
  lastFetchTime?: string
  createdAt: string
  updatedAt: string
}

export interface SourceRequest {
  sourceName: string
  sourceType: 'RSS' | 'WEB'
  sourceUrl: string
  category: string
  tags: string[]
  priority: number
  enabled: boolean
  fetchIntervalMinutes: number
  remark?: string
}

// ==================== 情报内容 ====================

export interface IntelligenceContent {
  id: number
  sourceId: number
  sourceName: string
  title: string
  originalUrl: string
  publishTime?: string
  author?: string
  rawText: string
  cleanText: string
  contentHash: string
  fetchStatus: 'pending' | 'success' | 'failed'
  analyzeStatus: 'pending' | 'processing' | 'success' | 'failed'
  createdAt: string
  updatedAt: string
}

export interface ContentAnalysis {
  id: number
  contentId: number
  category: string
  summary: string
  corePoints: string[]
  tags: string[]
  relevanceScore: number
  learningValue: '高' | '中' | '低'
  businessValue: '高' | '中' | '低'
  actionSuggestion?: string
  modelName: string
  createdAt: string
}

export interface ContentWithAnalysis extends IntelligenceContent {
  analysis?: ContentAnalysis
}

// ==================== 每日报告 ====================

export interface DailyReport {
  id: number
  reportDate: string
  title: string
  contentMarkdown: string
  itemCount: number
  generatedAt: string
  createdAt: string
  updatedAt: string
}

// ==================== 任务日志 ====================

export interface TaskLog {
  id: number
  taskName: string
  taskType: string
  status: 'running' | 'success' | 'failed'
  startTime: string
  endTime?: string
  message: string
  detailJson?: string
  createdAt: string
}

// ==================== 请求参数 ====================

export interface ContentQueryParams {
  keyword?: string
  category?: string
  sourceId?: number
  startDate?: string
  endDate?: string
  highValueOnly?: boolean
  pageIndex?: number
  pageSize?: number
}

export interface DashboardStats {
  todayCollected: number
  todayHighValue: number
  latestReport?: DailyReport
  recentReports: DailyReport[]
  recentContents: ContentWithAnalysis[]
  categoryStats: { category: string; count: number }[]
  recentTaskStatus?: TaskLog
}

// ==================== DTO 响应 ====================

export interface ContentItemDto {
  id: number
  sourceId: number
  sourceName: string
  title: string
  originalUrl: string
  publishTime?: string
  summary: string
  category: string
  tags: string[]
  relevanceScore: number
  learningValue: string
  businessValue: string
  analyzeStatus: string
  createdAt: string
}

export interface ReportResponseDto {
  id: number
  reportDate: string
  title: string
  contentMarkdown: string
  itemCount: number
  generatedAt: string
}

export interface CategoryStatDto {
  category: string
  count: number
}

export interface TaskLogDto {
  id: number
  taskName: string
  taskType: string
  status: string
  startTime: string
  endTime?: string
  message: string
  createdAt: string
}

export interface TaskTriggerResponseDto {
  taskId: string
  message: string
}

export interface ContentListResponseDto {
  total: number
  list: ContentItemDto[]
}
