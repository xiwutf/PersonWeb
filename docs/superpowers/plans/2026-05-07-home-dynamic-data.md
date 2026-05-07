# 首页动态数据关联 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 把首页 6 屏从全量硬编码改为从 .NET 后端 MySQL 读取真实数据，同时保持视觉风格不变。

**Architecture:** 新建 Nuxt 服务端聚合接口 `/api/home/overview`，在服务端并发调用 .NET 后端的 Projects / Articles / Timeline API，组装成统一 JSON 后缓存 5 分钟。前端通过 `useHomeOverview` composable 获取数据，`pages/index.vue` 拿到数据后以 props 分发给 6 个子组件，组件只管渲染，不自己发请求。任何子请求失败时返回空值，composable 内置 FALLBACK 数据保证首页不崩。

**Tech Stack:** Nuxt 3 / Nitro server routes / $fetch / Vue 3 Composition API / Vitest

---

## 文件清单

| 状态 | 路径 | 职责 |
|---|---|---|
| 新增 | `types/home.ts` | 首页所有数据类型 |
| 新增 | `server/utils/parseTechStack.ts` | 解析 TechStack 字段（纯函数，可测） |
| 新增 | `server/tests/parseTechStack.test.ts` | 上述函数的单元测试 |
| 新增 | `constants/nowBuilding.ts` | 项目进度常量（title→progress 映射） |
| 新增 | `constants/homeContact.ts` | 联系方式与合作方向常量 |
| 新增 | `server/api/home/overview.get.ts` | 首页聚合接口 |
| 新增 | `composables/useHomeOverview.ts` | 数据拉取 + FALLBACK 管理 |
| 修改 | `nuxt.config.ts` | 添加私有 `backendApiBase` 运行时配置 |
| 修改 | `pages/index.vue` | 接入 composable，向子组件传 props |
| 修改 | `components/home/HomeHero.vue` | 加 stats / loading props |
| 修改 | `components/home/HomeProducts.vue` | 加 projects / loading props |
| 修改 | `components/home/HomeTimeline.vue` | 加 featuredArticle / articles / loading props |
| 修改 | `components/home/HomeAbout.vue` | 加 journey / loading props |
| 修改 | `components/home/HomeCTA.vue` | 替换硬编码为常量 import |
| 修改 | `components/home/HomeBuilding.vue` | 加 projects / loading props |

---

## Task 1: 类型定义

**Files:**
- Create: `types/home.ts`

- [ ] **Step 1: 创建类型文件**

```typescript
// types/home.ts
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
  status: string        // 'Active' | 'Completed' | 'Archived'
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
  path: string          // /blog/${slug ?? id}
}

export interface HomeJourneyItem {
  id: number
  year: number
  title: string
  description: string | null
  icon: string | null
  color: string | null
  isNow: boolean        // year === new Date().getFullYear()
}

export interface HomeNowBuildingItem {
  id: string
  title: string
  description: string
  techStack: string[]
  status: string
  progress: number      // 来自 constants/nowBuilding.ts
}

export interface HomeOverview {
  stats: HomeStats
  featuredProjects: HomeProjectCard[]     // ViewCount DESC，最多 5 条，排除 Archived
  featuredArticle: HomeArticleCard | null // CreatedAt DESC 第 1 篇（已发布）
  latestArticles: HomeArticleCard[]       // CreatedAt DESC，最多 4 篇，按 id 排除 featuredArticle
  nowBuilding: HomeNowBuildingItem[]      // status=Active，UpdatedAt DESC，最多 4 条
  journey: HomeJourneyItem[]              // timeline_event，year ASC
}
```

- [ ] **Step 2: Commit**

```bash
git add types/home.ts
git commit -m "feat(home): add HomeOverview type definitions"
```

---

## Task 2: TechStack 解析工具 + 测试

**Files:**
- Create: `server/utils/parseTechStack.ts`
- Create: `server/tests/parseTechStack.test.ts`

- [ ] **Step 1: 写测试（先写测试）**

```typescript
// server/tests/parseTechStack.test.ts
import { describe, it, expect } from 'vitest'
import { parseTechStack } from '../utils/parseTechStack'

describe('parseTechStack', () => {
  it('parses JSON array string', () => {
    expect(parseTechStack('["Vue 3", "TypeScript"]')).toEqual(['Vue 3', 'TypeScript'])
  })

  it('parses comma-separated string', () => {
    expect(parseTechStack('Vue 3, TypeScript, Nuxt')).toEqual(['Vue 3', 'TypeScript', 'Nuxt'])
  })

  it('returns empty array for null', () => {
    expect(parseTechStack(null)).toEqual([])
  })

  it('returns empty array for empty string', () => {
    expect(parseTechStack('')).toEqual([])
  })

  it('returns empty array for non-string input', () => {
    expect(parseTechStack(123)).toEqual([])
  })

  it('falls back to comma-split when JSON is malformed', () => {
    expect(parseTechStack('[malformed')).toEqual(['[malformed'])
  })

  it('filters out empty entries from comma split', () => {
    expect(parseTechStack('Vue 3,,TypeScript, ')).toEqual(['Vue 3', 'TypeScript'])
  })
})
```

- [ ] **Step 2: 运行测试，确认失败**

```bash
npx vitest run server/tests/parseTechStack.test.ts
```

Expected: 报错 `Cannot find module '../utils/parseTechStack'`

- [ ] **Step 3: 实现函数**

```typescript
// server/utils/parseTechStack.ts
export const parseTechStack = (raw: unknown): string[] => {
  if (!raw || typeof raw !== 'string') return []
  const s = raw.trim()
  if (s.startsWith('[')) {
    try {
      const parsed = JSON.parse(s) as unknown[]
      return parsed.filter((x): x is string => typeof x === 'string' && x.trim() !== '')
    } catch {
      // fall through to comma split
    }
  }
  return s.split(',').map(x => x.trim()).filter(Boolean)
}
```

- [ ] **Step 4: 运行测试，确认通过**

```bash
npx vitest run server/tests/parseTechStack.test.ts
```

Expected: 7 tests passed

- [ ] **Step 5: Commit**

```bash
git add server/utils/parseTechStack.ts server/tests/parseTechStack.test.ts
git commit -m "feat(home): add parseTechStack utility with tests"
```

---

## Task 3: 常量文件

**Files:**
- Create: `constants/nowBuilding.ts`
- Create: `constants/homeContact.ts`

- [ ] **Step 1: 创建进度常量**

```typescript
// constants/nowBuilding.ts
// ⚠️ key = 项目 title 精确匹配，标题变更会导致 progress 失效
// 后续建议改为 id 匹配（需后端返回 id）
export const PROJECT_PROGRESS: Record<string, number> = {
  '心动联谊平台': 68,
  'AI Workflow 系统': 42,
  'Desktop Pet 2.0': 15,
  '个人知识中枢 2.0': 5,
}

export const DEFAULT_PROGRESS = 30
```

- [ ] **Step 2: 创建联系方式常量**

```typescript
// constants/homeContact.ts
export const CONTACT_METHODS = [
  {
    title: '邮箱',
    value: 'linxiwanting@gmail.com',
    note: '工作日 24h 内回复',
    action: '发送邮件',
    href: 'mailto:linxiwanting@gmail.com',
    icon: 'icon-mail'
  },
  {
    title: '微信',
    value: '扫码添加',
    note: '备注「合作」优先通过',
    action: '查看二维码',
    href: '/contact',
    icon: 'icon-wechat'
  },
  {
    title: '预约会议',
    value: '适合项目咨询与深度沟通',
    note: '',
    action: '预约时间',
    href: '/contact',
    icon: 'icon-calendar'
  }
] as const

export const COLLAB_DIRECTIONS = [
  { title: 'AI 应用开发', description: 'AI 工具、智能体、自动化系统开发', icon: 'collab-ai' },
  { title: '产品合作', description: '产品共创、联合开发、资源互补', icon: 'collab-product' },
  { title: '内容与知识', description: '技术内容共创、知识付费、课程合作', icon: 'collab-knowledge' },
  { title: '咨询与顾问', description: 'AI 落地咨询、产品咨询、技术顾问', icon: 'collab-consult' }
] as const
```

- [ ] **Step 3: Commit**

```bash
git add constants/nowBuilding.ts constants/homeContact.ts
git commit -m "feat(home): add nowBuilding and homeContact constants"
```

---

## Task 4: 运行时配置 + 聚合接口

**Files:**
- Modify: `nuxt.config.ts` (runtimeConfig 块，约第 22 行)
- Create: `server/api/home/overview.get.ts`

- [ ] **Step 1: 在 nuxt.config.ts 添加私有运行时配置**

找到 `runtimeConfig` 块，修改为：

```typescript
runtimeConfig: {
  // 服务端私有配置：.NET 后端地址，用于 Nitro 服务端路由调用
  backendApiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5234/api',
  public: {
    apiBase: process.env.NUXT_PUBLIC_API_BASE || '/api'
  }
},
```

- [ ] **Step 2: 创建聚合接口**

```typescript
// server/api/home/overview.get.ts
import { readMarkdownCollection } from '../../utils/content-files'
import { parseTechStack } from '../../utils/parseTechStack'
import { PROJECT_PROGRESS, DEFAULT_PROGRESS } from '../../../constants/nowBuilding'
import type {
  HomeOverview, HomeProjectCard, HomeArticleCard,
  HomeJourneyItem, HomeNowBuildingItem, HomeStats
} from '../../../types/home'

export default defineEventHandler(async (event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=300, s-maxage=300')

  const config = useRuntimeConfig()
  const base = (config.backendApiBase as string) || 'http://localhost:5234/api'

  // ── 并发拉取三个来源 ──────────────────────────────────────────
  const [rawProjects, rawArticlesRes, rawTimeline] = await Promise.allSettled([
    $fetch<{ code: number; data: any[] }>(`${base}/Projects`, { timeout: 5000 }),
    $fetch<{ code: number; data: { List: any[]; Total: number } }>(
      `${base}/Articles`,
      { query: { status: 1, pageSize: 50, page: 1 }, timeout: 5000 }
    ),
    $fetch<{ code: number; data: any[] }>(`${base}/Timeline`, { timeout: 5000 }),
  ])

  const projects: any[] = rawProjects.status === 'fulfilled'
    ? (Array.isArray(rawProjects.value?.data) ? rawProjects.value.data : [])
    : []

  const articles: any[] = rawArticlesRes.status === 'fulfilled'
    ? (rawArticlesRes.value?.data?.List ?? [])
    : []

  const timeline: any[] = rawTimeline.status === 'fulfilled'
    ? (Array.isArray(rawTimeline.value?.data) ? rawTimeline.value.data : [])
    : []

  const toolsCount = readMarkdownCollection('tools').length

  // ── 处理 Projects ─────────────────────────────────────────────
  const allProjects: HomeProjectCard[] = projects.map((p: any) => ({
    id: String(p.Id ?? p.id ?? ''),
    title: String(p.Title ?? p.title ?? ''),
    description: String(p.Description ?? p.description ?? ''),
    techStack: parseTechStack(p.TechStack ?? p.techStack),
    coverUrl: p.CoverUrl ?? p.coverUrl ?? null,
    demoUrl: p.DemoUrl ?? p.demoUrl ?? null,
    githubUrl: p.GithubUrl ?? p.githubUrl ?? null,
    status: String(p.Status ?? p.status ?? 'Active'),
    viewCount: Number(p.ViewCount ?? p.viewCount ?? 0),
  }))

  const featuredProjects: HomeProjectCard[] = [...allProjects]
    .filter(p => p.status !== 'Archived')
    .sort((a, b) => b.viewCount - a.viewCount)
    .slice(0, 5)

  // nowBuilding: Active 项目，UpdatedAt DESC 排序，取前 4
  const nowBuilding: HomeNowBuildingItem[] = projects
    .filter((p: any) => String(p.Status ?? p.status) === 'Active')
    .sort((a: any, b: any) => {
      const aT = new Date(a.UpdatedAt ?? a.updatedAt ?? 0).getTime()
      const bT = new Date(b.UpdatedAt ?? b.updatedAt ?? 0).getTime()
      return bT - aT
    })
    .slice(0, 4)
    .map((p: any) => {
      const title = String(p.Title ?? p.title ?? '')
      return {
        id: String(p.Id ?? p.id ?? ''),
        title,
        description: String(p.Description ?? p.description ?? ''),
        techStack: parseTechStack(p.TechStack ?? p.techStack),
        status: String(p.Status ?? p.status ?? 'Active'),
        progress: PROJECT_PROGRESS[title] ?? DEFAULT_PROGRESS,
      }
    })

  // ── 处理 Articles ─────────────────────────────────────────────
  const allArticles: HomeArticleCard[] = articles.map((a: any) => {
    const id = Number(a.Id ?? a.id ?? 0)
    const slug = a.Slug ?? a.slug ?? null
    return {
      id,
      title: String(a.Title ?? a.title ?? ''),
      slug,
      summary: a.Summary ?? a.summary ?? null,
      coverUrl: a.CoverUrl ?? a.coverUrl ?? null,
      publishTime: a.PublishTime ?? a.publishTime ?? a.CreatedAt ?? a.createdAt ?? null,
      viewCount: Number(a.ViewCount ?? a.viewCount ?? 0),
      categoryName: a.CategoryName ?? a.categoryName ?? null,
      path: `/blog/${slug ?? id}`,
    }
  })

  const featuredArticle: HomeArticleCard | null = allArticles[0] ?? null
  // 按 id 排除精选文章（不用数组下标）
  const latestArticles: HomeArticleCard[] = allArticles
    .filter(a => a.id !== featuredArticle?.id)
    .slice(0, 4)

  // ── 处理 Timeline ─────────────────────────────────────────────
  const currentYear = new Date().getFullYear()
  const journey: HomeJourneyItem[] = timeline.map((t: any) => ({
    id: Number(t.Id ?? t.id ?? 0),
    year: Number(t.Year ?? t.year ?? 0),
    title: String(t.Title ?? t.title ?? ''),
    description: t.Description ?? t.description ?? null,
    icon: t.Icon ?? t.icon ?? null,
    color: t.Color ?? t.color ?? null,
    isNow: Number(t.Year ?? t.year) === currentYear,
  }))

  // ── Stats ─────────────────────────────────────────────────────
  const stats: HomeStats = {
    projects: allProjects.filter(p => p.status !== 'Archived').length,
    articles: allArticles.length,
    tools: toolsCount,
  }

  return {
    stats,
    featuredProjects,
    featuredArticle,
    latestArticles,
    nowBuilding,
    journey,
  } satisfies HomeOverview
})
```

- [ ] **Step 3: 启动开发服务器，验证接口**

```bash
npm run dev
```

访问 `http://localhost:3000/api/home/overview`，确认返回 JSON 结构包含：
`{ stats: { projects, articles, tools }, featuredProjects: [...], featuredArticle, latestArticles: [...], nowBuilding: [...], journey: [...] }`

若 .NET 后端未启动，各数组应为空，stats 全为 0（tools 可能有值）。

- [ ] **Step 4: Commit**

```bash
git add nuxt.config.ts server/api/home/overview.get.ts
git commit -m "feat(home): add /api/home/overview aggregation endpoint"
```

---

## Task 5: Composable + FALLBACK

**Files:**
- Create: `composables/useHomeOverview.ts`

- [ ] **Step 1: 创建 composable**

```typescript
// composables/useHomeOverview.ts
import type { HomeOverview } from '~/types/home'

const FALLBACK: HomeOverview = {
  stats: { projects: 20, articles: 50, tools: 8 },
  featuredProjects: [],
  featuredArticle: null,
  latestArticles: [],
  nowBuilding: [],
  journey: [
    {
      id: 0,
      year: 2022,
      title: '探索开始',
      description: '接触编程与自动化，开启数字世界的探索之旅。',
      icon: 'icon-terminal',
      color: 'blue',
      isNow: false,
    },
    {
      id: 1,
      year: 2024,
      title: '产品探索',
      description: '从想法到原型，开始构建自己的产品。',
      icon: 'icon-cube',
      color: 'purple',
      isNow: false,
    },
    {
      id: 2,
      year: new Date().getFullYear(),
      title: '系统构建',
      description: '专注产品化与系统化，打造可复用的解决方案。',
      icon: 'icon-layers',
      color: 'blue',
      isNow: true,
    },
  ],
}

export const useHomeOverview = () => {
  const { data, pending, error } = useLazyAsyncData(
    'home-overview',
    () => $fetch<HomeOverview>('/api/home/overview'),
    { server: false }  // 首屏用 FALLBACK 快速渲染，数据到了自动替换
  )

  const overview = computed<HomeOverview>(() => data.value ?? FALLBACK)
  const loading = computed(() => pending.value)

  return { overview, loading, error }
}
```

- [ ] **Step 2: Commit**

```bash
git add composables/useHomeOverview.ts
git commit -m "feat(home): add useHomeOverview composable with fallback"
```

---

## Task 6: pages/index.vue 接入 composable

**Files:**
- Modify: `pages/index.vue`

- [ ] **Step 1: 替换 script setup 中的导入**

找到 `pages/index.vue` 的 `<script setup lang="ts">` 块（约第 75 行），在 `import '~/assets/css/home.css'` 后添加 composable 调用，并删除 `activeNav = ref('home')` 之前的任何旧统计/数据代码（当前文件无此代码，只需添加）：

```typescript
import '~/assets/css/home.css'

definePageMeta({
  layout: false
})

const { toggleDark } = useTheme()
const { overview, loading } = useHomeOverview()  // ← 新增

const activeNav = ref('home')
// ...其余导航逻辑不变
```

- [ ] **Step 2: 替换 `<main>` 内的组件调用**

找到当前 `<main>` 块（约第 42-49 行），替换为带 props 的版本：

```html
<main>
  <HomeHero
    :stats="overview.stats"
    :loading="loading"
  />
  <LazyHomeProducts
    :projects="overview.featuredProjects"
    :loading="loading"
  />
  <LazyHomeTimeline
    :featured-article="overview.featuredArticle"
    :articles="overview.latestArticles"
    :loading="loading"
  />
  <LazyHomeAbout
    :journey="overview.journey"
    :loading="loading"
  />
  <LazyHomeCTA />
  <LazyHomeBuilding
    :projects="overview.nowBuilding"
    :loading="loading"
  />
</main>
```

- [ ] **Step 3: Commit**

```bash
git add pages/index.vue
git commit -m "feat(home): wire useHomeOverview into index.vue"
```

---

## Task 7: HomeHero.vue — 动态统计数字

**Files:**
- Modify: `components/home/HomeHero.vue`

- [ ] **Step 1: 添加 props 并替换硬编码 stats**

找到 `<script setup lang="ts">` 块（约第 100 行）。在文件顶部（`<script setup>` 内）添加 props 定义，并用 computed 替换原有 `stats` 常量：

```typescript
// 替换原 <script setup lang="ts"> 内容（保留 featureCards 不变）
import type { HomeStats } from '~/types/home'

const props = withDefaults(defineProps<{
  stats: HomeStats
  loading: boolean
}>(), {
  stats: () => ({ projects: 20, articles: 50, tools: 8 }),
  loading: false,
})

// 替换原来硬编码的 stats 数组
const displayStats = computed(() => [
  { value: props.loading ? '—' : `${props.stats.projects}+`, label: '产品与项目' },
  { value: props.loading ? '—' : `${props.stats.articles}+`, label: '文章分享' },
  { value: props.loading ? '—' : `${props.stats.tools}+`, label: '工具数量' },
  { value: '∞', label: '探索可能' },
])

// featureCards 数组保持不变（静态内容）
const featureCards = [ /* ... 保持原有内容 ... */ ]
```

- [ ] **Step 2: 替换模板中的 `stats` 为 `displayStats`**

找到模板中（约第 76 行）：

```html
<!-- 改前 -->
<div v-for="stat in stats" :key="stat.label" class="home-stat">

<!-- 改后 -->
<div v-for="stat in displayStats" :key="stat.label" class="home-stat">
```

- [ ] **Step 3: Commit**

```bash
git add components/home/HomeHero.vue
git commit -m "feat(home): HomeHero accepts dynamic stats props"
```

---

## Task 8: HomeProducts.vue — 真实项目数据

**Files:**
- Modify: `components/home/HomeProducts.vue`

- [ ] **Step 1: 添加 props，替换 products 数据**

找到 `<script setup lang="ts">` 块（约第 90 行），在原有代码之前插入 props 定义和辅助函数，**删除**原 `products` 常量数组：

```typescript
import type { HomeProjectCard } from '~/types/home'

const props = withDefaults(defineProps<{
  projects: HomeProjectCard[]
  loading: boolean
}>(), {
  projects: () => [],
  loading: false,
})

// 根据 tech stack 推导装饰 art class
const getProductArt = (project: HomeProjectCard): string => {
  const tech = project.techStack.join(' ').toLowerCase()
  const title = project.title.toLowerCase()
  if (tech.includes('ai') || title.includes('ai') || title.includes('智能')) return 'product-art-ai'
  if (title.includes('联谊') || title.includes('社交') || title.includes('心动')) return 'product-art-heart'
  if (tech.includes('.net') || tech.includes('python') || tech.includes('fastapi')) return 'product-art-code'
  if (tech.includes('vue') || tech.includes('nuxt') || tech.includes('react')) return 'product-art-dashboard'
  return 'product-art-cube'
}

// labItems 和 stats 保持不变（静态内容）
```

- [ ] **Step 2: 替换模板中产品行的渲染**

找到模板中（约第 16-26 行）：

```html
<!-- 改前 -->
<div class="products-row reveal-up reveal-delay-1" aria-label="产品与项目列表">
  <NuxtLink v-for="product in products" :key="product.title" :to="product.href" class="product-show-card">
    <span class="product-art" :class="product.art" aria-hidden="true"></span>
    <strong>{{ product.title }}</strong>
    <span class="product-desc">{{ product.description }}</span>
    <span class="product-tags">
      <em v-for="tag in product.tags" :key="tag">{{ tag }}</em>
    </span>
    <span class="product-arrow" aria-hidden="true">→</span>
  </NuxtLink>
</div>

<!-- 改后 -->
<div class="products-row reveal-up reveal-delay-1" aria-label="产品与项目列表">
  <!-- loading 骨架 -->
  <template v-if="loading">
    <span v-for="n in 5" :key="n" class="product-show-card product-show-card--skeleton"></span>
  </template>
  <!-- 真实数据 -->
  <template v-else-if="projects.length > 0">
    <NuxtLink
      v-for="project in projects"
      :key="project.id"
      :to="project.demoUrl || '/projects'"
      class="product-show-card"
    >
      <span class="product-art" :class="getProductArt(project)" aria-hidden="true"></span>
      <strong>{{ project.title }}</strong>
      <span class="product-desc">{{ project.description }}</span>
      <span class="product-tags">
        <em v-for="tag in project.techStack.slice(0, 2)" :key="tag">{{ tag }}</em>
      </span>
      <span class="product-arrow" aria-hidden="true">→</span>
    </NuxtLink>
  </template>
</div>
```

- [ ] **Step 3: 为骨架卡片添加最小样式**

在 `<style scoped>` 末尾添加：

```css
.product-show-card--skeleton {
  min-height: 14rem;
  background: rgba(255, 255, 255, 0.04);
  border-radius: var(--radius-lg);
  animation: pulse 1.5s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}
```

- [ ] **Step 4: Commit**

```bash
git add components/home/HomeProducts.vue
git commit -m "feat(home): HomeProducts renders real project data from props"
```

---

## Task 9: HomeTimeline.vue — 真实文章数据

**Files:**
- Modify: `components/home/HomeTimeline.vue`

- [ ] **Step 1: 添加 props 和辅助函数，删除硬编码数据**

在 `<script setup lang="ts">` 块（约第 116 行），用以下内容**替换** `topArticles` 和 `notes` 常量（保留 `filters` 不变）：

```typescript
import type { HomeArticleCard } from '~/types/home'

const props = withDefaults(defineProps<{
  featuredArticle: HomeArticleCard | null
  articles: HomeArticleCard[]
  loading: boolean
}>(), {
  featuredArticle: null,
  articles: () => [],
  loading: false,
})

// 相对时间格式化
const formatRelativeTime = (dateStr: string | null): string => {
  if (!dateStr) return ''
  const diffDays = Math.floor((Date.now() - new Date(dateStr).getTime()) / 86400000)
  if (diffDays === 0) return '今天'
  if (diffDays < 30) return `${diffDays} 天前`
  if (diffDays < 365) return `${Math.floor(diffDays / 30)} 个月前`
  return `${Math.floor(diffDays / 365)} 年前`
}

// 短日期格式 MM.DD
const formatShortDate = (dateStr: string | null): string => {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return `${String(d.getMonth() + 1).padStart(2, '0')}.${String(d.getDate()).padStart(2, '0')}`
}

// 按索引轮换 art / icon 类（无需分类映射）
const TOP_ARTS = ['top-art-planet', 'top-art-chart', 'top-art-path', 'top-art-en']
const NOTE_ICONS = ['note-icon-agent', 'note-icon-code', 'note-icon-book', 'note-icon-light']
const getTopArt = (index: number) => TOP_ARTS[index % TOP_ARTS.length]
const getNoteIcon = (index: number) => NOTE_ICONS[index % NOTE_ICONS.length]

// filters 保持不变
const filters = [ /* ... 保持原有内容 ... */ ]
```

- [ ] **Step 2: 替换模板中 featured-article 区域**

找到 `<article class="featured-article">` 块（约第 42-64 行），替换为：

```html
<!-- 精选文章 -->
<article class="featured-article">
  <span class="featured-badge">
    <span class="icon-star" aria-hidden="true"></span>
    精选文章
  </span>
  <div class="featured-orbit" aria-hidden="true">
    <span class="featured-cube"></span>
  </div>
  <template v-if="featuredArticle">
    <div class="featured-copy">
      <p>
        <span>{{ featuredArticle.categoryName || 'AI' }}</span>
        · {{ formatRelativeTime(featuredArticle.publishTime) }}
      </p>
      <h3>{{ featuredArticle.title }}</h3>
      <p>{{ featuredArticle.summary || '点击查看完整内容。' }}</p>
    </div>
    <div class="featured-meta">
      <span><span class="icon-eye" aria-hidden="true"></span>{{ featuredArticle.viewCount }}</span>
    </div>
    <NuxtLink :to="featuredArticle.path" class="featured-link">
      阅读文章
      <span aria-hidden="true">→</span>
    </NuxtLink>
  </template>
  <template v-else>
    <div class="featured-copy">
      <p><span>AI</span> · 持续更新中</p>
      <h3>探索 AI 与技术的边界</h3>
      <p>更多精选文章即将发布，敬请期待。</p>
    </div>
    <NuxtLink to="/blog" class="featured-link">
      浏览全部文章
      <span aria-hidden="true">→</span>
    </NuxtLink>
  </template>
  <span class="featured-next" aria-hidden="true">›</span>
</article>
```

- [ ] **Step 3: 替换 top-article-list 区域**

找到 `<aside class="top-article-list">` 块（约第 66-78 行），替换为：

```html
<aside class="top-article-list" aria-label="热门文章">
  <NuxtLink
    v-for="(article, index) in articles.slice(0, 4)"
    :key="article.id"
    :to="article.path"
    class="top-article-item"
  >
    <span class="top-art" :class="getTopArt(index)" aria-hidden="true"></span>
    <span class="top-copy">
      <em>{{ article.categoryName || '文章' }} · {{ formatRelativeTime(article.publishTime) }}</em>
      <strong>{{ article.title }}</strong>
    </span>
    <span class="top-stats">
      <span><span class="icon-eye" aria-hidden="true"></span>{{ article.viewCount }}</span>
    </span>
  </NuxtLink>
</aside>
```

- [ ] **Step 4: 替换 latest-notes 区域**

找到 `<div class="latest-notes">` 块（约第 86-101 行），替换为：

```html
<div class="latest-notes reveal-up reveal-delay-3">
  <NuxtLink
    v-for="(article, index) in articles"
    :key="article.id"
    :to="article.path"
    class="note-card"
  >
    <span class="note-icon" :class="getNoteIcon(index)" aria-hidden="true"></span>
    <time>{{ formatShortDate(article.publishTime) }}</time>
    <strong>{{ article.title }}</strong>
    <p>{{ article.summary || '点击查看完整内容。' }}</p>
    <span class="note-meta">
      <span><span class="icon-eye" aria-hidden="true"></span>{{ article.viewCount }}</span>
      <span aria-hidden="true">→</span>
    </span>
  </NuxtLink>

  <button class="note-next" type="button" aria-label="下一组笔记">
    <span aria-hidden="true">›</span>
  </button>
</div>
```

- [ ] **Step 5: Commit**

```bash
git add components/home/HomeTimeline.vue
git commit -m "feat(home): HomeTimeline renders real article data from props"
```

---

## Task 10: HomeAbout.vue — 真实 Journey 数据

**Files:**
- Modify: `components/home/HomeAbout.vue`

- [ ] **Step 1: 添加 props，用计算属性替换硬编码 timeline**

在 `<script setup lang="ts">` 块（约第 65 行），在原有 `timeline` 常量之前添加 props，**删除** `timeline` 常量，改为从 props 读取（保留 `stats` 常量不变）：

```typescript
import type { HomeJourneyItem } from '~/types/home'

const props = withDefaults(defineProps<{
  journey: HomeJourneyItem[]
  loading: boolean
}>(), {
  journey: () => [],
  loading: false,
})

// 当 API 返回空时的静态兜底（3 个最有代表性的节点）
const FALLBACK_TIMELINE = [
  { id: 0, year: 2022, title: '探索开始', description: '接触编程与自动化，开启数字世界的探索之旅。', icon: 'icon-terminal', color: 'blue', isNow: false },
  { id: 1, year: 2024, title: '产品探索', description: '从想法到原型，开始构建自己的产品。', icon: 'icon-cube', color: 'blue', isNow: false },
  { id: 2, year: 2025, title: '系统构建', description: '专注产品化与系统化，打造可复用的解决方案。', icon: 'icon-layers', color: 'purple', isNow: true },
]

// API 有数据用 API 数据，否则用静态兜底
const displayTimeline = computed(() =>
  props.journey.length > 0 ? props.journey : FALLBACK_TIMELINE
)

// dot 颜色：优先用 API 返回的 color 字段，无则按奇偶交替
const getDotTheme = (item: HomeJourneyItem | typeof FALLBACK_TIMELINE[number], index: number): string => {
  if ('color' in item && item.color) return `dot-${item.color}`
  return index % 2 === 0 ? 'dot-blue' : 'dot-purple'
}

// stats 常量保持不变
const stats = [ /* ... 保持原有内容 ... */ ]
```

- [ ] **Step 2: 替换模板中 journey-timeline 的循环**

找到（约第 26 行）：

```html
<!-- 改前 -->
<article v-for="item in timeline" :key="item.year" class="journey-step" :class="{ 'is-current': item.now }">
  ...
  <span class="journey-dot" :class="item.theme" aria-hidden="true"></span>
  <div class="journey-card">
    <span class="journey-icon" :class="item.icon" aria-hidden="true"></span>
    <ul>
      <li v-for="point in item.points" :key="point">{{ point }}</li>
    </ul>
  </div>

<!-- 改后 -->
<article
  v-for="(item, index) in displayTimeline"
  :key="item.id ?? item.year"
  class="journey-step"
  :class="{ 'is-current': item.isNow }"
>
  <span v-if="item.isNow" class="journey-now">NOW</span>
  <div class="journey-step-copy">
    <strong>{{ item.year }}</strong>
    <h3>{{ item.title }}</h3>
    <p>{{ item.description }}</p>
  </div>
  <span class="journey-dot" :class="getDotTheme(item, index)" aria-hidden="true"></span>
  <div class="journey-card">
    <span class="journey-icon" :class="item.icon || 'icon-cube'" aria-hidden="true"></span>
  </div>
```

- [ ] **Step 3: Commit**

```bash
git add components/home/HomeAbout.vue
git commit -m "feat(home): HomeAbout renders real journey data from props"
```

---

## Task 11: HomeCTA.vue — 替换为常量

**Files:**
- Modify: `components/home/HomeCTA.vue`

- [ ] **Step 1: 替换 script setup 中的硬编码数据**

找到 `<script setup lang="ts">` 块（约第 89 行），**删除** `methods` 和 `collaborations` 常量，改为 import（保留 `partners` 不变）：

```typescript
import { CONTACT_METHODS, COLLAB_DIRECTIONS } from '~/constants/homeContact'

// 将模板中原来引用 methods 的地方改为 CONTACT_METHODS
// 将模板中原来引用 collaborations 的地方改为 COLLAB_DIRECTIONS

// partners 保持不变
const partners = [ /* ... 保持原有内容 ... */ ]
```

- [ ] **Step 2: 更新模板中的变量名**

找到模板中两处变量引用：

```html
<!-- 改前 -->
<article v-for="method in methods" :key="method.title" class="contact-card">
<!-- 改后 -->
<article v-for="method in CONTACT_METHODS" :key="method.title" class="contact-card">

<!-- 改前 -->
<NuxtLink v-for="item in collaborations" :key="item.title" to="/contact" class="collab-item">
<!-- 改后 -->
<NuxtLink v-for="item in COLLAB_DIRECTIONS" :key="item.title" to="/contact" class="collab-item">
```

- [ ] **Step 3: Commit**

```bash
git add components/home/HomeCTA.vue constants/homeContact.ts
git commit -m "feat(home): HomeCTA uses homeContact constants instead of hardcoded data"
```

---

## Task 12: HomeBuilding.vue — 真实 nowBuilding 数据

**Files:**
- Modify: `components/home/HomeBuilding.vue`

- [ ] **Step 1: 添加 props 和辅助函数，删除硬编码 projects 数据**

在 `<script setup lang="ts">` 块（约第 118 行），**删除** `projects`、`today`、`weekStats`、`goals`、`keywords` 常量，改为 props（保留 `today`、`weekStats`、`goals`、`keywords` 作为静态内容，仅 `projects` 改为 props）：

```typescript
import type { HomeNowBuildingItem } from '~/types/home'

const props = withDefaults(defineProps<{
  projects: HomeNowBuildingItem[]
  loading: boolean
}>(), {
  projects: () => [],
  loading: false,
})

// status → 展示文案
const getStatusText = (status: string): string => {
  const s = status.toLowerCase()
  if (s.includes('active')) return '进行中'
  if (s.includes('complete')) return '已上线'
  if (s.includes('plan')) return '规划中'
  return '进行中'
}

// status → badge 颜色 class
const getStatusTheme = (status: string): string => {
  const s = status.toLowerCase()
  if (s.includes('active')) return 'status-blue'
  if (s.includes('complete')) return 'status-teal'
  return 'status-indigo'
}

// 按索引轮换装饰 art class
const PROJECT_ARTS = ['art-match', 'art-workflow', 'art-pet', 'art-knowledge']
const getArtClass = (index: number) => PROJECT_ARTS[index % PROJECT_ARTS.length]

// 以下静态内容保持不变（今日进展、本周数据等预留结构）
const today = [ /* ... 保持原有内容 ... */ ]
const weekStats = [ /* ... 保持原有内容 ... */ ]
const goals = [ /* ... 保持原有内容 ... */ ]
const keywords = [ /* ... 保持原有内容 ... */ ]
```

- [ ] **Step 2: 替换模板中 project-grid 区域**

找到（约第 21-39 行）：

```html
<!-- 改前 -->
<div class="project-grid">
  <article v-for="project in projects" :key="project.title" class="project-card">
    <span class="project-status" :class="project.statusTheme">
      <i aria-hidden="true"></i>
      {{ project.status }}
    </span>
    <span class="project-art" :class="project.art" aria-hidden="true"></span>
    <div class="project-progress-head">
      <h3>{{ project.title }}</h3>
      <strong>{{ project.progress }}%</strong>
    </div>
    <span class="project-progress-label">开发进度</span>
    <span class="project-progress">
      <i :style="{ width: `${project.progress}%` }"></i>
    </span>
    <p>{{ project.description }}</p>
    <div class="project-tags">
      <span v-for="tag in project.tags" :key="tag">{{ tag }}</span>
    </div>
  </article>
</div>

<!-- 改后 -->
<div class="project-grid">
  <!-- loading 骨架 -->
  <template v-if="loading">
    <article v-for="n in 4" :key="n" class="project-card project-card--skeleton"></article>
  </template>
  <!-- 真实数据 -->
  <template v-else-if="projects.length > 0">
    <article v-for="(project, index) in projects" :key="project.id" class="project-card">
      <span class="project-status" :class="getStatusTheme(project.status)">
        <i aria-hidden="true"></i>
        {{ getStatusText(project.status) }}
      </span>
      <span class="project-art" :class="getArtClass(index)" aria-hidden="true"></span>
      <div class="project-progress-head">
        <h3>{{ project.title }}</h3>
        <strong>{{ project.progress }}%</strong>
      </div>
      <span class="project-progress-label">开发进度</span>
      <span class="project-progress">
        <i :style="{ width: `${project.progress}%` }"></i>
      </span>
      <p>{{ project.description }}</p>
      <div class="project-tags">
        <span v-for="tag in project.techStack.slice(0, 3)" :key="tag">{{ tag }}</span>
      </div>
    </article>
  </template>
  <!-- 空状态：保持视觉不崩，显示占位提示 -->
  <template v-else>
    <article v-for="n in 4" :key="n" class="project-card project-card--skeleton"></article>
  </template>
</div>
```

- [ ] **Step 3: 添加骨架样式**

在 `<style scoped>` 末尾（`.building-showcase` 样式块之后）添加：

```css
.project-card--skeleton {
  background: rgba(255, 255, 255, 0.03) !important;
  box-shadow: none !important;
  animation: building-pulse 1.5s ease-in-out infinite;
}

@keyframes building-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}
```

- [ ] **Step 4: Commit**

```bash
git add components/home/HomeBuilding.vue
git commit -m "feat(home): HomeBuilding renders real nowBuilding data from props"
```

---

## Task 13: 构建验证

- [ ] **Step 1: 运行构建**

```bash
npm run build
```

Expected: 无 TypeScript 类型错误，无构建失败。常见错误：
- `Property 'X' does not exist on type 'HomeJourneyItem'` → 检查 `displayTimeline` computed 的类型
- `No overload matches` → 检查 props 默认值是否满足类型
- `Cannot find module '~/types/home'` → 确认 `types/home.ts` 已创建

- [ ] **Step 2: 开发服务器验证首页**

```bash
npm run dev
```

访问 `http://localhost:3000`，逐屏检查：
- 第 1 屏：统计数字是真实数值（或 FALLBACK 值 20/50/8），不是硬编码的 '20+'
- 第 2 屏：产品卡片来自数据库，切换后台项目可看到变化
- 第 3 屏：精选文章标题 + 摘要来自数据库
- 第 4 屏：Journey 时间线来自 `timeline_event` 表（或 FALLBACK）
- 第 5 屏：邮箱显示 `linxiwanting@gmail.com`
- 第 6 屏：进行中项目 + 对应进度条

- [ ] **Step 3: 检查 loading 状态**

在浏览器 DevTools → Network → 将网速调慢至 Slow 3G，刷新首页，确认：
- 第 1 屏统计数字短暂显示 `—`
- 第 2 屏出现骨架卡片
- 第 6 屏出现骨架占位

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat(home): homepage dynamic data integration complete"
```

---

## Task 14: Fallback 验证

- [ ] **Step 1: 模拟后端不可用**

临时修改 `nuxt.config.ts` 中 `backendApiBase` 指向一个不存在的地址：

```typescript
backendApiBase: 'http://localhost:9999/api',  // 临时改为无效地址
```

- [ ] **Step 2: 启动开发服务器，访问首页**

```bash
npm run dev
```

访问 `http://localhost:3000`，确认：
- 首页正常渲染，不崩溃，不显示白屏
- 第 1 屏显示 FALLBACK stats（20/50/8）
- 第 4 屏显示 FALLBACK journey（3 个静态节点）
- 第 2/3/6 屏显示空状态（骨架占位），不报错

浏览器 Console 应看到 $fetch 超时错误日志（这是预期行为，Nitro 层静默处理了）。

- [ ] **Step 3: 还原配置**

```typescript
backendApiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5234/api',
```

- [ ] **Step 4: 最终 Commit**

```bash
git add nuxt.config.ts
git commit -m "test(home): verify fallback behavior, restore config"
```

---

## 自检清单（Spec → Plan 对照）

| 需求 | 对应 Task |
|---|---|
| stats 从 Projects/Articles/Tools 聚合 | Task 4（overview API stats 块） |
| featuredProjects ViewCount DESC，排除 Archived | Task 4（allProjects 过滤排序） |
| nowBuilding UpdatedAt DESC 排序 | Task 4（nowBuilding 排序块） |
| progress 精确匹配 title，匹配不上给 DEFAULT_PROGRESS | Task 3 + Task 4 |
| featuredArticle 按 id 排除，不用下标 | Task 4（latestArticles filter） |
| journey isNow = (year === currentYear) | Task 4（currentYear 逻辑） |
| loading 骨架屏（各组件） | Task 7–12 |
| 接口失败 FALLBACK 兜底 | Task 5 + Task 14 |
| HomeCTA 直接 import 常量，无需父组件传参 | Task 11 |
| `npm run build` 通过 | Task 13 |
