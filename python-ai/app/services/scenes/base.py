"""
Scene 基类
定义所有 Scene 的统一接口
"""

from abc import ABC, abstractmethod
from typing import Dict, Any, List
from app.services.llm.client import get_llm_client


class BaseScene(ABC):
    """Scene 抽象基类"""
    
    @property
    @abstractmethod
    def scene_name(self) -> str:
        """
        场景名称
        
        Returns:
            str: 场景名称（如 website_chat）
        """
        pass
    
    @abstractmethod
    def build_messages(self, payload: Dict[str, Any]) -> List[Dict[str, str]]:
        """
        构建消息列表
        
        Args:
            payload: 请求负载
            
        Returns:
            List[Dict[str, str]]: 消息列表，格式为 [{"role": "system", "content": "..."}, ...]
        """
        pass
    
    def postprocess(self, text: str) -> str:
        """
        后处理回复文本
        
        Args:
            text: LLM 返回的原始文本
            
        Returns:
            str: 处理后的文本
        """
        # 默认不做处理，子类可以重写
        return text
    
    async def run(self, payload: Dict[str, Any], trace_id: str) -> Dict[str, Any]:
        """
        统一执行流程：build -> llm.chat -> postprocess -> return
        
        Args:
            payload: 请求负载
            trace_id: 追踪 ID
            
        Returns:
            Dict[str, Any]: 执行结果，包含 content 和 usage
        """
        # 1. 构建消息列表
        messages = self.build_messages(payload)
        
        # 2. 调用 LLM
        llm_client = get_llm_client()
        content, usage = await llm_client.chat(
            messages=messages,
            model=payload.get("model"),
            temperature=payload.get("temperature"),
            max_tokens=payload.get("max_tokens")
        )
        
        # 3. 后处理
        processed_content = self.postprocess(content)
        
        # 4. 返回结果
        return {
            "content": processed_content,
            "usage": usage,
        }

