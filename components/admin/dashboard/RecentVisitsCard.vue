<template>
  <div class="recent-visits-card">
    <div class="card-header">
      <h3 class="card-title">最近访问</h3>
    </div>
    <div class="visits-list">
      <div
        v-for="(visit, index) in recentVisits.slice(0, 10)"
        :key="visit.Id || visit.id || index"
        class="visit-item"
      >
        <div class="visit-time">{{ formatTime(visit.Timestamp || visit.timestamp) }}</div>
        <div class="visit-path">{{ formatPath(visit.Path || visit.path) }}</div>
        <div class="visit-ip">{{ visit.Ip || visit.ip || '未知' }}</div>
      </div>
      <div v-if="recentVisits.length === 0" class="empty-state">暂无访问记录</div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  recentVisits: any[]
}

const props = withDefaults(defineProps<Props>(), {
  recentVisits: () => []
})

const formatPath = (path: string | null | undefined) => {
  if (!path || path === '') return '首页'
  if (path === '/') return '首页'

  const cleanPath = path.replace(/^\//, '')
  if (!cleanPath) return '首页'

  if (cleanPath.startsWith('blog/')) {
    const slug = cleanPath.replace('blog/', '')
    return slug ? `博客: ${slug.substring(0, 20)}` : '博客列表'
  }
  if (cleanPath.startsWith('tools/')) {
    const tool = cleanPath.replace('tools/', '')
    return tool ? `工具: ${tool.substring(0, 20)}` : '工具列表'
  }
  if (cleanPath.startsWith('ai/')) {
    return 'AI助手'
  }
  if (cleanPath.startsWith('projects/')) {
    const project = cleanPath.replace('projects/', '')
    return project ? `项目: ${project.substring(0, 20)}` : '项目列表'
  }
  if (cleanPath.startsWith('lab')) {
    return '实验室'
  }
  if (cleanPath.startsWith('admin')) {
    return '管理后台'
  }

  return cleanPath.substring(0, 30) || '首页'
}

const formatTime = (timeStr: string | null | undefined) => {
  if (!timeStr) return '未知'

  try {
    const date = new Date(timeStr)
    if (isNaN(date.getTime())) {
      return '未知'
    }

    const now = new Date()
    const diff = now.getTime() - date.getTime()

    if (diff < 0) {
      return date.toLocaleString('zh-CN', {
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
      })
    }

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
  } catch (e) {
    console.warn('时间格式化失败:', timeStr, e)
    return '未知'
  }
}
</script>

<style scoped>
.recent-visits-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.5rem;
  margin-bottom: 2rem;
}

.card-header {
  margin-bottom: 1.5rem;
}

.card-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #f3f4f6;
}

.visits-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-height: 400px;
  overflow-y: auto;
}

.visit-item {
  display: grid;
  grid-template-columns: 100px 1fr 120px;
  gap: 1rem;
  padding: 0.625rem 0.75rem;
  background: rgba(255, 255, 255, 0.02);
  border-radius: 0.5rem;
  font-size: 0.875rem;
  transition: all 0.2s;
}

.visit-item:hover {
  background: rgba(255, 255, 255, 0.05);
}

.visit-time {
  color: #6b7280;
  font-size: 0.75rem;
  white-space: nowrap;
}

.visit-path {
  color: #e5e7eb;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.visit-ip {
  color: #9ca3af;
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
  font-size: 0.75rem;
  text-align: right;
  white-space: nowrap;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #6b7280;
  font-size: 0.875rem;
}

@media (max-width: 640px) {
  .visit-item {
    grid-template-columns: 1fr;
    gap: 0.25rem;
  }

  .visit-ip {
    text-align: left;
  }
}
</style>

