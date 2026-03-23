/**
 * 管理员认证中间件（统一版本）
 *
 * 合并原 admin-auth.ts（服务端）和 admin-auth.client.ts（客户端）
 * 修复：两个同名中间件文件冲突导致页面组件无法正常挂载
 */

export default defineNuxtRouteMiddleware((to) => {
  console.log('[admin-auth middleware] 执行中, 路径:', to.path)

  // 如果是登录页面，允许访问
  if (to.path === '/admin/login') {
    console.log('[admin-auth middleware] 登录页面，允许访问')
    return
  }

  // 客户端认证检查
  if (process.client) {
    const token = localStorage.getItem('admin_token')
    console.log('[admin-auth middleware] 客户端, token存在:', !!token)

    if (!token && to.path !== '/admin/login') {
      console.log('[admin-auth middleware] 无token，跳转登录')
      return navigateTo('/admin/login')
    }
    console.log('[admin-auth middleware] 认证通过')
  }

  // 服务端不做拦截，因为无法访问客户端的 token
  // 实际的认证验证在客户端完成
})
