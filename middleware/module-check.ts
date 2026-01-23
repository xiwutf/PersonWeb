/**
 * 模块检查中间件
 * 检查当前路由是否属于启用的模块
 */

export default defineNuxtRouteMiddleware(async (to) => {
  // 在客户端执行
  if (process.client) {
    const { isRouteEnabled, loadEnabledModules, moduleRegistry } = useModuleSystem()
    
    // 如果模块注册表为空，说明模块可能还在加载中，先尝试加载
    // 这通常发生在直接访问链接时，插件可能还没完成加载
    if (moduleRegistry.value.size === 0) {
      try {
        await loadEnabledModules()
      } catch (e) {
        console.warn('加载模块失败，继续检查路由', e)
      }
    }
    
    // 检查路由是否启用
    if (!isRouteEnabled(to.path)) {
      // 如果路由未启用，重定向到首页或显示404
      return navigateTo('/')
    }
  }
})

