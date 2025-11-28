/**
 * 安全地使用 Naive UI 的 composables
 * 在服务端渲染时返回安全的 fallback
 */
export const useSafeMessage = () => {
  if (process.server) {
    // 服务端返回一个安全的 fallback
    return {
      success: () => {},
      error: () => {},
      warning: () => {},
      info: () => {},
      loading: () => {}
    }
  }
  
  // 客户端正常使用
  try {
    const { useMessage } = require('naive-ui')
    return useMessage()
  } catch (e) {
    // 如果 Provider 还未挂载，返回 fallback
    return {
      success: () => {},
      error: () => {},
      warning: () => {},
      info: () => {},
      loading: () => {}
    }
  }
}

export const useSafeDialog = () => {
  if (process.server) {
    // 服务端返回一个安全的 fallback
    return {
      warning: () => {},
      error: () => {},
      info: () => {},
      success: () => {}
    }
  }
  
  // 客户端正常使用
  try {
    const { useDialog } = require('naive-ui')
    return useDialog()
  } catch (e) {
    // 如果 Provider 还未挂载，返回 fallback
    return {
      warning: () => {},
      error: () => {},
      info: () => {},
      success: () => {}
    }
  }
}

