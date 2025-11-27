// 统一 API 响应接口
interface ApiResponse<T> {
    code: number
    message: string
    data: T
}

export const useApi = () => {
    const config = useRuntimeConfig()
    const baseUrl = config.public.apiBase

    // 通用请求处理
    const request = async <T>(url: string, options: any = {}) => {
        try {
            // 自动携带 Token
            if (process.client) {
                const token = localStorage.getItem('admin_token')
                if (token) {
                    options.headers = {
                        ...options.headers,
                        Authorization: `Bearer ${token}`
                    }
                }
            }

            const response = await $fetch<ApiResponse<T>>(url, {
                baseURL: baseUrl,
                ...options
            })

            // 统一错误处理
            // 兼容两种格式：
            // 1. 标准格式 { code: 0, data: ... }
            // 2. 直接返回数据 (code 为 undefined)
            if (response.code !== undefined && response.code !== 0) {
                throw new Error(response.message || '请求失败')
            }

            // 如果是标准格式，返回 data；否则直接返回 response
            return response.code === 0 ? response.data : response
        } catch (error: any) {
            console.error('API Error:', error)
            // 如果是 401，跳转登录
            if (error.response?.status === 401 && process.client) {
                localStorage.removeItem('admin_token')
                localStorage.removeItem('admin_user')
                navigateTo('/admin/login')
            }
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
