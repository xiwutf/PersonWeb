/**
 * 设计令牌验证脚本
 * 用途：验证是否使用了语义变量而非硬编码值
 */

const fs = require('fs')
const path = require('path')

// 扫描结果统计
const stats = {
  totalFiles: 0,
  filesWithHardcoded: 0,
  filesWithSemantic: 0,
  filesMixed: 0,
  totalHardcoded: 0,
  totalSemantic: 0
}

/**
 * 验证单个文件
 */
function verifyFile(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8')

    const hasHardcodedSpacing = /(?:padding|margin|gap):\s*\d+(?:\.\d+)?px/.test(content)
    const hasSemanticSpacing = /var\(--spacing-[\d_]+\)/.test(content)

    const hasHardcodedRadius = /border-radius:\s*\d+(?:\.\d+)?px/.test(content)
    const hasSemanticRadius = /var\(--radius-[\w-]+\)/.test(content)

    const hasHardcodedFontSize = /font-size:\s*\d+px/.test(content)
    const hasSemanticFontSize = /var\(--text-[\w-]+\)/.test(content)

    // === 语义错误检测 ===

    // 错误：font-size 使用 spacing token
    const fontSizeUsesSpacing = /font-size:\s*var\(--spacing-[\w-]+\)/.test(content)

    // 错误：border-radius 使用 spacing token
    const radiusUsesSpacing = /border-radius:\s*var\(--spacing-[\w-]+\)/.test(content)

    // 错误：使用不存在的 spacing token (12xl, 5xl, 6xl 等)
    const invalidSpacingTokens12xl = /--spacing-12xl\b/.test(content)
    const invalidSpacingTokens5xl = /--spacing-5xl\b/.test(content)
    const invalidSpacingTokens6xl = /--spacing-6xl\b/.test(content)
    const invalidSpacingTokens7xl = /--spacing-7xl\b/.test(content)
    const invalidSpacingTokens8xl = /--spacing-8xl\b/.test(content)
    const invalidSpacingTokens9xl = /--spacing-9xl\b/.test(content)

    // 错误：使用不存在的 text token
    // 注意：--text-6xl 到 --text-9xl 是合法 token，不拦截
    const invalidTextToken0 = /--text-0\b/.test(content)
    const invalidTextToken10 = /--text-10\b/.test(content)

    // 错误：使用不存在的 radius token
    // 注意：--radius-4xl 是合法 token，不拦截
    const invalidRadiusToken5xl = /--radius-5xl\b/.test(content)
    const invalidRadiusToken6xl = /--radius-6xl\b/.test(content)

    // 收集所有语义错误
    const semanticErrors = []
    if (fontSizeUsesSpacing) semanticErrors.push('font-size使用了spacing token')
    if (radiusUsesSpacing) semanticErrors.push('border-radius使用了spacing token')
    if (invalidSpacingTokens12xl || invalidSpacingTokens5xl || invalidSpacingTokens6xl ||
        invalidSpacingTokens7xl || invalidSpacingTokens8xl || invalidSpacingTokens9xl) {
      semanticErrors.push('使用不存在的spacing token')
    }
    if (invalidTextToken0 || invalidTextToken10) {
      semanticErrors.push('使用不存在的text token')
    }
    if (invalidRadiusToken5xl || invalidRadiusToken6xl) {
      semanticErrors.push('使用不存在的radius token')
    }

    stats.totalFiles++

    // 分类文件
    const hasAnyHardcoded = hasHardcodedSpacing || hasHardcodedRadius || hasHardcodedFontSize
    const hasAnySemantic = hasSemanticSpacing || hasSemanticRadius || hasSemanticFontSize
    const hasSemanticError = semanticErrors.length > 0

    if (hasSemanticError) {
      stats.filesWithHardcoded++
      stats.totalHardcoded++
    } else if (hasAnyHardcoded && !hasAnySemantic) {
      stats.filesWithHardcoded++
      stats.totalHardcoded++
    } else if (hasAnyHardcoded && hasAnySemantic) {
      stats.filesMixed++
    } else if (!hasAnyHardcoded && hasAnySemantic) {
      stats.filesWithSemantic++
      stats.totalSemantic++
    }

    return {
      file: filePath.replace(process.cwd() + '/', ''),
      hardcoded: {
        spacing: hasHardcodedSpacing,
        radius: hasHardcodedRadius,
        fontSize: hasHardcodedFontSize
      },
      semantic: {
        spacing: hasSemanticSpacing,
        radius: hasSemanticRadius,
        fontSize: hasSemanticFontSize
      },
      semanticErrors,
      status: (hasSemanticError || hasAnyHardcoded) ? 'needs_work' : 'good'
    }
  } catch (error) {
    return {
      file: filePath.replace(process.cwd() + '/', ''),
      status: 'error',
      error: error.message
    }
  }
}

/**
 * 扫描指定目录（递归）
 */
function scanDirectory(dir) {
  const results = []

  function scanDir(currentDir) {
    try {
      const entries = fs.readdirSync(currentDir, { withFileTypes: true })

      for (const entry of entries) {
        const fullPath = path.join(currentDir, entry.name)

        if (entry.isDirectory()) {
          // 跳过 node_modules, .git, .nuxt 等目录
          if (!['node_modules', '.git', '.nuxt', 'dist', '.output'].includes(entry.name)) {
            scanDir(fullPath)
          }
        } else if (entry.isFile() && (entry.name.endsWith('.vue') || entry.name.endsWith('.css') || entry.name.endsWith('.scss'))) {
          const result = verifyFile(fullPath)
          results.push(result)
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
function printResults() {
  console.log('\n=== 设计令牌验证结果 ===\n')

  console.log(`总文件数: ${stats.totalFiles}`)
  console.log(`完全合规: ${stats.filesWithSemantic} (${((stats.filesWithSemantic / stats.totalFiles) * 100).toFixed(2)}%)`)
  console.log(`混合状态: ${stats.filesMixed} (${((stats.filesMixed / stats.totalFiles) * 100).toFixed(2)}%)`)
  console.log(`完全不合规: ${stats.filesWithHardcoded} (${((stats.filesWithHardcoded / stats.totalFiles) * 100).toFixed(2)}%)`)

  // 退出码
  process.exit(stats.filesWithHardcoded > 0 ? 1 : 0)
}

/**
 * 主函数
 */
function main() {
  const command = process.argv[2]

  console.log('开始验证设计令牌使用...')

  switch (command) {
    case 'scan-pages':
      console.log('扫描 pages/ 目录...')
      const pagesResults = scanDirectory('pages')
      pagesResults.forEach(result => {
        if (result.status === 'needs_work') {
          console.log(`  [${result.file}] 需要改进`)
          const issues = Object.entries(result.hardcoded).filter(([_, value]) => value)
          if (issues.length > 0) {
            console.log(`    硬编码：${issues.map(([k]) => k).join(', ')}`)
          }
          if (result.semanticErrors && result.semanticErrors.length > 0) {
            console.log(`    语义错误：${result.semanticErrors.join(', ')}`)
          }
        } else if (result.status === 'good') {
          console.log(`  [${result.file}] 完全合规`)
        } else if (result.status === 'error') {
          console.log(`  [${result.file}] 错误: ${result.error}`)
        }
      })
      printResults()
      break

    case 'scan-components':
      console.log('扫描 components/ 目录...')
      const componentsResults = scanDirectory('components')
      componentsResults.forEach(result => {
        if (result.status === 'needs_work') {
          console.log(`  [${result.file}] 需要改进`)
          const issues = Object.entries(result.hardcoded).filter(([_, value]) => value)
          if (issues.length > 0) {
            console.log(`    硬编码：${issues.map(([k]) => k).join(', ')}`)
          }
          if (result.semanticErrors && result.semanticErrors.length > 0) {
            console.log(`    语义错误：${result.semanticErrors.join(', ')}`)
          }
        } else if (result.status === 'good') {
          console.log(`  [${result.file}] 完全合规`)
        } else if (result.status === 'error') {
          console.log(`  [${result.file}] 错误: ${result.error}`)
        }
      })
      printResults()
      break

    case 'scan-assets-css':
      console.log('扫描 assets/css/ 目录...')
      const cssResults = scanDirectory('assets/css')
      cssResults.forEach(result => {
        if (result.status === 'needs_work') {
          console.log(`  [${result.file}] 需要改进`)
          const issues = Object.entries(result.hardcoded).filter(([_, value]) => value)
          if (issues.length > 0) {
            console.log(`    硬编码：${issues.map(([k]) => k).join(', ')}`)
          }
          if (result.semanticErrors && result.semanticErrors.length > 0) {
            console.log(`    语义错误：${result.semanticErrors.join(', ')}`)
          }
        } else if (result.status === 'good') {
          console.log(`  [${result.file}] 完全合规`)
        } else if (result.status === 'error') {
          console.log(`  [${result.file}] 错误: ${result.error}`)
        }
      })
      printResults()
      break

    case 'scan-all':
      console.log('扫描所有目录（pages + components + assets）...')
      const allResults = [
        ...scanDirectory('pages'),
        ...scanDirectory('components'),
        ...scanDirectory('assets/css'),
        ...scanDirectory('assets/styles')
      ]

      console.log('\n=== 扫描结果 ===')
      allResults.forEach(result => {
        if (result.status === 'needs_work') {
          console.log(`  [${result.file}] 需要改进`)
          const issues = Object.entries(result.hardcoded).filter(([_, value]) => value)
          if (issues.length > 0) {
            console.log(`    硬编码：${issues.map(([k]) => k).join(', ')}`)
          }
          if (result.semanticErrors && result.semanticErrors.length > 0) {
            console.log(`    语义错误：${result.semanticErrors.join(', ')}`)
          }
        } else if (result.status === 'good') {
          console.log(`  [${result.file}] 完全合规`)
        } else if (result.status === 'error') {
          console.log(`  [${result.file}] 错误: ${result.error}`)
        }
      })
      printResults()
      break

    default:
      console.log(`
用法: node node/scripts/verify-design-tokens.js <command>

命令:
  scan-pages      - 扫描 pages/ 目录
  scan-components  - 扫描 components/ 目录
  scan-assets-css  - 扫描 assets/css/ 目录
  scan-all        - 扫描所有目录

验证规则:
  - 间距应该使用 var(--spacing-*) 变量（数字序列，如 --spacing-1, --spacing-2）
  - 圆角应该使用 var(--radius-*) 变量
  - 字体大小应该使用 var(--text-*) 变量
  - 禁止：font-size 使用 --spacing-* 变量（语义错误）
  - 禁止：border-radius 使用 --spacing-* 变量（语义错误）
  - 禁止：使用不存在的 token（如 --spacing-12xl, --spacing-5xl 等）

示例:
  npm run style:tokens:verify scan-all
  node node/scripts/verify-design-tokens.js scan-pages

退出码:
   0 - 完全合规（无硬编码）
   1 - 有硬编码，需要修复
  `)
  }
}

main()
