import db from '~/server/services/database'

export default defineEventHandler(async (event) => {
  try {
    // 获取查询参数
    const query = getQuery(event)
    const { category, enabled, search, page = 1, pageSize = 10 } = query

    // 使用数据库服务获取模块列表
    const result = await db.getModules({
      category,
      enabled,
      search,
      page: Number(page),
      pageSize: Number(pageSize)
    })

    // 处理数据格式
    const modules = result.data.map((row: any) => ({
      id: row.id,
      moduleKey: row.module_key,
      moduleName: row.module_name,
      moduleVersion: row.module_version,
      description: row.description,
      author: row.author,
      icon: row.icon,
      category: row.category,
      dependencies: JSON.parse(row.dependencies || '[]'),
      routes: JSON.parse(row.routes || '[]'),
      components: JSON.parse(row.components || '[]'),
      permissions: JSON.parse(row.permissions || '{}'),
      configSchema: JSON.parse(row.config_schema || '{}'),
      isEnabled: Boolean(row.is_enabled),
      isCore: Boolean(row.is_core),
      sort: row.sort,
      createdAt: row.created_at,
      updatedAt: row.updated_at
    }))

    return {
      success: true,
      data: modules,
      pagination: result.pagination
    }
  } catch (error) {
    console.error('获取模块列表失败:', error)
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块列表失败',
      data: { error: error.message }
    })
  }
})