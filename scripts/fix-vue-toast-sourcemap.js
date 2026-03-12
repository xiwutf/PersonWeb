/**
 * 为 vue-toast-notification 补全缺失的 mitt.mjs.map，消除 Vite 开发时的 source map 警告
 * 在 npm install 后通过 postinstall 自动执行
 */
const fs = require('fs')
const path = require('path')

const emptyMap = '{"version":3,"sources":[],"mappings":""}'
const targetDir = path.join(__dirname, '..', 'node_modules', 'vue-toast-notification', 'dist')
const targetFile = path.join(targetDir, 'mitt.mjs.map')

try {
  if (!fs.existsSync(targetFile)) {
    if (!fs.existsSync(targetDir)) {
      console.log('postinstall: vue-toast-notification/dist 不存在，跳过补全 source map')
      process.exit(0)
    }
    fs.writeFileSync(targetFile, emptyMap, 'utf8')
    console.log('postinstall: 已创建 vue-toast-notification/dist/mitt.mjs.map')
  }
} catch (e) {
  console.warn('postinstall: 创建 mitt.mjs.map 失败', e.message)
}
