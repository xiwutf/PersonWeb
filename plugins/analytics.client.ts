export default defineNuxtPlugin((nuxtApp) => {
  if (process.client) {
    // 获取或创建访客ID
    let visitorId = localStorage.getItem('visitor_id')
    if (!visitorId) {
      visitorId = crypto.randomUUID()
      localStorage.setItem('visitor_id', visitorId)
    }

    const trackVisit = async () => {
      try {
        // 获取当前路径
        const path = window.location.pathname + window.location.search

        // 获取搜索关键词（从URL参数）
        const urlParams = new URLSearchParams(window.location.search)
        const searchKeyword = urlParams.get('q') || urlParams.get('keyword') || null

        // 获取 API 基础 URL
        const getApiBaseUrl = (): string => {
          const hostname = window.location.hostname
          if (hostname === 'localhost' || hostname === '127.0.0.1' || hostname === '0.0.0.0') {
            return 'http://localhost:5234/api'
          }
          if (hostname.includes('xifg.com.cn')) {
            return 'https://api.xifg.com.cn/api'
          }
          return 'http://localhost:5234/api' // 默认
        }

        // 发送追踪请求
        const apiBase = getApiBaseUrl()
        const response = await fetch(`${apiBase}/Analytics/track`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            visitorId: visitorId,
            path: path,
            searchKeyword: searchKeyword
          })
        })
      } catch (err) {
        // 静默失败，不影响用户体验
        console.debug('Analytics tracking error:', err)
      }
    }

    // 初始追踪（延迟一下，确保页面加载完成）
    setTimeout(() => {
      trackVisit()
    }, 1000)

    // 路由变化时追踪
    nuxtApp.hook('page:finish', () => {
      trackVisit()
    })

    // 定期更新在线状态（每2分钟）
    setInterval(() => {
      trackVisit()
    }, 120000) // 2分钟
  }
})

