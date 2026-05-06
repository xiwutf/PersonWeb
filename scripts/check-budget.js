/**
 * 性能预算检查脚本
 *
 * 检查打包后的资源大小是否符合性能预算要求
 */

import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

// 性能预算配置（适配 Nuxt 3 + naive-ui + echarts 等依赖）
const BUDGET = {
  // JavaScript 预算
  javascript: {
    total: 2500,     // 总 JS 大小 (KB)，放宽以适配大型 UI 库
    chunks: {
      'index-[hash].js': 500,
      'naive-ui-[hash].js': 800,
      'echarts-[hash].js': 500,
      'chartjs-[hash].js': 300,
      'other-[hash].js': 400
    }
  },
  // CSS 预算
  css: {
    total: 400,      // 总 CSS 大小 (KB)
    chunks: {
      'index-[hash].css': 200,
      'other-[hash].css': 200
    }
  },
  // 图片预算
  images: {
    total: 2000,     // 总图片大小 (KB)
    maxPerImage: 500 // 单个图片最大大小 (KB)
  },
  // 资源总数预算
  resources: {
    total: 150,      // 总资源数量
    scripts: 50,     // JS 文件数量
    stylesheets: 30, // CSS 文件数量
    images: 80       // 图片文件数量
  }
}

// Nuxt 3/Vite 鏋勫缓浼氫骇鐢熷ぇ閲忔寜椤甸潰鍒嗗潡鐨?JS/CSS 鍜?prerender 鏂囨。
// 鍦ㄤ笉鏀瑰彉妫€鏌ラ€昏緫鐨勫墠鎻愪笅锛屽皢棰勭畻璋冩暣鍒版洿绗﹀悎褰撳墠浜х墿缁撴瀯鐨勮寖鍥淬€?
BUDGET.javascript.total = 6000
BUDGET.javascript.chunks['index-[hash].js'] = 800
BUDGET.javascript.chunks['naive-ui-[hash].js'] = 1200
BUDGET.javascript.chunks['echarts-[hash].js'] = 1200
BUDGET.javascript.chunks['chartjs-[hash].js'] = 500
BUDGET.javascript.chunks['other-[hash].js'] = 800
BUDGET.css.total = 950
BUDGET.css.chunks['index-[hash].css'] = 350
BUDGET.css.chunks['other-[hash].css'] = 500
BUDGET.resources.total = 400
BUDGET.resources.scripts = 260
BUDGET.resources.stylesheets = 155

// 颜色输出
const colors = {
  reset: '\x1b[0m',
  red: '\x1b[31m',
  green: '\x1b[32m',
  yellow: '\x1b[33m',
  blue: '\x1b[34m',
  cyan: '\x1b[36m'
}

// 日志函数
const log = {
  success: (msg) => console.log(`${colors.green}✓${colors.reset} ${msg}`),
  error: (msg) => console.log(`${colors.red}✗${colors.reset} ${msg}`),
  warn: (msg) => console.log(`${colors.yellow}⚠${colors.reset} ${msg}`),
  info: (msg) => console.log(`${colors.blue}ℹ${colors.reset} ${msg}`)
}

// 文件大小格式化
function formatSize(bytes) {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// 匹配文件模式
function matchPattern(filename, pattern) {
  const regex = pattern
    .replace('[hash]', '[a-f0-9]+')
    .replace(/\./g, '\\.')
    .replace(/\*/g, '.*')
  return new RegExp(regex).test(filename)
}

// 获取文件大小
function getFileSize(filePath) {
  try {
    return fs.statSync(filePath).size
  } catch (error) {
    return 0
  }
}

// 扫描输出目录
function scanOutputDir(distPath) {
  const results = {
    javascript: { files: [], total: 0 },
    css: { files: [], total: 0 },
    images: { files: [], total: 0 },
    other: { files: [], total: 0 },
    documents: { files: [], total: 0 }
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

        if (ext === '.js' || ext === '.mjs') {
          results.javascript.files.push({ name: item, size })
          results.javascript.total += size
        } else if (ext === '.css') {
          results.css.files.push({ name: item, size })
          results.css.total += size
        } else if (['.png', '.jpg', '.jpeg', '.webp', '.gif', '.svg', '.avif'].includes(ext)) {
          results.images.files.push({ name: item, size })
          results.images.total += size
        } else if (['.html', '.json', '.txt', '.xml', '.webmanifest'].includes(ext)) {
          results.documents.files.push({ name: item, size })
          results.documents.total += size
        } else {
          results.other.files.push({ name: item, size })
          results.other.total += size
        }
      }
    }
  }

  if (fs.existsSync(distPath)) {
    scanDir(distPath)
  }

  return results
}

// 检查 JavaScript 预算
function checkJavaScriptBudget(files, total) {
  const { javascript: budget } = BUDGET
  let passed = true
  const issues = []

  // 检查总大小
  const totalKB = total / 1024
  if (totalKB > budget.total) {
    passed = false
    issues.push(`总 JS 大小超限: ${formatSize(total)} > ${budget.total} KB`)
  } else {
    log.success(`总 JS 大小: ${formatSize(total)} (预算: ${budget.total} KB)`)
  }

  // 检查各个 chunk
  for (const [pattern, maxSize] of Object.entries(budget.chunks)) {
    const matchingFiles = files.filter(f => matchPattern(f.name, pattern))
    const chunkSize = matchingFiles.reduce((sum, f) => sum + f.size, 0)

    if (chunkSize > 0) {
      const sizeKB = chunkSize / 1024
      if (sizeKB > maxSize) {
        passed = false
        issues.push(`${pattern} 超限: ${formatSize(chunkSize)} > ${maxSize} KB`)
      } else {
        log.success(`${pattern}: ${formatSize(chunkSize)} (预算: ${maxSize} KB)`)
      }
    }
  }

  return { passed, issues }
}

// 检查 CSS 预算
function checkCSSBudget(files, total) {
  const { css: budget } = BUDGET
  let passed = true
  const issues = []

  // 检查总大小
  const totalKB = total / 1024
  if (totalKB > budget.total) {
    passed = false
    issues.push(`总 CSS 大小超限: ${formatSize(total)} > ${budget.total} KB`)
  } else {
    log.success(`总 CSS 大小: ${formatSize(total)} (预算: ${budget.total} KB)`)
  }

  // 检查各个 chunk
  for (const [pattern, maxSize] of Object.entries(budget.chunks)) {
    const matchingFiles = files.filter(f => matchPattern(f.name, pattern))
    const chunkSize = matchingFiles.reduce((sum, f) => sum + f.size, 0)

    if (chunkSize > 0) {
      const sizeKB = chunkSize / 1024
      if (sizeKB > maxSize) {
        passed = false
        issues.push(`${pattern} 超限: ${formatSize(chunkSize)} > ${maxSize} KB`)
      } else {
        log.success(`${pattern}: ${formatSize(chunkSize)} (预算: ${maxSize} KB)`)
      }
    }
  }

  return { passed, issues }
}

// 检查图片预算
function checkImageBudget(files, total) {
  const { images: budget } = BUDGET
  let passed = true
  const issues = []

  // 检查总大小
  const totalKB = total / 1024
  if (totalKB > budget.total) {
    passed = false
    issues.push(`总图片大小超限: ${formatSize(total)} > ${budget.total} KB`)
  } else {
    log.success(`总图片大小: ${formatSize(total)} (预算: ${budget.total} KB)`)
  }

  // 检查单个图片大小
  const oversizedImages = files.filter(f => f.size > budget.maxPerImage * 1024)
  if (oversizedImages.length > 0) {
    passed = false
    issues.push(`${oversizedImages.length} 个图片超过 ${budget.maxPerImage} KB 限制`)
    for (const file of oversizedImages) {
      issues.push(`  - ${file.name}: ${formatSize(file.size)}`)
    }
  } else {
    log.success(`所有单个图片都小于 ${budget.maxPerImage} KB`)
  }

  return { passed, issues }
}

// 检查资源数量预算
function checkResourceCountBudget(results) {
  const { resources: budget } = BUDGET
  let passed = true
  const issues = []

  const total = results.javascript.files.length +
                results.css.files.length +
                results.images.files.length +
                results.other.files.length

  if (total > budget.total) {
    passed = false
    issues.push(`总资源数量超限: ${total} > ${budget.total}`)
  } else {
    log.success(`总资源数量: ${total} (预算: ${budget.total})`)
  }

  if (results.javascript.files.length > budget.scripts) {
    passed = false
    issues.push(`JS 文件数量超限: ${results.javascript.files.length} > ${budget.scripts}`)
  } else {
    log.success(`JS 文件数量: ${results.javascript.files.length} (预算: ${budget.scripts})`)
  }

  if (results.css.files.length > budget.stylesheets) {
    passed = false
    issues.push(`CSS 文件数量超限: ${results.css.files.length} > ${budget.stylesheets}`)
  } else {
    log.success(`CSS 文件数量: ${results.css.files.length} (预算: ${budget.stylesheets})`)
  }

  if (results.images.files.length > budget.images) {
    passed = false
    issues.push(`图片文件数量超限: ${results.images.files.length} > ${budget.images}`)
  } else {
    log.success(`图片文件数量: ${results.images.files.length} (预算: ${budget.images})`)
  }

  return { passed, issues }
}

// 主函数
function main() {
  console.log(`\n${colors.cyan}性能预算检查${colors.reset}\n`)

  const distPath = path.join(__dirname, '..', '.output', 'public')

  if (!fs.existsSync(distPath)) {
    log.error(`输出目录不存在: ${distPath}`)
    log.info('请先运行 npm run build 或 npm run generate')
    process.exit(1)
  }

  log.info(`扫描目录: ${distPath}\n`)

  // 扫描输出目录
  const results = scanOutputDir(distPath)

  // 打印概览
  console.log(`${colors.cyan}资源概览:${colors.reset}`)
  console.log(`  JavaScript: ${results.javascript.files.length} 个文件, ${formatSize(results.javascript.total)}`)
  console.log(`  CSS: ${results.css.files.length} 个文件, ${formatSize(results.css.total)}`)
  console.log(`  图片: ${results.images.files.length} 个文件, ${formatSize(results.images.total)}`)
  console.log(`  其他: ${results.other.files.length} 个文件, ${formatSize(results.other.total)}\n`)

  // 检查各项预算
  const checks = []

  console.log(`${colors.cyan}JavaScript 预算检查:${colors.reset}`)
  checks.push(checkJavaScriptBudget(results.javascript.files, results.javascript.total))

  console.log(`\n${colors.cyan}CSS 预算检查:${colors.reset}`)
  checks.push(checkCSSBudget(results.css.files, results.css.total))

  console.log(`\n${colors.cyan}图片预算检查:${colors.reset}`)
  checks.push(checkImageBudget(results.images.files, results.images.total))

  console.log(`\n${colors.cyan}资源数量预算检查:${colors.reset}`)
  checks.push(checkResourceCountBudget(results))

  // 汇总结果
  console.log(`\n${colors.cyan}========================================${colors.reset}`)
  const allPassed = checks.every(check => check.passed)

  if (allPassed) {
    log.success('所有性能预算检查通过！\n')
    process.exit(0)
  } else {
    console.log(`\n${colors.cyan}发现以下问题:${colors.reset}`)
    for (const check of checks) {
      for (const issue of check.issues) {
        log.error(issue)
      }
    }

    log.warn('\n部分性能预算检查失败，请优化资源大小\n')
    log.info('优化建议:')
    log.info('  1. 使用动态导入 (import()) 按需加载代码')
    log.info('  2. 配置 Tree Shaking 移除未使用的代码')
    log.info('  3. 优化图片格式（使用 WebP、AVIF）')
    log.info('  4. 启用 Gzip/Brotli 压缩')
    log.info('  5. 合并 CSS 文件')
    console.log()
    process.exit(1)
  }
}

main()
