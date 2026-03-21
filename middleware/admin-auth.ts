/**
 * 管理员认证中间件（统一版本）
 * 
 * 合并原 admin-auth.ts（服务端）和 admin-auth.client.ts（客户端）
 * 修复：两个同名中间件文件冲突导致页面组件无法正常挂载
 */

export default defineNuxtRouteMiddleware((to) => {
  // 如果是登录页面，允许访问
  if (to.path === '/admin/login') {
    return
  }

  // 客户端认证检查
  if (process.client) {
    const token = localStorage.getItem('admin_token')

    if (!token && to.path !== '/admin/login') {
      return navigateTo('/admin/login')
    }
  }

  // 服务端不做拦截，因为无法访问客户端的 token
  // 实际的认证验证在客户端完成
})
