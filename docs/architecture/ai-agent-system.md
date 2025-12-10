# AI 智能体系统架构说明

## 概述

本系统实现了一套统一的 AI 智能体框架，支持多种业务场景的 AI 能力调用。系统采用分层架构，便于扩展和维护。

## 技术栈

- **前端**：Nuxt3 + Vue3 + Naive UI
- **后端**：.NET 8 WebAPI + EF Core + MySQL
- **AI 服务**：Python AI Service (FastAPI) - 运行在 `http://localhost:8001/api/ai`

## 架构分层

### 1. 数据访问层（Repository）

- **位置**：`backend/PersonalSite.Api/Data/AppDbContext.cs`
- **职责**：使用 EF Core 管理数据库实体和关系
- **关键表**：
  - `AiAgentLog`：记录所有 AI 调用日志
  - `Article`：文章表（内容生成智能体使用）
  - `Project`：项目表（Demo 上架智能体使用）
  - `PreSaleConsultation`：售前咨询表（表单线索处理智能体使用）

### 2. 服务层（Service）

- **位置**：`backend/PersonalSite.Api/Services/`
- **核心服务**：
  - `AiServiceClient`：统一的 AI 服务 HTTP 客户端，封装与 Python AI Service 的通信
  - `AiAgentService`：智能体服务基类，提供统一的调用接口和日志记录
  - `ContentAgentService`：内容生成智能体服务
  - `DemoAgentService`：Demo 上架智能体服务
  - `LeadAgentService`：表单线索处理智能体服务

### 3. 控制器层（Controller）

- **位置**：`backend/PersonalSite.Api/Controllers/`
- **路由前缀**：`/api/ai/`
- **关键控制器**：
  - `AiAgentController`：统一管理所有智能体的 API 端点
    - `POST /api/ai/content/generate`：内容生成
    - `POST /api/ai/demo/describe`：Demo 描述生成
    - `POST /api/ai/leads/analyze`：线索分析

### 4. 前端页面层（Pages）

- **位置**：`pages/admin/ai/`
- **关键页面**：
  - `pages/admin/ai/index.vue`：AI 智能体中心（入口页面）
  - `pages/admin/ai/content.vue`：内容生成智能体页面
  - `pages/admin/projects/index.vue`：项目列表（集成 Demo 智能体）
  - `pages/admin/consultations.vue`：咨询列表（集成 Lead 智能体）

## 数据流

```
前端页面
  ↓ (HTTP Request)
Controller (接收请求，参数验证)
  ↓ (调用 Service)
Service (业务逻辑，组装 Prompt)
  ↓ (调用 AiServiceClient)
AiServiceClient (HTTP 请求到 Python AI Service)
  ↓ (返回结果)
Service (处理响应，写入数据库)
  ↓ (记录日志到 AiAgentLog)
Controller (返回响应给前端)
  ↓ (HTTP Response)
前端页面 (展示结果)
```

## 扩展新智能体

### 步骤 1：创建 Service

在 `backend/PersonalSite.Api/Services/` 下创建新的 Service 类，例如：

```csharp
public class NewAgentService : AiAgentService
{
    public NewAgentService(
        AiServiceClient aiClient,
        AppDbContext dbContext,
        ILogger<NewAgentService> logger)
        : base(aiClient, dbContext, logger)
    {
    }

    public async Task<NewAgentResponse> ProcessAsync(NewAgentRequest request)
    {
        // 1. 组装 Prompt
        var prompt = BuildPrompt(request);
        
        // 2. 调用 AI
        var response = await CallAiAsync("NewAgent", prompt, request);
        
        // 3. 处理响应
        var result = ParseResponse(response);
        
        // 4. 写入数据库（如果需要）
        // ...
        
        return result;
    }
}
```

### 步骤 2：创建 Controller 端点

在 `AiAgentController` 中添加新的端点：

```csharp
[HttpPost("new-agent/process")]
public async Task<IActionResult> ProcessNewAgent([FromBody] NewAgentRequest request)
{
    // 参数验证
    // 调用 Service
    // 返回结果
}
```

### 步骤 3：创建前端页面

在 `pages/admin/ai/` 下创建新的 Vue 页面，使用 Naive UI 组件。

### 步骤 4：注册服务

在 `Program.cs` 中注册新的 Service：

```csharp
builder.Services.AddScoped<NewAgentService>();
```

## 数据库表结构

### AiAgentLog 表

记录所有 AI 调用的日志，用于监控和调试。

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | long | 主键 |
| AgentType | string | 智能体类型（Content/Demo/Lead） |
| RequestPayload | text | 请求内容（JSON） |
| ResponsePayload | text | 响应内容（JSON） |
| Success | bool | 是否成功 |
| CreatedAt | DateTime | 创建时间 |

## 配置

### appsettings.json

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "your-internal-token",
    "TimeoutSeconds": 300
  }
}
```

## 注意事项

1. **统一错误处理**：所有 AI 调用都通过 `AiAgentService` 基类统一处理错误和日志
2. **中文注释**：所有代码注释使用中文，便于团队维护
3. **路由前缀**：所有智能体相关接口统一使用 `/api/ai/` 前缀
4. **日志记录**：所有 AI 调用都会记录到 `AiAgentLog` 表，便于问题排查

