"""
情报中心 API 路由
提供情报采集、分析、日报生成等功能
"""
from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel, Field

from app.core.auth import check_internal_token
from app.core.app_logging import logger
from app.models.dto import BaseResponse
from app.services.intelligence import (
    get_enabled_sources,
    run_collect_task,
    run_analyze_task,
    run_generate_report_task
)

router = APIRouter()


# ==================== Pydantic 模型 ====================

class SourceInfo(BaseModel):
    """来源信息"""
    id: int
    source_name: str
    source_type: str
    source_url: str
    category: str
    tags: list
    priority: int
    enabled: bool
    fetch_interval_minutes: int
    remark: str = None
    last_fetch_time: str = None


class TaskTriggerResponse(BaseModel):
    """任务触发响应"""
    task_id: str = Field(..., description="任务ID")
    message: str = Field(default="任务已提交", description="消息")


# ==================== 来源相关 ====================

@router.get("/sources", response_model=BaseResponse)
async def get_sources(token: str = Depends(check_internal_token)):
    """
    获取启用中的来源列表（Python 服务内部使用）
    """
    try:
        sources = get_enabled_sources()
        source_list = [SourceInfo(**s) for s in sources]

        return BaseResponse(
            success=True,
            data=source_list,
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"获取来源列表失败: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "SOURCES_ERROR",
                "message": str(e),
                "data": None
            }
        )


# ==================== 任务相关 ====================

@router.post("/tasks/collect", response_model=BaseResponse)
async def trigger_collect(token: str = Depends(check_internal_token)):
    """
    触发采集任务
    """
    try:
        task_id = run_collect_task()

        return BaseResponse(
            success=True,
            data=TaskTriggerResponse(task_id=task_id, message="采集任务已提交"),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"触发采集任务失败: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "TASK_ERROR",
                "message": str(e),
                "data": None
            }
        )


@router.post("/tasks/analyze", response_model=BaseResponse)
async def trigger_analyze(token: str = Depends(check_internal_token)):
    """
    触发分析任务
    """
    try:
        task_id = run_analyze_task()

        return BaseResponse(
            success=True,
            data=TaskTriggerResponse(task_id=task_id, message="分析任务已提交"),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"触发分析任务失败: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "TASK_ERROR",
                "message": str(e),
                "data": None
            }
        )


@router.post("/tasks/generate-daily-report", response_model=BaseResponse)
async def trigger_generate_report(token: str = Depends(check_internal_token)):
    """
    触发日报生成任务
    """
    try:
        task_id = run_generate_report_task()

        return BaseResponse(
            success=True,
            data=TaskTriggerResponse(task_id=task_id, message="日报生成任务已提交"),
            error_code=None,
            message=""
        )
    except Exception as e:
        logger.error(f"触发日报生成任务失败: {e}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail={
                "success": False,
                "error_code": "TASK_ERROR",
                "message": str(e),
                "data": None
            }
        )
