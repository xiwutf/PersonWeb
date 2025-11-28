<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 to-white py-12">
    <div class="container mx-auto px-4 max-w-5xl">
      <!-- 返回按钮 -->
      <div class="mb-6">
        <NuxtLink
          to="/projects"
          class="inline-flex items-center px-4 py-2 bg-white rounded-lg shadow-sm hover:shadow-md transition-all text-slate-700 hover:text-slate-900 border border-slate-200"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          返回项目列表
        </NuxtLink>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="text-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-slate-600">加载中...</p>
      </div>

      <!-- 项目详情 -->
      <div v-else-if="project" class="space-y-8">
        <!-- 项目头部 -->
        <div class="bg-white rounded-2xl shadow-lg overflow-hidden">
          <div class="h-64 bg-gradient-to-br from-blue-500 to-purple-600 relative overflow-hidden">
            <img
              v-if="project.coverUrl"
              :src="project.coverUrl"
              :alt="project.title"
              class="w-full h-full object-cover"
            />
            <div class="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent"></div>
            <div class="absolute bottom-0 left-0 right-0 p-8 text-white">
              <div class="flex items-center gap-3 mb-3">
                <span
                  class="px-3 py-1 rounded-full text-sm font-medium backdrop-blur-sm bg-white/20"
                  :class="{
                    'bg-green-500/80': project.status === 'Active',
                    'bg-gray-500/80': project.status === 'Completed',
                    'bg-slate-500/80': project.status === 'Archived'
                  }"
                >
                  {{ project.status === 'Active' ? '进行中' : project.status === 'Completed' ? '已完成' : '已归档' }}
                </span>
                <span class="text-sm text-white/80 flex items-center gap-1">
                  <i class="fas fa-eye"></i>
                  {{ project.viewCount || 0 }} 次浏览
                </span>
              </div>
              <h1 class="text-4xl font-bold mb-2">{{ project.title }}</h1>
              <p class="text-lg text-white/90">{{ project.description }}</p>
            </div>
          </div>

          <div class="p-8">
            <!-- 技术栈 -->
            <div v-if="techStackArray.length > 0" class="mb-6">
              <h3 class="text-sm font-medium text-slate-700 mb-3">技术栈</h3>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="tech in techStackArray"
                  :key="tech"
                  class="inline-flex items-center px-3 py-1.5 rounded-full text-sm font-medium transition-all duration-200 hover:scale-105 shadow-sm hover:shadow-md"
                  :class="getTechTagClass(tech)"
                >
                  <span class="mr-1.5">{{ getTechIcon(tech) }}</span>
                  {{ tech }}
                </span>
              </div>
            </div>

            <!-- 链接按钮 -->
            <div class="flex flex-wrap gap-4">
              <a
                v-if="project.demoUrl"
                :href="project.demoUrl"
                target="_blank"
                class="bg-blue-600 text-white px-6 py-3 rounded-xl hover:bg-blue-700 transition-colors font-medium inline-flex items-center gap-2 shadow-lg shadow-blue-500/30"
              >
                <i class="fas fa-external-link-alt"></i>
                在线体验
              </a>
              <a
                v-if="project.githubUrl"
                :href="project.githubUrl"
                target="_blank"
                class="border-2 border-slate-600 text-slate-700 px-6 py-3 rounded-xl hover:bg-slate-50 transition-colors font-medium inline-flex items-center gap-2"
              >
                <i class="fab fa-github"></i>
                查看源码
              </a>
            </div>
          </div>
        </div>

        <!-- 项目内容 -->
        <div v-if="project.content" class="bg-white rounded-2xl shadow-lg p-8">
          <div class="prose prose-lg max-w-none">
            <ContentRenderer :value="markdownContent" />
          </div>
        </div>
      </div>

      <!-- 错误状态 -->
      <div v-else class="bg-white rounded-2xl shadow-lg p-8 text-center">
        <div class="text-red-600 mb-4">
          <i class="fas fa-exclamation-triangle text-4xl"></i>
        </div>
        <h2 class="text-2xl font-bold text-slate-900 mb-2">项目不存在</h2>
        <p class="text-slate-600 mb-6">未找到该项目，可能已被删除或 ID 不正确。</p>
        <NuxtLink
          to="/projects"
          class="inline-flex items-center px-6 py-3 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition-colors"
        >
          返回项目列表
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '~/types/api'
import { useMarkdown } from '~/composables/useMarkdown'

const route = useRoute()
const api = useApi()

const project = ref<Project | null>(null)
const loading = ref(true)
const markdownContent = ref<string>('')

// 解析技术栈
const techStackArray = computed(() => {
  if (!project.value?.techStack) return []
  if (Array.isArray(project.value.techStack)) {
    return project.value.techStack
  }
  if (typeof project.value.techStack === 'string') {
    try {
      const parsed = JSON.parse(project.value.techStack)
      return Array.isArray(parsed) ? parsed : project.value.techStack.split(',').map(t => t.trim())
    } catch {
      return project.value.techStack.split(',').map(t => t.trim())
    }
  }
  return []
})

// 获取技术栈标签样式
const getTechTagClass = (tech: string) => {
  const techLower = tech.toLowerCase()
  
  // 前端技术
  if (techLower.includes('vue') || techLower.includes('react') || techLower.includes('angular') || techLower.includes('nuxt') || techLower.includes('next')) {
    return 'bg-gradient-to-r from-blue-500 to-blue-600 text-white'
  }
  // JavaScript/TypeScript
  if (techLower.includes('javascript') || techLower.includes('typescript') || techLower.includes('js') || techLower.includes('ts')) {
    return 'bg-gradient-to-r from-yellow-400 to-yellow-500 text-gray-900'
  }
  // Python
  if (techLower.includes('python')) {
    return 'bg-gradient-to-r from-blue-400 to-blue-500 text-white'
  }
  // Node.js
  if (techLower.includes('node') || techLower.includes('express')) {
    return 'bg-gradient-to-r from-green-500 to-green-600 text-white'
  }
  // 数据库
  if (techLower.includes('mysql') || techLower.includes('postgresql') || techLower.includes('mongodb') || techLower.includes('redis')) {
    return 'bg-gradient-to-r from-orange-500 to-orange-600 text-white'
  }
  // 框架
  if (techLower.includes('spring') || techLower.includes('fastapi') || techLower.includes('django') || techLower.includes('flask')) {
    return 'bg-gradient-to-r from-emerald-500 to-emerald-600 text-white'
  }
  // 小程序
  if (techLower.includes('小程序') || techLower.includes('wechat') || techLower.includes('miniprogram')) {
    return 'bg-gradient-to-r from-green-400 to-green-500 text-white'
  }
  // AI/ML
  if (techLower.includes('ai') || techLower.includes('ml') || techLower.includes('langchain') || techLower.includes('openai')) {
    return 'bg-gradient-to-r from-purple-500 to-purple-600 text-white'
  }
  // 默认样式
  return 'bg-gradient-to-r from-gray-500 to-gray-600 text-white'
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

// 获取项目详情
const fetchProject = async () => {
  loading.value = true
  try {
    const projectId = route.params.id as string
    
    // 记录访问（异步，不阻塞页面加载）
    api.post(`/Projects/${projectId}/view`).catch(() => {
      // 静默失败
    })

    // 获取项目详情
    const res = await api.get<Project>(`/Projects/${projectId}`)
    project.value = res

    // 解析 Markdown 内容
    if (res.content) {
      const { parse } = useMarkdown()
      markdownContent.value = parse(res.content)
    }
  } catch (e: unknown) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch project:', e)
    }
    project.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchProject()
})

// 设置页面标题
useHead({
  title: computed(() => `${project.value?.title || '项目详情'} - 项目展示 - 溪午听风`),
  meta: [
    {
      name: 'description',
      content: computed(() => project.value?.description || '项目详情页面')
    }
  ]
})
</script>

<style scoped>
:deep(.prose pre) {
  @apply bg-slate-900 text-slate-100;
  border-radius: 12px;
  padding: 1.5rem;
  overflow-x: auto;
}

:deep(.prose code) {
  @apply bg-slate-100 text-slate-800 px-1 py-0.5 rounded text-sm;
}
</style>

