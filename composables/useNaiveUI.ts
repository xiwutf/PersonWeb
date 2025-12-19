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

// 缓存 message 实例，避免重复初始化
let messageInstance: any = null
let messageInitialized = false

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
  
  // 客户端：懒加载 message 实例
  if (process.client) {
    // 如果已经初始化过且有效，直接返回
    if (messageInitialized && messageInstance) {
      return messageInstance
    }
    
    // 尝试初始化 message
    try {
      // 确保模块已加载
      if (naiveUIModule && naiveUIModule.useMessage) {
        try {
          const message = naiveUIModule.useMessage()
          // 验证 message 对象是否有效
          if (message && typeof message.success === 'function') {
            messageInstance = message
            messageInitialized = true
            return message
          }
        } catch (e) {
          // useMessage() 调用失败（Provider 未挂载），返回 fallback
          // 不输出错误，因为这是预期的行为（在 provider 挂载前）
          messageInitialized = true // 标记为已尝试，避免重复尝试
        }
      }
    } catch (e) {
      // 模块加载或使用出错，静默返回 fallback
      messageInitialized = true
    }
  }
  
  // 如果 Provider 还未挂载或初始化失败，返回安全的 fallback
  // fallback 函数会在 Provider 准备好后自动使用真正的 message（通过懒加载）
  return {
    success: (content: string) => {
      try {
        if (process.client && naiveUIModule && naiveUIModule.useMessage) {
          const msg = naiveUIModule.useMessage()
          if (msg && typeof msg.success === 'function') {
            messageInstance = msg
            messageInitialized = true
            msg.success(content)
            return
          }
        }
      } catch (e) {
        // 静默失败
      }
      // fallback: 在控制台输出（仅开发环境）
      if (import.meta.dev) {
        console.log('[Message]', content)
      }
    },
    error: (content: string) => {
      try {
        if (process.client && naiveUIModule && naiveUIModule.useMessage) {
          const msg = naiveUIModule.useMessage()
          if (msg && typeof msg.error === 'function') {
            messageInstance = msg
            messageInitialized = true
            msg.error(content)
            return
          }
        }
      } catch (e) {
        // 静默失败
      }
      if (import.meta.dev) {
        console.error('[Message Error]', content)
      }
    },
    warning: (content: string) => {
      try {
        if (process.client && naiveUIModule && naiveUIModule.useMessage) {
          const msg = naiveUIModule.useMessage()
          if (msg && typeof msg.warning === 'function') {
            messageInstance = msg
            messageInitialized = true
            msg.warning(content)
            return
          }
        }
      } catch (e) {
        // 静默失败
      }
      if (import.meta.dev) {
        console.warn('[Message Warning]', content)
      }
    },
    info: (content: string) => {
      try {
        if (process.client && naiveUIModule && naiveUIModule.useMessage) {
          const msg = naiveUIModule.useMessage()
          if (msg && typeof msg.info === 'function') {
            messageInstance = msg
            messageInitialized = true
            msg.info(content)
            return
          }
        }
      } catch (e) {
        // 静默失败
      }
      if (import.meta.dev) {
        console.info('[Message Info]', content)
      }
    },
    loading: (content: string) => {
      try {
        if (process.client && naiveUIModule && naiveUIModule.useMessage) {
          const msg = naiveUIModule.useMessage()
          if (msg && typeof msg.loading === 'function') {
            messageInstance = msg
            messageInitialized = true
            return msg.loading(content)
          }
        }
      } catch (e) {
        // 静默失败
      }
      return { destroy: () => {} } // 返回一个销毁函数，避免调用错误
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

