import fs from 'node:fs'
import path from 'node:path'

const contentDir = path.resolve(process.cwd(), 'content')
const toolsDir = path.resolve(process.cwd(), 'content/tools') // Assuming tools are in content/tools
const statsFile = path.resolve(process.cwd(), 'server/data/stats.json')

export default defineEventHandler(async (event) => {
    // Check authentication
    const cookies = parseCookies(event)
    if (cookies.admin_auth !== 'true') {
        throw createError({
            statusCode: 401,
            statusMessage: 'Unauthorized'
        })
    }

    // Helper to count files recursively
    const countFiles = (dir: string, extension: string = '.md'): number => {
        if (!fs.existsSync(dir)) return 0
        let count = 0
        const files = fs.readdirSync(dir)
        files.forEach(file => {
            const filePath = path.join(dir, file)
            const stat = fs.statSync(filePath)
            if (stat.isDirectory()) {
                count += countFiles(filePath, extension)
            } else if (file.endsWith(extension)) {
                count++
            }
        })
        return count
    }

    // Count articles (excluding tools if they are in content/tools)
    // For simplicity, we'll count all MD files in content as articles for now, 
    // maybe subtracting tools if they are a subdirectory.
    // Let's assume content/blog, content/projects etc are articles.
    // And content/tools are tools.

    let articleCount = 0
    let toolCount = 0

    if (fs.existsSync(contentDir)) {
        articleCount = countFiles(contentDir)
    }

    if (fs.existsSync(toolsDir)) {
        toolCount = countFiles(toolsDir)
        // If tools are inside content, they might be double counted if we just count contentDir.
        // But let's assume flat structure or distinct folders for now.
        // If toolsDir is inside contentDir, we should subtract.
        if (toolsDir.startsWith(contentDir)) {
            articleCount -= toolCount
        }
    }

    // Get visit stats
    let todayVisits = 0
    if (fs.existsSync(statsFile)) {
        try {
            const data = JSON.parse(fs.readFileSync(statsFile, 'utf-8'))
            todayVisits = data.todayVisits || 0
        } catch (e) {
            console.error('Error reading stats file', e)
        }
    }

    return {
        articleCount,
        toolCount,
        todayVisits
    }
})
