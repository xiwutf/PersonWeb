# 文档知识管家 Agent 故障排查

## 问题：文档一直处于"处理中"状态

### 可能的原因和解决方案

#### 1. Python Agent 服务未启动

**检查方法：**
```bash
# 检查 Python 服务是否运行
curl http://localhost:8001/health
```

**解决方案：**
```bash
cd ai-service
uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload
```

#### 2. 配置问题

**检查 `appsettings.json` 或 `appsettings.Development.json`：**

```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "default-internal-token-change-in-production",
    "TimeoutSeconds": 300
  }
}
```

**重要：**
- `BaseUrl` 必须指向正确的 Python Agent 地址
- `InternalToken` 必须与 Python Agent 的 `.env` 文件中的 `AI_INTERNAL_TOKEN` 一致

#### 3. 认证 Token 不匹配

**检查步骤：**

1. 检查 `.NET` 后端的 `appsettings.json`：
   ```json
   "AiService": {
     "InternalToken": "your-token-here"
   }
   ```

2. 检查 Python Agent 的 `.env` 文件：
   ```bash
   AI_INTERNAL_TOKEN=your-token-here
   ```

3. 确保两者完全一致（包括大小写和空格）

#### 4. 文件路径问题

**问题：** Python Agent 无法访问 .NET 后端保存的文件

**解决方案：**
- 确保文件路径是绝对路径
- 或者使用共享存储路径
- 检查文件权限

#### 5. 查看日志

**查看 .NET 后端日志：**
- 检查控制台输出
- 查看日志文件（如果配置了）

**查看 Python Agent 日志：**
```bash
tail -f ai-service/logs/ai-service.log
```

或在 Windows PowerShell：
```powershell
Get-Content ai-service/logs/ai-service.log -Wait
```

#### 6. 测试 Python Agent 接口

**手动测试文档处理接口：**

```bash
# 设置 Token（从 .env 文件中获取）
TOKEN="default-internal-token-change-in-production"

# 测试接口
curl -X POST "http://localhost:8001/api/ai/document/process" \
  -H "X-Internal-Token: $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "document_id": "test-001",
    "file_path": "D:/00-Project/AI/PersonWeb/backend/PersonalSite.Api/uploads/documents/test.txt",
    "file_type": "txt",
    "user_id": "test-user"
  }'
```

#### 7. 数据库状态检查

**检查文档状态：**
```sql
SELECT id, title, status, error_message, created_at, processed_at 
FROM Document 
WHERE status = 'processing' 
ORDER BY created_at DESC;
```

如果状态一直是 `processing`，可能是：
- 异步任务失败但未更新状态
- 需要手动更新状态为 `failed`

#### 8. 常见错误

**错误：401 Unauthorized**
- Token 不匹配或未设置
- 检查 `X-Internal-Token` 请求头

**错误：Connection refused**
- Python Agent 服务未启动
- 检查端口 8001 是否被占用

**错误：Timeout**
- 文档太大，处理时间超过超时设置
- 增加 `TimeoutSeconds` 配置

**错误：File not found**
- 文件路径不正确
- 检查文件是否存在

## 快速诊断步骤

1. ✅ 检查 Python Agent 是否运行：`curl http://localhost:8001/health`
2. ✅ 检查配置中的 BaseUrl 和 InternalToken
3. ✅ 检查日志文件中的错误信息
4. ✅ 手动测试 Python Agent 接口
5. ✅ 检查数据库中的文档状态和错误信息

## 开发环境配置示例

**`appsettings.Development.json`：**
```json
{
  "AiService": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "InternalToken": "default-internal-token-change-in-production",
    "TimeoutSeconds": 300
  }
}
```

**`ai-service/.env`：**
```bash
AI_INTERNAL_TOKEN=default-internal-token-change-in-production
DEEPSEEK_API_KEY=your-deepseek-api-key-here
LLM_PROVIDER=deepseek
```

确保两个文件中的 `InternalToken` 和 `AI_INTERNAL_TOKEN` 完全一致！

