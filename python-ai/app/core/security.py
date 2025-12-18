"""
安全模块
提供内网鉴权等功能
"""

from fastapi import Header, HTTPException, status
from typing import Optional
from app.core.config import get_settings
from app.core.errors import SecurityException


async def verify_internal_key(
    x_internal_key: Optional[str] = Header(None, alias="X-Internal-Key")
) -> str:
    """
    验证内部 API Key
    
    规则：
    - 如果未配置 INTERNAL_API_KEY，dev 环境允许放行，prod 环境必须校验
    - 如果已配置，则必须匹配
    
    Args:
        x_internal_key: 请求头中的内部 Key
        
    Returns:
        str: 验证通过返回 key
        
    Raises:
        HTTPException: 验证失败时抛出
    """
    settings = get_settings()
    
    # 如果未配置 INTERNAL_API_KEY
    if not settings.INTERNAL_API_KEY:
        # dev 环境允许放行
        if settings.ENV == "dev":
            return x_internal_key or "dev-bypass"
        # prod 环境必须配置
        else:
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail="内部 API Key 未配置，生产环境必须配置"
            )
    
    # 如果已配置，则必须匹配
    if not x_internal_key or x_internal_key != settings.INTERNAL_API_KEY:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="内部 API Key 验证失败"
        )
    
    return x_internal_key

