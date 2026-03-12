import { H3Event, readBody, getHeader } from 'h3'
import { PaymentGatewayConfig, PaymentRequest, PaymentResponse, WebhookPayload } from '~/types/payment'

// 支付网关配置
const paymentGateways: Record<string, PaymentGatewayConfig> = {
  alipay: {
    provider: 'alipay',
    apiKey: process.env.ALIPAY_APP_ID || '',
    apiSecret: process.env.ALIPAY_PRIVATE_KEY || '',
    sandbox: process.env.NODE_ENV === 'development',
    webhookSecret: process.env.ALIPAY_WEBHOOK_SECRET || ''
  },
  wechat: {
    provider: 'wechat',
    apiKey: process.env.WECHAT_APP_ID || '',
    apiSecret: process.env.WECHAT_APP_SECRET || '',
    merchantId: process.env.WECHAT_MCH_ID || '',
    sandbox: process.env.NODE_ENV === 'development'
  },
  stripe: {
    provider: 'stripe',
    apiKey: process.env.STRIPE_SECRET_KEY || '',
    sandbox: process.env.NODE_ENV === 'development',
    webhookSecret: process.env.STRIPE_WEBHOOK_SECRET || ''
  }
}

// 创建订单
export async function createPaymentOrder(order: PaymentRequest): Promise<PaymentResponse> {
  try {
    const gatewayConfig = paymentGateways[order.paymentGateway]
    if (!gatewayConfig) {
      throw new Error(`Unsupported payment gateway: ${order.paymentGateway}`)
    }

    switch (order.paymentGateway) {
      case 'alipay':
        return await createAlipayOrder(order, gatewayConfig)
      case 'wechat':
        return await createWechatPayOrder(order, gatewayConfig)
      case 'stripe':
        return await createStripeOrder(order, gatewayConfig)
      default:
        throw new Error(`Payment gateway not implemented: ${order.paymentGateway}`)
    }
  } catch (error) {
    console.error('Failed to create payment order:', error)
    return {
      success: false,
      errorMessage: error instanceof Error ? error.message : 'Unknown error'
    }
  }
}

// 创建支付宝订单
async function createAlipayOrder(order: PaymentRequest, config: PaymentGatewayConfig): Promise<PaymentResponse> {
  // 这里应该集成支付宝SDK
  // 示例实现，实际使用时需要集成真实的支付宝API

  // 生成订单号
  const outTradeNo = `ORDER_${Date.now()}_${order.orderId}`

  // 模拟创建支付宝订单
  return {
    success: true,
    paymentUrl: `https://openapi.alipay.com/gateway.do?out_trade_no=${outTradeNo}`,
    transactionId: outTradeNo,
    qrCode: `data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==` // 示例二维码
  }
}

// 创建微信支付订单
async function createWechatPayOrder(order: PaymentRequest, config: PaymentGatewayConfig): Promise<PaymentResponse> {
  // 这里应该集成微信支付SDK
  // 示例实现，实际使用时需要集成真实的微信支付API

  const outTradeNo = `ORDER_${Date.now()}_${order.orderId}`

  return {
    success: true,
    paymentUrl: `weixin://wxpay/bizpayurl?pr=${Date.now()}`,
    transactionId: outTradeNo,
    qrCode: `data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==` // 示例二维码
  }
}

// 创建Stripe订单
async function createStripeOrder(order: PaymentRequest, config: PaymentGatewayConfig): Promise<PaymentResponse> {
  // 这里应该集成Stripe SDK
  // 示例实现，实际使用时需要集成真实的Stripe API

  const sessionId = `cs_${Date.now()}`

  return {
    success: true,
    paymentUrl: `https://hooks.stripe.com/redirect`, // 实际会是Stripe的支付页面
    transactionId: sessionId
  }
}

// 处理支付回调
export async function handlePaymentWebhook(event: H3Event, gateway: string) {
  try {
    const config = paymentGateways[gateway]
    if (!config) {
      throw new Error(`Unsupported payment gateway: ${gateway}`)
    }

    // 获取请求体
    const body = await readBody(event)
    const signature = getHeader(event, 'x-signature') || ''

    // 验证签名
    if (!verifyWebhookSignature(body, signature, config.webhookSecret)) {
      throw new Error('Invalid webhook signature')
    }

    // 解析webhook payload
    const payload: WebhookPayload = JSON.parse(body)

    // 更新订单状态
    await updateOrderStatus(payload)

    return { success: true }
  } catch (error) {
    console.error('Webhook handling failed:', error)
    throw error
  }
}

// 验证webhook签名
function verifyWebhookSignature(payload: string, signature: string, secret: string): boolean {
  // 这里应该实现具体的签名验证逻辑
  // 示例实现，实际需要根据支付网关的具体要求实现
  return true
}

// 更新订单状态，支付成功时自动创建许可证
async function updateOrderStatus(payload: WebhookPayload) {
  const { query } = await import('~/server/services/database')
  const { createLicense } = await import('~/server/services/license')

  const status = payload.status === 'success' ? 'paid' :
    payload.status === 'failed' ? 'cancelled' : 'cancelled'

  const updateData: Record<string, unknown> = {
    status,
    transaction_id: payload.transactionId,
    updated_at: new Date().toISOString()
  }

  // 支付成功时：创建许可证并回写 order.license_key
  if (status === 'paid') {
    const orders = await query(
      'SELECT * FROM `order` WHERE id = ? AND status = ?',
      [payload.orderId, 'pending']
    ) as any[]

    if (orders[0] && !orders[0].license_key) {
      const order = orders[0]
      const license = await createLicense({
        orderId: order.id,
        moduleKey: order.module_key,
        userId: order.user_id,
        type: order.type === 'subscription' ? 'subscription' : order.type === 'trial' ? 'trial' : 'permanent',
        maxActivations: order.max_activations || 1
      })
      updateData.license_key = license.licenseKey
      updateData.valid_from = new Date().toISOString()
      if (license.validUntil) updateData.valid_until = license.validUntil
    }
  }

  await query(
    'UPDATE `order` SET ? WHERE id = ?',
    [updateData, payload.orderId]
  )
}

// 检查支付状态
export async function checkPaymentStatus(orderId: number, gateway: string) {
  // 这里应该查询支付网关获取最新的支付状态
  // 示例实现，实际需要调用支付网关API

  const { query } = await import('~/server/services/database')

  const orders = await query(
    'SELECT * FROM `order` WHERE id = ?',
    [orderId]
  ) as any[]

  return orders[0] || null
}

// 生成订单号
export function generateOrderNumber(): string {
  const timestamp = Date.now()
  const random = Math.floor(Math.random() * 1000000)
  return `MODULE_${timestamp}_${random}`
}

/**
 * 在数据库中创建订单记录（支付前先落库）
 */
export async function createOrderRecord(params: {
  userId: number
  moduleKey: string
  version: string
  type: 'module' | 'subscription' | 'trial'
  price: number
  currency?: string
  paymentMethod?: string
  paymentGateway?: string
}): Promise<{ orderId: number; orderNumber: string }> {
  const { query } = await import('~/server/services/database')

  const orderNumber = generateOrderNumber()
  const insertData = {
    order_number: orderNumber,
    user_id: params.userId,
    module_key: params.moduleKey,
    version: params.version,
    type: params.type,
    price: params.price,
    currency: params.currency || 'CNY',
    status: 'pending',
    payment_method: params.paymentMethod || null,
    payment_gateway: params.paymentGateway || null,
    max_activations: 1,
    activations_used: 0
  }

  const result = await query(
    'INSERT INTO `order` SET ?',
    [insertData]
  ) as { insertId: number }

  return { orderId: result.insertId, orderNumber }
}

// 计算折扣金额
export function calculateDiscount(
  amount: number,
  coupon?: {
    type: 'fixed' | 'percentage'
    value: number
    maxUses?: number
    uses: number
    validUntil?: string
    minAmount?: number
  }
): number {
  if (!coupon) return 0

  // 检查优惠码是否过期
  if (coupon.validUntil && new Date(coupon.validUntil) < new Date()) {
    return 0
  }

  // 检查最小订单金额
  if (coupon.minAmount && amount < coupon.minAmount) {
    return 0
  }

  let discount = 0

  if (coupon.type === 'fixed') {
    discount = Math.min(coupon.value, amount)
  } else if (coupon.type === 'percentage') {
    discount = amount * (coupon.value / 100)
  }

  return Math.floor(discount)
}