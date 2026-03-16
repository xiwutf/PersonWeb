<template>
  <n-collapse arrow-placement="right" :default-expanded-names="['core']">
    <!-- 核心指标 -->
    <n-collapse-item title="核心指标" name="core">
      <div class="perf-metrics">
        <n-grid :x-gap="16" :y-gap="12" :cols="2">
          <!-- Web Vitals -->
          <div class="perf-card">
            <div class="perf-card-header">
              <h4 class="perf-title">Web Vitals</h4>
              <n-tag :type="getScoreType(metrics.score)" size="small">
                {{ metrics.score }}分
              </n-tag>
            </div>
            <div class="perf-metrics">
              <div class="perf-metric">
                <span class="perf-label">FCP</span>
                <span class="perf-value">
                  {{ formatValue(metrics.fcp) }}s
                  <span class="perf-target" :class="{ 'perf-target--pass': metrics.fcp < 1800, 'perf-target--fail': metrics.fcp >= 1800 }">
                    目标: < 1.8s
                  </span>
                </span>
              </div>
              <div class="perf-metric">
                <span class="perf-label">LCP</span>
                <span class="perf-value">
                  {{ formatValue(metrics.lcp) }}s
                  <span class="perf-target" :class="{ 'perf-target--pass': metrics.lcp < 2500, 'perf-target--fail': metrics.lcp >= 2500 }">
                    目标: < 2.5s
                  </span>
                </span>
              </div>
              <div class="perf-metric">
                <span class="perf-label">FID</span>
                <span class="perf-value">
                  {{ formatValue(metrics.fid) }}ms
                  <span class="perf-target" :class="{ 'perf-target--pass': metrics.fid < 100, 'perf-target--fail': metrics.fid >= 100 }">
                    目标: < 100ms
                  </span>
                </span>
              </div>
              <div class="perf-metric">
                <span class="perf-label">CLS</span>
                <span class="perf-value">
                  {{ formatValue(metrics.cls) }}
                  <span class="perf-target" :class="{ 'perf-target--pass': metrics.cls < 0.1, 'perf-target--fail': metrics.cls >= 0.1 }">
                    目标: < 0.1
                  </span>
                </span>
              </div>
            </div>
          </div>

          <!-- 评分等级 -->
          <div class="perf-card">
            <div class="perf-card-header">
              <h4 class="perf-title">评分等级</h4>
            </div>
            <div class="perf-score-grade">
              <div
                class="perf-grade-item"
                :class="`perf-grade--${metrics.grade}`"
              >
                <div class="grade-letter">{{ metrics.grade }}</div>
                <div class="grade-text">
                  <template v-if="metrics.grade === 'A'">优秀</template>
                  <template v-else-if="metrics.grade === 'B'">良好</template>
                  <template v-else-if="metrics.grade === 'C'">及格</template>
                  <template v-else>较差</template>
                </div>
              </div>
            </div>
          </div>
        </n-grid>

        <!-- 资源加载 -->
        <div class="perf-card">
          <div class="perf-card-header">
            <h4 class="perf-title">资源加载</h4>
          </div>
          <div class="perf-resource-list">
            <div class="perf-resource">
              <span class="perf-resource-label">JS 大小</span>
              <span class="perf-resource-value">
                {{ formatSize(resourceMetrics.jsSize) }}
                <span v-if="resourceMetrics.jsSize > 1000000" class="perf-resource-warn">
                  > 1MB
                </span>
              </span>
            </div>
            <div class="perf-resource">
              <span class="perf-resource-label">CSS 大小</span>
              <span class="perf-resource-value">
                {{ formatSize(resourceMetrics.cssSize) }}
              </span>
            </div>
            <div class="perf-resource">
              <span class="perf-resource-label">图片数量</span>
              <span class="perf-resource-value">
                {{ resourceMetrics.imageCount }}
              </span>
            </div>
            <div class="perf-resource">
              <span class="perf-resource-label">请求数量</span>
              <span class="perf-resource-value">
                {{ resourceMetrics.requestCount }}
              </span>
            </div>
          </div>
        </div>

        <!-- 运行时 -->
        <div class="perf-card">
          <div class="perf-card-header">
            <h4 class="perf-title">运行时</h4>
          </div>
          <div class="perf-metrics">
            <div class="perf-metric">
              <span class="perf-label">DOM 节点</span>
              <span class="perf-value">{{ runtimeMetrics.domNodes }}</span>
            </div>
            <div class="perf-metric">
              <span class="perf-label">组件数量</span>
              <span class="perf-value">{{ runtimeMetrics.componentCount }}</span>
            </div>
            <div class="perf-metric">
              <span class="perf-label">监听器</span>
              <span class="perf-value">{{ runtimeMetrics.listeners }}</span>
            </div>
            <div class="perf-metric">
              <span class="perf-label">内存使用</span>
span class="perf-value">
                {{ formatSize(runtimeMetrics.memoryUsed) }}
                /
                {{ formatSize(runtimeMetrics.memoryTotal) }}
                <n-progress
                  type="line"
                  :percentage="(runtimeMetrics.memoryUsed / runtimeMetrics.memoryTotal) * 100"
                  :color="getMemoryColor(runtimeMetrics.memoryUsed, runtimeMetrics.memoryTotal)"
                />
              </span>
            </div>
          </div>
        </div>
      </n-collapse-item>

    <!-- 自定义测量 -->
    <n-collapse-item title="自定义测量" name="custom">
      <div class="perf-measure-section">
        <div class="perf-measure-form">
          <n-space :vertical="true" :size="12">
            <n-input-group>
              <n-input
                v-model:value="measureName"
                placeholder="测量名称"
                clearable
              />
              <n-button type="primary" @click="startMeasure" :disabled="isMeasuring">
                {{ isMeasuring ? '停止' : '开始' }}
              </n-button>
            </n-input-group>
          </n-space>

          <div v-if="measureResult" class="perf-measure-result">
            <n-descriptions :column="1" bordered>
              <n-descriptions-item label="测量名称">
                {{ measureResult.name }}
              </n-descriptions-item>
              <n-descriptions-item label="耗时">
                <n-statistic label="" :value="`${measureResult.duration.toFixed(2)}ms`" />
              </n-descriptions-item>
            </n-descriptions>

            <n-button size="small" @click="clearMeasureResult">
              清除
            </n-button>
          </div>
        </div>
      </div>
    </n-collapse-item>

    <!-- 历史记录 -->
    <n-collapse-item title="历史记录" name="history">
      <div class="perf-history-section">
        <n-space vertical :size="8">
          <div
            v-for="(record, index) in historyRecords"
            :key="record.id"
            class="perf-history-item"
          >
            <div class="perf-history-header">
              <span class="perf-history-time">{{ formatTime(record.timestamp) }}</span>
              <span class="perf-history-score">
                评分: <n-tag :type="getScoreType(record.score)" size="small">
                  {{ record.score }}分
                </n-tag>
              </span>
              <n-button text size="tiny" @click="removeHistory(index)">
                删除
              </n-button>
            </div>
            <div class="perf-history-metrics">
              <div class="perf-history-metric">
                FCP: {{ formatValue(record.fcp) }}s
              </div>
              <div class="perf-history-metric">
                LCP: {{ formatValue(record.lcp) }}s
              </div>
              <div class="perf-history-metric">
                FID: {{ formatValue(record.fid) }}ms
              </div>
              <div class="perf-history-metric">
                CLS: {{ formatValue(record.cls) }}
              </div>
            </div>
          </div>
        </n-space>

        <n-button size="small" @click="clearHistory">
          清除全部
        </n-button>
      </div>
    </n-collapse-item>

    <!-- 性能建议 -->
    <n-collapse-item title="性能建议" name="suggestions">
      <div class="perf-suggestions">
        <n-alert v-for="(suggestion, index) in suggestions" :key="index" :type="suggestion.type">
          <template #icon>
            <n-icon
              v-if="suggestion.type === 'success'"
              :component="CheckCircleIcon"
            />
            <n-icon
              v-else-if="suggestion.type === 'warning'"
              :component="WarningIcon"
            />
            <n-icon
              v-else-if="suggestion.type === 'error'"
              :component="XCircleIcon"
            />
          </template>
          {{ suggestion.title }}
        </n-alert>
      </div>
    </n-collapse-item>

    <!-- 操作 -->
    <div class="perf-actions">
      <n-space>
        <n-button @click="refreshMetrics" :loading="isRefreshing">
          <template #icon>
            <n-icon :component="RefreshIcon" />
          </template>
          刷新指标
        </n-button>
        <n-button @click="exportReport">
          <template #icon>
            <n-icon :component="DownloadIcon" />
          </template>
          导出报告
        </n-button>
        <n-button @click="clearCache">
          <template #icon>
            <n-icon :component="TrashIcon" />
          </template>
          清除缓存
        </n-button>
      </n-space>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import {
  NCollapse,
  NGrid,
  NTag,
  NProgress,
  NButton,
  NSpace,
  NDescriptions,
  NDescriptionsItem,
  NStatistic,
  NAlert,
  NIcon
} from 'naive-ui'
import {
  CheckCircle as CheckCircleIcon,
  Warning as WarningIcon,
  XCircle as XCircleIcon,
  Refresh as RefreshIcon,
  Download as DownloadIcon,
  Trash as TrashIcon
} from '@vicons/ionicons5'
import { usePerformance } from '~/composables/usePerformance'

// 性能指标
const metrics = ref({
  score: 0,
  grade: 'A',
  fcp: 0,
  lcp: 0,
  fid: 0,
  cls: 0
})

// 资源指标
const resourceMetrics = ref({
  jsSize: 0,
  cssSize: 0,
  imageCount: 0,
  requestCount: 0
})

// 运行时指标
const runtimeMetrics = ref({
  domNodes: 0,
  componentCount: 0,
  listeners: 0,
  memoryUsed: 0,
  memoryTotal: 0
})

// 自定义测量
const measureName = ref('')
const isMeasuring = ref(false)
const measureResult = ref<{ name: string, duration: number } | null>(null)

// 历史记录
const historyRecords = ref<any[]>([])

// 刷新状态
const isRefreshing = ref(false)

// 性能建议
const suggestions = ref<Array<{
  type: 'success' | 'warning' | 'error'
  title: string
  description?: string
}>>([])

// 使用性能监控
const { checkScore, startMonitoring, generateReport } = usePerformance()

// 格式化数值
const formatValue = (value: number): string => {
  if (value === 0) return '0'
  if (value < 1000) return `${value.toFixed(0)}`
  if (value < 1000000) return `${(value / 1000).toFixed(1)}KB`
  return `${(value / 1000).toFixed(2)}MB`
}

const formatSize = (bytes: number): string => {
  if (bytes === 0) return '0 B'
  const units = ['B', 'KB', 'MB', 'GB']
  const unitIndex = Math.floor(Math.log(bytes) / Math.log(1024))
  return `${(bytes / Math.pow(1024, unitIndex)).toFixed(2)} ${units[unitIndex]}`
}

const formatTime = (timestamp: number): string => {
  const date = new Date(timestamp)
  return date.toLocaleTimeString()
}

// 获取评分类型
const getScoreType = (score: number): 'success' | 'warning' | 'error' => {
  if (score >= 90) return 'success'
  if (score >= 75) return 'warning'
  return 'error'
}

// 获取内存颜色
const getMemoryColor = (used: number, total: number): string => {
  const percentage = (used / total) * 100
  if (percentage < 60) return 'success'
  if (percentage < 80) return 'warning'
  return 'error'
}

// 刷新性能指标
const refreshMetrics = async () => {
  isRefreshing.value = true
  await new Promise(resolve => setTimeout(resolve, 500))

  try {
    // 获取 Web Vitals
    const vitals = await (usePerformance().globalMonitor as any).getWebVitals()
    metrics.value = {
      ...vitals,
      score: (usePerformance().globalMonitor as any).checkScore().score
    }

    // 模拟资源指标
    resourceMetrics.value = {
      jsSize: 1500000,
      cssSize: 850000,
      imageCount: 42,
      requestCount: 156
    }

    // 获取运行时指标
    runtimeMetrics.value = {
      domNodes: document.querySelectorAll('*').length,
      componentCount: getComponentCount(),
      listeners: getListenerCount(),
      memoryUsed: performance?.memory?.usedJSHeapSize || 50000000,
      memoryTotal: performance?.memory?.totalJSHeapSize || 100000000
    }

    // 生成建议
    generateSuggestions()

    // 保存到历史记录
    saveToHistory()

  } catch (error) {
    console.error('Failed to refresh metrics:', error)
  } finally {
    isRefreshing.value = false
  }
}

// 获取组件数量
const getComponentCount = (): number => {
  const app = document.getElementById('app') || document.body
  const components = app.querySelectorAll('[class*="n-"]')
  return components.length
}

// 获取监听器数量
const getListenerCount = (): number => {
  let count = 0
  // 遍历所有事件类型并统计监听器
  const eventTypes = Object.getOwnPropertyNames(window)
  for (const eventType of eventTypes) {
    try {
      const listeners = (window as any).getEventListeners?.(eventType)?.length || 0
      count += listeners
    } catch {
      // 某些事件类型可能无法获取
    }
  }
  return count
}

// 生成性能建议
const generateSuggestions = () => {
  const suggestions = []

  // FCP 检查
  if (metrics.value.fcp > 1800) {
    suggestions.push({
      type: 'warning',
      title: 'FCP 较慢',
      description: '首次内容绘制超过 1.8s，建议优化首屏关键资源'
    })
  }

  // LCP 检查
  if (metrics.value.lcp > 2500) {
    suggestions.push({
      type: 'warning',
      title: 'LCP 较慢',
      description: '最大内容绘制超过 2.5s，建议优化关键图片和字体加载'
    })
  }

  // FID 检查
  if (metrics.value.fid > 100) {
    suggestions.push({
      type: 'error',
      title: 'FID 较慢',
      description: '首次输入延迟超过 100ms，建议减少 JS 主线程阻塞'
    })
  }

  // CLS 检查
  if (metrics.value.cls > 0.1) {
    suggestions.push({
      type: 'warning',
      title: 'CLS 偏移',
      description: '累积布局偏移超过 0.1，建议减少动态元素和布局变化'
    })
  }

  // 资源检查
  if (resourceMetrics.value.jsSize > 2000000) {
    suggestions.push({
      type: 'warning',
      title: 'JS 体积较大',
      description: 'JavaScript 超过 2MB，建议进行代码分割和 tree shaking'
    })
  }

  // 内存检查
  const memoryPercent = (runtimeMetrics.value.memoryUsed / runtimeMetrics.value.memoryTotal) * 100
  if (memoryPercent > 80) {
    suggestions.push({
      type: 'error',
      title: '内存使用过高',
      description: `内存使用超过 80% (${formatSize(runtimeMetrics.value.memoryUsed)} / ${formatSize(runtimeMetrics.value.memoryTotal)})，建议检查内存泄漏`
    })
  }

  // 添加成功建议
  if (suggestions.length === 0) {
    suggestions.push({
      type: 'success',
      title: '性能表现良好',
      description: '所有指标都达到了目标值，继续保持！'
    })
  }

  suggestions.value = suggestions
}

// 开始自定义测量
const startMeasure = () => {
  if (!measureName.value.trim()) {
    return
  }

  isMeasuring.value = !isMeasuring.value
  measureResult.value = null

  const perf = usePerformance()
  const { globalMonitor } = perf

  // 标记开始
  globalMonitor.mark({ name: `custom-${measureName.value}-start` })

  // 执行测量（这里模拟）
  setTimeout(() => {
    globalMonitor.mark({ name: `custom-${measureName.value}-end` })
    const duration = globalMonitor.measure(measureName.value, `custom-${measureName.value}-start`, `custom-${measureName.value}-end`)

    measureResult.value = {
      name: measureName.value,
      duration
    }

    isMeasuring.value = !isMeasuring.value
  }, 1000)
}

// 清除测量结果
const clearMeasureResult = () => {
  measureResult.value = null
}

// 保存到历史
const saveToHistory = () => {
  const record = {
    id: Date.now(),
    timestamp: Date.now(),
    ...metrics.value,
    ...resourceMetrics.value,
    ...runtimeMetrics.value
  }

  historyRecords.value.unshift(record)

  // 只保留最近 50 条记录
  if (historyRecords.value.length > 50) {
    historyRecords.value = historyRecords.value.slice(0, 50)
  }

  try {
    localStorage.setItem('perf-history', JSON.stringify(historyRecords.value))
  } catch (e) {
    console.warn('Failed to save history:', e)
  }
}

// 从历史加载
const loadFromHistory = () => {
  try {
    const saved = localStorage.getItem('perf-history')
    if (saved) {
      historyRecords.value = JSON.parse(saved)
    }
  } catch (e) {
    console.warn('Failed to load history:', e)
  }
}

// 清除历史记录
const clearHistory = () => {
  historyRecords.value = []
  try {
    localStorage.removeItem('perf-history')
  } catch (e) {
    console.warn('Failed to clear history:', e)
  }
}

// 导出报告
const exportReport = () => {
  const report = generateReport()

  // 创建下载链接
  const blob = new Blob([report], { type: 'text/plain;charset=utf-8' })
  const url = URL.createObjectURL(blob)

  const a = document.createElement('a')
  a.href = url
  a.download = `performance-report-${Date.now()}.txt`
  document.body.appendChild(a)
  a.click()

  setTimeout(() => {
    document.body.removeChild(a)
    URL.revokeObjectURL(url)
  }, 100)
}

// 清除缓存
const clearCache = () => {
  if ('caches' in window) {
    caches.keys().forEach(cacheName => {
      caches.delete(cacheName)
    })
  }
}

// 组件挂载时
onMounted(() => {
  startMonitoring()
  loadFromHistory()
})

// 组件卸载时
onUnmounted(() => {
  // 注意：这里不停止监控，让页面保持监控
})
</script>

<style scoped>
.perf-metrics {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.perf-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg);
}

.perf-card-header {
  margin-bottom: var(--spacing-md);
  border-bottom: 1px solid var(--color-border-subtle);
  padding-bottom: var(--spacing-md);
}

.perf-title {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.perf-metrics {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.perf-metric {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-sm) 0;
  border-bottom: 1px solid var(--color-border-subtle);
}

.perf-metric:last-child {
  border-bottom: none;
}

.perf-label {
  font-size: var(--text-sm);
  color: var(--color-text-sec);
}

.perf-value {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main);
}

.perf-target {
  font-size: var(--text-xs);
  color: var(--color-text-muted);
}

.perf-target--pass {
  color: var(--color-success);
}

.perf-target--fail {
  color: var(--color-error);
}

.perf-score-grade {
  display: flex;
  gap: var(--spacing-md);
}

.perf-grade-item {
  flex: 1;
  text-align: center;
}

.grade-letter {
  font-size: 48px;
  font-weight: 700;
  width: 64px;
  height: 64px;
  border-radius: var(--radius-xl);
  line-height: 1;
  margin-bottom: var(--spacing-sm);
}

.perf-grade--A .grade-letter {
  background: var(--color-success);
  color: white;
}

.perf-grade--B .grade-letter {
  background: var(--color-warning);
  color: white;
}

.perf-grade--C .grade-letter {
  background: var(--color-error);
  color: white;
}

.perf-grade--D .grade-letter {
  background: var(--color-text-muted);
  color: white;
}

.grade-text {
  text-align: center;
  font-size: var(--text-sm);
}

/* 评分等级背景 */
.perf-grade--A {
  background: linear-gradient(135deg, var(--color-success) 0%, var(--color-success-600) 100%);
}

.perf-grade--B {
  background: linear-gradient(135deg, var(--color-warning) 0%, var(--color-warning-600) 100%);
}

.perf-grade--C {
  background: linear-gradient(135deg, var(--color-error) 0%, var(--color-error-600) 100%);
}

.perf-grade--D {
  background: var(--color-bg-elevated);
}

.perf-resource-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.perf-resource {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-sm) 0;
}

.perf-resource-label {
  font-size: var(--text-sm);
  color: var(--color-text-sec);
}

.perf-resource-value {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main);
}

.perf-resource-warn {
  color: var(--color-error);
  font-size: var(--text-xs);
  margin-left: var(--spacing-xs);
}

/* 自定义测量 */
.perf-measure-section {
  padding: var(--spacing-lg);
  background: var(--color-bg-elevated);
  border-radius: var(--radius-lg);
  margin-bottom: var(--spacing-xl);
}

.perf-measure-form {
  max-width: 400px;
}

.perf-measure-result {
  margin-top: var(--spacing-lg);
}

/* 历史记录 */
.perf-history-section {
  padding: var(--spacing-lg);
}

.perf-history-item {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
}

.perf-history-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-sm);
}

.perf-history-time {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
}

.perf-history-score {
  display: flex;
  gap: var(--spacing-sm);
  align-items: center;
}

.perf-history-metric {
  font-size: var(--text-sm);
  color: var(--color-text-sec);
}

/* 性能建议 */
.perf-suggestions {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

/* 操作按钮 */
.perf-actions {
  padding: var(--spacing-xl) 0;
  border-top: 1px solid var(--color-border-subtle);
  margin-top: var(--spacing-xl);
}

/* 响应式 */
@media (max-width: 768px) {
  .perf-score-grade {
    flex-direction: column;
    align-items: center;
  }

  .grade-letter {
    font-size: 36px;
    width: 48px;
    height: 48px;
  }

  .perf-metrics {
    gap: var(--spacing-xs);
  }

  .perf-metric {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
