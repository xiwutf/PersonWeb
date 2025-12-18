"""
WebsiteChat Scene
前台 AI 聊天场景
"""

from typing import Dict, Any, List
from app.services.scenes.base import BaseScene
from app.services.prompt.loader import get_prompt_loader
from app.services.prompt.renderer import PromptRenderer
from app.services.prompt.guardrails import ensure_plain_text


class WebsiteChatScene(BaseScene):
    """WebsiteChat 场景"""
    
    @property
    def scene_name(self) -> str:
        return "website_chat"
    
    def build_messages(self, payload: Dict[str, Any]) -> List[Dict[str, str]]:
        """
        构建消息列表
        
        支持两种 payload 格式：
        1. 传 messages（前端对话消息数组）
        2. 只传 user_input（单轮）
        
        Args:
            payload: 请求负载
            
        Returns:
            List[Dict[str, str]]: 消息列表
        """
        loader = get_prompt_loader()
        renderer = PromptRenderer()
        
        messages: List[Dict[str, str]] = []
        
        # 情况1: 如果传了 messages，则在 system 前插入 system_v1
        if "messages" in payload and payload["messages"]:
            # 加载 system prompt
            system_prompt = loader.load(self.scene_name, "system", "v1")
            messages.append({"role": "system", "content": system_prompt})
            
            # 添加历史消息
            for msg in payload["messages"]:
                role = msg.get("role", "user")
                content = msg.get("content", "")
                if role in ["system", "user", "assistant"] and content:
                    messages.append({"role": role, "content": content})
        
        # 情况2: 只传 user_input（单轮）
        elif "user_input" in payload:
            # 加载 system 和 user prompt
            system_prompt = loader.load(self.scene_name, "system", "v1")
            user_template = loader.load(self.scene_name, "user", "v1")
            
            # 渲染 user prompt
            extra = payload.get("extra", {})
            user_content = renderer.render_safe(
                user_template,
                user_input=payload["user_input"],
                page=extra.get("page", "")
            )
            
            messages = [
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_content}
            ]
        
        else:
            raise ValueError("payload 必须包含 messages 或 user_input")
        
        return messages
    
    def postprocess(self, text: str) -> str:
        """
        后处理：基础清洗
        
        Args:
            text: LLM 返回的原始文本
            
        Returns:
            str: 清洗后的文本
        """
        return ensure_plain_text(text)

