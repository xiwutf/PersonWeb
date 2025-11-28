<template>
  <div class="relative min-h-screen">
    <!-- 3D 旋转空间视图切换（移动端隐藏） -->
    <div class="fixed top-20 right-4 z-20 hidden md:block">
      <button
        @click="viewMode = viewMode === 'grid' ? '3d' : 'grid'"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 active:bg-blue-800 transition shadow-lg backdrop-blur-md touch-target"
      >
        {{ viewMode === 'grid' ? '🌐 3D视图' : '📋 列表视图' }}
      </button>
    </div>
    
    <!-- 3D 旋转空间 -->
    <Project3DSpace v-if="viewMode === '3d' && !loading && projects.length > 0" :projects="projects" />
    
    <!-- 传统网格视图 -->
    <div v-else class="container mx-auto px-4 py-12">
      <div class="text-center mb-16">
        <h1 class="text-4xl font-bold text-gray-900 dark:text-white mb-4">项目展示</h1>
        <p class="text-xl text-gray-600 dark:text-gray-400">探索我的开源项目和技术实验</p>
      </div>

      <div v-if="loading" class="text-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      </div>

      <!-- 错误提示 -->
      <div v-if="error" class="bg-red-100 dark:bg-red-900 text-red-700 dark:text-red-300 p-4 rounded mb-8">
        <p class="font-semibold">加载失败</p>
        <p class="text-sm mt-1">{{ error }}</p>
        <p v-if="debugData" class="text-xs mt-2 opacity-75">{{ debugData }}</p>
      </div>
      
      <!-- 无数据提示 -->
      <div v-if="projects.length === 0 && !loading && !error" class="text-center py-20">
        <div class="text-6xl mb-4">📦</div>
        <h3 class="text-xl font-semibold text-gray-700 dark:text-gray-300 mb-2">暂无项目</h3>
        <p class="text-gray-500 dark:text-gray-400">还没有添加任何项目，请先在后台管理中创建项目</p>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
      <NuxtLink v-for="project in projects" :key="project.id" :to="`/projects/${project.id}`" class="bg-white dark:bg-gray-800 rounded-xl shadow-lg overflow-hidden hover:shadow-xl transition-shadow duration-300 flex flex-col">
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
            <div class="flex items-center gap-2">
              <span class="px-2 py-1 text-xs font-semibold rounded bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">
                {{ project.status }}
              </span>
              <span class="px-2 py-1 text-xs text-gray-500 dark:text-gray-400 flex items-center gap-1">
                <i class="fas fa-eye"></i>
                {{ project.viewCount || 0 }}
              </span>
            </div>
          </div>
          
          <p class="text-gray-600 dark:text-gray-400 mb-4 flex-1">{{ project.description }}</p>

          <!-- 技术栈 -->
          <div class="flex flex-wrap gap-2 mb-6">
            <span 
              v-for="tech in project.techStack" 
              :key="tech" 
              class="inline-flex items-center px-3 py-1.5 text-xs font-medium rounded-full transition-all duration-200 hover:scale-105"
              :class="getTechTagClass(tech)"
            >
              <span class="mr-1.5">{{ getTechIcon(tech) }}</span>
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
      </NuxtLink>
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
import type { Project } from '~/types/api'

// 注册 Chart.js 组件
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const api = useApi()
const projects = ref<Project[]>([])
const loading = ref(true)
const error = ref('')
const debugData = ref('')
const viewMode = ref<'grid' | '3d'>('grid')

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
    loading.value = true
    error.value = ''
    
    // 调用后端 API
    const res = await api.get<Project[]>('/Projects')
    debugData.value = JSON.stringify(res)
    
    // 处理响应数据
    if (Array.isArray(res)) {
      // Process projects to match frontend expectations
      projects.value = res.map(p => ({
        ...p,
        // Handle TechStack: if string, split by comma; if array, keep it; else empty array
        techStack: typeof p.techStack === 'string' 
          ? p.techStack.split(',').map((t: string) => t.trim()).filter((t: string) => t) 
          : (Array.isArray(p.techStack) ? p.techStack : [])
      }))
      
      // 异步加载 GitHub 数据
      loadGithubStats()
    } else {
      projects.value = []
      error.value = 'API 返回格式错误'
    }
  } catch (e: any) {
    console.error('Failed to fetch projects', e)
    error.value = e.message || 'Failed to load projects'
    projects.value = []
  } finally {
    loading.value = false
  }
}

// 获取技术栈标签样式
const getTechTagClass = (tech: string) => {
  const techLower = tech.toLowerCase()
  
  // 前端技术
  if (techLower.includes('vue') || techLower.includes('react') || techLower.includes('angular') || techLower.includes('nuxt') || techLower.includes('next')) {
    return 'bg-gradient-to-r from-blue-500 to-blue-600 text-white shadow-md hover:shadow-lg'
  }
  // JavaScript/TypeScript
  if (techLower.includes('javascript') || techLower.includes('typescript') || techLower.includes('js') || techLower.includes('ts')) {
    return 'bg-gradient-to-r from-yellow-400 to-yellow-500 text-gray-900 shadow-md hover:shadow-lg'
  }
  // Python
  if (techLower.includes('python')) {
    return 'bg-gradient-to-r from-blue-400 to-blue-500 text-white shadow-md hover:shadow-lg'
  }
  // Node.js
  if (techLower.includes('node') || techLower.includes('express')) {
    return 'bg-gradient-to-r from-green-500 to-green-600 text-white shadow-md hover:shadow-lg'
  }
  // 数据库
  if (techLower.includes('mysql') || techLower.includes('postgresql') || techLower.includes('mongodb') || techLower.includes('redis')) {
    return 'bg-gradient-to-r from-orange-500 to-orange-600 text-white shadow-md hover:shadow-lg'
  }
  // 框架
  if (techLower.includes('spring') || techLower.includes('fastapi') || techLower.includes('django') || techLower.includes('flask')) {
    return 'bg-gradient-to-r from-emerald-500 to-emerald-600 text-white shadow-md hover:shadow-lg'
  }
  // 小程序
  if (techLower.includes('小程序') || techLower.includes('wechat') || techLower.includes('miniprogram')) {
    return 'bg-gradient-to-r from-green-400 to-green-500 text-white shadow-md hover:shadow-lg'
  }
  // AI/ML
  if (techLower.includes('ai') || techLower.includes('ml') || techLower.includes('langchain') || techLower.includes('openai')) {
    return 'bg-gradient-to-r from-purple-500 to-purple-600 text-white shadow-md hover:shadow-lg'
  }
  // 默认样式
  return 'bg-gradient-to-r from-gray-500 to-gray-600 text-white shadow-md hover:shadow-lg'
}

// 获取技术栈图标
const getTechIcon = (tech: string) => {
  const techLower = tech.toLowerCase()
  
  if (techLower.includes('vue')) return '⚡'
  if (techLower.includes('react')) return '⚛️'
  if (techLower.includes('python')) return '🐍'
  if (techLower.includes('node')) return '🟢'
  if (techLower.includes('mysql')) return '🗄️'
  if (techLower.includes('redis')) return '🔴'
  if (techLower.includes('小程序')) return '💬'
  if (techLower.includes('ai') || techLower.includes('langchain')) return '🤖'
  if (techLower.includes('spring')) return '🍃'
  if (techLower.includes('docker')) return '🐳'
  if (techLower.includes('kubernetes')) return '☸️'
  if (techLower.includes('git')) return '📦'
  
  return '💻'
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
