<template>
  <div class="min-h-screen py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold font-['Outfit']">čŽżĺŽ˘ć°ćŽ</h1>
        <button 
          @click="fetchData" 
          :disabled="loading"
          class="px-4 py-2 rounded-lg text-sm font-medium transition-colors disabled:opacity-50"
          :class="loading ? 'bg-gray-300 cursor-not-allowed' : 'bg-primary text-var(--color-bg-light, white) hover:bg-primary-hover'"
        >
          <i v-if="loading" class="fas fa-spinner fa-spin mr-2"></i>
          <i v-else class="fas fa-sync-alt mr-2"></i>
          {{ loading ? 'ĺ č˝˝ä¸?..' : 'ĺˇć°ć°ćŽ' }}
        </button>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
          <div class="text-sm font-medium text-gray-500 mb-1">Total Visits</div>
          <div class="text-3xl font-bold text-gray-900">{{ data?.totalVisits || 0 }}</div>
        </div>
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
          <div class="text-sm font-medium text-gray-500 mb-1">Unique Visitors</div>
          <div class="text-3xl font-bold text-blue-600">{{ data?.uniqueVisitors || 0 }}</div>
        </div>
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-gray-100">
          <div class="text-sm font-medium text-gray-500 mb-1">Today's Visits</div>
          <div class="text-3xl font-bold text-green-600">{{ data?.todayVisits || 0 }}</div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Recent Visits Table -->
        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900">Recent Visits</h2>
          </div>
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Time</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">IP Address</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Path</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Visitor ID</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-if="loading" class="text-center py-8">
                  <td colspan="4" class="px-6 py-8 text-gray-500">
                    <i class="fas fa-spinner fa-spin mr-2"></i>
                    ĺ č˝˝ä¸?..
                  </td>
                </tr>
                <tr v-else-if="!data?.recentVisits || data.recentVisits.length === 0" class="text-center py-8">
                  <td colspan="4" class="px-6 py-8 text-gray-500">
                    ćć čŽżéŽčŽ°ĺ˝
                  </td>
                </tr>
                <tr v-else v-for="visit in data.recentVisits" :key="visit.id || visit.Id || `${visit.timestamp}-${visit.path}`" class="hover:bg-gray-50">
                  <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-500">
                    {{ formatDate(visit.timestamp || visit.Timestamp) }}
                  </td>
                  <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm font-mono text-gray-600">
                    {{ visit.ip || visit.Ip || 'N/A' }}
                  </td>
                  <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm text-gray-900">
                    {{ visit.path || visit.Path || 'N/A' }}
                  </td>
                  <td class="px-6 py-4 var(--color-bg-light, white)space-nowrap text-sm font-mono text-gray-400 text-xs">
                    {{ (visit.visitorId || visit.VisitorId || '').substring(0, 8) }}...
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Top Paths -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 h-fit">
          <div class="px-6 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900">Top Referrers/Paths</h2>
          </div>
          <div class="p-6">
            <div v-if="loading" class="text-center py-8 text-gray-500">
              <i class="fas fa-spinner fa-spin mr-2"></i>
              ĺ č˝˝ä¸?..
            </div>
            <div v-else-if="!data?.topPaths || data.topPaths.length === 0" class="text-center py-8 text-gray-500">
              ćć ć°ćŽ
            </div>
            <div v-else v-for="(item, index) in data.topPaths" :key="item.path || item.Path || index" class="flex items-center justify-between mb-4 last:mb-0">
              <div class="flex items-center">
                <span class="w-6 h-6 rounded-full bg-gray-100 text-gray-600 flex items-center justify-center text-xs font-bold mr-3">
                  {{ index + 1 }}
                </span>
                <span class="text-sm text-gray-700 truncate max-w-[150px]" :title="item.path || item.Path">
                  {{ item.path || item.Path || 'N/A' }}
                </span>
              </div>
              <span class="text-sm font-bold text-gray-900">{{ item.count || item.Count || 0 }}</span>
            </div>
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
const data = ref<any>(null)
const loading = ref(false)

const fetchData = async () => {
  try {
    loading.value = true
    // ä˝żç¨ć­ŁçĄŽç?API çŤŻçščˇĺčŽżĺŽ˘ć°ćŽ
    const res = await api.get('/Stats')
    
    // ĺ¤ç API čżĺçć°ćŽć źĺźďźĺźĺŽšĺ¤§ĺ°ĺďź
    if (res) {
      data.value = {
        totalVisits: res.TotalVisits ?? res.totalVisits ?? 0,
        uniqueVisitors: res.UniqueVisitors ?? res.uniqueVisitors ?? 0,
        todayVisits: res.TodayVisits ?? res.todayVisits ?? 0,
        recentVisits: res.RecentVisits ?? res.recentVisits ?? res.VisitLogs ?? res.visitLogs ?? [],
        topPaths: res.TopPaths ?? res.topPaths ?? []
      }
    }
  } catch (e) {
    console.error('Failed to fetch visitor stats', e)
    // ćžç¤şéčŻŻćç¤ş
    const { error } = useNotification()
    error('čˇĺčŽżĺŽ˘ć°ćŽĺ¤ąč´ĽďźčŻˇç¨ĺéčŻ')
  } finally {
    loading.value = false
  }
}

// ć źĺźĺćĽć?const formatDate = (dateString?: string | Date) => {
  if (!dateString) return 'N/A'
  try {
    const date = typeof dateString === 'string' ? new Date(dateString) : dateString
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    })
  } catch {
    return String(dateString)
  }
}

onMounted(() => {
  fetchData()
})
</script>

