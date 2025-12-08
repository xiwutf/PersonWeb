<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 via-emerald-50 to-teal-50 py-8">
    <div class="container mx-auto px-4 max-w-2xl">
      <!-- 成功提示 -->
      <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center mb-6">
        <div class="mb-6">
          <div class="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <i class="fas fa-check-circle text-5xl text-green-600"></i>
          </div>
          <h1 class="text-3xl font-bold text-gray-900 mb-2">订单提交成功！</h1>
          <p class="text-gray-600">您的订单已成功提交，我们会尽快与您联系</p>
        </div>

        <!-- 订单信息 -->
        <div v-if="orderInfo" class="bg-gray-50 rounded-lg p-6 mb-6 text-left">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">订单信息</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600">订单编号：</span>
              <span class="font-mono font-semibold text-gray-900">{{ orderInfo.orderNo }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">商品名称：</span>
              <span class="text-gray-900">{{ orderInfo.productName }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">订单状态：</span>
              <span class="px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                待确认
              </span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">提交时间：</span>
              <span class="text-gray-900">{{ formatDate(orderInfo.createdAt) }}</span>
            </div>
          </div>
        </div>

        <!-- 提示信息 -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6 text-left">
          <h4 class="font-semibold text-blue-900 mb-2 flex items-center">
            <i class="fas fa-info-circle mr-2"></i>
            温馨提示
          </h4>
          <ul class="text-sm text-blue-800 space-y-1 list-disc list-inside">
            <li>请截图保存本页面，以便后续查询订单状态</li>
            <li>我们会尽快与您联系，确认订单详情</li>
            <li>您也可以主动添加我的微信，加快沟通效率</li>
          </ul>
        </div>

        <!-- 微信二维码 -->
        <div v-if="wechatInfo" class="bg-white rounded-lg p-6 mb-6 border border-gray-200">
          <p class="text-sm text-gray-600 mb-3">您也可以直接添加我的微信：</p>
          <p class="text-lg font-semibold text-gray-800 mb-4">{{ wechatInfo.wechat }}</p>
          <div v-if="wechatInfo.qrcode" class="flex justify-center mb-3">
            <img :src="wechatInfo.qrcode" alt="微信二维码" class="w-48 h-48 border border-gray-300 rounded-lg" />
          </div>
          <p class="text-xs text-gray-500">扫描二维码或搜索微信号添加</p>
        </div>

        <!-- 操作按钮 -->
        <div class="flex flex-col sm:flex-row gap-4">
          <button
            @click="$router.push('/')"
            class="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            <i class="fas fa-home mr-2"></i>
            返回首页
          </button>
          <button
            @click="$router.push('/order/query')"
            class="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-red-500 text-white rounded-lg hover:from-orange-600 hover:to-red-600 transition-all font-medium shadow-lg"
          >
            <i class="fas fa-search mr-2"></i>
            查看订单状态
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()
const router = useRouter()
const api = useApi()
const message = useSafeMessage()

const orderNo = computed(() => route.query.orderNo as string)
const orderInfo = ref<any>(null)
const wechatInfo = ref<{ wechat?: string; qrcode?: string } | null>(null)

// 获取订单信息
const fetchOrderInfo = async () => {
  if (!orderNo.value) {
    return
  }

  try {
    // 这里需要用户输入联系方式才能查询，所以先不自动查询
    // 只显示订单号
    orderInfo.value = {
      orderNo: orderNo.value,
      productName: '商品信息',
      status: 0,
      createdAt: new Date().toISOString()
    }
  } catch (e: any) {
    console.error('获取订单信息失败:', e)
  }
}

// 获取微信信息
const fetchWechatInfo = async () => {
  try {
    const configRes = await api.get<any>('/Config')
    if (configRes && configRes.wechat) {
      wechatInfo.value = {
        wechat: configRes.wechat,
        qrcode: configRes.wechatQrcode
      }
    } else {
      wechatInfo.value = {
        wechat: '请查看网站联系方式',
        qrcode: undefined
      }
    }
  } catch (e) {
    console.warn('获取微信信息失败', e)
    wechatInfo.value = {
      wechat: '请查看网站联系方式',
      qrcode: undefined
    }
  }
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

onMounted(() => {
  fetchOrderInfo()
  fetchWechatInfo()
})

// 设置页面标题
useHead({
  title: '订单提交成功 - 溪午听风',
  meta: [
    { name: 'description', content: '订单提交成功页面' }
  ]
})
</script>

<style scoped>
/* 样式已通过 Tailwind CSS 类实现 */
</style>

