import fs from 'node:fs'
import path from 'node:path'
// import matter from 'gray-matter'

const toolsDir = path.resolve(process.cwd(), 'content/tools')

import { checkAuth } from '../../utils/auth'

export default defineEventHandler(async (event) => {
    // Check authentication
    checkAuth(event)

    const method = event.method

    // Helper to get tools
    const getTools = () => {
        if (!fs.existsSync(toolsDir)) return []
        const files = fs.readdirSync(toolsDir)
        return files.filter(file => file.endsWith('.md')).map(file => {
            const filePath = path.join(toolsDir, file)
            const content = fs.readFileSync(filePath, 'utf-8')
            const { data } = parseFrontmatter(content)
            return {
                path: file,
                name: file,
                title: data.title || file.replace('.md', ''),
                description: data.description,
                icon: data.icon,
                url: data.url,
                ...data
            }
        })
    }

    if (method === 'GET') {
        return getTools()
    }

    if (method === 'POST') {
        const body = await readBody(event)
        const { title, slug, description, icon, url, contentMd } = body

        if (!title) throw createError({ statusCode: 400, statusMessage: 'Title required' })

        const safeSlug = (slug || title).toLowerCase().replace(/[^a-z0-9]+/g, '-')
        const filename = `${safeSlug}.md`
        const filePath = path.join(toolsDir, filename)

        if (!fs.existsSync(toolsDir)) {
            fs.mkdirSync(toolsDir, { recursive: true })
        }

        if (fs.existsSync(filePath)) {
            throw createError({ statusCode: 409, statusMessage: 'Tool already exists' })
        }

        const frontmatter = {
            title,
            description,
            icon,
            url,
            date: new Date().toISOString()
        }

        const fileContent = stringifyFrontmatter(contentMd || '', frontmatter)
        fs.writeFileSync(filePath, fileContent)
        return { success: true }
    }

    if (method === 'PUT') {
        const body = await readBody(event)
        const { path: oldPath, title, slug, description, icon, url, contentMd } = body

        if (!oldPath) throw createError({ statusCode: 400, statusMessage: 'Path required' })

        const filePath = path.join(toolsDir, oldPath)
        if (!fs.existsSync(filePath)) {
            throw createError({ statusCode: 404, statusMessage: 'Tool not found' })
        }

        // If slug/title changed, we might need to rename, but for simplicity let's just update content
        // and keep filename or maybe we should allow renaming?
        // Let's keep it simple: update content in place. 
        // If we really want to rename, we'd need to move the file.

        const existingContent = fs.readFileSync(filePath, 'utf-8')
        const { data: existingData } = parseFrontmatter(existingContent)

        const frontmatter = {
            ...existingData,
            title,
            description,
            icon,
            url,
            updatedAt: new Date().toISOString()
        }

        const fileContent = stringifyFrontmatter(contentMd || '', frontmatter)
        fs.writeFileSync(filePath, fileContent)
        return { success: true }
    }

    // PUT and DELETE are similar to articles, can be added if needed.
    // For now, let's implement DELETE at least.
    if (method === 'DELETE') {
        const query = getQuery(event)
        const filename = query.filename as string

        if (!filename) throw createError({ statusCode: 400, statusMessage: 'Filename required' })

        const filePath = path.join(toolsDir, filename)
        if (fs.existsSync(filePath)) {
            fs.unlinkSync(filePath)
        }
        return { success: true }
    }
})
