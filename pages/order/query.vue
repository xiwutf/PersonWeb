<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 py-8">
    <div class="container mx-auto px-4 max-w-2xl">
      <!-- 页面标题 -->
      <div class="mb-8 text-center">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">订单查询</h1>
        <p class="text-gray-600">请输入订单编号和联系方式查询订单状态</p>
      </div>

      <!-- 查询表单 -->
      <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 mb-6">
        <form @submit.prevent="handleQuery" class="space-y-6">
          <!-- 订单编号 -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              订单编号 <span class="text-red-500">*</span>
            </label>
            <input
              v-model="queryForm.orderNo"
              type="text"
              placeholder="请输入订单编号（如：20251208-1234）"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              :class="{ 'border-red-500': errors.orderNo }"
              required
            />
            <p v-if="errors.orderNo" class="text-red-500 text-sm mt-1">{{ errors.orderNo }}</p>
          </div>

          <!-- 联系方式 -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              联系方式 <span class="text-red-500">*</span>
            </label>
            <p class="text-sm text-gray-500 mb-3">请输入下单时填写的手机号、邮箱或微信号</p>
            <input
              v-model="queryForm.contact"
              type="text"
              placeholder="请输入联系方式"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              :class="{ 'border-red-500': errors.contact }"
              required
            />
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- 查询按钮 -->
          <button
            type="submit"
            :disabled="querying"
            class="w-full px-6 py-3 bg-gradient-to-r from-blue-500 to-indigo-500 text-white rounded-lg hover:from-blue-600 hover:to-indigo-600 transition-all font-medium shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="querying">查询中...</span>
            <span v-else>
              <i class="fas fa-search mr-2"></i>
              查询订单
            </span>
          </button>
        </form>
      </div>

      <!-- 查询结果 -->
      <div v-if="orderResult" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <h3 class="text-xl font-bold text-gray-900 mb-6">订单详情</h3>
        
        <div class="space-y-4">
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">订单编号：</span>
            <span class="font-mono font-semibold text-gray-900">{{ orderResult.orderNo }}</span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">商品名称：</span>
            <span class="text-gray-900 font-medium">{{ orderResult.productName }}</span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">订单状态：</span>
            <span :class="getStatusClass(orderResult.status)" class="px-3 py-1 rounded-full text-sm font-medium">
              {{ getStatusText(orderResult.status) }}
            </span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">创建时间：</span>
            <span class="text-gray-900">{{ formatDate(orderResult.createdAt) }}</span>
          </div>
          
          <div v-if="orderResult.remark" class="pt-4">
            <span class="text-gray-600 block mb-2">备注/需求说明：</span>
            <p class="text-gray-900 bg-gray-50 rounded-lg p-4 whitespace-pre-line">{{ orderResult.remark }}</p>
          </div>
        </div>

        <!-- 操作按钮 -->
        <div class="mt-6 pt-6 border-t border-gray-200">
          <button
            @click="handleReset"
            class="w-full px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            查询其他订单
          </button>
        </div>
      </div>

      <!-- 错误提示 -->
      <div v-if="queryError" class="bg-red-50 border border-red-200 rounded-lg p-4 text-center">
        <p class="text-red-800">{{ queryError }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()
const message = useSafeMessage()

const queryForm = ref({
  orderNo: '',
  contact: ''
})

const errors = ref<Record<string, string>>({})
const querying = ref(false)
const orderResult = ref<any>(null)
const queryError = ref<string | null>(null)

// 表单验证
const validate = (): boolean => {
  errors.value = {}

  if (!queryForm.value.orderNo.trim()) {
    errors.value.orderNo = '请输入订单编号'
    return false
  }

  if (!queryForm.value.contact.trim()) {
    errors.value.contact = '请输入联系方式'
    return false
  }

  return true
}

// 查询订单
const handleQuery = async () => {
  if (!validate()) {
    return
  }

  querying.value = true
  queryError.value = null
  orderResult.value = null

  try {
    const res = await api.get<any>('/Orders/lookup', {
      params: {
        orderNo: queryForm.value.orderNo.trim(),
        contact: queryForm.value.contact.trim()
      }
    })

    console.log('订单查询响应:', res)

    // useApi 已经处理了响应格式，如果成功，res 直接是 data 部分
    // 后端返回格式：{ code: 0, data: {...} }
    // useApi 处理后，res 就是 {...}
    if (res && (res.orderNo || res.OrderNo)) {
      orderResult.value = res
      queryError.value = null
    } else if (res && res.code === 0 && res.data) {
      // 兼容未处理的响应格式
      orderResult.value = res.data
      queryError.value = null
    } else {
      queryError.value = res?.message || '订单不存在或联系方式不匹配'
    }
  } catch (e: any) {
    console.error('查询订单失败:', e)
    console.error('错误详情:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    queryError.value = e.response?.data?.message || e.message || '查询失败，请稍后重试'
  } finally {
    querying.value = false
  }
}

// 重置查询
const handleReset = () => {
  queryForm.value = {
    orderNo: '',
    contact: ''
  }
  orderResult.value = null
  queryError.value = null
  errors.value = {}
}

// 获取状态文本
const getStatusText = (status: number): string => {
  const statusMap: Record<number, string> = {
    0: '待确认',
    1: '进行中',
    2: '已完成',
    3: '已关闭'
  }
  return statusMap[status] || '未知状态'
}

// 获取状态样式类
const getStatusClass = (status: number): string => {
  const classMap: Record<number, string> = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-blue-100 text-blue-800',
    2: 'bg-green-100 text-green-800',
    3: 'bg-gray-100 text-gray-800'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800'
}

// 格式化日期
const formatDate = (dateString: string) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 设置页面标题
useHead({
  title: '订单查询 - 溪午听风',
  meta: [
    { name: 'description', content: '订单查询页面' }
  ]
})
</script>

<style scoped>
/* 样式已通过 Tailwind CSS 类实现 */
</style>

