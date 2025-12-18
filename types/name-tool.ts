/**
 * 智能取名助手类型定义
 */

// 取名类型
export type NameType = 'game' | 'nickname' | 'english'

// 风格类型
export type NameStyle = '霸气' | '可爱' | '文艺' | '搞笑' | '克制' | '科幻' | '二次元' | '古风' | '赛博'

// 性别类型
export type Gender = '男' | '女' | '中性'

// 名字长度类型
export type NameLength = '短' | '中' | '长'

// 语言类型
export type Language = '中文' | '英文' | '混合' | '自动'

// 名字评分维度
export interface NameScores {
  memorability: number // 好记度 0-100
  uniqueness: number // 独特性 0-100
  fit: number // 贴合度 0-100
  aesthetics: number // 美观度 0-100
}

// 名字项
export interface NameItem {
  name: string
  totalScore: number // 总分 0-100
  scores: NameScores
  reason: string // 评分理由（不超过40字）
  tags?: string[] // 标签，如["古风", "短", "偏霸气"]
}

// 生成名字请求
export interface NameGenerateRequest {
  type: NameType // 必填
  style: NameStyle[] // 必填，多选
  gender?: Gender // 可选
  length?: NameLength // 可选
  keywords?: string // 可选，多个关键词用逗号分隔
  language?: Language // 可选，默认自动
  banned?: string // 可选，禁用词，多个用逗号分隔
  traceId?: string // 可选，用于"再来一批"时保持风格一致
}

// 生成名字响应
export interface NameGenerateResponse {
  traceId: string // 追踪ID，用于"再来一批"
  items: NameItem[] // 生成的名字列表（20个）
}

// 收藏项
export interface NameFavorite {
  id: number
  name: string
  type: NameType
  style: string // 多风格用逗号存储
  language: string
  totalScore: number
  reason: string
  metaJson?: string // 存储维度分、输入条件等
  createdAt: string
  updatedAt: string
}

// 创建收藏请求
export interface FavoriteCreateRequest {
  name: string
  type: NameType
  style: string[]
  language: string
  totalScore: number
  reason: string
  scores: NameScores
  requestMeta?: Partial<NameGenerateRequest> // 保存输入条件摘要
}

// 收藏列表响应
export interface FavoriteListResponse {
  items: NameFavorite[]
  total: number
  page: number
  pageSize: number
}

// 排序方式
export type SortBy = 'totalScore' | 'uniqueness' | 'memorability' | 'fit' | 'aesthetics'

