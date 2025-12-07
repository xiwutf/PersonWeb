import mysql from 'mysql2/promise'

// Create a connection pool with enhanced configuration
const pool = mysql.createPool({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0,
    // 连接超时设置（毫秒）
    connectTimeout: 10000,
    // 获取连接的超时时间（毫秒）
    acquireTimeout: 60000,
    // 字符集
    charset: 'utf8mb4',
    // 时区
    timezone: '+00:00',
    // 启用多语句查询（如果需要）
    multipleStatements: false
})

// 监听连接池事件，处理连接错误
const pingIntervals = new Map<number, NodeJS.Timeout>()

pool.on('connection', (connection) => {
    const threadId = connection.threadId
    // 定期发送 ping 保持连接活跃（每 30 秒）
    const interval = setInterval(() => {
        connection.ping().catch((err) => {
            console.error(`[DB] Ping failed for connection ${threadId}:`, err)
            // 如果 ping 失败，清除定时器
            if (pingIntervals.has(threadId)) {
                clearInterval(pingIntervals.get(threadId)!)
                pingIntervals.delete(threadId)
            }
        })
    }, 30000)
    
    pingIntervals.set(threadId, interval)
    
    // 连接关闭时清除定时器
    connection.on('end', () => {
        if (pingIntervals.has(threadId)) {
            clearInterval(pingIntervals.get(threadId)!)
            pingIntervals.delete(threadId)
        }
    })
})

pool.on('error', (err) => {
    console.error('[DB] Pool error:', err)
    // 如果是连接丢失错误，连接池会自动重连
    if (
        err.code === 'PROTOCOL_CONNECTION_LOST' || 
        err.code === 'ECONNRESET' ||
        err.code === 'ETIMEDOUT' ||
        err.message?.includes('Connection lost') ||
        err.message?.includes('The server closed the connection')
    ) {
        console.log('[DB] Connection lost, pool will handle reconnection automatically')
        // 连接池会自动处理重连，不需要手动操作
    } else {
        // 其他错误需要关注
        console.error('[DB] Unexpected pool error:', err.code, err.message)
    }
})

/**
 * 带重试机制的数据库查询包装函数
 * @param query SQL 查询语句
 * @param params 查询参数
 * @param maxRetries 最大重试次数
 * @returns 查询结果
 */
/**
 * 检查连接是否有效
 * @param connection 数据库连接
 * @returns 连接是否有效
 */
async function isConnectionValid(connection: mysql.PoolConnection): Promise<boolean> {
    try {
        await connection.ping()
        return true
    } catch (error) {
        return false
    }
}

/**
 * 获取有效的数据库连接
 * @param maxAttempts 最大尝试次数
 * @returns 有效的数据库连接
 */
async function getValidConnection(maxAttempts: number = 3): Promise<mysql.PoolConnection> {
    for (let attempt = 1; attempt <= maxAttempts; attempt++) {
        try {
            const connection = await pool.getConnection()
            
            // 检查连接是否有效
            if (await isConnectionValid(connection)) {
                return connection
            } else {
                // 连接无效，释放它
                connection.release()
                console.warn(`[DB] Connection invalid, releasing and retrying (attempt ${attempt}/${maxAttempts})...`)
                
                if (attempt < maxAttempts) {
                    await new Promise(resolve => setTimeout(resolve, 500 * attempt))
                }
            }
        } catch (error: any) {
            console.warn(`[DB] Failed to get connection (attempt ${attempt}/${maxAttempts}):`, error.message || error.code)
            
            if (attempt < maxAttempts) {
                await new Promise(resolve => setTimeout(resolve, 500 * attempt))
            } else {
                throw error
            }
        }
    }
    
    throw new Error('Failed to get valid database connection after retries')
}

/**
 * 带重试机制的数据库查询包装函数
 * @param query SQL 查询语句
 * @param params 查询参数
 * @param maxRetries 最大重试次数
 * @returns 查询结果
 */
async function queryWithRetry(
    query: string,
    params?: any[],
    maxRetries: number = 3
): Promise<any> {
    let lastError: any = null
    let connection: mysql.PoolConnection | null = null
    
    for (let attempt = 1; attempt <= maxRetries; attempt++) {
        try {
            // 获取有效连接
            connection = await getValidConnection(3)
            
            // 执行查询
            const result = params 
                ? await connection.query(query, params) 
                : await connection.query(query)
            
            // 释放连接
            connection.release()
            connection = null
            
            return result
        } catch (error: any) {
            // 如果连接存在，释放它
            if (connection) {
                try {
                    connection.release()
                } catch (releaseError) {
                    // 忽略释放错误
                }
                connection = null
            }
            
            lastError = error
            
            // 检查是否是连接相关错误
            const isConnectionError = 
                error.code === 'PROTOCOL_CONNECTION_LOST' ||
                error.code === 'ECONNRESET' ||
                error.code === 'ETIMEDOUT' ||
                error.code === 'ECONNREFUSED' ||
                error.message?.includes('Connection lost') ||
                error.message?.includes('The server closed the connection') ||
                error.message?.includes('Connection closed')
            
            if (isConnectionError) {
                if (attempt < maxRetries) {
                    const delay = attempt * 1000 // 递增延迟：1s, 2s, 3s
                    console.warn(`[DB] Connection error (attempt ${attempt}/${maxRetries}): ${error.message || error.code}`)
                    console.warn(`[DB] Retrying in ${delay}ms...`)
                    
                    // 等待后重试
                    await new Promise(resolve => setTimeout(resolve, delay))
                    continue
                } else {
                    // 达到最大重试次数，记录错误
                    console.error(`[DB] Max retries (${maxRetries}) reached. Last error:`, error.message || error.code)
                    // 不抛出错误，返回 null 让调用者处理
                    return null
                }
            }
            
            // 其他错误直接抛出
            throw error
        }
    }
    
    // 如果所有重试都失败，返回 null
    console.error('[DB] All retries failed, returning null')
    return null
}

// 数据库连接池
// 注意：数据库表结构需要手动维护，不会自动创建
export const db = {
    // 原始 pool 对象（用于需要直接访问的场景）
    pool,
    
    // 带重试的 query 方法
    query: queryWithRetry,
    
    // 执行查询（兼容原有代码）
    execute: async (query: string, params?: any[]) => {
        return queryWithRetry(query, params)
    }
}
