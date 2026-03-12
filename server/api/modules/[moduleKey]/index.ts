import { Module } from '~/types/module'
import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')

  if (!moduleKey) {
    throw createError({
      statusCode: 400,
      statusMessage: '缺少模块标识'
    })
  }

  try {
    // 使用数据库服务获取模块详情
    const moduleRow = await db.getModuleByKey(moduleKey)

    if (!moduleRow) {
      throw createError({
        statusCode: 404,
        statusMessage: '模块不存在'
      })
    }

    // 处理数据格式
    const module: Module = {
      id: moduleRow.id,
      moduleKey: moduleRow.module_key,
      moduleName: moduleRow.module_name,
      moduleVersion: moduleRow.module_version,
      description: moduleRow.description,
      author: moduleRow.author,
      icon: moduleRow.icon,
      category: moduleRow.category,
      dependencies: JSON.parse(moduleRow.dependencies || '[]'),
      routes: JSON.parse(moduleRow.routes || '[]'),
      components: JSON.parse(moduleRow.components || '[]'),
      permissions: JSON.parse(moduleRow.permissions || '{}'),
      configSchema: JSON.parse(moduleRow.config_schema || '{}'),
      isEnabled: Boolean(moduleRow.is_enabled),
      isCore: Boolean(moduleRow.is_core),
      sort: moduleRow.sort,
      createdAt: moduleRow.created_at,
      updatedAt: moduleRow.updated_at
    }

    return {
      success: true,
      data: module
    }
  } catch (error) {
    console.error('获取模块详情失败:', error)
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块详情失败',
      data: { error: error.message }
    })
  }
})