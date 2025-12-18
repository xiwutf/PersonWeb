"""
Prompt 加载器
按 scene + version 读取 prompts 文件内容
"""

import os
from pathlib import Path
from typing import Optional
from app.core.errors import PromptException


class PromptLoader:
    """Prompt 加载器"""
    
    def __init__(self, prompts_dir: Optional[str] = None):
        """
        初始化 Prompt 加载器
        
        Args:
            prompts_dir: prompts 目录路径，如果为 None 则使用默认路径
        """
        if prompts_dir:
            self.prompts_dir = Path(prompts_dir)
        else:
            # 默认路径：app/prompts 目录（相对于当前文件）
            current_file = Path(__file__)
            # 从 app/services/prompt/loader.py 到 app/prompts
            # 向上三级：prompt -> services -> app，然后进入 prompts
            app_dir = current_file.parent.parent.parent
            self.prompts_dir = app_dir / "prompts"
        
        if not self.prompts_dir.exists():
            raise PromptException(f"Prompts 目录不存在: {self.prompts_dir}")
    
    def load(self, scene: str, template_type: str, version: str = "v1") -> str:
        """
        加载 Prompt 文件内容
        
        文件路径格式: prompts/{scene}/{template_type}_{version}.txt
        例如: prompts/website_chat/system_v1.txt
        
        Args:
            scene: 场景名称（如 website_chat）
            template_type: 模板类型（如 system, user）
            version: 版本号（默认 v1）
            
        Returns:
            str: Prompt 文件内容
            
        Raises:
            PromptException: 文件不存在或读取失败
        """
        file_path = self.prompts_dir / scene / f"{template_type}_{version}.txt"
        
        if not file_path.exists():
            raise PromptException(f"Prompt 文件不存在: {file_path}")
        
        try:
            with open(file_path, "r", encoding="utf-8") as f:
                content = f.read().strip()
            return content
        except Exception as e:
            raise PromptException(f"读取 Prompt 文件失败: {file_path}, 错误: {str(e)}")


# 全局单例
_prompt_loader: Optional[PromptLoader] = None


def get_prompt_loader() -> PromptLoader:
    """
    获取 Prompt 加载器单例
    
    Returns:
        PromptLoader: Prompt 加载器实例
    """
    global _prompt_loader
    if _prompt_loader is None:
        _prompt_loader = PromptLoader()
    return _prompt_loader

