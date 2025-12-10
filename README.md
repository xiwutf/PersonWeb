# 溪午听风 个人网站

一个现代化、响应式的个人开发者网站，基于 Nuxt 3 构建。

## ✨ 特性

### 🎨 现代化设计
- **统一品牌风格**：一致的颜色搭配和视觉元素
- **设计系统 v1**：完整的设计系统，统一管理主题、色彩、组件规范
- **响应式布局**：完美适配桌面端、平板和移动设备
- **流畅动画**：页面淡入、悬停效果、浮动动画
- **渐变背景**：每个页面都有独特的渐变背景色
- **深色/浅色模式**：支持主题切换，所有组件自动适配

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
- **Node.js** 18+ 
- **.NET SDK** 8.0+
- **MySQL** 5.7+ 或 8.0+
- **Git**

### 一键配置（推荐）

**Windows:**
```powershell
.\scripts\setup-dev-env.ps1
```

**Linux/macOS:**
```bash
chmod +x scripts/setup-dev-env.sh
./scripts/setup-dev-env.sh
```

### 手动配置

1. **安装依赖**
   ```bash
   # 前端
   npm install
   
   # 后端
   cd backend/PersonalSite.Api
   dotnet restore
   ```

2. **配置环境变量**
   ```bash
   # 复制模板文件
   cp .env.example .env
   # 编辑 .env 文件，设置 API 地址
   ```

3. **配置数据库**
   - 编辑 `backend/PersonalSite.Api/appsettings.Development.json`
   - 设置数据库连接字符串

4. **创建数据库并执行脚本**
   ```sql
   CREATE DATABASE personal_site CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```
   ```bash
   mysql -u root -p personal_site < database/all_tables.sql
   ```

5. **启动服务**
   ```bash
   # 终端1 - 后端
   cd backend/PersonalSite.Api
   dotnet run
   
   # 终端2 - 前端
   npm run dev
   ```

6. **访问应用**
   - 前端：http://localhost:3000
   - API 文档：http://localhost:5234/swagger

### 📱 移动端访问

如果想在手机上测试：

```bash
# 启动移动端开发服务器
npm run dev:mobile

# 或使用脚本（会自动显示 IP 地址）
.\scripts\dev-mobile.ps1  # Windows
./scripts/dev-mobile.sh   # Linux/macOS
```

然后在手机浏览器访问：`http://你的电脑IP:3000`

详细说明请查看 [移动端访问指南](./docs/deployment/MOBILE_ACCESS.md)

### 📚 详细文档

- [快速开始指南](./docs/deployment/QUICK_START.md)
- [开发环境配置指南](./docs/deployment/DEVELOPMENT_SETUP.md)
- [后端启动指南](./docs/deployment/START_BACKEND.md)

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
- **桌面端**：1024px+（完整功能，3D 效果）
- **平板**：768px - 1024px（响应式布局，简化 3D）
- **手机**：320px - 768px（移动优化，禁用 3D，触摸友好）

### 📱 移动端优化
- ✅ 响应式导航菜单
- ✅ 触摸友好的按钮（最小 44x44px）
- ✅ 移动端自动禁用 3D 场景（性能优化）
- ✅ 自适应字体大小
- ✅ 优化的滚动性能
- 详细优化说明请查看 [移动端优化指南](./docs/features/MOBILE_OPTIMIZATION.md)

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

## 📚 项目文档

### ⭐ 必读文档

**在开始开发前，请务必阅读以下文档：**

- **[项目概览文档](./docs/PROJECT_OVERVIEW.md)** ⭐ **必读** - 项目整体架构、技术栈和目录结构
- **[开发规范文档](./docs/development/DEVELOPMENT_GUIDELINES.md)** ⭐ **必读** - 项目开发规范和要求，包括样式管理、代码组织、命名规范等
- **[设计系统 v1](./docs/DESIGN_SYSTEM_V1.md)** ⭐ **必读** - 完整的设计系统文档（主题、色彩、组件规范）
- **[UI 编码规范](./docs/CODING_STYLE_UI.md)** ⭐ **必读** - UI 开发编码规范和最佳实践
- **[代码规范检查报告](./docs/quality/CODE_STANDARDS_CHECK.md)** - 代码规范符合度检查结果和改进建议
- [模块系统文档](./docs/architecture/README_MODULES.md) - 模块化系统说明
- [Naive UI 使用指南](./docs/config/README_NAIVE_UI.md) - UI 组件库使用说明

### 📖 其他文档

项目文档已分类整理到 `docs/` 目录，包括：

- 📋 [功能开发文档](./docs/features/) - 功能状态、开发日志、优化计划
- 🔧 [改进与优化文档](./docs/improvements/) - 改进计划、升级记录
- ⚙️ [配置文档](./docs/config/) - API 配置、环境配置
- 🏗️ [架构文档](./docs/architecture/) - 系统架构说明
- 🚀 [部署文档](./docs/deployment/) - 部署指南、启动说明
- 🐛 [故障排除文档](./docs/troubleshooting/) - 问题排查、Bug 修复
- 📊 [代码质量文档](./docs/quality/) - 代码质量报告、规范检查
- 📝 [迁移文档](./docs/migration/) - 数据迁移指南

详细文档索引请查看 [文档目录](./docs/README.md)

## 🆕 新电脑快速配置

如果你在新电脑上配置开发环境，请按以下步骤：

1. **查看快速开始指南**：[快速开始指南](./docs/deployment/QUICK_START.md)
2. **运行一键配置脚本**：
   - Windows: `.\scripts\setup-dev-env.ps1`
   - Linux/macOS: `./scripts/setup-dev-env.sh`
3. **按照检查清单验证**：[ENVIRONMENT_CHECKLIST.md](./docs/deployment/ENVIRONMENT_CHECKLIST.md)

## 📧 联系方式

- **邮箱**：linxiwanting@gmail.com
- **微信**：LinXi-5152
- **GitHub**：https://github.com/Lijing327

## 📄 许可证

MIT License - 详见 [LICENSE](LICENSE) 文件

---

**让代码改变世界，让技术创造价值！** 🚀 