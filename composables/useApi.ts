// 简单的 API 客户端封装
export const useApi = () => {
    const config = useRuntimeConfig()
    const baseUrl = config.public.apiBase

    // 通用请求处理
    const request = async <T>(url: string, options: any = {}) => {
        try {
            // 这里可以处理 token 自动携带等逻辑
            // const token = useCookie('admin_token')
            // if (token.value) {
            //   options.headers = { ...options.headers, Authorization: `Bearer ${token.value}` }
            // }

            return await $fetch<T>(url, {
                baseURL: baseUrl,
                ...options
            })
        } catch (error) {
            // 统一错误处理
            console.error('API Error:', error)
            throw error
        }
    }

    // GET 请求
    const get = <T>(url: string, options = {}) => {
        return request<T>(url, { method: 'GET', ...options })
    }

    // POST 请求
    const post = <T>(url: string, body: any, options = {}) => {
        return request<T>(url, { method: 'POST', body, ...options })
    }

    // PUT 请求
    const put = <T>(url: string, body: any, options = {}) => {
        return request<T>(url, { method: 'PUT', body, ...options })
    }

    // DELETE 请求
    const del = <T>(url: string, options = {}) => {
        return request<T>(url, { method: 'DELETE', ...options })
    }

    return {
        get,
        post,
        put,
        del
    }
}
