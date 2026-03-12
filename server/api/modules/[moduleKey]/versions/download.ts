import { Module } from '~/types/module'
import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')
  const version = getRouterParam(event, 'version')

  if (!moduleKey || !version) {
    throw createError({
      statusCode: 400,
      statusMessage: '缺少模块标识或版本号'
    })
  }

  try {
    // 检查模块是否存在
    const module = await db.getModuleByKey(moduleKey) as Module
    if (!module) {
      throw createError({
        statusCode: 404,
        statusMessage: '模块不存在'
      })
    }

    // 检查版本是否存在
    const moduleVersion = await db.getModuleVersion(moduleKey, version)
    if (!moduleVersion) {
      throw createError({
        statusCode: 404,
        statusMessage: '版本不存在'
      })
    }

    // 获取用户信息（如果已登录）
    const userId = event.context.user?.id
    const ipAddress = event.node.req.socket.remoteAddress
    const userAgent = event.node.req.headers['user-agent']

    // 记录下载
    await db.recordDownload(moduleKey, version, userId, ipAddress, userAgent)

    // 返回下载链接
    const downloadUrl = `${process.env.BASE_URL || 'http://localhost:3000'}${moduleVersion.package_url}`

    return {
      success: true,
      data: {
        downloadUrl,
        filename: `${moduleKey}-${version}.zip`,
        size: moduleVersion.package_size,
        checksum: moduleVersion.checksum
      },
      message: '下载链接已生成'
    }
  } catch (error) {
    console.error('处理下载请求失败:', error)
    throw createError({
      statusCode: error.statusCode || 500,
      statusMessage: error.statusMessage || '下载失败',
      data: { error: error.message }
    })
  }
})