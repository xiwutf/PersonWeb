# 项目开发规范文档

本文档定义了本项目的开发规范和要求，所有开发工作（包括AI辅助开发）都应严格遵循这些规范。

## 📋 目录

1. [样式管理规范](#样式管理规范)
2. [代码组织规范](#代码组织规范)
3. [命名规范](#命名规范)
4. [组件开发规范](#组件开发规范)
5. [API 开发规范](#api-开发规范)
6. [数据库规范](#数据库规范)
7. [Git 提交规范](#git-提交规范)
8. [最佳实践](#最佳实践)

---

## 🎨 样式管理规范

### 核心原则

**所有样式必须统一管理，禁止在 template 中使用内联样式（`:style` 除外，但需最小化使用）**

### 样式组织方式

#### 1. 组件级样式（`<style scoped>`）

- **用途**：组件特有的样式
- **位置**：组件文件内的 `<style scoped>` 块
- **示例**：
```vue
<style scoped>
.component-container {
  /* 组件特有样式 */
}
</style>
```

#### 2. 全局统一样式（`assets/css/`）

- **用途**：跨组件复用的样式类
- **位置**：`assets/css/` 目录下的 CSS 文件
- **命名规范**：使用功能前缀，如 `visitor-*`、`admin-*` 等
- **示例**：
  - `assets/css/visitor-interaction.css` - 访客互动统一样式
  - `assets/css/themes.css` - 主题样式
  - `assets/css/main.css` - 全局基础样式

#### 3. 动态样式（`:style` 绑定）

- **使用场景**：仅用于必须动态计算的属性
- **允许的属性**：
  - 位置（`left`, `top`, `transform`）
  - 颜色（`color`, `backgroundColor`）- 仅当需要运行时计算时
  - 动画参数（`animationDuration`, `animationDelay`）
- **禁止的属性**：
  - 固定尺寸（应使用 CSS 类）
  - 固定颜色（应使用 CSS 类）
  - 布局属性（应使用 CSS 类）

#### 4. 样式类命名规范

```css
/* 功能前缀 + 元素类型 + 状态/变体 */
.visitor-modal-overlay      /* 访客-模态框-遮罩层 */
.visitor-form-input         /* 访客-表单-输入框 */
.visitor-button-primary     /* 访客-按钮-主要 */
.visitor-emoji-option-selected  /* 访客-表情-选项-选中状态 */
```

### 样式提取规则

1. **重复样式** → 提取到统一样式文件
2. **组件特有样式** → 保留在组件 `<style scoped>` 中
3. **动态计算样式** → 使用 `:style` 绑定（最小化）

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
│   └── css/              # 全局样式文件
│       ├── main.css      # 基础样式
│       ├── themes.css    # 主题样式
│       └── visitor-interaction.css  # 功能模块统一样式
├── components/           # Vue 组件
│   ├── home/            # 首页相关组件
│   ├── admin/           # 后台管理组件
│   └── [功能模块]/      # 按功能模块组织
├── pages/               # 页面路由
│   ├── admin/           # 后台管理页面
│   └── [功能模块]/      # 按功能模块组织
├── composables/         # 组合式函数
├── layouts/             # 布局组件
├── middleware/          # 路由中间件
├── plugins/             # 插件
├── types/               # TypeScript 类型定义
├── backend/             # 后端代码（C#）
│   └── PersonalSite.Api/
│       ├── Controllers/  # API 控制器
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

## ✅ 最佳实践

### 1. 样式管理

- ✅ 所有样式放在 `<style scoped>` 或统一样式文件中
- ✅ 使用 CSS 类而非内联样式
- ✅ 动态样式仅用于必须运行时计算的属性
- ✅ 提取可复用的样式到统一样式文件

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

---

## 🚫 禁止事项

### 样式相关

- ❌ 禁止在 template 中使用内联 `style` 属性（固定样式）
- ❌ 禁止在 template 中使用 Tailwind 类（应使用自定义 CSS 类）
- ❌ 禁止在多个组件中重复定义相同的样式

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

- [模块系统文档](./README_MODULES.md)
- [Naive UI 使用指南](./README_NAIVE_UI.md)
- [UI 组件库文档](./config/UI_COMPONENT_LIBRARY.md)

---

## 🔄 更新日志

- **2024-12-XX**: 初始版本，定义样式管理规范
- **2024-12-XX**: 添加访客互动模块开发规范
- **2024-12-XX**: 完善 API 和数据库规范

---

**注意**：本文档会持续更新，请定期查看最新版本。如有疑问或建议，请及时反馈。

