/**
 * 设计令牌检查脚本
 * 用途：扫描并验证是否使用了语义变量而非硬编码值
 */

const { execSync } = require('child_process')
const { readFileSync, writeFileSync } = require('fs')
const { resolve } = require('path')

// 扫描目录
const pagesDir = resolve(process.cwd(), 'pages')
const assetsDir = resolve(process.cwd(), 'assets')
const componentsDir = resolve(process.cwd(), 'components')

/**
 * 扫描并统计硬编码值
 */
function scanHardcodedValues(dir) {
  const result = {
    spacing: [],
    borderRadius: [],
    fontSize: [],
    shadow: [],
    total: 0
  }

  try {
    const files = execSync(`find "${dir}" -name "*.vue" -o -name "*.css"`, { encoding: 'utf8' })
    files.forEach(file => {
      const filePath = resolve(dir, file)
      const content = readFileSync(filePath, 'utf8')

      // 扫描硬编码间距值
      const spacingMatches = content.matchAll(/(?:padding|margin|gap):\s*(\d+(?:\.\d+)?px)/g)
      spacingMatches.forEach(match => {
        const value = match[0] // 获取数字部分
        result.spacing.push({ file, value, line: getLineNumber(content, match.index) })
      })

      // 扫描硬编码圆角值
      const radiusMatches = content.matchAll(/border-radius:\s*(\d+(?:\.\d+)?px)/g)
      radiusMatches.forEach(match => {
        const value = match[0] // 获取数字部分
        result.borderRadius.push({ file, value, line: getLineNumber(content, match.index) })
      })

      // 扫描硬编码字体大小（px 单位）
      const fontSizeMatches = content.matchAll(/font-size:\s*(\d+px)/g)
      fontSizeMatches.forEach(match => {
        const value = match[0] // 获取数字部分
        result.fontSize.push({ file, value, line: getLineNumber(content, match.index) })
      })

      result.total++
    })
  } catch (error) {
    console.error(`扫描目录时出错: ${dir}`, error.message)
  }

  return result
}

/**
 * 获取行号（简化版）
 */
function getLineNumber(content, index) {
  const before = content.substring(0, index).split('\n').length
  return before + 1
}

/**
 * 检查是否使用了语义变量
 */
function checkSemanticVariables(dir) {
  const result = {
    spacing: [],
    borderRadius: [],
    fontSize: [],
    total: 0
  }

  try {
    const files = execSync(`find "${dir}" -name "*.vue" -o -name "*.css"`, { encoding: 'utf8' })
    files.forEach(file => {
      const filePath = resolve(dir, file)
      const content = readFileSync(filePath, 'utf8')

      // 检查语义间距变量使用
      const spacingVarMatches = content.matchAll(/var\(--spacing-[\d]+)/g)
      spacingVarMatches.forEach(match => {
        result.spacing.push({ file, variable: match[0], line: getLineNumber(content, match.index) })
      })

      // 检查语义圆角变量使用
      const radiusVarMatches = content.matchAll(/var\(--radius-[\w]+)/g)
      radiusVarMatches.forEach(match => {
        result.borderRadius.push({ file, variable: match[0], line: getLineNumber(content, match.index) })
      })

      result.total++
    })
  } catch (error) {
    console.error(`检查语义变量时出错: ${dir}`, error.message)
  }

  return result
}

/**
 * 打印扫描结果
 */
function printScanResult(result) {
  console.log('\n=== 硬编码值扫描结果 ===\n')
  console.log(`总计发现: ${result.total} 个文件\n`)

  console.log('\n--- 间距硬编码 ---')
  console.log(`发现 ${result.spacing.length} 处硬编码间距:`)
  result.spacing.forEach((item, index) => {
    if (index < 10) {
      console.log(`  ${index + 1}. ${item.file}:${item.line} - ${item.value}`)
    }
    if (index === result.spacing.length - 1 && result.spacing.length > 10) {
      console.log(`  ... 还有 ${result.spacing.length - 10} 处\n`)
    }
  })

  console.log('\n--- 圆角硬编码 ---')
  console.log(`发现 ${result.borderRadius.length} 处硬编码圆角:`)
  result.borderRadius.forEach((item, index) => {
    if (index < 10) {
      console.log(`  ${index + 1}. ${item.file}:${item.line} - ${item.value}`)
    }
    if (index === result.borderRadius.length - 1 && result.borderRadius.length > 10) {
      console.log(`  ... 还有 ${result.borderRadius.length - 10} 处\n`)
    }
  })

  console.log('\n--- 字体大小硬编码 ---')
  console.log(`发现 ${result.fontSize.length} 处硬编码字体大小:`)
  result.fontSize.forEach((item, index) => {
    if (index < 10) {
      console.log(`  ${index + 1}. ${item.file}:${item.line} - ${item.value}`)
    }
    if (index === result.fontSize.length - 1 && result.fontSize.length > 10) {
      console.log(`  ... 还有 ${result.fontSize.length - 10} 处\n`)
    }
  })
}

/**
 * 打印语义变量检查结果
 */
function printSemanticCheck(result) {
  console.log('\n=== 语义变量检查结果 ===\n')
  console.log(`总计发现: ${result.total} 个文件\n`)

  console.log('\n--- 间距变量使用 ---')
  console.log(`发现 ${result.spacing.length} 处语义间距变量:`)
  result.spacing.forEach((item, index) => {
    if (index < 10) {
      console.log(`  ${index + 1}. ${item.file}:${item.line} - ${item.variable}`)
    }
    if (index === result.spacing.length - 1 && result.spacing.length > 10) {
      console.log(`  ... 还有 ${result.spacing.length - 10} 处\n`)
    }
  })

  console.log('\n--- 圆角变量使用 ---')
  console.log(`发现 ${result.borderRadius.length} 处语义圆角变量:`)
  result.borderRadius.forEach((item, index) => {
    if (index < 10) {
      console.log(`  ${index + 1}. ${item.file}:${item.line} - ${item.variable}`)
    }
    if (index === result.borderRadius.length - 1 && result.borderRadius.length > 10) {
      console.log(`  ... 还有 ${result.borderRadius.length - 10} 处\n`)
    }
  })
}

// 主函数
function main() {
  const command = process.argv[2]

  switch (command) {
    case 'scan':
      console.log('开始扫描硬编码值...')
      const hardcodedResult = {
        pages: scanHardcodedValues(pagesDir),
        assets: scanHardcodedValues(assetsDir),
        components: scanHardcodedValues(componentsDir)
      }

      printScanResult(hardcodedResult.pages)
      console.log('\nassets/css:')
      printScanResult(hardcodedResult.assets)
      console.log('\ncomponents:')
      printScanResult(hardcodedResult.components)
      break

    case 'check':
      console.log('开始检查语义变量...')
      const semanticCheck = {
        pages: checkSemanticVariables(pagesDir),
        assets: checkSemanticVariables(assetsDir),
        components: checkSemanticVariables(componentsDir)
      }

      printSemanticCheck(semanticCheck.pages)
      console.log('\nassets/css:')
      printSemanticCheck(semanticCheck.assets)
      console.log('\ncomponents:')
      printSemanticCheck(semanticCheck.components)
      break

    case 'full':
      console.log('开始完整扫描...')
      console.log('\n=== 步骤 1: 扫描硬编码值 ===')
      const hardcodedResult = {
        pages: scanHardcodedValues(pagesDir),
        assets: scanHardcodedValues(assetsDir),
        components: scanHardcodedValues(componentsDir)
      }
      printScanResult(hardcodedResult.pages)
      console.log('\nassets/css:')
      printScanResult(hardcodedResult.assets)
      console.log('\ncomponents:')
      printScanResult(hardcodedResult.components)

      console.log('\n=== 步骤 2: 检查语义变量 ===')
      const semanticCheck = {
        pages: checkSemanticVariables(pagesDir),
        assets: checkSemanticVariables(assetsDir),
        components: checkSemanticVariables(componentsDir)
      }
      printSemanticCheck(semanticCheck.pages)
      console.log('\nassets/css:')
      printSemanticCheck(semanticCheck.assets)
      console.log('\ncomponents:')
      printSemanticCheck(semanticCheck.components)
      break

    default:
      console.log(`
用法: node scripts/check-design-tokens.js <command>

命令:
  scan          - 扫描硬编码值（间距、圆角、字体大小）
  check         - 检查语义变量使用
  full          - 执行完整扫描（硬编码 + 语义变量）

示例:
  npm run style:tokens:scan
  npm run style:tokens:check
  npm run style:tokens:full
      `)
      break
  }
}

if (command) {
  main()
} else {
  main('scan')
}
