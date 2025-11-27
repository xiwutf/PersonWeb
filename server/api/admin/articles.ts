import fs from 'node:fs'
import path from 'node:path'
// import matter from 'gray-matter' // Replaced by server/utils/frontmatter.ts

const contentDir = path.resolve(process.cwd(), 'content')

import { checkAuth } from '../../utils/auth'

export default defineEventHandler(async (event) => {
    // Check authentication
    checkAuth(event)

    const method = event.method

    // Helper to get files recursively with metadata
    const getFiles = (dir: string, fileList: any[] = []) => {
        if (!fs.existsSync(dir)) return []
        const files = fs.readdirSync(dir)
        files.forEach(file => {
            const filePath = path.join(dir, file)
            const stat = fs.statSync(filePath)
            if (stat.isDirectory()) {
                getFiles(filePath, fileList)
            } else {
                if (file.endsWith('.md')) {
                    const content = fs.readFileSync(filePath, 'utf-8')
                    const { data } = parseFrontmatter(content)

                    fileList.push({
                        path: path.relative(contentDir, filePath).replace(/\\/g, '/'),
                        name: file,
                        title: data.title || file.replace('.md', ''),
                        date: data.date || stat.mtime,
                        size: stat.size,
                        mtime: stat.mtime,
                        category: data.category || 'Uncategorized'
                    })
                }
            }
        })
        return fileList
    }

    if (method === 'GET') {
        const query = getQuery(event)

        // Get single article
        if (query.id) {
            const filePath = path.join(contentDir, query.id as string)
            if (fs.existsSync(filePath)) {
                const content = fs.readFileSync(filePath, 'utf-8')
                // We might want to return parsed content or raw content depending on need
                // For editing, we usually want raw content or separated frontmatter/body
                // Let's return raw content for now, or maybe parsed if the frontend expects it
                // The frontend edit page seems to expect 'contentMd' and other fields
                const { data, content: body } = parseFrontmatter(content)
                return {
                    ...data,
                    contentMd: body,
                    id: query.id // Use path as ID
                }
            }
            throw createError({ statusCode: 404, statusMessage: 'File not found' })
        }

        // List all
        return getFiles(contentDir)
    }

    if (method === 'POST') {
        // Create new file
        const body = await readBody(event)
        const { title, slug, contentMd, category, summary, coverUrl, tags } = body

        if (!title) throw createError({ statusCode: 400, statusMessage: 'Title required' })

        // Generate filename from slug or title
        const safeSlug = (slug || title).toLowerCase().replace(/[^a-z0-9]+/g, '-')
        // Determine path based on category or default to blog
        const categoryPath = category ? (category === 'Uncategorized' ? 'blog' : category.toLowerCase()) : 'blog'
        const filename = `${categoryPath}/${safeSlug}.md`
        const filePath = path.join(contentDir, filename)

        // Ensure dir exists
        const dir = path.dirname(filePath)
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, { recursive: true })
        }

        if (fs.existsSync(filePath)) {
            throw createError({ statusCode: 409, statusMessage: 'File already exists' })
        }

        // Construct frontmatter
        const frontmatter = {
            title,
            date: new Date().toISOString(),
            summary,
            cover: coverUrl,
            category,
            tags
        }

        const fileContent = stringifyFrontmatter(contentMd || '', frontmatter)
        fs.writeFileSync(filePath, fileContent)
        return { success: true, id: filename }
    }

    if (method === 'PUT') {
        // Update file
        const body = await readBody(event)
        const { id, title, slug, contentMd, category, summary, coverUrl, tags } = body

        if (!id) throw createError({ statusCode: 400, statusMessage: 'ID (filepath) required' })

        const oldFilePath = path.join(contentDir, id)
        if (!fs.existsSync(oldFilePath)) {
            throw createError({ statusCode: 404, statusMessage: 'File not found' })
        }

        // Check if we need to rename/move (if slug or category changed)
        // For simplicity, let's keep it in the same place for now unless explicitly requested
        // Or we can just update the content in place

        // Read existing to preserve other fields if needed
        const existingContent = fs.readFileSync(oldFilePath, 'utf-8')
        const { data: existingData } = parseFrontmatter(existingContent)

        const frontmatter = {
            ...existingData,
            title,
            summary,
            cover: coverUrl,
            category,
            tags,
            updatedAt: new Date().toISOString()
        }

        const fileContent = stringifyFrontmatter(contentMd || '', frontmatter)
        fs.writeFileSync(oldFilePath, fileContent)
        return { success: true }
    }

    if (method === 'DELETE') {
        const query = getQuery(event)
        const id = query.id as string // id is the relative path

        if (!id) throw createError({ statusCode: 400, statusMessage: 'ID required' })

        const filePath = path.join(contentDir, id)

        if (fs.existsSync(filePath)) {
            fs.unlinkSync(filePath)
        }

        return { success: true }
    }
})
