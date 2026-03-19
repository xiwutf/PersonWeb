<template>
  <ClientOnly>
    <div class="intelligence-center-page">
      <PageHeader
        title="情报中心"
        description="统一管理采集、分析、日报与内容沉淀，让情报流程更清晰。"
      >
        <template #actions>
          <n-space :size="12" wrap>
            <n-button :loading="taskRunning" secondary type="primary" @click="runCollectTask">
              <template #icon>
                <i class="fas fa-download"></i>
              </template>
              手动采集
            </n-button>
            <n-button :loading="taskRunning" secondary type="info" @click="runAnalyzeTask">
              <template #icon>
                <i class="fas fa-brain"></i>
              </template>
              手动分析
            </n-button>
            <n-button :loading="taskRunning" type="primary" @click="runGenerateReport">
              <template #icon>
                <i class="fas fa-file-alt"></i>
              </template>
              生成日报
            </n-button>
          </n-space>
        </template>
      </PageHeader>

      <n-card v-if="loading" :bordered="false" class="intelligence-loading-card">
        <div class="loading-container">
          <n-spin size="large" />
          <p>正在加载情报中心数据...</p>
        </div>
      </n-card>

      <template v-else>
        <n-grid class="intelligence-kpi-grid" cols="1 s:2 m:4" responsive="screen" :x-gap="16" :y-gap="16">
          <n-gi v-for="card in statCards" :key="card.label">
            <n-card
              :bordered="false"
              class="intelligence-kpi-card"
              :class="{ 'is-clickable': !!card.to }"
              hoverable
              @click="card.to && navigateTo(card.to)"
            >
              <div class="kpi-card-shell">
                <div class="kpi-icon" :class="card.tone">
                  <i :class="card.icon"></i>
                </div>
                <div class="kpi-copy">
                  <div class="kpi-label">{{ card.label }}</div>
                  <div class="kpi-value">{{ card.value }}</div>
                  <div class="kpi-meta">{{ card.meta }}</div>
                </div>
              </div>
            </n-card>
          </n-gi>
        </n-grid>

        <section class="intelligence-main-grid">
          <n-card :bordered="false" class="intelligence-panel intelligence-panel--hero">
            <template #header>
              <div class="panel-heading">
                <div>
                  <p class="panel-kicker">日报中心</p>
                  <h2>最新日报</h2>
                </div>
                <n-button text @click="navigateTo('/admin/intelligence/daily-report')">
                  查看全部
                </n-button>
              </div>
            </template>

            <div
              v-if="dashboard.latestReport"
              class="hero-report"
              @click="navigateTo(`/admin/intelligence/daily-report/${dashboard.latestReport.id}`)"
            >
              <div class="hero-report-copy">
                <span class="hero-report-chip">最新生成</span>
                <h3>{{ dashboard.latestReport.title }}</h3>
                <p>{{ formatDate(dashboard.latestReport.reportDate) }}</p>
              </div>
              <div class="hero-report-metrics">
                <div class="hero-metric">
                  <strong>{{ dashboard.latestReport.itemCount }}</strong>
                  <span>收录条目</span>
                </div>
                <div class="hero-metric">
                  <strong>{{ formatTime(dashboard.latestReport.generatedAt) }}</strong>
                  <span>生成时间</span>
                </div>
              </div>
            </div>

            <n-empty v-else class="intelligence-empty" description="暂无日报数据">
              <template #extra>
                <n-button type="primary" @click="runGenerateReport">立即生成日报</n-button>
              </template>
            </n-empty>

            <div class="hero-footer">
              <div class="hero-summary">
                <span class="hero-summary-label">近日报告</span>
                <strong>{{ dashboard.recentReports.length }}</strong>
              </div>
              <div class="hero-summary">
                <span class="hero-summary-label">高价值内容</span>
                <strong>{{ dashboard.todayHighValue }}</strong>
              </div>
            </div>
          </n-card>

          <div class="intelligence-side-stack">
            <n-card :bordered="false" class="intelligence-panel">
              <template #header>
                <div class="panel-heading">
                  <div>
                    <p class="panel-kicker">任务状态</p>
                    <h2>最近任务</h2>
                  </div>
                  <n-button text @click="navigateTo('/admin/intelligence/tasks')">
                    查看任务
                  </n-button>
                </div>
              </template>

              <div v-if="dashboard.recentTaskStatus" class="task-card">
                <div class="task-card-top">
                  <div>
                    <div class="task-name">{{ dashboard.recentTaskStatus.taskName }}</div>
                    <div class="task-message">{{ dashboard.recentTaskStatus.message }}</div>
                  </div>
                  <n-tag :type="getStatusTagType(dashboard.recentTaskStatus.status)" :bordered="false" round>
                    {{ getStatusText(dashboard.recentTaskStatus.status) }}
                  </n-tag>
                </div>
                <div class="task-time">{{ formatTime(dashboard.recentTaskStatus.startTime) }}</div>
              </div>

              <n-empty v-else class="intelligence-empty" description="暂无任务记录" />
            </n-card>

            <n-card :bordered="false" class="intelligence-panel">
              <template #header>
                <div class="panel-heading">
                  <div>
                    <p class="panel-kicker">分类分布</p>
                    <h2>内容分类</h2>
                  </div>
                  <n-button text @click="navigateTo('/admin/intelligence/content')">
                    查看内容
                  </n-button>
                </div>
              </template>

              <div v-if="dashboard.categoryStats.length" class="category-list">
                <button
                  v-for="stat in topCategoryStats"
                  :key="stat.category"
                  type="button"
                  class="category-item"
                  @click="navigateTo('/admin/intelligence/content', { query: { category: stat.category } })"
                >
                  <div class="category-item-head">
                    <span class="category-name">{{ stat.category }}</span>
                    <strong class="category-count">{{ stat.count }}</strong>
                  </div>
                  <div class="category-bar">
                    <div class="category-bar-fill" :style="{ width: `${(stat.count / maxCategoryCount) * 100}%` }"></div>
                  </div>
                </button>
              </div>

              <n-empty v-else class="intelligence-empty" description="暂无分类统计" />
            </n-card>
          </div>
        </section>

        <section class="intelligence-bottom-grid">
          <n-card :bordered="false" class="intelligence-panel">
            <template #header>
              <div class="panel-heading">
                <div>
                  <p class="panel-kicker">最新内容</p>
                  <h2>待跟进线索</h2>
                </div>
                <n-button text @click="navigateTo('/admin/intelligence/content')">
                  查看更多
                </n-button>
              </div>
            </template>

            <div v-if="dashboard.recentContents.length" class="content-list">
              <button
                v-for="content in dashboard.recentContents.slice(0, 6)"
                :key="content.id"
                type="button"
                class="content-item"
                @click="navigateTo(`/admin/intelligence/content/${content.id}`)"
              >
                <div class="content-item-main">
                  <div class="content-item-top">
                    <h3 class="content-title">{{ content.title }}</h3>
                    <n-tag v-if="content.relevanceScore >= 70" size="small" type="warning" :bordered="false" round>
                      高价值
                    </n-tag>
                  </div>
                  <div class="content-meta">
                    <n-tag size="small" :bordered="false" type="info" round>{{ content.category }}</n-tag>
                    <span>{{ content.sourceName }}</span>
                    <span>{{ formatTime(content.createdAt) }}</span>
                  </div>
                </div>
                <i class="fas fa-chevron-right content-arrow"></i>
              </button>
            </div>

            <n-empty v-else class="intelligence-empty" description="暂无内容数据">
              <template #extra>
                <n-button secondary type="primary" @click="navigateTo('/admin/intelligence/source')">
                  配置采集来源
                </n-button>
              </template>
            </n-empty>
          </n-card>

          <n-card :bordered="false" class="intelligence-panel intelligence-panel--aside">
            <template #header>
              <div class="panel-heading">
                <div>
                  <p class="panel-kicker">操作建议</p>
                  <h2>下一步动作</h2>
                </div>
              </div>
            </template>

            <div class="action-list">
              <button type="button" class="action-item" @click="runCollectTask">
                <div>
                  <strong>补充新采集</strong>
                  <span>手动抓取最新线索，更新今日数据面板。</span>
                </div>
                <i class="fas fa-download"></i>
              </button>
              <button type="button" class="action-item" @click="runAnalyzeTask">
                <div>
                  <strong>执行分析任务</strong>
                  <span>对新内容打标签、提炼摘要、筛选高价值线索。</span>
                </div>
                <i class="fas fa-brain"></i>
              </button>
              <button type="button" class="action-item" @click="navigateTo('/admin/intelligence/source')">
                <div>
                  <strong>维护采集源</strong>
                  <span>补充来源配置，避免日报长期空缺。</span>
                </div>
                <i class="fas fa-rss"></i>
              </button>
            </div>
          </n-card>
        </section>
      </template>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import PageHeader from '~/components/admin/patterns/PageHeader.vue'
import { useIntelligenceApi } from '~/composables/useIntelligenceApi'
import { useNotification } from '~/composables/useToast'
import type { DashboardStats } from '~/types/intelligence'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useIntelligenceApi()
const notification = useNotification()

const loading = ref(true)
const taskRunning = ref(false)
const dashboard = ref<DashboardStats>({
  todayCollected: 0,
  todayHighValue: 0,
  recentReports: [],
  recentContents: [],
  categoryStats: []
})

const statCards = computed(() => [
  {
    label: '今日采集',
    value: dashboard.value.todayCollected,
    meta: '新抓取内容数',
    icon: 'fas fa-inbox',
    tone: 'tone-blue'
  },
  {
    label: '今日高价值',
    value: dashboard.value.todayHighValue,
    meta: '高相关内容数',
    icon: 'fas fa-star',
    tone: 'tone-rose'
  },
  {
    label: '日报数量',
    value: dashboard.value.recentReports.length,
    meta: '点击查看日报列表',
    icon: 'fas fa-calendar-day',
    tone: 'tone-cyan',
    to: '/admin/intelligence/daily-report'
  },
  {
    label: '分类数量',
    value: dashboard.value.categoryStats.length,
    meta: '点击查看分类内容',
    icon: 'fas fa-layer-group',
    tone: 'tone-green',
    to: '/admin/intelligence/content'
  }
])

const maxCategoryCount = computed(() => {
  const counts = dashboard.value.categoryStats.map(item => item.count)
  return counts.length ? Math.max(...counts) : 1
})

const topCategoryStats = computed(() => dashboard.value.categoryStats.slice(0, 5))

const fetchDashboard = async () => {
  loading.value = true
  try {
    dashboard.value = await api.getDashboardStats()
  } catch (error) {
    console.error('获取情报中心数据失败:', error)
  } finally {
    loading.value = false
  }
}

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

const formatDate = (date: string) => {
  if (!date) return ''
  const parsed = new Date(date)
  return `${parsed.getFullYear()}年${parsed.getMonth() + 1}月${parsed.getDate()}日`
}

const formatTime = (time?: string) => {
  if (!time) return ''

  const parsed = new Date(time)
  const now = new Date()
  const diff = now.getTime() - parsed.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 60) return `${Math.max(minutes, 1)}分钟前`
  if (hours < 24) return `${hours}小时前`
  return `${days}天前`
}

const getStatusTagType = (status: string) => {
  switch (status) {
    case 'running':
      return 'info'
    case 'success':
      return 'success'
    case 'failed':
      return 'error'
    default:
      return 'default'
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'running':
      return '运行中'
    case 'success':
      return '成功'
    case 'failed':
      return '失败'
    default:
      return status
  }
}

onMounted(() => {
  fetchDashboard()
})
</script>

<style scoped>
.intelligence-center-page {
  padding: var(--spacing-lg);
  max-width: 1480px;
  margin: 0 auto;
}

.intelligence-loading-card,
.intelligence-panel,
.intelligence-kpi-card {
  background:
    linear-gradient(180deg, rgba(18, 24, 39, 0.98), rgba(15, 23, 42, 0.92)),
    var(--color-bg-card);
  border: 1px solid var(--color-border-subtle, rgba(148, 163, 184, 0.18));
  box-shadow: none;
}

.loading-container {
  min-height: 280px;
  display: grid;
  place-items: center;
  gap: var(--spacing-md);
  color: var(--color-text-muted);
}

.loading-container p {
  margin: 0;
}

.intelligence-kpi-grid {
  margin-bottom: var(--spacing-lg);
}

.intelligence-kpi-card.is-clickable {
  cursor: pointer;
}

.kpi-card-shell {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.kpi-icon {
  width: 56px;
  height: 56px;
  border-radius: 18px;
  display: grid;
  place-items: center;
  color: #fff;
  font-size: 1.1rem;
  flex-shrink: 0;
}

.tone-blue {
  background: linear-gradient(135deg, #2563eb, #4f46e5);
}

.tone-rose {
  background: linear-gradient(135deg, #e11d48, #f97316);
}

.tone-cyan {
  background: linear-gradient(135deg, #0891b2, #2563eb);
}

.tone-green {
  background: linear-gradient(135deg, #059669, #22c55e);
}

.kpi-copy {
  min-width: 0;
}

.kpi-label {
  font-size: 0.875rem;
  color: var(--color-text-muted);
}

.kpi-value {
  font-size: clamp(1.8rem, 2.8vw, 2.4rem);
  font-weight: 700;
  line-height: 1.1;
  color: var(--color-text-main);
  margin: 4px 0;
}

.kpi-meta {
  font-size: 0.8rem;
  color: var(--color-text-muted);
}

.intelligence-main-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.5fr) minmax(320px, 0.9fr);
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
}

.intelligence-side-stack,
.intelligence-bottom-grid {
  display: grid;
  gap: var(--spacing-lg);
}

.intelligence-bottom-grid {
  grid-template-columns: minmax(0, 1.5fr) minmax(280px, 0.9fr);
}

.panel-heading {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--spacing-md);
}

.panel-heading h2 {
  margin: 4px 0 0;
  font-size: 1.125rem;
  line-height: 1.3;
  color: var(--color-text-main);
}

.panel-kicker {
  margin: 0;
  font-size: 0.75rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: var(--color-text-muted);
}

.hero-report {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  gap: var(--spacing-lg);
  padding: 24px;
  border-radius: var(--radius-xl);
  background:
    radial-gradient(circle at top right, rgba(59, 130, 246, 0.22), transparent 32%),
    linear-gradient(135deg, rgba(37, 99, 235, 0.18), rgba(8, 145, 178, 0.12));
  border: 1px solid rgba(59, 130, 246, 0.2);
  cursor: pointer;
}

.hero-report-copy h3 {
  margin: 12px 0 8px;
  font-size: 1.35rem;
  color: var(--color-text-main);
}

.hero-report-copy p {
  margin: 0;
  color: var(--color-text-muted);
}

.hero-report-chip {
  display: inline-flex;
  align-items: center;
  min-height: 28px;
  padding: 0 12px;
  border-radius: 999px;
  background: rgba(59, 130, 246, 0.18);
  color: #93c5fd;
  font-size: 0.8rem;
}

.hero-report-metrics {
  display: grid;
  gap: var(--spacing-sm);
  min-width: 140px;
}

.hero-metric {
  padding: 14px 16px;
  border-radius: var(--radius-lg);
  background: rgba(15, 23, 42, 0.5);
  border: 1px solid rgba(148, 163, 184, 0.14);
}

.hero-metric strong {
  display: block;
  font-size: 1.1rem;
  color: var(--color-text-main);
}

.hero-metric span {
  display: block;
  margin-top: 4px;
  color: var(--color-text-muted);
  font-size: 0.8rem;
}

.hero-footer {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: var(--spacing-md);
  margin-top: var(--spacing-lg);
}

.hero-summary {
  padding: 16px 18px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(148, 163, 184, 0.12);
}

.hero-summary-label {
  display: block;
  margin-bottom: 6px;
  font-size: 0.8rem;
  color: var(--color-text-muted);
}

.hero-summary strong {
  font-size: 1.5rem;
  color: var(--color-text-main);
}

.task-card {
  padding: 18px;
  border-radius: var(--radius-lg);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(148, 163, 184, 0.12);
}

.task-card-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-md);
}

.task-name {
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-text-main);
}

.task-message,
.task-time {
  color: var(--color-text-muted);
}

.task-message {
  margin-top: 6px;
  line-height: 1.5;
}

.task-time {
  margin-top: 16px;
  font-size: 0.85rem;
}

.category-list,
.action-list,
.content-list {
  display: grid;
  gap: 12px;
}

.category-item,
.content-item,
.action-item {
  width: 100%;
  border: 0;
  text-align: left;
  background: rgba(255, 255, 255, 0.03);
  border-radius: var(--radius-lg);
  border: 1px solid rgba(148, 163, 184, 0.12);
  color: inherit;
  cursor: pointer;
  transition: border-color 0.2s ease, background 0.2s ease, transform 0.2s ease;
}

.category-item:hover,
.content-item:hover,
.action-item:hover,
.hero-report:hover {
  transform: translateY(-2px);
  border-color: rgba(96, 165, 250, 0.28);
  background: rgba(59, 130, 246, 0.08);
}

.category-item {
  padding: 14px 16px;
}

.category-item-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-sm);
  margin-bottom: 10px;
}

.category-name {
  color: var(--color-text-main);
  font-weight: 500;
}

.category-count {
  color: #93c5fd;
}

.category-bar {
  width: 100%;
  height: 8px;
  border-radius: 999px;
  overflow: hidden;
  background: rgba(148, 163, 184, 0.14);
}

.category-bar-fill {
  height: 100%;
  border-radius: inherit;
  background: linear-gradient(90deg, #3b82f6, #22c55e);
}

.content-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-md);
  padding: 16px 18px;
}

.content-item-main {
  min-width: 0;
  flex: 1;
}

.content-item-top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--spacing-sm);
}

.content-title {
  margin: 0;
  font-size: 1rem;
  line-height: 1.45;
  color: var(--color-text-main);
}

.content-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 12px;
  color: var(--color-text-muted);
  font-size: 0.85rem;
}

.content-arrow {
  color: var(--color-text-muted);
}

.action-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-md);
  padding: 18px;
}

.action-item strong {
  display: block;
  color: var(--color-text-main);
  margin-bottom: 6px;
}

.action-item span {
  display: block;
  color: var(--color-text-muted);
  line-height: 1.5;
  font-size: 0.85rem;
}

.action-item i {
  color: #60a5fa;
}

.intelligence-empty {
  padding: 32px 0;
}

@media (max-width: 1200px) {
  .intelligence-main-grid,
  .intelligence-bottom-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .intelligence-center-page {
    padding: var(--spacing-md);
  }

  .hero-report {
    grid-template-columns: 1fr;
  }

  .hero-footer {
    grid-template-columns: 1fr;
  }

  .task-card-top,
  .content-item,
  .content-item-top,
  .panel-heading {
    flex-direction: column;
    align-items: flex-start;
  }

  .action-item {
    align-items: flex-start;
  }
}
</style>
