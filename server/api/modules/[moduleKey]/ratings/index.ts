import { ModuleReview } from '~/types/module'
import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')
  const query = getQuery(event)
  const { version, page = 1, pageSize = 10 } = query

  if (!moduleKey) {
    throw createError({
      statusCode: 400,
      statusMessage: '缺少模块标识'
    })
  }

  try {
    // 获取评价列表
    const result = await db.getModuleReviews(moduleKey, version ? version as string : undefined, Number(page), Number(pageSize))

    // 处理数据格式
    const reviews = result.data.map((row: any) => ({
      id: row.id,
      moduleKey: row.module_key,
      version: row.version,
      userId: row.user_id,
      rating: row.rating,
      title: row.title,
      content: row.content,
      isVerified: Boolean(row.is_verified),
      createdAt: row.created_at,
      updatedAt: row.updated_at
    } as ModuleReview))

    return {
      success: true,
      data: reviews,
      pagination: result.pagination
    }
  } catch (error) {
    console.error('获取模块评价失败:', error)
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块评价失败',
      data: { error: error.message }
    })
  }
})