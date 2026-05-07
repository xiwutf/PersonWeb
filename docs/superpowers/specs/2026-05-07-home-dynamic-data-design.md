# 首页动态数据关联设计文档

**日期**：2026-05-07  
**状态**：已确认，待实施  
**目标**：把首页 6 屏从全量硬编码改为从现有业务模块读取真实数据，不改视觉风格，不新增复杂业务表。

---

## 一、背景与约束

- 首页 1～6 屏视觉结构已定型，**不改 UI 风格**
- 不重写设计 token，不新增复杂业务表
- 数据来源：**.NET 后端 MySQL**（Projects、Articles、Timeline）+ **Nuxt markdown 文件**（Tools）
- 接口失败时 fallback 到静态默认数据，保证首页不崩
- 所有动态数据都有 loading / empty / error 状态

---

## 二、现有数据模型（已确认字段）

### projects 表
| 字段 | 类型 | 说明 |
|---|---|---|
| Id | CHAR(36) | GUID |
| Title | VARCHAR(100) | 项目标题 |
| Description | VARCHAR(500) | 描述 |
| CoverUrl | VARCHAR(200) | 封面图 |
| DemoUrl | VARCHAR(200) | 演示地址 |
| GithubUrl | VARCHAR(200) | GitHub 地址 |
| Status | VARCHAR(50) | `Active` \| `Completed` \| `Archived` |
| TechStack | TEXT | JSON 数组或逗号分隔 |
| ViewCount | INT | 访问量 |
| CreatedAt / UpdatedAt | DATETIME | 时间戳 |

**缺失字段处理**：
- `featured` → 用 `ViewCount DESC` 排序取 top N
- `tags` → 解析 `TechStack` 字段
- `progress` → 由前端常量 `constants/nowBuilding.ts` 维护

### article 表
| 字段 | 类型 | 说明 |
|---|---|---|
| id | BIGINT | 自增 ID |
| title | VARCHAR(255) | 标题 |
| slug | VARCHAR(255) | URL 别名 |
| summary | VARCHAR(500) | 摘要 |
| cover_url | VARCHAR(500) | 封面图 |
| status | TINYINT | `0`=草稿 `1`=已发布 `2`=下线 |
| publish_time | DATETIME | 发布时间 |
| view_count | INT | 浏览量 |
| CategoryName | (关联) | 分类名 |

**精选文章**：`view_count DESC` 第一篇；最新文章：`publish_time DESC` 跳过精选取前 4

### timeline_event 表（已有完整 API）
| 字段 | 类型 | 说明 |
|---|---|---|
| id | BIGINT | |
| year | INT | 年份 |
| title | VARCHAR(255) | 节点标题 |
| description | TEXT | 描述 |
| icon | VARCHAR(50) | 图标类名或 emoji |
| color | VARCHAR(50) | 颜色主题 |
| status | TINYINT | `0`=隐藏 `1`=显示 |

API：`GET /api/Timeline` → 返回 status=1 的事件，按 year ASC

### Tools（markdown 文件）
`content/tools/*.md` — 通过 `readMarkdownCollection('tools')` 读取，仅用于统计数量

---

## 三、架构方案（方案 A：单一聚合接口）

```
浏览器请求 /
  └── pages/index.vue
        └── useHomeOverview()
              └── useLazyAsyncData('home-overview', server: false)
                    └── GET /api/home/overview（Nuxt 服务端）
                          ├── $fetch .NET /api/Projects
                          ├── $fetch .NET /api/Articles?status=1&pageSize=50
                          ├── $fetch .NET /api/Timeline
                          └── readMarkdownCollection('tools')
                          ↓
                          组装 HomeOverview + Cache-Control: public, max-age=300
              ↓
              data ?? FALLBACK → overview
              ↓
        分发 props 给 6 个子组件
```

**关键约定**：
- 每个子请求独立 try/catch，单个失败只影响对应字段
- `server: false` → 首屏用 FALLBACK 快速渲染，数据到了自动替换
- 组件只接收 props，不自己请求接口

---

## 四、类型定义（`types/home.ts`）

```typescript
export interface HomeStats {
  projects: number
  articles: number
  tools: number
}

export interface HomeProjectCard {
  id: string
  title: string
  description: string
  techStack: string[]
  coverUrl: string | null
  demoUrl: string | null
  githubUrl: string | null
  status: string           // 'Active' | 'Completed' | 'Archived'
  viewCount: number
}

export interface HomeArticleCard {
  id: number
  title: string
  slug: string | null
  summary: string | null
  coverUrl: string | null
  publishTime: string | null
  viewCount: number
  categoryName: string | null
  path: string             // /blog/${slug ?? id}
}

export interface HomeJourneyItem {
  id: number
  year: number
  title: string
  description: string | null
  icon: string | null
  color: string | null
  isNow: boolean           // year === new Date().getFullYear()
}

export interface HomeNowBuildingItem {
  id: string
  title: string
  description: string
  techStack: string[]
  status: string
  progress: number         // 来自 constants/nowBuilding.ts，无匹配给 30
}

export interface HomeOverview {
  stats: HomeStats
  featuredProjects: HomeProjectCard[]    // ViewCount DESC，最多 5 条，排除 Archived
  featuredArticle: HomeArticleCard | null
  latestArticles: HomeArticleCard[]       // publish_time DESC，最多 4 篇，不含精选
  nowBuilding: HomeNowBuildingItem[]      // status=Active，最多 4 条
  journey: HomeJourneyItem[]
}
```

---

## 五、聚合接口（`server/api/home/overview.get.ts`）

### 数据处理逻辑

```
Projects 原始数据：
  featuredProjects = 过滤掉 Archived → ViewCount DESC → 取前 5
  nowBuilding      = status='Active' → 取前 4
                     → 每条查 PROJECT_PROGRESS[title] ?? 30

Articles 原始数据（status=1）：
  featuredArticle  = view_count DESC → 第 1 篇
  latestArticles   = publish_time DESC → 跳过 featuredArticle → 取前 4

Timeline：
  journey          = 全部按 year ASC
                     isNow = (year === new Date().getFullYear())

Tools：
  stats.tools      = readMarkdownCollection('tools').length

stats：
  projects = projects 总数（不含 Archived）
  articles = articles status=1 总数
  tools    = 上面计数
```

### 运行时配置

Nuxt 服务端需知道 .NET 后端地址，通过 `useRuntimeConfig()` 读取：
- 开发：`NUXT_PUBLIC_API_BASE=http://localhost:5234/api`
- 生产：`NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api`

---

## 六、常量文件

### `constants/nowBuilding.ts`
```typescript
// key = 项目 title（精确匹配）
export const PROJECT_PROGRESS: Record<string, number> = {
  '心动联谊平台': 68,
  'AI Workflow 系统': 42,
  'Desktop Pet 2.0': 15,
  '个人知识中枢 2.0': 5,
}
export const DEFAULT_PROGRESS = 30
```

### `constants/homeContact.ts`
```typescript
export const CONTACT_METHODS = [
  {
    title: 'Email',
    value: 'linxiwanting@gmail.com',
    note: '工作日 24h 内回复',
    href: 'mailto:linxiwanting@gmail.com',
    action: '发送邮件',
    icon: 'contact-icon-mail'
  },
  {
    title: 'GitHub',
    value: 'Lijing327',
    note: '查看开源项目与代码',
    href: 'https://github.com/Lijing327',
    action: '访问主页',
    icon: 'contact-icon-github'
  },
  {
    title: '微信',
    value: '扫码添加',
    note: '备注「合作」优先通过',
    href: '/contact',
    action: '查看二维码',
    icon: 'contact-icon-wechat'
  }
] as const

export const COLLAB_DIRECTIONS = [
  'AI 应用开发与落地',
  '企业数字化改造',
  '个人产品孵化',
  '技术咨询与评审',
] as const
```

---

## 七、Composable（`composables/useHomeOverview.ts`）

```typescript
const FALLBACK: HomeOverview = {
  stats: { projects: 20, articles: 50, tools: 8 },
  featuredProjects: [],
  featuredArticle: null,
  latestArticles: [],
  nowBuilding: [],
  journey: [
    { id: 0, year: 2020, title: '开始构建', description: '从第一行代码开始', icon: null, color: null, isNow: false },
    { id: 1, year: 2023, title: '持续进化', description: '产品与 AI 深度融合', icon: null, color: null, isNow: false },
    { id: 2, year: new Date().getFullYear(), title: '正在构建', description: '持续迭代，把想法变成产品', icon: null, color: null, isNow: true },
  ]
}

export const useHomeOverview = () => {
  const { data, pending, error } = useLazyAsyncData(
    'home-overview',
    () => $fetch<HomeOverview>('/api/home/overview'),
    { server: false }
  )

  const overview = computed(() => data.value ?? FALLBACK)
  const loading = computed(() => pending.value)

  return { overview, loading, error }
}
```

---

## 八、组件 Props 契约

| 组件 | 新增 Props | loading 行为 |
|---|---|---|
| HomeHero | `stats: HomeStats`, `loading: boolean` | 统计数字显示 `—` |
| HomeProducts | `projects: HomeProjectCard[]`, `loading: boolean` | 3 个骨架卡片 |
| HomeTimeline | `featuredArticle: HomeArticleCard\|null`, `articles: HomeArticleCard[]`, `loading: boolean` | 精选区占位卡 |
| HomeAbout | `journey: HomeJourneyItem[]`, `loading: boolean` | fallback 到组件内 3 条静态数据 |
| HomeCTA | 无 props | 纯常量驱动 |
| HomeBuilding | `projects: HomeNowBuildingItem[]`, `loading: boolean` | 骨架卡片 |

### pages/index.vue 编排（关键改动）

```vue
<script setup lang="ts">
definePageMeta({ layout: false })
const { toggleDark } = useTheme()
const { overview, loading } = useHomeOverview()
// 导航 scroll 逻辑不变
</script>

<template>
  <HomeHero :stats="overview.stats" :loading="loading" />
  <LazyHomeProducts :projects="overview.featuredProjects" :loading="loading" />
  <LazyHomeTimeline
    :featured-article="overview.featuredArticle"
    :articles="overview.latestArticles"
    :loading="loading"
  />
  <LazyHomeAbout :journey="overview.journey" :loading="loading" />
  <LazyHomeCTA />
  <LazyHomeBuilding :projects="overview.nowBuilding" :loading="loading" />
</template>
```

---

## 九、状态值映射（前端展示）

| DB Status | 首屏卡片文案 | 样式 class |
|---|---|---|
| `Active` | 进行中 | `status-blue` |
| `Completed` | 已上线 | `status-teal` |
| `Archived` | 已归档 | `status-indigo`（第二屏不展示） |

文章 status：
| DB status | 含义 |
|---|---|
| `0` | 草稿，不展示 |
| `1` | 已发布，展示 |
| `2` | 下线，不展示 |

---

## 十、验收标准

1. 首页刷新后能正常展示动态数据
2. 后台新增 `status=1` 文章后，首页第三屏自动出现
3. 后台新增 `status=Active` 项目后，首页第二屏和第六屏自动出现
4. 接口全部失败时首页仍显示 FALLBACK 内容，不崩
5. 不影响现有 `/blog`、`/projects`、`/tools`、`/lab` 页面
6. `npm run build` 通过

---

## 十一、修改文件清单

### 新增
- `types/home.ts`
- `composables/useHomeOverview.ts`
- `server/api/home/overview.get.ts`
- `constants/nowBuilding.ts`
- `constants/homeContact.ts`

### 修改
- `pages/index.vue` — 接入 `useHomeOverview`，传 props
- `components/home/HomeHero.vue` — 加 stats/loading props
- `components/home/HomeProducts.vue` — 加 projects/loading props
- `components/home/HomeTimeline.vue` — 加 articles/loading props
- `components/home/HomeAbout.vue` — 加 journey/loading props
- `components/home/HomeCTA.vue` — 替换硬编码为常量引用
- `components/home/HomeBuilding.vue` — 加 projects/loading props
