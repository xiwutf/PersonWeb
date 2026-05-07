import { readMarkdownCollection } from '../../utils/content-files'

interface SearchResult {
  slug: string
  title: string
  description: string
  category?: string
  tags?: string[]
  icon?: string
  isHot?: boolean
  relevance: number
}

export default defineEventHandler((event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=60, s-maxage=60')
  const query = getQuery(event)
  const keyword = (query.q as string || '').toLowerCase().trim()

  // 读取所有工具
  const tools = readMarkdownCollection('tools')

  // 如果没有关键词，返回热门工具推荐
  if (!keyword) {
    const hotTools = tools
      .filter(tool => tool.isHot || (tool as any).hot)
      .map(tool => ({
        slug: tool.slug,
        title: tool.title || '',
        description: tool.description || '',
        category: tool.category,
        tags: tool.tags,
        icon: tool.icon,
        isHot: tool.isHot || (tool as any).hot,
        relevance: 1
      }))
      .slice(0, 5)

    return {
      keyword: '',
      results: hotTools,
      total: hotTools.length,
      recommendations: tools.slice(0, 3).map(tool => ({
        slug: tool.slug,
        title: tool.title || '',
        description: tool.description || '',
        category: tool.category,
        tags: tool.tags,
        icon: tool.icon
      }))
    }
  }

  // 搜索匹配的工具
  const results: SearchResult[] = tools
    .map(tool => {
      const title = (tool.title || '').toLowerCase()
      const description = (tool.description || '').toLowerCase()
      const tags = (tool.tags || []).map(t => t.toLowerCase())
      const category = (tool.category || '').toLowerCase()

      let relevance = 0

      // 标题完全匹配
      if (title === keyword) {
        relevance = 100
      }
      // 标题包含关键词
      else if (title.includes(keyword)) {
        relevance = 80
      }
      // 描述包含关键词
      else if (description.includes(keyword)) {
        relevance = 60
      }
      // 标签匹配
      else if (tags.some(tag => tag.includes(keyword))) {
        relevance = 50
      }
      // 分类匹配
      else if (category.includes(keyword)) {
        relevance = 40
      }

      if (relevance > 0) {
        return {
          slug: tool.slug,
          title: tool.title || '',
          description: tool.description || '',
          category: tool.category,
          tags: tool.tags,
          icon: tool.icon,
          isHot: tool.isHot || (tool as any).hot,
          relevance
        }
      }

      return null
    })
    .filter((r): r is SearchResult => r !== null)
    .sort((a, b) => b.relevance - a.relevance)

  // 提取相关标签推荐
  const matchedTags = new Set<string>()
  results.forEach(tool => {
    tool.tags?.forEach(tag => {
      if (tag.toLowerCase().includes(keyword) || keyword.includes(tag.toLowerCase())) {
        matchedTags.add(tag)
      }
    })
  })

  // 基于匹配标签推荐其他工具
  const recommendations = tools
    .filter(tool => {
      if (results.some(r => r.slug === tool.slug)) return false
      return tool.tags?.some(tag => matchedTags.has(tag))
    })
    .slice(0, 3)
    .map(tool => ({
      slug: tool.slug,
      title: tool.title || '',
      description: tool.description || '',
      category: tool.category,
      tags: tool.tags,
      icon: tool.icon
    }))

  return {
    keyword,
    results,
    total: results.length,
    recommendations,
    relatedTags: Array.from(matchedTags).slice(0, 5)
  }
})
