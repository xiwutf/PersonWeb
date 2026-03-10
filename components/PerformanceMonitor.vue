<template>
  <div class="performance-monitor" :class="{ 'monitoring': isMonitoring }">
    <button
      class="monitor-toggle"
      @click="toggleMonitoring"
      :class="{ active: isMonitoring }"
    >
      <i class="fas fa-tachometer-alt"></i>
      <span class="monitor-text">{{ isMonitoring ? '监控中' : '监控' }}</span>
      <span v-if="metrics.fcp" class="metric-fps">FPS: {{ metrics.fps }}</span>
    </button>

    <!-- 性能指标面板 -->
    <Transition name="slide">
      <div v-if="isMonitoring" class="metrics-panel">
        <div class="metrics-header">
          <h3>性能指标</h3>
          <button class="close-btn" @click="stopMonitoring">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- Web Vitals -->
        <div class="metrics-section">
          <h4>Web Vitals</h4>
          <div class="metric-item">
            <span class="metric-label">LCP (最大内容渲染)</span>
            <span
              class="metric-value"
              :class="getMetricClass(metrics.lcp, 2500, 4000)"
            >
              {{ formatMetric(metrics.lcp) || '-' }}
            </span>
          </div>
          <div class="metric-item">
            <span class="metric-label">FID (首次输入延迟)</span>
            <span
              class="metric-value"
              :class="getMetricClass(metrics.fid, 100, 300)"
            >
              {{ formatMetric(metrics.fid) || '-' }}
            </span>
          </div>
          <div class="metric-item">
            <span class="metric-label">CLS (布局偏移)</span>
            <span
              class="metric-value"
              :class="getMetricClass(metrics.cls, 0.1, 0.25)"
            >
              {{ formatMetric(metrics.cls) || '-' }}
            </span>
          </div>
          <div class="metric-item">
            <span class="metric-label">FCP (首次内容渲染)</span>
            <span
              class="metric-value"
              :class="getMetricClass(metrics.fcp, 1800, 3000)"
            >
              {{ formatMetric(metrics.fcp) || '-' }}
            </span>
          </div>
        </div>

        <!-- 资源指标 -->
        <div class="metrics-section">
          <h4>资源加载</h4>
          <div class="metric-item">
            <span class="metric-label">页面加载时间</span>
            <span class="metric-value">{{ timing.navigationStart }}ms</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">DOM 加载完成</span>
            <span class="metric-value">{{ timing.domContentLoadedEventEnd }}ms</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">页面完全加载</span>
            <span class="metric-value">{{ timing.loadEventEnd }}ms</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">JS 资源大小</span>
            <span class="metric-value">{{ formatBytes(performance.jsSize || 0) }}</span>
          </div>
        </div>

        <!-- 内存使用 -->
        <div class="metrics-section">
          <h4>内存使用</h4>
          <div class="metric-item">
            <span class="metric-label">已用内存</span>
            <span class="metric-value">{{ formatBytes(memory.used) }}</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">总内存</span>
            <span class="metric-value">{{ formatBytes(memory.total) }}</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">内存使用率</span>
            <span
              class="metric-value"
              :class="getMetricClass(memory.used / memory.total * 100, 80, 95)"
            >
              {{ Math.round(memory.used / memory.total * 100) }}%
            </span>
          </div>
        </div>

        <!-- 网络信息 -->
        <div class="metrics-section">
          <h4>网络信息</h4>
          <div class="metric-item">
            <span class="metric-label">网络类型</span>
            <span class="metric-value">{{ connection.effectiveType || 'Unknown' }}</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">下行速度</span>
            <span class="metric-value">{{ connection.downlink || 0 }} Mbps</span>
          </div>
          <div class="metric-item">
            <span class="metric-label">往返时间</span>
            <span class="metric-value">{{ connection.rtt || 0 }} ms</span>
          </div>
        </div>

        <!-- 性能建议 -->
        <div class="performance-tips">
          <h4>优化建议</h4>
          <ul>
            <li v-if="metrics.lcp > 2500">
              <i class="fas fa-exclamation-triangle"></i>
              LCP 时间过长，考虑优化首屏资源加载
            </li>
            <li v-if="metrics.fid > 100">
              <i class="fas fa-exclamation-triangle"></i>
              FID 时间过长，考虑优化 JavaScript 执行
            </li>
            <li v-if="metrics.cls > 0.1">
              <i class="fas fa-exclamation-triangle"></i>
              CLS 过大，考虑为图片和视频预留空间
            </li>
            <li v-if="memory.used / memory.total > 0.8">
              <i class="fas fa-exclamation-triangle"></i>
              内存使用率过高，考虑清理未使用的数据
            </li>
            <li v-if="connection.effectiveType === 'slow-2g' || connection.effectiveType === '2g'">
              <i class="fas fa-info-circle"></i>
              当前网络较慢，建议优化资源加载
            </li>
          </ul>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'

// 状态
const isMonitoring = ref(false)
const metrics = ref({
  fcp: 0,
  lcp: 0,
  fid: 0,
  cls: 0,
  fps: 0
})
const timing = ref({
  navigationStart: 0,
  domContentLoadedEventEnd: 0,
  loadEventEnd: 0
})
const memory = ref({
  used: 0,
  total: 0
})
const connection = ref({
  effectiveType: '',
  downlink: 0,
  rtt: 0
})
const frameCount = ref(0)
const lastFrameTime = ref(0)
const fpsInterval = ref<number>()

// 初始化性能监控
onMounted(() => {
  // 获取基础性能指标
  if ('performance' in window) {
    const perfData = performance.timing
    timing.value = {
      navigationStart: perfData.navigationStart,
      domContentLoadedEventEnd: perfData.domContentLoadedEventEnd - perfData.navigationStart,
      loadEventEnd: perfData.loadEventEnd - perfData.navigationStart
    }

    // 计算 JS 资源大小
    if (performance.getEntriesByType) {
      const scripts = performance.getEntriesByType('script')
      let totalSize = 0
      scripts.forEach(script => {
        if (script.decodedBodySize) {
          totalSize += script.decodedBodySize
        }
      })
      ;(performance as any).jsSize = totalSize
    }
  }

  // 获取内存信息（如果可用）
  if ('memory' in performance) {
    memory.value = {
      used: performance.memory!.usedJSHeapSize,
      total: performance.memory!.totalJSHeapSize
    }
  }

  // 获取网络信息（如果可用）
  if ('connection' in navigator) {
    const conn = (navigator as any).connection
    connection.value = {
      effectiveType: conn.effectiveType,
      downlink: conn.downlink,
      rtt: conn.rtt
    }
  }
})

// 开始监控
const startMonitoring = () => {
  if (isMonitoring.value) return

  isMonitoring.value = true

  // 使用 Performance API 监控
  if ('PerformanceObserver' in window) {
    // 监控 FCP
    const fcpObserver = new PerformanceObserver((list) => {
      for (const entry of list.getEntries()) {
        metrics.value.fcp = entry.startTime
      }
    })
    fcpObserver.observe({ entryTypes: ['paint'] })

    // 监控 LCP
    const lcpObserver = new PerformanceObserver((list) => {
      const entries = list.getEntries()
      const lastEntry = entries[entries.length - 1]
      metrics.value.lcp = lastEntry.startTime
    })
    lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] })

    // 监控 CLS
    const clsObserver = new PerformanceObserver((list) => {
      let clsValue = 0
      list.getEntries().forEach((entry) => {
        if (!entry.hadRecentInput) {
          clsValue += entry.value
        }
      })
      metrics.value.cls = clsValue
    })
    clsObserver.observe({ entryTypes: ['layout-shift'] })

    // 监控 FID
    const fidObserver = new PerformanceObserver((list) => {
      const firstInput = list.getEntries()[0]
      if (firstInput) {
        metrics.value.fid = firstInput.processingStart - firstInput.startTime
      }
    })
    fidObserver.observe({ entryTypes: ['first-input'] })
  }

  // 监控 FPS
  startFPSMonitoring()

  // 更新网络信息
  if ('connection' in navigator) {
    setInterval(() => {
      const conn = (navigator as any).connection
      connection.value = {
        effectiveType: conn.effectiveType,
        downlink: conn.downlink,
        rtt: conn.rtt
      }
    }, 5000)
  }

  // 更新内存信息
  if ('memory' in performance) {
    setInterval(() => {
      memory.value = {
        used: performance.memory!.usedJSHeapSize,
        total: performance.memory!.totalJSHeapSize
      }
    }, 5000)
  }
}

// 停止监控
const stopMonitoring = () => {
  isMonitoring.value = false
  if (fpsInterval.value) {
    cancelAnimationFrame(fpsInterval.value)
  }
}

// 切换监控状态
const toggleMonitoring = () => {
  if (isMonitoring.value) {
    stopMonitoring()
  } else {
    startMonitoring()
  }
}

// FPS 监控
const startFPSMonitoring = () => {
  frameCount.value = 0
  lastFrameTime.value = performance.now()

  const calculateFPS = () => {
    frameCount.value++
    const now = performance.now()
    const delta = now - lastFrameTime.value

    if (delta >= 1000) {
      const fps = Math.round((frameCount.value * 1000) / delta)
      metrics.value.fps = fps
      frameCount.value = 0
      lastFrameTime.value = now
    }

    if (isMonitoring.value) {
      fpsInterval.value = requestAnimationFrame(calculateFPS)
    }
  }

  fpsInterval.value = requestAnimationFrame(calculateFPS)
}

// 工具函数
const formatMetric = (value: number) => {
  if (!value) return null
  return `${value.toFixed(0)}ms`
}

const formatBytes = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const getMetricClass = (value: number, good: number, poor: number) => {
  if (!value) return ''
  if (value <= good) return 'good'
  if (value <= poor) return 'needs-improvement'
  return 'poor'
}
</script>

<style scoped>
.performance-monitor {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 1000;
}

.monitor-toggle {
  background: #374151;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 10px 16px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.monitor-toggle:hover {
  background: #4b5563;
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}

.monitor-toggle.active {
  background: #10b981;
}

.monitor-text {
  font-weight: 500;
}

.metric-fps {
  background: rgba(255, 255, 255, 0.2);
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 12px;
  min-width: 50px;
  text-align: center;
}

.metrics-panel {
  position: absolute;
  bottom: 60px;
  right: 0;
  width: 400px;
  max-height: 80vh;
  background: white;
  border-radius: 12px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.metrics-header {
  background: #374151;
  color: white;
  padding: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.metrics-header h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: background 0.3s ease;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.1);
}

.metrics-section {
  padding: 16px;
  border-bottom: 1px solid #e5e7eb;
}

.metrics-section:last-child {
  border-bottom: none;
}

.metrics-section h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.metric-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  font-size: 14px;
}

.metric-item:last-child {
  margin-bottom: 0;
}

.metric-label {
  color: #6b7280;
}

.metric-value {
  font-weight: 600;
  color: #374151;
}

.metric-value.good {
  color: #10b981;
}

.metric-value.needs-improvement {
  color: #f59e0b;
}

.metric-value.poor {
  color: #ef4444;
}

.performance-tips {
  padding: 16px;
  background: #f9fafb;
  flex: 1;
  overflow-y: auto;
}

.performance-tips h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.performance-tips ul {
  margin: 0;
  padding: 0;
  list-style: none;
}

.performance-tips li {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  margin-bottom: 8px;
  font-size: 13px;
  color: #6b7280;
}

.performance-tips li i {
  font-size: 14px;
  margin-top: 2px;
}

.performance-tips li.warning i {
  color: #f59e0b;
}

.performance-tips li.danger i {
  color: #ef4444;
}

.performance-tips li.info i {
  color: #3b82f6;
}

.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
}

.slide-enter-from {
  opacity: 0;
  transform: translateY(10px);
}

.slide-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .performance-monitor {
    bottom: 10px;
    right: 10px;
  }

  .metrics-panel {
    width: 350px;
    bottom: 70px;
    right: -10px;
  }

  .monitor-toggle {
    padding: 8px 12px;
    font-size: 12px;
  }

  .metric-fps {
    display: none;
  }
}
</style>