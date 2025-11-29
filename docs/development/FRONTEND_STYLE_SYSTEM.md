# 前端页面样式配置系统文档

## 📋 概述

前端页面样式配置系统允许在后台管理系统中动态配置前端页面的样式，无需修改代码即可调整页面的颜色、字体、间距等样式属性。

## 🏗️ 系统架构

### 1. 数据库层

#### 表结构

**frontend_page_style** - 页面样式配置表
```sql
- id: BIGINT (主键)
- page_key: VARCHAR(100) (唯一索引) - 页面标识
- page_name: VARCHAR(100) - 页面名称
- style_config: JSON - 样式配置
- enabled: TINYINT(1) - 是否启用
- is_default: TINYINT(1) - 是否默认
- version: INT - 版本号
- created_at: DATETIME
- updated_at: DATETIME
```

**frontend_style_variable** - 样式变量表
```sql
- id: BIGINT (主键)
- page_key: VARCHAR(100) - 页面标识
- variable_key: VARCHAR(100) - 变量键名
- variable_value: TEXT - 变量值
- variable_type: VARCHAR(50) - 变量类型
- description: VARCHAR(255) - 描述
- created_at: DATETIME
- updated_at: DATETIME
- 唯一索引: (page_key, variable_key)
```

**frontend_style_rule** - 样式规则表
```sql
- id: BIGINT (主键)
- page_key: VARCHAR(100) - 页面标识
- selector: VARCHAR(255) - CSS 选择器
- css_properties: JSON - CSS 属性
- priority: INT - 优先级
- enabled: TINYINT(1) - 是否启用
- description: VARCHAR(255) - 描述
- created_at: DATETIME
- updated_at: DATETIME
```

### 2. 后端层

#### Controller
- **文件**: `backend/PersonalSite.Api/Controllers/FrontendStyleController.cs`
- **命名空间**: `PersonalSite.Api.Controllers`
- **路由**: `/api/frontend-styles`

#### 实体类
- **位置**: `backend/PersonalSite.Api/Models/`
- `FrontendPageStyle` - 页面样式配置实体
- `FrontendStyleVariable` - 样式变量实体
- `FrontendStyleRule` - 样式规则实体

#### DbContext 配置
需要在 `AppDbContext.cs` 中添加：
```csharp
public DbSet<FrontendPageStyle> FrontendPageStyles { get; set; }
public DbSet<FrontendStyleVariable> FrontendStyleVariables { get; set; }
public DbSet<FrontendStyleRule> FrontendStyleRules { get; set; }
```

### 3. 前端层

#### Composable
- **文件**: `composables/usePageStyle.ts`
- **功能**: 动态加载和应用样式配置

#### 管理页面
- **文件**: `pages/admin/settings/frontend-styles.vue`
- **路径**: `/admin/settings/frontend-styles`
- **功能**: 可视化配置页面样式

## 🔌 API 接口

### 公开接口（无需认证）

#### 获取页面样式配置
```
GET /api/frontend-styles/page/{pageKey}
```

**响应示例**:
```json
{
  "success": true,
  "data": {
    "pageKey": "tools",
    "pageName": "工具页面",
    "styleConfig": {
      "primaryColor": "#f97316",
      "secondaryColor": "#dc2626",
      "backgroundColor": "#0f172a",
      "textColor": "#e2e8f0",
      "cardBackground": "rgba(30, 41, 59, 0.3)",
      "borderColor": "rgba(255, 255, 255, 0.05)",
      "borderRadius": "1.5rem",
      "fontFamily": "\"Outfit\", sans-serif"
    },
    "enabled": true,
    "version": 1
  }
}
```

#### 获取样式变量
```
GET /api/frontend-styles/variables/{pageKey}
```

**响应示例**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "pageKey": "tools",
      "variableKey": "card-padding",
      "variableValue": "1.5rem",
      "variableType": "spacing",
      "description": "卡片内边距"
    }
  ]
}
```

#### 获取样式规则
```
GET /api/frontend-styles/rules/{pageKey}
```

**响应示例**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "pageKey": "tools",
      "selector": ".tools-card:hover",
      "cssProperties": {
        "transform": "translateY(-4px)",
        "box-shadow": "0 10px 30px rgba(249, 115, 22, 0.3)"
      },
      "priority": 10,
      "enabled": true,
      "description": "卡片悬停效果"
    }
  ]
}
```

### 管理接口（需要认证）

#### 获取所有页面样式配置
```
GET /api/frontend-styles/pages
Authorization: Bearer {token}
```

#### 更新页面样式配置
```
PUT /api/frontend-styles/page/{pageKey}
Authorization: Bearer {token}
Content-Type: application/json

{
  "pageName": "工具页面",
  "styleConfig": {
    "primaryColor": "#f97316",
    "secondaryColor": "#dc2626",
    ...
  }
}
```

#### 更新样式变量
```
PUT /api/frontend-styles/variables/{pageKey}/{variableKey}
Authorization: Bearer {token}
Content-Type: application/json

{
  "variableValue": "1.5rem",
  "variableType": "spacing",
  "description": "卡片内边距"
}
```

#### 更新样式规则
```
PUT /api/frontend-styles/rules/{pageKey}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "selector": ".tools-card:hover",
  "cssProperties": {
    "transform": "translateY(-4px)",
    "box-shadow": "0 10px 30px rgba(249, 115, 22, 0.3)"
  },
  "priority": 10,
  "enabled": true,
  "description": "卡片悬停效果"
}
```

#### 删除样式规则
```
DELETE /api/frontend-styles/rules/{id}
Authorization: Bearer {token}
```

## 💻 前端使用

### 在页面中使用

```vue
<template>
  <div class="tools-page">
    <!-- 页面内容 -->
  </div>
</template>

<script setup lang="ts">
// 引入样式配置 composable
const { styleConfig, loading, error } = usePageStyle('tools')

// 样式会自动应用到页面
// 可以通过 styleConfig 访问配置的值
console.log(styleConfig.value?.primaryColor) // #f97316
</script>
```

### 访问 CSS 变量

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

### 更新样式配置

```typescript
const { updateStyleConfig } = usePageStyle('tools')

// 更新样式配置
await updateStyleConfig({
  primaryColor: '#ff6b35',
  secondaryColor: '#f7931e'
})
```

## 🎨 样式配置项说明

### 基础样式配置

| 配置项 | 类型 | 说明 | 示例 |
|--------|------|------|------|
| primaryColor | string | 主色调 | `#f97316` |
| secondaryColor | string | 次色调 | `#dc2626` |
| backgroundColor | string | 背景色 | `#0f172a` |
| textColor | string | 文字颜色 | `#e2e8f0` |
| cardBackground | string | 卡片背景 | `rgba(30, 41, 59, 0.3)` |
| borderColor | string | 边框颜色 | `rgba(255, 255, 255, 0.05)` |
| borderRadius | string | 圆角大小 | `1.5rem` |
| fontFamily | string | 字体族 | `"Outfit", sans-serif` |

### 样式变量类型

- `color` - 颜色值
- `size` - 尺寸值
- `spacing` - 间距值
- `font` - 字体相关
- `other` - 其他类型

## 🔄 样式应用流程

1. **页面加载时**
   - 调用 `usePageStyle(pageKey)`
   - 自动请求 API 获取样式配置
   - 应用 CSS 变量和样式规则

2. **样式应用**
   - 生成 CSS 变量：`--{pageKey}-{key}`
   - 注入样式规则到 `<style>` 标签
   - 按优先级排序规则

3. **降级处理**
   - API 失败时使用默认样式
   - 确保页面始终有样式显示

## 📝 最佳实践

1. **样式变量命名**
   - 使用语义化命名：`primary-color` 而不是 `color1`
   - 遵循 kebab-case 命名规范

2. **样式规则优先级**
   - 基础样式：0-10
   - 组件样式：10-50
   - 特殊效果：50-100

3. **性能优化**
   - 使用版本号控制缓存
   - 避免过多的样式规则
   - 合理使用 CSS 变量

4. **样式测试**
   - 在后台管理页面使用实时预览
   - 在不同设备上测试样式效果
   - 确保样式降级正常工作

## 🐛 故障排查

### 样式未应用

1. 检查 API 是否正常返回
2. 检查浏览器控制台是否有错误
3. 检查 CSS 变量是否正确生成
4. 检查样式规则是否正确注入

### 样式冲突

1. 检查样式规则的优先级
2. 检查 CSS 选择器的特异性
3. 使用浏览器开发者工具检查样式

### 性能问题

1. 减少样式规则数量
2. 合并相似的样式规则
3. 使用 CSS 变量代替重复值

## 📚 相关文档

- [样式重构进度文档](./STYLE_REFACTORING_PROGRESS.md)
- [开发规范文档](./DEVELOPMENT_GUIDELINES.md)
- [数据库设计原则](../../database/DESIGN_PRINCIPLES.md)

