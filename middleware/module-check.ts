/**
 * 模块检查中间件
 * 检查当前路由是否属于启用的模块
 */

export default defineNuxtRouteMiddleware((to) => {
  // 在客户端执行
  if (process.client) {
    const { isRouteEnabled } = useModuleSystem()
    
    // 检查路由是否启用
    if (!isRouteEnabled(to.path)) {
      // 如果路由未启用，重定向到首页或显示404
      return navigateTo('/')
    }
  }
})

