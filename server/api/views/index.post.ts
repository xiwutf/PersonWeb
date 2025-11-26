import fs from 'node:fs'
import path from 'node:path'

const viewsFilePath = path.resolve(process.cwd(), 'server/data/views.json')

export default defineEventHandler(async (event) => {
    const body = await readBody(event)
    const slug = body.slug

    if (!slug) {
        throw createError({
            statusCode: 400,
            statusMessage: 'Slug is required'
        })
    }

    let data: Record<string, number> = {}

    if (fs.existsSync(viewsFilePath)) {
        try {
            data = JSON.parse(fs.readFileSync(viewsFilePath, 'utf-8'))
        } catch (e) {
            // If file is corrupt, start fresh
            data = {}
        }
    } else {
        // Ensure directory exists
        const dir = path.dirname(viewsFilePath)
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, { recursive: true })
        }
    }

    if (!data[slug]) {
        data[slug] = 0
    }

    data[slug]++

    fs.writeFileSync(viewsFilePath, JSON.stringify(data, null, 2))

    return { views: data[slug] }
})
