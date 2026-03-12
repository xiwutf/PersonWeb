# 模块系统 API 文档

## 概述

模块系统 API 提供了完整的模块生命周期管理功能，包括模块的创建、读取、更新、删除，以及配置管理、依赖检查等高级功能。

## API 端点

### 1. 模块列表

**GET** `/api/modules`

获取所有模块的列表，支持筛选、搜索和分页。

#### 参数

| 参数 | 类型 | 必需 | 说明 |
|------|------|------|------|
| `category` | string | 否 | 按分类筛选 |
| `enabled` | boolean | 否 | 按启用状态筛选 |
| `search` | string | 否 | 搜索关键词 |
| `page` | number | 否 | 页码，默认 1 |
| `pageSize` | number | 否 | 每页大小，默认 10 |

#### 响应

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "moduleKey": "ai-assistant",
      "moduleName": "AI Assistant",
      "moduleVersion": "1.0.0",
      "description": "智能AI助手系统",
      "author": "PersonWeb Team",
      "icon": "🤖",
      "category": "ai",
      "dependencies": [],
      "routes": [
        {
          "path": "/ai",
          "name": "ai-assistant",
          "meta": {
            "title": "AI 助手",
            "icon": "🤖",
            "order": 100
          }
        }
      ],
      "components": [...],
      "permissions": [...],
      "configSchema": {...},
      "isEnabled": true,
      "isCore": false,
      "sort": 100,
      "createdAt": "2024-01-01T00:00:00.000Z",
      "updatedAt": "2024-01-01T00:00:00.000Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "total": 15,
    "totalPages": 2
  }
}
```

### 2. 获取模块详情

**GET** `/api/modules/{moduleKey}`

获取特定模块的详细信息。

#### 路径参数

| 参数 | 类型 | 必需 | 说明 |
|------|------|------|------|
| `moduleKey` | string | 是 | 模块唯一标识 |

#### 响应

与模块列表中的单个模块结构相同。

### 3. 创建模块

**POST** `/api/modules`

创建新的模块。

#### 请求体

```json
{
  "moduleKey": "new-module",
  "moduleName": "新模块",
  "description": "模块描述",
  "author": "作者名",
  "icon": "📦",
  "category": "content",
  "dependencies": [],
  "routes": [
    {
      "path": "/new-module",
      "name": "new-module",
      "meta": {
        "title": "新模块"
      }
    }
  ],
  "components": [
    {
      "name": "NewComponent",
      "path": "~/components/NewComponent.vue",
      "global": false
    }
  ],
  "permissions": [
    {
      "key": "new-module.read",
      "name": "读取新模块",
      "description": "允许读取新模块",
      "level": "basic"
    }
  ],
  "configSchema": {
    "apiKey": {
      "type": "string",
      "required": true,
      "description": "API密钥"
    }
  },
  "sort": 10
}
```

### 4. 更新模块

**PUT** `/api/modules/{moduleKey}`

更新现有模块的信息。

#### 路径参数

| 参数 | 类型 | 必需 | 说明 |
|------|------|------|------|
| `moduleKey` | string | 是 | 模块唯一标识 |

#### 请求体

与创建模块类似，但所有字段都是可选的。

### 5. 启用/禁用模块

**POST** `/api/modules/{moduleKey}/enable`

启用或禁用模块。

#### 请求体

```json
{
  "enabled": true
}
```

### 6. 获取模块配置

**GET** `/api/modules/{moduleKey}/config`

获取模块的配置项列表。

#### 响应

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "moduleKey": "ai-assistant",
      "configKey": "apiKey",
      "configValue": "sk-xxx",
      "description": "AI服务API密钥",
      "createdAt": "2024-01-01T00:00:00.000Z",
      "updatedAt": "2024-01-01T00:00:00.000Z"
    }
  ]
}
```

### 7. 依赖关系图

**GET** `/api/modules/dependency-graph`

获取所有模块的依赖关系图，包括循环依赖检测。

#### 响应

```json
{
  "success": true,
  "data": {
    "graph": {
      "ai-assistant": {
        "dependsOn": [],
        "dependents": ["analytics"]
      },
      "blog": {
        "dependsOn": [],
        "dependents": ["comments", "analytics"]
      }
    },
    "hasCycles": false,
    "cycles": [],
    "allPaths": {
      "analytics": [
        ["ai-assistant", "analytics"],
        ["blog", "analytics"]
      ]
    }
  }
}
```

### 8. 模块统计

**GET** `/api/modules/stats`

获取模块系统的统计数据。

#### 响应

```json
{
  "success": true,
  "data": {
    "totalModules": 15,
    "enabledModules": 12,
    "disabledModules": 3,
    "coreModules": 2,
    "categoryDistribution": {
      "content": 5,
      "tool": 4,
      "ai": 3,
      "experiment": 2,
      "interaction": 1
    },
    "recentDownloads": 245,
    "avgRating": 4.2,
    "downloadsByDay": [...],
    "topModules": [...],
    "recentActivity": [...]
  }
}
```

## 错误响应

所有 API 在发生错误时都会返回统一的格式：

```json
{
  "statusCode": 400,
  "statusMessage": "错误描述",
  "data": {
    "error": "详细错误信息"
  }
}
```

## 状态码说明

| 状态码 | 说明 |
|--------|------|
| 200 | 请求成功 |
| 400 | 请求参数错误 |
| 401 | 未认证 |
| 403 | 无权限 |
| 404 | 资源不存在 |
| 409 | 资源冲突（如模块已存在） |
| 500 | 服务器内部错误 |

## 数据库表结构

### module 表
- `id`: 主键
- `module_key`: 模块唯一标识
- `module_name`: 模块名称
- `module_version`: 模块版本
- `description`: 模块描述
- `author`: 作者
- `icon`: 图标
- `category`: 分类
- `dependencies`: 依赖（JSON）
- `routes`: 路由配置（JSON）
- `components`: 组件列表（JSON）
- `permissions`: 权限配置（JSON）
- `config_schema`: 配置Schema（JSON）
- `is_enabled`: 是否启用
- `is_core`: 是否核心模块
- `sort`: 排序
- `created_at`: 创建时间
- `updated_at`: 更新时间

### module_config 表
- `id`: 主键
- `module_key`: 模块标识
- `config_key`: 配置键
- `config_value`: 配置值
- `description`: 配置描述
- `created_at`: 创建时间
- `updated_at`: 更新时间

## 使用示例

### 获取所有AI模块
```javascript
const response = await fetch('/api/modules?category=ai&enabled=true')
const data = await response.json()
```

### 创建新模块
```javascript
const response = await fetch('/api/modules', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    moduleKey: 'weather-widget',
    moduleName: '天气组件',
    description: '显示天气预报的组件',
    category: 'tool',
    dependencies: [],
    routes: [{
      path: '/weather',
      name: 'weather-widget',
      meta: { title: '天气' }
    }],
    components: [
      {
        name: 'WeatherWidget',
        path: '~/components/weather/Widget.vue',
        global: false
      }
    ],
    permissions: [
      {
        key: 'weather.read',
        name: '查看天气',
        description: '允许查看天气信息',
        level: 'basic'
      }
    ],
    configSchema: {
      apiKey: {
        type: 'string',
        required: true,
        description: '天气API密钥'
      }
    },
    sort: 20
  })
})
```

### 更新模块状态
```javascript
const response = await fetch('/api/modules/ai-assistant/enable', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    enabled: false
  })
})
```

---

*本文档持续更新中，如有疑问请联系开发团队。*