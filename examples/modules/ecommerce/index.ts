import { defineModule } from '@personweb/module-system'
import moduleConfig from './module.json'
import { useCart } from './composables/useCart'
import { useProducts } from './composables/useShop'

/**
 * E-Commerce 示例模块
 *
 * 这是一个功能完整的电商模块示例，包含：
 * - 商品展示和管理
 * - 购物车功能
 * - 订单处理
 * - 用户配置管理
 * - 支付集成
 */
export default defineModule({
  // 模块定义
  module: moduleConfig,

  // 依赖项
  dependencies: [
    'auth-module',
    'payment-gateway'
  ],

  // 导出的组合式函数和组件
  exports: {
    // 组合式函数
    useCart,
    useProducts,

    // 工具函数
    formatCurrency: formatCurrency,
    calculateTax: calculateTax,
    generateOrderNumber: generateOrderNumber
  },

  // 全局状态
  setup() {
    // 共享状态
    const sharedState = reactive({
      currency: 'CNY',
      taxRate: 0.13,
      isLoading: false
    })

    return {
      sharedState
    }
  },

  // 生命周期钩子
  async init() {
    console.log('🛒 E-Commerce 模块初始化中...')

    // 初始化购物车
    const { cart } = useCart()
    cart.loadFromLocalStorage()

    // 预加载热门商品
    const { fetchProducts } = useProducts()
    await fetchProducts({ sortBy: 'popular', sortOrder: 'desc' })
  },

  async onInstall() {
    console.log('🛒 E-Commerce 模块已安装')

    // 初始化数据库表
    await initializeDatabase()
  },

  async onActivate() {
    console.log('🛒 E-Commerce 模块已激活')

    // 注册全局事件监听
    setupEventListeners()
  },

  async onDeactivate() {
    console.log('🛒 E-Commerce 模块已停用')

    // 清理事件监听
    cleanupEventListeners()
  },

  async onUninstall() {
    console.log('🛒 E-Commerce 模块已卸载')

    // 清理数据
    await cleanupModuleData()
  }
})

/**
 * 初始化数据库
 */
async function initializeDatabase() {
  // 创建商品表
  // 创建订单表
  // 创建用户配置表
  console.log('数据库初始化完成')
}

/**
 * 设置事件监听器
 */
function setupEventListeners() {
  // 监听商品添加事件
  document.addEventListener('product-added', handleProductAdded)

  // 监听用户登录事件
  document.addEventListener('user-logged-in', handleUserLoggedIn)
}

/**
 * 清理事件监听器
 */
function cleanupEventListeners() {
  document.removeEventListener('product-added', handleProductAdded)
  document.removeEventListener('user-logged-in', handleUserLoggedIn)
}

/**
 * 清理模块数据
 */
async function cleanupModuleData() {
  // 清理本地存储
  localStorage.removeItem('cart')

  // 清理缓存
  // 清理临时文件
}

/**
 * 格式化货币
 */
function formatCurrency(amount: number, currency: string = 'CNY'): string {
  return new Intl.NumberFormat('zh-CN', {
    style: 'currency',
    currency: currency
  }).format(amount)
}

/**
 * 计算税费
 */
function calculateTax(amount: number, taxRate: number = 0.13): number {
  return Math.round(amount * taxRate * 100) / 100
}

/**
 * 生成订单号
 */
function generateOrderNumber(): string {
  const timestamp = Date.now().toString()
  const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0')
  return `ORD${timestamp}${random}`
}

/**
 * 处理商品添加事件
 */
function handleProductAdded(event: CustomEvent) {
  console.log('商品已添加到购物车:', event.detail)
  // 可以在这里添加通知或动画效果
}

/**
 * 处理用户登录事件
 */
function handleUserLoggedIn(event: CustomEvent) {
  console.log('用户已登录，加载用户购物车...')
  // 从服务器加载用户的购物车
}