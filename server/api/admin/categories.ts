import fs from 'node:fs'
import path from 'node:path'

const categoriesFile = path.resolve(process.cwd(), 'server/data/categories.json')

export default defineEventHandler(async (event) => {
    // Check authentication
    const cookies = parseCookies(event)
    if (cookies.admin_auth !== 'true') {
        throw createError({
            statusCode: 401,
            statusMessage: 'Unauthorized'
        })
    }

    const method = event.method

    if (method === 'GET') {
        if (!fs.existsSync(categoriesFile)) {
            return []
        }
        const data = fs.readFileSync(categoriesFile, 'utf-8')
        return JSON.parse(data)
    }

    if (method === 'POST') {
        const body = await readBody(event)
        const { name, slug, description } = body

        if (!name) throw createError({ statusCode: 400, statusMessage: 'Name required' })

        let categories = []
        if (fs.existsSync(categoriesFile)) {
            categories = JSON.parse(fs.readFileSync(categoriesFile, 'utf-8'))
        }

        const newCategory = {
            id: Date.now(), // Simple ID generation
            name,
            slug: slug || name.toLowerCase().replace(/[^a-z0-9]+/g, '-'),
            description,
            count: 0 // Initial count
        }

        categories.push(newCategory)

        // Ensure dir exists
        const dir = path.dirname(categoriesFile)
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, { recursive: true })
        }

        fs.writeFileSync(categoriesFile, JSON.stringify(categories, null, 2))
        return newCategory
    }

    if (method === 'PUT') {
        const body = await readBody(event)
        const { id, name, slug, description } = body

        if (!id) throw createError({ statusCode: 400, statusMessage: 'ID required' })

        if (!fs.existsSync(categoriesFile)) {
            throw createError({ statusCode: 404, statusMessage: 'Categories not found' })
        }

        let categories = JSON.parse(fs.readFileSync(categoriesFile, 'utf-8'))
        const index = categories.findIndex((c: any) => c.id === id)

        if (index === -1) {
            throw createError({ statusCode: 404, statusMessage: 'Category not found' })
        }

        categories[index] = {
            ...categories[index],
            name,
            slug: slug || name.toLowerCase().replace(/[^a-z0-9]+/g, '-'),
            description
        }

        fs.writeFileSync(categoriesFile, JSON.stringify(categories, null, 2))
        return categories[index]
    }

    if (method === 'DELETE') {
        const query = getQuery(event)
        const id = parseInt(query.id as string)

        if (!id) throw createError({ statusCode: 400, statusMessage: 'ID required' })

        if (fs.existsSync(categoriesFile)) {
            let categories = JSON.parse(fs.readFileSync(categoriesFile, 'utf-8'))
            categories = categories.filter((c: any) => c.id !== id)
            fs.writeFileSync(categoriesFile, JSON.stringify(categories, null, 2))
        }

        return { success: true }
    }
})
