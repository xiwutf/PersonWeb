<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 via-red-50 to-pink-50 py-8">
    <div class="container mx-auto px-4 max-w-3xl">
      <!-- ???? -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">????</h1>
        <p class="text-gray-600">???????????</p>
      </div>

      <!-- ?????-->
      <div v-if="loading" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <div class="animate-pulse">
          <div class="h-8 bg-gray-200 rounded mb-4"></div>
          <div class="h-4 bg-gray-200 rounded mb-2"></div>
        </div>
      </div>

      <!-- ???? -->
      <div v-else-if="product" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <!-- ???????? -->
        <div class="mb-6 p-4 bg-gray-50 rounded-lg border border-gray-200">
          <h3 class="text-sm font-medium text-gray-700 mb-2">????</h3>
          <p class="text-lg font-semibold text-gray-900">{{ product.name }}</p>
          <p v-if="product.price > 0" class="text-xl font-bold text-green-600 mt-2">
            ?{{ product.price }}
            <span v-if="product.originalPrice && product.originalPrice > product.price" class="text-sm text-gray-500 line-through ml-2">
              ?{{ product.originalPrice }}
            </span>
          </p>
          <p v-else class="text-gray-600 mt-2">????</p>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- ???? -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              ???? <span class="text-red-500">*</span>
            </label>
            <input
              v-model="form.customerName"
              type="text"
              placeholder="???????"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              :class="{ 'border-red-500': errors.customerName }"
              required
            />
            <p v-if="errors.customerName" class="text-red-500 text-sm mt-1">{{ errors.customerName }}</p>
          </div>

          <!-- ???? -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              ???? <span class="text-red-500">*</span>
            </label>
            <p class="text-sm text-gray-500 mb-3">??????????</p>
            
            <div class="space-y-3">
              <input
                v-model="form.customerPhone"
                type="tel"
                placeholder="???"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
              <input
                v-model="form.customerWeChat"
                type="text"
                placeholder="??"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
              <input
                v-model="form.customerEmail"
                type="email"
                placeholder="??"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
              />
            </div>
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- ?? -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">??</label>
            <input
              v-model.number="form.quantity"
              type="number"
              min="1"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500"
            />
          </div>

          <!-- ?????-->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              ???????<span class="text-red-500">*</span>
            </label>
            <textarea
              v-model="form.remark"
              rows="5"
              placeholder="??????????.."
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500 resize-y"
              :class="{ 'border-red-500': errors.remark }"
              required
            ></textarea>
            <p v-if="errors.remark" class="text-red-500 text-sm mt-1">{{ errors.remark }}</p>
            <p class="text-sm text-gray-500 mt-1">????{{ form.remark.length }} ???</p>
          </div>

          <!-- ???? -->
          <div class="flex gap-4 pt-4">
            <button
              type="button"
              @click="$router.back()"
              class="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
            >
              ??
            </button>
            <button
              type="submit"
              :disabled="submitting"
              class="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-red-500 text-var(--color-bg-light, white) rounded-lg hover:from-orange-600 hover:to-red-600 transition-all font-medium shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="submitting">????..</span>
              <span v-else>????</span>
            </button>
          </div>
        </form>
      </div>

      <!-- ??????-->
      <div v-else class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 text-center">
        <p class="text-gray-600">?????????</p>
        <NuxtLink to="/tools" class="mt-4 inline-block text-orange-600 hover:text-orange-700">
          ??????
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

// ??????
const fetchProduct = async () => {
  if (!productId.value) {
    loading.value = false
    return
  }

  loading.value = true
  try {
    console.log('???????productId:', productId.value)
    const res = await api.get<any>(`/Toolbox/${productId.value}`)
    console.log('??????:', res)
    
    // useApi ?????????????? data ??
    // ??????{ code: 0, data: {...} }?? res ?? data
    if (res && (res.id || res.name)) {
      product.value = res
      console.log('????????:', product.value)
    } else {
      console.error('????????:', res)
      message.error('????????')
    }
  } catch (e: any) {
    console.error('????????:', e)
    console.error('????:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    message.error(e.response?.data?.message || e.message || '????????')
  } finally {
    loading.value = false
  }
}

// ????
const validate = (): boolean => {
  errors.value = {}

  if (!form.value.customerName.trim()) {
    errors.value.customerName = '?????'
    return false
  }

  if (!form.value.customerPhone && !form.value.customerWeChat && !form.value.customerEmail) {
    errors.value.contact = '???????????'
    return false
  }

  if (!form.value.remark.trim()) {
    errors.value.remark = '???????'
    return false
  }

  // ????
  return true
}

// ????
const handleSubmit = async () => {
  if (!validate()) {
    return
  }

  submitting.value = true
  try {
    console.log('????????', {
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

    console.log('??????:', res)

    // useApi ???????????????res ????data ??
    // ???????{ code: 0, data: { OrderId: xxx, OrderNo: 'xxx' } }
    // useApi ????res ?? { OrderId: xxx, OrderNo: 'xxx' }
    if (res && res.orderNo) {
      // ??????
      message.success('??????')
      // ??????
      setTimeout(() => {
        router.push(`/order/success?orderNo=${res.orderNo}`)
      }, 500)
    } else if (res && res.OrderNo) {
      // ????????PascalCase ??
      message.success('??????')
      setTimeout(() => {
        router.push(`/order/success?orderNo=${res.OrderNo}`)
      }, 500)
    } else {
      console.error('????????:', res)
      message.error('??????????')
    }
  } catch (e: any) {
    console.error('??????:', e)
    console.error('????:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    message.error(e.response?.data?.message || e.message || '????????????')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  fetchProduct()
})

// ??????
useHead({
  title: '???? - ????',
  meta: [
    { name: 'description', content: '??????' }
  ]
})
</script>

<style scoped>
/* ????? Tailwind CSS ????*/
</style>

