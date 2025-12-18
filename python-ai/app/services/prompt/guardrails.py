"""
Prompt 安全护栏
提供文本清洗和验证功能
"""

import re
import json
from typing import Optional
from app.core.errors import PromptException


def ensure_plain_text(s: str) -> str:
    """
    去除异常控制字符（基础清洗）
    
    保留：
    - 普通文本字符
    - 常见标点符号
    - 换行符、制表符
    
    移除：
    - 控制字符（除了 \n, \t, \r）
    - 零宽字符
    
    Args:
        s: 输入字符串
        
    Returns:
        str: 清洗后的字符串
    """
    if not isinstance(s, str):
        return ""
    
    # 移除零宽字符
    s = re.sub(r'[\u200b-\u200f\ufeff]', '', s)
    
    # 移除控制字符（保留 \n, \t, \r）
    s = re.sub(r'[\x00-\x08\x0b\x0c\x0e-\x1f\x7f-\x9f]', '', s)
    
    # 规范化空白字符
    s = re.sub(r'[ \t]+', ' ', s)  # 多个空格/制表符合并为一个空格
    s = re.sub(r'\n{3,}', '\n\n', s)  # 多个换行符合并为两个
    
    return s.strip()


def ensure_json(text: str) -> Optional[dict]:
    """
    确保文本是有效的 JSON（预留功能）
    
    当前实现：仅做基础验证，不强制要求 JSON 格式
    后续 name_tool 等功能会使用此函数
    
    Args:
        text: 输入文本
        
    Returns:
        Optional[dict]: 如果是有效 JSON 则返回字典，否则返回 None
        
    Raises:
        PromptException: JSON 解析失败（如果后续需要强制 JSON）
    """
    # TODO: 实现 JSON 验证和解析
    # 当前仅做基础检查，不强制
    try:
        # 尝试解析 JSON
        data = json.loads(text)
        if isinstance(data, dict):
            return data
        return None
    except (json.JSONDecodeError, TypeError):
        # 不是 JSON 格式，返回 None（不抛出异常）
        return None

