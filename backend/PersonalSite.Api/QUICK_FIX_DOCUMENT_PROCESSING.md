# 文档处理问题快速修复指南

## 问题：文档一直处于"处理中"状态

### 快速检查清单

#### ✅ 1. 检查 Python Agent 是否运行

```powershell
# 在 PowerShell 中运行
curl http://localhost:8001/health
```

**应该返回：**
```json
{"success":true,"data":{"status":"ok","service":"ai-service","version":"0.1.0"}}
```

**如果失败：**
- 启动 Python Agent：`cd ai-service && uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload`

#### ✅ 2. 检查配置文件

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

**检查 `ai-service/.env`：**

```bash
AI_INTERNAL_TOKEN=default-internal-token-change-in-production
DEEPSEEK_API_KEY=your-deepseek-api-key-here
LLM_PROVIDER=deepseek
```

**重要：** `InternalToken` 和 `AI_INTERNAL_TOKEN` 必须完全一致！

#### ✅ 3. 查看 .NET 后端日志

在 .NET 后端控制台中查找：
- `"开始处理文档: DocumentId=..."`
- `"调用文档处理接口: DocumentId=..."`
- 任何错误信息

#### ✅ 4. 查看 Python Agent 日志

```powershell
# 在 ai-service 目录下
Get-Content logs/ai-service.log -Wait
```

查找：
- `"收到文档处理请求: document_id=..."`
- `"开始处理文档: document_id=..."`
- 任何错误信息

#### ✅ 5. 手动测试 Python Agent 接口

```powershell
# 设置 Token
$token = "default-internal-token-change-in-production"

# 测试接口（需要替换为实际的文件路径）
$body = @{
    document_id = "test-001"
    file_path = "D:\00-Project\AI\PersonWeb\backend\PersonalSite.Api\uploads\documents\实际文件名.txt"
    file_type = "txt"
    user_id = "test-user"
} | ConvertTo-Json

$headers = @{
    "X-Internal-Token" = $token
    "Content-Type" = "application/json"
}

Invoke-WebRequest -Uri "http://localhost:8001/api/ai/document/process" `
    -Method POST `
    -Headers $headers `
    -Body $body
```

#### ✅ 6. 检查数据库状态

```sql
-- 查看处理中的文档
SELECT id, title, status, error_message, created_at, processed_at 
FROM Document 
WHERE status = 'processing' 
ORDER BY created_at DESC;

-- 查看失败的文档
SELECT id, title, status, error_message 
FROM Document 
WHERE status = 'failed';
```

### 常见问题解决

#### 问题 1: 401 Unauthorized

**原因：** Token 不匹配

**解决：**
1. 检查 `.NET` 的 `appsettings.json` 中的 `InternalToken`
2. 检查 Python Agent 的 `.env` 中的 `AI_INTERNAL_TOKEN`
3. 确保两者完全一致
4. 重启两个服务

#### 问题 2: Connection refused

**原因：** Python Agent 未启动或端口错误

**解决：**
1. 检查 Python Agent 是否运行：`curl http://localhost:8001/health`
2. 检查端口 8001 是否被占用
3. 启动 Python Agent

#### 问题 3: File not found

**原因：** Python Agent 无法访问文件路径

**解决：**
1. 检查文件是否存在
2. 检查文件路径是否正确（Windows 路径格式）
3. 确保 Python Agent 有读取文件的权限

#### 问题 4: Timeout

**原因：** 文档处理时间过长

**解决：**
1. 增加 `TimeoutSeconds` 配置（已设置为 300 秒）
2. 检查 Python Agent 日志，看是否卡在某个步骤

### 调试步骤

1. **重启服务**
   - 重启 .NET 后端
   - 重启 Python Agent

2. **清除旧数据（可选）**
   ```sql
   -- 将处理中的文档重置为待处理
   UPDATE Document SET status = 'pending' WHERE status = 'processing';
   ```

3. **重新上传文档测试**

4. **查看完整日志**
   - .NET 后端控制台
   - Python Agent 日志文件

### 测试脚本

运行测试脚本：
```powershell
cd backend/PersonalSite.Api
.\test-document-agent.ps1
```

### 如果仍然失败

请提供以下信息：
1. .NET 后端控制台的完整错误日志
2. Python Agent 日志文件内容
3. 数据库中的文档状态和错误信息
4. 配置文件内容（隐藏敏感信息）

