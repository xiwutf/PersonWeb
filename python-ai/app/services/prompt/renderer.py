"""
Prompt 渲染器
使用 Python 原生 format 渲染 Prompt 模板
"""

from typing import Dict, Any
from app.core.errors import PromptException


class PromptRenderer:
    """Prompt 渲染器"""
    
    @staticmethod
    def render(template: str, **kwargs) -> str:
        """
        渲染 Prompt 模板
        
        使用 Python 原生 format 方法，支持缺省字段容错
        
        Args:
            template: Prompt 模板字符串
            kwargs: 模板变量（如 user_input, page 等）
            
        Returns:
            str: 渲染后的 Prompt 内容
            
        Raises:
            PromptException: 渲染失败
        """
        try:
            # 使用 format 方法，如果缺少字段则使用空字符串
            # 注意：这里不捕获 KeyError，让调用方知道缺少哪些字段
            rendered = template.format(**kwargs)
            return rendered
        except KeyError as e:
            # 如果缺少必需字段，抛出异常
            raise PromptException(f"Prompt 模板缺少必需字段: {e}")
        except Exception as e:
            raise PromptException(f"Prompt 渲染失败: {str(e)}")
    
    @staticmethod
    def render_safe(template: str, **kwargs) -> str:
        """
        安全渲染 Prompt 模板（缺省字段使用空字符串）
        
        Args:
            template: Prompt 模板字符串
            kwargs: 模板变量
            
        Returns:
            str: 渲染后的 Prompt 内容
        """
        try:
            # 使用 format_map 和 defaultdict，缺省字段返回空字符串
            from collections import defaultdict
            safe_kwargs = defaultdict(str, kwargs)
            rendered = template.format_map(safe_kwargs)
            return rendered
        except Exception as e:
            raise PromptException(f"Prompt 安全渲染失败: {str(e)}")

