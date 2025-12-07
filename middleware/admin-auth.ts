export default defineNuxtRouteMiddleware((to) => {
    // 简单的客户端路由守卫
    // 注意：这只是前端 UI 层面的拦截，真正的安全需要在后端 API 验证 Token

    // 在客户端和服务端都检查
    if (process.client) {
        const token = localStorage.getItem('admin_token')
        console.log('[Middleware:admin-auth] Checking token for path:', to.path)
        console.log('[Middleware:admin-auth] Token exists:', !!token)

        if (!token && to.path !== '/admin/login') {
            console.log('[Middleware:admin-auth] No token, redirecting to login')
            return navigateTo('/admin/login')
        }
    }
    
    // 服务端也进行基本检查（如果需要）
    if (process.server) {
        // 服务端可以检查 cookie 或其他认证信息
        // 这里暂时跳过，因为主要依赖客户端 localStorage
    }
})
