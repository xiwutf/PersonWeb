"""
配置管理模块
通过环境变量读取配置，使用 Pydantic BaseSettings 管理
"""

from pydantic_settings import BaseSettings
from typing import List


class Settings(BaseSettings):
    """应用配置类"""
    
    # 服务基础信息
    SERVICE_NAME: str = "ai-service"
    VERSION: str = "0.1.0"
    
    # 内部鉴权 Token
    AI_INTERNAL_TOKEN: str = "default-internal-token-change-in-production"
    
    # 大模型配置
    OPENAI_API_KEY: str = ""
    DEFAULT_MODEL_NAME: str = "gpt-3.5-turbo"
    
    # DeepSeek 配置
    DEEPSEEK_API_KEY: str = ""
    DEEPSEEK_BASE_URL: str = "https://api.deepseek.com/v1"
    DEEPSEEK_MODEL_NAME: str = "deepseek-chat"
    
    # 其他模型配置（预留）
    QWEN_API_KEY: str = ""
    QWEN_BASE_URL: str = ""
    
    # 模型提供商选择: openai, deepseek, qwen
    LLM_PROVIDER: str = "deepseek"
    
    # CORS 配置
    ALLOWED_ORIGINS: List[str] = ["*"]
    
    # 日志配置
    LOG_LEVEL: str = "INFO"
    LOG_FILE: str = "logs/ai-service.log"
    
    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"
        case_sensitive = True


# 创建全局配置实例
settings = Settings()

