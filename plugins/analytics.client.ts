export default defineNuxtPlugin((nuxtApp) => {
  if (process.client) {
    // 获取或创建访客ID
    let visitorId = localStorage.getItem('visitor_id')
    if (!visitorId) {
      visitorId = crypto.randomUUID()
      localStorage.setItem('visitor_id', visitorId)
    }

    // 请求控制变量
    let lastTrackTime = 0
    let lastTrackPath = ''
    let isTracking = false
    let trackTimeout: NodeJS.Timeout | null = null
    let onlineStatusInterval: NodeJS.Timeout | null = null

    // 最小请求间隔（5秒）
    const MIN_TRACK_INTERVAL = 5000
    // 定期更新间隔（5分钟，从2分钟改为5分钟）
    const ONLINE_UPDATE_INTERVAL = 300000

    const trackVisit = async (force = false) => {
      // 如果正在请求中，跳过
      if (isTracking && !force) {
        console.debug('Analytics tracking already in progress, skipping')
        return
      }

      const now = Date.now()
      const currentPath = window.location.pathname + window.location.search

      // 检查请求间隔（除非强制请求）
      if (!force && now - lastTrackTime < MIN_TRACK_INTERVAL) {
        console.debug('Analytics tracking rate limited, skipping')
        return
      }

      // 如果路径没有变化且不是定期更新，跳过（除非强制）
      if (!force && currentPath === lastTrackPath && now - lastTrackTime < ONLINE_UPDATE_INTERVAL) {
        console.debug('Analytics tracking path unchanged, skipping')
        return
      }

      try {
        isTracking = true
        lastTrackTime = now
        lastTrackPath = currentPath

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
            path: currentPath,
            searchKeyword: searchKeyword
          })
        })

        // 检查响应状态
        if (!response.ok) {
          if (response.status === 429) {
            console.warn('Analytics tracking rate limited by server')
            // 如果遇到速率限制，延长下次请求时间
            lastTrackTime = now + 60000 // 延迟1分钟
          } else {
            console.debug('Analytics tracking failed:', response.status, response.statusText)
          }
        }
      } catch (err) {
        // 静默失败，不影响用户体验
        console.debug('Analytics tracking error:', err)
        // 请求失败时，也更新最后请求时间，避免频繁重试
        lastTrackTime = now
      } finally {
        isTracking = false
      }
    }

    // 防抖函数：延迟执行，如果短时间内多次调用，只执行最后一次
    const debouncedTrack = (delay = 1000) => {
      if (trackTimeout) {
        clearTimeout(trackTimeout)
      }
      trackTimeout = setTimeout(() => {
        trackVisit()
      }, delay)
    }

    // 初始追踪（延迟一下，确保页面加载完成）
    setTimeout(() => {
      trackVisit(true) // 首次请求强制执行
    }, 2000) // 从1秒改为2秒

    // 路由变化时追踪（使用防抖）
    nuxtApp.hook('page:finish', () => {
      debouncedTrack(1500) // 延迟1.5秒，避免路由切换时立即请求
    })

    // 定期更新在线状态（每5分钟，从2分钟改为5分钟）
    onlineStatusInterval = setInterval(() => {
      // 定期更新时，即使路径没变化也发送请求（用于更新在线状态）
      trackVisit(true)
    }, ONLINE_UPDATE_INTERVAL)

    // 页面卸载时清理定时器
    if (process.client) {
      window.addEventListener('beforeunload', () => {
        if (trackTimeout) {
          clearTimeout(trackTimeout)
        }
        if (onlineStatusInterval) {
          clearInterval(onlineStatusInterval)
        }
      })
    }
  }
})

