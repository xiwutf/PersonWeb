/**
 * 数据库连接和查询服务
 * 提供统一的数据库访问接口
 */

import mysql from 'mysql2/promise'

// 数据库配置
const dbConfig = {
  host: process.env.DB_HOST || 'localhost',
  user: process.env.DB_USER || 'root',
  password: process.env.DB_PASSWORD || '',
  database: process.env.DB_NAME || 'personweb',
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0
}

// 创建连接池
const pool = mysql.createPool(dbConfig)

/**
 * 执行查询
 */
export async function query(sql: string, params: any[] = []) {
  try {
    const [rows] = await pool.execute(sql, params)
    return rows
  } catch (error) {
    console.error('数据库查询错误:', error)
    throw error
  }
}

/**
 * 事务执行
 */
export async function transaction(queries: { sql: string; params: any[] }[]) {
  const connection = await pool.getConnection()
  try {
    await connection.beginTransaction()

    const results = []
    for (const { sql, params } of queries) {
      const [rows] = await connection.execute(sql, params)
      results.push(rows)
    }

    await connection.commit()
    return results
  } catch (error) {
    await connection.rollback()
    console.error('事务执行错误:', error)
    throw error
  } finally {
    connection.release()
  }
}

/**
 * 获取模块列表
 */
export async function getModules(filters: {
  category?: string
  enabled?: boolean
  search?: string
  page?: number
  pageSize?: number
} = {}) {
  let sql = `
    SELECT id, module_key, module_name, module_version, description, author, icon,
           category, dependencies, routes, components, permissions, config_schema,
           is_enabled, is_core, sort, created_at, updated_at
    FROM module
  `

  const whereConditions: string[] = []
  const params: any[] = []

  if (filters.category) {
    whereConditions.push('category = ?')
    params.push(filters.category)
  }

  if (filters.enabled !== undefined) {
    whereConditions.push('is_enabled = ?')
    params.push(filters.enabled)
  }

  if (filters.search) {
    whereConditions.push(
      '(module_name LIKE ? OR description LIKE ? OR category LIKE ?)'
    )
    const searchPattern = `%${filters.search}%`
    params.push(searchPattern, searchPattern, searchPattern)
  }

  if (whereConditions.length > 0) {
    sql += ' WHERE ' + whereConditions.join(' AND ')
  }

  sql += ' ORDER BY sort ASC, created_at DESC'

  const { page = 1, pageSize = 10 } = filters
  const offset = (page - 1) * pageSize
  sql += ' LIMIT ? OFFSET ?'
  params.push(pageSize, offset)

  const [rows] = await query(sql, params)

  // 获取总数
  let countSql = 'SELECT COUNT(*) as total FROM module'
  if (whereConditions.length > 0) {
    countSql += ' WHERE ' + whereConditions.join(' AND ')
  }

  const [countRows] = await query(countSql, params.slice(0, -2))
  const total = countRows[0].total

  return {
    data: rows,
    pagination: {
      page,
      pageSize,
      total,
      totalPages: Math.ceil(total / pageSize)
    }
  }
}

/**
 * 获取模块详情
 */
export async function getModuleByKey(moduleKey: string) {
  const sql = `
    SELECT id, module_key, module_name, module_version, description, author, icon,
           category, dependencies, routes, components, permissions, config_schema,
           is_enabled, is_core, sort, created_at, updated_at
    FROM module
    WHERE module_key = ?
  `

  const [rows] = await query(sql, [moduleKey])
  return rows[0] || null
}

/**
 * 创建模块
 */
export async function createModule(moduleData: any) {
  const sql = `
    INSERT INTO module (
      module_key, module_name, module_version, description, author, icon,
      category, dependencies, routes, components, permissions, config_schema,
      is_enabled, is_core, sort
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
  `

  const {
    moduleKey, moduleName, moduleVersion = '1.0.0', description, author, icon,
    category, dependencies = [], routes = [], components = [], permissions = {},
    configSchema = {}, isEnabled = true, isCore = false, sort = 0
  } = moduleData

  const params = [
    moduleKey, moduleName, moduleVersion, description, author, icon,
    category, JSON.stringify(dependencies), JSON.stringify(routes),
    JSON.stringify(components), JSON.stringify(permissions), JSON.stringify(configSchema),
    isEnabled, isCore, sort
  ]

  const [result] = await query(sql, params)
  return result.insertId
}

/**
 * 更新模块
 */
export async function updateModule(moduleKey: string, updateData: any) {
  const sets: string[] = []
  const params: any[] = []

  const allowedFields = [
    'module_name', 'module_version', 'description', 'author', 'icon',
    'category', 'dependencies', 'routes', 'components', 'permissions',
    'config_schema', 'is_enabled', 'sort'
  ]

  for (const [key, value] of Object.entries(updateData)) {
    if (allowedFields.includes(key)) {
      sets.push(`${key} = ?`)

      if (['dependencies', 'routes', 'components', 'permissions', 'config_schema'].includes(key)) {
        params.push(JSON.stringify(value))
      } else {
        params.push(value)
      }
    }
  }

  if (sets.length === 0) {
    throw new Error('没有提供有效的更新字段')
  }

  params.push(moduleKey)

  const sql = `
    UPDATE module
    SET ${sets.join(', ')}, updated_at = CURRENT_TIMESTAMP
    WHERE module_key = ?
  `

  const [result] = await query(sql, params)
  return result.affectedRows > 0
}

/**
 * 删除模块
 */
export async function deleteModule(moduleKey: string) {
  // 检查是否有其他模块依赖此模块
  const dependenciesQuery = `
    SELECT module_key FROM module
    WHERE JSON_CONTAINS(dependencies, ?)
  `
  const [dependentModules] = await query(dependenciesQuery, [`"${moduleKey}"`])

  if (dependentModules.length > 0) {
    throw new Error(`无法删除：有 ${dependentModules.length} 个模块依赖此模块`)
  }

  const sql = 'DELETE FROM module WHERE module_key = ? AND is_core = 0'
  const [result] = await query(sql, [moduleKey])

  if (result.affectedRows === 0) {
    throw new Error('模块不存在或是核心模块')
  }

  return true
}

/**
 * 更新模块状态
 */
export async function updateModuleStatus(moduleKey: string, enabled: boolean) {
  // 检查是否为核心模块
  const module = await getModuleByKey(moduleKey)
  if (!module) {
    throw new Error('模块不存在')
  }

  if (module.is_core && !enabled) {
    throw new Error('核心模块不能禁用')
  }

  // 检查依赖关系
  if (enabled) {
    const dependentModules = await getModules({
      enabled: false
    })

    const hasDisabledDependents = dependentModules.data.some((m: any) =>
      m.dependencies && m.dependencies.includes(moduleKey)
    )

    if (hasDisabledDependents) {
      throw new Error('存在禁用的依赖模块，需要先启用它们')
    }
  }

  const sql = `
    UPDATE module
    SET is_enabled = ?, updated_at = CURRENT_TIMESTAMP
    WHERE module_key = ?
  `

  const [result] = await query(sql, [enabled, moduleKey])
  return result.affectedRows > 0
}

/**
 * 获取模块配置
 */
export async function getModuleConfigs(moduleKey: string) {
  const sql = `
    SELECT id, module_key, config_key, config_value, description, created_at, updated_at
    FROM module_config
    WHERE module_key = ?
    ORDER BY config_key
  `

  const [rows] = await query(sql, [moduleKey])
  return rows
}

/**
 * 更新模块配置
 */
export async function updateModuleConfig(moduleKey: string, configKey: string, configValue: any) {
  const sql = `
    INSERT INTO module_config (module_key, config_key, config_value, description)
    VALUES (?, ?, ?, ?)
    ON DUPLICATE KEY UPDATE
      config_value = VALUES(config_value),
      updated_at = CURRENT_TIMESTAMP
  `

  const [result] = await query(sql, [
    moduleKey,
    configKey,
    configValue,
    `配置项: ${configKey}`
  ])

  return result.affectedRows > 0
}

/**
 * 批量更新模块配置
 */
export async function batchUpdateModuleConfigs(moduleKey: string, configs: any[]) {
  const queries = configs.map(config => ({
    sql: `
      INSERT INTO module_config (module_key, config_key, config_value, description)
      VALUES (?, ?, ?, ?)
      ON DUPLICATE KEY UPDATE
        config_value = VALUES(config_value),
        updated_at = CURRENT_TIMESTAMP
    `,
    params: [
      moduleKey,
      config.configKey,
      config.configValue,
      config.description || `配置项: ${config.configKey}`
    ]
  }))

  await transaction(queries)
  return true
}

/**
 * 获取模块版本列表
 */
export async function getModuleVersions(moduleKey: string, filters: {
  stable?: boolean
  latest?: boolean
  page?: number
  pageSize?: number
} = {}) {
  let sql = `
    SELECT id, module_key, version, package_url, package_size, checksum,
           changelog, is_latest, is_stable, published_at, created_by, created_at, updated_at
    FROM module_version
    WHERE module_key = ?
  `

  const params: any[] = [moduleKey]

  if (filters.stable !== undefined) {
    sql += ' AND is_stable = ?'
    params.push(filters.stable)
  }

  if (filters.latest !== undefined) {
    sql += ' AND is_latest = ?'
    params.push(filters.latest)
  }

  sql += ' ORDER BY published_at DESC'

  const { page = 1, pageSize = 10 } = filters
  const offset = (page - 1) * pageSize
  sql += ' LIMIT ? OFFSET ?'
  params.push(pageSize, offset)

  const [rows] = await query(sql, params)

  // 获取总数
  let countSql = 'SELECT COUNT(*) as total FROM module_version WHERE module_key = ?'
  const countParams = [moduleKey]
  if (filters.stable !== undefined) {
    countSql += ' AND is_stable = ?'
    countParams.push(filters.stable)
  }
  if (filters.latest !== undefined) {
    countSql += ' AND is_latest = ?'
    countParams.push(filters.latest)
  }

  const [countRows] = await query(countSql, countParams)
  const total = countRows[0].total

  return {
    data: rows,
    pagination: {
      page,
      pageSize,
      total,
      totalPages: Math.ceil(total / pageSize)
    }
  }
}

/**
 * 获取特定版本信息
 */
export async function getModuleVersion(moduleKey: string, version: string) {
  const sql = `
    SELECT id, module_key, version, package_url, package_size, checksum,
           changelog, is_latest, is_stable, published_at, created_by, created_at, updated_at
    FROM module_version
    WHERE module_key = ? AND version = ?
  `

  const [rows] = await query(sql, [moduleKey, version])
  return rows[0] || null
}

/**
 * 创建模块版本
 */
export async function createModuleVersion(versionData: any) {
  const sql = `
    INSERT INTO module_version (
      module_key, version, package_url, package_size, checksum,
      changelog, is_stable, is_latest, published_at, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
  `

  const {
    moduleKey, version, packageUrl, packageSize, checksum,
    changelog, isStable = true, isLatest = false, publishedAt, createdBy
  } = versionData

  const params = [
    moduleKey, version, packageUrl, packageSize, checksum,
    changelog, isStable, isLatest, publishedAt, createdBy
  ]

  const [result] = await query(sql, params)
  return result.insertId
}

/**
 * 更新版本信息
 */
export async function updateModuleVersion(moduleKey: string, version: string, updateData: any) {
  const sets: string[] = []
  const params: any[] = []

  const allowedFields = [
    'changelog', 'is_stable', 'is_latest'
  ]

  for (const [key, value] of Object.entries(updateData)) {
    if (allowedFields.includes(key)) {
      sets.push(`${key.replace(/([A-Z])/g, '_$1').toLowerCase()} = ?`)
      params.push(value)
    }
  }

  if (sets.length === 0) {
    throw new Error('没有提供有效的更新字段')
  }

  params.push(moduleKey, version)

  const sql = `
    UPDATE module_version
    SET ${sets.join(', ')}, updated_at = CURRENT_TIMESTAMP
    WHERE module_key = ? AND version = ?
  `

  const [result] = await query(sql, params)
  return result.affectedRows > 0
}

/**
 * 设置最新版本
 */
export async function setLatestVersion(moduleKey: string, version: string) {
  // 开始事务
  const connection = await pool.getConnection()

  try {
    await connection.beginTransaction()

    // 将其他版本的 is_latest 设为 0
    await connection.query(
      'UPDATE module_version SET is_latest = 0 WHERE module_key = ?',
      [moduleKey]
    )

    // 设置新版本为最新版本
    await connection.query(
      'UPDATE module_version SET is_latest = 1 WHERE module_key = ? AND version = ?',
      [moduleKey, version]
    )

    await connection.commit()
    return true
  } catch (error) {
    await connection.rollback()
    throw error
  } finally {
    connection.release()
  }
}

/**
 * 记录下载
 */
export async function recordDownload(moduleKey: string, version: string, userId?: number, ipAddress?: string, userAgent?: string) {
  const sql = `
    INSERT INTO module_download (
      module_key, version, user_id, ip_address, user_agent
    ) VALUES (?, ?, ?, ?, ?)
  `

  const params = [moduleKey, version, userId, ipAddress, userAgent]

  const [result] = await query(sql, params)
  return result.insertId
}

/**
 * 获取下载统计
 */
export async function getDownloadStats(moduleKey?: string, startDate?: string, endDate?: string) {
  let sql = `
    SELECT
      module_key,
      version,
      COUNT(*) as download_count,
      DATE(downloaded_at) as download_date
    FROM module_download
  `

  const params: any[] = []
  const whereConditions: string[] = []

  if (moduleKey) {
    whereConditions.push('module_key = ?')
    params.push(moduleKey)
  }

  if (startDate) {
    whereConditions.push('downloaded_at >= ?')
    params.push(startDate)
  }

  if (endDate) {
    whereConditions.push('downloaded_at <= ?')
    params.push(endDate)
  }

  if (whereConditions.length > 0) {
    sql += ' WHERE ' + whereConditions.join(' AND ')
  }

  sql += ' GROUP BY module_key, version, DATE(downloaded_at) ORDER BY download_date DESC'

  const [rows] = await query(sql, params)
  return rows
}

/**
 * 创建模块评价
 */
export async function createReview(reviewData: any) {
  const sql = `
    INSERT INTO module_review (
      module_key, version, user_id, rating, title, content, is_verified
    ) VALUES (?, ?, ?, ?, ?, ?, ?)
  `

  const {
    moduleKey, version, userId, rating, title, content, isVerified = false
  } = reviewData

  const params = [moduleKey, version, userId, rating, title, content, isVerified]

  const [result] = await query(sql, params)
  return result.insertId
}

/**
 * 获取模块评价
 */
export async function getModuleReviews(moduleKey: string, version?: string, page = 1, pageSize = 10) {
  let sql = `
    SELECT id, module_key, version, user_id, rating, title, content,
           is_verified, created_at, updated_at
    FROM module_review
    WHERE module_key = ?
  `

  const params: any[] = [moduleKey]

  if (version) {
    sql += ' AND version = ?'
    params.push(version)
  }

  sql += ' ORDER BY created_at DESC'

  const offset = (page - 1) * pageSize
  sql += ' LIMIT ? OFFSET ?'
  params.push(pageSize, offset)

  const [rows] = await query(sql, params)

  // 获取评价统计
  let countSql = 'SELECT COUNT(*) as total FROM module_review WHERE module_key = ?'
  const countParams = [moduleKey]
  if (version) {
    countSql += ' AND version = ?'
    countParams.push(version)
  }

  const [countRows] = await query(countSql, countParams)
  const total = countRows[0].total

  return {
    data: rows,
    pagination: {
      page,
      pageSize,
      total,
      totalPages: Math.ceil(total / pageSize)
    }
  }
}

/**
 * 计算模块评分
 */
export async function calculateModuleRating(moduleKey: string, version?: string) {
  let sql = `
    SELECT
      COUNT(*) as total_reviews,
      AVG(rating) as average_rating,
      SUM(CASE WHEN rating = 5 THEN 1 ELSE 0 END) as rating_5,
      SUM(CASE WHEN rating = 4 THEN 1 ELSE 0 END) as rating_4,
      SUM(CASE WHEN rating = 3 THEN 1 ELSE 0 END) as rating_3,
      SUM(CASE WHEN rating = 2 THEN 1 ELSE 0 END) as rating_2,
      SUM(CASE WHEN rating = 1 THEN 1 ELSE 0 END) as rating_1
    FROM module_review
    WHERE module_key = ?
  `

  const params: any[] = [moduleKey]
  if (version) {
    sql += ' AND version = ?'
    params.push(version)
  }

  const [rows] = await query(sql, params)
  const stats = rows[0]

  return {
    averageRating: parseFloat(stats.average_rating || 0),
    totalReviews: parseInt(stats.total_reviews || 0),
    distribution: {
      5: parseInt(stats.rating_5 || 0),
      4: parseInt(stats.rating_4 || 0),
      3: parseInt(stats.rating_3 || 0),
      2: parseInt(stats.rating_2 || 0),
      1: parseInt(stats.rating_1 || 0)
    }
  }
}

/**
 * 更新兼容性信息
 */
export async function updateCompatibility(compatibilityData: any) {
  const sql = `
    INSERT INTO module_compatibility (
      module_key, version, nuxt_version_min, nuxt_version_max,
      node_version_min, browser_compatibility, dependencies
    ) VALUES (?, ?, ?, ?, ?, ?, ?)
    ON DUPLICATE KEY UPDATE
      nuxt_version_min = VALUES(nuxt_version_min),
      nuxt_version_max = VALUES(nuxt_version_max),
      node_version_min = VALUES(node_version_min),
      browser_compatibility = VALUES(browser_compatibility),
      dependencies = VALUES(dependencies),
      updated_at = CURRENT_TIMESTAMP
  `

  const {
    moduleKey, version, nuxtVersionMin, nuxtVersionMax,
    nodeVersionMin, browserCompatibility, dependencies
  } = compatibilityData

  const params = [
    moduleKey, version, nuxtVersionMin, nuxtVersionMax,
    nodeVersionMin, JSON.stringify(browserCompatibility), JSON.stringify(dependencies)
  ]

  const [result] = await query(sql, params)
  return result.affectedRows > 0
}

export default {
  query,
  transaction,
  getModules,
  getModuleByKey,
  createModule,
  updateModule,
  deleteModule,
  updateModuleStatus,
  getModuleConfigs,
  updateModuleConfig,
  batchUpdateModuleConfigs,
  getModuleVersions,
  getModuleVersion,
  createModuleVersion,
  updateModuleVersion,
  setLatestVersion,
  recordDownload,
  getDownloadStats,
  createReview,
  getModuleReviews,
  calculateModuleRating,
  updateCompatibility
}