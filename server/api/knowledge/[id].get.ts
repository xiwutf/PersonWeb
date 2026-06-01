import { normalizeKnowledgeItem } from '../../utils/knowledge'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: '缺少知识库 ID' })
  }

  const config = useRuntimeConfig()
  const base = (config.backendApiBase as string) || 'http://localhost:5234/api'

  try {
    const response = await $fetch<{ code?: number; data?: Record<string, unknown>; message?: string }>(
      `${base}/KnowledgeBase/${id}`,
      { timeout: 5000 },
    )

    if (response.code !== undefined && response.code !== 0) {
      throw createError({
        statusCode: response.code === 404 ? 404 : 502,
        statusMessage: response.message || '获取知识库详情失败',
      })
    }

    const raw = (response.code === 0 ? response.data : response) as Record<string, unknown>
    if (!raw || !raw.Id && !raw.id) {
      throw createError({ statusCode: 404, statusMessage: '未找到知识库条目' })
    }

    return normalizeKnowledgeItem(raw)
  } catch (error: unknown) {
    if (error && typeof error === 'object' && 'statusCode' in error) {
      throw error
    }
    throw createError({ statusCode: 502, statusMessage: '知识库服务暂不可用' })
  }
})
