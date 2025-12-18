/**
 * SSR Document Fix - 最早执行的插件
 * 在所有其他插件和模块导入之前设置 document mock
 * 
 * 文件名以 00- 开头确保在其他插件之前执行
 */

// 在模块级别立即执行，确保在任何导入之前
// 使用立即执行函数确保在所有导入之前执行
(function setupSSRDocument() {
  // 检查是否在服务端环境
  const isServer = typeof process !== 'undefined' && process.server
  
  if (isServer) {
    // 获取全局对象（兼容不同环境）
    const globalObj = (function() {
      if (typeof global !== 'undefined') return global
      if (typeof globalThis !== 'undefined') return globalThis
      if (typeof window !== 'undefined') return window
      return {} as any
    })()
    
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
        getElementsByClassName: () => [],
        insertAdjacentElement: () => {},
        insertAdjacentHTML: () => {},
        insertAdjacentText: () => {}
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
          contains: () => false,
          toggle: () => false
        }
      }),
      createTextNode: () => ({}),
      getElementById: () => null,
      querySelector: () => null,
      querySelectorAll: () => []
    })
    
    // 设置全局 document（优先级最高）
    const docMock = createDocumentMock()
    
    if (!globalObj.document) {
      globalObj.document = docMock as any
    } else if (!globalObj.document.head) {
      globalObj.document.head = docMock.head as any
    }
    
    // 确保 globalThis 也有 document
    if (typeof globalThis !== 'undefined') {
      if (!globalThis.document) {
        globalThis.document = docMock as any
      } else if (!globalThis.document.head) {
        globalThis.document.head = docMock.head as any
      }
    }
    
    // 确保 global 也有 document（Node.js 环境）
    if (typeof global !== 'undefined') {
      if (!global.document) {
        global.document = docMock as any
      } else if (!global.document.head) {
        global.document.head = docMock.head as any
      }
    }
  }
})()

export default defineNuxtPlugin({
  name: 'ssr-document-fix',
  enforce: 'pre', // 最高优先级
  setup() {
    // 插件已经通过模块级别的代码设置了 document
    // 这里只是确保插件被注册
  }
})

