/**
 * 性能回归测试脚本
 *
 * 用于检测代码变更是否导致性能回归
 */

import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

// 颜色输出
const colors = {
  reset: '\x1b[0m',
  red: '\x1b[31m',
  green: '\x1b[32m',
  yellow: '\x1b[33m',
  blue: '\x1b[34m',
  cyan: '\x1b[36m',
  magenta: '\x1b[35m'
}

// 日志函数
const log = {
  success: (msg) => console.log(`${colors.green}✓${colors.reset} ${msg}`),
  error: (msg) => console.log(`${colors.red}✗${colors.reset} ${msg}`),
  warn: (msg) => console.log(`${colors.yellow}⚠${colors.reset} ${msg}`),
  info: (msg) => console.log(`${colors.blue}ℹ${colors.reset} ${msg}`),
  section: (msg) => console.log(`\n${colors.cyan}${msg}${colors.reset}`)
}

// 性能阈值配置
const PERFORMANCE_THRESHOLDS = {
  // Web Vitals
  fcp: { good: 1800, warning: 2400, critical: 3000 },      // ms
  lcp: { good: 2500, warning: 3200, critical: 4000 },      // ms
  fid: { good: 100, warning: 200, critical: 300 },          // ms
  cls: { good: 0.1, warning: 0.2, critical: 0.25 },       // score
  ttfb: { good: 600, warning: 1000, critical: 1500 },      // ms

  // 资源大小
  jsSize: { good: 300000, warning: 400000, critical: 500000 },      // bytes
  cssSize: { good: 80000, warning: 120000, critical: 150000 },      // bytes
  totalSize: { good: 500000, warning: 700000, critical: 900000 },   // bytes

  // 回归阈值（百分比）
  regression: {
    minor: 5,      // 5% 变化为轻微回归
    moderate: 15,   // 15% 变化为中等回归
    severe: 30      // 30% 变化为严重回归
  }
}

// 测试结果
const testResults = []

/**
 * 格式化大小
 */
function formatSize(bytes) {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

/**
 * 读取基线数据
 */
function readBaseline() {
  const baselinePath = path.join(__dirname, '..', 'baseline', 'performance-baseline.json')

  if (!fs.existsSync(baselinePath)) {
    log.warn('基线数据不存在，创建新基线')
    return null
  }

  try {
    const data = fs.readFileSync(baselinePath, 'utf-8')
    return JSON.parse(data)
  } catch (error) {
    log.error('读取基线数据失败')
    return null
  }
}

/**
 * 保存基线数据
 */
function saveBaseline(data) {
  const baselineDir = path.join(__dirname, '..', 'baseline')

  if (!fs.existsSync(baselineDir)) {
    fs.mkdirSync(baselineDir, { recursive: true })
  }

  const baselinePath = path.join(baselineDir, 'performance-baseline.json')

  try {
    fs.writeFileSync(baselinePath, JSON.stringify(data, null, 2))
    log.success(`基线数据已保存: ${baselinePath}`)
    return true
  } catch (error) {
    log.error('保存基线数据失败')
    return false
  }
}

/**
 * 分析构建输出
 */
function analyzeBuildOutput() {
  const distPath = path.join(__dirname, '..', '.output', 'public')

  if (!fs.existsSync(distPath)) {
    log.error('构建输出目录不存在，请先运行 npm run build')
    return null
  }

  const analysis = {
    jsSize: 0,
    cssSize: 0,
    totalSize: 0,
    fileCount: 0,
    jsFiles: [],
    cssFiles: [],
    timestamp: Date.now()
  }

  const scanDir = (dir) => {
    const items = fs.readdirSync(dir)

    for (const item of items) {
      const fullPath = path.join(dir, item)
      const stat = fs.statSync(fullPath)

      if (stat.isDirectory()) {
        scanDir(fullPath)
      } else {
        const ext = path.extname(item).toLowerCase()
        const size = stat.size

        analysis.totalSize += size
        analysis.fileCount++

        if (ext === '.js' || ext === '.mjs') {
          analysis.jsSize += size
          analysis.jsFiles.push({ name: item, size })
        } else if (ext === '.css') {
          analysis.cssSize += size
          analysis.cssFiles.push({ name: item, size })
        }
      }
    }
  }

  scanDir(distPath)

  // 排序文件（按大小降序）
  analysis.jsFiles.sort((a, b) => b.size - a.size)
  analysis.cssFiles.sort((a, b) => b.size - a.size)

  return analysis
}

/**
 * 评估性能指标
 */
function evaluateMetric(name, value, baseline) {
  const threshold = PERFORMANCE_THRESHOLDS[name]

  if (!threshold) {
    return { status: 'unknown', message: '未知指标' }
  }

  // 首次运行，创建基线
  if (!baseline) {
    return { status: 'baseline', message: '创建基线' }
  }

  const baselineValue = baseline[name]
  const diff = value - baselineValue
  const diffPercent = baselineValue !== 0 ? (diff / baselineValue) * 100 : 0

  // 判断是否回归
  const isRegression = diff > 0 // 对于性能指标，值越大越差
  const absPercent = Math.abs(diffPercent)

  let status, severity, message

  if (!isRegression) {
    // 改进
    status = 'improved'
    severity = 'info'
    message = `改进 ${Math.abs(diffPercent).toFixed(1)}%`
  } else if (absPercent < PERFORMANCE_THRESHOLDS.regression.minor) {
    // 轻微变化
    status = 'neutral'
    severity = 'info'
    message = `变化 ${diffPercent.toFixed(1)}%`
  } else if (absPercent < PERFORMANCE_THRESHOLDS.regression.moderate) {
    // 轻微回归
    status = 'regression'
    severity = 'warning'
    message = `轻微回归 ${diffPercent.toFixed(1)}%`
  } else if (absPercent < PERFORMANCE_THRESHOLDS.regression.severe) {
    // 中等回归
    status = 'regression'
    severity = 'error'
    message = `中等回归 ${diffPercent.toFixed(1)}%`
  } else {
    // 严重回归
    status = 'regression'
    severity = 'critical'
    message = `严重回归 ${diffPercent.toFixed(1)}%`
  }

  return {
    status,
    severity,
    message,
    baseline: baselineValue,
    current: value,
    diff,
    diffPercent
  }
}

/**
 * 检查 Lighthouse 报告
 */
function checkLighthouseReports() {
  const lighthouseDir = path.join(__dirname, '..', '.lighthouseci')

  if (!fs.existsSync(lighthouseDir)) {
    log.warn('Lighthouse 报告不存在，跳过 Lighthouse 检查')
    return null
  }

  const reports = []
  const files = fs.readdirSync(lighthouseDir)

  for (const file of files) {
    if (file.endsWith('.json')) {
      const filePath = path.join(lighthouseDir, file)
      try {
        const data = JSON.parse(fs.readFileSync(filePath, 'utf-8'))
        reports.push(data)
      } catch (error) {
        // 跳过无效的报告
      }
    }
  }

  if (reports.length === 0) {
    return null
  }

  // 使用最新的报告
  const latestReport = reports[reports.length - 1]

  return {
    fcp: latestReport.fcp,
    lcp: latestReport.lcp,
    fid: latestReport.fid,
    cls: latestReport.cls,
    score: latestReport.categories?.performance?.score * 100 || 0,
    timestamp: Date.now()
  }
}

/**
 * 运行回归测试
 */
function runRegressionTest() {
  log.section('='.repeat(60))
  log.section('性能回归测试')
  log.section('='.repeat(60))

  // 读取基线
  const baseline = readBaseline()

  // 分析当前构建
  log.info('分析当前构建...')
  const currentBuild = analyzeBuildOutput()

  if (!currentBuild) {
    return false
  }

  log.success(`JS 大小: ${formatSize(currentBuild.jsSize)}`)
  log.success(`CSS 大小: ${formatSize(currentBuild.cssSize)}`)
  log.success(`总大小: ${formatSize(currentBuild.totalSize)}`)
  log.success(`文件数量: ${currentBuild.fileCount}`)

  // 检查 Lighthouse 报告
  log.info('检查 Lighthouse 报告...')
  const lighthouseData = checkLighthouseReports()

  if (lighthouseData) {
    log.success(`Lighthouse 得分: ${lighthouseData.score}`)
    if (lighthouseData.fcp) log.info(`FCP: ${lighthouseData.fcp.toFixed(0)}ms`)
    if (lighthouseData.lcp) log.info(`LCP: ${lighthouseData.lcp.toFixed(0)}ms`)
    if (lighthouseData.fid) log.info(`FID: ${lighthouseData.fid.toFixed(0)}ms`)
    if (lighthouseData.cls) log.info(`CLS: ${lighthouseData.cls.toFixed(3)}`)
  }

  // 准备当前性能数据
  const currentPerformance = {
    ...currentBuild,
    ...(lighthouseData || {})
  }

  // 比较指标
  log.section('性能对比')

  const metricsToCompare = [
    { name: 'jsSize', label: 'JS 大小', format: formatSize },
    { name: 'cssSize', label: 'CSS 大小', format: formatSize },
    { name: 'totalSize', label: '总大小', format: formatSize },
    { name: 'fcp', label: 'FCP', format: v => `${v.toFixed(0)}ms` },
    { name: 'lcp', label: 'LCP', format: v => `${v.toFixed(0)}ms` },
    { name: 'fid', label: 'FID', format: v => `${v.toFixed(0)}ms` },
    { name: 'cls', label: 'CLS', format: v => v.toFixed(3) },
    { name: 'score', label: 'Lighthouse 得分', format: v => `${v.toFixed(0)}`, reverse: true }
  ]

  let regressions = 0
  let improvements = 0

  for (const metric of metricsToCompare) {
    const currentValue = currentPerformance[metric.name]

    if (currentValue === undefined) {
      continue
    }

    const evaluation = evaluateMetric(metric.name, currentValue, baseline)

    if (evaluation.status === 'baseline') {
      log.info(`${metric.label}: ${metric.format(currentValue)} (基线)`)
    } else if (evaluation.status === 'improved') {
      log.success(`${metric.label}: ${metric.format(currentValue)} (${evaluation.message})`)
      improvements++
    } else if (evaluation.status === 'regression') {
      const color = evaluation.severity === 'critical' ? colors.red :
                    evaluation.severity === 'error' ? colors.red :
                    evaluation.severity === 'warning' ? colors.yellow : colors.blue
      console.log(`${color}${evaluation.severity.toUpperCase()}${colors.reset} ${metric.label}: ${metric.format(currentValue)} (${evaluation.message})`)
      regressions++
    } else {
      log.info(`${metric.label}: ${metric.format(currentValue)} (${evaluation.message})`)
    }

    testResults.push({
      metric: metric.name,
      label: metric.label,
      ...evaluation
    })
  }

  // 生成测试报告
  const testReport = {
    timestamp: new Date().toISOString(),
    baseline: baseline,
    current: currentPerformance,
    results: testResults,
    summary: {
      regressions,
      improvements,
      overallStatus: regressions === 0 ? 'passed' : 'failed'
    }
  }

  // 保存测试报告
  const reportPath = path.join(__dirname, '..', 'performance-report.json')
  fs.writeFileSync(reportPath, JSON.stringify(testReport, null, 2))
  log.success(`测试报告已保存: ${reportPath}`)

  // 输出摘要
  log.section('测试摘要')
  console.log(`  改进: ${improvements}`)
  console.log(`  回归: ${regressions}`)
  console.log(`  总体状态: ${testReport.summary.overallStatus.toUpperCase()}`)

  // 如果没有基线，创建基线
  if (!baseline) {
    log.section('\n创建基线')
    const baselineData = {
      ...currentPerformance,
      timestamp: Date.now(),
      gitCommit: process.env.GIT_COMMIT || 'unknown',
      gitBranch: process.env.GIT_BRANCH || 'unknown'
    }

    if (saveBaseline(baselineData)) {
      log.success('基线创建成功，下次运行将进行对比')
      return true
    }
    return false
  }

  // 如果有回归，更新基线需要手动确认
  if (regressions > 0) {
    log.warn('\n检测到性能回归')
    log.info('如果这些变化是预期的，可以更新基线：')
    log.info('  npm run perf:baseline:update')
    return false
  }

  log.success('\n所有性能检查通过！')
  return true
}

/**
 * 更新基线
 */
function updateBaseline() {
  log.section('更新性能基线')

  const currentBuild = analyzeBuildOutput()

  if (!currentBuild) {
    return false
  }

  const lighthouseData = checkLighthouseReports()

  const baselineData = {
    ...currentBuild,
    ...(lighthouseData || {}),
    timestamp: Date.now(),
    gitCommit: process.env.GIT_COMMIT || 'unknown',
    gitBranch: process.env.GIT_BRANCH || 'unknown'
  }

  return saveBaseline(baselineData)
}

// 主函数
function main() {
  const args = process.argv.slice(2)
  const command = args[0] || 'test'

  if (command === 'update-baseline') {
    const success = updateBaseline()
    process.exit(success ? 0 : 1)
  } else if (command === 'test') {
    const success = runRegressionTest()
    process.exit(success ? 0 : 1)
  } else {
    console.log(`
性能回归测试工具

用法:
  node scripts/test-performance-regression.js test          - 运行回归测试
  node scripts/test-performance-regression.js update-baseline - 更新基线

说明:
  - 首次运行会创建基线数据
  - 后续运行会对比当前构建与基线
  - 检测到性能回归时会返回非零退出码
`)
    process.exit(0)
  }
}

main()
