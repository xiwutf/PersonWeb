import { H3Event } from 'h3'
import { handlePaymentWebhook } from '~/server/services/payment'

export default defineEventHandler(async (event) => {
  try {
    // 处理支付宝支付回调
    const result = await handlePaymentWebhook(event, 'alipay')

    return result
  } catch (error) {
    console.error('Webhook processing failed:', error)

    // 返回失败状态，支付宝会重试
    throw createError({
      statusCode: 400,
      statusMessage: 'Webhook failed'
    })
  }
})