"""
内部鉴权模块
校验来自主后端的请求 Token
"""

from fastapi import HTTPException, status, Header
from typing import Optional

from app.core.config import settings


def verify_internal_token(header_token: str) -> bool:
    """
    验证内部调用 Token
    
    Args:
        header_token: 请求头中的 Token
        
    Returns:
        bool: Token 是否有效
    """
    if not header_token:
        return False
    
    return header_token == settings.AI_INTERNAL_TOKEN


async def check_internal_token(
    x_internal_token: Optional[str] = Header(None, alias="X-Internal-Token")
) -> str:
    """
    中间件函数：检查内部 Token
    
    Args:
        x_internal_token: 请求头中的 X-Internal-Token
        
    Returns:
        str: Token 值
        
    Raises:
        HTTPException: Token 无效时抛出 401 错误
    """
    if not x_internal_token:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail={
                "success": False,
                "error_code": "MISSING_TOKEN",
                "message": "缺少内部调用 Token",
                "data": None
            }
        )
    
    if not verify_internal_token(x_internal_token):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail={
                "success": False,
                "error_code": "INVALID_TOKEN",
                "message": "内部调用 Token 无效",
                "data": None
            }
        )
    
    return x_internal_token

