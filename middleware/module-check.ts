/**
 * 模块检查中间件
 * 检查当前路由是否属于启用的模块
 */

export default defineNuxtRouteMiddleware(async (to) => {
  // 只在客户端执行
  if (!process.client) {
    return
  }

  const { isRouteEnabled, loadEnabledModules, moduleRegistry, enabledModules } = useModuleSystem()
  
  // 如果模块注册表为空，说明模块可能还在加载中，先尝试加载
  // 这通常发生在直接访问链接时，插件可能还没完成加载
  if (moduleRegistry.value.size === 0) {
    try {
      await loadEnabledModules()
    } catch (e) {
      console.warn('加载模块失败，继续检查路由', e)
    }
  }

  // 如果加载后仍然为空，可能是模块系统未初始化
  // 为了避免误拦截，在模块系统未完全初始化时，允许所有路由访问
  // 这样可以避免在直接访问链接时，因为模块加载延迟而误拦截
  if (moduleRegistry.value.size === 0) {
    console.warn('模块注册表为空，允许访问路由（避免误拦截）:', to.path)
    return
  }

  // 检查路由是否启用
  const isEnabled = isRouteEnabled(to.path)
  
  // 调试信息（开发环境）
  if (process.env.NODE_ENV === 'development') {
    console.log('路由检查:', {
      path: to.path,
      isEnabled,
      moduleCount: moduleRegistry.value.size,
      enabledModules: Array.from(enabledModules.value)
    })
  }

  if (!isEnabled) {
    // 如果路由未启用，重定向到首页或显示404
    console.warn('路由未启用，重定向到首页:', to.path)
    return navigateTo('/')
  }
})

