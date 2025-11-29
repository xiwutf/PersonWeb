<template>
  <div class="admin-dashboard">
    <!-- 背景装饰 -->
    <div class="dashboard-bg-decoration">
      <div class="bg-decoration-item bg-decoration-blue"></div>
      <div class="bg-decoration-item bg-decoration-purple"></div>
    </div>

    <div class="dashboard-content">
      <!-- 头部 -->
      <header class="dashboard-header">
        <div class="header-content">
          <div class="header-left">
            <h1 class="dashboard-title">仪表盘</h1>
            <p class="dashboard-subtitle">
              <span class="status-dot"></span>
              欢迎回来，Admin
            </p>
          </div>
          <div class="header-right">
            <div class="time-label">当前时间</div>
            <div class="time-value">{{ currentTime }}</div>
          </div>
        </div>
      </header>

      <!-- 统计卡片 -->
      <div class="stats-grid">
        <!-- 文章数卡片 -->
        <div class="stat-card stat-card-blue">
          <div class="stat-card-decoration stat-card-decoration-blue"></div>
          <div class="stat-card-content">
            <div class="stat-card-header">
              <div class="stat-card-icon stat-card-icon-blue">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
              <div class="stat-card-label">总文章数</div>
            </div>
            <div class="stat-card-value stat-card-value-blue">{{ stats.articleCount || 0 }}</div>
            <div class="stat-card-desc">篇已发布</div>
          </div>
        </div>

        <!-- 项目数卡片 -->
        <div class="stat-card stat-card-purple">
          <div class="stat-card-decoration stat-card-decoration-purple"></div>
          <div class="stat-card-content">
            <div class="stat-card-header">
              <div class="stat-card-icon stat-card-icon-purple">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                </svg>
              </div>
              <div class="stat-card-label">总项目数</div>
            </div>
            <div class="stat-card-value stat-card-value-purple">{{ stats.toolCount || 0 }}</div>
            <div class="stat-card-desc">个项目</div>
          </div>
        </div>

        <!-- 今日访问卡片 -->
        <div class="stat-card stat-card-green">
          <div class="stat-card-decoration stat-card-decoration-green"></div>
          <div class="stat-card-content">
            <div class="stat-card-header">
              <div class="stat-card-icon stat-card-icon-green">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
              </div>
              <div class="stat-card-label">今日访问</div>
            </div>
            <div class="stat-card-value stat-card-value-green">{{ stats.todayVisits || 0 }}</div>
            <div class="stat-card-desc">
              <span v-if="stats.yesterdayVisits > 0">
                {{ stats.todayVisits > stats.yesterdayVisits ? '↑' : '↓' }} 
                {{ Math.abs(((stats.todayVisits - stats.yesterdayVisits) / stats.yesterdayVisits * 100).toFixed(1)) }}%
              </span>
              <span v-else>次访问</span>
            </div>
          </div>
        </div>

        <!-- 总访问量卡片 -->
        <div class="stat-card stat-card-cyan">
          <div class="stat-card-decoration stat-card-decoration-cyan"></div>
          <div class="stat-card-content">
            <div class="stat-card-header">
              <div class="stat-card-icon stat-card-icon-cyan">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <div class="stat-card-label">总访问量</div>
            </div>
            <div class="stat-card-value stat-card-value-cyan">{{ stats.totalVisits || 0 }}</div>
            <div class="stat-card-desc">{{ stats.uniqueVisitors || 0 }} 独立访客</div>
          </div>
        </div>

        <!-- 在线人数卡片 -->
        <div class="stat-card stat-card-orange">
          <div class="stat-card-decoration stat-card-decoration-orange"></div>
          <div class="stat-card-content">
            <div class="stat-card-header">
              <div class="stat-card-icon stat-card-icon-orange">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
              </div>
              <div class="stat-card-label">在线人数</div>
            </div>
            <div class="stat-card-value stat-card-value-orange">{{ stats.onlineCount || 0 }}</div>
            <div class="stat-card-desc">最近5分钟活跃</div>
          </div>
        </div>

        <!-- 待审核留言卡片 -->
        <div class="stat-card stat-card-pink" v-if="stats.pendingMessages > 0">
          <div class="stat-card-decoration stat-card-decoration-pink"></div>
          <div class="stat-card-content">
            <div class="stat-card-header">
              <div class="stat-card-icon stat-card-icon-pink">
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
                </svg>
              </div>
              <div class="stat-card-label">待审核留言</div>
            </div>
            <div class="stat-card-value stat-card-value-pink">{{ stats.pendingMessages || 0 }}</div>
            <div class="stat-card-desc">
              <NuxtLink to="/admin/time-capsules" class="stat-card-link">去审核 →</NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- 访问趋势图表 -->
      <div class="chart-section" v-if="visitTrend.length > 0">
        <div class="action-section">
          <h2 class="section-title">
            <svg class="section-title-icon section-title-icon-green" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            访问趋势（最近7天）
          </h2>
          <div class="chart-container">
            <canvas ref="trendChart"></canvas>
          </div>
        </div>
      </div>

      <!-- 快捷操作和数据概览 -->
      <div class="actions-grid">
        <div class="action-section">
          <h2 class="section-title">
            <svg class="section-title-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
            快捷操作
          </h2>
          <div class="action-buttons-grid">
            <NuxtLink to="/admin/articles/edit" class="action-button action-button-blue">
              <div class="action-button-content">
                <div class="action-button-icon">✍️</div>
                <div class="action-button-title">发布新文章</div>
                <div class="action-button-subtitle">快速创建</div>
              </div>
              <div class="action-button-overlay"></div>
            </NuxtLink>
            
            <NuxtLink to="/admin/knowledge" class="action-button action-button-purple">
              <div class="action-button-content">
                <div class="action-button-icon">📚</div>
                <div class="action-button-title">知识库</div>
                <div class="action-button-subtitle">管理笔记</div>
              </div>
              <div class="action-button-overlay"></div>
            </NuxtLink>
            
            <NuxtLink to="/admin/analytics" class="action-button action-button-green">
              <div class="action-button-content">
                <div class="action-button-icon">📊</div>
                <div class="action-button-title">访客分析</div>
                <div class="action-button-subtitle">查看数据</div>
              </div>
              <div class="action-button-overlay"></div>
            </NuxtLink>
            
            <NuxtLink to="/admin/investment" class="action-button action-button-orange">
              <div class="action-button-content">
                <div class="action-button-icon">📈</div>
                <div class="action-button-title">投资仪表盘</div>
                <div class="action-button-subtitle">查看收益</div>
              </div>
              <div class="action-button-overlay"></div>
            </NuxtLink>
          </div>
        </div>

        <!-- 最近活动 -->
        <div class="action-section">
          <h2 class="section-title">
            <svg class="section-title-icon section-title-icon-purple" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            最近活动
          </h2>
          <div class="activity-list">
            <div class="activity-item">
              <div class="activity-icon activity-icon-blue">
                <span>📝</span>
              </div>
              <div class="activity-content">
                <div class="activity-title">文章管理</div>
                <div class="activity-desc">管理你的文章内容</div>
              </div>
              <NuxtLink to="/admin/articles" class="activity-link activity-link-blue">查看 →</NuxtLink>
            </div>
            
            <div class="activity-item">
              <div class="activity-icon activity-icon-purple">
                <span>⏰</span>
              </div>
              <div class="activity-content">
                <div class="activity-title">时间胶囊</div>
                <div class="activity-desc">审核访客留言</div>
              </div>
              <NuxtLink to="/admin/time-capsules" class="activity-link activity-link-purple">查看 →</NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- 热门页面和最近访问 -->
      <div class="data-grid">
        <!-- 热门页面 -->
        <div class="action-section">
          <h2 class="section-title">
            <svg class="section-title-icon section-title-icon-orange" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            热门页面
          </h2>
          <div class="top-paths-list">
            <div v-for="(path, index) in topPaths" :key="index" class="top-path-item">
              <div class="top-path-rank">{{ index + 1 }}</div>
              <div class="top-path-content">
                <div class="top-path-name">{{ formatPath(path.Path) }}</div>
                <div class="top-path-count">{{ path.Count }} 次访问</div>
              </div>
            </div>
            <div v-if="topPaths.length === 0" class="empty-state">暂无数据</div>
          </div>
        </div>

        <!-- 最近访问记录 -->
        <div class="action-section">
          <h2 class="section-title">
            <svg class="section-title-icon section-title-icon-cyan" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            最近访问
          </h2>
          <div class="recent-visits-list">
            <div v-for="(visit, index) in recentVisits.slice(0, 10)" :key="visit.Id" class="recent-visit-item">
              <div class="recent-visit-time">{{ formatTime(visit.Timestamp) }}</div>
              <div class="recent-visit-path">{{ formatPath(visit.Path) }}</div>
              <div class="recent-visit-ip">{{ visit.Ip || '未知' }}</div>
            </div>
            <div v-if="recentVisits.length === 0" class="empty-state">暂无访问记录</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const stats = ref({
  articleCount: 0,
  toolCount: 0,
  todayVisits: 0,
  totalVisits: 0,
  uniqueVisitors: 0,
  yesterdayVisits: 0,
  onlineCount: 0,
  pendingMessages: 0,
  pendingTasks: 0
})

const topPaths = ref<any[]>([])
const recentVisits = ref<any[]>([])
const visitTrend = ref<any[]>([])
const trendChart = ref<HTMLCanvasElement | null>(null)

const fetchStats = async () => {
  try {
    // 后端 Stats API 返回格式: { code: 0, data: { TotalVisits, TodayVisits, ArticleCount, ProjectCount, ... } }
    // 注意：Stats API 需要 [Authorize]，确保已登录
    const res = await api.get<any>('/Stats')
    // useApi 已经处理了响应格式，直接返回 data
    if (res) {
      stats.value.todayVisits = res.TodayVisits ?? res.todayVisits ?? 0
      stats.value.articleCount = res.ArticleCount ?? res.articleCount ?? 0
      stats.value.toolCount = res.ProjectCount ?? res.projectCount ?? 0
      stats.value.totalVisits = res.TotalVisits ?? res.totalVisits ?? 0
      stats.value.uniqueVisitors = res.UniqueVisitors ?? res.uniqueVisitors ?? 0
      stats.value.yesterdayVisits = res.YesterdayVisits ?? res.yesterdayVisits ?? 0
      stats.value.onlineCount = res.OnlineCount ?? res.onlineCount ?? 0
      stats.value.pendingMessages = res.PendingMessages ?? res.pendingMessages ?? 0
      stats.value.pendingTasks = res.PendingTasks ?? res.pendingTasks ?? 0
      
      topPaths.value = res.TopPaths ?? res.topPaths ?? []
      recentVisits.value = res.RecentVisits ?? res.recentVisits ?? []
      visitTrend.value = res.VisitTrend ?? res.visitTrend ?? []
      
      // 渲染图表
      if (visitTrend.value.length > 0) {
        setTimeout(() => {
          renderTrendChart()
        }, 100)
      }
    } else {
      // 如果返回空，使用默认值
      stats.value.todayVisits = 0
      stats.value.articleCount = 0
      stats.value.toolCount = 0
    }
  } catch (e: any) {
    console.error('Failed to fetch stats:', e)
    // 如果 API 返回错误（如 500），使用默认值，不阻塞页面显示
    stats.value.todayVisits = 0
    stats.value.articleCount = 0
    stats.value.toolCount = 0
  }
}

const renderTrendChart = () => {
  if (!trendChart.value || visitTrend.value.length === 0) return
  
  const ctx = trendChart.value.getContext('2d')
  if (!ctx) return

  const labels = visitTrend.value.map((item: any) => {
    const date = new Date(item.Date)
    return `${date.getMonth() + 1}/${date.getDate()}`
  })
  const visitData = visitTrend.value.map((item: any) => item.Count || 0)
  const visitorData = visitTrend.value.map((item: any) => item.UniqueVisitors || 0)

  // 简单的 Canvas 绘制（也可以使用 Chart.js）
  const width = trendChart.value.width || 800
  const height = trendChart.value.height || 300
  trendChart.value.width = width
  trendChart.value.height = height

  ctx.clearRect(0, 0, width, height)
  
  // 绘制背景
  ctx.fillStyle = 'rgba(255, 255, 255, 0.05)'
  ctx.fillRect(0, 0, width, height)

  // 计算最大值
  const maxValue = Math.max(...visitData, ...visitorData, 1)
  const padding = 40
  const chartWidth = width - padding * 2
  const chartHeight = height - padding * 2

  // 绘制网格线
  ctx.strokeStyle = 'rgba(255, 255, 255, 0.1)'
  ctx.lineWidth = 1
  for (let i = 0; i <= 5; i++) {
    const y = padding + (chartHeight / 5) * i
    ctx.beginPath()
    ctx.moveTo(padding, y)
    ctx.lineTo(width - padding, y)
    ctx.stroke()
  }

  // 绘制访问量折线
  ctx.strokeStyle = '#60a5fa'
  ctx.lineWidth = 2
  ctx.beginPath()
  visitData.forEach((value, index) => {
    const x = padding + (chartWidth / (visitData.length - 1 || 1)) * index
    const y = height - padding - (value / maxValue) * chartHeight
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()

  // 绘制访客数折线
  ctx.strokeStyle = '#86efac'
  ctx.lineWidth = 2
  ctx.beginPath()
  visitorData.forEach((value, index) => {
    const x = padding + (chartWidth / (visitorData.length - 1 || 1)) * index
    const y = height - padding - (value / maxValue) * chartHeight
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()

  // 绘制标签
  ctx.fillStyle = '#9ca3af'
  ctx.font = '12px sans-serif'
  ctx.textAlign = 'center'
  labels.forEach((label, index) => {
    const x = padding + (chartWidth / (labels.length - 1 || 1)) * index
    ctx.fillText(label, x, height - 10)
  })
}

const formatPath = (path: string) => {
  if (!path) return '未知页面'
  if (path === '/' || path === '') return '首页'
  return path.replace(/^\//, '').substring(0, 30) || '首页'
}

const formatTime = (timeStr: string) => {
  if (!timeStr) return '-'
  const date = new Date(timeStr)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  
  if (minutes < 1) return '刚刚'
  if (minutes < 60) return `${minutes}分钟前`
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `${hours}小时前`
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days}天前`
  
  return date.toLocaleString('zh-CN', {
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const currentTime = ref('')

const updateTime = () => {
  const now = new Date()
  currentTime.value = now.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

onMounted(() => {
  fetchStats()
  updateTime()
  setInterval(updateTime, 1000)
  // 每30秒刷新一次数据
  setInterval(fetchStats, 30000)
})
</script>

<style scoped>
/* 仪表盘容器 */
.admin-dashboard {
  min-height: 100vh;
}

/* 背景装饰 */
.dashboard-bg-decoration {
  position: fixed;
  inset: 0;
  overflow: hidden;
  pointer-events: none;
}

.bg-decoration-item {
  position: absolute;
  width: 24rem;
  height: 24rem;
  border-radius: 9999px;
  filter: blur(3rem);
}

.bg-decoration-blue {
  top: 0;
  right: 0;
  background-color: rgba(59, 130, 246, 0.03);
}

.bg-decoration-purple {
  bottom: 0;
  left: 0;
  background-color: rgba(168, 85, 247, 0.03);
}

/* 内容区域 */
.dashboard-content {
  position: relative;
  z-index: 10;
}

/* 头部样式 */
.dashboard-header {
  margin-bottom: 2rem;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  flex: 1;
}

.dashboard-title {
  font-size: 2.25rem;
  font-weight: 700;
  color: #f3f4f6;
  margin-bottom: 0.5rem;
}

.dashboard-subtitle {
  color: #9ca3af;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.status-dot {
  width: 0.5rem;
  height: 0.5rem;
  background-color: #86efac;
  border-radius: 9999px;
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.header-right {
  text-align: right;
}

.time-label {
  font-size: 0.875rem;
  color: #6b7280;
}

.time-value {
  font-size: 1.125rem;
  font-weight: 600;
  color: #e5e7eb;
}

/* 统计卡片网格 */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(1, minmax(0, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (min-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(3, minmax(0, 1fr));
  }
}

@media (min-width: 1024px) {
  .stats-grid {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }
}

/* 统计卡片 */
.stat-card {
  position: relative;
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(4px);
  padding: 1.5rem;
  border-radius: 1rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  overflow: hidden;
  transition: all 0.3s;
}

.stat-card:hover {
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.stat-card-decoration {
  position: absolute;
  top: 0;
  right: 0;
  width: 8rem;
  height: 8rem;
  border-radius: 9999px;
  margin-right: -4rem;
  margin-top: -4rem;
  transition: transform 0.5s;
}

.stat-card:hover .stat-card-decoration {
  transform: scale(1.5);
}

.stat-card-decoration-blue {
  background-color: rgba(59, 130, 246, 0.05);
}

.stat-card-decoration-purple {
  background-color: rgba(168, 85, 247, 0.05);
}

.stat-card-decoration-green {
  background-color: rgba(34, 197, 94, 0.05);
}

.stat-card-content {
  position: relative;
  z-index: 10;
}

.stat-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.stat-card-icon {
  width: 3rem;
  height: 3rem;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-card-icon svg {
  width: 1.5rem;
  height: 1.5rem;
}

.stat-card-icon-blue {
  background-color: rgba(59, 130, 246, 0.1);
  color: #60a5fa;
}

.stat-card-icon-purple {
  background-color: rgba(168, 85, 247, 0.1);
  color: #a78bfa;
}

.stat-card-icon-green {
  background-color: rgba(34, 197, 94, 0.1);
  color: #86efac;
}

.stat-card-label {
  font-size: 0.75rem;
  color: #9ca3af;
}

.stat-card-value {
  font-size: 2.25rem;
  font-weight: 700;
  margin-bottom: 0.25rem;
}

.stat-card-value-blue {
  color: #60a5fa;
}

.stat-card-value-purple {
  color: #a78bfa;
}

.stat-card-value-green {
  color: #86efac;
}

.stat-card-cyan {
  --stat-color: #06b6d4;
}

.stat-card-decoration-cyan {
  background-color: rgba(6, 182, 212, 0.05);
}

.stat-card-icon-cyan {
  background-color: rgba(6, 182, 212, 0.1);
  color: #22d3ee;
}

.stat-card-value-cyan {
  color: #22d3ee;
}

.stat-card-orange {
  --stat-color: #f97316;
}

.stat-card-decoration-orange {
  background-color: rgba(249, 115, 22, 0.05);
}

.stat-card-icon-orange {
  background-color: rgba(249, 115, 22, 0.1);
  color: #fb923c;
}

.stat-card-value-orange {
  color: #fb923c;
}

.stat-card-pink {
  --stat-color: #ec4899;
}

.stat-card-decoration-pink {
  background-color: rgba(236, 72, 153, 0.05);
}

.stat-card-icon-pink {
  background-color: rgba(236, 72, 153, 0.1);
  color: #f472b6;
}

.stat-card-value-pink {
  color: #f472b6;
}

.stat-card-link {
  color: #f472b6;
  text-decoration: none;
  transition: color 0.2s;
}

.stat-card-link:hover {
  color: #fb7185;
  text-decoration: underline;
}

.stat-card-desc {
  font-size: 0.875rem;
  color: #9ca3af;
}

/* 操作区域网格 */
.actions-grid {
  display: grid;
  grid-template-columns: repeat(1, minmax(0, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (min-width: 1024px) {
  .actions-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

/* 操作区域 */
.action-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(4px);
  border-radius: 1rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  padding: 1.5rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  color: #e5e7eb;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.section-title-icon {
  width: 1.25rem;
  height: 1.25rem;
  color: #60a5fa;
}

.section-title-icon-purple {
  color: #a78bfa;
}

/* 操作按钮网格 */
.action-buttons-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 1rem;
}

/* 操作按钮 */
.action-button {
  position: relative;
  overflow: hidden;
  background: linear-gradient(135deg, var(--button-color-start), var(--button-color-end));
  color: white;
  padding: 1rem;
  border-radius: 0.75rem;
  transition: all 0.3s;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  text-decoration: none;
}

.action-button:hover {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  transform: translateY(-0.25rem);
}

.action-button-blue {
  --button-color-start: #3b82f6;
  --button-color-end: #2563eb;
}

.action-button-blue:hover {
  --button-color-start: #2563eb;
  --button-color-end: #1d4ed8;
}

.action-button-purple {
  --button-color-start: #a855f7;
  --button-color-end: #9333ea;
}

.action-button-purple:hover {
  --button-color-start: #9333ea;
  --button-color-end: #7e22ce;
}

.action-button-green {
  --button-color-start: #22c55e;
  --button-color-end: #16a34a;
}

.action-button-green:hover {
  --button-color-start: #16a34a;
  --button-color-end: #15803d;
}

.action-button-orange {
  --button-color-start: #f97316;
  --button-color-end: #ea580c;
}

.action-button-orange:hover {
  --button-color-start: #ea580c;
  --button-color-end: #c2410c;
}

.action-button-content {
  position: relative;
  z-index: 10;
}

.action-button-icon {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.action-button-title {
  font-weight: 600;
}

.action-button-subtitle {
  font-size: 0.75rem;
  margin-top: 0.25rem;
  opacity: 0.9;
}

.action-button-overlay {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.2);
  transform: scale(0);
  transition: transform 0.3s;
}

.action-button:hover .action-button-overlay {
  transform: scale(1);
}

/* 活动列表 */
.activity-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.activity-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.activity-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 9999px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.activity-icon-blue {
  background-color: rgba(59, 130, 246, 0.1);
  color: #60a5fa;
}

.activity-icon-purple {
  background-color: rgba(168, 85, 247, 0.1);
  color: #a78bfa;
}

.activity-content {
  flex: 1;
}

.activity-title {
  font-weight: 500;
  color: #e5e7eb;
}

.activity-desc {
  font-size: 0.875rem;
  color: #9ca3af;
}

.activity-link {
  font-size: 0.875rem;
  text-decoration: none;
  transition: color 0.2s;
}

.activity-link:hover {
  text-decoration: underline;
}

.activity-link-blue {
  color: #60a5fa;
}

.activity-link-blue:hover {
  color: #93c5fd;
}

.activity-link-purple {
  color: #a78bfa;
}

.activity-link-purple:hover {
  color: #c4b5fd;
}

/* 图表区域 */
.chart-section {
  margin-bottom: 2rem;
}

.chart-container {
  height: 300px;
  position: relative;
}

.chart-container canvas {
  width: 100%;
  height: 100%;
}

/* 数据网格 */
.data-grid {
  display: grid;
  grid-template-columns: repeat(1, minmax(0, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (min-width: 1024px) {
  .data-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

/* 热门页面列表 */
.top-paths-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  max-height: 400px;
  overflow-y: auto;
}

.top-path-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.2s;
}

.top-path-item:hover {
  background: rgba(255, 255, 255, 0.08);
  transform: translateX(4px);
}

.top-path-rank {
  width: 2rem;
  height: 2rem;
  border-radius: 9999px;
  background: linear-gradient(135deg, #f97316, #ea580c);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 0.875rem;
  flex-shrink: 0;
}

.top-path-content {
  flex: 1;
  min-width: 0;
}

.top-path-name {
  font-weight: 500;
  color: #e5e7eb;
  margin-bottom: 0.25rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.top-path-count {
  font-size: 0.875rem;
  color: #9ca3af;
}

/* 最近访问列表 */
.recent-visits-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 400px;
  overflow-y: auto;
}

.recent-visit-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 0.5rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
  font-size: 0.875rem;
  transition: all 0.2s;
}

.recent-visit-item:hover {
  background: rgba(255, 255, 255, 0.08);
}

.recent-visit-time {
  color: #9ca3af;
  min-width: 80px;
  font-size: 0.75rem;
}

.recent-visit-path {
  flex: 1;
  color: #e5e7eb;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.recent-visit-ip {
  color: #6b7280;
  font-family: monospace;
  font-size: 0.75rem;
  min-width: 100px;
  text-align: right;
}

/* 空状态 */
.empty-state {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
  font-size: 0.875rem;
}

.section-title-icon-green {
  color: #86efac;
}

.section-title-icon-orange {
  color: #fb923c;
}

.section-title-icon-cyan {
  color: #22d3ee;
}
</style>

