"""
Prompt 加载工具
提供从文件加载和渲染 Prompt 的功能
"""

from pathlib import Path
from typing import Dict, Any
from app.core.app_logging import logger


class PromptLoadError(Exception):
    """Prompt 加载异常"""
    pass


class PromptRenderError(Exception):
    """Prompt 渲染异常"""
    pass


def _get_prompts_dir() -> Path:
    """
    获取 prompts 目录路径
    
    基于当前文件路径定位，不依赖运行目录
    
    Returns:
        Path: prompts 目录路径
    """
    # 当前文件：app/core/prompt_loader.py
    # 目标目录：app/prompts/
    current_file = Path(__file__)
    # 从 core/ 回到 app/，然后进入 prompts/
    app_dir = current_file.parent.parent
    prompts_dir = app_dir / "prompts"
    return prompts_dir


def load_prompt(rel_path: str) -> str:
    """
    从 app/prompts/ 下读取文件内容并返回
    
    Args:
        rel_path: 相对路径，例如 "name_tool/system_v1.txt"
        
    Returns:
        str: 文件内容（去除首尾空白）
        
    Raises:
        PromptLoadError: 文件不存在或读取失败
    """
    prompts_dir = _get_prompts_dir()
    file_path = prompts_dir / rel_path
    
    if not file_path.exists():
        raise PromptLoadError(
            f"Prompt 文件不存在: {file_path}，请检查路径是否正确"
        )
    
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            content = f.read().strip()
        return content
    except Exception as e:
        raise PromptLoadError(
            f"读取 Prompt 文件失败: {file_path}，错误: {str(e)}"
        )


def render_prompt(template: str, **kwargs) -> str:
    """
    使用 Python str.format 渲染变量
    
    若缺少变量，抛出友好的异常，包含缺失字段名
    
    Args:
        template: Prompt 模板字符串
        kwargs: 模板变量
        
    Returns:
        str: 渲染后的 Prompt 内容
        
    Raises:
        PromptRenderError: 缺少必需变量时抛出
    """
    try:
        # 安全地转义用户输入中的大括号，避免被误当作占位符
        # 注意：Python 的 str.format 会将 {variable} 形式的字符串当作占位符
        # 如果用户输入的内容包含大括号（如 {summary}），会被误解析
        # 解决方案：转义所有用户输入中的大括号，但保留模板中的占位符
        safe_kwargs = {}
        for key, value in kwargs.items():
            if isinstance(value, str):
                # 转义字符串值中的大括号，避免被误当作占位符
                # 将 { 替换为 {{，} 替换为 }}
                # 这样在 format 时，这些大括号会被还原为单个大括号，而不是被当作占位符
                safe_value = value.replace('{', '{{').replace('}', '}}')
                safe_kwargs[key] = safe_value
            else:
                safe_kwargs[key] = value
        
        # 使用 format 方法渲染
        # 模板中的占位符 {key} 会被替换为 safe_value
        # safe_value 中的 {{ 和 }} 会被还原为 { 和 }
        rendered = template.format(**safe_kwargs)
        
        return rendered
    except KeyError as e:
        # 提取缺失的字段名
        missing_key = str(e).strip("'")
        raise PromptRenderError(
            f"Prompt 模板缺少必需变量: \n  \"{missing_key}\"，请检查模板和传入的参数是否匹配"
        )
    except Exception as e:
        raise PromptRenderError(
            f"Prompt 渲染失败: {str(e)}"
        )


def load_and_render_prompt(rel_path: str, **kwargs) -> str:
    """
    加载并渲染 Prompt（便捷方法）
    
    Args:
        rel_path: 相对路径，例如 "name_tool/user_v1.txt"
        **kwargs: 模板变量
        
    Returns:
        str: 渲染后的 Prompt 内容
        
    Raises:
        PromptLoadError: 文件加载失败
        PromptRenderError: 渲染失败
    """
    template = load_prompt(rel_path)
    if kwargs:
        return render_prompt(template, **kwargs)
    return template

