import { H3Event } from 'h3'
import { checkAuth } from '~/server/utils/auth'
import { query } from '~/server/services/database'

export default defineEventHandler(async (event) => {
  // 验证用户身份
  try {
    checkAuth(event)
  } catch (error) {
    throw createError({
      statusCode: 401,
      statusMessage: 'Unauthorized'
    })
  }

  const userId = 1 // 简化处理，使用固定用户ID

  // 获取分页参数
  const page = parseInt(getQuery(event).page as string) || 1
  const limit = parseInt(getQuery(event).limit as string) || 10
  const offset = (page - 1) * limit

  try {
    // 查询订单列表（query 返回行数组，无需解构）
    const orders = await query(
      `SELECT * FROM \`order\`
       WHERE user_id = ?
       ORDER BY created_at DESC
       LIMIT ? OFFSET ?`,
      [userId, limit, offset]
    ) as any[]

    // 查询总数
    const countRows = await query(
      'SELECT COUNT(id) as count FROM `order` WHERE user_id = ?',
      [userId]
    ) as any[]
    const total = countRows[0]?.count ?? 0

    return {
      success: true,
      data: {
        orders,
        pagination: {
          page,
          limit,
          total,
          pages: Math.ceil(total / limit)
        }
      }
    }
  } catch (error) {
    console.error('Failed to fetch orders:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch orders'
    })
  }
})