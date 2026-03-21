/**
 * 在直接打开 /admin（非登录页）时，于应用挂载前预加载 Naive UI 各 Provider 分包。
 * 配合 AppNaiveConfig：首屏即可同步创建 Provider，避免 v-if 切换根节点导致整页插槽被销毁重建
 * （表现为后台首页「闪一下后空白」）。
 */
export default defineNuxtPlugin({
  name: 'naive-admin-preload',
  enforce: 'pre',
  async setup() {
    if (import.meta.server) return

    const path = window.location.pathname
    if (!path.startsWith('/admin') || path === '/admin/login') return

    try {
      // 使用包入口单次动态导入，避免线上环境对 es/xxx 深路径分包 404 / 缓存不一致
      const naive = await import('naive-ui')

      useState<{
        NConfigProvider: (typeof naive)['NConfigProvider']
        NMessageProvider: (typeof naive)['NMessageProvider']
        NDialogProvider: (typeof naive)['NDialogProvider']
        NNotificationProvider: (typeof naive)['NNotificationProvider']
        darkTheme: (typeof naive)['darkTheme']
      } | null>('naive-admin-provider-bundles', () => null).value = {
        NConfigProvider: markRaw(naive.NConfigProvider),
        NMessageProvider: markRaw(naive.NMessageProvider),
        NDialogProvider: markRaw(naive.NDialogProvider),
        NNotificationProvider: markRaw(naive.NNotificationProvider),
        darkTheme: markRaw(naive.darkTheme)
      }
    } catch (e) {
      console.warn('[naive-admin-preload] 预加载失败，将回退到 AppNaiveConfig 内动态导入', e)
    }
  }
})
