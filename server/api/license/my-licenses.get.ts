import { H3Event } from 'h3'
import { getUserLicenses } from '~/server/services/license'
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

  // 简化处理，使用固定用户ID
  const userId = 1

  try {
    const licenses = await getUserLicenses(userId)

    return {
      success: true,
      data: licenses
    }
  } catch (error) {
    console.error('Failed to fetch user licenses:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Failed to fetch user licenses'
    })
  }
})