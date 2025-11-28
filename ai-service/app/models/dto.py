"""
请求/响应数据模型定义
使用 Pydantic 定义所有 API 的请求和响应模型
"""

from pydantic import BaseModel, Field
from typing import Optional, Dict, List, Any


# ==================== 通用响应模型 ====================

class BaseResponse(BaseModel):
    """基础响应模型"""
    success: bool
    error_code: Optional[str] = None
    message: str = ""
    data: Optional[Any] = None


# ==================== 健康检查相关 ====================

class HealthResponse(BaseModel):
    """健康检查响应数据"""
    status: str
    service: str
    version: str
    timestamp: Optional[str] = None


# ==================== 聊天相关 ====================

class ChatRequest(BaseModel):
    """聊天请求模型"""
    user_id: str = Field(..., description="用户 ID")
    session_id: Optional[str] = Field(None, description="会话 ID，用于上下文管理")
    message: str = Field(..., min_length=1, description="用户输入的消息")
    model: Optional[str] = Field(None, description="指定使用的模型")
    meta: Optional[Dict[str, Any]] = Field(None, description="额外信息，如来源、UA 等")


class ChatUsage(BaseModel):
    """Token 使用量"""
    prompt_tokens: int
    completion_tokens: int
    total_tokens: int


class ChatResponseData(BaseModel):
    """聊天响应数据"""
    reply: str = Field(..., description="模型回复文本")
    model: str = Field(..., description="实际使用的模型名称")
    usage: Optional[ChatUsage] = Field(None, description="Token 使用量")


# ==================== AI 工具相关 ====================

class SummarizeRequest(BaseModel):
    """文章摘要请求模型"""
    text: str = Field(..., min_length=1, description="原文内容")
    max_length: Optional[int] = Field(100, ge=10, le=500, description="期望摘要长度")


class SummarizeResponseData(BaseModel):
    """摘要响应数据"""
    summary: str = Field(..., description="生成的摘要")


class TitleAndTagsRequest(BaseModel):
    """标题和标签生成请求模型"""
    text: str = Field(..., min_length=1, description="文章内容")
    max_tags: Optional[int] = Field(5, ge=1, le=20, description="标签数量上限")


class TitleAndTagsResponseData(BaseModel):
    """标题和标签响应数据"""
    title: str = Field(..., description="生成的标题")
    tags: List[str] = Field(..., description="生成的标签列表")


class CodeExplainRequest(BaseModel):
    """代码解释请求模型"""
    code: str = Field(..., min_length=1, description="代码内容")
    language: Optional[str] = Field(None, description="编程语言")


class CodeExplainResponseData(BaseModel):
    """代码解释响应数据"""
    explanation: str = Field(..., description="代码功能与逻辑的中文解释")


# ==================== RAG 知识库相关 ====================

class UpsertDocRequest(BaseModel):
    """文档入库/更新请求模型"""
    doc_id: str = Field(..., description="文档 ID")
    user_id: str = Field(..., description="用户 ID")
    title: str = Field(..., description="文档标题")
    content: str = Field(..., min_length=1, description="文档内容")
    tags: Optional[List[str]] = Field(None, description="文档标签")
    url: Optional[str] = Field(None, description="文档 URL")


class UpsertDocResponseData(BaseModel):
    """文档入库响应数据"""
    doc_id: str
    success: bool


class RelevantDoc(BaseModel):
    """相关文档信息"""
    doc_id: str
    title: str
    url: Optional[str] = None
    score: float = Field(..., ge=0.0, le=1.0, description="相似度分数")


class RAGQueryRequest(BaseModel):
    """知识库问答请求模型"""
    user_id: str = Field(..., description="用户 ID")
    query: str = Field(..., min_length=1, description="查询问题")
    top_k: Optional[int] = Field(5, ge=1, le=20, description="返回相关文档数量")


class RAGQueryResponseData(BaseModel):
    """知识库问答响应数据"""
    answer: str = Field(..., description="生成的答案")
    relevant_docs: List[RelevantDoc] = Field(..., description="相关文档列表")

