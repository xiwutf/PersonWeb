"""
配置管理模块
通过环境变量读取配置，使用 Pydantic BaseSettings 管理
"""
import json
from pathlib import Path
from typing import Annotated, List

from pydantic import field_validator
from pydantic_settings import BaseSettings, NoDecode


AI_SERVICE_ROOT = Path(__file__).resolve().parents[2]


class Settings(BaseSettings):
    """应用配置类"""

    # 服务基础信息
    SERVICE_NAME: str = "ai-service"
    VERSION: str = "0.1.0"

    # 内部鉴权 Token
    AI_INTERNAL_TOKEN: str = "   "

    # 大模型配置
    OPENAI_API_KEY: str = ""
    DEFAULT_MODEL_NAME: str = "gpt-3.5-turbo"
    OPENAI_BASE_URL: str = "https://api.openai.com/v1"
    OPENAI_VISION_MODEL: str = "gpt-4.1-mini"
    VISION_PROVIDER: str = ""
    VISION_TIMEOUT_SECONDS: float = 90.0

    # DeepSeek 配置
    DEEPSEEK_API_KEY: str = ""
    DEEPSEEK_BASE_URL: str = "https://api.deepseek.com/v1"
    DEEPSEEK_MODEL_NAME: str = "deepseek-chat"
    DEEPSEEK_RESPONSES_BASE_URL: str = "https://api.deepseek.com"
    DEEPSEEK_VISION_MODEL: str = "deepseek-v4-flash-vision-exp"

    # 其他模型配置（预留）
    QWEN_API_KEY: str = ""
    QWEN_BASE_URL: str = ""

    # 模型提供商选择: openai, deepseek, qwen
    LLM_PROVIDER: str = "deepseek"

    # CORS 配置
    ALLOWED_ORIGINS: Annotated[List[str], NoDecode] = ["*"]

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

    @field_validator("ALLOWED_ORIGINS", mode="before")
    @classmethod
    def parse_allowed_origins(cls, value):
        if isinstance(value, list):
            return value
        if not isinstance(value, str) or not value.strip():
            return ["*"]
        raw = value.strip()
        if raw.startswith("["):
            return json.loads(raw)
        return [origin.strip() for origin in raw.split(",") if origin.strip()]

    class Config:
        env_file = str(AI_SERVICE_ROOT / ".env")
        env_file_encoding = "utf-8"
        case_sensitive = True
        extra = "ignore"


# 创建全局配置实例
settings = Settings()
