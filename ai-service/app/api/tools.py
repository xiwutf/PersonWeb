"""
AI 工具接口
提供摘要、标题标签、代码解释等工具类功能
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import (
    BaseResponse,
    SummarizeRequest,
    SummarizeResponseData,
    TitleAndTagsRequest,
    TitleAndTagsResponseData,
    CodeExplainRequest,
    CodeExplainResponseData
)
from app.services.tools_service import ToolsService

router = APIRouter()
tools_service = ToolsService()


@router.post("/tools/summarize", response_model=BaseResponse)
async def summarize(
    request: SummarizeRequest,
    token: str = Depends(check_internal_token)
):
    """
    文章摘要接口
    
    对输入文本生成摘要
    
    Args:
        request: 摘要请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 包含摘要的响应
    """
    try:
        logger.info(f"收到摘要请求: text_length={len(request.text)}, max_length={request.max_length}")
        
        result = await tools_service.summarize(
            text=request.text,
            max_length=request.max_length
        )
        
        return BaseResponse(
            success=True,
            data=SummarizeResponseData(summary=result),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"摘要接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "SUMMARIZE_ERROR",
                "message": f"摘要生成失败: {str(e)}",
                "data": None
            }
        )


@router.post("/tools/title-and-tags", response_model=BaseResponse)
async def title_and_tags(
    request: TitleAndTagsRequest,
    token: str = Depends(check_internal_token)
):
    """
    文章标题和标签生成接口
    
    根据文章内容生成标题和标签
    
    Args:
        request: 标题标签请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 包含标题和标签的响应
    """
    try:
        logger.info(f"收到标题标签请求: text_length={len(request.text)}, max_tags={request.max_tags}")
        
        result = await tools_service.generate_title_and_tags(
            text=request.text,
            max_tags=request.max_tags
        )
        
        return BaseResponse(
            success=True,
            data=TitleAndTagsResponseData(
                title=result["title"],
                tags=result["tags"]
            ),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"标题标签接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "TITLE_TAGS_ERROR",
                "message": f"标题标签生成失败: {str(e)}",
                "data": None
            }
        )


@router.post("/tools/code-explain", response_model=BaseResponse)
async def code_explain(
    request: CodeExplainRequest,
    token: str = Depends(check_internal_token)
):
    """
    代码解释接口
    
    用中文解释代码的功能与逻辑
    
    Args:
        request: 代码解释请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 包含代码解释的响应
    """
    try:
        logger.info(f"收到代码解释请求: code_length={len(request.code)}, language={request.language}")
        
        result = await tools_service.explain_code(
            code=request.code,
            language=request.language
        )
        
        return BaseResponse(
            success=True,
            data=CodeExplainResponseData(explanation=result),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"代码解释接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "CODE_EXPLAIN_ERROR",
                "message": f"代码解释失败: {str(e)}",
                "data": None
            }
        )

