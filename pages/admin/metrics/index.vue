<template>
  <div>
    <h1 class="text-2xl font-bold text-gray-800 dark:text-var(--color-bg-light, white) mb-6">个人数据录入</h1>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- 录入表单 -->
      <div class="lg:col-span-1 bg-var(--color-bg-light, white) dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h2 class="text-lg font-semibold mb-4 text-gray-700 dark:text-gray-300">每日记录</h2>
        <form @submit.prevent="handleSave" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">日期</label>
            <input v-model="form.date" type="date" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-var(--color-bg-light, white) dark:bg-gray-900 text-gray-800 dark:text-gray-200" required />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">步数 (Steps)</label>
            <input v-model="form.steps" type="number" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-var(--color-bg-light, white) dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="e.g. 10000" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">睡眠时长 (小时)</label>
            <input v-model="form.sleep" type="number" step="0.1" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-var(--color-bg-light, white) dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="e.g. 7.5" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">体重 (kg)</label>
            <input v-model="form.weight" type="number" step="0.1" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-var(--color-bg-light, white) dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="e.g. 70.5" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">净资产 (CNY)</label>
            <input v-model="form.netWorth" type="number" class="w-full border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-var(--color-bg-light, white) dark:bg-gray-900 text-gray-800 dark:text-gray-200" placeholder="e.g. 150000" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">能量值 (1-100)</label>
            <input v-model="form.energy" type="range" min="1" max="100" class="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer dark:bg-gray-700" />
            <div class="text-right text-sm text-gray-500">{{ form.energy }}</div>
          </div>

          <button type="submit" class="w-full bg-blue-600 text-var(--color-bg-light, white) py-2 rounded hover:bg-blue-700 transition" :disabled="saving">
            {{ saving ? '保存中...' : '保存记录' }}
          </button>
        </form>
      </div>

      <!-- 历史数据表格 -->
      <div class="lg:col-span-2 bg-var(--color-bg-light, white) dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-lg font-semibold text-gray-700 dark:text-gray-300">历史数据</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">日期</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">步数</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">睡眠</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">体重</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">净资产</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">能量</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">操作</th>
              </tr>
            </thead>
            <tbody class="bg-var(--color-bg-light, white) dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="item in sortedMetrics" :key="item.date">
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-900 dark:text-var(--color-bg-light, white)">{{ item.date }}</td>
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-500 dark:text-gray-400">{{ item.steps }}</td>
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-500 dark:text-gray-400">{{ item.sleep }}h</td>
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-500 dark:text-gray-400">{{ item.weight }}kg</td>
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-500 dark:text-gray-400">¥{{ item.netWorth }}</td>
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-500 dark:text-gray-400">
                  <div class="w-16 bg-gray-200 rounded-full h-2.5 dark:bg-gray-700">
                    <div class="bg-green-600 h-2.5 rounded-full" :style="{ width: item.energy + '%' }"></div>
                  </div>
                </td>
                <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-right text-sm font-medium">
                  <button @click="editRecord(item)" class="text-blue-600 hover:text-blue-900 dark:hover:text-blue-400">编辑</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Metric {
  date: string
  steps: number
  sleep: number
  weight: number
  netWorth: number
  energy: number
}

import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const metrics = ref<Metric[]>([])
const saving = ref(false)

const form = ref({
  date: new Date().toISOString().split('T')[0],
  steps: 0,
  sleep: 7.0,
  weight: 0,
  netWorth: 0,
  energy: 80
})

const sortedMetrics = computed(() => {
  return [...metrics.value].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

const fetchMetrics = async () => {
  try {
    const res = await api.get<Metric[]>('/Metrics')
    // Format date to YYYY-MM-DD
    metrics.value = res.map(m => ({
      ...m,
      date: m.date.split('T')[0]
    }))
    
    // 如果今天有数据，自动填充
    const today = new Date().toISOString().split('T')[0]
    const todayData = metrics.value.find(m => m.date === today)
    if (todayData) {
      form.value = { ...todayData }
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch metrics', e)
    }
  }
}

const handleSave = async () => {
  saving.value = true
  const { success } = useNotification()
  const { handleError } = useErrorHandler()
  
  try {
    await api.post('/Metrics', form.value)
    await fetchMetrics()
    success('保存成功')
  } catch (e: unknown) {
    handleError(e, '保存失败')
  } finally {
    saving.value = false
  }
}

const editRecord = (item: Metric) => {
  form.value = { ...item }
}

onMounted(() => {
  fetchMetrics()
})
</script>
