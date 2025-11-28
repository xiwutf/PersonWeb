# AI Service

AI 能力服务，基于 FastAPI 构建，提供统一的 AI 功能接口。

## 📋 功能特性

- ✅ **通用聊天接口**：支持与 AI 模型对话
- ✅ **AI 工具接口**：文章摘要、标题标签生成、代码解释
- ✅ **RAG 知识库**：文档入库和知识库问答（当前为 mock 实现）
- ✅ **内部鉴权**：通过 Token 验证请求来源
- ✅ **统一响应格式**：所有接口使用统一的 JSON 响应格式
- ✅ **日志记录**：完整的请求日志和错误日志

## 🚀 快速开始

### 环境要求

- Python 3.10+（推荐 3.11+）
- pip

### 1. 安装依赖

```bash
pip install -r requirements.txt
```

### 2. 配置环境变量

复制环境变量模板文件：

```bash
cp .env.example .env
```

编辑 `.env` 文件，设置必要的配置：

```bash
# 必须修改
AI_INTERNAL_TOKEN=your-internal-token-here

# 可选：配置大模型 API Key
OPENAI_API_KEY=your-openai-api-key
DEFAULT_MODEL_NAME=gpt-3.5-turbo
```

### 3. 启动服务

#### 开发模式（带热重载）

```bash
uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload
```

#### 生产模式

```bash
uvicorn app.main:app --host 0.0.0.0 --port 8001
```

### 4. 验证服务

访问健康检查接口：

```bash
curl http://localhost:8001/health
```

或访问：

```bash
curl http://localhost:8001/api/ai/health
```

## 📚 API 文档

启动服务后，访问以下地址查看自动生成的 API 文档：

- Swagger UI: http://localhost:8001/docs
- ReDoc: http://localhost:8001/redoc

## 🔌 API 接口说明

### 统一约定

- **路径前缀**：所有业务接口统一使用 `/api/ai` 前缀
- **鉴权方式**：所有接口需要在请求头中携带 `X-Internal-Token`
- **响应格式**：所有接口使用统一的 JSON 响应格式

### 响应格式

**成功响应**：
```json
{
  "success": true,
  "data": { ... },
  "error_code": null,
  "message": ""
}
```

**错误响应**：
```json
{
  "success": false,
  "error_code": "ERROR_CODE",
  "message": "错误描述",
  "data": null
}
```

### 接口列表

#### 1. 健康检查

- **GET** `/health` 或 `/api/ai/health`
- **无需鉴权**

#### 2. 通用聊天

- **POST** `/api/ai/chat`
- **需要鉴权**

请求示例：
```json
{
  "user_id": "user123",
  "session_id": "session456",
  "message": "你好",
  "model": "gpt-3.5-turbo"
}
```

#### 3. AI 工具接口

- **POST** `/api/ai/tools/summarize` - 文章摘要
- **POST** `/api/ai/tools/title-and-tags` - 标题标签生成
- **POST** `/api/ai/tools/code-explain` - 代码解释
- **需要鉴权**

#### 4. RAG 知识库接口

- **POST** `/api/ai/rag/upsert-doc` - 文档入库
- **POST** `/api/ai/rag/query` - 知识库问答
- **需要鉴权**

## 🧪 测试

运行测试：

```bash
pytest tests/
```

运行特定测试：

```bash
pytest tests/test_health.py -v
```

## 🖥️ ECS 服务器部署

### 快速部署

详细部署步骤请参考 [DEPLOYMENT.md](./DEPLOYMENT.md)。

#### 1. 创建目录并拉取代码

```bash
sudo mkdir -p /srv/ai-service
sudo chown $USER:$USER /srv/ai-service
cd /srv/ai-service
git clone <your-repo-url> .
```

#### 2. 安装依赖

```bash
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt
```

#### 3. 配置环境变量

创建 `.env` 文件并设置必要的环境变量（参考 `.env.example`）。

#### 4. 配置 Systemd 服务

```bash
# 复制服务文件
sudo cp ai-service.service /etc/systemd/system/

# 编辑配置（修改 Token 和 API Key）
sudo nano /etc/systemd/system/ai-service.service

# 启动服务
sudo systemctl daemon-reload
sudo systemctl start ai-service
sudo systemctl enable ai-service
```

#### 5. 配置 Nginx 反向代理

将 `nginx-ai-service.conf` 中的配置添加到你的 Nginx 配置中，并修改：

- `server_name`：你的实际域名
- `X-Internal-Token`：与 `.env` 中的 `AI_INTERNAL_TOKEN` 一致

```bash
sudo nginx -t
sudo nginx -s reload
```

#### 6. 验证部署

```bash
# 检查服务状态
sudo systemctl status ai-service

# 测试接口
curl http://127.0.0.1:8001/health
```

**重要提示**：

- AI Service 只监听 `127.0.0.1:8001`，不对外暴露
- 通过 Nginx 反向代理访问，路径为 `/_internal/ai/`
- 确保 `AI_INTERNAL_TOKEN` 在 `.env`、systemd 服务和 Nginx 配置中一致

详细说明请查看 [DEPLOYMENT.md](./DEPLOYMENT.md)。

## 🐳 Docker 部署

### 构建镜像

```bash
docker build -t ai-service:latest .
```

### 运行容器

```bash
docker run -d \
  -p 8001:8001 \
  -e AI_INTERNAL_TOKEN=your-token \
  -e OPENAI_API_KEY=your-key \
  --name ai-service \
  ai-service:latest
```

### 使用 Docker Compose

创建 `docker-compose.yml`：

```yaml
version: '3.8'

services:
  ai-service:
    build: .
    ports:
      - "8001:8001"
    environment:
      - AI_INTERNAL_TOKEN=your-token
      - OPENAI_API_KEY=your-key
    volumes:
      - ./logs:/app/logs
```

启动：

```bash
docker-compose up -d
```

## 📁 项目结构

```
ai-service/
├── app/
│   ├── __init__.py
│   ├── main.py              # FastAPI 入口
│   ├── api/                 # API 路由
│   │   ├── health.py        # 健康检查
│   │   ├── chat.py          # 聊天接口
│   │   ├── tools.py         # AI 工具接口
│   │   └── rag.py           # RAG 接口
│   ├── core/                # 核心模块
│   │   ├── config.py        # 配置管理
│   │   ├── llm_client.py    # 大模型客户端
│   │   ├── auth.py          # 鉴权
│   │   └── logging.py       # 日志
│   ├── services/            # 服务层
│   │   ├── chat_service.py  # 聊天服务
│   │   ├── tools_service.py # 工具服务
│   │   └── rag_service.py   # RAG 服务
│   └── models/              # 数据模型
│       └── dto.py           # 请求/响应模型
├── tests/                   # 测试
│   └── test_health.py
├── requirements.txt         # 依赖
├── Dockerfile               # Docker 配置
└── README.md               # 项目说明
```

## 🔧 配置说明

所有配置通过环境变量设置，主要配置项：

- `AI_INTERNAL_TOKEN`: 内部调用 Token（必须）
- `OPENAI_API_KEY`: OpenAI API Key（可选）
- `DEFAULT_MODEL_NAME`: 默认模型名称
- `LOG_LEVEL`: 日志级别（INFO/DEBUG/WARNING/ERROR）
- `LOG_FILE`: 日志文件路径

## 📝 开发说明

### 当前状态

- ✅ 项目结构完整
- ✅ 所有接口结构已实现
- ✅ 鉴权机制已实现
- ✅ 统一响应格式已实现
- ⚠️ 大模型调用为 mock 实现
- ⚠️ RAG 功能为 mock 实现

### 后续开发

1. **接入真实大模型**：
   - 在 `core/llm_client.py` 中实现真实的 API 调用
   - 支持 OpenAI、Qwen 等模型

2. **实现 RAG 功能**：
   - 接入向量数据库（如 Milvus、Pinecone）
   - 实现文档分段和向量化
   - 实现向量检索和上下文注入

3. **上下文管理**：
   - 基于 `session_id` 实现对话上下文
   - 使用 Redis 或其他存储管理会话

## 📄 许可证

MIT License

