/**
 * CSS Render SSR 服务端修复插件
 * 防止在服务端渲染时访问 document.head 导致的错误
 * 
 * 这个插件在服务端运行时，为 css-render 和 vueuc 提供安全的 fallback
 */

export default defineNuxtPlugin({
  name: 'css-render-ssr-server-fix',
  enforce: 'pre', // 在其他插件之前运行
  setup() {
    // 只在服务端执行
    if (process.server) {
      // 在服务端，创建一个模拟的 document 对象
      // 这必须在任何模块导入之前完成
      if (typeof global !== 'undefined') {
        // 创建一个模拟的 document 对象（仅在服务端）
        if (!global.document) {
          global.document = {
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
              childNodes: []
            },
            documentElement: {
              insertBefore: () => {},
              firstChild: null
            },
            body: {},
            createElement: () => ({
              setAttribute: () => {},
              appendChild: () => {},
              removeChild: () => {},
              style: {}
            })
          } as any
        } else if (!global.document.head) {
          // 如果 document 存在但没有 head，添加一个
          global.document.head = {
            appendChild: () => {},
            removeChild: () => {},
            querySelector: () => null,
            querySelectorAll: () => [],
            insertBefore: () => {},
            remove: () => {},
            append: () => {},
            prepend: () => {},
            children: [],
            childNodes: []
          } as any
        }
      }

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

