"""
大模型客户端统一封装
提供统一的大模型调用接口，支持多种模型提供商
"""

import httpx
from typing import Optional, Dict, Any, AsyncIterator
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
        provider = settings.LLM_PROVIDER.lower()
        
        logger.info(f"调用大模型: provider={provider}, model={model_name}, prompt 长度: {len(prompt)}")
        
        try:
            if provider == "deepseek":
                return await self._chat_deepseek(prompt, model_name, system_prompt, temperature, max_tokens)
            elif provider == "openai":
                return await self._chat_openai(prompt, model_name, system_prompt, temperature, max_tokens)
            elif provider == "qwen":
                return await self._chat_qwen(prompt, model_name, system_prompt, temperature, max_tokens)
            else:
                logger.warning(f"未知的模型提供商: {provider}，使用模拟回复")
                return self._chat_mock(prompt, model_name, system_prompt)
        except Exception as e:
            logger.error(f"调用大模型失败: {str(e)}", exc_info=True)
            # 失败时返回模拟回复，避免服务中断
            return self._chat_mock(prompt, model_name, system_prompt)
    
    async def _chat_deepseek(
        self,
        prompt: str,
        model: str,
        system_prompt: Optional[str],
        temperature: float,
        max_tokens: Optional[int]
    ) -> Dict[str, Any]:
        """调用 DeepSeek API"""
        if not settings.DEEPSEEK_API_KEY:
            raise ValueError("DEEPSEEK_API_KEY 未配置")
        
        base_url = settings.DEEPSEEK_BASE_URL or "https://api.deepseek.com/v1"
        model_name = model or settings.DEEPSEEK_MODEL_NAME or "deepseek-chat"
        
        # 构建消息列表
        messages = []
        if system_prompt:
            messages.append({"role": "system", "content": system_prompt})
        messages.append({"role": "user", "content": prompt})
        
        # 构建请求体
        payload = {
            "model": model_name,
            "messages": messages,
            "temperature": temperature
        }
        if max_tokens:
            payload["max_tokens"] = max_tokens
        
        # 调用 API
        async with httpx.AsyncClient(timeout=60.0) as client:
            response = await client.post(
                f"{base_url}/chat/completions",
                headers={
                    "Authorization": f"Bearer {settings.DEEPSEEK_API_KEY}",
                    "Content-Type": "application/json"
                },
                json=payload
            )
            response.raise_for_status()
            result = response.json()
        
        # 解析响应
        reply = result["choices"][0]["message"]["content"]
        usage = result.get("usage", {})
        
        return {
            "reply": reply,
            "model": model_name,
            "usage": {
                "prompt_tokens": usage.get("prompt_tokens", 0),
                "completion_tokens": usage.get("completion_tokens", 0),
                "total_tokens": usage.get("total_tokens", 0)
            }
        }
    
    async def _chat_openai(
        self,
        prompt: str,
        model: str,
        system_prompt: Optional[str],
        temperature: float,
        max_tokens: Optional[int]
    ) -> Dict[str, Any]:
        """调用 OpenAI API（预留实现）"""
        # TODO: 实现 OpenAI API 调用
        logger.warning("OpenAI API 调用尚未实现，使用模拟回复")
        return self._chat_mock(prompt, model, system_prompt)
    
    async def _chat_qwen(
        self,
        prompt: str,
        model: str,
        system_prompt: Optional[str],
        temperature: float,
        max_tokens: Optional[int]
    ) -> Dict[str, Any]:
        """调用通义千问 API（预留实现）"""
        # TODO: 实现通义千问 API 调用
        logger.warning("通义千问 API 调用尚未实现，使用模拟回复")
        return self._chat_mock(prompt, model, system_prompt)
    
    def _chat_mock(
        self,
        prompt: str,
        model: str,
        system_prompt: Optional[str]
    ) -> Dict[str, Any]:
        """模拟回复（用于测试或 API 调用失败时）"""
        reply = f"【模拟模型回复】{prompt[:50]}..."
        
        if system_prompt:
            reply = f"[系统提示: {system_prompt[:30]}...] {reply}"
        
        return {
            "reply": reply,
            "model": model,
            "usage": {
                "prompt_tokens": len(prompt) // 4,
                "completion_tokens": len(reply) // 4,
                "total_tokens": (len(prompt) + len(reply)) // 4
            }
        }
    
    async def chat_stream(
        self,
        prompt: str,
        model: Optional[str] = None,
        system_prompt: Optional[str] = None
    ) -> AsyncIterator[str]:
        """
        流式调用大模型（预留）
        
        Args:
            prompt: 用户输入的提示词
            model: 模型名称
            system_prompt: 系统提示词
            
        Yields:
            str: 流式返回的文本片段
        """
        provider = settings.LLM_PROVIDER.lower()
        model_name = model or self.default_model
        
        try:
            if provider == "deepseek":
                async for chunk in self._chat_deepseek_stream(prompt, model_name, system_prompt):
                    yield chunk
            else:
                # 其他提供商或模拟流式回复
                reply = f"【模拟流式回复】{prompt[:50]}..."
                for char in reply:
                    yield char
        except Exception as e:
            logger.error(f"流式调用大模型失败: {str(e)}", exc_info=True)
            yield f"【错误】流式调用失败: {str(e)}"
    
    async def _chat_deepseek_stream(
        self,
        prompt: str,
        model: str,
        system_prompt: Optional[str]
    ) -> AsyncIterator[str]:
        """DeepSeek 流式调用（预留实现）"""
        # TODO: 实现 DeepSeek 流式调用
        logger.warning("DeepSeek 流式调用尚未实现，使用模拟流式回复")
        reply = f"【模拟流式回复】{prompt[:50]}..."
        for char in reply:
            yield char


# 创建全局客户端实例
llm_client = LLMClient()

