# E-Commerce 模块 API 文档

## 概述

E-Commerce 模块提供了完整的电商功能，包括商品管理、购物车、订单处理、支付集成等。本文档详细介绍了 E-Commerce 模块的所有 API 接口。

## 目录

1. [商品管理 API](#商品管理-api)
2. [购物车 API](#购物车-api)
3. [订单管理 API](#订单管理-api)
4. [支付系统 API](#支付系统-api)
5. [用户评价 API](#用户评价-api)
6. [搜索和过滤 API](#搜索和过滤-api)
7. [配置管理 API](#配置管理-api)
8. [类型定义](#类型定义)

---

## 商品管理 API

### useProducts

获取和管理商品数据

```typescript
const { products, loading, error, fetchProducts, getProduct } = useProducts()
```

#### fetchProducts

获取商品列表

```typescript
/**
 * 获取商品列表
 * @param filter 过滤条件
 */
fetchProducts(filter?: SearchFilter): Promise<void>
```

**SearchFilter:**
```typescript
interface SearchFilter {
  query?: string            // 搜索关键词
  category?: string          // 分类
  minPrice?: number          // 最低价格
  maxPrice?: number          // 最高价格
  tags?: string[]           // 标签
  rating?: number           // 最低评分
  sortBy?: 'price' | 'rating' | 'newest' | 'popular'
  sortOrder?: 'asc' | 'desc'
}
```

**示例：**
```typescript
// 获取电子产品分类的商品
await fetchProducts({
  category: 'electronics',
  sortBy: 'price',
  sortOrder: 'asc'
})

// 搜索包含"耳机"的商品
await fetchProducts({
  query: '耳机',
  minPrice: 100
})
```

#### getProduct

获取单个商品详情

```typescript
/**
 * 获取商品详情
 * @param id 商品ID
 */
getProduct(id: string): Product | undefined
```

### useCategories

管理商品分类

```typescript
const { categories, loading, fetchCategories } = useCategories()
```

#### fetchCategories

获取分类列表

```typescript
/**
 * 获取分类列表
 */
fetchCategories(): Promise<void>
```

### useRecommendations

商品推荐功能

```typescript
const { recommendations, loading, fetchRecommendations } = useRecommendations()
```

#### fetchRecommendations

获取推荐商品

```typescript
/**
 * 获取推荐商品
 * @param productId 当前商品ID（用于相关推荐）
 */
fetchRecommendations(productId?: string): Promise<void>
```

---

## 购物车 API

### useCart

购物车管理

```typescript
const {
  cart,
  addToCart,
  updateQuantity,
  removeFromCart,
  clearCart,
  formatPrice
} = useCart()
```

#### addToCart

添加商品到购物车

```typescript
/**
 * 添加商品到购物车
 * @param product 商品对象
 * @param quantity 数量
 */
addToCart(product: Product, quantity?: number): void
```

**示例：**
```typescript
const product = getProduct('1')
if (product) {
  addToCart(product, 2)
}
```

#### updateQuantity

更新购物车商品数量

```typescript
/**
 * 更新购物车商品数量
 * @param productId 商品ID
 * @param quantity 新数量
 */
updateQuantity(productId: string, quantity: number): void
```

#### removeFromCart

从购物车移除商品

```typescript
/**
 * 从购物车移除商品
 * @param productId 商品ID
 */
removeItem(productId: string): void
```

#### clearCart

清空购物车

```typescript
/**
 * 清空购物车
 */
clearCart(): void
```

#### formatPrice

格式化价格显示

```typescript
/**
 * 格式化价格
 * @param price 价格
 */
formatPrice(price: number): string
```

### useCartStore (Pinia Store)

购物车状态管理

```typescript
const cartStore = useCartStore()

// 购物车状态
cartStore.items          // 购物车项目
cartStore.totalItems     // 总数量
cartStore.subtotal      // 小计
cartStore.tax           // 税费
cartStore.total         // 总计

// 方法
cartStore.addItem()       // 添加商品
cartStore.updateItemQuantity()  // 更新数量
cartStore.removeItem()    // 移除商品
cartStore.clearCart()    // 清空
```

---

## 订单管理 API

### useOrders

订单管理功能

```typescript
const {
  orders,
  loading,
  error,
  createOrder,
  getOrder,
  updateOrderStatus,
  fetchUserOrders
} = useOrders()
```

#### createOrder

创建订单

```typescript
/**
 * 创建订单
 * @param items 订单项
 * @param shippingAddress 收货地址
 * @param paymentMethod 支付方式
 */
createOrder(
  items: OrderItem[],
  shippingAddress: Address,
  paymentMethod: string
): Promise<Order>
```

**示例：**
```typescript
const selectedItems = cartStore.getSelectedItems()
const order = await createOrder(
  selectedItems,
  address,
  'credit_card'
)
```

#### getOrder

获取订单详情

```typescript
/**
 * 获取订单详情
 * @param orderId 订单ID
 */
getOrder(orderId: string): Promise<Order | null>
```

#### updateOrderStatus

更新订单状态

```typescript
/**
 * 更新订单状态
 * @param orderId 订单ID
 * @param status 新状态
 */
updateOrderStatus(orderId: string, status: OrderStatus): Promise<boolean>
```

#### fetchUserOrders

获取用户订单列表

```typescript
/**
 * 获取用户订单列表
 * @param page 页码
 * @param limit 每页数量
 */
fetchUserOrders(page?: number, limit?: number): Promise<PaginatedResponse<Order>>
```

### useCheckout

结算流程管理

```typescript
const {
  checkout,
  isProcessing,
  error,
  validateOrder,
  estimateShipping
} = useCheckout()
```

#### checkout

执行结算

```typescript
/**
 * 执行结算
 * @param orderData 订单数据
 * @param paymentMethod 支付方式
 */
checkout(orderData: Order, paymentMethod: string): Promise<Order>
```

#### validateOrder

验证订单数据

```typescript
/**
 * 验证订单数据
 * @param order 订单数据
 */
validateOrder(order: Order): { valid: boolean, errors: string[] }
```

#### estimateShipping

估算运费

```typescript
/**
 * 估算运费
 * @param address 地址
 * @param items 商品列表
 */
estimateShipping(address: Address, items: Product[]): Promise<number>
```

---

## 支付系统 API

### usePayment

支付管理

```typescript
const {
  availableMethods,
  processPayment,
  refundPayment,
  verifyPayment
} = usePayment()
```

#### processPayment

处理支付

```typescript
/**
 * 处理支付
 * @param orderId 订单ID
 * @param paymentMethod 支付方式
 * @param paymentToken 支付令牌
 */
processPayment(
  orderId: string,
  paymentMethod: string,
  paymentToken: string
): Promise<PaymentResult>
```

**PaymentResult:**
```typescript
interface PaymentResult {
  success: boolean
  transactionId?: string
  error?: string
  redirectUrl?: string
}
```

#### refundPayment

退款

```typescript
/**
 * 退款
 * @param paymentId 支付ID
 * @param amount 退款金额
 * @param reason 退款原因
 */
refundPayment(
  paymentId: string,
  amount?: number,
  reason?: string
): Promise<RefundResult>
```

#### verifyPayment

验证支付结果

```typescript
/**
 * 验证支付结果
 * @param transactionId 交易ID
 */
verifyPayment(transactionId: string): Promise<boolean>
```

### usePaymentMethods

支付方式管理

```typescript
const { paymentMethods, loadPaymentMethods } = usePaymentMethods()
```

#### loadPaymentMethods

加载可用支付方式

```typescript
/**
 * 加载可用支付方式
 */
loadPaymentMethods(): Promise<PaymentMethod[]>
```

---

## 用户评价 API

### useReviews

商品评价管理

```typescript
const {
  reviews,
  loading,
  error,
  addReview,
  getReviews,
  helpfulVote
} = useReviews()
```

#### addReview

添加评价

```typescript
/**
 * 添加评价
 * @param productId 商品ID
 * @param review 评价数据
 */
addReview(productId: string, review: ReviewData): Promise<Review>
```

**ReviewData:**
```typescript
interface ReviewData {
  rating: number        // 评分 (1-5)
  comment: string       // 评价内容
  images?: string[]     // 图片
  verified: boolean      // 是否已验证购买
}
```

#### getReviews

获取商品评价

```typescript
/**
 * 获取商品评价
 * @param productId 商品ID
 * @param page 页码
 * @param limit 每页数量
 */
getReviews(productId: string, page?: number, limit?: number): Promise<PaginatedResponse<Review>>
```

#### helpfulVote

对评价投票

```typescript
/**
 * 评价有用投票
 * @param reviewId 评价ID
 * @param vote 投票类型 (up/down)
 */
helpfulVote(reviewId: string, vote: 'up' | 'down'): Promise<boolean>
```

---

## 搜索和过滤 API

### useSearch

商品搜索功能

```typescript
const {
  searchQuery,
  searchResults,
  isLoading,
  performSearch,
  getSuggestions
} = useSearch()
```

#### performSearch

执行搜索

```typescript
/**
 * 执行搜索
 * @param query 搜索关键词
 */
performSearch(query: string): Promise<void>
```

#### getSuggestions

获取搜索建议

```typescript
/**
 * 获取搜索建议
 * @param query 搜索关键词
 */
getSuggestions(query: string): Promise<string[]>
```

### useFilters

高级过滤

```typescript
const {
  filters,
  updateFilter,
  resetFilters,
  applyFilters
} = useFilters()
```

#### updateFilter

更新过滤条件

```typescript
/**
 * 更新过滤条件
 * @param key 过滤键
 * @param value 过滤值
 */
updateFilter<T>(key: string, value: T): void
```

#### applyFilters

应用过滤条件

```typescript
/**
 * 应用过滤条件
 */
applyFilters(): Promise<Product[]>
```

---

## 配置管理 API

### useEcommerceConfig

电商模块配置

```typescript
const {
  config,
  updateConfig,
  resetConfig,
  currencies,
  settings
} = useEcommerceConfig()
```

#### updateConfig

更新配置

```typescript
/**
 * 更新配置
 * @param newConfig 新配置
 */
updateConfig(newConfig: Partial<EcommerceConfig>): Promise<void>
```

**EcommerceConfig:**
```typescript
interface EcommerceConfig {
  defaultCurrency: string     // 默认货币
  showOutOfStock: boolean     // 显示缺货商品
  enableReviews: boolean      // 启用评价
  itemsPerPage: number        // 每页数量
  taxRate: number             // 税率
}
```

#### resetConfig

重置配置

```typescript
/**
 * 重置为默认配置
 */
resetConfig(): Promise<void>
```

### useUserPreferences

用户偏好设置

```typescript
const {
  preferences,
  updatePreferences,
  savePreferences
} = useUserPreferences()
```

#### updatePreferences

更新用户偏好

```typescript
/**
 * 更新用户偏好
 * @param newPreferences 新偏好设置
 */
updatePreferences(newPreferences: Partial<UserPreferences>): void
```

---

## 类型定义

### 核心类型

#### Product

商品类型

```typescript
interface Product {
  id: string                    // 商品ID
  name: string                  // 商品名称
  description: string           // 商品描述
  price: number                 // 价格
  originalPrice?: number        // 原价
  currency: string              // 货币
  images: string[]              // 图片列表
  category: string              // 分类
  tags: string[]                // 标签
  sku: string                   // SKU
  stock: number                 // 库存
  rating: number                // 评分
  reviewCount: number           // 评价数量
  specifications?: Record<string, string>  // 规格
  isFeatured?: boolean           // 是否特色商品
  isNew?: boolean                // 是否新品
  isOnSale?: boolean             // 是否促销
  createdAt: string             // 创建时间
  updatedAt: string              // 更新时间
}
```

#### Order

订单类型

```typescript
interface Order {
  id: string                    // 订单ID
  orderNumber: string           // 订单号
  userId: string                // 用户ID
  items: OrderItem[]            // 订单项
  subtotal: number              // 小计
  tax: number                   // 税费
  shipping: number               // 运费
  total: number                 // 总计
  currency: string              // 货币
  status: OrderStatus           // 订单状态
  paymentMethod: string          // 支付方式
  shippingAddress: Address       // 收货地址
  billingAddress?: Address       // 账单地址
  createdAt: string             // 创建时间
  updatedAt: string              // 更新时间
  estimatedDelivery?: string     // 预计送达时间
  trackingNumber?: string       // 物流单号
}
```

#### OrderStatus

订单状态枚举

```typescript
enum OrderStatus {
  PENDING = 'pending',          // 待处理
  CONFIRMED = 'confirmed',      // 已确认
  PROCESSING = 'processing',    // 处理中
  SHIPPED = 'shipped',          // 已发货
  DELIVERED = 'delivered',      // 已送达
  CANCELLED = 'cancelled',      // 已取消
  REFUNDED = 'refunded'         // 已退款
}
```

#### Address

地址类型

```typescript
interface Address {
  id?: string                   // 地址ID
  name: string                  // 收货人姓名
  phone: string                 // 电话
  email: string                 // 邮箱
  country: string               // 国家
  province: string              // 省份
  city: string                  // 城市
  district: string              // 区县
  address: string               // 详细地址
  postalCode: string            // 邮编
  isDefault?: boolean           // 是否默认地址
}
```

#### Review

评价类型

```typescript
interface Review {
  id: string                    // 评价ID
  productId: string             // 商品ID
  userId: string                // 用户ID
  userName: string              // 用户名
  rating: number                // 评分
  comment: string               // 评价内容
  images?: string[]             // 图片
  verified: boolean              // 是否已验证购买
  createdAt: string             // 创建时间
  updatedAt: string              // 更新时间
}
```

### 辅助类型

#### CartItem

购物车项类型

```typescript
interface CartItem {
  id: string                    // 购物车项ID
  productId: string             // 商品ID
  product: Product              // 商品信息
  quantity: number               // 数量
  price: number                  // 单价
  totalPrice: number              // 总价
  selected: boolean              // 是否选中
  createdAt: string             // 创建时间
  updatedAt: string              // 更新时间
}
```

#### OrderItem

订单项类型

```typescript
interface OrderItem {
  id: string                    // 订单项ID
  orderId: string                // 订单ID
  productId: string             // 商品ID
  product: Product              // 商品信息
  quantity: number               // 数量
  price: number                  // 单价
  totalPrice: number              // 总价
}
```

#### PaginatedResponse

分页响应类型

```typescript
interface PaginatedResponse<T> {
  data: T[]                     // 数据列表
  pagination: {
    page: number                // 当前页码
    limit: number               // 每页数量
    total: number               // 总数量
    totalPages: number          // 总页数
  }
}
```

---

## 错误处理

### 常见错误类型

- `ProductNotFoundError`: 商品未找到
- `InsufficientStockError`: 库存不足
- `OrderValidationError`: 订单验证失败
- `PaymentError`: 支付失败
- `NetworkError`: 网络错误

### 错误处理示例

```typescript
try {
  const order = await createOrder(items, address, 'credit_card')
  console.log('订单创建成功:', order)
} catch (error) {
  if (error instanceof ProductNotFoundError) {
    console.error('商品不存在:', error.productId)
  } else if (error instanceof InsufficientStockError) {
    console.error('库存不足:', error.message)
  } else if (error instanceof PaymentError) {
    console.error('支付失败:', error.message)
  }
}
```

---

## 使用示例

### 基本使用

```typescript
// 1. 获取商品
const { fetchProducts, products } = useProducts()
await fetchProducts({ category: 'electronics' })

// 2. 添加到购物车
const { addToCart } = useCart()
const product = products.value[0]
addToCart(product, 1)

// 3. 创建订单
const { createOrder } = useOrders()
const order = await createOrder(
  cartStore.getSelectedItems(),
  address,
  'credit_card'
)
```

### 高级搜索

```typescript
const { performSearch, searchResults } = useSearch()

// 搜索电子类商品，价格在100-1000之间，评分4星以上
await performSearch('耳机')
const filtered = useFilters()
filtered.updateFilter('category', 'electronics')
filtered.updateFilter('minPrice', 100)
filtered.updateFilter('maxPrice', 1000)
filtered.updateFilter('rating', 4)
const results = await filtered.applyFilters()
```

### 支付处理

```typescript
const { processPayment } = usePayment()

try {
  const result = await processPayment(
    order.id,
    'credit_card',
    paymentToken
  )

  if (result.success) {
    console.log('支付成功，交易ID:', result.transactionId)
  } else {
    console.error('支付失败:', result.error)
  }
} catch (error) {
  console.error('支付处理异常:', error)
}
```

---

## 版本历史

### v1.0.0 (2024-03-13)
- 初始版本发布
- 商品管理功能
- 购物车功能
- 订单处理
- 支付系统集成
- 用户评价系统
- 搜索和过滤功能

---

## 常见问题

### Q: 如何获取商品库存信息？
A: 通过 `product.stock` 属性获取，注意处理库存不足的情况。

### Q: 如何处理货币转换？
A: 使用 `formatPrice` 函数，它会根据当前配置的货币格式化价格。

### Q: 订单状态如何更新？
A: 使用 `updateOrderStatus` 方法，传入新的 OrderStatus 值。

### Q: 支付失败如何重试？
A: 检查错误类型，如果是临时错误（如网络问题），可以重试支付。

---

更多详细信息请参考[模块开发指南](../development/MODULE_DEVELOPMENT_GUIDE.md)和[最佳实践](../development/MODULE_BEST_PRACTICES.md)。