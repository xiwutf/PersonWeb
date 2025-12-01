<template>
  <div class="projects-page">
    <!-- 3D 旋转空间视图切换（移动端隐藏） -->
    <div class="projects-view-toggle-container">
      <button
        v-if="viewMode === 'grid'"
        @click.stop.prevent="handleToggle3DView"
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

// 切换3D视图处理函数
const handleToggle3DView = () => {
  console.log('切换3D视图，当前模式:', viewMode.value)
  viewMode.value = '3d'
  console.log('切换后模式:', viewMode.value)
}

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
      let processedProjects = res.map(p => ({
        ...p,
        // Handle TechStack: if string, split by comma; if array, keep it; else empty array
        techStack: typeof p.techStack === 'string' 
          ? p.techStack.split(',').map((t: string) => t.trim()).filter((t: string) => t) 
          : (Array.isArray(p.techStack) ? p.techStack : [])
      }))
      
      // 去重：根据标题去重，保留最早创建的版本
      const titleMap = new Map<string, any>()
      processedProjects.forEach(project => {
        const title = (project.title || project.Title || '').trim()
        if (!title) return // 跳过无标题的项目
        
        // 标准化标题用于比较（去除空格、转换为小写）
        const normalizedTitle = title.toLowerCase().replace(/\s+/g, '')
        
        if (!titleMap.has(normalizedTitle)) {
          titleMap.set(normalizedTitle, project)
        } else {
          // 如果已存在，比较创建时间，保留更早的
          const existing = titleMap.get(normalizedTitle)!
          const existingDate = new Date(existing.createdAt || existing.CreatedAt || 0)
          const currentDate = new Date(project.createdAt || project.CreatedAt || 0)
          if (currentDate < existingDate) {
            titleMap.set(normalizedTitle, project)
          }
        }
      })
      
      // 转换为数组
      let uniqueProjects = Array.from(titleMap.values())
      
      // 进一步去重相似标题（处理已知的重复项）
      const finalProjects: any[] = []
      const seenTitles = new Set<string>()
      
      // 定义重复项映射（保留的标题 -> 要排除的标题）
      const duplicateMap: Record<string, string[]> = {
        '个人数字资产平台': ['个人网站系统', '个人网站v2'],
        'ai创作助手': ['ai智能助手'],
        '访客分析系统（analytics）': ['访客分析系统']
      }
      
      uniqueProjects.forEach(project => {
        const title = (project.title || project.Title || '').trim()
        const normalizedTitle = title.toLowerCase().replace(/\s+/g, '')
        
        // 检查是否应该被排除
        let shouldExclude = false
        for (const [keepTitle, excludeTitles] of Object.entries(duplicateMap)) {
          const keepNormalized = keepTitle.toLowerCase().replace(/\s+/g, '')
          const isExcluded = excludeTitles.some(exclude => 
            normalizedTitle.includes(exclude.toLowerCase().replace(/\s+/g, ''))
          )
          const isKept = seenTitles.has(keepNormalized) || normalizedTitle.includes(keepNormalized)
          
          if (isExcluded && isKept) {
            shouldExclude = true
            break
          }
        }
        
        // 如果应该排除，跳过
        if (shouldExclude) return
        
        // 添加到最终列表
        finalProjects.push(project)
        seenTitles.add(normalizedTitle)
      })
      
      projects.value = finalProjects
      
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
</style>
