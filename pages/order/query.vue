<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 py-8">
    <div class="container mx-auto px-4 max-w-2xl">
      <!-- é¡µé¢æ é¢ -->
      <div class="mb-8 text-center">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">è®¢åæ¥è¯¢</h1>
        <p class="text-gray-600">è¯·è¾å¥è®¢åç¼å·åèç³»æ¹å¼æ¥è¯¢è®¢åç¶æ?/p>
      </div>

      <!-- æ¥è¯¢è¡¨å -->
      <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 mb-6">
        <form @submit.prevent="handleQuery" class="space-y-6">
          <!-- è®¢åç¼å· -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              è®¢åç¼å· <span class="text-red-500">*</span>
            </label>
            <input
              v-model="queryForm.orderNo"
              type="text"
              placeholder="è¯·è¾å¥è®¢åç¼å·ï¼å¦ï¼20251208-1234ï¼?
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              :class="{ 'border-red-500': errors.orderNo }"
              required
            />
            <p v-if="errors.orderNo" class="text-red-500 text-sm mt-1">{{ errors.orderNo }}</p>
          </div>

          <!-- èç³»æ¹å¼ -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              èç³»æ¹å¼ <span class="text-red-500">*</span>
            </label>
            <p class="text-sm text-gray-500 mb-3">è¯·è¾å¥ä¸åæ¶å¡«åçææºå·ãé®ç®±æå¾®ä¿¡å?/p>
            <input
              v-model="queryForm.contact"
              type="text"
              placeholder="è¯·è¾å¥èç³»æ¹å¼?
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              :class="{ 'border-red-500': errors.contact }"
              required
            />
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- æ¥è¯¢æé® -->
          <button
            type="submit"
            :disabled="querying"
            class="w-full px-6 py-3 bg-gradient-to-r from-blue-500 to-indigo-500 text-var(--color-bg-light, white) rounded-lg hover:from-blue-600 hover:to-indigo-600 transition-all font-medium shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="querying">æ¥è¯¢ä¸?..</span>
            <span v-else>
              <i class="fas fa-search mr-2"></i>
              æ¥è¯¢è®¢å
            </span>
          </button>
        </form>
      </div>

      <!-- æ¥è¯¢ç»æ -->
      <div v-if="orderResult" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <h3 class="text-xl font-bold text-gray-900 mb-6">è®¢åè¯¦æ</h3>
        
        <div class="space-y-4">
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">è®¢åç¼å·ï¼?/span>
            <span class="font-mono font-semibold text-gray-900">{{ orderResult.orderNo }}</span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">åååç§°ï¼?/span>
            <span class="text-gray-900 font-medium">{{ orderResult.productName }}</span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">è®¢åç¶æï¼</span>
            <span :class="getStatusClass(orderResult.status)" class="px-3 py-1 rounded-full text-sm font-medium">
              {{ getStatusText(orderResult.status) }}
            </span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">åå»ºæ¶é´ï¼?/span>
            <span class="text-gray-900">{{ formatDate(orderResult.createdAt) }}</span>
          </div>
          
          <div v-if="orderResult.remark" class="pt-4">
            <span class="text-gray-600 block mb-2">å¤æ³¨/éæ±è¯´æï¼</span>
            <p class="text-gray-900 bg-gray-50 rounded-lg p-4 var(--color-bg-light, white)space-pre-line">{{ orderResult.remark }}</p>
          </div>
        </div>

        <!-- æä½æé® -->
        <div class="mt-6 pt-6 border-t border-gray-200">
          <button
            @click="handleReset"
            class="w-full px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            æ¥è¯¢å¶ä»è®¢å
          </button>
        </div>
      </div>

      <!-- éè¯¯æç¤º -->
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

// è¡¨åéªè¯
const validate = (): boolean => {
  errors.value = {}

  if (!queryForm.value.orderNo.trim()) {
    errors.value.orderNo = 'è¯·è¾å¥è®¢åç¼å?
    return false
  }

  if (!queryForm.value.contact.trim()) {
    errors.value.contact = 'è¯·è¾å¥èç³»æ¹å¼?
    return false
  }

  return true
}

// æ¥è¯¢è®¢å
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

    console.log('è®¢åæ¥è¯¢ååº:', res)

    // useApi å·²ç»å¤çäºååºæ ¼å¼ï¼å¦ææåï¼res ç´æ¥æ?data é¨å
    // åç«¯è¿åæ ¼å¼ï¼{ code: 0, data: {...} }
    // useApi å¤çåï¼res å°±æ¯ {...}
    if (res && (res.orderNo || res.OrderNo)) {
      orderResult.value = res
      queryError.value = null
    } else if (res && res.code === 0 && res.data) {
      // å¼å®¹æªå¤ççååºæ ¼å¼
      orderResult.value = res.data
      queryError.value = null
    } else {
      queryError.value = res?.message || 'è®¢åä¸å­å¨æèç³»æ¹å¼ä¸å¹é?
    }
  } catch (e: any) {
    console.error('æ¥è¯¢è®¢åå¤±è´¥:', e)
    console.error('éè¯¯è¯¦æ:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    queryError.value = e.response?.data?.message || e.message || 'æ¥è¯¢å¤±è´¥ï¼è¯·ç¨åéè¯'
  } finally {
    querying.value = false
  }
}

// éç½®æ¥è¯¢
const handleReset = () => {
  queryForm.value = {
    orderNo: '',
    contact: ''
  }
  orderResult.value = null
  queryError.value = null
  errors.value = {}
}

// è·åç¶æææ?const getStatusText = (status: number): string => {
  const statusMap: Record<number, string> = {
    0: 'å¾ç¡®è®?,
    1: 'è¿è¡ä¸?,
    2: 'å·²å®æ?,
    3: 'å·²å³é?
  }
  return statusMap[status] || 'æªç¥ç¶æ?
}

// è·åç¶ææ ·å¼ç±»
const getStatusClass = (status: number): string => {
  const classMap: Record<number, string> = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-blue-100 text-blue-800',
    2: 'bg-green-100 text-green-800',
    3: 'bg-gray-100 text-gray-800'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800'
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

// è®¾ç½®é¡µé¢æ é¢
useHead({
  title: 'è®¢åæ¥è¯¢ - æºªåå¬é£',
  meta: [
    { name: 'description', content: 'è®¢åæ¥è¯¢é¡µé¢' }
  ]
})
</script>

<style scoped>
/* æ ·å¼å·²éè¿ Tailwind CSS ç±»å®ç?*/
</style>

