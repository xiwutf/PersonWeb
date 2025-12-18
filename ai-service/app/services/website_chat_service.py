"""
网站聊天服务
处理前台访客的 AI 聊天功能
"""

from typing import List, Optional, Dict, Any

from app.core.llm_client import llm_client
from app.core.app_logging import logger
from app.core.prompt_loader import load_prompt, PromptLoadError
from app.models.dto import WebsiteChatMessage, WebsiteChatResponseData, ChatUsage


class WebsiteChatService:
    """网站聊天服务类"""
    
    async def chat(
        self,
        messages: List[WebsiteChatMessage],
        scene: str = "website-chat",
        extra: Optional[Dict[str, Any]] = None
    ) -> WebsiteChatResponseData:
        """
        处理网站聊天请求
        
        Args:
            messages: 消息列表（包含 system 和 user 消息）
            scene: 场景标识
            extra: 额外信息
            
        Returns:
            WebsiteChatResponseData: 包含 AI 回复和 token 使用量
        """
        logger.info(
            f"处理网站聊天请求: scene={scene}, messages_count={len(messages)}, "
            f"extra={extra}"
        )
        
        # 分离 system prompt 和 user messages
        system_prompt = None
        user_messages = []
        
        for msg in messages:
            if msg.role == "system":
                if system_prompt:
                    # 如果已有 system prompt，追加内容
                    system_prompt += "\n\n" + msg.content
                else:
                    system_prompt = msg.content
            elif msg.role == "user":
                user_messages.append(msg.content)
            elif msg.role == "assistant":
                # 历史 assistant 消息，暂时忽略（如果需要上下文，可以后续实现）
                pass
        
        # 如果 messages 中没有 system prompt，从文件加载默认的
        if not system_prompt:
            try:
                system_prompt = load_prompt("website_chat/system_v1.txt")
                logger.debug(f"从文件加载 website_chat system prompt (scene={scene})")
            except PromptLoadError as e:
                logger.warning(f"加载默认 system prompt 失败，将使用空 system prompt: {e}")
                system_prompt = None
        
        # 合并所有 user 消息为单个 prompt
        # 如果有多个 user 消息，用换行符连接
        user_prompt = "\n".join(user_messages) if user_messages else ""
        
        if not user_prompt:
            raise ValueError("用户消息不能为空")
        
        # 调用大模型
        # 使用合适的参数：temperature=0.7, max_tokens=1000（与原来保持一致）
        result = await llm_client.chat(
            prompt=user_prompt,
            model=None,  # 使用默认模型
            system_prompt=system_prompt,
            temperature=0.7,
            max_tokens=1000
        )
        
        # 构建响应
        usage = None
        if result.get("usage"):
            usage = ChatUsage(
                prompt_tokens=result["usage"].get("prompt_tokens", 0),
                completion_tokens=result["usage"].get("completion_tokens", 0),
                total_tokens=result["usage"].get("total_tokens", 0)
            )
        
        logger.info(
            f"网站聊天完成: content_length={len(result['reply'])}, "
            f"tokens={usage.total_tokens if usage else 0}"
        )
        
        return WebsiteChatResponseData(
            content=result["reply"],
            usage=usage
        )

