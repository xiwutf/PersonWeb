<template>
  <div class="projects-page">
    <!-- 3D 旋转空间视图切换（移动端隐藏） -->
    <div class="projects-view-toggle-container">
      <button
        v-if="viewMode === 'grid'"
        @click="viewMode = '3d'"
        class="projects-view-toggle-button"
        title="切换到3D视图"
      >
        <span class="projects-view-toggle-icon">🌐</span>
        <span class="projects-view-toggle-text">3D视图</span>
      </button>
    </div>
    
    <!-- 3D 旋转空间 -->
    <Project3DSpace 
      v-if="viewMode === '3d' && !loading && projects.length > 0" 
      :projects="projects"
      @back-to-list="viewMode = 'grid'"
    />
    
    <!-- 传统网格视图 -->
    <div v-else class="projects-container">
      <!-- 返回按钮 -->
      <div class="projects-back-button-container">
        <NuxtLink to="/lab" class="projects-back-button">
          <svg class="projects-back-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          <span>返回AI实验室</span>
        </NuxtLink>
      </div>

      <div class="projects-header">
        <h1 class="projects-title">项目展示</h1>
        <p class="projects-subtitle">探索我的开源项目和技术实验</p>
      </div>

      <div v-if="loading" class="projects-loading">
        <div class="projects-loading-spinner"></div>
      </div>

      <!-- 错误提示 -->
      <div v-if="error" class="projects-error">
        <p class="projects-error-title">加载失败</p>
        <p class="projects-error-message">{{ error }}</p>
        <p v-if="debugData" class="projects-error-debug">{{ debugData }}</p>
      </div>
      
      <!-- 无数据提示 -->
      <div v-if="projects.length === 0 && !loading && !error" class="projects-empty">
        <div class="projects-empty-icon">📦</div>
        <h3 class="projects-empty-title">暂无项目</h3>
        <p class="projects-empty-text">还没有添加任何项目，请先在后台管理中创建项目</p>
      </div>

      <div v-else class="projects-grid">
        <NuxtLink 
          v-for="project in projects" 
          :key="project.id" 
          :to="`/projects/${project.id}`" 
          class="projects-card"
        >
          <!-- 封面图 -->
          <div class="projects-card-cover">
            <img :src="project.coverUrl || 'https://placehold.co/600x400'" :alt="project.title" />
            <div class="projects-card-cover-overlay">
              <div class="projects-card-cover-actions">
                <a 
                  v-if="project.demoUrl" 
                  :href="project.demoUrl" 
                  target="_blank" 
                  class="projects-card-cover-button projects-card-cover-button--white"
                  @click.stop
                >
                  Live Demo
                </a>
                <a 
                  v-if="project.githubUrl" 
                  :href="project.githubUrl" 
                  target="_blank" 
                  class="projects-card-cover-button projects-card-cover-button--dark"
                  @click.stop
                >
                  GitHub
                </a>
              </div>
            </div>
          </div>

          <!-- 内容 -->
          <div class="projects-card-body">
            <div class="projects-card-header">
              <h3 class="projects-card-title">{{ project.title }}</h3>
              <div class="projects-card-meta">
                <span class="projects-card-badge projects-card-badge--blue">
                  {{ project.status }}
                </span>
                <span class="projects-card-view-count">
                  <i class="fas fa-eye"></i>
                  {{ project.viewCount || 0 }}
                </span>
              </div>
            </div>
            
            <p class="projects-card-description">{{ project.description }}</p>

            <!-- 技术栈 -->
            <div class="projects-card-tech-stack">
              <span 
                v-for="tech in project.techStack" 
                :key="tech" 
                class="projects-tech-tag"
                :class="getTechTagClass(tech)"
              >
                <span class="projects-tech-tag-icon">{{ getTechIcon(tech) }}</span>
                {{ tech }}
              </span>
            </div>

            <!-- GitHub Activity Chart -->
            <div v-if="project.githubUrl && project.chartData" class="projects-card-chart">
              <p class="projects-card-chart-label">GitHub Activity (Last Year)</p>
              <div class="projects-card-chart-container">
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

// 获取技术栈标签样式类名
const getTechTagClass = (tech: string) => {
  const techLower = tech.toLowerCase()
  
  // 前端技术
  if (techLower.includes('vue') || techLower.includes('react') || techLower.includes('angular') || techLower.includes('nuxt') || techLower.includes('next')) {
    return 'projects-tech-tag--vue'
  }
  // JavaScript/TypeScript
  if (techLower.includes('javascript') || techLower.includes('typescript') || techLower.includes('js') || techLower.includes('ts')) {
    return 'projects-tech-tag--js'
  }
  // Python
  if (techLower.includes('python')) {
    return 'projects-tech-tag--python'
  }
  // Node.js
  if (techLower.includes('node') || techLower.includes('express')) {
    return 'projects-tech-tag--node'
  }
  // 数据库
  if (techLower.includes('mysql') || techLower.includes('postgresql') || techLower.includes('mongodb') || techLower.includes('redis')) {
    return 'projects-tech-tag--database'
  }
  // 框架
  if (techLower.includes('spring') || techLower.includes('fastapi') || techLower.includes('django') || techLower.includes('flask')) {
    return 'projects-tech-tag--framework'
  }
  // 小程序
  if (techLower.includes('小程序') || techLower.includes('wechat') || techLower.includes('miniprogram')) {
    return 'projects-tech-tag--miniprogram'
  }
  // AI/ML
  if (techLower.includes('ai') || techLower.includes('ml') || techLower.includes('langchain') || techLower.includes('openai')) {
    return 'projects-tech-tag--ai'
  }
  // 默认样式
  return 'projects-tech-tag--default'
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

<style scoped>
/* 页面特有样式已移至 assets/css/projects.css */
/* 这里只保留组件特有的样式（如果有） */

.projects-back-button-container {
  margin-bottom: 2rem;
}

.projects-back-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(15, 23, 42, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: rgb(148, 163, 184);
  text-decoration: none;
  transition: all 0.3s ease;
  font-size: 0.875rem;
}

.projects-back-button:hover {
  background: rgba(15, 23, 42, 0.7);
  border-color: rgba(139, 92, 246, 0.3);
  color: white;
}

.projects-back-icon {
  width: 1.25rem;
  height: 1.25rem;
}
</style>
