import fs from 'node:fs'
import path from 'node:path'

const viewsFilePath = path.resolve(process.cwd(), 'server/data/views.json')

export default defineEventHandler(async (event) => {
    const query = getQuery(event)
    const slug = query.slug as string

    if (!fs.existsSync(viewsFilePath)) {
        return {}
    }

    const data = JSON.parse(fs.readFileSync(viewsFilePath, 'utf-8'))

    if (slug) {
        return { views: data[slug] || 0 }
    }

    return data
})
