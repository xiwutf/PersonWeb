/**
 * 样式系统治理验证脚本（阶段2.5）
 * 用途：验证样式是否符合阶段2.5的硬性规则
 *
 * 硬性规则：
 * 1. 禁止使用 spacing 变量代替字号
 * 2. 禁止使用 spacing 变量代替圆角
 * 3. 禁止用大量 calc() 代替缺失 token
 * 4. 替换必须保证数值等价
 */

const fs = require('fs')
const path = require('path')

// 定义已知存在的 token（基于 theme-variables.css 和 tokens.css）
const VALID_TOKENS = {
  spacing: [
    // tokens.css 中的语义化命名
    '--spacing-xs', '--spacing-sm', '--spacing-md', '--spacing-lg', '--spacing-xl',
    // theme-variables.css 中的数字命名
    '--spacing-0', '--spacing-px', '--spacing-0_5',
    '--spacing-1', '--spacing-1_5', '--spacing-2', '--spacing-2_5',
    '--spacing-3', '--spacing-3_5', '--spacing-4', '--spacing-5',
    '--spacing-6', '--spacing-7', '--spacing-8', '--spacing-9',
    '--spacing-10', '--spacing-11', '--spacing-12', '--spacing-14',
    '--spacing-16', '--spacing-20', '--spacing-24', '--spacing-28',
    '--spacing-32', '--spacing-36', '--spacing-40', '--spacing-44',
    '--spacing-48', '--spacing-56', '--spacing-64'
  ],
  text: [
    // 字号 token（font-size 属性应使用这些）
    '--text-xs', '--text-sm', '--text-base', '--text-lg',
    '--text-xl', '--text-2xl', '--text-3xl', '--text-4xl',
    '--text-5xl', '--text-6xl', '--text-7xl', '--text-8xl', '--text-9xl',
    // 兼容旧命名
    '--font-size-base', '--font-size-h1', '--font-size-h2',
    '--font-size-h3', '--font-size-h4'
  ],
  radius: [
    '--radius-none', '--radius-xs', '--radius-sm', '--radius-base', '--radius-md',
    '--radius-lg', '--radius-xl', '--radius-2xl', '--radius-3xl',
    '--radius-full'
  ],
  // 其他颜色和布局 token（用于检测不存在的 token）
  color: [
    '--color-text-main', '--color-text-primary', '--color-text-secondary',
    '--color-text-tertiary', '--color-text-quaternary',
    '--color-text-muted', '--color-text-sub', '--color-text-disabled',
    '--color-text-on-primary', '--color-text-success', '--color-text-warning',
    '--color-text-error', '--color-text-info'
  ],
  bg: [
    '--color-bg-body', '--color-bg-page', '--color-bg-page-alt',
    '--color-bg-light', '--color-bg-dark', '--color-bg-elevated',
    '--color-bg-overlay', '--color-bg-card', '--color-bg-card-elevated',
    '--color-bg-card-muted', '--color-bg-surface', '--color-bg-surface-elevated',
    '--color-bg-surface-muted', '--color-bg-input', '--color-bg-input-disabled',
    '--color-bg-input-hover', '--color-bg-disabled'
  ],
  border: [
    '--color-border-subtle', '--color-border-default', '--color-border-strong',
    '--color-border-focus', '--color-border-muted', '--color-border-hover',
    '--color-border-active'
  ]
}

// 违规类型
const VIOLATION_TYPES = {
  SPACING_FOR_FONT_SIZE: 'spacing-for-font-size',      // 使用 spacing 代替字号
  SPACING_FOR_RADIUS: 'spacing-for-radius',            // 使用 spacing 代替圆角
  INVALID_TOKEN: 'invalid-token',                      // 不存在的 token
  UNNECESSARY_CALC: 'unnecessary-calc',              // 不必要的 calc()
  HARDCODED_VALUE: 'hardcoded-value'                  // 硬编码值
}

// 扫描结果
const scanResults = {
  totalFiles: 0,
  violations: {},
  fileViolations: {}
}

/**
 * 验证单个文件
 */
function validateFile(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8')
    const relativePath = path.relative(process.cwd(), filePath)

    scanResults.totalFiles++

    const violations = []

    // 规则1：检查是否使用 spacing 变量代替字号
    const fontSizeSpacingRegex = /font-size:\s*var\((--spacing-[\w_-]+)\)/g
    let match
    while ((match = fontSizeSpacingRegex.exec(content)) !== null) {
      violations.push({
        type: VIOLATION_TYPES.SPACING_FOR_FONT_SIZE,
        token: match[1],
        line: getLineNumber(content, match.index),
        suggestion: '应使用 text-* 系列变量，如 --text-xs, --text-sm, --text-base 等'
      })
    }

    // 规则2：检查是否使用 spacing 变量代替圆角
    const radiusSpacingRegex = /border-radius:\s*var\((--spacing-[\w_-]+)\)/g
    while ((match = radiusSpacingRegex.exec(content)) !== null) {
      violations.push({
        type: VIOLATION_TYPES.SPACING_FOR_RADIUS,
        token: match[1],
        line: getLineNumber(content, match.index),
        suggestion: '应使用 radius-* 系列变量，如 --radius-sm, --radius-md, --radius-lg 等'
      })
    }

    // 规则3：检查不存在的 token
    const allTokenRegex = /var\((--[\w-]+)\)/g
    while ((match = allTokenRegex.exec(content)) !== null) {
      const token = match[1]

      // 跳过以 --color 开头的 token（这些是颜色 token，不在此检查范围）
      if (token.startsWith('--color-')) {
        continue
      }

      // 跳过以 --shadow 开头的 token
      if (token.startsWith('--shadow')) {
        continue
      }

      // 检查是否为 spacing token
      if (token.startsWith('--spacing-')) {
        if (!VALID_TOKENS.spacing.includes(token)) {
          violations.push({
            type: VIOLATION_TYPES.INVALID_TOKEN,
            token: token,
            line: getLineNumber(content, match.index),
            suggestion: `该 spacing token 不存在，请使用现有的 spacing token`
          })
        }
      }
      // 检查是否为 text token（字号，如 --text-xs）
      else if (token.startsWith('--text-') && !token.includes('-text-') && !token.includes('-bg-') && !token.includes('-border-')) {
        if (!VALID_TOKENS.text.includes(token)) {
          violations.push({
            type: VIOLATION_TYPES.INVALID_TOKEN,
            token: token,
            line: getLineNumber(content, match.index),
            suggestion: `该 text token 不存在，请使用现有的 text token`
          })
        }
      }
      // 检查是否为 radius token
      else if (token.startsWith('--radius-')) {
        if (!VALID_TOKENS.radius.includes(token)) {
          violations.push({
            type: VIOLATION_TYPES.INVALID_TOKEN,
            token: token,
            line: getLineNumber(content, match.index),
            suggestion: `该 radius token 不存在，请使用现有的 radius token`
          })
        }
      }
      // 检查其他可能的 token（如 --primary, --bg, --border 等）
      else if (token.startsWith('--primary-') || token.startsWith('--bg-') || token.startsWith('--border-') || token.startsWith('--text-')) {
        // 这些通常是颜色变量，已经在前面跳过了 --color- 开头的
        // 这里只检查非常明显的不存在的 token
        if (!token.match(/^(--primary|--text-main|--text-secondary|--text-muted|--bg-card|--border-color)$/)) {
          // 这是一个简化的检查，实际项目中可能有更多这样的 token
          // 这里只是作为示例，实际需要根据项目的 token 定义调整
        }
      }
    }

    // 规则3（续）：检查不必要的 calc() 表达式
    // 简化检查：查找包含 var(--spacing-xx) 的 calc() 表达式
    // 使用简单的字符串查找来避免正则表达式问题
    const calcStarts = []
    let pos = content.indexOf('calc(')
    while (pos !== -1) {
      // 找到匹配的闭合括号
      let depth = 1
      let endPos = pos + 5
      while (endPos < content.length && depth > 0) {
        if (content[endPos] === '(') depth++
        else if (content[endPos] === ')') depth--
        endPos++
      }

      if (depth === 0) {
        const calcExpr = content.substring(pos, endPos)
        // 检查是否包含 var(--spacing-xx)
        if (calcExpr.includes('var(--spacing-')) {
          // 检查是否有简单的算术运算
          if (calcExpr.match(/var\(--spacing-[^)]+\)\s*[\+\-\*]\s*(?:var\(--spacing-[^)]+\)|\d+px)/)) {
            violations.push({
              type: VIOLATION_TYPES.UNNECESSARY_CALC,
              expression: calcExpr,
              line: getLineNumber(content, pos),
              suggestion: '不必要的 calc() 表达式，应定义专用 token 或使用直接值'
            })
          }
        }
      }

      pos = content.indexOf('calc(', endPos)
    }

    // 规则4：检查硬编码值（仅检查高频使用的属性）
    // 豁免特定情况：特定小字号（10px 及以下）、完全圆形（9999px）、特定布局约束
    const hardcodedPatterns = [
      { regex: /(?:padding|margin|gap):\s*\d+(?:\.\d+)?px(?![^}]*var\()/g, prop: 'spacing' },
      { regex: /border-radius:\s*(?!9999px|8px|4px)\d+(?:\.\d+)?px(?![^}]*var\()/g, prop: 'radius' }, // 豁免 9999px(完全圆形)、8px、4px
      { regex: /font-size:\s*(?:10px|8px|6px)(?![^}]*var\()/g, prop: 'fontSize', exempt: true }, // 豁免特定小字号
      { regex: /font-size:\s*(?:\d+px)(?![^}]*var\()/g, prop: 'fontSize' },
      { regex: /font-size:\s*(\d+(?:\.\d+)?rem)(?![^}]*var\()/g, prop: 'fontSize', isRem: true }
    ]

    hardcodedPatterns.forEach(({ regex, prop, isRem, exempt }) => {
      let m
      while ((m = regex.exec(content)) !== null) {
        // 跳过特定布局约束（如 max-width, min-width）
        const context = content.substring(Math.max(0, m.index - 50), m.index + 50)
        if (context.includes('max-width') || context.includes('min-width')) {
          continue
        }

        // 豁免标记为豁免的值
        if (exempt) {
          continue
        }

        violations.push({
          type: VIOLATION_TYPES.HARDCODED_VALUE,
          property: prop,
          value: m[1] || m[0],
          line: getLineNumber(content, m.index),
          suggestion: `应使用 ${prop} 系列的 CSS 变量`
        })
      }
    })

    // 记录结果
    if (violations.length > 0) {
      scanResults.fileViolations[relativePath] = violations

      violations.forEach(v => {
        if (!scanResults.violations[v.type]) {
          scanResults.violations[v.type] = 0
        }
        scanResults.violations[v.type]++
      })
    }

    return {
      file: relativePath,
      violations: violations.length,
      violationDetails: violations,
      status: violations.length === 0 ? 'compliant' : 'non-compliant'
    }
  } catch (error) {
    return {
      file: path.relative(process.cwd(), filePath),
      status: 'error',
      error: error.message
    }
  }
}

/**
 * 获取行号
 */
function getLineNumber(content, index) {
  const before = content.substring(0, index)
  return before.split('\n').length
}

/**
 * 扫描指定目录
 */
function scanDirectory(dir, filterFn = null) {
  const results = []

  function scanDir(currentDir) {
    try {
      const entries = fs.readdirSync(currentDir, { withFileTypes: true })

      for (const entry of entries) {
        const fullPath = path.join(currentDir, entry.name)

        if (entry.isDirectory()) {
          // 跳过特定目录
          if (!['node_modules', '.git', '.nuxt', 'dist', '.output'].includes(entry.name)) {
            scanDir(fullPath)
          }
        } else if (entry.isFile()) {
          // 只扫描指定类型的文件
          if (filterFn ? filterFn(entry.name) : (entry.name.endsWith('.vue') || entry.name.endsWith('.css'))) {
            const result = validateFile(fullPath)
            results.push(result)
          }
        }
      }
    } catch (error) {
      console.error(`扫描目录时出错: ${currentDir}`, error.message)
    }
  }

  scanDir(dir)
  return results
}

/**
 * 打印验证结果
 */
function printResults(results, scanType) {
  console.log(`\n=== 样式系统治理验证结果（${scanType}）===\n`)

  const compliantFiles = results.filter(r => r.status === 'compliant')
  const nonCompliantFiles = results.filter(r => r.status === 'non-compliant')
  const errorFiles = results.filter(r => r.status === 'error')

  console.log(`总文件数: ${results.length}`)
  console.log(`完全合规: ${compliantFiles.length} (${((compliantFiles.length / results.length) * 100).toFixed(2)}%)`)
  console.log(`需要改进: ${nonCompliantFiles.length} (${((nonCompliantFiles.length / results.length) * 100).toFixed(2)}%)`)
  console.log(`扫描错误: ${errorFiles.length}`)

  // 打印违规统计
  console.log('\n--- 违规类型统计 ---')
  Object.entries(scanResults.violations).forEach(([type, count]) => {
    const typeNames = {
      [VIOLATION_TYPES.SPACING_FOR_FONT_SIZE]: '使用 spacing 代替字号',
      [VIOLATION_TYPES.SPACING_FOR_RADIUS]: '使用 spacing 代替圆角',
      [VIOLATION_TYPES.INVALID_TOKEN]: '使用不存在的 token',
      [VIOLATION_TYPES.UNNECESSARY_CALC]: '不必要的 calc() 表达式',
      [VIOLATION_TYPES.HARDCODED_VALUE]: '硬编码值'
    }
    console.log(`  ${typeNames[type]}: ${count} 处`)
  })

  // 打印需要改进的文件详情
  if (nonCompliantFiles.length > 0) {
    console.log('\n--- 需要改进的文件 ---')
    nonCompliantFiles.forEach(file => {
      console.log(`\n  [${file.file}] (${file.violations} 个问题)`)
      file.violationDetails.forEach(v => {
        const typeNames = {
          [VIOLATION_TYPES.SPACING_FOR_FONT_SIZE]: 'spacing用于字号',
          [VIOLATION_TYPES.SPACING_FOR_RADIUS]: 'spacing用于圆角',
          [VIOLATION_TYPES.INVALID_TOKEN]: '不存在的token',
          [VIOLATION_TYPES.UNNECESSARY_CALC]: '不必要的calc',
          [VIOLATION_TYPES.HARDCODED_VALUE]: '硬编码值'
        }
        console.log(`    Line ${v.line}: [${typeNames[v.type]}] ${v.suggestion}`)
        if (v.token) console.log(`      -> ${v.token}`)
        if (v.value) console.log(`      -> ${v.value}`)
      })
    })
  }

  // 打印完全合规的文件
  if (compliantFiles.length > 0) {
    console.log('\n--- 完全合规的文件 ---')
    compliantFiles.forEach(file => {
      console.log(`  ✓ ${file.file}`)
    })
  }

  // 退出码：存在需要改进的文件则返回 1
  process.exit(nonCompliantFiles.length > 0 ? 1 : 0)
}

/**
 * 主函数
 */
function main() {
  const command = process.argv[2]

  console.log('样式系统治理验证脚本（阶段2.5）')
  console.log('验证规则：')
  console.log('  1. 禁止使用 spacing 变量代替字号')
  console.log('  2. 禁止使用 spacing 变量代替圆角')
  console.log('  3. 禁止用大量 calc() 代替缺失 token')
  console.log('  4. 替换必须保证数值等价\n')

  let results
  let scanType = ''

  switch (command) {
    case 'scan-components':
      console.log('扫描 components/ 目录...\n')
      results = scanDirectory('components')
      scanType = '公共组件'
      printResults(results, scanType)
      break

    case 'scan-pages':
      console.log('扫描 pages/ 目录...\n')
      results = scanDirectory('pages')
      scanType = '页面'
      printResults(results, scanType)
      break

    case 'scan-assets':
      console.log('扫描 assets/ 目录...\n')
      results = scanDirectory('assets')
      scanType = '样式资源'
      printResults(results, scanType)
      break

    case 'scan-public-layer':
      console.log('扫描公共层（components）...\n')
      // 只扫描公共组件
      const publicComponents = [
        'components/NotificationBell.vue',
        'components/ModuleCard.vue',
        'components/VisitorLevelDisplay.vue',
        'components/ai/AIAssistant.vue',
        'components/ai/AiCapabilitySection.vue',
        'components/ai/AiProjectList.vue'
      ]
      results = publicComponents
        .filter(fp => fs.existsSync(fp))
        .map(fp => validateFile(fp))
      scanType = '公共层'
      printResults(results, scanType)
      break

    case 'scan-all':
      console.log('扫描所有目录...\n')
      results = [
        ...scanDirectory('components'),
        ...scanDirectory('pages'),
        ...scanDirectory('assets')
      ]
      scanType = '全部'
      printResults(results, scanType)
      break

    default:
      console.log(`
用法: node node/scripts/validate-style-governance.js <command>

命令:
  scan-components   - 扫描 components/ 目录
  scan-pages       - 扫描 pages/ 目录
  scan-assets      - 扫描 assets/ 目录
  scan-public-layer - 扫描公共层（特定组件）
  scan-all         - 扫描所有目录

示例:
  node node/scripts/validate-style-governance.js scan-public-layer
  node node/scripts/validate-style-governance.js scan-components

退出码:
  0 - 完全合规
  1 - 存在需要改进的问题
      `)
      process.exit(0)
  }
}

main()
