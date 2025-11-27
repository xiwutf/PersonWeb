import fs from 'fs'
import path from 'path'

const dataDir = path.resolve(process.cwd(), 'server/data')
const metricsFile = path.join(dataDir, 'personal_metrics.json')

// 确保数据文件存在
if (!fs.existsSync(metricsFile)) {
    if (!fs.existsSync(dataDir)) {
        fs.mkdirSync(dataDir, { recursive: true })
    }
    fs.writeFileSync(metricsFile, '[]')
}

export default defineEventHandler(async (event) => {
    const method = event.node.req.method

    // GET: 获取所有数据
    if (method === 'GET') {
        const data = fs.readFileSync(metricsFile, 'utf-8')
        return JSON.parse(data)
    }

    // POST: 添加或更新单日数据
    if (method === 'POST') {
        const body = await readBody(event)
        const { date, steps, sleep, weight, netWorth, energy } = body

        if (!date) {
            throw createError({ statusCode: 400, statusMessage: 'Date is required' })
        }

        let metrics = JSON.parse(fs.readFileSync(metricsFile, 'utf-8'))

        // 查找是否已存在当天的记录
        const index = metrics.findIndex((m: any) => m.date === date)

        const newRecord = {
            date,
            steps: Number(steps) || 0,
            sleep: Number(sleep) || 0,
            weight: Number(weight) || 0,
            netWorth: Number(netWorth) || 0,
            energy: Number(energy) || 0
        }

        if (index !== -1) {
            metrics[index] = { ...metrics[index], ...newRecord }
        } else {
            metrics.push(newRecord)
        }

        // 按日期排序
        metrics.sort((a: any, b: any) => new Date(a.date).getTime() - new Date(b.date).getTime())

        fs.writeFileSync(metricsFile, JSON.stringify(metrics, null, 2))
        return { success: true }
    }
})
