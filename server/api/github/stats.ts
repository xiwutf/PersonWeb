
export default defineEventHandler(async (event) => {
    const query = getQuery(event)
    const repo = query.repo as string // format: owner/repo
    const type = (query.type as string) || 'activity' // activity, languages, contributions

    if (!repo) {
        throw createError({ statusCode: 400, statusMessage: 'Repo is required' })
    }

    // 获取 GitHub Token (如果有)
    const token = process.env.GITHUB_TOKEN

    const headers: any = {
        'Accept': 'application/vnd.github+json',
        'X-GitHub-Api-Version': '2022-11-28',
        'User-Agent': 'Nuxt-App'
    }

    if (token) {
        headers['Authorization'] = `Bearer ${token}`
    }

    try {
        if (type === 'languages') {
            // 获取语言分布
            const languages = await $fetch(`https://api.github.com/repos/${repo}/languages`, {
                headers
            })
            
            // 计算总字节数
            const total = Object.values(languages as Record<string, number>).reduce((sum: number, val: number) => sum + val, 0)
            
            // 转换为百分比
            const result = Object.entries(languages as Record<string, number>).map(([lang, bytes]) => ({
                language: lang,
                bytes,
                percentage: ((bytes / total) * 100).toFixed(2)
            })).sort((a, b) => b.bytes - a.bytes)
            
            return result
        } else if (type === 'contributions') {
            // 获取贡献统计（需要 GitHub API v4 GraphQL）
            // 这里返回基本的仓库统计信息
            const repoInfo = await $fetch(`https://api.github.com/repos/${repo}`, {
                headers
            })
            
            return {
                stars: (repoInfo as any).stargazers_count || 0,
                forks: (repoInfo as any).forks_count || 0,
                watchers: (repoInfo as any).watchers_count || 0,
                openIssues: (repoInfo as any).open_issues_count || 0,
                size: (repoInfo as any).size || 0, // KB
                createdAt: (repoInfo as any).created_at,
                updatedAt: (repoInfo as any).updated_at
            }
        } else {
            // 默认：获取 Commit Activity (Last year)
            const response = await $fetch(`https://api.github.com/repos/${repo}/stats/commit_activity`, {
                headers
            })
            return response
        }
    } catch (e: any) {
        console.error('GitHub API Error:', e)
        // 如果是 403/429 (Rate Limit)，返回空数据
        if (type === 'languages') {
            return []
        } else if (type === 'contributions') {
            return {
                stars: 0,
                forks: 0,
                watchers: 0,
                openIssues: 0,
                size: 0,
                createdAt: null,
                updatedAt: null
            }
        } else {
            return []
        }
    }
})
