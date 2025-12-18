"""
重试机制
实现轻量级重试装饰器，仅对网络超时/5xx 重试
"""

import asyncio
import functools
from typing import Callable, Any
from httpx import TimeoutException, HTTPStatusError
from app.core.errors import LLMTimeoutException, LLMNetworkException


def retry_on_network_error(max_retries: int = 2, base_delay: float = 1.0):
    """
    重试装饰器（仅对网络超时/5xx 重试）
    
    Args:
        max_retries: 最大重试次数（不包括首次调用）
        base_delay: 基础延迟时间（秒），使用指数退避
    """
    def decorator(func: Callable) -> Callable:
        @functools.wraps(func)
        async def wrapper(*args, **kwargs) -> Any:
            last_exception = None
            
            for attempt in range(max_retries + 1):
                try:
                    return await func(*args, **kwargs)
                except (TimeoutException, LLMTimeoutException) as e:
                    last_exception = e
                    if attempt < max_retries:
                        delay = base_delay * (2 ** attempt)  # 指数退避
                        await asyncio.sleep(delay)
                        continue
                    else:
                        raise LLMTimeoutException(f"请求超时，已重试 {max_retries} 次: {str(e)}")
                except HTTPStatusError as e:
                    last_exception = e
                    # 仅对 5xx 错误重试
                    if e.response.status_code >= 500 and attempt < max_retries:
                        delay = base_delay * (2 ** attempt)
                        await asyncio.sleep(delay)
                        continue
                    else:
                        raise LLMNetworkException(f"HTTP 错误: {str(e)}")
                except Exception as e:
                    # 其他异常不重试
                    raise
            
            # 如果所有重试都失败
            if last_exception:
                raise last_exception
                
        return wrapper
    return decorator

