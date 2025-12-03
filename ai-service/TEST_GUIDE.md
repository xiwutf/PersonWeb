# 测试指南

## 服务已启动 ✅

服务正在运行在 `http://0.0.0.0:8001`

## 快速测试

### 1. 健康检查

```bash
# 基础健康检查
curl http://localhost:8001/health

# API 健康检查
curl http://localhost:8001/api/ai/health
```

### 2. 查看 API 文档

在浏览器中访问：
- Swagger UI: http://localhost:8001/docs
- ReDoc: http://localhost:8001/redoc

### 3. 测试文档处理接口

#### 准备测试文件

创建一个测试文本文件 `test.txt`：

```bash
echo "这是一个测试文档。文档内容包含多个段落。第一段介绍了基本概念。第二段详细说明了实现方法。第三段总结了关键要点。" > test.txt
```

#### 测试文档处理（需要内部 Token）

```bash
# 设置 Token（从 .env 文件中获取 AI_INTERNAL_TOKEN）
TOKEN="your-internal-token-here"

# 测试文档处理接口
curl -X POST "http://localhost:8001/api/ai/document/process" \
  -H "X-Internal-Token: $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "document_id": "test-001",
    "file_path": "./test.txt",
    "file_type": "txt",
    "user_id": "test-user"
  }'
```

#### 测试文档问答（需要内部 Token）

```bash
# 测试文档问答接口
curl -X POST "http://localhost:8001/api/ai/document/query" \
  -H "X-Internal-Token: $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "document_id": "test-001",
    "user_id": "test-user",
    "query": "这个文档的主要内容是什么？",
    "top_k": 3
  }'
```

## 通过 .NET 后端测试

### 1. 确保 .NET 后端配置正确

在 `.NET` 后端的 `appsettings.json` 中配置：

```json
{
  "AiServiceOptions": {
    "BaseUrl": "http://localhost:8001/api/ai",
    "TimeoutSeconds": 300
  }
}
```

### 2. 启动 .NET 后端

```bash
cd backend/PersonalSite.Api
dotnet run
```

### 3. 通过前端测试

1. 访问前端页面：`http://localhost:3000/admin/document-agent`
2. 上传一个测试文档（TXT 或 MD 格式）
3. 等待文档处理完成
4. 点击"问答"按钮，测试问答功能

## 测试检查清单

### ✅ 基础功能
- [ ] 服务启动成功
- [ ] 健康检查接口正常
- [ ] API 文档可以访问

### ✅ DeepSeek 集成
- [ ] 文档摘要生成正常
- [ ] 文档问答功能正常
- [ ] 关键点提取正常

### ✅ 文档处理流程
- [ ] 文档上传成功
- [ ] 文档解析正常
- [ ] 文本分段正常
- [ ] 摘要生成正常
- [ ] 知识结构构建正常

### ⚠️ 待完善功能
- [ ] PDF 文档解析（需要安装 PyPDF2）
- [ ] DOCX 文档解析（需要安装 python-docx）
- [ ] 向量数据库集成
- [ ] Embedding 模型集成

## 常见问题

### Q: 文档处理失败，提示 "DEEPSEEK_API_KEY 未配置"

**A:** 检查 `.env` 文件，确保已设置：
```bash
DEEPSEEK_API_KEY=your-deepseek-api-key-here
LLM_PROVIDER=deepseek
```

### Q: 调用接口返回 401 错误

**A:** 检查请求头中的 `X-Internal-Token` 是否与 `.env` 中的 `AI_INTERNAL_TOKEN` 一致。

### Q: 文档处理很慢

**A:** 这是正常的，因为需要：
1. 解析文档
2. 分段处理
3. 调用 DeepSeek API 生成摘要（可能需要几秒到几十秒）
4. 构建知识结构

大文档可能需要几分钟时间。

### Q: 如何查看详细日志

**A:** 查看日志文件：
```bash
tail -f logs/ai-service.log
```

或在 Windows PowerShell：
```powershell
Get-Content logs/ai-service.log -Wait
```

## 性能优化建议

1. **文档大小限制**：建议单个文档不超过 10MB
2. **分段大小**：默认 1000 字符，可根据需要调整
3. **并发处理**：当前为单线程处理，后续可优化为并发
4. **缓存机制**：可以为已处理的文档添加缓存

## 下一步

1. ✅ 服务已启动 - 完成
2. ⏭️ 测试基础功能
3. ⏭️ 测试文档上传和处理
4. ⏭️ 测试问答功能
5. ⏭️ 完善 PDF/DOCX 解析
6. ⏭️ 集成向量数据库

