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

  const orderId = parseInt(getRouterParam(event, 'id') as string)
  if (!orderId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Invalid order ID'
    })
  }

  try {
    // 查询订单详情（query 返回行数组）
    const orders = await query(
      'SELECT * FROM `order` WHERE id = ?',
      [orderId]
    ) as any[]

    if (!orders[0]) {
      throw createError({
        statusCode: 404,
        statusMessage: 'Order not found'
      })
    }

    const order = orders[0]

    // 查询关联的许可证
    let license = null
    if (order.license_key) {
      const licenses = await query(
        'SELECT * FROM `license` WHERE license_key = ?',
        [order.license_key]
      ) as any[]
      license = licenses[0] || null
    }

    // 查询支付交易记录
    const transactions = await query(
      'SELECT * FROM `payment_transaction` WHERE order_id = ? ORDER BY created_at DESC',
      [orderId]
    ) as any[]

    return {
      success: true,
      data: {
        order,
        license,
        transactions
      }
    }
  } catch (error) {
    console.error('Failed to fetch order:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch order'
    })
  }
})