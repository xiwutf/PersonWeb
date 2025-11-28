<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">时间胶囊管理</h1>
    </div>

    <!-- 统计信息 -->
    <div class="grid grid-cols-3 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400">待审核</div>
        <div class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ stats.pending }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400">已展示</div>
        <div class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats.approved }}</div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
        <div class="text-sm text-gray-500 dark:text-gray-400">已拒绝</div>
        <div class="text-2xl font-bold text-red-600 dark:text-red-400">{{ stats.rejected }}</div>
      </div>
    </div>

    <!-- 筛选 -->
    <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6 flex gap-4">
      <select v-model="statusFilter" @change="fetchCapsules" class="border border-gray-300 dark:border-gray-600 rounded px-3 py-2 bg-white dark:bg-gray-900 text-gray-800 dark:text-gray-200">
        <option value="">全部状态</option>
        <option value="0">待审核</option>
        <option value="1">已展示</option>
        <option value="2">已拒绝</option>
      </select>
    </div>

    <!-- 胶囊列表 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
      <table class="w-full text-left">
        <thead class="bg-gray-50 dark:bg-gray-700/50 border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">内容</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">访客</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">提交时间</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">状态</th>
            <th class="px-6 py-3 text-sm font-medium text-gray-500 dark:text-gray-400">操作</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-if="loading">
            <td colspan="5" class="px-6 py-4 text-center text-gray-500 dark:text-gray-400">加载中...</td>
          </tr>
          <tr v-else-if="capsules.length === 0">
            <td colspan="5" class="px-6 py-4 text-center text-gray-500 dark:text-gray-400">暂无时间胶囊</td>
          </tr>
          <tr v-for="capsule in capsules" :key="capsule.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/30 transition-colors">
            <td class="px-6 py-4 text-gray-800 dark:text-gray-200 max-w-md">
              <p class="line-clamp-2">{{ capsule.content }}</p>
            </td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400 text-sm">
              {{ capsule.visitorName || '匿名' }}
            </td>
            <td class="px-6 py-4 text-gray-500 dark:text-gray-400 text-sm">
              {{ formatDate(capsule.createdAt) }}
            </td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(capsule.status)" class="px-2 py-1 rounded text-xs">
                {{ getStatusText(capsule.status) }}
              </span>
            </td>
            <td class="px-6 py-4">
              <div class="flex gap-2">
                <button
                  v-if="capsule.status === 0"
                  @click="approveCapsule(capsule.id)"
                  class="text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-300"
                >
                  通过
                </button>
                <button
                  v-if="capsule.status === 0"
                  @click="rejectCapsule(capsule.id)"
                  class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300"
                >
                  拒绝
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

const capsules = ref<any[]>([])
const loading = ref(false)
const statusFilter = ref('')
const stats = ref({
  pending: 0,
  approved: 0,
  rejected: 0
})

const fetchCapsules = async () => {
  loading.value = true
  try {
    const endpoint = statusFilter.value ? '/TimeCapsule/pending' : '/TimeCapsule/pending'
    const res = await api.get<any>(endpoint, {
      params: {
        page: 1,
        pageSize: 100
      }
    })

    if (res && res.List) {
      capsules.value = res.List
    } else if (Array.isArray(res)) {
      capsules.value = res
    } else {
      capsules.value = []
    }

    // 更新统计
    updateStats()
  } catch (e) {
    console.error('Failed to fetch capsules:', e)
    capsules.value = []
  } finally {
    loading.value = false
  }
}

const updateStats = () => {
  stats.value = {
    pending: capsules.value.filter(c => c.status === 0).length,
    approved: capsules.value.filter(c => c.status === 1).length,
    rejected: capsules.value.filter(c => c.status === 2).length
  }
}

const approveCapsule = async (id: number) => {
  if (!confirm('确定要通过这个时间胶囊吗？')) return

  try {
    await api.post(`/TimeCapsule/${id}/approve`)
    alert('已通过审核')
    fetchCapsules()
  } catch (e: any) {
    alert(e.message || '操作失败')
  }
}

const rejectCapsule = async (id: number) => {
  if (!confirm('确定要拒绝这个时间胶囊吗？')) return

  try {
    await api.post(`/TimeCapsule/${id}/reject`)
    alert('已拒绝')
    fetchCapsules()
  } catch (e: any) {
    alert(e.message || '操作失败')
  }
}

const getStatusText = (status: number) => {
  const statusMap: Record<number, string> = {
    0: '待审核',
    1: '已展示',
    2: '已拒绝'
  }
  return statusMap[status] || '未知'
}

const getStatusClass = (status: number) => {
  const classMap: Record<number, string> = {
    0: 'bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-300',
    1: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300',
    2: 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300'
  }
  return classMap[status] || 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300'
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-'
  return new Date(dateStr).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchCapsules()
})
</script>

