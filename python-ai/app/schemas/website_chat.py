"""
WebsiteChat Schema 定义
"""

from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any
from app.schemas.common import Message


class WebsiteChatRequest(BaseModel):
    """WebsiteChat 请求模型"""
    messages: Optional[List[Message]] = Field(None, description="消息列表（前端对话消息数组）")
    user_input: Optional[str] = Field(None, description="用户输入（单轮对话）")
    extra: Optional[Dict[str, Any]] = Field(None, description="额外信息，包含 page、scene、traceId 等")
    model: Optional[str] = Field(None, description="指定模型")
    temperature: Optional[float] = Field(None, description="温度参数")
    max_tokens: Optional[int] = Field(None, description="最大 token 数")
    
    class Config:
        # 允许使用字段名或别名
        populate_by_name = True

