"""
配置管理
使用 pydantic-settings 从环境变量读取配置
"""

from functools import lru_cache
from typing import Optional
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    """应用配置"""
    
    # 应用基础配置
    APP_NAME: str = "python-ai-hub"
    ENV: str = "dev"  # dev | prod
    
    # 日志配置
    LOG_LEVEL: str = "INFO"
    
    # AI 服务配置
    AI_BASE_URL: Optional[str] = None  # OpenAI 兼容网关地址，为空则使用官方 OpenAI
    AI_API_KEY: Optional[str] = None  # API Key
    AI_MODEL_DEFAULT: str = "gpt-4o-mini"  # 默认模型
    AI_TIMEOUT_SECONDS: int = 30  # 请求超时时间（秒）
    
    # 内部 API 鉴权
    INTERNAL_API_KEY: Optional[str] = None  # 用于 .NET 调用 Python 的内网鉴权 Key
    
    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        case_sensitive=True,
        extra="ignore"
    )


# 单例缓存
_settings: Optional[Settings] = None


@lru_cache()
def get_settings() -> Settings:
    """
    获取配置单例（带缓存）
    
    Returns:
        Settings: 配置实例
    """
    global _settings
    if _settings is None:
        _settings = Settings()
    return _settings

