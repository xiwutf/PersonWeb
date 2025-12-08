/**
 * 安全地使用 Naive UI 的 composables
 * 在服务端渲染时返回安全的 fallback
 * 在客户端如果 provider 未挂载，也会返回安全的 fallback
 */

// 在客户端动态导入 Naive UI（避免在构建时被打包到服务端）
let naiveUIModule: any = null
if (process.client) {
  // 使用动态导入，只在客户端加载
  import('naive-ui').then((module) => {
    naiveUIModule = module
  }).catch(() => {
    // 导入失败，使用 fallback
  })
}

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
      // 如果模块已加载，尝试使用
      if (naiveUIModule && naiveUIModule.useMessage) {
        const message = naiveUIModule.useMessage()
        // 验证 message 对象是否有效
        if (message && typeof message.success === 'function') {
          return message
        }
      }
    } catch (e) {
      // 如果 Provider 还未挂载或出错，静默返回 fallback
      // 不输出警告，因为这是预期的行为（在 provider 挂载前）
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
      // 如果模块已加载，尝试使用
      if (naiveUIModule && naiveUIModule.useDialog) {
        const dialog = naiveUIModule.useDialog()
        // 验证 dialog 对象是否有效
        if (dialog && typeof dialog.warning === 'function') {
          return dialog
        }
      }
    } catch (e) {
      // 如果 Provider 还未挂载或出错，静默返回 fallback
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

