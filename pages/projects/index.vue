<template>
  <div class="projects-page">
    <!-- 3D ????????????????-->
    <div class="projects-view-toggle-container">
      <button
        v-if="viewMode === 'grid'"
        @click.stop.prevent="handleToggle3DView"
        class="projects-view-toggle-button"
        title="????D??"
      >
        <span class="projects-view-toggle-icon">??</span>
        <span class="projects-view-toggle-text">3D??</span>
      </button>
    </div>
    
    <!-- 3D ???? -->
    <Project3DSpace 
      v-if="viewMode === '3d' && !loading && projects.length > 0" 
      :projects="projects"
      @back-to-list="viewMode = 'grid'"
    />
    
    <!-- ?????? -->
    <div v-else class="projects-container">
      <div class="projects-header">
        <h1 class="projects-title">????</h1>
        <p class="projects-subtitle">?????????????</p>
      </div>

      <!-- ???? Tab -->
      <div class="projects-filters">
        <button
          v-for="category in projectCategories"
          :key="category"
          @click="selectedCategory = category"
          class="projects-filter-tab"
          :class="{ 'projects-filter-tab--active': selectedCategory === category }"
        >
          {{ category }}
        </button>
      </div>

      <div v-if="loading" class="projects-loading">
        <div class="projects-loading-spinner"></div>
      </div>

      <!-- ???? -->
      <div v-if="error" class="projects-error">
        <p class="projects-error-title">????</p>
        <p class="projects-error-message">{{ error }}</p>
        <p v-if="debugData" class="projects-error-debug">{{ debugData }}</p>
      </div>
      
      <!-- ??????-->
      <div v-if="projects.length === 0 && !loading && !error" class="projects-empty">
        <div class="projects-empty-icon">??</div>
        <h3 class="projects-empty-title">????</h3>
        <p class="projects-empty-text">??????????????????????</p>
      </div>

      <div v-else class="projects-grid">
        <NuxtLink 
          v-for="project in filteredProjects" 
          :key="project.id" 
          :to="getProjectLink(project)" 
          class="projects-card"
        >
          <!-- ????-->
          <div class="projects-card-cover">
            <img 
              v-if="project.coverUrl && !imageLoadErrors[project.id]" 
              :src="project.coverUrl" 
              :alt="project.title"
              @error="handleImageError(project.id)"
              @load="handleImageLoad(project.id)"
              style="display: block;"
            />
            <div 
              v-if="!project.coverUrl || imageLoadErrors[project.id]"
              class="projects-card-cover-placeholder"
              :class="getPlaceholderClass(project)"
            >
              <div class="projects-card-cover-placeholder-icon">
                {{ getPlaceholderIcon(project) }}
              </div>
              <div class="projects-card-cover-placeholder-text">
                {{ project.title }}
              </div>
            </div>
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

          <!-- ?? -->
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

            <!-- ??? -->
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
import Project3DSpace from '~/components/three/Project3DSpace.vue'

// ???? default ????? Header?
definePageMeta({
  layout: 'default'
})

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

// ?? Chart.js ??
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const api = useApi()
const projects = ref<Project[]>([])
const loading = ref(true)
const error = ref('')
const debugData = ref('')
const viewMode = ref<'grid' | '3d'>('grid')
const imageLoadErrors = ref<Record<number | string, boolean>>({})

// ????
const projectCategories = ref(['??', 'AI??', 'Web ??', '??', '??'])
const selectedCategory = ref('??')

// ????? AI/??? ????
const isAiProject = (project: Project) => {
  const title = (project.title || '').toLowerCase()
  const description = (project.description || '').toLowerCase()
  const techStack = Array.isArray(project.techStack) 
    ? project.techStack.join(' ').toLowerCase()
    : (project.techStack || '').toLowerCase()
  
  const aiKeywords = ['ai', '??', '???', 'agent', 'rag', 'llm', 'gpt', 'openai', 'langchain', '????', '????', 'ml']
  const text = `${title} ${description} ${techStack}`
  
  return aiKeywords.some(keyword => text.includes(keyword))
}

// ????? Web ??
const isWebProject = (project: Project) => {
  const title = (project.title || '').toLowerCase()
  const description = (project.description || '').toLowerCase()
  const techStack = Array.isArray(project.techStack) 
    ? project.techStack.join(' ').toLowerCase()
    : (project.techStack || '').toLowerCase()
  
  const webKeywords = ['web', '??', '??', 'vue', 'react', 'nuxt', 'next', '??', '??', '??']
  const text = `${title} ${description} ${techStack}`
  
  return webKeywords.some(keyword => text.includes(keyword)) && !isAiProject(project)
}

// ??????????
const isPluginProject = (project: Project) => {
  const title = (project.title || '').toLowerCase()
  const description = (project.description || '').toLowerCase()
  
  const pluginKeywords = ['??', 'plugin', 'extension', '??', 'tool', 'vscode', 'chrome', '???']
  const text = `${title} ${description}`
  
  return pluginKeywords.some(keyword => text.includes(keyword)) && !isAiProject(project) && !isWebProject(project)
}

// ??????
const filteredProjects = computed(() => {
  if (selectedCategory.value === '??') {
    return projects.value
  }
  
  if (selectedCategory.value === 'AI??') {
    return projects.value.filter(isAiProject)
  }
  
  if (selectedCategory.value === 'Web ??') {
    return projects.value.filter(isWebProject)
  }
  
  if (selectedCategory.value === '??') {
    return projects.value.filter(isPluginProject)
  }
  
  // ????
  return projects.value.filter(project => 
    !isAiProject(project) && 
    !isWebProject(project) && 
    !isPluginProject(project)
  )
})

// ??3D??????
const handleToggle3DView = () => {
  viewMode.value = '3d'
}

// ????
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
    
    // ?? API
    const res = await api.get<Project[]>('/Projects')
    debugData.value = JSON.stringify(res)
    
    // ????
    if (Array.isArray(res)) {
      // Process projects to match frontend expectations
      let processedProjects = res.map(p => ({
        ...p,
        // Handle TechStack: if string, split by comma; if array, keep it; else empty array
        techStack: typeof p.techStack === 'string' 
          ? p.techStack.split(',').map((t: string) => t.trim()).filter((t: string) => t) 
          : (Array.isArray(p.techStack) ? p.techStack : [])
      }))
      
      // ??????
      const titleMap = new Map<string, any>()
      processedProjects.forEach(project => {
        const title = (project.title || project.Title || '').trim()
        if (!title) return // ?????
        
        // ?????
        const normalizedTitle = title.toLowerCase().replace(/\s+/g, '')
        
        if (!titleMap.has(normalizedTitle)) {
          titleMap.set(normalizedTitle, project)
        } else {
          // ?????
          const existing = titleMap.get(normalizedTitle)!
          const existingDate = new Date(existing.createdAt || existing.CreatedAt || 0)
          const currentDate = new Date(project.createdAt || project.CreatedAt || 0)
          if (currentDate < existingDate) {
            titleMap.set(normalizedTitle, project)
          }
        }
      })
      
      // ??
      let uniqueProjects = Array.from(titleMap.values())
      
      // ????
      const finalProjects: any[] = []
      const seenTitles = new Set<string>()
      
      // ??????? -> ??????
      const duplicateMap: Record<string, string[]> = {
        'Revit': ['Revit', 'Revit v2'],
        'AI??': ['AI??'],
        'Google Analytics': ['Google Analytics']
      }
      
      uniqueProjects.forEach(project => {
        const title = (project.title || project.Title || '').trim()
        const normalizedTitle = title.toLowerCase().replace(/\s+/g, '')
        
        // ?????
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
        
        // ?????
        if (shouldExclude) return
        
        // ????
        finalProjects.push(project)
        seenTitles.add(normalizedTitle)
      })
      
      projects.value = finalProjects
      
      // ??????
      imageLoadErrors.value = {}
      
      // ?? GitHub ??
      loadGithubStats()
    } else {
      projects.value = []
      error.value = 'API ??????'
    }
  } catch (e: any) {
    console.error('Failed to fetch projects', e)
    error.value = e.message || 'Failed to load projects'
    projects.value = []
  } finally {
    loading.value = false
  }
}

// ???????
const getTechTagClass = (tech: string) => {
  const techLower = tech.toLowerCase()
  
  // ????
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
  // ???
  if (techLower.includes('mysql') || techLower.includes('postgresql') || techLower.includes('mongodb') || techLower.includes('redis')) {
    return 'projects-tech-tag--database'
  }
  // ??
  if (techLower.includes('spring') || techLower.includes('fastapi') || techLower.includes('django') || techLower.includes('flask')) {
    return 'projects-tech-tag--framework'
  }
  // ???
  if (techLower.includes('miniprogram') || techLower.includes('wechat')) {
    return 'projects-tech-tag--miniprogram'
  }
  // AI/ML
  if (techLower.includes('ai') || techLower.includes('ml') || techLower.includes('langchain') || techLower.includes('openai')) {
    return 'projects-tech-tag--ai'
  }
  // ????
  return 'projects-tech-tag--default'
}

// ?????
const getTechIcon = (tech: string) => {
  const techLower = tech.toLowerCase()
  
  if (techLower.includes('vue')) return 'i-mdi-vuejs'
  if (techLower.includes('react')) return 'i-mdi-react'
  if (techLower.includes('python')) return 'i-mdi-language-python'
  if (techLower.includes('node')) return 'i-mdi-nodejs'
  if (techLower.includes('mysql')) return 'i-mdi-database'
  if (techLower.includes('redis')) return 'i-mdi-database'
  if (techLower.includes('miniprogram') || techLower.includes('wechat')) return 'i-mdi-wechat'
  if (techLower.includes('ai') || techLower.includes('langchain')) return 'i-mdi-robot'
  if (techLower.includes('spring')) return 'i-mdi-leaf'
  if (techLower.includes('docker')) return 'i-mdi-docker'
  if (techLower.includes('kubernetes')) return 'i-mdi-kubernetes'
  if (techLower.includes('git')) return 'i-mdi-git'

  return 'i-mdi-code-tags'
}

// ??????
const getPlaceholderClass = (project: Project) => {
  if (isAiProject(project)) {
    return 'projects-card-cover-placeholder--ai'
  }
  if (isWebProject(project)) {
    return 'projects-card-cover-placeholder--web'
  }
  if (isPluginProject(project)) {
    return 'projects-card-cover-placeholder--plugin'
  }
  return 'projects-card-cover-placeholder--default'
}

// ????????????
const getPlaceholderIcon = (project: Project) => {
  if (isAiProject(project)) {
    return 'i-mdi-robot'
  }
  if (isWebProject(project)) {
    return 'i-mdi-web'
  }
  if (isPluginProject(project)) {
    return 'i-mdi-puzzle'
  }
  return 'i-mdi-folder'
}

// ??????????? Vue Router "No match found"
const getProjectLink = (project: Project) => {
  const id = project.id
  if (id == null || id === '') return '/projects'
  if (typeof id === 'number') return `/projects/${id}`
  const s = String(id)
  if (s.startsWith('/') || /\.(png|jpg|jpeg|gif|webp|svg)(\?|$)/i.test(s)) return '/projects'
  return `/projects/${id}`
}

// ??????
const handleImageError = (projectId: number | string) => {
  imageLoadErrors.value[projectId] = true
}

// ??????
const handleImageLoad = (projectId: number | string) => {
  imageLoadErrors.value[projectId] = false
}

const loadGithubStats = async () => {
  for (const project of projects.value) {
    if (project.githubUrl) {
      try {
        // ?? owner/repo
        const match = project.githubUrl.match(/github\.com\/([^\/]+\/[^\/]+)/)
        if (match) {
          const repo = match[1]
          const stats = await api.get<any[]>(`/github/stats?repo=${repo}`)
          
          if (stats && Array.isArray(stats)) {
            // stats ??????: { total: number, week: timestamp, days: [] }
            // ????26 ??(??)
            const recentStats = stats.slice(-26)
            
            project.chartData = {
              labels: recentStats.map(w => new Date(w.week * 1000).toLocaleDateString()),
              datasets: [{
                label: 'Commits',
                data: recentStats.map(w => w.total),
                backgroundColor: 'var(--color-primary)',
                hoverBackgroundColor: 'var(--color-primary-hover)'
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
/* ??????????assets/css/projects.css */
/* ??????????????????*/
</style>
