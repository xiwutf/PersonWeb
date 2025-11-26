import fs from 'node:fs'
import path from 'node:path'

const statsFile = path.resolve(process.cwd(), 'server/data/stats.json')

export default defineEventHandler(async (event) => {
    if (!fs.existsSync(statsFile)) {
        return { totalVisits: 0, todayVisits: 0 }
    }

    const data = JSON.parse(fs.readFileSync(statsFile, 'utf-8'))
    return data
})
