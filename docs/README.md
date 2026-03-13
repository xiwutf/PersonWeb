# 项目文档目录

本文档目录用于分类整理项目中的所有 Markdown 文档。

## ⭐ 必读文档

**在开始开发前，请务必阅读：**

- **[项目概览文档](./PROJECT_OVERVIEW.md)** ⭐ **必读** - 项目整体架构、技术栈和目录结构
- **[项目结构指南](./PROJECT_STRUCTURE_GUIDE.md)** ⭐ **必读** - 快速定位项目中的功能和内容编辑位置
- **[开发规范文档](./development/DEVELOPMENT_GUIDELINES.md)** ⭐ **必读** - 项目开发规范和要求
  - 样式管理规范（禁止内联样式）
  - 代码组织规范
  - 命名规范
  - 组件开发规范
  - API 开发规范
  - 数据库规范
  - Git 提交规范
  - 最佳实践和禁止事项
- **[设计系统 v1](./design-system/DESIGN_SYSTEM_V1.md)** ⭐ **必读** - 完整的设计系统文档（主题、色彩、组件规范）
- **[UI 编码规范](./design-system/CODING_STYLE_UI.md)** ⭐ **必读** - UI 开发编码规范和最佳实践
- **[代码规范检查报告](./quality/CODE_STANDARDS_CHECK.md)** - 代码规范符合度检查结果

## 📁 文档分类

### 🌐 开源与社区 (`open-source/`)

- `OPEN_SOURCE.md` - 开源项目声明与许可证说明
- `OPENSOURCE_CHECKLIST.md` - 开源化快速完成清单
- `OPENSOURCE_SETUP.md` - 开源项目配置指南

### 🚀 部署 (`deployment/`)

- `GitHub-Secrets-Setup-Guide.md` - GitHub Secrets 与阿里云 OSS 部署配置
- 其他部署、环境、Nginx 等文档见本目录

### 🗺️ 路线图与规划 (`roadmap/`、`planning/`)

- `roadmap/ROADMAP_MODULE_SYSTEM.md` - 模块化系统发展路线图
- `planning/AI咨询收集.md` - 情报中心/Intelligence 模块需求与执行清单

### 📖 文档管理 (`documentation/`)

- `DOCUMENTATION_INDEX.md` ⭐ - 完整文档索引（按分类和使用场景）
- `DOCUMENTATION_GUIDE.md` ⭐ - 文档使用指南（如何查找和使用文档）
- `DOCUMENTATION_ORGANIZATION.md` - 文档组织说明
- `DOCUMENTATION_SUMMARY.md` - 文档整理总结
- `DOCUMENTATION_STATUS.md` - 文档整理状态报告
- `DOCUMENTATION_CLEANUP.md` - 文档整理清理报告
- `DOCUMENTATION_CLEANUP_SUMMARY.md` - 文档整理完成总结
- `MODULE_DOCUMENTATION_INDEX.md` - 模块化专题文档索引
- `整理说明.md` - 文档整理说明

### 🎨 设计系统文档 (`design-system/`)

**UI 开发前必读：**

- `design-system/DESIGN_SYSTEM_V1.md` ⭐ **必读** - 完整的设计系统文档（主题、色彩、组件规范）
- `design-system/DESIGN_SYSTEM_V2.md` - 设计系统 v2 文档
- `design-system/CODING_STYLE_UI.md` ⭐ **必读** - UI 编码规范和最佳实践
- `design-system/DESIGN_SYSTEM_V1_SUMMARY.md` - 设计系统 v1 完成总结

**主题系统演进：**

- `design-system/THEME_REFACTORING_COMPLETE.md` - 第一阶段：主题重构完成
- `design-system/PHASE2_THEME_OPTIMIZATION.md` - 第二阶段：主题优化
- `design-system/PHASE3_THEME_OPTIMIZATION.md` - 第三阶段：视觉统一 & tokens 精简
- `design-system/PHASE4_DESIGN_SYSTEM_FINALIZATION.md` - 第四阶段：规范固化 & 防回退

**视觉升级：**

- `design-system/VISION_PRO_GLASSMORPHISM_UPGRADE.md` - Vision Pro × 玻璃拟态风格升级
- `design-system/NEON_GLASSMORPHISM_CHARTS_UPGRADE.md` - 图表 & 表格模块「霓虹渐变玻璃风」升级
- `design-system/ADMIN_DASHBOARD_VISUAL_UPGRADE.md` - 后台仪表盘视觉升级
- `design-system/DASHBOARD_REFACTORING_COMPLETE.md` - 仪表盘重构完成报告

**辅助文档：**

- `design-system/COLOR_STATISTICS.md` - 颜色统计和迁移指南
- `design-system/THEME_REFACTOR_SUMMARY.md` - 主题重构总结
- `design-system/THEME_SYSTEM_ANALYSIS.md` - 主题系统分析

详见：[设计系统文档目录](./design-system/README.md)

### 💻 开发文档 (`development/`)

开发相关的文档，包括开发规范、构建优化等。

- `DEVELOPMENT_GUIDELINES.md` ⭐ **必读** - 项目开发规范和要求
- `STYLE_ARCHITECTURE.md` ⭐ **必读** - 样式架构规范（分层设计、CSS 变量、Naive UI 集成）
- `STYLE_MANAGEMENT_REMINDER.md` ⭐ **必读** - 样式管理开发提醒
- `STYLE_REMINDER.md` - 样式管理快速提醒
- `FRONTEND_STYLE_SYSTEM.md` - 前端样式系统说明
- `STYLE_REFACTORING_PROGRESS.md` - 样式重构进度
- `STYLE_ARCHITECTURE_REFACTOR.md` - 样式架构重构
- `BUILD_OPTIMIZATION.md` - 构建优化说明
- `UI_COMPONENT_LIBRARY_RECOMMENDATIONS.md` - UI 组件库推荐
- `CODE_REVIEW_REPORT.md` - 代码审查报告
- `DATABASE_SCRIPTS_REVIEW.md` - 数据库脚本审查

#### 📦 模块开发文档
- `MODULE_DEVELOPMENT_GUIDE.md` ⭐ **必读** - 模块开发指南（架构设计、API规范、编码规范）
- `MODULE_BEST_PRACTICES.md` ⭐ **必读** - 模块开发最佳实践（目录结构、命名规范、错误处理）

### 🏗️ 架构文档 (`architecture/`)

系统架构、技术架构相关文档。

- `MODULE_SYSTEM.md` - 模块系统架构说明
- `README_MODULES.md` - 模块化系统使用指南
- `ai-agent-system.md` - AI 代理系统架构

### 🤖 AI 代理文档 (`ai-agents/`)

AI 功能和文档处理相关文档。

- `README.md` - AI 代理系统概述
- `QUICK_START.md` - 快速开始指南
- `DEPLOYMENT_CHECKLIST.md` - 部署检查清单
- `IMPLEMENTATION_SUMMARY.md` - 实现总结
- `document-agent-architecture.md` - 文档代理架构
- `document-agent-implementation-summary.md` - 文档代理实现总结
- `COGNITION_DOC_SYSTEM.md` - 认知文档系统
- `AI_ASSISTANTS_DIFFERENCE.md` - AI 助手差异说明

### ⚙️ 配置文档 (`config/`)

系统配置、API 配置、环境配置等相关文档。

- `API_CONFIG.md` - API 配置说明
- `ENV_COMPATIBILITY.md` - 环境兼容性说明
- `NAIVE_UI_USAGE.md` - Naive UI 使用说明
- `README_NAIVE_UI.md` - Naive UI 集成指南
- `UI_COMPONENT_LIBRARY.md` - UI 组件库配置

### 🚀 部署文档 (`deployment/`)

部署、运维相关的文档。

- `QUICK_START.md` ⭐ **必读** - 快速开始指南（包含环境配置和服务启动）
- `DEVELOPMENT_SETUP.md` - 开发环境配置指南
- `ENVIRONMENT_CHECKLIST.md` - 环境检查清单
- `MOBILE_ACCESS.md` - 移动端访问指南
- `SETUP_SUMMARY.md` - 配置总结
- `START_BACKEND.md` - 后端启动指南
- `content-update-guide.md` - 内容更新指南
- `BACKEND_PRODUCTION_CONFIG.md` - 后端生产环境配置
- `NGINX_CONFIG_FIX.md` - Nginx 配置修复
- `NGINX_IP_FORWARDING.md` - Nginx IP 转发配置
- `OPEN_SOURCE_SECURITY.md` - 开源安全配置

### 🔌 API 文档 (`api/`)

API 接口和调用相关文档。

- `MODULE_SYSTEM_API.md` - 模块系统 API 参考
- `module-system-api.md` - 模块系统 API 说明
- `ECOMMERCE_MODULE_API.md` - 电商模块 API 参考

### 🧩 模块文档 (`modules/`)

各功能模块的文档。

#### 核心模块文档
- `module-storage-versioning.md` - 模块存储版本控制

#### 关系跟进模块 (`modules/relations/`)
- `relations_module_features.md` - 关系跟进模块功能描述
- `relations_module_observation_period.md` - 观察期管理
- `relations_heat_score_calculation.md` - 热度评分计算
- `relations_module_phase0_analysis.md` - 阶段 0：需求分析
- `relations_module_phase1_completion.md` - 阶段 1：基础功能完成
- `relations_module_phase2_completion.md` - 阶段 2：完成
- `relations_module_phase3_completion.md` - 阶段 3：完成
- `relations_module_phase4_completion.md` - 阶段 4：完成
- `relations_module_phase5_completion.md` - 阶段 5：完成
- `relations_module_phase6_completion.md` - 阶段 6：完成
- `relations_module_phase6_independence_analysis.md` - 阶段 6：独立性分析

#### 资产管理模块 (`modules/asset/`)
- `ASSET_MANAGEMENT_COMPLETE.md` - 资产管理完成文档
- `ASSET_MANAGEMENT_EXTENSION.md` - 资产管理扩展

#### 投资模块 (`modules/investment/`)
- `INVESTMENT_MODULE.md` - 投资模块文档
- `INVESTMENT_DATA_SOURCE.md` - 投资数据来源

#### 副业项目模块 (`modules/side-projects/`)
- `SIDE_PROJECT_ANALYTICS.md` - 副业项目分析
- `SIDE_PROJECT_ENHANCEMENT.md` - 副业项目增强
- `SIDE_PROJECT_NOTIFICATIONS.md` - 副业项目通知

#### 工具模块 (`modules/tools/`)
- `name-tool-v2.md` - 取名工具 v2

#### 支付许可证模块 (`modules/payment/`)
- `PAYMENT_LICENSE_ANALYSIS.md` - 支付许可证分析

### 📋 功能开发文档 (`features/`)

功能开发相关的文档，包括功能状态、开发日志、优化计划等。

- `IMPLEMENTATION_STATUS.md` - 功能实现状态
- `OPTIMIZATION_PLAN.md` - 功能优化计划
- `TASK_SYSTEM.md` - 任务系统说明
- `TODO_FEATURES.md` - 待开发功能列表
- `V2.0_DEVELOPMENT_PLAN.md` - V2.0 开发计划
- `CREATIVE_FEATURES.md` - 创意功能建议
- `FUTURISTIC_FEATURES.md` - 未来功能构想
- `PENDING_TASKS.md` - 待处理任务
- `MOBILE_BEST_PRACTICES.md` - 移动端最佳实践
- `MOBILE_OPTIMIZATION.md` - 移动端优化指南
- `COMMERCIAL_FEATURES.md` - 商业功能
- `COMMERCIAL_IMPLEMENTATION.md` - 商业功能实现
- `HOMEPAGE_HYBRID_SUPER_REDESIGN.md` - 首页混合重设计
- `task-homepage-redesign.md` - 首页重设计任务
- `EASTMONEY_API_GUIDE.md` - 东方财富 API 指南
- `VISITOR_DATA_COLLECTION.md` - 访客数据收集
- `visitor-analytics-progress.md` - 访客分析页面进度报告

### 🔧 改进与优化文档 (`improvements/`)

项目改进、优化相关的文档。

- `IMPROVEMENT_PLAN.md` - 改进计划
- `IMPROVEMENT_PROGRESS.md` - 改进进度
- `IMPROVEMENT_SUMMARY.md` - 改进总结
- `THEME_REFACTORING_SUMMARY.md` - 主题重构总结
- `MARKDOWN_EDITOR_UPGRADE.md` - Markdown 编辑器升级文档
- `BACKEND_FRONTEND_SUPPORT_SUMMARY.md` - 后端前端支持总结
- `DEPENDENCY_OPTIMIZATION.md` - 依赖优化
- `DEPENDENCY_OPTIMIZATION_PLAN.md` - 依赖优化计划
- `PERFORMANCE_OPTIMIZATION_SUMMARY.md` - 性能优化总结

### 🐛 故障排除文档 (`troubleshooting/`)

问题排查、Bug 修复相关的文档。

- `API_ERRORS_FIX.md` - API 错误修复记录
- `ARTICLES_TROUBLESHOOTING.md` - 文章系统故障排除
- `GIT_FIX.md` - Git 问题修复记录
- `TOOLS_API_FIX.md` - 工具 API 修复记录
- `COMPONENTS_AFFECTING_SCROLLBAR_AND_HEADER.md` - 滚动条和头部相关组件
- `MODULE_TROUBLESHOOTING.md` - 模块系统排查指南

### 📊 代码质量文档 (`quality/`)

代码质量、规范检查相关的文档。

- `CODE_STANDARDS_CHECK.md` - 代码规范检查报告
- `PROJECT_STANDARDS_CHECK_REPORT.md` - 项目标准检查报告
- `REFACTORING_PROGRESS.md` - 代码重构进度报告
- `COMPONENT_ORGANIZATION_CHECK.md` - 组件组织检查
- `COMPONENT_REORGANIZATION_SUMMARY.md` - 组件重组总结
- `COMPOSABLES_CHECK.md` - 组合式函数检查

### 🔧 修复文档 (`fixes/`)

数据修复、问题修复相关文档。

- `DATA_DUPLICATE_CHECK.md` - 数据重复检查
- `PROJECT_DUPLICATE_FIX.md` - 项目重复修复
- `ANALYTICS_DATA_FIX.md` - 分析数据修复
- `VISITOR_ANALYTICS_IP_FIX.md` - 访客分析 IP 修复

### 🔄 迁移文档 (`migration/`)

系统迁移相关文档。

- `AICONTROLLER_MIGRATION.md` - AI 控制器迁移

### 📦 归档文档 (`archive/`)

归档的旧文档。

- `PROJECT_REORGANIZATION_CHECKLIST.md` - 项目重组检查清单
- `PROJECT_REORGANIZATION_COMPLETE.md` - 项目重组完成
- `PROJECT_REORGANIZATION_PROGRESS.md` - 项目重组进度

## 📌 使用说明

1. **查找文档**：
   - 查看 [完整文档索引](./documentation/DOCUMENTATION_INDEX.md) 按分类或使用场景查找
   - 查看 [文档使用指南](./documentation/DOCUMENTATION_GUIDE.md) 了解如何使用文档
   - 根据类别在对应目录下查找
2. **更新文档**：请在对应分类目录下更新
3. **新增文档**：根据内容选择合适的分类目录

## 🔄 文档维护

- 定期检查文档的时效性
- 删除过时或重复的文档
- 保持文档结构清晰

## 📚 快速导航

### 开发相关
- [项目概览](./PROJECT_OVERVIEW.md) ⭐ **必读**
- [项目结构指南](./PROJECT_STRUCTURE_GUIDE.md) ⭐ **必读**
- [开发规范](./development/DEVELOPMENT_GUIDELINES.md) ⭐ **必读**
- [设计系统 v1](./design-system/DESIGN_SYSTEM_V1.md) ⭐ **必读**
- [UI 编码规范](./design-system/CODING_STYLE_UI.md) ⭐ **必读**
- [代码规范检查](./quality/CODE_STANDARDS_CHECK.md)
- [模块系统](./architecture/README_MODULES.md)
- [功能开发状态](./features/IMPLEMENTATION_STATUS.md)
- [功能优化计划](./features/OPTIMIZATION_PLAN.md)
- [改进计划](./improvements/IMPROVEMENT_PLAN.md)

### 模块相关
- [模块开发指南](./development/MODULE_DEVELOPMENT_GUIDE.md) ⭐ **必读**
- [模块开发最佳实践](./development/MODULE_BEST_PRACTICES.md) ⭐ **必读**
- [模块文档索引](./documentation/MODULE_DOCUMENTATION_INDEX.md)
- [模块系统 API](./api/MODULE_SYSTEM_API.md)
- [关系跟进模块](./modules/relations/)
- [资产管理模块](./modules/asset/)
- [投资模块](./modules/investment/)

### AI 相关
- [AI 代理快速开始](./ai-agents/QUICK_START.md)
- [AI 代理部署检查清单](./ai-agents/DEPLOYMENT_CHECKLIST.md)
- [文档代理架构](./ai-agents/document-agent-architecture.md)

### 配置相关
- [API 配置](./config/API_CONFIG.md)
- [环境配置](./config/ENV_COMPATIBILITY.md)
- [Naive UI 使用](./config/README_NAIVE_UI.md)

### 部署相关
- [快速开始](./deployment/QUICK_START.md)
- [开发环境配置](./deployment/DEVELOPMENT_SETUP.md)
- [后端启动](./deployment/START_BACKEND.md)
- [后端生产环境配置](./deployment/BACKEND_PRODUCTION_CONFIG.md)

### 问题排查
- [API 错误修复](./troubleshooting/API_ERRORS_FIX.md)
- [文章系统问题](./troubleshooting/ARTICLES_TROUBLESHOOTING.md)
- [模块系统排查](./troubleshooting/MODULE_TROUBLESHOOTING.md)

## 📝 文档结构说明

```
docs/
├── PROJECT_OVERVIEW.md           # 项目概览（架构、技术栈、目录结构）⭐
├── PROJECT_STRUCTURE_GUIDE.md      # 项目结构快速定位指南 ⭐
├── README.md                     # 文档主索引（本文件）
├── documentation/                # 文档管理和索引
├── design-system/                # 设计系统文档（设计规范、主题系统、视觉升级）
├── development/                  # 开发文档（开发规范、构建优化）
├── architecture/                 # 架构文档（系统架构、模块系统）
├── ai-agents/                   # AI 代理文档
├── config/                       # 配置文档（API、环境、UI组件库）
├── deployment/                   # 部署文档（环境配置、启动指南）
├── api/                          # API 文档
├── modules/                      # 各功能模块文档
│   ├── relations/                # 关系跟进模块
│   ├── asset/                    # 资产管理模块
│   ├── investment/               # 投资模块
│   ├── side-projects/            # 副业项目模块
│   ├── tools/                    # 工具模块
│   └── payment/                  # 支付模块
├── features/                     # 功能开发文档（功能状态、开发计划）
├── improvements/                 # 改进文档（改进计划、优化记录）
├── quality/                      # 代码质量文档（规范检查、质量报告）
├── fixes/                        # 修复文档（数据修复、问题修复）
├── troubleshooting/              # 故障排除文档（问题排查、Bug修复）
├── migration/                    # 迁移文档
└── archive/                      # 归档文档
```
