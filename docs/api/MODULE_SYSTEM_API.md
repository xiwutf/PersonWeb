# 模块系统 API 文档

## 概述

PersonWeb 模块系统提供了一套完整的 API，用于创建、管理、安装和使用模块。本文档详细介绍了所有可用的 API 接口。

## 目录

1. [模块管理 API](#模块管理-api)
2. [模块存储 API](#模块存储-api)
3. [模块验证 API](#模块验证-api)
4. [模块市场 API](#模块市场-api)
5. [模块构建 API](#模块构建-api)
6. [事件系统 API](#事件系统-api)
7. [工具函数 API](#工具函数-api)
8. [类型定义](#类型定义)

---

## 模块管理 API

### IModuleManager

模块管理器接口，提供模块的基础操作。

#### install

安装模块

```typescript
/**
 * 安装模块
 * @param moduleKey 模块键
 * @param options 安装选项
 * @returns 模块实例
 */
install(moduleKey: string, options?: ModuleInstallOptions): Promise<ModuleInstance>
```

**参数：**
- `moduleKey`: 模块唯一标识符
- `options`: 可选的安装选项

**ModuleInstallOptions:**
```typescript
interface ModuleInstallOptions {
  config?: Record<string, any>     // 模块配置
  activate?: boolean                // 是否激活
  overwrite?: boolean              // 是否覆盖现有配置
  path?: string                    // 安装路径
}
```

**返回：**
`Promise<ModuleInstance>`

**示例：**
```typescript
const moduleManager = useModuleManager()
const instance = await moduleManager.install('blog-module', {
  config: {
    postsPerPage: 10
  },
  activate: true
})
```

#### uninstall

卸载模块

```typescript
/**
 * 卸载模块
 * @param moduleKey 模块键
 * @returns 是否成功卸载
 */
uninstall(moduleKey: string): Promise<boolean>
```

**示例：**
```typescript
await moduleManager.uninstall('blog-module')
```

#### activate

激活模块

```typescript
/**
 * 激活模块
 * @param moduleKey 模块键
 * @returns 是否成功激活
 */
activate(moduleKey: string): Promise<boolean>
```

#### deactivate

停用模块

```typescript
/**
 * 停用模块
 * @param moduleKey 模块键
 * @returns 是否成功停用
 */
deactivate(moduleKey: string): Promise<boolean>
```

#### getModule

获取模块实例

```typescript
/**
 * 获取模块实例
 * @param moduleKey 模块键
 * @returns 模块实例或 null
 */
getModule(moduleKey: string): ModuleInstance | null
```

#### listModules

列出模块

```typescript
/**
 * 列出模块
 * @param filter 过滤器
 * @returns 模块实例列表
 */
listModules(filter?: ModuleFilter): ModuleInstance[]
```

#### getStatus

获取模块状态

```typescript
/**
 * 获取模块状态
 * @param moduleKey 模块键
 * @returns 模块状态
 */
getStatus(moduleKey: string): ModuleStatus
```

#### updateConfig

更新模块配置

```typescript
/**
 * 更新模块配置
 * @param moduleKey 模块键
 * @param config 配置对象
 * @returns 是否成功更新
 */
updateConfig(moduleKey: string, config: Record<string, any>): Promise<boolean>
```

#### getConfig

获取模块配置

```typescript
/**
 * 获取模块配置
 * @param moduleKey 模块键
 * @returns 配置对象
 */
getConfig(moduleKey: string): Record<string, any>
```

#### executeHook

执行模块生命周期钩子

```typescript
/**
 * 执行模块生命周期钩子
 * @param moduleKey 模块键
 * @param hookName 钩子名称
 * @param args 参数
 * @returns 钩子执行结果
 */
executeHook(moduleKey: string, hookName: string, ...args: any[]): Promise<any>
```

---

## 模块存储 API

### IModuleStorage

模块存储接口，负责模块实例和配置的持久化存储。

#### save

保存模块实例

```typescript
/**
 * 保存模块实例
 * @param instance 模块实例
 * @returns 是否成功保存
 */
save(instance: ModuleInstance): Promise<boolean>
```

#### load

加载模块实例

```typescript
/**
 * 加载模块实例
 * @param moduleKey 模块键
 * @returns 模块实例或 null
 */
load(moduleKey: string): Promise<ModuleInstance | null>
```

#### delete

删除模块实例

```typescript
/**
 * 删除模块实例
 * @param moduleKey 模块键
 * @returns 是否成功删除
 */
delete(moduleKey: string): Promise<boolean>
```

#### list

列出模块实例

```typescript
/**
 * 列出模块实例
 * @param filter 过滤器
 * @returns 模块实例列表
 */
list(filter?: ModuleFilter): Promise<ModuleInstance[]>
```

#### saveConfig

保存模块配置

```typescript
/**
 * 保存模块配置
 * @param moduleKey 模块键
 * @param config 配置对象
 * @returns 是否成功保存
 */
saveConfig(moduleKey: string, config: Record<string, any>): Promise<boolean>
```

#### loadConfig

加载模块配置

```typescript
/**
 * 加载模块配置
 * @param moduleKey 模块键
 * @returns 配置对象或 null
 */
loadConfig(moduleKey: string): Promise<Record<string, any> | null>
```

---

## 模块验证 API

### IModuleValidator

模块验证接口，用于验证模块定义、依赖和权限。

#### validateDefinition

验证模块定义

```typescript
/**
 * 验证模块定义
 * @param definition 模块定义
 * @returns 验证结果
 */
validateDefinition(definition: ModuleDefinition): Promise<ValidationResult>
```

**ValidationResult:**
```typescript
interface ValidationResult {
  valid: boolean
  errors: ValidationError[]
  warnings: ValidationWarning[]
}
```

#### validateDependencies

验证模块依赖

```typescript
/**
 * 验证模块依赖
 * @param definition 模块定义
 * @param installedModules 已安装模块列表
 * @returns 依赖验证结果
 */
validateDependencies(definition: ModuleDefinition, installedModules: string[]): Promise<DependencyResult>
```

**DependencyResult:**
```typescript
interface DependencyResult {
  valid: boolean
  missing: string[]
  conflicts: Array<{
    module: string
    version: string
    required: string
  }>
}
```

#### validatePermissions

验证模块权限

```typescript
/**
 * 验证模块权限
 * @param permissions 权限列表
 * @returns 权限验证结果
 */
validatePermissions(permissions: ModulePermission[]): Promise<PermissionResult>
```

**PermissionResult:**
```typescript
interface PermissionResult {
  valid: boolean
  missing: string[]
  excess: string[]
}
```

---

## 模块市场 API

### IModuleMarket

模块市场接口，用于搜索、获取和下载模块。

#### search

搜索模块

```typescript
/**
 * 搜索模块
 * @param query 搜索查询
 * @param category 分类筛选
 * @returns 市场项目列表
 */
search(query: string, category?: string): Promise<ModuleMarketItem[]>
```

#### getDetail

获取模块详情

```typescript
/**
 * 获取模块详情
 * @param moduleKey 模块键
 * @returns 市场项目或 null
 */
getDetail(moduleKey: string): Promise<ModuleMarketItem | null>
```

#### download

下载模块

```typescript
/**
 * 下载模块
 * @param moduleKey 模块键
 * @param version 版本号（可选）
 * @returns 模块清单
 */
download(moduleKey: string, version?: string): Promise<ModuleManifest>
```

#### install

从市场安装模块

```typescript
/**
 * 从市场安装模块
 * @param moduleKey 模块键
 * @param options 安装选项
 * @returns 模块实例
 */
install(moduleKey: string, options?: any): Promise<ModuleInstance>
```

---

## 模块构建 API

### IModuleBuilder

模块构建器接口，用于创建、打包和发布模块。

#### create

创建模块

```typescript
/**
 * 创建新模块
 * @param name 模块名称
 * @param options 创建选项
 * @returns 模块清单
 */
create(name: string, options: ModuleCreateOptions): Promise<ModuleManifest>
```

**ModuleCreateOptions:**
```typescript
interface ModuleCreateOptions {
  category: string                    // 分类
  author: string                     // 作者
  license?: string                    // 许可证
  dependencies?: string[]            // 依赖
  routes?: ModuleRoute[]              // 路由
  components?: ModuleComponent[]      // 组件
  configs?: ModuleConfig[]           // 配置
  permissions?: ModulePermission[]   // 权限
}
```

#### build

构建模块

```typescript
/**
 * 构建模块
 * @param manifest 模块清单
 * @returns 构产包路径
 */
build(manifest: ModuleManifest): Promise<string>
```

#### publish

发布模块

```typescript
/**
 * 发布模块
 * @param manifest 模块清单
 * @param repository 仓库地址
 * @returns 发布ID
 */
publish(manifest: ModuleManifest, repository: string): Promise<string>
```

---

## 事件系统 API

### IModuleEventBus

模块事件总线接口，用于模块间通信。

#### on

监听事件

```typescript
/**
 * 监听事件
 * @param event 事件名称
 * @param handler 事件处理函数
 */
on(event: string, handler: (event: ModuleEvent) => void): void
```

#### off

移除事件监听

```typescript
/**
 * 移除事件监听
 * @param event 事件名称
 * @param handler 事件处理函数
 */
off(event: string, handler: (event: ModuleEvent) => void): void
```

#### emit

触发事件

```typescript
/**
 * 触发事件
 * @param event 事件名称
 * @param data 事件数据
 */
emit(event: string, data?: any): void
```

#### once

一次性事件监听

```typescript
/**
 * 一次性事件监听
 * @param event 事件名称
 * @param handler 事件处理函数
 */
once(event: string, handler: (event: ModuleEvent) => void): void
```

---

## 工具函数 API

### IModuleTools

模块工具接口，提供常用工具方法。

#### createRoute

创建路由配置

```typescript
/**
 * 创建路由配置
 * @param path 路径
 * @param component 组件
 * @returns 路由配置
 */
createRoute(path: string, component?: string): ModuleRoute
```

#### createComponent

创建组件配置

```typescript
/**
 * 创建组件配置
 * @param name 组件名称
 * @param path 组件路径
 * @returns 组件配置
 */
createComponent(name: string, path: string): ModuleComponent
```

#### createConfig

创建配置项

```typescript
/**
 * 创建配置项
 * @param key 配置键
 * @param value 配置值
 * @returns 配置项
 */
createConfig(key: string, value: any): ModuleConfig
```

#### createPermission

创建权限项

```typescript
/**
 * 创建权限项
 * @param key 权限键
 * @param name 权限名称
 * @param level 权限级别
 * @returns 权限项
 */
createPermission(key: string, name: string, level: string): ModulePermission
```

---

## 类型定义

### 核心类型

#### ModuleStatus

模块状态枚举

```typescript
enum ModuleStatus {
  NOT_INSTALLED = 'not_installed',  // 未安装
  INSTALLED = 'installed',         // 已安装
  ACTIVE = 'active',               // 已激活
  INACTIVE = 'inactive',           // 已停用
  ERROR = 'error'                  // 错误
}
```

#### ModuleCategory

模块分类枚举

```typescript
enum ModuleCategory {
  AI = 'ai',                       // AI
  VISITOR = 'visitor',             // 访客
  3D = '3d',                       // 3D
  ADMIN = 'admin',                 // 管理
  PERFORMANCE = 'performance',     // 性能
  I18N = 'i18n',                   // 国际化
  TOOLS = 'tools',                 // 工具
  UI = 'ui',                       // UI
  LAYOUT = 'layout',               // 布局
  CONTENT = 'content',             // 内容
  SECURITY = 'security',           // 安全
  ANALYTICS = 'analytics'          // 分析
}
```

#### ModuleDefinition

模块定义接口

```typescript
interface ModuleDefinition {
  meta: ModuleMetadata              // 元数据
  routes?: ModuleRoute[]            // 路由
  components?: ModuleComponent[]    // 组件
  permissions?: ModulePermission[]  // 权限
  configs?: ModuleConfig[]         // 配置
  migrations?: ModuleMigration[]    // 迁移脚本
  lifecycle?: ModuleLifecycle      // 生命周期钩子
  entry?: string                   // 入口文件
}
```

#### ModuleManifest

模块清单接口

```typescript
interface ModuleManifest {
  module: ModuleDefinition         // 模块定义
  build: {                         // 构建信息
    timestamp: string              // 时间戳
    hash: string                  // 哈希值
    size: number                  // 大小
  }
  dependencies: Record<string, string>  // 依赖信息
}
```

#### ModuleInstance

模块实例接口

```typescript
interface ModuleInstance {
  definition: ModuleDefinition     // 模块定义
  path: string                    // 安装路径
  status: ModuleStatus            // 状态
  activatedAt?: Date              // 激活时间
  config: Record<string, any>     // 配置
  error?: string                  // 错误信息
}
```

### 辅助类型

#### ModuleEvent

模块事件接口

```typescript
interface ModuleEvent {
  type: string                    // 事件类型
  moduleKey: string              // 模块键
  timestamp: number               // 时间戳
  data?: any                      // 事件数据
}
```

#### ModuleFilter

模块过滤器接口

```typescript
interface ModuleFilter {
  category?: ModuleCategory       // 分类
  author?: string                 // 作者
  keyword?: string                // 关键词
  free?: boolean                  // 是否免费
  installed?: boolean             // 是否已安装
  active?: boolean                // 是否已激活
}
```

---

## 错误处理

所有 API 方法都可能抛出以下类型的错误：

- `ModuleError`: 模块系统通用错误
- `ValidationError`: 验证错误
- `NotFoundError`: 资源未找到错误
- `PermissionError`: 权限错误
- `NetworkError`: 网络错误

### 错误处理示例

```typescript
try {
  const instance = await moduleManager.install('my-module')
  console.log('模块安装成功:', instance)
} catch (error) {
  if (error instanceof ValidationError) {
    console.error('验证失败:', error.errors)
  } else if (error instanceof NetworkError) {
    console.error('网络错误:', error.message)
  } else {
    console.error('未知错误:', error)
  }
}
```

---

## 使用示例

### 基本使用

```typescript
import { useModuleManager } from '@personweb/module-system'

// 获取模块管理器
const moduleManager = useModuleManager()

// 安装模块
await moduleManager.install('blog-module', {
  config: {
    postsPerPage: 10
  },
  activate: true
})

// 获取模块信息
const module = moduleManager.getModule('blog-module')
if (module) {
  console.log('模块状态:', module.status)
  console.log('模块配置:', module.config)
}

// 列出所有模块
const modules = moduleManager.listModules({
  category: 'content',
  active: true
})
```

### 模块验证

```typescript
import { useModuleValidator } from '@personweb/module-system'

const validator = useModuleValidator()

// 验证模块定义
const result = await validator.validateDefinition(moduleDefinition)
if (!result.valid) {
  console.error('模块定义验证失败:', result.errors)
}

// 验证依赖
const dependencyResult = await validator.validateDependencies(
  moduleDefinition,
  ['auth-module', 'ui-components']
)
if (!dependencyResult.valid) {
  console.error('缺少依赖:', dependencyResult.missing)
}
```

### 事件监听

```typescript
import { useModuleEventBus } from '@personweb/module-system'

const eventBus = useModuleEventBus()

// 监听模块事件
eventBus.on('module:installed', (event) => {
  console.log('模块已安装:', event.moduleKey)
})

// 触发事件
eventBus.emit('module:installed', {
  moduleKey: 'blog-module',
  timestamp: Date.now()
})
```

---

## 版本历史

### v1.0.0 (2024-03-13)
- 初始版本发布
- 基础模块管理功能
- 模块验证机制
- 事件系统支持
- 市场集成接口

---

## 常见问题

### Q: 如何获取模块管理器实例？
A: 使用 `useModuleManager()` 组合式函数。

### Q: 模块安装失败如何处理？
A: 检查错误类型，如果是依赖缺失，先安装依赖模块。

### Q: 如何监听模块状态变化？
A: 使用事件总线监听 `module:status-changed` 事件。

---

更多详细信息请参考[模块开发指南](../development/MODULE_DEVELOPMENT_GUIDE.md)。