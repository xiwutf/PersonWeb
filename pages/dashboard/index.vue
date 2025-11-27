<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 px-4 sm:px-6 lg:px-8 font-['Outfit']">
    <div class="max-w-7xl mx-auto">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-blue-400 to-purple-600 mb-2">
          DIGITAL TWIN DASHBOARD
        </h1>
        <p class="text-gray-500 dark:text-gray-400 tracking-widest uppercase text-sm">Quantified Self & Life Metrics</p>
      </div>

      <!-- 核心指标卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <!-- Life Battery -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-green-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Life Battery</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ latest.energy }}%</span>
            <span class="text-sm text-green-500 mb-1">Energy</span>
          </div>
          <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 mt-4">
            <div class="bg-gradient-to-r from-green-400 to-green-600 h-2 rounded-full transition-all duration-1000" :style="{ width: latest.energy + '%' }"></div>
          </div>
          <p class="text-xs text-gray-400 mt-2">Sleep: {{ latest.sleep }}h</p>
        </div>

        <!-- Activity -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-orange-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Daily Activity</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ latest.steps }}</span>
            <span class="text-sm text-orange-500 mb-1">Steps</span>
          </div>
          <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2 mt-4">
            <div class="bg-gradient-to-r from-orange-400 to-red-500 h-2 rounded-full transition-all duration-1000" :style="{ width: Math.min((latest.steps / 10000) * 100, 100) + '%' }"></div>
          </div>
          <p class="text-xs text-gray-400 mt-2">Goal: 10,000</p>
        </div>

        <!-- Physique -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-blue-500" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Physique</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ latest.weight }}</span>
            <span class="text-sm text-blue-500 mb-1">kg</span>
          </div>
          <div class="mt-4">
             <span class="text-xs px-2 py-1 rounded bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">BMI: {{ (latest.weight / (1.75 * 1.75)).toFixed(1) }}</span>
          </div>
        </div>

        <!-- Net Worth -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700 relative overflow-hidden group">
          <div class="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <svg class="w-24 h-24 text-purple-500" fill="currentColor" viewBox="0 0 20 20"><path d="M4 4a2 2 0 00-2 2v1h16V6a2 2 0 00-2-2H4z"></path><path fill-rule="evenodd" d="M18 9H2v5a2 2 0 002 2h12a2 2 0 002-2V9zM4 13a1 1 0 011-1h1a1 1 0 110 2H5a1 1 0 01-1-1zm5-1a1 1 0 100 2h1a1 1 0 100-2H9z" clip-rule="evenodd"></path></svg>
          </div>
          <h3 class="text-gray-500 dark:text-gray-400 text-sm uppercase tracking-wider mb-2">Net Worth</h3>
          <div class="flex items-end gap-2">
            <span class="text-4xl font-bold text-gray-900 dark:text-white">¥{{ (latest.netWorth / 10000).toFixed(1) }}</span>
            <span class="text-sm text-purple-500 mb-1">万</span>
          </div>
          <div class="mt-4 flex items-center text-xs text-green-500">
            <svg class="w-3 h-3 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path></svg>
            <span>Growing</span>
          </div>
        </div>
      </div>

      <!-- 图表区域 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- 趋势图 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-6">Health Trends</h3>
          <div class="h-64">
            <Line :data="healthChartData" :options="chartOptions" />
          </div>
        </div>

        <!-- 财富趋势 -->
        <div class="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-lg border border-gray-100 dark:border-gray-700">
          <h3 class="text-lg font-bold text-gray-800 dark:text-white mb-6">Wealth Growth</h3>
          <div class="h-64">
            <Line :data="wealthChartData" :options="chartOptions" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'
import { Line } from 'vue-chartjs'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

const api = useApi()
const metrics = ref<any[]>([])
const latest = ref({
  steps: 0,
  sleep: 0,
  weight: 0,
  netWorth: 0,
  energy: 0
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
      labels: { color: '#9ca3af' }
    }
  },
  scales: {
    y: {
      grid: { color: '#374151' },
      ticks: { color: '#9ca3af' }
    },
    x: {
      grid: { display: false },
      ticks: { color: '#9ca3af' }
    }
  },
  interaction: {
    mode: 'index',
    intersect: false,
  },
}

const healthChartData = computed(() => {
  const labels = metrics.value.map(m => m.date.slice(5)).slice(-30) // Last 30 days
  const steps = metrics.value.map(m => m.steps).slice(-30)
  const weight = metrics.value.map(m => m.weight).slice(-30)

  return {
    labels,
    datasets: [
      {
        label: 'Steps',
        data: steps,
        borderColor: '#f97316',
        backgroundColor: 'rgba(249, 115, 22, 0.1)',
        yAxisID: 'y',
        fill: true,
        tension: 0.4
      },
      {
        label: 'Weight (kg)',
        data: weight,
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        yAxisID: 'y1',
        fill: true,
        tension: 0.4
      }
    ]
  }
})

const wealthChartData = computed(() => {
  const labels = metrics.value.map(m => m.date.slice(5)).slice(-30)
  const netWorth = metrics.value.map(m => m.netWorth).slice(-30)

  return {
    labels,
    datasets: [
      {
        label: 'Net Worth',
        data: netWorth,
        borderColor: '#a855f7',
        backgroundColor: 'rgba(168, 85, 247, 0.1)',
        fill: true,
        tension: 0.4
      }
    ]
  }
})

onMounted(async () => {
  try {
    const res = await api.get<any[]>('/admin/metrics')
    // Sort by date ascending for charts
    metrics.value = res.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    
    if (metrics.value.length > 0) {
      latest.value = metrics.value[metrics.value.length - 1]
    }
  } catch (e) {
    console.error(e)
  }
})
</script>
