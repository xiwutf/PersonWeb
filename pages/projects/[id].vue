<template>
  <div class="projects-detail-page">
    <div class="projects-detail-background">
      <div class="projects-detail-blob projects-detail-blob--blue"></div>
      <div class="projects-detail-blob projects-detail-blob--violet"></div>
      <div class="projects-detail-grid"></div>
    </div>

    <div class="projects-detail-shell">
      <div class="projects-detail-topbar">
        <NuxtLink to="/projects" class="projects-detail-back">
          <span class="projects-detail-back-icon">
            <i class="fas fa-arrow-left"></i>
          </span>
          <span class="projects-detail-back-text">
            返回项目展示
          </span>
        </NuxtLink>
      </div>

      <section v-if="loading" class="projects-detail-state">
        <div class="projects-loading-spinner"></div>
        <p class="projects-state-title">项目详情加载中</p>
        <p class="projects-state-text">正在整理项目资料、技术栈与正文内容，请稍候。</p>
      </section>

      <section v-else-if="project" class="projects-detail-layout">
        <div class="projects-detail-main">
          <header class="projects-detail-hero">
            <div
              class="projects-detail-cover"
              :class="getProjectToneClass(project)"
            >
              <img
                v-if="project.coverUrl"
                :src="project.coverUrl"
                :alt="project.title"
                class="projects-detail-cover-image"
              />

              <div class="projects-detail-cover-overlay"></div>

              <div class="projects-detail-hero-content">
                <div class="projects-detail-meta-row">
                  <span class="projects-card-status" :class="getStatusClass(project.status)">
                    {{ getStatusText(project.status) }}
                  </span>
                  <span class="projects-detail-meta-pill">
                    <i class="fas fa-eye"></i>
                    {{ project.viewCount || 0 }} 次浏览
                  </span>
                </div>

                <h1 class="projects-detail-title">{{ project.title }}</h1>
                <p class="projects-detail-description">
                  {{ project.description || '这个项目正在持续完善中，后续会补充更完整的介绍与演示说明。' }}
                </p>

                <div class="projects-detail-actions">
                  <a
                    v-if="project.demoUrl"
                    :href="project.demoUrl"
                    target="_blank"
                    rel="noreferrer"
                    class="projects-detail-button projects-detail-button--primary"
                  >
                    <i class="fas fa-external-link-alt"></i>
                    在线体验
                  </a>
                  <a
                    v-if="project.githubUrl"
                    :href="project.githubUrl"
                    target="_blank"
                    rel="noreferrer"
                    class="projects-detail-button projects-detail-button--secondary"
                  >
                    <i class="fab fa-github"></i>
                    查看源码
                  </a>
                </div>
              </div>
            </div>
          </header>

          <article v-if="projectBodyHtml" class="projects-detail-content-card">
            <div class="projects-detail-section-head">
              <div>
                <p class="projects-detail-section-kicker">Project Notes</p>
                <h2 class="projects-detail-section-title">项目说明</h2>
              </div>
            </div>
            <div
              class="projects-detail-content prose prose-lg max-w-none"
              v-html="projectBodyHtml"
            ></div>
          </article>

          <article v-else class="projects-detail-content-card">
            <div class="projects-detail-section-head">
              <div>
                <p class="projects-detail-section-kicker">Overview</p>
                <h2 class="projects-detail-section-title">项目概览</h2>
              </div>
            </div>
            <p class="projects-detail-empty-copy">
              当前项目还没有补充完整正文内容，后续会继续整理背景、功能结构与实现思路。
            </p>
          </article>
        </div>

        <aside class="projects-detail-sidebar">
          <section class="projects-detail-panel">
            <div class="projects-detail-panel-head">
              <p class="projects-detail-panel-kicker">Snapshot</p>
              <h2 class="projects-detail-panel-title">项目信息</h2>
            </div>

            <div class="projects-detail-facts">
              <div class="projects-detail-fact">
                <span class="projects-detail-fact-label">当前状态</span>
                <span class="projects-detail-fact-value">{{ getStatusText(project.status) }}</span>
              </div>
              <div class="projects-detail-fact">
                <span class="projects-detail-fact-label">访问热度</span>
                <span class="projects-detail-fact-value">{{ project.viewCount || 0 }} 次</span>
              </div>
              <div class="projects-detail-fact">
                <span class="projects-detail-fact-label">演示地址</span>
                <span class="projects-detail-fact-value">{{ project.demoUrl ? '可体验' : '暂无' }}</span>
              </div>
              <div class="projects-detail-fact">
                <span class="projects-detail-fact-label">源码仓库</span>
                <span class="projects-detail-fact-value">{{ project.githubUrl ? '已开放' : '未公开' }}</span>
              </div>
            </div>
          </section>

          <section v-if="techStackArray.length > 0" class="projects-detail-panel">
            <div class="projects-detail-panel-head">
              <p class="projects-detail-panel-kicker">Stack</p>
              <h2 class="projects-detail-panel-title">技术栈</h2>
            </div>
            <div class="projects-card-tech-stack projects-detail-tech-stack">
              <span
                v-for="tech in techStackArray"
                :key="tech"
                class="projects-tech-tag"
                :class="getTechTagClass(tech)"
              >
                <span class="projects-tech-tag-icon">{{ getTechIcon(tech) }}</span>
                {{ tech }}
              </span>
            </div>
          </section>
        </aside>
      </section>

      <section v-else class="projects-detail-state projects-detail-state--error">
        <div class="projects-empty-icon">!</div>
        <p class="projects-state-title">没有找到这个项目</p>
        <p class="projects-state-text">项目可能已被删除、隐藏，或者当前访问链接不正确。</p>
        <NuxtLink to="/projects" class="projects-detail-button projects-detail-button--primary">
          返回项目列表
        </NuxtLink>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '~/types/api'
import { resolveProjectBodyHtml } from '~/composables/useProjectContent'
import '~/assets/css/projects.css'

definePageMeta({
  layout: 'default'
})

const route = useRoute()
const api = useApi()
usePageStyle('projects')

const project = ref<Project | null>(null)
const loading = ref(true)
const projectBodyHtml = ref('')

const cleanTechTag = (value: string) => value
  .replace(/^[\[\(\{'"`\s]+/, '')
  .replace(/[\]\)\}'"`\s]+$/, '')
  .replace(/^["']+|["']+$/g, '')
  .replace(/^["']|["']$/g, '')
  .trim()

const techStackArray = computed(() => {
  const techStack = project.value?.techStack
  if (!techStack) return []

  if (Array.isArray(techStack)) {
    return techStack
      .filter((item): item is string => typeof item === 'string')
      .map(item => cleanTechTag(item))
      .filter(Boolean)
  }

  if (typeof techStack === 'string') {
    const trimmed = techStack.trim()
    if (trimmed.startsWith('[') && trimmed.endsWith(']')) {
      try {
        const parsed = JSON.parse(trimmed) as unknown[]
        if (Array.isArray(parsed)) {
          return parsed
            .filter((item): item is string => typeof item === 'string')
            .map(item => cleanTechTag(item))
            .filter(Boolean)
        }
      } catch {
        // 解析失败则按逗号分割
      }
    }
    return techStack.split(',').map(item => cleanTechTag(item)).filter(Boolean)
  }

  return []
})

const getTechTagClass = (tech: string) => {
  const value = tech.toLowerCase()

  if (value.includes('vue') || value.includes('react') || value.includes('angular') || value.includes('nuxt') || value.includes('next')) {
    return 'projects-tech-tag--vue'
  }
  if (value.includes('javascript') || value.includes('typescript') || value.includes(' js') || value.includes(' ts')) {
    return 'projects-tech-tag--js'
  }
  if (value.includes('python')) {
    return 'projects-tech-tag--python'
  }
  if (value.includes('node') || value.includes('express')) {
    return 'projects-tech-tag--node'
  }
  if (value.includes('mysql') || value.includes('postgresql') || value.includes('mongodb') || value.includes('redis')) {
    return 'projects-tech-tag--database'
  }
  if (value.includes('spring') || value.includes('fastapi') || value.includes('django') || value.includes('flask') || value.includes('.net')) {
    return 'projects-tech-tag--framework'
  }
  if (value.includes('小程序') || value.includes('wechat') || value.includes('miniprogram')) {
    return 'projects-tech-tag--miniprogram'
  }
  if (value.includes('ai') || value.includes('ml') || value.includes('langchain') || value.includes('openai')) {
    return 'projects-tech-tag--ai'
  }

  return 'projects-tech-tag--default'
}

const getTechIcon = (tech: string) => {
  const value = tech.toLowerCase()

  if (value.includes('vue')) return 'V'
  if (value.includes('react')) return 'R'
  if (value.includes('python')) return 'Py'
  if (value.includes('node')) return 'N'
  if (value.includes('mysql')) return 'DB'
  if (value.includes('redis')) return 'Re'
  if (value.includes('小程序')) return 'MP'
  if (value.includes('ai') || value.includes('langchain')) return 'AI'
  if (value.includes('spring')) return 'Sp'
  if (value.includes('docker')) return 'Dc'
  if (value.includes('git')) return 'Gt'

  return '•'
}

const getStatusText = (status: string | undefined) => {
  const value = (status || '').toLowerCase()

  if (!value) return '进行中'
  if (value.includes('active') || value.includes('running') || value.includes('online') || value.includes('维护')) return '持续维护'
  if (value.includes('beta') || value.includes('测试')) return '测试阶段'
  if (value.includes('done') || value.includes('complete') || value.includes('finished')) return '已完成'
  if (value.includes('plan') || value.includes('draft')) return '规划中'

  return status || '进行中'
}

const getStatusClass = (status: string | undefined) => {
  const value = (status || '').toLowerCase()

  if (value.includes('active') || value.includes('running') || value.includes('online') || value.includes('维护')) {
    return 'projects-card-status--active'
  }
  if (value.includes('beta') || value.includes('测试')) {
    return 'projects-card-status--beta'
  }
  if (value.includes('done') || value.includes('complete') || value.includes('finished')) {
    return 'projects-card-status--done'
  }

  return 'projects-card-status--default'
}

const getProjectToneClass = (currentProject: Project) => {
  const tech = techStackArray.value.join(' ').toLowerCase()
  const title = (currentProject.title || '').toLowerCase()

  if (tech.includes('ai') || title.includes('ai')) return 'projects-card-cover--ai'
  if (tech.includes('vue') || tech.includes('react') || tech.includes('nuxt')) return 'projects-card-cover--web'
  if (tech.includes('.net') || tech.includes('mysql') || tech.includes('redis')) return 'projects-card-cover--data'

  return 'projects-card-cover--default'
}

const fetchProject = async () => {
  loading.value = true
  try {
    const projectId = route.params.id as string

    api.post(`/Projects/${projectId}/view`).catch(() => {})

    const response = await api.get<Project>(`/Projects/${projectId}`)
    project.value = response

    projectBodyHtml.value = resolveProjectBodyHtml({
      contentHtml: response.contentHtml,
      content: response.content
    })
  } catch (error) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch project:', error)
    }
    project.value = null
    projectBodyHtml.value = ''
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchProject()
})

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
.projects-detail-content :deep(h2),
.projects-detail-content :deep(h3),
.projects-detail-content :deep(h4) {
  color: var(--color-text);
  letter-spacing: -0.02em;
}

.projects-detail-content :deep(p),
.projects-detail-content :deep(li) {
  color: rgba(226, 232, 240, 0.82);
  line-height: 1.9;
}

.projects-detail-content :deep(a) {
  color: #93c5fd;
}

.projects-detail-content :deep(blockquote) {
  border-left: 3px solid rgba(96, 165, 250, 0.5);
  background: rgba(15, 23, 42, 0.46);
  color: rgba(226, 232, 240, 0.78);
}

.projects-detail-content :deep(pre) {
  overflow-x: auto;
  border: 1px solid var(--color-border);
  border-radius: 18px;
  background: rgba(2, 6, 23, 0.88);
  color: var(--color-text-muted);
  box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.04);
}

.projects-detail-content :deep(code) {
  border-radius: 8px;
  background: rgba(30, 41, 59, 0.86);
  color: var(--color-text);
  padding: 0.15rem 0.4rem;
}

.projects-detail-content :deep(pre code) {
  background: transparent;
  padding: 0;
}
</style>
