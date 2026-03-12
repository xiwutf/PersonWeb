import { createPaymentOrder, createOrderRecord } from '~/server/services/payment'
import { CreateOrderRequest } from '~/types/payment'
import { checkAuth } from '~/server/utils/auth'

/** 从认证信息中获取用户ID，当前简化实现 */
function getUserIdFromEvent(): number {
  // TODO: 从 JWT/session 解析真实用户 ID
  // 当前 checkAuth 仅验证是否登录，未返回用户信息
  return 1
}

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

  const body = await readBody<CreateOrderRequest>(event)

  // 验证请求参数
  if (!body.moduleKey || !body.version || !body.type) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Missing required parameters: moduleKey, version, type'
    })
  }

  if (typeof body.price !== 'number' || body.price <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Invalid price: must be a positive number'
    })
  }

  try {
    const userId = getUserIdFromEvent()
    const paymentGateway = body.paymentGateway || 'alipay'
    const paymentMethod = body.paymentMethod || paymentGateway

    // 1. 先在数据库创建订单记录
    const { orderId, orderNumber } = await createOrderRecord({
      userId,
      moduleKey: body.moduleKey,
      version: body.version,
      type: body.type,
      price: body.price,
      currency: body.currency,
      paymentMethod,
      paymentGateway
    })

    // 2. 调用支付网关创建支付（amount 单位由各网关定义：支付宝/微信为元，Stripe 为分）
    const paymentRequest = {
      orderId,
      amount: body.price,
      currency: body.currency || 'CNY',
      paymentMethod,
      paymentGateway,
      returnUrl: `${process.env.BASE_URL || ''}/payment/success?orderId=${orderId}`,
      cancelUrl: `${process.env.BASE_URL || ''}/payment/cancel?orderId=${orderId}`
    }

    const response = await createPaymentOrder(paymentRequest)

    if (!response.success) {
      throw createError({
        statusCode: 500,
        statusMessage: response.errorMessage || 'Failed to create payment'
      })
    }

    return {
      success: true,
      data: {
        ...response,
        orderId,
        orderNumber
      }
    }
  } catch (error) {
    if (error && typeof error === 'object' && 'statusCode' in error) {
      throw error
    }
    console.error('Failed to create payment order:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to create payment order'
    })
  }
})