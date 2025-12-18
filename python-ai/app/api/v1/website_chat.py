"""
WebsiteChat API 路由
"""

from fastapi import APIRouter, Depends, HTTPException, status, Request
from app.schemas.website_chat import WebsiteChatRequest
from app.schemas.common import WebsiteChatResponse, Usage
from app.core.security import verify_internal_key
from app.api.deps import get_request_logger
from app.services.scenes.website_chat import WebsiteChatScene
from app.services.observability.traces import get_trace_id_from_request
from app.core.errors import SceneException, LLMException
import logging

router = APIRouter()
logger = logging.getLogger(__name__)


@router.post("/website-chat", response_model=WebsiteChatResponse)
async def website_chat(
    request_body: WebsiteChatRequest,
    request: Request,
    internal_key: str = Depends(verify_internal_key),
    request_logger=Depends(get_request_logger)
):
    """
    网站聊天接口（前台访客 AI 助手）
    
    Args:
        request_body: 请求体
        request: FastAPI 请求对象
        internal_key: 内部 API Key（通过依赖注入验证）
        request_logger: 请求日志记录器
        
    Returns:
        WebsiteChatResponse: 包含 content、usage、traceId 的响应
    """
    try:
        # 获取 traceId
        trace_id = get_trace_id_from_request(request)
        
        # 如果请求体中有 extra.traceId，优先使用
        if request_body.extra and "traceId" in request_body.extra:
            trace_id = request_body.extra["traceId"]
        
        request_logger.info(
            f"收到网站聊天请求: scene=website_chat, "
            f"has_messages={bool(request_body.messages)}, "
            f"has_user_input={bool(request_body.user_input)}",
            extra={"scene": "website_chat"}
        )
        
        # 验证请求
        if not request_body.messages and not request_body.user_input:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="必须提供 messages 或 user_input"
            )
        
        # 构建 payload
        payload = {
            "messages": [msg.model_dump() for msg in request_body.messages] if request_body.messages else None,
            "user_input": request_body.user_input,
            "extra": request_body.extra or {},
            "model": request_body.model,
            "temperature": request_body.temperature,
            "max_tokens": request_body.max_tokens,
        }
        
        # 执行 Scene
        scene = WebsiteChatScene()
        result = await scene.run(payload, trace_id)
        
        # 构建响应
        usage = Usage(
            promptTokens=result["usage"].get("promptTokens", 0),
            completionTokens=result["usage"].get("completionTokens", 0)
        )
        
        response = WebsiteChatResponse(
            content=result["content"],
            usage=usage,
            traceId=trace_id
        )
        
        request_logger.info(
            f"网站聊天完成: content_length={len(result['content'])}, "
            f"tokens={usage.promptTokens + usage.completionTokens}",
            extra={"scene": "website_chat"}
        )
        
        return response
        
    except HTTPException:
        raise
    except (SceneException, LLMException) as e:
        request_logger.error(f"网站聊天处理失败: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"AI 服务处理失败: {str(e)}"
        )
    except Exception as e:
        request_logger.error(f"网站聊天接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="服务器内部错误"
        )

