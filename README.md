# 溪午听风 个人网站

一个现代化、响应式的个人开发者网站，基于 Nuxt 3 构建。

## ✨ 特性

### 🎨 现代化设计
- **统一品牌风格**：一致的颜色搭配和视觉元素
- **响应式布局**：完美适配桌面端、平板和移动设备
- **流畅动画**：页面淡入、悬停效果、浮动动画
- **渐变背景**：每个页面都有独特的渐变背景色

### 🏗️ 技术架构
- **Nuxt 3**：Vue.js 的全栈框架
- **Tailwind CSS**：原子化CSS框架
- **@nuxt/content**：基于Markdown的内容管理
- **TypeScript**：类型安全的JavaScript

### 📱 页面结构
- **首页**：个人介绍、快速导航、最新内容展示
- **插件工具**：Revit插件展示和销售
- **项目展示**：个人项目作品集
- **技术博客**：技术文章和经验分享
- **关于我**：个人信息和联系方式

### 🎯 核心功能

#### 内容管理系统
- **Markdown驱动**：所有内容通过Markdown文件管理
- **自动化更新**：添加新的Markdown文件即可自动生成页面
- **SEO优化**：自动生成页面标题、描述和关键词

#### 插件工具模块
- 工具展示卡片
- 价格和购买链接
- 标签分类
- 统计信息展示

#### 项目展示模块
- 项目分类筛选
- 技术栈展示
- 在线演示和源码链接
- 项目状态标识

#### 博客系统
- 文章分类和标签
- 时间归档
- 作者信息
- 阅读链接

## 🚀 快速开始

### 环境要求
- Node.js 18+
- npm 或 yarn

### 安装依赖
```bash
npm install
```

### 开发模式
```bash
npm run dev
```

访问 http://localhost:3000 查看网站

### 构建生产版本
```bash
npm run build
```

### 预览生产版本
```bash
npm run preview
```

## 📝 内容管理

### 添加新工具
在 `content/tools/` 目录下创建新的 `.md` 文件：

```markdown
---
title: "工具名称"
description: "工具描述"
price: 19.9
tags: ["Revit", "插件"]
buy_link: "https://example.com"
slug: "tool-name"
date: "2024-01-01"
---

# 工具详细介绍

这里是工具的详细说明...
```

### 添加新项目
在 `content/projects/` 目录下创建新的 `.md` 文件：

```markdown
---
title: "项目名称"
tech: ["Vue.js", "Nuxt 3"]
description: "项目描述"
demo_link: "https://demo.com"
source_link: "https://github.com/..."
slug: "project-name"
date: "2024-01-01"
status: "已上线"
category: "Web应用"
---

# 项目详细介绍

这里是项目的详细说明...
```

### 添加新博客文章
在 `content/blog/` 目录下创建新的 `.md` 文件：

```markdown
---
title: "文章标题"
date: "2024-01-01"
tags: ["Vue.js", "教程"]
description: "文章描述"
author: "溪午听风"
category: "技术文章"
---

# 文章内容

这里是文章的正文内容...
```

## 🎨 自定义样式

### 主题色彩
网站使用了以下主题色彩：
- **首页**：蓝色到紫色渐变
- **工具页**：橙色到红色渐变  
- **项目页**：紫色到粉色渐变
- **博客页**：绿色到蓝色渐变

### 自定义组件
主要组件位于 `components/` 目录：
- `Header.vue`：头部导航栏
- `Footer.vue`：页脚信息

### 全局样式
全局样式定义在 `assets/css/main.css`，包含：
- 基础样式重置
- 组件样式类
- 动画效果
- 响应式工具类

## 📱 响应式设计

网站在以下设备上完美显示：
- **桌面端**：1280px+
- **平板**：768px - 1024px
- **手机**：320px - 768px

## 🔧 配置文件

### Nuxt 配置 (`nuxt.config.ts`)
```typescript
export default defineNuxtConfig({
  modules: [
    '@nuxt/content',
    '@nuxtjs/tailwindcss'
  ],
  content: {
    highlight: {
      theme: 'github-light'
    }
  }
})
```

## 📊 性能优化

- **代码分割**：自动按页面分割代码
- **图片优化**：自动优化图片大小和格式
- **CSS优化**：自动移除未使用的CSS
- **预渲染**：静态生成提升首屏加载速度

## 🚀 部署

### Vercel 部署
1. 连接GitHub仓库
2. 自动检测Nuxt项目
3. 一键部署

### Netlify 部署
1. 构建命令：`npm run build`
2. 发布目录：`.output/public`

### 服务器部署
```bash
npm run build
npm run preview
```

## 📧 联系方式

- **邮箱**：255106739@qq.com
- **微信**：LinXi-5152
- **GitHub**：https://github.com/Lijing327

## 📄 许可证

MIT License - 详见 [LICENSE](LICENSE) 文件

---

**让代码改变世界，让技术创造价值！** 🚀 