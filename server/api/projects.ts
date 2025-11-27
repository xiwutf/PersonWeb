import fs from 'fs'
import path from 'path'
import { parseFrontmatter } from '../utils/frontmatter'

const contentDir = path.resolve(process.cwd(), 'content')
const projectsDir = path.join(contentDir, 'projects')

export default defineEventHandler(async (event) => {
    // 确保目录存在
    if (!fs.existsSync(projectsDir)) {
        return []
    }

    const files = fs.readdirSync(projectsDir).filter(file => file.endsWith('.md'))
    const projects = files.map(file => {
        const content = fs.readFileSync(path.join(projectsDir, file), 'utf-8')
        const { data } = parseFrontmatter(content)
        // 只返回 Active 或 Completed 的项目
        if (data.status === 'Archived') return null

        return {
            ...data,
            id: file.replace('.md', '')
        }
    }).filter(p => p !== null)

    // 按日期倒序
    return projects.sort((a: any, b: any) => new Date(b.date || 0).getTime() - new Date(a.date || 0).getTime())
})
