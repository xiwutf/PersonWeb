# 模块开发指南

## 概述

PersonWeb 模块系统是一个灵活可扩展的架构，允许开发者创建、管理和复用功能模块。本指南将详细介绍如何开发、测试和发布模块。

## 1. 模块系统架构

### 1.1 核心概念

- **模块（Module）**：独立的功能单元，包含配置、路由、组件等
- **模块定义（ModuleDefinition）**：模块的元数据和配置
- **模块清单（ModuleManifest）**：包含构建信息的发布格式
- **模块实例（ModuleInstance）**：已安装的模块实例及其状态

### 1.2 模块类型

| 类型 | 描述 | 示例 |
|------|------|------|
| Core Module | 核心系统模块 | 认证、权限管理 |
| Feature Module | 业务功能模块 | 博客、3D展示 |
| Plugin Module | 插件模块 | 第三方集成 |
| Theme Module | 主题模块 | UI主题定制 |

## 2. 模块结构

### 2.1 标准目录结构

```
module-name/
├── module.json          # 模块定义文件
├── index.ts            # 模块入口文件
├── components/         # 模块组件
│   ├── Component1.vue
│   └── Component2.vue
├── composables/        # 组合式函数
│   ├── useModule.ts
│   └── useModuleData.ts
├── types/              # 类型定义
│   └── module.ts
├── utils/              # 工具函数
│   └── helpers.ts
├── styles/             # 样式文件
│   └── module.css
├── locales/            # 国际化
│   ├── en.json
│   └── zh.json
├── tests/              # 测试文件
│   ├── component.test.ts
│   └── integration.test.ts
├── docs/               # 模块文档
│   └── README.md
└── public/             # 静态资源
    └── assets/
```

### 2.2 module.json 配置

```json
{
  "meta": {
    "key": "module-key",
    "name": "模块名称",
    "version": "1.0.0",
    "description": "模块描述",
    "author": "作者名称",
    "license": "MIT",
    "category": "ui",
    "tags": ["标签1", "标签2"],
    "icon": "🎨",
    "price": 0,
    "dependencies": ["module1", "module2"],
    "compatibility": {
      "nuxtVersion": "^3.0.0",
      "nodeVersion": "^16.0.0"
    }
  },
  "routes": [
    {
      "path": "/module-path",
      "name": "route-name",
      "component": "~/components/ModuleComponent.vue",
      "meta": {
        "title": "页面标题",
        "icon": "📝",
        "hidden": false,
        "order": 100
      }
    }
  ],
  "components": [
    {
      "name": "ModuleName",
      "path": "~/components/ModuleName.vue",
      "global": true
    }
  ],
  "configs": [
    {
      "configKey": "settingKey",
      "configValue": "defaultValue",
      "configType": "string",
      "description": "设置描述",
      "required": false,
      "defaultValue": "默认值",
      "validation": {
        "min": 1,
        "max": 100
      }
    }
  ],
  "permissions": [
    {
      "key": "module.view",
      "name": "查看模块",
      "description": "允许查看模块内容",
      "level": "basic"
    }
  ],
  "lifecycle": {
    "onInstall": "onInstallModule",
    "onUninstall": "onUninstallModule",
    "onActivate": "onActivateModule",
    "onDeactivate": "onDeactivateModule"
  }
}
```

## 3. 开发最佳实践

### 3.1 命名规范

- **模块名**：小写，使用连字符连接（如 `3d-display`）
- **组件名**：PascalCase（如 `Immersive3DScene`）
- **组合式函数**：以 `use` 开头（如 `useModuleData`）
- **文件名**：kebab-case（如 `module-utils.ts`）
- **路由名**：kebab-case（如 `/module-page`）

### 3.2 目录结构建议

1. **模块入口**：保持 `index.ts` 简洁，主要负责导出模块定义
2. **组件组织**：
   - 将相关组件放在同一目录
   - 使用子目录按功能分类
3. **类型定义**：单独的 `types/` 目录管理类型
4. **测试文件**：与对应文件同目录或放在 `tests/` 目录

### 3.3 错误处理

```typescript
// 在模块中优雅地处理错误
export async function fetchData() {
  try {
    const response = await $fetch('/api/module/data')
    return response.data
  } catch (error) {
    console.error('Module data fetch failed:', error)
    // 提供默认值或抛出用户友好的错误
    return getDefaultData()
  }
}
```

### 3.4 状态管理

```typescript
// 使用响应式状态管理
export const useModuleStore = defineStore('module', {
  state: () => ({
    data: null,
    loading: false,
    error: null
  }),

  getters: {
    isAuthenticated: (state) => !!state.user
  },

  actions: {
    async fetchData() {
      this.loading = true
      try {
        const response = await fetchModuleData()
        this.data = response
      } catch (error) {
        this.error = error.message
      } finally {
        this.loading = false
      }
    }
  }
})
```

## 4. API 规范

### 4.1 模块 API 接口

```typescript
// 模块导出的主要接口
interface ModuleAPI {
  // 初始化模块
  init(): Promise<void>
  // 销毁模块
  destroy(): Promise<void>
  // 获取模块状态
  getStatus(): ModuleStatus
  // 执行模块功能
  execute(action: string, params?: any): Promise<any>
}
```

### 4.2 事件系统

```typescript
// 定义模块事件
export const moduleEvents = {
  DATA_UPDATED: 'module:data-updated',
  CONFIG_CHANGED: 'module:config-changed',
  ERROR_OCCURRED: 'module:error-occurred'
}

// 发送事件
const emitter = defineEmits()
emitter(moduleEvents.DATA_UPDATED, { newData })

// 监听事件
onMounted(() => {
  emitter.on(moduleEvents.DATA_UPDATED, handleDataUpdate)
})
```

### 4.3 插件系统

```typescript
// 创建插件接口
export interface ModulePlugin {
  name: string
  version: string
  install(app: App): void
  uninstall(app: App): void
}

// 使用插件
export default defineNuxtPlugin((nuxtApp) => {
  const plugin = new MyModulePlugin()
  plugin.install(nuxtApp)
})
```

## 5. 测试规范

### 5.1 单元测试

```typescript
// components/Component.test.ts
import { mount } from '@vue/test-utils'
import Component from '../components/Component.vue'

describe('Component', () => {
  it('renders correctly', () => {
    const wrapper = mount(Component)
    expect(wrapper.text()).toContain('Expected text')
  })

  it('handles click events', async () => {
    const wrapper = mount(Component)
    await wrapper.trigger('click')
    expect(wrapper.emitted()).toHaveProperty('click')
  })
})
```

### 5.2 集成测试

```typescript
// tests/module.integration.test.ts
import { describe, it, expect } from 'vitest'
import { useModuleStore } from '~/stores/module'

describe('Module Integration', () => {
  it('initializes correctly', () => {
    const store = useModuleStore()
    expect(store.status).toBe('initialized')
  })

  it('loads data successfully', async () => {
    const store = useModuleStore()
    await store.loadData()
    expect(store.data).not.toBeNull()
  })
})
```

### 5.3 测试覆盖率要求

- 核心功能：> 90%
- 组件：> 80%
- 工具函数：> 90%
- 集成测试：> 70%

## 6. 国际化支持

### 6.1 基本配置

```json
// locales/zh.json
{
  "module": {
    "title": "模块标题",
    "description": "模块描述",
    "actions": {
      "save": "保存",
      "cancel": "取消"
    }
  }
}
```

### 6.2 使用方式

```typescript
const { t } = useI18n()

// 在组件中使用
<h1>{{ t('module.title') }}</h1>
<button>{{ t('module.actions.save') }}</button>
```

## 7. 性能优化

### 7.1 懒加载

```typescript
// 懒加载组件
const LazyComponent = defineAsyncComponent(() =>
  import('./components/LazyComponent.vue')
)

// 路由懒加载
export default defineNuxtRouteMiddleware((to) => {
  if (to.meta.lazy) {
    // 实现路由懒加载逻辑
  }
})
```

### 7.2 代码分割

```typescript
// 使用动态导入
const heavyFunction = () => import('./utils/heavyFunction')

// 条件导入
if (process.client) {
  import('./client-only')
}
```

### 7.3 优化策略

1. **首屏优化**：
   - 关键 CSS 内联
   - 核心组件预加载
   - 路由预加载

2. **运行时优化**：
   - 组件缓存
   - 事件防抖
   - 虚拟列表

## 8. 开发工具

### 8.1 模块脚手架

```bash
# 创建新模块
npx module-scaffold create my-module

# 快速启动开发服务器
npm run module:dev

# 构建模块
npm run module:build

# 发布模块
npm run module:publish
```

### 8.2 调试工具

```typescript
// 使用开发模式检查
if (process.dev) {
  console.log('Module debug info:', moduleInfo)
}

// 性能监控
import { usePerformance } from '@personweb/performance'
const perf = usePerformance()
perf.start('module-load')
// ... 加载模块
perf.end('module-load')
```

## 9. 发布流程

### 9.1 构建准备

```bash
# 安装依赖
npm install

# 运行测试
npm test

# 构建生产版本
npm run build

# 类型检查
npm run type-check
```

### 9.2 版本管理

遵循 [Semantic Versioning](https://semver.org/):

- `major.minor.patch`
- 示例：`1.0.0`

### 9.3 发布检查清单

- [ ] 测试通过（单元测试、集成测试）
- [ ] 类型检查通过
- [ ] 文档更新
- [ ] CHANGELOG.md 更新
- [ ] 版本号正确
- [ ] 许可证声明

## 10. 故障排查

### 10.1 常见问题

1. **模块无法加载**
   - 检查 module.json 格式
   - 验证依赖关系
   - 查看错误日志

2. **组件未注册**
   - 检查组件配置
   - 确认导入路径

3. **样式冲突**
   - 使用 scoped 样式
   - 检查 CSS 选择器优先级

### 10.2 调试技巧

```typescript
// 启用调试模式
export const debug = process.env.NODE_ENV === 'development'

if (debug) {
  console.debug('Debug info:', {...})
}
```

## 11. 社区贡献

### 11.1 代码规范

遵循 ESLint 和 Prettier 配置：

```json
{
  "extends": [
    "@personweb/module-standard"
  ]
}
```

### 11.2 提交规范

使用 [Conventional Commits](https://www.conventionalcommits.org/)：

```
feat: 新功能
fix: 修复bug
docs: 文档更新
style: 代码格式
refactor: 重构
test: 测试
chore: 构建工具
```

### 11.3 提交 PR

1. Fork 项目
2. 创建分支：`git checkout -b feature/module-name`
3. 提交更改
4. 创建 PR 并描述变更

## 12. 示例项目

参见 `examples/` 目录下的示例模块：
- `hello-world`: 简单的示例模块
- `ecommerce`: 电商模块示例
- `analytics`: 数据分析模块示例

## 13. API 参考

详见 `docs/api/` 目录下的 API 文档。

## 14. 更新日志

### 2024.03.13

- 初始版本发布
- 添加基础开发指南
- 示例模块

---

如有问题，请提交 Issue 或联系维护团队。