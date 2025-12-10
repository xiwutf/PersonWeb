# 设计系统文档

本目录包含所有与设计系统相关的文档，包括设计规范、主题系统、视觉升级等。

## 📚 文档分类

### 🎨 核心设计系统文档

**必读文档：**

- **[DESIGN_SYSTEM_V1.md](./DESIGN_SYSTEM_V1.md)** ⭐ **必读** - 完整的设计系统文档
  - 主题与模式
  - 色彩系统
  - 组件规范
  - Tokens 规范
  - 开发约定

- **[CODING_STYLE_UI.md](./CODING_STYLE_UI.md)** ⭐ **必读** - UI 编码规范和最佳实践
  - 禁止硬编码颜色
  - 组件使用规范
  - 样式管理规范

- **[DESIGN_SYSTEM_V1_SUMMARY.md](./DESIGN_SYSTEM_V1_SUMMARY.md)** - 设计系统 v1 完成总结

### 🔄 主题系统演进

**按阶段整理的主题系统优化文档：**

1. **[THEME_REFACTORING_COMPLETE.md](./THEME_REFACTORING_COMPLETE.md)** - 第一阶段：主题重构完成
   - 统一主题逻辑
   - 修复深色模式问题
   - 减少自定义 CSS 变量

2. **[PHASE2_THEME_OPTIMIZATION.md](./PHASE2_THEME_OPTIMIZATION.md)** - 第二阶段：主题优化
   - 全局按钮优化
   - 卡片统一化
   - 后台 layout/menu/sider 统一重构
   - 输入类组件优化
   - 表格视觉统一
   - ECharts 深色主题适配

3. **[PHASE3_THEME_OPTIMIZATION.md](./PHASE3_THEME_OPTIMIZATION.md)** - 第三阶段：视觉统一 & tokens 精简
   - Card 统一化
   - 硬编码颜色统计与迁移
   - tokens.css 精简

4. **[PHASE4_DESIGN_SYSTEM_FINALIZATION.md](./PHASE4_DESIGN_SYSTEM_FINALIZATION.md)** - 第四阶段：规范固化 & 防回退
   - 新增颜色使用守卫
   - Lint / 检查规则
   - 组件封装
   - 文档引用

### 🎯 视觉升级文档

**具体的视觉升级实现文档：**

- **[VISION_PRO_GLASSMORPHISM_UPGRADE.md](./VISION_PRO_GLASSMORPHISM_UPGRADE.md)** - Vision Pro × 玻璃拟态风格升级
  - 深空渐变背景
  - 玻璃卡片效果
  - 高亮色 & 霓虹感
  - 交互动效

- **[NEON_GLASSMORPHISM_CHARTS_UPGRADE.md](./NEON_GLASSMORPHISM_CHARTS_UPGRADE.md)** - 图表 & 表格模块「霓虹渐变玻璃风」升级
  - 统一霓虹渐变色板
  - 渐变霓虹曲线
  - 霓虹渐变柱状图
  - 发光圆环
  - 深色玻璃面板表格

- **[ADMIN_DASHBOARD_VISUAL_UPGRADE.md](./ADMIN_DASHBOARD_VISUAL_UPGRADE.md)** - 后台仪表盘视觉升级
  - 顶部欢迎区重构
  - KPI 卡片视觉强化
  - Section Header 添加
  - 图表卡片增强
  - 热门页面视觉增强
  - 最近活动增强
  - 待办列表增强

- **[DASHBOARD_REFACTORING_COMPLETE.md](./DASHBOARD_REFACTORING_COMPLETE.md)** - 仪表盘重构完成报告

### 📊 辅助文档

- **[COLOR_STATISTICS.md](./COLOR_STATISTICS.md)** - 颜色统计和迁移指南
  - 硬编码颜色统计
  - 颜色角色映射表
  - 迁移计划

- **[THEME_REFACTOR_SUMMARY.md](./THEME_REFACTOR_SUMMARY.md)** - 主题重构总结

- **[THEME_SYSTEM_ANALYSIS.md](./THEME_SYSTEM_ANALYSIS.md)** - 主题系统分析

## 📖 阅读顺序建议

### 新开发者

1. **[DESIGN_SYSTEM_V1.md](./DESIGN_SYSTEM_V1.md)** - 了解完整的设计系统
2. **[CODING_STYLE_UI.md](./CODING_STYLE_UI.md)** - 学习 UI 编码规范
3. **[DESIGN_SYSTEM_V1_SUMMARY.md](./DESIGN_SYSTEM_V1_SUMMARY.md)** - 快速了解设计系统概览

### 了解演进历史

1. **[THEME_REFACTORING_COMPLETE.md](./THEME_REFACTORING_COMPLETE.md)** - 第一阶段
2. **[PHASE2_THEME_OPTIMIZATION.md](./PHASE2_THEME_OPTIMIZATION.md)** - 第二阶段
3. **[PHASE3_THEME_OPTIMIZATION.md](./PHASE3_THEME_OPTIMIZATION.md)** - 第三阶段
4. **[PHASE4_DESIGN_SYSTEM_FINALIZATION.md](./PHASE4_DESIGN_SYSTEM_FINALIZATION.md)** - 第四阶段

### 了解视觉升级

1. **[VISION_PRO_GLASSMORPHISM_UPGRADE.md](./VISION_PRO_GLASSMORPHISM_UPGRADE.md)** - Vision Pro 风格
2. **[NEON_GLASSMORPHISM_CHARTS_UPGRADE.md](./NEON_GLASSMORPHISM_CHARTS_UPGRADE.md)** - 霓虹渐变图表
3. **[ADMIN_DASHBOARD_VISUAL_UPGRADE.md](./ADMIN_DASHBOARD_VISUAL_UPGRADE.md)** - 仪表盘升级

## 🔗 相关文档

- [开发规范](../development/DEVELOPMENT_GUIDELINES.md) - 项目开发规范
- [样式架构](../development/STYLE_ARCHITECTURE.md) - 样式架构规范
- [Naive UI 使用](../config/README_NAIVE_UI.md) - Naive UI 集成指南

