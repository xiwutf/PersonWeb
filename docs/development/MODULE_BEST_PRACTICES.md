# 模块开发最佳实践

## 目录

1. [架构设计原则](#架构设计原则)
2. [目录结构最佳实践](#目录结构最佳实践)
3. [命名规范](#命名规范)
4. [错误处理策略](#错误处理策略)
5. [性能优化技巧](#性能优化技巧)
6. [代码质量保证](#代码质量保证)
7. [测试策略](#测试策略)
8. [安全考虑](#安全考虑)
9. [维护性建议](#维护性建议)
10. [团队协作规范](#团队协作规范)

## 架构设计原则

### 1. 单一职责原则

每个模块应该专注于单一功能，避免功能耦合。

**反例：**
```typescript
// ❌ 一个模块同时处理用户管理和订单处理
export default defineModule({
  meta: {
    key: 'business-module',
    name: '业务模块',
    // ...
  },
  // 混合了多种职责
  services: {
    userService: { ... },
    orderService: { ... }
  }
})
```

**正例：**
```typescript
// ✅ 分离为用户模块和订单模块
// modules/user-module/index.ts
export default defineModule({
  meta: {
    key: 'user-module',
    name: '用户模块',
    // ...
  },
  services: {
    userService: { ... }
  }
})

// modules/order-module/index.ts
export default defineModule({
  meta: {
    key: 'order-module',
    name: '订单模块',
    // ...
  },
  services: {
    orderService: { ... }
  }
})
```

### 2. 依赖倒置原则

高层模块不应该依赖低层模块，两者都应该依赖抽象。

```typescript
// ✅ 定义接口
interface DataSource {
  fetchData(): Promise<Data>
}

// ✅ 实现具体类
class ApiDataSource implements DataSource {
  async fetchData() {
    return $fetch('/api/data')
  }
}

class LocalDataSource implements DataSource {
  async fetchData() {
    return localStorage.getItem('data')
  }
}

// ✅ 模块依赖接口，而不是具体实现
export default defineModule({
  dependencies: {
    dataSource: DataSource
  },
  inject: ['dataSource'],
  async init() {
    const data = await this.dataSource.fetchData()
    // 使用数据
  }
})
```

### 3. 开闭原则

模块应该对扩展开放，对修改关闭。

```typescript
// ✅ 使用策略模式扩展功能
interface Strategy {
  execute(): void
}

class StrategyA implements Strategy {
  execute() {
    console.log('Strategy A')
  }
}

class StrategyB implements Strategy {
  execute() {
    console.log('Strategy B')
  }
}

// 主模块不需要修改即可扩展新策略
export default defineModule({
  meta: {
    // ...
  },
  strategies: {
    a: StrategyA,
    b: StrategyB
  },
  useStrategy(strategy: string) {
    return this.strategies[strategy].execute()
  }
})
```

### 4. 迪米特法则

模块之间应该保持松耦合，只和直接的朋友通信。

```typescript
// ✅ 使用中介者模式
class ModuleMediator {
  private modules: Map<string, Module> = new Map()

  register(name: string, module: Module) {
    this.modules.set(name, module)
  }

  communicate(from: string, to: string, message: any) {
    const targetModule = this.modules.get(to)
    if (targetModule) {
      targetModule.receiveMessage(message)
    }
  }
}

// 模块之间不直接通信，通过中介者
export default defineModule({
  name: 'module-a',
  useMediator: true,
  sendMessage(to: string, message: any) {
    this.mediator.communicate(this.name, to, message)
  }
})
```

## 目录结构最佳实践

### 1. 标准结构

```
module-name/
├── module.json           # 模块配置
├── index.ts             # 模块入口
├── README.md            # 模块说明
├── src/
│   ├── index.ts         # 主要逻辑
│   ├── components/     # 组件目录
│   │   ├── Component.vue
│   │   └── components/
│   │       ├── SubComponent1.vue
│   │       └── SubComponent2.vue
│   ├── composables/    # 组合函数
│   │   ├── useModule.ts
│   │   ├── useData.ts
│   │   └── useApi.ts
│   ├── services/       # 服务层
│   │   ├── api.service.ts
│   │   ├── storage.service.ts
│   │   └── auth.service.ts
│   ├── types/          # 类型定义
│   │   ├── index.ts
│   │   └── types.d.ts
│   ├── utils/          # 工具函数
│   │   ├── helpers.ts
│   │   ├── formatters.ts
│   │   └── validators.ts
│   ├── constants/      # 常量
│   │   ├── index.ts
│   │   └── config.ts
│   ├── hooks/          # 自定义钩子
│   │   ├── useModuleHook.ts
│   │   └── useSubscription.ts
│   └── stores/         # 状态管理
│       ├── module.store.ts
│       └── data.store.ts
├── public/             # 静态资源
│   └── images/
│       └── logo.png
├── styles/             # 样式
│   ├── index.css
│   ├── components/
│   │   └── component.css
│   └── utils/
│       ├── mixins.css
│       └── variables.css
├── locales/            # 国际化
│   ├── en.json
│   ├── zh.json
│   └── fr.json
├── tests/              # 测试
│   ├── unit/
│   │   ├── components.test.ts
│   │   ├── services.test.ts
│   │   └── utils.test.ts
│   ├── integration/
│   │   ├── api.test.ts
│   │   └── workflow.test.ts
│   ├── e2e/
│   │   └── module.spec.ts
│   ├── mocks/
│   │   ├── api.ts
│   │   └── store.ts
│   └── utils/
│       └── helpers.ts
├── docs/               # 文档
│   ├── API.md
│   ├── README.md
│   └── CHANGELOG.md
├── .eslintrc.js        # ESLint 配置
├── .prettierrc.js      # Prettier 配置
├── jest.config.js      # Jest 配置
├── vitest.config.ts    # Vitest 配置
├── tsconfig.json       # TypeScript 配置
└── package.json       # 依赖配置
```

### 2. 目录组织原则

1. **按功能分层**：
   - `components/`：UI 组件
   - `services/`：业务逻辑
   - `utils/`：工具函数
   - `types/`：类型定义

2. **按业务分组**：
   ```typescript
   // ❌ 混合不同功能的组件
   src/components/
     ├── Button.tsx
     ├── UserCard.tsx
     ├── OrderList.tsx
     ├── LoginForm.tsx
     └── ProductCard.tsx

   // ✌ 按业务分组
   src/components/
     ├── ui/
     │   ├── Button.tsx
     │   └── Form.tsx
     ├── auth/
     │   └── LoginForm.tsx
     ├── user/
     │   ├── UserCard.tsx
     │   └── Profile.tsx
     └── shop/
         ├── OrderList.tsx
         └── ProductCard.tsx
   ```

3. **测试目录对齐**：
   - 测试文件与源文件结构保持一致
   - 使用 `tests/mocks/` 存放模拟数据

### 3. 文件命名约定

```typescript
// 组件：PascalCase
UserProfile.vue
OrderItem.vue

// 工具函数：camelCase
formatDate.ts
validateEmail.ts

// 服务类：PascalClass
UserService.ts
AuthService.ts

// 常量：UPPER_SNAKE_CASE
API_ENDPOINTS.ts
MAX_RETRIES.ts

// 测试文件：*.test.ts 或 *.spec.ts
user.service.test.ts
user.service.spec.ts
```

## 命名规范

### 1. 变量命名

```typescript
// ✅ 清晰描述用途
const userName = 'John'
const maxRetryCount = 3
const isLoading = ref(false)

// ❌ 不清晰或缩写过度
const n = 'John'  // n 代表什么？
const mrc = 3     // mrc 代表什么？
const load = ref(false)  // load 什么？

// ✅ 布尔值使用 is/has/can 前缀
const isActive = true
const hasPermission = false
canEdit = true

// ✅ 复合词使用 camelCase
const firstName = 'John'
const userId = '123'
```

### 2. 函数命名

```typescript
// ✅ 动词或动词短语
function getUser() { ... }
function setUserName() { ... }
function calculateTotal() { ... }
function validateInput() { ... }

// ✅ 事件处理函数
function handleLogin() { ... }
function handleSubmit() { ... }
function handleDelete() { ... }

// ❌ 不明确的命名
function getData() { ... }  // 什么数据？
function run() { ... }      // 运行什么？

// ✅ 异步函数使用 async 前缀或 Promise 相关命名
async function fetchUser() { ... }
function validateInputAsync() { ... }
```

### 3. 组件命名

```typescript
// ✅ 使用前缀（可选）
BaseButton.vue
PrimaryButton.vue

// ✅ 使用描述性名称
UserProfile.vue
OrderHistory.vue
ShoppingCart.vue

// ❌ 缩写或过于通用
User.vue    // 什么类型的用户？
Comp.vue   // 什么组件？
```

### 4. 模块命名

```typescript
// ✅ 使用清晰的功能描述
user-management-module
payment-gateway-module
content-management-system

// ✅ 使用连字符分隔
blog-editor-module
image-uploader-module

// ❌ 模糊或不明确
utils-module          // 什么工具？
shared-module         // 共享什么？
```

### 5. API 命名

```typescript
// ✅ RESTful 风格
GET /api/users
POST /api/users
PUT /api/users/:id
DELETE /api/users/:id

// ✅ 功能描述
GET /api/modules/search
POST /api/modules/install
PUT /api/modules/config/:id

// ❌ 不一致或模糊的命名
GET /get/user
POST /save/module
PUT /update/data
```

## 错误处理策略

### 1. 错误分类

```typescript
// 定义错误类型
export class ModuleError extends Error {
  constructor(
    message: string,
    public code: string,
    public statusCode?: number
  ) {
    super(message)
    this.name = 'ModuleError'
  }
}

export class ValidationError extends ModuleError {
  constructor(message: string, field: string) {
    super(message, 'VALIDATION_ERROR', 400)
    this.field = field
  }
}

export class NetworkError extends ModuleError {
  constructor(message: string) {
    super(message, 'NETWORK_ERROR', 0)
  }
}
```

### 2. 统一错误处理

```typescript
// 创建错误处理器
export class ErrorHandler {
  static handle(error: Error): void {
    console.error('Error occurred:', error)

    // 根据错误类型处理
    if (error instanceof ValidationError) {
      showToast(error.message)
    } else if (error instanceof NetworkError) {
      showToast('网络连接失败，请重试')
    } else {
      showToast('发生未知错误')
    }
  }
}

// 在组件中使用
async function fetchData() {
  try {
    const data = await api.getData()
    return data
  } catch (error) {
    ErrorHandler.handle(error)
    throw error
  }
}
```

### 3. 错误边界

```typescript
// 组件级错误边界
export default defineComponent({
  errorCaptured(err, instance, info) {
    console.error('Component error:', err)
    return false // 阻止错误继续向上传播
  }
})

// 应用级错误处理
app.config.errorHandler = (err, instance, info) => {
  console.error('Global error:', err)
  // 显示错误提示
  showErrorToast(err.message)
}
```

### 4. 重试机制

```typescript
// 指数退避重试
async function fetchWithRetry(
  url: string,
  retries = 3,
  delay = 1000
): Promise<any> {
  try {
    const response = await $fetch(url)
    return response
  } catch (error) {
    if (retries <= 0) throw error

    // 指数退避
    const waitTime = delay * 2
    await new Promise(resolve => setTimeout(resolve, waitTime))

    return fetchWithRetry(url, retries - 1, waitTime)
  }
}
```

### 5. 错误日志记录

```typescript
// 错误日志服务
class ErrorLogger {
  private static instance: ErrorLogger

  static getInstance(): ErrorLogger {
    if (!ErrorLogger.instance) {
      ErrorLogger.instance = new ErrorLogger()
    }
    return ErrorLogger.instance
  }

  async log(error: Error, context?: any): Promise<void> {
    const logEntry = {
      timestamp: new Date().toISOString(),
      error: {
        message: error.message,
        stack: error.stack,
        code: (error as any).code
      },
      context,
      userAgent: navigator.userAgent,
      url: window.location.href
    }

    // 发送到错误收集服务
    await $fetch('/api/errors', {
      method: 'POST',
      body: logEntry
    })
  }
}

// 使用示例
try {
  // 业务代码
} catch (error) {
  await ErrorLogger.getInstance().log(error, {
    action: 'user_login',
    userId: '123'
  })
}
```

## 性能优化技巧

### 1. 懒加载

```typescript
// 组件懒加载
const LazyComponent = defineAsyncComponent(() =>
  import('./components/LazyComponent.vue')
)

// 路由懒加载
const routes = [
  {
    path: '/lazy',
    component: () => import('./views/LazyView.vue')
  }
]

// 模块懒加载
export const useLazyModule = (moduleKey: string) => {
  return computed(() => {
    if (!isModuleLoaded.value) {
      loadModule(moduleKey)
    }
    return loadedModules[moduleKey]
  })
}
```

### 2. 缓存策略

```typescript
// 使用内存缓存
const cache = new Map()

async function getCachedData(key: string, fetcher: () => Promise<any>) {
  if (cache.has(key)) {
    return cache.get(key)
  }

  const data = await fetcher()
  cache.set(key, data)
  return data
}

// 使用 localStorage 缓存
class CacheStorage {
  set(key: string, value: any, ttl = 3600) {
    const data = {
      value,
      expires: Date.now() + ttl * 1000
    }
    localStorage.setItem(key, JSON.stringify(data))
  }

  get(key: string) {
    const data = localStorage.getItem(key)
    if (!data) return null

    const parsed = JSON.parse(data)
    if (parsed.expires < Date.now()) {
      localStorage.removeItem(key)
      return null
    }

    return parsed.value
  }
}
```

### 3. 防抖和节流

```typescript
// 防抖
export function useDebounce(fn: Function, delay: number) {
  let timeoutId: NodeJS.Timeout | null = null

  return function(...args: any[]) {
    if (timeoutId) {
      clearTimeout(timeoutId)
    }

    timeoutId = setTimeout(() => {
      fn.apply(this, args)
    }, delay)
  }
}

// 节流
export function useThrottle(fn: Function, limit: number) {
  let inThrottle = false

  return function(...args: any[]) {
    if (!inThrottle) {
      fn.apply(this, args)
      inThrottle = true
      setTimeout(() => {
        inThrottle = false
      }, limit)
    }
  }
}

// 使用示例
const search = useDebounce((query: string) => {
  // 搜索逻辑
}, 300)

window.addEventListener('resize', useThrottle(() => {
  // 窗口大小改变处理
}, 100))
```

### 4. 虚拟列表

```vue
<template>
  <VirtualList
    :items="items"
    :item-height="50"
    :container-height="500"
  >
    <template #default="{ item, index }">
      <div class="item" :style="{ height: '50px' }">
        {{ item.name }}
      </div>
    </template>
  </VirtualList>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import VirtualList from './components/VirtualList.vue'

const items = ref([])

onMounted(async () => {
  // 加载大量数据
  const response = await fetch('/api/items?limit=10000')
  items.value = await response.json()
})
</script>
```

### 5. 代码分割

```typescript
// 动态导入
const HeavyComponent = () => import('./components/HeavyComponent.vue')

// 条件导入
if (process.client) {
  import('./client-only-module')
}

// Webpack 魔法注释
const Module = () => import(/* webpackChunkName: "module" */ './module')
```

## 代码质量保证

### 1. TypeScript 最佳实践

```typescript
// ✅ 使用接口定义类型
interface User {
  id: string
  name: string
  email: string
}

// ✅ 使用泛型
function createArray<T>(length: number, fill: T): T[] {
  return Array(length).fill(fill)
}

// ✅ 使用类型别名
type ApiResponse<T> = {
  data: T
  message: string
  success: boolean
}

// ✅ 使用枚举
enum Status {
  Active = 'active',
  Inactive = 'inactive',
  Pending = 'pending'
}

// ✅ 使用类型守卫
function isUser(obj: any): obj is User {
  return obj && typeof obj.id === 'string'
}
```

### 2. ESLint 配置

```javascript
// .eslintrc.js
module.exports = {
  extends: [
    '@nuxtjs/eslint-config-typescript',
    '@nuxtjs/eslint-config-prettier'
  ],
  rules: {
    // 自定义规则
    'vue/multi-word-component-names': 'off',
    '@typescript-eslint/no-unused-vars': 'error',
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off'
  }
}
```

### 3. Prettier 配置

```json
{
  "semi": false,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 100,
  "arrowParens": "avoid"
}
```

### 4. 代码审查清单

```markdown
## 代码审查清单

### 代码质量
- [ ] 代码符合项目规范
- [ ] 函数职责单一
- [ ] 变量命名清晰
- [ ] 代码注释充分

### 性能考虑
- [ ] 避免不必要的渲染
- [ ] 使用适当的缓存
- [ ] 资源及时释放

### 安全性
- [ ] 输入验证充分
- [ ] XSS 防护
- [ ] 敏感数据保护

### 可测试性
- [ ] 函数可测试
- [ ] 依赖注入
- [ ] 模拟外部依赖
```

## 测试策略

### 1. 单元测试

```typescript
// components/UserCard.test.ts
import { render, screen } from '@testing-library/vue'
import UserCard from '~/components/UserCard.vue'

describe('UserCard', () => {
  it('renders user name', () => {
    const user = { name: 'John Doe', email: 'john@example.com' }

    render(UserCard, {
      props: { user }
    })

    expect(screen.getByText('John Doe')).toBeInTheDocument()
    expect(screen.getByText('john@example.com')).toBeInTheDocument()
  })

  it('emits click event when clicked', async () => {
    const user = { name: 'John', email: 'john@example.com' }
    const { emitted } = render(UserCard, { props: { user } })

    await screen.getByRole('button').click()

    expect(emitted()).toHaveProperty('click')
  })
})
```

### 2. 集成测试

```typescript
// tests/integration/user-flow.test.ts
import { createPinia, setActivePinia } from 'pinia'
import { mount } from '@vue/test-utils'
import { describe, it, expect, beforeEach } from 'vitest'
import UserProfile from '~/components/UserProfile.vue'
import UserStore from '~/stores/user'

describe('User Profile Integration', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('displays user data from store', async () => {
    const store = useUserStore()
    store.user = { id: '1', name: 'John', email: 'john@example.com' }

    const wrapper = mount(UserProfile)

    expect(wrapper.text()).toContain('John')
    expect(wrapper.text()).toContain('john@example.com')
  })
})
```

### 3. E2E 测试

```typescript
// tests/e2e/user-login.spec.ts
import { test, expect } from '@playwright/test'

test('user can login successfully', async ({ page }) => {
  // Navigate to login page
  await page.goto('/login')

  // Fill in login form
  await page.fill('#email', 'test@example.com')
  await page.fill('#password', 'password123')
  await page.click('button[type="submit"]')

  // Should redirect to dashboard
  await expect(page).toHaveURL('/dashboard')

  // Should show welcome message
  await expect(page.locator('h1')).toContainText('Welcome back!')
})
```

### 4. Mock 服务

```typescript
// tests/mocks/api.ts
export const mockApi = {
  async getUser(id: string) {
    return {
      id,
      name: 'Mock User',
      email: 'mock@example.com'
    }
  },

  async login(email: string, password: string) {
    if (email === 'valid@example.com' && password === 'valid') {
      return { success: true, token: 'mock-token' }
    }
    return { success: false, error: 'Invalid credentials' }
  }
}

// 在测试中使用
vitest.mock('~/services/api', () => ({
  useApi: () => mockApi
}))
```

## 安全考虑

### 1. XSS 防护

```typescript
// 自定义 v-html 指令，带 XSS 过滤
const safeHTML = {
  mounted(el: HTMLElement, binding: any) {
    if (binding.value) {
      // 使用 DOMPurify 等库进行清理
      const clean = DOMPurify.sanitize(binding.value)
      el.innerHTML = clean
    }
  }
}

// 在模板中使用
<div v-safe-html="userContent"></div>
```

### 2. CSRF 防护

```typescript
// 添加 CSRF token
export const useCsrf = () => {
  const token = ref('')

  const loadToken = async () => {
    const response = await $fetch('/api/csrf-token')
    token.value = response.token
  }

  const addCsrfHeader = (config: any) => {
    if (token.value) {
      config.headers = {
        ...config.headers,
        'X-CSRF-Token': token.value
      }
    }
    return config
  }

  return {
    token,
    loadToken,
    addCsrfHeader
  }
}
```

### 3. 输入验证

```typescript
// 使用 yup 进行验证
import * as yup from 'yup'

const userSchema = yup.object().shape({
  name: yup.string()
    .required('姓名不能为空')
    .min(2, '姓名至少2个字符')
    .max(50, '姓名最多50个字符'),
  email: yup.string()
    .required('邮箱不能为空')
    .email('邮箱格式不正确'),
  password: yup.string()
    .required('密码不能为空')
    .min(8, '密码至少8个字符')
})

// 在组件中使用
async function submitForm() {
  try {
    const validatedData = await userSchema.validate(form.value)
    // 提交数据
  } catch (error) {
    if (error instanceof yup.ValidationError) {
      errors.value = error.inner.reduce((acc, err) => {
        acc[err.path!] = err.message
        return acc
      }, {} as Record<string, string>)
    }
  }
}
```

## 维护性建议

### 1. 文档更新

```markdown
# README.md 模板

## 模块名称
[模块描述]

## 功能特性
- [ ] 功能1
- [ ] 功能2

## 安装依赖
```bash
npm install
```

## 开发
```bash
npm run dev
```

## 测试
```bash
npm test
```

## API 参考
详细 API 文档请查看 [API.md](./docs/API.md)

## 贡献指南
1. Fork 项目
2. 创建分支
3. 提交更改
4. 发起 PR
```

### 2. 变更日志

```markdown
## [1.0.0] - 2024-03-13

### 新增
- 初始版本发布
- 基础功能实现
- 用户认证功能

### 修复
- 修复了登录页面的响应式问题
- 修复了 API 请求超时错误

### 变更
- 更新了 UI 组件库版本

## [0.9.0] - 2024-03-10

### 新增
- 开发环境配置
- 基础组件结构
```

### 3. 代码注释规范

```typescript
/**
 * 用户服务类
 *
 * 提供用户相关的业务逻辑，包括：
 * - 用户注册
 * - 用户登录
 * - 用户信息更新
 */
export class UserService {
  /**
   * 用户注册
   * @param email 用户邮箱
   * @param password 用户密码
   * @returns 注册成功返回用户信息，失败抛出错误
   * @throws {ValidationError} 参数验证失败
   * @throws {NetworkError} 网络请求失败
   */
  async register(email: string, password: string): Promise<User> {
    // 实现代码
  }
}
```

## 团队协作规范

### 1. Git 工作流

```bash
# 功能分支命名规范
feature/user-management
feature/add-payment-gateway
fix/login-validation-error

# 发布分支
release/v1.0.0
hotfix/security-patch

# 提交信息规范
# 格式：<类型>(<范围>): <描述>
#
# 类型：
# feat: 新功能
# fix: 修复bug
# docs: 文档更新
# style: 代码格式
# refactor: 重构
# test: 测试
# chore: 构建工具或依赖管理
```

### 2. 代码审查标准

```markdown
## 代码审查清单

### 代码质量
- [ ] 代码符合项目规范
- [ ] 函数职责单一，不超过20行
- [ ] 变量命名清晰，使用有意义的名称
- [ ] 必要的代码注释

### 性能优化
- [ ] 避免不必要的重新渲染
- [ ] 使用适当的缓存策略
- [ ] 大列表使用虚拟滚动

### 安全性
- [ ] 输入验证充分
- [ ] 敏感数据加密
- [ ] 正确的错误处理

### 可维护性
- [ ] 良好的错误处理
- [ ] 合理的代码组织
- [ ] 清晰的文档说明
```

### 3. 持续集成

```yaml
# .github/workflows/test.yml
name: Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest

    strategy:
      matrix:
        node-version: [16.x, 18.x, 20.x]

    steps:
    - uses: actions/checkout@v3

    - name: Setup Node.js ${{ matrix.node-version }}
      uses: actions/setup-node@v3
      with:
        node-version: ${{ matrix.node-version }}
        cache: 'npm'

    - name: Install dependencies
      run: npm ci

    - name: Run unit tests
      run: npm run test:unit

    - name: Run integration tests
      run: npm run test:integration

    - name: Build
      run: npm run build
```

### 4. 部署规范

```yaml
# .github/workflows/deploy.yml
name: Deploy

on:
  release:
    types: [published]

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Build
      run: |
        npm run build
        npm run build:module

    - name: Deploy
      run: |
        # 部署到生产环境
        rsync -av dist/ production-server:/var/www/personweb/
```

---

遵循这些最佳实践可以帮助你开发高质量、可维护、高性能的模块。记住，最佳实践是指导原则，根据具体项目需求可能需要进行调整。