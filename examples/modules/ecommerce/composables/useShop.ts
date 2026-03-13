/**
 * 商店相关的组合式函数
 */
import { ref, computed, reactive } from 'vue'
import type { Product, Category, SearchFilter, PaginatedResponse } from '~/types/ecommerce'

// 模拟数据
const mockProducts: Product[] = [
  {
    id: '1',
    name: '无线蓝牙耳机',
    description: '高品质音效，长续航，舒适佩戴',
    price: 299,
    originalPrice: 399,
    currency: 'CNY',
    images: ['/images/products/headphone-1.jpg', '/images/products/headphone-2.jpg'],
    category: 'electronics',
    tags: ['audio', 'wireless', 'bluetooth'],
    sku: 'WH-001',
    stock: 100,
    rating: 4.5,
    reviewCount: 128,
    isFeatured: true,
    isNew: true,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  {
    id: '2',
    name: '智能手表',
    description: '健康监测，运动追踪，消息提醒',
    price: 1299,
    currency: 'CNY',
    images: ['/images/products/smartwatch-1.jpg'],
    category: 'electronics',
    tags: ['wearable', 'health', 'smart'],
    sku: 'SW-002',
    stock: 50,
    rating: 4.2,
    reviewCount: 85,
    isFeatured: false,
    isNew: false,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  }
]

const mockCategories: Category[] = [
  {
    id: 'electronics',
    name: '电子产品',
    slug: 'electronics',
    description: '各类电子产品',
    productCount: 100,
    image: '/images/categories/electronics.jpg'
  },
  {
    id: 'clothing',
    name: '服装',
    slug: 'clothing',
    description: '时尚服饰',
    productCount: 200
  }
]

/**
 * 获取商品列表
 */
export function useProducts() {
  const products = ref<Product[]>(mockProducts)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const fetchProducts = async (filter?: SearchFilter) => {
    loading.value = true
    error.value = null

    try {
      // 模拟 API 调用
      await new Promise(resolve => setTimeout(resolve, 500))

      let filteredProducts = [...mockProducts]

      // 应用过滤条件
      if (filter?.query) {
        const query = filter.query.toLowerCase()
        filteredProducts = filteredProducts.filter(p =>
          p.name.toLowerCase().includes(query) ||
          p.description.toLowerCase().includes(query)
        )
      }

      if (filter?.category) {
        filteredProducts = filteredProducts.filter(p => p.category === filter.category)
      }

      // 排序
      if (filter?.sortBy) {
        filteredProducts.sort((a, b) => {
          let comparison = 0

          switch (filter.sortBy) {
            case 'price':
              comparison = a.price - b.price
              break
            case 'rating':
              comparison = b.rating - a.rating
              break
            case 'newest':
              comparison = new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
              break
            case 'popular':
              comparison = b.reviewCount - a.reviewCount
              break
          }

          return filter.sortOrder === 'desc' ? -comparison : comparison
        })
      }

      products.value = filteredProducts
    } catch (err) {
      error.value = 'Failed to fetch products'
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const getProduct = (id: string) => {
    return products.value.find(p => p.id === id)
  }

  return {
    products,
    loading,
    error,
    fetchProducts,
    getProduct
  }
}

/**
 * 获取分类列表
 */
export function useCategories() {
  const categories = ref<Category[]>(mockCategories)
  const loading = ref(false)

  const fetchCategories = async () => {
    loading.value = true

    try {
      // 模拟 API 调用
      await new Promise(resolve => setTimeout(resolve, 300))
      // 数据已经在 mockCategories 中
    } catch (err) {
      console.error('Failed to fetch categories:', err)
    } finally {
      loading.value = false
    }
  }

  return {
    categories,
    loading,
    fetchCategories
  }
}

/**
 * 搜索功能
 */
export function useSearch() {
  const searchQuery = ref('')
  const searchResults = ref<Product[]>([])
  const isLoading = ref(false)

  const performSearch = async (query: string) => {
    if (!query.trim()) {
      searchResults.value = []
      return
    }

    isLoading.value = true
    searchQuery.value = query

    try {
      // 模拟搜索
      await new Promise(resolve => setTimeout(resolve, 300))

      const filtered = mockProducts.filter(p =>
        p.name.toLowerCase().includes(query.toLowerCase()) ||
        p.description.toLowerCase().includes(query.toLowerCase())
      )

      searchResults.value = filtered
    } catch (err) {
      console.error('Search failed:', err)
    } finally {
      isLoading.value = false
    }
  }

  return {
    searchQuery,
    searchResults,
    isLoading,
    performSearch
  }
}

/**
 * 商品推荐
 */
export function useRecommendations() {
  const recommendations = ref<Product[]>([])
  const loading = ref(false)

  const fetchRecommendations = async (productId?: string) => {
    loading.value = true

    try {
      // 模拟获取推荐
      await new Promise(resolve => setTimeout(resolve, 400))

      // 根据当前商品推荐相关商品
      if (productId) {
        const currentProduct = mockProducts.find(p => p.id === productId)
        if (currentProduct) {
          const related = mockProducts
            .filter(p =>
              p.id !== productId &&
              (p.category === currentProduct.category ||
               p.tags.some(tag => currentProduct.tags.includes(tag)))
            )
            .slice(0, 4)

          recommendations.value = related
        }
      } else {
        // 默认推荐热门商品
        recommendations.value = mockProducts
          .filter(p => p.isFeatured)
          .slice(0, 4)
      }
    } catch (err) {
      console.error('Failed to fetch recommendations:', err)
    } finally {
      loading.value = false
    }
  }

  return {
    recommendations,
    loading,
    fetchRecommendations
  }
}