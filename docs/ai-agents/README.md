# AI 智能体系统文档

## 概述

本系统实现了一套统一的 AI 智能体框架，支持多种业务场景的 AI 能力调用。系统采用分层架构，便于扩展和维护。

## 智能体列表

### 1. 内容生成智能体（ContentAgent）

**功能**：自动生成文章、项目介绍、工具介绍等内容

**接口**：`POST /api/ai/content/generate`

**请求参数**：
```json
{
  "type": "article|project_intro|tool_intro",
  "title": "标题（可选）",
  "tone": "mature|casual|technical",
  "targetAudience": "目标受众",
  "length": "short|medium|long",
  "extraNotes": "额外说明",
  "autoSaveDraft": true
}
```

**响应格式**：
```json
{
  "success": true,
  "content": {
    "title": "生成的标题",
    "summary": "摘要",
    "body": "正文内容（Markdown）"
  }
}
```

**使用方式**：
- 访问 `/admin/ai/content` 页面
- 填写表单参数
- 点击生成，可一键保存为文章草稿

### 2. Demo 上架智能体（DemoAgent）

**功能**：为项目和工具自动生成展示文案

**接口**：`POST /api/ai/demo/describe`

**请求参数**：
```json
{
  "projectId": "项目ID（与toolId二选一）",
  "toolId": "工具ID（与projectId二选一）",
  "name": "名称",
  "techStack": ["技术栈列表"],
  "usage": "用途",
  "targetAudience": "目标用户",
  "priceHint": "价格提示",
  "extraNotes": "额外说明"
}
```

**响应格式**：
```json
{
  "success": true,
  "demoDescription": {
    "title": "标题",
    "highlights": ["亮点1", "亮点2"],
    "features": "核心功能特性",
    "scenarios": "使用场景",
    "targetUsers": "目标用户",
    "shortCardText": "卡片短文本"
  }
}
```

**使用方式**：
- 在项目列表页（`/admin/projects`）中，点击项目操作列的「AI」按钮
- 填写项目信息
- 生成后自动保存到项目

### 3. 表单线索处理智能体（LeadAgent）

**功能**：分析客户咨询和需求，生成摘要、标签、评分和推荐建议

**接口**：`POST /api/ai/leads/analyze`

**请求参数**：
```json
{
  "leadId": 123,
  "rawText": "用户原始需求描述（可选）",
  "meta": {
    "budget": "预算范围",
    "deadline": "截止日期",
    "channel": "渠道"
  }
}
```

**响应格式**：
```json
{
  "success": true,
  "analysis": {
    "summary": "需求摘要",
    "tags": ["标签1", "标签2"],
    "score": 85,
    "recommendation": "推荐建议"
  }
}
```

**使用方式**：
- 在咨询列表页（`/admin/consultations`）中，点击咨询操作列的「AI」按钮
- 系统自动分析并保存结果到数据库

## 架构说明

### 数据流

```
前端页面
  ↓ (HTTP Request)
Controller (接收请求，参数验证)
  ↓ (调用 Service)
Service (业务逻辑，组装 Prompt)
  ↓ (调用 AiAgentService)
AiAgentService (统一调用接口)
  ↓ (调用 AiServiceClient)
AiServiceClient (HTTP 请求到 Python AI Service)
  ↓ (返回结果)
Service (处理响应，写入数据库)
  ↓ (记录日志到 AiAgentLog)
Controller (返回响应给前端)
  ↓ (HTTP Response)
前端页面 (展示结果)
```

### 核心类说明

1. **AiServiceClient**：统一的 AI 服务 HTTP 客户端，封装与 Python AI Service 的通信
2. **AiAgentService**：智能体服务基类，提供统一的调用接口和日志记录
3. **ContentAgentService**：内容生成智能体服务
4. **DemoAgentService**：Demo 上架智能体服务
5. **LeadAgentService**：表单线索处理智能体服务

### 数据库表

- **AiAgentLog**：记录所有 AI 调用的日志
- **Article**：文章表（内容生成智能体使用）
- **Project**：项目表（Demo 上架智能体使用，新增 AI 字段）
- **Tool**：工具表（Demo 上架智能体使用，新增 AI 字段）
- **PreSaleConsultation**：售前咨询表（线索处理智能体使用，新增 AI 字段）

## 扩展新智能体

### 步骤 1：创建 Service

在 `backend/PersonalSite.Api/Services/` 下创建新的 Service 类：

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

### 步骤 5：更新菜单和文档

- 在 `layouts/admin.vue` 中添加菜单入口
- 在 `pages/admin/ai/index.vue` 中添加智能体卡片
- 更新本文档

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
5. **数据库迁移**：新增字段需要执行对应的 SQL 脚本

## 数据库迁移脚本

执行以下 SQL 脚本以添加必要的字段：

1. `database/ai_agent_log_table.sql` - 创建 AI 调用日志表
2. `database/project_tool_ai_fields.sql` - 为 Project 和 Tool 表添加 AI 字段
3. `database/pre_sale_consultation_ai_fields.sql` - 为 PreSaleConsultation 表添加 AI 字段

