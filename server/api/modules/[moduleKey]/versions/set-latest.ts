import { Module } from '~/types/module'
import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')
  const version = getRouterParam(event, 'version')
  const body = await readBody(event)

  if (!moduleKey || !version) {
    throw createError({
      statusCode: 400,
      statusMessage: '缺少模块标识或版本号'
    })
  }

  try {
    // 验证权限
    // if (!event.context.user || event.context.user.role !== 'admin') {
    //   throw createError({
    //     statusCode: 403,
    //     statusMessage: '需要管理员权限'
    //   })
    // }

    // 验证模块是否存在
    const module = await db.getModuleByKey(moduleKey)
    if (!module) {
      throw createError({
        statusCode: 404,
        statusMessage: '模块不存在'
      })
    }

    // 验证版本是否存在
    const moduleVersion = await db.getModuleVersion(moduleKey, version)
    if (!moduleVersion) {
      throw createError({
        statusCode: 404,
        statusMessage: '版本不存在'
      })
    }

    // 更新最新版本标记
    const updated = await db.setLatestVersion(moduleKey, version)

    if (!updated) {
      throw createError({
        statusCode: 500,
        statusMessage: '设置最新版本失败'
      })
    }

    // 同时更新模块表的版本号
    await db.updateModule(moduleKey, {
      module_version: version
    })

    return {
      success: true,
      data: {
        moduleKey,
        version,
        isLatest: true
      },
      message: '已成功设置为最新版本'
    }
  } catch (error) {
    console.error('设置最新版本失败:', error)
    throw createError({
      statusCode: error.statusCode || 500,
      statusMessage: error.statusMessage || '设置最新版本失败',
      data: { error: error.message }
    })
  }
})