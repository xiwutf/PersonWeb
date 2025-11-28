# 项目总体架构文档

## 一、项目总体架构

本项目采用 **前后端分离 + 静态前端 + API 后端 + 云端部署** 的架构。

*   **前端**：Vue3 (Nuxt 3) - 包含展示网站和后台管理系统。
*   **后端**：.NET 8 WebAPI。
*   **数据存储**：MySQL (阿里云 RDS)。
*   **静态文件 & 图片**：阿里云 OSS。
*   **后端服务运行**：阿里云 ECS。
*   **自动部署 (CI/CD)**：GitHub Actions。

### 架构逻辑图

```mermaid
graph TD
    User[浏览器访问] --> OSS[阿里云 OSS (前端静态资源)]
    User --> API[后端 API (ECS)]
    
    subgraph Frontend_CI_CD [前端构建流程]
        Vue3[Vue3 源码] --> GA_Front[GitHub Actions]
        GA_Front -->|Build & Upload| OSS
    end

    subgraph Backend_System [后端系统]
        API --> RDS[(MySQL 数据库)]
        API -->|文件上传| OSS
    end
    
    subgraph Backend_CI_CD [后端构建流程]
        DotNet[WebAPI 源码] --> GA_Back[GitHub Actions]
        GA_Back -->|Build & Deploy| API
    end
```

## 二、前端部分 (Vue3/Nuxt 3)

前端分为两个功能模块（合并在同一个 Nuxt 项目中）：

### 1. 展示站点 (Public)
用于展示个人信息、作品集、博客文章等。
*   **部署方式**：`npm run generate` 构建为静态文件 -> 上传至 OSS -> HTTPS 访问。
*   **主要路由**：
    *   `/`：首页
    *   `/blog`：博客列表
    *   `/blog/[id]`：博客详情
    *   `/projects`：项目/作品集
    *   `/dashboard`：个人数据仪表盘

### 2. 后台管理系统 (Admin)
用于内容管理、数据统计等。
*   **主要路由**：
    *   `/admin/login`：登录
    *   `/admin/dashboard`：仪表盘
    *   `/admin/articles`：文章管理
    *   `/admin/projects`：项目管理
    *   `/admin/metrics`：指标录入
    *   `/admin/settings`：网站配置

## 三、后端部分 (.NET 8 WebAPI)

后端部署在 ECS，提供 REST API 服务。

### 核心功能模块
*   `/api/Auth`：登录、鉴权 (JWT)
*   `/api/Articles`：博客文章增删改查
*   `/api/Projects`：项目/工具管理
*   `/api/Metrics`：个人指标数据管理
*   `/api/Files`：文件上传 (OSS)
*   `/api/Stats`：数据统计
*   `/api/Config`：网站配置

### 技术栈
*   **框架**：.NET 8 WebAPI
*   **ORM**：Entity Framework Core
*   **数据库**：MySQL
*   **鉴权**：JWT (JSON Web Token)

## 四、数据存储 (MySQL)

数据库名称：`personal_site`

### 核心表结构
*   `Users`：管理员账户
*   `Articles`：文章数据 (Title, Content, Slug, etc.)
*   `Projects`：项目数据 (Title, Description, TechStack, etc.)
*   `Metrics`：每日指标数据 (Date, Steps, Sleep, etc.)
*   `Configs`：网站配置 (Key-Value)

## 五、文件存储 (OSS)

*   **用途**：前端静态资源 (dist)、文章图片、封面图、附件。
*   **目录结构**：
    *   `/site/`：前端静态网站文件
    *   `/uploads/images/`：图片资源

## 六、自动化部署 (CI/CD)

### 前端 (deploy-frontend.yml)
*   触发：Push to `master`
*   流程：Install -> Build (Generate) -> Upload to OSS (ossutil)

### 后端 (deploy-backend.yml)
*   触发：Push to `master` (或指定路径)
*   流程：Build -> Publish -> Deploy to ECS (Docker or Service restart)

## 七、阿里云资源规划
*   **ECS**：运行 .NET 后端 API
*   **OSS**：托管前端静态资源、存储上传文件
*   **RDS**：MySQL 数据库
*   **域名**：`xifg.com.cn`
    *   主站：`https://xifg.com.cn` (指向 OSS)
    *   API：`https://api.xifg.com.cn` (指向 ECS)

## 八、未来扩展规划
*   多媒体上传管理
*   小工具管理 (在线工具)
*   AI 智能体集成
*   更多数据分析维度
