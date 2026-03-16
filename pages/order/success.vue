<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 via-emerald-50 to-teal-50 py-8">
    <div class="container mx-auto px-4 max-w-2xl">
      <!-- ???? -->
      <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center mb-6">
        <div class="mb-6">
          <div class="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <i class="fas fa-check-circle text-5xl text-green-600"></i>
          </div>
          <h1 class="text-3xl font-bold text-gray-900 mb-2">??????</h1>
          <p class="text-gray-600">???????????????????</p>
        </div>

        <!-- ???? -->
        <div v-if="orderInfo" class="bg-gray-50 rounded-lg p-6 mb-6 text-left">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">????</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600">?????</span>
              <span class="font-mono font-semibold text-gray-900">{{ orderInfo.orderNo }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">?????</span>
              <span class="text-gray-900">{{ orderInfo.productName }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">?????</span>
              <span class="px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                ????              </span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">?????</span>
              <span class="text-gray-900">{{ formatDate(orderInfo.createdAt) }}</span>
            </div>
          </div>
        </div>

        <!-- ???? -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6 text-left">
          <h4 class="font-semibold text-blue-900 mb-2 flex items-center">
            <i class="fas fa-info-circle mr-2"></i>
            ????
          </h4>
          <ul class="text-sm text-blue-800 space-y-1 list-disc list-inside">
            <li>???????????????????</li>
            <li>????????????????</li>
            <li>???????????????????</li>
          </ul>
        </div>

        <!-- ??????-->
        <div v-if="wechatInfo" class="bg-white rounded-lg p-6 mb-6 border border-gray-200">
          <p class="text-sm text-gray-600 mb-3">?????????????</p>
          <p class="text-lg font-semibold text-gray-800 mb-4">{{ wechatInfo.wechat }}</p>
          <div v-if="wechatInfo.qrcode" class="flex justify-center mb-3">
            <img :src="wechatInfo.qrcode" alt="?????" class="w-48 h-48 border border-gray-300 rounded-lg" />
          </div>
          <p v-if="wechatInfo.qrcode" class="text-xs text-gray-500 text-center">?????????????</p>
          <p v-else class="text-xs text-gray-500 text-center">???????</p>
        </div>

        <!-- ???? -->
        <div class="flex flex-col sm:flex-row gap-4">
          <button
            @click="$router.push('/')"
            class="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            <i class="fas fa-home mr-2"></i>
            ????
          </button>
          <button
            @click="$router.push('/order/query')"
            class="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-red-500 text-var(--color-bg-light, white) rounded-lg hover:from-orange-600 hover:to-red-600 transition-all font-medium shadow-lg"
          >
            <i class="fas fa-search mr-2"></i>
            ???????          </button>
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

// ??????
const fetchOrderInfo = async () => {
  if (!orderNo.value) {
    return
  }

  try {
    // ??????????????????????????    // ??????
    orderInfo.value = {
      orderNo: orderNo.value,
      productName: '????',
      status: 0,
      createdAt: new Date().toISOString()
    }
  } catch (e: any) {
    console.error('????????:', e)
  }
}

// ??????
const fetchWechatInfo = async () => {
  try {
    
    const configRes = await api.get<Record<string, string>>('/Config')
    if (configRes && typeof configRes === 'object') {
      
      const wechat = configRes.wechat || configRes.wechat_id || configRes.contact_wechat || configRes.contact || ''
      const qrcode = configRes.wechatQrcode || configRes.wechat_qrcode || configRes.wechat_qr || configRes.contact_qr || ''
      
      if (wechat) {
        wechatInfo.value = {
          wechat: wechat,
          qrcode: qrcode || undefined
        }
      } else {
      wechatInfo.value = {
        wechat: 'LinXi-5152',
          qrcode: '/images/wechat-qr.png'
        }
      }
    } else {
      wechatInfo.value = {
        wechat: 'LinXi-5152',
        qrcode: '/images/wechat-qr.png'
      }
    }
  } catch (e) {
    console.warn('获取微信配置失败', e)
    wechatInfo.value = {
      wechat: 'LinXi-5152',
      qrcode: '/images/wechat-qr.png'
    }
  }
}

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

// ??????
useHead({
  title: '?????? - ????',
  meta: [
    { name: 'description', content: '????????' }
  ]
})
</script>

<style scoped>
/* ????? Tailwind CSS ????*/
</style>

