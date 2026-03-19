<template>
  <div class="side-projects-page">
    <div class="side-projects-background" aria-hidden="true">
      <div class="side-projects-blob side-projects-blob--blue"></div>
      <div class="side-projects-blob side-projects-blob--violet"></div>
      <div class="side-projects-grid"></div>
    </div>

    <div class="side-projects-shell">
      <section class="side-projects-hero">
        <div class="side-projects-hero-copy">
          <p class="side-projects-kicker">Independent Works</p>
          <h1 class="side-projects-title">副业项目展示</h1>
          <p class="side-projects-subtitle">
            展示部分独立完成的商业化与定制化项目，覆盖 Web、移动端、AI 工具和企业内部应用等不同场景。
          </p>
        </div>

        <div v-if="!loading && stats" class="side-projects-stats">
          <div class="side-projects-stat-card">
            <span class="side-projects-stat-label">项目总数</span>
            <strong class="side-projects-stat-value">{{ stats.totalProjects }}</strong>
          </div>
          <div class="side-projects-stat-card">
            <span class="side-projects-stat-label">项目类型</span>
            <strong class="side-projects-stat-value">{{ stats.categoryStats?.length || 0 }}</strong>
          </div>
          <div class="side-projects-stat-card">
            <span class="side-projects-stat-label">技术栈</span>
            <strong class="side-projects-stat-value">{{ stats.techStats?.length || 0 }}</strong>
          </div>
        </div>
      </section>

      <section v-if="!loading && stats" class="side-projects-insights">
        <article v-if="stats.categoryStats?.length" class="side-projects-panel">
          <div class="side-projects-panel-head">
            <p class="side-projects-panel-kicker">Categories</p>
            <h2 class="side-projects-panel-title">项目类型分布</h2>
          </div>

          <div class="side-projects-distribution-list">
            <div
              v-for="item in stats.categoryStats"
              :key="item.category"
              class="side-projects-distribution-item"
            >
              <div class="side-projects-distribution-copy">
                <span class="side-projects-distribution-label">{{ item.category }}</span>
                <span class="side-projects-distribution-count">{{ item.count }}</span>
              </div>
              <div class="side-projects-distribution-bar">
                <div
                  class="side-projects-distribution-fill"
                  :style="{ width: `${(item.count / getMaxCategoryCount) * 100}%` }"
                ></div>
              </div>
            </div>
          </div>
        </article>

        <article v-if="stats.techStats?.length" class="side-projects-panel">
          <div class="side-projects-panel-head">
            <p class="side-projects-panel-kicker">Technology</p>
            <h2 class="side-projects-panel-title">技术栈使用情况</h2>
          </div>

          <div class="side-projects-chip-list">
            <span
              v-for="item in topTechStats"
              :key="item.tech"
              class="side-projects-chip"
            >
              {{ item.tech }} · {{ item.count }}
            </span>
          </div>
        </article>
      </section>

      <section class="side-projects-listing">
        <div class="side-projects-listing-head">
          <div>
            <p class="side-projects-panel-kicker">Project List</p>
            <h2 class="side-projects-panel-title">项目列表</h2>
          </div>
          <p class="side-projects-listing-text">以下是已经整理并公开展示的部分副业项目。</p>
        </div>

        <div v-if="loading" class="side-projects-state">
          <div class="side-projects-loading-spinner"></div>
          <p class="side-projects-state-title">正在整理项目资料</p>
          <p class="side-projects-state-text">马上就好，请稍候片刻。</p>
        </div>

        <div v-else-if="error" class="side-projects-state side-projects-state--error">
          <div class="side-projects-empty-icon">!</div>
          <p class="side-projects-state-title">加载失败</p>
          <p class="side-projects-state-text">{{ error }}</p>
        </div>

        <div v-else-if="projects.length === 0" class="side-projects-state">
          <div class="side-projects-empty-icon">📁</div>
          <p class="side-projects-state-title">暂无项目</p>
          <p class="side-projects-state-text">当前还没有公开展示的副业项目。</p>
        </div>

        <div v-else class="side-projects-grid-list">
          <article
            v-for="project in projects"
            :key="project.id"
            class="side-projects-card"
            @click="handleProjectClick(project.id)"
          >
            <div class="side-projects-card-head">
              <h3 class="side-projects-card-title">{{ project.title }}</h3>
              <span class="side-projects-status" :class="getStatusClass(project.status)">
                {{ getStatusText(project.status) }}
              </span>
            </div>

            <p v-if="project.description" class="side-projects-card-description">
              {{ truncateText(project.description, 120) }}
            </p>

            <div class="side-projects-card-meta">
              <span v-if="project.category" class="side-projects-meta-item">
                <i class="fas fa-tag"></i>
                {{ project.category }}
              </span>
              <span v-if="project.clientName" class="side-projects-meta-item">
                <i class="fas fa-user"></i>
                {{ project.clientName }}
              </span>
              <span v-if="project.source" class="side-projects-meta-item">
                <i class="fas fa-link"></i>
                {{ project.source }}
              </span>
            </div>

            <div v-if="project.techStack" class="side-projects-tech-list">
              <span
                v-for="tech in parseTechStack(project.techStack).slice(0, 6)"
                :key="tech"
                class="side-projects-tech-tag"
              >
                {{ tech }}
              </span>
            </div>

            <div class="side-projects-card-footer">
              <span class="side-projects-time" v-if="project.endTime">
                <i class="fas fa-calendar"></i>
                {{ formatDate(project.endTime) }}
              </span>
              <span class="side-projects-link">查看详情 →</span>
            </div>
          </article>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '~/composables/useApi'

definePageMeta({
  layout: 'default'
})

useHead({
  title: '副业项目展示 - 溪午听风',
  meta: [
    { name: 'description', content: '展示部分独立完成的副业项目与技术方案，涵盖多领域的真实实践经验。' }
  ]
})

const router = useRouter()
const api = useApi()

const loading = ref(false)
const error = ref<string | null>(null)
const projects = ref<any[]>([])
const stats = ref<any>(null)

const fetchStats = async () => {
  try {
    const res = await api.get<any>('/side-projects/public/dashboard/summary')
    if (res && typeof res === 'object') {
      stats.value = {
        totalProjects: res.totalProjects ?? res.TotalProjects ?? 0,
        categoryStats: res.categoryStats ?? res.CategoryStats ?? [],
        techStats: res.techStats ?? res.TechStats ?? []
      }
    }
  } catch (e) {
    console.error('获取副业项目统计失败', e)
  }
}

const fetchProjects = async () => {
  loading.value = true
  error.value = null

  try {
    const res = await api.get<any>('/side-projects/public', {
      params: { page: 1, pageSize: 50 }
    })

    if (res && typeof res === 'object') {
      const list = res.list ?? res.List ?? []
      projects.value = Array.isArray(list) ? list : []
    } else {
      projects.value = []
    }
  } catch (e: any) {
    error.value = e.message || '加载项目列表失败'
    projects.value = []
  } finally {
    loading.value = false
  }
}

const getMaxCategoryCount = computed(() => {
  if (!stats.value?.categoryStats?.length) return 1
  return Math.max(...stats.value.categoryStats.map((item: any) => item.count))
})

const getMaxTechCount = computed(() => {
  if (!stats.value?.techStats?.length) return 1
  return Math.max(...stats.value.techStats.map((item: any) => item.count))
})

const topTechStats = computed(() => {
  if (!stats.value?.techStats?.length) return []
  return [...stats.value.techStats]
    .sort((a, b) => b.count - a.count)
    .slice(0, 10)
})

const cleanTechTag = (value: string) => value
  .replace(/^[\[\(\{'"`\s]+/, '')
  .replace(/[\]\)\}'"`\s]+$/, '')
  .replace(/^["']+|["']+$/g, '')
  .replace(/^["']|["']$/g, '')
  .trim()

const parseTechStack = (techStack: string | null | undefined): string[] => {
  if (!techStack) return []
  return techStack.split(',').map(item => cleanTechTag(item)).filter(Boolean)
}

const truncateText = (text: string, maxLength: number): string => {
  if (!text) return ''
  if (text.length <= maxLength) return text
  return `${text.substring(0, maxLength)}...`
}

const getStatusText = (status: number): string => {
  switch (status) {
    case 0: return '进行中'
    case 1: return '已完成'
    case 2: return '待付款'
    case 3: return '已取消'
    default: return '未知'
  }
}

const getStatusClass = (status: number): string => {
  switch (status) {
    case 0: return 'side-projects-status--info'
    case 1: return 'side-projects-status--success'
    case 2: return 'side-projects-status--warning'
    case 3: return 'side-projects-status--muted'
    default: return ''
  }
}

const formatDate = (dateStr: string | null | undefined): string => {
  if (!dateStr) return '-'

  try {
    return new Date(dateStr).toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })
  } catch {
    return dateStr
  }
}

const handleProjectClick = (id: number) => {
  router.push(`/side-projects/${id}`)
}

onMounted(() => {
  fetchStats()
  fetchProjects()
})
</script>
