
const cache = new Map<string, { data: unknown; expires: number }>()
const CACHE_TTL = 60 * 60 * 1000 // 1 hour

export default defineEventHandler(async (event) => {
    const query = getQuery(event)
    const repo = query.repo as string
    const type = (query.type as string) || 'activity'

    if (!repo) {
        throw createError({ statusCode: 400, statusMessage: 'Repo is required' })
    }

    const cacheKey = `${repo}:${type}`
    const cached = cache.get(cacheKey)
    if (cached && cached.expires > Date.now()) {
        return cached.data
    }

    const token = process.env.GITHUB_TOKEN

    const headers: Record<string, string> = {
        'Accept': 'application/vnd.github+json',
        'X-GitHub-Api-Version': '2022-11-28',
        'User-Agent': 'Nuxt-App'
    }

    if (token) {
        headers['Authorization'] = `Bearer ${token}`
    }

    let result: unknown

    try {
        if (type === 'languages') {
            const languages = await $fetch(`https://api.github.com/repos/${repo}/languages`, { headers })
            const total = Object.values(languages as Record<string, number>).reduce((sum, val) => sum + val, 0)
            result = Object.entries(languages as Record<string, number>)
                .map(([lang, bytes]) => ({
                    language: lang,
                    bytes,
                    percentage: ((bytes / total) * 100).toFixed(2)
                }))
                .sort((a, b) => b.bytes - a.bytes)
        } else if (type === 'contributions') {
            const repoInfo = await $fetch(`https://api.github.com/repos/${repo}`, { headers }) as Record<string, unknown>
            result = {
                stars: repoInfo.stargazers_count ?? 0,
                forks: repoInfo.forks_count ?? 0,
                watchers: repoInfo.watchers_count ?? 0,
                openIssues: repoInfo.open_issues_count ?? 0,
                size: repoInfo.size ?? 0,
                createdAt: repoInfo.created_at,
                updatedAt: repoInfo.updated_at
            }
        } else {
            result = await $fetch(`https://api.github.com/repos/${repo}/stats/commit_activity`, { headers })
        }
    } catch (e: unknown) {
        console.error('GitHub API Error:', e)
        if (type === 'languages') {
            result = []
        } else if (type === 'contributions') {
            result = { stars: 0, forks: 0, watchers: 0, openIssues: 0, size: 0, createdAt: null, updatedAt: null }
        } else {
            result = []
        }
    }

    cache.set(cacheKey, { data: result, expires: Date.now() + CACHE_TTL })
    return result
})
