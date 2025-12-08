<template>
  <div class="top-pages-card">
    <div class="card-header">
      <h3 class="card-title">热门页面</h3>
    </div>
    <div class="top-pages-list">
      <div
        v-for="(path, index) in topPaths"
        :key="index"
        class="top-page-item"
      >
        <div class="page-rank">{{ index + 1 }}</div>
        <div class="page-name">{{ formatPath(path.Path || path.path) }}</div>
        <div class="page-bar-wrapper">
          <div
            class="page-bar"
            :style="{ width: `${getBarWidth(path)}%` }"
            :class="`page-bar--${getBarColor(index)}`"
          ></div>
          <span class="page-count">{{ path.Count || path.count || 0 }} 次</span>
        </div>
      </div>
      <div v-if="topPaths.length === 0" class="empty-state">暂无数据</div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  topPaths: any[]
}

const props = withDefaults(defineProps<Props>(), {
  topPaths: () => []
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

const getMaxCount = () => {
  if (props.topPaths.length === 0) return 1
  return Math.max(...props.topPaths.map(p => p.Count || p.count || 0), 1)
}

const getBarWidth = (path: any) => {
  const maxCount = getMaxCount()
  const count = path.Count || path.count || 0
  return (count / maxCount) * 100
}

const getBarColor = (index: number) => {
  const colors = ['blue', 'green', 'purple', 'orange', 'cyan', 'yellow']
  return colors[index % colors.length]
}
</script>

<style scoped>
.top-pages-card {
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

.top-pages-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.top-page-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.03);
  border-radius: 0.75rem;
  transition: all 0.2s;
}

.top-page-item:hover {
  background: rgba(255, 255, 255, 0.06);
}

.page-rank {
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 0.875rem;
  flex-shrink: 0;
}

.page-name {
  flex: 1;
  min-width: 0;
  color: #e5e7eb;
  font-weight: 500;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.page-bar-wrapper {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex: 1;
  min-width: 0;
  max-width: 300px;
}

.page-bar {
  height: 0.5rem;
  border-radius: 0.25rem;
  flex: 1;
  min-width: 0;
  transition: width 0.3s ease;
}

.page-bar--blue {
  background: linear-gradient(90deg, #3b82f6, #60a5fa);
}

.page-bar--green {
  background: linear-gradient(90deg, #22c55e, #86efac);
}

.page-bar--purple {
  background: linear-gradient(90deg, #a855f7, #a78bfa);
}

.page-bar--orange {
  background: linear-gradient(90deg, #f97316, #fb923c);
}

.page-bar--cyan {
  background: linear-gradient(90deg, #06b6d4, #22d3ee);
}

.page-bar--yellow {
  background: linear-gradient(90deg, #eab308, #fbbf24);
}

.page-count {
  font-size: 0.875rem;
  color: #9ca3af;
  font-weight: 500;
  white-space: nowrap;
  min-width: 60px;
  text-align: right;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #6b7280;
  font-size: 0.875rem;
}
</style>

