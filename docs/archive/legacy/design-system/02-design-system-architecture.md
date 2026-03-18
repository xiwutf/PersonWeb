# PersonWeb Design System 最终结构方案

**版本**: v1.0
**创建日期**: 2026-03-14
**适用项目**: PersonWeb (个人网站 + 后台管理系统)

---

## 一、设计系统目标

### 1.1 核心目标

建立一套适合 PersonWeb 项目的 Design System，解决以下问题：

| 问题 | 当前状态 | Design System 解决方案 |
|------|---------|---------------------|
| 样式不一致 | 后台页面重复代码多，风格不统一 | 统一 Admin Pattern + CSS 变量系统 |
| 开发效率低 | 新页面重复编写相似结构 | 提供页面模板和 Pattern 组件 |
| 维护困难 | 样式修改需要改多处 | 集中管理 Token 和组件样式 |
| 主题切换不全 | 部分硬编码样式不响应主题 | 通过 CSS 变量统一主题 |
| 知识传递困难 | 缺少文档，新人不知道用什么 | 建立完整的设计系统文档 |

### 1.2 设计原则

| 原则 | 说明 |
|------|------|
| **分治原则** | 前台与后台采用不同策略，前台保持灵活性，后台追求一致性 |
| **渐进式** | 不重写现有代码，通过抽象 Pattern 逐步提升 |
| **兼容性** | 不引入新的 UI 框架，基于现有技术栈 (Naive UI + Tailwind CSS) |
| **实用性** | 优先解决高频重复场景，不追求完美抽象 |
| **可维护** | 减少 `!important`，建立清晰的优先级规范 |

---

## 二、系统边界

### 2.1 包含范围

✅ **包含**:

| 层级 | 内容 |
|------|------|
| **Token 体系** | 颜色、间距、圆角、阴影、字体等基础变量 |
| **Semantic 变量** | 语义化文字、背景、边框颜色变量 |
| **Admin Pattern** | 后台页面结构模板 (ListPage, FormPage 等) |
| **Web Pattern** | 前台 Section 组件 (Hero, FeatureGrid 等) |
| **Naive UI Theme 统一** | 统一 Naive UI 组件的主题配置 |
| **样式治理规则** | CSS 优先级规范、变量使用规范 |

### 2.2 不包含范围

❌ **不包含**:

| 内容 | 原因 |
|------|------|
| 重写完整组件库 | Naive UI 已提供完整组件，无需重写 |
| 新建独立 UI 框架 | 增加不必要的复杂度 |
| 前台风格强统一 | 前台风格多样是设计意图，需保持灵活性 |
| 替换 Tailwind CSS | Tailwind CSS 已深度集成，替换成本高 |

---

## 三、Design System 分层结构

```
┌─────────────────────────────────────────────────────────┐
│                  Layer 4: Application                   │
│  ┌─────────────┐         ┌─────────────┐           │
│  │ Admin Pages  │         │  Web Pages  │           │
│  │ (使用 Pattern)│       │  (使用 Section)│         │
│  └─────────────┘         └─────────────┘           │
└─────────────────────────────────────────────────────────┘
                           │ 使用
┌─────────────────────────────────────────────────────────┐
│                   Layer 3: Components                 │
│  ┌─────────────┐  ┌─────────────┐  ┌──────────┐  │
│  │Naive UI 组件│  │ App 组件     │  │Pattern 组件││
│  │(Table,Form) │  │(Card,Button)│  │(ListPage) │  │
│  └─────────────┘  └─────────────┘  └──────────┘  │
└─────────────────────────────────────────────────────────┘
                           │ 使用
┌─────────────────────────────────────────────────────────┐
│                  Layer 2: Semantic                    │
│  ┌─────────────┐  ┌─────────────┐  ┌──────────┐   │
│  │  文字颜色   │  │  背景颜色   │  │ 边框颜色 │   │
│  │--color-text│  │--color-bg-  │  │--color-  │   │
│  │    -main    │  │    card     │  │  border  │   │
│  └─────────────┘  └─────────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────┘
                           │ 映射自
┌─────────────────────────────────────────────────────────┐
│                 Layer 1: Foundation                   │
│  ┌─────────────┐  ┌─────────────┐  ┌──────────┐   │
│  │  颜色阶梯   │  │  间距系统   │  │ 字体系统 │   │
│  │--color-blue│  │--spacing-4  │  │--font-   │   │
│  │    -500     │  │             │  │  size-h1 │   │
│  └─────────────┘  └─────────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────┘
```

### 3.1 Layer 1: Foundation (基础层)

**文件位置**: `assets/styles/theme-variables.css`

| 类别 | 变量前缀 | 示例 | 说明 |
|------|---------|------|------|
| **颜色阶梯** | `--color-{hue}-{level}` | `--color-blue-500` | 完整的颜色系统，支持 50-950 |
| **间距** | `--spacing-{value}` | `--spacing-4` (16px) | 4px 基础单位的倍数 |
| **圆角** | `--radius-{size}` | `--radius-md` (8px) | 统一圆角规范 |
| **阴影** | `--shadow-{size}` | `--shadow-md` | 层级阴影系统 |
| **字体** | `--font-{family/size/weight}` | `--font-size-h1` | 字体系统 |

**状态**: ✅ 已完善，无需改动

### 3.2 Layer 2: Semantic (语义层)

**文件位置**: `assets/styles/theme-variables.css`

| 类别 | 变量前缀 | 示例 | 说明 |
|------|---------|------|------|
| **文字颜色** | `--color-text-{level}` | `--color-text-main`, `--color-text-muted` | 按重要程度分级 |
| **背景颜色** | `--color-bg-{type}` | `--color-bg-body`, `--color-bg-card` | 按层级分级 |
| **边框颜色** | `--color-border-{level}` | `--color-border-default`, `--color-border-strong` | 按视觉强度分级 |
| **状态颜色** | `--color-{status}-{type}` | `--color-success-bg`, `--color-error-text` | 成功/警告/错误/信息 |
| **组件变量** | `--{component}-{state}-{prop}` | `--btn-bg-primary`, `--input-border-focus` | 组件级变量 |

**状态**: ✅ 已完善，无需改动

### 3.3 Layer 3: Components (组件层)

#### 3.3.1 Naive UI 组件层

**文件位置**: `components/layout/AppNaiveConfig.vue`

```typescript
// Naive UI ThemeOverrides 配置
const themeOverrides = {
  common: {
    primaryColor: 'var(--color-primary)',
    primaryColorHover: 'var(--color-primary-hover)',
    textColor1: 'var(--color-text-main)',
    textColor2: 'var(--color-text-secondary)',
    textColor3: 'var(--color-text-muted)',
    borderColor: 'var(--color-border-default)',
  },
  Layout: {
    color: 'var(--color-bg-body)',
    siderColor: 'var(--color-bg-elevated)',
  },
  Card: {
    color: 'var(--color-bg-card)',
    colorModal: 'var(--color-bg-card)',
    borderColor: 'var(--color-border-subtle)',
  },
  Input: {
    color: 'var(--color-bg-input)',
    borderColor: 'var(--color-border-default)',
    borderFocus: 'var(--color-border-focus)',
    placeholderColor: 'var(--color-text-quaternary)',
  },
  Button: {
    textColor: 'var(--color-text-on-primary)',
    textColorHover: 'var(--color-text-on-primary)',
    textColorPressed: 'var(--color-text-on-primary)',
  },
  DataTable: {
    thColor: 'var(--color-bg-elevated)',
    thTextColor: 'var(--color-text-main)',
    tdColor: 'transparent',
    tdTextColor: 'var(--color-text-main)',
    tdColorHover: 'var(--color-bg-elevated)',
    borderColor: 'var(--color-border-subtle)',
  },
  // ... 更多组件配置
}
```

**状态**: 🟡 部分完善，需要补全缺失的组件配置

#### 3.3.2 App 组件层

**目录结构**:
```
components/
├── ui/                    # 基础 UI 组件
│   ├── AppCard.vue         # 卡片组件
│   ├── AppButton.vue       # 按钮组件
│   ├── AppInput.vue       # 输入框组件
│   └── AppSelect.vue      # 选择器组件
├── admin/                 # 后台专用组件
│   └── dashboard/         # Dashboard 组件
│       ├── KpiCard.vue    # KPI 卡片
│       ├── TrendChart.vue # 趋势图表
│       └── ...
└── layout/               # 布局组件
    ├── AppNaiveConfig.vue # Naive UI 配置
    └── ...
```

**状态**: ✅ 已有基础组件，继续完善

#### 3.3.3 Pattern 组件层 (新增)

**目录结构** (计划):
```
components/
├── patterns/              # 页面模式组件
│   ├── admin/            # 后台页面模式
│   │   ├── ListPage.vue          # 列表页模板
│   │   ├── FormPage.vue          # 表单页模板
│   │   ├── DetailPage.vue        # 详情页模板
│   │   ├── ConfigPage.vue        # 配置页模板
│   │   ├── DashboardPage.vue     # 仪表盘模板
│   │   ├── PageHeader.vue        # 页面头部
│   │   └── FilterBar.vue        # 筛选栏
│   └── web/             # 前台页面模式
│       ├── HeroSection.vue       # Hero 区域
│       ├── FeatureGrid.vue       # 功能网格
│       ├── ProjectShowcase.vue  # 项目展示
│       ├── ArticleLayout.vue     # 文章布局
│       └── CTASection.vue       # CTA 区域
```

**状态**: 🟢 待创建

### 3.4 Layer 4: Application (应用层)

#### 3.4.1 Admin Layer

**目录结构**:
```
pages/admin/
├── index.vue              # Dashboard
├── articles/
│   ├── index.vue          # 使用 ListPage Pattern
│   └── edit.vue          # 使用 FormPage Pattern
├── projects/
│   ├── index.vue          # 使用 ListPage Pattern
│   └── edit.vue          # 使用 FormPage Pattern
└── ...
```

**使用方式**:
```vue
<template>
  <ListPage
    title="文章管理"
    :columns="columns"
    :data="articles"
    :filter-schema="filterSchema"
    @create="handleCreate"
    @edit="handleEdit"
    @delete="handleDelete"
  />
</template>
```

#### 3.4.2 Web Layer

**目录结构**:
```
pages/
├── index.vue              # 首页，使用多个 Section
├── about.vue              # 关于页，使用 FeatureGrid
├── projects/             # 项目展示，使用 ProjectShowcase
└── blog/                 # 博客，使用 ArticleLayout
```

**使用方式**:
```vue
<template>
  <div class="page">
    <HeroSection
      title="溪午听风"
      subtitle="数字花园"
      :actions="heroActions"
    />
    <FeatureGrid :features="features" />
    <ProjectShowcase :projects="projects" />
    <CTASection
      title="开始探索"
      action-text="了解更多"
      @action="handleCTA"
    />
  </div>
</template>
```

---

## 四、Admin 页面 Pattern 设计

### 4.1 ListPage Pattern

**用途**: 后台列表页统一模板

**Props 定义（完整接口）**:
```typescript
interface ListPageProps {
  // 必选 props
  title: string                      // 页面标题
  columns: Column[]                  // 表格列定义
  data: any[]                       // 表格数据
  filterSchema: FilterSchema          // 筛选器配置

  // 可选 props
  headerActions?: Action[]           // 页面头部操作按钮
  loading?: boolean                  // 加载状态，默认 false
  pagination?: PaginationConfig       // 分页配置
  rowSelection?: boolean             // 是否支持行选择
  searchable?: boolean              // 是否显示搜索框，默认 true
  showRefresh?: boolean             // 是否显示刷新按钮

  // 扩展点
  tableClass?: string              // 表格自定义类名
  rowClass?: (row: any) => string  // 行样式函数
}
```

**Slots（扩展点）**:
```vue
<template>
  <ListPage title="文章管理" :data="articles" :columns="columns">
    <!-- slot: title-prefix - 标题前缀（如徽章） -->
    <template #title-prefix>
      <Badge>Beta</Badge>
    </template>

    <!-- slot: filter-extra - 筛选栏额外内容 -->
    <template #filter-extra>
      <DatePicker />
    </template>

    <!-- slot: table-extra - 表格上方额外内容 -->
    <template #table-extra>
      <Alert>提示信息</Alert>
    </template>

    <!-- slot: row-actions - 行操作按钮自定义 -->
    <template #row-actions="{ row }">
      <Button @click="handleCustom(row)">自定义操作</Button>
    </template>

    <!-- slot: footer - 页面底部额外内容 -->
    <template #footer>
      <ExportButton />
    </template>
  </ListPage>
</template>
```

**Emits（事件）**:
```typescript
interface ListPageEmits {
  (e: 'create'): void                          // 点击新建
  (e: 'edit', row: any): void                 // 点击编辑
  (e: 'delete', row: any): void               // 点击删除
  (e: 'search', params: SearchParams): void    // 搜索
  (e: 'reset'): void                          // 重置筛选
  (e: 'row-click', row: any): void            // 行点击
  (e: 'page-change', page: number): void       // 分页变化
  (e: 'selection-change', rows: any[]): void     // 选择变化
}
```

**结构**:
```vue
<template>
  <div class="admin-dashboard-page">
    <!-- 页面头部 -->
    <PageHeader
      :title="title"
      :actions="headerActions"
    />

    <!-- 筛选栏 -->
    <FilterBar
      :schema="filterSchema"
      :loading="loading"
      @search="handleSearch"
      @reset="handleReset"
    />

    <!-- 数据表格 -->
    <DataTable
      :columns="columns"
      :data="data"
      :loading="loading"
      :pagination="pagination"
      @row-click="handleRowClick"
    />
  </div>
</template>

<script setup lang="ts">
interface Props {
  title: string
  columns: Column[]
  data: any[]
  filterSchema: FilterSchema
  headerActions?: Action[]
}
</script>
```

**组件分解**:

| 组件 | 职责 |
|------|------|
| **PageHeader** | 页面标题 + 操作按钮 |
| **FilterBar** | 搜索框 + 筛选器 + 操作按钮 |
| **DataTable** | 表格展示 + 分页 |

**使用示例**:
```vue
<template>
  <ListPage
    title="文章管理"
    :columns="articleColumns"
    :data="articles"
    :filter-schema="articleFilterSchema"
    :header-actions="articleHeaderActions"
    @search="searchArticles"
    @create="createArticle"
    @edit="editArticle"
    @delete="deleteArticle"
  />
</template>
```

### 4.2 FormPage Pattern

**用途**: 后台表单页统一模板

**Props 定义（完整接口）**:
```typescript
interface FormPageProps {
  // 必选 props
  title: string                      // 实体名称（如"文章"）
  formSchema: FormSchema              // 表单配置
  initialModel?: Record<string, any>   // 初始表单数据

  // 可选 props
  mode?: 'create' | 'edit'         // 模式，自动判断或指定
  loading?: boolean                  // 提交中状态
  breadcrumb?: BreadcrumbItem[]       // 面包屑导航
  showCancel?: boolean              // 是否显示取消按钮，默认 true
  submitText?: string              // 提交按钮文字，默认"保存"
  cancelText?: string              // 取消按钮文字，默认"取消"

  // 扩展点
  layout?: 'vertical' | 'horizontal'  // 表单布局方向，默认 vertical
  formClass?: string               // 表单容器自定义类名
}
```

**Slots（扩展点）**:
```vue
<template>
  <FormPage title="文章" :form-schema="schema">
    <!-- slot: header-extra - 页面头部额外内容 -->
    <template #header-extra>
      <Button @click="handlePreview">预览</Button>
    </template>

    <!-- slot: form-before - 表单前额外内容 -->
    <template #form-before>
      <Alert>填写提示</Alert>
    </template>

    <!-- slot: form-after - 表单后额外内容 -->
    <template #form-after>
      <AdditionalFields />
    </template>

    <!-- slot: field-{key} - 自定义特定字段 -->
    <template #field-cover>
      <ImageUploader v-model="form.cover" />
    </template>
  </FormPage>
</template>
```

**Emits（事件）**:
```typescript
interface FormPageEmits {
  (e: 'submit', model: Record<string, any>): void     // 表单提交
  (e: 'cancel'): void                                 // 点击取消
  (e: 'field-change', key: string, value: any): void   // 字段变化
}
```

**迁移边界说明**:
- ✅ **适合迁移**: 单表单、字段数量 < 20、无复杂联动
- ⚠️ **需评估迁移**: 多表单分组、字段间复杂联动、自定义验证逻辑
- ❌ **不适合迁移**: 动态表单、步骤表单、高度定制的表单布局

**结构**:
```vue
<template>
  <div class="admin-dashboard-page">
    <PageHeader
      :title="isEdit ? `编辑${title}` : `新建${title}`"
      :actions="formActions"
      :breadcrumb="breadcrumb"
    />

    <FormCard
      :schema="formSchema"
      :model="formModel"
      :loading="saving"
      @submit="handleSubmit"
      @cancel="handleCancel"
    />
  </div>
</template>
```

**组件分解**:

| 组件 | 职责 |
|------|------|
| **PageHeader** | 带面包屑的页面头部 |
| **FormCard** | 表单卡片，包含验证和提交 |

**使用示例**:
```vue
<template>
  <FormPage
    title="文章"
    :form-schema="articleFormSchema"
    :initial-model="articleData"
    @submit="saveArticle"
    @cancel="goBack"
  />
</template>
```

### 4.3 DetailPage Pattern

**用途**: 后台详情页统一模板

**结构**:
```vue
<template>
  <div class="admin-dashboard-page">
    <PageHeader
      :title="title"
      :actions="detailActions"
      :breadcrumb="breadcrumb"
    />

    <div class="grid gap-6">
      <!-- 主要信息 -->
      <InfoCard
        :schema="mainInfoSchema"
        :data="detail"
      />

      <!-- 附加信息 -->
      <InfoCard
        v-if="extraInfoSchema"
        :schema="extraInfoSchema"
        :data="detail"
      />

      <!-- 关联数据 -->
      <RelatedData
        v-if="relatedData"
        :title="relatedData.title"
        :items="relatedData.items"
      />
    </div>
  </div>
</template>
```

### 4.4 ConfigPage Pattern

**用途**: 后台配置页统一模板

**结构**:
```vue
<template>
  <div class="admin-dashboard-page">
    <PageHeader
      title="系统配置"
      :actions="configActions"
    />

    <div class="space-y-6">
      <ConfigSection
        v-for="section in configSections"
        :key="section.key"
        :title="section.title"
        :description="section.description"
        :schema="section.schema"
        :model="config[section.key]"
        @update="handleUpdate(section.key, $event)"
      />
    </div>
  </div>
</template>
```

### 4.5 DashboardPage Pattern

**用途**: 后台仪表盘统一模板

**结构**:
```vue
<template>
  <div class="admin-dashboard-page">
    <!-- 欢迎区域 -->
    <WelcomeCard
      :user="user"
      :quick-actions="quickActions"
    />

    <!-- KPI 卡片网格 -->
    <KpiGrid :kpis="kpis" />

    <!-- 图表区域 -->
    <ChartGrid :charts="charts" />

    <!-- 待办事项 -->
    <TodoList :todos="todos" />
  </div>
</template>
```

---

## 五、Web 页面 Pattern 设计

### 5.1 HeroSection Pattern

**用途**: 首页或介绍页的顶部展示区域

**结构**:
```vue
<template>
  <section class="hero-section">
    <div class="hero-container">
      <div class="hero-content">
        <Badge v-if="badge" :text="badge" />
        <h1 class="hero-title">{{ title }}</h1>
        <p class="hero-subtitle">{{ subtitle }}</p>
        <div class="hero-actions">
          <Button
            v-for="action in actions"
            :key="action.text"
            :variant="action.variant || 'primary'"
            :to="action.to"
            @click="action.onClick"
          >
            {{ action.text }}
          </Button>
        </div>
      </div>
      <div v-if="visual" class="hero-visual">
        <img :src="visual" :alt="title" />
      </div>
    </div>
  </section>
</template>
```

**样式变量**:
```css
.hero-section {
  --hero-bg: var(--color-bg-body);
  --hero-text: var(--color-text-main);
  --hero-padding: var(--spacing-20) 0;
}
```

### 5.2 FeatureGrid Pattern

**用途**: 功能特性展示区域

**结构**:
```vue
<template>
  <section class="feature-section">
    <SectionHeader
      :title="title"
      :subtitle="subtitle"
    />
    <div class="feature-grid" :class="`feature-grid--${columns}`">
      <FeatureCard
        v-for="feature in features"
        :key="feature.id"
        :icon="feature.icon"
        :title="feature.title"
        :description="feature.description"
      />
    </div>
  </section>
</template>
```

**样式变量**:
```css
.feature-grid {
  --feature-grid-columns: 3;
  --feature-card-bg: var(--color-bg-card);
  --feature-card-hover: var(--color-bg-elevated);
}
```

### 5.3 ProjectShowcase Pattern

**用途**: 项目展示卡片网格

**结构**:
```vue
<template>
  <section class="project-showcase">
    <SectionHeader
      :title="title"
      :subtitle="subtitle"
      :action="viewAllAction"
    />
    <div class="project-grid">
      <ProjectCard
        v-for="project in projects"
        :key="project.id"
        :project="project"
        @click="handleProjectClick"
      />
    </div>
  </section>
</template>
```

### 5.4 ArticleLayout Pattern

**用途**: 文章列表和详情页布局

**结构**:
```vue
<template>
  <article class="article-layout">
    <!-- 文章头部 -->
    <ArticleHeader
      :title="article.title"
      :author="article.author"
      :date="article.date"
      :tags="article.tags"
    />

    <!-- 文章内容 -->
    <ArticleContent :content="article.content" />

    <!-- 文章底部 -->
    <ArticleFooter
      :prev-article="prevArticle"
      :next-article="nextArticle"
    />
  </article>
</template>
```

### 5.5 CTASection Pattern

**用途**: 呼吁行动区域

**结构**:
```vue
<template>
  <section class="cta-section" :class="`cta-section--${variant}`">
    <div class="cta-content">
      <h2 class="cta-title">{{ title }}</h2>
      <p class="cta-description">{{ description }}</p>
      <Button
        :variant="buttonVariant"
        @click="handleClick"
      >
        {{ actionText }}
      </Button>
    </div>
  </section>
</template>
```

---

## 六、样式治理规则

### 6.1 CSS 优先级规范

| 规则 | 优先级 | 说明 |
|------|--------|------|
| **Token 变量** | 1 | 最低，作为默认值 |
| **组件默认样式** | 2 | 组件的基础样式 |
| **类选择器** | 3 | 通过类名覆盖样式 |
| **ID 选择器** | 4 | 尽量避免使用 |
| **内联样式** | 5 | 避免使用，除非动态计算 |
| **!important** | 6 | 禁止使用，除非紧急修复 |

**禁止使用 !important 的情况**:
- ❌ 常规样式调整
- ❌ 主题切换适配
- ❌ 组件样式定制

**允许使用 !important 的情况**:
- ✅ 紧急 bug 修复（需添加 TODO 注释）
- ✅ 第三方库样式覆盖（需注明原因）

### 6.2 变量使用规范

| 场景 | 使用变量 | 示例 |
|------|---------|------|
| **文字颜色** | `--color-text-*` | `color: var(--color-text-main)` |
| **背景颜色** | `--color-bg-*` | `background: var(--color-bg-card)` |
| **边框颜色** | `--color-border-*` | `border-color: var(--color-border-default)` |
| **间距** | `--spacing-*` | `padding: var(--spacing-4)` |
| **圆角** | `--radius-*` | `border-radius: var(--radius-md)` |
| **阴影** | `--shadow-*` | `box-shadow: var(--shadow-md)` |

### 6.3 Tailwind CSS 使用规范

#### 6.3.1 允许的 Tailwind 类白名单

| 类别 | 允许的类 | 说明 | 示例 |
|------|----------|------|------|
| **布局** | `flex`, `grid`, `block`, `inline-block`, `hidden` | 基础布局 | `flex`, `grid grid-cols-2`, `hidden` |
| **Flex 方向** | `flex-row`, `flex-col`, `flex-row-reverse` 等 | Flex 方向 | `flex-col` |
| **对齐** | `justify-*`, `items-*`, `self-*` | 对齐方式 | `justify-center`, `items-center` |
| **Gap** | `gap-*` | 元素间距 | `gap-4`, `gap-x-2`, `gap-y-2` |
| **内边距** | `p-*`, `px-*`, `py-*`, `pt-*`, `pr-*`, `pb-*`, `pl-*` | padding | `p-4`, `px-6`, `py-2` |
| **外边距** | `m-*`, `mx-*`, `my-*`, `mt-*`, `mr-*`, `mb-*`, `ml-*` | margin | `m-4`, `mx-auto`, `my-2` |
| **宽度** | `w-*`, `w-{px}-%` | 宽度 | `w-full`, `w-1/2`, `w-64` |
| **高度** | `h-*`, `h-{px}-%`, `min-h-*`, `max-h-*` | 高度 | `h-full`, `h-64`, `min-h-screen` |
| **显示** | `hidden`, `block`, `inline-block`, `inline` | 显示状态 | `hidden` |
| **溢出** | `overflow-*`, `overflow-{x/y}-*` | 溢出处理 | `overflow-hidden`, `overflow-auto` |
| **定位** | `relative`, `absolute`, `fixed`, `sticky` | 定位方式 | `relative`, `fixed` |
| **层级** | `z-*` | z-index | `z-10`, `z-50` |
| **过渡** | `transition*` | 过渡效果 | `transition-all` |
| **变换** | `transform*` | 变换 | `transform` |
| **响应式前缀** | `sm:`, `md:`, `lg:`, `xl:`, `2xl:` | 响应式断点 | `md:flex`, `lg:grid-cols-3` |

#### 6.3.2 禁止的 Tailwind 类黑名单

| 类别 | 禁止的类 | 原因 | 正确做法 |
|------|----------|------|---------|
| **背景颜色** | `bg-white`, `bg-black`, `bg-gray-*`, `bg-blue-*` 等 | 不适配主题 | 使用 `bg-bg-card`, `bg-bg-elevated` |
| **文字颜色** | `text-white`, `text-black`, `text-gray-*`, `text-blue-*` 等 | 不适配主题 | 使用 `text-text-main`, `text-text-muted` |
| **边框颜色** | `border-white`, `border-gray-*`, `border-blue-*` 等 | 不适配主题 | 使用 `border-border-subtle`, `border-border-default` |
| **阴影** | `shadow-sm`, `shadow-md`, `shadow-lg` 等 | 与 Token 不一致 | 使用 CSS 变量 `--shadow-md` |
| **圆角** | `rounded`, `rounded-sm`, `rounded-md`, `rounded-lg` 等 | 与 Token 不一致 | 使用 CSS 变量 `--radius-md` |
| **字体大小** | `text-xs`, `text-sm`, `text-base`, `text-lg` 等 | 与 Token 不一致 | 使用 CSS 变量或语义化类 |
| **字体颜色状态** | `hover:text-*`, `focus:text-*` 等 | 不适配主题 | 使用 CSS 变量 |
| **背景状态** | `hover:bg-*`, `focus:bg-*` 等 | 不适配主题 | 使用 CSS 变量 |

#### 6.3.3 有条件允许的 Tailwind 类

| 类别 | 允许条件 | 示例 |
|------|----------|------|
| **渐变背景** | 仅在前台 Hero Section 等特殊场景 | `bg-gradient-to-br` |
| **动画** | 仅用于微交互效果 | `animate-pulse`, `animate-spin` |
| **特殊效果** | 仅在不影响主题切换的场景 | `backdrop-blur` |

#### 6.3.4 创建语义化 Tailwind 类（推荐）

为常用组合创建语义化类，避免每次重复：

```css
/* 在 assets/css/components.css 中添加 */

/* 页面容器 */
.page-container {
  @apply min-h-screen py-12 px-4 sm:px-6 lg:px-8;
}

/* 卡片基础 */
.card-base {
  @apply bg-bg-card rounded-lg shadow-sm border border-border-subtle;
}

/* 按钮基础 */
.btn-base {
  @apply px-4 py-2 rounded-md font-medium transition-colors;
}
```

然后在 Vue 中使用：
```vue
<!-- ✅ 正确 -->
<div class="card-base">
  <button class="btn-base">按钮</button>
</div>

<!-- ❌ 错误 -->
<div class="bg-white rounded-lg shadow-sm border border-gray-200">
  <button class="px-4 py-2 rounded-md font-medium transition-colors">按钮</button>
</div>
```

#### 6.3.5 自动化检查

可以在 `.eslintrc.js` 或 `.stylelintrc.js` 中添加规则来检测禁止的类：

```javascript
// .stylelintrc.js
module.exports = {
  rules: {
    'selector-class-pattern': [
      true,
      '^(page|card|btn|text|bg|border|flex|grid|spacing)-[a-z-]+$',
      {
        message: 'Expected custom class name, avoid raw Tailwind utility'
      }
    ]
  }
}
```

**颜色使用替代方案**:
```vue
<!-- ❌ 错误 -->
<div class="bg-white text-gray-900 border-gray-200">
<!-- ✅ 正确 -->
<div class="bg-bg-card text-text-main border-border-subtle">
```

### 6.4 组件开发规范

| 规范 | 说明 |
|------|------|
| **Props 命名** | 使用语义化名称，避免与 Naive UI 冲突 |
| **Emits 命名** | 使用动词开头 (`handleClick`, `handleSubmit`) |
| **样式作用域** | 使用 `scoped` 或模块化 CSS |
| **样式变量** | 组件内部样式优先使用 CSS 变量 |

---

## 七、设计系统目录结构

```
PersonWeb/
├── assets/
│   └── styles/
│       ├── theme-variables.css       # Foundation + Semantic 层
│       ├── base.css                # 基础重置
│       └── components.css          # 通用组件样式
├── components/
│   ├── ui/                        # 基础 UI 组件
│   │   ├── AppCard.vue
│   │   ├── AppButton.vue
│   │   ├── AppInput.vue
│   │   └── ...
│   ├── patterns/                  # 页面模式组件 🆕
│   │   ├── admin/
│   │   │   ├── ListPage.vue
│   │   │   ├── FormPage.vue
│   │   │   ├── DetailPage.vue
│   │   │   ├── ConfigPage.vue
│   │   │   ├── DashboardPage.vue
│   │   │   ├── PageHeader.vue
│   │   │   └── FilterBar.vue
│   │   └── web/
│   │       ├── HeroSection.vue
│   │       ├── FeatureGrid.vue
│   │       ├── ProjectShowcase.vue
│   │       ├── ArticleLayout.vue
│   │       └── CTASection.vue
│   └── layout/
│       └── AppNaiveConfig.vue     # Naive UI Theme 配置
├── docs/
│   └── design-system/
│       ├── 01-style-audit-report.md
│       ├── 02-design-system-architecture.md
│       ├── 03-implementation-roadmap.md
│       ├── 04-component-library.md  # 组件文档 🆕
│       ├── 05-pattern-guide.md     # Pattern 使用指南 🆕
│       └── 06-style-guidelines.md  # 样式规范文档 🆕
└── pages/
    ├── admin/                     # 后台页面 (使用 Admin Pattern)
    └── ...                        # 前台页面 (使用 Web Section)
```

---

## 八、技术栈与依赖

### 8.1 核心依赖

| 技术 | 版本 | 用途 |
|------|------|------|
| Vue 3 | ^3.4 | 前端框架 |
| Nuxt 3 | ^3.10 | 应用框架 |
| Naive UI | ^2.38 | UI 组件库 (后台) |
| Tailwind CSS | ^3.4 | CSS 框架 |
| PostCSS | ^8.4 | CSS 处理 |

### 8.2 开发工具

| 工具 | 用途 |
|------|------|
| TypeScript | 类型检查 |
| ESLint | 代码规范 |
| Stylelint | 样式规范检查 |
| Vitest | 单元测试 |

---

## 九、版本计划

| 版本 | 目标 | 预计时间 |
|------|------|---------|
| **v1.0** | Token 体系 + Admin Pattern | 1.5 个月 |
| **v1.1** | Web Section Pattern + 文档 | 1 个月 |
| **v1.2** | 组件库完善 + 主题扩展 | 持续迭代 |

---

**架构设计完成** ✅
