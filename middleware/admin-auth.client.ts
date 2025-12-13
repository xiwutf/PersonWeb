// 客户端中间件，只在客户端运行
export default defineNuxtRouteMiddleware((to) => {
    // 简单的客户端路由守卫
    // 注意：这只是前端 UI 层面的拦截，真正的安全需要在后端 API 验证 Token

    const token = localStorage.getItem('admin_token')

    if (!token && to.path !== '/admin/login') {
        return navigateTo('/admin/login')
    }
})
