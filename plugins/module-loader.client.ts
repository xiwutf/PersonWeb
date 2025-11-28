/**
 * 模块加载插件（客户端）
 * 在应用启动时加载所有启用的模块
 */

export default defineNuxtPlugin(async () => {
  const { loadEnabledModules } = useModuleSystem()

  // 加载所有启用的模块
  await loadEnabledModules()

  // 监听模块启用/禁用事件
  if (process.client) {
    window.addEventListener('module-enabled', async (e: any) => {
      const { loadModule } = useModuleSystem()
      await loadModule(e.detail.moduleKey)
    })

    window.addEventListener('module-disabled', (e: any) => {
      // 如果当前页面属于被禁用的模块，重定向到首页
      const { isRouteEnabled } = useModuleSystem()
      const currentPath = window.location.pathname
      if (!isRouteEnabled(currentPath)) {
        window.location.href = '/'
      }
    })
  }
})

