import { ModuleVersion } from '~/types/module'
import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')
  const query = getQuery(event)
  const { stable, latest, page = 1, pageSize = 10 } = query

  if (!moduleKey) {
    throw createError({
      statusCode: 400,
      statusMessage: '缺少模块标识'
    })
  }

  try {
    // 使用数据库服务获取模块版本列表
    const result = await db.getModuleVersions(moduleKey, {
      stable: stable ? stable === 'true' : undefined,
      latest: latest ? latest === 'true' : undefined,
      page: Number(page),
      pageSize: Number(pageSize)
    })

    // 处理数据格式
    const versions = result.data.map((row: any) => ({
      id: row.id,
      moduleKey: row.module_key,
      version: row.version,
      packageUrl: row.package_url,
      packageSize: row.package_size,
      checksum: row.checksum,
      changelog: row.changelog,
      isLatest: Boolean(row.is_latest),
      isStable: Boolean(row.is_stable),
      publishedAt: row.published_at,
      createdBy: row.created_by,
      createdAt: row.created_at,
      updatedAt: row.updated_at
    } as ModuleVersion))

    return {
      success: true,
      data: versions,
      pagination: result.pagination
    }
  } catch (error) {
    console.error('获取模块版本列表失败:', error)
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块版本列表失败',
      data: { error: error.message }
    })
  }
})