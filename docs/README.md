# 项目文档目录

本文档目录用于分类整理项目中的所有 Markdown 文档。

## ⭐ 必读文档

**在开始开发前，请务必阅读：**

- **[开发规范文档](./development/DEVELOPMENT_GUIDELINES.md)** ⭐ **必读** - 项目开发规范和要求
  - 样式管理规范（禁止内联样式）
  - 代码组织规范
  - 命名规范
  - 组件开发规范
  - API 开发规范
  - 数据库规范
  - Git 提交规范
  - 最佳实践和禁止事项

## 📁 文档分类

### 💻 开发文档 (`development/`)
开发相关的文档，包括开发规范、构建优化等。

- `DEVELOPMENT_GUIDELINES.md` ⭐ **必读** - 项目开发规范和要求
- `BUILD_OPTIMIZATION.md` - 构建优化说明

### 🏗️ 架构文档 (`architecture/`)
系统架构、技术架构相关文档。

- `MODULE_SYSTEM.md` - 模块系统架构说明
- `README_MODULES.md` - 模块化系统使用指南

### ⚙️ 配置文档 (`config/`)
系统配置、API 配置、环境配置等相关文档。

- `API_CONFIG.md` - API 配置说明
- `ENV_COMPATIBILITY.md` - 环境兼容性说明
- `NAIVE_UI_USAGE.md` - Naive UI 使用说明
- `README_NAIVE_UI.md` - Naive UI 集成指南
- `UI_COMPONENT_LIBRARY.md` - UI 组件库配置

### 🚀 部署文档 (`deployment/`)
部署、运维相关的文档。

- `DEVELOPMENT_SETUP.md` - 开发环境配置指南
- `ENVIRONMENT_CHECKLIST.md` - 环境检查清单
- `MOBILE_ACCESS.md` - 移动端访问指南
- `QUICK_START.md` - 快速开始指南
- `SETUP_SUMMARY.md` - 配置总结
- `START_BACKEND.md` - 后端启动指南

### 📋 功能开发文档 (`features/`)
功能开发相关的文档，包括功能状态、开发日志、优化计划等。

- `CREATIVE_FEATURES.md` - 创意功能建议
- `IMPLEMENTATION_STATUS.md` - 功能实现状态
- `MOBILE_BEST_PRACTICES.md` - 移动端最佳实践
- `MOBILE_OPTIMIZATION.md` - 移动端优化指南
- `OPTIMIZATION_PLAN.md` - 功能优化计划
- `TASK_SYSTEM.md` - 任务系统说明
- `TODO_FEATURES.md` - 待开发功能列表
- `V2.0_DEVELOPMENT_PLAN.md` - V2.0 开发计划

### 🔧 改进与优化文档 (`improvements/`)
项目改进、优化相关的文档。

- `IMPROVEMENT_PLAN.md` - 改进计划
- `IMPROVEMENT_PROGRESS.md` - 改进进度
- `IMPROVEMENT_SUMMARY.md` - 改进总结
- `MARKDOWN_EDITOR_UPGRADE.md` - Markdown 编辑器升级文档

### 🐛 故障排除文档 (`troubleshooting/`)
问题排查、Bug 修复相关的文档。

- `API_ERRORS_FIX.md` - API 错误修复记录
- `ARTICLES_TROUBLESHOOTING.md` - 文章系统故障排除
- `GIT_FIX.md` - Git 问题修复记录
- `TOOLS_API_FIX.md` - 工具 API 修复记录

## 📌 使用说明

1. **查找文档**：根据类别在对应目录下查找
2. **更新文档**：请在对应分类目录下更新
3. **新增文档**：根据内容选择合适的分类目录

## 🔄 文档维护

- 定期检查文档的时效性
- 删除过时或重复的文档
- 保持文档结构清晰

## 📚 快速导航

### 开发相关
- [开发规范](./development/DEVELOPMENT_GUIDELINES.md) ⭐ **必读**
- [模块系统](./architecture/README_MODULES.md)
- [功能开发状态](./features/IMPLEMENTATION_STATUS.md)
- [功能优化计划](./features/OPTIMIZATION_PLAN.md)
- [改进计划](./improvements/IMPROVEMENT_PLAN.md)

### 配置相关
- [API 配置](./config/API_CONFIG.md)
- [环境配置](./config/ENV_COMPATIBILITY.md)
- [Naive UI 使用](./config/README_NAIVE_UI.md)

### 部署相关
- [快速开始](./deployment/QUICK_START.md)
- [开发环境配置](./deployment/DEVELOPMENT_SETUP.md)
- [后端启动](./deployment/START_BACKEND.md)

### 问题排查
- [API 错误修复](./troubleshooting/API_ERRORS_FIX.md)
- [文章系统问题](./troubleshooting/ARTICLES_TROUBLESHOOTING.md)

## 📝 文档结构说明

```
docs/
├── development/          # 开发文档（开发规范、构建优化）
├── architecture/         # 架构文档（系统架构、模块系统）
├── config/               # 配置文档（API、环境、UI组件库）
├── deployment/           # 部署文档（环境配置、启动指南）
├── features/             # 功能开发文档（功能状态、开发计划）
├── improvements/         # 改进文档（改进计划、优化记录）
└── troubleshooting/      # 故障排除文档（问题排查、Bug修复）
```
