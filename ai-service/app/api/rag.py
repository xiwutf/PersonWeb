"""
RAG 知识库相关接口
提供文档入库和知识库问答功能
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import (
    BaseResponse,
    UpsertDocRequest,
    UpsertDocResponseData,
    RAGQueryRequest,
    RAGQueryResponseData,
    RelevantDoc
)
from app.services.rag_service import RAGService

router = APIRouter()
rag_service = RAGService()


@router.post("/rag/upsert-doc", response_model=BaseResponse)
async def upsert_doc(
    request: UpsertDocRequest,
    token: str = Depends(check_internal_token)
):
    """
    文档入库/更新接口
    
    将文档存入知识库（当前为 mock 实现）
    
    Args:
        request: 文档入库请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 入库结果
    """
    try:
        logger.info(
            f"收到文档入库请求: doc_id={request.doc_id}, "
            f"user_id={request.user_id}, title={request.title[:50]}..."
        )
        
        # 调用服务层处理
        result = await rag_service.upsert_doc(
            doc_id=request.doc_id,
            user_id=request.user_id,
            title=request.title,
            content=request.content,
            tags=request.tags,
            url=request.url
        )
        
        return BaseResponse(
            success=True,
            data=UpsertDocResponseData(
                doc_id=result["doc_id"],
                success=result["success"]
            ),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"文档入库接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "UPSERT_DOC_ERROR",
                "message": f"文档入库失败: {str(e)}",
                "data": None
            }
        )


@router.post("/rag/query", response_model=BaseResponse)
async def rag_query(
    request: RAGQueryRequest,
    token: str = Depends(check_internal_token)
):
    """
    知识库问答接口
    
    基于知识库进行问答（当前为 mock 实现）
    
    Args:
        request: 问答请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 包含答案和相关文档的响应
    """
    try:
        logger.info(f"收到知识库问答请求: user_id={request.user_id}, query={request.query[:50]}...")
        
        # 调用服务层处理
        result = await rag_service.query(
            user_id=request.user_id,
            query=request.query,
            top_k=request.top_k
        )
        
        return BaseResponse(
            success=True,
            data=RAGQueryResponseData(
                answer=result["answer"],
                relevant_docs=[
                    RelevantDoc(**doc) for doc in result["relevant_docs"]
                ]
            ),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"知识库问答接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "RAG_QUERY_ERROR",
                "message": f"知识库问答失败: {str(e)}",
                "data": None
            }
        )

