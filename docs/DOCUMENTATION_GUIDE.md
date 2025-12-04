# 文档使用指南

**最后更新**：2024-12-03

## 🎯 快速导航

### 我是新手，从哪里开始？

1. **[项目概览](./PROJECT_OVERVIEW.md)** - 了解项目整体架构和技术栈
2. **[快速开始](./deployment/QUICK_START.md)** - 快速启动项目
3. **[开发规范](./development/DEVELOPMENT_GUIDELINES.md)** - 学习开发规范
4. **[环境配置](./deployment/DEVELOPMENT_SETUP.md)** - 配置开发环境

### 我要开发新功能

1. **[开发规范](./development/DEVELOPMENT_GUIDELINES.md)** - 开发规范参考
2. **[样式管理提醒](./development/STYLE_MANAGEMENT_REMINDER.md)** - 样式开发必读
3. **[功能实现状态](./features/IMPLEMENTATION_STATUS.md)** - 查看现有功能
4. **[模块系统](./architecture/README_MODULES.md)** - 模块化开发指南

### 我遇到了问题

1. **[故障排除文档](./troubleshooting/)** - 查看问题排查文档
2. **[API 错误修复](./troubleshooting/API_ERRORS_FIX.md)** - API 相关问题
3. **[文章系统问题](./troubleshooting/ARTICLES_TROUBLESHOOTING.md)** - 文章系统问题

### 我要部署项目

1. **[快速开始](./deployment/QUICK_START.md)** - 快速部署指南
2. **[后端启动](./deployment/START_BACKEND.md)** - 后端服务启动
3. **[环境检查清单](./deployment/ENVIRONMENT_CHECKLIST.md)** - 环境检查

---

## 📚 文档分类说明

### 💻 开发文档 (`development/`)

**用途**：开发相关的规范和指南

**核心文档**：
- `DEVELOPMENT_GUIDELINES.md` ⭐ - 开发规范（必读）
- `STYLE_MANAGEMENT_REMINDER.md` ⭐ - 样式管理提醒（必读）

**何时查看**：
- 开始开发新功能前
- 修改样式时
- 需要了解代码规范时

### 🏗️ 架构文档 (`architecture/`)

**用途**：系统架构和技术架构说明

**核心文档**：
- `README_MODULES.md` - 模块化系统使用指南
- `MODULE_SYSTEM.md` - 模块系统架构说明

**何时查看**：
- 需要了解系统架构时
- 开发新模块时
- 需要理解模块化系统时

### ⚙️ 配置文档 (`config/`)

**用途**：系统配置、API 配置、环境配置

**核心文档**：
- `API_CONFIG.md` - API 配置说明
- `README_NAIVE_UI.md` - Naive UI 集成指南
- `ENV_COMPATIBILITY.md` - 环境兼容性说明

**何时查看**：
- 配置 API 时
- 使用 UI 组件库时
- 配置环境时

### 🚀 部署文档 (`deployment/`)

**用途**：部署、运维相关文档

**核心文档**：
- `QUICK_START.md` ⭐ - 快速开始指南
- `DEVELOPMENT_SETUP.md` - 开发环境配置
- `START_BACKEND.md` - 后端启动指南

**何时查看**：
- 首次部署时
- 配置开发环境时
- 启动服务时

### 📋 功能开发文档 (`features/`)

**用途**：功能开发相关的文档

**核心文档**：
- `IMPLEMENTATION_STATUS.md` - 功能实现状态
- `OPTIMIZATION_PLAN.md` - 功能优化计划
- `document-agent-architecture.md` - AI 功能架构

**何时查看**：
- 开发新功能时
- 查看功能状态时
- 规划功能优化时

### 🔧 改进与优化文档 (`improvements/`)

**用途**：项目改进、优化相关文档

**核心文档**：
- `IMPROVEMENT_PLAN.md` - 改进计划
- `IMPROVEMENT_PROGRESS.md` - 改进进度

**何时查看**：
- 规划改进时
- 查看改进进度时

### 🐛 故障排除文档 (`troubleshooting/`)

**用途**：问题排查、Bug 修复相关文档

**核心文档**：
- `API_ERRORS_FIX.md` - API 错误修复
- `ARTICLES_TROUBLESHOOTING.md` - 文章系统问题

**何时查看**：
- 遇到问题时
- 排查 Bug 时

### 📊 代码质量文档 (`quality/`)

**用途**：代码质量、规范检查相关文档

**核心文档**：
- `CODE_STANDARDS_CHECK.md` - 代码规范检查报告
- `REFACTORING_PROGRESS.md` - 代码重构进度

**何时查看**：
- 查看代码质量时
- 了解重构进度时

---

## 🔍 如何查找文档

### 方法 1：使用文档索引

查看 [完整文档索引](./DOCUMENTATION_INDEX.md)，按分类或使用场景查找。

### 方法 2：使用文档目录

查看 [文档目录](./README.md)，按分类浏览。

### 方法 3：使用关键词搜索

在文档文件名中搜索关键词：
- 样式相关：`STYLE_*`
- 配置相关：`*_CONFIG.md`, `*_SETUP.md`
- 功能相关：`features/*`
- 问题相关：`troubleshooting/*`, `*_FIX.md`

---

## 📝 文档维护规范

### 何时更新文档

1. **开发新功能时**：更新功能实现状态文档
2. **修改代码规范时**：更新开发规范文档
3. **修复 Bug 时**：更新故障排除文档
4. **部署变更时**：更新部署文档

### 如何更新文档

1. 找到对应的文档文件
2. 更新相关内容
3. 更新文档的"最后更新"日期
4. 如有必要，更新文档索引

### 文档命名规范

- `README.md` - 目录索引或使用指南
- `*_GUIDELINES.md` - 开发指南
- `*_PLAN.md` - 计划文档
- `*_STATUS.md` - 状态文档
- `*_SUMMARY.md` - 总结文档
- `*_FIX.md` - 修复记录
- `*_PROGRESS.md` - 进度报告

---

## ⚠️ 重要提醒

1. **开发前必读**：
   - [开发规范](./development/DEVELOPMENT_GUIDELINES.md)
   - [样式管理提醒](./development/STYLE_MANAGEMENT_REMINDER.md)

2. **修改样式时**：
   - 必须先查看 [样式管理提醒](./development/STYLE_MANAGEMENT_REMINDER.md)
   - 禁止使用内联样式
   - 禁止使用 Tailwind 类

3. **提交代码前**：
   - 检查代码是否符合规范
   - 更新相关文档
   - 确保文档与代码同步

---

## 🔗 相关链接

- [完整文档索引](./DOCUMENTATION_INDEX.md)
- [文档目录](./README.md)
- [文档整理总结](./DOCUMENTATION_SUMMARY.md)
- [项目概览](./PROJECT_OVERVIEW.md)

---

**最后更新**：2024-12-03  
**维护者**：项目团队

