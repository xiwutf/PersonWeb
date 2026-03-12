import { H3Event } from 'h3'
import { activateLicense } from '~/server/services/license'
import { ActivateLicenseRequest } from '~/types/payment'
import { checkAuth } from '~/server/utils/auth'

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

  const body = await readBody<ActivateLicenseRequest>(event)

  if (!body.licenseKey || !body.deviceId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'License key and device ID are required'
    })
  }

  try {
    const result = await activateLicense(body, event)

    return {
      success: true,
      data: result
    }
  } catch (error) {
    console.error('License activation failed:', error)
    throw createError({
      statusCode: 500,
      statusMessage: error instanceof Error ? error.message : 'License activation failed'
    })
  }
})