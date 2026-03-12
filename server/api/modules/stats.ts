import { ModuleStats } from '~/types/module'

// 模拟统计数据
export default defineEventHandler(async (event) => {
  try {
    // 模拟从数据库获取的统计数据
    const stats: ModuleStats = {
      totalModules: 15,
      enabledModules: 12,
      disabledModules: 3,
      coreModules: 2,
      categoryDistribution: {
        content: 5,
        tool: 4,
        ai: 3,
        experiment: 2,
        interaction: 1
      },
      recentDownloads: 245,
      avgRating: 4.2
    }

    // 获取详细统计信息
    const detailedStats = {
      ...stats,
      downloadsByDay: [
        { date: '2024-01-01', count: 15 },
        { date: '2024-01-02', count: 23 },
        { date: '2024-01-03', count: 31 },
        { date: '2024-01-04', count: 28 },
        { date: '2024-01-05', count: 35 },
        { date: '2024-01-06', count: 42 },
        { date: '2024-01-07', count: 38 }
      ],
      topModules: [
        { key: 'ai-assistant', name: 'AI助手', downloads: 156, rating: 4.5 },
        { key: 'blog', name: '博客系统', downloads: 134, rating: 4.3 },
        { key: 'analytics', name: '数据分析', downloads: 98, rating: 4.0 },
        { key: 'seo-tools', name: 'SEO工具', downloads: 87, rating: 4.1 },
        { key: 'newsletter', name: '邮件订阅', downloads: 76, rating: 3.9 }
      ],
      recentActivity: [
        { module: 'ai-assistant', action: 'enabled', user: 'admin', time: '2024-01-07 10:30' },
        { module: 'analytics', action: 'disabled', user: 'admin', time: '2024-01-07 09:15' },
        { module: 'comments', action: 'config_updated', user: 'admin', time: '2024-01-07 08:45' },
        { module: 'seo-tools', action: 'installed', user: 'admin', time: '2024-01-06 16:20' },
        { module: 'newsletter', action: 'updated', user: 'admin', time: '2024-01-06 14:10' }
      ]
    }

    return {
      success: true,
      data: detailedStats,
      cached: false
    }
  } catch (error) {
    throw createError({
      statusCode: 500,
      statusMessage: '获取模块统计失败',
      data: { error: error.message }
    })
  }
})