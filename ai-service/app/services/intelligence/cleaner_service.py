"""
内容清洗服务
负责清洗采集到的内容，去除 HTML 标签、广告等
"""
import re
from bs4 import BeautifulSoup
from app.core.app_logging import logger


def clean_text(raw_text: str) -> str:
    """
    清洗文本内容

    Args:
        raw_text: 原始文本

    Returns:
        清洗后的文本
    """
    if not raw_text:
        return ""

    try:
        # 使用 BeautifulSoup 去除 HTML 标签
        soup = BeautifulSoup(raw_text, 'html.parser')
        clean_text = soup.get_text(separator='\n', strip=True)

        # 移除多余的空白行
        clean_text = re.sub(r'\n\s*\n\s*\n+', '\n\n', clean_text)

        # 移除行首行尾空格
        lines = [line.strip() for line in clean_text.split('\n')]
        clean_text = '\n'.join(line for line in lines if line)

        # 移除广告和无关内容（简单规则）
        # 移除 "广告"、"推广" 等关键词附近的段落
        patterns = [
            r'【.*?广告.*?】',
            r'\[.*?广告.*?\]',
            r'（.*?广告.*?）',
            r'<[^>]*>',
        ]
        for pattern in patterns:
            clean_text = re.sub(pattern, '', clean_text, flags=re.IGNORECASE)

        # 移除重复的空格
        clean_text = re.sub(r' +', ' ', clean_text)

        return clean_text.strip()

    except Exception as e:
        logger.warning(f"清洗文本失败: {e}")
        return raw_text


def is_valid_content(text: str, min_length: int = 100) -> bool:
    """
    检查内容是否有效

    Args:
        text: 文本内容
        min_length: 最小长度要求

    Returns:
        是否有效
    """
    if not text:
        return False

    # 检查长度
    if len(text) < min_length:
        return False

    # 检查是否主要为空格或标点
    content_chars = sum(1 for c in text if c.strip())
    if content_chars < min_length / 2:
        return False

    return True
