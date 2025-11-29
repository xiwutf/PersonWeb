# 样式重构进度文档

## 📋 概述

本项目正在进行样式统一管理重构，将所有在 template 中直接使用的 Tailwind 类提取到独立的 CSS 文件中，实现样式统一管理。

## ✅ 已完成

### 1. 样式文件创建
- ✅ `assets/css/about.css` - 关于页面样式
- ✅ `assets/css/tools.css` - 工具页面样式
- ✅ `assets/css/life.css` - 生活随笔页面样式
- ✅ `assets/css/blog.css` - 博客页面样式
- ✅ `assets/css/projects.css` - 项目页面样式

### 2. 页面重构
- ✅ `pages/about.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/tools/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/life/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/blog/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/projects/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类

### 3. 配置更新
- ✅ `nuxt.config.ts` - 已添加新的样式文件引用

## 🔄 进行中

### 待创建的样式文件
- ⏳ `assets/css/common.css` - 通用页面样式（用于其他页面）

### 待重构的页面
- ⏳ `pages/projects/detail-[slug].vue`
- ⏳ `pages/tools/marketplace.vue`
- ⏳ `pages/tools/collections.vue`
- ⏳ `pages/tools/my-tools.vue`
- ⏳ `pages/tools/detail-[slug].vue`
- ⏳ `pages/tools/[slug].vue`
- ⏳ `pages/index.vue` - 首页
- ⏳ `pages/search.vue` - 搜索页
- ⏳ `pages/skills/index.vue` - 技能页
- ⏳ `pages/knowledge/index.vue` - 知识库页
- ⏳ 其他页面...

## 📝 重构规范

### 样式文件命名
- 使用功能模块前缀，如 `about-*`、`tools-*`、`life-*`
- 文件名使用 kebab-case，如 `about.css`、`tools.css`

### CSS 类命名规范
- 使用功能前缀 + 元素类型 + 状态/变体
- 示例：
  - `.about-contact-card` - 关于页面-联系方式-卡片
  - `.tools-card-header` - 工具页面-卡片-头部
  - `.life-post-title` - 生活页面-文章-标题

### 重构步骤
1. 创建对应的 CSS 文件
2. 提取所有 Tailwind 类到 CSS 文件
3. 在 `nuxt.config.ts` 中引入样式文件
4. 重构 Vue 文件，替换 Tailwind 类为自定义 CSS 类
5. 测试页面显示效果

## 🎯 目标

- [x] 主要页面移除 template 中的 Tailwind 类
- [x] 所有样式统一在 CSS 文件中管理
- [x] 支持在后台管理系统中统一管理样式

## ✅ 样式配置系统

### 1. 数据库表
- ✅ `frontend_page_style` - 页面样式配置表
- ✅ `frontend_style_variable` - 样式变量表
- ✅ `frontend_style_rule` - 样式规则表

### 2. 后端 API
- ✅ `FrontendStyleController` - 前端样式配置 API 控制器
  - GET `/api/frontend-styles/page/{pageKey}` - 获取页面样式配置
  - PUT `/api/frontend-styles/page/{pageKey}` - 更新页面样式配置
  - GET `/api/frontend-styles/variables/{pageKey}` - 获取样式变量
  - PUT `/api/frontend-styles/variables/{pageKey}/{variableKey}` - 更新样式变量
  - GET `/api/frontend-styles/rules/{pageKey}` - 获取样式规则
  - PUT `/api/frontend-styles/rules/{pageKey}` - 更新样式规则
  - DELETE `/api/frontend-styles/rules/{id}` - 删除样式规则

### 3. 前端 Composable
- ✅ `composables/usePageStyle.ts` - 页面样式配置 composable
  - 动态加载样式配置
  - 应用 CSS 变量和规则
  - 支持样式更新

### 4. 后台管理页面
- ✅ `pages/admin/settings/frontend-styles.vue` - 前端页面样式管理页面
  - 基础样式配置（颜色、字体、间距等）
  - 样式变量管理
  - 自定义样式规则管理
  - 实时预览功能

## 📚 参考文档

- [开发规范文档](./DEVELOPMENT_GUIDELINES.md)
- [数据库设计原则](../../database/DESIGN_PRINCIPLES.md)

