/**
 * 购物车相关的组合式函数
 */
import { ref, computed, reactive, watch } from 'vue'
import type { CartItem, Product } from '~/types/ecommerce'

export interface CartStore {
  items: CartItem[]
  totalItems: number
  subtotal: number
  tax: number
  total: number
  currency: string
}

/**
 * 使用 Pinia 管理购物车状态
 */
export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])
  const currency = ref('CNY')

  // 计算属性
  const totalItems = computed(() => {
    return items.value.reduce((sum, item) => sum + item.quantity, 0)
  })

  const subtotal = computed(() => {
    return items.value.reduce((sum, item) => sum + item.totalPrice, 0)
  })

  const tax = computed(() => {
    // 假设税率是 13%
    return subtotal.value * 0.13
  })

  const total = computed(() => {
    return subtotal.value + tax.value
  })

  // 方法
  const addItem = (product: Product, quantity: number = 1) => {
    const existingItem = items.value.find(item => item.productId === product.id)

    if (existingItem) {
      existingItem.quantity += quantity
      existingItem.totalPrice = existingItem.price * existingItem.quantity
      existingItem.updatedAt = new Date().toISOString()
    } else {
      const newItem: CartItem = {
        id: `cart-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
        productId: product.id,
        product,
        quantity,
        price: product.price,
        totalPrice: product.price * quantity,
        selected: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      items.value.push(newItem)
    }

    // 触发事件
    emitCartEvent('product_added_to_cart', { productId: product.id, quantity })
  }

  const updateItemQuantity = (productId: string, quantity: number) => {
    const item = items.value.find(item => item.productId === productId)

    if (item) {
      if (quantity <= 0) {
        removeItem(productId)
        return
      }

      item.quantity = quantity
      item.totalPrice = item.price * quantity
      item.updatedAt = new Date().toISOString()
    }
  }

  const removeItem = (productId: string) => {
    const index = items.value.findIndex(item => item.productId === productId)
    if (index > -1) {
      items.value.splice(index, 1)
      emitCartEvent('product_removed_from_cart', { productId })
    }
  }

  const clearCart = () => {
    items.value = []
    emitCartEvent('cart_cleared')
  }

  const toggleItemSelection = (productId: string) => {
    const item = items.value.find(item => item.productId === productId)
    if (item) {
      item.selected = !item.selected
    }
  }

  const selectAll = (selected: boolean) => {
    items.value.forEach(item => {
      item.selected = selected
    })
  }

  const getSelectedItems = () => {
    return items.value.filter(item => item.selected)
  }

  const getSelectedTotal = () => {
    return getSelectedItems().reduce((sum, item) => sum + item.totalPrice, 0)
  }

  // 持久化到 localStorage
  const saveToLocalStorage = () => {
    const cartData = {
      items: items.value,
      currency: currency.value,
      timestamp: Date.now()
    }
    localStorage.setItem('cart', JSON.stringify(cartData))
  }

  const loadFromLocalStorage = () => {
    try {
      const saved = localStorage.getItem('cart')
      if (saved) {
        const cartData = JSON.parse(saved)
        items.value = cartData.items || []
        currency.value = cartData.currency || 'CNY'

        // 清理超过30天的旧数据
        if (cartData.timestamp && Date.now() - cartData.timestamp > 30 * 24 * 60 * 60 * 1000) {
          localStorage.removeItem('cart')
        }
      }
    } catch (err) {
      console.error('Failed to load cart from localStorage:', err)
    }
  }

  // 监听变化并保存
  watch([items, currency], () => {
    saveToLocalStorage()
  }, { deep: true })

  return {
    items,
    totalItems,
    subtotal,
    tax,
    total,
    currency,
    addItem,
    updateItemQuantity,
    removeItem,
    clearCart,
    toggleItemSelection,
    selectAll,
    getSelectedItems,
    getSelectedTotal,
    saveToLocalStorage,
    loadFromLocalStorage
  }
})

/**
 * 购物车事件
 */
export const useCartEvents = () => {
  const emit = defineEmits<{
    'cart:product-added': [productId: string, quantity: number]
    'cart:product-removed': [productId: string]
    'cart:cleared': []
    'cart:updated': []
  }>()

  const emitCartEvent = (type: string, data?: any) => {
    switch (type) {
      case 'product_added_to_cart':
        emit('cart:product-added', data.productId, data.quantity)
        break
      case 'product_removed_from_cart':
        emit('cart:product-removed', data.productId)
        break
      case 'cart_cleared':
        emit('cart:cleared')
        break
    }
    emit('cart:updated')
  }

  return {
    emitCartEvent
  }
}

/**
 * 组合式函数：使用购物车
 */
export function useCart() {
  const cartStore = useCartStore()
  const { emitCartEvent } = useCartEvents()

  // 初始化时从 localStorage 加载
  onMounted(() => {
    cartStore.loadFromLocalStorage()
  })

  // 导出购物车操作
  const addToCart = (product: Product, quantity: number = 1) => {
    cartStore.addItem(product, quantity)
  }

  const updateQuantity = (productId: string, quantity: number) => {
    cartStore.updateItemQuantity(productId, quantity)
  }

  const removeFromCart = (productId: string) => {
    cartStore.removeItem(productId)
  }

  const clearCart = () => {
    cartStore.clearCart()
  }

  const toggleSelection = (productId: string) => {
    cartStore.toggleItemSelection(productId)
  }

  const selectAll = (selected: boolean) => {
    cartStore.selectAll(selected)
  }

  // 计算选中项的总数
  const selectedTotal = computed(() => cartStore.getSelectedTotal())

  // 检查商品是否在购物车中
  const isInCart = (productId: string) => {
    return cartStore.items.some(item => item.productId === productId)
  }

  // 获取商品数量
  const getItemQuantity = (productId: string) => {
    const item = cartStore.items.find(item => item.productId === productId)
    return item?.quantity || 0
  }

  // 格式化价格
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('zh-CN', {
      style: 'currency',
      currency: cartStore.currency
    }).format(price)
  }

  return {
    cart: cartStore,
    addToCart,
    updateQuantity,
    removeFromCart,
    clearCart,
    toggleSelection,
    selectAll,
    selectedTotal,
    isInCart,
    getItemQuantity,
    formatPrice
  }
}

// 使用示例：
/*
const { cart, addToCart, formatPrice } = useCart()

// 添加商品到购物车
addToCart(product, 2)

// 在模板中显示
<div v-for="item in cart.items" :key="item.id">
  {{ item.product.name }} - {{ item.quantity }} - {{ formatPrice(item.totalPrice) }}
</div>
*/