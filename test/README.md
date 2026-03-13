# 模块单元测试

本项目为各个模块提供了完整的单元测试套件，使用 Vitest 作为测试框架。

## 测试文件结构

```
test/
├── setup.ts                           # 测试环境设置
├── modules/
│   ├── ai-assistant.test.ts           # AI Assistant 模块测试
│   ├── admin-panel.test.ts            # Admin Panel 模块测试
│   ├── 3d-display.test.ts            # 3D Display 模块测试
│   └── visitor-interaction.test.ts    # Visitor Interaction 模块测试
└── api/
    ├── modules.test.ts                # Modules API 测试
    ├── modules-crud.test.ts          # 模块 CRUD 操作测试
    └── dependency-graph.test.ts      # 依赖关系图功能测试
```

## 运行测试

### 安装依赖

```bash
npm install
```

### 运行所有测试

```bash
npm run test
```

### 运行测试（单次）

```bash
npm run test:run
```

### 生成测试覆盖率报告

```bash
npm run test:coverage
```

覆盖率报告将生成在 `coverage/` 目录中。

### 运行测试 UI

```bash
npm run test:ui
```

## 测试覆盖

### AI Assistant 模块 (`ai-assistant.test.ts`)

- ✅ AI 配置验证
- ✅ 消息管理
- ✅ 快捷操作
- ✅ 会话 ID 生成
- ✅ 对话导出
- ✅ 状态文本计算
- ✅ 历史记录管理

### Admin Panel 模块 (`admin-panel.test.ts`)

- ✅ 管理配置
- ✅ 菜单项管理
- ✅ 仪表盘组件
- ✅ 通知系统
- ✅ 工具函数（文件大小格式化、防抖、节流等）
- ✅ 权限系统

### 3D Display 模块 (`3d-display.test.ts`)

- ✅ 场景配置
- ✅ 场景对象管理
- ✅ 灯光配置
- ✅ 相机配置
- ✅ 场景统计
- ✅ 几何体创建
- ✅ 材质创建
- ✅ 场景配置导出

### Visitor Interaction 模块 (`visitor-interaction.test.ts`)

- ✅ 互动配置
- ✅ 访客等级系统
- ✅ 互动消息管理
- ✅ 消息队列管理
- ✅ 等级进度计算
- ✅ 快捷消息
- ✅ 颜色工具函数
- ✅ 访客统计
- ✅ 数据导出

### Modules API (`modules.test.ts`)

- ✅ 获取模块列表
- ✅ 获取模块详情
- ✅ 创建新模块
- ✅ 更新模块
- ✅ 删除模块
- ✅ 分页和过滤
- ✅ 关键字搜索

### 模块 CRUD 操作 (`modules-crud.test.ts`)

- ✅ 创建模块
- ✅ 读取模块
- ✅ 更新模块
- ✅ 删除模块
- ✅ 版本管理
- ✅ 验证规则

### 依赖关系图 (`dependency-graph.test.ts`)

- ✅ 构建依赖图
- ✅ 循环依赖检测
- ✅ 查找循环
- ✅ 可达性检查
- ✅ 依赖路径查找
- ✅ 拓扑排序
- ✅ 依赖验证

## 测试最佳实践

### 编写新测试

1. 在相应的目录下创建 `*.test.ts` 文件
2. 导入 `describe`, `it`, `expect`, `beforeEach` 等测试函数
3. 使用 `describe` 组织相关的测试用例
4. 使用 `it` 编写单个测试用例
5. 使用 `expect` 进行断言

### 示例

```typescript
import { describe, it, expect, beforeEach, vi } from 'vitest'

describe('MyModule', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should do something', () => {
    const result = myFunction()
    expect(result).toBe(expectedValue)
  })
})
```

### Mock 外部依赖

使用 `vi.mock` 来 mock 外部模块和服务：

```typescript
vi.mock('~/services/api', () => ({
  fetchData: vi.fn()
}))
```

## 持续集成

测试应该在以下情况下运行：

- 提交代码前
- Pull Request 创建时
- 合并到主分支时

确保所有测试通过后再提交代码。
