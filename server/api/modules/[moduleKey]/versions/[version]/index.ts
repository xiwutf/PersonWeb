import { ModuleVersion } from '~/types/module'
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
    // 使用数据库服务获取模块版本信息
    const versionRow = await db.getModuleVersion(moduleKey, version)

    if (!versionRow) {
      throw createError({
        statusCode: 404,
        statusMessage: '模块版本不存在'
      })
    }

    // 处理数据格式
    const moduleVersion: ModuleVersion = {
      id: versionRow.id,
      moduleKey: versionRow.module_key,
      version: versionRow.version,
      packageUrl: versionRow.package_url,
      packageSize: versionRow.package_size,
      checksum: versionRow.checksum,
      changelog: versionRow.changelog,
      isLatest: Boolean(versionRow.is_latest),
      isStable: Boolean(versionRow.is_stable),
      publishedAt: versionRow.published_at,
      createdBy: versionRow.created_by,
      createdAt: versionRow.created_at,
      updatedAt: versionRow.updated_at
    }

    return {
      success: true,
      data: moduleVersion
    }
  } catch (error) {
    console.error('获取模块版本详情失败:', error)
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块版本详情失败',
      data: { error: error.message }
    })
  }
})