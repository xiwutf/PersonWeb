# 模块化系统使用指南

## 📦 模块化架构

本项目采用模块化设计，每个功能模块都是独立的，可以按需启用/禁用，降低耦合性，方便团队协作。

## 🎯 核心概念

### 1. 模块定义

每个模块包含：
- **模块标识** (`moduleKey`): 唯一标识符，如 `blog`、`projects`
- **模块名称** (`moduleName`): 显示名称，如 "技术博客"
- **路由配置**: 模块提供的页面路由
- **组件列表**: 模块使用的组件
- **依赖关系**: 依赖的其他模块
- **配置Schema**: 模块的配置项定义

### 2. 模块分类

- **core**: 核心模块（不能禁用）
- **content**: 内容模块（博客、项目等）
- **tool**: 工具模块（搜索、仪表盘等）
- **experiment**: 实验模块（AI实验室、3D场景等）
- **interaction**: 交互模块（时间胶囊等）

## 🚀 使用方法

### 在组件中使用模块保护

```vue
<template>
  <ModuleGuard module-key="blog">
    <!-- 博客相关组件 -->
    <BlogList />
  </ModuleGuard>
</template>
```

### 在代码中检查模块状态

```typescript
const { isModuleEnabled, getModuleConfig } = useModuleSystem()

// 检查模块是否启用
if (isModuleEnabled('blog')) {
  // 使用博客功能
}

// 获取模块配置
const config = getModuleConfig('blog', 'postsPerPage')
```

### 动态路由检查

```typescript
// 在页面组件中
const { isRouteEnabled } = useModuleSystem()

if (!isRouteEnabled(route.path)) {
  // 重定向到首页或显示404
  return navigateTo('/')
}
```

## 📁 模块结构

```
modules/
  blog/
    module.manifest.json    # 模块清单文件
    pages/                   # 模块页面（可选）
    components/             # 模块组件（可选）
    composables/            # 模块组合式函数（可选）
    assets/                 # 模块资源（可选）
```

## 🔧 创建新模块

### 1. 创建模块目录

```bash
mkdir -p modules/my-module
```

### 2. 创建模块清单文件

`modules/my-module/module.manifest.json`:

```json
{
  "moduleKey": "my-module",
  "moduleName": "我的模块",
  "version": "1.0.0",
  "description": "模块描述",
  "icon": "fas fa-star",
  "category": "tool",
  "dependencies": ["core"],
  "routes": [
    {
      "path": "/my-module",
      "component": "pages/my-module/index.vue"
    }
  ],
  "components": [
    {
      "name": "MyComponent",
      "path": "components/MyComponent.vue"
    }
  ]
}
```

### 3. 在数据库中注册模块

运行 SQL 插入语句或通过后台管理界面添加。

## 🎛️ 后台管理

访问 `/admin/settings/modules` 可以：
- 查看所有模块
- 启用/禁用模块
- 查看模块详情
- 管理模块配置

## ⚠️ 注意事项

1. **核心模块** (`core`) 不能禁用
2. **依赖检查**: 禁用模块前会检查是否有其他模块依赖它
3. **路由保护**: 禁用模块后，相关路由会自动重定向
4. **组件保护**: 使用 `ModuleGuard` 组件保护模块相关功能

## 📝 模块清单

当前已实现的模块：

- ✅ **core**: 核心模块
- ✅ **blog**: 技术博客
- ✅ **projects**: 项目展示
- ✅ **tools**: 插件工具
- ✅ **ai-lab**: AI实验室
- ✅ **lab-3d**: 3D实验室
- ✅ **time-capsule**: 时间胶囊
- ✅ **knowledge**: 知识库
- ✅ **timeline**: 时间线
- ✅ **skills**: 技能树
- ✅ **life**: 生活随笔
- ✅ **english**: 英语学习
- ✅ **game**: 小游戏
- ✅ **showcase**: 展示墙
- ✅ **dashboard**: 仪表盘
- ✅ **search**: 全站搜索

## 🔄 模块加载流程

1. 应用启动时，加载所有启用的模块定义
2. 注册模块的路由和组件
3. 加载模块配置
4. 应用模块权限和依赖检查

## 🎨 最佳实践

1. **模块独立性**: 每个模块应该尽可能独立，减少对其他模块的依赖
2. **配置驱动**: 使用配置而非硬编码
3. **优雅降级**: 模块禁用时提供友好的提示
4. **文档完善**: 为每个模块提供清晰的文档

