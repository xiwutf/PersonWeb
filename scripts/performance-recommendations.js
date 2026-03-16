/**
 * 性能优化建议工具
 *
 * 基于代码分析和最佳实践，提供性能优化建议
 */

import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'
import glob from 'glob'

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

// 严重程度
const SEVERITY = {
  CRITICAL: { level: 'critical', color: colors.red, emoji: '🔴' },
  HIGH: { level: 'high', color: colors.red, emoji: '🟠' },
  MEDIUM: { level: 'medium', color: colors.yellow, emoji: '🟡' },
  LOW: { level: 'low', color: colors.blue, emoji: '🔵' }
}

// 优化建议列表
const recommendations = []

// 日志函数
const log = {
  info: (msg) => console.log(`${colors.blue}ℹ${colors.reset} ${msg}`),
  success: (msg) => console.log(`${colors.green}✓${colors.reset} ${msg}`),
  error: (msg) => console.log(`${colors.red}✗${colors.reset} ${msg}`),
  warn: (msg) => console.log(`${colors.yellow}⚠${colors.reset} ${msg}`),
  section: (msg) => console.log(`\n${colors.cyan}${msg}${colors.reset}``)
}

// 添加建议
function addRecommendation(severity, category, title, description, fix, files = []) {
  recommendations.push({
    severity: SEVERITY[severity],
    category,
    title,
    description,
    fix,
    files
  })
}

// 读取文件内容
function readFileContent(filePath) {
  try {
    return fs.readFileSync(filePath, 'utf-8')
  } catch (error) {
    return null
  }
}

// 检查 1: 未优化的图片
async function checkImageOptimization() {
  log.section('📷 检查图片优化')

  const imagePatterns = [
    '**/*.png',
    '**/*.jpg',
    '**/*.jpeg',
    '**/*.gif'
  ]

  const imageFiles = []
  for (const pattern of imagePatterns) {
    const files = glob.sync(pattern, {
      cwd: path.join(__dirname, '..', 'public'),
      ignore: ['**/node_modules/**', '**/.nuxt/**', '**/.output/**']
    })
    imageFiles.push(...files.map(f => path.join('public', f)))
  }

  const largeImages = []
  const nonWebPImages = []

  for (const file of imageFiles) {
    const filePath = path.join(__dirname, '..', file)
    const stats = fs.statSync(filePath)
    const sizeKB = stats.size / 1024

    // 检查大图片
    if (sizeKB > 100) {
      largeImages.push({ file, size: sizeKB })
    }

    // 检查非 WebP 图片
    if (!file.includes('.webp') && !file.includes('.avif')) {
      nonWebPImages.push(file)
    }
  }

  if (largeImages.length > 0) {
    addRecommendation(
      'HIGH',
      'Image Optimization',
      '存在大图片文件',
      `发现 ${largeImages.length} 个超过 100KB 的图片文件`,
      '使用 WebP 或 AVIF 格式，使用图片压缩工具（如 sharp、imagemin）进行优化',
      largeImages.map(i => i.file)
    )
    log.warn(`发现 ${largeImages.length} 个大图片`)
  } else {
    log.success('没有发现超大图片')
  }

  if (nonWebPImages.length > 0) {
    addRecommendation(
      'MEDIUM',
      'Image Optimization',
      '图片未使用现代格式',
      `发现 ${nonWebPImages.length} 个未使用 WebP/AVIF 的图片`,
      '转换为 WebP 或 AVIF 格式，可减少 30-50% 的文件大小',
      nonWebPImages.slice(0, 5)
    )
    log.warn(`${nonWebPImages.length} 个图片未使用现代格式`)
  } else {
    log.success('所有图片都使用了现代格式')
  }
}

// 检查 2: 未使用的 CSS
function checkUnusedCSS() {
  log.section('🎨 检查 CSS 使用情况')

  const cssFiles = glob.sync('**/*.css', {
    cwd: __dirname,
    ignore: [
      '**/node_modules/**',
      '**/.nuxt/**',
      '**/.output/**',
      '**/dist/**',
      '**/coverage/**'
    ]
  })

  const allCSS = cssFiles.length
  const totalCSSLines = cssFiles.reduce((sum, file) => {
    const content = readFileContent(file)
    return sum + (content ? content.split('\n').length : 0)
  }, 0)

  if (totalCSSLines > 10000) {
    addRecommendation(
      'MEDIUM',
      'CSS Optimization',
      'CSS 代码量较大',
      `总 CSS 代码约 ${totalCSSLines} 行`,
      '使用 PurgeCSS 移除未使用的 CSS，考虑按需加载样式文件'
    )
    log.warn(`CSS 代码量: ${totalCSSLines} 行`)
  } else {
    log.success(`CSS 代码量: ${totalCSSLines} 行`)
  }
}

// 检查 3: 动态导入使用情况
function checkDynamicImports() {
  log.section('📦 检查动态导入使用')

  const vueFiles = glob.sync('**/*.vue', {
    cwd: __dirname,
    ignore: [
      '**/node_modules/**',
      '**/.nuxt/**',
      '**/.output/**',
      '**/dist/**',
      '**/coverage/**'
    ]
  })

  let componentFiles = 0
  let usesLazy = false
  let largeComponents = []

  for (const file of vueFiles) {
    const content = readFileContent(file)
    if (!content) continue

    componentFiles++

    const stats = fs.statSync(file)
    const sizeKB = stats.size / 1024

    // 检查是否使用了懒加载
    if (content.includes('defineAsyncComponent') || content.includes('lazy(')) {
      usesLazy = true
    }

    // 检查大组件
    if (sizeKB > 50 && file.startsWith('components/')) {
      largeComponents.push({ file, size: sizeKB })
    }
  }

  log.info(`组件数量: ${componentFiles}`)
  log.info(`使用懒加载: ${usesLazy ? '是' : '否'}`)

  if (!usesLazy) {
    addRecommendation(
      'MEDIUM',
      'Code Splitting',
      '未使用动态导入/懒加载',
      '发现大量组件但未使用懒加载优化',
      '对大型组件使用 defineAsyncComponent 或 lazy() 进行懒加载'
    )
  } else {
    log.success('已使用动态导入')
  }

  if (largeComponents.length > 0) {
    addRecommendation(
      'LOW',
      'Code Splitting',
      '存在大型组件文件',
      `发现 ${largeComponents.length} 个超过 50KB 的组件`,
      '考虑拆分大型组件，使用动态导入按需加载',
      largeComponents.slice(0, 3).map(c => c.file)
    )
    log.warn(`${largeComponents.length} 个大型组件`)
  } else {
    log.success('组件大小合理')
  }
}

// 检查 4: 第三方库使用
function checkThirdPartyLibs() {
  log.section('📚 检查第三方库使用')

  const packageJson = JSON.parse(readFileContent(path.join(__dirname, '..', 'package.json')))
  const dependencies = Object.keys(packageJson.dependencies || {})

  const heavyLibs = [
    'three.js',
    'echarts',
    'chart.js',
    'matter-js',
    'pixi.js',
    'gsap'
  ]

  const foundHeavyLibs = dependencies.filter(dep =>
    heavyLibs.some(lib => dep.toLowerCase().includes(lib))
  )

  if (foundHeavyLibs.length > 0) {
    addRecommendation(
      'LOW',
      'Bundle Size',
      '使用了较重的第三方库',
      `发现 ${foundHeavyLibs.join(', ')} 等库`,
      '确保按需引入，使用动态导入，考虑轻量级替代方案',
      foundHeavyLibs
    )
    log.warn(`发现重型库: ${foundHeavyLibs.join(', ')}`)
  } else {
    log.success('未发现明显的重型库')
  }

  // 检查重复依赖
  const allDeps = {
    ...packageJson.dependencies,
    ...packageJson.devDependencies
  }

  const potentialDuplicates = Object.keys(allDeps).filter(dep =>
    dep.includes('@vue/') || dep.includes('vue')
  )

  if (potentialDuplicates.length > 5) {
    addRecommendation(
      'LOW',
      'Dependencies',
      '可能有重复的 Vue 相关依赖',
      `发现 ${potentialDuplicates.length} 个 Vue 相关包`,
      '检查是否有重复依赖，考虑移除不必要的包',
      potentialDuplicates
    )
  }
}

// 检查 5: 性能 API 使用
function checkPerformanceAPI() {
  log.section('⚡ 检查性能监控实现')

  const files = glob.globSync('**/*.{ts,js,vue}', {
    cwd: __dirname,
    ignore: [
      '**/node_modules/**',
      '**/.nuxt/**',
      '**/.output/**',
      '**/dist/**',
      '**/coverage/**'
    ]
  })

  let usesPerformance = false
  let usesWebVitals = false
  let usesLighthouse = false

  for (const file of files) {
    const content = readFileContent(file)
    if (!content) continue

    if (content.includes('PerformanceObserver') ||
        content.includes('performance.mark') ||
        content.includes('performance.measure')) {
      usesPerformance = true
    }

    if (content.includes('web-vitals') ||
        content.includes('FCP') ||
        content.includes('LCP') ||
        content.includes('CLS')) {
      usesWebVitals = true
    }

    if (content.includes('lighthouse')) {
      usesLighthouse = true
    }
  }

  if (!usesPerformance) {
    addRecommendation(
      'MEDIUM',
      'Performance Monitoring',
      '未实现性能监控',
      '未发现 Performance API 的使用',
      '添加性能监控代码，使用 PerformanceObserver 监控关键指标'
    )
  } else {
    log.success('已使用 Performance API')
  }

  if (!usesWebVitals) {
    addRecommendation(
      'MEDIUM',
      'Performance Monitoring',
      '未监控 Web Vitals',
      '未发现 Web Vitals (FCP, LCP, CLS, FID) 的监控',
      '使用 web-vitals 库监控核心 Web 指标'
    )
  } else {
    log.success('已监控 Web Vitals')
  }
}

// 检查 6: 内存泄漏风险
function checkMemoryLeaks() {
  log.section('💾 检查内存泄漏风险')

  const vueFiles = glob.sync('**/*.vue', {
    cwd: __dirname,
    ignore: [
      '**/node_modules/**',
      '**/.nuxt/**',
      '**/.output/**',
      '**/dist/**',
      '**/coverage/**'
    ]
  })

  let usesWatchers = 0
  let usesEventListeners = 0
  let hasOnMounted = 0
  let hasOnUnmounted = 0

  for (const file of vueFiles) {
    const content = readFileContent(file)
    if (!content) continue

    if (content.includes('watch(') || content.includes('watchEffect(')) {
      usesWatchers++
    }

    if (content.includes('addEventListener')) {
      usesEventListeners++
    }

    if (content.includes('onMounted(') || content.includes('mounted()')) {
      hasOnMounted++
    }

    if (content.includes('onUnmounted(') || content.includes('beforeUnmount(') ||
        content.includes('unmounted()') || content.includes('beforeUnmount()')) {
      hasOnUnmounted++
    }
  }

  log.info(`使用 watch/watchEffect: ${usesWatchers} 个组件`)
  log.info(`使用 addEventListener: ${usesEventListeners} 个组件`)
  log.info(`有 onMounted: ${hasOnMounted} 个组件`)
  log.info(`有 onUnmounted: ${hasOnUnmounted} 个组件`)

  if (usesEventListeners > hasOnUnmounted) {
    addRecommendation(
      'HIGH',
      'Memory Management',
      '可能存在事件监听器未清理',
      `${usesEventListeners} 个组件使用 addEventListener，但只有 ${hasOnUnmounted} 个有清理代码`,
      '在 onUnmounted 或 beforeUnmount 中移除所有事件监听器'
    )
    log.warn('可能存在事件监听器泄漏')
  } else {
    log.success('事件监听器清理情况良好')
  }
}

// 检查 7: SSR/SSG 配置
function checkSSRConfiguration() {
  log.section('🌐 检查 SSR/SSG 配置')

  const nuxtConfig = readFileContent(path.join(__dirname, '..', 'nuxt.config.ts'))
  const packageJson = JSON.parse(readFileContent(path.join(__dirname, '..', 'package.json')))

  if (nuxtConfig) {
    const usesSSR = nuxtConfig.includes('ssr: true') ||
                    (!nuxtConfig.includes('ssr: false') && !nuxtConfig.includes('ssr:'))

    const usesNitro = nuxtConfig.includes('nitro:')
    const usesGenerate = packageJson.scripts?.generate

    if (usesGenerate) {
      log.success('配置了静态生成 (SSG)')
    } else if (usesSSR) {
      log.info('使用服务器端渲染 (SSR)')
    }

    if (usesNitro) {
      log.success('使用 Nitro 服务器')
    }
  }

  // 检查预渲染配置
  if (nuxtConfig && nuxtConfig.includes('prerender:')) {
    log.success('配置了预渲染')
  } else {
    addRecommendation(
      'LOW',
      'Performance',
      '未配置预渲染',
      '未发现 prerender 配置',
      '配置 prerender 以预渲染静态页面，提高首屏加载速度'
    )
  }
}

// 检查 8: 缓存策略
function checkCachingStrategy() {
  log.section('💾 检查缓存策略')

  const nuxtConfig = readFileContent(path.join(__dirname, '..', 'nuxt.config.ts'))

  // 检查路由级别的代码分割
  if (nuxtConfig) {
    const hasCodeSplitting = nuxtConfig.includes('splitChunks') ||
                            nuxtConfig.includes('manualChunks')

    if (hasCodeSplitting) {
      log.success('已配置代码分割')
    } else {
      addRecommendation(
        'LOW',
        'Build Optimization',
        '未配置代码分割',
        '未发现 splitChunks 或 manualChunks 配置',
        '配置路由级别或组件级别的代码分割，减少首屏加载体积'
      )
    }
  }
}

// 生成报告
function generateReport() {
  console.log(`\n${colors.cyan}${'='.repeat(60)}${colors.reset}`)
  console.log(`${colors.cyan}性能优化建议报告${colors.reset}`)
  console.log(`${colors.cyan}${'='.repeat(60)}${colors.reset}\n`)

  if (recommendations.length === 0) {
    log.success('未发现明显的性能问题！🎉')
    return
  }

  // 按严重程度分组
  const grouped = {
    critical: [],
    high: [],
    medium: [],
    low: []
  }

  for (const rec of recommendations) {
    grouped[rec.severity.level].push(rec)
  }

  // 按优先级输出
  const order = ['critical', 'high', 'medium', 'low']
  const count = {
    critical: 0,
    high: 0,
    medium: 0,
    low: 0
  }

  for (const level of order) {
    const items = grouped[level]
    count[level] = items.length

    if (items.length === 0) continue

    const severity = SEVERITY[level.toUpperCase()]
    console.log(`${severity.color}${severity.emoji} ${level.toUpperCase()} (${items.length})${colors.reset}\n`)

    for (const rec of items) {
      console.log(`  ${rec.severity.emoji} ${rec.title}`)
      console.log(`    ${colors.cyan}分类:${colors.reset} ${rec.category}`)
      console.log(`    ${colors.cyan}描述:${colors.reset} ${rec.description}`)
      console.log(`    ${colors.cyan}建议:${colors.reset} ${rec.fix}`)

      if (rec.files && rec.files.length > 0) {
        console.log(`    ${colors.cyan}相关文件:${colors.reset}`)
        for (const file of rec.files) {
          console.log(`      - ${file}`)
        }
      }
      console.log()
    }
  }

  // 总结
  console.log(`${colors.cyan}${'='.repeat(60)}${colors.reset}`)
  console.log(`${colors.cyan}总结${colors.reset}`)
  console.log(`${colors.cyan}${'='.repeat(60)}${colors.reset}\n`)

  console.log(`  🔴 Critical: ${count.critical}`)
  console.log(`  🟠 High: ${count.high}`)
  console.log(`  🟡 Medium: ${count.medium}`)
  console.log(`  🔵 Low: ${count.low}`)
  console.log(`  ${colors.green}Total: ${recommendations.length}${colors.reset}\n`)

  // 优先处理建议
  if (count.critical > 0 || count.high > 0) {
    log.warn('建议优先处理 Critical 和 High 级别的问题')
  } else if (count.medium > 0) {
    log.info('建议尽快处理 Medium 级别的问题')
  }

  console.log()
}

// 主函数
async function main() {
  console.log(`\n${colors.cyan}${'='.repeat(60)}${colors.reset}`)
  console.log(`${colors.cyan}性能优化分析工具${colors.reset}`)
  console.log(`${colors.cyan}${'='.repeat(60)}${colors.reset}\n`)

  try {
    await checkImageOptimization()
    checkUnusedCSS()
    checkDynamicImports()
    checkThirdPartyLibs()
    checkPerformanceAPI()
    checkMemoryLeaks()
    checkSSRConfiguration()
    checkCachingStrategy()

    generateReport()

    process.exit(0)
  } catch (error) {
    log.error(`分析过程中出错: ${error.message}`)
    console.error(error)
    process.exit(1)
  }
}

main()
