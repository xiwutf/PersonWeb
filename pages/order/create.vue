<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 via-red-50 to-pink-50 py-8">
    <div class="container mx-auto px-4 max-w-3xl">
      <!-- é¡µé¢æ é¢ -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">æäº¤è®¢å</h1>
        <p class="text-gray-600">è¯·å¡«åä»¥ä¸ä¿¡æ¯å®æä¸å?/p>
      </div>

      <!-- å è½½ç¶æ?-->
      <div v-if="loading" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
        </div>
      </div>

      <!-- è®¢åè¡¨å -->
      <div v-else-if="product" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <!-- ååä¿¡æ¯ï¼åªè¯»ï¼ -->
        <div class="mb-6 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <h3 class="text-sm font-medium text-gray-700 mb-2">ååä¿¡æ¯</h3>
          <p class="text-lg font-semibold text-gray-900">{{ product.name }}</p>
          <p v-if="product.price > 0" class="text-xl font-bold text-green-600 mt-2">
            Â¥{{ product.price }}
            <span v-if="product.originalPrice && product.originalPrice > product.price" class="text-sm text-gray-500 line-through ml-2">
              Â¥{{ product.originalPrice }}
            </span>
          </p>
          <p v-else class="text-gray-600 mt-2">ä»·æ ¼é¢è®®</p>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- å®¢æ·å§å -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              ä½ çç§°å¼ <span class="text-red-500">*</span>
            </label>
            <input
              v-model="form.customerName"
              type="text"
              placeholder="è¯·è¾å¥æ¨çå§å?
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              :class="{ 'border-red-500': errors.customerName }"
              required
            />
            <p v-if="errors.customerName" class="text-red-500 text-sm mt-1">{{ errors.customerName }}</p>
          </div>

          <!-- èç³»æ¹å¼ -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              èç³»æ¹å¼ <span class="text-red-500">*</span>
            </label>
            <p class="text-sm text-gray-500 mb-3">è³å°å¡«åä¸ç§èç³»æ¹å¼?/p>
            
            <div class="space-y-3">
              <input
                v-model="form.customerPhone"
                type="tel"
                placeholder="ææºå?
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
              <input
                v-model="form.customerWeChat"
                type="text"
                placeholder="å¾®ä¿¡å?
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
              <input
                v-model="form.customerEmail"
                type="email"
                placeholder="é®ç®±"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
            </div>
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- æ°é -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">æ°é</label>
            <input
              v-model.number="form.quantity"
              type="number"
              min="1"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
          </div>

          <!-- éæ±è¯´æ?-->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              éæ±è¡¥åè¯´æ?<span class="text-red-500">*</span>
            </label>
            <textarea
              v-model="form.remark"
              rows="5"
              placeholder="è¯·è¯¦ç»æè¿°æ¨çéæ±?.."
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500 resize-y"
              :class="{ 'border-red-500': errors.remark }"
              required
            ></textarea>
            <p v-if="errors.remark" class="text-red-500 text-sm mt-1">{{ errors.remark }}</p>
            <p class="text-sm text-gray-500 mt-1">å·²è¾å?{{ form.remark.length }} ä¸ªå­ç¬?/p>
          </div>

          <!-- æäº¤æé® -->
          <div class="flex gap-4 pt-4">
            <button
              type="button"
              @click="$router.back()"
              class="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
            >
              åæ¶
            </button>
            <button
              type="submit"
              :disabled="submitting"
              class="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-red-500 text-var(--color-bg-light, white) rounded-lg hover:from-orange-600 hover:to-red-600 transition-all font-medium shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="submitting">æäº¤ä¸?..</span>
              <span v-else>æäº¤è®¢å</span>
            </button>
          </div>
        </form>
      </div>

      <!-- æ æ°æ®ç¶æ?-->
      <div v-else class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <p class="text-gray-600">ååä¸å­å¨æå·²ä¸æ?/p>
        <NuxtLink to="/tools" class="mt-4 inline-block text-orange-600 hover:text-orange-700">
          è¿åå·¥å·åè¡¨
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

// è·åååä¿¡æ¯
const fetchProduct = async () => {
  if (!productId.value) {
    loading.value = false
    return
  }

  loading.value = true
  try {
    console.log('è·åååä¿¡æ¯ï¼productId:', productId.value)
    const res = await api.get<any>(`/Toolbox/${productId.value}`)
    console.log('ååä¿¡æ¯ååº:', res)
    
    // useApi å·²ç»å¤çäºååºæ ¼å¼ï¼è¿åçæ¯ data é¨å
    // å¦æååºæ?{ code: 0, data: {...} }ï¼å res å°±æ¯ data
    if (res && (res.id || res.name)) {
      product.value = res
      console.log('ååä¿¡æ¯å è½½æå:', product.value)
    } else {
      console.error('ååä¿¡æ¯æ ¼å¼éè¯¯:', res)
      message.error('è·åååä¿¡æ¯å¤±è´¥')
    }
  } catch (e: any) {
    console.error('è·åååä¿¡æ¯å¤±è´¥:', e)
    console.error('éè¯¯è¯¦æ:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    message.error(e.response?.data?.message || e.message || 'è·åååä¿¡æ¯å¤±è´¥')
  } finally {
    loading.value = false
  }
}

// è¡¨åéªè¯
const validate = (): boolean => {
  errors.value = {}

  if (!form.value.customerName.trim()) {
    errors.value.customerName = 'è¯·è¾å¥æ¨çå§å?
    return false
  }

  if (!form.value.customerPhone && !form.value.customerWeChat && !form.value.customerEmail) {
    errors.value.contact = 'è³å°éè¦å¡«åä¸ç§èç³»æ¹å¼?
    return false
  }

  if (!form.value.remark.trim()) {
    errors.value.remark = 'è¯·è¾å¥éæ±è¯´æ?
    return false
  }

  // ç§»é¤å­ç¬¦æ°éå?  return true
}

// æäº¤è®¢å
const handleSubmit = async () => {
  if (!validate()) {
    return
  }

  submitting.value = true
  try {
    console.log('æäº¤è®¢åï¼æ°æ?', {
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

    console.log('æäº¤è®¢åååº:', res)

    // useApi å·²ç»å¤çäºååºæ ¼å¼ï¼å¦ææåï¼res ç´æ¥æ?data é¨å
    // åç«¯è¿åæ ¼å¼ï¼{ code: 0, data: { OrderId: xxx, OrderNo: 'xxx' } }
    // useApi å¤çåï¼res å°±æ¯ { OrderId: xxx, OrderNo: 'xxx' }
    if (res && res.orderNo) {
      // æ¾ç¤ºæåæç¤º
      message.success('è®¢åæäº¤æåï¼?)
      // è·³è½¬å°æåé¡µ
      setTimeout(() => {
        router.push(`/order/success?orderNo=${res.orderNo}`)
      }, 500)
    } else if (res && res.OrderNo) {
      // å¼å®¹åç«¯è¿åç?PascalCase æ ¼å¼
      message.success('è®¢åæäº¤æåï¼?)
      setTimeout(() => {
        router.push(`/order/success?orderNo=${res.OrderNo}`)
      }, 500)
    } else {
      console.error('è®¢åååºæ ¼å¼éè¯¯:', res)
      message.error('æäº¤è®¢åå¤±è´¥ï¼ååºæ ¼å¼éè¯?)
    }
  } catch (e: any) {
    console.error('æäº¤è®¢åå¤±è´¥:', e)
    console.error('éè¯¯è¯¦æ:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    message.error(e.response?.data?.message || e.message || 'æäº¤è®¢åå¤±è´¥ï¼è¯·ç¨åéè¯')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  fetchProduct()
})

// è®¾ç½®é¡µé¢æ é¢
useHead({
  title: 'æäº¤è®¢å - æºªåå¬é£',
  meta: [
    { name: 'description', content: 'æäº¤è®¢åé¡µé¢' }
  ]
})
</script>

<style scoped>
/* æ ·å¼å·²éè¿ Tailwind CSS ç±»å®ç?*/
</style>

