/**
 * 模块加载插件（客户端）
 * 在应用启动时加载所有启用的模块
 * 
 * 重要修复：改为非阻塞模式，避免阻塞 NuxtPage 挂载
 */

export default defineNuxtPlugin(() => {
  // 非阻塞: 不使用 async 插件，避免阻塞页面渲染
  ;(async () => {
    const { loadEnabledModules } = useModuleSystem()

    // 加载所有启用的模块
    await loadEnabledModules()
  })().catch((e) => {
    console.warn('[module-loader] 加载模块失败:', e)
  })

  // 监听模块启用/禁用事件（同步注册，不阻塞）
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
