import { describe, expect, it } from 'vitest'
import {
  buildFeaturedProjects,
  buildHomeOverview,
  mapRawProjectsToCards,
} from '../../utils/home-overview'

describe('home-overview', () => {
  it('maps PascalCase and camelCase project fields', () => {
    const cards = mapRawProjectsToCards([
      {
        Id: 'a',
        Title: 'Alpha',
        Description: 'd',
        TechStack: 'Vue, Nuxt',
        Status: 'Active',
        ViewCount: 10,
      },
      {
        id: 'b',
        title: 'Beta',
        description: 'd2',
        techStack: '["TypeScript"]',
        status: 'Archived',
        viewCount: 99,
      },
    ])

    expect(cards[0]).toMatchObject({
      id: 'a',
      title: 'Alpha',
      techStack: ['Vue', 'Nuxt'],
      viewCount: 10,
    })
    expect(cards[1].techStack).toEqual(['TypeScript'])
  })

  it('features top viewCount projects and excludes Archived', () => {
    const featured = buildFeaturedProjects([
      { id: '1', title: 'A', description: '', techStack: [], coverUrl: null, demoUrl: null, githubUrl: null, status: 'Active', viewCount: 1 },
      { id: '2', title: 'B', description: '', techStack: [], coverUrl: null, demoUrl: null, githubUrl: null, status: 'Archived', viewCount: 100 },
      { id: '3', title: 'C', description: '', techStack: [], coverUrl: null, demoUrl: null, githubUrl: null, status: 'Completed', viewCount: 50 },
    ])

    expect(featured.map(p => p.id)).toEqual(['3', '1'])
  })

  it('buildHomeOverview returns featured slice from raw projects', () => {
    const overview = buildHomeOverview({
      projects: [
        { id: '1', title: '个人数字资产平台（本网站）', status: 'Active', viewCount: 5, techStack: 'Nuxt' },
        { id: '2', title: 'AI 创作助手', status: 'Active', viewCount: 3, techStack: 'Vue' },
      ],
    })
    expect(overview.featuredProjects).toHaveLength(2)
    expect(overview.featuredProjects[0].title).toContain('个人数字资产')
    expect(overview.stats.projects).toBe(2)
  })

  it('useHomeOverview fetches .NET Projects directly (not Nitro overview)', async () => {
    const { readFileSync } = await import('node:fs')
    const { resolve } = await import('node:path')
    const src = readFileSync(resolve(process.cwd(), 'composables/useHomeOverview.ts'), 'utf8')
    expect(src).toMatch(/fetchBackendApi/)
    expect(src).toMatch(/\/Projects/)
    expect(src.includes("'/api/home/overview'") || src.includes('"/api/home/overview"')).toBe(false)
  })
})
