# DeepSeek 配置说明

## 快速配置

### 1. 编辑 .env 文件

在 `ai-service` 目录下编辑 `.env` 文件，设置以下配置：

```bash
# 模型提供商选择
LLM_PROVIDER=deepseek

# DeepSeek API 配置
DEEPSEEK_API_KEY=your-deepseek-api-key-here
DEEPSEEK_BASE_URL=https://api.deepseek.com/v1
DEEPSEEK_MODEL_NAME=deepseek-chat

# 内部鉴权 Token（必须修改为强密码）
AI_INTERNAL_TOKEN=your-strong-internal-token-here
```

### 2. 验证配置

启动服务后，可以通过以下方式验证：

```bash
# 启动服务
uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload

# 测试健康检查
curl http://localhost:8001/health
```

## 已集成的功能

### ✅ 已实现

1. **LLM 客户端集成**
   - ✅ DeepSeek API 调用（`llm_client.py`）
   - ✅ 支持聊天、摘要、问答等功能
   - ✅ 错误处理和降级机制

2. **文档处理功能**
   - ✅ 文档摘要生成（使用 DeepSeek）
   - ✅ 文档问答（使用 DeepSeek）
   - ✅ 关键点提取（使用 DeepSeek）

### ⚠️ 待完善

1. **文档解析**
   - ⚠️ PDF 解析（需要安装 PyPDF2 或 pdfplumber）
   - ⚠️ DOCX 解析（需要安装 python-docx）
   - ✅ TXT/MD 解析（已实现）

2. **向量数据库**
   - ⚠️ 向量存储和检索（当前为占位实现）
   - 建议使用 Chroma、Milvus 或 Qdrant

3. **Embedding 模型**
   - ⚠️ 文本向量化（当前为占位实现）
   - 可以使用 OpenAI Embeddings 或其他 Embedding 服务

## 使用示例

### 文档处理流程

1. 用户上传文档 → .NET 后端保存文件
2. .NET 后端调用 Python Agent 处理文档
3. Python Agent 执行：
   - 解析文档提取文本
   - 文本分段
   - 使用 DeepSeek 生成摘要
   - 构建知识结构
   - 向量化并存储（待实现）

### 文档问答流程

1. 用户提问 → .NET 后端接收请求
2. .NET 后端调用 Python Agent
3. Python Agent 执行：
   - 将问题向量化（待实现）
   - 检索相关文档片段（待实现）
   - 使用 DeepSeek 生成答案 ✅

## 配置项说明

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| `LLM_PROVIDER` | 模型提供商：deepseek/openai/qwen | deepseek |
| `DEEPSEEK_API_KEY` | DeepSeek API Key | - |
| `DEEPSEEK_BASE_URL` | DeepSeek API 地址 | https://api.deepseek.com/v1 |
| `DEEPSEEK_MODEL_NAME` | DeepSeek 模型名称 | deepseek-chat |
| `AI_INTERNAL_TOKEN` | 内部调用 Token | - |

## 注意事项

1. **API Key 安全**
   - 不要将 `.env` 文件提交到 Git
   - 生产环境使用环境变量或密钥管理服务

2. **Token 限制**
   - DeepSeek API 有 token 限制，注意控制输入长度
   - 文档分段时已考虑此限制（默认 1000 字符/段）

3. **错误处理**
   - 如果 DeepSeek API 调用失败，系统会降级到模拟回复
   - 检查日志文件 `logs/ai-service.log` 查看详细错误

4. **性能优化**
   - 文档处理是异步的，不会阻塞用户操作
   - 大文档会自动分段处理

## 故障排查

### 问题：API 调用失败

**检查项：**
1. API Key 是否正确
2. 网络连接是否正常
3. API 配额是否用完

**解决方案：**
```bash
# 查看日志
tail -f logs/ai-service.log

# 测试 API 连接
curl -X POST https://api.deepseek.com/v1/chat/completions \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{"model":"deepseek-chat","messages":[{"role":"user","content":"Hello"}]}'
```

### 问题：摘要生成失败

**检查项：**
1. 文档内容是否为空
2. 文档长度是否超过限制
3. LLM 服务是否正常

**解决方案：**
- 系统会自动降级到简单截断摘要
- 检查日志查看具体错误信息

## 下一步计划

1. **完善文档解析**
   - [ ] 集成 PyPDF2 或 pdfplumber
   - [ ] 集成 python-docx
   - [ ] 提取文档元数据

2. **完善向量数据库**
   - [ ] 选择并集成向量数据库（推荐 Chroma）
   - [ ] 实现向量存储和检索

3. **完善 Embedding**
   - [ ] 集成 Embedding API
   - [ ] 支持批量向量化

4. **优化性能**
   - [ ] 实现流式响应
   - [ ] 添加缓存机制
   - [ ] 优化提示词

