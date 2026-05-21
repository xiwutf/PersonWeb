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
  contentHtml?: string
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
  contentHtml?: string
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
  stage?: string // 阶段：待开始/进行中/卡住/待验收/已完成
  progress?: number // 进度 0-100
  isProgressManual?: boolean // 进度是否手动覆盖
  priority?: number // 优先级：0=低，1=中，2=高，3=紧急
  deadlineAt?: string // 截止时间
  nextAction?: string // 下一步行动
  blocked?: boolean // 是否阻塞
  blockReason?: string // 阻塞原因
  totalAmount?: number // 总金额
  receivedAmount?: number // 已收款金额
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
  stage?: string
  progress?: number
  isProgressManual?: boolean
  priority?: number
  deadlineAt?: string
  nextAction?: string
  blocked?: boolean
  blockReason?: string
  totalAmount?: number
  receivedAmount?: number
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
  stage?: string
  progress?: number
  isProgressManual?: boolean
  priority?: number
  deadlineAt?: string
  nextAction?: string
  blocked?: boolean
  blockReason?: string
  totalAmount?: number
  receivedAmount?: number
}

// 项目需求
export interface SideProjectRequirement {
  id: number
  projectId: number
  scopeIn?: string
  scopeOut?: string
  acceptanceCriteria?: string
  deliverables?: string
  createdAt: string
  updatedAt: string
}

// 项目任务
export interface SideProjectTask {
  id: number
  projectId: number
  title: string
  description?: string
  status: number // 0=未开始，1=进行中，2=已完成，3=已取消
  priority?: number // 优先级：0=低，1=中，2=高，3=紧急
  dueAt?: string
  estHours?: number
  actHours?: number
  sortOrder: number
  createdAt: string
  updatedAt: string
}

// 项目里程碑
export interface SideProjectMilestone {
  id: number
  projectId: number
  title: string
  dueAt?: string
  status: number // 0=未完成，1=已完成
  notes?: string
  createdAt: string
  updatedAt: string
}

// 项目沟通记录
export interface SideProjectLog {
  id: number
  projectId: number
  channel?: string // 沟通渠道：微信/邮件/电话/会议/其他
  content?: string
  nextTodo?: string
  createdAt: string
}

// 项目附件
export interface SideProjectAttachment {
  id: number
  projectId: number
  type?: string // 附件类型：文档/图片/代码/其他
  name: string
  url: string
  createdAt: string
}

// 项目详情（包含所有子实体）
export interface SideProjectDetail extends SideProject {
  requirements: SideProjectRequirement[]
  tasks: SideProjectTask[]
  milestones: SideProjectMilestone[]
  logs: SideProjectLog[]
  attachments: SideProjectAttachment[]
}

// 仪表盘数据
export interface SideProjectDashboard {
  todayTasks: SideProjectTask[] // 今日待办（跨项目任务）
  inProgressProjects: SideProject[] // 进行中项目
  blockedProjects: SideProject[] // 卡住项目
  thisWeekMilestones: SideProjectMilestone[] // 本周里程碑
  totalIncome: number // 收入汇总
  thisMonthIncome: number // 本月收入
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

// ==================== 数据分析相关 ====================

// 数据分析汇总
export interface SideProjectAnalyticsSummary {
  kpis: AnalyticsKpis
  statusAgg: StatusAggregation[]
  monthlyRevenue: MonthlyRevenue[]
  deliveryCycle: DeliveryCycle[]
  customerTop: CustomerTop[]
  riskDueSoon: ProjectBrief[]
  riskOverdue: ProjectBrief[]
}

// KPI 指标
export interface AnalyticsKpis {
  totals: number // 项目总数
  inProgressCount: number // 进行中项目数
  overdueCount: number // 逾期项目数
  blockedCount: number // 卡住项目数
  receivedSum: number // 本期已收金额
  receivableSum: number // 本期待收金额
}

// 状态聚合
export interface StatusAggregation {
  status: number
  statusName: string
  count: number
  amountSum: number
}

// 月度收入
export interface MonthlyRevenue {
  month: string // YYYY-MM
  receivedSum: number
}

// 交付周期
export interface DeliveryCycle {
  groupName: string
  avgDays: number
  count: number
}

// 客户贡献 Top
export interface CustomerTop {
  customerName: string
  projectCount: number
  receivedSum: number
}

// 项目简要信息（用于风险列表）
export interface ProjectBrief {
  id: number
  title: string
  clientName?: string
  deadlineAt?: string
  daysRemaining?: number // 剩余天数（负数表示已逾期）
  nextAction?: string
  totalAmount?: number
  blockReason?: string
  overdueDays?: number // 逾期天数（仅逾期项目有值）
}

// ==================== 站内提醒相关 ====================

// 提醒类型枚举
export enum NotificationType {
  ProjectDueSoon = 1, // 项目即将到期
  TaskDueToday = 2, // 任务今天到期
  ProjectBlockedTooLong = 3 // 项目卡住超过2天
}

// 提醒严重程度枚举
export enum NotificationSeverity {
  Info = 1, // 信息
  Warning = 2, // 警告
  Danger = 3 // 危险/紧急
}

// 站内提醒
export interface SideNotification {
  id: number
  userId?: number
  type: NotificationType
  title: string
  content?: string
  severity: NotificationSeverity
  entityType: string // SideProject / SideProjectTask
  entityId: number
  payloadJson?: string // JSON 格式的负载数据
  isRead: boolean
  readAt?: string
  isDismissed: boolean
  dismissedAt?: string
  snoozeUntil?: string // 延后到某个时间再出现
  occurDate: string // 发生日期（用于去重）
  firstTriggeredAt: string
  lastTriggeredAt: string
  triggerCount: number
  createdAt: string
}

// 提醒列表查询参数
export interface NotificationQueryParams {
  status?: 'unread' | 'all' | 'dismissed' // 状态筛选
  severity?: 'info' | 'warning' | 'danger' // 严重程度筛选
  type?: NotificationType // 类型筛选
  keyword?: string // 关键字搜索
  page?: number
  pageSize?: number
}

// 提醒列表响应
export interface NotificationListResponse {
  items: SideNotification[]
  total: number
  page: number
  pageSize: number
  unreadCount: number
}

// 延后提醒请求
export interface SnoozeNotificationRequest {
  snoozeUntil?: string // 延后到指定时间
  preset?: '1d' | '3d' | 'nextMon' // 预设延后选项
}

// ==================== API 响应格式 ====================

export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
}

