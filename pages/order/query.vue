<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 py-8">
    <div class="container mx-auto px-4 max-w-2xl">
      <!-- ???? -->
      <div class="mb-8 text-center">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">????</h1>
        <p class="text-gray-600">??????????????????</p>
      </div>

      <!-- ???? -->
      <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8 mb-6">
        <form @submit.prevent="handleQuery" class="space-y-6">
          <!-- ???? -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              ???? <span class="text-red-500">*</span>
            </label>
            <input
              v-model="queryForm.orderNo"
              type="text"
              placeholder="??????????20251208-1234?"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              :class="{ 'border-red-500': errors.orderNo }"
              required
            />
            <p v-if="errors.orderNo" class="text-red-500 text-sm mt-1">{{ errors.orderNo }}</p>
          </div>

          <!-- ???? -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              ???? <span class="text-red-500">*</span>
            </label>
            <p class="text-sm text-gray-500 mb-3">??????????????????</p>
            <input
              v-model="queryForm.contact"
              type="text"
              placeholder="???????"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              :class="{ 'border-red-500': errors.contact }"
              required
            />
            <p v-if="errors.contact" class="text-red-500 text-sm mt-1">{{ errors.contact }}</p>
          </div>

          <!-- ???? -->
          <button
            type="submit"
            :disabled="querying"
            class="w-full px-6 py-3 bg-gradient-to-r from-blue-500 to-indigo-500 text-var(--color-bg-light, white) rounded-lg hover:from-blue-600 hover:to-indigo-600 transition-all font-medium shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="querying">????..</span>
            <span v-else>
              <i class="fas fa-search mr-2"></i>
              ????
            </span>
          </button>
        </form>
      </div>

      <!-- ???? -->
      <div v-if="orderResult" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl p-8">
        <h3 class="text-xl font-bold text-gray-900 mb-6">????</h3>
        
        <div class="space-y-4">
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">?????</span>
            <span class="font-mono font-semibold text-gray-900">{{ orderResult.orderNo }}</span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">?????</span>
            <span class="text-gray-900 font-medium">{{ orderResult.productName }}</span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">?????</span>
            <span :class="getStatusClass(orderResult.status)" class="px-3 py-1 rounded-full text-sm font-medium">
              {{ getStatusText(orderResult.status) }}
            </span>
          </div>
          
          <div class="flex justify-between items-center pb-4 border-b border-gray-200">
            <span class="text-gray-600">?????</span>
            <span class="text-gray-900">{{ formatDate(orderResult.createdAt) }}</span>
          </div>
          
          <div v-if="orderResult.remark" class="pt-4">
            <span class="text-gray-600 block mb-2">??/?????</span>
            <p class="text-gray-900 bg-gray-50 rounded-lg p-4 var(--color-bg-light, white)space-pre-line">{{ orderResult.remark }}</p>
          </div>
        </div>

        <!-- ???? -->
        <div class="mt-6 pt-6 border-t border-gray-200">
          <button
            @click="handleReset"
            class="w-full px-6 py-3 border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
          >
            ??????
          </button>
        </div>
      </div>

      <!-- ???? -->
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

// ????
const validate = (): boolean => {
  errors.value = {}

  if (!queryForm.value.orderNo.trim()) {
    errors.value.orderNo = '???????'
    return false
  }

  if (!queryForm.value.contact.trim()) {
    errors.value.contact = '???????'
    return false
  }

  return true
}

// ????
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

    console.log('??????:', res)

    // useApi ???????????????res ????data ??
    // ???????{ code: 0, data: {...} }
    // useApi ????res ?? {...}
    if (res && (res.orderNo || res.OrderNo)) {
      orderResult.value = res
      queryError.value = null
    } else if (res && res.code === 0 && res.data) {
      // ??????????
      orderResult.value = res.data
      queryError.value = null
    } else {
      queryError.value = res?.message || '???????'
    }
  } catch (e: any) {
    console.error('??????:', e)
    console.error('????:', {
      message: e.message,
      response: e.response,
      status: e.response?.status,
      data: e.response?.data
    })
    queryError.value = e.response?.data?.message || e.message || '????'
  } finally {
    querying.value = false
  }
}

// ????
const handleReset = () => {
  queryForm.value = {
    orderNo: '',
    contact: ''
  }
  orderResult.value = null
  queryError.value = null
  errors.value = {}
}

// ??????
const getStatusText = (status: number): string => {
  const statusMap: Record<number, string> = {
    0: '???',
    1: '???',
    2: '???',
    3: '???'
  }
  return statusMap[status] || '??'
}

// ???????
const getStatusClass = (status: number): string => {
  const classMap: Record<number, string> = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-blue-100 text-blue-800',
    2: 'bg-green-100 text-green-800',
    3: 'bg-gray-100 text-gray-800'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800'
}

// ?????
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

