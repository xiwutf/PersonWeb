# 模块化系统架构文档

## 🎯 设计目标

1. **低耦合**: 各模块独立，互不依赖
2. **按需加载**: 只加载启用的模块
3. **易于扩展**: 新增模块无需修改核心代码
4. **团队协作**: 不同人员可以独立开发不同模块

## 📐 架构设计

### 1. 模块定义

每个模块包含以下信息：

```typescript
interface ModuleDefinition {
  moduleKey: string        // 唯一标识
  moduleName: string      // 显示名称
  moduleVersion: string   // 版本号
  description?: string    // 描述
  icon?: string          // 图标
  category?: string      // 分类
  dependencies?: string[] // 依赖的模块
  routes?: Route[]       // 路由配置
  components?: Component[] // 组件列表
  configSchema?: object  // 配置Schema
  isEnabled: boolean     // 是否启用
  isCore: boolean        // 是否核心模块
}
```

### 2. 模块生命周期

```
1. 模块注册 (Database)
   ↓
2. 模块加载 (Plugin)
   ↓
3. 模块启用检查 (Middleware)
   ↓
4. 模块使用 (Component/Page)
```

### 3. 模块目录结构

```
modules/
  {module-key}/
    module.manifest.json  # 模块清单
    pages/                 # 模块页面
    components/           # 模块组件
    composables/          # 模块组合式函数
    assets/               # 模块资源
    README.md             # 模块文档
```

## 🔌 模块接口

### 模块清单文件格式

```json
{
  "moduleKey": "blog",
  "moduleName": "技术博客",
  "version": "1.0.0",
  "description": "技术文章和博客功能",
  "icon": "fas fa-blog",
  "category": "content",
  "dependencies": ["core"],
  "routes": [
    {
      "path": "/blog",
      "name": "blog-index",
      "component": "pages/blog/index.vue"
    }
  ],
  "components": [
    {
      "name": "MarkdownEditor",
      "path": "components/MarkdownEditor.vue",
      "global": false
    }
  ],
  "configSchema": {
    "enableComments": {
      "type": "boolean",
      "default": true
    }
  }
}
```

## 🛠️ 使用示例

### 1. 在组件中使用模块保护

```vue
<template>
  <ModuleGuard module-key="blog">
    <BlogList />
    <template #fallback>
      <div>博客模块未启用</div>
    </template>
  </ModuleGuard>
</template>
```

### 2. 在代码中检查模块

```typescript
const { isModuleEnabled, getModuleConfig } = useModuleSystem()

if (isModuleEnabled('blog')) {
  const postsPerPage = getModuleConfig('blog', 'postsPerPage') || 10
  // 使用博客功能
}
```

### 3. 动态路由

```typescript
// pages/blog/index.vue
const { isRouteEnabled } = useModuleSystem()

if (!isRouteEnabled('/blog')) {
  throw createError({
    statusCode: 404,
    message: '页面不存在'
  })
}
```

## 📦 模块分类

### 核心模块 (core)
- 不能禁用
- 包含基础布局、导航等

### 内容模块 (content)
- blog: 技术博客
- projects: 项目展示
- knowledge: 知识库
- timeline: 时间线
- skills: 技能树
- life: 生活随笔

### 工具模块 (tool)
- tools: 插件工具
- search: 全站搜索
- dashboard: 仪表盘

### 实验模块 (experiment)
- ai-lab: AI实验室
- lab-3d: 3D实验室
- game: 小游戏

### 交互模块 (interaction)
- time-capsule: 时间胶囊
- showcase: 展示墙

## 🔒 权限控制

模块可以定义权限：

```json
{
  "permissions": {
    "read": "public",
    "write": "admin",
    "delete": "admin"
  }
}
```

## ⚙️ 配置管理

每个模块可以有独立的配置：

```typescript
// 设置配置
await setModuleConfig('blog', 'postsPerPage', 20)

// 获取配置
const config = getModuleConfig('blog', 'postsPerPage')
```

## 🚀 部署建议

1. **开发环境**: 启用所有模块
2. **生产环境**: 根据需求启用必要模块
3. **按团队分配**: 不同团队负责不同模块

## 📝 最佳实践

1. **模块独立性**: 尽量减少模块间依赖
2. **配置驱动**: 使用配置而非硬编码
3. **优雅降级**: 模块禁用时提供友好提示
4. **文档完善**: 为每个模块提供清晰文档
5. **版本管理**: 使用语义化版本号

