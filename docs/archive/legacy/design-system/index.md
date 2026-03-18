# Aurora Design System

> Aurora 设计系统 - 统一的设计语言和组件库

## 📚 文档导航

### 🎨 设计令牌 (Design Tokens)

| 文档 | 描述 |
|------|------|
| [颜色系统](./tokens/colors.md) | 色彩规范、主题色彩、状态颜色 |
| [排版系统](./tokens/typography.md) | 字体规范、字号、行高、字重 |
| [间距系统](./tokens/spacing.md) | 间距标尺、空间关系 |
| [圆角系统](./tokens/border-radius.md) | 圆角规范、组件圆角 |
| [阴影系统](./tokens/shadows.md) | 阴影层级、深度效果 |
| [动画系统](./tokens/animations.md) | 过渡动画、缓动函数 |

### 🧩 Admin Pattern 组件

| 组件 | 文档 | 示例 |
|------|------|------|
| [PageHeader](./patterns-admin/page-header.md) | 页面头部组件 | [查看](./examples-admin/page-header.vue) |
| [FilterBar](./patterns-admin/filter-bar.md) | 筛选栏组件 | [查看](./examples-admin/filter-bar.vue) |
| [ListPage](./patterns-admin/list-page.md) | 列表页面组件 | [查看](./examples-admin/list-page.vue) |
| [FormPage](./patterns-admin/form-page.md) | 表单页面组件 | [查看](./examples-admin/form-page.vue) |
| [DetailPage](./patterns-admin/detail-page.md) | 详情页面组件 | [查看](./examples-admin/detail-page.vue) |
| [ConfigPage](./patterns-admin/config-page.md) | 配置页面组件 | [查看](./examples-admin/config-page.vue) |

### 🌐 Web Pattern 组件

| 组件 | 文档 | 示例 |
|------|------|------|
| [HeroSection](./patterns-web/hero-section.md) | 英雄区块组件 | [查看](./examples-web/hero-section.vue) |
| [FeatureGrid](./patterns-web/feature-grid.md) | 功能网格组件 | [查看](./examples-web/feature-grid.vue) |
| [ProjectShowcase](./patterns-web/project-showcase.md) | 项目展示组件 | [查看](./examples-web/project-showcase.vue) |
| [CTASection](./patterns-web/cta-section.md) | CTA 区块组件 | [查看](./examples-web/cta-section.vue) |
| [ArticleLayout](./patterns-web/article-layout.md) | 文章布局组件 | [查看](./examples-web/article-layout.vue) |

### 🎯 使用指南

| 文档 | 描述 |
|------|------|
| [快速开始](./guides/getting-started.md) | 如何开始使用设计系统 |
| [主题系统](./guides/theme-system.md) | 主题切换与配置 |
| [开发规范](./guides/coding-standards.md) | 组件开发编码规范 |
| [响应式设计](./guides/responsive.md) | 断点与响应式最佳实践 |
| [无障碍性](./guides/accessibility.md) | A11y 标准与实现 |

### 📖 历史文档

| 文档 | 描述 |
|------|------|
| [Phase 1 完成报告](./phase1-completion-report.md) | 样式审计与架构设计 |
| [Phase 2 完成报告](./phase2-completion-report.md) | 语义变量迁移 |
| [Phase 3 完成报告](./phase3-completion-report.md) | Admin Pattern 实现 |
| [Phase 4 完成报告](./phase4-completion-report.md) | Pattern 扩展与迁移 |
| [Phase 5 完成报告](./phase5-completion-report.md) | Web Pattern 与主题配置 |

---

## 🎨 设计原则

### 1. 一致性 (Consistency)

- 使用设计令牌确保视觉一致性
- 组件间保持统一的行为和外观
- 遵循既定的交互模式

### 2. 可复用性 (Reusability)

- 组件应独立且可组合
- 通过 props 和 slots 提供灵活性
- 避免重复代码

### 3. 可访问性 (Accessibility)

- 支持键盘导航
- 提供适当的 ARIA 属性
- 确保颜色对比度符合 WCAG 标准

### 4. 响应式 (Responsive)

- 移动优先设计
- 流式布局适应不同屏幕
- 优化触摸目标大小

---

## 🚀 快速开始

```vue
<template>
  <!-- 使用 AppNaiveConfig 包裹应用 -->
  <AppNaiveConfig>
    <NuxtPage />
  </AppNaiveConfig>
</template>
```

```vue
<!-- 使用 Pattern 组件 -->
<template>
  <ListPage
    title="用户管理"
    :columns="tableColumns"
    :data="users"
    @row-click="handleRowClick"
  >
    <template #header-actions>
      <n-button type="primary" @click="handleAdd">
        新增用户
      </n-button>
    </template>
  </ListPage>
</template>
```

---

## 📦 版本信息

| 版本 | 日期 | 变更 |
|------|------|------|
| 3.0 | 2026-03 | Aurora Design System V3 - 完整的 Pattern 系统 |
| 2.0 | 2025-12 | 主题优化与视觉升级 |
| 1.0 | 2025-10 | 初始设计系统 |

---

## 🤝 贡献

- 遵循组件开发规范
- 添加必要的文档和示例
- 确保通过所有检查
- 提交前运行测试

---

## 📞 支持

- 查看 [FAQ](./faq.md)
- 报告问题请创建 Issue
- 提交 PR 贡献代码
