# 优化脚本使用指南

本目录包含了用于提升项目性能的各种脚本。

## 📋 脚本列表

### 构建和优化
| 脚本 | 命令 | 用途 |
|------|------|------|
| `build-css.js` | `npm run build:css` | 合并和压缩 CSS 文件 |
| `optimize-images.js` | `npm run optimize:images` | 优化图片资源 |
| `optimize-all.js` | `npm run optimize:all` | 运行所有优化任务 |

### 清理和维护
| 脚本 | 命令 | 用途 |
|------|------|------|
| `clean-node-modules.js` | `npm run clean:modules` | 清理 node_modules 和缓存 |
| `fast-start.ps1` | PowerShell 快速启动 | 检查环境并启动项目 |
| `fast-start.sh` | Bash 快速启动 | Linux/macOS 快速启动 |

## 🚀 快速开始

### 开发环境
```bash
# Windows
.\scripts\fast-start.ps1

# Linux/macOS
./scripts/fast-start.sh
```

### 生产环境构建
```bash
# 运行所有优化
npm run optimize:all

# 单独步骤
npm run build:css          # 优化 CSS
npm run optimize:images    # 优化图片
npm run build              # 构建
```

### 维护和清理
```bash
# 清理依赖和缓存
npm run clean:modules

# 手动清理
rm -rf node_modules package-lock.json
npm install
```

## 📊 优化效果

### 已完成的优化
- ✅ **依赖精简**: 移除了重复的 chart.js
- ✅ **代码分割**: 大型库单独打包
- ✅ **CSS 合并**: 25个文件合并为1个
- ✅ **图片优化**: WebP 格式，减少40%体积
- ✅ **懒加载**: 图片按需加载
- ✅ **性能监控**: 实时监控组件

### 预期效果
- **启动时间**: 减少 40-50%
- **首屏加载**: 减少 40-60%
- **包体积**: 减少 30-40%
- **内存占用**: 优化 20-30%

## 🎯 使用建议

### 开发时
1. 使用 `fast-start.ps1` 或 `fast-start.sh` 启动
2. 修改代码后自动热重载
3. 使用 `PerformanceMonitor` 组件监控性能

### 部署前
1. 运行 `npm run optimize:all`
2. 检查优化后的文件大小
3. 运行测试确保功能正常

### 定期维护
1. 每周运行 `clean:modules` 清理缓存
2. 添加新图片后运行 `optimize:images`
3. 定期检查 `node_modules` 大小

## 🔧 故障排除

### 问题：npm 命令未找到
```bash
# 安装 npm（如果需要）
# Windows: 下载并安装 Node.js
# Linux: sudo apt install nodejs npm
```

### 问题：优化后样式错乱
```bash
# 重新构建 CSS
npm run build:css

# 重新构建项目
rm -rf .nuxt
npm run dev
```

### 问题：图片加载失败
```bash
# 确保图片路径正确
# 检查优化后的图片文件是否存在
```

## 📚 相关文档

- [性能优化总结](../docs/improvements/PERFORMANCE_OPTIMIZATION_SUMMARY.md)
- [依赖优化计划](../docs/improvements/DEPENDENCY_OPTIMIZATION.md)
- [开发规范](../docs/development/DEVELOPMENT_GUIDELINES.md)