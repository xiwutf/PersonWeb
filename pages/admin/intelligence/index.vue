<template>
  <ClientOnly>
    <div class="intelligence-center-page">
      <!-- 页面标题 -->
      <div class="page-header">
        <h1 class="page-title">情报中心</h1>
        <div class="header-actions">
          <n-button :loading="taskRunning" type="primary" @click="runCollectTask">
            <template #icon>
              <i class="fas fa-download"></i>
            </template>
            手动采集
          </n-button>
          <n-button :loading="taskRunning" type="info" @click="runAnalyzeTask">
            <template #icon>
              <i class="fas fa-brain"></i>
            </template>
            手动分析
          </n-button>
          <n-button :loading="taskRunning" type="success" @click="runGenerateReport">
            <template #icon>
              <i class="fas fa-file-alt"></i>
            </template>
            生成日报
          </n-button>
        </div>
      </div>

      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <n-spin size="large" />
      </div>

      <template v-else>
        <!-- 核心指标卡片 -->
        <div class="kpi-cards">
          <div class="kpi-card kpi-icon-blue">
            <div class="kpi-icon">
              <i class="fas fa-inbox"></i>
            </div>
            <div class="kpi-content">
              <div class="kpi-value">{{ dashboard.todayCollected }}</div>
              <div class="kpi-label">今日采集</div>
            </div>
          </div>
          <div class="kpi-card kpi-icon-pink">
            <div class="kpi-icon">
              <i class="fas fa-star"></i>
            </div>
            <div class="kpi-content">
              <div class="kpi-value">{{ dashboard.todayHighValue }}</div>
              <div class="kpi-label">今日高价值</div>
            </div>
          </div>
          <div class="kpi-card kpi-icon-cyan clickable" @click="navigateTo('/admin/intelligence/daily-report')">
            <div class="kpi-icon">
              <i class="fas fa-calendar-day"></i>
            </div>
            <div class="kpi-content">
              <div class="kpi-value">{{ dashboard.recentReports.length }}</div>
              <div class="kpi-label">日报数量</div>
            </div>
          </div>
          <div class="kpi-card kpi-icon-green clickable" @click="navigateTo('/admin/intelligence/content')">
            <div class="kpi-icon">
              <i class="fas fa-layer-group"></i>
            </div>
            <div class="kpi-content">
              <div class="kpi-value">{{ dashboard.categoryStats.length }}</div>
              <div class="kpi-label">分类数量</div>
            </div>
          </div>
        </div>

        <div class="content-grid">
          <!-- 最新日报 -->
          <div class="content-section">
            <div class="section-header">
              <h2 class="section-title">最新日报</h2>
              <n-button text @click="navigateTo('/admin/intelligence/daily-report')">
                查看更多 <i class="fas fa-arrow-right ml-1"></i>
              </n-button>
            </div>
            <div v-if="dashboard.latestReport" class="latest-report-card" @click="navigateTo(`/admin/intelligence/daily-report/${dashboard.latestReport.id}`)">
              <div class="report-date">{{ formatDate(dashboard.latestReport.reportDate) }}</div>
              <div class="report-title">{{ dashboard.latestReport.title }}</div>
              <div class="report-meta">
                <span><i class="fas fa-list"></i> {{ dashboard.latestReport.itemCount }} 条内容</span>
                <span><i class="fas fa-clock"></i> {{ formatTime(dashboard.latestReport.generatedAt) }}</span>
              </div>
            </div>
            <div v-else class="empty-state">
              <i class="fas fa-file-alt"></i>
              <p>暂无日报数据</p>
              <n-button type="primary" size="small" @click="runGenerateReport">
                生成第一份日报
              </n-button>
            </div>
          </div>

          <!-- 最近任务状态 -->
          <div class="content-section">
            <div class="section-header">
              <h2 class="section-title">最近任务状态</h2>
              <n-button text @click="navigateTo('/admin/intelligence/tasks')">
                查看更多 <i class="fas fa-arrow-right ml-1"></i>
              </n-button>
            </div>
            <div v-if="dashboard.recentTaskStatus" class="task-status-card">
              <div class="task-info">
                <div class="task-name">{{ dashboard.recentTaskStatus.taskName }}</div>
                <div class="task-message">{{ dashboard.recentTaskStatus.message }}</div>
              </div>
              <div class="task-status">
                <n-tag
                  :type="getStatusTagType(dashboard.recentTaskStatus.status)"
                  :bordered="false"
                >
                  {{ getStatusText(dashboard.recentTaskStatus.status) }}
                </n-tag>
              </div>
              <div class="task-time">{{ formatTime(dashboard.recentTaskStatus.startTime) }}</div>
            </div>
            <div v-else class="empty-state">
              <i class="fas fa-tasks"></i>
              <p>暂无任务记录</p>
            </div>
          </div>

          <!-- 分类统计 -->
          <div class="content-section">
            <div class="section-header">
              <h2 class="section-title">分类统计</h2>
            </div>
            <div v-if="dashboard.categoryStats.length > 0" class="category-stats">
              <div
                v-for="stat in dashboard.categoryStats"
                :key="stat.category"
                class="category-item"
                @click="navigateTo('/admin/intelligence/content', { query: { category: stat.category } })"
              >
                <div class="category-name">{{ stat.category }}</div>
                <div class="category-count">{{ stat.count }}</div>
                <div class="category-bar">
                  <div
                    class="category-bar-fill"
                    :style="{ width: `${(stat.count / maxCategoryCount) * 100}%` }"
                  ></div>
                </div>
              </div>
            </div>
            <div v-else class="empty-state">
              <i class="fas fa-chart-pie"></i>
              <p>暂无分类数据</p>
            </div>
          </div>

          <!-- 最新内容 -->
          <div class="content-section">
            <div class="section-header">
              <h2 class="section-title">最新内容</h2>
              <n-button text @click="navigateTo('/admin/intelligence/content')">
                查看更多 <i class="fas fa-arrow-right ml-1"></i>
              </n-button>
            </div>
            <div v-if="dashboard.recentContents.length > 0" class="content-list">
              <div
                v-for="content in dashboard.recentContents"
                :key="content.id"
                class="content-item"
                @click="navigateTo(`/admin/intelligence/content/${content.id}`)"
              >
                <div class="content-title">{{ content.title }}</div>
                <div class="content-meta">
                  <n-tag size="tiny" :bordered="false">{{ content.category }}</n-tag>
                  <span class="content-source">{{ content.sourceName }}</span>
                  <span class="content-time">{{ formatTime(content.createdAt) }}</span>
                </div>
                <div class="content-score">
                  <span v-if="content.relevanceScore >= 70" class="high-value">高价值</span>
                </div>
              </div>
            </div>
            <div v-else class="empty-state">
              <i class="fas fa-newspaper"></i>
              <p>暂无内容数据</p>
              <n-button type="primary" size="small" @click="navigateTo('/admin/intelligence/source')">
                配置采集来源
              </n-button>
            </div>
          </div>
        </div>
      </template>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import { useNotification } from '~/composables/useToast'
import type { DashboardStats } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useIntelligenceApi()
const notification = useNotification()

// 状态
const loading = ref(true)
const taskRunning = ref(false)
const dashboard = ref<DashboardStats>({
  todayCollected: 0,
  todayHighValue: 0,
  recentReports: [],
  recentContents: [],
  categoryStats: []
})

// 计算属性
const maxCategoryCount = computed(() => {
  const counts = dashboard.value.categoryStats.map(s => s.count)
  return counts.length > 0 ? Math.max(...counts) : 1
})

// 获取仪表盘数据
const fetchDashboard = async () => {
  loading.value = true
  try {
    dashboard.value = await api.getDashboardStats()
  } catch (error) {
    console.error('获取仪表盘数据失败:', error)
  } finally {
    loading.value = false
  }
}

// 运行采集任务
const runCollectTask = async () => {
  taskRunning.value = true
  try {
    const result = await api.runCollectTask()
    notification.success(result.message || '采集任务已提交')
    await fetchDashboard()
  } catch (error) {
    console.error('触发采集任务失败:', error)
    notification.error('采集任务提交失败')
  } finally {
    taskRunning.value = false
  }
}

// 运行分析任务
const runAnalyzeTask = async () => {
  taskRunning.value = true
  try {
    const result = await api.runAnalyzeTask()
    notification.success(result.message || '分析任务已提交')
    await fetchDashboard()
  } catch (error) {
    console.error('触发分析任务失败:', error)
    notification.error('分析任务提交失败')
  } finally {
    taskRunning.value = false
  }
}

// 生成日报
const runGenerateReport = async () => {
  taskRunning.value = true
  try {
    const result = await api.runGenerateReport()
    notification.success(result.message || '日报生成任务已提交')
    await fetchDashboard()
  } catch (error) {
    console.error('生成日报失败:', error)
    notification.error('日报生成任务提交失败')
  } finally {
    taskRunning.value = false
  }
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}年${d.getMonth() + 1}月${d.getDate()}日`
}

// 格式化时间
const formatTime = (time?: string) => {
  if (!time) return ''
  const d = new Date(time)
  const now = new Date()
  const diff = now.getTime() - d.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  return `${days}天前`
}

// 获取状态标签类型
const getStatusTagType = (status: string) => {
  switch (status) {
    case 'running': return 'info'
    case 'success': return 'success'
    case 'failed': return 'error'
    default: return 'default'
  }
}

// 获取状态文本
const getStatusText = (status: string) => {
  switch (status) {
    case 'running': return '运行中'
    case 'success': return '成功'
    case 'failed': return '失败'
    default: return status
  }
}

// 初始化
onMounted(() => {
  fetchDashboard()
})
</script>

<style scoped>
.intelligence-center-page {
  padding: var(--spacing-lg);
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
  color: var(--color-text-main);
}

.header-actions {
  display: flex;
  gap: var(--spacing-md);
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

/* KPI 卡片 */
.kpi-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.kpi-card {
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  box-shadow: var(--shadow-card);
  transition: transform 0.2s, box-shadow 0.2s;
}

.kpi-card.clickable {
  cursor: pointer;
}

.kpi-card.clickable:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}

.kpi-icon {
  width: 56px;
  height: 56px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 24px;
}

/* KPI 图标背景色 */
.kpi-card.kpi-icon-blue .kpi-icon {
  background: linear-gradient(135deg, var(--color-primary) 0%, #764ba2 100%);
}

.kpi-card.kpi-icon-pink .kpi-icon {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.kpi-card.kpi-icon-cyan .kpi-icon {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.kpi-card.kpi-icon-green .kpi-icon {
  background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
}

.kpi-content {
  flex: 1;
}

.kpi-value {
  font-size: 28px;
  font-weight: 700;
  color: var(--color-text-main);
  line-height: 1.2;
}

.kpi-label {
  font-size: 14px;
  color: var(--color-text-sec);
  margin-top: 4px;
}

/* 内容网格 */
.content-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.content-section {
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
  box-shadow: var(--shadow-card);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

/* 最新日报 */
.latest-report-card {
  background: var(--color-primary);
  border-radius: var(--radius-md);
  padding: var(--spacing-lg);
  color: #fff;
  cursor: pointer;
  transition: transform 0.2s;
}

.latest-report-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
}

.report-date {
  font-size: 14px;
  opacity: 0.9;
  margin-bottom: var(--spacing-sm);
}

.report-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: var(--spacing-md);
}

.report-meta {
  display: flex;
  gap: var(--spacing-md);
  font-size: 13px;
  color: rgba(255, 255, 255, 0.8);
}

.report-meta i {
  margin-right: 4px;
}

/* 任务状态卡片 */
.task-status-card {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
}

.task-info {
  margin-bottom: var(--spacing-md);
}

.task-name {
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-sm);
}

.task-message {
  font-size: 13px;
  color: var(--color-text-sec);
  margin-bottom: var(--spacing-sm);
}

.task-status {
  display: inline-flex;
  align-items: center;
}

.task-time {
  font-size: 12px;
  color: var(--color-text-sub);
}

/* 分类统计 */
.category-stats {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.category-item {
  cursor: pointer;
  padding: var(--spacing-md);
  border-radius: var(--radius-md);
  background: var(--color-bg-card);
  transition: background 0.2s;
}

.category-item:hover {
  background: var(--color-border);
}

.category-name {
  font-weight: 500;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-sm);
}

.category-count {
  font-size: 12px;
  color: var(--color-text-sec);
}

.category-bar {
  height: 4px;
  background: var(--color-border);
  border-radius: 2px;
  overflow: hidden;
  margin-top: var(--spacing-sm);
}

.category-bar-fill {
  height: 100%;
  background: var(--color-primary);
  transition: width 0.3s;
}

/* 内容列表 */
.content-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.content-item {
  padding: var(--spacing-md);
  border-radius: var(--radius-md);
  background: var(--color-bg-card);
  cursor: pointer;
  transition: background 0.2s;
}

.content-item:hover {
  background: var(--color-border);
}

.content-title {
  font-weight: 500;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-sm);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.content-meta {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: 13px;
  color: var(--color-text-sec);
}

.content-source {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.content-time {
  opacity: 0.7;
}

.content-score {
  position: absolute;
  top: var(--spacing-md);
  right: var(--spacing-md);
}

.high-value {
  font-size: 11px;
  padding: 2px var(--spacing-sm);
  background: var(--color-error);
  color: #fff;
  border-radius: var(--radius-sm);
}

/* 空状态 */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 40px var(--spacing-lg);
  color: var(--color-text-sub);
}

.empty-state i {
  font-size: 48px;
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.empty-state p {
  margin: 0;
}

.ml-1 {
  margin-left: var(--spacing-sm);
}
</style>
