# Python AI 中台

提供统一的 AI 能力服务，基于 FastAPI + Pydantic 构建。

## 特性

- 🚀 **统一架构**：Scene 作为第一等公民，每个能力一个 Scene 类
- 📝 **Prompt 文件化**：所有 Prompt 模板存储在 `prompts/` 目录
- 🔍 **可观测性**：统一日志格式，包含 traceId、scene 等结构化字段
- 🔒 **安全验证**：内网 API Key 鉴权
- 🔄 **重试机制**：自动重试网络超时和 5xx 错误
- 🧪 **可测试**：完整的测试框架

## 目录结构

```
python-ai/
├─ app/
│  ├─ main.py                 # 应用入口
│  ├─ core/                   # 核心模块
│  │  ├─ config.py           # 配置管理
│  │  ├─ logging.py          # 日志配置
│  │  ├─ errors.py           # 异常定义
│  │  └─ security.py          # 安全验证
│  ├─ api/                    # API 路由
│  │  ├─ router.py           # 路由注册
│  │  ├─ deps.py             # 依赖项
│  │  └─ v1/                 # v1 版本 API
│  │     └─ website_chat.py # 网站聊天接口
│  ├─ schemas/                # Pydantic Schema
│  │  ├─ common.py           # 通用 Schema
│  │  └─ website_chat.py     # WebsiteChat Schema
│  ├─ services/               # 服务层
│  │  ├─ llm/                # LLM 客户端
│  │  │  ├─ client.py        # LLM 客户端
│  │  │  ├─ models.py        # 模型配置
│  │  │  └─ retry.py         # 重试机制
│  │  ├─ prompt/             # Prompt 处理
│  │  │  ├─ loader.py        # Prompt 加载器
│  │  │  ├─ renderer.py      # Prompt 渲染器
│  │  │  └─ guardrails.py    # Prompt 安全护栏
│  │  ├─ scenes/             # Scene 场景
│  │  │  ├─ base.py          # Scene 基类
│  │  │  └─ website_chat.py  # WebsiteChat Scene
│  │  └─ observability/      # 可观测性
│  │     └─ traces.py        # 追踪管理
│  ├─ prompts/               # Prompt 模板
│  │  └─ website_chat/       # WebsiteChat Prompt
│  │     ├─ system_v1.txt    # System Prompt
│  │     └─ user_v1.txt      # User Prompt
│  └─ tests/                 # 测试
│     └─ test_website_chat.py
├─ pyproject.toml            # 项目配置
└─ README.md                 # 本文档
```

## 安装依赖

### 使用 pip

```bash
cd python-ai
pip install -e .
```

### 使用 poetry（推荐）

```bash
cd python-ai
poetry install
```

### 使用 uv

```bash
cd python-ai
uv pip install -e .
```

## 配置环境变量

创建 `.env` 文件（或设置环境变量）：

```bash
# 应用配置
APP_NAME=python-ai-hub
ENV=dev  # dev | prod
LOG_LEVEL=INFO

# AI 服务配置
AI_BASE_URL=  # 可选，OpenAI 兼容网关地址，为空则使用官方 OpenAI
AI_API_KEY=sk-xxx  # 必需，API Key
AI_MODEL_DEFAULT=gpt-4o-mini  # 默认模型
AI_TIMEOUT_SECONDS=30  # 请求超时时间（秒）

# 内部 API 鉴权
INTERNAL_API_KEY=your-internal-key-here  # 可选，用于 .NET 调用 Python 的内网鉴权
```

### 环境变量说明

- `AI_BASE_URL`: 如果配置了自定义网关地址，会使用该地址；否则使用官方 OpenAI API
- `AI_API_KEY`: LLM 服务的 API Key（必需）
- `INTERNAL_API_KEY`: 内部 API Key，用于验证来自 .NET 的请求
  - 如果未配置，dev 环境允许放行，prod 环境必须配置
  - 如果已配置，则必须匹配请求头中的 `X-Internal-Key`

## 启动服务

### 开发模式（自动重载）

```bash
cd python-ai
uvicorn app.main:app --reload --host 0.0.0.0 --port 8001
```

### 生产模式

```bash
cd python-ai
uvicorn app.main:app --host 0.0.0.0 --port 8001 --workers 4
```

## 调用接口

### 健康检查

```bash
curl http://localhost:8001/healthz
```

### WebsiteChat 接口

#### 使用 user_input（单轮对话）

```bash
curl -X POST http://localhost:8001/api/ai/website-chat \
  -H "Content-Type: application/json" \
  -H "X-Internal-Key: your-internal-key-here" \
  -H "X-Trace-Id: custom-trace-id-123" \
  -d '{
    "user_input": "你好，介绍一下你自己",
    "extra": {
      "page": "home",
      "traceId": "optional-trace-id"
    }
  }'
```

#### 使用 messages（多轮对话）

```bash
curl -X POST http://localhost:8001/api/ai/website-chat \
  -H "Content-Type: application/json" \
  -H "X-Internal-Key: your-internal-key-here" \
  -H "X-Trace-Id: custom-trace-id-123" \
  -d '{
    "messages": [
      {"role": "user", "content": "你好"},
      {"role": "assistant", "content": "你好！我是小智，有什么可以帮助你的吗？"},
      {"role": "user", "content": "介绍一下溪午听风"}
    ]
  }'
```

#### 响应格式

```json
{
  "content": "AI 回复内容",
  "usage": {
    "promptTokens": 100,
    "completionTokens": 50
  },
  "traceId": "trace-id-123"
}
```

## 运行测试

```bash
cd python-ai
pytest
```

## 扩展新能力

### 1. 创建 Scene

在 `app/services/scenes/` 下创建新的 Scene 类，继承 `BaseScene`：

```python
from app.services.scenes.base import BaseScene

class MyNewScene(BaseScene):
    @property
    def scene_name(self) -> str:
        return "my_new_scene"
    
    def build_messages(self, payload):
        # 构建消息列表
        pass
    
    def postprocess(self, text):
        # 后处理
        return text
```

### 2. 创建 Prompt 模板

在 `app/prompts/my_new_scene/` 下创建 Prompt 文件：
- `system_v1.txt`
- `user_v1.txt`

### 3. 创建 Schema

在 `app/schemas/` 下创建请求/响应 Schema。

### 4. 创建 API 路由

在 `app/api/v1/` 下创建新的路由文件，注册到 `router.py`。

## 架构说明

### Scene 模式

每个 AI 能力对应一个 Scene 类，Scene 负责：
- 构建消息列表（`build_messages`）
- 后处理回复（`postprocess`）
- 统一执行流程（`run`：build -> llm.chat -> postprocess）

### Prompt 文件化

所有 Prompt 模板存储在 `prompts/` 目录，按 `scene/template_type_version.txt` 命名：
- `prompts/website_chat/system_v1.txt`
- `prompts/website_chat/user_v1.txt`

### 可观测性

- **TraceId**: 每个请求自动生成/提取 traceId，注入到日志和响应中
- **结构化日志**: 日志包含 traceId、path、scene 等结构化字段
- **统一错误处理**: 所有异常统一处理，返回友好错误信息

### 安全

- **内网鉴权**: 通过 `X-Internal-Key` Header 验证内部调用
- **环境区分**: dev 环境允许未配置时放行，prod 环境必须配置

## 注意事项

1. **Prompt 模板变量**: 使用 Python `format` 方法渲染，确保模板中的变量名与传入的 kwargs 匹配
2. **LLM 客户端**: 使用全局单例，避免重复创建连接
3. **重试机制**: 仅对网络超时和 5xx 错误重试，最多 2 次，使用指数退避
4. **TraceId 优先级**: extra.traceId > Header X-Trace-Id > 自动生成

## 许可证

MIT

