"""手写纸张识别接口。"""

import base64
import binascii
import json

import httpx
from fastapi import APIRouter, Depends, HTTPException, status
from pydantic import BaseModel, Field

from app.core.app_logging import logger
from app.core.auth import check_internal_token
from app.models.dto import BaseResponse
from app.services.handwriting_service import handwriting_service

router = APIRouter()
ALLOWED_CONTENT_TYPES = {"image/jpeg", "image/png", "image/webp"}
MAX_IMAGE_BYTES = 10 * 1024 * 1024


class HandwritingExtractRequest(BaseModel):
    content_type: str = Field(..., description="图片 MIME 类型")
    image_base64: str = Field(..., min_length=1, description="Base64 编码的图片")


@router.post("/handwriting/extract", response_model=BaseResponse)
async def extract_handwriting(
    request: HandwritingExtractRequest,
    token: str = Depends(check_internal_token),
):
    if request.content_type not in ALLOWED_CONTENT_TYPES:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="不支持的图片格式")

    try:
        image_bytes = base64.b64decode(request.image_base64, validate=True)
    except (binascii.Error, ValueError) as exc:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="图片数据无效") from exc

    if not image_bytes or len(image_bytes) > MAX_IMAGE_BYTES:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail="图片为空或超过 10MB")

    try:
        sentences = await handwriting_service.extract_sentences(
            request.image_base64,
            request.content_type,
        )
        return BaseResponse(success=True, data={"sentences": sentences})
    except ValueError as exc:
        logger.error("手写识别配置错误: %s", str(exc))
        raise HTTPException(status_code=status.HTTP_503_SERVICE_UNAVAILABLE, detail="纸张识别服务尚未配置") from exc
    except httpx.TimeoutException as exc:
        raise HTTPException(status_code=status.HTTP_504_GATEWAY_TIMEOUT, detail="纸张识别超时") from exc
    except (httpx.RequestError, RuntimeError, json.JSONDecodeError) as exc:
        logger.error("手写识别失败: %s", type(exc).__name__)
        raise HTTPException(status_code=status.HTTP_502_BAD_GATEWAY, detail="纸张识别暂时失败") from exc
