"""
健康检查接口
提供服务的健康状态检查
"""

from fastapi import APIRouter
from datetime import datetime

from app.models.dto import BaseResponse, HealthResponse
from app.core.config import settings

router = APIRouter()


@router.get("/health", response_model=BaseResponse)
@router.get("/api/ai/health", response_model=BaseResponse)
async def health_check():
    """
    健康检查接口
    
    返回服务状态、版本等基本信息
    """
    return BaseResponse(
        success=True,
        data=HealthResponse(
            status="ok",
            service=settings.SERVICE_NAME,
            version=settings.VERSION,
            timestamp=datetime.now().isoformat()
        ),
        error_code=None,
        message=""
    )

