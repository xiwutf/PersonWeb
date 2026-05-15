/**
 * 模块商店 Composable
 * 数据来自 Nuxt Nitro `server/api/modules`（MySQL），不再请求 .NET 上不存在的 ModuleStore 控制器。
 */

import type { ModuleCategory, ModuleManifest, ModuleMarketItem } from '~/types/module'

const STORE_CACHE_TTL_MS = 5 * 60 * 1000

type ModulesListResponse = {
  success?: boolean
  data?: unknown[]
  pagination?: { page: number; pageSize: number; total: number; totalPages: number }
}

type ModuleDetailResponse = {
  success?: boolean
  data?: Record<string, unknown>
}

function isRecord(v: unknown): v is Record<string, unknown> {
  return typeof v === 'object' && v !== null
}

/** 将 /api/modules 列表项转为商店卡片所需结构 */
export function mapApiModuleToMarketItem(m: Record<string, unknown>): ModuleMarketItem {
  const depsRaw = m.dependencies
  const dependencies: string[] = Array.isArray(depsRaw)
    ? depsRaw
        .map((d) => {
          if (typeof d === 'string') return d
          if (isRecord(d) && typeof d.key === 'string') return d.key
          if (isRecord(d) && typeof d.name === 'string') return d.name
          return ''
        })
        .filter(Boolean)
    : []

  const key = String(m.moduleKey ?? m.key ?? '')
  const name = String(m.moduleName ?? m.name ?? key)
  const version = String(m.moduleVersion ?? m.version ?? '1.0.0')

  return {
    key,
    name,
    version,
    description: String(m.description ?? ''),
    author: String(m.author ?? ''),
    category: String(m.category ?? 'content'),
    price: typeof m.price === 'number' ? m.price : undefined,
    downloads: typeof m.downloads === 'number' ? m.downloads : Number(m.sort ?? 0) || 0,
    rating: typeof m.rating === 'number' ? m.rating : 4.5,
    screenshots: Array.isArray(m.screenshots) ? (m.screenshots as string[]) : undefined,
    tags: Array.isArray(m.tags) ? (m.tags as string[]) : [String(m.category ?? 'module')],
    dependencies,
    icon: typeof m.icon === 'string' ? m.icon : '📦',
  }
}

const storeCache = ref<{
  modules: ModuleMarketItem[]
  categories: string[]
  lastUpdate: number
}>({
  modules: [],
  categories: [],
  lastUpdate: 0,
})

const isLoading = ref(false)
const error = ref<string | null>(null)

function unwrapModulesResponse(res: unknown): { list: ModuleMarketItem[]; pagination?: ModulesListResponse['pagination'] } {
  const r = res as ModulesListResponse
  if (r?.success && Array.isArray(r.data)) {
    return { list: r.data.map((row) => mapApiModuleToMarketItem(row as Record<string, unknown>)), pagination: r.pagination }
  }
  throw new Error('获取模块列表失败')
}

export const useModuleStore = () => {
  const api = useApi()

  const getModules = async (forceRefresh = false): Promise<ModuleMarketItem[]> => {
    if (!forceRefresh && Date.now() - storeCache.value.lastUpdate < STORE_CACHE_TTL_MS && storeCache.value.modules.length > 0) {
      return storeCache.value.modules
    }

    try {
      isLoading.value = true
      error.value = null

      const res = await api.get<ModulesListResponse>('/api/modules', {
        params: { page: 1, pageSize: 500 },
      })

      const { list } = unwrapModulesResponse(res)
      storeCache.value.modules = list
      storeCache.value.lastUpdate = Date.now()

      const cats = [...new Set(list.map((x) => x.category).filter(Boolean))]
      storeCache.value.categories = cats

      return list
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Unknown error'
      console.error('Failed to fetch modules:', e)
      return storeCache.value.modules
    } finally {
      isLoading.value = false
    }
  }

  const getCategories = async (forceRefresh = false): Promise<string[]> => {
    if (!forceRefresh && storeCache.value.categories.length > 0 && Date.now() - storeCache.value.lastUpdate < STORE_CACHE_TTL_MS) {
      return storeCache.value.categories
    }
    await getModules(true)
    return storeCache.value.categories
  }

  const searchModules = async (
    query: string,
    options?: {
      category?: ModuleCategory
      author?: string
      tags?: string[]
      sortBy?: 'popular' | 'newest' | 'price'
      sortOrder?: 'asc' | 'desc'
      page?: number
      pageSize?: number
    },
  ): Promise<{
    modules: ModuleMarketItem[]
    total: number
    page: number
    pageSize: number
  }> => {
    try {
      isLoading.value = true
      error.value = null

      const page = options?.page || 1
      const pageSize = options?.pageSize || 20

      const res = await api.get<ModulesListResponse>('/api/modules', {
        params: {
          search: query || undefined,
          category: options?.category || undefined,
          page,
          pageSize,
        },
      })

      const { list, pagination } = unwrapModulesResponse(res)
      let modules = [...list]

      const sortBy = options?.sortBy || 'popular'
      const order = options?.sortOrder === 'asc' ? 1 : -1
      modules.sort((a, b) => {
        switch (sortBy) {
          case 'newest':
            return order * String(a.version).localeCompare(String(b.version))
          case 'price':
            return order * ((a.price || 0) - (b.price || 0))
          case 'popular':
          default:
            return order * (a.downloads - b.downloads)
        }
      })

      return {
        modules,
        total: pagination?.total ?? modules.length,
        page: pagination?.page ?? page,
        pageSize: pagination?.pageSize ?? pageSize,
      }
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Unknown error'
      console.error('Search failed:', e)
      return { modules: [], total: 0, page: 1, pageSize: 20 }
    } finally {
      isLoading.value = false
    }
  }

  const getModuleDetail = async (moduleKey: string, _version?: string): Promise<ModuleMarketItem | null> => {
    try {
      const res = (await api.get<ModuleDetailResponse>(`/api/modules/${moduleKey}`)) as ModuleDetailResponse
      if (res?.success && res.data && isRecord(res.data)) {
        return mapApiModuleToMarketItem(res.data)
      }
      return null
    } catch (e) {
      console.error('Failed to fetch module detail:', e)
      return null
    }
  }

  const downloadModule = async (moduleKey: string, _version?: string): Promise<{
    downloadUrl: string
    manifest: ModuleManifest
  }> => {
    const res = (await api.get<{ success?: boolean; data?: { downloadUrl: string; manifest: ModuleManifest } }>(
      `/api/modules/${moduleKey}/manifest`,
    )) as { success?: boolean; data?: { downloadUrl: string; manifest: ModuleManifest } }

    if (res?.success && res.data?.manifest) {
      return {
        downloadUrl: res.data.downloadUrl || '',
        manifest: res.data.manifest,
      }
    }
    throw new Error('无法加载模块清单')
  }

  const getRecommendedModules = async (count = 6): Promise<ModuleMarketItem[]> => {
    const all = await getModules(false)
    return [...all].sort((a, b) => b.rating - a.rating).slice(0, count)
  }

  const getPopularModules = async (_category?: ModuleCategory, count = 10): Promise<ModuleMarketItem[]> => {
    const all = await getModules(false)
    return [...all].sort((a, b) => b.downloads - a.downloads).slice(0, count)
  }

  const getModuleReviews = async (_moduleKey: string) => []

  const submitModuleReview = async (_moduleKey: string, _rating: number, _content: string) => false

  const checkModulePurchase = async (_moduleKey: string) => ({ purchased: false as const })

  const purchaseModule = async (_moduleKey: string) => ({
    success: false as const,
    error: '模块商店购买流程未接入',
  })

  const getPurchases = async () => []

  const clearCache = () => {
    storeCache.value = { modules: [], categories: [], lastUpdate: 0 }
  }

  const onModuleUpdate = (callback: (module: ModuleMarketItem) => void): (() => void) => {
    const interval = setInterval(async () => {
      const modules = await getModules(true)
      if (modules.length) {
        callback(modules[0])
      }
    }, 30000)
    return () => clearInterval(interval)
  }

  return {
    isLoading: readonly(isLoading),
    error: readonly(error),
    getModules,
    getCategories,
    getModuleDetail,
    getRecommendedModules,
    getPopularModules,
    searchModules,
    getModuleReviews,
    submitModuleReview,
    downloadModule,
    checkModulePurchase,
    purchaseModule,
    getPurchases,
    clearCache,
    onModuleUpdate,
  }
}
