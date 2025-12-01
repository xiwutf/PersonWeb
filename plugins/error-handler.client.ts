/**
 * 前端错误捕获插件
 * 自动捕获 JavaScript 错误和未处理的 Promise 拒绝
 */
export default defineNuxtPlugin(() => {
  if (process.server) return

  const api = useApi()

  // 获取访客ID
  const getVisitorId = () => {
    let visitorId = localStorage.getItem('visitor_id')
    if (!visitorId) {
      visitorId = crypto.randomUUID()
      localStorage.setItem('visitor_id', visitorId)
    }
    return visitorId
  }

  // 发送错误到后端
  const logError = async (error: Error, errorType: string, errorInfo?: any) => {
    try {
      await api.post('/ErrorLog', {
        errorType,
        errorMessage: error.message || 'Unknown error',
        errorStack: error.stack,
        errorUrl: window.location.href,
        visitorId: getVisitorId(),
        metadata: {
          userAgent: navigator.userAgent,
          language: navigator.language,
          platform: navigator.platform,
          ...errorInfo
        }
      })
    } catch (e) {
      // 静默失败，避免错误循环
      console.error('Failed to log error:', e)
    }
  }

  // 捕获 JavaScript 错误
  window.addEventListener('error', (event) => {
    const error = event.error || new Error(event.message)
    logError(error, 'JavaScript', {
      filename: event.filename,
      lineno: event.lineno,
      colno: event.colno
    })
  })

  // 捕获未处理的 Promise 拒绝
  window.addEventListener('unhandledrejection', (event) => {
    // 忽略开发服务器连接错误（ECONNABORTED），这些是正常的开发时警告
    const errorMessage = event.reason?.message || String(event.reason)
    if (errorMessage.includes('ECONNABORTED') || errorMessage.includes('write ECONNABORTED')) {
      // 开发环境下的连接中断是正常的，不需要记录
      if (process.env.NODE_ENV === 'development') {
        return
      }
    }
    
    const error = event.reason instanceof Error 
      ? event.reason 
      : new Error(String(event.reason))
    logError(error, 'Promise', {
      reason: event.reason
    })
  })

  // Vue 错误处理
  const nuxtApp = useNuxtApp()
  nuxtApp.hook('app:error', (error: Error) => {
    logError(error, 'Vue', {
      component: error.name
    })
  })

  // 提供全局错误记录方法
  return {
    provide: {
      logError: (error: Error, errorType: string = 'Manual', errorInfo?: any) => {
        logError(error, errorType, errorInfo)
      }
    }
  }
})

