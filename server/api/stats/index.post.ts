import { v4 as uuidv4 } from 'uuid'

export default defineEventHandler(async (event) => {
    // 1. 获取或创建 Visitor ID
    let visitorId = getCookie(event, 'visitor_id')

    if (!visitorId) {
        visitorId = uuidv4()
        setCookie(event, 'visitor_id', visitorId, {
            maxAge: 60 * 60 * 24 * 365, // 1 year
            httpOnly: false
        })
    }

    // 2. 获取请求信息
    const headers = getRequestHeaders(event)
    const ip = headers['x-forwarded-for'] || event.node.req.socket.remoteAddress
    const userAgent = headers['user-agent']
    const timestamp = new Date()

    // 3. 记录详细日志 (Insert into DB)
    try {
        await db.execute(
            'INSERT INTO visit_logs (id, visitor_id, ip, user_agent, path, timestamp) VALUES (?, ?, ?, ?, ?, ?)',
            [uuidv4(), visitorId, ip, userAgent, headers['referer'] || 'unknown', timestamp]
        )
    } catch (e) {
        console.error('Failed to log visit to DB:', e)
    }

    // 4. 获取统计数据 (Select from DB)
    // For performance, we might want to cache this or use a separate stats table in the future.
    // For now, we'll do a simple count query.
    let totalVisits = 0
    let todayVisits = 0

    try {
        const [totalRows] = await db.query('SELECT COUNT(*) as count FROM visit_logs') as any[]
        totalVisits = totalRows[0]?.count || 0

        const [todayRows] = await db.query('SELECT COUNT(*) as count FROM visit_logs WHERE DATE(timestamp) = CURDATE()') as any[]
        todayVisits = todayRows[0]?.count || 0
    } catch (e) {
        console.error('Failed to fetch stats from DB:', e)
    }

    return {
        totalVisits,
        todayVisits,
        visitorId
    }
})
