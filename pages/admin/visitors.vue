<template>
  <div class="min-h-screen py-12 px-4 sm:px-6 lg:px-8 bg-gray-50">
    <div class="max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900 font-['Outfit']">Visitor Dashboard</h1>
        <button @click="fetchData" class="px-4 py-2 bg-white border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50 transition-colors">
          Refresh Data
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
                <tr v-for="visit in data?.recentVisits" :key="visit.id" class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ new Date(visit.timestamp).toLocaleString() }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-mono text-gray-600">
                    {{ visit.ip }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {{ visit.path }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-mono text-gray-400 text-xs">
                    {{ visit.visitorId.substring(0, 8) }}...
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
            <div v-for="(item, index) in data?.topPaths" :key="item.path" class="flex items-center justify-between mb-4 last:mb-0">
              <div class="flex items-center">
                <span class="w-6 h-6 rounded-full bg-gray-100 text-gray-600 flex items-center justify-center text-xs font-bold mr-3">
                  {{ index + 1 }}
                </span>
                <span class="text-sm text-gray-700 truncate max-w-[150px]" :title="item.path">{{ item.path }}</span>
              </div>
              <span class="text-sm font-bold text-gray-900">{{ item.count }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: false,
  middleware: 'admin-auth'
})

const api = useApi()
const data = ref<any>(null)

const fetchData = async () => {
  try {
    data.value = await api.get('/stats')
  } catch (e) {
    console.error('Failed to fetch stats', e)
  }
}

onMounted(() => {
  fetchData()
})
</script>
