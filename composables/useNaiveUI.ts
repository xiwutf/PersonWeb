/**
 * 安全地使用 Naive UI 的 composables
 * 在服务端渲染时返回安全的 fallback
 * 在客户端如果 provider 未挂载，也会返回安全的 fallback
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
  if (process.client) {
    try {
      const { useMessage } = require('naive-ui')
      // 使用 nextTick 确保 provider 已经挂载
      const message = useMessage()
      // 验证 message 对象是否有效
      if (message && typeof message.success === 'function') {
        return message
      }
    } catch (e) {
      // 如果 Provider 还未挂载或出错，返回 fallback
      console.warn('useMessage failed, using fallback:', e)
    }
  }
  
  // 如果 Provider 还未挂载，返回 fallback
  return {
    success: () => {},
    error: () => {},
    warning: () => {},
    info: () => {},
    loading: () => {}
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
  if (process.client) {
    try {
      const { useDialog } = require('naive-ui')
      const dialog = useDialog()
      // 验证 dialog 对象是否有效
      if (dialog && typeof dialog.warning === 'function') {
        return dialog
      }
    } catch (e) {
      // 如果 Provider 还未挂载或出错，返回 fallback
      console.warn('useDialog failed, using fallback:', e)
    }
  }
  
  // 如果 Provider 还未挂载，返回 fallback
  return {
    warning: () => {},
    error: () => {},
    info: () => {},
    success: () => {}
  }
}

