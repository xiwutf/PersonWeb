/**
 * CSS Render SSR 服务端修复插件
 * 防止在服务端渲染时访问 document.head 导致的错误
 * 
 * 这个插件在服务端运行时，为 css-render 和 vueuc 提供安全的 fallback
 */

// 在模块级别就设置 document，确保在任何导入之前
// 使用立即执行函数确保在所有导入之前执行
(function setupSSRDocument() {
  if (typeof process !== 'undefined' && process.server) {
    const globalObj = typeof global !== 'undefined' ? global : 
                     typeof globalThis !== 'undefined' ? globalThis : 
                     typeof window !== 'undefined' ? window : {}
    
    // 创建完整的 document mock
    const createDocumentMock = () => ({
      head: {
        appendChild: () => {},
        removeChild: () => {},
        querySelector: () => null,
        querySelectorAll: () => [],
        insertBefore: () => {},
        remove: () => {},
        append: () => {},
        prepend: () => {},
        children: [],
        childNodes: [],
        getElementsByTagName: () => [],
        getElementsByClassName: () => []
      },
      documentElement: {
        insertBefore: () => {},
        firstChild: null,
        appendChild: () => {},
        removeChild: () => {}
      },
      body: {
        appendChild: () => {},
        removeChild: () => {}
      },
      createElement: () => ({
        setAttribute: () => {},
        appendChild: () => {},
        removeChild: () => {},
        style: {},
        className: '',
        classList: {
          add: () => {},
          remove: () => {},
          contains: () => false
        }
      }),
      createTextNode: () => ({}),
      getElementById: () => null,
      querySelector: () => null,
      querySelectorAll: () => []
    })
    
    // 设置全局 document
    if (!globalObj.document) {
      globalObj.document = createDocumentMock() as any
    } else if (!globalObj.document.head) {
      globalObj.document.head = createDocumentMock().head as any
    }
    
    // 确保 window.document 也存在（某些库可能检查 window）
    if (typeof window !== 'undefined' && !window.document) {
      window.document = globalObj.document as any
    }
  }
})()

export default defineNuxtPlugin({
  name: 'css-render-ssr-server-fix',
  enforce: 'pre', // 在其他插件之前运行
  setup() {
    // 只在服务端执行
    if (process.server) {

      // 为 css-render 提供安全的 mount 函数
      // 这样即使组件在 SSR 时被渲染，也不会报错
      // 注意：使用动态导入避免在服务端直接 require
      if (typeof global !== 'undefined') {
        // 在全局对象上设置一个标记，让 css-render 知道这是服务端环境
        (global as any).__SSR__ = true
        
        // 尝试修复 css-render（使用动态导入）
        Promise.resolve().then(async () => {
          try {
            const cssRenderModule = await import('css-render')
            const cssRender = cssRenderModule.default || cssRenderModule
            if (cssRender && typeof cssRender.mount === 'function') {
              // 保存原始的 mount 函数
              const originalMount = cssRender.mount.bind(cssRender)
              // 替换为安全的 mount 函数
              cssRender.mount = function(...args: any[]) {
                // 在服务端，不执行实际的 mount 操作
                if (typeof document === 'undefined' || !document?.head) {
                  // 返回一个空的 unmount 函数
                  return () => {}
                }
                // 在客户端，使用原始函数
                return originalMount(...args)
              }
            }
          } catch (e) {
            // 如果 css-render 未安装或加载失败，忽略错误
          }
        }).catch(() => {
          // 忽略异步错误
        })
      }
    }
  }
})

