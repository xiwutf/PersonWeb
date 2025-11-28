"""
通用聊天接口
提供基础的 AI 对话能力
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.logging import logger
from app.models.dto import (
    BaseResponse,
    ChatRequest,
    ChatResponseData
)
from app.services.chat_service import ChatService

router = APIRouter()
chat_service = ChatService()


@router.post("/chat", response_model=BaseResponse)
async def chat(
    request: ChatRequest,
    token: str = Depends(check_internal_token)
):
    """
    通用聊天接口
    
    接收用户消息，调用大模型生成回复
    
    Args:
        request: 聊天请求数据
        token: 内部调用 Token（通过依赖注入验证）
        
    Returns:
        BaseResponse: 包含模型回复的响应
    """
    try:
        logger.info(f"收到聊天请求: user_id={request.user_id}, session_id={request.session_id}")
        
        # 调用服务层处理业务逻辑
        result = await chat_service.chat(
            user_id=request.user_id,
            session_id=request.session_id,
            message=request.message,
            model=request.model,
            meta=request.meta
        )
        
        return BaseResponse(
            success=True,
            data=result,
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"聊天接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "CHAT_ERROR",
                "message": f"聊天处理失败: {str(e)}",
                "data": None
            }
        )

