/**
 * E-Commerce 模块类型定义
 */

// 商品类型
export interface Product {
  id: string
  name: string
  description: string
  price: number
  originalPrice?: number
  currency: string
  images: string[]
  category: string
  tags: string[]
  sku: string
  stock: number
  rating: number
  reviewCount: number
  specifications?: Record<string, string>
  isFeatured?: boolean
  isNew?: boolean
  isOnSale?: boolean
  createdAt: string
  updatedAt: string
}

// 商品分类
export interface Category {
  id: string
  name: string
  slug: string
  description?: string
  parentId?: string
  children?: Category[]
  image?: string
  productCount: number
}

// 购物车项
export interface CartItem {
  id: string
  productId: string
  product: Product
  quantity: number
  price: number
  totalPrice: number
  selected: boolean
  createdAt: string
  updatedAt: string
}

// 订单状态
export enum OrderStatus {
  PENDING = 'pending',
  CONFIRMED = 'confirmed',
  PROCESSING = 'processing',
  SHIPPED = 'shipped',
  DELIVERED = 'delivered',
  CANCELLED = 'cancelled',
  REFUNDED = 'refunded'
}

// 订单
export interface Order {
  id: string
  orderNumber: string
  userId: string
  items: OrderItem[]
  subtotal: number
  tax: number
  shipping: number
  total: number
  currency: string
  status: OrderStatus
  paymentMethod: string
  shippingAddress: Address
  billingAddress?: Address
  createdAt: string
  updatedAt: string
  estimatedDelivery?: string
  trackingNumber?: string
}

// 订单项
export interface OrderItem {
  id: string
  orderId: string
  productId: string
  product: Product
  quantity: number
  price: number
  totalPrice: number
}

// 地址
export interface Address {
  id?: string
  name: string
  phone: string
  email: string
  country: string
  province: string
  city: string
  district: string
  address: string
  postalCode: string
  isDefault?: boolean
}

// 评价
export interface Review {
  id: string
  productId: string
  userId: string
  userName: string
  rating: number
  comment: string
  images?: string[]
  verified: boolean
  createdAt: string
  updatedAt: string
}

// 搜索过滤器
export interface SearchFilter {
  query?: string
  category?: string
  minPrice?: number
  maxPrice?: number
  tags?: string[]
  rating?: number
  sortBy?: 'price' | 'rating' | 'newest' | 'popular'
  sortOrder?: 'asc' | 'desc'
}

// 分页参数
export interface PaginationParams {
  page: number
  limit: number
  total: number
  totalPages: number
}

// API 响应类型
export interface ApiResponse<T> {
  success: boolean
  data?: T
  message?: string
  error?: string
}

export interface PaginatedResponse<T> {
  data: T[]
  pagination: PaginationParams
}

// 用户配置
export interface UserPreferences {
  currency: string
  language: string
  theme: 'light' | 'dark'
  notifications: {
    orderUpdates: boolean
    promotions: boolean
    reviews: boolean
  }
}

// 支付方式
export interface PaymentMethod {
  id: string
  name: string
  type: 'credit_card' | 'debit_card' | 'paypal' | 'bank_transfer' | 'alipay' | 'wechat'
  enabled: boolean
  image?: string
  description?: string
}

// 配置类型
export interface EcommerceConfig {
  defaultCurrency: string
  showOutOfStock: boolean
  enableReviews: boolean
  itemsPerPage: number
  taxRate: number
}

// 事件类型
export type EcommerceEvent =
  | { type: 'product_added_to_cart'; productId: string; quantity: number }
  | { type: 'product_removed_from_cart'; productId: string }
  | { type: 'cart_cleared' }
  | { type: 'order_created'; orderId: string }
  | { type: 'order_updated'; orderId: string; status: OrderStatus }
  | { type: 'review_added'; productId: string; review: Review }