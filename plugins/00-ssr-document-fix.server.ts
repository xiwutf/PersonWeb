/**
 * SSR Document Fix - 最早执行的插件
 * 在所有其他插件和模块导入之前设置 document mock
 * 
 * 文件名以 00- 开头确保在其他插件之前执行
 * 修复 css-render 和 vueuc 在 SSR 时访问 document.head 的错误
 */

// 在模块级别立即执行，确保在任何导入之前
// 使用立即执行函数确保在所有导入之前执行
// 必须在所有其他代码之前执行，包括任何可能的 import 语句
// 使用更早的执行时机，在 Node.js 模块加载时立即执行
;(function setupSSRDocument() {
  // 检查是否在服务端环境
  // 使用多种方式检查，确保在 Node.js 环境中正确识别
  const isServer = (typeof process !== 'undefined' && process.server) ||
                   (typeof window === 'undefined' && typeof global !== 'undefined') ||
                   (typeof globalThis !== 'undefined' && typeof globalThis.window === 'undefined')
  
  if (isServer) {
    // 获取全局对象（兼容不同环境）
    const globalObj = (function() {
      if (typeof global !== 'undefined') return global
      if (typeof globalThis !== 'undefined') return globalThis
      if (typeof window !== 'undefined') return window
      return {} as any
    })()
    
    // 创建完整的 document mock
    const createDocumentMock = () => {
      const headMock = {
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
        insertAdjacentText: () => {},
        // 添加更多可能被访问的属性
        firstChild: null,
        lastChild: null,
        parentNode: null,
        nodeType: 1,
        nodeName: 'HEAD',
        tagName: 'HEAD'
      }
      
      return {
        head: headMock,
      documentElement: {
        insertBefore: () => {},
        firstChild: headMock,
        appendChild: () => {},
        removeChild: () => {},
        setAttribute: () => {},
        getAttribute: () => null,
        nodeType: 1,
        nodeName: 'HTML',
        tagName: 'HTML'
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
      }
    }
    
    // 设置全局 document（优先级最高）
    const docMock = createDocumentMock()
    
    // 强制设置 document，即使已存在也要确保 head 存在
    // 使用 Object.defineProperty 确保属性不可被覆盖（如果可能）
    try {
      if (!globalObj.document) {
        Object.defineProperty(globalObj, 'document', {
          value: docMock,
          writable: true, // 允许后续修改，但初始值已设置
          configurable: true
        })
      } else {
        // 如果 document 已存在，确保 head 存在
        if (!globalObj.document.head || typeof globalObj.document.head.appendChild !== 'function') {
          Object.defineProperty(globalObj.document, 'head', {
            value: docMock.head,
            writable: true,
            configurable: true
          })
        }
      }
    } catch (e) {
      // 如果 defineProperty 失败（某些环境不支持），使用普通赋值
      if (!globalObj.document) {
        globalObj.document = docMock as any
      }
      if (!globalObj.document.head || typeof globalObj.document.head.appendChild !== 'function') {
        globalObj.document.head = docMock.head as any
      }
    }
    
    // 确保 globalThis 也有 document
    if (typeof globalThis !== 'undefined') {
      if (!globalThis.document) {
        globalThis.document = docMock as any
      }
      if (!globalThis.document.head || typeof globalThis.document.head.appendChild !== 'function') {
        globalThis.document.head = docMock.head as any
      }
    }
    
    // 确保 global 也有 document（Node.js 环境）
    if (typeof global !== 'undefined') {
      if (!global.document) {
        global.document = docMock as any
      }
      if (!global.document.head || typeof global.document.head.appendChild !== 'function') {
        global.document.head = docMock.head as any
      }
    }
    
    // 确保 window 也有 document（某些库可能直接访问 window.document）
    if (typeof window !== 'undefined') {
      if (!window.document) {
        window.document = docMock as any
      }
      if (!window.document.head || typeof window.document.head.appendChild !== 'function') {
        window.document.head = docMock.head as any
      }
    }
    
    // 尝试设置 css-render 的 SSR adapter（如果可用）
    // 使用同步方式，确保在模块加载时就设置
    try {
      // 检查是否可以通过 require 加载 css-render
      // 注意：在 ESM 环境中，这可能会失败，但不会影响 document mock
      if (typeof require !== 'undefined') {
        try {
          const cssRender = require('css-render')
          if (cssRender && (cssRender.default || cssRender)) {
            const cr = cssRender.default || cssRender
            // 设置 SSR adapter，避免访问 document.head
            if (typeof cr.setSSRAdapter === 'function') {
              cr.setSSRAdapter({
                adapter: (id: string, style: string) => {
                  // 在 SSR 时收集样式，但不挂载到 DOM
                  // 样式会在客户端 hydration 时应用
                }
              })
            }
          }
        } catch (e) {
          // require 失败是正常的（ESM 环境），忽略
        }
      }
    } catch (e) {
      // 设置 SSR adapter 失败不影响 document mock
    }
  }
})()

export default defineNuxtPlugin({
  name: 'ssr-document-fix',
  enforce: 'pre', // 最高优先级
  setup() {
    // 插件已经通过模块级别的代码设置了 document
    // 这里设置 css-render 的 SSR adapter，避免访问 document.head
    
    if (process.server) {
      // 尝试设置 css-render 的 SSR adapter
      // 使用动态导入避免在服务端直接 require
      Promise.resolve().then(async () => {
        try {
          // 动态导入 css-render
          const cssRenderModule = await import('css-render')
          const cssRender = cssRenderModule.default || cssRenderModule
          
          // 设置 SSR adapter，在 SSR 时不访问 document.head
          if (cssRender && typeof cssRender.mount === 'function') {
            // 创建一个 SSR adapter，收集样式但不挂载到 DOM
            const styles: Map<string, string> = new Map()
            
            cssRender.setSSRAdapter({
              adapter: (id: string, style: string) => {
                // 在 SSR 时收集样式，但不挂载到 DOM
                styles.set(id, style)
              }
            })
          }
        } catch (e) {
          // 如果 css-render 未安装或加载失败，忽略错误
          // 插件已经设置了 document mock，应该可以工作
        }
      }).catch(() => {
        // 忽略异步错误
      })
    }
  }
})

