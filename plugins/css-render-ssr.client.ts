/**
 * 客户端兜底：确保 css-render 可挂载到真实 document.head。
 * SSR 适配见 plugins/naive-ui-ssr.server.ts，此处不要 mock document。
 */
export default defineNuxtPlugin({
  name: 'css-render-ssr-fix',
  enforce: 'pre',
  setup() {
    if (typeof document === 'undefined' || !document.documentElement) return
    if (!document.head) {
      const head = document.createElement('head')
      document.documentElement.insertBefore(head, document.documentElement.firstChild)
    }
  },
})
