"""
关系跟进助理接口
提供关系跟进总结和建议生成功能
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import (
    BaseResponse,
    RelationFollowupRequest,
    RelationFollowupResponseData
)
from app.services.relation_followup_service import RelationFollowupService

router = APIRouter()
relation_followup_service = RelationFollowupService()


@router.post("/relation/summarize", response_model=BaseResponse)
async def summarize_relation(
    request: RelationFollowupRequest,
    token: str = Depends(check_internal_token)
):
    """
    生成关系跟进总结和建议接口
    
    根据互动记录生成结构化总结、下一步建议和消息草案
    
    Args:
        request: 关系跟进请求数据
        token: 内部调用 Token（通过依赖注入验证）
        
    Returns:
        BaseResponse: 包含总结、建议、消息草案等的响应
    """
    try:
        logger.info(
            f"收到关系跟进总结请求: nickname={request.person.nickname}, "
            f"stage={request.person.stage}, interaction_type={request.interaction.type}"
        )
        
        # 验证请求
        if not request.person.nickname:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="对象昵称不能为空"
            )
        
        if not request.interaction.summary:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="互动摘要不能为空"
            )
        
        # 调用服务层处理业务逻辑
        result = await relation_followup_service.summarize(request)
        
        # 使用 model_dump 确保 JSON 字段名匹配
        return BaseResponse(
            success=True,
            data=result.model_dump(exclude_none=True),
            error_code=None,
            message=""
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"关系跟进总结接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "RELATION_SUMMARIZE_ERROR",
                "message": f"生成总结失败: {str(e)}",
                "data": None
            }
        )

