import { H3Event } from 'h3'
import { verifyLicense } from '~/server/services/license'
import { VerifyLicenseRequest } from '~/types/payment'

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

  const body = await readBody<VerifyLicenseRequest>(event)

  if (!body.licenseKey) {
    throw createError({
      statusCode: 400,
      statusMessage: 'License key is required'
    })
  }

  try {
    const result = await verifyLicense(body)

    return {
      success: true,
      data: result
    }
  } catch (error) {
    console.error('License verification failed:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'License verification failed'
    })
  }
})