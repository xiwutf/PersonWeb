<template>
  <div class="side-project-detail-page">
    <!-- 返回按钮 -->
    <div class="back-button-container">
      <NuxtLink to="/side-projects" class="back-button">
        <i class="fas fa-arrow-left"></i>
        <span>返回项目列表</span>
      </NuxtLink>
    </div>

    <!-- 加载状态 -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p class="loading-text">加载中...</p>
    </div>

    <!-- 错误状态 -->
    <div v-else-if="error" class="error-container">
      <div class="error-icon">⚠️</div>
      <h2 class="error-title">加载失败</h2>
      <p class="error-message">{{ error }}</p>
      <NuxtLink to="/side-projects" class="error-button">
        返回项目列表
      </NuxtLink>
    </div>

    <!-- 项目详情 -->
    <div v-else-if="project" class="project-detail">
      <!-- 项目头部 -->
      <div class="project-header">
        <div class="project-header-content">
          <div class="project-title-section">
            <h1 class="project-title">{{ project.title }}</h1>
            <div class="project-meta-row">
              <span :class="getStatusClass(project.status)" class="status-badge">
                {{ getStatusText(project.status) }}
              </span>
              <span v-if="project.category" class="category-badge">
                <i class="fas fa-tag"></i>
                {{ project.category }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 项目内容 -->
      <div class="project-content">
        <!-- 左侧主要内容 -->
        <div class="project-main">
          <!-- 项目描述 -->
          <div class="content-section">
            <h2 class="section-title">项目描述</h2>
            <div class="section-content">
              <p v-if="project.description" class="description-text">
                {{ project.description }}
              </p>
              <p v-else class="description-text text-muted">暂无描述</p>
            </div>
          </div>

          <!-- 技术栈 -->
          <div v-if="techStackArray.length > 0" class="content-section">
            <h2 class="section-title">技术栈</h2>
            <div class="section-content">
              <div class="tech-stack-grid">
                <span
                  v-for="tech in techStackArray"
                  :key="tech"
                  class="tech-tag"
                >
                  {{ tech }}
                </span>
              </div>
            </div>
          </div>

          <!-- 项目信息 -->
          <div class="content-section">
            <h2 class="section-title">项目信息</h2>
            <div class="section-content">
              <div class="info-grid">
                <div v-if="project.clientName" class="info-item">
                  <div class="info-label">
                    <i class="fas fa-user"></i>
                    <span>客户名称</span>
                  </div>
                  <div class="info-value">{{ project.clientName }}</div>
                </div>
                <div v-if="project.source" class="info-item">
                  <div class="info-label">
                    <i class="fas fa-link"></i>
                    <span>客户来源</span>
                  </div>
                  <div class="info-value">{{ project.source }}</div>
                </div>
                <div v-if="project.startTime" class="info-item">
                  <div class="info-label">
                    <i class="fas fa-calendar-alt"></i>
                    <span>开始时间</span>
                  </div>
                  <div class="info-value">{{ formatDate(project.startTime) }}</div>
                </div>
                <div v-if="project.endTime" class="info-item">
                  <div class="info-label">
                    <i class="fas fa-calendar-check"></i>
                    <span>结束时间</span>
                  </div>
                  <div class="info-value">{{ formatDate(project.endTime) }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 右侧侧边栏 -->
        <div class="project-sidebar">
          <!-- 项目状态卡片 -->
          <div class="sidebar-card">
            <h3 class="sidebar-card-title">项目状态</h3>
            <div class="sidebar-card-content">
              <div class="status-display">
                <span :class="getStatusClass(project.status)" class="status-badge-large">
                  {{ getStatusText(project.status) }}
                </span>
              </div>
            </div>
          </div>

          <!-- 时间线 -->
          <div v-if="project.startTime || project.endTime" class="sidebar-card">
            <h3 class="sidebar-card-title">时间线</h3>
            <div class="sidebar-card-content">
              <div class="timeline">
                <div v-if="project.startTime" class="timeline-item">
                  <div class="timeline-marker timeline-marker-start"></div>
                  <div class="timeline-content">
                    <div class="timeline-label">开始</div>
                    <div class="timeline-date">{{ formatDate(project.startTime) }}</div>
                  </div>
                </div>
                <div v-if="project.endTime" class="timeline-item">
                  <div class="timeline-marker timeline-marker-end"></div>
                  <div class="timeline-content">
                    <div class="timeline-label">完成</div>
                    <div class="timeline-date">{{ formatDate(project.endTime) }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 项目类型 -->
          <div v-if="project.category" class="sidebar-card">
            <h3 class="sidebar-card-title">项目类型</h3>
            <div class="sidebar-card-content">
              <span class="category-tag-large">
                <i class="fas fa-tag"></i>
                {{ project.category }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useApi } from '~/composables/useApi'

definePageMeta({
  layout: 'default'
})

const route = useRoute()
const router = useRouter()
const api = useApi()

// 数据状态
const loading = ref(true)
const error = ref<string | null>(null)
const project = ref<any>(null)

// 技术栈数组
const techStackArray = computed(() => {
  if (!project.value?.techStack) return []
  return project.value.techStack.split(',').map((t: string) => t.trim()).filter((t: string) => t.length > 0)
})

// 获取项目详情
const fetchProject = async () => {
  loading.value = true
  error.value = null
  
  const id = route.params.id
  if (!id || isNaN(Number(id))) {
    error.value = '无效的项目ID'
    loading.value = false
    return
  }

  try {
    const res = await api.get<any>(`/side-projects/public/${id}`)
    
    if (res && typeof res === 'object') {
      project.value = res
    } else {
      error.value = '项目不存在或未公开'
    }
  } catch (e: any) {
    error.value = e.message || '加载项目详情失败'
    if (e.response?.status === 404) {
      error.value = '项目不存在或未公开'
    }
  } finally {
    loading.value = false
  }
}

// 获取状态文本
const getStatusText = (status: number): string => {
  switch (status) {
    case 0: return '进行中'
    case 1: return '已完成'
    case 2: return '待付款'
    case 3: return '已取消'
    default: return '未知'
  }
}

// 获取状态样式类
const getStatusClass = (status: number): string => {
  switch (status) {
    case 0: return 'status-badge--info'
    case 1: return 'status-badge--success'
    case 2: return 'status-badge--warning'
    case 3: return 'status-badge--error'
    default: return ''
  }
}

// 格式化日期
const formatDate = (dateStr: string | null | undefined): string => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
  } catch {
    return dateStr
  }
}

// 初始化
onMounted(() => {
  fetchProject()
})
</script>

<style scoped>
.side-project-detail-page {
  min-height: 100vh;
  background: var(--color-bg-body);
  padding: var(--spacing-xl) var(--spacing-lg);
  max-width: 1200px;
  margin: 0 auto;
}

/* 返回按钮 */
.back-button-container {
  margin-bottom: var(--spacing-xl);
}

.back-button {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  color: var(--color-text-main);
  text-decoration: none;
  transition: all 0.2s ease;
  font-size: var(--font-size-body);
}

.back-button:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-primary);
  color: var(--color-primary);
}

/* 加载和错误状态 */
.loading-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--spacing-3xl);
  text-align: center;
  min-height: 400px;
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

.loading-text {
  color: var(--color-text-muted);
  font-size: var(--font-size-body);
}

.error-icon {
  font-size: 64px;
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.error-title {
  font-size: var(--font-size-h2);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm) 0;
}

.error-message {
  color: var(--color-text-muted);
  font-size: var(--font-size-body);
  margin: 0 0 var(--spacing-lg) 0;
}

.error-button {
  display: inline-block;
  padding: var(--spacing-sm) var(--spacing-lg);
  background: var(--color-primary);
  color: var(--color-bg-card);
  border-radius: var(--radius-md);
  text-decoration: none;
  transition: all 0.2s ease;
}

.error-button:hover {
  background: var(--color-primary-hover);
  transform: translateY(-2px);
}

/* 项目详情 */
.project-detail {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

/* 项目头部 */
.project-header {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-2xl);
  box-shadow: var(--shadow-sm);
}

.project-header-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.project-title-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.project-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0;
  line-height: 1.2;
}

.project-meta-row {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  flex-wrap: wrap;
}

.status-badge {
  display: inline-block;
  padding: var(--spacing-xs) var(--spacing-sm);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-sm);
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

.category-badge {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-xs);
  padding: var(--spacing-xs) var(--spacing-sm);
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-sm);
  color: var(--color-text-main);
}

/* 项目内容 */
.project-content {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: var(--spacing-xl);
}

.project-main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.content-section {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
  box-shadow: var(--shadow-sm);
}

.section-title {
  font-size: var(--font-size-h3);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-lg) 0;
  padding-bottom: var(--spacing-md);
  border-bottom: 2px solid var(--color-border-subtle);
}

.section-content {
  color: var(--color-text-main);
}

.description-text {
  font-size: var(--font-size-body);
  line-height: 1.8;
  color: var(--color-text-main);
  margin: 0;
}

.description-text.text-muted {
  color: var(--color-text-muted);
  font-style: italic;
}

.tech-stack-grid {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}

.tech-tag {
  display: inline-block;
  padding: var(--spacing-xs) var(--spacing-md);
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  color: var(--color-text-main);
  transition: all 0.2s ease;
}

.tech-tag:hover {
  background: var(--color-primary);
  color: var(--color-bg-card);
  border-color: var(--color-primary);
  transform: translateY(-2px);
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--spacing-lg);
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.info-label {
  display: flex;
  align-items: center;
  gap: var(--spacing-xs);
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  font-weight: 500;
}

.info-value {
  font-size: var(--font-size-body);
  color: var(--color-text-main);
  font-weight: 500;
}

/* 侧边栏 */
.project-sidebar {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.sidebar-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  box-shadow: var(--shadow-sm);
}

.sidebar-card-title {
  font-size: var(--font-size-h4);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md) 0;
}

.sidebar-card-content {
  color: var(--color-text-main);
}

.status-badge-large {
  display: inline-block;
  padding: var(--spacing-sm) var(--spacing-lg);
  border-radius: var(--radius-md);
  font-size: var(--font-size-body);
  font-weight: 600;
}

.status-display {
  text-align: center;
}

.timeline {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.timeline-item {
  display: flex;
  gap: var(--spacing-md);
  position: relative;
}

.timeline-item:not(:last-child)::after {
  content: '';
  position: absolute;
  left: 7px;
  top: 24px;
  bottom: -24px;
  width: 2px;
  background: var(--color-border-subtle);
}

.timeline-marker {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  flex-shrink: 0;
  margin-top: 4px;
}

.timeline-marker-start {
  background: var(--color-primary);
  border: 3px solid var(--color-bg-card);
  box-shadow: 0 0 0 2px var(--color-primary);
}

.timeline-marker-end {
  background: var(--color-success);
  border: 3px solid var(--color-bg-card);
  box-shadow: 0 0 0 2px var(--color-success);
}

.timeline-content {
  flex: 1;
}

.timeline-label {
  font-size: var(--font-size-sm);
  color: var(--color-text-muted);
  margin-bottom: var(--spacing-xs);
}

.timeline-date {
  font-size: var(--font-size-body);
  color: var(--color-text-main);
  font-weight: 500;
}

.category-tag-large {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm) var(--spacing-lg);
  background: var(--color-bg-elevated);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  font-size: var(--font-size-body);
  color: var(--color-text-main);
  font-weight: 500;
}

/* 响应式 */
@media (max-width: 968px) {
  .project-content {
    grid-template-columns: 1fr;
  }

  .project-sidebar {
    order: -1;
  }
}

@media (max-width: 768px) {
  .side-project-detail-page {
    padding: var(--spacing-lg) var(--spacing-md);
  }

  .project-header {
    padding: var(--spacing-lg);
  }

  .content-section {
    padding: var(--spacing-lg);
  }

  .info-grid {
    grid-template-columns: 1fr;
  }
}
</style>

