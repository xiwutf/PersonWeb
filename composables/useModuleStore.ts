/**
 * 模块商店 Composable
 * 提供模块的浏览、搜索、下载等功能
 */

import type {
  ModuleDefinition,
  ModuleManifest,
  ModuleMarketItem,
  ModuleCategory
} from '~/types/module'

// 模块商店缓存
const storeCache = ref<{
  modules: ModuleMarketItem[]
  categories: string[]
  lastUpdate: number
}>({
  modules: [],
  categories: [],
  lastUpdate: 0
})

const isLoading = ref(false)
const error = ref<string | null>(null)

/**
 * 模块商店
 */
export const useModuleStore = () => {
  const api = useApi()

  /**
   * 获取模块列表
   */
  const getModules = async (forceRefresh = false): Promise<ModuleMarketItem[]> => {
    // 如果缓存有效且不需要强制刷新，返回缓存
    const CACHE_TTL = 5 * 60 * 1000 // 5分钟
    if (!forceRefresh && Date.now() - storeCache.value.lastUpdate < CACHE_TTL) {
      return storeCache.value.modules
    }

    try {
      isLoading.value = true
      error.value = null

      // 调用后端API获取模块列表
      const response = await api.get('/ModuleStore/modules')

      if (response.success) {
        storeCache.value.modules = response.data.modules
        storeCache.value.lastUpdate = Date.now()
        return response.data.modules
      } else {
        throw new Error(response.message || 'Failed to fetch modules')
      }
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Unknown error'
      console.error('Failed to fetch modules:', e)

      // 返回缓存的旧数据
      return storeCache.value.modules
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 获取分类列表
   */
  const getCategories = async (forceRefresh = false): Promise<string[]> => {
    // 如果缓存有效且不需要强制刷新，返回缓存
    const CACHE_TTL = 5 * 60 * 1000 // 5分钟
    if (!forceRefresh && Date.now() - storeCache.value.lastUpdate < CACHE_TTL) {
      return storeCache.value.categories
    }

    try {
      const response = await api.get('/ModuleStore/categories')

      if (response.success) {
        storeCache.value.categories = response.data.categories
        return response.data.categories
      } else {
        throw new Error(response.message || 'Failed to fetch categories')
      }
    } catch (e) {
      console.error('Failed to fetch categories:', e)

      // 返回缓存的旧数据
      return storeCache.value.categories
    }
  }

  /**
   * 搜索模块
   */
  const searchModules = async (query: string, options?: {
    category?: ModuleCategory
    author?: string
    tags?: string[]
    sortBy?: 'popular' | 'newest' | 'price'
    sortOrder?: 'asc' | 'desc'
    page?: number
    pageSize?: number
  }): Promise<{
    modules: ModuleMarketItem[]
    total: number
    page: number
    pageSize: number
  }> => {
    try {
      isLoading.value = true
      error.value = null

      const params = {
        query,
        category: options?.category,
        author: options?.author,
        tags: options?.tags?.join(','),
        sortBy: options?.sortBy || 'popular',
        sortOrder: options?.sortOrder || 'desc',
        page: options?.page || 1,
        pageSize: options?.pageSize || 20
      }

      // 过滤掉undefined的参数
      const filteredParams = Object.fromEntries(
        Object.entries(params).filter(([_, value]) => value !== undefined)
      )

      const response = await api.get('/ModuleStore/search', {
        params: filteredParams
      })

      if (response.success) {
        return {
          modules: response.data.modules,
          total: response.data.total,
          page: response.data.page,
          pageSize: response.data.pageSize
        }
      } else {
        throw new Error(response.message || 'Search failed')
      }
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Unknown error'
      console.error('Search failed:', e)

      // 返回空结果
      return {
        modules: [],
        total: 0,
        page: 1,
        pageSize: 20
      }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 获取模块详情
   */
  const getModuleDetail = async (moduleKey: string, version?: string): Promise<ModuleMarketItem | null> => {
    try {
      const params = version ? { version } : {}
      const response = await api.get(`/ModuleStore/${moduleKey}`, {
        params
      })

      if (response.success) {
        return response.data
      } else {
        throw new Error(response.message || 'Failed to fetch module detail')
      }
    } catch (e) {
      console.error('Failed to fetch module detail:', e)
      return null
    }
  }

  /**
   * 下载模块
   */
  const downloadModule = async (moduleKey: string, version?: string): Promise<{
    downloadUrl: string
    manifest: ModuleManifest
  }> => {
    try {
      const params = version ? { version } : {}
      const response = await api.post(`/ModuleStore/${moduleKey}/download`, {
        params
      })

      if (response.success) {
        return {
          downloadUrl: response.data.downloadUrl,
          manifest: response.data.manifest
        }
      } else {
        throw new Error(response.message || 'Download failed')
      }
    } catch (e) {
      console.error('Download failed:', e)
      throw e
    }
  }

  /**
   * 获取推荐模块
   */
  const getRecommendedModules = async (count = 6): Promise<ModuleMarketItem[]> => {
    try {
      const response = await api.get('/ModuleStore/recommended', {
        params: { count }
      })

      if (response.success) {
        return response.data.modules
      } else {
        throw new Error(response.message || 'Failed to fetch recommended modules')
      }
    } catch (e) {
      console.error('Failed to fetch recommended modules:', e)
      return []
    }
  }

  /**
   * 获取热门模块
   */
  const getPopularModules = async (category?: ModuleCategory, count = 10): Promise<ModuleMarketItem[]> => {
    try {
      const response = await api.get('/ModuleStore/popular', {
        params: { category, count }
      })

      if (response.success) {
        return response.data.modules
      } else {
        throw new Error(response.message || 'Failed to fetch popular modules')
      }
    } catch (e) {
      console.error('Failed to fetch popular modules:', e)
      return []
    }
  }

  /**
   * 获取模块评论
   */
  const getModuleReviews = async (moduleKey: string): Promise<Array<{
    id: string
    author: string
    rating: number
    content: string
    createdAt: string
  }>> => {
    try {
      const response = await api.get(`/ModuleStore/${moduleKey}/reviews`)

      if (response.success) {
        return response.data.reviews
      } else {
        throw new Error(response.message || 'Failed to fetch reviews')
      }
    } catch (e) {
      console.error('Failed to fetch reviews:', e)
      return []
    }
  }

  /**
   * 提交模块评论
   */
  const submitModuleReview = async (moduleKey: string, rating: number, content: string): Promise<boolean> => {
    try {
      const response = await api.post(`/ModuleStore/${moduleKey}/reviews`, {
        rating,
        content
      })

      return response.success
    } catch (e) {
      console.error('Failed to submit review:', e)
      return false
    }
  }

  /**
   * 检查模块是否已购买
   */
  const checkModulePurchase = async (moduleKey: string): Promise<{
    purchased: boolean
    licenseKey?: string
    expiresAt?: string
  }> => {
    try {
      const response = await api.get(`/ModuleStore/${moduleKey}/purchase`)

      return {
        purchased: response.data.purchased,
        licenseKey: response.data.licenseKey,
        expiresAt: response.data.expiresAt
      }
    } catch (e) {
      console.error('Failed to check purchase status:', e)
      return { purchased: false }
    }
  }

  /**
   * 购买模块
   */
  const purchaseModule = async (moduleKey: string): Promise<{
    success: boolean
    order?: {
      id: string
      amount: number
      paymentUrl: string
    }
    error?: string
  }> => {
    try {
      const response = await api.post(`/ModuleStore/${moduleKey}/purchase`)

      if (response.success) {
        return {
          success: true,
          order: response.data.order
        }
      } else {
        return {
          success: false,
          error: response.message
        }
      }
    } catch (e) {
      return {
        success: false,
        error: e instanceof Error ? e.message : 'Unknown error'
      }
    }
  }

  /**
   * 获取我的购买记录
   */
  const getPurchases = async (): Promise<Array<{
    id: string
    module: ModuleMarketItem
    price: number
    purchasedAt: string
    expiresAt?: string
    status: 'active' | 'expired' | 'refunded'
  }>> => {
    try {
      const response = await api.get('/ModuleStore/purchases')

      if (response.success) {
        return response.data.purchases
      } else {
        throw new Error(response.message || 'Failed to fetch purchases')
      }
    } catch (e) {
      console.error('Failed to fetch purchases:', e)
      return []
    }
  }

  /**
   * 清除缓存
   */
  const clearCache = () => {
    storeCache.value = {
      modules: [],
      categories: [],
      lastUpdate: 0
    }
  }

  /**
   * 监听模块更新
   */
  const onModuleUpdate = (callback: (module: ModuleMarketItem) => void): () => void => {
    // 这里应该使用WebSocket或Server-Sent Events
    // 模拟实现
    const interval = setInterval(async () => {
      const modules = await getModules(true)
      // 检查是否有更新
      // ...
    }, 30000) // 30秒检查一次

    return () => clearInterval(interval)
  }

  return {
    // 状态
    isLoading: readonly(isLoading),
    error: readonly(error),

    // 数据获取
    getModules,
    getCategories,
    getModuleDetail,
    getRecommendedModules,
    getPopularModules,

    // 搜索
    searchModules,
    getModuleReviews,
    submitModuleReview,

    // 下载和购买
    downloadModule,
    checkModulePurchase,
    purchaseModule,
    getPurchases,

    // 工具
    clearCache,
    onModuleUpdate
  }
}