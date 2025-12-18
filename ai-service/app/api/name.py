"""
智能取名助手接口
提供名字生成功能
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import (
    BaseResponse,
    NameGenerateRequest,
    NameGenerateResponseData
)
from app.services.name_tool_service import NameToolService

router = APIRouter()
name_tool_service = NameToolService()


@router.post("/name/generate", response_model=BaseResponse)
async def generate_names(
    request: NameGenerateRequest,
    token: str = Depends(check_internal_token)
):
    """
    生成名字接口
    
    根据用户输入的条件生成20个名字，每个名字包含评分和理由
    
    Args:
        request: 生成名字请求数据
        token: 内部调用 Token（通过依赖注入验证）
        
    Returns:
        BaseResponse: 包含生成的名字列表和 traceId 的响应
    """
    try:
        logger.info(
            f"收到生成名字请求: type={request.type}, style={request.style}, "
            f"traceId={request.traceId}"
        )
        
        # 验证请求
        if not request.type:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="取名类型不能为空"
            )
        
        if not request.style or len(request.style) == 0:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="至少选择一个风格"
            )
        
        # 调用服务层处理业务逻辑
        result = await name_tool_service.generate_names(request)
        
        # 使用 model_dump 确保 JSON 字段名与前端/后端 DTO 匹配（camelCase）
        return BaseResponse(
            success=True,
            data=result.model_dump(exclude_none=True),
            error_code=None,
            message=""
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"生成名字接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "NAME_GENERATE_ERROR",
                "message": f"生成名字失败: {str(e)}",
                "data": None
            }
        )


@router.post("/name/regenerate", response_model=BaseResponse)
async def regenerate_names(
    request: NameGenerateRequest,
    token: str = Depends(check_internal_token)
):
    """
    再来一批接口（重新生成）
    
    使用相同的 traceId 和条件重新生成名字，保持风格一致
    
    Args:
        request: 生成名字请求数据（必须包含 traceId）
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 包含生成的名字列表和 traceId 的响应
    """
    try:
        logger.info(
            f"收到重新生成名字请求: type={request.type}, traceId={request.traceId}"
        )
        
        # 验证请求
        if not request.type:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="取名类型不能为空"
            )
        
        if not request.style or len(request.style) == 0:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="至少选择一个风格"
            )
        
        if not request.traceId:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="traceId 不能为空"
            )
        
        # 调用服务层处理业务逻辑
        result = await name_tool_service.generate_names(request)
        
        # 使用 model_dump 确保 JSON 字段名与前端/后端 DTO 匹配（camelCase）
        return BaseResponse(
            success=True,
            data=result.model_dump(exclude_none=True),
            error_code=None,
            message=""
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"重新生成名字接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "NAME_REGENERATE_ERROR",
                "message": f"重新生成失败: {str(e)}",
                "data": None
            }
        )

