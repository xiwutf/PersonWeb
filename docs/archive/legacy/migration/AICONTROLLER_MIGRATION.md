# AIController 迁移到 Python AI 服务

## 概述

将前台 AI 聊天功能（AIController）从「.NET 直接调用 LLM」迁移为「.NET 通过 HTTP 调用 Python AI 服务」。

## 迁移目标

- ✅ 所有 AI 模型调用统一走 Python 服务
- ✅ .NET 仅作为前端 API 网关，负责请求/响应透传
- ✅ 前端 API 路径、参数、返回完全不变
- ✅ Python 成为唯一 AI 调用出口

## 实现内容

### 1. Python 服务端

#### 新增 DTO 模型 (`ai-service/app/models/dto.py`)

```python
class WebsiteChatMessage(BaseModel):
    """网站聊天消息"""
    role: str  # system | user | assistant
    content: str

class WebsiteChatRequest(BaseModel):
    """网站聊天请求模型"""
    messages: List[WebsiteChatMessage]
    scene: str = "website-chat"
    extra: Optional[Dict[str, Any]]

class WebsiteChatResponseData(BaseModel):
    """网站聊天响应数据"""
    content: str  # AI 回复内容
    usage: Optional[ChatUsage]  # Token 使用量
```

#### 新增服务 (`ai-service/app/services/website_chat_service.py`)

- 处理网站聊天请求
- 分离 system prompt 和 user messages
- 调用 LLM 生成回复
- 返回 AI 回复内容和 token 使用量

#### 新增 API 路由 (`ai-service/app/api/chat.py`)

- `POST /api/ai/website-chat`
- 接收消息列表（包含 system 和 user 消息）
- 调用 `WebsiteChatService` 处理业务逻辑
- 返回统一格式的响应

### 2. .NET 服务端

#### 修改 AIController (`backend/PersonalSite.Api/Controllers/AIController.cs`)

**变更内容：**
- ❌ 移除直接调用 LLM SDK 的代码（DeepSeek/OpenAI API 调用）
- ✅ 改为使用 `HttpClient` 调用 Python 接口 `/api/ai/website-chat`
- ✅ 保留系统提示词构建逻辑（需要查询数据库获取文章和项目信息）
- ✅ 请求参数从前端原样映射到 Python 服务格式
- ✅ 响应内容原样返回给前端（保持前端 API 格式不变）

**关键实现：**
```csharp
// 构建系统提示词（需要查询数据库）
var systemPrompt = BuildSystemPrompt();

// 构建消息列表
var messages = new List<WebsiteChatMessageDto>
{
    new WebsiteChatMessageDto { Role = "system", Content = systemPrompt }
};
// ... 添加历史消息和当前消息

// 调用 Python AI 服务
var pythonRequest = new WebsiteChatRequestDto
{
    Messages = messages,
    Scene = "website-chat",
    Extra = new Dictionary<string, object> { { "page", "home" } }
};

var response = await _httpClient.SendAsync(requestMessage);
var result = await response.Content.ReadFromJsonAsync<AiServiceResponse<WebsiteChatResponseDto>>();

// 返回格式与前端保持一致
return Ok(ApiResponse.Success(new { message = result.Data.Content }));
```

### 3. 配置

#### .NET 配置 (`backend/PersonalSite.Api/appsettings.json`)

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "default-internal-token-change-in-production",
    "TimeoutSeconds": 300
  }
}
```

#### Python 服务配置

- 使用现有的 LLM 配置（DeepSeek/OpenAI）
- 通过环境变量或 `.env` 文件配置 API Key

## 数据格式

### 请求格式（.NET → Python）

```json
{
  "messages": [
    { "role": "system", "content": "..." },
    { "role": "user", "content": "..." }
  ],
  "scene": "website-chat",
  "extra": {
    "page": "home",
    "traceId": "optional"
  }
}
```

### 响应格式（Python → .NET）

```json
{
  "success": true,
  "data": {
    "content": "AI 回复内容",
    "usage": {
      "promptTokens": 100,
      "completionTokens": 50,
      "totalTokens": 150
    }
  },
  "error_code": null,
  "message": ""
}
```

### 前端 API 格式（保持不变）

**请求：**
```json
{
  "message": "用户消息",
  "history": [
    { "role": "user", "content": "..." },
    { "role": "assistant", "content": "..." }
  ]
}
```

**响应：**
```json
{
  "success": true,
  "data": {
    "message": "AI 回复内容"
  },
  "error": null
}
```

## 字段命名约定

- Python 服务内部使用 `snake_case`（如 `prompt_tokens`）
- JSON 序列化时使用 `camelCase`（如 `promptTokens`）
- .NET DTO 使用 `PascalCase`，通过 `JsonPropertyName` 属性映射到 `camelCase`

## 错误处理

### Python 服务异常
- 记录详细日志
- 返回统一错误格式
- HTTP 状态码：400（请求错误）、500（服务器错误）

### .NET 服务异常
- 捕获 `HttpRequestException`（网络错误）
- 返回友好错误信息给前端
- 记录错误日志

## 测试验证

### 验收标准

1. ✅ 前台 AI 聊天功能可正常使用
2. ✅ .NET 不再直接调用任何 LLM SDK
3. ✅ Python 成为唯一 AI 调用出口
4. ✅ 现有 Prompt 行为与之前基本一致
5. ✅ 前端 API 路径、参数、返回完全不变

### 测试步骤

1. 启动 Python AI 服务（`http://localhost:8001`）
2. 启动 .NET WebAPI
3. 前端调用 `/api/ai/chat` 接口
4. 验证：
   - 请求能正常发送到 Python 服务
   - Python 服务能正常调用 LLM
   - 响应能正常返回给前端
   - 前端显示正常

## 注意事项

1. **系统提示词构建**：仍在 .NET 端完成，因为需要查询数据库获取文章和项目信息
2. **历史消息处理**：当前实现将历史消息直接传递给 Python 服务，Python 服务会正确处理 system/user/assistant 消息
3. **Token 使用量**：Python 服务返回 token 使用量，但前端暂不使用，仅用于监控和日志
4. **配置管理**：生产环境需要修改 `InternalToken` 为安全的随机字符串

## 相关文件

### Python 服务
- `ai-service/app/models/dto.py` - DTO 模型定义
- `ai-service/app/services/website_chat_service.py` - 网站聊天服务
- `ai-service/app/api/chat.py` - API 路由
- `ai-service/app/main.py` - 路由注册

### .NET 服务
- `backend/PersonalSite.Api/Controllers/AIController.cs` - AI 聊天控制器
- `backend/PersonalSite.Api/appsettings.json` - 配置文件
- `backend/PersonalSite.Api/Program.cs` - 服务注册（已配置 `AiServiceOptions`）

## 后续优化

1. **上下文管理**：实现基于 session_id 的上下文管理（当前历史消息直接传递）
2. **流式响应**：支持流式返回 AI 回复（提升用户体验）
3. **错误重试**：在 Python 服务端实现 LLM 调用失败时的自动重试
4. **监控告警**：添加 AI 服务调用的监控和告警机制

