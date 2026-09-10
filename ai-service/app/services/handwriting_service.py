"""手写纸张识别服务。"""

import json
from typing import List

import httpx

from app.core.app_logging import logger
from app.core.config import settings


class HandwritingService:
    """调用视觉模型，按纸面顺序提取完整句子。"""

    @staticmethod
    def _vision_config() -> tuple[str, str, str, str]:
        provider = (settings.VISION_PROVIDER or settings.LLM_PROVIDER).strip().lower()
        if provider == "deepseek":
            return (
                provider,
                settings.DEEPSEEK_API_KEY,
                settings.DEEPSEEK_RESPONSES_BASE_URL,
                settings.DEEPSEEK_VISION_MODEL,
            )
        if provider == "openai":
            return (
                provider,
                settings.OPENAI_API_KEY,
                settings.OPENAI_BASE_URL,
                settings.OPENAI_VISION_MODEL,
            )
        raise ValueError(f"不支持的 VISION_PROVIDER: {provider}")

    async def extract_sentences(self, image_base64: str, content_type: str) -> List[str]:
        provider, api_key, base_url, model = self._vision_config()
        if not api_key:
            raise ValueError(f"{provider.upper()}_API_KEY 未配置")

        payload = {
            "model": model,
            "store": False,
            "input": [{
                "role": "user",
                "content": [
                    {
                        "type": "input_text",
                        "text": (
                            "阅读这张手写纸。只提取作者写下的完整句子，保持原文字词、标点和顺序；"
                            "不要总结、改写、分类，也不要把页码、标题、涂改废字单独列为句子。"
                        ),
                    },
                    {
                        "type": "input_image",
                        "image_url": f"data:{content_type};base64,{image_base64}",
                        "detail": "high",
                    },
                ],
            }],
            "text": {
                "format": {
                    "type": "json_schema",
                    "name": "handwritten_sentences",
                    "schema": {
                        "type": "object",
                        "properties": {
                            "sentences": {"type": "array", "items": {"type": "string"}}
                        },
                        "required": ["sentences"],
                        "additionalProperties": False,
                    },
                }
            },
        }

        async with httpx.AsyncClient(timeout=settings.VISION_TIMEOUT_SECONDS) as client:
            response = await client.post(
                f"{base_url.rstrip('/')}/responses",
                headers={"Authorization": f"Bearer {api_key}"},
                json=payload,
            )

        if response.status_code != 200:
            logger.error(
                "手写识别模型请求失败: provider=%s status=%s",
                provider,
                response.status_code,
            )
            raise RuntimeError("视觉模型请求失败")

        body = response.json()
        output_text = body.get("output_text", "") or next(
            (
                content.get("text", "")
                for item in body.get("output", [])
                for content in item.get("content", [])
                if content.get("type") == "output_text"
            ),
            "",
        )
        if not output_text:
            return []

        sentences = json.loads(output_text).get("sentences", [])
        return list(dict.fromkeys(
            sentence.strip()
            for sentence in sentences
            if isinstance(sentence, str) and sentence.strip()
        ))


handwriting_service = HandwritingService()
