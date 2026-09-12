# 项目结构完整指南

> **本文档用于快速定位项目中的功能、内容和编辑位置**

**最后更新**：2025-01-22

---

## 📋 目录

1. [核心目录结构](#核心目录结构)
2. [功能模块位置](#功能模块位置)
3. [内容编辑位置](#内容编辑位置)
4. [后台管理位置](#后台管理位置)
5. [数据库相关](#数据库相关)
6. [快速查找表](#快速查找表)

---

## 🗂️ 核心目录结构

```
PersonWeb/
├── 📄 配置文件
│   ├── nuxt.config.ts          # Nuxt 主配置
│   ├── package.json            # 前端依赖
│   ├── tailwind.config.ts      # Tailwind 配置
│   └── tsconfig.json           # TypeScript 配置
│
├── 📱 前端核心目录
│   ├── pages/                  # 页面路由（自动生成路由）
│   ├── components/            # Vue 组件
│   ├── layouts/               # 布局组件
│   ├── composables/           # 组合式函数（可复用逻辑）
│   ├── middleware/            # 路由中间件
│   ├── plugins/               # 插件（启动时执行）
│   ├── server/                # 服务端 API（Nuxt Server API）
│   ├── types/                 # TypeScript 类型定义
│   └── constants/             # 常量定义
│
├── 🎨 样式资源
│   └── assets/
│       ├── css/               # 全局 CSS 文件
│       └── styles/            # 主题样式
│
├── 💾 后端代码
│   └── backend/
│       └── PersonalSite.Api/  # .NET 8 WebAPI 项目
│           ├── Controllers/   # API 控制器
│           ├── Models/        # 数据模型
│           ├── Services/      # 业务逻辑
│           └── Data/          # 数据访问层（EF Core）
│
├── 🤖 AI 服务
│   └── ai-service/        # Python FastAPI
│
├── 🗄️ 数据库
│   └── database/              # SQL 脚本和迁移文件
│
├── 📝 内容文件（Markdown）
│   └── content/               # @nuxt/content 管理的 Markdown 文件
│
└── 📚 项目文档
    └── docs/                  # 所有项目文档
```

---

## 🎯 功能模块位置

### 1. 前端页面（`pages/`）

| 功能 | 路径 | 说明 |
|------|------|------|
| **首页** | `pages/index.vue` | 网站首页 |
| **关于我** | `pages/work/about.vue` | 个人介绍页面 |
| **博客列表** | `pages/work/blog/index.vue` | 博客文章列表 |
| **博客详情** | `pages/work/blog/[id].vue` | 博客文章详情 |
| **项目列表** | `pages/work/projects/index.vue` | 项目展示列表 |
| **项目详情** | `pages/work/projects/[id].vue` | 项目详情页 |
| **工具列表** | `pages/work/tools/index.vue` | 工具展示列表 |
| **工具详情** | `pages/work/tools/[slug].vue` | 工具详情页 |
| **生活随笔** | `pages/life/index.vue` | 生活随笔列表 |
| **生活随笔详情** | `pages/life/[...slug].vue` | 生活随笔详情 |
| **AI 方案** | `pages/work/ai/index.vue` | Work 世界 AI 解决方案 |
| **知识库** | `pages/knowledge/index.vue` | 知识库页面 |
| **技能树** | `pages/skills/index.vue` | 技能展示 |
| **认知说明书** | `pages/life/cognition/index.vue` | Life 世界个人认知说明书 |
| **生活想法** | `pages/life/thoughts/index.vue`、`[category].vue` | 精选页 + 分类归档 |

### 2. 后台管理页面（`pages/admin/`）

| 功能 | 路径 | 说明 |
|------|------|------|
| **后台首页** | `pages/admin/index.vue` | 数据概览 |
| **数据分析** | `pages/admin/analytics.vue` | 访问分析 |
| **AI 中心** | `pages/admin/ai/index.vue` | AI 服务与工具入口 |
| **访客留言** | `pages/admin/visitor-messages.vue` | 互动收件箱 |
| **咨询管理** | `pages/admin/consultations.vue` | 咨询线索 |
| **访客明细** | `pages/admin/visitors.vue` | 深链，不在侧栏 |
| **订单** | `pages/admin/orders.vue` | 深链，不在侧栏 |
| **认知说明书** | `pages/admin/cognition/index.vue` | 深链；公开页在 Life |

### 3. 组件位置（`components/`）

| 类型 | 路径 | 说明 |
|------|------|------|
| **首页组件** | `components/home/` | 首页专用组件 |
| **布局组件** | `components/layout/` | Header、Footer、导航等 |
| **AI 组件** | `components/ai/` | AI 相关组件 |
| **3D 组件** | `components/3d/` | Three.js 3D 场景 |
| **效果组件** | `components/effects/` | 动画、粒子等视觉效果 |
| **内容组件** | `components/content/` | 内容展示组件 |
| **后台组件** | `components/admin/` | 后台管理组件 |
| **工具组件** | `components/tools/` | 工具相关组件 |
| **访客组件** | `components/Visitor*.vue` | 访客互动组件 |
| **时间组件** | `components/time/` | 时间胶囊、时间轴等 |
| **英语组件** | `components/english/` | 英语学习组件 |
| **关系组件** | `components/relations/` | 人际关系管理组件 |

### 4. 组合式函数（`composables/`）

| 功能 | 文件 | 说明 |
|------|------|------|
| **API 调用** | `composables/useApi.ts` | 统一的 API 请求封装 |
| **主题管理** | `composables/useTheme.ts` | 主题切换和管理 |
| **模块系统** | `composables/useModuleSystem.ts` | 功能模块开关 |
| **Markdown** | `composables/useMarkdown.ts` | Markdown 渲染 |
| **错误处理** | `composables/useErrorHandler.ts` | 统一错误处理 |
| **Toast 提示** | `composables/useToast.ts` | 消息提示 |
| **设备检测** | `composables/useDevice.ts` | 设备类型检测 |
| **页面样式** | `composables/usePageStyle.ts` | 页面样式管理 |

---

## ✏️ 内容编辑位置

### 1. 通过后台管理系统编辑（推荐）

**访问方式**：`/admin` → 登录 → 选择对应功能模块

| 内容类型 | 编辑位置 | 说明 |
|----------|----------|------|
| **Life / Work 文案** | 前台登录后点改，或 `content/` / Cursor | 后台不做 CMS |
| **工具 / 友链** | MySQL / 代码直接维护 | 已移除 `/admin/content` 等 CRUD |
| **互动收件** | `/admin/visitor-messages`、`/admin/consultations` | 留言与咨询 |
| **数据看盘** | `/admin`、`/admin/analytics` | 概览与分析 |

### 2. 通过 Markdown 文件编辑（部分功能）

**位置**：`content/` 目录

| 内容类型 | 路径 | 说明 |
|----------|------|------|
| **Life 首页文案** | `content/life/home.yml` | `/life` 固定话术；登录后也可在页面上点改 |
| **Life About** | `content/life/profile.md` | `/life/about` 自我介绍 |
| **Life Now** | `content/life/now.yml` | `/life`「最近在」 |
| **Life Moments** | `content/life/moments.yml` | `/life`「最近」时间线 |
| **生活随笔** | `content/life/*.md` | 随笔正文（不要用 `profile.md` 这个文件名） |
| **认知说明书** | `/life/cognition`（`content/life/cognition.yml` + Nitro） | Life 世界当前页内联编辑；`/admin/cognition` 为遗留深链 |
| **生活想法** | `/life/thoughts`（`content/life/thoughts/*` + Nitro） | 精选卡片首页；`/life/thoughts/:category` 全量归档；`/life/margin` 301 |
| **Work 首页文案** | `content/work/home.yml` | `/work`；登录后可在页面上点改 |

> ⚠️ **注意**：大部分内容已迁移到数据库，通过后台管理系统编辑。只有部分内容仍使用 Markdown 文件。

### 3. 直接编辑代码

| 内容类型 | 位置 | 说明 |
|----------|------|------|
| **首页内容** | `pages/index.vue` | 首页展示内容 |
| **关于我** | `pages/about.vue` | 个人信息 |
| **页面文案** | 对应页面的 `.vue` 文件 | 直接修改组件中的文本 |

---

## 🎛️ 后台管理位置

### 后台管理入口

- **URL**：`/admin` 或 `/admin/login`
- **默认布局**：`layouts/admin.vue`

### 主要管理功能

#### 数据与互动（侧栏）
- **网站概览**：`/admin`
- **数据分析**：`/admin/analytics`
- **AI 中心**：`/admin/ai`
- **访客留言**：`/admin/visitor-messages`
- **咨询管理**：`/admin/consultations`

#### 内容
- 站点正文与工具/友链：**不用后台 CMS**；改 `content/`、前台点改，或直接改库/代码

#### 系统管理
- **系统设置**：`/admin/settings`
- **模块管理**：`/admin/settings/modules`
- **主题设置**：`/admin/settings/themes`
- **样式管理**：`/admin/settings/styles`
- **用户管理**：`/admin/users`

#### AI 管理
- **AI 助手**：`/admin/ai`
- **AI 内容**：`/admin/ai/content`
- **AI 日志**：`/admin/ai/logs`

---

## 🗄️ 数据库相关

### 数据库脚本位置

**路径**：`database/` 目录

| 文件类型 | 说明 | 示例 |
|----------|------|------|
| **表结构** | 创建表的 SQL | `all_tables.sql` |
| **迁移脚本** | 表结构变更 | `*_migration.sql` |
| **数据脚本** | 初始化数据 | `sample_data.sql` |
| **文档** | 数据库说明 | `README.md` |

### 主要数据表

| 表名 | 说明 | 管理位置 |
|------|------|----------|
| `article` | 文章表 | `/admin/articles` |
| `projects` | 项目表 | `/admin/projects` |
| `tool` | 工具表 | 前台 `/work/tools`；无后台 CRUD 页 |
| `category` | 分类表 | `/admin/categories` |
| `user` | 用户表 | `/admin/users` |
| `visitor` | 访客表 | `/admin/visitors` |
| `relation_person` | 关系人表 | `/admin/relations` |

### 数据库连接配置

- **后端配置**：`backend/PersonalSite.Api/appsettings.Development.json`
- **连接字符串**：`ConnectionStrings:DefaultConnection`

---

## 🔍 快速查找表

### 我想编辑...

| 我想... | 去哪里找 |
|---------|----------|
| **前台改 Life/Work 文案** | 先 `/admin/login`，再打开 `/life` 或 `/work`，点击文字保存 |
| **编辑文章** | `/admin/articles` → 点击"编辑" |
| **添加新文章** | `/admin/articles` → 点击"新增文章" |
| **编辑项目** | `/admin/projects` → 点击"编辑" |
| **管理工具数据** | 改 MySQL `Tools` / `.NET`，或 Cursor；无 `/admin/tools` |
| **修改首页内容** | `pages/index.vue` |
| **修改关于我** | `pages/about.vue` |
| **修改网站标题** | `/admin/settings` 或 `nuxt.config.ts` |
| **切换主题** | `/admin/settings/themes` |
| **启用/禁用功能** | `/admin/settings/modules` |
| **查看访客数据** | `/admin/visitors` |
| **管理分类** | `/admin/categories` |
| **修改样式** | `assets/css/` 或 `/admin/settings/styles` |
| **添加新页面** | `pages/` 目录下创建 `.vue` 文件 |
| **添加新组件** | `components/` 目录下创建 `.vue` 文件 |
| **修改 API** | `backend/PersonalSite.Api/Controllers/` |
| **查看数据库** | 使用 MySQL 客户端连接数据库 |
| **执行 SQL** | `database/` 目录下的 SQL 文件 |

### 我想了解...

| 我想了解... | 查看文档 |
|-------------|----------|
| **项目整体架构** | `docs/PROJECT_OVERVIEW.md` |
| **开发规范** | `docs/development/DEVELOPMENT_GUIDELINES.md` |
| **模块系统** | `docs/architecture/README_MODULES.md` |
| **API 配置** | `docs/config/API_CONFIG.md` |
| **部署指南** | `docs/deployment/QUICK_START.md` |
| **设计系统** | `docs/DESIGN_SYSTEM_V1.md` |

---

## 📂 重要文件说明

### 配置文件

| 文件 | 说明 |
|------|------|
| `nuxt.config.ts` | Nuxt 主配置（模块、路由、构建等） |
| `package.json` | 前端依赖和脚本 |
| `tailwind.config.ts` | Tailwind CSS 配置 |
| `tsconfig.json` | TypeScript 配置 |

### 常量定义

| 文件 | 说明 |
|------|------|
| `constants/modules.ts` | 功能模块定义 |
| `constants/design/tokens.ts` | 设计令牌（颜色、间距等） |

### 类型定义

| 文件 | 说明 |
|------|------|
| `types/api.ts` | API 相关类型 |
| `types/module.ts` | 模块系统类型 |
| `types/name-tool.ts` | 取名工具类型 |

---

## 🎨 样式管理

### 样式文件位置

| 类型 | 路径 | 说明 |
|------|------|------|
| **全局样式** | `assets/css/main.css` | 全局样式 |
| **设计系统** | `assets/css/design-system.css` | 设计令牌 |
| **主题样式** | `assets/css/themes.css` | 主题样式 |
| **页面样式** | `assets/css/*.css` | 各页面专用样式 |

### 样式管理方式

1. **通过后台管理**：`/admin/settings/styles`
2. **直接编辑文件**：`assets/css/` 目录
3. **使用设计令牌**：`constants/design/tokens.ts`

---

## 🔧 后端 API

### API 控制器位置

**路径**：`backend/PersonalSite.Api/Controllers/`

| 控制器 | 说明 | 前端调用 |
|--------|------|----------|
| `ArticlesController.cs` | 文章 API | `/api/Articles` |
| `ProjectsController.cs` | 项目 API | `/api/Projects` |
| `ToolsController.cs` | 工具 API | `/api/Tools` |
| `CategoriesController.cs` | 分类 API | `/api/Categories` |

### API 调用方式

前端使用 `composables/useApi.ts`：

```typescript
const api = useApi()
const articles = await api.get('/Articles')
```

---

## 🤖 AI 服务

### AI 服务位置

**路径**：`python-ai/app/`

| 目录 | 说明 |
|------|------|
| `api/` | API 路由 |
| `services/` | 服务层（LLM、RAG 等） |
| `schemas/` | Pydantic 数据模型 |
| `prompts/` | Prompt 模板 |

### AI 服务管理

- **管理界面**：`/admin/ai`
- **内容管理**：`/admin/ai/content`
- **日志查看**：`/admin/ai/logs`

---

## 📝 总结

### 内容编辑流程

1. **登录后台**：访问 `/admin` 并登录
2. **选择功能**：根据内容类型选择对应管理页面
3. **编辑保存**：在编辑页面修改内容并保存
4. **查看效果**：访问前端页面查看效果

### 开发新功能流程

1. **创建页面**：在 `pages/` 目录创建 `.vue` 文件
2. **创建组件**：在 `components/` 目录创建组件
3. **创建 API**：在 `backend/` 创建控制器
4. **创建数据表**：在 `database/` 创建 SQL 脚本
5. **添加路由**：Nuxt 自动根据 `pages/` 生成路由

### 常见问题

**Q: 我想修改某个页面的内容，但不知道在哪里？**  
A: 查看 [快速查找表](#快速查找表)，或搜索页面标题关键词。

**Q: 我想添加新功能，应该从哪里开始？**  
A: 1) 在 `pages/` 创建页面；2) 在 `components/` 创建组件；3) 在 `backend/` 创建 API；4) 在 `database/` 创建数据表。

**Q: 内容应该存在数据库还是 Markdown 文件？**  
A: **优先使用数据库**，通过后台管理系统编辑。只有部分特殊内容（如生活随笔）使用 Markdown 文件。

---

**最后更新**：2025-01-22  
**维护者**：溪午听风
