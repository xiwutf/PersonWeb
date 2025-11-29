import { db } from '~/server/utils/db'

export default defineEventHandler(async (event) => {
    try {
        // 从数据库查询实时统计数据
        const [totalRows] = await db.query('SELECT COUNT(*) as count FROM visit_logs') as any[]
        const totalVisits = totalRows[0]?.count || 0

        // 查询今日访问量（使用当前日期）
        const today = new Date().toISOString().split('T')[0] // YYYY-MM-DD
        const [todayRows] = await db.query(
            'SELECT COUNT(*) as count FROM visit_logs WHERE DATE(timestamp) = ?',
            [today]
        ) as any[]
        const todayVisits = todayRows[0]?.count || 0

        return {
            totalVisits,
            todayVisits
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
