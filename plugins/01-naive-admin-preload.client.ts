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
      const [
        configProviderModule,
        messageModule,
        dialogModule,
        notificationModule,
        themeModule
      ] = await Promise.all([
        import('naive-ui/es/config-provider'),
        import('naive-ui/es/message'),
        import('naive-ui/es/dialog'),
        import('naive-ui/es/notification'),
        import('naive-ui/es/themes')
      ])

      // naive-ui/es/message、es/notification 为命名导出，无 default（用 .default 会得到 undefined → 空白页）
      useState<{
        NConfigProvider: typeof configProviderModule.NConfigProvider
        NMessageProvider: typeof messageModule.NMessageProvider
        NDialogProvider: typeof dialogModule.NDialogProvider
        NNotificationProvider: typeof notificationModule.NNotificationProvider
        darkTheme: (typeof themeModule)['darkTheme']
      } | null>('naive-admin-provider-bundles', () => null).value = {
        NConfigProvider: markRaw(configProviderModule.NConfigProvider),
        NMessageProvider: markRaw(messageModule.NMessageProvider),
        NDialogProvider: markRaw(dialogModule.NDialogProvider),
        NNotificationProvider: markRaw(notificationModule.NNotificationProvider),
        darkTheme: markRaw(themeModule.darkTheme)
      }
    } catch (e) {
      console.warn('[naive-admin-preload] 预加载失败，将回退到 AppNaiveConfig 内动态导入', e)
    }
  }
})
