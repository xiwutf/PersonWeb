#!/usr/bin/env node

const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('🧹 开始清理 node_modules...\n');

// 1. 清理缓存
console.log('🗑️ 清理 npm 缓存...');
try {
  execSync('npm cache clean --force', { stdio: 'inherit' });
  console.log('✅ npm 缓存清理完成\n');
} catch (error) {
  console.log('⚠️ npm 缓存清理失败:', error.message);
}

// 2. 删除 package-lock.json 重新安装
const packageLockPath = path.join(__dirname, '../package-lock.json');
if (fs.existsSync(packageLockPath)) {
  console.log('📦 删除 package-lock.json...');
  fs.unlinkSync(packageLockPath);
  console.log('✅ package-lock.json 已删除\n');
}

// 3. 重新安装依赖（只安装生产依赖）
console.log('📥 重新安装依赖...');
try {
  execSync('npm install --production', {
    stdio: 'inherit',
    cwd: path.join(__dirname, '..')
  });
  console.log('✅ 生产依赖安装完成\n');
} catch (error) {
  console.log('❌ 生产依赖安装失败:', error.message);
  console.log('尝试完整安装...');
  execSync('npm install', {
    stdio: 'inherit',
    cwd: path.join(__dirname, '..')
  });
}

// 4. 清理构建缓存
const nuxtCacheDir = path.join(__dirname, '../.nuxt');
if (fs.existsSync(nuxtCacheDir)) {
  console.log('🗄️ 清理 Nuxt 缓存...');
  fs.rmSync(nuxtCacheDir, { recursive: true, force: true });
  console.log('✅ Nuxt 缓存清理完成\n');
}

// 5. 清理 dist 目录
const distDir = path.join(__dirname, '../dist');
if (fs.existsSync(distDir)) {
  console.log('🗂️ 清理 dist 目录...');
  fs.rmSync(distDir, { recursive: true, force: true });
  console.log('✅ dist 目录清理完成\n');
}

console.log('🎉 清理完成！现在运行:');
console.log('   npm run dev  # 开发模式');
console.log('   npm run build # 生产构建');

console.log('\n💡 提示：');
console.log('   - 依赖包已精简，移除了重复的 chart.js');
console.log('   - 生产环境可考虑使用 CDN 加载大型库');
console.log('   - 定期运行此脚本保持项目整洁');