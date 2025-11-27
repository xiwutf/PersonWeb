// 统一 API 响应接口
interface ApiResponse<T> {
    code: number
    message: string
    data: T
}

export const useApi = () => {
    const config = useRuntimeConfig()
    
    /**
     * 根据当前环境自动获取 API 基础路径
     * - 本地开发（localhost/127.0.0.1）: 使用本地 API
     * - 生产环境（xifg.com.cn）: 使用生产 API
     */
    const getApiBaseUrl = (): string => {
        // 客户端运行时，根据当前域名自动判断
        if (typeof window !== 'undefined') {
            const hostname = window.location.hostname
            
            // 本地开发环境
            if (hostname === 'localhost' || hostname === '127.0.0.1' || hostname === '0.0.0.0') {
                return 'http://localhost:5234/api'
            }
            
            // 生产环境（xifg.com.cn 域名）
            if (hostname.includes('xifg.com.cn')) {
                return 'https://api.xifg.com.cn/api'
            }
        }
        
        // 服务端渲染或其他情况，使用环境变量配置
        return config.public.apiBase
    }
    
    // 动态获取 API 基础路径
    const baseUrl = getApiBaseUrl()

    // 通用请求处理
    const request = async <T>(url: string, options: any = {}) => {
        try {
            // 自动携带 Token
            if (typeof window !== 'undefined') {
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
            if (error.response?.status === 401 && typeof window !== 'undefined') {
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
        del,
        baseUrl // 暴露 baseUrl 用于调试
    }
}
