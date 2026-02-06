"""
思维记录批注 API
POST /api/ai/thought_comment：接收原文，返回 AI 批注 Markdown
"""

import logging
from pathlib import Path

from fastapi import APIRouter, Depends, HTTPException, status

from app.core.security import verify_internal_key
from app.services.llm.client import get_llm_client
from app.core.errors import LLMException

logger = logging.getLogger(__name__)

router = APIRouter()

# 项目根目录：app/api/v1/thought.py -> app/
_APP_DIR = Path(__file__).resolve().parent.parent.parent
_PROMPT_FILE = _APP_DIR / "prompts" / "thought_comment_v1.txt"


def _load_prompt() -> str:
    """加载批注 prompt 模板"""
    if not _PROMPT_FILE.exists():
        raise FileNotFoundError(f"Prompt 文件不存在: {_PROMPT_FILE}")
    return _PROMPT_FILE.read_text(encoding="utf-8").strip()


@router.post("/thought_comment")
async def thought_comment(
    body: dict,
    internal_key: str = Depends(verify_internal_key),
):
    """
    思维批注接口：根据原文生成指导老师风格的 Markdown 批注。
    请求体: { "content": "..." }
    响应: { "comment_md": "..." }
    """
    content = (body.get("content") or "").strip()
    if not content:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail="content 不能为空",
        )

    try:
        prompt_template = _load_prompt()
        system_content = prompt_template + "\n\n---\n\n" + content
        messages = [
            {"role": "system", "content": system_content},
            {"role": "user", "content": "请对上述思维记录进行批注，直接输出 Markdown 批注内容，不要前言。"},
        ]

        llm = get_llm_client()
        comment_md, _ = await llm.chat(messages)
        if not comment_md or not comment_md.strip():
            comment_md = "（未生成有效批注，请重试）"

        return {"comment_md": comment_md.strip()}
    except FileNotFoundError as e:
        logger.exception("思维批注 Prompt 文件缺失")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="批注服务配置错误：缺少 Prompt 文件",
        )
    except LLMException as e:
        logger.exception("思维批注 LLM 调用失败: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"AI 批注失败: {str(e)}",
        )
    except Exception as e:
        logger.exception("思维批注接口异常: %s", e)
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="服务器内部错误",
        )
