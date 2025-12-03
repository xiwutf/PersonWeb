"""
大模型客户端统一封装
提供统一的大模型调用接口，支持多种模型提供商
"""

import httpx
import json
from typing import Optional, Dict, Any, AsyncIterator
from app.core.config import settings
from app.core.app_logging import logger


class LLMClient:
    """大模型客户端"""
    
    def __init__(self):
        """初始化客户端"""
        # 根据 LLM_PROVIDER 设置默认模型
        provider = settings.LLM_PROVIDER.lower()
        if provider == "deepseek":
            self.default_model = settings.DEEPSEEK_MODEL_NAME or "deepseek-chat"
        elif provider == "openai":
            self.default_model = settings.DEFAULT_MODEL_NAME or "gpt-3.5-turbo"
        elif provider == "qwen":
            self.default_model = "qwen-turbo"  # 默认通义千问模型
        else:
            self.default_model = settings.DEFAULT_MODEL_NAME or "gpt-3.5-turbo"
        
        logger.info(f"LLMClient 初始化: provider={provider}, default_model={self.default_model}")
    
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
        
        # 检查配置
        if provider == "deepseek":
            if not settings.DEEPSEEK_API_KEY:
                logger.error("❌ DEEPSEEK_API_KEY 未配置！请检查 .env 文件")
                logger.warning("将使用模拟回复")
                return self._chat_mock(prompt, model_name, system_prompt)
            else:
                logger.info(f"✅ DEEPSEEK_API_KEY 已配置 (前10字符: {settings.DEEPSEEK_API_KEY[:10]}...)")
                logger.info(f"✅ DEEPSEEK_BASE_URL: {settings.DEEPSEEK_BASE_URL}")
        
        try:
            if provider == "deepseek":
                logger.info(f"🚀 开始调用 DeepSeek API: base_url={settings.DEEPSEEK_BASE_URL}, model={model_name}")
                result = await self._chat_deepseek(prompt, model_name, system_prompt, temperature, max_tokens)
                logger.info(f"✅ DeepSeek API 调用成功，返回结果长度: {len(result.get('reply', ''))}")
                return result
            elif provider == "openai":
                return await self._chat_openai(prompt, model_name, system_prompt, temperature, max_tokens)
            elif provider == "qwen":
                return await self._chat_qwen(prompt, model_name, system_prompt, temperature, max_tokens)
            else:
                logger.warning(f"未知的模型提供商: {provider}，使用模拟回复")
                return self._chat_mock(prompt, model_name, system_prompt)
        except Exception as e:
            # 详细记录错误信息
            error_msg = str(e)
            error_type = type(e).__name__
            
            print(f"\n{'='*60}")
            print(f"❌ 调用大模型失败！")
            print(f"{'='*60}")
            print(f"错误类型: {error_type}")
            print(f"错误信息: {error_msg}")
            print(f"Provider: {provider}")
            print(f"Model: {model_name}")
            print(f"DEEPSEEK_API_KEY: {'已配置' if settings.DEEPSEEK_API_KEY else '❌ 未配置'}")
            if settings.DEEPSEEK_API_KEY:
                print(f"API Key 前10字符: {settings.DEEPSEEK_API_KEY[:10]}...")
            print(f"{'='*60}\n")
            
            logger.error(f"调用大模型失败: {error_type} - {error_msg}", exc_info=True)
            logger.error(f"错误详情: provider={provider}, model={model_name}")
            logger.error(f"DEEPSEEK_API_KEY 状态: {'已配置' if settings.DEEPSEEK_API_KEY else '未配置'}")
            
            # 失败时返回模拟回复，避免服务中断
            logger.warning("API 调用失败，返回模拟回复")
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
        
        logger.info(f"准备调用 DeepSeek API: url={base_url}/chat/completions, model={model_name}")
        
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
        
        logger.debug(f"DeepSeek API 请求: model={model_name}, messages_count={len(messages)}")
        
        # 调用 API
        try:
            async with httpx.AsyncClient(timeout=60.0) as client:
                logger.info(f"发送请求到: {base_url}/chat/completions")
                logger.debug(f"请求体: model={model_name}, messages_count={len(messages)}, temperature={temperature}")
                
                response = await client.post(
                    f"{base_url}/chat/completions",
                    headers={
                        "Authorization": f"Bearer {settings.DEEPSEEK_API_KEY}",
                        "Content-Type": "application/json"
                    },
                    json=payload
                )
                
                logger.info(f"DeepSeek API 响应状态码: {response.status_code}")
                
                if response.status_code != 200:
                    # 打印完整的错误信息
                    error_text = response.text
                    error_body = ""
                    try:
                        error_json = response.json()
                        error_body = json.dumps(error_json, ensure_ascii=False, indent=2)
                    except:
                        error_body = error_text
                    
                    print(f"\n{'='*60}")
                    print(f"❌ DeepSeek API 调用失败")
                    print(f"{'='*60}")
                    print(f"状态码: {response.status_code}")
                    print(f"响应头: {dict(response.headers)}")
                    print(f"响应体:")
                    print(error_body[:2000])  # 打印前2000字符
                    print(f"{'='*60}\n")
                    
                    logger.error(f"DeepSeek API 调用失败:")
                    logger.error(f"  状态码: {response.status_code}")
                    logger.error(f"  响应头: {dict(response.headers)}")
                    logger.error(f"  响应体: {error_body[:2000]}")
                    
                    raise Exception(f"DeepSeek API 返回错误: {response.status_code} - {error_text[:500]}")
                
                result = response.json()
                logger.info(f"DeepSeek API 调用成功: choices_count={len(result.get('choices', []))}")
                logger.debug(f"DeepSeek API 响应: {json.dumps(result, ensure_ascii=False)[:500]}")
        except httpx.TimeoutException as e:
            error_msg = f"DeepSeek API 调用超时: {str(e)}"
            print(f"\n❌ {error_msg}\n")
            logger.error(error_msg)
            raise
        except httpx.RequestError as e:
            error_msg = f"DeepSeek API 请求错误: {str(e)}"
            print(f"\n❌ {error_msg}\n")
            logger.error(error_msg, exc_info=True)
            raise
        except Exception as e:
            error_msg = f"DeepSeek API 调用异常: {str(e)}"
            print(f"\n❌ {error_msg}\n")
            logger.error(error_msg, exc_info=True)
            raise
        
        # 解析响应
        if "choices" not in result or len(result["choices"]) == 0:
            logger.error(f"DeepSeek API 响应格式错误: {result}")
            raise ValueError("DeepSeek API 响应格式错误：缺少 choices")
        
        reply = result["choices"][0]["message"]["content"]
        usage = result.get("usage", {})
        
        logger.info(f"DeepSeek API 调用成功: reply_length={len(reply)}, tokens={usage.get('total_tokens', 0)}")
        
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
        logger.warning("⚠️ 使用模拟回复 - 这不应该在生产环境中出现！")
        logger.warning(f"提示: 检查 DEEPSEEK_API_KEY 配置和网络连接")
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

