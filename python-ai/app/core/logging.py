"""
日志配置
统一日志格式，包含 traceId、scene 等结构化字段
"""

import logging
import sys
from typing import Optional
from fastapi import Request


def setup_logging(log_level: str = "INFO"):
    """
    配置日志系统
    
    Args:
        log_level: 日志级别
    """
    # 配置日志格式
    log_format = "%(asctime)s - %(name)s - %(levelname)s - [traceId=%(traceId)s] [path=%(path)s] [scene=%(scene)s] - %(message)s"
    
    # 创建格式化器
    formatter = logging.Formatter(log_format)
    
    # 配置根日志记录器
    root_logger = logging.getLogger()
    root_logger.setLevel(getattr(logging, log_level.upper(), logging.INFO))
    
    # 清除现有处理器
    root_logger.handlers.clear()
    
    # 添加控制台处理器
    console_handler = logging.StreamHandler(sys.stdout)
    console_handler.setFormatter(formatter)
    root_logger.addHandler(console_handler)


class ContextualLoggerAdapter(logging.LoggerAdapter):
    """带上下文的日志适配器"""
    
    def __init__(self, logger: logging.Logger, trace_id: Optional[str] = None, 
                 path: Optional[str] = None, scene: Optional[str] = None):
        super().__init__(logger, {})
        self.trace_id = trace_id
        self.path = path
        self.scene = scene
    
    def process(self, msg, kwargs):
        """处理日志消息，添加上下文信息"""
        extra = kwargs.get("extra", {})
        extra["traceId"] = self.trace_id or "unknown"
        extra["path"] = self.path or "unknown"
        extra["scene"] = self.scene or "unknown"
        kwargs["extra"] = extra
        return msg, kwargs


def get_logger(name: str, trace_id: Optional[str] = None, 
               path: Optional[str] = None, scene: Optional[str] = None) -> ContextualLoggerAdapter:
    """
    获取带上下文的日志记录器
    
    Args:
        name: 日志记录器名称
        trace_id: 追踪 ID
        path: 请求路径
        scene: 场景标识
        
    Returns:
        ContextualLoggerAdapter: 日志适配器
    """
    logger = logging.getLogger(name)
    return ContextualLoggerAdapter(logger, trace_id, path, scene)


def get_trace_id(request: Request) -> str:
    """
    从请求中获取 traceId
    
    Args:
        request: FastAPI 请求对象
        
    Returns:
        str: traceId
    """
    # 优先从 request.state 获取
    if hasattr(request.state, "trace_id"):
        return request.state.trace_id
    
    # 其次从 Header 获取
    trace_id = request.headers.get("X-Trace-Id")
    if trace_id:
        return trace_id
    
    # 最后从 extra 参数获取（如果存在）
    # 这里暂时返回 None，由 middleware 生成
    return "unknown"

