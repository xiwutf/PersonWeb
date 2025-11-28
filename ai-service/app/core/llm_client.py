"""
大模型客户端统一封装
提供统一的大模型调用接口，支持多种模型提供商
"""

from typing import Optional, Dict, Any
from app.core.config import settings
from app.core.logging import logger


class LLMClient:
    """大模型客户端"""
    
    def __init__(self):
        """初始化客户端"""
        self.default_model = settings.DEFAULT_MODEL_NAME
    
    async def chat(
        self,
        prompt: str,
        model: Optional[str] = None,
        system_prompt: Optional[str] = None,
        tools: Optional[list] = None,
        temperature: float = 0.7,
        max_tokens: Optional[int] = None
    ) -> Dict[str, Any]:
        """
        调用大模型进行对话
        
        Args:
            prompt: 用户输入的提示词
            model: 模型名称，如果为 None 则使用默认模型
            system_prompt: 系统提示词
            tools: 工具列表（预留）
            temperature: 温度参数
            max_tokens: 最大 token 数
            
        Returns:
            dict: 包含回复文本、模型名称、使用量等信息
        """
        model_name = model or self.default_model
        
        logger.info(f"调用大模型: {model_name}, prompt 长度: {len(prompt)}")
        
        # TODO: 后续替换为真实的大模型调用
        # 当前为模拟实现
        reply = f"【模拟模型回复】{prompt[:50]}..."
        
        if system_prompt:
            reply = f"[系统提示: {system_prompt[:30]}...] {reply}"
        
        return {
            "reply": reply,
            "model": model_name,
            "usage": {
                "prompt_tokens": len(prompt) // 4,  # 粗略估算
                "completion_tokens": len(reply) // 4,
                "total_tokens": (len(prompt) + len(reply)) // 4
            }
        }
    
    async def chat_stream(
        self,
        prompt: str,
        model: Optional[str] = None,
        system_prompt: Optional[str] = None
    ):
        """
        流式调用大模型（预留）
        
        Args:
            prompt: 用户输入的提示词
            model: 模型名称
            system_prompt: 系统提示词
            
        Yields:
            str: 流式返回的文本片段
        """
        # TODO: 实现流式调用
        model_name = model or self.default_model
        reply = f"【模拟流式回复】{prompt[:50]}..."
        
        for char in reply:
            yield char


# 创建全局客户端实例
llm_client = LLMClient()

