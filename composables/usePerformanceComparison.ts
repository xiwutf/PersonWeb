/**
 * 性能对比工具 Composable
 *
 * 用于对比不同时间点的性能指标，识别性能回归和改进
 */

import { ref, computed } from 'vue'

// 性能快照类型
export interface PerformanceSnapshot {
  id: string
  timestamp: number
  label: string
  metrics: {
    fcp?: number
    lcp?: number
    fid?: number
    cls?: number
    ttfb?: number
    inp?: number
    tbt?: number
  }
  resources: {
    jsSize: number
    cssSize: number
    totalSize: number
    requestCount: number
  }
  score: {
    overall: number
    grade: 'A' | 'B' | 'C' | 'D'
  }
  metadata?: {
    branch?: string
    commit?: string
    environment?: string
    device?: string
    browser?: string
  }
}

// 性能对比结果
export interface ComparisonResult {
  metric: string
  baseline: number | null
  current: number | null
  diff: number | null
  diffPercent: number | null
  status: 'improved' | 'regressed' | 'neutral' | 'no-data'
  significance: 'high' | 'medium' | 'low' | 'none'
}

// 性能对比报告
export interface ComparisonReport {
  baseline: PerformanceSnapshot | null
  current: PerformanceSnapshot
  results: ComparisonResult[]
  summary: {
    improved: number
    regressed: number
    neutral: number
    overallStatus: 'improved' | 'regressed' | 'neutral'
  }
}

// 回归检测阈值
const REGRESSION_THRESHOLDS = {
  fcp: { warning: 100, critical: 500 },        // ms
  lcp: { warning: 200, critical: 1000 },       // ms
  fid: { warning: 50, critical: 100 },         // ms
  cls: { warning: 0.01, critical: 0.05 },     // score
  ttfb: { warning: 100, critical: 300 },       // ms
  inp: { warning: 100, critical: 200 },        // ms
  tbt: { warning: 50, critical: 100 },         // ms
  jsSize: { warning: 10240, critical: 51200 },  // bytes (10KB/50KB)
  cssSize: { warning: 5120, critical: 20480 }, // bytes (5KB/20KB)
  totalSize: { warning: 20480, critical: 102400 } // bytes (20KB/100KB)
}

/**
 * 性能对比 Hook
 */
export function usePerformanceComparison() {
  const snapshots = ref<PerformanceSnapshot[]>([])
  const currentSnapshot = ref<PerformanceSnapshot | null>(null)
  const baselineSnapshot = ref<PerformanceSnapshot | null>(null)
  const comparisonResult = ref<ComparisonReport | null>(null)

  /**
   * 创建性能快照
   */
  const createSnapshot = (label: string, metrics: PerformanceSnapshot['metrics'], metadata?: PerformanceSnapshot['metadata']): PerformanceSnapshot => {
    const snapshot: PerformanceSnapshot = {
      id: `snapshot-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
      timestamp: Date.now(),
      label,
      metrics,
      resources: {
        jsSize: collectResourceSizes().js,
        cssSize: collectResourceSizes().css,
        totalSize: collectResourceSizes().total,
        requestCount: collectResourceSizes().count
      },
      score: calculateScore(metrics),
      metadata
    }

    return snapshot
  }

  /**
   * 收集资源大小
   */
  const collectResourceSizes = () => {
    if (typeof performance === 'undefined') {
      return { js: 0, css: 0, total: 0, count: 0 }
    }

    const resources = performance.getEntriesByType('resource') as PerformanceResourceTiming[]
    let js = 0, css = 0, total = 0

    for (const resource of resources) {
      const size = resource.transferSize || 0
      total += size

      if (resource.name.endsWith('.js') || resource.name.endsWith('.mjs')) {
        js += size
      } else if (resource.name.endsWith('.css')) {
        css += size
      }
    }

    return { js, css, total, count: resources.length }
  }

  /**
   * 计算性能评分
   */
  const calculateScore = (metrics: PerformanceSnapshot['metrics']) => {
    const targets = {
      fcp: { good: 1800, needsImprovement: 3000 },
      lcp: { good: 2500, needsImprovement: 4000 },
      fid: { good: 100, needsImprovement: 300 },
      cls: { good: 0.1, needsImprovement: 0.25 }
    }

    const getMetricScore = (value: number | undefined, target: any) => {
      if (value === undefined) return 0
      if (value <= target.good) return 100
      if (value <= target.needsImprovement) {
        return Math.round(100 - ((value - target.good) / (target.needsImprovement - target.good)) * 40)
      }
      return Math.max(0, Math.round(60 - ((value - target.needsImprovement) / target.needsImprovement) * 60))
    }

    const scores = [
      getMetricScore(metrics.fcp, targets.fcp),
      getMetricScore(metrics.lcp, targets.lcp),
      getMetricScore(metrics.fid, targets.fid),
      getMetricScore(metrics.cls, targets.cls)
    ].filter(s => s > 0)

    if (scores.length === 0) return { overall: 0, grade: 'D' as const }

    const overall = Math.round(scores.reduce((sum, s) => sum + s, 0) / scores.length)

    let grade: 'A' | 'B' | 'C' | 'D'
    if (overall >= 90) grade = 'A'
    else if (overall >= 75) grade = 'B'
    else if (overall >= 60) grade = 'C'
    else grade = 'D'

    return { overall, grade }
  }

  /**
   * 保存快照
   */
  const saveSnapshot = (snapshot: PerformanceSnapshot) => {
    snapshots.value.push(snapshot)
    saveToStorage()
  }

  /**
   * 设置基线快照
   */
  const setBaseline = (snapshot: PerformanceSnapshot) => {
    baselineSnapshot.value = snapshot
    saveToStorage()
  }

  /**
   * 设置当前快照
   */
  const setCurrent = (snapshot: PerformanceSnapshot) => {
    currentSnapshot.value = snapshot
    saveToStorage()
  }

  /**
   * 执行性能对比
   */
  const compare = (current: PerformanceSnapshot, baseline?: PerformanceSnapshot): ComparisonReport => {
    const results: ComparisonResult[] = []

    // 比较核心指标
    const metricsToCompare = [
      { key: 'fcp', label: 'FCP', unit: 'ms' },
      { key: 'lcp', label: 'LCP', unit: 'ms' },
      { key: 'fid', label: 'FID', unit: 'ms' },
      { key: 'cls', label: 'CLS', unit: '' },
      { key: 'ttfb', label: 'TTFB', unit: 'ms' },
      { key: 'inp', label: 'INP', unit: 'ms' }
    ]

    for (const metric of metricsToCompare) {
      const baselineValue = baseline?.metrics[metric.key as keyof PerformanceSnapshot['metrics']]
      const currentValue = current.metrics[metric.key as keyof PerformanceSnapshot['metrics']]

      const result = compareMetric(metric.key, baselineValue, currentValue)
      results.push(result)
    }

    // 比较资源大小
    const resourcesToCompare = [
      { key: 'jsSize', label: 'JS 大小', unit: 'B' },
      { key: 'cssSize', label: 'CSS 大小', unit: 'B' },
      { key: 'totalSize', label: '总大小', unit: 'B' }
    ]

    for (const resource of resourcesToCompare) {
      const baselineValue = baseline?.resources[resource.key as keyof PerformanceSnapshot['resources']]
      const currentValue = current.resources[resource.key as keyof PerformanceSnapshot['resources']]

      const result = compareMetric(resource.key, baselineValue, currentValue)
      results.push(result)
    }

    // 生成摘要
    const summary = {
      improved: results.filter(r => r.status === 'improved').length,
      regressed: results.filter(r => r.status === 'regressed').length,
      neutral: results.filter(r => r.status === 'neutral' || r.status === 'no-data').length,
      overallStatus: 'neutral' as 'improved' | 'regressed' | 'neutral'
    }

    const regressedHigh = results.filter(r => r.status === 'regressed' && r.significance === 'high')
    if (regressedHigh.length > 0) {
      summary.overallStatus = 'regressed'
    } else if (summary.improved > summary.regressed) {
      summary.overallStatus = 'improved'
    }

    return {
      baseline: baseline || null,
      current,
      results,
      summary
    }
  }

  /**
   * 比较单个指标
   */
  const compareMetric = (metric: string, baseline: number | null | undefined, current: number | null | undefined): ComparisonResult => {
    if (baseline === null || baseline === undefined || current === null || current === undefined) {
      return {
        metric,
        baseline: null,
        current: null,
        diff: null,
        diffPercent: null,
        status: 'no-data',
        significance: 'none'
      }
    }

    const diff = current - baseline
    const diffPercent = baseline !== 0 ? (diff / baseline) * 100 : 0

    let status: 'improved' | 'regressed' | 'neutral' | 'no-data' = 'neutral'
    let significance: 'high' | 'medium' | 'low' | 'none' = 'none'

    // 确定改进或回归（对于某些指标，越小越好；对于分数，越大越好）
    const smallerIsBetter = ['fcp', 'lcp', 'fid', 'cls', 'ttfb', 'inp', 'tbt', 'jsSize', 'cssSize', 'totalSize'].includes(metric)

    if (smallerIsBetter) {
      if (diff < 0) status = 'improved'
      else if (diff > 0) status = 'regressed'
    } else {
      if (diff > 0) status = 'improved'
      else if (diff < 0) status = 'regressed'
    }

    // 确定显著性
    const threshold = REGRESSION_THRESHOLDS[metric as keyof typeof REGRESSION_THRESHOLDS]
    if (threshold) {
      const absoluteDiff = Math.abs(diff)
      if (absoluteDiff >= threshold.critical) significance = 'high'
      else if (absoluteDiff >= threshold.warning) significance = 'medium'
      else if (absoluteDiff > 0) significance = 'low'
    } else {
      if (Math.abs(diffPercent) >= 10) significance = 'high'
      else if (Math.abs(diffPercent) >= 5) significance = 'medium'
      else if (Math.abs(diffPercent) > 0) significance = 'low'
    }

    return {
      metric,
      baseline,
      current,
      diff,
      diffPercent,
      status,
      significance
    }
  }

  /**
   * 检测回归
   */
  const detectRegression = (report: ComparisonReport) => {
    return report.results.filter(r =>
      r.status === 'regressed' &&
      (r.significance === 'high' || r.significance === 'medium')
    )
  }

  /**
   * 生成对比报告
   */
  const generateReport = (): ComparisonReport | null => {
    if (!currentSnapshot.value) return null

    const report = compare(currentSnapshot.value, baselineSnapshot.value)
    comparisonResult.value = report
    return report
  }

  /**
   * 导出报告
   */
  const exportReport = (report: ComparisonReport) => {
    const data = {
      timestamp: new Date().toISOString(),
      baseline: report.baseline,
      current: report.current,
      results: report.results,
      summary: report.summary
    }

    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `performance-comparison-${Date.now()}.json`
    link.click()
    URL.revokeObjectURL(url)
  }

  /**
   * 保存到 localStorage
   */
  const saveToStorage = () => {
    try {
      const data = {
        snapshots: snapshots.value,
        baseline: baselineSnapshot.value,
        current: currentSnapshot.value
      }
      localStorage.setItem('performance-comparison', JSON.stringify(data))
    } catch (error) {
      console.warn('Failed to save comparison data:', error)
    }
  }

  /**
   * 从 localStorage 加载
   */
  const loadFromStorage = () => {
    try {
      const data = JSON.parse(localStorage.getItem('performance-comparison') || '{}')
      if (data.snapshots) snapshots.value = data.snapshots
      if (data.baseline) baselineSnapshot.value = data.baseline
      if (data.current) currentSnapshot.value = data.current
    } catch (error) {
      console.warn('Failed to load comparison data:', error)
    }
  }

  /**
   * 清除数据
   */
  const clear = () => {
    snapshots.value = []
    baselineSnapshot.value = null
    currentSnapshot.value = null
    comparisonResult.value = null
    localStorage.removeItem('performance-comparison')
  }

  /**
   * 获取趋势数据
   */
  const getTrend = (metric: string) => {
    const values = snapshots.value
      .map(s => s.metrics[metric as keyof PerformanceSnapshot['metrics']])
      .filter((v): v is number => v !== undefined)

    if (values.length < 2) return null

    const first = values[0]
    const last = values[values.length - 1]
    const trend = last - first
    const trendPercent = first !== 0 ? (trend / first) * 100 : 0

    return {
      first,
      last,
      trend,
      trendPercent,
      dataPoints: snapshots.value.map(s => ({
        timestamp: s.timestamp,
        value: s.metrics[metric as keyof PerformanceSnapshot['metrics']],
        label: s.label
      }))
    }
  }

  // 计算属性
  const canCompare = computed(() => currentSnapshot.value !== null)

  // 初始化时加载数据
  loadFromStorage()

  return {
    snapshots,
    currentSnapshot,
    baselineSnapshot,
    comparisonResult,
    canCompare,
    createSnapshot,
    saveSnapshot,
    setBaseline,
    setCurrent,
    compare,
    generateReport,
    exportReport,
    detectRegression,
    getTrend,
    clear
  }
}

/**
 * 回归检测阈值常量
 */
export const REGRESSION_THRESHOLDS_CONSTANTS = REGRESSION_THRESHOLDS
