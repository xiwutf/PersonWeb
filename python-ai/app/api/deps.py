"""
API 依赖项
"""

from fastapi import Request
from app.core.logging import get_logger, get_trace_id
from app.services.observability.traces import get_trace_id_from_request


def get_request_logger(request: Request):
    """
    获取请求日志记录器（带 traceId 和 path）
    
    Args:
        request: FastAPI 请求对象
        
    Returns:
        ContextualLoggerAdapter: 日志适配器
    """
    trace_id = get_trace_id_from_request(request)
    path = request.url.path
    return get_logger("api", trace_id=trace_id, path=path)

