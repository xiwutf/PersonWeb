import { ModuleConfig } from '~/types/module'
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
    // 使用数据库服务获取模块配置
    const configs = await db.getModuleConfigs(moduleKey)

    // 转换数据格式
    const formattedConfigs: ModuleConfig[] = configs.map((row: any) => ({
      id: row.id,
      moduleKey: row.module_key,
      configKey: row.config_key,
      configValue: row.config_value,
      description: row.description,
      createdAt: row.created_at,
      updatedAt: row.updated_at
    }))

    return {
      success: true,
      data: formattedConfigs
    }
  } catch (error) {
    console.error('获取模块配置失败:', error)
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块配置失败',
      data: { error: error.message }
    })
  }
})