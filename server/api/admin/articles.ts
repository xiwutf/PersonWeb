import fs from 'node:fs'
import path from 'node:path'

const contentDir = path.resolve(process.cwd(), 'content')

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
        // List all markdown files recursively is a bit complex, 
        // for now let's assume flat structure or just list top level for simplicity
        // or use Nuxt Content's own query if possible? 
        // Actually, direct FS access gives us raw control which is better for editing.

        // Let's just list files in the content directory for now.
        // The user can organize them.

        // Helper to get files recursively
        const getFiles = (dir: string, fileList: any[] = []) => {
            const files = fs.readdirSync(dir)
            files.forEach(file => {
                const filePath = path.join(dir, file)
                const stat = fs.statSync(filePath)
                if (stat.isDirectory()) {
                    getFiles(filePath, fileList)
                } else {
                    if (file.endsWith('.md')) {
                        fileList.push({
                            path: path.relative(contentDir, filePath).replace(/\\/g, '/'),
                            name: file,
                            size: stat.size,
                            mtime: stat.mtime
                        })
                    }
                }
            })
            return fileList
        }

        if (!fs.existsSync(contentDir)) {
            return []
        }

        return getFiles(contentDir)
    }

    if (method === 'POST') {
        // Create new file
        const body = await readBody(event)
        const { filename, content } = body

        if (!filename) throw createError({ statusCode: 400, statusMessage: 'Filename required' })

        const filePath = path.join(contentDir, filename)

        // Ensure dir exists if filename has path
        const dir = path.dirname(filePath)
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, { recursive: true })
        }

        if (fs.existsSync(filePath)) {
            throw createError({ statusCode: 409, statusMessage: 'File already exists' })
        }

        fs.writeFileSync(filePath, content || '')
        return { success: true }
    }

    if (method === 'PUT') {
        // Update file
        const body = await readBody(event)
        const { filename, content, oldFilename } = body // oldFilename for renaming support if needed later

        if (!filename) throw createError({ statusCode: 400, statusMessage: 'Filename required' })

        const filePath = path.join(contentDir, filename)

        if (!fs.existsSync(filePath)) {
            throw createError({ statusCode: 404, statusMessage: 'File not found' })
        }

        fs.writeFileSync(filePath, content)
        return { success: true }
    }

    if (method === 'DELETE') {
        const query = getQuery(event)
        const filename = query.filename as string

        if (!filename) throw createError({ statusCode: 400, statusMessage: 'Filename required' })

        const filePath = path.join(contentDir, filename)

        if (fs.existsSync(filePath)) {
            fs.unlinkSync(filePath)
        }

        return { success: true }
    }

    // Also need a way to read a single file content for editing
    // Let's handle that with a query param 'action=read'
    const query = getQuery(event)
    if (query.action === 'read' && query.filename) {
        const filePath = path.join(contentDir, query.filename as string)
        if (fs.existsSync(filePath)) {
            const content = fs.readFileSync(filePath, 'utf-8')
            return { content }
        }
        throw createError({ statusCode: 404, statusMessage: 'File not found' })
    }
})
