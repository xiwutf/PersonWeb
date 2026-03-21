/**
 * 在直接打开 /admin（非登录页）时，于应用挂载前注入 Naive UI Provider 引用到 useState。
 * 使用本插件顶层的静态 import（非 lib 下的 .client 再导出文件），避免 Nitro prerender / SSR
 * 分析链加载该模块时出现「Unexpected token 'default'」等解析错误。
 */
import { markRaw } from 'vue'
import {
  NConfigProvider,
  NMessageProvider,
  NDialogProvider,
  NNotificationProvider,
  darkTheme
} from 'naive-ui'

export default defineNuxtPlugin({
  name: 'naive-admin-preload',
  enforce: 'pre',
  async setup() {
    if (import.meta.server) return

    const path = window.location.pathname
    if (!path.startsWith('/admin') || path === '/admin/login') return

    try {
      useState<{
        NConfigProvider: typeof NConfigProvider
        NMessageProvider: typeof NMessageProvider
        NDialogProvider: typeof NDialogProvider
        NNotificationProvider: typeof NNotificationProvider
        darkTheme: typeof darkTheme
      } | null>('naive-admin-provider-bundles', () => null).value = {
        NConfigProvider: markRaw(NConfigProvider),
        NMessageProvider: markRaw(NMessageProvider),
        NDialogProvider: markRaw(NDialogProvider),
        NNotificationProvider: markRaw(NNotificationProvider),
        darkTheme: markRaw(darkTheme)
      }
    } catch (e) {
      console.warn('[naive-admin-preload] 预加载失败，将回退到 AppNaiveConfig 内动态导入', e)
    }
  }
})
