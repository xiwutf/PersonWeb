import { useApi } from './useApi'
import type {
  IntelligenceSource,
  SourceRequest,
  ContentWithAnalysis,
  ContentQueryParams,
  DailyReport,
  DashboardStats,
  TaskLog,
  ContentItemDto,
  CategoryStatDto,
  ReportResponseDto,
  TaskLogDto,
  TaskTriggerResponseDto,
  ContentListResponseDto
} from '~/types/intelligence'

export const useIntelligenceApi = () => {
  const { get, post, put, del } = useApi()

  // ==================== 来源管理 ====================

  /**
   * 获取来源列表
   */
  const getSourceList = () => {
    return get<IntelligenceSource[]>('/intelligence/sources')
  }

  /**
   * 获取来源详情
   */
  const getSource = (id: number) => {
    return get<IntelligenceSource>(`/intelligence/sources/${id}`)
  }

  /**
   * 新增来源
   */
  const createSource = (data: SourceRequest) => {
    return post<IntelligenceSource>('/intelligence/sources', data)
  }

  /**
   * 更新来源
   */
  const updateSource = (id: number, data: SourceRequest) => {
    return put<IntelligenceSource>(`/intelligence/sources/${id}`, data)
  }

  /**
   * 删除来源
   */
  const deleteSource = (id: number) => {
    return del(`/intelligence/sources/${id}`)
  }

  /**
   * 启用/禁用来源
   */
  const toggleSource = (id: number, enabled: boolean) => {
    return put(`/intelligence/sources/${id}/enabled`, { enabled })
  }

  // ==================== 内容查询 ====================

  /**
   * 获取内容列表
   */
  const getContentList = (params: ContentQueryParams) => {
    return get<{
      total: number
      list: ContentItemDto[]
    }>('/intelligence/contents', params)
  }

  /**
   * 获取内容详情
   */
  const getContentDetail = (id: number) => {
    return get<ContentItemDto>(`/intelligence/contents/${id}`)
  }

  /**
   * 获取分类统计
   */
  const getCategoryStats = () => {
    return get<{ category: string; count: number }[]>('/intelligence/contents/stats')
  }

  // ==================== 日报查询 ====================

  /**
   * 获取日报列表
   */
  const getReportList = (params?: { pageIndex?: number; pageSize?: number }) => {
    return get<{
      total: number
      list: ReportResponseDto[]
    }>('/intelligence/reports/daily', params)
  }

  /**
   * 获取日报详情
   */
  const getReportDetail = (id: number) => {
    return get<ReportResponseDto>(`/intelligence/reports/daily/${id}`)
  }

  /**
   * 获取最新日报
   */
  const getLatestReport = () => {
    return get<ReportResponseDto>('/intelligence/reports/daily/latest')
  }

  // ==================== 任务管理 ====================

  /**
   * 手动执行采集任务
   */
  const runCollectTask = () => {
    return post<{ taskId: string; message: string }>('/intelligence/tasks/collect')
  }

  /**
   * 手动执行分析任务
   */
  const runAnalyzeTask = () => {
    return post<{ taskId: string; message: string }>('/intelligence/tasks/analyze')
  }

  /**
   * 手动生成日报
   */
  const runGenerateReport = () => {
    return post<{ taskId: string; message: string }>('/intelligence/tasks/generate-daily-report')
  }

  /**
   * 获取任务日志
   */
  const getTaskLogs = (params?: { pageIndex?: number; pageSize?: number }) => {
    return get<{
      total: number
      list: TaskLogDto[]
    }>('/intelligence/tasks/logs', params)
  }

  /**
   * 获取仪表盘数据
   */
  const getDashboardStats = () => {
    return get<DashboardStats>('/intelligence/dashboard')
  }

  return {
    // 来源
    getSourceList,
    getSource,
    createSource,
    updateSource,
    deleteSource,
    toggleSource,
    // 内容
    getContentList,
    getContentDetail,
    getCategoryStats,
    // 日报
    getReportList,
    getReportDetail,
    getLatestReport,
    // 任务
    runCollectTask,
    runAnalyzeTask,
    runGenerateReport,
    getTaskLogs,
    // 仪表盘
    getDashboardStats
  }
}
