<template>
  <div class="top-pages-content">
    <div class="top-pages-list">
      <div
        v-for="(path, index) in topPaths"
        :key="index"
        class="top-page-item"
      >
        <div class="page-rank rank-pill">{{ index + 1 }}</div>
        <div class="page-name">{{ formatPath(path.Path || path.path) }}</div>
        <div class="page-bar-wrapper">
          <div
            class="page-bar"
            :style="{ width: `${getBarWidth(path)}%` }"
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
</script>

<style scoped>
/* 只保留布局相关的样式，不写颜色 */
.top-pages-content {
  width: 100%;
}

.top-pages-list {
  display: flex;
  flex-direction: column;
}

.top-page-item {
  display: flex;
  align-items: center;
  height: 40px;
  gap: 12px;
  padding: 0 4px;
  border-bottom: 1px solid rgba(148, 163, 184, 0.20);
  transition: background-color 0.2s ease;
}

.top-page-item:hover {
  background-color: rgba(148, 163, 184, 0.06);
}

.top-page-item:last-child {
  border-bottom: none;
}

.page-rank {
  /* 使用全局 .rank-pill 样式，这里只保留必要的覆盖 */
  flex-shrink: 0;
}

.page-name {
  flex: 1;
  min-width: 0;
  font-weight: 500;
  font-size: 14px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-main, var(--n-text-color, #0f172a));
}

.page-bar-wrapper {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
  min-width: 0;
  max-width: 300px;
}

.page-bar {
  height: 4px;
  border-radius: 2px;
  flex: 1;
  min-width: 0;
  background: var(--color-primary);
  transition: width 0.3s ease;
}

.page-count {
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
  min-width: 60px;
  text-align: right;
  font-variant-numeric: tabular-nums;
  color: var(--color-text-main, var(--n-text-color, #0f172a));
  opacity: 0.85;
}

.empty-state {
  text-align: center;
  padding: 48px 16px;
  font-size: 14px;
  opacity: 0.5;
}
</style>
