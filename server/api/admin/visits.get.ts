import fs from 'node:fs'
import path from 'node:path'

const logsFile = path.resolve(process.cwd(), 'server/data/visit-logs.json')

export default defineEventHandler(async (event) => {
    // In a real app, you would add authentication check here

    let totalVisits = 0
    let uniqueVisitors = 0
    let todayVisits = 0
    let topPaths: any[] = []
    let recentVisits: any[] = []

    try {
        // Total Visits
        const [totalRows] = await db.query('SELECT COUNT(*) as count FROM visit_logs') as any[]
        totalVisits = totalRows[0]?.count || 0

        // Unique Visitors
        const [uniqueRows] = await db.query('SELECT COUNT(DISTINCT visitor_id) as count FROM visit_logs') as any[]
        uniqueVisitors = uniqueRows[0]?.count || 0

        // Today's Visits
        const [todayRows] = await db.query('SELECT COUNT(*) as count FROM visit_logs WHERE DATE(timestamp) = CURDATE()') as any[]
        todayVisits = todayRows[0]?.count || 0

        // Top Paths
        const [pathRows] = await db.query('SELECT path, COUNT(*) as count FROM visit_logs GROUP BY path ORDER BY count DESC LIMIT 5') as any[]
        topPaths = pathRows

        // Recent Visits
        const [recentRows] = await db.query('SELECT id, visitor_id as visitorId, ip, path, timestamp FROM visit_logs ORDER BY timestamp DESC LIMIT 50') as any[]
        recentVisits = recentRows
    } catch (e) {
        console.error('Failed to fetch admin stats from DB:', e)
    }

    return {
        stats: {
            totalVisits,
            uniqueVisitors,
            todayVisits
        },
        topPaths,
        recentVisits
    }
})
