# 溪午听风 Personal Site

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Nuxt 3](https://img.shields.io/badge/Nuxt-3-00DC82.svg)](https://nuxt.com)
[![.NET 8](https://img.shields.io/badge/.NET-8-512BD4.svg)](https://dotnet.microsoft.com/)
[![Node.js](https://img.shields.io/badge/Node.js-18+-339933.svg)](https://nodejs.org)

全栈个人站。首页是入口，分成 **生活**（`/life`）和 **工作**（`/work`）两个世界；后台负责数据与互动，业务数据走 .NET API。

线上站点：[https://xifg.com.cn](https://xifg.com.cn)

> 代码采用 [MIT](./LICENSE) 发布。文章、作品等内容版权归原作者所有。

## 站点结构

| 区域 | 路径 | 做什么 |
| --- | --- | --- |
| 入口 | `/` | 生活 / 工作两个世界的入口 |
| 生活 | `/life` | 随笔、最近在做的事、认知说明书、生活想法 |
| 工作 | `/work` | 项目、工具、文章、关于与联系 |
| 后台 | `/admin` | 概览、访问分析、AI 中心、留言与咨询 |
| 旧地址 | `/blog`、`/projects` 等 | 301 到对应的 Work / Life 路径 |

## 技术栈

| 层 | 技术 |
| --- | --- |
| 前端 | Nuxt 3、Vue 3、TypeScript、Naive UI、Tailwind CSS |
| 前端服务端 | Nitro（`server/api/`），Life / Work 文案与文章正文 |
| 后端 | .NET 8 WebAPI（`backend/PersonalSite.Api/`，默认 `http://localhost:5234`） |
| AI 服务 | Python FastAPI（`ai-service/`，可选） |
| 数据库 | MySQL（库名 `personal_site`，脚本在 `database/all_tables.sql`） |
| 样式 | `assets/styles/tokens.css` 为颜色、圆角、阴影的唯一来源 |

本地请求大致是：浏览器访问 `localhost:3000`；页面里的 API 在本机会打到 `http://localhost:5234/api`。生产环境由 Nginx 提供前端静态文件，并把 `/api` 转到 .NET。

## 快速开始

环境：Node.js 18+、.NET SDK 8、MySQL 8（或 5.7）、Git。

一键配置：

```powershell
# Windows
.\scripts\setup-dev-env.ps1
```

```bash
# Linux / macOS
chmod +x scripts/setup-dev-env.sh
./scripts/setup-dev-env.sh
```

手动启动：

```bash
npm install
cp .env.example .env
```

编辑 `.env`，至少设置 `NUXT_PUBLIC_API_BASE` 和 `ADMIN_PASSWORD`。数据库连接写在 `backend/PersonalSite.Api/appsettings.Development.json`，库名为 `personal_site`。建库后执行：

```bash
mysql -u root -p personal_site < database/all_tables.sql
```

两个终端分别启动：

```bash
cd backend/PersonalSite.Api && dotnet run
```

```bash
npm run dev
```

- 前端：http://localhost:3000
- Swagger：http://localhost:5234/swagger
- 后台：http://localhost:3000/admin/login

手机调试用 `npm run dev:mobile`，或 `.\scripts\dev-mobile.ps1` / `./scripts/dev-mobile.sh`，然后访问 `http://<电脑 IP>:3000`。

更细的步骤见 [快速开始](./docs/deployment/QUICK_START.md)、[开发环境](./docs/deployment/DEVELOPMENT_SETUP.md)、[后端启动](./docs/deployment/START_BACKEND.md)。

### 常用命令

| 命令 | 作用 |
| --- | --- |
| `npm run dev` | 开发服务器 |
| `npm run dev:fresh` | 清缓存后启动 |
| `npm run build` | 生产构建 |
| `npm run generate` | 静态站点生成 |
| `npm run preview` | 预览构建结果 |
| `npm run test:run` | 跑一遍测试 |
| `npm run lint:colors` | 检查颜色 token |

AI 服务不是本地必启项。需要时见 [ai-service/README.md](./ai-service/README.md)。

## 内容放在哪

| 内容 | 位置 | 说明 |
| --- | --- | --- |
| 文章正文 | `content/articles/*.md` | Git 是正文来源；运营字段在 MySQL `content_ops` |
| 生活文案 | `content/life/` | 首页话术、随笔、认知说明书、想法 |
| 工作文案 | `content/work/` | `/work` 首页、关于、联系等 |
| 项目 / 工具 | MySQL | 由后台写入，不再使用 `content/projects`、`content/tools` |

登录后，部分 Life / Work 文案可以在前台页面上直接改。文章示例见 `content/articles/` 里现有 frontmatter（`title`、`slug`、`status`、`summary`、`date`、`category`）。

## 目录

```
PersonWeb/
├── pages/          # 路由：index、life、work、admin
├── components/     # Vue 组件
├── composables/    # useApi、主题、模块系统
├── server/api/     # Nitro 路由（内容、登录等）
├── content/        # 文章、Life、Work 文案
├── assets/styles/  # tokens.css、base.css
├── backend/        # .NET 8 WebAPI
├── ai-service/     # Python FastAPI
├── database/       # MySQL 脚本
└── docs/           # 开发与部署手册
```

样式改动先看 [样式架构](./docs/development/STYLE_ARCHITECTURE.md)。颜色、圆角、阴影用 `tokens.css` 里的变量，不要在页面里硬编码。

## 文档

接手开发先读：

1. [AGENTS.md](./AGENTS.md) — 任务入口
2. [项目概览](./docs/PROJECT_OVERVIEW.md)
3. [项目结构](./docs/PROJECT_STRUCTURE_GUIDE.md)
4. [开发规范](./docs/development/DEVELOPMENT_GUIDELINES.md)

专题：

- [设计系统](./docs/design-system/README.md)
- [模块系统](./docs/architecture/README_MODULES.md)
- [模块开发指南](./docs/development/MODULE_DEVELOPMENT_GUIDE.md)
- [API 配置](./docs/config/API_CONFIG.md)
- [部署说明](./docs/deployment/README.md)
- [文档目录](./docs/README.md)

## 部署

生产环境是 Nginx 提供前端静态文件，`/api` 反向代理到 .NET。构建：

```bash
npm run generate
```

后端以 .NET 应用运行。SEO 与 Nginx 相关问题见 [SEO：OSS + Nginx](./docs/deployment/SEO_OSS_NGINX_FIX.md)。

## 贡献

1. Fork 后从 `master` 拉出分支：`git checkout -b feature/your-feature-name`
2. 提交信息用约定式提交，例如 `feat: ...`、`fix: ...`
3. 推送分支并开 Pull Request

问题与想法： [Issues](https://github.com/xiwutf/PersonWeb/issues)

## 作者

**谢峰（溪午听风）**

- 站点：https://xifg.com.cn
- GitHub：https://github.com/xiwutf
- 邮箱：linxiwanting@gmail.com

## 许可证

[MIT License](./LICENSE) © 2026

变更记录见 [CHANGELOG.md](./CHANGELOG.md)。
