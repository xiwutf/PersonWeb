"""
LLM 客户端
统一的 LLM 调用封装，兼容 OpenAI Chat Completions 风格
"""

import httpx
from typing import Optional, List, Dict, Any, Tuple
from app.core.config import get_settings
from app.core.errors import LLMException, LLMTimeoutException, LLMNetworkException, LLMAPIException
from app.services.llm.retry import retry_on_network_error
from app.services.llm.models import get_model_defaults


class LLMClient:
    """LLM 客户端"""
    
    def __init__(self):
        self.settings = get_settings()
        self.base_url = self._get_base_url()
        self.api_key = self.settings.AI_API_KEY
        self.timeout = self.settings.AI_TIMEOUT_SECONDS
        
        # 创建 httpx 客户端
        self.client = httpx.AsyncClient(
            timeout=self.timeout
        )
    
    def _get_base_url(self) -> str:
        """
        获取 API 基础 URL
        
        Returns:
            str: API 基础 URL（不包含路径）
        """
        if self.settings.AI_BASE_URL:
            # 如果配置了自定义 URL，使用该 URL（不包含路径）
            return self.settings.AI_BASE_URL.rstrip("/")
        else:
            # 默认使用官方 OpenAI
            return "https://api.openai.com"
    
    @retry_on_network_error(max_retries=2, base_delay=1.0)
    async def chat(
        self,
        messages: List[Dict[str, str]],
        model: Optional[str] = None,
        temperature: Optional[float] = None,
        max_tokens: Optional[int] = None
    ) -> Tuple[str, Dict[str, int]]:
        """
        调用 LLM 进行对话
        
        Args:
            messages: 消息列表，格式为 [{"role": "system", "content": "..."}, ...]
            model: 模型名称，如果为 None 则使用默认模型
            temperature: 温度参数，如果为 None 则使用模型默认值
            max_tokens: 最大 token 数，如果为 None 则使用模型默认值
            
        Returns:
            Tuple[str, Dict[str, int]]: (回复内容, token 使用量)
                token 使用量格式: {"promptTokens": int, "completionTokens": int}
                
        Raises:
            LLMException: LLM 调用异常
        """
        # 确定使用的模型
        model_name = model or self.settings.AI_MODEL_DEFAULT
        
        # 获取模型默认参数
        defaults = get_model_defaults(model_name)
        final_temperature = temperature if temperature is not None else defaults["temperature"]
        final_max_tokens = max_tokens if max_tokens is not None else defaults["max_tokens"]
        
        # 构建请求体
        payload = {
            "model": model_name,
            "messages": messages,
            "temperature": final_temperature,
            "max_tokens": final_max_tokens,
        }
        
        # 构建请求头
        headers = {
            "Content-Type": "application/json",
        }
        
        if self.api_key:
            # OpenAI 兼容格式
            headers["Authorization"] = f"Bearer {self.api_key}"
        
        try:
            # 构建完整 URL
            api_url = f"{self.base_url}/v1/chat/completions"
            
            # 发送请求
            response = await self.client.post(
                url=api_url,
                json=payload,
                headers=headers
            )
            
            # 检查响应状态
            response.raise_for_status()
            
            # 解析响应
            result = response.json()
            
            # 提取回复内容
            if "choices" not in result or len(result["choices"]) == 0:
                raise LLMAPIException("LLM 返回格式异常：缺少 choices")
            
            content = result["choices"][0]["message"]["content"]
            
            # 提取 token 使用量
            usage = result.get("usage", {})
            usage_dict = {
                "promptTokens": usage.get("prompt_tokens", 0),
                "completionTokens": usage.get("completion_tokens", 0),
            }
            
            return content, usage_dict
            
        except httpx.TimeoutException as e:
            raise LLMTimeoutException(f"LLM 请求超时: {str(e)}")
        except httpx.HTTPStatusError as e:
            raise LLMNetworkException(f"LLM HTTP 错误 ({e.response.status_code}): {str(e)}")
        except httpx.RequestError as e:
            raise LLMNetworkException(f"LLM 网络错误: {str(e)}")
        except KeyError as e:
            raise LLMAPIException(f"LLM 返回格式异常: {str(e)}")
        except Exception as e:
            raise LLMException(f"LLM 调用失败: {str(e)}")
    
    async def close(self):
        """关闭客户端"""
        await self.client.aclose()


# 全局单例
_llm_client: Optional[LLMClient] = None


def get_llm_client() -> LLMClient:
    """
    获取 LLM 客户端单例
    
    Returns:
        LLMClient: LLM 客户端实例
    """
    global _llm_client
    if _llm_client is None:
        _llm_client = LLMClient()
    return _llm_client

