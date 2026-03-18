<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">投资仪表盘</h1>
      <div class="flex gap-2">
        <button @click="refreshPrices" class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700">
          刷新价格
        </button>
        <button @click="showCreateModal = true" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
          + 新增投资
        </button>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总成本</div>
        <div class="text-2xl font-bold text-gray-800 dark:text-white">¥{{ formatMoney(stats.TotalCost || 0) }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总市值</div>
        <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">¥{{ formatMoney(stats.TotalMarketValue || 0) }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">总盈亏</div>
        <div 
          class="text-2xl font-bold"
          :class="(stats.TotalProfitLoss || 0) >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
        >
          ¥{{ formatMoney(stats.TotalProfitLoss || 0) }}
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">收益率</div>
        <div 
          class="text-2xl font-bold"
          :class="(stats.TotalProfitRate || 0) >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
        >
          {{ formatPercent(stats.TotalProfitRate || 0) }}%
        </div>
      </div>
    </div>

    <!-- 投资列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden mb-6">
      <table class="w-full text-left">
        <thead class="bg-gray-50 dark:bg-gray-700/50 border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">代码</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">名称</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">类型</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">持仓</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">成本价</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">当前价</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">市值</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">盈亏</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">操作</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="item in investments" :key="item.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/30">
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200 font-mono">{{ item.code }}</td>
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">{{ item.name }}</td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400">
              <span class="px-2 py-1 rounded text-xs" :class="item.type === 'stock' ? 'bg-blue-100 text-blue-700' : 'bg-green-100 text-green-700'">
                {{ item.type === 'stock' ? '股票' : '基金' }}
              </span>
            </td>
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">{{ item.quantity }}</td>
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">¥{{ formatMoney(item.costPrice) }}</td>
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">¥{{ formatMoney(item.currentPrice) }}</td>
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200">¥{{ formatMoney(item.marketValue) }}</td>
            <td class="px-6 py-4">
              <div 
                class="font-semibold"
                :class="item.profitLoss >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
              >
                ¥{{ formatMoney(item.profitLoss) }}
              </div>
              <div 
                class="text-xs"
                :class="item.profitRate >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'"
              >
                {{ formatPercent(item.profitRate) }}%
              </div>
            </td>
            <td class="px-6 py-4">
              <div class="flex gap-2">
                <button @click="editItem(item)" class="text-blue-600 hover:text-blue-800">编辑</button>
                <button @click="addTransaction(item)" class="text-green-600 hover:text-green-800">交易</button>
                <button @click="deleteItem(item.id)" class="text-red-600 hover:text-red-800">删除</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- 创建/编辑模态框 -->
    <div v-if="showCreateModal || editingItem" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl w-full max-w-md m-4">
        <div class="p-6">
          <h2 class="text-xl font-bold mb-4">{{ editingItem ? '编辑' : '新增' }}投资</h2>
          
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium mb-1">代码</label>
              <input v-model="form.code" type="text" class="w-full border rounded px-3 py-2" placeholder="例如: 000001" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">名称</label>
              <input v-model="form.name" type="text" class="w-full border rounded px-3 py-2" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">类型</label>
              <select v-model="form.type" class="w-full border rounded px-3 py-2">
                <option value="stock">股票</option>
                <option value="fund">基金</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">持仓数量</label>
              <input v-model.number="form.quantity" type="number" step="0.01" class="w-full border rounded px-3 py-2" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">成本价</label>
              <input v-model.number="form.costPrice" type="number" step="0.01" class="w-full border rounded px-3 py-2" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-1">备注</label>
              <textarea v-model="form.notes" rows="3" class="w-full border rounded px-3 py-2"></textarea>
            </div>
          </div>

          <div class="flex gap-2 mt-6">
            <button @click="saveItem" class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
              保存
            </button>
            <button @click="cancelEdit" class="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300">
              取消
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const investments = ref<any[]>([])
const stats = ref<any>({})
const showCreateModal = ref(false)
const editingItem = ref<any>(null)
const form = ref({
  code: '',
  name: '',
  type: 'stock',
  quantity: 0,
  costPrice: 0,
  notes: ''
})

const fetchList = async () => {
  try {
    const res = await api.get<any>('/Investment')
    if (Array.isArray(res)) {
      investments.value = res
    } else if (res && res.List) {
      investments.value = res.List
    }

    // 获取统计数据
    const statsRes = await api.get<any>('/Investment/stats')
    if (statsRes) {
      stats.value = statsRes
    }
  } catch (e) {
    console.error('Failed to fetch investments:', e)
  }
}

const refreshPrices = async () => {
  try {
    await api.post('/Investment/refresh-prices')
    alert('价格刷新成功')
    fetchList()
  } catch (e: any) {
    alert(e.message || '刷新失败')
  }
}

const editItem = (item: any) => {
  editingItem.value = item
  form.value = {
    code: item.code,
    name: item.name,
    type: item.type,
    quantity: item.quantity,
    costPrice: item.costPrice,
    notes: item.notes || ''
  }
}

const addTransaction = (item: any) => {
  // TODO: 实现交易记录功能
  alert('交易功能开发中...')
}

const saveItem = async () => {
  try {
    if (editingItem.value) {
      await api.put(`/Investment/${editingItem.value.id}`, form.value)
    } else {
      await api.post('/Investment', form.value)
    }

    alert('保存成功')
    cancelEdit()
    fetchList()
  } catch (e: any) {
    alert(e.message || '保存失败')
  }
}

const deleteItem = async (id: number) => {
  if (!confirm('确定要删除吗？')) return
  try {
    await api.delete(`/Investment/${id}`)
    alert('删除成功')
    fetchList()
  } catch (e: any) {
    alert(e.message || '删除失败')
  }
}

const cancelEdit = () => {
  showCreateModal.value = false
  editingItem.value = null
  form.value = { code: '', name: '', type: 'stock', quantity: 0, costPrice: 0, notes: '' }
}

const formatMoney = (value: number) => {
  return value.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const formatPercent = (value: number) => {
  return value.toFixed(2)
}

onMounted(() => {
  fetchList()
})
</script>

