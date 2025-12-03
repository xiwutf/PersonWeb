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
    DEEPSEEK_API_KEY: str = "your-deepseek-api-key-here"
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
    
    # 向量数据库配置（Chroma）
    CHROMA_DB_PATH: str = "./data/chroma"  # Chroma 数据库存储路径
    CHROMA_COLLECTION_NAME: str = "document_chunks"  # 默认集合名称
    
    # Embedding 配置
    LLM_EMBED_MODEL: str = "text-embedding-ada-002"  # 默认使用 OpenAI embedding 模型
    LLM_EMBED_API_KEY: str = ""  # Embedding API Key（如果与 Chat API Key 不同）
    LLM_EMBED_API_BASE: str = ""  # Embedding API Base URL（如果与 Chat API Base 不同）
    
    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"
        case_sensitive = True


# 创建全局配置实例
settings = Settings()

