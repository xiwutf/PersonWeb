<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 via-red-50 to-pink-50 py-8">
    <div class="container mx-auto px-4 max-w-3xl">
      <!-- 页面标题 -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">提交订单</h1>
        <p class="text-gray-600">请填写以下信息完成下单</p>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="bg-var(--color-bg-light, white)/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
        </div>
      </div>

      <!-- 订单表单 -->
      <div v-else-if="product" class="bg-var(--color-bg-light, white)/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <!-- 商品信息（只读） -->
        <div class="mb-6 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <h3 class="text-sm font-medium text-gray-700 mb-2">商品信息</h3>
          <p class="text-lg font-semibold text-gray-900">{{ product.name }}</p>
          <p v-if="product.price > 0" class="text-xl font-bold text-green-600 mt-2">
            ¥{{ product.price }}
            <span v-if="product.originalPrice && product.originalPrice > product.price" class="text-sm text-gray-500 line-through ml-2">
              ¥{{ product.originalPrice }}
            </span>
          </p>
          <p v-else class="text-gray-600 mt-2">价格面议</p>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- 客户姓名 -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              你的称呼 <span class="text-red-500">*</span>
            </label>
            <input
              v-model="form.customerName"
              type="text"
              placeholder="请输入您的姓名"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              :class="{ 'border-red-500': errors.customerName }"
              required
            />
            <p v-if="errors.customerName" class="text-red-500 text-sm mt-1">{{ errors.customerName }}</p>
          </div>

          <!-- 联系方式 -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              联系方式 <span class="text-red-500">*</span>
            </label>
            <p class="text-sm text-gray-500 mb-3">至少填写一种联系方式</p>
            
            <div class="space-y-3">
              <input
                v-model="form.customerPhone"
                type="tel"
                placeholder="手机号"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
              <input
                v-model="form.customerWeChat"
                type="text"
                placeholder="微信号"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
              <input
                v-model="form.customerEmail"
                type="email"
                placeholder="邮箱"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
            </div>
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- 数量 -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">数量</label>
            <input
              v-model.number="form.quantity"
              type="number"
              min="1"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
          </div>

          <!-- 需求说明 -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              需求补充说明 <span class="text-red-500">*</span>
            </label>
            <textarea
              v-model="form.remark"
              rows="5"
              placeholder="请详细描述您的需求..."
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500 resize-y"
              :class="{ 'border-red-500': errors.remark }"
              required
            ></textarea>
            <p v-if="errors.remark" class="text-red-500 text-sm mt-1">{{ errors.remark }}</p>
            <p class="text-sm text-gray-500 mt-1">已输入 {{ form.remark.length }} 个字符</p>
          </div>

          <!-- 提交按钮 -->
          <div class="flex gap-4 pt-4">
            <button
              type="button"
              @click="$router.back()"
              class="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
            >
              取消
            </button>
            <button
              type="submit"
              :disabled="submitting"
              class="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-red-500 text-var(--color-bg-light, white) rounded-lg hover:from-orange-600 hover:to-red-600 transition-all font-medium shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="submitting">提交中...</span>
              <span v-else>提交订单</span>
            </button>
          </div>
        </form>
      </div>

      <!-- 无数据状态 -->
      <div v-else class="bg-var(--color-bg-light, white)/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <p class="text-gray-600">商品不存在或已下架</p>
        <NuxtLink to="/tools" class="mt-4 inline-block text-orange-600 hover:text-orange-700">
          返回工具列表
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const router = useRouter()
const api = useApi()
const message = useSafeMessage()

const productId = computed(() => {
  const id = route.query.productId
  return id ? Number(id) : 0
})

const product = ref<any>(null)
const loading = ref(true)
const submitting = ref(false)

const form = ref({
  customerName: '',
  customerPhone: '',
  customerWeChat: '',
  customerEmail: '',
  quantity: 1,
  remark: ''
})

const errors = ref<Record<string, string>>({})

// 获取商品信息
const fetchProduct = async () => {
  if (!productId.value) {
    loading.value = false
    return
  }

  loading.value = true
  try {
    console.log('获取商品信息，productId:', productId.value)
    const res = await api.get<any>(`/Toolbox/${productId.value}`)
    console.log('商品信息响应:', res)
    
    // useApi 已经处理了响应格式，返回的是 data 部分
    // 如果响应是 { code: 0, data: {...} }，则 res 就是 data
    if (res && (res.id || res.name)) {
      product.value = res
      console.log('商品信息加载成功:', product.value)
    } else {
      console.error('商品信息格式错误:', res)
      message.error('获取商品信息失败')
    }
  } catch (e: any) {
    console.error('获取商品信息失败:', e)
    console.error('错误详情:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    message.error(e.response?.data?.message || e.message || '获取商品信息失败')
  } finally {
    loading.value = false
  }
}

// 表单验证
const validate = (): boolean => {
  errors.value = {}

  if (!form.value.customerName.trim()) {
    errors.value.customerName = '请输入您的姓名'
    return false
  }

  if (!form.value.customerPhone && !form.value.customerWeChat && !form.value.customerEmail) {
    errors.value.contact = '至少需要填写一种联系方式'
    return false
  }

  if (!form.value.remark.trim()) {
    errors.value.remark = '请输入需求说明'
    return false
  }

  // 移除字符数限制
  return true
}

// 提交订单
const handleSubmit = async () => {
  if (!validate()) {
    return
  }

  submitting.value = true
  try {
    console.log('提交订单，数据:', {
      productId: productId.value,
      customerName: form.value.customerName.trim(),
      quantity: form.value.quantity || 1
    })
    
    const res = await api.post<any>('/Orders', {
      productId: productId.value,
      customerName: form.value.customerName.trim(),
      customerPhone: form.value.customerPhone.trim() || undefined,
      customerWeChat: form.value.customerWeChat.trim() || undefined,
      customerEmail: form.value.customerEmail.trim() || undefined,
      quantity: form.value.quantity || 1,
      remark: form.value.remark.trim()
    })

    console.log('提交订单响应:', res)

    // useApi 已经处理了响应格式，如果成功，res 直接是 data 部分
    // 后端返回格式：{ code: 0, data: { OrderId: xxx, OrderNo: 'xxx' } }
    // useApi 处理后，res 就是 { OrderId: xxx, OrderNo: 'xxx' }
    if (res && res.orderNo) {
      // 显示成功提示
      message.success('订单提交成功！')
      // 跳转到成功页
      setTimeout(() => {
        router.push(`/order/success?orderNo=${res.orderNo}`)
      }, 500)
    } else if (res && res.OrderNo) {
      // 兼容后端返回的 PascalCase 格式
      message.success('订单提交成功！')
      setTimeout(() => {
        router.push(`/order/success?orderNo=${res.OrderNo}`)
      }, 500)
    } else {
      console.error('订单响应格式错误:', res)
      message.error('提交订单失败，响应格式错误')
    }
  } catch (e: any) {
    console.error('提交订单失败:', e)
    console.error('错误详情:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    message.error(e.response?.data?.message || e.message || '提交订单失败，请稍后重试')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  fetchProduct()
})

// 设置页面标题
useHead({
  title: '提交订单 - 溪午听风',
  meta: [
    { name: 'description', content: '提交订单页面' }
  ]
})
</script>

<style scoped>
/* 样式已通过 Tailwind CSS 类实现 */
</style>

