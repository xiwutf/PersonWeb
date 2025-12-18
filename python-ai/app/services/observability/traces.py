"""
追踪模块
提供 traceId 生成和管理功能
"""

import uuid
from typing import Optional
from fastapi import Request


def generate_trace_id() -> str:
    """
    生成新的 traceId
    
    Returns:
        str: UUID 格式的 traceId
    """
    return str(uuid.uuid4())


def get_trace_id_from_request(request: Request) -> str:
    """
    从请求中获取 traceId
    
    优先级：
    1. request.state.trace_id（由 middleware 注入）
    2. Header X-Trace-Id
    3. extra.traceId（如果存在）
    4. 生成新的 traceId
    
    Args:
        request: FastAPI 请求对象
        
    Returns:
        str: traceId
    """
    # 1. 从 request.state 获取（由 middleware 注入）
    if hasattr(request.state, "trace_id"):
        return request.state.trace_id
    
    # 2. 从 Header 获取
    trace_id = request.headers.get("X-Trace-Id")
    if trace_id:
        return trace_id
    
    # 3. 从 extra 参数获取（需要从请求体中解析，这里暂时不处理）
    # 如果后续需要，可以在 API 层处理
    
    # 4. 生成新的 traceId
    return generate_trace_id()

