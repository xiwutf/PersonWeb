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

      <!-- 加载状态 -->
      <div v-if="isLoading" class="dashboard-loading">
        <div class="loading-spinner"></div>
        <p class="loading-text">正在加载数据...</p>
      </div>

      <!-- 1. 核心 KPI 区 -->
      <AdminDashboardKpiRow v-else :kpis="kpiData" />

      <!-- 2. 业务统计区 -->
      <AdminDashboardStatsGrid v-if="!isLoading" :stats="statsData" />

      <!-- 3. 数据可视化区 -->
      <AdminDashboardTrendAndSource v-if="!isLoading" :visit-trend="visitTrend" />

      <!-- 4. 热门页面 -->
      <AdminDashboardTopPagesCard v-if="!isLoading" :top-paths="topPaths" />

      <!-- 5. 最近访问 -->
      <AdminDashboardRecentVisitsCard v-if="!isLoading" :recent-visits="recentVisits" />

      <!-- 6. 快捷操作 + 最近活动 -->
      <div v-if="!isLoading" class="actions-section">
        <AdminDashboardQuickActions :actions="quickActions" />
        <AdminDashboardTimeline :items="timelineItems" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
// 显式导入组件，确保 Nuxt3 自动导入正常工作
import AdminDashboardKpiRow from '~/components/admin/dashboard/KpiRow.vue'
import AdminDashboardStatsGrid from '~/components/admin/dashboard/StatsGrid.vue'
import AdminDashboardTrendAndSource from '~/components/admin/dashboard/TrendAndSource.vue'
import AdminDashboardTopPagesCard from '~/components/admin/dashboard/TopPagesCard.vue'
import AdminDashboardRecentVisitsCard from '~/components/admin/dashboard/RecentVisitsCard.vue'
import AdminDashboardQuickActions from '~/components/admin/dashboard/QuickActions.vue'
import AdminDashboardTimeline from '~/components/admin/dashboard/Timeline.vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false // 禁用 SSR，提升加载速度
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
  pendingTasks: 0,
  pendingOrders: 0,
  pendingConsultations: 0,
  totalOrders: 0,
  totalConsultations: 0,
  todayOrders: 0,
  todayConsultations: 0
})

const topPaths = ref<any[]>([])
const recentVisits = ref<any[]>([])
const visitTrend = ref<any[]>([])
const isLoading = ref(true) // 加载状态

// 计算核心 KPI 数据
const kpiData = computed(() => {
  const todayVisitsTrend = stats.value.yesterdayVisits > 0
    ? ((stats.value.todayVisits - stats.value.yesterdayVisits) / stats.value.yesterdayVisits) * 100
    : null

  return [
    {
      key: 'todayVisits',
      label: '今日访问量',
      value: stats.value.todayVisits,
      icon: '👁️',
      color: 'blue' as const,
      trend: todayVisitsTrend
    },
    {
      key: 'todayConsultations',
      label: '今日咨询数',
      value: stats.value.todayConsultations || stats.value.pendingConsultations,
      icon: '💬',
      color: 'green' as const,
      trend: null
    },
    {
      key: 'todayOrders',
      label: '今日订单数',
      value: stats.value.todayOrders || stats.value.pendingOrders,
      icon: '🛒',
      color: 'yellow' as const,
      trend: null
    },
    {
      key: 'onlineCount',
      label: '在线人数',
      value: stats.value.onlineCount,
      icon: '🟢',
      color: 'orange' as const,
      trend: null
    }
  ]
})

// 计算业务统计数据
const statsData = computed(() => {
  return [
    {
      key: 'totalVisits',
      label: '总访问量',
      value: stats.value.totalVisits,
      desc: '累计访问',
      iconPath: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
      color: 'cyan' as const
    },
    {
      key: 'uniqueVisitors',
      label: '独立访客',
      value: stats.value.uniqueVisitors,
      desc: '去重访客',
      iconPath: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
      color: 'blue' as const
    },
    {
      key: 'articleCount',
      label: '总文章数',
      value: stats.value.articleCount,
      desc: '内容数量',
      iconPath: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z',
      color: 'purple' as const
    },
    {
      key: 'toolCount',
      label: '项目数',
      value: stats.value.toolCount,
      desc: '工具/项目',
      iconPath: 'M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10',
      color: 'green' as const
    },
    {
      key: 'totalOrders',
      label: '总订单数',
      value: stats.value.totalOrders,
      desc: '订单统计',
      iconPath: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
      color: 'yellow' as const
    },
    {
      key: 'totalConsultations',
      label: '总咨询数',
      value: stats.value.totalConsultations,
      desc: '咨询统计',
      iconPath: 'M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z',
      color: 'teal' as const
    }
  ]
})

// 快捷操作数据
const quickActions = computed(() => [
  { path: '/admin/articles/edit', icon: '✍️', label: '发布新文章' },
  { path: '/admin/knowledge', icon: '📚', label: '知识库' },
  { path: '/admin/analytics', icon: '📊', label: '访客分析' },
  { path: '/admin/investment', icon: '📈', label: '投资仪表盘' },
  { path: '/admin/orders', icon: '🛒', label: '订单管理' },
  { path: '/admin/consultations', icon: '💬', label: '咨询管理' }
])

// 时间线数据
const timelineItems = computed(() => [
  {
    path: '/admin/articles',
    icon: '📝',
    title: '文章管理',
    desc: '管理你的文章内容',
    color: 'blue' as const
  },
  {
    path: '/admin/time-capsules',
    icon: '⏰',
    title: '时间胶囊',
    desc: '审核访客留言',
    color: 'purple' as const
  },
  {
    path: '/admin/orders',
    icon: '🛒',
    title: '订单管理',
    desc: stats.value.pendingOrders > 0 ? `${stats.value.pendingOrders} 个待处理` : '管理所有订单',
    color: 'yellow' as const
  },
  {
    path: '/admin/consultations',
    icon: '💬',
    title: '咨询管理',
    desc: stats.value.pendingConsultations > 0 ? `${stats.value.pendingConsultations} 条新咨询` : '管理客户咨询',
    color: 'teal' as const
  }
])

const fetchStats = async () => {
  try {
    isLoading.value = true
    const res = await api.get<any>('/Stats')
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
      
      // 处理热门路径数据
      const topPathsData = res.TopPaths ?? res.topPaths ?? []
      topPaths.value = Array.isArray(topPathsData) 
        ? topPathsData.filter((p: any) => p && (p.Path || p.path))
        : []
      
      // 处理最近访问数据
      const recentVisitsData = res.RecentVisits ?? res.recentVisits ?? []
      recentVisits.value = Array.isArray(recentVisitsData) ? recentVisitsData : []
      
      // 处理访问趋势数据
      const visitTrendData = res.VisitTrend ?? res.visitTrend ?? []
      visitTrend.value = Array.isArray(visitTrendData) ? visitTrendData : []
      
      // 并行获取所有统计数据，提升加载速度
      await Promise.all([
        fetchPendingTimeCapsules(),
        fetchOrderStats(),
        fetchConsultationStats()
      ])
    }
  } catch (e: any) {
    console.error('Failed to fetch stats:', e)
  } finally {
    isLoading.value = false
  }
}

const fetchPendingTimeCapsules = async () => {
  try {
    const res = await api.get<any>('/TimeCapsule/stats')
    if (res) {
      // 可以在这里处理时间胶囊数据
    }
  } catch (e: any) {
    if (process.dev) {
      console.warn('获取时间胶囊统计数据失败:', e)
    }
  }
}

const fetchOrderStats = async () => {
  try {
    const today = new Date()
    today.setHours(0, 0, 0, 0)
    
    // 并行获取所有订单统计数据
    const [pendingRes, totalRes, todayRes] = await Promise.all([
      // 待处理订单数
      api.get<any>('/admin/orders', {
        params: { status: 0, page: 1, pageSize: 1 }
      }),
      // 总订单数
      api.get<any>('/admin/orders', {
        params: { page: 1, pageSize: 1 }
      }),
      // 今日订单（只获取第一页，减少数据量）
      api.get<any>('/admin/orders', {
        params: { page: 1, pageSize: 50 } // 减少到 50 条，通常今日订单不会超过这个数
      })
    ])
    
    if (pendingRes) {
      stats.value.pendingOrders = pendingRes.Total ?? pendingRes.total ?? 0
    }
    
    if (totalRes) {
      stats.value.totalOrders = totalRes.Total ?? totalRes.total ?? 0
    }
    
    // 计算今日订单数
    if (todayRes) {
      const list = todayRes.List ?? todayRes.list ?? (Array.isArray(todayRes) ? todayRes : [])
      const todayOrders = list.filter((order: any) => {
        if (!order.CreatedAt && !order.createdAt) return false
        const orderDate = new Date(order.CreatedAt || order.createdAt)
        return orderDate >= today
      })
      stats.value.todayOrders = todayOrders.length
    }
  } catch (e: any) {
    if (process.dev) {
      console.warn('获取订单统计数据失败:', e)
    }
  }
}

const fetchConsultationStats = async () => {
  try {
    const today = new Date()
    today.setHours(0, 0, 0, 0)
    
    // 并行获取所有咨询统计数据
    const [pendingRes, totalRes, todayRes] = await Promise.all([
      // 待处理咨询数
      api.get<any>('/admin/consultations', {
        params: { status: 0, page: 1, pageSize: 1 }
      }),
      // 总咨询数
      api.get<any>('/admin/consultations', {
        params: { page: 1, pageSize: 1 }
      }),
      // 今日咨询（只获取第一页，减少数据量）
      api.get<any>('/admin/consultations', {
        params: { page: 1, pageSize: 50 } // 减少到 50 条，通常今日咨询不会超过这个数
      })
    ])
    
    if (pendingRes) {
      stats.value.pendingConsultations = pendingRes.Total ?? pendingRes.total ?? 0
    }
    
    if (totalRes) {
      stats.value.totalConsultations = totalRes.Total ?? totalRes.total ?? 0
    }
    
    // 计算今日咨询数
    if (todayRes) {
      const list = todayRes.List ?? todayRes.list ?? (Array.isArray(todayRes) ? todayRes : [])
      const todayConsultations = list.filter((consultation: any) => {
        if (!consultation.CreatedAt && !consultation.createdAt) return false
        const consultationDate = new Date(consultation.CreatedAt || consultation.createdAt)
        return consultationDate >= today
      })
      stats.value.todayConsultations = todayConsultations.length
    }
  } catch (e: any) {
    if (process.dev) {
      console.warn('获取咨询统计数据失败:', e)
    }
  }
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
  setInterval(fetchStats, 30000)
  
})
</script>

<style scoped>
/* 仪表盘容器 */
.admin-dashboard {
  min-height: 100vh;
  position: relative;
}

/* 背景装饰 */
.dashboard-bg-decoration {
  position: fixed;
  inset: 0;
  overflow: hidden;
  pointer-events: none;
  z-index: 0;
}

.bg-decoration-item {
  position: absolute;
  width: 24rem;
  height: 24rem;
  border-radius: 9999px;
  filter: blur(3rem);
  opacity: 0.3;
}

.bg-decoration-blue {
  top: 0;
  right: 0;
  background: radial-gradient(circle at center, rgba(59, 130, 246, 0.15) 0%, transparent 70%);
  animation: pulse-blue 4s ease-in-out infinite;
}

.bg-decoration-purple {
  bottom: 0;
  left: 0;
  background: radial-gradient(circle at center, rgba(168, 85, 247, 0.15) 0%, transparent 70%);
  animation: pulse-purple 4s ease-in-out infinite 2s;
}

@keyframes pulse-blue {
  0%, 100% {
    opacity: 0.3;
    transform: scale(1);
  }
  50% {
    opacity: 0.5;
    transform: scale(1.1);
  }
}

@keyframes pulse-purple {
  0%, 100% {
    opacity: 0.3;
    transform: scale(1);
  }
  50% {
    opacity: 0.5;
    transform: scale(1.1);
  }
}

/* 加载状态 */
.dashboard-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  gap: 1rem;
}

.loading-spinner {
  width: 48px;
  height: 48px;
  border: 4px solid rgba(59, 130, 246, 0.2);
  border-top-color: rgba(59, 130, 246, 1);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

.loading-text {
  color: rgba(255, 255, 255, 0.7);
  font-size: 14px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* 内容区域 */
.dashboard-content {
  position: relative;
  z-index: 10;
  max-width: 100%;
  width: 100%;
  box-sizing: border-box;
  padding: 2rem 1rem;
}

@media (min-width: 768px) {
  .dashboard-content {
    padding: 2rem 2rem;
  }
}

@media (min-width: 1024px) {
  .dashboard-content {
    padding: 2rem 3rem;
  }
}

/* 头部样式 */
.dashboard-header {
  margin-bottom: 2rem;
  width: 100%;
  box-sizing: border-box;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
}

.header-left {
  flex: 1;
}

.dashboard-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #f3f4f6;
  margin-bottom: 0.5rem;
}

@media (min-width: 640px) {
  .dashboard-title {
    font-size: 2.25rem;
  }
}

.dashboard-subtitle {
  color: #9ca3af;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.status-dot {
  width: 0.5rem;
  height: 0.5rem;
  background-color: #86efac;
  border-radius: 9999px;
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.header-right {
  text-align: right;
  flex-shrink: 0;
}

@media (max-width: 640px) {
  .header-content {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .header-right {
    text-align: left;
    width: 100%;
  }
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

/* 操作区域 */
.actions-section {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (min-width: 1024px) {
  .actions-section {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
