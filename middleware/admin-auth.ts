/**
 * 管理员认证中间件（服务端版本）
 * 用于服务端渲染时的认证检查
 * 注意：客户端认证逻辑在 admin-auth.client.ts 中
 */

export default defineNuxtRouteMiddleware((to) => {
  // 服务端渲染时的认证检查
  // 注意：在服务端无法访问 localStorage，所以这里主要做路由保护
  // 真正的认证验证应该在客户端进行（admin-auth.client.ts）
  
  // 如果是登录页面，允许访问
  if (to.path === '/admin/login') {
    return;
  }
  
  // 对于其他 admin 页面，在服务端不做拦截
  // 因为服务端无法访问客户端的 token
  // 实际的认证检查会在客户端进行（admin-auth.client.ts）
  
  // 如果需要服务端认证，可以在这里添加：
  // const event = useRequestEvent()
  // const token = getCookie(event, 'admin_token')
  // if (!token && to.path !== '/admin/login') {
  //   return navigateTo('/admin/login')
  // }
})
