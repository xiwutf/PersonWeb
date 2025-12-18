"""
通用 Schema 定义
"""

from pydantic import BaseModel
from typing import Optional, Dict, Any


class Message(BaseModel):
    """消息模型"""
    role: str  # system | user | assistant
    content: str


class Usage(BaseModel):
    """Token 使用量"""
    promptTokens: int = 0
    completionTokens: int = 0


class WebsiteChatResponse(BaseModel):
    """WebsiteChat 响应模型"""
    content: str
    usage: Usage
    traceId: str

