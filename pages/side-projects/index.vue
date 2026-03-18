<template>
  <div class="side-projects-page">
    <!-- Hero 区域 -->
    <div class="hero-section">
      <div class="hero-content">
        <h1 class="hero-title">副业项目展示</h1>
        <p class="hero-subtitle">展示我的技术能力和项目经验，涵盖多个领域的技术栈和解决方案</p>
      </div>
    </div>

    <!-- 统计概览 -->
    <div class="stats-section" v-if="!loading && stats">
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-label">项目总数</div>
          <div class="stat-value">{{ stats.totalProjects }}</div>
          <div class="stat-unit">个</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">项目类型</div>
          <div class="stat-value">{{ stats.categoryStats?.length || 0 }}</div>
          <div class="stat-unit">类</div>
        </div>
        <div class="stat-card">
          <div class="stat-label">技术栈</div>
          <div class="stat-value">{{ stats.techStats?.length || 0 }}</div>
          <div class="stat-unit">项</div>
        </div>
      </div>
    </div>

    <!-- 项目类型和技术栈分布 -->
    <div class="distribution-section" v-if="!loading && stats">
      <div class="distribution-grid">
        <!-- 项目类型分布 -->
        <div class="distribution-card" v-if="stats.categoryStats && stats.categoryStats.length > 0">
          <h3 class="distribution-title">项目类型分布</h3>
          <div class="distribution-list">
            <div
              v-for="item in stats.categoryStats"
              :key="item.category"
              class="distribution-item"
            >
              <div class="distribution-item-left">
                <span class="distribution-label">{{ item.category }}</span>
              </div>
              <div class="distribution-item-right">
                <span class="distribution-count">{{ item.count }} / {{ getMaxCategoryCount() }}</span>
                <div class="distribution-bar">
                  <div
                    class="distribution-bar-fill"
                    :style="{ width: `${(item.count / getMaxCategoryCount()) * 100}%` }"
                  ></div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 技术栈分布 -->
        <div class="distribution-card" v-if="stats.techStats && stats.techStats.length > 0">
          <h3 class="distribution-title">技术栈使用情况</h3>
          <div class="distribution-list">
            <div
              v-for="item in stats.techStats"
              :key="item.tech"
              class="distribution-item"
            >
              <div class="distribution-item-left">
                <span class="distribution-label">{{ item.tech }}</span>
              </div>
              <div class="distribution-item-right">
                <span class="distribution-count">{{ item.count }} / {{ getMaxTechCount() }}</span>
                <div class="distribution-bar">
                  <div
                    class="distribution-bar-fill"
                    :style="{ width: `${(item.count / getMaxTechCount()) * 100}%` }"
                  ></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 项目列表 -->
    <div class="projects-section">
      <div class="projects-header">
        <h2 class="projects-title">项目列表</h2>
        <p class="projects-subtitle">以下是我完成的部分副业项目</p>
      </div>

      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p class="loading-text">加载中...</p>
      </div>

      <div v-else-if="error" class="error-container">
        <p class="error-text">{{ error }}</p>
      </div>

      <div v-else-if="projects.length === 0" class="empty-container">
        <div class="empty-icon">📦</div>
        <h3 class="empty-title">暂无项目</h3>
        <p class="empty-text">还没有公开的副业项目</p>
      </div>

      <div v-else class="projects-grid">
        <div
          v-for="project in projects"
          :key="project.id"
          class="project-card"
          @click="handleProjectClick(project.id)"
        >
          <div class="project-header">
            <h3 class="project-title">{{ project.title }}</h3>
            <div class="project-status">
              <span :class="getStatusClass(project.status)">
                {{ getStatusText(project.status) }}
              </span>
            </div>
          </div>

          <p v-if="project.description" class="project-description">
            {{ truncateText(project.description, 120) }}
          </p>

          <div class="project-meta">
            <div class="project-meta-item" v-if="project.category">
              <i class="fas fa-tag"></i>
              <span>{{ project.category }}</span>
            </div>
            <div class="project-meta-item" v-if="project.clientName">
              <i class="fas fa-user"></i>
              <span>{{ project.clientName }}</span>
            </div>
            <div class="project-meta-item" v-if="project.source">
              <i class="fas fa-link"></i>
              <span>{{ project.source }}</span>
            </div>
          </div>

          <div class="project-tech-stack" v-if="project.techStack">
            <span
              v-for="tech in parseTechStack(project.techStack)"
              :key="tech"
              class="tech-tag"
            >
              {{ tech }}
            </span>
          </div>

          <div class="project-footer">
            <div class="project-time" v-if="project.endTime">
              <i class="fas fa-calendar"></i>
              <span>{{ formatDate(project.endTime) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useApi } from '~/composables/useApi'

definePageMeta({
  layout: 'default'
})

const router = useRouter()
const api = useApi()

// 数据状态
const loading = ref(false)
const error = ref<string | null>(null)
const projects = ref<any[]>([])
const stats = ref<any>(null)

// 获取统计数据
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
  } catch (e: any) {
    console.error('获取统计数据失败:', e)
  }
}

// 获取项目列表
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

// 获取最大类型数量
const getMaxCategoryCount = computed(() => {
  if (!stats.value?.categoryStats || stats.value.categoryStats.length === 0) return 1
  return Math.max(...stats.value.categoryStats.map((item: any) => item.count))
})

// 获取最大技术栈数量
const getMaxTechCount = computed(() => {
  if (!stats.value?.techStats || stats.value.techStats.length === 0) return 1
  return Math.max(...stats.value.techStats.map((item: any) => item.count))
})

// 解析技术栈
const parseTechStack = (techStack: string | null | undefined): string[] => {
  if (!techStack) return []
  return techStack.split(',').map(t => t.trim()).filter(t => t.length > 0)
}

// 截断文本
const truncateText = (text: string, maxLength: number): string => {
  if (!text) return ''
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}

// 获取状态文本
const getStatusText = (status: number): string => {
  switch (status) {
    case 0: return '进行中'
    case 1: return '已完成'
    case 2: return '待付款'
    case 3: return '已取货'
    default: return '未知'
  }
}

// 获取状态样式类
const getStatusClass = (status: number): string => {
  switch (status) {
    case 0: return 'status-badge status-badge--info'
    case 1: return 'status-badge status-badge--success'
    case 2: return 'status-badge status-badge--warning'
    case 3: return 'status-badge status-badge--error'
    default: return 'status-badge'
  }
}

// 格式化日期
const formatDate = (dateStr: string | null | undefined): string => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })
  } catch {
    return dateStr
  }
}

// 项目点击
const handleProjectClick = (id: number) => {
  router.push(`/side-projects/${id}`)
}

// 初始加载
onMounted(() => {
  fetchStats()
  fetchProjects()
})
</script>

<style scoped>
.side-projects-page {
  min-height: 100vh;
  background: var(--color-bg-body);
  padding: var(--spacing-2xl) var(--spacing-lg);
}

/* Hero 区域 */
.hero-section {
  text-align: center;
  padding: var(--spacing-3xl) 0 var(--spacing-2xl);
  margin-bottom: var(--spacing-2xl);
}

.hero-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md) 0;
}

.hero-subtitle {
  font-size: var(--font-size-lg);
  color: var(--color-text-muted);
  margin: 0;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

/* 统计区域 */
.stats-section {
  margin-bottom: var(--spacing-2xl);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
  max-width: 1200px;
  margin: 0 auto;
}

.stat-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  text-align: center;
  box-shadow: var(--shadow-sm);
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
  border-color: var(--color-primary);
}

.stat-label {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-sm);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-value {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  line-height: 1;
  margin-bottom: var(--spacing-xs);
}

.stat-unit {
  font-size: var(--font-size-body);
  color: var(--color-text-muted);
}

/* 分布区域 */
.distribution-section {
  margin-bottom: var(--spacing-2xl);
}

.distribution-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--spacing-xl);
  max-width: 1200px;
  margin: 0 auto;
}

.distribution-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  box-shadow: var(--shadow-sm);
}

.distribution-title {
  font-size: var(--font-size-h3);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-lg) 0;
}

.distribution-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.distribution-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-md);
}

.distribution-item-left {
  flex: 1;
  min-width: 0;
}

.distribution-label {
  font-size: var(--font-size-body);
  color: var(--color-text-main);
  font-weight: 500;
}

.distribution-item-right {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  flex-shrink: 0;
}

.distribution-count {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  min-width: 50px;
  text-align: right;
}

.distribution-bar {
  width: 100px;
  height: 6px;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-sm);
  overflow: hidden;
}

.distribution-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-primary-hover));
  border-radius: var(--radius-sm);
  transition: width 0.3s ease;
}

/* 项目区域 */
.projects-section {
  max-width: 1200px;
  margin: 0 auto;
}

.projects-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.projects-title {
  font-size: var(--font-size-h2);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm) 0;
}

.projects-subtitle {
  font-size: var(--font-size-body);
  color: var(--color-text-muted);
  margin: 0;
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: var(--spacing-xl);
}

.project-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  box-shadow: var(--shadow-sm);
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.project-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
  border-color: var(--color-primary);
}

.project-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-md);
}

.project-title {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
  flex: 1;
}

.project-status {
  flex-shrink: 0;
}

.status-badge {
  display: inline-block;
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-xs);
  font-weight: 500;
}

.status-badge--info {
  background: rgba(59, 130, 246, 0.1);
  color: var(--color-primary);
}

.status-badge--success {
  background: rgba(16, 185, 129, 0.1);
  color: var(--color-success);
}

.status-badge--warning {
  background: rgba(245, 158, 11, 0.1);
  color: var(--color-warning);
}

.status-badge--error {
  background: rgba(239, 68, 68, 0.1);
  color: var(--color-danger);
}

.project-description {
  font-size: var(--font-size-body);
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
}

.project-meta {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-md);
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
}

.project-meta-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.project-meta-item i {
  opacity: 0.7;
}

.project-tech-stack {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-xs);
}

.tech-tag {
  display: inline-block;
  padding: var(--spacing-xs) var(--spacing-sm);
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-xs);
  color: var(--color-text-main);
}

.project-footer {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: var(--spacing-xs);
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  margin-top: auto;
}

.project-time {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
}

.project-time i {
  opacity: 0.7;
}

/* 加载和空状�?*/
.loading-container,
.empty-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-3xl);
  text-align: center;
}

.loading-spinner {
  width: 48px;
  height: 48px;
  border: 4px solid var(--color-border-subtle);
  border-top-color: var(--color-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: var(--spacing-md);
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.loading-text,
.error-text {
  color: var(--color-text-muted);
  font-size: var(--font-size-body);
}

.empty-icon {
  font-size: 64px;
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.empty-title {
  font-size: var(--font-size-h3);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm) 0;
}

.empty-text {
  color: var(--color-text-muted);
  font-size: var(--font-size-body);
  margin: 0;
}

/* 响应�?*/
@media (max-width: 768px) {
  .side-projects-page {
    padding: var(--spacing-xl) var(--spacing-md);
  }

  .hero-section {
    padding: var(--spacing-2xl) 0 var(--spacing-xl);
  }

  .stats-grid,
  .distribution-grid {
    grid-template-columns: 1fr;
  }

  .projects-grid {
    grid-template-columns: 1fr;
  }
}
</style>

