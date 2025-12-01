/**
 * CSS Render SSR 兼容插件
 * 修复 Naive UI 在 SSR 环境下的 css-render 错误
 * 
 * 这个插件确保在客户端环境中，css-render 可以正常访问 document.head
 * 同时防止在服务端渲染时访问 document
 */

export default defineNuxtPlugin({
  name: 'css-render-ssr-fix',
  enforce: 'pre', // 在其他插件之前运行
  setup() {
    // 只在客户端执行
    if (process.client) {
      // 确保 document 和 document.head 存在
      if (typeof document !== 'undefined') {
        // 如果 document.head 不存在（理论上不应该发生），创建一个
        if (!document.head && document.documentElement) {
          const head = document.createElement('head')
          document.documentElement.insertBefore(head, document.documentElement.firstChild)
        }
      }
    } else {
      // 服务端：模拟 document.head 以防止错误
      // 注意：这不会真正创建 DOM，只是防止访问 undefined
      if (typeof global !== 'undefined' && !global.document) {
        global.document = {
          head: null,
          documentElement: null
        } as any
      }
    }
  }
})
