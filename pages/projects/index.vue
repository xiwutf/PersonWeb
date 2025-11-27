<template>
  <div class="container mx-auto px-4 py-12">
    <div class="text-center mb-16">
      <h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">项目展示</h1>
      <p class="text-xl text-gray-600 dark:text-gray-400">探索我的开源项目和技术实验</p>
    </div>

    <div v-if="loading" class="text-center py-20">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
    </div>

    <!-- Debug Info -->
    <div v-if="error" class="bg-red-100 text-red-700 p-4 rounded mb-8">
      Error: {{ error }}
    </div>
    <div v-if="projects.length === 0 && !loading" class="text-center text-gray-500">
      No projects found. API Response: {{ debugData }}
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
      <div v-for="project in projects" :key="project.id" class="bg-white dark:bg-gray-800 rounded-xl shadow-lg overflow-hidden hover:shadow-xl transition-shadow duration-300 flex flex-col">
        <!-- 封面图 -->
        <div class="h-48 overflow-hidden relative group">
          <img :src="project.coverUrl || 'https://placehold.co/600x400'" :alt="project.title" class="w-full h-full object-cover transform group-hover:scale-105 transition-transform duration-500" />
          <div class="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-30 transition-opacity duration-300 flex items-center justify-center opacity-0 group-hover:opacity-100">
            <div class="flex gap-4">
              <a v-if="project.demoUrl" :href="project.demoUrl" target="_blank" class="bg-white text-gray-900 px-4 py-2 rounded-full font-bold hover:bg-gray-100 transition">Live Demo</a>
              <a v-if="project.githubUrl" :href="project.githubUrl" target="_blank" class="bg-gray-900 text-white px-4 py-2 rounded-full font-bold hover:bg-gray-800 transition">GitHub</a>
            </div>
          </div>
        </div>

        <!-- 内容 -->
        <div class="p-6 flex-1 flex flex-col">
          <div class="flex justify-between items-start mb-4">
            <h3 class="text-xl font-bold text-gray-900 dark:text-white">{{ project.title }}</h3>
            <span class="px-2 py-1 text-xs font-semibold rounded bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">
              {{ project.status }}
            </span>
          </div>
          
          <p class="text-gray-600 dark:text-gray-400 mb-4 flex-1">{{ project.description }}</p>

          <!-- 技术栈 -->
          <div class="flex flex-wrap gap-2 mb-6">
            <span v-for="tech in project.techStack" :key="tech" class="px-2 py-1 text-xs bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 rounded">
              {{ tech }}
            </span>
          </div>

          <!-- GitHub Activity Chart -->
          <div v-if="project.githubUrl && project.chartData" class="mt-auto pt-4 border-t border-gray-100 dark:border-gray-700">
            <p class="text-xs text-gray-500 mb-2">GitHub Activity (Last Year)</p>
            <div class="h-24">
              <Bar :data="project.chartData" :options="chartOptions" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js'
import { Bar } from 'vue-chartjs'

// 注册 Chart.js 组件
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const api = useApi()
const projects = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const debugData = ref('')

// 图表配置
const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: { 
      enabled: true,
      intersect: false,
      mode: 'index'
    }
  },
  scales: {
    x: { display: false },
    y: { display: false }
  },
  elements: {
    bar: {
      borderRadius: 2
    }
  }
}

const fetchProjects = async () => {
  try {
    const res = await api.get<any[]>('/projects')
    console.log('API Response:', res)
    debugData.value = JSON.stringify(res)
    projects.value = res
    
    // 异步加载 GitHub 数据
    loadGithubStats()
  } catch (e: any) {
    console.error('Failed to fetch projects', e)
    error.value = e.message || 'Failed to load projects'
  } finally {
    loading.value = false
  }
}

const loadGithubStats = async () => {
  for (const project of projects.value) {
    if (project.githubUrl) {
      try {
        // 解析 owner/repo
        const match = project.githubUrl.match(/github\.com\/([^\/]+\/[^\/]+)/)
        if (match) {
          const repo = match[1]
          const stats = await api.get<any[]>(`/github/stats?repo=${repo}`)
          
          if (stats && Array.isArray(stats)) {
            // stats 是周数据数组: { total: number, week: timestamp, days: [] }
            // 我们取最近 26 周 (半年)
            const recentStats = stats.slice(-26)
            
            project.chartData = {
              labels: recentStats.map(w => new Date(w.week * 1000).toLocaleDateString()),
              datasets: [{
                label: 'Commits',
                data: recentStats.map(w => w.total),
                backgroundColor: '#3b82f6',
                hoverBackgroundColor: '#2563eb'
              }]
            }
          }
        }
      } catch (e) {
        console.error(`Failed to load stats for ${project.title}`, e)
      }
    }
  }
}

onMounted(() => {
  fetchProjects()
})
</script>
