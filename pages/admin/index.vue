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

        <!-- 访问数卡片 -->
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
            <div class="stat-card-desc">次访问</div>
          </div>
        </div>
      </div>

      <!-- 快捷操作 -->
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
  todayVisits: 0
})

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
</style>

