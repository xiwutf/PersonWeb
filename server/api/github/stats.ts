
export default defineEventHandler(async (event) => {
    const query = getQuery(event)
    const repo = query.repo as string // format: owner/repo

    if (!repo) {
        throw createError({ statusCode: 400, statusMessage: 'Repo is required' })
    }

    // 获取 GitHub Token (如果有)
    // 暂时硬编码或从 config 获取，这里先尝试从 process.env 获取
    const token = process.env.GITHUB_TOKEN

    try {
        // 获取 Commit Activity (Last year)
        // https://docs.github.com/en/rest/metrics/statistics?apiVersion=2022-11-28#get-the-weekly-commit-activity
        const headers: any = {
            'Accept': 'application/vnd.github+json',
            'X-GitHub-Api-Version': '2022-11-28',
            'User-Agent': 'Nuxt-App'
        }

        if (token) {
            headers['Authorization'] = `Bearer ${token}`
        }

        const response = await $fetch(`https://api.github.com/repos/${repo}/stats/commit_activity`, {
            headers
        })

        return response
    } catch (e: any) {
        console.error('GitHub API Error:', e)
        // 如果是 403/429 (Rate Limit)，返回空数据或缓存数据
        return []
    }
})
