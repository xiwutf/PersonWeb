/**
 * 压缩 public/images/projects/covers 下的 PNG 封面为 WebP
 * 目标：单张 < 500 KB，总图片预算可通过 check:budget
 */

const fs = require('fs')
const path = require('path')
const sharp = require('sharp')

const COVERS_DIR = path.join(__dirname, '../public/images/projects/covers')
const MAX_WIDTH = 1024
const WEBP_QUALITY = 80

const COVER_PNGS = [
  'mindtrace.png',
  'personweb.png',
  'personweb-system.png',
  'ai-service.png',
  'tools.png',
  'labs.png',
  'ai-assistant.png',
  'tool-market.png',
  'theme-store.png',
  'love-cube.png',
  'investment-system.png',
  'iot-control.png',
  'finance-assistant.png',
]

function formatSize(bytes) {
  return `${(bytes / 1024).toFixed(1)} KB`
}

async function optimizeCover(pngName) {
  const inputPath = path.join(COVERS_DIR, pngName)
  if (!fs.existsSync(inputPath)) {
    console.log(`跳过（不存在）: ${pngName}`)
    return
  }

  const baseName = path.basename(pngName, '.png')
  const outputPath = path.join(COVERS_DIR, `${baseName}.webp`)
  const before = fs.statSync(inputPath).size

  await sharp(inputPath)
    .resize(MAX_WIDTH, null, { fit: 'inside', withoutEnlargement: true })
    .webp({ quality: WEBP_QUALITY, effort: 6 })
    .toFile(outputPath)

  const after = fs.statSync(outputPath).size
  fs.unlinkSync(inputPath)

  console.log(
    `${baseName}: ${formatSize(before)} → ${formatSize(after)} (webp)`,
  )
}

async function main() {
  if (!fs.existsSync(COVERS_DIR)) {
    console.error(`目录不存在: ${COVERS_DIR}`)
    process.exit(1)
  }

  console.log('正在优化项目封面 PNG → WebP...\n')

  for (const png of COVER_PNGS) {
    await optimizeCover(png)
  }

  console.log('\n项目封面优化完成。请确认 constants/projects/covers.ts 已使用 .webp 路径。')
}

if (require.main === module) {
  main().catch((error) => {
    console.error('优化失败:', error)
    process.exit(1)
  })
}

module.exports = { optimizeCover, COVER_PNGS }
