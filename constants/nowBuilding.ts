// ⚠️ key = 项目 title 精确匹配，标题变更会导致 progress 失效
// 后续建议改为 id 匹配（需后端返回 id）
export const PROJECT_PROGRESS: Record<string, number> = {
  '心动联谊平台': 68,
  'AI Workflow 系统': 42,
  'Desktop Pet 2.0': 15,
  '个人知识中枢 2.0': 5,
}

export const DEFAULT_PROGRESS = 30
