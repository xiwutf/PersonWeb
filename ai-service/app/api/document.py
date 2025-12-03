"""
文档知识管家 Agent 相关接口
提供文档解析、处理、问答等功能
"""

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import (
    BaseResponse,
    DocumentProcessRequest,
    DocumentProcessResponseData,
    DocumentQueryRequest,
    DocumentQueryResponseData
)
from app.services.document_agent_service import DocumentAgentService
from app.services.vector_store import vector_store

router = APIRouter()
document_agent_service = DocumentAgentService()


@router.post("/document/process", response_model=BaseResponse)
async def process_document(
    request: DocumentProcessRequest,
    token: str = Depends(check_internal_token)
):
    """
    处理文档接口
    
    解析文档、分段、生成摘要、构建知识结构并入库
    
    Args:
        request: 文档处理请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 处理结果
    """
    try:
        logger.info(
            f"收到文档处理请求: document_id={request.document_id}, "
            f"file_type={request.file_type}, file_path={request.file_path[:50]}..."
        )
        
        # 调用 Agent 服务处理文档
        result = await document_agent_service.process_document(
            document_id=request.document_id,
            file_path=request.file_path,
            file_type=request.file_type,
            user_id=request.user_id
        )
        
        return BaseResponse(
            success=True,
            data=result,
            error_code=None,
            message="文档处理成功"
        )
    except Exception as e:
        logger.error(f"文档处理接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "DOCUMENT_PROCESS_ERROR",
                "message": f"文档处理失败: {str(e)}",
                "data": None
            }
        )


@router.post("/document/query", response_model=BaseResponse)
async def query_document(
    request: DocumentQueryRequest,
    token: str = Depends(check_internal_token)
):
    """
    文档问答接口
    
    对单个文档进行问答
    
    Args:
        request: 问答请求数据
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 包含答案和相关文档片段
    """
    try:
        # 使用 print 确保输出到控制台
        print("\n" + "=" * 60)
        print("📝 收到文档问答请求")
        print(f"document_id={request.document_id}")
        print(f"user_id={request.user_id}")
        print(f"query={request.query[:50]}...")
        print(f"top_k={request.top_k}")
        print("=" * 60 + "\n")
        
        logger.info("=" * 60)
        logger.info("📝 收到文档问答请求")
        logger.info(
            f"document_id={request.document_id}, "
            f"user_id={request.user_id}, "
            f"query={request.query[:50]}..., "
            f"top_k={request.top_k}"
        )
        logger.info("=" * 60)
        
        # 调用 Agent 服务进行问答
        logger.info("🚀 开始调用 document_agent_service.query_document")
        result = await document_agent_service.query_document(
            document_id=request.document_id,
            user_id=request.user_id,
            query=request.query,
            top_k=request.top_k
        )
        
        # result 是 DocumentQueryResponseData 对象，直接访问属性
        answer_length = len(result.answer) if result.answer else 0
        chunks_count = len(result.relevant_chunks) if result.relevant_chunks else 0
        
        print(f"✅ 问答完成: answer_length={answer_length}, chunks_count={chunks_count}")
        logger.info(f"✅ 问答完成: answer_length={answer_length}, chunks_count={chunks_count}")
        logger.info("=" * 60)
        
        return BaseResponse(
            success=True,
            data=result,
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"文档问答接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "DOCUMENT_QUERY_ERROR",
                "message": f"文档问答失败: {str(e)}",
                "data": None
            }
        )


@router.delete("/document/{document_id}/vectors", response_model=BaseResponse)
async def delete_document_vectors(
    document_id: str,
    token: str = Depends(check_internal_token)
):
    """
    删除文档的所有向量
    
    用于清理不再需要的文档向量，释放存储空间
    
    Args:
        document_id: 文档 ID
        token: 内部调用 Token
        
    Returns:
        BaseResponse: 删除结果
    """
    try:
        logger.info(f"收到删除文档向量请求: document_id={document_id}")
        
        # 调用 vector_store 删除文档的所有向量
        success = await vector_store.delete_document_vectors(document_id=str(document_id))
        
        return BaseResponse(
            success=success,
            data={"document_id": document_id},
            error_code=None,
            message="文档向量删除成功" if success else "文档向量删除失败"
        )
    except Exception as e:
        logger.error(f"删除文档向量接口异常: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "DELETE_VECTORS_ERROR",
                "message": f"删除文档向量失败: {str(e)}",
                "data": None
            }
        )

