<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 via-emerald-50 to-teal-50 py-8">
    <div class="container mx-auto px-4 max-w-2xl">
      <!-- æåæç¤º -->
      <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center mb-6">
        <div class="mb-6">
          <div class="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <i class="fas fa-check-circle text-5xl text-green-600"></i>
          </div>
          <h1 class="text-3xl font-bold text-gray-900 mb-2">è®¢åæäº¤æåï¼?/h1>
          <p class="text-gray-600">æ¨çè®¢åå·²æåæäº¤ï¼æä»¬ä¼å°½å¿«ä¸æ¨èç³?/p>
        </div>

        <!-- è®¢åä¿¡æ¯ -->
        <div v-if="orderInfo" class="bg-gray-50 rounded-lg p-6 mb-6 text-left">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">è®¢åä¿¡æ¯</h3>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-gray-600">è®¢åç¼å·ï¼?/span>
              <span class="font-mono font-semibold text-gray-900">{{ orderInfo.orderNo }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">åååç§°ï¼?/span>
              <span class="text-gray-900">{{ orderInfo.productName }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">è®¢åç¶æï¼</span>
              <span class="px-3 py-1 bg-yellow-100 text-yellow-800 rounded-full text-sm font-medium">
                å¾ç¡®è®?              </span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">æäº¤æ¶é´ï¼?/span>
              <span class="text-gray-900">{{ formatDate(orderInfo.createdAt) }}</span>
            </div>
          </div>
        </div>

        <!-- æç¤ºä¿¡æ¯ -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6 text-left">
          <h4 class="font-semibold text-blue-900 mb-2 flex items-center">
            <i class="fas fa-info-circle mr-2"></i>
            æ¸©é¦¨æç¤º
          </h4>
          <ul class="text-sm text-blue-800 space-y-1 list-disc list-inside">
            <li>è¯·æªå¾ä¿å­æ¬é¡µé¢ï¼ä»¥ä¾¿åç»­æ¥è¯¢è®¢åç¶æ?/li>
            <li>æä»¬ä¼å°½å¿«ä¸æ¨èç³»ï¼ç¡®è®¤è®¢åè¯¦æ</li>
            <li>æ¨ä¹å¯ä»¥ä¸»å¨æ·»å æçå¾®ä¿¡ï¼å å¿«æ²éæç?/li>
          </ul>
        </div>

        <!-- å¾®ä¿¡äºç»´ç ?-->
        <div v-if="wechatInfo" class="bg-white rounded-lg p-6 mb-6 border border-gray-200">
          <p class="text-sm text-gray-600 mb-3">æ¨ä¹å¯ä»¥ç´æ¥æ·»å æçå¾®ä¿¡ï¼?/p>
          <p class="text-lg font-semibold text-gray-800 mb-4">{{ wechatInfo.wechat }}</p>
          <div v-if="wechatInfo.qrcode" class="flex justify-center mb-3">
            <img :src="wechatInfo.qrcode" alt="å¾®ä¿¡äºç»´ç ? class="w-48 h-48 border border-gray-300 rounded-lg" />
          </div>
          <p v-if="wechatInfo.qrcode" class="text-xs text-gray-500 text-center">æ«æäºç»´ç ææç´¢å¾®ä¿¡å·æ·»å?/p>
          <p v-else class="text-xs text-gray-500 text-center">æç´¢å¾®ä¿¡å·æ·»å?/p>
        </div>

        <!-- æä½æé® -->
        <div class="flex flex-col sm:flex-row gap-4">
          <button
            @click="$router.push('/')"
            class="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            <i class="fas fa-home mr-2"></i>
            è¿åé¦é¡µ
          </button>
          <button
            @click="$router.push('/order/query')"
            class="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-red-500 text-var(--color-bg-light, white) rounded-lg hover:from-orange-600 hover:to-red-600 transition-all font-medium shadow-lg"
          >
            <i class="fas fa-search mr-2"></i>
            æ¥çè®¢åç¶æ?          </button>
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

// è·åè®¢åä¿¡æ¯
const fetchOrderInfo = async () => {
  if (!orderNo.value) {
    return
  }

  try {
    // è¿ééè¦ç¨æ·è¾å¥èç³»æ¹å¼æè½æ¥è¯¢ï¼æä»¥åä¸èªå¨æ¥è¯?    // åªæ¾ç¤ºè®¢åå·
    orderInfo.value = {
      orderNo: orderNo.value,
      productName: 'ååä¿¡æ¯',
      status: 0,
      createdAt: new Date().toISOString()
    }
  } catch (e: any) {
    console.error('è·åè®¢åä¿¡æ¯å¤±è´¥:', e)
  }
}

// è·åå¾®ä¿¡ä¿¡æ¯
const fetchWechatInfo = async () => {
  try {
    // å°è¯ä»éç½®æ¥å£è·åèç³»æ¹å¼ä¿¡æ?    const configRes = await api.get<Record<string, string>>('/Config')
    if (configRes && typeof configRes === 'object') {
      // ä»éç½®ä¸­è·åèç³»æ¹å¼ï¼æ¯æå¤ç§éç½®é®å?      const wechat = configRes.wechat || configRes.wechat_id || configRes.contact_wechat || configRes.å¾®ä¿¡ || ''
      const qrcode = configRes.wechatQrcode || configRes.wechat_qrcode || configRes.wechat_qr || configRes.å¾®ä¿¡äºç»´ç ?|| ''
      
      if (wechat) {
        wechatInfo.value = {
          wechat: wechat,
          qrcode: qrcode || undefined
        }
      } else {
        // å¦æéç½®ä¸­æ²¡æå¾®ä¿¡ï¼ä½¿ç¨é»è®¤å?        wechatInfo.value = {
          wechat: 'LinXi-5152',
          qrcode: '/images/wechat-qr.png'
        }
      }
    } else {
      // å¦æéç½®æ¥å£è¿åæ ¼å¼ä¸å¯¹ï¼ä½¿ç¨é»è®¤å?      wechatInfo.value = {
        wechat: 'LinXi-5152',
        qrcode: '/images/wechat-qr.png'
      }
    }
  } catch (e) {
    console.warn('è·åå¾®ä¿¡ä¿¡æ¯å¤±è´¥ï¼ä½¿ç¨é»è®¤å?, e)
    // å³ä½¿è·åå¤±è´¥ï¼ä¹æ¾ç¤ºé»è®¤çå¾®ä¿¡èç³»æ¹å¼?    wechatInfo.value = {
      wechat: 'LinXi-5152',
      qrcode: '/images/wechat-qr.png'
    }
  }
}

// æ ¼å¼åæ¥æ?const formatDate = (dateString: string) => {
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

// è®¾ç½®é¡µé¢æ é¢
useHead({
  title: 'è®¢åæäº¤æå - æºªåå¬é£',
  meta: [
    { name: 'description', content: 'è®¢åæäº¤æåé¡µé¢' }
  ]
})
</script>

<style scoped>
/* æ ·å¼å·²éè¿ Tailwind CSS ç±»å®ç?*/
</style>

