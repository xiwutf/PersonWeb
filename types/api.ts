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
  tools?: SearchResultItem[]
  themes?: SearchResultItem[]
}

// ==================== 友情链接相关 ====================

export interface FriendLink {
  id: number
  name: string
  url: string
  description?: string
  logoUrl?: string
  sortOrder: number
  status: number
  createdAt: string
  updatedAt: string
}

export interface FriendLinkRequest {
  name: string
  url: string
  description?: string
  logoUrl?: string
  sortOrder?: number
  status?: number
}

// ==================== 后台风格相关 ====================

export interface AdminGlobalStyle {
  id: number
  styleKey: string
  styleName: string
  description?: string
  styleConfig: string
  previewImage?: string
  enabled: boolean
  isDefault: boolean
  createdAt: string
  updatedAt: string
}

export interface AdminModuleStyle {
  id: number
  moduleKey: string
  moduleName: string
  styleConfig: string
  enabled: boolean
  isDefault: boolean
  createdAt: string
  updatedAt: string
}

export interface StyleCategory {
  id: number
  name: string
  code: string
  description?: string
  sort: number
  createdAt: string
  updatedAt: string
}

export interface StyleDefinition {
  id: number
  categoryId: number
  name: string
  code: string
  cssClass: string
  backgroundColor?: string
  borderColor?: string
  textColor?: string
  fontSize?: string
  fontWeight?: string
  padding?: string
  borderRadius?: string
  borderWidth?: string
  customCss?: string
  description?: string
  isActive: boolean
  sort: number
  createdAt: string
  updatedAt: string
}

export interface StyleUsage {
  id: number
  styleId: number
  pagePath: string
  componentName?: string
  usageCount: number
  lastUsedAt?: string
  createdAt: string
  updatedAt: string
}

export interface StyleUsageStats {
  styleId: number
  styleCode: string
  styleName: string
  categoryName: string
  totalUsage: number
  pageCount: number
  componentCount: number
  pages: string[]
  components: string[]
  lastUsedAt?: string
}

// ==================== 兼职项目相关 ====================

export interface SideProject {
  id: number
  title: string
  description?: string
  clientName?: string
  clientContact?: string
  source?: string
  category?: string
  incomeType?: string // 'development' | 'investment' - 收入类型：软件开发/投资
  techStack?: string
  budgetMin?: number
  budgetMax?: number
  priceFinal?: number
  status: number // 0 进行中 / 1 已完成 / 2 待付款 / 3 已取消
  startTime?: string
  endTime?: string
  isPublic: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateSideProjectDto {
  title: string
  description?: string
  clientName?: string
  clientContact?: string
  source?: string
  category?: string
  incomeType?: string // 'development' | 'investment'
  techStack?: string
  budgetMin?: number
  budgetMax?: number
  priceFinal?: number
  status?: number
  startTime?: string
  endTime?: string
  isPublic?: boolean
}

export interface UpdateSideProjectDto {
  title?: string
  description?: string
  clientName?: string
  clientContact?: string
  source?: string
  category?: string
  incomeType?: string // 'development' | 'investment'
  techStack?: string
  budgetMin?: number
  budgetMax?: number
  priceFinal?: number
  status?: number
  startTime?: string
  endTime?: string
  isPublic?: boolean
}

export interface ProjectDashboardSummaryDto {
  totalIncome: number
  totalProjects: number
  avgProjectPrice: number
  avgDurationDays: number
  // 按收入类型分类统计
  developmentIncome?: number // 软件开发收入
  developmentProjects?: number // 软件开发项目数
  investmentIncome?: number // 投资收入
  investmentProjects?: number // 投资项目数
}

export interface IncomeTrendPointDto {
  date: string
  income: number
}

export interface CategoryDistributionItemDto {
  category: string
  count: number
  income: number
}

export interface TechStackDistributionItemDto {
  tech: string
  count: number
  income: number
}

export interface ClientSourceItemDto {
  source: string
  count: number
  income: number
}

export interface DurationBucketItemDto {
  bucketName: string
  count: number
}

export interface SideProjectListResponse {
  Total: number
  List: SideProject[]
}

// ==================== API 响应格式 ====================

export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
}

