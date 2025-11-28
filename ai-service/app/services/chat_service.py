"""
聊天服务
处理聊天相关的业务逻辑
"""

from typing import Optional, Dict, Any

from app.core.llm_client import llm_client
from app.core.logging import logger


class ChatService:
    """聊天服务类"""
    
    async def chat(
        self,
        user_id: str,
        session_id: Optional[str],
        message: str,
        model: Optional[str] = None,
        meta: Optional[Dict[str, Any]] = None
    ) -> Dict[str, Any]:
        """
        处理聊天请求
        
        Args:
            user_id: 用户 ID
            session_id: 会话 ID（用于上下文管理，当前未实现）
            message: 用户消息
            model: 指定模型
            meta: 额外信息
            
        Returns:
            dict: 包含回复、模型、使用量等信息
        """
        logger.info(f"处理聊天请求: user_id={user_id}, message_length={len(message)}")
        
        # TODO: 实现上下文管理（基于 session_id）
        # 当前暂时忽略 session_id
        
        # 构建系统提示词（可根据 meta 信息调整）
        system_prompt = None
        if meta:
            logger.debug(f"收到 meta 信息: {meta}")
        
        # 调用大模型
        result = await llm_client.chat(
            prompt=message,
            model=model,
            system_prompt=system_prompt
        )
        
        logger.info(f"聊天完成: model={result['model']}, tokens={result['usage']['total_tokens']}")
        
        return {
            "reply": result["reply"],
            "model": result["model"],
            "usage": result["usage"]
        }

