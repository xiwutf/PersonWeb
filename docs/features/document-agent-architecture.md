# 文档知识管家 Agent 架构设计

## 一、整体架构

### 1.1 系统组件

```
┌─────────────────┐
│   Vue3 前端     │
│  (Nuxt 3)       │
│                 │
│ - 文档上传      │
│ - 文档列表      │
│ - 问答界面      │
└────────┬────────┘
         │ HTTP/HTTPS
         │
┌────────▼────────┐
│  .NET WebAPI    │
│                 │
│ - 文件上传      │
│ - 文档管理      │
│ - 问答接口      │
└────────┬────────┘
         │ HTTP
         │
┌────────▼────────┐
│ Python Agent    │
│                 │
│ - 文档解析      │
│ - 文本分段      │
│ - 生成摘要      │
│ - 构建知识结构  │
│ - 向量化存储    │
│ - 问答生成      │
└────────┬────────┘
         │
┌────────▼────────┐
│   数据库        │
│                 │
│ - MySQL         │
│   (文档元数据)   │
│ - 向量数据库    │
│   (文档向量)     │
└─────────────────┘
```

### 1.2 数据流

1. **文档上传流程**
   - 用户在前端上传 PDF/文档
   - 前端调用 .NET API 上传文件
   - .NET API 保存文件并调用 Python Agent
   - Python Agent 解析文档、分段、生成摘要、构建知识结构
   - Python Agent 将元数据返回给 .NET API
   - .NET API 保存元数据到 MySQL
   - Python Agent 将向量数据存储到向量数据库

2. **问答流程**
   - 用户在前端输入问题
   - 前端调用 .NET API 问答接口
   - .NET API 调用 Python Agent
   - Python Agent 进行向量检索，找到相关文档片段
   - Python Agent 调用 LLM 生成答案
   - 返回答案和相关文档给前端

## 二、数据库设计

### 2.1 MySQL 表结构

#### 文档表 (Document)
- id: 主键
- title: 文档标题
- file_name: 原始文件名
- file_path: 文件存储路径
- file_type: 文件类型 (pdf, docx, txt, etc.)
- file_size: 文件大小 (bytes)
- status: 处理状态 (pending, processing, completed, failed)
- summary: 文档摘要
- knowledge_structure: 知识结构 (JSON)
- total_chunks: 分段总数
- user_id: 用户 ID
- created_at: 创建时间
- updated_at: 更新时间
- processed_at: 处理完成时间

#### 文档分段表 (DocumentChunk)
- id: 主键
- document_id: 文档 ID (外键)
- chunk_index: 分段索引
- content: 分段内容
- summary: 分段摘要
- metadata: 元数据 (JSON)
- created_at: 创建时间

### 2.2 向量数据库

使用向量数据库（如 Chroma、Milvus、Qdrant 等）存储文档分段的向量表示：
- document_id: 文档 ID
- chunk_id: 分段 ID
- vector: 向量数据
- metadata: 元数据

## 三、Agent Tools 设计

### 3.1 文档解析工具 (parse_document)
- **输入**: file_path (文件路径), file_type (文件类型)
- **输出**: 
  - raw_text: 原始文本
  - metadata: 文档元数据 (页数、标题等)
- **功能**: 解析 PDF/DOCX/TXT 等格式文档，提取文本内容

### 3.2 文本分段工具 (chunk_text)
- **输入**: text (文本内容), chunk_size (分段大小), chunk_overlap (重叠大小)
- **输出**: 
  - chunks: 分段列表
  - chunk_metadata: 每个分段的元数据
- **功能**: 将长文本按照语义或固定大小分段

### 3.3 生成摘要工具 (generate_summary)
- **输入**: text (文本内容), summary_type (摘要类型: document/chunk)
- **输出**: 
  - summary: 摘要文本
  - key_points: 关键点列表
- **功能**: 使用 LLM 生成文档或分段的摘要

### 3.4 构建知识结构工具 (build_knowledge_structure)
- **输入**: chunks (分段列表), document_title (文档标题)
- **输出**: 
  - knowledge_structure: 知识结构 (JSON)
    - sections: 章节列表
    - topics: 主题列表
    - relationships: 关系列表
- **功能**: 分析文档内容，构建知识图谱或层次结构

### 3.5 向量化工具 (embed_text)
- **输入**: text (文本内容), model_name (模型名称)
- **输出**: 
  - vector: 向量数据
  - dimension: 向量维度
- **功能**: 将文本转换为向量表示

### 3.6 向量存储工具 (store_vectors)
- **输入**: vectors (向量列表), metadata (元数据列表)
- **输出**: 
  - success: 是否成功
  - stored_count: 存储数量
- **功能**: 将向量存储到向量数据库

### 3.7 向量检索工具 (search_vectors)
- **输入**: query_vector (查询向量), top_k (返回数量), document_id (可选，限定文档)
- **输出**: 
  - results: 检索结果列表
    - chunk_id: 分段 ID
    - document_id: 文档 ID
    - score: 相似度分数
    - content: 内容片段
- **功能**: 在向量数据库中检索相似文档片段

### 3.8 问答生成工具 (generate_answer)
- **输入**: query (问题), context (上下文文档片段), document_id (文档 ID)
- **输出**: 
  - answer: 答案文本
  - sources: 来源文档片段列表
  - confidence: 置信度
- **功能**: 基于检索到的文档片段，使用 LLM 生成答案

## 四、API 接口设计

### 4.1 .NET WebAPI 接口

#### 文档管理
- `POST /api/document/upload` - 上传文档
- `GET /api/document/list` - 获取文档列表
- `GET /api/document/{id}` - 获取文档详情
- `DELETE /api/document/{id}` - 删除文档
- `GET /api/document/{id}/chunks` - 获取文档分段列表

#### 问答接口
- `POST /api/document/{id}/query` - 对单个文档进行问答
- `POST /api/document/query` - 全局知识库问答（可选）

### 4.2 Python Agent 接口

- `POST /api/ai/document/parse` - 解析文档
- `POST /api/ai/document/process` - 处理文档（解析+分段+摘要+知识结构）
- `POST /api/ai/document/query` - 文档问答

## 五、技术栈

- **前端**: Vue 3 + Nuxt 3 + TypeScript
- **后端**: .NET 8 WebAPI + Entity Framework Core + MySQL
- **AI 服务**: Python + FastAPI + LangChain + 向量数据库
- **存储**: MySQL (元数据) + 向量数据库 (向量数据) + 文件系统 (原始文件)

