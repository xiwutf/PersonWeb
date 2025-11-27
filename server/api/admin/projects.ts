import fs from 'fs'
import path from 'path'
import { v4 as uuidv4 } from 'uuid'
import { parseFrontmatter, stringifyFrontmatter } from '../../utils/frontmatter'

const contentDir = path.resolve(process.cwd(), 'content')
const projectsDir = path.join(contentDir, 'projects')

// 确保目录存在
if (!fs.existsSync(projectsDir)) {
    fs.mkdirSync(projectsDir, { recursive: true })
}

export default defineEventHandler(async (event) => {
    const method = event.node.req.method

    // GET: 获取项目列表或单个项目
    if (method === 'GET') {
        const query = getQuery(event)
        const id = query.id as string

        if (id) {
            // 获取单个项目
            const filePath = path.join(projectsDir, `${id}.md`)
            if (fs.existsSync(filePath)) {
                const content = fs.readFileSync(filePath, 'utf-8')
                const { data, content: body } = parseFrontmatter(content)
                return { ...data, id, contentMd: body.trim() }
            } else {
                throw createError({ statusCode: 404, statusMessage: 'Project not found' })
            }
        } else {
            // 获取列表
            const files = fs.readdirSync(projectsDir).filter(file => file.endsWith('.md'))
            const projects = files.map(file => {
                const content = fs.readFileSync(path.join(projectsDir, file), 'utf-8')
                const { data } = parseFrontmatter(content)
                return {
                    ...data,
                    id: file.replace('.md', '')
                }
            })
            // 按日期倒序
            return projects.sort((a: any, b: any) => new Date(b.date || 0).getTime() - new Date(a.date || 0).getTime())
        }
    }

    // POST: 创建新项目
    if (method === 'POST') {
        const body = await readBody(event)
        const id = uuidv4()
        const { title, description, coverUrl, githubUrl, demoUrl, techStack, status, contentMd, date } = body

        const frontmatter = {
            title,
            description,
            coverUrl,
            githubUrl,
            demoUrl,
            techStack: Array.isArray(techStack) ? techStack : [],
            status: status || 'Active',
            date: date || new Date().toISOString()
        }

        const fileContent = stringifyFrontmatter(contentMd || '', frontmatter)
        fs.writeFileSync(path.join(projectsDir, `${id}.md`), fileContent)

        return { success: true, id }
    }

    // PUT: 更新项目
    if (method === 'PUT') {
        const body = await readBody(event)
        const { id, title, description, coverUrl, githubUrl, demoUrl, techStack, status, contentMd, date } = body

        if (!id) throw createError({ statusCode: 400, statusMessage: 'ID is required' })

        const filePath = path.join(projectsDir, `${id}.md`)
        if (!fs.existsSync(filePath)) {
            throw createError({ statusCode: 404, statusMessage: 'Project not found' })
        }

        // 读取现有数据以保留其他字段
        const existingContent = fs.readFileSync(filePath, 'utf-8')
        const { data: existingData } = parseFrontmatter(existingContent)

        const frontmatter = {
            ...existingData,
            title,
            description,
            coverUrl,
            githubUrl,
            demoUrl,
            techStack: Array.isArray(techStack) ? techStack : [],
            status,
            date: date || existingData.date
        }

        const fileContent = stringifyFrontmatter(contentMd || '', frontmatter)
        fs.writeFileSync(filePath, fileContent)

        return { success: true }
    }

    // DELETE: 删除项目
    if (method === 'DELETE') {
        const query = getQuery(event)
        const id = query.id as string

        if (!id) throw createError({ statusCode: 400, statusMessage: 'ID is required' })

        const filePath = path.join(projectsDir, `${id}.md`)
        if (fs.existsSync(filePath)) {
            fs.unlinkSync(filePath)
            return { success: true }
        } else {
            throw createError({ statusCode: 404, statusMessage: 'Project not found' })
        }
    }
})
