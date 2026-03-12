import { ModuleReview } from '~/types/module'
import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')
  const body = await readBody(event)

  if (!moduleKey) {
    throw createError({
      statusCode: 400,
      statusMessage: '缺少模块标识'
    })
  }

  try {
    // 验证用户权限
    // if (!event.context.user) {
    //   throw createError({
    //     statusCode: 401,
    //     statusMessage: '需要登录'
    //   })
    // }

    const {
      version,
      rating,
      title,
      content
    } = body

    // 验证评分
    if (!rating || rating < 1 || rating > 5) {
      throw createError({
        statusCode: 400,
        statusMessage: '评分必须是1-5之间的整数'
      })
    }

    // 验证模块是否存在
    const module = await db.getModuleByKey(moduleKey)
    if (!module) {
      throw createError({
        statusCode: 404,
        statusMessage: '模块不存在'
      })
    }

    // 验证版本是否存在（如果指定了版本）
    if (version) {
      const moduleVersion = await db.getModuleVersion(moduleKey, version)
      if (!moduleVersion) {
        throw createError({
          statusCode: 404,
          statusMessage: '模块版本不存在'
        })
      }
    }

    // 检查用户是否已经评价过该版本
    // const existingReview = await db.getModuleReviews(moduleKey, version, 1, 1)
    // if (existingReview.data.length > 0) {
    //   throw createError({
    //     statusCode: 409,
    //     statusMessage: '您已经评价过该版本'
    //   })
    // }

    // 创建评价
    const reviewData = {
      moduleKey,
      version: version || module.moduleVersion,
      userId: event.context.user?.id, // 需要从认证中间件获取
      rating,
      title,
      content,
      isVerified: false // 需要通过购买记录验证
    }

    const reviewId = await db.createReview(reviewData)

    // 获取新创建的评价
    const newReview = await db.getModuleReviews(moduleKey, version || module.moduleVersion, 1, 1)
    const createdReview = newReview.data[0]

    return {
      success: true,
      data: createdReview,
      message: '评价提交成功'
    }
  } catch (error) {
    console.error('创建模块评价失败:', error)
    throw createError({
      statusCode: error.statusCode || 500,
      statusMessage: error.statusMessage || '评价提交失败',
      data: { error: error.message }
    })
  }
})