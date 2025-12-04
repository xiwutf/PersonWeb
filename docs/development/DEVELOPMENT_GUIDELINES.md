# 项目开发规范文档

本文档定义了本项目的开发规范和要求，所有开发工作（包括AI辅助开发）都应严格遵循这些规范。

## ⚠️ 重要提醒

**每次修改页面样式时，请务必先查看 [样式管理开发提醒](./STYLE_MANAGEMENT_REMINDER.md)！**

## 📋 目录

1. [样式管理规范](#样式管理规范) ⭐ **必读**
   - [样式架构规范](./STYLE_ARCHITECTURE.md) - 详细的样式架构说明
2. [代码组织规范](#代码组织规范)
3. [命名规范](#命名规范)
4. [组件开发规范](#组件开发规范)
5. [API 开发规范](#api-开发规范)
6. [数据库规范](#数据库规范)
7. [Git 提交规范](#git-提交规范)
8. [文档维护规范](#文档维护规范) ⭐ **重要**
9. [最佳实践](#最佳实践)
10. [禁止事项](#禁止事项)

---

## 🎨 样式管理规范

> 📌 **开发前必读**：[样式管理开发提醒](./STYLE_MANAGEMENT_REMINDER.md)

### 核心原则

**所有样式必须统一管理，禁止在 template 中使用内联样式（`:style` 除外，但需最小化使用）**

⚠️ **每次修改页面样式前，请先查看样式管理开发提醒文档！**

### 样式架构（分层设计）

项目采用**分层样式架构**，结合 Naive UI 主题系统，实现统一的样式管理。

#### 架构分层

```
┌─────────────────────────────────────────┐
│  后端主题配置 (/api/Config/theme)       │
│  ↓                                      │
│  useTheme().setTheme()                  │
│  ↓                                      │
│  document.documentElement.dataset.theme │
│  ↓                                      │
│  tokens.css (CSS 变量)                  │
│  ↓                                      │
│  ┌─────────────────┬─────────────────┐ │
│  │ 自定义组件样式   │  Naive UI 主题  │ │
│  │ (使用 CSS 变量) │ (themeOverrides)│ │
│  └─────────────────┴─────────────────┘ │
└─────────────────────────────────────────┘
```

#### 1. 设计 Token 层（`assets/styles/tokens.css`）

**职责**：定义全局 CSS 变量（颜色、圆角、阴影、字号、间距等）

**特点**：
- 通过 `:root` 定义默认主题变量
- 通过 `html[data-theme="xxx"]` 为不同主题设置变量覆盖
- 支持的主题：`light`、`dark`、`lab`、`tech-blue`、`paper`、`forest`、`hybrid-super`、`hybrid-super-dark`、`hybrid-super-light`

**变量命名规范**：
```css
/* 颜色变量 */
--color-bg-page          /* 页面背景色 */
--color-bg-body          /* 主体背景色 */
--color-bg-card          /* 卡片背景色 */
--color-bg-elevated      /* 提升层背景色 */
--color-text-main        /* 主要文字颜色 */
--color-text-muted       /* 次要文字颜色 */
--color-text-disabled    /* 禁用文字颜色 */
--color-primary          /* 主色调 */
--color-primary-base     /* 主色基础值 */
--color-primary-soft     /* 主色柔和版本 */
--color-primary-hover    /* 主色悬停状态 */
--color-border-subtle    /* 细微边框颜色 */
--color-border-default    /* 默认边框颜色 */
--color-border-strong     /* 强调边框颜色 */

/* 圆角变量 */
--radius-sm              /* 小圆角 */
--radius-md              /* 中等圆角 */
--radius-lg              /* 大圆角 */
--radius-xl              /* 超大圆角 */

/* 阴影变量 */
--shadow-sm              /* 小阴影 */
--shadow-md              /* 中等阴影 */
--shadow-lg              /* 大阴影 */
--shadow-xl              /* 超大阴影 */
--shadow-2xl             /* 2倍超大阴影 */
--shadow-soft            /* 柔和阴影 */

/* 字号变量 */
--font-size-h1           /* 一级标题 */
--font-size-h2           /* 二级标题 */
--font-size-h3           /* 三级标题 */
--font-size-h4           /* 四级标题 */
--font-size-body         /* 正文 */
--font-size-sm           /* 小号 */
--font-size-xs           /* 超小号 */

/* 间距变量 */
--spacing-xs             /* 超小间距 */
--spacing-sm             /* 小间距 */
--spacing-md             /* 中等间距 */
--spacing-lg             /* 大间距 */
--spacing-xl             /* 超大间距 */
--spacing-2xl            /* 2倍超大间距 */
```

**使用方式**：
```vue
<style scoped>
.my-component {
  background-color: var(--color-bg-card);
  color: var(--color-text-main);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  padding: var(--spacing-md);
}
</style>
```

#### 2. 基础样式层（`assets/styles/base.css`）

**职责**：基础样式和 Reset

**内容**：
- `html, body` 的默认字体、背景色、文字颜色
- 链接、图片、按钮、输入框、列表等基础重置
- 标题、段落、代码等排版样式
- 滚动条和选择文本样式

**特点**：不覆盖 Naive UI 内部样式，只做基础重置

#### 3. Naive UI 补丁层（`assets/styles/ui-patch-naive.css`）

**职责**：Naive UI 全局补丁样式

**内容**：
- Card、Dialog、Button、Input、Select、Form、Table、Menu 等组件的统一风格补丁
- 使用 CSS 变量（如 `var(--radius-xl)`、`var(--shadow-soft)`）确保与主题系统联动

**特点**：只做视觉风格调整，不覆盖核心功能

#### 4. Naive UI 配置层（`components/layout/AppNaiveConfig.vue`）

**职责**：统一管理 Naive UI 的主题配置

**功能**：
- 使用 `NConfigProvider`、`NDialogProvider`、`NMessageProvider`、`NNotificationProvider` 包裹内容
- 基于 `useTheme().currentTheme` 计算 Naive 主题（light 用默认，其他用 darkTheme）
- 在 `themeOverrides` 中使用 CSS 变量（如 `var(--color-primary)`）作为主题色
- 支持 SSR，使用动态导入避免服务端错误

**使用方式**：
- 已在 `layouts/default.vue` 中使用 `<AppNaiveConfig>` 包裹内容
- 所有 Naive UI 组件自动应用统一的主题配置，无需额外设置

#### 5. 组件级样式（`<style scoped>`）

- **用途**：组件特有的样式
- **位置**：组件文件内的 `<style scoped>` 块
- **要求**：优先使用 CSS 变量，避免硬编码颜色、尺寸等值
- **示例**：
```vue
<style scoped>
.component-container {
  background-color: var(--color-bg-card);
  color: var(--color-text-main);
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-sm);
  padding: var(--spacing-md);
}
</style>
```

#### 6. 功能模块统一样式（`assets/css/`）

- **用途**：跨组件复用的样式类
- **位置**：`assets/css/` 目录下的 CSS 文件
- **命名规范**：使用功能前缀，如 `visitor-*`、`admin-*`、`home-*` 等
- **要求**：使用 CSS 变量，确保与主题系统联动
- **示例**：
  - `assets/css/visitor-interaction.css` - 访客互动统一样式
  - `assets/css/header.css` - Header 组件统一样式
  - `assets/css/footer.css` - Footer 组件统一样式
  - `assets/css/home.css` - 首页组件统一样式

#### 7. 动态样式（`:style` 绑定）

- **使用场景**：仅用于必须动态计算的属性
- **允许的属性**：
  - 位置（`left`, `top`, `transform`）
  - 颜色（`color`, `backgroundColor`）- 仅当需要运行时计算时
  - 动画参数（`animationDuration`, `animationDelay`）
- **禁止的属性**：
  - 固定尺寸（应使用 CSS 类）
  - 固定颜色（应使用 CSS 变量）
  - 布局属性（应使用 CSS 类）

### 主题切换机制

#### 工作原理

1. **后端配置驱动**：
   - 从 `/api/Config/theme` 获取当前主题
   - 保存主题时调用 `PUT /api/Config/theme`
   - 保存成功后立即调用 `setTheme()` 切换前端主题

2. **前端主题应用**：
   - `useTheme().setTheme()` 设置 `document.documentElement.dataset.theme`
   - `tokens.css` 中的 `html[data-theme="xxx"]` 选择器生效
   - CSS 变量自动切换，所有使用变量的组件自动更新

3. **Naive UI 主题联动**：
   - `AppNaiveConfig` 组件监听 `currentTheme` 变化
   - 自动计算 Naive 主题（light 用默认，其他用 darkTheme）
   - `themeOverrides` 中使用 CSS 变量，确保与主题系统联动

#### 切换主题示例

```typescript
// 在组件中切换主题
const { setTheme } = useTheme()

// 切换到深色主题
setTheme('dark')

// 切换到实验室主题
setTheme('lab')

// 切换到混合超级风格主题
setTheme('hybrid-super')
```

### 样式类命名规范

```css
/* 功能前缀 + 元素类型 + 状态/变体 */
.visitor-modal-overlay      /* 访客-模态框-遮罩层 */
.visitor-form-input         /* 访客-表单-输入框 */
.visitor-button-primary     /* 访客-按钮-主要 */
.visitor-emoji-option-selected  /* 访客-表情-选项-选中状态 */
```

### 样式提取规则

1. **重复样式** → 提取到统一样式文件（`assets/css/`）
2. **组件特有样式** → 保留在组件 `<style scoped>` 中
3. **动态计算样式** → 使用 `:style` 绑定（最小化）
4. **设计变量** → 使用 CSS 变量（`var(--color-primary)` 等）

### 样式开发流程

1. **确定样式类型**：
   - 设计变量（颜色、圆角、阴影等）→ 使用 CSS 变量
   - 组件特有样式 → 使用 `<style scoped>`
   - 跨组件复用样式 → 提取到 `assets/css/` 统一样式文件

2. **使用 CSS 变量**：
   - 优先使用 `tokens.css` 中定义的变量
   - 避免硬编码颜色、尺寸等值
   - 确保样式与主题系统联动

3. **Naive UI 组件**：
   - 直接使用 Naive UI 组件，无需额外配置
   - 组件会自动应用 `AppNaiveConfig` 中的主题配置
   - 如需自定义样式，使用 CSS 变量覆盖

### 示例对比

❌ **错误示例**：
```vue
<template>
  <div :style="{ padding: '1rem', background: 'rgba(0,0,0,0.5)' }">
    <button :style="{ width: '100px', height: '40px', color: '#fff' }">按钮</button>
  </div>
</template>
```

✅ **正确示例**：
```vue
<template>
  <div class="visitor-modal">
    <button class="visitor-button-primary">按钮</button>
  </div>
</template>

<style scoped>
.visitor-modal {
  padding: var(--spacing-md);
  background-color: var(--color-bg-elevated);
  border-radius: var(--radius-lg);
}

.visitor-button-primary {
  width: 100px;
  height: 40px;
  background-color: var(--color-primary);
  color: var(--color-text-main);
  border-radius: var(--radius-md);
}
</style>
```

✅ **动态样式正确示例**：
```vue
<template>
  <div 
    class="danmaku-item danmaku-row-0"
    :style="{ 
      color: dynamicColor,  // ✅ 必须动态计算
      animationDuration: `${duration}s`  // ✅ 必须动态计算
    }"
  >
    弹幕内容
  </div>
</template>

<style scoped>
.danmaku-item {
  position: absolute;
  left: 100%;
  background-color: var(--color-bg-card);  /* ✅ 使用 CSS 变量 */
  border-radius: var(--radius-md);        /* ✅ 使用 CSS 变量 */
  /* 其他固定样式 */
}

.danmaku-row-0 { top: 10%; }  /* ✅ 使用CSS类控制位置 */
</style>
```

### Naive UI 组件使用

所有 Naive UI 组件会自动应用 `AppNaiveConfig` 中的主题配置，无需额外设置：

```vue
<template>
  <!-- 直接使用，自动应用主题 -->
  <n-button type="primary">按钮</n-button>
  <n-card>卡片内容</n-card>
  <n-input v-model:value="inputValue" placeholder="请输入" />
  <n-data-table :columns="columns" :data="data" />
</template>
```

如需自定义样式，使用 CSS 变量：

```vue
<style scoped>
/* 自定义 Naive 组件样式 */
.n-card {
  background-color: var(--color-bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-soft);
}
</style>
```

### 示例对比

❌ **错误示例**：
```vue
<template>
  <div :style="{ padding: '1rem', background: 'rgba(0,0,0,0.5)' }">
    <button :style="{ width: '100px', height: '40px' }">按钮</button>
  </div>
</template>
```

✅ **正确示例**：
```vue
<template>
  <div class="visitor-modal">
    <button class="visitor-button-primary">按钮</button>
  </div>
</template>

<style scoped>
.visitor-modal {
  padding: 1rem;
  background: rgba(0, 0, 0, 0.5);
}

.visitor-button-primary {
  width: 100px;
  height: 40px;
}
</style>
```

✅ **动态样式正确示例**：
```vue
<template>
  <div 
    class="danmaku-item danmaku-row-0"
    :style="{ 
      color: dynamicColor,  // ✅ 必须动态计算
      animationDuration: `${duration}s`  // ✅ 必须动态计算
    }"
  >
    弹幕内容
  </div>
</template>

<style scoped>
.danmaku-item {
  position: absolute;
  left: 100%;
  /* 其他固定样式 */
}

.danmaku-row-0 { top: 10%; }  /* ✅ 使用CSS类控制位置 */
</style>
```

---

## 📁 代码组织规范

### 目录结构

```
项目根目录/
├── assets/
│   ├── styles/          # 全局样式文件（设计 Token 和基础样式）
│   │   ├── tokens.css   # 设计 Token：CSS 变量定义（颜色、圆角、阴影等）
│   │   ├── base.css     # 基础样式和 Reset
│   │   ├── ui-patch-naive.css  # Naive UI 补丁样式
│   │   └── theme.css    # 旧主题文件（兼容性，逐步迁移）
│   └── css/             # 功能模块统一样式文件
│       ├── main.css     # 全局基础样式
│       ├── themes.css   # 主题样式（兼容性）
│       ├── header.css  # Header 组件统一样式
│       ├── footer.css  # Footer 组件统一样式
│       ├── home.css     # 首页组件统一样式
│       └── visitor-interaction.css  # 功能模块统一样式
├── components/          # Vue 组件
│   ├── layout/          # 布局组件
│   │   └── AppNaiveConfig.vue  # Naive UI 统一配置容器
│   ├── home/            # 首页相关组件
│   ├── admin/           # 后台管理组件
│   └── [功能模块]/      # 按功能模块组织
├── pages/               # 页面路由
│   ├── admin/           # 后台管理页面
│   └── [功能模块]/      # 按功能模块组织
├── composables/         # 组合式函数
│   └── useTheme.ts      # 主题管理（设置 data-theme）
├── layouts/             # 布局组件
│   └── default.vue      # 默认布局（使用 AppNaiveConfig 包裹）
├── middleware/          # 路由中间件
├── plugins/             # 插件
│   └── init-theme.client.ts  # 主题初始化（从后端获取主题）
├── types/               # TypeScript 类型定义
├── backend/             # 后端代码（C#）
│   └── PersonalSite.Api/
│       ├── Controllers/  # API 控制器
│       │   └── ConfigController.cs  # 配置控制器（主题配置）
│       ├── Models/       # 数据模型
│       └── Data/         # 数据访问层
└── database/            # 数据库脚本
```

### 文件命名规范

- **组件文件**：PascalCase，如 `VisitorDanmakuWall.vue`
- **页面文件**：kebab-case，如 `visitor-messages.vue`
- **组合式函数**：camelCase，如 `useModuleSystem.ts`
- **类型文件**：kebab-case，如 `api.ts`、`module.ts`
- **样式文件**：kebab-case，如 `visitor-interaction.css`

---

## 🏷️ 命名规范

### Vue 组件命名

- **组件文件名**：PascalCase
- **组件内部 name**：PascalCase（可选，但建议与文件名一致）
- **示例**：`VisitorDanmakuWall.vue` → `<VisitorDanmakuWall />`

### CSS 类命名

- **全局样式类**：`功能前缀-元素类型-状态`
- **组件样式类**：`组件名-元素类型-状态`（在 scoped 中）
- **示例**：
  - `.visitor-modal-overlay`（全局）
  - `.danmaku-item`（组件内）

### 变量命名

- **JavaScript/TypeScript**：camelCase
- **常量**：UPPER_SNAKE_CASE
- **私有属性**：`_` 前缀（不推荐，优先使用 TypeScript private）

### API 端点命名

- **RESTful 风格**：`/api/资源名/操作`
- **示例**：
  - `GET /api/VisitorInteraction/messages/approved`
  - `POST /api/VisitorInteraction/message`
  - `POST /api/VisitorInteraction/message/{id}/approve`

---

## 🧩 组件开发规范

### 组件结构

```vue
<template>
  <!-- 1. 模板内容，只使用 class 和必要的 :style -->
  <div class="component-container">
    <button class="component-button" :style="dynamicStyle">
      按钮
    </button>
  </div>
</template>

<script setup lang="ts">
// 2. 类型定义
interface Props {
  // ...
}

// 3. Props 和 Emits
const props = defineProps<Props>()
const emit = defineEmits<{...}>()

// 4. 组合式函数导入
const api = useApi()
const route = useRoute()

// 5. 响应式数据
const data = ref<Type>([])

// 6. 计算属性
const computed = computed(() => {
  // ...
})

// 7. 方法
const method = () => {
  // ...
}

// 8. 生命周期
onMounted(() => {
  // ...
})
</script>

<style scoped>
/* 9. 组件样式 */
.component-container {
  /* ... */
}
</style>
```

### 组件开发要求

1. **单一职责**：每个组件只负责一个功能
2. **可复用性**：提取可复用的逻辑到 composables
3. **类型安全**：使用 TypeScript 定义所有类型
4. **样式隔离**：使用 `<style scoped>`
5. **响应式设计**：考虑移动端适配

### 组合式函数（Composables）

- **位置**：`composables/` 目录
- **命名**：`use` 前缀，如 `useModuleSystem.ts`
- **职责**：封装可复用的逻辑
- **示例**：
  - `useApi()` - API 调用封装
  - `useModuleSystem()` - 模块系统管理
  - `useTheme()` - 主题管理

---

## 🔌 API 开发规范

### 后端 API（C#）

#### 控制器结构

```csharp
[ApiController]
[Route("api/[controller]")]
public class ResourceController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ResourceController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: api/Resource
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Resource>>>> GetResources()
    {
        // 实现
    }
    
    // POST: api/Resource
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Resource>>> CreateResource([FromBody] ResourceDto dto)
    {
        // 实现
    }
}
```

#### 响应格式

所有 API 响应使用统一的 `ApiResponse<T>` 格式：

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public int Code { get; set; }
    
    public static ApiResponse<T> Success(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Code = 200
        };
    }
    
    public static ApiResponse<T> Error(string message, int code = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Code = code
        };
    }
}
```

#### 错误处理

- 使用 try-catch 捕获异常
- 返回适当的 HTTP 状态码
- 提供有意义的错误消息

### 前端 API 调用

#### 使用 `useApi()` composable

```typescript
const api = useApi()

// GET 请求
const data = await api.get<Type[]>('/endpoint')

// POST 请求
const result = await api.post('/endpoint', { ...data })

// 错误处理
try {
  const result = await api.post('/endpoint', data)
} catch (e) {
  handleError(e, '操作失败')
}
```

---

## 🗄️ 数据库规范

### 表命名

- **命名规范**：snake_case
- **前缀**：根据功能模块，如 `visitor_*`、`module_*`、`style_*`
- **示例**：
  - `visitor_message` - 访客留言表
  - `module_config` - 模块配置表
  - `style_definition` - 样式定义表

### 字段命名

- **命名规范**：snake_case
- **主键**：统一使用 `id`（BIGINT）
- **外键**：`关联表名_id` 或 `关联字段名`
- **时间字段**：`created_at`、`updated_at`、`deleted_at`

### 数据库脚本组织

- **位置**：`database/` 目录
- **命名**：`功能模块_tables.sql`
- **示例**：
  - `visitor_interaction_tables.sql`
  - `module_system_tables.sql`
  - `style_management_tables.sql`

### 迁移脚本

- 每个功能模块创建独立的 SQL 脚本
- 包含完整的表结构定义
- 包含必要的初始数据（INSERT 语句）

---

## 📝 Git 提交规范

### 提交信息格式

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type 类型

- `feat`: 新功能
- `fix`: 修复 bug
- `docs`: 文档更新
- `style`: 代码格式调整（不影响功能）
- `refactor`: 重构
- `perf`: 性能优化
- `test`: 测试相关
- `chore`: 构建/工具链相关

### 示例

```
feat(visitor): 添加访客互动弹幕墙功能

- 实现弹幕实时显示
- 添加弹幕审核功能
- 统一弹幕样式管理

Closes #123
```

---

## 📝 文档维护规范

### 核心要求

**开发过程中必须随时记录文档并更新对应的文档，在每次提交代码前更新相关文档。**

### 文档更新时机

1. **功能开发时**：
   - 新增功能 → 更新功能开发文档（`docs/features/`）
   - 修改功能 → 更新对应的功能文档
   - 优化功能 → 更新优化计划文档

2. **代码重构时**：
   - 架构变更 → 更新架构文档（`docs/architecture/`）
   - 规范变更 → 更新开发规范文档（`docs/development/DEVELOPMENT_GUIDELINES.md`）

3. **配置变更时**：
   - API 变更 → 更新配置文档（`docs/config/`）
   - 环境变更 → 更新部署文档（`docs/deployment/`）

4. **问题修复时**：
   - Bug 修复 → 更新故障排除文档（`docs/troubleshooting/`）
   - 记录问题和解决方案

5. **提交代码前**：
   - **必须**检查并更新所有相关文档
   - 确保文档与代码同步

### 文档更新清单

在提交代码前，请检查以下文档是否需要更新：

- [ ] **功能文档**（`docs/features/`）
  - [ ] 功能实现状态（`IMPLEMENTATION_STATUS.md`）
  - [ ] 待开发功能列表（`TODO_FEATURES.md`）
  - [ ] 功能优化计划（`OPTIMIZATION_PLAN.md`）

- [ ] **开发规范**（`docs/development/`）
  - [ ] 开发规范文档（`DEVELOPMENT_GUIDELINES.md`）- 如有规范变更

- [ ] **架构文档**（`docs/architecture/`）
  - [ ] 模块系统文档（`README_MODULES.md`）- 如有模块变更
  - [ ] 系统架构文档（`MODULE_SYSTEM.md`）- 如有架构变更

- [ ] **配置文档**（`docs/config/`）
  - [ ] API 配置（`API_CONFIG.md`）- 如有 API 变更
  - [ ] 环境配置（`ENV_COMPATIBILITY.md`）- 如有环境变更

- [ ] **部署文档**（`docs/deployment/`）
  - [ ] 快速开始指南（`QUICK_START.md`）- 如有流程变更
  - [ ] 开发环境配置（`DEVELOPMENT_SETUP.md`）- 如有环境变更

- [ ] **故障排除文档**（`docs/troubleshooting/`）
  - [ ] 记录修复的问题和解决方案

- [ ] **README 文档**
  - [ ] 主 README（`README.md`）- 如有重大变更
  - [ ] 文档索引（`docs/README.md`）- 如有新增文档

### 文档更新原则

1. **及时性**：开发过程中随时记录，不要等到最后才补文档
2. **准确性**：确保文档内容与代码实现一致
3. **完整性**：记录关键信息，包括：
   - 功能说明
   - 使用方法
   - 配置说明
   - 注意事项
   - 已知问题

4. **可维护性**：
   - 使用清晰的标题和结构
   - 添加必要的代码示例
   - 保持文档格式统一

### 文档更新模板

#### 功能开发文档更新模板

```markdown
## [功能名称] - [日期]

### 变更内容
- 新增/修改/删除：[具体内容]

### 实现细节
- [技术实现说明]

### 使用方法
- [使用说明]

### 注意事项
- [注意事项]

### 相关文件
- `路径/文件.vue` - [说明]
- `路径/文件.ts` - [说明]
```

#### Bug 修复文档更新模板

```markdown
## [问题描述] - [日期]

### 问题现象
- [问题描述]

### 问题原因
- [原因分析]

### 解决方案
- [解决方法]

### 相关文件
- `路径/文件.vue` - [修改说明]
```

### AI 辅助开发时的文档更新

当使用 AI 辅助开发时，AI 应该：

1. **自动识别文档更新需求**：
   - 分析代码变更
   - 识别需要更新的文档
   - 提示用户更新文档

2. **提供文档更新建议**：
   - 建议更新的文档列表
   - 提供文档更新内容模板
   - 检查文档完整性

3. **在提交前提醒**：
   - 提醒检查文档更新清单
   - 确保文档与代码同步

### 提交代码前的文档检查流程

当用户说"提交"时，AI 应该：

1. **自动检查代码变更**：
   - 分析修改的文件
   - 识别功能变更、Bug 修复、配置变更等

2. **提示需要更新的文档**：
   - 列出可能需要更新的文档列表
   - 提供文档更新建议

3. **协助更新文档**：
   - 根据代码变更生成文档更新内容
   - 使用文档更新模板
   - 确保文档与代码同步

4. **确认文档完整性**：
   - 检查文档更新清单
   - 确保所有相关文档都已更新

**示例对话流程**：

```
用户: "提交代码"

AI: "检测到以下变更：
- 新增访客互动功能
- 修改了样式管理规范

需要更新的文档：
1. docs/features/IMPLEMENTATION_STATUS.md - 更新功能实现状态
2. docs/development/DEVELOPMENT_GUIDELINES.md - 更新开发规范

正在更新文档..."
```

---

## ✅ 最佳实践

### 1. 样式管理

- ✅ 所有样式放在 `<style scoped>` 或统一样式文件中
- ✅ 使用 CSS 类而非内联样式
- ✅ **优先使用 CSS 变量**（`var(--color-primary)` 等），避免硬编码颜色、尺寸等值
- ✅ 动态样式仅用于必须运行时计算的属性
- ✅ 提取可复用的样式到统一样式文件
- ✅ 使用 `AppNaiveConfig` 统一管理 Naive UI 主题
- ✅ 确保样式与主题系统联动（使用 CSS 变量）

### 2. 代码质量

- ✅ 使用 TypeScript 确保类型安全
- ✅ 使用 ESLint 和 Prettier 保持代码风格一致
- ✅ 编写清晰的注释（中文）
- ✅ 遵循单一职责原则

### 3. 性能优化

- ✅ 使用 `v-if` 而非 `v-show`（当条件很少变化时）
- ✅ 使用 `computed` 缓存计算结果
- ✅ 避免在模板中使用复杂表达式
- ✅ 使用 `async/await` 处理异步操作

### 4. 用户体验

- ✅ 提供加载状态反馈
- ✅ 处理错误情况并给出提示
- ✅ 考虑移动端适配
- ✅ 使用骨架屏或加载动画

### 5. 安全性

- ✅ 后端验证所有输入
- ✅ 使用参数化查询防止 SQL 注入
- ✅ 实现适当的权限控制
- ✅ 敏感信息不暴露在前端

### 6. 可维护性

- ✅ 模块化组织代码
- ✅ 提取可复用的逻辑到 composables
- ✅ 使用统一的错误处理机制
- ✅ 保持代码结构清晰

### 7. 文档维护

- ✅ 开发过程中随时记录文档
- ✅ 提交代码前更新所有相关文档
- ✅ 确保文档与代码同步
- ✅ 使用文档更新模板保持格式统一

---

## 🚫 禁止事项

### 样式相关

- ❌ 禁止在 template 中使用内联 `style` 属性（固定样式）
- ❌ 禁止在 template 中使用 Tailwind 类（应使用自定义 CSS 类）
- ❌ 禁止在多个组件中重复定义相同的样式
- ❌ **禁止硬编码颜色、圆角、阴影等设计值**（应使用 CSS 变量）
- ❌ 禁止在组件中直接设置 `document.documentElement.dataset.theme`（应使用 `useTheme().setTheme()`）

### 代码相关

- ❌ 禁止使用 `any` 类型（除非必要）
- ❌ 禁止在组件中直接操作 DOM（优先使用 Vue 响应式）
- ❌ 禁止硬编码配置值（应使用环境变量或配置文件）

### API 相关

- ❌ 禁止在前端直接拼接 SQL
- ❌ 禁止暴露敏感信息（API 密钥、密码等）
- ❌ 禁止忽略错误处理

---

## 📚 相关文档

- [样式架构规范](./STYLE_ARCHITECTURE.md) - 详细的样式架构说明和最佳实践
- [样式管理开发提醒](./STYLE_MANAGEMENT_REMINDER.md) - 样式开发流程和示例
- [模块系统文档](../architecture/README_MODULES.md)
- [Naive UI 使用指南](../config/README_NAIVE_UI.md)
- [UI 组件库文档](../config/UI_COMPONENT_LIBRARY.md)
- [样式架构重构报告](../STYLE_ARCHITECTURE_REFACTOR.md) - 重构完成报告

---

## 🔄 更新日志

- **2024-12-XX**: 初始版本，定义样式管理规范
- **2024-12-XX**: 添加访客互动模块开发规范
- **2024-12-XX**: 完善 API 和数据库规范
- **2024-12-XX**: 添加文档维护规范，要求开发过程中随时记录和更新文档
- **2024-12-03**: 重构样式架构，添加分层样式管理规范（tokens.css、base.css、ui-patch-naive.css、AppNaiveConfig）

---

## 📌 重要提醒

1. **文档更新是开发的一部分**：不要将文档更新视为额外工作，而是开发流程的必要环节
2. **提交前必查文档**：每次提交代码前，必须检查并更新相关文档
3. **文档与代码同步**：确保文档内容与代码实现保持一致
4. **AI 辅助开发**：使用 AI 辅助开发时，AI 应主动提醒文档更新需求

---

**注意**：本文档会持续更新，请定期查看最新版本。如有疑问或建议，请及时反馈。

