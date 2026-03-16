<template>
  <div class="bundle-analyzer">
    <div class="bundle-analyzer-header">
      <h2 class="bundle-analyzer-title">Bundle 分析</h2>
      <div class="bundle-analyzer-actions">
        <n-button @click="refreshAnalysis" :loading="loading" secondary>
          <template #icon>
            <n-icon :component="RefreshIcon" />
          </template>
          刷新
        </n-button>
        <n-button @click="exportReport" secondary>
          <template #icon>
            <n-icon :component="DownloadIcon" />
          </template>
          导出
        </n-button>
      </div>
    </div>

    <!-- 概览统计 -->
    <div class="bundle-overview">
      <div class="bundle-stat">
        <div class="bundle-stat-label">总大小</div>
        <div class="bundle-stat-value">{{ formatSize(totalSize) }}</div>
        <div class="bundle-stat-sub">{{ totalChunks }} 个 chunk</div>
      </div>
      <div class="bundle-stat">
        <div class="bundle-stat-label">JS 大小</div>
        <div class="bundle-stat-value">{{ formatSize(jsSize) }}</div>
        <div class="bundle-stat-sub">{{ jsChunks }} 个文件</div>
      </div>
      <div class="bundle-stat">
        <div class="bundle-stat-label">CSS 大小</div>
        <div class="bundle-stat-value">{{ formatSize(cssSize) }}</div>
        <div class="bundle-stat-sub">{{ cssChunks }} 个文件</div>
      </div>
      <div class="bundle-stat">
        <div class="bundle-stat-label">其他资源</div>
        <div class="bundle-stat-value">{{ formatSize(otherSize) }}</div>
        <div class="bundle-stat-sub">{{ otherChunks }} 个文件</div>
      </div>
    </div>

    <!-- 最大的 chunk -->
    <div v-if="largestChunks.length > 0" class="bundle-section">
      <h3 class="bundle-section-title">
        <n-icon :component="ChartIcon" />
        最大的 chunk (前 10)
      </h3>
      <div class="chunk-list">
        <div
          v-for="(chunk, index) in largestChunks"
          :key="index"
          class="chunk-item"
          :style="{ '--chunk-size': `${chunk.percentage}%` }"
        >
          <div class="chunk-info">
            <div class="chunk-name">{{ chunk.name }}</div>
            <div class="chunk-size">{{ formatSize(chunk.size) }}</div>
          </div>
          <div class="chunk-bar">
            <div class="chunk-bar-fill" :style="{ width: `${chunk.percentage}%` }" />
          </div>
        </div>
      </div>
    </div>

    <!-- 按类型分组 -->
    <div class="bundle-section">
      <h3 class="bundle-section-title">
        <n-icon :component="FolderIcon" />
        按类型分组
      </h3>
      <div class="bundle-grid">
        <div
          v-for="(group, key) in groupedChunks"
          :key="key"
          class="bundle-group"
          @click="toggleGroup(key)"
        >
          <div class="bundle-group-header">
            <n-icon :component="expandedGroups.has(key) ? ChevronDownIcon : ChevronRightIcon" />
            <span class="bundle-group-name">{{ key }}</span>
            <span class="bundle-group-size">{{ formatSize(group.total) }}</span>
          </div>
          <div v-if="expandedGroups.has(key)" class="bundle-group-content">
            <div
              v-for="(chunk, index) in group.chunks"
              :key="index"
              class="bundle-group-item"
            >
              <span class="bundle-group-item-name">{{ chunk.name }}</span>
              <span class="bundle-group-item-size">{{ formatSize(chunk.size) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 依赖分析 -->
    <div class="bundle-section">
      <h3 class="bundle-section-title">
        <n-icon :component="PackageIcon" />
        依赖分析
      </h3>
      <div class="dependency-list">
        <div
          v-for="(dep, index) in topDependencies"
          :key="index"
          class="dependency-item"
        >
          <div class="dependency-info">
            <div class="dependency-name">{{ dep.name }}</div>
            <div class="dependency-count">{{ dep.count } 个文件</div>
          </div>
          <div class="dependency-size">{{ formatSize(dep.size) }}</div>
        </div>
      </div>
    </div>

    <!-- 优化建议 -->
    <div class="bundle-section">
      <h3 class="bundle-section-title">
        <n-icon :component="BulbIcon" />
        优化建议
      </h3>
      <div class="suggestions-list">
        <div
          v-for="(suggestion, index) in suggestions"
          :key="index"
          class="suggestion-item"
          :class="`suggestion-item--${suggestion.severity}`"
        >
          <n-icon :component="getSuggestionIcon(suggestion.severity)" />
          <div class="suggestion-content">
            <div class="suggestion-title">{{ suggestion.title }}</div>
            <div class="suggestion-description">{{ suggestion.description }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { NButton, NIcon } from 'naive-ui'
import {
  Refresh as RefreshIcon,
  Download as DownloadIcon,
  BarChart as ChartIcon,
  Folder as FolderIcon,
  ChevronDown as ChevronDownIcon,
  ChevronForward as ChevronRightIcon,
  Cube as PackageIcon,
  Bulb as BulbIcon,
  CheckmarkCircle as SuccessIcon,
  AlertTriangle as WarningIcon,
  CloseCircle as ErrorIcon,
  InformationCircle as InfoIcon
} from '@vicons/ionicons5'

interface ChunkInfo {
  name: string
  size: number
  type: 'js' | 'css' | 'other'
  percentage: number
}

interface DependencyInfo {
  name: string
  size: number
  count: number
}

interface Suggestion {
  severity: 'info' | 'warning' | 'error'
  title: string
  description: string
}

const loading = ref(false)
const expandedGroups = ref<Set<string>>(new Set())

// 模拟数据（实际应从构建输出读取）
const chunks = ref<ChunkInfo[]>([
  { name: 'app-[hash].js', size: 180000, type: 'js', percentage: 45 },
  { name: 'naive-ui-[hash].js', size: 150000, type: 'js', percentage: 37.5 },
  { name: 'echarts-[hash].js', size: 120000, type: 'js', percentage: 30 },
  { name: 'chartjs-[hash].js', size: 80000, type: 'js', percentage: 20 },
  { name: 'index-[hash].css', size: 60000, type: 'css', percentage: 15 },
  { name: 'three-[hash].js', size: 50000, type: 'js', percentage: 12.5 },
  { name: 'other-[hash].js', size: 40000, type: 'js', percentage: 10 },
  { name: 'vendors-[hash].js', size: 35000, type: 'js', percentage: 8.75 }
])

// 计算属性
const totalSize = computed(() => chunks.value.reduce((sum, c) => sum + c.size, 0))
const jsChunks = computed(() => chunks.value.filter(c => c.type === 'js'))
const cssChunks = computed(() => chunks.value.filter(c => c.type === 'css'))
const otherChunks = computed(() => chunks.value.filter(c => c.type === 'other'))
const jsSize = computed(() => jsChunks.value.reduce((sum, c) => sum + c.size, 0))
const cssSize = computed(() => cssChunks.value.reduce((sum, c) => sum + c.size, 0))
const otherSize = computed(() => otherChunks.value.reduce((sum, c) => sum + c.size, 0))
const totalChunks = computed(() => chunks.value.length)

// 最大的 chunk
const largestChunks = computed(() => {
  return [...chunks.value]
    .sort((a, b) => b.size - a.size)
    .slice(0, 10)
    .map(chunk => ({
      ...chunk,
      percentage: (chunk.size / totalSize.value) * 100
    }))
})

// 按类型分组
const groupedChunks = computed(() => {
  const groups: Record<string, { total: number; chunks: ChunkInfo[] }> = {}

  chunks.value.forEach(chunk => {
    const type = chunk.type.toUpperCase()
    if (!groups[type]) {
      groups[type] = { total: 0, chunks: [] }
    }
    groups[type].total += chunk.size
    groups[type].chunks.push(chunk)
  })

  return groups
})

// 依赖分析（模拟）
const topDependencies = ref<DependencyInfo[]>([
  { name: 'naive-ui', size: 150000, count: 1 },
  { name: 'echarts', size: 120000, count: 1 },
  { name: 'chart.js', size: 80000, count: 1 },
  { name: 'three.js', size: 50000, count: 1 },
  { name: 'vue', size: 45000, count: 3 }
])

// 优化建议
const suggestions = ref<Suggestion[]>([
  {
    severity: 'error',
    title: 'echarts chunk 较大',
    description: '考虑按需引入 ECharts 组件，使用 ECharts GL 按需加载 3D 图表'
  },
  {
    severity: 'warning',
    title: 'Naive UI 可以优化',
    description: '使用按需引入减少打包体积，配置组件级别的 Tree Shaking'
  },
  {
    severity: 'info',
    title: '建议使用代码分割',
    description: '对路由级别的组件使用动态导入，实现按需加载'
  }
])

// 格式化大小
function formatSize(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// 刷新分析
async function refreshAnalysis() {
  loading.value = true
  try {
    // 这里应该读取实际的构建分析结果
    // 目前使用模拟数据
    await new Promise(resolve => setTimeout(resolve, 1000))
  } catch (error) {
    console.error('刷新分析失败:', error)
  } finally {
    loading.value = false
  }
}

// 导出报告
function exportReport() {
  const report = {
    timestamp: new Date().toISOString(),
    totalSize: totalSize.value,
    jsSize: jsSize.value,
    cssSize: cssSize.value,
    otherSize: otherSize.value,
    totalChunks: totalChunks.value,
    largestChunks: largestChunks.value,
    topDependencies: topDependencies.value,
    suggestions: suggestions.value
  }

  const blob = new Blob([JSON.stringify(report, null, 2)], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = `bundle-analysis-${Date.now()}.json`
  link.click()
  URL.revokeObjectURL(url)
}

// 切换分组展开状态
function toggleGroup(key: string) {
  if (expandedGroups.value.has(key)) {
    expandedGroups.value.delete(key)
  } else {
    expandedGroups.value.add(key)
  }
  expandedGroups.value = new Set(expandedGroups.value)
}

// 获取建议图标
function getSuggestionIcon(severity: string) {
  switch (severity) {
    case 'error': return ErrorIcon
    case 'warning': return WarningIcon
    case 'info': return InfoIcon
    default: return SuccessIcon
  }
}

// 组件挂载
onMounted(() => {
  refreshAnalysis()
})
</script>

<style scoped>
.bundle-analyzer {
  padding: 24px;
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
}

.bundle-analyzer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.bundle-analyzer-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: var(--color-text-main);
}

.bundle-analyzer-actions {
  display: flex;
  gap: 12px;
}

.bundle-overview {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 32px;
}

.bundle-stat {
  padding: 20px;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border-default);
}

.bundle-stat-label {
  font-size: 14px;
  color: var(--color-text-muted);
  margin-bottom: 8px;
}

.bundle-stat-value {
  font-size: 28px;
  font-weight: 700;
  color: var(--color-text-main);
  margin-bottom: 4px;
}

.bundle-stat-sub {
  font-size: 12px;
  color: var(--color-text-muted);
}

.bundle-section {
  margin-bottom: 32px;
}

.bundle-section-title {
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 0 0 16px 0;
  font-size: 18px;
  font-weight: 600;
  color: var(--color-text-main);
}

.chunk-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.chunk-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.chunk-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.chunk-name {
  font-size: 14px;
  font-weight: 500;
  color: var(--color-text-main);
  font-family: monospace;
}

.chunk-size {
  font-size: 14px;
  color: var(--color-text-muted);
  font-weight: 500;
}

.chunk-bar {
  height: 8px;
  background: var(--color-bg-dark);
  border-radius: var(--radius-full);
  overflow: hidden;
}

.chunk-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--color-primary), var(--color-accent));
  border-radius: var(--radius-full);
  transition: width 0.3s;
}

.bundle-grid {
  display: grid;
  gap: 12px;
}

.bundle-group {
  padding: 16px;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-md);
  border: 1px solid var(--color-border-default);
  cursor: pointer;
  transition: all 0.2s;
}

.bundle-group:hover {
  border-color: var(--color-primary);
}

.bundle-group-header {
  display: flex;
  align-items: center;
  gap: 8px;
}

.bundle-group-name {
  flex: 1;
  font-size: 14px;
  font-weight: 500;
  color: var(--color-text-main);
}

.bundle-group-size {
  font-size: 14px;
  color: var(--color-text-muted);
  font-weight: 500;
}

.bundle-group-content {
  margin-top: 12px;
  padding-left: 24px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.bundle-group-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 13px;
}

.bundle-group-item-name {
  color: var(--color-text-muted);
  font-family: monospace;
}

.bundle-group-item-size {
  color: var(--color-text-muted);
}

.dependency-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.dependency-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-md);
  border: 1px solid var(--color-border-default);
}

.dependency-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.dependency-name {
  font-size: 14px;
  font-weight: 500;
  color: var(--color-text-main);
}

.dependency-count {
  font-size: 12px;
  color: var(--color-text-muted);
}

.dependency-size {
  font-size: 16px;
  font-weight: 600;
  color: var(--color-primary);
}

.suggestions-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.suggestion-item {
  display: flex;
  gap: 12px;
  padding: 16px;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-md);
  border-left: 4px solid var(--color-border-default);
}

.suggestion-item--info {
  border-left-color: var(--color-info);
}

.suggestion-item--warning {
  border-left-color: var(--color-warning);
}

.suggestion-item--error {
  border-left-color: var(--color-error);
}

.suggestion-content {
  display: flex;
  flex-direction: column;
  gap: 4px;
  flex: 1;
}

.suggestion-title {
  font-size: 14px;
  font-weight: 500;
  color: var(--color-text-main);
}

.suggestion-description {
  font-size: 13px;
  color: var(--color-text-muted);
}
</style>
