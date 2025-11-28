# 代码规范改进计划

## 🎯 目标

将代码质量从 7/10 提升到 9/10，符合主流最佳实践。

## 📅 实施计划

### 第一阶段：基础改进（1-2天）

#### 1. 安装 Toast 通知组件
```bash
npm install vue-toastification
```

#### 2. 创建类型定义文件
创建 `types/` 目录，定义所有 API 接口类型。

#### 3. 清理调试代码
移除或条件化所有 `console.log`。

### 第二阶段：类型安全（2-3天）

#### 4. 替换所有 `any` 类型
- 为所有 API 响应定义接口
- 为所有组件 props 定义类型
- 为所有 ref 定义类型

#### 5. 创建常量文件
集中管理所有魔法数字和字符串。

### 第三阶段：用户体验（1-2天）

#### 6. 统一错误处理
创建 `useErrorHandler` composable。

#### 7. 表单验证
引入 VeeValidate 进行统一验证。

## 🔧 具体实施步骤

### 步骤 1：安装依赖

```bash
# Toast 通知
npm install vue-toastification

# 表单验证（可选）
npm install vee-validate yup @vee-validate/yup
```

### 步骤 2：创建类型定义

创建 `types/api.ts`：
```typescript
export interface Article {
  id: number
  title: string
  slug?: string
  summary?: string
  contentMd?: string
  contentHtml?: string
  coverUrl?: string
  categoryId?: number
  status: number
  tags?: string[]
  createdAt: string
  updatedAt: string
  publishTime?: string
  viewCount: number
  categoryName?: string
}

export interface ArticleRequest {
  id?: number
  title: string
  slug?: string
  summary?: string
  contentMd?: string
  contentHtml?: string
  coverUrl?: string
  categoryId?: number
  status: number
  tags?: string[]
}

export interface Category {
  id: number
  name: string
  slug?: string
  description?: string
}

// ... 更多类型定义
```

### 步骤 3：创建 Toast 通知

创建 `composables/useToast.ts`：
```typescript
import { useToast } from 'vue-toastification'

export const useNotification = () => {
  const toast = useToast()
  
  return {
    success: (message: string) => toast.success(message),
    error: (message: string) => toast.error(message),
    warning: (message: string) => toast.warning(message),
    info: (message: string) => toast.info(message)
  }
}
```

### 步骤 4：创建常量文件

创建 `constants/index.ts`：
```typescript
export const ARTICLE_STATUS = {
  DRAFT: 0,
  PUBLISHED: 1,
  ARCHIVED: 2
} as const

export const TIME_CAPSULE_STATUS = {
  PENDING: 0,
  APPROVED: 1,
  REJECTED: 2
} as const

export const PROJECT_STATUS = {
  ACTIVE: 'Active',
  COMPLETED: 'Completed',
  ARCHIVED: 'Archived'
} as const
```

### 步骤 5：统一错误处理

创建 `composables/useErrorHandler.ts`：
```typescript
import { useNotification } from './useToast'

export const useErrorHandler = () => {
  const { error } = useNotification()
  
  const handleError = (err: unknown) => {
    if (err instanceof Error) {
      error(err.message)
    } else if (typeof err === 'object' && err !== null && 'message' in err) {
      error(String(err.message))
    } else {
      error('操作失败，请稍后重试')
    }
  }
  
  return { handleError }
}
```

## 📊 改进效果预期

### 改进前
- TypeScript 类型覆盖率：~30%
- 用户体验：6/10
- 代码可维护性：7/10

### 改进后
- TypeScript 类型覆盖率：~90%
- 用户体验：9/10
- 代码可维护性：9/10

## ✅ 检查清单

- [ ] 安装 Toast 通知组件
- [ ] 创建类型定义文件
- [ ] 替换所有 `alert()` 为 Toast
- [ ] 清理所有调试代码
- [ ] 替换所有 `any` 类型
- [ ] 创建常量文件
- [ ] 统一错误处理
- [ ] 添加表单验证（可选）

