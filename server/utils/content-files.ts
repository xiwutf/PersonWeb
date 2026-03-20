import fs from 'node:fs'
import path from 'node:path'
import { parseFrontmatter } from './frontmatter'

const contentRoot = path.resolve(process.cwd(), 'content')

export interface MarkdownContentItem {
  slug: string
  path: string
  _path: string
  title?: string
  description?: string
  summary?: string
  category?: string
  date?: string
  updatedAt?: string
  tags?: string[]
  cover?: string
  icon?: string
  status?: string
  role?: string
  duration?: string
  tech?: string[]
  demo_link?: string
  source_link?: string
  content: string
  [key: string]: unknown
}

const toArray = (value: unknown): string[] => {
  if (Array.isArray(value)) {
    return value.filter((item): item is string => typeof item === 'string' && Boolean(item.trim()))
  }

  if (typeof value === 'string' && value.trim()) {
    return value
      .split(/[,\u3001|/]/)
      .map(item => item.trim())
      .filter(Boolean)
  }

  return []
}

const createItem = (fullPath: string, routePath: string): MarkdownContentItem => {
  const fileContent = fs.readFileSync(fullPath, 'utf-8')
  const { data, content } = parseFrontmatter(fileContent)
  const slug = path.basename(fullPath, '.md')

  return {
    ...data,
    slug,
    path: routePath,
    _path: routePath,
    tags: toArray(data.tags),
    tech: toArray(data.tech),
    content
  }
}

const getSectionDirectory = (...segments: string[]) => path.join(contentRoot, ...segments)

export const readMarkdownCollection = (...segments: string[]) => {
  const sectionDir = getSectionDirectory(...segments)
  if (!fs.existsSync(sectionDir)) {
    return [] as MarkdownContentItem[]
  }

  return fs
    .readdirSync(sectionDir)
    .filter(file => file.endsWith('.md'))
    .map((file) => {
      const slug = path.basename(file, '.md')
      const routePath = `/${[...segments, slug].join('/')}`
      return createItem(path.join(sectionDir, file), routePath)
    })
    .sort((a, b) => new Date(b.date || b.updatedAt || 0).getTime() - new Date(a.date || a.updatedAt || 0).getTime())
}

export const readMarkdownDocument = (segments: string[], slug: string) => {
  const collection = readMarkdownCollection(...segments)
  return collection.find((item) => item.slug === slug || item.path.endsWith(`/${slug}`)) || null
}
