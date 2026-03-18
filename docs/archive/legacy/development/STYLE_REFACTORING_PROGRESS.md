# 样式重构进度文档

## 📋 概述

本项目正在进行样式统一管理重构，将所有在 template 中直接使用的 Tailwind 类提取到独立的 CSS 文件中，实现样式统一管理。

## ✅ 已完成

### 1. 样式文件创建
- ✅ `assets/css/about.css` - 关于页面样式
- ✅ `assets/css/tools.css` - 工具页面样式
- ✅ `assets/css/life.css` - 生活随笔页面样式
- ✅ `assets/css/blog.css` - 博客页面样式
- ✅ `assets/css/projects.css` - 项目页面样式

### 2. 页面重构
- ✅ `pages/about.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/tools/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/life/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/blog/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类
- ✅ `pages/projects/index.vue` - 已移除所有 Tailwind 类，使用自定义 CSS 类

### 3. 配置更新
- ✅ `nuxt.config.ts` - 已添加新的样式文件引用

## 🔄 进行中

### 待创建的样式文件
- ⏳ `assets/css/common.css` - 通用页面样式（用于其他页面）

### 待重构的页面
- ⏳ `pages/projects/detail-[slug].vue`
- ⏳ `pages/tools/marketplace.vue`
- ⏳ `pages/tools/collections.vue`
- ⏳ `pages/tools/my-tools.vue`
- ⏳ `pages/tools/detail-[slug].vue`
- ⏳ `pages/tools/[slug].vue`
- ⏳ `pages/index.vue` - 首页
- ⏳ `pages/search.vue` - 搜索页
- ⏳ `pages/skills/index.vue` - 技能页
- ⏳ `pages/knowledge/index.vue` - 知识库页
- ⏳ 其他页面...

## 📝 重构规范

### 样式文件命名
- 使用功能模块前缀，如 `about-*`、`tools-*`、`life-*`
- 文件名使用 kebab-case，如 `about.css`、`tools.css`

### CSS 类命名规范
- 使用功能前缀 + 元素类型 + 状态/变体
- 示例：
  - `.about-contact-card` - 关于页面-联系方式-卡片
  - `.tools-card-header` - 工具页面-卡片-头部
  - `.life-post-title` - 生活页面-文章-标题

### 重构步骤
1. 创建对应的 CSS 文件
2. 提取所有 Tailwind 类到 CSS 文件
3. 在 `nuxt.config.ts` 中引入样式文件
4. 重构 Vue 文件，替换 Tailwind 类为自定义 CSS 类
5. 测试页面显示效果

## 🎯 目标

- [x] 主要页面移除 template 中的 Tailwind 类
- [x] 所有样式统一在 CSS 文件中管理
- [x] 支持在后台管理系统中统一管理样式

## ✅ 样式配置系统

### 1. 数据库表
- ✅ `frontend_page_style` - 页面样式配置表
- ✅ `frontend_style_variable` - 样式变量表
- ✅ `frontend_style_rule` - 样式规则表

### 2. 后端 API
- ✅ `FrontendStyleController` - 前端样式配置 API 控制器
  - **位置**: `backend/PersonalSite.Api/Controllers/FrontendStyleController.cs`
  - **命名空间**: `PersonalSite.Api.Controllers`
  - **路由**: `/api/frontend-styles`
  
  **API 接口列表**:
  - `GET /api/frontend-styles/page/{pageKey}` - 获取页面样式配置（公开接口）
  - `GET /api/frontend-styles/pages` - 获取所有页面样式配置列表（需认证）
  - `PUT /api/frontend-styles/page/{pageKey}` - 创建或更新页面样式配置（需认证）
  - `GET /api/frontend-styles/variables/{pageKey}` - 获取页面样式变量（公开接口）
  - `PUT /api/frontend-styles/variables/{pageKey}/{variableKey}` - 创建或更新样式变量（需认证）
  - `GET /api/frontend-styles/rules/{pageKey}` - 获取页面样式规则（公开接口）
  - `PUT /api/frontend-styles/rules/{pageKey}` - 创建或更新样式规则（需认证）
  - `DELETE /api/frontend-styles/rules/{id}` - 删除样式规则（需认证）

### 2.1 后端实体类
- ✅ **位置**: `backend/PersonalSite.Api/Models/`（需要在 Models 文件夹中创建）
- ✅ `FrontendPageStyle` - 页面样式配置实体
  - `Id` - 主键
  - `PageKey` - 页面标识（唯一）
  - `PageName` - 页面名称
  - `StyleConfig` - 样式配置 JSON 字符串
  - `Enabled` - 是否启用
  - `IsDefault` - 是否默认
  - `Version` - 版本号
  - `CreatedAt` / `UpdatedAt` - 时间戳

- ✅ `FrontendStyleVariable` - 样式变量实体
  - `Id` - 主键
  - `PageKey` - 页面标识
  - `VariableKey` - 变量键名（与 PageKey 组成唯一索引）
  - `VariableValue` - 变量值
  - `VariableType` - 变量类型（color, size, spacing, font, other）
  - `Description` - 描述

- ✅ `FrontendStyleRule` - 样式规则实体
  - `Id` - 主键
  - `PageKey` - 页面标识
  - `Selector` - CSS 选择器
  - `CssProperties` - CSS 属性 JSON 字符串
  - `Priority` - 优先级
  - `Enabled` - 是否启用
  - `Description` - 描述

### 2.2 数据库配置
- ✅ **数据库表脚本**: `database/frontend_page_styles_tables.sql`
- ⚠️ **需要在 `AppDbContext.cs` 中添加 DbSet**:
  ```csharp
  public DbSet<FrontendPageStyle> FrontendPageStyles { get; set; }
  public DbSet<FrontendStyleVariable> FrontendStyleVariables { get; set; }
  public DbSet<FrontendStyleRule> FrontendStyleRules { get; set; }
  ```

### 3. 前端 Composable
- ✅ `composables/usePageStyle.ts` - 页面样式配置 composable
  - 动态加载样式配置
  - 应用 CSS 变量和规则
  - 支持样式更新

### 4. 后台管理页面
- ✅ `pages/admin/settings/frontend-styles.vue` - 前端页面样式管理页面
  - 基础样式配置（颜色、字体、间距等）
  - 样式变量管理
  - 自定义样式规则管理
  - 实时预览功能

## 📖 使用指南

### 在页面中使用样式配置

#### 1. 引入 composable
```vue
<script setup lang="ts">
// 在页面中引入样式配置
const { styleConfig, loading, error } = usePageStyle('tools')

// styleConfig 包含所有配置的样式属性
// loading 表示是否正在加载
// error 表示是否有错误
</script>
```

#### 2. 样式自动应用
样式配置会自动应用到页面，通过以下方式：
- CSS 变量：`--tools-primary-color`, `--tools-background-color` 等
- 自定义 CSS 规则：通过样式规则表配置的规则

#### 3. 访问 CSS 变量
```vue
<template>
  <div :style="{ color: getCssVar('primaryColor') }">
    使用 CSS 变量
  </div>
</template>

<script setup lang="ts">
const { getCssVar } = usePageStyle('tools')
</script>
```

### 在后台管理中配置样式

#### 1. 访问管理页面
- 路径：`/admin/settings/frontend-styles`
- 需要管理员权限

#### 2. 配置基础样式
1. 选择要配置的页面（tools、blog、life、projects、about）
2. 修改基础样式配置：
   - 主色调、次色调
   - 背景色、文字颜色
   - 卡片背景、边框颜色
   - 圆角大小、字体族
3. 点击"保存配置"

#### 3. 管理样式变量
1. 点击"新增变量"
2. 填写变量键名、变量值、变量类型
3. 保存后变量会自动应用到页面

#### 4. 管理样式规则
1. 点击"新增规则"
2. 填写 CSS 选择器和 CSS 属性（JSON 格式）
3. 设置优先级和启用状态
4. 保存后规则会自动应用到页面

#### 5. 实时预览
在配置页面底部可以看到实时预览效果，修改配置后立即看到效果。

### 数据库迁移

#### 1. 执行 SQL 脚本
```bash
# 执行数据库表创建脚本
mysql -u username -p database_name < database/frontend_page_styles_tables.sql
```

#### 2. 在 AppDbContext 中注册实体
在 `backend/PersonalSite.Api/Data/AppDbContext.cs` 中添加：
```csharp
public DbSet<FrontendPageStyle> FrontendPageStyles { get; set; }
public DbSet<FrontendStyleVariable> FrontendStyleVariables { get; set; }
public DbSet<FrontendStyleRule> FrontendStyleRules { get; set; }
```

#### 3. 创建实体类文件
在 `backend/PersonalSite.Api/Models/` 目录下创建实体类文件，或从 Controller 中提取实体类到 Models 文件夹。

### API 使用示例

#### 获取页面样式配置
```typescript
const api = useApi()
const style = await api.get('/frontend-styles/page/tools')
// 返回: { pageKey, pageName, styleConfig, enabled, version }
```

#### 更新页面样式配置
```typescript
await api.put('/frontend-styles/page/tools', {
  pageName: '工具页面',
  styleConfig: {
    primaryColor: '#f97316',
    secondaryColor: '#dc2626',
    // ... 其他配置
  }
})
```

#### 获取样式变量
```typescript
const variables = await api.get('/frontend-styles/variables/tools')
// 返回: [{ id, pageKey, variableKey, variableValue, variableType, description }]
```

#### 更新样式变量
```typescript
await api.put('/frontend-styles/variables/tools/card-padding', {
  variableValue: '1.5rem',
  variableType: 'spacing',
  description: '卡片内边距'
})
```

#### 获取样式规则
```typescript
const rules = await api.get('/frontend-styles/rules/tools')
// 返回: [{ id, pageKey, selector, cssProperties, priority, enabled, description }]
```

#### 创建样式规则
```typescript
await api.put('/frontend-styles/rules/tools', {
  selector: '.tools-card:hover',
  cssProperties: {
    'transform': 'translateY(-4px)',
    'box-shadow': '0 10px 30px rgba(249, 115, 22, 0.3)'
  },
  priority: 10,
  enabled: true,
  description: '卡片悬停效果'
})
```

## 🔧 技术细节

### 样式应用机制

1. **CSS 变量应用**
   - 从 `styleConfig` 生成 CSS 变量，格式：`--{pageKey}-{key}`
   - 例如：`--tools-primary-color`, `--blog-background-color`

2. **样式规则应用**
   - 从数据库加载样式规则
   - 按优先级排序后应用
   - 动态注入到 `<style>` 标签中

3. **降级处理**
   - API 失败时使用默认样式配置
   - 确保页面始终有样式显示

### 性能优化

- 样式配置支持版本号，可用于缓存控制
- 样式规则按优先级排序，避免冲突
- 样式变量支持类型分类，便于管理

## 📚 参考文档

- [开发规范文档](./DEVELOPMENT_GUIDELINES.md)
- [数据库设计原则](../../database/DESIGN_PRINCIPLES.md)
- [API 文档](../../backend/PersonalSite.Api/Controllers/FrontendStyleController.cs)

