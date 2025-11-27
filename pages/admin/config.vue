<template>
  <div>
    <h1 class="text-2xl font-bold text-gray-800 dark:text-white mb-6">站点配置</h1>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 max-w-2xl">
      <div v-if="loading" class="text-center py-8 text-gray-500 dark:text-gray-400">加载中...</div>
      
      <form v-else class="space-y-6" @submit.prevent>
        <div v-for="(value, key) in configs" :key="key">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ formatKey(key) }} ({{ key }})</label>
          <div class="flex gap-2">
            <input 
              v-model="configs[key]" 
              type="text" 
              class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200"
              :disabled="savingKey === key"
            />
            <button 
              @click="saveConfig(key as string)" 
              class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
              :disabled="savingKey === key"
            >
              {{ savingKey === key ? '保存中' : '保存' }}
            </button>
          </div>
        </div>

        <!-- 添加新配置项 (可选) -->
        <div class="pt-6 border-t border-gray-100 dark:border-gray-700">
          <h3 class="text-sm font-bold text-gray-900 dark:text-white mb-4">添加配置</h3>
          <div class="flex gap-2">
            <input v-model="newKey" type="text" placeholder="Key (e.g. site_title)" class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            <input v-model="newValue" type="text" placeholder="Value" class="flex-1 border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200" />
            <button @click="addConfig" class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700">添加</button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const router = useRouter()
const api = useApi()

const loading = ref(false)
const savingKey = ref<string | null>(null)
const configs = ref<Record<string, string>>({})

const newKey = ref('')
const newValue = ref('')

const formatKey = (key: string | number) => {
  const k = String(key)
  const map: Record<string, string> = {
    site_title: '网站标题',
    site_subtitle: '网站副标题',
    icp_record: '备案号',
    home_page_size: '首页文章数'
  }
  return map[k] || k
}

const fetchConfigs = async () => {
  loading.value = true
  try {
    const res = await api.get<Record<string, string>>('/admin/config')
    configs.value = res
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

const saveConfig = async (key: string) => {
  savingKey.value = key
  try {
    await api.post('/admin/config', { [key]: configs.value[key] })
    // alert('保存成功') 
  } catch (e: any) {
    alert(e.message || '保存失败')
  } finally {
    savingKey.value = null
  }
}

const addConfig = async () => {
  if (!newKey.value) return
  configs.value[newKey.value] = newValue.value
  await saveConfig(newKey.value)
  newKey.value = ''
  newValue.value = ''
}

onMounted(() => {
  fetchConfigs()
})
</script>

