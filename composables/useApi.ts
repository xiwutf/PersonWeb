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
     * - 生产环境（xifg.com.cn）: 使用 https://api.xifg.com.cn/api
     * - 生产环境（xing.com.cn）: 使用 https://api.xing.com.cn/api
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

            // 生产环境（xing.com.cn 域名）
            if (hostname.includes('xing.com.cn')) {
                return 'https://api.xing.com.cn/api'
            }
        }

        // 服务端渲染或未匹配域名，使用环境变量配置
        return config.public.apiBase
    }
    
    // 动态获取 API 基础路径
    const baseUrl = getApiBaseUrl()

    // 通用请求处理
    const request = async <T>(url: string, options: any = {}) => {
        const { silent, ...fetchOptions } = options
        try {
            // 自动携带 Token
            if (typeof window !== 'undefined') {
                const token = localStorage.getItem('admin_token')
                if (token) {
                    fetchOptions.headers = {
                        ...fetchOptions.headers,
                        Authorization: `Bearer ${token}`
                    }
                }
            }

            // 判断是否为 Nuxt server API（以 /api/ 开头）
            // Nuxt server API 应该直接调用，不添加后端 baseURL
            // 注意：在生产环境静态生成后，Nuxt Server API 可能不可用
            const isNuxtServerAPI = url.startsWith('/api/')
            
            // 如果是 Nuxt Server API，直接使用相对路径，不添加 baseURL
            // 在生产环境静态生成后，这些路由可能不存在，需要确保部署时包含 server 功能
            // 或者使用 SSR 模式（nuxt build）而不是静态生成（nuxt generate）
            let finalBaseURL: string | undefined = undefined
            
            if (isNuxtServerAPI) {
                // Nuxt Server API：使用相对路径，不添加 baseURL
                finalBaseURL = undefined
            } else {
                // 后端 API：添加 baseURL
                finalBaseURL = baseUrl
            }
            
            // 移除详细的请求日志，减少控制台输出
            // 只在需要调试时手动启用
            // if (process.env.NODE_ENV === 'development') {
            //     console.log(`[useApi] ${options.method || 'GET'} ${url}`, {
            //         isNuxtServerAPI,
            //         baseURL: finalBaseURL,
            //         originalBaseUrl: baseUrl,
            //         finalUrl: finalBaseURL ? `${finalBaseURL}${url}` : url
            //     })
            // }
            
            const response = await $fetch<ApiResponse<T>>(url, {
                baseURL: finalBaseURL,
                ...fetchOptions
            })

            // 统一错误处理
            // 兼容两种格式：
            // 1. 标准格式 { code: 0, data: ... }
            // 2. 直接返回数据 (code 为 undefined)
            if (response.code !== undefined && response.code !== 0) {
                // 后端返回错误时，message 字段包含错误信息
                const errorMessage = response.message || '请求失败'
                const error = new Error(errorMessage)
                // 添加 code 属性，方便错误处理
                ;(error as any).code = response.code
                throw error
            }

            // 如果是标准格式，返回 data；否则直接返回 response
            const result = response.code === 0 ? response.data : response
            // 移除响应日志，减少控制台输出
            // 只在需要调试时手动启用
            // if (process.env.NODE_ENV === 'development') {
            //     console.log(`[API] ${options.method || 'GET'} ${url}:`, result)
            // }
            return result
        } catch (error: any) {
            if (!silent && typeof window !== 'undefined') {
                console.error('API Error:', error)
                console.error('API Error URL:', url)
                console.error('API Error Response:', error.response)
            }
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
    const post = <T>(url: string, body: unknown, options = {}) => {
        return request<T>(url, { method: 'POST', body, ...options })
    }

    // PUT 请求
    const put = <T>(url: string, body: unknown, options = {}) => {
        return request<T>(url, { method: 'PUT', body, ...options })
    }

    // PATCH 请求
    const patch = <T>(url: string, body: unknown, options = {}) => {
        return request<T>(url, { method: 'PATCH', body, ...options })
    }

    // DELETE 请求
    const del = <T>(url: string, options = {}) => {
        return request<T>(url, { method: 'DELETE', ...options })
    }

    // DELETE 别名（更符合常见命名习惯）
    const deleteMethod = del

    return {
        get,
        post,
        put,
        patch,
        del,
        delete: deleteMethod, // 添加 delete 方法作为 del 的别名
        baseUrl // 暴露 baseUrl 用于调试
    }
}
