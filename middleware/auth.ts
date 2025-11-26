export default defineNuxtRouteMiddleware((to, from) => {
    const auth = useCookie('admin_auth')

    if (auth.value !== 'true') {
        return navigateTo('/')
    }
})
