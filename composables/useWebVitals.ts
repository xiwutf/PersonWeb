/**
 * Web Vitals 集成 Composable
 *
 * 集成 web-vitals 库，收集真实用户的性能指标
 */

import { ref, onMounted, onUnmounted } from 'vue'

// Web Vitals 指标类型
export interface WebVitalsMetrics {
  // 核心指标
  fcp?: number           // First Contentful Paint
  lcp?: number           // Largest Contentful Paint
  fid?: number           // First Input Delay
  cls?: number           // Cumulative Layout Shift
  ttfb?: number          // Time to First Byte

  // 辅助指标
  tbt?: number           // Total Blocking Time
  inp?: number           // Interaction to Next Paint

  // 导航指标
  navigationType?: string
  redirectCount?: number

  // 资源指标
  resourceCount?: number
  resourceSize?: number
}

// 性能评分
export interface PerformanceScore {
  overall: number
  grade: 'A' | 'B' | 'C' | 'D'
  metrics: {
    fcp: { score: number; grade: 'A' | 'B' | 'C' | 'D' | 'N/A' }
    lcp: { score: number; grade: 'A' | 'B' | 'C' | 'D' | 'N/A' }
    fid: { score: number; grade: 'A' | 'B' | 'C' | 'D' | 'N/A' }
    cls: { score: number; grade: 'A' | 'B' | 'C' | 'D' | 'N/A' }
  }
}

// 性能目标值
const PERFORMANCE_TARGETS = {
  fcp: { good: 1800, needsImprovement: 3000 },  // ms
  lcp: { good: 2500, needsImprovement: 4000 },  // ms
  fid: { good: 100, needsImprovement: 300 },    // ms
  cls: { good: 0.1, needsImprovement: 0.25 },  // score
  ttfb: { good: 600, needsImprovement: 1500 }, // ms
  inp: { good: 200, needsImprovement: 500 }    // ms
}

/**
 * Web Vitals Hook
 */
export function useWebVitals() {
  const metrics = ref<WebVitalsMetrics>({})
  const isReady = ref(false)
  const isCollecting = ref(false)
  const error = ref<string | null>(null)

  // 动态导入 web-vitals
  let webVitals: any = null

  /**
   * 加载 web-vitals 库
   */
  const loadWebVitals = async () => {
    if (webVitals) return webVitals

    try {
      webVitals = await import('web-vitals')
      return webVitals
    } catch (err) {
      error.value = 'Failed to load web-vitals library'
      console.error('Failed to load web-vitals:', err)
      return null
    }
  }

  /**
   * 处理指标
   */
  const handleMetric = (metric: any) => {
    if (!metric || !metric.value) return

    const value = metric.value
    const name = metric.name

    switch (name) {
      case 'FCP':
        metrics.value.fcp = value
        break
      case 'LCP':
        metrics.value.lcp = value
        break
      case 'FID':
        metrics.value.fid = value
        break
      case 'CLS':
        metrics.value.cls = value
        break
      case 'TTFB':
        metrics.value.ttfb = value
        break
      case 'INP':
        metrics.value.inp = value
        break
      case 'TBT':
        metrics.value.tbt = value
        break
    }
  }

  /**
   * 计算单个指标的评分
   */
  const calculateMetricScore = (value: number | undefined, target: any) => {
    if (value === undefined) return { score: 0, grade: 'N/A' as const }

    let score: number
    let grade: 'A' | 'B' | 'C' | 'D'

    if (value <= target.good) {
      score = 100
      grade = 'A'
    } else if (value <= target.needsImprovement) {
      score = Math.round(100 - ((value - target.good) / (target.needsImprovement - target.good)) * 40)
      grade = score >= 75 ? 'B' : 'C'
    } else {
      score = Math.max(0, Math.round(60 - ((value - target.needsImprovement) / target.needsImprovement) * 60))
      grade = score >= 40 ? 'C' : 'D'
    }

    return { score, grade }
  }

  /**
   * 计算性能评分
   */
  const calculateScore = (): PerformanceScore => {
    const fcpScore = calculateMetricScore(metrics.value.fcp, PERFORMANCE_TARGETS.fcp)
    const lcpScore = calculateMetricScore(metrics.value.lcp, PERFORMANCE_TARGETS.lcp)
    const fidScore = calculateMetricScore(metrics.value.fid, PERFORMANCE_TARGETS.fid)
    const clsScore = calculateMetricScore(metrics.value.cls, PERFORMANCE_TARGETS.cls)

    const validScores = [fcpScore, lcpScore, fidScore, clsScore].filter(s => s.grade !== 'N/A')

    if (validScores.length === 0) {
      return {
        overall: 0,
        grade: 'D',
        metrics: { fcp: fcpScore, lcp: lcpScore, fid: fidScore, cls: clsScore }
      }
    }

    const overall = Math.round(validScores.reduce((sum, s) => sum + s.score, 0) / validScores.length)

    let grade: 'A' | 'B' | 'C' | 'D'
    if (overall >= 90) grade = 'A'
    else if (overall >= 75) grade = 'B'
    else if (overall >= 60) grade = 'C'
    else grade = 'D'

    return {
      overall,
      grade,
      metrics: {
        fcp: fcpScore,
        lcp: lcpScore,
        fid: fidScore,
        cls: clsScore
      }
    }
  }

  /**
   * 收集导航指标
   */
  const collectNavigationMetrics = () => {
    if (!import.meta.client) return

    const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming

    if (navigation) {
      metrics.value.ttfb = navigation.responseStart - navigation.requestStart
      metrics.value.navigationType = navigation.type
      metrics.value.redirectCount = navigation.redirectCount
    }
  }

  /**
   * 收集资源指标
   */
  const collectResourceMetrics = () => {
    if (!import.meta.client) return

    const resources = performance.getEntriesByType('resource') as PerformanceResourceTiming[]
    const size = resources.reduce((sum, r) => sum + (r.transferSize || 0), 0)

    metrics.value.resourceCount = resources.length
    metrics.value.resourceSize = size
  }

  /**
   * 上报指标到分析服务
   */
  const reportMetrics = (customEndpoint?: string) => {
    if (Object.keys(metrics.value).length === 0) {
      console.warn('No metrics to report')
      return
    }

    const score = calculateScore()
    const report = {
      url: window.location.href,
      timestamp: Date.now(),
      userAgent: navigator.userAgent,
      metrics: metrics.value,
      score
    }

    // 默认控制台输出
    console.log('Web Vitals Report:', report)

    // 自定义上报
    if (customEndpoint) {
      fetch(customEndpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(report)
      }).catch(err => {
        console.error('Failed to report metrics:', err)
      })
    }

    // 存储到 localStorage 用于分析
    try {
      const history = JSON.parse(localStorage.getItem('web-vitals-history') || '[]')
      history.push(report)
      // 只保留最近 100 条记录
      if (history.length > 100) {
        history.shift()
      }
      localStorage.setItem('web-vitals-history', JSON.stringify(history))
    } catch (err) {
      console.warn('Failed to store metrics history:', err)
    }

    return report
  }

  /**
   * 收集所有指标
   */
  const collectMetrics = async () => {
    if (!import.meta.client) {
      console.warn('Web Vitals collection only works on client side')
      return
    }

    isCollecting.value = true
    error.value = null

    try {
      const lib = await loadWebVitals()
      if (!lib) {
        return
      }

      // 收集导航和资源指标
      collectNavigationMetrics()
      collectResourceMetrics()

      // 收集核心 Web Vitals
      const vitals = [
        { name: 'onFCP', fn: lib.onFCP },
        { name: 'onLCP', fn: lib.onLCP },
        { name: 'onFID', fn: lib.onFID },
        { name: 'onCLS', fn: lib.onCLS },
        { name: 'onTTFB', fn: lib.onTTFB },
        { name: 'onINP', fn: lib.onINP }
      ]

      vitals.forEach(({ fn }) => {
        fn(handleMetric)
      })

      isReady.value = true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
      console.error('Failed to collect metrics:', err)
    } finally {
      isCollecting.value = false
    }
  }

  /**
   * 获取历史记录
   */
  const getHistory = () => {
    try {
      const history = JSON.parse(localStorage.getItem('web-vitals-history') || '[]')
      return history as Array<{
        url: string
        timestamp: number
        metrics: WebVitalsMetrics
        score: PerformanceScore
      }>
    } catch (err) {
      console.warn('Failed to get history:', err)
      return []
    }
  }

  /**
   * 清除历史记录
   */
  const clearHistory = () => {
    try {
      localStorage.removeItem('web-vitals-history')
    } catch (err) {
      console.warn('Failed to clear history:', err)
    }
  }

  /**
   * 获取统计摘要
   */
  const getStats = () => {
    const history = getHistory()
    if (history.length === 0) return null

    const fcpValues = history.filter(h => h.metrics.fcp).map(h => h.metrics.fcp!)
    const lcpValues = history.filter(h => h.metrics.lcp).map(h => h.metrics.lcp!)
    const fidValues = history.filter(h => h.metrics.fid).map(h => h.metrics.fid!)
    const clsValues = history.filter(h => h.metrics.cls).map(h => h.metrics.cls!)

    const calculateStats = (values: number[]) => {
      if (values.length === 0) return null
      const sorted = [...values].sort((a, b) => a - b)
      return {
        min: sorted[0],
        max: sorted[sorted.length - 1],
        avg: values.reduce((sum, v) => sum + v, 0) / values.length,
        p75: sorted[Math.floor(sorted.length * 0.75)],
        p95: sorted[Math.floor(sorted.length * 0.95)]
      }
    }

    return {
      fcp: calculateStats(fcpValues),
      lcp: calculateStats(lcpValues),
      fid: calculateStats(fidValues),
      cls: calculateStats(clsValues),
      totalSamples: history.length,
      dateRange: {
        start: history[0]?.timestamp,
        end: history[history.length - 1]?.timestamp
      }
    }
  }

  // 组件挂载时自动收集指标
  onMounted(() => {
    if (import.meta.client) {
      collectMetrics()
    }
  })

  return {
    metrics,
    isReady,
    isCollecting,
    error,
    calculateScore,
    reportMetrics,
    collectMetrics,
    getHistory,
    clearHistory,
    getStats
  }
}

/**
 * 性能目标常量（供外部使用）
 */
export const PERFORMANCE_TARGETS_CONSTANTS = PERFORMANCE_TARGETS

/**
 * 评分等级常量（供外部使用）
 */
export const SCORE_GRADES = {
  A: { min: 90, label: '优秀', color: '#22c55e' },
  B: { min: 75, label: '良好', color: '#eab308' },
  C: { min: 60, label: '一般', color: '#f97316' },
  D: { min: 0, label: '需改进', color: '#ef4444' }
}
