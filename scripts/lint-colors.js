/**
 * 颜色检查脚本
 * 扫描项目中的硬编码颜色值，输出警告
 * 
 * 使用方法：
 *   node scripts/lint-colors.js
 * 
 * 或在 package.json 中添加：
 *   "lint:colors": "node scripts/lint-colors.js"
 */

const fs = require('fs')
const path = require('path')

// 需要检查的文件类型
const fileExtensions = ['.vue', '.css', '.scss', '.ts', '.js']

// 需要检查的颜色模式
const colorPatterns = [
  {
    pattern: /#[0-9a-fA-F]{3,6}\b/g,
    name: '十六进制颜色'
  },
  {
    pattern: /rgba?\([^)]+\)/g,
    name: 'RGB/RGBA 颜色'
  }
]

// 允许的颜色（特殊组件使用，需注释说明）
const allowedColors = [
  // 图表颜色（在 useEChartsTheme.ts 中定义）
  '#3b82f6',  // 主色调
  '#10b981',  // 成功色
  '#f59e0b',  // 警告色
  '#ef4444',  // 错误色
  '#8b5cf6',  // 紫色
  '#ffffff',  // 白色（用于图表文字等）
  '#000000',  // 黑色（用于图表文字等）
  // 特殊效果（需注释说明）
  'rgba(255,255,255,0.05)',  // 深色模式蒙层（特殊组件）
  'rgba(255,255,255,0.1)',   // 深色模式边框（特殊组件）
]

// 需要排除的文件/目录
const excludePatterns = [
  /node_modules/,
  /dist/,
  /\.git/,
  /tokens\.css$/,  // tokens.css 中的颜色是兼容变量
  /useEChartsTheme\.ts$/,  // ECharts 主题文件（允许使用颜色）
  /lint-colors\.js$/,  // 本脚本文件
  /\.test\./,  // 测试文件
  /\.spec\./,  // 测试文件
]

// 需要排除的目录
const excludeDirs = [
  'node_modules',
  'dist',
  '.git',
  '.nuxt',
  '.output',
]

/**
 * 检查文件是否需要排除
 */
function shouldExclude(filePath) {
  // 检查排除模式
  if (excludePatterns.some(pattern => pattern.test(filePath))) {
    return true
  }
  
  // 检查排除目录
  const parts = filePath.split(path.sep)
  if (parts.some(part => excludeDirs.includes(part))) {
    return true
  }
  
  return false
}

/**
 * 检查颜色是否在允许列表中
 */
function isAllowedColor(color) {
  return allowedColors.includes(color.toLowerCase())
}

/**
 * 检查是否是特殊注释标记的颜色
 */
function hasSpecialComment(line, index, content) {
  // 检查前后几行是否有特殊注释
  const lines = content.split('\n')
  const startLine = Math.max(0, index - 3)
  const endLine = Math.min(lines.length - 1, index + 3)
  
  for (let i = startLine; i <= endLine; i++) {
    const line = lines[i]
    if (line.includes('特殊视觉组件') || 
        line.includes('允许自定义颜色') ||
        line.includes('特殊效果')) {
      return true
    }
  }
  
  return false
}

/**
 * 扫描文件
 */
function scanFile(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf-8')
    const issues = []
    
    colorPatterns.forEach(({ pattern, name }) => {
      const matches = content.matchAll(pattern)
      for (const match of matches) {
        const color = match[0]
        const lineNumber = content.substring(0, match.index).split('\n').length
        const line = content.split('\n')[lineNumber - 1]
        
        // 检查是否是允许的颜色
        if (isAllowedColor(color)) {
          continue
        }
        
        // 检查是否有特殊注释
        if (hasSpecialComment(line, lineNumber - 1, content)) {
          continue
        }
        
        // 检查是否是 CSS 变量（var(--xxx)）
        if (line.includes('var(--')) {
          continue
        }
        
        // 检查是否是字符串内容（可能是示例代码）
        const beforeMatch = content.substring(0, match.index)
        const afterMatch = content.substring(match.index + match[0].length)
        const beforeQuote = beforeMatch.lastIndexOf('"')
        const afterQuote = afterMatch.indexOf('"')
        if (beforeQuote > -1 && afterQuote > -1) {
          // 可能是字符串内容，跳过
          continue
        }
        
        issues.push({
          file: filePath,
          line: lineNumber,
          color: color,
          type: name,
          context: line.trim()
        })
      }
    })
    
    return issues
  } catch (error) {
    console.error(`读取文件失败: ${filePath}`, error)
    return []
  }
}

/**
 * 扫描目录
 */
function scanDirectory(dir, issues = []) {
  try {
    const files = fs.readdirSync(dir)
    
    files.forEach(file => {
      const filePath = path.join(dir, file)
      
      // 检查是否需要排除
      if (shouldExclude(filePath)) {
        return
      }
      
      try {
        const stat = fs.statSync(filePath)
        
        if (stat.isDirectory()) {
          scanDirectory(filePath, issues)
        } else if (fileExtensions.some(ext => file.endsWith(ext))) {
          const fileIssues = scanFile(filePath)
          issues.push(...fileIssues)
        }
      } catch (error) {
        // 忽略无法访问的文件
      }
    })
    
    return issues
  } catch (error) {
    console.error(`扫描目录失败: ${dir}`, error)
    return issues
  }
}

/**
 * 主函数
 */
function main() {
  const projectRoot = path.resolve(__dirname, '..')
  const issues = scanDirectory(projectRoot)
  
  if (issues.length === 0) {
    console.log('✅ 未发现硬编码颜色问题')
    console.log('💡 所有颜色都通过 themeOverrides 或 Naive UI 组件控制\n')
    process.exit(0)
  }
  
  console.log(`\n⚠️  发现 ${issues.length} 个硬编码颜色问题：\n`)
  
  // 按文件分组
  const issuesByFile = {}
  issues.forEach(issue => {
    if (!issuesByFile[issue.file]) {
      issuesByFile[issue.file] = []
    }
    issuesByFile[issue.file].push(issue)
  })
  
  // 输出问题
  Object.keys(issuesByFile).forEach(file => {
    const fileIssues = issuesByFile[file]
    console.log(`  📄 ${file}`)
    fileIssues.forEach(issue => {
      console.log(`    第 ${issue.line} 行: ${issue.color} (${issue.type})`)
      console.log(`    上下文: ${issue.context.substring(0, 80)}${issue.context.length > 80 ? '...' : ''}`)
    })
    console.log('')
  })
  
  console.log('💡 建议：')
  console.log('  - 使用 themeOverrides 或 Naive UI 组件')
  console.log('  - 参考 docs/DESIGN_SYSTEM_V1.md')
  console.log('  - 参考 docs/CODING_STYLE_UI.md')
  console.log('  - 特殊组件需添加注释：// 特殊视觉组件，允许自定义颜色\n')
  
  process.exit(1)
}

// 运行主函数
if (require.main === module) {
  main()
}

module.exports = { scanFile, scanDirectory }

