"""
通用聊天接口
提供基础的 AI 对话能力
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import (
    BaseResponse,
    ChatRequest,
    ChatResponseData,
    WebsiteChatRequest,
    WebsiteChatResponseData
)
from app.services.chat_service import ChatService
from app.services.website_chat_service import WebsiteChatService

router = APIRouter()
chat_service = ChatService()
website_chat_service = WebsiteChatService()


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


@router.post("/website-chat", response_model=BaseResponse)
async def website_chat(
    request: WebsiteChatRequest,
    token: str = Depends(check_internal_token)
):
    """
    网站聊天接口（前台访客 AI 助手）
    
    接收消息列表（包含 system 和 user 消息），调用大模型生成回复
    
    Args:
        request: 网站聊天请求数据
        token: 内部调用 Token（通过依赖注入验证）
        
    Returns:
        BaseResponse: 包含 AI 回复内容和 token 使用量的响应
    """
    try:
        logger.info(
            f"收到网站聊天请求: scene={request.scene}, "
            f"messages_count={len(request.messages)}, extra={request.extra}"
        )
        
        # 验证请求
        if not request.messages or len(request.messages) == 0:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="消息列表不能为空"
            )
        
        # 调用服务层处理业务逻辑
        result = await website_chat_service.chat(
            messages=request.messages,
            scene=request.scene,
            extra=request.extra
        )
        
        return BaseResponse(
            success=True,
            data=result.model_dump(exclude_none=True, by_alias=True),  # 使用别名（camelCase）
            error_code=None,
            message=""
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"网站聊天接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "WEBSITE_CHAT_ERROR",
                "message": f"网站聊天处理失败: {str(e)}",
                "data": None
            }
        )

