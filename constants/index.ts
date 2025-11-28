/**
 * 常量定义
 */

// ==================== 文章状态 ====================
export const ARTICLE_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  ARCHIVED: 2
} as const

export type ArticleStatus = typeof ARTICLE_STATUS[keyof typeof ARTICLE_STATUS]

// ==================== 时间胶囊状态 ====================
export const TIME_CAPSULE_STATUS = {
  PENDING: 0,
  APPROVED: 1,
  REJECTED: 2
} as const

export type TimeCapsuleStatus = typeof TIME_CAPSULE_STATUS[keyof typeof TIME_CAPSULE_STATUS]

// ==================== 项目状态 ====================
export const PROJECT_STATUS = {
  ACTIVE: 'Active',
  COMPLETED: 'Completed',
  ARCHIVED: 'Archived'
} as const

export type ProjectStatus = typeof PROJECT_STATUS[keyof typeof PROJECT_STATUS]

// ==================== 知识库状态 ====================
export const KNOWLEDGE_BASE_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  ARCHIVED: 2
} as const

export type KnowledgeBaseStatus = typeof KNOWLEDGE_BASE_STATUS[keyof typeof KNOWLEDGE_BASE_STATUS]

// ==================== 知识库分类 ====================
export const KNOWLEDGE_BASE_CATEGORY = {
  DEVELOPMENT_NOTES: '开发笔记',
  PITFALL_RECORDS: '踩坑记录',
  IDEA_INSPIRATIONS: '想法灵感'
} as const

export type KnowledgeBaseCategory = typeof KNOWLEDGE_BASE_CATEGORY[keyof typeof KNOWLEDGE_BASE_CATEGORY]

// ==================== 时间线事件状态 ====================
export const TIMELINE_EVENT_STATUS = {
  HIDDEN: 0,
  VISIBLE: 1
} as const

export type TimelineEventStatus = typeof TIMELINE_EVENT_STATUS[keyof typeof TIMELINE_EVENT_STATUS]

// ==================== 分页默认值 ====================
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_PAGE_SIZE: 10,
  MAX_PAGE_SIZE: 100
} as const

// ==================== 文件上传限制 ====================
export const FILE_UPLOAD = {
  MAX_IMAGE_SIZE: 5 * 1024 * 1024, // 5MB
  ALLOWED_IMAGE_TYPES: ['image/jpeg', 'image/png', 'image/gif', 'image/webp']
} as const

