# 模块系统故障排查指南

## 概述

本指南帮助您快速诊断和解决模块系统使用过程中的常见问题。从模块安装、加载到运行时问题，提供详细的解决方案。

## 目录

1. [模块安装问题](#模块安装问题)
2. [模块加载失败](#模块加载失败)
3. [模块依赖问题](#模块依赖问题)
4. [运行时错误](#运行时错误)
5. [性能问题](#性能问题)
6. [配置问题](#配置问题)
7. [调试工具](#调试工具)
8. [常见错误代码](#常见错误代码)

---

## 模块安装问题

### 问题：模块安装失败

**症状：**
```bash
$ npm install my-module
ERROR: Failed to install module my-module
```

**可能原因和解决方案：**

#### 1. 网络连接问题
```javascript
// 检查网络连接
const checkConnection = async () => {
  try {
    const response = await fetch('https://registry.npmjs.org')
    console.log('网络连接正常')
  } catch (error) {
    console.error('网络连接失败:', error)
  }
}
```

**解决方案：**
- 检查网络连接
- 使用 `npm config get registry` 确认注册表地址
- 设置代理（如果需要）：
  ```bash
  npm config set proxy http://proxy.company.com:8080
  ```

#### 2. 权限问题
```javascript
// 检查文件权限
const fs = require('fs')
const path = './modules/my-module'

if (!fs.accessSync(path, fs.constants.R_OK)) {
  console.error('没有读取权限')
}
```

**解决方案：**
- 确保有足够的文件权限
- 在 Windows 上，以管理员身份运行命令
- 在 macOS/Linux 上，检查文件所有者

#### 3. 依赖冲突
```javascript
// 检查依赖冲突
const checkDependencies = () => {
  const moduleDeps = require('./my-module/package.json').dependencies
  const projectDeps = require('./package.json').dependencies

  const conflicts = Object.keys(moduleDeps).filter(dep =>
    projectDeps[dep] && projectDeps[dep] !== moduleDeps[dep]
  )

  if (conflicts.length > 0) {
    console.error('依赖冲突:', conflicts)
  }
}
```

**解决方案：**
- 使用 `npm install --legacy-peer-deps` 安装
- 更新项目依赖以匹配模块要求
- 使用 `npm ls` 查看依赖树

---

## 模块加载失败

### 问题：模块无法加载

**症状：**
```javascript
Uncaught Error: Module not found: Error: Cannot resolve module 'my-module'
```

**解决方案：**

#### 1. 检查模块注册
```javascript
// 检查模块是否已注册
const checkModuleRegistration = (moduleKey) => {
  const registeredModules = useModuleManager().getLoadedModules()
  console.log('已注册模块:', Array.from(registeredModules.keys()))
  return registeredModules.has(moduleKey)
}
```

#### 2. 检查模块路径
```javascript
// 验证模块路径配置
const validateModulePath = () => {
  const modulePath = path.resolve('modules', 'my-module')
  if (!fs.existsSync(modulePath)) {
    console.error(`模块路径不存在: ${modulePath}`)
  }
}
```

#### 3. 检查模块清单
```javascript
// 验证 module.json
const validateModuleManifest = (modulePath) => {
  const manifestPath = path.join(modulePath, 'module.json')
  try {
    const manifest = require(manifestPath)
    console.log('模块清单:', manifest)
  } catch (error) {
    console.error('无效的模块清单:', error)
  }
}
```

#### 4. 使用开发模式
```javascript
// 启用模块调试模式
const enableDebugMode = () => {
  process.env.MODULE_DEBUG = 'true'
  console.log('模块调试模式已启用')
}
```

---

## 模块依赖问题

### 问题：循环依赖

**症状：**
```javascript
DeprecationWarning: Circular dependency detected
```

**解决方案：**

#### 1. 识别循环依赖
```javascript
// 使用依赖图工具分析
const dependencyGraph = {
  moduleA: ['moduleB'],
  moduleB: ['moduleC'],
  moduleC: ['moduleA'] // 循环依赖
}

const detectCircularDeps = (graph) => {
  const visited = new Set()
  const recursionStack = new Set()

  const dfs = (node) => {
    visited.add(node)
    recursionStack.add(node)

    for (const neighbor of graph[node] || []) {
      if (!visited.has(neighbor)) {
        if (dfs(neighbor)) return true
      } else if (recursionStack.has(neighbor)) {
        console.error(`检测到循环依赖: ${node} -> ${neighbor}`)
        return true
      }
    }

    recursionStack.delete(node)
    return false
  }

  Object.keys(graph).forEach(dfs)
}
```

#### 2. 重构代码
```javascript
// 重构前 - 循环依赖
// moduleA.js
import { funcB } from './moduleB'

// moduleB.js
import { funcA } from './moduleA'

// 重构后 - 使用事件总线
// event-bus.js
const eventBus = new EventTarget()

// moduleA.js
eventBus.addEventListener('eventB', handleEventB)

// moduleB.js
eventBus.dispatchEvent(new CustomEvent('eventA'))
```

#### 3. 使用依赖注入
```javascript
// 使用依赖注入解决循环依赖
class ServiceA {
  constructor(serviceB) {
    this.serviceB = serviceB
  }
}

class ServiceB {
  constructor() {
    // 不直接依赖 ServiceA
  }
}

// 注入依赖
const serviceB = new ServiceB()
const serviceA = new ServiceA(serviceB)
```

### 问题：依赖版本不兼容

**解决方案：**

#### 1. 使用版本范围
```json
// package.json
{
  "dependencies": {
    "some-module": "^2.0.0",  // 兼容 2.x.x
    "other-module": "~1.2.0"   // 兼容 1.2.x
  }
}
```

#### 2. 固定版本
```json
{
  "dependencies": {
    "exact-module": "1.2.3"  // 精确版本
  }
}
```

#### 3. 使用 npm audit
```bash
# 检查安全漏洞和依赖问题
npm audit

# 自动修复
npm audit fix
```

---

## 运行时错误

### 问题：模块执行失败

**症状：**
```javascript
Uncaught TypeError: Cannot read property 'xxx' of undefined
```

**调试步骤：**

#### 1. 使用错误边界
```javascript
// 组件级错误边界
export const ErrorBoundary = defineComponent({
  errorCaptured(err, instance, info) {
    console.error('组件错误:', err, info)
    return false // 阻止错误继续传播
  }
})

// 全局错误处理
app.config.errorHandler = (err, instance, info) => {
  console.error('全局错误:', err)
  sendErrorReport(err)
}
```

#### 2. 断点调试
```javascript
// 使用 console.log 调试
const debugFunction = () => {
  console.log('进入函数，参数:', arguments)
  const result = complexOperation()
  console.log('函数结果:', result)
  return result
}

// 使用断言
const assert = require('assert')
const validateInput = (input) => {
  assert(typeof input === 'string', '输入必须是字符串')
  // ... 更多验证
}
```

#### 3. 使用 Vue DevTools
```javascript
// 开发时启用调试
if (process.env.NODE_ENV === 'development') {
  const app = createApp(App)
  app.config.devtools = true
}
```

### 问题：状态管理错误

**症状：**
```javascript
Uncaught (in promise) TypeError: Cannot set property 'xxx' of undefined
```

**解决方案：**

#### 1. 使用 Pinia 调试
```javascript
// store/index.js
export const useStore = defineStore('main', {
  state: () => ({
    user: null,
    loading: false
  }),
  actions: {
    async fetchUser() {
      this.loading = true
      try {
        const user = await api.fetchUser()
        this.user = user
      } catch (error) {
        console.error('获取用户失败:', error)
      } finally {
        this.loading = false
      }
    }
  }
})
```

#### 2. 检查响应式数据
```javascript
// 检查响应式数据是否正确初始化
const store = useStore()
watch(() => store.user, (newUser) => {
  console.log('用户状态变化:', newUser)
}, { deep: true })
```

---

## 性能问题

### 问题：模块加载慢

**症状：**
应用启动缓慢，模块加载时间过长。

**解决方案：**

#### 1. 使用代码分割
```javascript
// 路由级代码分割
const routes = [
  {
    path: '/heavy-module',
    component: () => import('./modules/heavy-module/HeavyModule.vue')
  }
]

// 组件级代码分割
const HeavyComponent = defineAsyncComponent(() =>
  import('./components/HeavyComponent.vue')
)
```

#### 2. 预加载策略
```javascript
// 预加载关键模块
const preloadModules = async () => {
  const criticalModules = ['auth', 'navigation']
  const promises = criticalModules.map(key => import(`./modules/${key}`))
  await Promise.all(promises)
}
```

#### 3. 使用缓存
```javascript
// 内存缓存
const cache = new Map()

const getCachedData = async (key, fetcher) => {
  if (cache.has(key)) {
    return cache.get(key)
  }

  const data = await fetcher()
  cache.set(key, data)
  return data
}

// localStorage 缓存
const storageCache = {
  set(key, value, ttl = 3600) {
    const item = {
      value,
      expires: Date.now() + ttl * 1000
    }
    localStorage.setItem(key, JSON.stringify(item))
  },

  get(key) {
    const item = localStorage.getItem(key)
    if (!item) return null

    const parsed = JSON.parse(item)
    if (parsed.expires < Date.now()) {
      localStorage.removeItem(key)
      return null
    }

    return parsed.value
  }
}
```

### 问题：内存泄漏

**症状：**
应用运行一段时间后内存持续增长。

**解决方案：**

#### 1. 检查事件监听器
```javascript
// 组件卸载时清理事件监听
export default defineComponent({
  setup() {
    const eventBus = useEventBus()

    onMounted(() => {
      const handler = () => console.log('事件触发')
      eventBus.on('custom-event', handler)

      // 返回清理函数
      return () => {
        eventBus.off('custom-event', handler)
      }
    })
  }
})
```

#### 2. 使用弱引用
```javascript
// 使用 WeakMap 存储临时数据
const weakCache = new WeakMap()

const getCachedData = (obj) => {
  if (weakCache.has(obj)) {
    return weakCache.get(obj)
  }

  const data = expensiveOperation(obj)
  weakCache.set(obj, data)
  return data
}
```

#### 3. 检查定时器
```javascript
// 使用单例管理定时器
class TimerManager {
  static timers = new Set()

  static setInterval(fn, delay) {
    const id = setInterval(fn, delay)
    this.timers.add(id)
    return id
  }

  static clearInterval(id) {
    clearInterval(id)
    this.timers.delete(id)
  }

  static clearAllIntervals() {
    this.timers.forEach(id => clearInterval(id))
    this.timers.clear()
  }
}
```

---

## 配置问题

### 问题：模块配置错误

**症状：**
模块功能异常或不生效。

**解决方案：**

#### 1. 验证配置格式
```javascript
// 使用 JSON Schema 验证配置
const validateConfig = (config, schema) => {
  const Ajv = require('ajv')
  const ajv = new Ajv()

  const validate = ajv.compile(schema)
  const valid = validate(config)

  if (!valid) {
    console.error('配置验证失败:', validate.errors)
  }

  return valid
}

// 配置 Schema
const configSchema = {
  type: 'object',
  properties: {
    enableFeature: { type: 'boolean' },
    maxItems: { type: 'number', minimum: 1 },
    settings: {
      type: 'object',
      properties: {
        theme: { type: 'string', enum: ['light', 'dark'] }
      }
    }
  },
  required: ['enableFeature']
}
```

#### 2. 环境变量检查
```javascript
// 检查必需的环境变量
const checkEnvVariables = () => {
  const requiredVars = [
    'API_BASE_URL',
    'DATABASE_URL',
    'SECRET_KEY'
  ]

  const missingVars = requiredVars.filter(varName =>
    !process.env[varName]
  )

  if (missingVars.length > 0) {
    console.error('缺少环境变量:', missingVars)
    process.exit(1)
  }
}
```

#### 3. 默认配置
```javascript
// 提供有意义的默认值
const mergeWithDefaults = (userConfig, defaultConfig) => {
  return {
    ...defaultConfig,
    ...userConfig,
    nested: {
      ...defaultConfig.nested,
      ...userConfig.nested
    }
  }
}
```

---

## 调试工具

### 1. 开发环境配置

```javascript
// vite.config.js
export default defineConfig({
  define: {
    __DEV__: process.env.NODE_ENV === 'development'
  },
  server: {
    hmr: {
      overlay: true
    }
  }
})
```

### 2. 性能分析工具

```javascript
// 使用 Chrome DevTools
// 1. 打开开发者工具
// 2. 切换到 Performance 标签
// 3. 点击录制按钮
// 4. 执行操作
// 5. 停止录制分析结果

// 使用内存分析
const heapdump = require('heapdump')
const analyzeMemory = () => {
  // 触发内存转储
  heapdump.writeSnapshot((err, filename) => {
    console.log('内存转储文件:', filename)
  })
}
```

### 3. 日志工具

```javascript
// 统一日志记录
const logger = {
  info: (message, ...args) => {
    if (process.env.NODE_ENV === 'development') {
      console.log(`[INFO] ${message}`, ...args)
    }
  },

  warn: (message, ...args) => {
    console.warn(`[WARN] ${message}`, ...args)
  },

  error: (message, ...args) => {
    console.error(`[ERROR] ${message}`, ...args)
    // 发送到错误跟踪系统
    sendErrorToService(new Error(message))
  }
}
```

### 4. 模块调试助手

```javascript
// debug-module.js
export const useModuleDebug = () => {
  const debugInfo = reactive({
    loadedModules: new Map(),
    loadingTime: {},
    errors: []
  })

  const startLoading = (moduleKey) => {
    debugInfo.loadingTime[moduleKey] = performance.now()
  }

  const endLoading = (moduleKey) => {
    const duration = performance.now() - debugInfo.loadingTime[moduleKey]
    console.log(`模块 ${moduleKey} 加载耗时: ${duration}ms`)
  }

  const logError = (error, moduleKey) => {
    debugInfo.errors.push({
      module: moduleKey,
      error: error.message,
      stack: error.stack,
      timestamp: new Date()
    })
  }

  return {
    debugInfo,
    startLoading,
    endLoading,
    logError
  }
}
```

---

## 常见错误代码

### 错误代码一览

| 错误代码 | 错误类型 | 描述 | 解决方案 |
|---------|---------|------|----------|
| MOD_001 | ModuleNotFound | 模块未找到 | 检查模块路径和注册 |
| MOD_002 | InvalidManifest | 无效的模块清单 | 验证 module.json 格式 |
| MOD_003 | DependencyError | 依赖错误 | 检查并安装依赖 |
| MOD_004 | ConfigError | 配置错误 | 验证配置格式 |
| MOD_005 | LoadingError | 加载错误 | 检查网络和权限 |
| MOD_006 | ActivationError | 激活错误 | 检查依赖和权限 |
| MOD_007 | ConflictError | 冲突错误 | 解决版本冲突 |
| MOD_008 | PermissionError | 权限错误 | 检查权限设置 |

### 错误处理示例

```javascript
// 全局错误处理
const handleError = (error) => {
  switch (error.code) {
    case 'MOD_001':
      console.error('模块未找到，请检查安装路径')
      break
    case 'MOD_003':
      console.error('依赖错误，请运行 npm install')
      break
    case 'MOD_005':
      console.error('加载错误，请检查网络连接')
      break
    default:
      console.error('未知错误:', error)
  }
}

// 使用示例
try {
  const module = await moduleManager.install('my-module')
} catch (error) {
  handleError(error)
}
```

---

## 联系支持

如果问题仍未解决，请：

1. 查看详细错误日志
2. 收集环境信息（Node.js 版本、操作系统等）
3. 准备最小复现代码
4. 提交 Issue 到 [GitHub 仓库](https://github.com/personweb/personweb)

**提交 Issue 时请包含：**
- 错误信息
- 复现步骤
- 期望行为
- 实际行为
- 环境信息

---

## 版本历史

### v1.0.0 (2024-03-13)
- 初始版本发布
- 基础故障排查指南
- 常见问题解决方案
- 调试工具介绍

---

希望本指南能帮助您快速解决模块系统使用中的问题。如有其他问题，请参考相关文档或联系技术支持。