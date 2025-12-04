import { db } from '~/server/utils/db'

export default defineEventHandler(async (event) => {
    try {
        // 从数据库查询实时统计数据
        // mysql2 的 query 方法返回 [rows, fields]，rows 是结果数组
        const [totalRows] = await db.query('SELECT COUNT(*) as count FROM visit_logs') as any[]
        // 处理结果：totalRows 可能是数组或对象数组
        const totalVisits = Array.isArray(totalRows) && totalRows.length > 0 
            ? (totalRows[0]?.count || 0) 
            : (totalRows as any)?.count || 0

        // 查询今日访问量（使用 MySQL 的 CURDATE() 函数）
        const [todayRows] = await db.query(
            'SELECT COUNT(*) as count FROM visit_logs WHERE DATE(timestamp) = CURDATE()'
        ) as any[]
        // 处理结果：todayRows 可能是数组或对象数组
        const todayVisits = Array.isArray(todayRows) && todayRows.length > 0 
            ? (todayRows[0]?.count || 0) 
            : (todayRows as any)?.count || 0

        return {
            totalVisits: Number(totalVisits),
            todayVisits: Number(todayVisits)
        }
    } catch (e) {
        console.error('Failed to fetch stats from database:', e)
        // 如果数据库查询失败，返回默认值
        return {
            totalVisits: 0,
            todayVisits: 0
        }
    }
})
