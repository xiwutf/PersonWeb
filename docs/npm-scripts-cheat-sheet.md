# NPM 脚本速查指南

> 快速查找常用命令，无需记忆所有脚本

---

## 🚀 开发相关

| 脚本 | 说明 | 何时使用 |
|------|------|----------|
| `npm run dev` | 启动开发服务器 | 日常开发 |
| `npm run dev:mobile` | 启动局域网访问 | 手机调试 |
| `npm run dev:prod` | 生产模式调试 | 模拟生产环境 |
| `npm run build` | 构建生产版本 | 部署前 |
| `npm run preview` | 预览构建结果 | 本地验证 |

---

## 🧪 测试相关

| 脚本 | 说明 | 何时使用 |
|------|------|----------|
| `npm test` | 运行测试（监听模式） | 开发时持续测试 |
| `npm run test:run` | 运行测试一次 | CI/CD 或提交前 |
| `npm run test:coverage` | 生成覆盖率报告 | 代码审查 |
| `npm run test:ui` | 打开测试可视化界面 | 调试测试 |

---

## 🎨 样式相关

| 脚本 | 说明 | 何时使用 |
|------|------|----------|
| `npm run build:css` | 构建样式系统 | 修改设计令牌后 |
| `npm run style:lint` | 检查样式规范 | 提交前检查 |
| `npm run style:lint:fix` | 自动修复样式问题 | 发现风格问题 |
| `npm run lint:colors` | 检查颜色使用规范 | 颜色命名不合规 |

---

## 🚦 性能相关

| 脚本 | 说明 | 何时使用 |
|------|------|----------|
| `npm run perf:lighthouse` | 运行 Lighthouse 测试 | 需要先启动 `npm run dev` |
| `npm run check:budget` | 检查资源大小预算 | 构建后验证 |
| `npm run perf:analyze` | 生成性能优化建议 | 性能优化阶段 |
| `npm run bundle:analyze` | 分析打包大小 | 优化 Bundle |
| `npm run perf:regression:test` | 性能回归测试 | 提交前验证 |
| `npm run perf:baseline:update` | 更新性能基线 | 预期的性能变化 |

---

## 🛠️ 维护相关

| 脚本 | 说明 | 何时使用 |
|------|------|----------|
| `npm run optimize:images` | 优化图片 | 添加新图片后 |
| `npm run clean:modules` | 清理 node_modules | 依赖问题 |

---

## ⚡ 常用流程

### 日常开发流程
```bash
npm run dev          # 启动开发
npm run test:run     # 运行测试
npm run style:lint   # 检查样式
```

### 提交前检查
```bash
npm run build
npm run test:run
npm run style:lint
npm run perf:regression:test
```

### 性能优化流程
```bash
npm run build:analyze     # 分析打包
npm run check:budget      # 检查预算
npm run perf:lighthouse   # Lighthouse 测试
```

### CI/CD 流程
```bash
npm run build
npm run test:run
npm run test:coverage
npm run style:lint
npm run perf:regression:test
```

---

## 📌 核心脚本（记住这 5 个就够了）

| 脚本 | 别称 | 记忆方法 |
|------|------|----------|
| `npm run dev` | 开发 | **D**ev = **开**发 |
| `npm run build` | 构建 | **B**uild = **建**设 |
| `npm test` | 测试 | **T**est = **测**试 |
| `npm run style:lint` | 检查样式 | **S**tyle = **样**式 |
| `npm run perf:lighthouse` | 性能测试 | **P**erf = **性**能 |

其他脚本可以在需要时参考此文档！

---

## 💡 小技巧

### 查看所有可用脚本
```bash
npm run
```

### 查看脚本定义
```json
"scripts": {
  "dev": "nuxt dev",      // ← 脚本名称: 命令
  ...
}
```

### 创建自己的别名
```json
"scripts": {
  "start": "npm run dev",
  "precommit": "npm run test:run && npm run style:lint"
}
```

---

*最后更新: 2026-03-16*
