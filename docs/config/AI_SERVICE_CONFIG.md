# AI 服务配置说明

AI 小智、文档管家等 AI 功能依赖 Python ai-service 与 .NET 后端的配合。本文说明如何配置，以及与其他隐私数据的配置方式一致性。

## 隐私数据配置方式概览

本项目敏感配置**不提交到 Git**，采用以下模式：

| 配置类型 | 模板文件 | 实际配置（不提交） | 说明 |
|---------|----------|---------------------|------|
| .NET 开发环境 | `appsettings.Development.json.example` | `appsettings.Development.json` | 复制后填写，含数据库、JWT、AiService |
| .NET 生产环境 | `appsettings.Production.json.example` | `appsettings.Production.json` | 服务器上手动创建，含数据库、JWT、AiService |
| 前端开发 | `.env.example` | `.env` | 复制后填写 API 地址 |
| 前端生产 | - | `.env.production` | 构建时使用，配置生产 API |
| Python ai-service | `ai-service/.env.example` | `ai-service/.env` | 复制后填写 API Key、Token |

**原则**：模板文件（`.example`）可提交；实际配置（含密码、Key、Token）不提交。

## AI 服务配置

### 开发环境

#### 1. .NET 后端

复制模板并编辑：

```bash
cd backend/PersonalSite.Api
copy appsettings.Development.json.example appsettings.Development.json
# 编辑 appsettings.Development.json，确保 AiService 段存在
```

`appsettings.Development.json` 中需包含：

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "default-internal-token-change-in-production",
    "TimeoutSeconds": 300
  }
}
```

#### 2. Python ai-service

```bash
cd ai-service
copy .env.example .env
# 编辑 .env，至少配置：
# AI_INTERNAL_TOKEN=default-internal-token-change-in-production  （与 .NET 一致）
# DEEPSEEK_API_KEY=你的DeepSeek密钥  （否则使用模拟回复）
```

#### 3. 启动顺序

1. ai-service：`uvicorn app.main:app --host 0.0.0.0 --port 8001`
2. .NET 后端：`dotnet run`
3. 前端：`npm run dev`

### 生产环境

#### 1. .NET 后端

在服务器部署目录创建 `appsettings.Production.json`（参考 [BACKEND_PRODUCTION_CONFIG](../../archive/legacy/deployment/BACKEND_PRODUCTION_CONFIG.md)）：

```json
{
  "ConnectionStrings": { "Default": "..." },
  "Jwt": { "Key": "...", "Issuer": "...", "Audience": "..." },
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "生产环境强随机Token",
    "TimeoutSeconds": 300
  }
}
```

**重要**：`InternalToken` 必须与 ai-service 的 `AI_INTERNAL_TOKEN` 完全一致。

#### 2. Python ai-service

在服务器上配置 `ai-service/.env` 或 systemd 环境变量：

```bash
AI_INTERNAL_TOKEN=与后端InternalToken相同的强随机Token
DEEPSEEK_API_KEY=生产环境DeepSeek密钥
```

#### 3. 生成强随机 Token

```bash
openssl rand -hex 32
```

将生成的字符串同时填入 .NET 的 `InternalToken` 和 ai-service 的 `AI_INTERNAL_TOKEN`。

## 故障排查

| 现象 | 可能原因 | 处理 |
|------|----------|------|
| 401 Unauthorized | Token 不一致或为空 | 检查 .NET `InternalToken` 与 ai-service `AI_INTERNAL_TOKEN` 是否一致 |
| 使用模拟回复 | DEEPSEEK_API_KEY 未配置 | 在 ai-service `.env` 中配置 `DEEPSEEK_API_KEY` |
| 服务暂时不可用 | ai-service 未启动或端口错误 | 确认 ai-service 运行在 8001，且 .NET `BaseUrl` 正确 |

## 相关文档

- [API 配置说明](./API_CONFIG.md)
- [后端生产配置](../../archive/legacy/deployment/BACKEND_PRODUCTION_CONFIG.md)
- [ai-service 部署](../../ai-service/DEPLOYMENT.md)
