import fs from 'node:fs'
import path from 'node:path'

const statsFile = path.resolve(process.cwd(), 'server/data/stats.json')

export default defineEventHandler(async (event) => {
    let data = { totalVisits: 0, todayVisits: 0, lastUpdate: new Date().toISOString().split('T')[0] }

    if (fs.existsSync(statsFile)) {
        try {
            data = JSON.parse(fs.readFileSync(statsFile, 'utf-8'))
        } catch (e) {
            // ignore error
        }
    }

    const today = new Date().toISOString().split('T')[0]

    if (data.lastUpdate !== today) {
        data.todayVisits = 0
        data.lastUpdate = today
    }

    data.totalVisits++
    data.todayVisits++

    fs.writeFileSync(statsFile, JSON.stringify(data, null, 2))

    return data
})
